using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Villeon.Systems;
using Villeon.Components;

namespace Villeon
{
    public class Manager : IUpdate, IRender
    {
        private static Manager? _manager { get; set; } 

        private Manager()
        {
        }

        public static Manager GetInstance()
        {
            if (_manager == null)
            {
                _manager = new Manager();
            }
            return _manager;
        }

        public IEntity AddEntity(IEntity entity)
        {
            _entities.Add(entity);
            AddToSystems(entity);
            return entity;
        }

        public void RegisterSystem(ISystem system)
        {
            if (system is IUpdateSystem)
                _updateSystems.Add((IUpdateSystem)system);

            if (system is IRenderSystem)
                _renderSystems.Add((IRenderSystem)system);

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
            foreach (ISystem system in _updateSystems)
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

        public void Update(double time)
        {
            foreach (IUpdateSystem system in _updateSystems)
            {
                system.Update(time);
            }
        }

        public void Render()
        {
            foreach (IRenderSystem renderSystem in _renderSystems)
            {
                renderSystem.Render();
            }
        }

        public bool RemoveEntity(IEntity entity)
        {
            bool removed = false;
            removed = _entities.Remove(entity);
            foreach (IUpdateSystem updateSystem in _updateSystems)
            {
                updateSystem.Entities.Remove(entity);
            }

            foreach (IRenderSystem renderSystem in _renderSystems)
            {
                renderSystem.Entities.Remove(entity);
            }
            return removed;
        }

        public bool UnregisterSystem(ISystem system)
        {
            bool removed = false;
            if (system is IUpdateSystem)
               removed = _updateSystems.Remove((IUpdateSystem)system);

            if (system is IRenderSystem)
                removed = _renderSystems.Remove((IRenderSystem)system);

            return removed;
        }

        private List<IEntity> _entities = new();
        private List<IUpdateSystem> _updateSystems = new();
        private List<IRenderSystem> _renderSystems = new();
    }
}
