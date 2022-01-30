﻿using System.Device.Gpio;
using System.Device.Pwm;

namespace AllSkyCameraConditionService.GpioManaging {
   internal enum GpioMode {
      Binary,
      Pwm
   }
   internal abstract class GpioManager : IDisposable {
      private GpioController? Gpio { get; set; }
      private PwmChannel? Pwm { get; set; }
      private int PinOut { get; set; }
      private GpioMode GpioMode { get; set; }
      protected bool Status => GpioMode == GpioMode.Binary ? Gpio?.Read(PinOut) == PinValue.High : Pwm?.DutyCycle > 0;

      protected GpioManager(int pinOut, GpioMode gpioMode) {
         PinOut = pinOut;
         GpioMode = gpioMode;
         if (GpioMode == GpioMode.Binary) {
            Gpio = new();
            Gpio.OpenPin(PinOut, PinMode.Output);
         } else {
            Pwm = PwmChannel.Create(1, pinOut, 400, 0);
         }
      }

      protected void High(int dutyCycle) {
         if (null != Gpio && GpioMode == GpioMode.Binary) {
            Gpio.Write(PinOut, PinValue.High);
            Thread.Sleep(dutyCycle*1000);
            Low();
         } else if(null != Pwm) {
            Pwm.DutyCycle = dutyCycle< 0 ? 0 : dutyCycle > 100 ? 1 : dutyCycle / 100;
            Pwm.Start();
         }
      }

      protected void Low() {
         if (null != Gpio && GpioMode == GpioMode.Binary) Gpio.Write(PinOut, PinValue.Low);
         else if (null != Pwm) Pwm.Stop();
         Thread.Sleep(100);
      }

      public void Dispose() => Dispose(true);

      public void Dispose(bool disposing) {
         if (!disposing) return;
         Gpio?.Dispose();
         Pwm?.Dispose();
      }
   }
}
