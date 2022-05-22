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
        private bool _usesCamera = true;

        // Holds all Sprite Data (Position, Color, TextureCoods, TextureID)
        private Vertex[] _vertices;

        // Vertex Buffer Object, VertexArrayObject, ElementBufferObject
        private int _vboID = 0;
        private int _vaoID = 0;
        private Shader _shader;

        public RenderBatch(bool usesCamera)
        {
            _shader = Assets.GetShader(@"shader");
            _renderingData = new HashSet<RenderingData>(Constants.MAX_BATCH_SIZE);
            _textures = new List<Texture2D>();

            // Quad has 4 Vertices
            _vertices = new Vertex[Constants.MAX_BATCH_SIZE * Size.QUAD];
            _spriteCount = 0;
            _isFull = false;
            _usesCamera = usesCamera;
        }

        public void Start()
        {
            // Generate Vertex Array Object & Select it
            _vaoID = GL.GenVertexArray();
            GL.BindVertexArray(_vaoID);

            // Allocate needed space for the vertices
            _vboID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * Size.VERTEX_BYTES, (IntPtr)null, BufferUsageHint.DynamicDraw);

            // Generate index buffer
            int eboID = GL.GenBuffer();
            int[] indices = GenerateIndices();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, eboID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(float), indices, BufferUsageHint.StaticDraw);

            // 0: position attribute, 2: xy,
            // 1: color attribute, 4: rgba,
            // 2: Texture attribute, 2: uv,
            // 3: Texture ID, 1: ID,
            GL.VertexAttribPointer(0, Size.POSITION, VertexAttribPointerType.Float, false, Size.VERTEX_BYTES, (IntPtr)Offset.POSITION);
            GL.VertexAttribPointer(1, Size.COLOR, VertexAttribPointerType.Float, false, Size.VERTEX_BYTES, (IntPtr)Offset.COLOR);
            GL.VertexAttribPointer(2, Size.TEX_COORDS, VertexAttribPointerType.Float, false, Size.VERTEX_BYTES, (IntPtr)Offset.TEX_COORDS);
            GL.VertexAttribPointer(3, Size.TEX_ID, VertexAttribPointerType.Float, false, Size.VERTEX_BYTES, (IntPtr)Offset.TEX_ID);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);
            _shader.Use();
        }

        public void LoadBuffer()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboID);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, _vertices.Length * Size.VERTEX_BYTES, _vertices);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
        }

        public void RebufferAll()
        {
            int tmpSpriteCount = _spriteCount;
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
            _shader.UploadMat4("screenMatrix", Camera.GetScreenMatrix(Constants.SCREEN_SCALE));

            // Bind all textures that this batch contains
            int i = 0;
            foreach (Texture2D texture in _textures)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i + 1);
                texture.Bind();
                i++;
            }

            _shader.UploadIntArray("textures", _texSlots);
            _shader.UploadBool("usesCamera", _usesCamera);

            // Bind VAO & Enable all the attributes
            GL.BindVertexArray(_vaoID);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);

            // Draw indices
            GL.DrawElements(PrimitiveType.Triangles, _spriteCount * 6, DrawElementsType.UnsignedInt, 0);

            // Unbind Array, Attributes & Shader
            GL.DisableVertexAttribArray(3);
            GL.DisableVertexAttribArray(2);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(0);
            GL.BindVertexArray(0);

            // Unbind all textures that this batch contains
            GL.BindTexture(TextureTarget.Texture2D, 0);

            _shader.Detach();
        }

        public void AddSprite(RenderingData data)
        {
            // Save Current index & Store Sprite
            int index = _spriteCount;
            _renderingData.Add(data);
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

        public void RemoveEntity(RenderingData data)
        {
            if (_renderingData.Contains(data))
            {
                _renderingData.Remove(data);
                _spriteCount--;
                RebufferAll();
            }
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

        private void FillVertexAttributes(RenderingData data, int index)
        {
            int offset = index * Size.QUAD;
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

            // Line Fix
            _vertices[offset + 0].TextureCoords = new Vector2(0.0001f, 0.0001f);
            _vertices[offset + 1].TextureCoords = new Vector2(-0.0001f, 0.0001f);
            _vertices[offset + 2].TextureCoords = new Vector2(0.0001f, -0.0001f);
            _vertices[offset + 3].TextureCoords = new Vector2(-0.0001f, -0.0001f);

            // Fill vertex with attribute
            Vector2 add = new Vector2(0f, 0f);
            for (int i = 0; i < Size.QUAD; i++)
            {
                // Set dimensions
                if (i == 1)
                {
                    add = new Vector2(sprite.Width, 0f);
                }
                else if (i == 2)
                {
                    add = new Vector2(0f, sprite.Height);
                }
                else if (i == 3)
                {
                    add = new Vector2(sprite.Width, sprite.Height);
                }

                // Position
                _vertices[offset + i].Position = transform.Position - data.Offset + (add * data.Scale);

                // Color
                _vertices[offset + i].Color = sprite.Color;

                // Texture Coords
                _vertices[offset + i].TextureCoords += texCoords[i];

                // Texture ID
                _vertices[offset + i].TextureSlot = slot;
            }
        }

        private struct Vertex
        {
            public Vector2 Position;
            public Color4 Color;
            public Vector2 TextureCoords;
            public float TextureSlot;
        }

        // Attribute Sizes
        private struct Size
        {
            public const int QUAD = 4;
            public const int TEX_SLOTS = 8;
            public const int POSITION = 2;
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
