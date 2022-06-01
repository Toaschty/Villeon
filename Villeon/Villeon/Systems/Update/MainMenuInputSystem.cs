using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.ECS;
using Villeon.GUI;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class MainMenuInputSystem : System, IUpdateSystem
    {
        public MainMenuInputSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(MainMenu));
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                MainMenu mainMenu = entity.GetComponent<MainMenu>();

                // Check for key presses
                Keys? currentKey = KeyHandler.GetLastReleasedKey();

                if (currentKey != null)
                {
                    // Move selection up
                    if (currentKey == Keys.W)
                    {
                        mainMenu.CurrentSelection--;

                        if (mainMenu.CurrentSelection < 0)
                            mainMenu.CurrentSelection = mainMenu.MaxSelection;

                        UpdateText(mainMenu);
                    }

                    // Move selection down
                    if (currentKey == Keys.S)
                    {
                        mainMenu.CurrentSelection++;

                        if (mainMenu.CurrentSelection > mainMenu.MaxSelection)
                            mainMenu.CurrentSelection = 0;

                        UpdateText(mainMenu);
                    }

                    if (currentKey == Keys.Space)
                    {
                        HandleSelectedAction(mainMenu);
                    }
                }
            }
        }

        private void HandleSelectedAction(MainMenu menu)
        {
            switch (menu.CurrentSelection)
            {
                case 0: SceneLoader.SetActiveScene("LoadingScene"); break;
                case 1: SceneLoader.SetActiveScene("LoadingScene"); break;
                case 2: WindowCreator.CloseWindow(); break;
            }
        }

        // Update the text by unloading and loading the text
        private void UpdateText(MainMenu menu)
        {
            UnloadText(menu);
            LoadText(menu);
        }

        private void UnloadText(MainMenu menu)
        {
            // Remove all existing text entities from scene
            Array.ForEach(menu.SelectorText.GetEntities(), entity =>
            {
                Manager.GetInstance().RemoveEntity(entity);
            });
        }

        private void LoadText(MainMenu menu)
        {
            // Create new selector at correct position
            Vector2 newPos = new Vector2(menu.SelectionStart.X, menu.SelectionStart.Y - (menu.CurrentSelection * menu.SelectionStep.Y));
            menu.SelectorText = new Text(">               <", newPos, "Alagard", 0f, 0.5f, menu.LetterScale);

            // Add text entities to main menu scene
            Array.ForEach(menu.SelectorText.GetEntities(), entity => Manager.GetInstance().AddEntityToScene(entity, "MainMenuScene"));
        }
    }
}
