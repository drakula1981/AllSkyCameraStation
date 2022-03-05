using AllSkyCameraConditionService.Model;
using System.Device.I2c;
/// <summary>
/// Port in .NET6.0 of the TSL2591 Driver developped by Leivo Sepp
/// https://github.com/LeivoSepp/Lesson-LightSensor-TSL2591
/// </summary>
namespace AllSkyCameraConditionService.GpioManaging {
   internal class TSL2591 : IDisposable {
      // Address Constant
      private const int TSL2591_ADDR = 0x29;
      // Commands
      private const int TSL2591_CMD = 0xA0;

      // Registers
      private const int TSL2591_REG_ENABLE = 0x00;
      private const int TSL2591_REG_CONTROL = 0x01;
      private const int TSL2591_REG_ID = 0x12;
      private const int TSL2591_REG_DATA_0 = 0x14;
      private const int TSL2591_REG_DATA_1 = 0x16;

      //private const int TSL2591_STATUS_REG = 0x13;

      /*
       LOW gain: use in bright light to avoid sensor saturation
       MED: use in low light to boost sensitivity 
       HIGH: use in very low light condition
       */
      public const int GAIN_LOW = 0x00;
      public const int GAIN_MED = 0x10;
      public const int GAIN_HIGH = 0x20;
      public const int GAIN_MAX = 0x30;
      /*
       100ms: fast reading but low resolution
       600ms: slow reading but best accuracy
       */
      public const int INT_TIME_100MS = 0x00;
      public const int INT_TIME_200MS = 0x01;
      public const int INT_TIME_300MS = 0x02;
      public const int INT_TIME_400MS = 0x03;
      public const int INT_TIME_500MS = 0x04;
      public const int INT_TIME_600MS = 0x05;
      // Constants for LUX calculation
      private const double LUX_DF = 408.0;
      private const double LUX_COEFB = 1.64;  // CH0 coefficient
      private const double LUX_COEFC = 0.59;  // CH1 coefficient A
      private const double LUX_COEFD = 0.86;  //CH2 coefficient B

      //default values
      private const int gainDefault = GAIN_MED;
      private const int integrationTimeDefault = INT_TIME_200MS;

      private uint IntTimeSet { get; set; } = integrationTimeDefault;
      private uint GainSet { get; set; } = gainDefault;

      // I2C Device
      private I2cDevice? I2C;
      private readonly int I2C_ADDRESS = TSL2591_ADDR;
      public TSL2591() => Initialize();
      public static bool IsInitialized { get; private set; } = false;
      private void Initialize() {
         if (IsInitialized) { return; }
         EnsureInitializedAsync().Wait();
      }
      private async Task EnsureInitializedAsync() {
         if (IsInitialized) { return; }
         try {
            I2cConnectionSettings settings = new(1, I2C_ADDRESS);
            I2C = I2cDevice.Create(settings);

            PowerUp();
            await SetGainAsync();
            IsInitialized = true;
         } catch (Exception ex) {
            throw new Exception("I2C Initialization Failed", ex);
         }
      }
      // Sensor Power up
      private void PowerUp() => Write8(TSL2591_REG_ENABLE, 0x03);

      // Sensor Power down
      private void PowerDown() => Write8(TSL2591_REG_ENABLE, 0x00);

      // Retrieve sensor ID
      public byte GetId() => I2CRead8(TSL2591_REG_ID);

      public async Task SetGainAsync(byte gain = gainDefault, byte int_time = integrationTimeDefault) {
         IntTimeSet = int_time;
         GainSet = gain;
         Write8(TSL2591_REG_CONTROL, (byte)(gain + int_time));
         await Task.Delay(100);
      }

      public uint[] GetData() {
         uint[] Data = new uint[2];

         Data[0] = I2CRead16(TSL2591_REG_DATA_0);
         Data[1] = I2CRead16(TSL2591_REG_DATA_1);

         return Data;
      }
      // Calculate Lux
      private static uint CalibrateIRReadingForTemperature(uint ir, float temp) {
         if (-4242 == temp) return ir;

         float irCalibrationFactor = temp * TemperatureCalibration.irSlope + TemperatureCalibration.irIntercept;
         return (uint)(ir * irCalibrationFactor);
      }

      private static uint CalibrateFullReadingForTemperature(uint full, float temp) {
         if (-4242 == temp) return full;

         float fullCalibrationFactor = temp * TemperatureCalibration.fullLuminositySlope + TemperatureCalibration.fullLuminosityIntercept;
         return (uint)(full * fullCalibrationFactor);
      }
      public async Task<SkyConditions> GetLuxAsync(float temp) {
         uint gain = GainSet;
         uint itime = IntTimeSet;
         uint[] Data = GetData();
         await Task.Delay(100);
         uint CH0 = Data[0];
         uint CH1 = Data[1];

         double d0, d1;

         // Determine if either sensor saturated (0xFFFF)
         if ((CH0 == 0xFFFF) || (CH1 == 0xFFFF)) return new();

         // Convert from unsigned integer to floating point
         d0 = CalibrateFullReadingForTemperature(CH0, temp); 
         d1 = CalibrateIRReadingForTemperature(CH1, temp);
         //d0 = CH0;
         //d1 = CH1;

         int atime = (int)itime + ((int)itime == 0 ? 1 : 0) * 100;
         var again = gain switch {
            0x00 => 1,
            0x10 => 25,
            0x20 => 428,
            0x30 => 9876,
               _ => 1,
         };

         double cpl = (atime * again) / LUX_DF;
         double lux1 = (d0 - (LUX_COEFB * d1)) / cpl;
         double lux2 = ((LUX_COEFC * d0) - (LUX_COEFD * d1)) / cpl;
         double integ = Math.Max(lux1, lux2); ;
         return new(d0-d1, d1, d0, Math.Round(integ, 4), again, atime);
      }
      // Write byte
      private void Write8(byte addr, byte cmd) {
         byte[] Command = new byte[] { (byte)((addr) | TSL2591_CMD), cmd };

         I2C?.Write(Command);
      }
      // Read byte
      private byte I2CRead8(byte addr) {
         byte[] aaddr = new byte[] { (byte)((addr) | TSL2591_CMD) };
         byte[] data = new byte[1];

         I2C?.WriteRead(aaddr, data);

         return data[0];
      }
      // Read integer
      private ushort I2CRead16(byte addr) {
         byte[] aaddr = new byte[] { (byte)((addr) | TSL2591_CMD) };
         byte[] data = new byte[2];

         I2C?.WriteRead(aaddr, data);

         return (ushort)((data[1] << 8) | (data[0]));
      }

      public void Dispose() => Dispose(true);

      public void Dispose(bool disposing) {
         if(!disposing) return;
         PowerDown();
         I2C?.Dispose();
      }
   }
}
