using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Villeon.Systems;
using Villeon.Components;

namespace Villeon
{
    public class Manager : IManager
    {
        public IEntity CreateEntity(string name, Signature signature)
        {
            IEntity entity = new Entity(name, signature);
            _entities.Add(entity);
            AddToSystems(entity);
            return entity;
        }

        public void RegisterSystem(ISystem system)
        {
            if (system is IUpdateSystem)
            {
                _systems.Add((IUpdateSystem)system);
            }

            if (system is IRenderSystem)
            {
                _renderSystems.Add((IRenderSystem)system);
            }

            // Make sure, every system has its assigned Entities
            foreach (IEntity entity in _entities)
            {
                if (entity.Signature.Contains(system.Signature))
                {
                    system.Entities.Add(entity);
                }
            }
        }

        private void AddToSystems(IEntity entity)
        {
            foreach (ISystem system in _systems)
            {
                if (entity.Signature.Contains(system.Signature))
                {
                    system.Entities.Add(entity);
                }
            }

            foreach (IRenderSystem renderSystem in _renderSystems)
            {
                if (entity.Signature.Contains(renderSystem.Signature))
                {
                    renderSystem.Entities.Add(entity);
                }
            }
        }

        public void Update()
        {
            foreach (IUpdateSystem system in _systems)
            {
                system.Update();
            }
        }

        public void Render()
        {
            foreach (IRenderSystem renderSystem in _renderSystems)
            {
                renderSystem.Render();
            }
        }

        public void RemoveEntity(IEntity entity)
        {
            throw new NotImplementedException();
        }

        public void UnregisterSystem(ISystem system)
        {
            throw new NotImplementedException();
        }

        private List<IEntity> _entities = new();
        private List<IUpdateSystem> _systems = new();
        private List<IRenderSystem> _renderSystems = new();
    }
}
