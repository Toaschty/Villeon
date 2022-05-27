using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;

namespace Villeon.GUI
{
    public class DungeonMenu : IGUIMenu
    {
        private List<Entity> _entities;

        public DungeonMenu()
        {
            // Create Menu layout
            _entities = new List<Entity>();

            // Load Sprites
            Sprite backgroundScrollSprite = Assets.GetSprite("GUI.Scroll_Dungeonmenu.png", Render.SpriteLayer.ScreenGuiBackground, false);
            Sprite horizontalLine1Sprite = Assets.GetSprite("GUI.Scroll_Line_Horizontal_1.png", Render.SpriteLayer.ScreenGuiMiddleground, false);
            Sprite horizontalLine2Sprite = Assets.GetSprite("GUI.Scroll_Line_Horizontal_2.png", Render.SpriteLayer.ScreenGuiMiddleground, false);
            Sprite horizontalLine3Sprite = Assets.GetSprite("GUI.Scroll_Line_Horizontal_3.png", Render.SpriteLayer.ScreenGuiMiddleground, false);
            Sprite verticalLineSprite = Assets.GetSprite("GUI.Scroll_Line_Vertical.png", Render.SpriteLayer.ScreenGuiMiddleground, false);

            // Background
            Vector2 scrollMiddle = new Vector2(backgroundScrollSprite.Width / 2f, backgroundScrollSprite.Height / 2f);
            Entity backgroundImage = new Entity(new Transform(Vector2.Zero - (scrollMiddle * 0.5f), 0.5f, 0f), "BackgroundImage");
            backgroundImage.AddComponent(backgroundScrollSprite);
            _entities.Add(backgroundImage);

            // Horizontal Lines
            Entity hLine1 = new Entity(new Transform(new Vector2(-6f, 2), 0.5f, 0f), "Horizontal Line 1");
            hLine1.AddComponent(horizontalLine1Sprite);
            Entity hLine2 = new Entity(new Transform(new Vector2(-6f, 0), 0.5f, 0f), "Horizontal Line 2");
            hLine2.AddComponent(horizontalLine2Sprite);
            Entity hLine3 = new Entity(new Transform(new Vector2(-6f, -2), 0.5f, 0f), "Horizontal Line 3");
            hLine3.AddComponent(horizontalLine3Sprite);
            Entity hLine4 = new Entity(new Transform(new Vector2(-6f, -4), 0.5f, 0f), "Horizontal Line 4");
            hLine4.AddComponent(horizontalLine2Sprite);

            _entities.Add(hLine1);
            _entities.Add(hLine2);
            _entities.Add(hLine3);
            _entities.Add(hLine4);

            // Text
            float letterSpacing = 0.01f;
            float lineSpacing = 0.01f;
            float letterScale = 0.6f;
            TextBox caveSelect = new TextBox("Cave", new Vector2(-6f, 2.5f), false, true, letterSpacing, lineSpacing, letterScale);
            TextBox castleSelect = new TextBox("Castle", new Vector2(-6f, 0.5f), false, true, letterSpacing, lineSpacing, letterScale);
            TextBox swampSelect = new TextBox("Swamp", new Vector2(-6f, -1.5f), false, true, letterSpacing, lineSpacing, letterScale);
            TextBox mushroomsSelect = new TextBox("Mushrooms", new Vector2(-6f, -3.5f), false, true, letterSpacing, lineSpacing, letterScale);

            // Infos
            TextBox caveText = new TextBox("Cave", new Vector2(1f, 3f), false, true, letterSpacing, lineSpacing, letterScale);
            TextBox caveDescription = new TextBox("A dark and spooky\nCave in the woods.\nYou will probably\nfind some slimes\nin there.", new Vector2(1f, 1f), false, true, letterSpacing, lineSpacing, letterScale / 2f);

            TextBox goText = new TextBox("Explore", new Vector2(2f, -3.2f), false, true, letterSpacing, lineSpacing, letterScale / 1.5f);
        }

        public IEntity[] GetEntities()
        {
            return _entities.ToArray();
        }
    }
}
