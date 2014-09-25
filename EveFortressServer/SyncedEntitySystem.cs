﻿using EveFortressModel;
using EveFortressModel.Components;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressServer
{
    public class SyncedEntitySystem : EntitySystem
    {
        public override List<Type> ComponentSubscriptions
        {
            get { return new List<Type> { typeof(Synced) }; }
        }

        public override void Update(Entity entity)
        {
            if (entity.UpdatedComponents.Count > 0 || entity.PositionUpdated)
            {
                var patch = entity.GetPatch();
                var entityLoc = Chunk.GetChunkCoords(entity.Position);

                foreach (var client in Program.PlayerManager.Connections.Values)
                {
                    var player = Program.PlayerManager.Players[Program.PlayerManager.ConnectionNames[client]];
                    if (player.SubscribedChunks.Contains(entityLoc))
                    {
                        Program.ClientMethods.PatchEntity(patch, client);
                    }
                }
            }

            entity.UpdatedComponents.Clear();
            entity.PositionUpdated = false;
        }
    }
}
