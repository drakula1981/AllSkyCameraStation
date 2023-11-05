using Serilog;
using System.Device.I2c;

namespace AllSkyCameraConditionService.GpioManaging {
   public abstract class I2CDeviceBase {
      protected static I2cConnectionSettings? I2cSettings { get; private set; }
      protected static I2cDevice? I2cDevice { get; private set; }
      private static bool IsInitialized => I2cSettings != null;

      protected static void Initialize(string infoMessageBase, int deviceAddress) {
         if (IsInitialized) return;
         try {
            I2cSettings = new(1, deviceAddress);
            I2cDevice = I2cDevice.Create(I2cSettings);
         } catch (IOException ioex) {
            Log.Logger.Error($"[{infoMessageBase}] Error while intializing sensor : {ioex.Message}");
         } catch (Exception ex) {
            Log.Logger.Error(ex, $"[{infoMessageBase}] Error while intializing sensor");
         }
      }

   }
}
