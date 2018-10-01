using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public interface IColor : IEquatable<IColor>
    {
        int R { get; }
        int G { get; }
        int B { get; }
    }
}
