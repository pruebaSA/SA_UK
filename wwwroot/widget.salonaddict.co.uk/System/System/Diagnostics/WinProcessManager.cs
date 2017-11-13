﻿namespace System.Diagnostics
{
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.InteropServices;

    internal static class WinProcessManager
    {
        public static ModuleInfo[] GetModuleInfos(int processId)
        {
            IntPtr ptr = (IntPtr) (-1);
            GCHandle handle = new GCHandle();
            ArrayList list = new ArrayList();
            try
            {
                ptr = NativeMethods.CreateToolhelp32Snapshot(8, processId);
                if (ptr == ((IntPtr) (-1)))
                {
                    throw new Win32Exception();
                }
                int num = Marshal.SizeOf(typeof(NativeMethods.WinModuleEntry));
                int val = (num + 260) + 0x100;
                int[] numArray = new int[val / 4];
                handle = GCHandle.Alloc(numArray, GCHandleType.Pinned);
                IntPtr ptr2 = handle.AddrOfPinnedObject();
                Marshal.WriteInt32(ptr2, val);
                HandleRef ref2 = new HandleRef(null, ptr);
                if (NativeMethods.Module32First(ref2, ptr2))
                {
                    do
                    {
                        NativeMethods.WinModuleEntry structure = new NativeMethods.WinModuleEntry();
                        Marshal.PtrToStructure(ptr2, structure);
                        ModuleInfo info = new ModuleInfo {
                            baseName = Marshal.PtrToStringAnsi((IntPtr) (((long) ptr2) + num)),
                            fileName = Marshal.PtrToStringAnsi((IntPtr) ((((long) ptr2) + num) + 0x100L)),
                            baseOfDll = structure.modBaseAddr,
                            sizeOfImage = structure.modBaseSize,
                            Id = structure.th32ModuleID
                        };
                        list.Add(info);
                        Marshal.WriteInt32(ptr2, val);
                    }
                    while (NativeMethods.Module32Next(ref2, ptr2));
                }
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
                if (ptr != ((IntPtr) (-1)))
                {
                    Microsoft.Win32.SafeNativeMethods.CloseHandle(new HandleRef(null, ptr));
                }
            }
            ModuleInfo[] array = new ModuleInfo[list.Count];
            list.CopyTo(array, 0);
            return array;
        }

        public static int[] GetProcessIds()
        {
            ProcessInfo[] processInfos = GetProcessInfos();
            int[] numArray = new int[processInfos.Length];
            for (int i = 0; i < processInfos.Length; i++)
            {
                numArray[i] = processInfos[i].processId;
            }
            return numArray;
        }

        public static ProcessInfo[] GetProcessInfos()
        {
            IntPtr ptr = (IntPtr) (-1);
            GCHandle handle = new GCHandle();
            ArrayList list = new ArrayList();
            Hashtable hashtable = new Hashtable();
            try
            {
                ptr = NativeMethods.CreateToolhelp32Snapshot(6, 0);
                if (ptr == ((IntPtr) (-1)))
                {
                    throw new Win32Exception();
                }
                int num = Marshal.SizeOf(typeof(NativeMethods.WinProcessEntry));
                int val = num + 260;
                int[] numArray = new int[val / 4];
                handle = GCHandle.Alloc(numArray, GCHandleType.Pinned);
                IntPtr ptr2 = handle.AddrOfPinnedObject();
                Marshal.WriteInt32(ptr2, val);
                HandleRef ref2 = new HandleRef(null, ptr);
                if (NativeMethods.Process32First(ref2, ptr2))
                {
                    do
                    {
                        NativeMethods.WinProcessEntry entry = new NativeMethods.WinProcessEntry();
                        Marshal.PtrToStructure(ptr2, entry);
                        ProcessInfo info = new ProcessInfo();
                        string path = Marshal.PtrToStringAnsi((IntPtr) (((long) ptr2) + num));
                        info.processName = Path.ChangeExtension(Path.GetFileName(path), null);
                        info.handleCount = entry.cntUsage;
                        info.processId = entry.th32ProcessID;
                        info.basePriority = entry.pcPriClassBase;
                        info.mainModuleId = entry.th32ModuleID;
                        hashtable.Add(info.processId, info);
                        Marshal.WriteInt32(ptr2, val);
                    }
                    while (NativeMethods.Process32Next(ref2, ptr2));
                }
                NativeMethods.WinThreadEntry structure = new NativeMethods.WinThreadEntry();
                structure.dwSize = Marshal.SizeOf(structure);
                if (NativeMethods.Thread32First(ref2, structure))
                {
                    do
                    {
                        ThreadInfo info2 = new ThreadInfo {
                            threadId = structure.th32ThreadID,
                            processId = structure.th32OwnerProcessID,
                            basePriority = structure.tpBasePri,
                            currentPriority = structure.tpBasePri + structure.tpDeltaPri
                        };
                        list.Add(info2);
                    }
                    while (NativeMethods.Thread32Next(ref2, structure));
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ThreadInfo info3 = (ThreadInfo) list[i];
                    ProcessInfo info4 = (ProcessInfo) hashtable[info3.processId];
                    if (info4 != null)
                    {
                        info4.threadInfoList.Add(info3);
                    }
                }
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
                if (ptr != ((IntPtr) (-1)))
                {
                    Microsoft.Win32.SafeNativeMethods.CloseHandle(new HandleRef(null, ptr));
                }
            }
            ProcessInfo[] array = new ProcessInfo[hashtable.Values.Count];
            hashtable.Values.CopyTo(array, 0);
            return array;
        }
    }
}

