using System.Collections.Generic;

namespace workspacer.ActionMenu
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
