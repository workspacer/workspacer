using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public interface IWindowsManager
    {
        IWindowsDeferPosHandle DeferWindowsPos(int count);
        void DumpWindowDebugOutput();
        void DumpWindowUnderCursorDebugOutput();

    }
}
