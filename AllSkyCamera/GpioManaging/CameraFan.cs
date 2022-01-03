namespace AllSkyCameraConditionService.GpioManaging {
   internal class CameraFan : GpioManager {
      public bool IsCooling => Status;
      public CameraFan(int pinOut) : base(pinOut, GpioMode.Pwm) { }
      public void StartFan(int dutyCycle) => High(dutyCycle);
      public void StopFan() => Low();
   }
}
