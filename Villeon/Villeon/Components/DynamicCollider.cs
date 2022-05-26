using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Villeon.Components
{
    public class DynamicCollider : IComponent
    {
        private Vector2 _position;
        private Vector2 _lastPosition;

        private Vector2 _vertex2;
        private Vector2 _vertex3;
        private Vector2 _vertex4;

        private Vector2 _lastVertex2;
        private Vector2 _lastVertex3;
        private Vector2 _lastVertex4;

        private Vector2[] _polygon = new Vector2[6];

        private float _width;
        private float _height;

        public DynamicCollider(Collider collider)
        {
            _position = collider.Position + Vector2.Zero;
            _lastPosition = collider.Position + Vector2.Zero;

            _width = collider.Width;
            _height = collider.Height;

            _vertex2 = new Vector2(_position.X + _width, _position.Y);
            _vertex3 = new Vector2(_position.X + _width, _position.Y + _height);
            _vertex4 = new Vector2(_position.X, _position.Y + _height);

            _lastVertex2 = new Vector2(_position.X + _width, _position.Y);
            _lastVertex3 = new Vector2(_position.X + _width, _position.Y + _height);
            _lastVertex4 = new Vector2(_position.X, _position.Y + _height);
        }

        public int PolygonSize { get; set; }

        public float Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public float Height
        {
            get { return _height; }
            set { _height = value; }
        }

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

        public Vector2 LastCenter
        {
            get { return new Vector2(_lastPosition.X + (Width / 2), _lastPosition.Y + (Height / 2)); }
        }

        public Vector2[] GetPolygon()
        {
            UpdatePositionVertices();

            // didn't move
            if (_position == _lastPosition)
            {
                PolygonSize = 4;
                _polygon[0] = _position;
                _polygon[1] = _vertex2;
                _polygon[2] = _vertex3;
                _polygon[3] = _vertex4;
                return _polygon;
            }

            // get direction
            Vector2 direction = new (_position.X - _lastPosition.X, _position.Y - _lastPosition.Y);

            UpdateLastPositionVertices();

            CreatePolygon(direction);

            return _polygon;
        }

        private void CreatePolygon(Vector2 direction)
        {
            switch (direction.X)
            {
                // moved straight right
                case > 0 when direction.Y == 0:
                    PolygonSize = 4;
                    _polygon[0] = _lastPosition;
                    _polygon[1] = _vertex2;
                    _polygon[2] = _vertex3;
                    _polygon[3] = _lastVertex4;
                    break;

                // moved straight left
                case < 0 when direction.Y == 0:
                    PolygonSize = 4;
                    _polygon[0] = _position;
                    _polygon[1] = _lastVertex2;
                    _polygon[2] = _lastVertex3;
                    _polygon[3] = _vertex4;
                    break;

                // moved straight up
                case 0 when direction.Y > 0:
                    PolygonSize = 4;
                    _polygon[0] = _lastPosition;
                    _polygon[1] = _lastVertex2;
                    _polygon[2] = _vertex3;
                    _polygon[3] = _vertex4;
                    break;

                // moved straight down
                case 0 when direction.Y < 0:
                    PolygonSize = 4;
                    _polygon[0] = _lastVertex4;
                    _polygon[1] = _lastVertex3;
                    _polygon[2] = _vertex2;
                    _polygon[3] = _position;
                    break;

                // moved up right
                case > 0 when direction.Y > 0:
                    PolygonSize = 6;
                    _polygon[0] = _lastPosition;
                    _polygon[1] = _lastVertex2;
                    _polygon[2] = _vertex2;
                    _polygon[3] = _vertex3;
                    _polygon[4] = _vertex4;
                    _polygon[5] = _lastVertex4;
                    break;

                // moved down right
                case > 0 when direction.Y < 0:
                    PolygonSize = 6;
                    _polygon[0] = _lastPosition;
                    _polygon[1] = _position;
                    _polygon[2] = _vertex2;
                    _polygon[3] = _vertex3;
                    _polygon[4] = _lastVertex3;
                    _polygon[5] = _lastVertex4;
                    break;

                // moved up left
                case < 0 when direction.Y > 0:
                    PolygonSize = 6;
                    _polygon[0] = _position;
                    _polygon[1] = _lastPosition;
                    _polygon[2] = _lastVertex2;
                    _polygon[3] = _lastVertex3;
                    _polygon[4] = _vertex3;
                    _polygon[5] = _vertex4;
                    break;

                // moved down left
                case < 0 when direction.Y < 0:
                    PolygonSize = 6;
                    _polygon[0] = _position;
                    _polygon[1] = _vertex2;
                    _polygon[2] = _lastVertex2;
                    _polygon[3] = _lastVertex3;
                    _polygon[4] = _lastVertex4;
                    _polygon[5] = _vertex4;
                    break;
            }
        }

        private void UpdateLastPositionVertices()
        {
            _lastVertex2.X = _lastPosition.X + _width;
            _lastVertex2.Y = _lastPosition.Y;
            _lastVertex3.X = _lastPosition.X + _width;
            _lastVertex3.Y = _lastPosition.Y + _height;
            _lastVertex4.X = _lastPosition.X;
            _lastVertex4.Y = _lastPosition.Y + _height;
        }

        private void UpdatePositionVertices()
        {
            _vertex2.X = _position.X + _width;
            _vertex2.Y = _position.Y;
            _vertex3.X = _position.X + _width;
            _vertex3.Y = _position.Y + _height;
            _vertex4.X = _position.X;
            _vertex4.Y = _position.Y + _height;
        }
    }
}
