using EveFortressModel;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace EveFortressClient
{
    public class MapTab : UITab
    {
        public override int MinimumWidth
        {
            get { return 5; }
        }

        public override int MinimumHeight
        {
            get { return 5; }
        }

        public override string Title
        {
            get { return "Map Test"; }
        }

        private long x = 0;
        private long y = 0;

        private long left
        {
            get
            {
                return x - Width / 2;
            }
        }

        private long right
        {
            get
            {
                return left + Width;
            }
        }

        private long top
        {
            get
            {
                return y - Height / 2;
            }
        }

        private long bottom
        {
            get
            {
                return top + Height;
            }
        }

        Dictionary<Point<long>, Chunk> cachedChunks = new Dictionary<Point<long>, Chunk>();
        Dictionary<Point<long>, Tuple<TerrainType, byte>> cachedLocations =
            new Dictionary<Point<long>, Tuple<TerrainType, byte>>();

        public override void Render()
        {
            var dx = 0;
            var dy = 0;
            while (top + dy <= bottom)
            {
                var topLeft = GetTypeAtLocation(new Point<long>(left + dx, top + dy));
                var topRight = GetTypeAtLocation(new Point<long>(left + dx + 1, top + dy));
                var bottomLeft = GetTypeAtLocation(new Point<long>(left + dx, top + dy + 1));
                var bottomRight = GetTypeAtLocation(new Point<long>(left + dx + 1, top + dy + 1));

                if (topLeft != null && topRight != null && bottomLeft != null && bottomRight != null)
                {
                    string sheetName;
                    int index;
                    bool solid = true;
                    TerrainType lower = TerrainType.Water;
                    TerrainType higher = TerrainType.Water;
                    solid = SolidOrOrder(ref lower, ref higher, topLeft.Item1, topRight.Item1);
                    if (solid)
                        solid = SolidOrOrder(ref lower, ref higher, topLeft.Item1, bottomLeft.Item1);
                    if (solid)
                        solid = SolidOrOrder(ref lower, ref higher, topLeft.Item1, bottomRight.Item1);

                    if (!solid)
                    {
                        sheetName = TerrainUtils.Names[lower] + TerrainUtils.Names[higher] + "Transition";
                        index = TerrainUtils.GetSpriteIndex(topLeft.Item1 == higher, topRight.Item1 == higher, bottomLeft.Item1 == higher, bottomRight.Item1 == higher);
                    }
                    else
                    {
                        sheetName = TerrainUtils.Names[lower] + "Tiles";
                        var sheetCount = Game.TileManager.TileSheets[sheetName].Value.TileWidth;
                        index = (int)(((float)topLeft.Item2 / 256f) * sheetCount);
                    }
                    Game.TileManager.DrawTile(new TileDisplayInformation(sheetName, index), dx, dy, this);
                }

                dx++;
                if (dx > Width)
                {
                    dy++;
                    dx = 0;
                }
            }

            var entities = cachedChunks.Values.SelectMany((chunk) => chunk.Entities.Values);
            foreach (var entity in entities)
            {
                if (entity.HasComponent<Appearance>())
                {
                    var entityX = (int)(entity.Position.X - left);
                    var entityY = (int)(entity.Position.Y - top);
                    var appearance = entity.GetComponent<Appearance>();
                    Game.TileManager.DrawTile(appearance.DisplayInfo, entityX, entityY, this);
                }
            }

            cachedChunks.Clear();
            cachedLocations.Clear();

            var coordsTiles = Game.TileManager.GetTilesFromString("X" + x + "Y" + y);
            var startPos = Width - coordsTiles.Count;
            for (int i = 0; i < coordsTiles.Count; i++)
            {
                var tileX = i + startPos;
                Game.TileManager.DrawTile(coordsTiles[i], tileX, 0, this);
            }
        }

        public bool SolidOrOrder(ref TerrainType lower, ref TerrainType higher, TerrainType one, TerrainType two)
        {
            if (one != two)
            {
                var oneStartLevel = TerrainUtils.TypeLevelStarts[one];
                var twoStartLevel = TerrainUtils.TypeLevelStarts[two];
                if (oneStartLevel < twoStartLevel)
                {
                    lower = one;
                    higher = two;
                    return false;
                }
                else
                {
                    lower = two;
                    higher = one;
                    return false;
                }
            }
            else
            {
                lower = one;
                return true;
            }
        }

        public Tuple<TerrainType, byte> GetTypeAtLocation(Point<long> loc)
        {
            if (!cachedLocations.ContainsKey(loc))
            {
                var chunkCoords = Chunk.GetChunkCoords(loc);
                var localCoords = Chunk.GetLocalCoords(loc);

                Chunk chunk;
                if (!cachedChunks.TryGetValue(chunkCoords, out chunk))
                {
                    chunk = Game.ChunkManager.GetChunk(chunkCoords);
                    if (chunk != null)
                    {
                        cachedChunks[chunkCoords] = chunk;
                    }
                }

                if (chunk != null)
                {
                    var tuple = Tuple.Create(chunk.GetTerrainLevel(localCoords), chunk.GetRandomSelection(localCoords));
                    cachedLocations[loc] = tuple;
                    return tuple;
                }
                else
                {
                    cachedLocations[loc] = null;
                    return null;
                }
            }
            else
            {
                return cachedLocations[loc];
            }
        }

        public override Task<bool> ManageInput()
        {
            var inputManaged = false;
            if (Game.InputManager.KeyDown(Keys.A))
            {
                x -= 1;
                inputManaged = true;
            }
            if (Game.InputManager.KeyDown(Keys.D))
            {
                x += 1;
                inputManaged = true;
            }
            if (Game.InputManager.KeyDown(Keys.W))
            {
                y -= 1;
                inputManaged = true;
            }
            if (Game.InputManager.KeyDown(Keys.S))
            {
                y += 1;
                inputManaged = true;
            }
            return Task.FromResult(inputManaged);
        }
    }
}