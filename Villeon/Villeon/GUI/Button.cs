using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;
using Villeon.Render;

namespace Villeon.GUI
{
    public class Button
    {
        private IEntity _button = new Entity("emptyFrame");

        private Vector2 _position;
        private Vector2 _scale;

        public Button(string buttonImage, Vector2 position, Vector2 scale)
        {
            _position = position;
            _scale = scale;

            CreateButtonImage(buttonImage);
        }

        public IEntity Entity => _button;

        private void CreateButtonImage(string buttonImage)
        {
            _button = new Entity(new Transform(_position, _scale, 0f), "ButtonFrame");
            Sprite image = Assets.GetSprite("GUI." + buttonImage, SpriteLayer.ScreenGuiMiddleground, false);
            _button.AddComponent(image);
        }
    }
}
