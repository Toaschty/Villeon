using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;

namespace Villeon.Render
{
    public class RenderingData
    {
        public RenderingData(Sprite? sprite, Transform transform, Vector2 offset, Vector2 scale)
        {
            Sprite = sprite;
            Transform = transform;
            Offset = offset;
            Scale = scale;
        }

        public Sprite? Sprite { get; set; }

        public Transform Transform { get; set; }

        public Vector2 Offset { get; set; }

        public Vector2 Scale { get; set; }

        public int SpriteNumber { get; set; }
    }
}
