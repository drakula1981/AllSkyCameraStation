using Pwm = System.Device.Pwm;

namespace AllSkyCameraConditionService.GpioManaging;
internal abstract class PwmController {
   private readonly Pwm.PwmChannel PwmChannel;
   private readonly int Frequency; // Fréquence du signal PWM en Hertz (par exemple, 1000 Hz)
   protected double DutyCycle { get; private set; } // Cycle de travail du signal PWM (entre 0.0 et 1.0)
   public PwmController(int gpioPin, int frequency = 400, double dutyCycle = 0.5) {
      Frequency = frequency;
      DutyCycle = dutyCycle;

      // Initialise le contrôleur PWM
      PwmChannel = new Pwm.Drivers.SoftwarePwmChannel(gpioPin, Frequency, DutyCycle, false, null, true);

      // Configure la fréquence et le cycle de travail
      SetDutyCycle(DutyCycle);
   }

   protected void SetDutyCycle(double newDutyCycle) {
      if (newDutyCycle < 0.0 || newDutyCycle > 1.0) {
         throw new ArgumentOutOfRangeException(nameof(newDutyCycle), "Le cycle de travail doit être compris entre 0.0 et 1.0.");
      }

      DutyCycle = newDutyCycle;
      PwmChannel.DutyCycle = DutyCycle;
   }
   public void Start() => PwmChannel.Start();

   public void Stop() => PwmChannel.Stop();

   public void Dispose() => PwmChannel.Dispose();
}
