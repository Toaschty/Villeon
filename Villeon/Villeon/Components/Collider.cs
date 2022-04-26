using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Villeon.Components
{
    public class Collider : IComponent
    {
        private Vector2 _position;

        public Collider(Vector2 position, float width, float height)
        {
            Position = position;
            LastPosition = position;
            Width = width;
            Height = height;
            LastCenter = new Vector2(position.X + (width / 2), position.Y + (height / 2));
            Center = new Vector2(position.X + (width / 2), position.Y + (height / 2));
        }

        public bool HasMoved { get; set; } = false;

        public bool HasCollidedTop { get; set; } = false;

        public bool HasCollidedBottom { get; set; } = false;

        public bool HasCollidedLeft { get; set; } = false;

        public bool HasCollidedRight { get; set; } = false;

        public Vector2 LastPosition { get; private set; }

        public Vector2 Position
        {
            get
            {
                return _position;
            }

            set
            {
                if (!HasMoved)
                {
                    HasMoved = true;
                    LastPosition = _position;
                    LastCenter = new Vector2(_position.X + (Width / 2), _position.Y + (Height / 2));
                }

                _position = value;
                Center = new Vector2(_position.X + (Width / 2), _position.Y + (Height / 2));
            }
        }

        public Vector2 LastCenter { get; private set; }

        public Vector2 Center { get; private set; }

        public float Width { get; set; }

        public float Height { get; set; }
    }
}
