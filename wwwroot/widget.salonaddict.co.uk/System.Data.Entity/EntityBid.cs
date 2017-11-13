using System;
using System.Runtime.InteropServices;
using System.Security;

[SecurityTreatAsSafe, SecurityCritical(SecurityCriticalScope.Everything)]
internal static class EntityBid
{
    private const string dllName = "System.Data.dll";

    internal static void PlanCompilerPutStr(string str)
    {
        if (PlanCompilerOn)
        {
            RawPutStrChuncked(str);
        }
    }

    internal static void PlanCompilerTrace(string fmtPrintfW, int a1, int a2)
    {
        if (PlanCompilerOn)
        {
            NativeMethods.Trace(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, (long) a2);
        }
    }

    internal static void PutStr(string str)
    {
        if (Bid.TraceOn && Bid.IsInitialized)
        {
            NativeMethods.PutStr(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, str);
        }
    }

    internal static void PutStrChunked(string str)
    {
        if (Bid.TraceOn && Bid.IsInitialized)
        {
            RawPutStrChuncked(str);
        }
    }

    private static void RawPutStrChuncked(string str)
    {
        for (int i = 0; i < str.Length; i += 0x3e8)
        {
            NativeMethods.PutStr(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, str.Substring(i, Math.Min(str.Length - i, 0x3e8)));
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string strConst)
    {
        if (Bid.ScopeOn && Bid.IsInitialized)
        {
            NativeMethods.ScopeEnter(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, out hScp, strConst);
        }
        else
        {
            hScp = Bid.NoData;
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1)
    {
        if (Bid.ScopeOn && Bid.IsInitialized)
        {
            NativeMethods.ScopeEnter(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1);
        }
        else
        {
            hScp = Bid.NoData;
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, bool a2)
    {
        if (Bid.ScopeOn && Bid.IsInitialized)
        {
            NativeMethods.ScopeEnter(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2);
        }
        else
        {
            hScp = Bid.NoData;
        }
    }

    internal static void ScopeLeave(ref IntPtr hScp)
    {
        Bid.ScopeLeave(ref hScp);
    }

    internal static void Trace(string strConst)
    {
        if (Bid.TraceOn && Bid.IsInitialized)
        {
            NativeMethods.Trace(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, strConst);
        }
    }

    internal static void Trace(string fmtPrintfW, bool a1)
    {
        if (Bid.TraceOn && Bid.IsInitialized)
        {
            NativeMethods.Trace(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1)
    {
        if (Bid.TraceOn && Bid.IsInitialized)
        {
            NativeMethods.Trace(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1);
        }
    }

    internal static void Trace(string fmtPrintfW, string a1)
    {
        if (Bid.TraceOn && Bid.IsInitialized)
        {
            NativeMethods.Trace(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, bool a2)
    {
        if (Bid.TraceOn && Bid.IsInitialized)
        {
            NativeMethods.Trace(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, int a2)
    {
        if (Bid.TraceOn && Bid.IsInitialized)
        {
            NativeMethods.Trace(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, (long) a2);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, string a2)
    {
        if (Bid.TraceOn && Bid.IsInitialized)
        {
            NativeMethods.Trace(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2);
        }
    }

    internal static void Trace(string fmtPrintfW, string a1, string a2)
    {
        if (Bid.TraceOn && Bid.IsInitialized)
        {
            NativeMethods.Trace(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, int a2, int a3)
    {
        if (Bid.TraceOn && Bid.IsInitialized)
        {
            NativeMethods.Trace(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3);
        }
    }

    internal static bool AdvancedOn =>
        Bid.AdvancedOn;

    internal static bool PlanCompilerOn =>
        (Bid.IsOn((Bid.ApiGroup) 0x8000) && Bid.IsInitialized);

    internal static bool TraceOn =>
        Bid.TraceOn;

    [SuppressUnmanagedCodeSecurity]
    private static class NativeMethods
    {
        [DllImport("System.Data.dll", EntryPoint="DllBidPutStrW", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        internal static extern void PutStr(IntPtr hID, UIntPtr src, UIntPtr info, string str);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string strConst);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1, bool a2);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1, int a2);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeLeave")]
        internal static extern void ScopeLeave(IntPtr hID, UIntPtr src, UIntPtr info, ref IntPtr hScp);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string strConst);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, bool a1);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, string a1);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, bool a2);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, long a2);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, string a2);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, string a1, string a2);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, int a2, int a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, int a2, string a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, string a2, string a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, int a2, int a3, int a4);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, int a2, string a3, string a4);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ScopeAuto : IDisposable
    {
        private IntPtr _hscp;
        internal ScopeAuto(string fmtPrintfW, int arg)
        {
            if (Bid.ScopeOn && Bid.IsInitialized)
            {
                EntityBid.NativeMethods.ScopeEnter(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, out this._hscp, fmtPrintfW, arg);
            }
            else
            {
                this._hscp = Bid.NoData;
            }
        }

        internal ScopeAuto(string fmtPrintfW, int a1, int a2)
        {
            if (Bid.ScopeOn && Bid.IsInitialized)
            {
                EntityBid.NativeMethods.ScopeEnter(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, out this._hscp, fmtPrintfW, a1, a2);
            }
            else
            {
                this._hscp = Bid.NoData;
            }
        }

        public void Dispose()
        {
            if ((Bid.ScopeOn && Bid.IsInitialized) && (this._hscp != Bid.NoData))
            {
                EntityBid.NativeMethods.ScopeLeave(Bid.ID, UIntPtr.Zero, UIntPtr.Zero, ref this._hscp);
            }
        }
    }
}

