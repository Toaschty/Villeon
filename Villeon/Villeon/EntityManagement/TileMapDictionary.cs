using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OpenTK.Mathematics;
using Villeon.Assets;
using Villeon.Components;
using Villeon.GUI;
using Villeon.Helper;
using Zenseless.OpenTK;
using static Villeon.Components.Tile;

namespace Villeon.EntityManagement
{
    public class TileMapDictionary
    {
        private readonly string _mapName;

        // Holds the reference to the whole tilemap file
        private readonly TiledLib.Map _map;

        private int _caveIndex = -1;

        // Dictionary tileId => Tile object
        private Dictionary<uint, Tile> _tiles = new Dictionary<uint, Tile>();

        private XmlDocument _tileSetXml;

        public TileMapDictionary(string mapName)
        {
            _mapName = mapName;

            // Load in the tilemap and all associated tileset files
            _map = TiledLib.Map.FromStream(ResourceLoader.LoadContentAsStream("TileMap." + mapName), ts => ResourceLoader.LoadContentAsStream("TileMap." + ts.Source));

            _tileSetXml = new XmlDocument();

            // Start the setup prozess
            SetupTileDictionaries();
        }

        public TileMapDictionary(string mapName, int caveIndex)
        {
            _mapName = mapName;
            _caveIndex = caveIndex;

            // Load in the tilemap and all associated tileset files
            _map = TiledLib.Map.FromStream(ResourceLoader.LoadContentAsStream("TileMap." + mapName), ts => ResourceLoader.LoadContentAsStream("TileMap." + ts.Source));

            _tileSetXml = new XmlDocument();

            // Start the setup prozess
            SetupTileDictionaries();
        }

        public TiledLib.Map Map => _map;

        public string MapName => _mapName;

        public Dictionary<uint, Tile> Tiles => _tiles;

        public int CaveIndex => _caveIndex;

        // Start the setup prozess
        private void SetupTileDictionaries()
        {
            foreach (var tileSet in _map !.Tilesets)
            {
                float rezImageWidth = 1f / tileSet.ImageWidth;
                float rezImageHeight = 1f / tileSet.ImageHeight;

                // Load in corresponding tileSet texture
                TileSetStruct graphicsTileSet = LoadTileSet("TileMap." + tileSet.ImagePath, (uint)tileSet.Columns, (uint)tileSet.Rows);

                // Go through all tiles inside the current tileset
                for (int gid = tileSet.FirstGid; gid < tileSet.FirstGid + tileSet.TileCount; gid++)
                {
                    // Get tile with gid
                    TiledLib.Tile tile = tileSet[gid];

                    // Create a new Tile-Object with given data
                    Tile currentTile = new Tile(tile.Left * rezImageWidth, (tileSet.ImageHeight - tileSet.TileHeight - tile.Top) * rezImageHeight, graphicsTileSet);

                    // Add current tile to dictionary
                    _tiles.Add((uint)gid, currentTile);
                }
            }

            // Read additional data like colliders and animations
            GetAdditionalData();
        }

        // Read additional data like colliders and animations
        private void GetAdditionalData()
        {
            // Go through all tilesets
            foreach (var tileSet in _map.Tilesets)
            {
                // Load .tsx file as xml document
                _tileSetXml.Load(ResourceLoader.LoadContentAsStream("TileMap.Tilesets." + tileSet.Name + ".tsx"));

                // Select all "Tile"-Nodes in .tsx file
                XmlNodeList tileNodes = _tileSetXml.SelectNodes("tileset/tile") !;

                // Go through all "Tile"-Nodes
                foreach (XmlNode tileNode in tileNodes)
                {
                    // Read attribute "id" from current node (! = Attribute is always here)
                    uint tileId = uint.Parse(tileNode.Attributes!["id"] !.Value);

                    // Get current tile from dictionary (+1: Empty grid = 0. Tile-IDs saved 1 upwards)
                    Tile currentTile = _tiles[tileId + 1];

                    // Go through all child nodes of current tile. Possible node: collision = "objectgroup", animation = "animation"
                    foreach (XmlNode tileChildNode in tileNode.ChildNodes)
                    {
                        if (tileChildNode.Name == "objectgroup")
                        {
                            GetCollisionData(tileChildNode, tileId, tileSet);
                        }

                        if (tileChildNode.Name == "animation")
                        {
                            GetAnimationData(tileChildNode, tileId);
                        }
                    }
                }
            }
        }

        // Read animation data from file
        private void GetAnimationData(XmlNode tileChildNode, uint tileId)
        {
            // Get all child nodes from "animation" nodes
            XmlNodeList frames = tileChildNode.ChildNodes;

            Tile tile = _tiles[tileId + 1];

            // Go through all "frame"-Nodes of animation
            foreach (XmlNode frame in frames)
            {
                // Read animation frame id and save it into AnimationFrames
                int frameId = int.Parse(frame.Attributes!["tileid"] !.Value);
                tile.AnimationFrames.Add(_tiles[(uint)frameId + 1]);

                // Add current frame duration to animation FrameDuration. Value given in ms => /1000
                tile.FrameDuration += float.Parse(frame.Attributes!["duration"] !.Value) / 1000;
            }

            // Divide by framecount => Average out different frame durations
            tile.FrameDuration /= tile.AnimationFrames.Count;
        }

        // Read collision data from file
        private void GetCollisionData(XmlNode tileChildNode, uint tileId, TiledLib.ITileset tileSet)
        {
            // Get all child nodes from "objectgroup" nodes
            XmlNodeList collisions = tileChildNode.ChildNodes;

            // Go through all "object"-Nodes. Each Node represents one collision box
            foreach (XmlNode collision in collisions)
            {
                // Get collider offset from (0,0)
                Vector2 offset = new ();
                offset.X = float.Parse(collision.Attributes!["x"] !.Value);
                offset.Y = float.Parse(collision.Attributes!["y"] !.Value);

                // Get collider coordinates. Coordinates only represents size not the position
                string[] coords = collision.ChildNodes[0] !.Attributes!["points"] !.Value.Split(' ', ',');

                Vector2 minCoords = new ();
                minCoords.X = float.Parse(coords[0]);
                minCoords.Y = float.Parse(coords[1]);

                Vector2 maxCoords = new ();
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
        }

        // Load in tileSet texture. Set settings for texture. Tile width and height calculation for tileSet.
        private TileSetStruct LoadTileSet(string imagePath, uint columns, uint rows)
        {
            Texture2D tileSetTexture = Asset.GetTexture(imagePath);
            Vector2 delta = new Vector2(0.5f / tileSetTexture.Width, 0.5f / tileSetTexture.Height);

            return new TileSetStruct()
            {
                Texture2D = tileSetTexture,
                TileWidth = 1f / columns,
                TileHeight = 1f / rows,
                Delta = delta,
            };
        }
    }
}
