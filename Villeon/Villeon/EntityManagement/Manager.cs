using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;

namespace Villeon.EntityManagement
{
    public class Manager : IUpdate, IRender
    {
        private static Manager? _manager;

        private Manager()
        {
        }

        public static Manager GetInstance()
        {
            if (_manager == null)
                _manager = new Manager();
            return _manager;
        }

        public IEntity AddEntity(IEntity entity)
        {
            SceneLoader.CurrentScene.AddEntity(entity);
            return entity;
        }

        public void AddEntityToScene(IEntity entity, string sceneName)
        {
            SceneLoader.AddToScene(entity, sceneName);
        }

        public void AddEntities(IEntity[] entities)
        {
            foreach (IEntity entity in entities)
            {
                SceneLoader.CurrentScene.AddEntity(entity);
            }
        }

        public void AddEntities(List<Entity> entities)
        {
            foreach (IEntity entity in entities)
            {
                SceneLoader.CurrentScene.AddEntity(entity);
            }
        }

        public void RegisterSystem(ISystem system)
        {
            SceneLoader.CurrentScene.AddSystem(system);
        }

        public void Update(float time)
        {
            SceneLoader.CurrentScene.Update(time);
        }

        public void AddComponent(IEntity entity, IComponent component)
        {
            entity.AddComponent(component);
            SceneLoader.CurrentScene.EntityComponentAdded(entity);
        }

        public void RemoveComponent<T>(IEntity entity)
            where T : class, IComponent
        {
            entity.RemoveComponent<T>();
            SceneLoader.CurrentScene.EntityComponentRemoved<T>(entity);
        }

        public IEntity? GetEntity(string name)
        {
            foreach (IEntity entity in SceneLoader.CurrentScene.GetEntities())
            {
                if (entity.Name == name)
                    return entity;
            }

            return null;
        }

        public void Render()
        {
            SceneLoader.CurrentScene.Render();
        }

        public bool RemoveEntity(IEntity entity)
        {
            return SceneLoader.CurrentScene.RemoveEntity(entity);
        }

        public void RemoveEntities(IEntity[] entities)
        {
            foreach (IEntity entity in entities)
            {
                SceneLoader.CurrentScene.RemoveEntity(entity);
            }
        }

        public void RemoveEntities(List<IEntity> entities)
        {
            foreach (IEntity entity in entities)
            {
                SceneLoader.CurrentScene.RemoveEntity(entity);
            }
        }

        public bool UnregisterSystem(ISystem system)
        {
            return SceneLoader.CurrentScene.RemoveSystem(system);
        }

        public HashSet<IEntity> GetEntities()
        {
            return SceneLoader.CurrentScene.GetEntities();
        }

        public HashSet<IUpdateSystem> GetUpdateSystems()
        {
            return SceneLoader.CurrentScene.GetUpdateSystems();
        }

        public HashSet<IRenderSystem> GetRenderSystems()
        {
            return SceneLoader.CurrentScene.GetRenderSystems();
        }
    }
}
