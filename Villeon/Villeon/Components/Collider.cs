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
        private Vector2 _lastPosition;
        private Vector2 _offset;

        private Vector2 _vertex2;
        private Vector2 _vertex3;
        private Vector2 _vertex4;

        private Vector2 _lastVertex2;
        private Vector2 _lastVertex3;
        private Vector2 _lastVertex4;

        private Vector2[] _polygon = new Vector2[6];

        public Collider(Vector2 offset, Transform transform, float width, float height)
        {
            Position = transform.Position - offset;
            LastPosition = transform.Position - offset;
            Width = width * transform.Scale.X;
            Height = height * transform.Scale.Y;
            _offset = offset;

            _vertex2 = new Vector2(_position.X + width, _position.Y);
            _vertex3 = new Vector2(_position.X + width, _position.Y + height);
            _vertex4 = new Vector2(_position.X, _position.Y + height);

            _lastVertex2 = new Vector2(_position.X + width, _position.Y);
            _lastVertex3 = new Vector2(_position.X + width, _position.Y + height);
            _lastVertex4 = new Vector2(_position.X, _position.Y + height);
        }

        public Collider(Vector2 offset, Vector2 position, float width, float height)
        {
            Position = position - offset;
            LastPosition = position - offset;
            Width = width;
            Height = height;
            _offset = offset;

            _vertex2 = new Vector2(_position.X + width, _position.Y);
            _vertex3 = new Vector2(_position.X + width, _position.Y + height);
            _vertex4 = new Vector2(_position.X, _position.Y + height);

            _lastVertex2 = new Vector2(_position.X + width, _position.Y);
            _lastVertex3 = new Vector2(_position.X + width, _position.Y + height);
            _lastVertex4 = new Vector2(_position.X, _position.Y + height);
        }

        public Collider(Vector2 position, float width, float height)
        {
            Position = position - Vector2.Zero;
            LastPosition = position - Vector2.Zero;
            Width = width;
            Height = height;
            _offset = Vector2.Zero;

            _vertex2 = new Vector2(_position.X + width, _position.Y);
            _vertex3 = new Vector2(_position.X + width, _position.Y + height);
            _vertex4 = new Vector2(_position.X, _position.Y + height);

            _lastVertex2 = new Vector2(_position.X + width, _position.Y);
            _lastVertex3 = new Vector2(_position.X + width, _position.Y + height);
            _lastVertex4 = new Vector2(_position.X, _position.Y + height);
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
            get
            {
                return _position;
            }

            set
            {
                _position = value;
            }
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

        public int PolygonSize { get; private set; }

        public Vector2[] GetPolygon()
        {
            // update position vertices
            _vertex2.X = _position.X + Width;
            _vertex2.Y = _position.Y;
            _vertex3.X = _position.X + Width;
            _vertex3.Y = _position.Y + Height;
            _vertex4.X = _position.X;
            _vertex4.Y = _position.Y + Height;

            // didn't move
            if (!HasMoved)
            {
                PolygonSize = 4;
                _polygon[0] = _position;
                _polygon[1] = _vertex2;
                _polygon[2] = _vertex3;
                _polygon[3] = _vertex4;
                return _polygon;
            }

            // get direction
            Vector2 direction = new (_position.X - _lastPosition.X, _position.Y - LastPosition.Y);

            // update last position vertices
            _lastVertex2.X = _lastPosition.X + Width;
            _lastVertex2.Y = _lastPosition.Y;
            _lastVertex3.X = _lastPosition.X + Width;
            _lastVertex3.Y = _lastPosition.Y + Height;
            _lastVertex4.X = _lastPosition.X;
            _lastVertex4.Y = _lastPosition.Y + Height;

            // moved straight right
            if (direction.X > 0 && direction.Y == 0)
            {
                PolygonSize = 4;
                _polygon[0] = _lastPosition;
                _polygon[1] = _vertex2;
                _polygon[2] = _vertex3;
                _polygon[3] = _lastVertex4;
            }

            // moved straight left
            else if (direction.X < 0 && direction.Y == 0)
            {
                PolygonSize = 4;
                _polygon[0] = _position;
                _polygon[1] = _lastVertex2;
                _polygon[2] = _lastVertex3;
                _polygon[3] = _vertex4;
            }

            // moved straight up
            else if (direction.X == 0 && direction.Y > 0)
            {
                PolygonSize = 4;
                _polygon[0] = _lastPosition;
                _polygon[1] = _lastVertex2;
                _polygon[2] = _vertex3;
                _polygon[3] = _vertex4;
            }

            // moved straight down
            else if (direction.X == 0 && direction.Y < 0)
            {
                PolygonSize = 4;
                _polygon[0] = _lastVertex4;
                _polygon[1] = _lastVertex3;
                _polygon[2] = _vertex2;
                _polygon[3] = _position;
            }

            // moved up right
            else if (direction.X > 0 && direction.Y > 0)
            {
                PolygonSize = 6;
                _polygon[0] = _lastPosition;
                _polygon[1] = _lastVertex2;
                _polygon[2] = _vertex2;
                _polygon[3] = _vertex3;
                _polygon[4] = _vertex4;
                _polygon[5] = _lastVertex4;
            }

            // moved down right
            else if (direction.X > 0 && direction.Y < 0)
            {
                PolygonSize = 6;
                _polygon[0] = _lastPosition;
                _polygon[1] = _position;
                _polygon[2] = _vertex2;
                _polygon[3] = _vertex3;
                _polygon[4] = _lastVertex3;
                _polygon[5] = _lastVertex4;
            }

            // moved up left
            else if (direction.X < 0 && direction.Y > 0)
            {
                PolygonSize = 6;
                _polygon[0] = _position;
                _polygon[1] = _lastPosition;
                _polygon[2] = _lastVertex2;
                _polygon[3] = _lastVertex3;
                _polygon[4] = _vertex3;
                _polygon[5] = _vertex4;
            }

            // moved down left
            else if (direction.X < 0 && direction.Y < 0)
            {
                PolygonSize = 6;
                _polygon[0] = _position;
                _polygon[1] = _vertex2;
                _polygon[2] = _lastVertex2;
                _polygon[3] = _lastVertex3;
                _polygon[4] = _lastVertex4;
                _polygon[5] = _vertex4;
            }

            return _polygon;
        }
    }
}
