using System.Collections.Generic;
using Villeon.Components;
using Villeon.GUI;
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

            // Reset pressed keys & menu states
            KeyHandler.ClearKeys();
            StateManager.ResetMenu();
            StateManager.SetSceneFlag(sceneName);
            GUIHandler.GetInstance().ClearMenu();

            // Unload hotbar items -> Prevent reference loss
            Hotbar.GetInstance().UnloadHotbar();

            CurrentScene = scene;
            CurrentScene.StartUp();

            // Reload hotbar entities
            Hotbar.GetInstance().UpdateItems();
            Hotbar.GetInstance().ReloadHotbar();
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
