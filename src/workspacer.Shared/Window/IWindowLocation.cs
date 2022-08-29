namespace workspacer
{
    public interface IWindowLocation
    {
        int X { get; }
        int Y { get; }
        int Width { get; }
        int Height { get; }

        WindowState State { get; }

        bool IsPointInside(int x, int y);
    }
}
