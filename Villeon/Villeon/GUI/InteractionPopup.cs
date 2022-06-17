using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.GUI
{
    public class InteractionPopup
    {
        private List<IEntity> _frames = new List<IEntity>();
        private List<Entity> _text = new List<Entity>();
        private List<Option> _options;

        public InteractionPopup(Vector2 position, Interactable interactable, SpriteLayer frameLayer, SpriteLayer textLayer)
        {
            _options = interactable.Options;

            // Figure out startingposition of the boxes
            float popupFrameScale = 0.5f;
            Sprite popupSprite = Assets.Asset.GetSprite("GUI.Popup.png", frameLayer, true);
            Vector2 popupFramePos = new Vector2(position.X + 2f, position.Y + ((interactable.Options.Count * popupSprite.Height * popupFrameScale) / 2f));
            foreach (Option option in _options)
            {
                // Create the popup frame
                Transform transform = new Transform(popupFramePos, popupFrameScale, 0.0f);
                IEntity popupFrame = new Entity(transform, "InteractionPopupFrame");
                popupFrame.AddComponent(popupSprite);

                Text optionText = new Text(option.OptionString, popupFramePos + new Vector2(0.4f, 0.2f), "Alagard", textLayer, 0f, 0f, 0.3f);
                List<Entity> textEntities = optionText.Letters;

                // Add locally
                _frames.Add(popupFrame);
                _text.AddRange(textEntities);

                // Move the next Frame down by the height of the PopupSprite
                popupFramePos.Y -= popupSprite.Height * popupFrameScale;
            }
        }

        public List<Option> Options { get => _options; }

        public void Spawn()
        {
            foreach (IEntity entity in _frames)
            {
                Manager.GetInstance().AddEntity(entity);
            }

            foreach (Entity text in _text)
            {
                Manager.GetInstance().AddEntity(text);
            }
        }

        public void Delete()
        {
            foreach (IEntity entity in _frames)
            {
                Manager.GetInstance().RemoveEntity(entity);
            }

            foreach (Entity text in _text)
            {
                Manager.GetInstance().RemoveEntity(text);
            }

            _frames.Clear();
            _text.Clear();
        }
    }
}
