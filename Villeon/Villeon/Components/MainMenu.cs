using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.ECS;
using Villeon.GUI;

namespace Villeon.Components
{
    public class MainMenu : IComponent
    {
        // Start location of selector. Step = Distance between each selector locations
        private Vector2 _selectorStartPosition = new Vector2(-2.7f, -0.2f);
        private Vector2 _selectorStep = new Vector2(0f, 1f);

        private float _letterScale = 0.35f;

        // Current selection
        private int _maxSelection = 2;
        private int _currentSelection = 0;

        // Holds the reference to the text
        private Text _selectorText;

        public MainMenu()
        {
            // Spawn first Text at first position
            _selectorText = new Text(">               <", new Vector2(-2.7f, -0.2f), "Alagard", 0f, 0.5f, _letterScale);
            Array.ForEach(_selectorText.GetEntities(), entity => Manager.GetInstance().AddEntityToScene(entity, "MainMenuScene"));
        }

        public Text SelectorText
        {
            get { return _selectorText; }
            set { _selectorText = value; }
        }

        public int CurrentSelection
        {
            get => _currentSelection;
            set => _currentSelection = value;
        }

        public int MaxSelection => _maxSelection;

        public Vector2 SelectionStart => _selectorStartPosition;

        public Vector2 SelectionStep => _selectorStep;

        public float LetterScale => _letterScale;
    }
}
