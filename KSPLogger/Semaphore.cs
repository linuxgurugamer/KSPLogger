using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.Localization;


namespace KSPLogger
{
    // This Semaphore class is a bit different than the MSC Semaphore class
    // This version does not throw an exception when releasing a semaphore
    // copied from:  https://stackoverflow.com/questions/7330834/how-to-check-the-state-of-a-semaphore

    public class Semaphore
    {
        public int count = 0;
        public int limit = 0;
        private object locker = new object();


        public Semaphore(int initialCount, int maximumCount)
        {
            count = initialCount;
            limit = maximumCount;
        }

        public bool Wait(int x = -1, string s = "")
        {
#if false
            if (s != "")
                Log.Info("Wait: " + s);
#endif
            lock (locker)
            {
                while (count == 0)
                {
                    if (x >= 0)
                        return false;
                    Monitor.Wait(locker);
                }
                count--;
            }
            return true;
        }

        public bool TryRelease(string s = "")
        {
#if false
            if (s != "")
                Log.Info("TryRelease: " + s);
#endif
            lock (locker)
            {
                if (count < limit)
                {
                    count++;
                    Monitor.PulseAll(locker);
                    return true;
                }
                return false;
            }

        }
    }
}