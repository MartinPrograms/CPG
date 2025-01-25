namespace CPG.Interface;

public class SettingsContainer
{
    private readonly Dictionary<Type, object> _settings = new();
     
    public void Add<T>(T settings) where T : notnull
    {
        _settings[typeof(T)] = settings;
    }
    
    public T Get<T>()
    {
        return (T)_settings[typeof(T)];
    }
    
    public bool Contains<T>()
    {
        return _settings.ContainsKey(typeof(T));
    }
    
    public bool TryGet<T>(out T settings)
    {
        if (Contains<T>())
        {
            settings = Get<T>();
            return true;
        }

        settings = default;
        return false;
    }
    
    public void Remove<T>()
    {
        _settings.Remove(typeof(T));
    }
}