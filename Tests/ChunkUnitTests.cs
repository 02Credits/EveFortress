using EveFortressModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    [TestClass]
    public class ChunkUnitTests
    {
        [TestMethod]
        public void GetChunkCoords_ReturnsCorrectly()
        {
            for (int x = -64; x < 64; x++)
            {
                var specificX = x % 32;
                if (specificX < 0)
                    specificX = 32 + specificX;
                var expectedX = (x - specificX) / 32;
                var chunkCoords = Chunk.GetChunkCoords(x, 0, 0);
                Assert.AreEqual(new Point<long>(expectedX, 0, 0), chunkCoords);
            }
        }

        [TestMethod]
        public void GetBlockCoords_ReturnsCorrectly()
        {
            Func<long, byte> oldMethod = (loc) =>
            {
                var specific = loc % 32;
                if (specific < 0)
                    specific = 32 + specific;
                return (byte)specific;
            };

            for (int x = -512; x < 512; x++)
            {
                var actualBlockCoords = Chunk.GetBlockCoords(x, 0, 0);
                var expectedBlockCoords = new Point<byte>(oldMethod(x), 0, 0);
                Assert.AreEqual(expectedBlockCoords, actualBlockCoords);
            }
        }
    }
}