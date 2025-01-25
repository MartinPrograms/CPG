namespace CPG.Common;

public class GraphicsSystem
{
    public OperatingSystem SupportedSystem { get; }

    public GraphicsSystem(OperatingSystem supportedSystem)
    {
        SupportedSystem = supportedSystem;
    }
    
    public bool IsSupported()
    {
        if (System.OperatingSystem.IsWindows() && SupportedSystem.HasFlag(OperatingSystem.Windows))
            return true;
        
        if (System.OperatingSystem.IsLinux() && SupportedSystem.HasFlag(OperatingSystem.Linux))
            return true;

        if (System.OperatingSystem.IsMacOS() && SupportedSystem.HasFlag(OperatingSystem.MacOS))
            return true;
        
        if (System.OperatingSystem.IsFreeBSD() && SupportedSystem.HasFlag(OperatingSystem.FreeBSD))
            return true;

        if (System.OperatingSystem.IsAndroid() && SupportedSystem.HasFlag(OperatingSystem.Android))
            return true;

        if (System.OperatingSystem.IsIOS() && SupportedSystem.HasFlag(OperatingSystem.iOS))
            return true;
        
        // TODO: Support Xbox, PlayStation, Nintendo, Web, Other
        return false;
    }
}

[Flags]
public enum OperatingSystem
{
    Windows,
    Linux,
    MacOS,
    FreeBSD,
    Android,
    iOS,
    Xbox,
    PlayStation,
    Nintendo,
    Web,
    Other
}