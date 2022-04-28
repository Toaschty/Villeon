using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.Systems;

namespace Villeon
{
    public class Manager : IUpdate, IRender
    {
        private static Manager? _manager;

        private Scene _currentScene = new Scene("none");

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
            _currentScene.AddEntity(entity);
            return entity;
        }

        public void RegisterSystem(ISystem system)
        {
            _currentScene.AddSystem(system);
        }

        public void Update(float time)
        {
            _currentScene.Update(time);
        }

        public void AddComponent(IEntity entity, IComponent component)
        {
            entity.AddComponent(component);
            _currentScene.EntityComponentAdded(entity);
        }

        public void RemoveComponent<T>(IEntity entity)
            where T : class, IComponent
        {
            entity.RemoveComponent<T>();
            _currentScene.EntityComponentRemoved<T>(entity);
        }

        public void Render()
        {
            _currentScene.Render();
        }

        public bool RemoveEntity(IEntity entity)
        {
            return _currentScene.RemoveEntity(entity);
        }

        public bool UnregisterSystem(ISystem system)
        {
            return _currentScene.RemoveSystem(system);
        }

        public void SetScene(Scene scene)
        {
            _currentScene = scene;
        }

        public HashSet<IEntity> GetEntities()
        {
            return _currentScene.GetEntities();
        }


    }
}
