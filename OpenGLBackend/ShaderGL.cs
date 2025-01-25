using System.Numerics;
using CPG.Common.Rendering;
using CPG.Interface;
using Silk.NET.OpenGL;

namespace OpenGLBackend;

public class ShaderGL : IShader
{
    public uint ID;
    
    private GL _gl;
    private ShaderSettings _settings;
    public ShaderGL(GL gl, ShaderSettings settings)
    {
        _gl = gl;
        _settings = settings;
        ID = _gl.CreateProgram();
        Compile();  
    }

    private void Compile()
    {
        string infoLog;
        foreach (var stage in _settings.Stages)
        {
            var type = stage.Type;
            var source = stage.Source;
            
            ShaderType shaderType = type switch
            {
                ShaderStageType.Vertex => ShaderType.VertexShader,
                ShaderStageType.Fragment => ShaderType.FragmentShader,
                ShaderStageType.Compute => ShaderType.ComputeShader,
                _ => throw new ArgumentOutOfRangeException()
            };
            var shader = _gl.CreateShader(shaderType);
            _gl.ShaderSource(shader, source);
            _gl.CompileShader(shader);
            
            infoLog = _gl.GetShaderInfoLog(shader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new Exception($"Shader compilation failed: {infoLog}");
            }
            
            _gl.AttachShader(ID, shader);
            _gl.DeleteShader(shader);
        }
        
        _gl.LinkProgram(ID);
        infoLog = _gl.GetProgramInfoLog(ID);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Shader linking failed: {infoLog}");
        }
        
        _gl.ValidateProgram(ID);
        infoLog = _gl.GetProgramInfoLog(ID);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Shader validation failed: {infoLog}");
        }
    }
    
    public void Use()
    {
        _gl.UseProgram(ID);
    }
    
    public void Dispose()
    {
        _gl.DeleteProgram(ID);
    }

    internal bool SetUniformInternal<T>(string name, T value)
    {
        var index = _gl.GetUniformLocation(ID, name);
        if (index == -1)
        {
            return false;
        }
        
        if (value is int i)
        {
            _gl.Uniform1(index, i);
        }
        else if (value is float f)
        {
            _gl.Uniform1(index, f);
        }
        else if (value is Vector2 v2)
        {
            _gl.Uniform2(index, v2.X, v2.Y);
        }
        else if (value is Vector3 v3)
        {
            _gl.Uniform3(index, v3.X, v3.Y, v3.Z);
        }
        else if (value is Vector4 v4)
        {
            _gl.Uniform4(index, v4.X, v4.Y, v4.Z, v4.W);
        }
        else if (value is Matrix4x4 m4)
        {
            _gl.UniformMatrix4(index,1, false, m4.M11);
        }
        else
        {
            return false;
        }
        
        return true;
    }
}