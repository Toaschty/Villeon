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

    public class TileMap
    {
        // Holds all tileMap data
        private readonly TiledLib.Map map;

        // Dictionary tileId => Tile object
        public Dictionary<uint, Villeon.Components.Tile> tiles = new();

        // Used for generating optimized colliders
        private bool[,] colliderGrid;

        private bool collisionOptimization = false;

        // XML
        XmlReader tileSetReader;

        public TileMap(string mapName, bool enableCollisionOptimization)
        {
            // Load in tilemap from file + TileSets 
            map = TiledLib.Map.FromStream(ResourceLoader.LoadContentAsStream("TileMap." + mapName), ts => ResourceLoader.LoadContentAsStream("TileMap." + ts.Source));
            
            colliderGrid = new bool[map.Width, map.Height];
            collisionOptimization = enableCollisionOptimization;

            // Starting generation
            SetupTileDictionary();
        }

        // Start setup prozess. Fill dictionary with all tiles.
        private void SetupTileDictionary()
        {
            // Go through all tilesets
            foreach (var tileSet in map.Tilesets)
            {
                float rezImageWidth = 1f / tileSet.ImageWidth;
                float rezImageHeight = 1f / tileSet.ImageHeight;
         
                // Load in corresponding tileset
                TileSetStruct graphicsTileSet = LoadTileSet("TileMap." + tileSet.ImagePath, (uint)tileSet.Columns, (uint)tileSet.Rows);

                // Go through all tiles in tileset -> Safe all tiles in dictionary
                for (int gid = tileSet.FirstGid; gid < tileSet.FirstGid + tileSet.TileCount; gid++)
                {
                    TiledLib.Tile tile = tileSet[gid];

                    Tile currentTile = new Tile(tile.Left * rezImageWidth, (tileSet.ImageHeight - tileSet.TileHeight - tile.Top) * rezImageHeight, graphicsTileSet);
                    
                    tiles.Add((uint)gid, currentTile);
                    // AddTileToDictionary(graphicsTileSet, (uint)gid, tile.Left * rezImageWidth, (tileSet.ImageHeight - tileSet.TileHeight - tile.Top) * rezImageHeight);
                }
            }

            // If optimization is turn on => Generate collisions | Turned off => Load collisions from tilemap
            GetCollisionBoxes();
            if (!collisionOptimization)
                AddOptimizedColliders();
        }

        // Read collisions from tilemap
        public void GetCollisionBoxes()
        {
            // Go through all tilesets
            foreach (var tileSet in map.Tilesets)
            {
                tileSetReader = XmlReader.Create(ResourceLoader.LoadContentAsStream("TileMap.Tilesets." + tileSet.Name + ".tsx"));

                // Go through all tiles in file
                while (tileSetReader.ReadToFollowing("tile"))
                {
                    // Read id of tile
                    tileSetReader.MoveToAttribute("id");
                    uint tileId = uint.Parse(tileSetReader.Value);

                    // Load tile from dictionary with id
                    Tile currentTile = tiles[tileId + 1];

                    // Open element "objectgroup" as new XMLReader
                    tileSetReader.ReadToFollowing("objectgroup");
                    XmlReader collisionReader = tileSetReader.ReadSubtree();
                    
                    // Go through all collisions from a tile
                    while( collisionReader.ReadToFollowing("object") )
                    {
                        // // Calculate offset
                        Vector2 offset = new Vector2();
                        // Get x from reader
                        tileSetReader.MoveToAttribute("x");
                        offset.X = float.Parse(tileSetReader.Value);

                        // Get y from reader
                        tileSetReader.MoveToAttribute("y");
                        offset.Y = float.Parse(tileSetReader.Value);

                        // // Calculate size of collider
                        // Move to collision coordinates within tile
                        tileSetReader.ReadToFollowing("polygon");
                        tileSetReader.MoveToAttribute("points");

                        // Get coordinates by splitting string
                        string[] coords = tileSetReader.Value.Split(' ', ',');

                        // Create collider bounds
                        Vector2 minCoords = new Vector2();
                        minCoords.X = float.Parse(coords[0]);
                        minCoords.Y = float.Parse(coords[1]);

                        Vector2 maxCoords = new Vector2();
                        maxCoords.X = float.Parse(coords[4]);
                        maxCoords.Y = float.Parse(coords[5]);

                        // Flip Y axis for proper drawing (Coordinate system origin different)
                        offset.Y = tileSet.TileHeight - offset.Y;
                        maxCoords.Y = -maxCoords.Y;

                        // Create collider and add it to the loaded tile
                        // Devide by tile width to get coordinates inside of tile, e.g. (4, 12) -> (0.25, 0.75)
                        Box2 collider = new Box2((minCoords + offset) / tileSet.TileWidth, (maxCoords + offset) / tileSet.TileWidth);
                        tiles[tileId + 1].colliders.Add(collider);
                    }
                }
            }
        }

        // Generate tile-entitys depending on the tilemap.
        public void CreateTileMapEntitys()
        {
            // Go through all layers
            foreach (TileLayer layer in map.Layers.OfType<TileLayer>())
            {
                for (int y = 0, i = 0; y < layer.Height; y++)
                {
                    for (int x = 0; x < layer.Width; x++, i++)
                    {
                        // Get id of current cell
                        UInt32 gid = (uint)layer.Data[i];

                        // No tile -> Skip this tile
                        if (gid == 0)
                            continue;

                        // Generate Entity
                        IEntity entity = new Entity("Tile");

                        // Search for tile in dictionary and set coordinates
                        Tile dictionaryTile = (Tile) tiles[gid].Clone();
                        dictionaryTile.Position = new Vector2(x, layer.Height - 1 - y);
                        entity.AddComponent(dictionaryTile);
                        entity.AddComponent(new Transform(new Vector2(x, layer.Height - 1 - y), 1, 1));
                        Manager.GetInstance().AddEntity(entity);

                        // Adjust colliderGrid if collisionOptimization is turned on and the tile has at least one collision
                        if (collisionOptimization && dictionaryTile.colliders.Count > 0)
                            colliderGrid[x, y] = true;

                        // Generate collision entities
                        if (!collisionOptimization)
                        {
                            // Check if current tile has colliders
                            if (dictionaryTile.colliders.Count > 0)
                            {
                                // Go through all colliders
                                foreach (var collider in dictionaryTile.colliders)
                                {
                                    // Generate new entity for collision
                                    IEntity collisionEntity = new Entity("CollisonTile");
                                    collisionEntity.AddComponent(new Transform(new Vector2(x, layer.Height - 1 - y) + collider.Min, collider.Size.X, collider.Size.Y));
                                    collisionEntity.AddComponent(new Collider(new Vector2(x, layer.Height - 1 - y) + collider.Min, collider.Size.X, collider.Size.Y));
                                    Manager.GetInstance().AddEntity(collisionEntity);
                                }
                            }
                        }
                    }
                }
            }

            // Add optimized colliders if turned on
            if (collisionOptimization)
                AddOptimizedColliders();
        }


        // Add optimized colliders to tiles.
        private void AddOptimizedColliders()
        {
            //create unionized colliders in Grid
            Vector2 min = new Vector2(-1, -1);
            Vector2 max = new Vector2(-1, -1);
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {

                    if (colliderGrid[x, y] && min.X != -1)
                    {
                        max.X = x;
                        colliderGrid[x, y] = false;
                    }
                    if (colliderGrid[x, y] && min.X == -1)
                    {
                        min.X = x;
                        min.Y = y;
                        max.X = x;
                        max.Y = y;
                        colliderGrid[x, y] = false;
                    }
                    else if (min.X != -1 && ((x + 1) == map.Width) || !colliderGrid[(x + 1) % map.Width, y])
                    {
                        for (int y2 = (int)min.Y + 1; y2 <= map.Height && min.X != -1; y2++)
                        {
                            bool completeLine = true;
                            if (y2 != map.Height)
                            {
                                for (int x2 = (int)min.X; x2 <= (int)max.X && completeLine; x2++)
                                {
                                    if (!colliderGrid[x2, y2])
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
                                    colliderGrid[x2, y2] = false;
                                }
                            }
                            else
                            {
                                IEntity entity = new Entity("Map");
                                entity.AddComponent(new Collider(new Vector2(min.X, (max.Y * -1) + map.Height - 1), max.X - min.X + 1, max.Y - min.Y + 1));
                                entity.AddComponent(new Transform(new Vector2(min.X, (max.Y * -1) + map.Height - 1), max.X - min.X + 1, max.Y - min.Y + 1));
                                //Manager.GetInstance().AddEntity(entity);
                                entities.Add(entity);
                                min = new Vector2(-1, -1);
                                max = new Vector2(-1, -1);
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Load in tileSet texture. Set settings for texture. Tile width and height calculation for tileSet.
        public TileSetStruct LoadTileSet(string imagePath, uint columns, uint rows)
        {
            Texture2D tileSetTexture = ResourceLoader.LoadContentAsTexture2D(imagePath);
            GL.BindTexture(TextureTarget.Texture2D, tileSetTexture.Handle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)OpenTK.Graphics.OpenGL.TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)OpenTK.Graphics.OpenGL.TextureMinFilter.Nearest);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            return new TileSetStruct()
            {
                Texture2D = tileSetTexture.Handle,
                TileWidth = 1f / columns,
                TileHeight = 1f / rows,
            };
        }
    }
}
