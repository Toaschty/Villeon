using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Villeon
{
    /// <summary>
    /// Rectangle Min is the Bottom left corner, while Max is top right, relative to Min.
    /// </summary>
    /// <param name="Min">Bottom left cornerl.</param>
    /// <param name="Max">Top right corner.</param>
    public struct Rect
    {
        public Vector2 Min;
        public Vector2 Max;
        public float Width;
        public float Height;

        public Rect(Vector2 position, float width, float height)
        {
            Min = position;
            Width = width;
            Height = height;
            Max = position + new Vector2(width, height);
        }

        public Rect(Vector2 min, Vector2 max)
        {
            Min = min;
            Width = max.X - min.X;
            Height = max.Y - max.Y;
            Max = min + max;
        }
    }
}
