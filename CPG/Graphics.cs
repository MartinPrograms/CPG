using CPG.Interface;
using CPG.Interface.Settings;

namespace CPG;

/// <summary>
/// CPG: Cross Platform Graphics
/// 
/// Initializes the graphics system, using a simple builder pattern.
/// </summary>
public class Graphics
{
    private WindowSettings _windowSettings;
    private IGraphicsInterface _graphicsInterface;
    public static Graphics Initialize()
    {
        return new Graphics();
    }
    
    public Graphics WithWindowSettings(WindowSettings settings)
    {
        _windowSettings = settings;
        return this;
    }
    
    public Graphics WithGraphicsInterface(IGraphicsInterface graphicsInterface)
    {
        _graphicsInterface = graphicsInterface;
        return this;
    }
    
    public IWindow Build()
    {
        return _graphicsInterface.Create(_windowSettings);
    }

    public Graphics WithCustomSettings(SettingsContainer settingsContainer)
    {
        _graphicsInterface.Settings = settingsContainer;
        return this;
    }
}