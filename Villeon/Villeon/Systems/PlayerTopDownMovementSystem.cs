using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.Helper;

namespace Villeon.Systems
{
    internal class PlayerTopDownMovementSystem : IUpdateSystem
    {
        public PlayerTopDownMovementSystem(string name)
        {
            Name = name;
            Signature.Add<Collider>();
            Signature.Add<Player>();
        }

        public string Name { get; }

        public List<IEntity> Entities { get; private set; } = new();

        public Signature Signature { get; private set; } = new();

        public void Update(double time)
        {
            float leftRightAxis = KeyHandler.IsPressed(Keys.A) ? -1 : KeyHandler.IsPressed(Keys.D) ? 1 : 0;
            float topDownAxis = KeyHandler.IsPressed(Keys.S) ? -1 : KeyHandler.IsPressed(Keys.W) ? 1 : 0;
            
            foreach (IEntity entity in Entities)
            {
                Collider playerCollider = entity.GetComponent<Collider>();
                playerCollider.Position += new Vector2(leftRightAxis, topDownAxis) * (float)time * Constants.TOPDOWNMOVEMENTSPEED;
            }
        }
    }
}
