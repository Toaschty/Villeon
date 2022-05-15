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
        // For removing Entities
        protected HashSet<IEntity> _entities = new HashSet<IEntity>();

        // Sprites Stored
        private Sprite[] _sprites;
        private List<Texture2D> _textures;
        private int[] _texSlots = { 0, 1, 2, 3, 4, 5, 6, 7 };
        private int _spriteCount;
        private bool _isFull;

        // Holds all Sprite Data (Position, Color, TextureCoods, TextureID)
        private Vertex[] _vertices;

        // Vertex Buffer Object, VertexArrayObject, ElementBufferObject
        private int _vboID = 0;
        private int _vaoID = 0;
        private int _maxBatchSize;
        private Shader _shader;

        public RenderBatch(int maxBatchSize)
        {
            _shader = Assets.GetShader(@"shader");
            _sprites = new Sprite[maxBatchSize];
            _textures = new List<Texture2D>();
            _maxBatchSize = maxBatchSize;

            // Quad has 4 Vertices
            _vertices = new Vertex[maxBatchSize * Size.QUAD];
            _spriteCount = 0;
            _isFull = false;
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

        private int[] GenerateIndices()
        {
            int[] elements = new int[6 * _maxBatchSize];
            for (int i = 0; i < _maxBatchSize; i++)
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

        public void LoadBuffer()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboID);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, _vertices.Length * Size.VERTEX_BYTES, _vertices);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
        }

        public virtual void Render()
        {
            _shader.Use();
            // Update Camera, Set Transform in VertexShader
            Camera.Update();
            _shader.UploadMat4("cameraMatrix", Camera.GetMatrix());

            // Bind all textures that this batch contains
            int i = 0;
            foreach (Texture2D texture in _textures)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i + 1);
                texture.Bind();
                i++;
            }

            _shader.UploadIntArray("textures", _texSlots);

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

        public void AddEntity(IEntity entity)
        {
            Sprite sprite = entity.GetComponent<Sprite>();

            // Save Current index & Store Sprite
            int index = _spriteCount;
            _sprites[index] = sprite;
            _spriteCount++;

            // Add Entity
            _entities.Add(entity);

            // If the sprite has a texture, add it to _textures
            if (sprite.Texture != null)
            {
                if (!_textures.Contains(sprite.Texture))
                {
                    _textures.Add(sprite.Texture);
                }
            }

            // Fill the Attributes für this sprite
            FillVertexAttributes(index, entity);

            if (_spriteCount >= _maxBatchSize)
                _isFull = true;
        }

        public void RemoveEntity(IEntity entity)
        {
            if (_entities.Contains(entity))
            {
                _entities.Remove(entity);

                // Tell that there is nothing to render
                Clear();

                // Fill it up with Entities again
                foreach (IEntity e in _entities)
                {
                    AddEntity(e);
                }

                // Load openGL buffers
                LoadBuffer();
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

        public void Clear()
        {
            _spriteCount = 0;
            _textures.Clear();
        }

        private void FillVertexAttributes(int index, IEntity entity)
        {
            Transform transform = entity.GetComponent<Transform>();
            Sprite sprite = _sprites[index];
            int offset = index * Size.QUAD;
            Vector2[] texCoords = sprite.TexCoords;

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
                if (i == 1)
                {
                    add = new Vector2(1f, 0f);
                }
                else if (i == 2)
                {
                    add = new Vector2(0f, 1f);
                }
                else if (i == 3)
                {
                    add = new Vector2(1f, 1f);
                }

                //   2 --- 4
                //   |     |     <- Quad Vertex order
                //   0 --- 1

                // Fix Aspect ratio
                // Fix lining weirdness

                // Position
                _vertices[offset + i].Position.X = transform.Position.X + (add.X * transform.Scale.X);
                _vertices[offset + i].Position.Y = transform.Position.Y + (add.Y * transform.Scale.Y * sprite.AspectRatio);

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
