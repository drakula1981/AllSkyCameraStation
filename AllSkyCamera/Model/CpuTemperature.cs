using Newtonsoft.Json;

namespace AllSkyCameraConditionService.Model {
   [JsonObject("CpuTemperature")]
   internal class CpuTemperatureDatas {
      private const double HEATING_TEMP_TRESHOLD = 50;
      private IDictionary<int, int> dutyCyclesByTemperatures;
      [JsonProperty("id")] public Guid Id { get; set; }
      [JsonProperty("timeStamp")] public DateTime MeasureDate { get; private set; }
      [JsonProperty("cpuTemp")] public double? CpuTemp { get; private set; }
      [JsonProperty("gpuTemp")] public double? GpuTemp { get; private set; }
      [JsonProperty("heatingRequired")] public bool HeatingRequired => CpuTemp >= HEATING_TEMP_TRESHOLD || GpuTemp >= HEATING_TEMP_TRESHOLD;

      public CpuTemperatureDatas() : this(0, 0) { }

      public CpuTemperatureDatas(double cpuTemp, double gpuTemp) {
         Id = Guid.NewGuid();
         MeasureDate = DateTime.UtcNow;
         CpuTemp = cpuTemp;
         GpuTemp = gpuTemp;
         dutyCyclesByTemperatures = new Dictionary<int, int>() {
            {50, 30 },
            {58, 50 },
            {60, 70 },
            {61, 100 }
         };
      }
      public override string ToString() => JsonConvert.SerializeObject(this);
   }
}
