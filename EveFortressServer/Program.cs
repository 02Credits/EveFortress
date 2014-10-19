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

        public static Dictionary<Type, object> Systems = new Dictionary<Type, object>();

        public static long Time = 0;
        public static Random Random = new Random();

        private static void Main(string[] args)
        {
            AddSystem(new ChunkLoader());
            AddSystem(new ChunkManager());
            AddSystem(new ServerMethods());
            AddSystem(new ClientMethods());
            AddSystem(new MessageParser());
            AddSystem(new PlayerManager());
            AddSystem(new ServerNetworkManager());
            var entityManager = new EntitySystemManager();
            AddSystem(entityManager);

            entityManager.AddSystem(new MobileEntitySystem());

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

        public static T GetSystem<T>()
        {
            return (T)Systems[typeof(T)];
        }

        public static void AddSystem(object system)
        {
            var updateable = system as IUpdateNeeded;
            if (updateable != null)
            {
                Updateables.Add(updateable);
            }

            var disposable = system as IDisposeNeeded;
            if (disposable != null)
            {
                Disposables.Add(disposable);
            }

            Systems[system.GetType()] = system;
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