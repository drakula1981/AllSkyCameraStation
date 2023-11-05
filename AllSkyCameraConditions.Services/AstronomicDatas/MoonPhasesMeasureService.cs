using AllSkyCameraConditions.Interfaces;
using AllSkyCameraConditions.Interfaces.AstronomicDatas;
using AllSkyCameraConditionService.Model.AstronomicDatas;

namespace AllSkyCameraConditions.Services.AstronomicDatas {
   public class MoonPhasesMeasureService : IMoonPhasesMeasureService {
      public Moon.PhaseResult ReadMeasure(string? measureContext = null) => Moon.Now();

      public Moon.PhaseResult? ReadMeasure(int? measureContext1 = null, double? measureContext2 = null, bool debugMode = false) => throw new NotImplementedException();

      Task<Moon.PhaseResult?> IMeasureServiceEntity<Moon.PhaseResult>.ReadMeasure(double? measureContext1, double? measureContext2, double? measureContext3, double? measureContext4, double? measureContext5) => throw new NotImplementedException();
   }
}
