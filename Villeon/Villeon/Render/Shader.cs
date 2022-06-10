using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Villeon.Helper;

namespace Villeon.Render
{
    public class Shader
    {
        private int _shaderProgramID;
        private string _vertexSource = string.Empty;
        private string _fragmentSource = string.Empty;
        private string _filePath = string.Empty;
        private Dictionary<string, int> _uniformLocations = new Dictionary<string, int>();

        public Shader()
        {

        }

        public Shader(string shaderPath)
        {
            Load(shaderPath);
        }

        public void Load(string shaderPath)
        {
            _filePath = shaderPath;
            _vertexSource = ResourceLoader.LoadContentAsText(_filePath + ".vert");
            _fragmentSource = ResourceLoader.LoadContentAsText(_filePath + ".frag");
        }

        public void Compile()
        {
            _shaderProgramID = GL.CreateProgram();
            int vs = Compile(ShaderType.VertexShader, _vertexSource);
            int fs = Compile(ShaderType.FragmentShader, _fragmentSource);

            // Link Vertex & Fragment shader into the Program
            GL.AttachShader(_shaderProgramID, vs);
            GL.AttachShader(_shaderProgramID, fs);
            GL.LinkProgram(_shaderProgramID);
            GL.ValidateProgram(_shaderProgramID);

            // Delete leftovers
            GL.DeleteShader(vs);
            GL.DeleteShader(fs);
        }

        public void Use()
        {
            GL.UseProgram(_shaderProgramID);
        }

        public void Detach()
        {
            GL.UseProgram(0);
        }

        public void Delete()
        {
            GL.DeleteProgram(_shaderProgramID);
        }

        public void UploadMat4(in string uniformName, Matrix4 mat4)
        {
            GL.UniformMatrix4(GetLocation(uniformName), true, ref mat4);
        }

        public void UploadVec3(in string uniformName, Vector3 vec3)
        {
            GL.Uniform3(GetLocation(uniformName), vec3);
        }

        public void UploadVec4(in string uniformName, Vector4 vec4)
        {
            GL.Uniform4(GetLocation(uniformName), vec4);
        }

        public void UploadVec4(in string uniformName, Color4 color)
        {
            GL.Uniform4(GetLocation(uniformName), color);
        }

        public void UploadTexture(in string uniformName, int textureSlot)
        {
            GL.Uniform1(GetLocation(uniformName), textureSlot);
        }

        public void UploadIntArray(in string uniformName, int[] array)
        {
            GL.Uniform1(GetLocation(uniformName), array.Length, array);
        }

        public void UploadFloatArray(in string uniformName, float[] array)
        {
            GL.Uniform1(GetLocation(uniformName), array.Length, array);
        }

        public void UploadBool(in string uniformName, bool value)
        {
            GL.Uniform1(GetLocation(uniformName), value ? 1 : 0);
        }

        internal void UploadFloat(string uniformName, float val)
        {
            GL.Uniform1(GetLocation(uniformName), val);
        }

        private int GetLocation(string uniformName)
        {
            if (_uniformLocations.ContainsKey(uniformName))
            {
                return _uniformLocations[uniformName];
            }
            else
            {
                int location = GL.GetUniformLocation(_shaderProgramID, uniformName);
                _uniformLocations.Add(uniformName, location);
                return location;
            }
        }

        private int Compile(ShaderType shaderType, in string source)
        {
            int shaderId = GL.CreateShader(shaderType);
            GL.ShaderSource(shaderId, source);
            GL.CompileShader(shaderId);
            ShaderErrorHandling(shaderId);
            return shaderId;
        }

        private bool ShaderErrorHandling(int shaderId)
        {
            // Error handling
            int success = 0;
            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out success);
            if (success == 0)
            {
                int length;
                string message;
                GL.GetShader(shaderId, ShaderParameter.InfoLogLength, out length);
                GL.GetShaderInfoLog(shaderId, out message);
                Console.WriteLine(message);
                GL.DeleteShader(shaderId);
                return false;
            }

            return true;
        }
    }
}
