using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Generation;
using Villeon.Helper;
using Villeon.Utils;

namespace Villeon.GUI
{
    public class PauseMenu : IGUIMenu
    {
        private List<IEntity> _entities;

        private float _letterScale = 0.35f;

        private Vector2 _selectorStartPosition = new Vector2(-2.6f, 1f);
        private Vector2 _selectorStep = new Vector2(0f, 1f);

        private int _maxSelection = 2;
        private int _currentSelection = 0;

        private Text _selectorText;

        public PauseMenu()
        {
            // Create Pause layout
            _entities = new List<IEntity>();

            // Load Sprites
            Sprite backgroundScrollSprite = Asset.GetSprite("GUI.Scrolls.Scroll_Pausemenu.png", SpriteLayer.ScreenGuiBackground, false);

            // Background
            Vector2 scrollMiddle = new Vector2(backgroundScrollSprite.Width / 2f, (backgroundScrollSprite.Height / 2f) - 1f);
            Entity backgroundImage = new Entity(new Transform(Vector2.Zero - (scrollMiddle * 0.5f), 0.5f, 0f), "BackgroundImage");
            backgroundImage.AddComponent(backgroundScrollSprite);
            _entities.Add(backgroundImage);

            // Menu Texts
            Text resumeText = new Text("Resume", new Vector2(-1.35f, 1f), "Alagard", 0f, 1f, _letterScale);
            _entities.AddRange(resumeText.GetEntities());

            Text saveGameText = new Text("Save Game", new Vector2(-1.9f, 0f), "Alagard", 0f, 1f, _letterScale);
            _entities.AddRange(saveGameText.GetEntities());

            Text exitGameText = new Text("Exit Game", new Vector2(-1.85f, -1f), "Alagard", 0f, 1f, _letterScale);
            _entities.AddRange(exitGameText.GetEntities());

            // Selector
            _selectorText = new Text(">               <", _selectorStartPosition, "Alagard", 0f, 0.5f, _letterScale);
            _entities.AddRange(_selectorText.GetEntities());
        }

        public IEntity[] GetEntities()
        {
            return _entities.ToArray();
        }

        public bool OnKeyReleased(Keys key)
        {
            if (key == Keys.W)
            {
                _currentSelection--;

                if (_currentSelection < 0)
                    _currentSelection = _maxSelection;

                UpdateText();
            }

            if (key == Keys.S)
            {
                _currentSelection++;

                if (_currentSelection > _maxSelection)
                    _currentSelection = 0;

                UpdateText();
            }

            if (key == Keys.Space)
            {
                HandleSelectedAction();
                return false;
            }

            return true;
        }

        private void HandleSelectedAction()
        {
            switch (_currentSelection)
            {
                // Resume game
                case 0:
                    // Remove menu from scene
                    Manager.GetInstance().RemoveEntities(GetEntities());

                    // GUIHandler.GetInstance().ClearMenu();
                    StateManager.InMenu = false;
                    break;

                // Save game
                case 1:
                    SaveLoad.Save();

                    // Spawn SaveParticle
                    IEntity savingIcon = ParticleBuilder.AnimatedStationaryParticle(new Vector2(-0.95f, -5f), 2, 0.2f, true, "Animations.Saving.png", 0.5f, Components.SpriteLayer.ScreenGuiOnTopOfForeground);
                    Manager.GetInstance().AddEntity(savingIcon);
                    break;

                // Exit game
                case 2:
                    WindowHelper.CloseWindow();
                    break;
            }
        }

        private void UpdateText()
        {
            UnloadText();
            LoadText();
        }

        private void UnloadText()
        {
            // Remove all existing text entities from scene
            Array.ForEach(_selectorText.GetEntities(), entity =>
            {
                _entities.Remove(entity);
                Manager.GetInstance().RemoveEntity(entity);
            });
        }

        private void LoadText()
        {
            // Create new selector at correct position
            Vector2 newPos = new Vector2(_selectorStartPosition.X, _selectorStartPosition.Y - (_currentSelection * _selectorStep.Y));
            _selectorText = new Text(">               <", newPos, "Alagard", 0f, 0.5f, _letterScale);

            // Add text entities to main menu scene
            Array.ForEach(_selectorText.GetEntities(), entity => _entities.Add(entity));
        }
    }
}
