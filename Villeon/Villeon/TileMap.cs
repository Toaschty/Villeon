using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledLib.Layer;
using Villeon.Components;
using Zenseless.OpenTK;
using Zenseless.Resources;
using static Villeon.Components.Tile;

namespace Villeon
{
    internal class TileMap
    {
        private readonly TiledLib.Map map;

        // Dictionary tileId => Tile object
        public Dictionary<uint, Villeon.Components.Tile> tiles = new();

        private readonly Manager manager;

        // Embedded Directory
        EmbeddedResourceDirectory resourceDir = new EmbeddedResourceDirectory("Villeon.Content.TileMap");

        public TileMap(string mapName, Manager manager)
        {
            // Load in tilemap from file + TileSets 
            map = TiledLib.Map.FromStream(LoadContent(mapName), ts => LoadContent(ts.Source));
            this.manager = manager;

            // Starting generation
            CreateTileMapEntitys();
        }

        private void CreateTileMapEntitys()
        {
            // Go through all tilesets
            foreach (var tileSet in map.Tilesets)
            {
                float rezImageWidth = 1f / tileSet.ImageWidth;
                float rezImageHeight = 1f / tileSet.ImageHeight;

                // Load in corresponding tileset
                TileSetStruct graphicsTileSet = LoadTileSet(tileSet.ImagePath, (uint)tileSet.Columns, (uint)tileSet.Rows);

                // Go through all tiles in tileset -> Safe all tiles in dictionary
                for (int gid = tileSet.FirstGid; gid < tileSet.FirstGid + tileSet.TileCount; gid++)
                {
                    TiledLib.Tile tile = tileSet[gid];
                    AddTileToDictionary(graphicsTileSet, (uint)gid, tile.Left * rezImageWidth, (tileSet.ImageHeight - tileSet.TileHeight - tile.Top) * rezImageHeight);
                }
            }

            // Go through all layers
            foreach (TileLayer layer in map.Layers.OfType<TileLayer>())
            {
                // Check if collider is needed
                bool hasCollider = false;
                string value;
                if( layer.Properties.TryGetValue("hasCollider", out value) )
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
                            entity.AddComponent(new Collider(new Vector2(x, layer.Height - 1 - y), 1, 1));
                            entity.AddComponent(new Transform(new Vector2(x, layer.Height - 1 - y), 1, 1));
                        }
                        manager.AddEntity(entity);
                    }
                }

                hasCollider = false;
            }
        }

        void AddTileToDictionary(TileSetStruct TileSet, uint tileId, float startX, float startY)
        {
            // Add given tile to dictionary -> gui is key
            tiles.Add(tileId, new Tile(startX, startY, TileSet));
        }

        // Resource Loading from Embedded Directory => Remove if solved other way
        public Stream LoadContent(string name)
        {
            return resourceDir.Resource(name).Open();
        }
        
        TileSetStruct LoadTileSet(string imagePath, uint columns, uint rows)
        {
            Stream stream = LoadContent(imagePath);
            Texture2D tileSetTexture = Texture2DLoader.Load(stream);
            GL.BindTexture(TextureTarget.Texture2D, tileSetTexture.Handle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)OpenTK.Graphics.OpenGL.TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)OpenTK.Graphics.OpenGL.TextureMinFilter.Nearest);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            return new TileSetStruct()
            {
                Texture2D = tileSetTexture.Handle,
                TileWidth = 1f/columns,
                TileHeight = 1f/rows,
            };
        }
    }
}
