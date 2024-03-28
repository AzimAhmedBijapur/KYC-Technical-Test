using System.Collections.Generic;
using KYC.Models;

namespace KYC.Repositories
{
    public interface IEntityRepository
    {
        // Retrieves all entities
        IQueryable<Entity> GetEntities();

        // Retrieves a single entity by its ID
        Entity GetEntityById(string id);

        // Adds a new entity
        void AddEntity(Entity entity);

        // Updates an existing entity
        void UpdateEntity(Entity entity);

        // Deletes an entity
        void DeleteEntity(Entity entity);
    }
}
