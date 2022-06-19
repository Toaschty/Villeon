using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using TiledLib;
using TiledLib.Layer;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Utils;

namespace Villeon.Generation
{
    public static class TileMapBuilder
    {
        // This list will be return as it contains all needed entities of this tilemap
        private static List<IEntity> _entities = new List<IEntity>();

        // Array is used for the collider optimization
        private static bool[,] _colliderGrid = new bool[1, 1];

        // Holds the height / width of current tilemap
        private static int _width;
        private static int _height;

        // EntitySpawner
        private static EnemyBuilder _spawner = new EnemyBuilder();

        // Generate entities depending on the tilemap
        public static List<IEntity> GenerateEntitiesFromTileMap(TileMapDictionary tileMap, bool collisionOptimization)
        {
            // Clear existing entities
            _entities = new List<IEntity>();

            // Set height / width
            _width = tileMap.Map.Width;
            _height = tileMap.Map.Height;

            // Initialize collider grid
            _colliderGrid = new bool[_width, _height];

            // Go through all layers of the tilemap
            foreach (TileLayer layer in tileMap.Map.Layers.OfType<TileLayer>())
            {
                for (int y = 0, i = 0; y < layer.Height; y++)
                {
                    for (int x = 0; x < layer.Width; x++, i++)
                    {
                        // Get gid of current tile
                        uint gid = (uint)layer.Data[i];

                        // If gid is 0 -> Skip tile because it is space / air
                        if (gid == 0)
                            continue;

                        // If current tile is ladder -> Spawn trigger
                        if (gid == 4 || gid == 5)
                        {
                            IEntity ladder = new Entity(new Transform(new Vector2(x, layer.Height - 1 - y), 1f, 0), "Ladder");
                            ladder.AddComponent(new Trigger(TriggerLayerType.LADDER, 1, 1f));
                            ladder.AddComponent(new Ladder());
                            _entities.Add(ladder);
                        }

                        // If current tile is enemy spawn -> Spawn enemy
                        if (gid == 8)
                        {
                            EnemySpawner.Spawn("DungeonScene", "slime", new Vector2(x, layer.Height - 1 - y), new Vector2(1f));
                        }

                        if (gid == 32)
                        {
                            EnemySpawner.Spawn("DungeonScene", "slime", new Vector2(x, layer.Height - 1 - y), new Vector2(5f));
                        }

                        // Get tile from dicitionary with gid
                        Components.Tile currentTile = tileMap.Tiles[gid];

                        // Create new entity for the upcoming tile
                        IEntity tileEntity = new Entity(new Transform(new Vector2(x, layer.Height - 1 - y), 1, 1), "Tile");

                        // Setup Sprite Component
                        Sprite tileSprite = new Sprite(currentTile, currentTile.TexCoords);

                        // Make sprite dynamic in order to get updated after frame changes
                        tileEntity.AddComponent(tileSprite);

                        // Add animation component if tile contains any frames
                        if (currentTile.AnimationFrames.Count() != 0)
                        {
                            // Dynamic: updated after frame changes
                            tileSprite.IsDynamic = true;

                            // Create new animationController component for current tile
                            AnimationController animController = new AnimationController();

                            // Create new animation for tile
                            Animation animation = new Animation(currentTile.FrameDuration);

                            // Add each animationtile as sprite to frames
                            foreach (Components.Tile frameTile in currentTile.AnimationFrames)
                            {
                                animation.AnimationSprite.Add(new Sprite(frameTile, frameTile.TexCoords));
                            }

                            animController.AddAnimation(animation);

                            // Add animation component
                            tileEntity.AddComponent(animController);
                        }

                        // Convert current tile into Sprite
                        _entities.Add(tileEntity);

                        // If collider optimization is turned on and the current tile contains at least 1 collider..
                        if (collisionOptimization && currentTile.Colliders.Count > 0)
                        {
                            // ... set current position in grid to true
                            _colliderGrid[x, y] = true;
                        }

                        // If collider optimization is turned on and the current tile contains at least 1 collider..
                        if (!collisionOptimization && currentTile.Colliders.Count > 0)
                        {
                            // ... go trough all colliders
                            foreach (var collider in currentTile.Colliders)
                            {
                                // Create new entity which holds a collider with given data and add it to entities
                                Vector2 currentPos = new Vector2(x, layer.Height - 1 - y) + collider.Min;
                                IEntity collisionEntity = new Entity(new Transform(currentPos, 1f, 1f), "CollisionBox");
                                collisionEntity.AddComponent(new Collider(new Vector2(0, 0), currentPos, collider.Size.X, collider.Size.Y));
                                _entities.Add(collisionEntity);
                            }
                        }
                    }
                }
            }

            // If collider optimization was turned on during the previous step...
            if (collisionOptimization)
            {
                // .. add the colliders
                AddOptimizedColliders();
            }

            // Return the list of entities
            return _entities;
        }

        // Calculate optimized Colliders
        private static void AddOptimizedColliders()
        {
            // Create unionized colliders in Grid
            Vector2 min = new(-1, -1);
            Vector2 max = new(-1, -1);

            // Go trough all positions in the collidergrid
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    // Startposition already found -> Increase width by one
                    if (_colliderGrid[x, y] && min.X != -1)
                    {
                        max.X = x;
                        _colliderGrid[x, y] = false;
                    }

                    // Find startposition
                    if (_colliderGrid[x, y] && min.X == -1)
                    {
                        min.X = x;
                        min.Y = y;
                        max.X = x;
                        max.Y = y;
                        _colliderGrid[x, y] = false;
                    }
                    else if (min.X != -1 && x + 1 == _width || !_colliderGrid[(x + 1) % _width, y])
                    {
                        for (int y2 = (int)min.Y + 1; y2 <= _height && min.X != -1; y2++)
                        {
                            bool completeLine = true;
                            if (y2 != _height)
                            {
                                for (int x2 = (int)min.X; x2 <= (int)max.X && completeLine; x2++)
                                {
                                    if (!_colliderGrid[x2, y2])
                                        completeLine = false;
                                }
                            }
                            else
                            {
                                completeLine = false;
                            }

                            if (completeLine)
                            {
                                max.Y++;
                                for (int x2 = (int)min.X; x2 <= (int)max.X; x2++)
                                {
                                    _colliderGrid[x2, y2] = false;
                                }
                            }
                            else
                            {
                                // Add collider entity to entities
                                IEntity entity = new Entity(new Transform(new Vector2(min.X, max.Y * -1 + _height - 1), 1f, 0), "Map");
                                entity.AddComponent(new Collider(new Vector2(0, 0), new Vector2(min.X, max.Y * -1 + _height - 1), max.X - min.X + 1, max.Y - min.Y + 1));
                                _entities.Add(entity);
                                min = new Vector2(-1, -1);
                                max = new Vector2(-1, -1);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
