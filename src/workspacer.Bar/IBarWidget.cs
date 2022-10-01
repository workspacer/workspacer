namespace workspacer.Bar
{
    public interface IBarWidget
    {
        void Initialize(IBarWidgetContext context);
        IBarWidgetPart[] GetParts();
        bool IsDirty();
        void MarkDirty();
        void MarkClean();
    }
}
