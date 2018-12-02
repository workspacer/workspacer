using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    public interface ILayoutEngine
    {
        string Name { get; }

        IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight);

        void ShrinkPrimaryArea();
        void ExpandPrimaryArea();
        void ResetPrimaryArea();

        void IncrementNumInPrimary();
        void DecrementNumInPrimary();
    }
}
