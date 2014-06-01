using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressClient
{
    public class VertexManager
    {
        public static Dictionary<Texture2D, VertexManager> Managers = new Dictionary<Texture2D, VertexManager>();
        public static List<Texture2D> TextureOrder = new List<Texture2D>();
        public static VertexPositionColorTexture vertexToAdd;

        static VertexManager GetManager(Texture2D texture)
        {
            VertexManager returnManager = null;
            Managers.TryGetValue(texture, out returnManager);
            if (returnManager == null)
            {
                returnManager = new VertexManager();
                Managers[texture] = returnManager;
            }

            if (!TextureOrder.Contains(texture))
            {
                TextureOrder.Add(texture);
            }
            return returnManager;
        }

        public static void AddVertex(Texture2D texture, Vector3 position, Vector2 texturePosition, Color color)
        {
            GetManager(texture).AddVertex(position, texturePosition, color);
        }

        public static void AddRectangle(Texture2D texture, Color color,
            Vector3 topLeftWorld, Vector3 topRightWorld, Vector3 bottomRightWorld, Vector3 bottomLeftWorld,
            Vector2 topLeftTexture, Vector2 topRightTexture, Vector2 bottomRightTexture, Vector2 bottomLeftTexture)
        {
            var manager = GetManager(texture);
            manager.AddRectangle(color,
                topLeftWorld, topRightWorld, bottomRightWorld, bottomLeftWorld,
                topLeftTexture, topRightTexture, bottomRightTexture, bottomLeftTexture);
        }

        public static void ClearAll()
        {
            foreach (var value in Managers.Values)
            {
                value.Clear();
                TextureOrder.Clear();
            }
        }

        public VertexPositionColorTexture[] Vertices = new VertexPositionColorTexture[4000];
        public Int16[] Indices = new Int16[4000];
        public int VertexCount { get; private set; }
        public int IndexCount { get; private set; }
        Int16 CurrentIndex { get; set; }
        int CurrentMaxVertexCount { get; set; }
        int CurrentMaxIndexCount { get; set; }

        public void AddRectangle(Color color,
            Vector3 topLeftWorld, Vector3 topRightWorld, Vector3 bottomRightWorld, Vector3 bottomLeftWorld,
            Vector2 topLeftTexture, Vector2 topRightTexture, Vector2 bottomRightTexture, Vector2 bottomLeftTexture)
        {
            AddVertex(topLeftWorld, topLeftTexture, color);
            AddVertex(topRightWorld, topRightTexture, color);
            AddVertex(bottomRightWorld, bottomRightTexture, color);
            AddVertex(bottomLeftWorld, bottomLeftTexture, color);
            AddIndex(CurrentIndex);
            AddIndex((short)(CurrentIndex + 1));
            AddIndex((short)(CurrentIndex + 2));
            AddIndex((short)(CurrentIndex + 2));
            AddIndex((short)(CurrentIndex + 3));
            AddIndex((short)(CurrentIndex));

            CurrentIndex += 4;
        }

        public void AddVertex(Vector3 position, Vector2 texturePosition, Color color)
        {
            VertexCount += 1;

            ExtendVertexArrayIfNeeded();

            vertexToAdd.Position = position;
            vertexToAdd.TextureCoordinate = texturePosition;
            vertexToAdd.Color = color;
            Vertices[VertexCount - 1] = vertexToAdd;
        }

        public void AddIndex(Int16 index)
        {
            IndexCount += 1;

            ExtendIndexArrayIfNeeded();

            Indices[IndexCount - 1] = index;
        }

        private void ExtendVertexArrayIfNeeded()
        {
            if (VertexCount > CurrentMaxVertexCount)
            {
                CurrentMaxVertexCount = VertexCount;
                if (CurrentMaxVertexCount > Vertices.Length)
                {
                    var newArray = new VertexPositionColorTexture[CurrentMaxVertexCount + 500];
                    Vertices.CopyTo(newArray, 0);
                    Vertices = newArray;
                }
            }
        }

        private void ExtendIndexArrayIfNeeded()
        {
            if (IndexCount > CurrentMaxIndexCount)
            {
                CurrentMaxIndexCount = IndexCount;
                if (CurrentMaxIndexCount > Indices.Length)
                {
                    var newArray = new Int16[CurrentMaxIndexCount + 500];
                    Indices.CopyTo(newArray, 0);
                    Indices = newArray;
                }
            }
        }

        public void Clear()
        {
            VertexCount = 0;
            IndexCount = 0;
            CurrentIndex = 0;
        }
    }
}
