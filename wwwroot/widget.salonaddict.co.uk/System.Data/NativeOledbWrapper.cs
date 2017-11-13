using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

[CLSCompliant(false)]
internal class NativeOledbWrapper
{
    internal static int modopt(IsConst) SizeOfPROPVARIANT = 0x10;

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static unsafe int modopt(IsLong) IChapteredRowsetReleaseChapter(IntPtr ptr, IntPtr chapter)
    {
        int num = -2147418113;
        uint num3 = 0;
        uint num2 = (uint) chapter.ToPointer();
        IChapteredRowset* rowsetPtr = null;
        IUnknown* unknownPtr = (IUnknown*) ptr.ToPointer();
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
        {
            num = **(*((int*) unknownPtr))(unknownPtr, &IID_IChapteredRowset, &rowsetPtr);
            if (null != rowsetPtr)
            {
                num = **(((int*) rowsetPtr))[0x10](rowsetPtr, num2, &num3);
                **(((int*) rowsetPtr))[8](rowsetPtr);
            }
        }
        return num;
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static unsafe int modopt(IsLong) ITransactionAbort(IntPtr ptr)
    {
        int num = -2147418113;
        ITransactionLocal* localPtr = null;
        IUnknown* unknownPtr = (IUnknown*) ptr.ToPointer();
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
        {
            num = **(*((int*) unknownPtr))(unknownPtr, &IID_ITransactionLocal, &localPtr);
            if (null != localPtr)
            {
                num = **(((int*) localPtr))[0x10](localPtr, 0, 0, 0);
                **(((int*) localPtr))[8](localPtr);
            }
        }
        return num;
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static unsafe int modopt(IsLong) ITransactionCommit(IntPtr ptr)
    {
        int num = -2147418113;
        ITransactionLocal* localPtr = null;
        IUnknown* unknownPtr = (IUnknown*) ptr.ToPointer();
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
        {
            num = **(*((int*) unknownPtr))(unknownPtr, &IID_ITransactionLocal, &localPtr);
            if (null != localPtr)
            {
                num = **(((int*) localPtr))[12](localPtr, 0, 2, 0);
                **(((int*) localPtr))[8](localPtr);
            }
        }
        return num;
    }

    [return: MarshalAs(UnmanagedType.U1)]
    internal static unsafe bool MemoryCompare(IntPtr buf1, IntPtr buf2, int count)
    {
        int num4;
        byte num5;
        byte num6;
        Debug.Assert(buf1 != buf2, "buf1 and buf2 are the same");
        if ((buf1.ToInt64() >= buf2.ToInt64()) && ((buf2.ToInt64() + count) > buf1.ToInt64()))
        {
            num6 = 0;
        }
        else
        {
            num6 = 1;
        }
        Debug.Assert((bool) num6, "overlapping region buf1");
        if ((buf2.ToInt64() >= buf1.ToInt64()) && ((buf1.ToInt64() + count) > buf2.ToInt64()))
        {
            num5 = 0;
        }
        else
        {
            num5 = 1;
        }
        Debug.Assert((bool) num5, "overlapping region buf2");
        byte num7 = (0 <= count) ? ((byte) 1) : ((byte) 0);
        Debug.Assert((bool) num7, "negative count");
        uint num3 = (uint) count;
        void* voidPtr2 = buf2.ToPointer();
        void* voidPtr = buf1.ToPointer();
        if (count != 0)
        {
            sbyte num2 = *((sbyte*) voidPtr);
            sbyte num = *((sbyte*) voidPtr2);
            if (num2 >= num)
            {
                do
                {
                    if (num2 > num)
                    {
                        break;
                    }
                    if (num3 == 1)
                    {
                        goto Label_00DB;
                    }
                    num3--;
                    voidPtr++;
                    voidPtr2++;
                    num2 = *((sbyte*) voidPtr);
                    num = *((sbyte*) voidPtr2);
                }
                while (num2 >= num);
            }
            num4 = 1;
            goto Label_00DE;
        }
    Label_00DB:
        num4 = 0;
    Label_00DE:
        return (bool) ((byte) num4);
    }

    internal static void MemoryCopy(IntPtr dst, IntPtr src, int count)
    {
        byte num;
        byte num2;
        Debug.Assert(dst != src, "dst and src are the same");
        if ((dst.ToInt64() >= src.ToInt64()) && ((src.ToInt64() + count) > dst.ToInt64()))
        {
            num2 = 0;
        }
        else
        {
            num2 = 1;
        }
        Debug.Assert((bool) num2, "overlapping region dst");
        if ((src.ToInt64() >= dst.ToInt64()) && ((dst.ToInt64() + count) > src.ToInt64()))
        {
            num = 0;
        }
        else
        {
            num = 1;
        }
        Debug.Assert((bool) num, "overlapping region src");
        byte num3 = (0 <= count) ? ((byte) 1) : ((byte) 0);
        Debug.Assert((bool) num3, "negative count");
        memcpy(dst.ToPointer(), src.ToPointer(), count);
    }
}

