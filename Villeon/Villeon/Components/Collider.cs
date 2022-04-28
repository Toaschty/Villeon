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
        private Vector2 _lastPosition;
        private Vector2 _offset;

        public Collider(Vector2 offset, Vector2 position, float width, float height)
        {
            Position = position - offset;
            LastPosition = position - offset;
            Width = width;
            Height = height;
            _offset = offset;
        }

        public bool HasMoved { get; set; } = false;

        public bool HasCollidedTop { get; set; } = false;

        public bool HasCollidedBottom { get; set; } = false;

        public bool HasCollidedLeft { get; set; } = false;

        public bool HasCollidedRight { get; set; } = false;

        public Vector2 LastPosition
        {
            get { return _lastPosition; }
            set { _lastPosition = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2 Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        public Vector2 LastCenter
        {
            get { return new Vector2(_lastPosition.X + (Width / 2), _lastPosition.Y + (Height / 2)); }
        }

        public float Width { get; set; }

        public float Height { get; set; }
    }
}
