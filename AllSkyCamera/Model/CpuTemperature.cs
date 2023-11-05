using Newtonsoft.Json;

namespace AllSkyCameraConditionService.Model;
[JsonObject("CpuTemperature")]
internal class CpuTemperatureDatas {
   public enum FanSpeed {
      Min,
      Low,
      Med,
      High,
      Max
   }

   private const double HEATING_TEMP_TRESHOLD = 30;
   private readonly IDictionary<int, double> dutyCyclesByTemperatures;
   [JsonProperty("id")] public Guid Id { get; set; }
   [JsonProperty("timeStamp")] public DateTime MeasureDate { get; private set; }
   [JsonProperty("cpuTemp")] public double? CpuTemp { get; private set; }
   [JsonProperty("coolingRequired")] public bool CoolingRequired => CpuTemp >= HEATING_TEMP_TRESHOLD;
   [JsonProperty("coolingValue")]
   public double CoolingValue {
      get {
         if (CpuTemp <= HEATING_TEMP_TRESHOLD) return dutyCyclesByTemperatures[(int)FanSpeed.Min];
         else if (CpuTemp > 30 && CpuTemp <= 40) return dutyCyclesByTemperatures[(int)FanSpeed.Low];
         else if (CpuTemp > 40 && CpuTemp <= 50) return dutyCyclesByTemperatures[(int)FanSpeed.Med];
         else if (CpuTemp > 50 && CpuTemp <= 65) return dutyCyclesByTemperatures[(int)FanSpeed.High];
         else return dutyCyclesByTemperatures[(int)FanSpeed.Max];
      }
   }

   public CpuTemperatureDatas() : this(0) { }

   public CpuTemperatureDatas(double cpuTemp) {
      Id = Guid.NewGuid();
      MeasureDate = DateTime.UtcNow;
      CpuTemp = cpuTemp;
      dutyCyclesByTemperatures = new Dictionary<int, double>() {
            {0, 0.25 },
            {1, 0.30 },
            {2, 0.50 },
            {3, 0.75 },
            {4, 1 }
         };
   }
   public override string ToString() => JsonConvert.SerializeObject(this);
}