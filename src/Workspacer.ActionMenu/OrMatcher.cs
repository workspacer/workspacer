using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer.ActionMenu
{
    public class OrMatcher : IMatcher
    {
        private IMatcher[] _matchers;

        public OrMatcher(params IMatcher[] matchers)
        {
            _matchers = matchers;
        }

        public IMatch Match(string query, string toMatch)
        {
            foreach (var matcher in _matchers)
            {
                var match = matcher.Match(query, toMatch);
                if (match != null)
                    return match;
            }
            return null;
        }
    }
}
