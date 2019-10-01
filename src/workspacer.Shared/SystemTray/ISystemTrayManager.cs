using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    public interface ISystemTrayManager : IDisposable
    {
        void AddToContextMenu(string text, Action handler);
        void Destroy();
    }
}
