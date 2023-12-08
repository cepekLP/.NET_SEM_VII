namespace NET_SEM_VII.Server.db
{
    public interface ISensorService
    {
        Task<List<Entity>> GetAllEntities();

        Task<List<Entity>> GetWithFiltersAndSort(string? type, string? id, DateTime? minDate = null, DateTime? maxDate = null, string sortOrder = "Ascending", string? sortBy = null);
   Task<List<Entity>> GetAllEntitiesByType(string type);
Task<List<Entity>> GetAllEntitiesByID(string id);
            Task<List<Entity>> GetLast100EntitiesByTypeAndID(string type, string id);

        void SaveEntity(Entity entity);

        void SaveEntities(List<Entity> entities);
            }
}
