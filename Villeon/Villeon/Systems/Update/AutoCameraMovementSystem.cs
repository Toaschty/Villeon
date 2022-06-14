using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;
using Villeon.Render;

namespace Villeon.Systems.Update
{
    public class AutoCameraMovementSystem : System, IUpdateSystem
    {
        public AutoCameraMovementSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(AutoCameraMovement));
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                AutoCameraMovement movingCameraShot = entity.GetComponent<AutoCameraMovement>();

                movingCameraShot.Position = RotatePoint(movingCameraShot.Origin, movingCameraShot.Position, movingCameraShot.Speed);

                Camera.SetTracker(movingCameraShot.Position);
            }
        }

        private Vector2 RotatePoint(Vector2 origin, Vector2 position, float speed)
        {
            double angleInRadians = speed * (Math.PI / 180);
            float cosTheta = (float)Math.Cos(angleInRadians);
            float sinTheta = (float)Math.Sin(angleInRadians);

            Vector2 newPos = new Vector2(0f, 0f);

            newPos.X = (cosTheta * (position.X - origin.X)) - (sinTheta * (position.Y - origin.Y)) + origin.X;
            newPos.Y = (sinTheta * (position.X - origin.X)) + (cosTheta * (position.Y - origin.Y)) + origin.Y;

            return newPos;
        }
    }
}
