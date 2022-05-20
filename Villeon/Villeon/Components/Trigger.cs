using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.ECS;

namespace Villeon.Components
{
    public enum TriggerType
    {
        MOB,
        DAMAGE,
        PORTAL,
    }

    public class Trigger : IComponent
    {
        private float _time = float.PositiveInfinity;

        public Trigger(TriggerType type, Vector2 offset, float width, float height)
        {
            Type = type;
            Offset = offset;
            Width = width;
            Height = height;
        }

        public TriggerType Type { get; private set; }

        public Vector2 Position { get; set; }

        public Vector2 Offset { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public float Time
        {
            get => _time;
            set
            {
                _time = value;
                if (_time < 0)
                    ToBeRemoved = true;
            }
        }

        public bool ToBeRemoved { get; set; } = false;

        public int Damage { get; set; } = 0;

        public Vector2 Impulse { get; set; } = Vector2.Zero;

        public string SceneName { get; set; }
    }
}
