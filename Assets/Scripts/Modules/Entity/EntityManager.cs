using System.Collections.Generic;
using Steven.Framework;

namespace TTGJ.Entity
{
    public class EntityManager : Singleton<EntityManager>
    {

        private Dictionary<uint, IEntity> entities = new Dictionary<uint, IEntity>();
        private Dictionary<EntityType, List<IEntity>> entitiesByType = new Dictionary<EntityType, List<IEntity>>();
        public uint nextEntityId = 1;


        public void AddEntity(IEntity entity) { 
            entities[entity.EntityId] = entity;
            if (!entitiesByType.ContainsKey(entity.EntityType)) { 
                entitiesByType[entity.EntityType] = new List<IEntity>();
            }
            entitiesByType[entity.EntityType].Add(entity);
        }

        public void RemoveEntity(uint entityId) { 
            var entity = GetEntity(entityId);
            if (entity == null) { 
                return;
            }
            entities.Remove(entityId);
            entitiesByType[entity.EntityType].Remove(entity);
            
        }
        public IEntity GetEntity(uint entityId) { 
            return entities[entityId];
        }
        public List<IEntity> GetEntitiesByType(EntityType entityType) { 
            return entitiesByType[entityType];
        }
        public uint GetNextEntityId() { 
            return nextEntityId++;
        }
    }
}