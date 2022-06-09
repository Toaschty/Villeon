using OpenTK.Graphics.OpenGL;

namespace Villeon.Render
{
    public class EBO
    {
        private int _eboID = 0;

        public EBO(int[] indices, int sizeInBytes)
        {
            GL.GenBuffers(1, out _eboID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeInBytes, indices, BufferUsageHint.StaticDraw);
        }

        public int ID { get => _eboID; }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboID);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void Delete()
        {
            GL.DeleteBuffers(1, ref _eboID);
        }
    }
}