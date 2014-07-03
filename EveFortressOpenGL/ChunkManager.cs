using EveFortressModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace EveFortressClient
{
    public class ChunkManager : IUpdateNeeded, IResetNeeded
    {
        Dictionary<Tuple<long, long, long>, Chunk> SubscribedChunks { get; set; }
        Dictionary<Tuple<long, long, long>, bool> ChunkLocationsToSubscribeTo { get; set; }

        public ChunkManager()
        {
            SubscribedChunks = new Dictionary<Tuple<long, long, long>, Chunk>();
            ChunkLocationsToSubscribeTo = new Dictionary<Tuple<long, long, long>, bool>();
            Game.Updateables.Add(this);
            Game.Resetables.Add(this);
        }

        public BlockTypes GetBlock(long x, long y, long z)
        {
            var chunkPos = Chunk.GetChunkCoords(x, y, z);
            var chunk = GetChunk(chunkPos);
            
            if (chunk != null)
            {
                var blockPos = Chunk.GetBlockCoords(x, y, z);
                return chunk.GetBlock(blockPos);
            }

            ChunkLocationsToSubscribeTo[chunkPos] = true;
            return BlockTypes.Unknown;
        }

        public List<TileDisplayInformation> PerspectiveRayCast(long cameraX, long cameraY, long cameraZ, long planeX, long planeY, long planeZ)
        {
            var changeInZ = (float)(planeZ - cameraZ);
            var changeInX = (float)(planeX - cameraX);
            var changeInY = (float)(planeY - cameraY);
            var xOverZ = changeInX / changeInZ;
            var yOverZ = changeInY / changeInZ;
            var zOverX = changeInZ / changeInX;
            var zOverY = changeInZ / changeInY;
            Func<long, long> XOfZ = (z) => (long)((float)planeX + xOverZ * (float)(z - planeZ));
            Func<long, long> YOfZ = (z) => (long)((float)planeY + yOverZ * (float)(z - planeZ));
            Func<long, long> ZOfY = (y) => (long)((float)planeZ + zOverY * (float)(y - planeY));
            Func<long, long> ZOfX = (x) => (long)((float)planeZ + zOverX * (float)(x - planeX));
            long currentZ = planeZ;
            Chunk currentChunk = null;
            while (currentZ > 0)
            {
                long currentX = XOfZ(currentZ);
                long currentY = YOfZ(currentZ);
                if (currentChunk == null || !currentChunk.ContainsLoc(currentX, currentY, currentZ))
                {
                    currentChunk = GetChunk(currentX, currentY, currentZ);
                }

                if (currentChunk != null)
                {
                    var currentPos = Chunk.GetBlockCoords(currentX, currentY, currentZ);

                    var containingOctree = currentChunk.Blocks.GetContainingOctree(currentPos);
                    if (containingOctree.Children != null)
                    {
                        long highestNextZ = 0;
                        Action<long> replaceIfSuitable = (c) => { if (c < currentZ && c > highestNextZ) highestNextZ = c; };

                        replaceIfSuitable((currentChunk.Z * (long)Chunk.DIAMETER) + (long)containingOctree.Z + (long)containingOctree.Diameter / 2 - 1);
                        replaceIfSuitable((currentChunk.Z * (long)Chunk.DIAMETER) + (long)containingOctree.Z - 1);
                        if (changeInX > 0)
                        {
                            replaceIfSuitable(ZOfX(currentChunk.X * (long)Chunk.DIAMETER + (long)containingOctree.X + (long)containingOctree.Diameter + 1));
                            replaceIfSuitable(ZOfX(currentChunk.X * (long)Chunk.DIAMETER + (long)containingOctree.X + (long)containingOctree.Diameter / 2 + 1));
                        }
                        else
                        {
                            replaceIfSuitable(ZOfX(currentChunk.X * (long)Chunk.DIAMETER + (long)containingOctree.X - 1));
                            replaceIfSuitable(ZOfX(currentChunk.X * (long)Chunk.DIAMETER + (long)containingOctree.X + (long)containingOctree.Diameter / 2 - 1));
                        }
                        if (changeInY > 0)
                        {
                            replaceIfSuitable(ZOfY(currentChunk.Y * (long)Chunk.DIAMETER + (long)containingOctree.Y + (long)containingOctree.Diameter + 1));
                            replaceIfSuitable(ZOfY(currentChunk.Y * (long)Chunk.DIAMETER + (long)containingOctree.Y + (long)containingOctree.Diameter / 2 + 1));
                        }
                        else
                        {
                            replaceIfSuitable(ZOfY(currentChunk.Y * (long)Chunk.DIAMETER + (long)containingOctree.Y - 1));
                            replaceIfSuitable(ZOfY(currentChunk.Y * (long)Chunk.DIAMETER + (long)containingOctree.Y + (long)containingOctree.Diameter / 2 - 1));
                        }

                        currentZ = highestNextZ;
                    }
                    else
                    {
                        var blockType = containingOctree.BlockType;
                        var displayInfo = BlockDisplayer.GetDisplayInfo(blockType);
                        var zDistance = planeZ - currentZ;
                        var shading = 1 - ((float)zDistance / 30);
                        if (shading < 0) shading = 0;

                        displayInfo.R = (byte)(displayInfo.R * shading);
                        displayInfo.G = (byte)(displayInfo.G * shading);
                        displayInfo.B = (byte)(displayInfo.B * shading);
                        return new List<TileDisplayInformation> { displayInfo };
                    }
                }
                else
                {
                    return new List<TileDisplayInformation> { TerrainTiles.Checkered };
                }
            }
            return new List<TileDisplayInformation> { TerrainTiles.None };
        }

        public Chunk GetChunk(Tuple<long, long, long> pos)
        {
            return GetChunk(pos.Item1, pos.Item2, pos.Item3);
        }

        Chunk cachedChunk;
        public Chunk GetChunk(long x, long y, long z)
        {
            var loc = Chunk.GetChunkCoords(x, y, z);
            if (loc != cachedChunk.Maybe((c) => c.Loc))
            {
                if (SubscribedChunks.TryGetValue(loc, out cachedChunk))
                {
                    return cachedChunk;
                }
                else
                {
                    ChunkLocationsToSubscribeTo[loc] = true;
                    return null;
                }
            }
            else
            {
                return cachedChunk;
            }
        }

        bool subscribing;
        public async void Update()
        {
            if (!subscribing)
            {
                subscribing = true;
                foreach (var chunkLocation in ChunkLocationsToSubscribeTo.Keys.ToList())
                {
                    var x = chunkLocation.Item1;
                    var y = chunkLocation.Item2;
                    var z = chunkLocation.Item3;
                    var chunk = await Game.ServerMethods.SubscribeToChunk(x, y, z);
                    chunk.Blocks.UnPack();
                    SubscribedChunks[chunkLocation] = chunk;
                    ChunkLocationsToSubscribeTo.Remove(chunkLocation);
                }
                subscribing = false;
            }
        }

        public void Reset()
        {
            SubscribedChunks.Clear();
            ChunkLocationsToSubscribeTo.Clear();
            subscribing = false;
        }
    }
}
