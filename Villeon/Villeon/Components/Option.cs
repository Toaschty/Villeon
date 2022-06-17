using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Villeon.Components
{
    public class Option
    {
        public Option(string option, Keys interactionKey)
        {
            OptionString = option;
            Key = interactionKey;
        }

        public string OptionString { get; set; }

        public Keys Key { get; set; }
    }
}
