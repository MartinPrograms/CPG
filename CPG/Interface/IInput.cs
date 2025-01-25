using System.Numerics;
using Silk.NET.Input;

namespace CPG.Interface;

/// <summary>
/// Input interface.
/// </summary>
public interface IInput
{
    /// <summary>
    /// Update input state. Should be called once per frame, don't call this outside of a backend.
    /// </summary>
    public void Update();
    
    /// <summary>
    /// Is key down this frame.
    /// </summary>
    public bool IsKeyDown(Key key);
    
    /// <summary>
    /// Is key up this frame.
    /// </summary>
    public bool IsKeyUp(Key key);
    
    /// <summary>
    /// Is key pressed this frame (Up last frame, Down this frame).
    /// </summary>
    public bool IsKeyPressed(Key key);
    
    /// <summary>
    /// Is key released this frame (Down last frame, Up this frame).
    /// </summary>
    public bool IsKeyReleased(Key key);
    
    
    /// <summary>
    /// Is mouse button down this frame.
    /// </summary>
    public bool IsMouseButtonDown(MouseButton button);
    
    /// <summary>
    /// Is mouse button up this frame.
    /// </summary>
    public bool IsMouseButtonUp(MouseButton button);
    
    /// <summary>
    /// Is mouse button pressed this frame (Up last frame, Down this frame).
    /// </summary>
    public bool IsMouseButtonPressed(MouseButton button);
    
    /// <summary>
    /// Is mouse button released this frame (Down last frame, Up this frame).
    /// </summary>
    public bool IsMouseButtonReleased(MouseButton button);
    
    public Vector2 MousePosition { get; }
    public Vector2 MouseDelta { get; }
}