#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework;
using EveFortressModel;
#endregion

namespace EveFortressClient
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager Graphics;
        public static SpriteBatch SpriteBatch;
        public static BasicEffect BasicEffect;

        public static List<IUpdateNeeded> Updateables = new List<IUpdateNeeded>();
        public static List<IDrawNeeded> Drawables = new List<IDrawNeeded>();
        public static List<IDisposeNeeded> Disposables = new List<IDisposeNeeded>();
        public static List<IResetNeeded> Resetables = new List<IResetNeeded>();

        public static ChatManager ChatManager;
        public static ChunkManager ChunkManager;
        public static ClientNetworkManager ClientNetworkManager;
        public static ClientMethods ClientMethods;
        public static ServerMethods ServerMethods;
        public static MessageParser MessageParser;
        public static InputManager InputManager;
        public static SpriteManager SpriteManager;
        public static TabManager TabManager;
        public static TileManager TileManager;
        public static WindowManager WindowManager;

        public static ContentManager ContentManager;
        public static GameWindow GameWindow;

        public static Random Random { get; set; }
        public static long Time { get; set; }

        public Game()
            : base()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Random = new Random();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            BasicEffect = new BasicEffect(GraphicsDevice);

            ContentManager = Content;
            GameWindow = Window;

            ChatManager = new ChatManager();
            ChunkManager = new ChunkManager();
            ClientNetworkManager = new ClientNetworkManager();
            ClientMethods = new ClientMethods();
            ServerMethods = new ServerMethods();
            MessageParser = new MessageParser();
            TileManager = new TileManager();
            WindowManager = new WindowManager();
            TabManager = new TabManager();
            SpriteManager = new SpriteManager();
            InputManager = new InputManager();
        }

        static bool resetNextFrame;
        protected override void Update(GameTime gameTime)
        {
            if (resetNextFrame)
            {
                resetNextFrame = false;
                foreach (var resetable in Resetables)
                {
                    resetable.Reset();
                }
            }

            foreach (var updatable in Updateables)
            {
                updatable.Update();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            foreach (var drawable in Drawables)
            {
                drawable.Draw();
            }
        }

        protected override void UnloadContent()
        {
            foreach (var disposable in Disposables)
            {
                disposable.Dispose();
            }
        }

        public static void QueueReset()
        {
            resetNextFrame = true;
        }
    }
}
