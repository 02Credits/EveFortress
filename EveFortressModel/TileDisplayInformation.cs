using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressModel
{
    [ProtoContract]
    public class TileDisplayInformation
    {
        [ProtoMember(1)]
        public TerrainTiles TerrainTile { get; set; }
        [ProtoMember(2)]
        public EntityTiles EntityTile { get; set; }
        [ProtoMember(3)]
        public UITiles UITile { get; set; }
        [ProtoMember(4)]
        public ItemTiles ItemTile { get; set; }
        [ProtoMember(5)]
        public byte R { get; set; }
        [ProtoMember(6)]
        public byte G { get; set; }
        [ProtoMember(7)]
        public byte B { get; set; }
        [ProtoMember(8)]
        public byte A { get; set; }
        [ProtoMember(9)]
        public bool IncludeColor { get; set; }

        public TileDisplayInformation() {}
        public TileDisplayInformation(TerrainTiles tile)
        {
            TerrainTile = tile;
            IncludeColor = false;
        }
        public TileDisplayInformation(TerrainTiles tile, byte r, byte g, byte b, byte a)
        {
            TerrainTile = tile;
            R = r;
            G = g;
            B = b;
            IncludeColor = true;
        }
        public TileDisplayInformation(EntityTiles tile)
        {
            EntityTile = tile;
            IncludeColor = false;
        }
        public TileDisplayInformation(EntityTiles tile, byte r = 255, byte g = 255, byte b = 255, byte a = 255)
        {
            EntityTile = tile;
            R = r;
            G = g;
            B = b;
            IncludeColor = true;
        }
        public TileDisplayInformation(UITiles tile)
        {
            UITile = tile;
            IncludeColor = false;
        }
        public TileDisplayInformation(UITiles tile, byte r = 255, byte g = 255, byte b = 255, byte a = 255)
        {
            UITile = tile;
            R = r;
            G = g;
            B = b;
            IncludeColor = true;
        }
        public TileDisplayInformation(ItemTiles tile)
        {
            ItemTile = tile;
            IncludeColor = false;
        }
        public TileDisplayInformation(ItemTiles tile, byte r = 255, byte g = 255, byte b = 255, byte a = 255)
        {
            ItemTile = tile;
            R = r;
            G = g;
            B = b;
            IncludeColor = true;
        }

        public static implicit operator TileDisplayInformation(TerrainTiles tiles) { return new TileDisplayInformation(tiles); }
        public static implicit operator TileDisplayInformation(EntityTiles tiles) { return new TileDisplayInformation(tiles); }
        public static implicit operator TileDisplayInformation(UITiles tiles) { return new TileDisplayInformation(tiles); }
        public static implicit operator TileDisplayInformation(ItemTiles tiles) { return new TileDisplayInformation(tiles); }
    }
}
