using EveFortressModel;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using EveFortressModel.Components;

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
                var topLeftLoc = new Point<long>(left + dx, top + dy);
                var corners = new[] 
                { 
                    Tuple.Create(1, GetTypeAtLocation(topLeftLoc)),
                    Tuple.Create(2, GetTypeAtLocation(new Point<long>(left + dx + 1, top + dy))),
                    Tuple.Create(4, GetTypeAtLocation(new Point<long>(left + dx, top + dy + 1))),
                    Tuple.Create(8, GetTypeAtLocation(new Point<long>(left + dx + 1, top + dy + 1)))
                };

                if (!corners.Any((t) => t.Item2 == null))
                {
                    var orderedGroups = corners.GroupBy((corner) => corner.Item2.Item1).OrderByDescending((grouping) => (int)grouping.Key);

                    var tilesToDisplay = new List<TileDisplayInformation>();
                    var topLeft = corners.First((t) => t.Item1 == 1);
                    var sheetName = TerrainUtils.Names[orderedGroups.First().First().Item2.Item1] + "Tiles";
                    if (orderedGroups.Count() == 1)
                    {
                        var sheetCount = Game.TileManager.TileSheets[sheetName].Value.Count;
                        if (topLeft.Item2.Item2 >= 255 - sheetCount + 1)
                        {
                            var index = topLeft.Item2.Item2 % sheetCount;
                            tilesToDisplay.Add(new TileDisplayInformation(sheetName, index));
                        }
                        else
                        {
                            tilesToDisplay.Add(new TileDisplayInformation(sheetName, 0));
                        }
                    }
                    else
                    {
                        tilesToDisplay.Add(new TileDisplayInformation(sheetName, 0));
                        foreach (var transition in orderedGroups.Skip(1))
                        {
                            sheetName = TerrainUtils.Names[transition.Key] + "Transition";
                            var index = TerrainUtils.GetSpriteIndex(transition.Select((t) => t.Item1));
                            tilesToDisplay.Add(new TileDisplayInformation(sheetName, index));
                        }
                    }
                    Game.TileManager.DrawTile(tilesToDisplay, dx, dy, this);
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