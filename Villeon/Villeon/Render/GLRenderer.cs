using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Villeon.Utils;

namespace Villeon.Render
{
    public class GLRenderer
    {
        public void DrawTile(Box2 rectangle, Box2 texCoords)
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

        public void DrawTile(Rect rectangle, Rect texCoords)
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

        public void DrawColliderQuad(Color4 color, Vector2 point, float width, float height)
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Color4(color);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2(point);
            GL.Vertex2(point.X + width, point.Y);
            GL.Vertex2(point.X + width, point.Y + height);
            GL.Vertex2(point.X, point.Y + height);
            GL.End();
            GL.Enable(EnableCap.Texture2D);
        }

        public void DrawPolygon(Color4 color, Vector2[] points, int size)
        {
            GL.Color4(color);
            GL.Begin(PrimitiveType.LineLoop);
            for (int i = 0; i < size; i++)
            {
                GL.Vertex2(points[i]);
            }

            GL.End();
        }
    }
}
