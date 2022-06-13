using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Villeon.Components
{
    public class AutoCameraMovement : IComponent
    {
        private Vector2 _currentPosition;
        private Vector2 _origin;
        private float _radius;
        private float _speed;

        public AutoCameraMovement(Vector2 origin, float radius, float speed)
        {
            _origin = origin;
            _radius = radius;
            _speed = speed;

            _currentPosition = origin + new Vector2(radius, 0);
        }

        public Vector2 Origin => _origin;

        public float Radius => _radius;

        public float Speed => _speed;

        public Vector2 Position
        {
            get { return _currentPosition; }
            set { _currentPosition = value; }
        }
    }
}
