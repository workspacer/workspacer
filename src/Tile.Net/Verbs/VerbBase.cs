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
            var title = GetType().Name.Replace("Verb", "");
            MessageHelper.ShowMessage(title, message);
        }

        public abstract int Execute();
    }
}
