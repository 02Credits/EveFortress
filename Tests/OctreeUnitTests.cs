using EveFortressModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    [TestClass]
    public class OctreeUnitTests
    {
        [TestMethod]
        public void ContainsLoc_InBounds_ReturnsTrue()
        {
            var octree = new Octree(null, 8, 4, 2, 16);

            Assert.IsTrue(octree.ContainsLoc(9, 6, 8));
        }

        [TestMethod]
        public void ContainsLoc_OutOfBounds_ReturnsFalse()
        {
            var octree = new Octree(null, 8, 4, 2, 16);

            Assert.IsFalse(octree.ContainsLoc(9, 6, 18));
        }

        [TestMethod]
        public void GetContainingOctree_ReturnsCorrectTree()
        {
            var topOctree = new Octree(null, 0, 0, 0, 4);
            var correctOctree = topOctree.CreateOctTreeForIndex(4);
            topOctree.Children[4] = correctOctree;

            Assert.AreEqual(topOctree.GetContainingOctree(1, 3, 0), correctOctree);
        }

        [TestMethod]
        public void Simplify_OneLevel()
        {
            var octree = new Octree(null, 0, 0, 0, 2);
            octree.SetBlock(0, 0, 0, BlockTypes.Dirt);
            octree.SetBlock(1, 0, 0, BlockTypes.Dirt);
            octree.SetBlock(0, 1, 0, BlockTypes.Dirt);
            octree.SetBlock(0, 0, 1, BlockTypes.Dirt);
            octree.SetBlock(1, 1, 0, BlockTypes.Dirt);
            octree.SetBlock(0, 1, 1, BlockTypes.Dirt);
            octree.SetBlock(1, 0, 1, BlockTypes.Dirt);
            octree.SetBlock(1, 1, 1, BlockTypes.Dirt);
            octree.Simplify();

            Assert.IsNull(octree.Children);
            Assert.AreEqual(BlockTypes.Dirt, octree.BlockType);
        }

        [TestMethod]
        public void Simplify_FourLevels()
        {
            var octree = new Octree(null, 0, 0, 0, 16);
            for (byte x = 0; x < 16; x++)
            {
                for (byte y = 0; y < 16; y++)
                {
                    for (byte z = 0; z < 16; z++)
                    {
                        octree.SetBlock(x, y, z, BlockTypes.Dirt);
                    }
                }
            }
            octree.Simplify();

            Assert.IsNull(octree.Children);
            Assert.AreEqual(BlockTypes.Dirt, octree.BlockType);
        }

        [TestMethod]
        public void SetBlock_NonZeroLocation_SetsCorrectBlock()
        {
            var octree = new Octree(null, 0, 0, 0, 8);
            octree.SetBlock(5, 6, 3, BlockTypes.Water);

            Assert.AreEqual(octree.GetBlock(5, 6, 3), octree.Children[5].Children[7].Children[2].BlockType);
        }

        [TestMethod]
        public void SetBlock_NonZeroLocation_OnlySetsAtThatLocation()
        {
            var octree = new Octree(null, 0, 0, 0, 8);
            octree.SetBlock(3, 4, 5, BlockTypes.Dirt);
            octree.PackUp();
            octree.UnPack();

            for (byte x = 0; x < 8; x++)
            {
                for (byte y = 0; y < 8; y++)
                {
                    for (byte z = 0; z < 8; z++)
                    {
                        if (x == 3 && y == 4 && z == 5)
                        {
                            Assert.AreEqual(octree.GetBlock(x, y, z), BlockTypes.Dirt);
                        }
                        else
                        {
                            Assert.AreEqual(octree.GetBlock(x, y, z), BlockTypes.None);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Simplify_DeleteBlock()
        {
            var octree = new Octree(null, 0, 0, 0, 4);
            var childtree = new Octree(octree, 0, 0, 0, 2);
            octree.Children[0] = childtree;
            childtree.SetBlock(0, 0, 0, BlockTypes.Dirt);

            octree.DeleteBlock(0, 0, 0);

            octree.Simplify();

            Assert.IsNull(octree.Children[0]);
        }

        [TestMethod]
        public void Octree_SerializedForm_Serializes()
        {
            var octree = new Octree(null, 0, 0, 0, 4);
            octree.Children[0] = octree.CreateOctTreeForIndex(0);
            octree.Children[0].Children[5] = octree.Children[0].CreateOctTreeForIndex(5);
            octree.Children[0].Children[5].BlockType = BlockTypes.Stone;
            octree.Children[0].Children[5].Children = null;
            octree.Children[3] = octree.CreateOctTreeForIndex(3);
            octree.Children[3].BlockType = BlockTypes.Dirt;
            octree.Children[3].Children = null;
            octree.PackUp();

            var expectedData = new byte[]
            {
                0,
                0,
                0,
                4,
                0,
                Convert.ToByte("00001001", 2),
                0,
                Convert.ToByte("00100000", 2),
                4,
                3
            };

            Assert.AreEqual(octree.SerializedForm.Length, expectedData.Length);
            for (int i = 0; i < octree.SerializedForm.Length; i++)
            {
                Assert.AreEqual(expectedData[i], octree.SerializedForm[i]);
            }
        }
    }
}