using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;

namespace Villeon.GUI
{
    public class MainMenuSetup
    {
        private List<Entity> _entities;

        private float _logoTextSize = 1.5f;
        private float _letterScale = 0.35f;

        public MainMenuSetup()
        {
            _entities = new List<Entity>();

            // Load Sprites
            Sprite villeonLogo = Assets.GetSprite("GUI.Scroll_Logo.png", Render.SpriteLayer.ScreenGuiBackground, false);

            // Load logo background
            Vector2 scrollMiddle = new Vector2(villeonLogo.Width / 2, villeonLogo.Height / 2);
            Entity scrollEntity = new Entity(new Transform(Vector2.Zero - (scrollMiddle * 0.5f), 0.5f, 0f), "Scroll Image");
            scrollEntity.AddComponent(villeonLogo);
            _entities.Add(scrollEntity);

            // Load logo text
            Text logoText = new Text("Villeon", new Vector2(-5.5f, 1f), "Alagard", 0f, 0.5f, _logoTextSize);
            Array.ForEach(logoText.GetEntities(), entity => _entities.Add(entity));

            // Load menu text
            Text newGameText = new Text("New Game", new Vector2(-1.9f, -0.2f), "Alagard", 0f, 0.5f, _letterScale);
            Array.ForEach(newGameText.GetEntities(), entity => _entities.Add(entity));
            Text loadGameText = new Text("Load Game", new Vector2(-1.95f, -1.2f), "Alagard", 0f, 0.5f, _letterScale);
            Array.ForEach(loadGameText.GetEntities(), entity => _entities.Add(entity));
            Text exitGameText = new Text("Exit Game", new Vector2(-1.9f, -2.2f), "Alagard", 0f, 0.5f, _letterScale);
            Array.ForEach(exitGameText.GetEntities(), entity => _entities.Add(entity));

            foreach (Entity entity in _entities)
            {
                Manager.GetInstance().AddEntityToScene(entity, "MainMenuScene");
            }
        }
    }
}
