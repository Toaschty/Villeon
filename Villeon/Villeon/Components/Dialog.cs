using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Dialog : IComponent
    {
        private string[] _dialogLines;

        public Dialog(params string[] dialogLines)
        {
            _dialogLines = dialogLines;
        }

        public string[] DialogLines { get => _dialogLines; }
    }
}
