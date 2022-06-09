using System;
using OpenTK.Graphics.OpenGL;

namespace Villeon.Render
{
    public class VAO
    {
        private int _vaoID = 0;

        public VAO()
        {
            GL.GenVertexArrays(1, out _vaoID);
        }

        public int ID { get => _vaoID; }

        public void LinkAttribute(ref VBO vbo, int layout, int numComponents, VertexAttribPointerType type, int strideInByte, int offsetInByte)
        {
            vbo.Bind();
            GL.VertexAttribPointer(layout, numComponents, type, false, strideInByte, (IntPtr)offsetInByte);
            GL.EnableVertexAttribArray(layout);
            vbo.Unbind();
        }

        public void Bind()
        {
            GL.BindVertexArray(_vaoID);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }

        public void Delete()
        {
            GL.DeleteVertexArrays(1, ref _vaoID);
        }
    }
}
