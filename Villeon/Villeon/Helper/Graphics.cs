﻿using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Helper
{
    static class Graphics
    {
        public static void DrawTile(Box2 rectangle, Box2 texCoords)
        {
            GL.Color4(Color4.White);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(texCoords.Min);
            GL.Vertex2(rectangle.Min);
            GL.TexCoord2(texCoords.Max.X, texCoords.Min.Y);
            GL.Vertex2(rectangle.Max.X, rectangle.Min.Y);
            GL.TexCoord2(texCoords.Max);
            GL.Vertex2(rectangle.Max);
            GL.TexCoord2(texCoords.Min.X, texCoords.Max.Y);
            GL.Vertex2(rectangle.Min.X, rectangle.Max.Y);
            GL.End();
        }

        public static void DrawColliderQuad(Color4 color, Vector2 point, float width, float height)
        {
            GL.Color4(color);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2(point);
            GL.Vertex2(point.X + width, point.Y);
            GL.Vertex2(point.X + width, point.Y + height);
            GL.Vertex2(point.X, point.Y + height);
            GL.End();

            color.A = 0.3f;
            GL.Color4(color);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(point);
            GL.Vertex2(point.X + width, point.Y);
            GL.Vertex2(point.X + width, point.Y + height);
            GL.Vertex2(point.X, point.Y + height);
            GL.End();
        }

        public static void DrawPolygon(Color4 color, List<Vector2> points)
        {
            GL.Color4(color);
            GL.Begin(PrimitiveType.LineLoop);
            foreach (Vector2 point in points)
            {
                GL.Vertex2(point);
            }

            GL.End();
        }
    }
}
