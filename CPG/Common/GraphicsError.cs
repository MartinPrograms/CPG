namespace CPG.Common;

public class GraphicsError
{
    public string Message { get; }
    public GraphicsError(string message)
    {
        Message = message;
    }
    
    public override string ToString()
    {
        return Message;
    }
}