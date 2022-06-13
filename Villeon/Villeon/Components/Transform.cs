using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Villeon.Components
{
    public class Transform : IComponent
    {
        private Vector2 _scale;
        private Vector2 _position;

        public Transform(Vector2 position, float scale, float degrees)
        {
            _position = position;
            _scale.X = scale;
            _scale.Y = scale;
            Degrees = degrees;
        }

        public Transform(Vector2 position, Vector2 scale, float degrees)
        {
            _position = position;
            _scale = scale;
            Degrees = degrees;
        }

        public Transform(Transform copyTransform)
        {
            _position = copyTransform.Position;
            _scale = copyTransform.Scale;
            Degrees = copyTransform.Degrees;
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public float Degrees { get; set; }
    }
}
