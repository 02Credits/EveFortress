using EveFortressModel;
using System;

namespace EveFortressClient
{
    public class TimeManager : IUpdateNeeded, IDrawNeeded, IResetNeeded
    {
        public long Time { get; set; }

        public int FrameRate { get; set; }

        private DateTime GameStarted;
        private int frameCounter;
        private long lastFrameTime;
        private long elapsedTimeSinceCounterReset;

        public TimeManager()
        {
            GameStarted = DateTime.Now;
            Game.Updateables.Add(this);
            Game.Drawables.Add(this);
            Game.Resetables.Add(this);
        }

        public void Update()
        {
            Time = (long)(DateTime.Now - GameStarted).TotalMilliseconds;
            var frameTime = Time - lastFrameTime;
            lastFrameTime = Time;
            elapsedTimeSinceCounterReset += frameTime;

            if (elapsedTimeSinceCounterReset > 1000)
            {
                elapsedTimeSinceCounterReset -= 1000;
                FrameRate = frameCounter;
                frameCounter = 0;
            }
        }

        public void Draw()
        {
            frameCounter++;
        }

        public void Reset()
        {
            GameStarted = DateTime.Now;
        }
    }
}