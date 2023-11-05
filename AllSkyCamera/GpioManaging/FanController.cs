namespace AllSkyCameraConditionService.GpioManaging;
internal class FanController(int pinOut, double dutyCycle) : PwmController(pinOut, 400, dutyCycle) {
   public void AdjustDutyCycle(double dutyCycle) => SetDutyCycle(dutyCycle);
}
