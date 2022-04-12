using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Villeon.Component;

namespace Villeon
{
    public class Player : Entity
    {
        public Vector2 Position;
        private Input _input;

        public Player(Input input) 
        {
            _input = input;
        }

        public override void Update()
        {
            _input.Update(this);  // Mark smart
        }

        public override void Draw(Renderer renderer)
        {
            renderer.Render(this);
        }
    }
}
