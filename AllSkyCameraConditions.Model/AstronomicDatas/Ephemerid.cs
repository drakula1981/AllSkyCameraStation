using AllSkyCameraConditions.Model;
using Newtonsoft.Json;

namespace AllSkyCameraConditionService.Model.AstronomicDatas;
public abstract class ImcceData {
   [JsonProperty("coord")] public string? Coord { get; set; }
   [JsonProperty("hour")] public string? Hour { get; set; }
   public override string ToString() => JsonConvert.SerializeObject(this);
}
public class DawnAstronomical : ImcceData { }

public class DawnCivil : ImcceData { }

public class DawnNautical : ImcceData { }

public class DuskAstronomical : ImcceData { }

public class DuskCivil : ImcceData { }

public class DuskNautical : ImcceData { }

public class Rising : ImcceData { }

public class Setting : ImcceData { }

public class TransitSup : ImcceData { }
public class Root : EntityBase {
   [JsonProperty("name")] public string? Name { get; set; }
   [JsonProperty("date")] public string? Date { get; set; }
   [JsonProperty("dawn-astronomical")] protected List<DawnAstronomical>? DawnAstronomics { get; set; }
   [JsonIgnore] public DawnAstronomical? DawnAstronomic => DawnAstronomics?.FirstOrDefault();
   [JsonProperty("dawn-nautical")] protected List<DawnNautical>? DawnNautics { get; set; }
   [JsonIgnore] public DawnNautical? DawnNautic => DawnNautics?.FirstOrDefault();
   [JsonProperty("dawn-civil")] protected List<DawnCivil>? DawnCivils { get; set; }
   [JsonIgnore] public DawnCivil? DawnCivil => DawnCivils?.FirstOrDefault();
   [JsonProperty("rising")] protected List<Rising>? Risings { get; set; }
   [JsonIgnore] public Rising? Rising => Risings?.FirstOrDefault();
   [JsonProperty("transit-inf")] public object? TransitInfs { get; set; }
   [JsonProperty("transit-sup")] protected List<TransitSup>? TransitSups { get; set; }
   [JsonIgnore] public TransitSup? TransitSup => TransitSups?.FirstOrDefault();
   [JsonProperty("setting")] protected List<Setting>? Settings { get; set; }
   [JsonIgnore] public Setting? Setting => Settings?.FirstOrDefault();
   [JsonProperty("dusk-civil")] protected List<DuskCivil>? DuskCivils { get; set; }
   [JsonIgnore] public DuskCivil? DuskCivil => DuskCivils?.FirstOrDefault();
   [JsonProperty("dusk-nautical")] protected List<DuskNautical>? DuskNauticals { get; set; }
   [JsonIgnore] public DuskNautical? DuskNautical => DuskNauticals?.FirstOrDefault();
   [JsonProperty("dusk-astronomical")] protected List<DuskAstronomical>? DuskAstronomicals { get; set; }
   [JsonIgnore] public DuskAstronomical? DuskAstronomic => DuskAstronomicals?.FirstOrDefault();

   public Root() {
      Id = Guid.NewGuid();
      MeasureDate = DateTime.UtcNow;
   }
   public override string ToString() => JsonConvert.SerializeObject(this);
}
