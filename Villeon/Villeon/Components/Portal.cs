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
        public Portal(string sceneToTeleportTo, Vector2 positionToTeleport)
        {
            SceneToLoad = sceneToTeleportTo;
            PositionToTeleport = positionToTeleport;
        }

        public string SceneToLoad { get; set; }

        public Vector2 PositionToTeleport { get; set; }
    }
}
