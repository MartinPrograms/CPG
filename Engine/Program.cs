using System.Numerics;
using CPG;
using CPG.Common.Rendering;
using CPG.Interface;
using CPG.Interface.Settings;
using Engine;

var window = Graphics.Initialize()
    .WithWindowSettings(new WindowSettings("Testing", 800, 600, false, false))
    .WithGraphicsInterface(new OpenGLBackend.OpenGLBackend())
    .Build();

Mesh mesh = null;
Shader shader = null;

window.SetRenderCallback(new GraphicsCallback((a, w) =>
{
    a.Clear(ClearMask.Color | ClearMask.Depth);   
    a.SetClearColor(new Vector4(0.2f, 0.2f, 0.2f, 1.0f));
    a.SetViewport(0,0,w.Width,w.Height);
    
    // Drawing stuff:
    shader!.Use();
    mesh!.Draw();
    
    w.SwapBuffers();
}));

window.SetUpdateCallback((w) =>
{
    
});

window.SetLoadCallback((w) =>
{
    mesh = new Mesh(w.GraphicsApi);
    
    mesh.Vertices = new float[]
    {
        -0.5f, -0.5f, 0.0f,
        0.5f, -0.5f, 0.0f,
        0.0f, 0.5f, 0.0f
    };
    
    mesh.Indices = new uint[]
    {
        0, 1, 2
    };
    
    mesh.Init();
    
    // Shader languages differ per backend, so this is a bit of a mess, cant really do much about it unless we use SPIRV but it's not supported by all backends.
    shader = new Shader(w.GraphicsApi, File.ReadAllText("Shaders/Basic.vert"), File.ReadAllText("Shaders/Basic.frag"));
});

window.Show();