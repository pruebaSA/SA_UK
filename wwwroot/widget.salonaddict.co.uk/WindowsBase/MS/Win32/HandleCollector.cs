namespace MS.Win32
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal static class HandleCollector
    {
        private static object handleMutex = new object();
        private static int handleTypeCount = 0;
        private static HandleType[] handleTypes;

        internal static IntPtr Add(IntPtr handle, int type)
        {
            handleTypes[type - 1].Add();
            return handle;
        }

        internal static SafeHandle Add(SafeHandle handle, int type)
        {
            handleTypes[type - 1].Add();
            return handle;
        }

        internal static int RegisterType(string typeName, int expense, int initialThreshold)
        {
            lock (handleMutex)
            {
                if ((handleTypeCount == 0) || (handleTypeCount == handleTypes.Length))
                {
                    HandleType[] destinationArray = new HandleType[handleTypeCount + 10];
                    if (handleTypes != null)
                    {
                        Array.Copy(handleTypes, 0, destinationArray, 0, handleTypeCount);
                    }
                    handleTypes = destinationArray;
                }
                handleTypes[handleTypeCount++] = new HandleType(typeName, expense, initialThreshold);
                return handleTypeCount;
            }
        }

        internal static IntPtr Remove(IntPtr handle, int type)
        {
            handleTypes[type - 1].Remove();
            return handle;
        }

        internal static SafeHandle Remove(SafeHandle handle, int type)
        {
            handleTypes[type - 1].Remove();
            return handle;
        }

        private class HandleType
        {
            private readonly int deltaPercent;
            private int handleCount;
            private int initialThreshHold;
            internal readonly string name;
            private int threshHold;

            internal HandleType(string name, int expense, int initialThreshHold)
            {
                this.name = name;
                this.initialThreshHold = initialThreshHold;
                this.threshHold = initialThreshHold;
                this.deltaPercent = 100 - expense;
            }

            internal void Add()
            {
                bool flag = false;
                lock (this)
                {
                    this.handleCount++;
                    flag = this.NeedCollection();
                    if (!flag)
                    {
                        return;
                    }
                }
                if (flag)
                {
                    GC.Collect();
                    int millisecondsTimeout = (100 - this.deltaPercent) / 4;
                    Thread.Sleep(millisecondsTimeout);
                }
            }

            internal bool NeedCollection()
            {
                if (this.handleCount > this.threshHold)
                {
                    this.threshHold = this.handleCount + ((this.handleCount * this.deltaPercent) / 100);
                    return true;
                }
                int num = (100 * this.threshHold) / (100 + this.deltaPercent);
                if ((num >= this.initialThreshHold) && (this.handleCount < ((int) (num * 0.9f))))
                {
                    this.threshHold = num;
                }
                return false;
            }

            internal void Remove()
            {
                lock (this)
                {
                    this.handleCount--;
                    this.handleCount = Math.Max(0, this.handleCount);
                }
            }
        }
    }
}

