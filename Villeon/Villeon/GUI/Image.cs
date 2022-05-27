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
    public class Image
    {
        private IEntity _image = new Entity("emptyFrame");

        private Vector2 _position;
        private Vector2 _scale;

        public Image(string buttonImage, Vector2 position, Vector2 scale)
        {
            _position = position;
            _scale = scale;

            CreateImage(buttonImage);
        }

        public IEntity Entity => _image;

        private void CreateImage(string imageName)
        {
            _image = new Entity(new Transform(_position, _scale, 0f), "ButtonFrame");
            Sprite image = Assets.GetSprite("GUI." + imageName, SpriteLayer.ScreenGuiBackground, false);
            _image.AddComponent(image);
        }
    }
}
