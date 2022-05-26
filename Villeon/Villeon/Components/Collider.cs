using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.ECS;

namespace Villeon.Components
{
    public class Collider : IComponent
    {
        private Vector2 _position;
        private Vector2 _offset;

        private Vector2 _vertex2;
        private Vector2 _vertex3;
        private Vector2 _vertex4;

        private Vector2[] _polygon = new Vector2[4];

        public Collider(Vector2 offset, Transform transform, float width, float height)
        {
            Position = transform.Position - offset;
            Width = width * transform.Scale.X;
            Height = height * transform.Scale.Y;
            _offset = offset;

            _vertex2 = new Vector2(_position.X + width, _position.Y);
            _vertex3 = new Vector2(_position.X + width, _position.Y + height);
            _vertex4 = new Vector2(_position.X, _position.Y + height);
        }

        public Collider(Vector2 offset, Vector2 position, float width, float height)
        {
            Position = position - offset;
            Width = width;
            Height = height;
            _offset = offset;

            _vertex2 = new Vector2(_position.X + width, _position.Y);
            _vertex3 = new Vector2(_position.X + width, _position.Y + height);
            _vertex4 = new Vector2(_position.X, _position.Y + height);
        }

        public bool HasCollidedTop { get; set; } = false;

        public bool HasCollidedBottom { get; set; } = false;

        public bool HasCollidedLeft { get; set; } = false;

        public bool HasCollidedRight { get; set; } = false;

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

        public float Width { get; set; }

        public float Height { get; set; }

        public void ResetHasCollided()
        {
            HasCollidedLeft = false;
            HasCollidedRight = false;
            HasCollidedTop = false;
            HasCollidedBottom = false;
        }

        public Vector2[] GetPolygon()
        {
            // update position vertices
            _vertex2.X = _position.X + Width;
            _vertex2.Y = _position.Y;
            _vertex3.X = _position.X + Width;
            _vertex3.Y = _position.Y + Height;
            _vertex4.X = _position.X;
            _vertex4.Y = _position.Y + Height;

            // set polygon
            _polygon[0] = _position;
            _polygon[1] = _vertex2;
            _polygon[2] = _vertex3;
            _polygon[3] = _vertex4;
            return _polygon;
        }
    }
}
