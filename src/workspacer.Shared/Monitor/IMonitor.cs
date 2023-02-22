namespace workspacer
{
    public interface IMonitor
    {
        string Name { get; }
        int Index { get; }
        int Width { get; }
        int Height { get; }
        int X { get; }
        int Y { get; }
    }
}
