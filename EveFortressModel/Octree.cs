using ProtoBuf;
using System;
using System.Collections.Generic;
using Utils;

namespace EveFortressModel
{
    [ProtoContract]
    public class Octree
    {
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte Z { get; set; }
        public byte Diameter { get; set; }
        public BlockTypes BlockType { get; set; }
        public Octree[] Children { get; set; }
        public Octree Parent { get; set; }

        [ProtoMember(1)]
        public byte[] SerializedForm { get; set; }

        public void PackUp()
        {
            var byteList = new List<byte>();
            byteList.Add(X);
            byteList.Add(Y);
            byteList.Add(Z);
            byteList.Add(Diameter);
            Serialize(byteList);
            SerializedForm = byteList.ToArray();
        }

        public void UnPack()
        {
            X = SerializedForm[0];
            Y = SerializedForm[1];
            Z = SerializedForm[2];
            Diameter = SerializedForm[3];
            Deserialize(SerializedForm, 4);
        }

        public void Serialize(List<byte> current)
        {
            if (Children == null)
            {
                current.Add((byte)(BlockType + 1));
            }
            else
            {
                current.Add(0);
                byte binary = 0;
                byte currentBit = 1;
                for (int i = 0; i < 8; i++)
                {
                    if (Children[i] != null)
                    {
                        binary = (byte)(binary | currentBit);
                    }
                    currentBit = (byte)(currentBit << 1);
                }
                current.Add(binary);
                for (int i = 0; i < 8; i++)
                {
                    if (Children[i] != null)
                    {
                        Children[i].Serialize(current);
                    }
                }
            }
        }

        public int Deserialize(byte[] data, int currentIndex)
        {
            if (data[currentIndex] == 0)
            {
                currentIndex++;
                var presentChildren = data[currentIndex];
                currentIndex++;
                Children = new Octree[8];
                byte currentBit = 1;
                for (int i = 0; i < 8; i++)
                {
                    if ((presentChildren & currentBit) != 0)
                    {
                        Children[i] = CreateOctTreeForIndex(i);
                        currentIndex = Children[i].Deserialize(data, currentIndex);
                    }
                    currentBit = (byte)(currentBit << 1);
                }
            }
            else
            {
                BlockType = (BlockTypes)(data[currentIndex] - 1);
                Children = null;
                currentIndex++;
            }
            return currentIndex;
        }

        public Octree() { }

        public Octree(Octree parent, byte x, byte y, byte z, byte diameter)
        {
            Parent = parent;
            X = x;
            Y = y;
            Z = z;
            Diameter = diameter;
            if (Diameter > 1)
            {
                Children = new Octree[8];
            }
        }

        public bool ContainsLoc(byte x, byte y, byte z)
        {
            return x >= X && x < X + Diameter &&
                   y >= Y && y < Y + Diameter &&
                   z >= Z && z < Z + Diameter;
        }

        public Octree GetContainingOctree(Tuple<byte, byte, byte> loc)
        {
            return GetContainingOctree(loc.Item1, loc.Item2, loc.Item3);
        }

        public Octree GetContainingOctree(byte x, byte y, byte z)
        {
            if (Children != null)
            {
                var index = IndexWhichContainsLocation(x, y, z);
                if (Children[index] != null)
                {
                    return Children[index].GetContainingOctree(x, y, z);
                }
            }
            return this;
        }

        int IndexWhichContainsLocation(byte x, byte y, byte z)
        {
            var radius = (byte)(Diameter / 2);
            if (x >= X + radius)
            {
                if (y >= Y + radius)
                {
                    if (z >= Z + radius)
                    {
                        return 6;
                    }
                    else
                    {
                        return 5;
                    }
                }
                else
                {
                    if (z >= Z + radius)
                    {
                        return 2;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            else
            {
                if (y >= Y + radius)
                {
                    if (z >= Z + radius)
                    {
                        return 7;
                    }
                    else
                    {
                        return 4;
                    }
                }
                else
                {
                    if (z >= Z + radius)
                    {
                        return 3;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        public Octree CreateOctTreeForIndex(int i)
        {
            byte radius = (byte)(Diameter / 2);
            switch (i)
            {
                case 0:
                    return new Octree(this, X, Y, Z, radius);
                case 1:
                    return new Octree(this, (byte)(X + radius), Y, Z, radius);
                case 2:
                    return new Octree(this, (byte)(X + radius), Y, (byte)(Z + radius), radius);
                case 3:
                    return new Octree(this, X, Y, (byte)(Z + radius), radius);
                case 4:
                    return new Octree(this, X, (byte)(Y + radius), Z, radius);
                case 5:
                    return new Octree(this, (byte)(X + radius), (byte)(Y + radius), Z, radius);
                case 6:
                    return new Octree(this, (byte)(X + radius), (byte)(Y + radius), (byte)(Z + radius), radius);
                case 7:
                    return new Octree(this, X, (byte)(Y + radius), (byte)(Z + radius), radius);
            }
            throw new ArgumentException("Index out of range");
        }

        public BlockTypes GetBlock(byte x, byte y, byte z)
        {
            if (Children == null)
            {
                return BlockType;
            }
            else
            {
                return Children[IndexWhichContainsLocation(x, y, z)].Maybe((o) => o.GetBlock(x, y, z), BlockTypes.None);
            }
        }

        public Octree SetBlock(byte x, byte y, byte z, BlockTypes type)
        {
            var childIndex = IndexWhichContainsLocation(x, y, z);
            if (Diameter == 1)
            {
                Children = null;
                BlockType = type;
                return this;
            }
            else if (Children == null)
            {
                if (BlockType != type)
                {
                    Children = new Octree[8];
                    for (int i = 0; i < 8; i++)
                    {
                        Children[i] = CreateOctTreeForIndex(i);
                        Children[i].Children = null;
                        Children[i].BlockType = BlockType;

                        if (i == childIndex)
                        {
                            return Children[i].SetBlock(x, y, z, type);
                        }
                    }
                }
                else
                {
                    return this;
                }
            }
            else
            {
                if (Children[childIndex] == null)
                {
                    Children[childIndex] = CreateOctTreeForIndex(childIndex);
                }
                return Children[childIndex].SetBlock(x, y, z, type);
            }
            return null;
        }

        public void DeleteBlock(byte x, byte y, byte z)
        {
            if (Children == null)
            {
                if (Diameter == 1)
                {
                    var myIndex = Parent.IndexWhichContainsLocation(X, Y, Z);
                    Parent.Children[myIndex] = null;
                }
                else
                {
                    var indexToDelete = IndexWhichContainsLocation(x, y, z);
                    Children = new Octree[8];
                    for (int i = 0; i < 8; i++)
                    {
                        Children[i] = CreateOctTreeForIndex(i);
                        Children[i].Children = null;
                        Children[i].BlockType = BlockType;
                    }
                    Children[indexToDelete].DeleteBlock(x, y, z);
                }
            }
            else
            {
                var indexToDelete = IndexWhichContainsLocation(x, y, z);
                Children[indexToDelete].MaybeAction((o) => o.DeleteBlock(x, y, z));
            }
        }

        public void Simplify()
        {
            if (Children != null)
            {
                bool simple = true;
                bool empty = true;
                bool blockTypeFound = false;
                BlockTypes blockType = BlockTypes.None;
                for (int i = 0; i < 8; i++)
                {
                    Children[i].MaybeAction((c) => c.Simplify());
                    if (Children[i] != null)
                    {
                        empty = false;
                        if (blockTypeFound)
                        {
                            if (Children[i].Children != null ||
                                Children[i].BlockType != blockType)
                            {
                                simple = false;
                            }
                        }
                        else
                        {
                            if (Children[i].Children == null)
                            {
                                blockTypeFound = true;
                                blockType = Children[i].BlockType;
                            }
                            else
                            {
                                simple = false;
                            }
                        }
                    }
                    else
                    {
                        simple = false;
                    }
                }

                if (empty)
                {
                    if (Parent != null)
                    {
                        var myIndex = Parent.IndexWhichContainsLocation(X, Y, Z);
                        Parent.Children[myIndex] = null;
                    }
                }
                else if (simple && blockTypeFound)
                {
                    Children = null;
                    BlockType = blockType;
                }
            }
        }
    }
}
