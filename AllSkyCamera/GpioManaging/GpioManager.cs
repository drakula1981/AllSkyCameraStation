using System.Device.Gpio;
using System.Device.Pwm;

namespace AllSkyCameraConditionService.GpioManaging {

   internal abstract class GpioManager : IDisposable {
      private GpioController? Gpio { get; set; }
      private int PinOut { get; set; }
      protected bool Status => Gpio?.Read(PinOut) == PinValue.High;

      protected GpioManager(int pinOut) {
         PinOut = pinOut;
         Gpio = new();
         Gpio.OpenPin(PinOut, PinMode.Output);
      }

      protected void High() => Gpio?.Write(PinOut, PinValue.High);
      protected void Low() => Gpio?.Write(PinOut, PinValue.Low);
      public void Dispose() => Dispose(true);
      public void Dispose(bool disposing) {
         if (!disposing) return;
         Gpio?.Dispose();
      }
   }
}
