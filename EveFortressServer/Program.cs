using EveFortressModel;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace EveFortressServer
{
    internal class Program
    {
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        private static TimeSpan TimePerTick = new TimeSpan(0, 0, 0, 0, 50);

        public static List<IUpdateNeeded> Updateables = new List<IUpdateNeeded>();
        public static List<IDisposeNeeded> Disposables = new List<IDisposeNeeded>();

        public static ChunkManager WorldManager;
        public static ServerMethods ServerMethods;
        public static ClientMethods ClientMethods;
        public static MessageParser MessageParser;
        public static PlayerManager PlayerManager;
        public static ServerNetworkManager ServerNetworkManager;
        public static EntitySystemManager EntityManager;

        public static long Time = 0;
        public static Random Random = new Random();

        private static void Main(string[] args)
        {
            WorldManager = new ChunkManager();
            ServerMethods = new ServerMethods();
            ClientMethods = new ClientMethods();
            MessageParser = new MessageParser();
            PlayerManager = new PlayerManager();
            ServerNetworkManager = new ServerNetworkManager();
            EntityManager = new EntitySystemManager();

            EntityManager.AddSystem(new SyncedEntitySystem());
            EntityManager.AddSystem(new MobileEntitySystem());

            var hr = new HandlerRoutine(OnConsoleClose);
            SetConsoleCtrlHandler(hr, true);

            while (true)
            {
                var start = DateTime.Now;

                foreach (var updateable in Updateables)
                {
                    updateable.Update();
                }

                Time += 50;
                var updateTime = start - DateTime.Now;
                if (updateTime > TimePerTick)
                {
                    Thread.Sleep(TimePerTick - updateTime);
                }
            }
        }

        private static bool OnConsoleClose(CtrlTypes ctrlType)
        {
            foreach (var disposable in Disposables)
            {
                disposable.Dispose();
            }
            return true;
        }
    }
}