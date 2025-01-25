namespace CPG.Common;

public class GraphicsException : Exception
{
    public GraphicsError Error { get; }
    public GraphicsException(GraphicsError error) : base(error.ToString())
    {
        Error = error;
    }
}