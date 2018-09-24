using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public interface ILayoutEngine
    {
        string Name { get; }

        IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight);

        void ShrinkMasterArea();
        void ExpandMasterArea();
        void ResetMasterArea();

        void IncrementNumInMaster();
        void DecrementNumInMaster();
    }
}
