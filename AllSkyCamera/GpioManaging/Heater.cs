namespace AllSkyCameraConditionService.GpioManaging {
   internal class Heater : GpioManager {
      public bool IsHeating => Status;
      public Heater(int pinOut) : base(pinOut, GpioMode.Pwm) { }
      public void Heat(double tempIndex) => High((int)tempIndex*10);
      public void StopHeat() => Low();
   }
}
