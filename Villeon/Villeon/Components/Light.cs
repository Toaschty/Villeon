using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Villeon.Components
{
    public class Light : IComponent
    {
        private float _lightHeight = -6;
        private float _lightAmbientIntensity = 2f;
        private float _constant = 1.0f;
        private float _linear = 0.22f;
        private float _expo = 0.20f;
        private Vector3 _color = new Vector3(1f, 1f, 1f);

        public Light(Color4 color)
        {
            _color.X = color.R;
            _color.Y = color.G;
            _color.Z = color.B;
        }

        public Light(Color4 color, float lightHeight, float lightAmbientIntensity)
        {
            _color.X = color.R;
            _color.Y = color.G;
            _color.Z = color.B;
            _lightHeight = lightHeight;
            _lightAmbientIntensity = lightAmbientIntensity;
        }

        public Light(Color4 color, float lightHeight, float lightAmbientIntensity, float constant, float linear, float expo)
        {
            _color.X = color.R;
            _color.Y = color.G;
            _color.Z = color.B;
            _lightHeight = lightHeight;
            _lightAmbientIntensity = lightAmbientIntensity;
            _constant = constant;
            _linear = linear;
            _expo = expo;
        }

        public float LightHeight { get => _lightHeight; set => _lightHeight = value; }

        public float LightAmbientIntensity { get => _lightAmbientIntensity; set => _lightAmbientIntensity = value; }

        public float Constant { get => _constant; set => _constant = value; }

        public float Linear { get => _linear; set => _linear = value; }

        public float Expo { get => _expo; set => _expo = value; }

        public Vector3 Color { get => _color; set => _color = value; }
    }
}
