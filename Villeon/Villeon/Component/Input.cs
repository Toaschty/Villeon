using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Helper;

namespace Villeon.Component
{
    public class Input
    {
        public Input()
        {

        }

        public void Update(Player player)
        {
            foreach (Keys key in KeyHandler.pressedKeys)
            {
                switch (key)
                {
                    case Keys.W: // JUUMP
                        break;
                    case Keys.A:
                        player.Position.X++;
                        break;
                    case Keys.S: // Down?
                        break;
                    case Keys.D:
                        player.Position.X--;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}