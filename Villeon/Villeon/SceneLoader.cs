using System.Collections.Generic;
using Villeon.Systems;

namespace Villeon
{
    public class SceneLoader
    {
        private static List<Scene> _scenes = new List<Scene>();

        public static void LoadScene(string sceneName)
        {
            foreach (Scene scene in _scenes)
            {
                if (scene.Name == sceneName)
                {
                    Manager.GetInstance().SetScene(scene);
                    break;
                }
            }
        }

        public static void AddScene(Scene scene)
        {
            _scenes.Add(scene);
        }
    }
}
