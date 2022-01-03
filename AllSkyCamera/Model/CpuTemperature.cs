using Newtonsoft.Json;

namespace AllSkyCameraConditionService.Model {
   [JsonObject("CpuTemperature")]
   internal class CpuTemperatureDatas {
      private const double HEATING_TEMP_TRESHOLD = 50;
      private readonly IDictionary<int, int> dutyCyclesByTemperatures;
      [JsonProperty("id")] public Guid Id { get; set; }
      [JsonProperty("timeStamp")] public DateTime MeasureDate { get; private set; }
      [JsonProperty("cpuTemp")] public double? CpuTemp { get; private set; }
      [JsonProperty("gpuTemp")] public double? GpuTemp { get; private set; }
      [JsonProperty("coolingRequired")] public bool CoolingRequired => CpuTemp >= HEATING_TEMP_TRESHOLD || GpuTemp >= HEATING_TEMP_TRESHOLD;
       [JsonProperty("coolingValue")] public int CoolingValue { get {
            if (CpuTemp <= HEATING_TEMP_TRESHOLD) return dutyCyclesByTemperatures[50];
            else if(CpuTemp > 50 && CpuTemp <= 60) return dutyCyclesByTemperatures[60];
            else if(CpuTemp > 60 && CpuTemp <= 65) return dutyCyclesByTemperatures[65];
            else return dutyCyclesByTemperatures[70];
         } 
      }

      public CpuTemperatureDatas() : this(0, 0) { }

      public CpuTemperatureDatas(double cpuTemp, double gpuTemp) {
         Id = Guid.NewGuid();
         MeasureDate = DateTime.UtcNow;
         CpuTemp = cpuTemp;
         GpuTemp = gpuTemp;
         dutyCyclesByTemperatures = new Dictionary<int, int>() {
            {50, 30 },
            {60, 50 },
            {65, 70 },
            {70, 100 }
         };
      }
      public override string ToString() => JsonConvert.SerializeObject(this);
   }
}
