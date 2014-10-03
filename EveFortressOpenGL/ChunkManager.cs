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
        private Dictionary<Point<long>, Chunk> SubscribedChunks { get; set; }

        // Chunks which will be fetched from the server next update loop
        private Dictionary<Point<long>, bool> ChunkLocationsToSubscribeTo { get; set; }

        // Initialize storage and subscribe to updates and resets
        public ChunkManager()
        {
            SubscribedChunks = new Dictionary<Point<long>, Chunk>();
            ChunkLocationsToSubscribeTo = new Dictionary<Point<long>, bool>();
            Game.Updateables.Add(this);
            Game.Resetables.Add(this);
        }

        public Chunk GetChunk(Point<long> loc)
        {
            var tempLoc = new Point<long>(loc.X - 1, loc.Y);
            if (!SubscribedChunks.ContainsKey(tempLoc))
                ChunkLocationsToSubscribeTo[tempLoc] = true;
            tempLoc = new Point<long>(loc.X + 1, loc.Y);
            if (!SubscribedChunks.ContainsKey(tempLoc))
                ChunkLocationsToSubscribeTo[tempLoc] = true;
            tempLoc = new Point<long>(loc.X, loc.Y - 1);
            if (!SubscribedChunks.ContainsKey(tempLoc))
                ChunkLocationsToSubscribeTo[tempLoc] = true;
            tempLoc = new Point<long>(loc.X, loc.Y + 1);
            if (!SubscribedChunks.ContainsKey(tempLoc))
                ChunkLocationsToSubscribeTo[tempLoc] = true;

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

        private bool subscribing;

        public async void Update()
        {
            if (!subscribing)
            {
                subscribing = true;
                foreach (var chunkLocation in ChunkLocationsToSubscribeTo.Keys.ToList())
                {
                    var chunk = await Game.ServerMethods.SubscribeToChunk(chunkLocation.X, chunkLocation.Y);
                    SubscribedChunks[chunkLocation] = chunk;
                    ChunkLocationsToSubscribeTo.Remove(chunkLocation);
                }
                subscribing = false;
            }
        }

        Dictionary<Point<long>, EntityPatch> cachedPatches = new Dictionary<Point<long>, EntityPatch>();
        public async void ApplyEntityPatch(EntityPatch patch)
        {
            var previousChunkLoc = Chunk.GetChunkCoords(patch.PreviousPosition);
            Chunk chunk;
            if (SubscribedChunks.TryGetValue(previousChunkLoc, out chunk))
            {
                if (!chunk.ContainsLoc(patch.Position))
                {
                    var entity = chunk.Entities[patch.ID];
                    chunk.Entities.Remove(patch.ID);
                    var targetChunkLoc = Chunk.GetChunkCoords(patch.Position);
                    Chunk targetChunk;
                    if (SubscribedChunks.TryGetValue(targetChunkLoc, out targetChunk))
                    {
                        targetChunk.Entities[patch.ID] = entity;
                        entity.ApplyPatch(patch);
                    }
                    else
                    {
                        if (ChunkLocationsToSubscribeTo.ContainsKey(targetChunkLoc))
                        {
                            cachedPatches.Add(targetChunkLoc, patch);
                        }
                    }
                }
                else
                {

                }
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