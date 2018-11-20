using System;
using System.Collections.Generic;
using System.Text;

namespace TAiO
{
    public class GoodRandom
    {
        private static readonly Random Random = new Random();
        private static readonly object SyncLock = new object();
        public static int Next(int min, int max)
        {
            lock (SyncLock)
            { 
                return Random.Next(min, max);
            }
        }

        public static int Next(int max)
        {
            lock (SyncLock)
            {
                return Random.Next(max);
            }
        }

        public static int Next()
        {
            lock (SyncLock)
            {
                return Random.Next();
            }
        }

        public static bool Bool()
        {
            return Next(2) == 0;
        }
    }
}
