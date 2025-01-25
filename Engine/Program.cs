using System.Numerics;
using CPG;
using CPG.Common.Rendering;
using CPG.Interface;
using CPG.Interface.Settings;
using Engine;
using Silk.NET.Input;

var window = Graphics.Initialize()
    .WithWindowSettings(new WindowSettings("Testing", 800, 600, false, false))
    .WithGraphicsInterface(new OpenGLBackend.OpenGLBackend())
    .Build();

Mesh mesh = null!;

window.SetRenderCallback(new GraphicsCallback((a, w) =>
{
    a.Clear(ClearMask.Color | ClearMask.Depth);   
    a.SetClearColor(new Vector4(0.2f, 0.2f, 0.2f, 1.0f));
    a.SetViewport(0,0,w.Width,w.Height);
    
    mesh!.Draw();
    
    w.SwapBuffers();
}));

window.SetUpdateCallback((w) =>
{
    if (w.Input.IsKeyDown(Key.A))
    {
        mesh!.Transform.Position += new Vector3(-0.1f, 0.0f, 0.0f) * (float)w.DeltaTime;
    }

    if (w.Input.IsKeyDown(Key.D))
    {
        mesh!.Transform.Position += new Vector3(0.1f, 0.0f, 0.0f) * (float)w.DeltaTime;
    }
    
    if (w.Input.IsKeyDown(Key.W))
    {
        mesh!.Transform.Position += new Vector3(0.0f, 0.1f, 0.0f) * (float)w.DeltaTime;
    }

    if (w.Input.IsKeyDown(Key.S))
    {
        mesh!.Transform.Position += new Vector3(0.0f, -0.1f, 0.0f) * (float)w.DeltaTime;
    }
});

window.SetLoadCallback((w) =>
{
    var shader = new Shader(w.GraphicsApi, File.ReadAllText("Shaders/Basic.vert"), File.ReadAllText("Shaders/Basic.frag"));
    mesh = new Mesh(w.GraphicsApi, shader);
    
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
});

window.Show();