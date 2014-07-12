#region Using Statements
using EveFortressModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
#endregion

namespace EveFortressClient
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        // Used to manage screen size and render settings
        public static GraphicsDeviceManager Graphics { get; private set; }
        // Used to draw the tiles to the screen efficiently (ideally)
        public static BasicEffect BasicEffect { get; private set; }
        // Used to provide a standard access to random numbers
        public static Random Random { get; private set; }

        // Lists of each type of interaction with the game.
        // Updateables:
        //      Update loop calls the Update method on each object
        // Drawables:
        //      Draw loop calls the Draw method on each object
        // Disposables:
        //      When the app is closed normally, the Dispose method is called. This is used
        //      mainly for saving settings and such.
        // Resetables:
        //      This is called when the app loses connection and is expected to reset the
        //      game to an initial state. Deletes all connection information.
        public static List<IUpdateNeeded> Updateables = new List<IUpdateNeeded>();
        public static List<IDrawNeeded> Drawables = new List<IDrawNeeded>();
        public static List<IDisposeNeeded> Disposables = new List<IDisposeNeeded>();
        public static List<IResetNeeded> Resetables = new List<IResetNeeded>();

        // This is where globably variables are stored. This way I can automatically call
        // the methods which are required in the collections while still having "static"
        // properties and methods throught the Game class.
        public static ChatManager ChatManager { get; private set; }
        public static ChunkManager ChunkManager { get; private set; }
        public static ClientNetworkManager ClientNetworkManager { get; private set; }
        public static ClientMethods ClientMethods { get; private set; }
        public static ServerMethods ServerMethods { get; private set; }
        public static MessageParser MessageParser { get; private set; }
        public static InputManager InputManager { get; private set; }
        public static SpriteManager SpriteManager { get; private set; }
        public static TabManager TabManager { get; private set; }
        public static TileManager TileManager { get; private set; }
        public static WindowManager WindowManager { get; private set; }
        public static TimeManager TimeManager { get; private set; }

        // These are properties which are set in the loading method which are already on the
        // game object instance. This allows the singleton classes elsewhere to still access
        // them.
        public static ContentManager ContentManager { get; private set; }
        public static GameWindow GameWindow { get; private set; }

        // General initialization of properties on the class;
        public Game()
            : base()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            // Initialize global properties
            BasicEffect = new BasicEffect(GraphicsDevice);
            Random = new Random();

            // Set references to game objects
            ContentManager = Content;
            GameWindow = Window;

            // Initialize all of the singleton level classes
            TimeManager = new TimeManager();
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

        // The update loop which is pumped by xna or in this case monogame.
        static bool resetNextFrame;
        protected override void Update(GameTime gameTime)
        {
            // Simple check which queues a reset if needed
            if (resetNextFrame)
            {
                resetNextFrame = false;
                // Reset all the resetables
                foreach (var resetable in Resetables)
                {
                    resetable.Reset();
                }
            }

            // Update all of the updateables
            foreach (var updatable in Updateables)
            {
                updatable.Update();
            }
        }

        // Draw loop which is also pumped by xna and/or monogame. This might be skipped
        // if the update is taking too long.
        protected override void Draw(GameTime gameTime)
        {
            // Clear is required to let the basic effect batch properly
            GraphicsDevice.Clear(Color.Black);

            // Draw all of the drawables
            foreach (var drawable in Drawables)
            {
                drawable.Draw();
            }
        }

        // This is called when the xna/monogame game window is closed properly. Unfortuanately
        // this is not called when visual studio closes after a stop debugging is queued.
        protected override void UnloadContent()
        {
            foreach (var disposable in Disposables)
            {
                disposable.Dispose();
            }
        }

        // Simple helper method to queue a reset
        public static void QueueReset()
        {
            resetNextFrame = true;
        }
    }
}
