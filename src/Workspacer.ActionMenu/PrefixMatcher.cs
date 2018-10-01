using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer.ActionMenu
{
    public class PrefixMatcher : IMatcher
    {
        public IMatch Match(string query, string toMatch)
        {
            query = query.ToLower();
            toMatch = toMatch.ToLower();

            if (toMatch.StartsWith(query))
            {
                return new Match()
                {
                    MatchParts = new List<IMatchPart>()
                    {
                        new MatchPart()
                        {
                            Start = 0,
                            End = query.Length
                        }
                    }
                };
            }
            return null;
        }
    }
}
