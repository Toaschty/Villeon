using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon
{


    public class Renderer
    {
        public void Render(in Player player) // "in" functions -> readonly
        {
            DrawBody(CreateUnitCirclePoints((int)player.Position.X));
        }

        public List<Vector2> CreateUnitCirclePoints(int corners)
        {
            var points = new List<Vector2>();
            Console.WriteLine(corners);

            if (corners <= 1)
            {
                points.Add(Vector2.Zero);
                return points;
            }

            float delta = (2 * MathF.PI) / corners; // Winkelschritt 

            for (int i = 0; i < corners; ++i)
            {
                var alpha = i * delta;
                var x = MathF.Cos(alpha);
                var y = MathF.Sin(alpha);
                var pointOnUnitCircle = new Vector2(x, y);
                points.Add(pointOnUnitCircle);
            }
            return points;
        }

        public void DrawBody(List<Vector2> points)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Begin(BeginMode.TriangleFan);
            GL.Color4(Color4.Red);
            GL.Vertex2(Vector2.Zero); // Middle
            foreach (var point in points)
            {
                GL.Vertex2(point);
            }
            GL.Vertex2(points[0]);
            GL.End();
        }
    }
}
