using Newtonsoft.Json;

namespace AllSkyCameraConditionService.Model {
   public class DawnAstronomical {
      [JsonProperty("coord")] public string Coord { get; set; }
      [JsonProperty("hour")] public string Hour { get; set; }
      public override string ToString() => JsonConvert.SerializeObject(this);
   }

   public class DawnCivil {
      [JsonProperty("coord")] public string Coord { get; set; }
      [JsonProperty("hour")] public string Hour { get; set; }
      public override string ToString() => JsonConvert.SerializeObject(this);
   }

   public class DawnNautical {
      [JsonProperty("coord")] public string Coord { get; set; }
      [JsonProperty("hour")] public string Hour { get; set; }
      public override string ToString() => JsonConvert.SerializeObject(this);
   }

   public class DuskAstronomical {
      [JsonProperty("coord")] public string Coord { get; set; }
      [JsonProperty("hour")] public string Hour { get; set; }
      public override string ToString() => JsonConvert.SerializeObject(this);
   }

   public class DuskCivil {
      [JsonProperty("coord")] public string Coord { get; set; }
      [JsonProperty("hour")] public string Hour { get; set; }
      public override string ToString() => JsonConvert.SerializeObject(this);
   }

   public class DuskNautical {
      [JsonProperty("coord")] public string Coord { get; set; }
      [JsonProperty("hour")] public string Hour { get; set; }
      public override string ToString() => JsonConvert.SerializeObject(this);
   }

   public class Rising {
      [JsonProperty("coord")] public string Coord { get; set; }
      [JsonProperty("hour")] public string Hour { get; set; }
      public override string ToString() => JsonConvert.SerializeObject(this);
   }

   public class Root {
      [JsonProperty("name")] public string Name { get; set; }
      [JsonProperty("date")] public string Date { get; set; }
      [JsonProperty("dawn-astronomical")] protected List<DawnAstronomical>? DawnAstronomicals { get; set; }
      [JsonIgnore] public DawnAstronomical? DawnAstronomical => DawnAstronomicals?.FirstOrDefault();
      [JsonProperty("dawn-nautical")] protected List<DawnNautical>? DawnNauticals { get; set; }
      [JsonIgnore] public DawnNautical? DawnNautical => DawnNauticals?.FirstOrDefault();
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
      [JsonIgnore] public DuskAstronomical? DuskAstronomical => DuskAstronomicals?.FirstOrDefault();
      public override string ToString() => JsonConvert.SerializeObject(this);
   }

   public class Setting {
      [JsonProperty("coord")] public string Coord { get; set; }
      [JsonProperty("hour")] public string Hour { get; set; }
      public override string ToString() => JsonConvert.SerializeObject(this);
   }

   public class TransitSup {
      [JsonProperty("coord")] public string Coord { get; set; }
      [JsonProperty("hour")] public string Hour { get; set; }
      public override string ToString() => JsonConvert.SerializeObject(this);

   }
}
