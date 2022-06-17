using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Dialog : IComponent
    {
        private string _dialog;

        public Dialog(string dialog)
        {
            _dialog = dialog;
        }

        public string DialogString { get => _dialog; set => _dialog = value; }
    }
}
