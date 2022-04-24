namespace Villeon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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

        public TileMap(string mapName)
        {
            // Load in tilemap from file + TileSets 
            map = TiledLib.Map.FromStream(ResourceLoader.LoadContentAsStream("TileMap." + mapName), ts => ResourceLoader.LoadContentAsStream("TileMap." + ts.Source));
            colliderGrid = new bool[map.Width, map.Height];

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
                    AddTileToDictionary(graphicsTileSet, (uint)gid, tile.Left * rezImageWidth, (tileSet.ImageHeight - tileSet.TileHeight - tile.Top) * rezImageHeight);
                }
            }
        }

        // Generate tile-entitys depending on the tilemap.
        public void CreateTileMapEntitys()
        {
            // Go through all layers
            foreach (TileLayer layer in map.Layers.OfType<TileLayer>())
            {
                // Check if collider is needed
                bool hasCollider = false;
                string value;
                if (layer.Properties.TryGetValue("hasCollider", out value))
                    hasCollider = true;


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
                        // Search for tile in dictionary 
                        Tile dictionaryTile = tiles[gid];
                        // Use current x and y as coordinates. Use data from dictionaryTile.
                        entity.AddComponent(new Tile(new Vector2(x, layer.Height - 1 - y), dictionaryTile._x, dictionaryTile._y, dictionaryTile.TileSet));

                        if (hasCollider)
                        {
                            //entity.AddComponent(new Collider(new Vector2(x, layer.Height - 1 - y), 1, 1));
                            colliderGrid[x, y] = true;
                            entity.AddComponent(new Transform(new Vector2(x, layer.Height - 1 - y), 1, 1));
                        }
                        Manager.GetInstance().AddEntity(entity);
                    }
                }

                hasCollider = false;
            }

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
                                Manager.GetInstance().AddEntity(entity);
                                min = new Vector2(-1, -1);
                                max = new Vector2(-1, -1);
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Add new tile to dictionary with given data -> Used later to get right tile for generation.
        void AddTileToDictionary(TileSetStruct TileSet, uint tileId, float startX, float startY)
        {
            // Add given tile to dictionary -> gui is key
            tiles.Add(tileId, new Tile(startX, startY, TileSet));
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
