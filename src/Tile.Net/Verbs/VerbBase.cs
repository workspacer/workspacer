using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.Verbs
{
    public abstract class VerbBase
    {
        protected void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }

        public abstract int Execute();
    }
}
