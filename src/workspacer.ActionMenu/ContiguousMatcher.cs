using System.Collections.Generic;

namespace workspacer.ActionMenu
{
    public class ContiguousMatcher : IMatcher
    {
        public IMatch Match(string query, string toMatch)
        {
            query = query.ToLower();
            toMatch = toMatch.ToLower();

            var index = toMatch.IndexOf(query);
            if (index >= 0)
            {
                return new Match()
                {
                    MatchParts = new List<IMatchPart>()
                    {
                        new MatchPart()
                        {
                            Start = index,
                            End = index + query.Length,
                        }
                    }
                };
            }
            return null;
        }
    }
}
