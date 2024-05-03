

using System.Diagnostics;

namespace TroyServer
{
    class Timer
    {
        static DateTime time1 = DateTime.Now;
        static DateTime time2 = DateTime.Now;
        static float deltaTime;

        public static float calculate()
        {
                // This is it, use it where you want, it is time between
                // two iterations of while loop
                time2 = DateTime.Now;
                deltaTime = (time2.Ticks - time1.Ticks) / 10_000_000f; 
                time1 = time2;
                return deltaTime;
        }
    }
}