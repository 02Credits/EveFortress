using ProtoBuf;

namespace EveFortressModel
{
    [ProtoContract]
    public class TileDisplayInformation
    {
        [ProtoMember(1)]
        public string SheetID { get; set; }

        [ProtoMember(2)]
        public int TileNumber { get; set; }

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

        public TileDisplayInformation()
        {
        }

        public TileDisplayInformation(string sheetID, int tileNumber, bool includeColor = false)
        {
            SheetID = sheetID;
            TileNumber = tileNumber;
            IncludeColor = includeColor;
        }

        public TileDisplayInformation(EntityTiles tile)
        {
            SheetID = "Entities";
            TileNumber = (int)tile;
            IncludeColor = false;
        }

        public TileDisplayInformation(EntityTiles tile, byte r, byte g, byte b, byte a = 255)
        {
            SheetID = "Entities";
            TileNumber = (int)tile;
            R = r;
            G = g;
            B = b;
            IncludeColor = true;
        }

        public TileDisplayInformation(UITiles tile)
        {
            SheetID = "UI";
            TileNumber = (int)tile;
            IncludeColor = false;
        }

        public TileDisplayInformation(UITiles tile, byte r, byte g, byte b, byte a = 255)
        {
            SheetID = "UI";
            TileNumber = (int)tile;
            R = r;
            G = g;
            B = b;
            IncludeColor = true;
        }

        public TileDisplayInformation(ItemTiles tile)
        {
            SheetID = "Items";
            TileNumber = (int)tile;
            IncludeColor = false;
        }

        public TileDisplayInformation(ItemTiles tile, byte r, byte g, byte b, byte a = 255)
        {
            SheetID = "Items";
            TileNumber = (int)tile;
            R = r;
            G = g;
            B = b;
            IncludeColor = true;
        }

        public static implicit operator TileDisplayInformation(EntityTiles tiles)
        {
            return new TileDisplayInformation(tiles);
        }

        public static implicit operator TileDisplayInformation(UITiles tiles)
        {
            return new TileDisplayInformation(tiles);
        }

        public static implicit operator TileDisplayInformation(ItemTiles tiles)
        {
            return new TileDisplayInformation(tiles);
        }
    }
}