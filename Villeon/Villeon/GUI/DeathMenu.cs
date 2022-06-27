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
using Villeon.Helper;
using Villeon.Utils;

namespace Villeon.GUI
{
    public class DeathMenu : IGUIMenu
    {
        private List<IEntity> _entities;

        private float _titleScale = 0.45f;
        private float _letterScale = 0.35f;

        private Vector2 _selectorStartPosition = new Vector2(-2.2f, 0f);
        private Vector2 _selectorStep = new Vector2(0f, 1f);

        private int _maxSelection = 1;
        private int _currentSelection = 0;

        private Text _selectorText;

        public DeathMenu()
        {
            // Create Pause layout
            _entities = new List<IEntity>();

            // Load Sprites
            Sprite backgroundScrollSprite = Asset.GetSprite("GUI.Scroll_Pausemenu.png", SpriteLayer.ScreenGuiBackground, false);

            // Background
            Vector2 scrollMiddle = new Vector2(backgroundScrollSprite.Width / 2f, (backgroundScrollSprite.Height / 2f) - 1f);
            Entity backgroundImage = new Entity(new Transform(Vector2.Zero - (scrollMiddle * 0.5f), 0.5f, 0f), "BackgroundImage");
            backgroundImage.AddComponent(backgroundScrollSprite);
            _entities.Add(backgroundImage);

            // Menu Texts
            Text resumeText = new Text("You Died!", new Vector2(-2.1f, 1f), "Alagard", 0f, 1f, _titleScale);
            _entities.AddRange(resumeText.GetEntities());

            Text saveGameText = new Text("Respawn", new Vector2(-1.4f, 0f), "Alagard", 0f, 1f, _letterScale);
            _entities.AddRange(saveGameText.GetEntities());

            Text exitGameText = new Text("Quit", new Vector2(-0.6f, -1f), "Alagard", 0f, 1f, _letterScale);
            _entities.AddRange(exitGameText.GetEntities());

            // Selector
            _selectorText = new Text(">             <", _selectorStartPosition, "Alagard", 0f, 0.5f, _letterScale);
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
                // Respawn game
                case 0:
                    StateManager.IsPlayerDead = false;
                    Manager.GetInstance().RemoveEntities(GetEntities());
                    SceneLoader.SetActiveScene("VillageScene");
                    break;

                // Quit
                case 1:
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
            _selectorText = new Text(">             <", newPos, "Alagard", 0f, 0.5f, _letterScale);

            // Add text entities to main menu scene
            Array.ForEach(_selectorText.GetEntities(), entity => _entities.Add(entity));
        }
    }
}
