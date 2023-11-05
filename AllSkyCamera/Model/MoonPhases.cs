//***********************************************************************
//* Initial sources : https://github.com/khalidabuhakmeh/MoonPhaseConsole
//************************************************************************
using Newtonsoft.Json;

namespace AllSkyCameraConditionService.Model;

internal static class Moon {
   private static readonly IReadOnlyList<string> NorthernHemisphere = new List<string> { "🌑", "🌒", "🌓", "🌔", "🌕", "🌖", "🌗", "🌘", "🌑" };

   private static readonly IReadOnlyList<string> SouthernHemisphere = NorthernHemisphere.Reverse().ToList();

   private static readonly List<string> Names = new() {
             Phase.NewMoon,
             Phase.WaxingCrescent,
             Phase.FirstQuarter,
             Phase.WaxingGibbous,
             Phase.FullMoon,
             Phase.WaningGibbous,
             Phase.ThirdQuarter,
             Phase.WaningCrescent
      };
   private const double TotalLengthOfCycle = 29.53;

   private static readonly List<Phase> allPhases = new();
   static Moon() {
      var period = 30 / Names.Count;
      // divide the phases into equal parts 
      // making sure there are no gaps
      allPhases.AddRange(Names.Select((t, i) => new Phase(t, period * i, i == Names.Count - 1 ? TotalLengthOfCycle : period * (i + 1))));
   }
   /// <summary>
   /// Calculate the current phase of the moon.
   /// Note: this calculation uses the last recorded new moon to calculate the cycles of
   /// of the moon since then. Any date in the past before 1920 might not work.
   /// </summary>
   /// <param name="utcDateTime"></param>
   /// <remarks>https://www.subsystems.us/uploads/9/8/9/4/98948044/moonphase.pdf</remarks>
   /// <returns></returns>
   private static PhaseResult Calculate(DateTime utcDateTime,
       Earth.Hemispheres viewFromEarth = Earth.Hemispheres.Northern) {
      const double julianConstant = 2415018.5;
      var julianDate = utcDateTime.ToOADate() - julianConstant;

      // Paris New Moon (2022 09)
      // https://www.timeanddate.com/moon/phases/france/paris
      var daysSinceLastNewMoon = new DateTime(2022, 8, 27, 15, 20, 00, DateTimeKind.Utc).ToOADate() - julianConstant;

      var newMoons = (julianDate - daysSinceLastNewMoon) / TotalLengthOfCycle;
      var intoCycle = (newMoons - Math.Truncate(newMoons)) * TotalLengthOfCycle;

      var phase = allPhases.First(p => intoCycle >= p.Start && intoCycle <= p.End);

      var index = allPhases.IndexOf(phase);
      var currentPhase = viewFromEarth switch {
         Earth.Hemispheres.Northern => NorthernHemisphere[index],
         _ => SouthernHemisphere[index]
      };

      return new PhaseResult(
          phase.Name,
          currentPhase,
          intoCycle,
          viewFromEarth,
          utcDateTime
      );
   }

   public static PhaseResult UtcNow(Earth.Hemispheres viewFromEarth = Earth.Hemispheres.Northern) => Calculate(DateTime.UtcNow, viewFromEarth);

   public static PhaseResult Now(Earth.Hemispheres viewFromEarth = Earth.Hemispheres.Northern) => Calculate(DateTime.Now.ToUniversalTime(), viewFromEarth);

   [JsonObject("MoonPhase")]
   public class PhaseResult {
      public PhaseResult(string name, string emoji, double daysIntoCycle, Earth.Hemispheres hemisphere, DateTime moment) {
         Name = name;
         Emoji = emoji;
         DaysIntoCycle = daysIntoCycle;
         Hemisphere = hemisphere;
         Moment = moment;
      }

      [JsonProperty("name")] public string Name { get; }
      [JsonProperty("logo")] public string Emoji { get; set; }
      [JsonIgnore] private double DaysIntoCycle { get; set; }
      [JsonProperty("age")]
      public string MoonAge {
         get {
            var moonAge = DateTime.Now.AddDays(DaysIntoCycle) - DateTime.Now;
            return $"{moonAge.Days}j {moonAge.Hours}h {moonAge.Minutes}m";
         }
      }
      [JsonIgnore] public Earth.Hemispheres Hemisphere { get; set; }
      [JsonProperty("timeStamp")] public DateTime Moment { get; }
      [JsonProperty("visibility")]
      public double Visibility {
         get {
            const int FullMoon = 15;
            const double halfCycle = TotalLengthOfCycle / 2;

            var numerator = DaysIntoCycle > FullMoon
                // past the full moon, we want to count down
                ? halfCycle - (DaysIntoCycle % halfCycle)
                // leading up to the full moon
                : DaysIntoCycle + 1.485;
            var vis = Math.Round(numerator / halfCycle * 100, 2);
            return DaysIntoCycle <= 2 ? 0 : vis > 100 ? 100 : vis;
         }
      }
      public override string ToString() => JsonConvert.SerializeObject(this);
   }

   public class Phase {
      public const string NewMoon = "Nouvelle lune";
      public const string WaxingCrescent = "Croissant montant";
      public const string FirstQuarter = "Premier quartier";
      public const string WaxingGibbous = "Gibbeuse montante";
      public const string FullMoon = "Pleine lune";
      public const string WaningGibbous = "Gibbeuse descendante";
      public const string ThirdQuarter = "Dernier quartier";
      public const string WaningCrescent = "Croissant descendant";

      public Phase(string name, double start, double end) {
         Name = name;
         Start = start;
         End = end;
      }

      public string Name { get; }

      /// <summary>
      /// The days into the cycle this phase starts
      /// </summary>
      public double Start { get; }

      /// <summary>
      /// The days into the cycle this phase ends
      /// </summary>
      public double End { get; }
   }
   public static class Earth {
      public enum Hemispheres {
         Northern,
         Southern
      }
   }
}