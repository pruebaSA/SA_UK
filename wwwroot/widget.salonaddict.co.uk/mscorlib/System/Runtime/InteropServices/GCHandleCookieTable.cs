namespace System.Runtime.InteropServices
{
    using System;
    using System.Collections;
    using System.Threading;

    internal class GCHandleCookieTable
    {
        private const uint CookieMaskIndex = 0xffffff;
        private const uint CookieMaskSentinal = 0xff000000;
        private byte[] m_CycleCounts = new byte[10];
        private int m_FreeIndex = 1;
        private IntPtr[] m_HandleList = new IntPtr[10];
        private Hashtable m_HandleToCookieMap = new Hashtable();
        private const int MaxListSize = 0xffffff;

        internal GCHandleCookieTable()
        {
            for (int i = 0; i < 10; i++)
            {
                this.m_HandleList[i] = IntPtr.Zero;
                this.m_CycleCounts[i] = 0;
            }
        }

        internal IntPtr FindOrAddHandle(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }
            object obj2 = null;
            obj2 = this.m_HandleToCookieMap[handle];
            if (obj2 != null)
            {
                return (IntPtr) obj2;
            }
            IntPtr zero = IntPtr.Zero;
            int freeIndex = this.m_FreeIndex;
            if (((freeIndex < this.m_HandleList.Length) && (this.m_HandleList[freeIndex] == IntPtr.Zero)) && (Interlocked.CompareExchange(ref this.m_HandleList[freeIndex], handle, IntPtr.Zero) == IntPtr.Zero))
            {
                zero = this.GetCookieFromData((uint) freeIndex, this.m_CycleCounts[freeIndex]);
                if ((freeIndex + 1) < this.m_HandleList.Length)
                {
                    this.m_FreeIndex = freeIndex + 1;
                }
            }
            if (zero == IntPtr.Zero)
            {
                freeIndex = 1;
                while (freeIndex < 0xffffff)
                {
                    if ((this.m_HandleList[freeIndex] == IntPtr.Zero) && (Interlocked.CompareExchange(ref this.m_HandleList[freeIndex], handle, IntPtr.Zero) == IntPtr.Zero))
                    {
                        zero = this.GetCookieFromData((uint) freeIndex, this.m_CycleCounts[freeIndex]);
                        if ((freeIndex + 1) < this.m_HandleList.Length)
                        {
                            this.m_FreeIndex = freeIndex + 1;
                        }
                        break;
                    }
                    if ((freeIndex + 1) >= this.m_HandleList.Length)
                    {
                        lock (this)
                        {
                            if ((freeIndex + 1) >= this.m_HandleList.Length)
                            {
                                this.GrowArrays();
                            }
                        }
                    }
                    freeIndex++;
                }
            }
            if (zero == IntPtr.Zero)
            {
                throw new OutOfMemoryException(Environment.GetResourceString("OutOfMemory_GCHandleMDA"));
            }
            lock (this)
            {
                obj2 = this.m_HandleToCookieMap[handle];
                if (obj2 != null)
                {
                    this.m_HandleList[freeIndex] = IntPtr.Zero;
                    return (IntPtr) obj2;
                }
                this.m_HandleToCookieMap[handle] = zero;
            }
            return zero;
        }

        private IntPtr GetCookieFromData(uint index, byte cycleCount)
        {
            byte num = (byte) (AppDomain.CurrentDomain.Id % 0xff);
            return (IntPtr) ((((cycleCount ^ num) << 0x18) + index) + ((ulong) 1L));
        }

        private void GetDataFromCookie(IntPtr cookie, out int index, out byte xorData)
        {
            uint num = (uint) ((int) cookie);
            index = (int) ((num & 0xffffff) - 1);
            xorData = (byte) ((num & -16777216) >> 0x18);
        }

        internal IntPtr GetHandle(IntPtr cookie)
        {
            if (!this.ValidateCookie(cookie))
            {
                return IntPtr.Zero;
            }
            return this.m_HandleList[this.GetIndexFromCookie(cookie)];
        }

        private int GetIndexFromCookie(IntPtr cookie)
        {
            uint num = (uint) ((int) cookie);
            return ((((int) num) & 0xffffff) - 1);
        }

        private void GrowArrays()
        {
            int length = this.m_HandleList.Length;
            IntPtr[] destinationArray = new IntPtr[length * 2];
            byte[] buffer = new byte[length * 2];
            Array.Copy(this.m_HandleList, destinationArray, length);
            Array.Copy(this.m_CycleCounts, buffer, length);
            this.m_HandleList = destinationArray;
            this.m_CycleCounts = buffer;
        }

        internal void RemoveHandleIfPresent(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
            {
                object obj2 = this.m_HandleToCookieMap[handle];
                if (obj2 != null)
                {
                    IntPtr cookie = (IntPtr) obj2;
                    if (this.ValidateCookie(cookie))
                    {
                        int indexFromCookie = this.GetIndexFromCookie(cookie);
                        this.m_CycleCounts[indexFromCookie] = (byte) (this.m_CycleCounts[indexFromCookie] + 1);
                        this.m_HandleList[indexFromCookie] = IntPtr.Zero;
                        this.m_HandleToCookieMap.Remove(handle);
                        this.m_FreeIndex = indexFromCookie;
                    }
                }
            }
        }

        private bool ValidateCookie(IntPtr cookie)
        {
            int num;
            byte num2;
            this.GetDataFromCookie(cookie, out num, out num2);
            if (num >= 0xffffff)
            {
                return false;
            }
            if (num >= this.m_HandleList.Length)
            {
                return false;
            }
            if (this.m_HandleList[num] == IntPtr.Zero)
            {
                return false;
            }
            byte num3 = (byte) (AppDomain.CurrentDomain.Id % 0xff);
            byte num4 = (byte) (this.m_CycleCounts[num] ^ num3);
            if (num2 != num4)
            {
                return false;
            }
            return true;
        }
    }
}

