using Villeon.Systems;

namespace Villeon
{
    public class SceneLoader
    {
        public static void LoadScene(Scene scene)
        {
            // Generate tilemap tiles
            if (scene.SceneTileMap != null)
                scene.SceneTileMap.CreateTileMapEntitys();

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
            // Unregister render systems
            foreach (ISystem system in Manager.GetInstance()._renderSystems.ToArray())
            {
                Manager.GetInstance().UnregisterSystem(system);
            }

            // Unregister update systems
            foreach (ISystem system in Manager.GetInstance()._updateSystems.ToArray())
            {
                Manager.GetInstance().UnregisterSystem(system);
            }

            // Register entitys
            foreach (IEntity entity in Manager.GetInstance()._entities.ToArray())
            {
                Manager.GetInstance().RemoveEntity(entity);
            }
        }
    }
}
