using Newtonsoft.Json;

namespace AllSkyCameraConditions.Model;
public abstract class EntityBase {
   [JsonProperty("id")] public Guid Id { get; init; }
   [JsonProperty("timeStamp")] public DateTime MeasureDate { get; init; }
}

