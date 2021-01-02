using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{ 
     public class NamedBind<I>
    {
        public I Binding { get; }
        public string Name { get; }

        public NamedBind(I binding, string name)
        {
            Binding = binding;
            Name = name;
        }
    }

    public partial class KeyMode : IKeyMode
    {
        public class NamedBind<I>
        {
            public I Binding { get; }
            public string Name { get; }

            public NamedBind(I binding, string name)
            {
                Binding = binding;
                Name = name;
            }
        }



        private IDictionary<Sub, NamedBind<KeybindHandler>> _kbdSubs;
        private IDictionary<MouseEvent, NamedBind<MouseHandler>> _mouseSubs;
        public string Name { get;}
        private IConfigContext _context;

        public KeyMode(IConfigContext context, string name)
        {
             Name = name;
            _context = context;
            _kbdSubs = new Dictionary<Sub, NamedBind<KeybindHandler>>();
            _mouseSubs = new Dictionary<MouseEvent, NamedBind<MouseHandler>>();

        }
        
        public IDictionary<Sub, NamedBind<KeybindHandler>> ReturnSubs() 
        {
            return _kbdSubs;
        }
    }
}
