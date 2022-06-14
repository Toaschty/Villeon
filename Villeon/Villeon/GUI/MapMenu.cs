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
    public class MapMenu : IGUIMenu
    {
        private List<Entity> _entities;

        private float _letterScale = 0.2f;

        private float _minZoom = 0.1f;
        private float _maxZoom = 1f;
        private float _currentZoom = 0.3f;
        private float _zoomStep = 0.1f;
        private float _moveStep = 0.08f;

        private Sprite _mapSprite;
        private Entity _markerEntity;
        private Sprite _playerMarker;

        private Vector2 _viewportMin = new Vector2(0f, 0f);
        private Vector2 _viewportMax = new Vector2(0.3f, 0.3f);
        private Vector2 _viewportPosition = new Vector2(0f, 0f);

        private Vector2 _playerMarkerPos = new Vector2(0f, 0f);
        private Vector2 _playerMarkerPosNorm = Vector2.Zero;

        public MapMenu()
        {
            // Create Map layout
            _entities = new List<Entity>();

            // Load sprites
            Sprite backgroundScrollSprite = Asset.GetSprite("GUI.Scroll.png", SpriteLayer.ScreenGuiBackground, false);
            _playerMarker = Asset.GetSprite("GUI.Map_Marker.png", SpriteLayer.ScreenGuiForeground, false);
            _mapSprite = Asset.GetSprite("GUI.Scroll_Map.png", SpriteLayer.ScreenGuiMiddleground, true);

            // Background
            Vector2 scrollMiddle = new Vector2(backgroundScrollSprite.Width / 2f, backgroundScrollSprite.Height / 2f);
            Entity backgroundImage = new Entity(new Transform(Vector2.Zero - (scrollMiddle * 0.5f), 0.5f, 0f), "BackgroundImage");
            backgroundImage.AddComponent(backgroundScrollSprite);
            _entities.Add(backgroundImage);

            // Map
            Vector2 mapMiddle = new Vector2(_mapSprite.Width / 2f, _mapSprite.Height / 2f);
            Entity mapEntity = new Entity(new Transform(Vector2.Zero - (mapMiddle * 0.085f), new Vector2(0.085f, 0.085f), 0), "Map");
            mapEntity.AddComponent(_mapSprite);
            _entities.Add(mapEntity);

            // Marker
            _markerEntity = new Entity(new Transform(_playerMarkerPos, 0.3f, 0), "Marker");
            _markerEntity.AddComponent(_playerMarker);
            _entities.Add(_markerEntity);

            UpdatePosition();

            // Control Text
            Text controlText = new Text("Move [W A S D] Zoom [R F]", new Vector2(-6f, -4.5f), "Alagard", 0f, 3f, _letterScale);
            Array.ForEach(controlText.GetEntities(), entity => _entities.Add(entity));
        }

        public IEntity[] GetEntities()
        {
            return _entities.ToArray();
        }

        public bool OnKeyReleased(Keys key)
        {
            // Handle zoom
            if (key == Keys.R)
            {
                _currentZoom -= _zoomStep;

                if (_currentZoom < _minZoom)
                    _currentZoom = _minZoom;

                UpdatePosition();
            }

            if (key == Keys.F)
            {
                _currentZoom += _zoomStep;

                if (_currentZoom > _maxZoom)
                    _currentZoom = _maxZoom;

                UpdatePosition();
            }

            // Handle movement
            if (key == Keys.W)
            {
                _viewportPosition += new Vector2(0, _moveStep * _currentZoom);
                UpdatePosition();
            }

            if (key == Keys.S)
            {
                _viewportPosition -= new Vector2(0, _moveStep * _currentZoom);
                UpdatePosition();
            }

            if (key == Keys.A)
            {
                _viewportPosition -= new Vector2(_moveStep * _currentZoom, 0);
                UpdatePosition();
            }

            if (key == Keys.D)
            {
                _viewportPosition += new Vector2(_moveStep * _currentZoom, 0);
                UpdatePosition();
            }

            // Update player marker
            UpdatePlayerMaker();

            return true;
        }

        public void MoveViewportToMarker()
        {
            // Update player marker to get current position
            UpdatePlayerMaker();

            // Set viewport position to marker position
            _viewportPosition = _playerMarkerPosNorm;

            // Update viewport
            UpdatePosition();

            // Update player marker again to get player marker back if gespawned
            UpdatePlayerMaker();
        }

        public void UpdatePlayerMaker()
        {
            Vector2 playerPos = Camera.TrackerPosition;

            // PlayerPos in relation to actual map size -> 0 - 1
            float playerXnorm = (playerPos.X - 16f) / 149f;
            float playerYnorm = (playerPos.Y - 20f) / 103f;

            // Save marker position (0 - 1) for later
            _playerMarkerPosNorm = new Vector2(playerXnorm, playerYnorm);

            // PlayerPos in relation to viewport
            float playerXviewportNorm = (playerXnorm - _viewportMin.X) / (_viewportMax.X - _viewportMin.X);
            float playerYviewportNorm = (playerYnorm - _viewportMin.Y) / (_viewportMax.Y - _viewportMin.Y);

            // Check if marker is inside boundaries
            if (playerXviewportNorm < 0 || playerYviewportNorm < 0 || playerXviewportNorm > 1 || playerYviewportNorm > 1)
            {
                // Remove marker if outside
                _entities.Remove(_markerEntity);
                Manager.GetInstance().RemoveEntity(_markerEntity);
            }
            else
            {
                // If marker inside -> Spawn marker if not already spawned
                if (!_entities.Contains(_markerEntity))
                    _entities.Add(_markerEntity);
            }

            // PlayerPos in relation to viewport to actual coordinates
            float playerX = _mapSprite.Width * 0.085f * playerXviewportNorm;
            float playerY = _mapSprite.Height * 0.085f * playerYviewportNorm;

            // Set position of marker
            _playerMarkerPos = new Vector2(playerX - ((_mapSprite.Width * 0.085f) / 2f), playerY - ((_mapSprite.Height * 0.085f) / 2f));

            // Move marker by one half of sprite width to be in the middle
            _playerMarkerPos -= new Vector2((_playerMarker.Width / 2f) * 0.3f, 0f);

            _markerEntity.GetComponent<Transform>().Position = _playerMarkerPos;
        }

        private void UpdatePosition()
        {
            // Change zoom
            _viewportMin = new Vector2(-_currentZoom / 2, -_currentZoom / 2);
            _viewportMax = new Vector2(_currentZoom / 2, _currentZoom / 2);

            // Change position
            _viewportMin += _viewportPosition;
            _viewportMax += _viewportPosition;

            // Keep viewport inside bounds
            RestrainViewport();

            // Apply changes
            _mapSprite.TexCoords = new Vector2[4]
            {
                new Vector2(_viewportMin.X, _viewportMin.Y),
                new Vector2(_viewportMax.X, _viewportMin.Y),
                new Vector2(_viewportMin.X, _viewportMax.Y),
                new Vector2(_viewportMax.X, _viewportMax.Y),
            };
        }

        // Keep viewport inside map
        private void RestrainViewport()
        {
            float diffx = 0;
            float diffy = 0;

            // Left
            if (_viewportMin.X < 0)
                diffx = -_viewportMin.X;

            // Bottom
            if (_viewportMin.Y < 0)
                diffy = -_viewportMin.Y;

            // Right
            if (_viewportMax.X > 1)
                diffx = 1 - _viewportMax.X;

            // Top
            if (_viewportMax.Y > 1)
                diffy = 1 - _viewportMax.Y;

            // Apply changes
            _viewportMin += new Vector2(diffx, diffy);
            _viewportMax += new Vector2(diffx, diffy);
            _viewportPosition += new Vector2(diffx, diffy);
        }
    }
}
