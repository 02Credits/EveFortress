using EveFortressModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace EveFortressServer
{
    class Program
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

        static TimeSpan TimePerTick = new TimeSpan(0, 0, 0, 0, 50);

        public static List<IUpdateNeeded> Updateables = new List<IUpdateNeeded>();
        public static List<IDisposeNeeded> Disposables = new List<IDisposeNeeded>();

        public static WorldManager WorldManager;
        public static ServerMethods ServerMethods;
        public static ClientMethods ClientMethods;
        public static MessageParser MessageParser;
        public static PlayerManager PlayerManager;
        public static ServerNetworkManager ServerNetworkManager;

        static void Main(string[] args)
        {
            WorldManager = new WorldManager();
            ServerMethods = new ServerMethods();
            ClientMethods = new ClientMethods();
            MessageParser = new MessageParser();
            PlayerManager = new PlayerManager();
            ServerNetworkManager = new ServerNetworkManager();

            var hr = new HandlerRoutine(OnConsoleClose);
            SetConsoleCtrlHandler(hr, true);

            while (true)
            {
                var start = DateTime.Now;
                
                foreach (var updateable in Updateables)
                {
                    updateable.Update();
                }

                var updateTime = start - DateTime.Now;
                if (updateTime > TimePerTick)
                {
                    Thread.Sleep(TimePerTick - updateTime);
                }
            }
        }

        static bool OnConsoleClose(CtrlTypes ctrlType)
        {
            foreach(var disposable in Disposables)
            {
                disposable.Dispose();
            }
            return true;
        }
    }
}
