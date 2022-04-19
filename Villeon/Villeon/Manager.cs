using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Villeon.Systems;
using Villeon.Components;

namespace Villeon
{
    public class Manager : IUpdate
    {
        public Entity CreateEntity(string name, Signature signature)
        {
            Entity entity = new Entity(name, signature);
            _entities.Add(entity);
            AddToSystems(entity);
            return entity;
        }

        public void RegisterSystem(ISystem system)
        {
            _systems.Add(system);

            // Make sure, every system has its assigned Entities
            foreach (Entity entity in _entities)
            {
                if (entity.Signature.Contains(system.Signature))
                {
                    system.Entities.Add(entity);
                }
            }
        }

        private void AddToSystems(Entity entity)
        {
            foreach (ISystem system in _systems)
            {
                if (entity.Signature.Contains(system.Signature))
                {
                    system.Entities.Add(entity);
                }
            }
        }

        public void Update()
        {
            foreach (ISystem system in _systems)
            {
                system.Update();
            }
        }

        private List<IEntity> _entities = new();
        private List<ISystem> _systems = new();
    }
}
