using Silk.NET.OpenGL;

namespace OpenGLBackend;

public class Helpers
{
    public static void CheckError(GL gl)
    {
        var error = gl.GetError();
        if (error != GLEnum.NoError)
        {
            throw new System.Exception($"OpenGL Error: {error}");
        }
    }
}