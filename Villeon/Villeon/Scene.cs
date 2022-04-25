using System;
using System.Collections.Generic;
using Villeon.Systems;

namespace Villeon
{
    public class Scene
    {
        private List<ISystem> systems = new List<ISystem>();
        private List<IEntity> entities = new List<IEntity>();

        public TileMap? SceneTileMap { get; set; }

        public void AddSystem(ISystem system) => systems.Add(system);

        public void RemoveSystem(ISystem system) => systems.Remove(system);

        public void AddEntity(IEntity entity) => entities.Add(entity);

        public void RemoveEntity(IEntity entity) => entities.Remove(entity);

        public void SetTileMap(TileMap map) => SceneTileMap = map;

        public List<ISystem> GetSystems() => systems;

        public List<IEntity> GetEntities() => entities;
    }
}
