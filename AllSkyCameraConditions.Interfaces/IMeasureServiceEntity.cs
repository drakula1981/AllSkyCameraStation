using AllSkyCameraConditions.Model;

namespace AllSkyCameraConditions.Interfaces;

public interface IMeasureServiceEntity<T> where T : EntityBase {

   Task<T?> ReadMeasure(double? measureContext1 = null, double? measureContext2 = null, double? measureContext3 = null, double? measureContext4 = null, double? measureContext5 = null);
   T ReadMeasure(string? measureContext = null);
   T? ReadMeasure(int? measureContext1 = null, double? measureContext2 = null, bool debugMode = false);

}

