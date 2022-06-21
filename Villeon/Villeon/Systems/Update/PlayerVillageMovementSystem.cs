using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    internal class PlayerVillageMovementSystem : System, IUpdateSystem
    {
        public PlayerVillageMovementSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Collider), typeof(Player));
        }

        public void Update(float time)
        {
            // Cant move when In Menu or Dialog
            if (StateManager.InMenu || StateManager.InDialog)
                return;

            StateManager.IsGrounded = true;

            foreach (IEntity entity in Entities)
            {
                int leftRightAxis = KeyHandler.IsHeld(Keys.A) ? -1 : KeyHandler.IsHeld(Keys.D) ? 1 : 0;
                int topDownAxis = KeyHandler.IsHeld(Keys.S) ? -1 : KeyHandler.IsHeld(Keys.W) ? 1 : 0;

                Player player = entity.GetComponent<Player>();
                player.IsWalking = true;

                // In case player is not moving
                if (leftRightAxis == 0 && topDownAxis == 0)
                    player.IsWalking = false;

                Transform playerTransform = entity.GetComponent<Transform>();
                playerTransform.Position += new Vector2(leftRightAxis, topDownAxis) * time * Constants.TOPDOWNMOVEMENTSPEED;
            }
        }
    }
}
