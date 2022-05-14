using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;

namespace Villeon.Systems
{
    internal class PlayerTopDownMovementSystem : System, IUpdateSystem
    {
        public PlayerTopDownMovementSystem(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(Collider));
            Signature = Signature.AddToSignature(typeof(Player));
        }

        public void Update(float time)
        {
            float leftRightAxis = KeyHandler.IsPressed(Keys.A) ? -1 : KeyHandler.IsPressed(Keys.D) ? 1 : 0;
            float topDownAxis = KeyHandler.IsPressed(Keys.S) ? -1 : KeyHandler.IsPressed(Keys.W) ? 1 : 0;

            foreach (IEntity entity in Entities)
            {
                Transform playerTransform = entity.GetComponent<Transform>();
                playerTransform.Position += new Vector2(leftRightAxis, topDownAxis) * time * Constants.TOPDOWNMOVEMENTSPEED;
            }
        }
    }
}
