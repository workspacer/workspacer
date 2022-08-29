namespace workspacer.ActionMenu
{
    public interface IMatcher
    {
        IMatch Match(string query, string toMatch);
    }
}
