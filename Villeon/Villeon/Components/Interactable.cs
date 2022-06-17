using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Villeon.Components
{
    public class Interactable : IComponent
    {
        private List<Option> _options;

        public Interactable(params Option[] options)
        {
            _options = new List<Option>();

            foreach (Option option in options)
            {
                _options.Add(option);
            }
        }

        public bool CanInteract { get; set; }

        public List<Option> Options
        {
            get => _options;
        }
    }
}
