using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Villeon.Components
{
    public class Portal : IComponent
    {
        public Portal(string sceneToTeleportTo, Vector2 currentSceneSpawnpoint)
        {
            SceneToLoad = sceneToTeleportTo;
            PositionToTeleport = currentSceneSpawnpoint;
        }

        public string SceneToLoad { get; set; }

        public Vector2 PositionToTeleport { get; set; }
    }
}
