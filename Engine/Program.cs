using System.Numerics;
using CPG;
using CPG.Common.Rendering;
using CPG.Interface;
using CPG.Interface.Settings;
using Engine;
using Silk.NET.Input;
using VulkanBackend.Settings;

Logger.Init(); // Rerout any crash to the logger, and then quit the application

var settings = new SettingsContainer();
settings.Add(new InitializationSettings()
{
    ValidationLayers = true,
    Debug = true,

    EnabledLayers = new List<string>()
    {

    },

    EnabledExtensions = new List<string>()
    {
        
    },
    EnabledDeviceExtensions = new List<string>()
    {
        "VK_KHR_swapchain",
        "VK_EXT_descriptor_indexing"
    },
});

var window = Graphics.Initialize()
    .WithWindowSettings(new WindowSettings("Testing", 800, 600, false, false))
    .WithGraphicsInterface(new VulkanBackend.VulkanBackend())
    .WithCustomSettings(settings)
    .Build();

//Mesh mesh = null!;

window.SetRenderCallback(new GraphicsCallback((a, w) =>
{
    /*
    a.Clear(ClearMask.Color | ClearMask.Depth);   
    a.SetClearColor(new Vector4(0.2f, 0.4f, 0.6f, 1.0f));
    a.SetViewport(0,0,w.Width,w.Height);
    
    mesh.Draw();
    */
    w.SwapBuffers();
}));

window.SetUpdateCallback((w) =>
{
    /*
    if (w.Input.IsKeyDown(Key.A))
    {
        mesh.Transform.Position += new Vector3(-0.1f, 0.0f, 0.0f) * (float)w.DeltaTime;
    }

    if (w.Input.IsKeyDown(Key.D))
    {
        mesh.Transform.Position += new Vector3(0.1f, 0.0f, 0.0f) * (float)w.DeltaTime;
    }
    
    if (w.Input.IsKeyDown(Key.W))
    {
        mesh.Transform.Position += new Vector3(0.0f, 0.1f, 0.0f) * (float)w.DeltaTime;
    }

    if (w.Input.IsKeyDown(Key.S))
    {
        mesh.Transform.Position += new Vector3(0.0f, -0.1f, 0.0f) * (float)w.DeltaTime;
    }
    */
    if (w.Input.IsKeyReleased(Key.Escape))
    {
        w.Close();
    }

    if (w.Input.IsKeyReleased(Key.F5))
    {
        w.Fullscreen = !w.Fullscreen;
    }

    if (w.Input.IsKeyReleased(Key.F10))
    {
        throw new Exception("Testing crash");
    }
});

window.SetLoadCallback((w) =>
{
    /*
    var shader = new Shader(w.GraphicsApi, File.ReadAllText("Shaders/Basic.vert"), File.ReadAllText("Shaders/Basic.frag"));
    
    mesh = new Mesh(w.GraphicsApi, shader)
    {
        // Square, Vec3, Vec2 (uv)
        Vertices = new float[]
        {
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,
            0.5f, -0.5f, 0.0f, 1.0f, 0.0f,
            0.5f, 0.5f, 0.0f, 1.0f, 1.0f,
            -0.5f, 0.5f, 0.0f, 0.0f, 1.0f
        },
        Indices = new uint[]
        {
            0, 1, 2,
            2, 3, 0
        }
    };

    mesh.Init();
    
    var texture = new Texture(w.GraphicsApi, "Textures/example.png");
    mesh.Textures.Add(texture);
*/
});

window.Show();