using System.Numerics;

namespace Engine;

public class Transform
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Scale { get; set; }
    
    public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }
    
    public Matrix4x4 GetModelMatrix()
    {
        return Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateTranslation(Position);
    }
    
    public static Transform Default()
    {
        return new Transform(Vector3.Zero, Quaternion.Identity, Vector3.One);
    }
}
