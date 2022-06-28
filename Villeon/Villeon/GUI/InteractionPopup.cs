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
        private List<IEntity> _text = new List<IEntity>();
        private List<Option> _options;
        private Interactable _interactable;

        public InteractionPopup(Vector2 position, Interactable interactable, SpriteLayer frameLayer, SpriteLayer textLayer)
        {
            _interactable = interactable;
            _options = interactable.Options;

            // Figure out startingposition of the boxes
            float popupFrameScale = 0.2f;
            Sprite popupSprite = Assets.Asset.GetSprite("GUI.Popup.png", frameLayer, true);
            Vector2 popupFramePos = new Vector2(position.X + 1f, position.Y + ((_interactable.Options.Count * popupSprite.Height * popupFrameScale) / 2f));
            foreach (Option option in _options)
            {
                // Create the popup frame
                Transform transform = new Transform(popupFramePos, popupFrameScale, 0.0f);
                IEntity popupFrame = new Entity(transform, "InteractionPopupFrame");
                popupFrame.AddComponent(popupSprite);

                Text optionText = new Text(option.OptionString, popupFramePos + new Vector2(0.2f, 0.1f), "Alagard_Thin", textLayer, 0f, 0f, 0.1f);
                List<IEntity> textEntities = optionText.Letters;

                // Add locally
                _frames.Add(popupFrame);
                _text.AddRange(textEntities);

                // Move the next Frame down by the height of the PopupSprite
                popupFramePos.Y -= popupSprite.Height * popupFrameScale;
            }
        }

        public List<Option> Options { get => _options; }

        public Interactable Interactable { get => _interactable; }

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
