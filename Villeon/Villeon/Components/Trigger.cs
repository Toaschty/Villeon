using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.ECS;

namespace Villeon.Components
{
    [Flags]
    public enum TriggerLayerType
    {
        FRIEND = 1,
        ENEMY = 2,
        PORTAL = 4,
    }

    public class Trigger : IComponent
    {
        private float _time = float.PositiveInfinity;

        public Trigger(TriggerLayerType layerType, float width, float height)
        {
            TriggerLayers = layerType;
            Offset = Vector2.Zero;
            Width = width;
            Height = height;
        }

        public Trigger(TriggerLayerType layerType, Vector2 offset, float width, float height)
        {
            TriggerLayers = layerType;
            Offset = offset;
            Width = width;
            Height = height;
        }

        public Trigger(TriggerLayerType layerType, Vector2 offset, float width, float height, float timeInSeconds)
        {
            TriggerLayers = layerType;
            Offset = offset;
            Width = width;
            Height = height;
            Time = timeInSeconds;
        }

        public TriggerLayerType TriggerLayers { get; private set; }

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
    }
}
