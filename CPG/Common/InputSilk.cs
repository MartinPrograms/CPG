using System.Numerics;
using CPG.Interface;
using Silk.NET.Input;

namespace CPG.Common;

public class InputSilk : IInput
{
    private IInputContext _inputContext;
    private List<Key> _keysDown = new List<Key>();
    private List<Key> _previousKeysDown = new List<Key>();
    private List<Key> _nextKeysDown = new List<Key>();
    
    private List<MouseButton> _mouseButtonsDown = new List<MouseButton>();
    private List<MouseButton> _previousMouseButtonsDown = new List<MouseButton>();
    private List<MouseButton> _nextMouseButtonsDown = new List<MouseButton>();
    
    public InputSilk(IInputContext inputContext)
    {
        _inputContext = inputContext;

        _inputContext.Keyboards[0].KeyDown += (sender, args, integer) =>
        {
            if (!_nextKeysDown.Contains(args))
            {
                _nextKeysDown.Add(args);
            }
        };

        _inputContext.Keyboards[0].KeyUp += (sender, args, integer) =>
        {
            _nextKeysDown.Remove(args);
        };
        
        _inputContext.Mice[0].MouseMove += (sender, args) =>
        {
            MousePosition = new Vector2(args.X, args.Y);
            MouseDelta = MousePosition - _previousMousePosition;
        };
        
        _inputContext.Mice[0].MouseDown += (sender, args) =>
        {
            if (!_nextMouseButtonsDown.Contains(args))
            {
                _nextMouseButtonsDown.Add(args);
            }
        };
        
        _inputContext.Mice[0].MouseUp += (sender, args) =>
        {
            _nextMouseButtonsDown.Remove(args);
        };
    }

    public void Update()
    {
        // Copy over the keys from the previous frame.
        _previousKeysDown.Clear();
        _previousKeysDown.AddRange(_keysDown);
        _keysDown.Clear();
        _keysDown.AddRange(_nextKeysDown);
        
        _previousMousePosition = MousePosition;
        _previousMouseButtonsDown.Clear();
        _previousMouseButtonsDown.AddRange(_mouseButtonsDown);
        _mouseButtonsDown.Clear();
        _mouseButtonsDown.AddRange(_nextMouseButtonsDown);
    }
    
    public bool IsKeyDown(Key key)
    {
        return _keysDown.Contains(key);
    }

    public bool IsKeyUp(Key key)
    {
        return !_keysDown.Contains(key);
    }

    public bool IsKeyPressed(Key key)
    {
        return _keysDown.Contains(key) && !_previousKeysDown.Contains(key);
    }

    public bool IsKeyReleased(Key key)
    {
        return !_keysDown.Contains(key) && _previousKeysDown.Contains(key);
    }

    public bool IsMouseButtonDown(MouseButton button)
    {
        return _mouseButtonsDown.Contains(button);
    }

    public bool IsMouseButtonUp(MouseButton button)
    {
        return !_mouseButtonsDown.Contains(button);
    }

    public bool IsMouseButtonPressed(MouseButton button)
    {
        return _mouseButtonsDown.Contains(button) && !_previousMouseButtonsDown.Contains(button);
    }

    public bool IsMouseButtonReleased(MouseButton button)
    {
        return !_mouseButtonsDown.Contains(button) && _previousMouseButtonsDown.Contains(button);
    }

    public Vector2 MousePosition { get; private set; }
    private Vector2 _previousMousePosition;
    public Vector2 MouseDelta { get; private set; }
}