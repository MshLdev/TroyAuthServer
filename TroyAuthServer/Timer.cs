

using System.Diagnostics;

namespace TroyAuthServer
{
    class Timer
    {
        DateTime time1;
        DateTime time2;
        public float deltaTime;

        public Timer()
        {
            time1 = DateTime.Now;
            time2 = DateTime.Now;
            deltaTime = 0f;
        }
        
        public void calculate()
        {
                // This is it, use it where you want, it is time between
                // two iterations of while loop
                time2 = DateTime.Now;
                deltaTime = (time2.Ticks - time1.Ticks) / 10_000_000f; 
                time1 = time2;
        }
    }
    
}