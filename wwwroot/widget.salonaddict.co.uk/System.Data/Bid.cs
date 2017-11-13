﻿using System;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

[ComVisible(false)]
internal static class Bid
{
    private static IntPtr __defaultCmdSpace;
    private static IntPtr __noData;
    private static object _setBitsLock = new object();
    private static AutoInit ai;
    private const int BidVer = 0x23fa;
    private const uint configFlags = 0xd0000000;
    private static BindingCookie cookieObject;
    private static CtrlCB ctrlCallback;
    private const string dllName = "System.Data.dll";
    private static GCHandle hCookie;
    private static ApiGroup modFlags;
    private static IntPtr modID = internalInitialize();
    private static string modIdentity;

    internal static bool AddMetaText(string metaStr)
    {
        if (modID != NoData)
        {
            NativeMethods.AddMetaText(modID, DefaultCmdSpace, CtlCmd.AddMetaText, IntPtr.Zero, metaStr, IntPtr.Zero);
        }
        return true;
    }

    internal static bool AreOn(ApiGroup flags) => 
        ((modFlags & flags) == flags);

    [Conditional("DEBUG")]
    internal static void DASSERT(bool condition)
    {
        if (!condition)
        {
            System.Diagnostics.Trace.Assert(false);
        }
    }

    private static void deterministicStaticInit()
    {
        __noData = (IntPtr) (-1);
        __defaultCmdSpace = (IntPtr) (-1);
        modFlags = ApiGroup.Off;
        modIdentity = string.Empty;
        ctrlCallback = new CtrlCB(Bid.SetApiGroupBits);
        cookieObject = new BindingCookie();
        hCookie = GCHandle.Alloc(cookieObject, GCHandleType.Pinned);
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    private static void doneEntryPoint()
    {
        if (modID == NoData)
        {
            modFlags = ApiGroup.Off;
        }
        else
        {
            try
            {
                NativeMethods.DllBidEntryPoint(ref modID, 0, IntPtr.Zero, 0xd0000000, ref modFlags, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                NativeMethods.DllBidFinalize();
            }
            catch
            {
                modFlags = ApiGroup.Off;
            }
            finally
            {
                cookieObject.Invalidate();
                modID = NoData;
                modFlags = ApiGroup.Off;
            }
        }
    }

    [Conditional("DEBUG")]
    internal static void DTRACE(string strConst)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr) 1, strConst);
        }
    }

    [Conditional("DEBUG")]
    internal static void DTRACE(string clrFormatString, params object[] args)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr) 1, string.Format(CultureInfo.CurrentCulture, clrFormatString, args));
        }
    }

    internal static bool Enabled(string traceControlString) => 
        ((((modFlags & ApiGroup.Trace) != ApiGroup.Off) && !(modID == NoData)) && NativeMethods.Enabled(modID, UIntPtr.Zero, UIntPtr.Zero, traceControlString));

    internal static ApiGroup GetApiGroupBits(ApiGroup mask) => 
        (modFlags & mask);

    private static string getAppDomainFriendlyName()
    {
        string friendlyName = AppDomain.CurrentDomain.FriendlyName;
        if ((friendlyName != null) && (friendlyName.Length > 0))
        {
            return friendlyName;
        }
        return ("AppDomain.H" + AppDomain.CurrentDomain.GetHashCode());
    }

    internal static IntPtr GetCmdSpaceID(string textID)
    {
        if (!(modID != NoData))
        {
            return IntPtr.Zero;
        }
        return NativeMethods.GetCmdSpaceID(modID, DefaultCmdSpace, CtlCmd.CmdSpaceQuery, 0, textID, IntPtr.Zero);
    }

    private static string getIdentity(Module mod)
    {
        object[] customAttributes = mod.GetCustomAttributes(typeof(BidIdentityAttribute), true);
        if (customAttributes.Length == 0)
        {
            return mod.Name;
        }
        return ((BidIdentityAttribute) customAttributes[0]).IdentityString;
    }

    [FileIOPermission(SecurityAction.Assert, Unrestricted=true)]
    private static string getModulePath(Module mod) => 
        mod.FullyQualifiedName;

    private static void initEntryPoint()
    {
        NativeMethods.DllBidInitialize();
        Module manifestModule = Assembly.GetExecutingAssembly().ManifestModule;
        modIdentity = getIdentity(manifestModule);
        modID = NoData;
        BIDEXTINFO pExtInfo = new BIDEXTINFO(Marshal.GetHINSTANCE(manifestModule), getModulePath(manifestModule), getAppDomainFriendlyName(), hCookie.AddrOfPinnedObject());
        NativeMethods.DllBidEntryPoint(ref modID, 0x23fa, modIdentity, 0xd0000000, ref modFlags, ctrlCallback, ref pExtInfo, IntPtr.Zero, IntPtr.Zero);
        if (modID != NoData)
        {
            foreach (object obj2 in manifestModule.GetCustomAttributes(typeof(BidMetaTextAttribute), true))
            {
                AddMetaText(((BidMetaTextAttribute) obj2).MetaText);
            }
        }
    }

    private static IntPtr internalInitialize()
    {
        deterministicStaticInit();
        ai = new AutoInit();
        return modID;
    }

    internal static bool IsOn(ApiGroup flag) => 
        ((modFlags & flag) != ApiGroup.Off);

    internal static uint NewLineEx(bool addNewLine)
    {
        if (!addNewLine)
        {
            return 0;
        }
        return 1;
    }

    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, string fmtPrintfW2)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, fmtPrintfW2);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, int a2)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, string fmtPrintfW2)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, fmtPrintfW2);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, string fmtPrintfW2, string fmtPrintfW3)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, fmtPrintfW2, fmtPrintfW3);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, int a2, string fmtPrintfW2)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2, fmtPrintfW2);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, string fmtPrintfW2, int a2)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, fmtPrintfW2, a2);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, string fmtPrintfW2, string fmtPrintfW3)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, fmtPrintfW2, fmtPrintfW3);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, string fmtPrintfW2, string fmtPrintfW3, string fmtPrintfW4)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, fmtPrintfW2, fmtPrintfW3, fmtPrintfW4);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, string fmtPrintfW2, string fmtPrintfW3, int a4)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, fmtPrintfW2, fmtPrintfW3, a4);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, string fmtPrintfW2, string fmtPrintfW3, string fmtPrintfW4, int a5)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, fmtPrintfW2, fmtPrintfW3, fmtPrintfW4, a5);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void NotificationsTrace(string fmtPrintfW)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW);
        }
    }

    internal static void NotificationsTrace(string fmtPrintfW, bool a1)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1);
        }
    }

    internal static void NotificationsTrace(string fmtPrintfW, int a1)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1);
        }
    }

    internal static void NotificationsTrace(string fmtPrintfW, string fmtPrintfW2)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, fmtPrintfW2);
        }
    }

    internal static void NotificationsTrace(string fmtPrintfW, bool a1, int a2)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2);
        }
    }

    internal static void NotificationsTrace(string fmtPrintfW, bool a1, string fmtPrintfW2)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, fmtPrintfW2);
        }
    }

    internal static void NotificationsTrace(string fmtPrintfW, int a1, bool a2)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2);
        }
    }

    internal static void NotificationsTrace(string fmtPrintfW, int a1, int a2)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2);
        }
    }

    internal static void NotificationsTrace(string fmtPrintfW, int a1, string fmtPrintfW2)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, fmtPrintfW2);
        }
    }

    internal static void NotificationsTrace(string fmtPrintfW, string fmtPrintfW2, int a1)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, fmtPrintfW2, a1);
        }
    }

    internal static void NotificationsTrace(string fmtPrintfW, string fmtPrintfW2, string fmtPrintfW3, int a1)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, fmtPrintfW2, fmtPrintfW3, (long) a1);
        }
    }

    internal static void NotificationsTrace(string fmtPrintfW, bool a1, string fmtPrintfW2, string fmtPrintfW3, string fmtPrintfW4)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, fmtPrintfW2, fmtPrintfW3, fmtPrintfW4);
        }
    }

    internal static void NotificationsTrace(string fmtPrintfW, int a1, string fmtPrintfW2, string fmtPrintfW3, string fmtPrintfW4)
    {
        if (((modFlags & ApiGroup.Dependency) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, fmtPrintfW2, fmtPrintfW3, fmtPrintfW4);
        }
    }

    internal static void PoolerScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1)
    {
        if (((modFlags & ApiGroup.Pooling) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1);
        }
        else
        {
            hScp = NoData;
        }
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static void PoolerTrace(string fmtPrintfW, int a1)
    {
        if (((modFlags & ApiGroup.Pooling) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1);
        }
    }

    internal static void PoolerTrace(string fmtPrintfW, int a1, int a2)
    {
        if (((modFlags & ApiGroup.Pooling) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2);
        }
    }

    internal static void PoolerTrace(string fmtPrintfW, int a1, int a2, int a3)
    {
        if (((modFlags & ApiGroup.Pooling) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3);
        }
    }

    internal static void PoolerTrace(string fmtPrintfW, int a1, int a2, int a3, int a4)
    {
        if (((modFlags & ApiGroup.Pooling) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3, a4);
        }
    }

    internal static void PutNewLine()
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr) 2, string.Empty);
        }
    }

    internal static void PutSmartNewLine()
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr) 1, string.Empty);
        }
    }

    internal static void PutStr(string str)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr) 0, str);
        }
    }

    internal static void PutStrEx(uint flags, string str)
    {
        if (modID != NoData)
        {
            NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr) flags, str);
        }
    }

    internal static void PutStrLine(string str)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr) 1, str);
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string strConst)
    {
        if (((modFlags & ApiGroup.Scope) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, strConst);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1)
    {
        if (((modFlags & ApiGroup.Scope) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, IntPtr a1)
    {
        if (((modFlags & ApiGroup.Scope) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, string a1)
    {
        if (((modFlags & ApiGroup.Scope) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, bool a2)
    {
        if (((modFlags & ApiGroup.Scope) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, Guid a2)
    {
        if (((modFlags & ApiGroup.Scope) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, int a2)
    {
        if (((modFlags & ApiGroup.Scope) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, string a2)
    {
        if (((modFlags & ApiGroup.Scope) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, bool a2, int a3)
    {
        if (((modFlags & ApiGroup.Scope) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2, a3);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, int a2, bool a3)
    {
        if (((modFlags & ApiGroup.Scope) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2, a3);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, int a2, int a3)
    {
        if (((modFlags & ApiGroup.Scope) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2, a3);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, int a2, string a3)
    {
        if (((modFlags & ApiGroup.Scope) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2, a3);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, string a2, bool a3)
    {
        if (((modFlags & ApiGroup.Scope) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2, a3);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, string a2, int a3)
    {
        if (((modFlags & ApiGroup.Scope) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2, a3);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, int a2, bool a3, int a4)
    {
        if (((modFlags & ApiGroup.Scope) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2, a3, a4);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, int a2, int a3, string a4)
    {
        if (((modFlags & ApiGroup.Scope) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2, a3, a4);
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static void ScopeLeave(ref IntPtr hScp)
    {
        if (((modFlags & ApiGroup.Scope) != ApiGroup.Off) && (modID != NoData))
        {
            if (hScp != NoData)
            {
                NativeMethods.ScopeLeave(modID, UIntPtr.Zero, UIntPtr.Zero, ref hScp);
            }
        }
        else
        {
            hScp = NoData;
        }
    }

    internal static ApiGroup SetApiGroupBits(ApiGroup mask, ApiGroup bits)
    {
        lock (_setBitsLock)
        {
            ApiGroup modFlags = Bid.modFlags;
            if (mask != ApiGroup.Off)
            {
                Bid.modFlags ^= (bits ^ modFlags) & mask;
            }
            return modFlags;
        }
    }

    internal static void Trace(string strConst)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, strConst);
        }
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static void Trace(string fmtPrintfW, OleDbHResult a1)
    {
        if (((a1 != OleDbHResult.S_OK) || ((modFlags & ApiGroup.StatusOk) != ApiGroup.Off)) && (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData)))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, (int) a1);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1);
        }
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static void Trace(string fmtPrintfW, IntPtr a1)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1);
        }
    }

    internal static void Trace(string fmtPrintfW, string a1)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1);
        }
    }

    internal static void Trace(string fmtPrintfW, OleDbHResult a1, int a2)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, (int) a1, a2);
        }
    }

    internal static void Trace(string fmtPrintfW, OleDbHResult a1, IntPtr a2)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, (int) a1, a2);
        }
    }

    internal static void Trace(string fmtPrintfW, OleDbHResult a1, string a2)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, (int) a1, a2);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, bool a2)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, int a2)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, long a2)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, IntPtr a2)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2);
        }
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static void Trace(string fmtPrintfW, int a1, string a2)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2);
        }
    }

    internal static void Trace(string fmtPrintfW, string a1, string a2)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, int a2, bool a3)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, int a2, int a3)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, int a2, long a3)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, IntPtr a2, IntPtr a3)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, string a2, bool a3)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, string a2, int a3)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, string a2, string a3)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, int a2, int a3, int a4)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3, a4);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, int a2, long a3, int a4)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3, a4);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, int a2, string a3, string a4)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3, a4);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, long a2, int a3, int a4)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3, a4);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, string a2, string a3, int a4)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3, a4);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, int a2, string a3, string a4, int a5)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3, a4, a5);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, string a2, int a3, int a4, bool a5)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3, a4, a5);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, int a2, int a3, string a4, string a5, int a6)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3, a4, a5, a6);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, string a2, string a3, string a4, int a5, long a6)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3, a4, a5, a6);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, int a2, int a3, int a4, int a5, int a6, int a7)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3, a4, a5, a6, a7);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, int a2, long a3, uint a4, int a5, uint a6, uint a7)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3, a4, a5, a6, a7);
        }
    }

    internal static void Trace(string fmtPrintfW, int a1, int a2, int a3, int a4, string a5, string a6, string a7, int a8)
    {
        if (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, a3, a4, a5, a6, a7, a8);
        }
    }

    internal static void TraceBin(string constStrHeader, byte[] buff, ushort length)
    {
        if (modID != NoData)
        {
            if ((constStrHeader != null) && (constStrHeader.Length > 0))
            {
                NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr) 1, constStrHeader);
            }
            if (((ushort) buff.Length) < length)
            {
                length = (ushort) buff.Length;
            }
            NativeMethods.TraceBin(modID, UIntPtr.Zero, (UIntPtr) 0x10, "<Trace|BLOB> %p %u\n", buff, length);
        }
    }

    internal static void TraceBinEx(byte[] buff, ushort length)
    {
        if (modID != NoData)
        {
            if (((ushort) buff.Length) < length)
            {
                length = (ushort) buff.Length;
            }
            NativeMethods.TraceBin(modID, UIntPtr.Zero, (UIntPtr) 0x10, "<Trace|BLOB> %p %u\n", buff, length);
        }
    }

    internal static void TraceEx(uint flags, string strConst)
    {
        if (modID != NoData)
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, (UIntPtr) flags, strConst);
        }
    }

    internal static void TraceEx(uint flags, string fmtPrintfW, string a1)
    {
        if (modID != NoData)
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, (UIntPtr) flags, fmtPrintfW, a1);
        }
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static void TraceSqlReturn(string fmtPrintfW, ODBC32.RetCode a1)
    {
        if (((a1 != ODBC32.RetCode.SUCCESS) || ((modFlags & ApiGroup.StatusOk) != ApiGroup.Off)) && (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData)))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, (int) a1);
        }
    }

    internal static void TraceSqlReturn(string fmtPrintfW, ODBC32.RetCode a1, string a2)
    {
        if (((a1 != ODBC32.RetCode.SUCCESS) || ((modFlags & ApiGroup.StatusOk) != ApiGroup.Off)) && (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData)))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, (int) a1, a2);
        }
    }

    internal static void TraceSqlReturn(string fmtPrintfW, ODBC32.RetCode a1, string a2, string a3)
    {
        if (((a1 != ODBC32.RetCode.SUCCESS) || ((modFlags & ApiGroup.StatusOk) != ApiGroup.Off)) && (((modFlags & ApiGroup.Trace) != ApiGroup.Off) && (modID != NoData)))
        {
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, (int) a1, a2, a3);
        }
    }

    internal static bool AdvancedOn =>
        ((modFlags & ApiGroup.Advanced) != ApiGroup.Off);

    internal static IntPtr DefaultCmdSpace =>
        __defaultCmdSpace;

    internal static bool DefaultOn =>
        ((modFlags & ApiGroup.Default) != ApiGroup.Off);

    internal static IntPtr ID =>
        modID;

    internal static bool IsInitialized =>
        (modID != NoData);

    internal static bool MemoryOn =>
        ((modFlags & ApiGroup.Memory) != ApiGroup.Off);

    internal static IntPtr NoData =>
        __noData;

    internal static bool PerfOn =>
        ((modFlags & ApiGroup.Perf) != ApiGroup.Off);

    internal static bool ResourceOn =>
        ((modFlags & ApiGroup.Resource) != ApiGroup.Off);

    internal static bool ScopeOn =>
        ((modFlags & ApiGroup.Scope) != ApiGroup.Off);

    internal static bool StateDumpOn =>
        ((modFlags & ApiGroup.StateDump) != ApiGroup.Off);

    internal static bool StatusOkOn =>
        ((modFlags & ApiGroup.StatusOk) != ApiGroup.Off);

    internal static bool TraceOn =>
        ((modFlags & ApiGroup.Trace) != ApiGroup.Off);

    internal enum ApiGroup : uint
    {
        Advanced = 0x80,
        Default = 1,
        Dependency = 0x2000,
        MaskAll = 0xffffffff,
        MaskBid = 0xfff,
        MaskUser = 0xfffff000,
        Memory = 0x20,
        Off = 0,
        Perf = 8,
        Pooling = 0x1000,
        Resource = 0x10,
        Scope = 4,
        StateDump = 0x4000,
        StatusOk = 0x40,
        Trace = 2
    }

    private sealed class AutoInit : SafeHandle
    {
        private bool _bInitialized;

        internal AutoInit() : base(IntPtr.Zero, true)
        {
            Bid.initEntryPoint();
            this._bInitialized = true;
        }

        protected override bool ReleaseHandle()
        {
            this._bInitialized = false;
            Bid.doneEntryPoint();
            return true;
        }

        public override bool IsInvalid =>
            !this._bInitialized;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BIDEXTINFO
    {
        private IntPtr hModule;
        [MarshalAs(UnmanagedType.LPWStr)]
        private string DomainName;
        private int Reserved2;
        private int Reserved;
        [MarshalAs(UnmanagedType.LPWStr)]
        private string ModulePath;
        private IntPtr ModulePathA;
        private IntPtr pBindCookie;
        internal BIDEXTINFO(IntPtr hMod, string modPath, string friendlyName, IntPtr cookiePtr)
        {
            this.hModule = hMod;
            this.DomainName = friendlyName;
            this.Reserved2 = 0;
            this.Reserved = 0;
            this.ModulePath = modPath;
            this.ModulePathA = IntPtr.Zero;
            this.pBindCookie = cookiePtr;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private class BindingCookie
    {
        internal IntPtr _data = ((IntPtr) (-1));
        internal BindingCookie()
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal void Invalidate()
        {
            this._data = (IntPtr) (-1);
        }
    }

    private enum CtlCmd : uint
    {
        AddExtension = 0x4000001e,
        AddMetaText = 0x40000022,
        AddResHandle = 0x40000026,
        CmdSpaceCount = 0x40000000,
        CmdSpaceEnum = 0x40000004,
        CmdSpaceQuery = 0x40000008,
        CplBase = 0x60000000,
        CplMax = 0x7ffffffc,
        DcsBase = 0x40000000,
        DcsMax = 0x5ffffffc,
        GetEventID = 0x40000016,
        LastItem = 0x4000002b,
        ParseString = 0x4000001a,
        Reverse = 1,
        Shutdown = 0x4000002a,
        Unicode = 2
    }

    private delegate Bid.ApiGroup CtrlCB(Bid.ApiGroup mask, Bid.ApiGroup bits);

    [ComVisible(false), SuppressUnmanagedCodeSecurity]
    private static class NativeMethods
    {
        [DllImport("System.Data.dll", EntryPoint="DllBidCtlProc", CharSet=CharSet.Unicode)]
        internal static extern void AddMetaText(IntPtr hID, IntPtr cmdSpace, Bid.CtlCmd cmd, IntPtr nop1, string txtID, IntPtr nop2);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("System.Data.dll")]
        internal static extern void DllBidEntryPoint(ref IntPtr hID, int bInitAndVer, IntPtr unused1, uint propBits, ref Bid.ApiGroup pGblFlags, IntPtr unused2, IntPtr unused3, IntPtr unused4, IntPtr unused5);
        [DllImport("System.Data.dll", CharSet=CharSet.Ansi)]
        internal static extern void DllBidEntryPoint(ref IntPtr hID, int bInitAndVer, string sIdentity, uint propBits, ref Bid.ApiGroup pGblFlags, Bid.CtrlCB fAddr, ref Bid.BIDEXTINFO pExtInfo, IntPtr pHooks, IntPtr pHdr);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("System.Data.dll")]
        internal static extern void DllBidFinalize();
        [DllImport("System.Data.dll")]
        internal static extern void DllBidInitialize();
        [DllImport("System.Data.dll", EntryPoint="DllBidEnabledW", CharSet=CharSet.Unicode)]
        internal static extern bool Enabled(IntPtr hID, UIntPtr src, UIntPtr info, string tcs);
        [DllImport("System.Data.dll", EntryPoint="DllBidCtlProc", CharSet=CharSet.Ansi)]
        internal static extern IntPtr GetCmdSpaceID(IntPtr hID, IntPtr cmdSpace, Bid.CtlCmd cmd, uint noOp, string txtID, IntPtr NoOp2);
        [DllImport("System.Data.dll", EntryPoint="DllBidPutStrW", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        internal static extern void PutStr(IntPtr hID, UIntPtr src, UIntPtr info, string str);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string strConst);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, IntPtr a1);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, string a1);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1, bool a2);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1, [In, MarshalAs(UnmanagedType.LPStruct)] Guid a2);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1, int a2);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1, string a2);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, string a1, string a2);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1, bool a2, int a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1, int a2, bool a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1, int a2, int a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1, int a2, string a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1, string a2, bool a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1, string a2, int a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1, string a2, string a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, string a1, string a2, string a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1, int a2, bool a3, int a4);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1, int a2, int a3, string a4);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1, string a2, string a3, int a4);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeEnterCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, int a1, string a2, string a3, string a4, int a5);
        [DllImport("System.Data.dll", EntryPoint="DllBidScopeLeave")]
        internal static extern void ScopeLeave(IntPtr hID, UIntPtr src, UIntPtr info, ref IntPtr hScp);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string strConst);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, bool a1);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, IntPtr a1);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, string a1);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, bool a1, int a2);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, bool a1, string fmtPrintfW2);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, bool a2);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, int a2);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, long a2);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, IntPtr a2);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, string a2);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, string fmtPrintfW2, int a1);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, string a1, string a2);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, int a2, bool a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, int a2, int a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, int a2, long a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, IntPtr a2, IntPtr a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, string a2, bool a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, string a2, int a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, string a2, string a3);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW1, string fmtPrintfW2, string fmtPrintfW3, long a4);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, bool a1, string a2, string a3, string a4);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, int a2, int a3, int a4);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, int a2, long a3, int a4);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, int a2, string a3, string a4);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, long a2, int a3, int a4);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, string a2, string a3, int a4);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, string a2, string a3, string a4);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, int a2, string a3, string a4, int a5);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, string a2, int a3, int a4, bool a5);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, int a2, int a3, string a4, string a5, int a6);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, string a2, string a3, string a4, int a5, long a6);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, int a2, int a3, int a4, int a5, int a6, int a7);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, int a2, long a3, uint a4, int a5, uint a6, uint a7);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, int a1, int a2, int a3, int a4, string a5, string a6, string a7, int a8);
        [DllImport("System.Data.dll", EntryPoint="DllBidTraceCW", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        internal static extern void TraceBin(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, byte[] buff, ushort len);
    }
}

