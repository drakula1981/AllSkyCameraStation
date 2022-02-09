namespace AllSkyCameraConditionService.GpioManaging {
   internal class Heater : GpioManager {
      public bool IsHeating => Status;
      public Heater(int pinOut) : base(pinOut) { }
      public void Heat() => High();
      public void StopHeat() => Low();
   }
}
