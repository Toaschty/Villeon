using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Systems;

namespace Villeon.Render
{
    public class GuiRenderer : Villeon.Systems.System, IRenderSystem
    {
        public GuiRenderer(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(Sprite));
            Signature = Signature.AddToSignature(typeof(ScreenGui));
        }

        public void Add(IEntity entity)
        {
        }

        public bool Contains(IEntity entity)
        {
            return false;
        }

        public void Remove(IEntity entity)
        {
        }

        public void Render()
        {
        }
    }
}
