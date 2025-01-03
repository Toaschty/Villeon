﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Helper;
using Villeon.Utils;

namespace Villeon.GUI
{
    [Obsolete]
    public class DeprecatedTextBox
    {
        private List<IEntity> _letters;
        private IEntity _frame = new Entity("FrameEmpty");
        private IEntity _background = new Entity("Empty background!");
        private IEntity _boundEntity = new Entity("Nothing bound!");

        private string _text;
        private Vector2 _framePosition;
        private float _letterSpacing;
        private float _lineSpacing;
        private float _letterScale;

        private Vector2 _lastBoundPosition;

        private bool _hasBackground;
        private bool _onGUI;

        public DeprecatedTextBox(string text, Vector2 framePosition, bool hasBackground = true, bool onGUI = false, float letterSpacing = 1.1f, float lineSpacing = 1.1f, float letterScale = 1f)
        {
            _text = text;
            _framePosition = framePosition;
            _letterSpacing = letterSpacing;
            _lineSpacing = lineSpacing;
            _letterScale = letterScale;
            _hasBackground = hasBackground;
            _onGUI = onGUI;

            _letters = CreateLetters(text);

            if (_hasBackground)
            {
                _frame = CreateTextFrame(framePosition);
                _background = CreateBackground(framePosition);
            }

            SpawnTextBox();
        }

        public void Overwrite(string text)
        {
            RemoveAllText();
            _letters = CreateLetters(text);
            foreach (IEntity entity in _letters)
            {
                Manager.GetInstance().AddEntity(entity);
            }
        }

        public void RemoveAllText()
        {
            foreach (IEntity entity in _letters)
            {
                Manager.GetInstance().RemoveEntity(entity);
            }

            _letters.Clear();
        }

        public void BindPositionTo(IEntity entity)
        {
            _boundEntity = entity;
            _lastBoundPosition = entity.GetComponent<Transform>().Position;
        }

        public void Update()
        {
            if (_boundEntity is not null)
                MoveTextbox(_boundEntity.GetComponent<Transform>().Position);
        }

        private void MoveTextbox(Vector2 newBoundPosition)
        {
            Vector2 positionChange = newBoundPosition - _lastBoundPosition;

            // Move the textbox
            foreach (IEntity entity in _letters)
            {
                entity.GetComponent<Transform>().Position += positionChange;
            }

            _frame.GetComponent<Transform>().Position += positionChange;
            _background.GetComponent<Transform>().Position += positionChange;

            _lastBoundPosition = newBoundPosition;
        }

        private List<IEntity> CreateLetters(string text)
        {
            List<IEntity> letters = new List<IEntity>();

            // Fill in new Letters
            SpriteSheet fontSheet = Asset.GetSpriteSheet("Fonts.VilleonFont.png") !;
            float spriteWidth = fontSheet.SpriteWidth * _letterScale;
            float spriteHeight = fontSheet.SpriteHeight * _letterScale;
            Sprite letterSprite;

            Vector2 letterPosition = _framePosition;
            foreach (char c in text)
            {
                if (c == '\n')
                {
                    letterPosition.X = _framePosition.X;
                    letterPosition.Y -= spriteHeight + _lineSpacing;
                    continue;
                }

                // Get Letter sprite
                if (_onGUI)
                    letterSprite = fontSheet.GetSprite(c - ' ', SpriteLayer.ScreenGuiForeground, false);
                else
                    letterSprite = fontSheet.GetSprite(c - ' ', SpriteLayer.GUIMiddleground, true);

                // Create Entity with letter spacing, scale & sprite
                IEntity letterEntity = new Entity(new Transform(letterPosition, _letterScale, 0f), "TBX[" + c + "]");
                letterEntity.AddComponent(letterSprite);
                letters.Add(letterEntity);

                // Set position for next letter
                letterPosition.X += spriteWidth + _letterSpacing;
            }

            return letters;
        }

        private IEntity CreateTextFrame(Vector2 position)
        {
            position -= new Vector2(0.5f, 0.5f);
            float boxWidth = _text.Length * 1.1f;
            IEntity frame = new Entity(new Transform(position, 1f, 0f), "Frame");
            Sprite sprite = Asset.GetSprite("GUI.Frame.png", SpriteLayer.GUIForeground, true);
            frame.AddComponent(sprite);
            return frame;
        }

        private IEntity CreateBackground(Vector2 position)
        {
            float boxWidth = (_text.Length * 1.1f) + 4.7f;
            position -= new Vector2(0.5f, 0.5f);
            IEntity background = new Entity(new Transform(position, 1f, 0f), "Frame Background");
            Sprite sprite = new Sprite(SpriteLayer.GUIBackground, boxWidth, 1.8f, true);
            sprite.Color = Color4.Black;
            background.AddComponent(sprite);
            return background;
        }

        private void SpawnTextBox()
        {
            foreach (IEntity entity in _letters)
            {
                Manager.GetInstance().AddEntity(entity);
            }

            if (_hasBackground)
            {
                Manager.GetInstance().AddEntity(_frame);
                Manager.GetInstance().AddEntity(_background);
            }
        }
    }
}
