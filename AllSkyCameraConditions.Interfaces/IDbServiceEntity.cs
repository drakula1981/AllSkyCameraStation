using AllSkyCameraConditions.Model;

namespace AllSkyCameraConditions.Interfaces;
public interface IDbServiceEntity<T> where T : EntityBase {
   List<T> GetAll();
   List<T> GetByDate(DateTime date);
   List<T> GetByDates(DateTime dateStart, DateTime dateEnd);
   T? GetById(Guid id);
   void AddMeasure(T? ent);
}
