namespace CPG.Interface;

public class SettingsContainer
{
    private readonly Dictionary<Type, object> _settings = new();
     
    public void Add<T>(T settings) where T : notnull, ISetting
    {
        _settings[typeof(T)] = settings;
    }
    
    public T Get<T>() where T : ISetting
    {
        return (T)_settings[typeof(T)];
    }
    
    public bool Contains<T>() where T : ISetting
    {
        return _settings.ContainsKey(typeof(T));
    }
    
    public bool TryGet<T>(out T settings) where T : ISetting
    {
        if (Contains<T>())
        {
            settings = Get<T>();
            return true;
        }

        settings = default;
        return false;
    }
    
    public void Remove<T>() where T : ISetting
    {
        _settings.Remove(typeof(T));
    }
}

public interface ISetting
{
    
}