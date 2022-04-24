using Villeon.Systems;

namespace Villeon
{
    public class SceneLoader
    {
        public static void LoadScene(Scene scene)
        {
            // Register systems
            foreach (ISystem system in scene.GetSystems())
            {
                Manager.GetInstance().RegisterSystem(system);
            }

            // Register entitys
            foreach (IEntity entity in scene.GetEntities())
            {
                Manager.GetInstance().AddEntity(entity);
            }
        }

        public static void UnloadScene(Scene scene)
        {
            // Register systems
            foreach (ISystem system in scene.GetSystems())
            {
                Manager.GetInstance().UnregisterSystem(system);
            }

            // Register entitys
            foreach (IEntity entity in scene.GetEntities())
            {
                Manager.GetInstance().RemoveEntity(entity);
            }
        }
    }
}
