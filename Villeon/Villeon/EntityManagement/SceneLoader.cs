using System.Collections.Generic;
using Villeon.Components;
using Villeon.Helper;
using Villeon.Systems;

namespace Villeon.EntityManagement
{
    public class SceneLoader
    {
        private static Dictionary<string, Scene> _scenesDictionary = new Dictionary<string, Scene>();

        public static Scene CurrentScene { get; private set; } = new Scene("NO_SCENE");

        public static void SetActiveScene(string sceneName)
        {
            Scene scene = _scenesDictionary[sceneName];
            KeyHandler.ClearKeys();
            StateManager.InMenu = false;
            CurrentScene = scene;
            CurrentScene.StartUp();
        }

        public static void AddToScene(IEntity entity, string sceneName)
        {
            _scenesDictionary[sceneName].AddEntity(entity);
        }

        public static void AddScene(Scene scene)
        {
            _scenesDictionary[scene.Name] = scene;
        }

        public static Scene GetScene(string sceneName)
        {
            return _scenesDictionary[sceneName];
        }
    }
}
