using CPG.Common.Rendering;
using CPG.Interface;

namespace Engine;

public class Shader
{
    private IGraphicsApi _graphicsApi;
    private IShader _shader;
    public Shader(IGraphicsApi objGraphicsApi, string vertexShader, string fragmentShader)
    {
        _graphicsApi = objGraphicsApi;
        
        ShaderSettings settings = new ShaderSettings(new[]
        {
            new ShaderStage(ShaderStageType.Vertex, vertexShader),
            new ShaderStage(ShaderStageType.Fragment, fragmentShader)
        });
        
        _shader = _graphicsApi.CreateShader(settings);
    }
    
    public void Use()
    {
        _graphicsApi.UseShader(_shader);
    }
}