using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.GUI
{
    public class DialogBox
    {
        private const int CHARACTERS_PER_LINE = 40;
        private const int MAX_LINES = 6;
        private Vector2 _scrollMiddle;

        private Dictionary<int, string[]> _pageToDialogWords;
        private int _pageNumber = 0;    // Current Page in the Dictionary
        private int _wordIndex = 0;     // Used if the words didn't fit on a page

        private List<IEntity> _currentPageText;
        private IEntity _dialogScrollEntity;

        public DialogBox(string[] pageLines)
        {
            _currentPageText = new List<IEntity>();

            // Fill the Page to DialogWords dictionary
            _pageToDialogWords = new Dictionary<int, string[]>();
            for (int i = 0; i < pageLines.Length; i++)
            {
                _pageToDialogWords.Add(i, pageLines[i].Split(' '));
            }

            // Spawn the Frame
            Sprite dialogScroll = Assets.Asset.GetSprite("GUI.Dialog.png", SpriteLayer.ScreenGuiBackground, true);
            _scrollMiddle = new Vector2(dialogScroll.Width / 2f, dialogScroll.Height / 2f);
            _dialogScrollEntity = new Entity(new Transform(Vector2.Zero - (_scrollMiddle * 0.5f), 0.5f, 0f), "dialogScroll");
            _dialogScrollEntity.AddComponent(dialogScroll);
            Manager.GetInstance().AddEntity(_dialogScrollEntity);

            // Spawn the Dialog Letters
            SpawnPageLetters();
        }

        public bool NextPage()
        {
            // No more pages! - all lines are written!
            if (_pageNumber >= _pageToDialogWords.Count)
                return false;

            ClearPageText();
            SpawnPageLetters();
            return true;
        }

        public void Close()
        {
            ClearPageText();
            Manager.GetInstance().RemoveEntity(_dialogScrollEntity);
        }

        private void SpawnPageLetters()
        {
            if (SpawnLettersForThisPage(_pageNumber, ref _wordIndex) is true)
            {
                _pageNumber++;
                _wordIndex = 0;
            }
        }

        private bool SpawnLettersForThisPage(int pageNr, ref int currentWordIndex)
        {
            bool pageLinesFit = true;

            string[] wordsForThisPage = _pageToDialogWords[pageNr];
            string textForThisPage = string.Empty;
            int lineCharacterCounter = 0;
            int lineCounter = 0;
            int i;
            for (i = currentWordIndex; i < wordsForThisPage.Length; i++)
            {
                // Next word would be too big, go to next line
                if (lineCharacterCounter + wordsForThisPage[i].Length > CHARACTERS_PER_LINE)
                {
                    lineCharacterCounter = 0;
                    textForThisPage += '\n';
                    lineCounter++;
                }

                // End of page is reached
                if (lineCounter > MAX_LINES)
                {
                    pageLinesFit = false;
                    break;
                }

                // Add the Word with a space to the string
                textForThisPage += wordsForThisPage[i] + " ";
                lineCharacterCounter += wordsForThisPage[i].Length + 1;
            }

            // Remember the word index for the next page
            currentWordIndex = i;

            // Spawn the Text for the current page
            Text text = new Text(textForThisPage, new Vector2(-5.65f, -1.40f), "Alagard_Thin", 0.1f, 0f, 0.3f);
            _currentPageText.AddRange(text.Letters);
            Manager.GetInstance().AddEntities(text.GetEntities());

            return pageLinesFit;
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
