using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Player : IComponent
    {
        public bool IsWalking { get; set; } = false;

        public bool MovingLeft { get; set; } = true;

        public bool MovingRight { get; set; } = false;

        public bool WasLookingLeft { get; set; } = true;

        public bool WasLookingRight { get; set; } = false;
    }
}
