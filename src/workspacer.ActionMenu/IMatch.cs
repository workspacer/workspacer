using System.Collections.Generic;

namespace workspacer.ActionMenu
{
    public interface IMatch
    {
        List<IMatchPart> MatchParts { get; }
    }
}
