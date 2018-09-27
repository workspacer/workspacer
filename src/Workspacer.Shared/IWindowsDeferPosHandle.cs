using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public interface IWindowsDeferPosHandle : IDisposable
    {
        void DeferWindowPos(IWindow window, IWindowLocation location);
    }
}
