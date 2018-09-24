using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer.Bar
{
    public interface IBarWidget
    {
        void Initialize(IBarWidgetContext context);
        IBarWidgetPart[] GetParts();
    }
}
