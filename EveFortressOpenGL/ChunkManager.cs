using EveFortressModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EveFortressClient
{
    // A class which is meant to manage the chunks. I also perform ray casts here, although it might be more
    // useful to make a specialized ray class for this. Havent decided yet.
    public class ChunkManager : IUpdateNeeded, IResetNeeded
    {

        // The chunks which are currently in storage and are being updated by the server.
        Dictionary<Point<long>, Chunk> SubscribedChunks { get; set; }
        // Chunks which will be fetched from the server next update loop
        Dictionary<Point<long>, bool> ChunkLocationsToSubscribeTo { get; set; }

        // Initialize storage and subscribe to updates and resets
        public ChunkManager()
        {
            SubscribedChunks = new Dictionary<Point<long>, Chunk>();
            ChunkLocationsToSubscribeTo = new Dictionary<Point<long>, bool>();
            Game.Updateables.Add(this);
            Game.Resetables.Add(this);
        }

        // Gets a block at the given world coordinates
        public BlockTypes GetBlock(long x, long y, long z)
        {
            // Get the chunk at the x y and z
            var chunkPos = Chunk.GetChunkCoords(x, y, z);
            var chunk = GetChunk(chunkPos);
            
            // Check if the chunk is present, if so get the block at the block position
            if (chunk != null)
            {
                var blockPos = Chunk.GetBlockCoords(x, y, z);
                return chunk.GetBlock(blockPos);
            }

            // this is only a possible control path if the chunk is null, so return unknown
            return BlockTypes.Unknown;
        }


        public List<TileDisplayInformation> PerspectiveRayCast(long cameraX, long cameraY, long cameraZ, long planeX, long planeY, long planeZ)
        {
            var blockAboveCamera = GetBlock(planeX, planeY, planeZ + 1);
            if (blockAboveCamera != BlockTypes.None && blockAboveCamera != BlockTypes.Unknown)
            {
                return new List<TileDisplayInformation> { TerrainTiles.EmptySpace };
            }

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
                    var chunkPos = Chunk.GetChunkCoords(currentX, currentY, currentZ);
                    currentChunk = GetChunk(chunkPos);
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

        public Chunk GetChunk(Point<long> loc)
        {
            Chunk chunk = null;
            if (SubscribedChunks.TryGetValue(loc, out chunk))
            {
                return chunk;
            }
            else
            {
                ChunkLocationsToSubscribeTo[loc] = true;
                return null;
            }
        }

        public Chunk GetChunk(long x, long y, long z)
        {
            return GetChunk(x, y, z);
        }

        bool subscribing;
        public async void Update()
        {
            if (!subscribing)
            {
                subscribing = true;
                foreach (var chunkLocation in ChunkLocationsToSubscribeTo.Keys.ToList())
                {
                    var chunk = await Game.ServerMethods.SubscribeToChunk(chunkLocation.X, chunkLocation.Y, chunkLocation.Z);
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
