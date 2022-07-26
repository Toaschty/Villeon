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
    public class CameraSystem : System, IUpdateSystem
    {
        private Vector2 _position = new Vector2(float.PositiveInfinity);
        private Vector2 _velocity = new Vector2(0.0f);

        public CameraSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Fokus));
        }

        public void Update(float time)
        {
            foreach (var entity in Entities)
            {
                Transform transform = entity.GetComponent<Transform>();
                Fokus fokus = entity.GetComponent<Fokus>();

                if (_position == new Vector2(float.PositiveInfinity))
                {
                    _position = transform.Position + transform.Scale + fokus.Offset;
                }
                else
                {
                    Vector2 idealPoint = transform.Position + transform.Scale + fokus.Offset;
                    if ((idealPoint - _position).Length > 1000)
                    {
                        _position = idealPoint;
                        _velocity = Vector2.Zero;
                    }

                    // calculate Accleration
                    Vector2 acceleration;
                    acceleration.X = fokus.Intensity * (float)Math.Pow(idealPoint.X - _position.X, 2) * ((idealPoint.X - _position.X > 0) ? 1 : -1);
                    acceleration.Y = fokus.Intensity * (float)Math.Pow(idealPoint.Y - _position.Y, 2) * ((idealPoint.Y - _position.Y > 0) ? 1 : -1);

                    Effect effect = entity.GetComponent<Effect>();
                    if (effect is not null)
                    {
                        if (effect.Effects.ContainsKey("CameraShake"))
                        {
                            Random random = new Random();
                            acceleration += new Vector2(random.Next(1000) - 500, random.Next(1000) - 500);
                        }
                    }

                    // calculate Velocity
                    Vector2 oldVelocity = new (_velocity.X, _velocity.Y);
                    _velocity.X = acceleration.X * time;
                    _velocity.Y = acceleration.Y * time;

                    // calculate new posistion
                    _position = _position + (0.5f * (oldVelocity + _velocity) * time);
                }

                Camera.SetTracker(_position);
            }
        }
    }
}
