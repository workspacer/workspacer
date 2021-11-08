using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.Bar
{
    public interface IBarWidget
    {
        void Initialize(IBarWidgetContext context);
        IBarWidgetPart[] GetParts();
        bool IsDirty();
        void MarkDirty();
        void MarkClean();
    }
}
