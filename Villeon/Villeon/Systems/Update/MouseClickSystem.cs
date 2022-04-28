using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.Helper;

namespace Villeon.Systems
{
    public class MouseClickSystem : System, IUpdateSystem
    {
        private EntitySpawner _entitySpawner = new EntitySpawner();

        public MouseClickSystem(string name)
            : base(name)
        {
        }

        public void Update(float time)
        {
            foreach (MouseHandler.ClickedMouseButton button in MouseHandler.ClickedMouseButtons)
            {
                if (button.Button == MouseButton.Left)
                {
                    _entitySpawner.Spawn(button.MousePosition);
                }
            }
        }
    }
}