using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;

namespace Villeon.Render
{
    public struct RenderingData
    {
        public Sprite? Sprite;
        public Transform Transform;
        public Vector2 Offset;
        public Vector2 Scale;
    }
}
