﻿using System.Collections.Generic;
using Villeon.Helper;
using Villeon.Systems;

namespace Villeon.ECS
{
    public class SceneLoader
    {
        private static List<Scene> _scenes = new List<Scene>();

        public static void SetActiveScene(string sceneName)
        {
            foreach (Scene scene in _scenes)
            {
                if (scene.Name == sceneName)
                {
                    // Clear all pressed and released keys before loading new scene
                    KeyHandler.ClearKeys();

                    Manager.GetInstance().SetScene(scene);
                    break;
                }
            }
        }

        public static void AddScene(Scene scene)
        {
            _scenes.Add(scene);
        }

        public static List<Scene> GetScenes()
        {
            return _scenes;
        }
    }
}
