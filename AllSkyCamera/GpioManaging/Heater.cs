namespace AllSkyCameraConditionService.GpioManaging;
internal interface IHeater {
   public bool IsHeating { get; }
   public void Heat();
   public void StopHeat();
}
internal class Heater(int pinOut) : GpioManager(pinOut), IHeater {
   public bool IsHeating => Status;
   public void Heat() => High();
   public void StopHeat() => Low();
}
internal class PwmHeater(int pinOut, double dutyCycle) : PwmController(pinOut, 400, dutyCycle), IHeater {
   public void AdjustDutyCycle(double dutyCycle) => SetDutyCycle(dutyCycle);
   public bool IsHeating => DutyCycle > 0;
   public void Heat() => Start();
   public void StopHeat() => Stop();


}
