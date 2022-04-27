using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Villeon.Systems;

namespace Villeon
{
    public class Scene : IUpdate, IRender
    {
        private HashSet<IEntity> _entities = new ();
        private HashSet<IUpdateSystem> _updateSystems = new ();
        private HashSet<IRenderSystem> _renderSystems = new ();

        public Scene(string name)
        {
            Name = name;
        }

        public TileMap? SceneTileMap { get; set; }

        public string Name { get; }

        public void AddSystem(ISystem system)
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

        public bool RemoveSystem(ISystem system)
        {
            bool removed = false;
            if (system is IUpdateSystem)
                removed = _updateSystems.Remove((IUpdateSystem)system);

            if (system is IRenderSystem)
                removed = _renderSystems.Remove((IRenderSystem)system);

            return removed;
        }

        public void AddEntity(IEntity entity)
        {
            _entities.Add(entity);
            AddToSystems(entity);
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

        public void SetTileMap(TileMap map)
        {
            SceneTileMap = map;
            map.CreateTileMapEntitys();

            foreach (IEntity entity in map.Entities)
            {
                AddEntity(entity);
            }
        }

        public HashSet<IUpdateSystem> GetUpdateSystems() => _updateSystems;

        public HashSet<IRenderSystem> GetRenderSystems() => _renderSystems;

        public HashSet<IEntity> GetEntities() => _entities;

        public void Update(float time)
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
    }

    //class SystemComparer : IEqualityComparer<ISystem>
    //{
    //    public bool Equals(ISystem? x, ISystem? y)
    //    {
    //        if (x.GetType() == y.GetType())
    //            return true;
    //        return false;
    //    }

    //    public int GetHashCode([DisallowNull] ISystem obj)
    //    {
    //        return 1;
    //    }
    //}
}
