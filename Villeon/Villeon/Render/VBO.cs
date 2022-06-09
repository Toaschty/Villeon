using System;
using OpenTK.Graphics.OpenGL;

namespace Villeon.Render
{
    public class VBO
    {
        private int _vboID = 0;

        public VBO(int sizeInBytes)
        {
            GL.GenBuffers(1, out _vboID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeInBytes, (IntPtr)null, BufferUsageHint.StaticDraw);
        }

        public int ID { get => _vboID; }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboID);
        }

        public void SetData(float[] vertices, int sizeInBytes)
        {
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, sizeInBytes, vertices);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Delete()
        {
            GL.DeleteBuffers(1, ref _vboID);
        }
    }
}
