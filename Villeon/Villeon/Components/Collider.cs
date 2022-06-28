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
        private Vector2 _offset;
        private Vector2[] _polygon = new Vector2[4];

        public Collider(Vector2 offset, Transform transform, float width, float height)
        {
            Position = transform.Position - offset;
            _offset = offset;
            Width = width;
            Height = height;

            _polygon[1] = new Vector2(_polygon[0].X + width, _polygon[0].Y);
            _polygon[2] = new Vector2(_polygon[0].X + width, _polygon[0].Y + height);
            _polygon[3] = new Vector2(_polygon[0].X, _polygon[0].Y + height);
        }

        public Collider(Vector2 offset, Vector2 position, float width, float height)
        {
            Position = position - offset;
            _offset = offset;
            Width = width;
            Height = height;

            _polygon[1] = new Vector2(_polygon[0].X + width, _polygon[0].Y);
            _polygon[2] = new Vector2(_polygon[0].X + width, _polygon[0].Y + height);
            _polygon[3] = new Vector2(_polygon[0].X, _polygon[0].Y + height);
        }

        public Vector2 Position
        {
            get { return _polygon[0]; }
            set { _polygon[0] = value; }
        }

        public Vector2 Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        public float Width { get; set; }

        public float Height { get; set; }

        public Vector2[] GetPolygon()
        {
            return _polygon;
        }
    }
}