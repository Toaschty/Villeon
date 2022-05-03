using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.Helper;

namespace Villeon.Systems
{
    public class SimpleAISystem : System, IUpdateSystem
    {
        public SimpleAISystem(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(Physics));
            Signature = Signature.AddToSignature(typeof(SimpleAI));
        }

        public void Update(float time)
        {
            HashSet<IEntity> allEntities = Manager.GetInstance().GetEntities();
            IEntity? player = null;
            foreach (IEntity entity in allEntities)
            {
                if (entity.GetComponent<Player>() != null)
                {
                    player = entity;
                }
            }

            if (player != null)
            {
                Transform playerTransform = player.GetComponent<Transform>();

                foreach (IEntity entity in Entities)
                {
                    Transform transform = entity.GetComponent<Transform>();
                    Vector2 playerDirection = transform.Position - playerTransform.Position;

                    float side = -(playerDirection.X / MathF.Abs(playerDirection.X));

                    if (playerDirection.Length < 10 && playerDirection.Length > 1.2f)
                    {
                        Physics physics = entity.GetComponent<Physics>();
                        Collider collider = entity.GetComponent<Collider>();

                        physics.Acceleration += new Vector2(Constants.MOVEMENTSPEED * side * 0.4f, physics.Acceleration.Y);

                        if (side < 0 && collider.HasCollidedLeft && collider.HasCollidedBottom)
                             physics.Velocity = new Vector2(physics.Velocity.X, Constants.JUMPSTRENGTH);
                        else if (side > 0 && collider.HasCollidedRight && collider.HasCollidedBottom)
                            physics.Velocity = new Vector2(physics.Velocity.X, Constants.JUMPSTRENGTH);
                    }
                    else if (playerDirection.Length <= 1.2f)
                    {
                        EntitySpawner.SpawnTrigger((side == 1) ? TriggerID.ATTACKRIGHT : TriggerID.ATTACKLEFT, transform);
                    }
                }
            }
        }
    }
}
