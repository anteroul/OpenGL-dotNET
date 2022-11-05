using System;
using Silk.NET.OpenGL;

namespace OpenGL
{
    public class Shader : IDisposable
    {
        private uint _handle;
        private GL _gl;

        public Shader(GL gl, string vsPath, string fsPath)
        {
            _gl = gl;

            uint vs = LoadShader(ShaderType.VertexShader, vsPath);
            uint fs = LoadShader(ShaderType.FragmentShader, fsPath);

            _handle = _gl.CreateProgram();

            _gl.AttachShader(_handle, vs);
            _gl.AttachShader(_handle, fs);
            _gl.LinkProgram(_handle);
            _gl.GetProgram(_handle, GLEnum.LinkStatus, out var status);

            if (status == 0)
            {
                throw new Exception($"Program failed to link with error: {_gl.GetProgramInfoLog(_handle)}");
            }

            _gl.DetachShader(_handle, vs);
            _gl.DetachShader(_handle, fs);
            _gl.DeleteShader(vs);
            _gl.DeleteShader(fs);
        }

        public void Use()
        {
            _gl.UseProgram(_handle);
        }

        public void SetUniform(string name, float value)
        {
            int location = _gl.GetUniformLocation(_handle, name);

            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }

            _gl.Uniform1(location, value);
        }

        public void Dispose()
        {
            _gl.DeleteProgram(_handle);
        }

        private uint LoadShader(ShaderType type, string path)
        {
            string src;
            uint handle;

            try
            {
                src = File.ReadAllText(path);
            }
            catch (Exception)
            {
                path = Program.rootDir + path;
                src = File.ReadAllText(path);
            }

            handle = _gl.CreateShader(type);

            _gl.ShaderSource(handle, src);
            _gl.CompileShader(handle);

            if (!string.IsNullOrWhiteSpace(_gl.GetShaderInfoLog(handle)))
            {
                throw new Exception($"Error compiling shader of type {type}, failed with error {_gl.GetShaderInfoLog(handle)}");
            }

            return handle;
        }
    }
}
