using CPG.Common.Rendering;
using CPG.Interface;
using Silk.NET.OpenGL;

namespace OpenGLBackend;

public static class ShaderManager
{
    public static List<IShader> Shaders = new List<IShader>();

    public static void UseShader(IShader shader)
    {
        if (shader is ShaderGL glShader)
        {
            glShader.Use();
        }
    }

    public static IShader CreateShader(GL gl, ShaderSettings settings)
    {
        var shader = new ShaderGL(gl,settings);
        Shaders.Add(shader);
        return shader;
    }

    public static void DeleteShader(IShader shader)
    {
        if (shader is ShaderGL glShader)
        {
            glShader.Dispose();
            Shaders.Remove(shader);
        }
    }

    public static bool SetUniform<T>(GL gl, IShader shader, string name, T value)
    {
        if (shader is ShaderGL glShader)
        {
            return glShader.SetUniformInternal<T>(name, value);
        }
        return false;
    }
}