namespace Villeon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;
    using TiledLib.Layer;
    using Villeon.Components;
    using Villeon.Helper;
    using Zenseless.OpenTK;
    using static Villeon.Components.Tile;
    using Villeon.ECS;

    public class TileMap
    {
        // Holds all tileMap data
        private readonly TiledLib.Map _map;

        // Dictionary tileId => Tile object
        public Dictionary<uint, Villeon.Components.Tile> _tiles = new ();
        public Dictionary<uint, Villeon.Components.AnimatedTile> _animTiles = new ();

        // Used for generating optimized colliders
        private bool[,] _colliderGrid;

        private bool _collisionOptimization = false;

        // XML
        private XmlDocument? _tileSetReader;

        public TileMap(string mapName, bool enableCollisionOptimization)
        {
            // Load in tilemap from file + TileSets
            _map = TiledLib.Map.FromStream(ResourceLoader.LoadContentAsStream("TileMap." + mapName), ts => ResourceLoader.LoadContentAsStream("TileMap." + ts.Source));

            _colliderGrid = new bool[_map.Width, _map.Height];
            _collisionOptimization = enableCollisionOptimization;

            // Starting generation
            SetupTileDictionary();
        }

        public List<IEntity> Entities { get; set; } = new List<IEntity>();

        // Read collisions from tilemap
        public void GetAdditionalData()
        {
            // Go through all tilesets
            foreach (var tileSet in _map.Tilesets)
            {
                //_tileSetReader = XmlReader.Create(ResourceLoader.LoadContentAsStream("TileMap.Tilesets." + tileSet.Name + ".tsx"));
                _tileSetReader = new XmlDocument();
                _tileSetReader.Load(ResourceLoader.LoadContentAsStream("TileMap.Tilesets." + tileSet.Name + ".tsx"));

                XmlNodeList tileNodes = _tileSetReader.SelectNodes("tileset/tile");
                foreach (XmlNode tileNode in tileNodes)
                {
                    uint tileId = uint.Parse(tileNode.Attributes["id"].Value);
                    Tile currentTile = _tiles[tileId + 1];

                    foreach (XmlNode child in tileNode.ChildNodes)
                    {
                        if (child.Name == "objectgroup")
                        {
                            GetCollisionData(child, tileId, tileSet);
                        }

                        if (child.Name == "animation")
                        {
                            GetAnimationData(child, tileId);
                        }
                    }
                }
                // Go through all tiles in file
                /*
                while (_tileSetReader.ReadToFollowing("tile"))
                {
                    // Read id of tile
                    _tileSetReader.MoveToAttribute("id");
                    uint tileId = uint.Parse(_tileSetReader.Value);

                    // Load tile from dictionary with id
                    Tile currentTile = _tiles[tileId + 1];

                    // Open element "objectgroup" as new XMLReader
                    if (_tileSetReader.ReadToDescendant("objectgroup"))
                    {
                        GetCollisionData(tileId, tileSet);
                    }

                    // Go through all collisions from a tile
                    if (_tileSetReader.ReadToDescendant("animation"))
                    {
                        GetAnimationData(tileId);
                    }
                }
                */
            }
        }

        public void GetCollisionData(XmlNode child, uint tileId, TiledLib.ITileset tileSet)
        {
            //XmlReader collisionReader = _tileSetReader.ReadSubtree();
            XmlNodeList collisions = child.ChildNodes;
            foreach (XmlNode collision in collisions)
            {
                Vector2 offset = new();
                offset.X = float.Parse(collision.Attributes["x"].Value);
                offset.Y = float.Parse(collision.Attributes["y"].Value);

                string[] coords = collision.ChildNodes[0].Attributes["points"].Value.Split(' ', ',');

                Vector2 minCoords = new();
                minCoords.X = float.Parse(coords[0]);
                minCoords.Y = float.Parse(coords[1]);

                Vector2 maxCoords = new();
                maxCoords.X = float.Parse(coords[4]);
                maxCoords.Y = float.Parse(coords[5]);

                // Flip Y axis for proper drawing (Coordinate system origin different)
                offset.Y = tileSet.TileHeight - offset.Y;
                maxCoords.Y = -maxCoords.Y;

                // Create collider and add it to the loaded tile
                // Devide by tile width to get coordinates inside of tile, e.g. (4, 12) -> (0.25, 0.75)
                Box2 collider = new Box2((minCoords + offset) / tileSet.TileWidth, (maxCoords + offset) / tileSet.TileWidth);
                _tiles[tileId + 1].Colliders.Add(collider);
            }
            /*
            while (collisionReader.ReadToFollowing("object"))
            {
                // // Calculate offset
                Vector2 offset = new();

                // Get x from reader
                _tileSetReader.MoveToAttribute("x");
                offset.X = float.Parse(_tileSetReader.Value);

                // Get y from reader
                _tileSetReader.MoveToAttribute("y");
                offset.Y = float.Parse(_tileSetReader.Value);

                // // Calculate size of collider
                // Move to collision coordinates within tile
                _tileSetReader.ReadToFollowing("polygon");
                _tileSetReader.MoveToAttribute("points");

                // Get coordinates by splitting string
                string[] coords = _tileSetReader.Value.Split(' ', ',');

                // Create collider bounds
                Vector2 minCoords = new();
                minCoords.X = float.Parse(coords[0]);
                minCoords.Y = float.Parse(coords[1]);

                Vector2 maxCoords = new();
                maxCoords.X = float.Parse(coords[4]);
                maxCoords.Y = float.Parse(coords[5]);

                // Flip Y axis for proper drawing (Coordinate system origin different)
                offset.Y = tileSet.TileHeight - offset.Y;
                maxCoords.Y = -maxCoords.Y;

                // Create collider and add it to the loaded tile
                // Devide by tile width to get coordinates inside of tile, e.g. (4, 12) -> (0.25, 0.75)
                Box2 collider = new Box2((minCoords + offset) / tileSet.TileWidth, (maxCoords + offset) / tileSet.TileWidth);
                _tiles[tileId + 1].Colliders.Add(collider);
            }
            */
        }

        public void GetAnimationData(XmlNode child, uint tileId)
        {
            XmlNodeList frames = child.ChildNodes;

            Tile tile = _tiles[tileId];
            AnimatedTile animTile = new AnimatedTile(tile._x, tile._y, tile.TileSet, tile.Colliders, tile.Texture2D);

            foreach (XmlNode frame in frames)
            {
                int frameId = int.Parse(frame.Attributes["tileid"].Value);
                animTile.AnimationFrames.Add(frameId);

                animTile.FrameDuration += float.Parse(frame.Attributes["duration"].Value) / 1000;
            }

            animTile.FrameDuration /= animTile.AnimationFrames.Count;
            _animTiles.Add(tileId + 1, animTile);
            /*
            XmlReader animationReader = _tileSetReader.ReadSubtree();

            Tile tile = _tiles[tileId];
            AnimatedTile animTile = new AnimatedTile(tile._x, tile._y, tile.TileSet, tile.Colliders);

            while (animationReader.ReadToFollowing("frame"))
            {
                animationReader.MoveToAttribute("tileid");
                int frameId = int.Parse(animationReader.Value);

                animTile.AnimationFrames.Add(frameId);

                animationReader.MoveToAttribute("duration");
                animTile.FrameDuration += float.Parse(animationReader.Value) / 1000;
            }

            animTile.FrameDuration /= animTile.AnimationFrames.Count;
            _animTiles.Add(tileId + 1, animTile);
            */
        }

        // Generate tile-entitys depending on the tilemap.
        public void CreateTileMapEntitys()
        {
            // Go through all layers
            foreach (TileLayer layer in _map.Layers.OfType<TileLayer>())
            {
                for (int y = 0, i = 0; y < layer.Height; y++)
                {
                    for (int x = 0; x < layer.Width; x++, i++)
                    {
                        // Get id of current cell
                        uint gid = (uint)layer.Data[i];

                        // No tile -> Skip this tile
                        if (gid == 0)
                            continue;

                        // Generate Entity
                        IEntity entity = new Entity(new Transform(new Vector2(x, layer.Height - 1 - y), 1, 1), "Tile");

                        // Search for tile in dictionary and set coordinates
                        Tile currentTile;
                        if (_animTiles.ContainsKey(gid))
                        {
                            AnimatedTile tile = (AnimatedTile)_animTiles[gid].Clone();
                            tile.Position = new Vector2(x, layer.Height - 1 - y);
                            //entity.AddComponent(tile);

                            currentTile = (Tile)tile;
                        }
                        else
                        {
                            Tile tile = (Tile)_tiles[gid].Clone();
                            tile.Position = new Vector2(x, layer.Height - 1 - y);
                            //entity.AddComponent(tile);

                            currentTile = (Tile)tile;
                        }

                        Sprite sprite = currentTile.ToSprite(currentTile);
                        entity.AddComponent(sprite);
                        Entities.Add(entity);

                        // Adjust colliderGrid if collisionOptimization is turned on and the tile has at least one collision
                        if (_collisionOptimization && currentTile.Colliders.Count > 0)
                            _colliderGrid[x, y] = true;

                        // Generate collision entities
                        if (!_collisionOptimization)
                        {
                            // Check if current tile has colliders
                            if (currentTile.Colliders.Count > 0)
                            {
                                // Go through all colliders
                                foreach (var collider in currentTile.Colliders)
                                {
                                    // Generate new entity for collision
                                    IEntity collisionEntity = new Entity(new Transform(new Vector2(x, layer.Height - 1 - y) + collider.Min, collider.Size.X, collider.Size.Y), "CollisonTile");
                                    collisionEntity.AddComponent(new Collider(new Vector2(0, 0), new Vector2(x, layer.Height - 1 - y) + collider.Min, collider.Size.X, collider.Size.Y));
                                    Entities.Add(collisionEntity);
                                }
                            }
                        }
                    }
                }
            }

            // Add optimized colliders if turned on
            if (_collisionOptimization)
                AddOptimizedColliders();
        }

        // Load in tileSet texture. Set settings for texture. Tile width and height calculation for tileSet.
        public TileSetStruct LoadTileSet(string imagePath, uint columns, uint rows)
        {
            Texture2D tileSetTexture = Assets.GetTexture(imagePath);
            //Texture2D tileSetTexture = ResourceLoader.LoadContentAsTexture2D(imagePath);
            //GL.BindTexture(TextureTarget.Texture2D, tileSetTexture.Handle);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)OpenTK.Graphics.OpenGL.TextureMagFilter.Nearest);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)OpenTK.Graphics.OpenGL.TextureMinFilter.Nearest);
            //GL.BindTexture(TextureTarget.Texture2D, 0);

            Vector2 delta = new Vector2(0.5f / tileSetTexture.Width, 0.5f / tileSetTexture.Height);

            return new TileSetStruct()
            {
                Texture2D = tileSetTexture.Handle,
                TileWidth = 1f / columns,
                TileHeight = 1f / rows,
                Delta = delta,
            };
        }

        // Start setup prozess. Fill dictionary with all tiles.
        private void SetupTileDictionary()
        {
            // Go through all tilesets
            foreach (var tileSet in _map.Tilesets)
            {
                float rezImageWidth = 1f / tileSet.ImageWidth;
                float rezImageHeight = 1f / tileSet.ImageHeight;

                // Load in corresponding tileset
                TileSetStruct graphicsTileSet = LoadTileSet("TileMap." + tileSet.ImagePath, (uint)tileSet.Columns, (uint)tileSet.Rows);
                Texture2D texture = Assets.GetTexture("TileMap." + tileSet.ImagePath);

                // Go through all tiles in tileset -> Safe all tiles in dictionary
                for (int gid = tileSet.FirstGid; gid < tileSet.FirstGid + tileSet.TileCount; gid++)
                {
                    TiledLib.Tile tile = tileSet[gid];

                    Tile currentTile = new Tile(tile.Left * rezImageWidth, (tileSet.ImageHeight - tileSet.TileHeight - tile.Top) * rezImageHeight, graphicsTileSet, texture);

                    // Gid starts with 1. 0 = No tile in current position on layer
                    _tiles.Add((uint)gid, currentTile);
                }
            }

            // If optimization is turn on => Generate collisions | Turned off => Load collisions from tilemap
            GetAdditionalData();
            if (!_collisionOptimization)
                AddOptimizedColliders();
        }

        // Add optimized colliders to tiles.
        private void AddOptimizedColliders()
        {
            // create unionized colliders in Grid
            Vector2 min = new (-1, -1);
            Vector2 max = new (-1, -1);
            for (int y = 0; y < _map.Height; y++)
            {
                for (int x = 0; x < _map.Width; x++)
                {
                    if (_colliderGrid[x, y] && min.X != -1)
                    {
                        max.X = x;
                        _colliderGrid[x, y] = false;
                    }

                    if (_colliderGrid[x, y] && min.X == -1)
                    {
                        min.X = x;
                        min.Y = y;
                        max.X = x;
                        max.Y = y;
                        _colliderGrid[x, y] = false;
                    }
                    else if ((min.X != -1 && ((x + 1) == _map.Width)) || !_colliderGrid[(x + 1) % _map.Width, y])
                    {
                        for (int y2 = (int)min.Y + 1; y2 <= _map.Height && min.X != -1; y2++)
                        {
                            bool completeLine = true;
                            if (y2 != _map.Height)
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
                                IEntity entity = new Entity(new Transform(new Vector2(min.X, (max.Y * -1) + _map.Height - 1), 0, 0), "Map");
                                entity.AddComponent(new Collider(new Vector2(0, 0), new Vector2(min.X, (max.Y * -1) + _map.Height - 1), max.X - min.X + 1, max.Y - min.Y + 1));
                                Entities.Add(entity);
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
