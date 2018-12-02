using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.ActionMenu
{
    public interface IMatcher
    {
        IMatch Match(string query, string toMatch);
    }
}
