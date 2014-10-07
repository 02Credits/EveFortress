using EveFortressModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EveFortressClient
{
    public class SpriteManager : IDrawNeeded
    {
        public Vector3 TopLeftWorld;
        public Vector3 TopRightWorld;
        public Vector3 BottomRightWorld;
        public Vector3 BottomLeftWorld;
        public Vector2 TopLeftTexture;
        public Vector2 TopRightTexture;
        public Vector2 BottomRightTexture;
        public Vector2 BottomLeftTexture;

        public SpriteManager()
        {
            Game.Drawables.Add(this);
        }

        public void AddSprite(Texture2D texture, Rectangle destinationRectangle, float z, Rectangle? sourceRectangle, Color color)
        {
            var destinationLeft = destinationRectangle.X;
            var destinationRight = destinationRectangle.X + destinationRectangle.Width;
            var destinationTop = destinationRectangle.Y;
            var destinationBottom = destinationRectangle.Y + destinationRectangle.Height;

            float sourceLeft;
            float sourceRight;
            float sourceTop;
            float sourceBottom;

            if (sourceRectangle.HasValue)
            {
                var rect = sourceRectangle.Value;
                sourceLeft = (float)rect.X / texture.Width;
                sourceRight = (float)(rect.X + rect.Width) / texture.Width;
                sourceTop = (float)rect.Y / texture.Height;
                sourceBottom = (float)(rect.Y + rect.Height) / texture.Height;
            }
            else
            {
                sourceLeft = 0;
                sourceRight = 1;
                sourceTop = 0;
                sourceBottom = 1;
            }

            TopLeftWorld.X = destinationLeft;
            TopLeftWorld.Y = destinationTop;
            TopLeftWorld.Z = z;
            TopLeftTexture.X = sourceLeft;
            TopLeftTexture.Y = sourceTop;

            TopRightWorld.X = destinationRight;
            TopRightWorld.Y = destinationTop;
            TopRightWorld.Z = z;
            TopRightTexture.X = sourceRight;
            TopRightTexture.Y = sourceTop;

            BottomRightWorld.X = destinationRight;
            BottomRightWorld.Y = destinationBottom;
            BottomRightWorld.Z = z;
            BottomRightTexture.X = sourceRight;
            BottomRightTexture.Y = sourceBottom;

            BottomLeftWorld.X = destinationLeft;
            BottomLeftWorld.Y = destinationBottom;
            BottomLeftWorld.Z = z;
            BottomLeftTexture.X = sourceLeft;
            BottomLeftTexture.Y = sourceBottom;

            VertexManager.AddRectangle(texture, color,
                TopLeftWorld, TopRightWorld, BottomRightWorld, BottomLeftWorld,
                TopLeftTexture, TopRightTexture, BottomRightTexture, BottomLeftTexture);
        }

        public void Draw()
        {
            var graphics = Game.Graphics.GraphicsDevice;
            var basicEffect = Game.BasicEffect;
            graphics.BlendState = BlendState.NonPremultiplied;
            graphics.RasterizerState = RasterizerState.CullNone;
            graphics.SamplerStates[0] = SamplerState.PointClamp;
            basicEffect.TextureEnabled = true;
            basicEffect.VertexColorEnabled = true;
            basicEffect.World = Matrix.Identity;
            basicEffect.View = Matrix.Identity;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, graphics.Viewport.Width, graphics.Viewport.Height, 0, 0, 5);

            foreach (var texture in VertexManager.TextureOrder)
            {
                var manager = VertexManager.Managers[texture];
                basicEffect.Texture = texture;

                if (manager.VertexCount > 0)
                {
                    foreach (var pass in basicEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        graphics.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, manager.Vertices, 0, manager.VertexCount,
                                                                                       manager.Indices, 0, manager.IndexCount / 3);
                    }
                }
            }

            VertexManager.ClearAll();
        }
    }
}