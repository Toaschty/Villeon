using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;
using Villeon.Utils;
using Zenseless.OpenTK;

namespace Villeon.Render
{
    public class RenderBatch
    {
        // Sprites Stored
        private HashSet<RenderingData> _renderingData;
        private List<Texture2D> _textures;
        private int[] _texSlots = { 0, 1, 2, 3, 4, 5, 6, 7 };
        private int _spriteCount;
        private bool _isFull;

        // Holds all Sprite Data (Position, Color, TextureCoods, TextureID)
        private float[] _vertices;

        // Vertex Buffer Object, VertexArrayObject, ElementBufferObject
        private VAO _vao;
        private VBO _vbo;
        private EBO _ebo;

        private Shader _shader;

        private int _l = 0;

        public RenderBatch(Shader shader)
        {
            _shader = shader;
            _renderingData = new HashSet<RenderingData>(Constants.MAX_BATCH_SIZE);
            _textures = new List<Texture2D>();

            // Quad has 4 Vertices, each vertex is 9 long
            _vertices = new float[Constants.MAX_BATCH_SIZE * Size.QUAD * Size.VERTEX];
            _spriteCount = 0;
            _isFull = false;

            // Create the vao and BIND IT so everything afterwards gets bound to it!
            _vao = new VAO();
            _vao.Bind();

            //  Create Vertex Buffer with the given vertices
            _vbo = new VBO(_vertices.Length * sizeof(float));

            // Create the Element buffer
            int[] indices = GenerateIndices();
            _ebo = new EBO(indices, indices.Length * sizeof(float));

            // Link the Attributes to the Vertexbuffer
            _vao.LinkAttribute(ref _vbo, 0, Size.POSITION, VertexAttribPointerType.Float, Size.VERTEX_BYTES, Offset.POSITION);
            _vao.LinkAttribute(ref _vbo, 1, Size.COLOR, VertexAttribPointerType.Float, Size.VERTEX_BYTES, Offset.COLOR);
            _vao.LinkAttribute(ref _vbo, 2, Size.TEX_COORDS, VertexAttribPointerType.Float, Size.VERTEX_BYTES, Offset.TEX_COORDS);
            _vao.LinkAttribute(ref _vbo, 3, Size.TEX_ID, VertexAttribPointerType.Float, Size.VERTEX_BYTES, Offset.TEX_ID);

            _vao.Unbind();
            _vbo.Unbind();
            _ebo.Unbind();
        }

        public void LoadBuffer()
        {
            _vao.Bind();
            _vbo.Bind();
            _vbo.SetData(_vertices, _spriteCount * Size.VERTEX_BYTES * Size.QUAD);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            _vao.Unbind();
            _vbo.Unbind();
        }

        public void Rebuffer()
        {
            _spriteCount = 0;
            _textures.Clear();
            foreach (RenderingData data in _renderingData)
            {
                AddSprite(data);
            }

            LoadBuffer();
        }

        public void Render()
        {
            _shader.Use();

            // Update Camera, Set Transform in VertexShader
            Camera.Update();
            _shader.UploadMat4("cameraMatrix", Camera.GetMatrix());
            _shader.UploadMat4("screenMatrix", Camera.GetScreenMatrix());

            // Bind all textures that this batch contains
            int i = 0;
            foreach (Texture2D texture in _textures)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i + 1);
                texture.Bind();
                i++;
            }

            _shader.UploadIntArray("textures", _texSlots);

            _shader.UploadInt("lightCount", MouseHandler.ClickedMouseButtonCount);

            // Directional Lighting
            _shader.UploadVec3("dirLight.direction", new Vector3(0, 0, -1));
            _shader.UploadVec3("dirLight.ambient", new Vector3(0.8f));
            _shader.UploadVec3("dirLight.diffuse", new Vector3(0.8f));

            Random random = new Random();
            foreach (MouseHandler.ClickedMouseButton c in MouseHandler.ClickedRightMouseButtons)
            {
                _shader.UploadVec3("pointLights[" + _l + "].position", new Vector3(c.MousePosition.X, c.MousePosition.Y, 0));
                _shader.UploadVec3("pointLights[" + _l + "].ambient", new Vector3(1, 0, 0.5f));
                _shader.UploadVec3("pointLights[" + _l + "].diffuse", new Vector3(1f));
                _l++;
            }

            MouseHandler.ClickedRightMouseButtons.Clear();

            // Bind VAO & Enable all the attributes & Draw
            _vao.Bind();
            GL.DrawElements(PrimitiveType.Triangles, _spriteCount * 6, DrawElementsType.UnsignedInt, 0);
        }

        public void AddRenderingData(RenderingData data)
        {
            _renderingData.Add(data);
        }

        public void AddSprite(RenderingData data)
        {
            // Save Current index & Store Sprite
            int index = _spriteCount;
            data.SpriteNumber = index;
            _spriteCount++;

            // If the sprite has a texture, add it to _textures
            if (data.Sprite!.Texture != null)
            {
                if (!_textures.Contains(data.Sprite.Texture))
                {
                    _textures.Add(data.Sprite.Texture);
                }
            }

            // Fill the Attributes für this sprite
            FillVertexAttributes(data, index);

            if (_spriteCount >= Constants.MAX_BATCH_SIZE)
                _isFull = true;
        }

        public bool RemoveEntity(RenderingData removableData)
        {
            if (_renderingData.Contains(removableData))
            {
                // Move the last verticies to the deleted verticies
                RenderingData backData = _renderingData.Last();

                // Clear the last vertices
                ClearVertices(backData.SpriteNumber);

                // Store sprite number
                //backData.SpriteNumber = removableData.SpriteNumber;

                // Overwrite the Removed black
                //FillVertexAttributes(backData, removableData.SpriteNumber);
                _renderingData.Remove(removableData);
                _spriteCount--;
                return true;
            }

            return false;
        }

        public bool Full()
        {
            return _isFull;
        }

        public bool HasTextureRoom()
        {
            return _textures.Count < Size.TEX_SLOTS;
        }

        public bool HasTexture(Texture2D? texture)
        {
            if (texture == null)
                return true;

            return _textures.Contains(texture);
        }

        private int[] GenerateIndices()
        {
            int[] elements = new int[6 * Constants.MAX_BATCH_SIZE];
            for (int i = 0; i < Constants.MAX_BATCH_SIZE; i++)
            {
                LoadElementIndices(elements, i);
            }

            return elements;
        }

        private void LoadElementIndices(int[] elements, int index)
        {
            int offsetArrayIndex = 6 * index;
            int offset = 4 * index;

            // Triangle 1: 0, 1, 2
            elements[offsetArrayIndex + 0] = offset + 0; // Bottom Right
            elements[offsetArrayIndex + 1] = offset + 1; // Top Right
            elements[offsetArrayIndex + 2] = offset + 2; // TOP Left

            // Triangle 2: 2, 1, 3
            elements[offsetArrayIndex + 3] = offset + 2; // Top Left
            elements[offsetArrayIndex + 4] = offset + 1; // Bottom Left
            elements[offsetArrayIndex + 5] = offset + 3; // Bottom Right
        }

        private void ClearVertices(int index)
        {
            // Offset in the vertex Array for the given sprite
            int start = index * Size.QUAD * Size.VERTEX;
            int end = start + (Size.QUAD * Size.VERTEX);

            // Clear the vertices
            for (int i = start; i < end; i++)
            {
                _vertices[i] = 0;
            }
        }

        private void FillVertexAttributes(RenderingData data, int index)
        {
            Sprite sprite = data.Sprite !;
            Transform transform = data.Transform !;
            Vector2[] texCoords = sprite.TexCoords !;

            // [0, tex1, tex2, tex3, ..]
            int slot = 0;
            if (sprite.Texture != null)
            {
               for (int i = 0; i < _textures.Count; i++)
                {
                    if (_textures[i] == sprite.Texture)
                    {
                        slot = i + 1;
                        break;
                    }
                }
            }

            // Offset in the vertex Array for the given sprite
            int offset = index * Size.QUAD * Size.VERTEX;

            // Fill vertex with attribute
            Vector2 add = new Vector2(0f, 0f);
            Vector2 line = new Vector2(0.0001f, 0.0001f);
            for (int i = 0; i < Size.QUAD; i++)
            {
                // Set dimensions
                switch (i)
                {
                    case 1:
                        add = new Vector2(sprite.Width, 0f);
                        line = new Vector2(-0.0001f, 0.0001f);
                        break;
                    case 2:
                        add = new Vector2(0f, sprite.Height);
                        line = new Vector2(0.0001f, -0.0001f);
                        break;
                    case 3:
                        add = new Vector2(sprite.Width, sprite.Height);
                        line = new Vector2(-0.0001f, -0.0001f);
                        break;
                }

                // Position
                _vertices[offset + 0] = transform.Position.X + data.Offset.X + (add.X * data.Scale.X);
                _vertices[offset + 1] = transform.Position.Y + data.Offset.Y + (add.Y * data.Scale.Y);
                _vertices[offset + 2] = -(int)sprite.RenderLayer;

                // Color
                _vertices[offset + 3] = sprite.Color.R;
                _vertices[offset + 4] = sprite.Color.G;
                _vertices[offset + 5] = sprite.Color.B;
                _vertices[offset + 6] = sprite.Color.A;

                // Texture Coord
                _vertices[offset + 7] = texCoords[i].X + line.X;
                _vertices[offset + 8] = texCoords[i].Y + line.Y;

                // Texture ID
                _vertices[offset + 9] = (float)slot;

                offset += Size.VERTEX;
            }
        }

        // Attribute Sizes
        private struct Size
        {
            public const int QUAD = 4;
            public const int TEX_SLOTS = 8;
            public const int POSITION = 3;
            public const int COLOR = 4;
            public const int TEX_COORDS = 2;
            public const int TEX_ID = 1;
            public const int VERTEX = POSITION + COLOR + TEX_COORDS + TEX_ID;
            public const int VERTEX_BYTES = VERTEX * sizeof(float);
        }

        // Attribute Offsets
        private struct Offset
        {
            public const int POSITION = 0;
            public const int COLOR = Size.POSITION * sizeof(float);
            public const int TEX_COORDS = (Size.POSITION + Size.COLOR) * sizeof(float);
            public const int TEX_ID = (Size.POSITION + Size.COLOR + Size.TEX_COORDS) * sizeof(float);
        }
    }
}
