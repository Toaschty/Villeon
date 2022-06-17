using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.GUI
{
    public class DialogBox
    {
        private const int CHARACTERS_PER_LINE = 45;
        private const int MAX_LINES = 6;
        private string[] _dialogWords;
        private int _wordIndex = 0;
        private Vector2 _scrollMiddle;
        private List<IEntity> _currentPageText;
        private IEntity _dialogScrollEntity;

        public DialogBox(string dialog)
        {
            _currentPageText = new List<IEntity>();
            _dialogWords = dialog.Split(' ');

            // Spawn the Frame
            Sprite dialogScroll = Assets.Asset.GetSprite("GUI.Dialog.png", SpriteLayer.ScreenGuiBackground, true);
            _scrollMiddle = new Vector2(dialogScroll.Width / 2f, dialogScroll.Height / 2f);
            _dialogScrollEntity = new Entity(new Transform(Vector2.Zero - (_scrollMiddle * 0.5f), 0.5f, 0f), "dialogScroll");
            _dialogScrollEntity.AddComponent(dialogScroll);
            Manager.GetInstance().AddEntity(_dialogScrollEntity);

            // Spawn the Dialog Letters
            SpawnLettersForThisPage();
        }

        public bool NextPage()
        {
            // No more pages! - if the word index is at its end
            if (_wordIndex >= _dialogWords.Length)
                return false;

            ClearPageText();
            SpawnLettersForThisPage();
            return true;
        }

        public void Close()
        {
            ClearPageText();
            Manager.GetInstance().RemoveEntity(_dialogScrollEntity);
        }

        private void SpawnLettersForThisPage()
        {
            string lettersForThisPage = string.Empty;
            int lineCharacterCounter = 0;
            int lineCounter = 0;
            int currentWordIndex;
            for (currentWordIndex = _wordIndex; currentWordIndex < _dialogWords.Length; currentWordIndex++)
            {
                // Next word would be too big, go to next line
                if (lineCharacterCounter + _dialogWords[currentWordIndex].Length > CHARACTERS_PER_LINE)
                {
                    lineCharacterCounter = 0;
                    lettersForThisPage += '\n';
                    lineCounter++;
                }

                // End of page is reached
                if (lineCounter > MAX_LINES)
                {
                    break;
                }

                // Add the Word with a space to the string
                lettersForThisPage += _dialogWords[currentWordIndex] + " ";
                lineCharacterCounter += _dialogWords[currentWordIndex].Length + 1;
            }

            // Remember the word index for the next page
            _wordIndex = currentWordIndex;

            Text text = new Text(lettersForThisPage, new Vector2(-6.35f, -1.30f), "Alagard_Thin", 0.1f, 0f, 0.3f);
            _currentPageText.AddRange(text.Letters);
            Manager.GetInstance().AddEntities(text.Letters);
        }

        private void ClearPageText()
        {
            if (_currentPageText.Count > 0)
            {
                Manager.GetInstance().RemoveEntities(_currentPageText);
                _currentPageText.Clear();
            }
        }
    }
}
