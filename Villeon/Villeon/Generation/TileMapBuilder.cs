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
using Villeon.Helper;
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

                        SpawnLadders(gid, x, y);
                        SpawnLights(tileMap.MapName, gid, x, y);

                        // Get tile from dicitionary with gid
                        Components.Tile currentTile = tileMap.Tiles[gid];

                        // Create new entity for the upcoming tile
                        IEntity tileEntity = new Entity(new Transform(new Vector2(x, layer.Height - 1 - y), 1, 1), "Tile");

                        // Setup Sprite Component
                        Sprite tileSprite = new Sprite(currentTile, currentTile.TexCoords);

                        // Make sprite dynamic in order to get updated after frame changes
                        tileEntity.AddComponent(tileSprite);

                        // Add animation component if tile contains any frames
                        AddAnimatedTiles(currentTile, tileSprite, tileEntity);

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

        // Generate entities depending on the tilemap
        public static List<IEntity> GenerateEntitiesFromArray(int[,] map, TileMapDictionary tileMap, bool collisionOptimization)
        {
            // Clear existing entities
            _entities = new List<IEntity>();

            // Set height / width
            _width = map.GetLength(1);
            _height = map.GetLength(0);

            // Initialize collider grid
            _colliderGrid = new bool[_width, _height];

            for (int y = 0, i = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++, i++)
                {
                    // Get gid of current tile
                    uint gid = (uint)map[y, x];

                    // If gid is 0 -> Skip tile because it is space / air
                    if (gid == 0)
                        continue;

                    SpawnLadders(gid, x, y);
                    SpawnLights("Dungeon", gid, x, y);
                    SpawnPortal(gid, x, y);
                    SpawnEnemies(gid, x, y);

                    // Get tile from dicitionary with gid
                    Components.Tile currentTile = tileMap.Tiles[gid];

                    // Create new entity for the upcoming tile
                    IEntity tileEntity = new Entity(new Transform(new Vector2(x, _height - 1 - y), 1, 1), "Tile");

                    // Setup Sprite Component
                    Sprite tileSprite = new Sprite(currentTile, currentTile.TexCoords);

                    // Make sprite dynamic in order to get updated after frame changes
                    tileEntity.AddComponent(tileSprite);

                    AddAnimatedTiles(currentTile, tileSprite, tileEntity);

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
                            Vector2 currentPos = new Vector2(x, _height - 1 - y) + collider.Min;
                            IEntity collisionEntity = new Entity(new Transform(currentPos, 1f, 1f), "CollisionBox");
                            collisionEntity.AddComponent(new Collider(new Vector2(0, 0), currentPos, collider.Size.X, collider.Size.Y));
                            _entities.Add(collisionEntity);
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
            Vector2 min = new (-1, -1);
            Vector2 max = new (-1, -1);

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

                    if ((min.X != -1 && x + 1 == _width) || !_colliderGrid[(x + 1) % _width, y])
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
                                IEntity entity = new Entity(new Transform(new Vector2(min.X, (max.Y * -1) + _height - 1), 1f, 0), "Map");
                                entity.AddComponent(new Collider(new Vector2(0, 0), new Vector2(min.X, (max.Y * -1) + _height - 1), max.X - min.X + 1, max.Y - min.Y + 1));
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

        private static void AddAnimatedTiles(Components.Tile currentTile, Sprite tileSprite, IEntity tileEntity)
        {
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
        }

        private static void SpawnLadders(uint gid, int x, int y)
        {
            // If current tile is ladder -> Spawn trigger
            if (gid == 4 || gid == 5)
            {
                IEntity ladder = new Entity(new Transform(new Vector2(x, _height - 1 - y), 1f, 0), "Ladder");
                ladder.AddComponent(new Trigger(TriggerLayerType.LADDER, 1, 1f));
                ladder.AddComponent(new Ladder());
                _entities.Add(ladder);
            }
        }

        private static void SpawnLights(string tileSetName, uint gid, int x, int y)
        {
            // If current tile is torch -> Spawn light
            // GIDs for village tileset
            if (tileSetName.Equals("Village.tmx") && (gid == 592 || gid == 593))
            {
                SpawnLight(x, y);
                return;
            }

            // GIDs for shop/smith tileset
            if ((tileSetName.Equals("VillageShop.tmx") || tileSetName.Equals("VillageSmith.tmx")) && (gid == 101 || gid == 217 || gid == 159))
            {
                SpawnLight(x, y);
                return;
            }

            // GIDs for dungeon tileset
            if (gid == 9)
                SpawnLight(x, y);
        }

        private static void SpawnLight(int x, int y)
        {
            IEntity torch = new Entity(new Transform(new Vector2(x + 0.5f, _height - 0.5f - y), 1f, 0), "Torch");
            torch.AddComponent(new Light(new Color4(255, 50, 50, 255), -13.5f, 4f, 1f, 0.7f, 1.8f));

            // Add particle spawner for the flakes
            ParticleSpawner particleSpawner = new ParticleSpawner(2, "Sprites.Particles.Smoke.png");
            particleSpawner.ParticleWeight = -0.1f;
            torch.AddComponent(particleSpawner);
            _entities.Add(torch);
        }

        private static void SpawnPortal(uint gid, int x, int y)
        {
            // Add portal glow
            if (gid == 292)
            {
                IEntity glow = new Entity(new Transform(new Vector2(x, _height - 1f - y), 1f, 0), "Glow");
                glow.AddComponent(new Light(new Color4(237, 0, 134, 255), -12f, 20f, 1f, 0.7f, 1.8f));
                ParticleSpawner particleSpawner = new ParticleSpawner(50, "Sprites.Particles.PortalDust.png");
                particleSpawner.VariationWidth = 2;
                particleSpawner.VariationHeight = 3;
                glow.AddComponent(particleSpawner);
                _entities.Add(glow);
            }

            // Add back portal
            if (gid == 368)
            {
                // Add Boss Portal
                IEntity portalTrigger = new Entity(new Transform(new Vector2(x, _height - y), 1f, 0f), "Portal Trigger");
                portalTrigger.AddComponent(new Trigger(TriggerLayerType.PORTAL, 4f, 5f));
                portalTrigger.AddComponent(new Portal("BossScene", Constants.DUNGEON_SPAWN_POINT));
                portalTrigger.AddComponent(new Interactable(new Option("Enter Boss Room [E]", OpenTK.Windowing.GraphicsLibraryFramework.Keys.E)));
                _entities.Add(portalTrigger);
            }
        }

        private static void SpawnEnemies(uint gid, int x, int y)
        {
            // If current tile is enemy spawn -> Spawn enemy
            Random random = new Random();
            if (gid == 33)
            {
                int type = random.Next(0, 3);
                if (type == 0)
                {
                    switch (random.Next(0, 4))
                    {
                        case 0: EnemySpawner.SpawnEnemy("DungeonScene", "slime_blue", new Vector2(x, _height - 1 - y)); break;

                        //case 1: EnemySpawner.Spawn("DungeonScene", "slime_magenta", new Vector2(x, _height - 1 - y)); break;
                        //case 2: EnemySpawner.Spawn("DungeonScene", "slime_green", new Vector2(x, _height - 1 - y)); break;
                        //case 3: EnemySpawner.Spawn("DungeonScene", "slime_red", new Vector2(x, _height - 1 - y)); break;
                    }
                }
                else if (type == 1)
                {
                    EnemySpawner.SpawnEnemy("DungeonScene", "bubble", new Vector2(x, _height - 1 - y));
                }
                else
                {
                    EnemySpawner.SpawnEnemy("DungeonScene", "bat", new Vector2(x, _height - 1 - y));
                }
            }
        }
    }
}
