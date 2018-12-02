using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.ActionMenu
{
    public interface IMatch
    {
        List<IMatchPart> MatchParts { get; }
    }
}
