using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public interface ISystemTrayManager
    {
        void AddToContextMenu(string text, Action handler);
    }
}
