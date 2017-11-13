namespace System.Runtime.InteropServices
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices.ComTypes;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;

    [SuppressUnmanagedCodeSecurity]
    public static class Marshal
    {
        private static readonly IntPtr HIWORDMASK = new IntPtr(-65536L);
        private static Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");
        private const int LMEM_FIXED = 0;
        private const int LMEM_MOVEABLE = 2;
        private const string s_strConvertedTypeInfoAssemblyDesc = "Type dynamically generated from ITypeInfo's";
        private const string s_strConvertedTypeInfoAssemblyName = "InteropDynamicTypes";
        private const string s_strConvertedTypeInfoAssemblyTitle = "Interop Dynamic Types";
        private const string s_strConvertedTypeInfoNameSpace = "InteropDynamicTypes";
        public static readonly int SystemDefaultCharSize;
        public static readonly int SystemMaxDBCSCharSize;

        static Marshal()
        {
            sbyte[] ptr = new sbyte[4];
            ptr[0] = 0x41;
            ptr[1] = 0x41;
            SystemDefaultCharSize = 3 - Win32Native.lstrlen(ptr);
            SystemMaxDBCSCharSize = GetSystemMaxDBCSCharSize();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void _GetTypeLibVersionForAssembly(Assembly inputAssembly, out int majorVersion, out int minorVersion);
        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern int AddRef(IntPtr pUnk);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr AllocCoTaskMem(int cb)
        {
            IntPtr ptr = Win32Native.CoTaskMemAlloc(cb);
            if (ptr == Win32Native.NULL)
            {
                throw new OutOfMemoryException();
            }
            return ptr;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr AllocHGlobal(int cb) => 
            AllocHGlobal((IntPtr) cb);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr AllocHGlobal(IntPtr cb)
        {
            IntPtr ptr = Win32Native.LocalAlloc_NoSafeHandle(0, cb);
            if (ptr == Win32Native.NULL)
            {
                throw new OutOfMemoryException();
            }
            return ptr;
        }

        [DllImport("ole32.dll", PreserveSig=false)]
        private static extern void BindMoniker(IMoniker pmk, uint grfOpt, ref Guid iidResult, [MarshalAs(UnmanagedType.Interface)] out object ppvResult);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static object BindToMoniker(string monikerName)
        {
            object ppvResult = null;
            IBindCtx ppbc = null;
            uint num;
            CreateBindCtx(0, out ppbc);
            IMoniker ppmk = null;
            MkParseDisplayName(ppbc, monikerName, out num, out ppmk);
            BindMoniker(ppmk, 0, ref IID_IUnknown, out ppvResult);
            return ppvResult;
        }

        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern void ChangeWrapperHandleStrength(object otp, bool fIsWeak);
        [DllImport("ole32.dll", PreserveSig=false)]
        private static extern void CLSIDFromProgID([MarshalAs(UnmanagedType.LPWStr)] string progId, out Guid clsid);
        [DllImport("ole32.dll", PreserveSig=false)]
        private static extern void CLSIDFromProgIDEx([MarshalAs(UnmanagedType.LPWStr)] string progId, out Guid clsid);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Copy(IntPtr source, byte[] destination, int startIndex, int length)
        {
            CopyToManaged(source, destination, startIndex, length);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Copy(IntPtr source, char[] destination, int startIndex, int length)
        {
            CopyToManaged(source, destination, startIndex, length);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Copy(IntPtr source, double[] destination, int startIndex, int length)
        {
            CopyToManaged(source, destination, startIndex, length);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Copy(IntPtr source, short[] destination, int startIndex, int length)
        {
            CopyToManaged(source, destination, startIndex, length);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Copy(IntPtr source, int[] destination, int startIndex, int length)
        {
            CopyToManaged(source, destination, startIndex, length);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Copy(IntPtr source, long[] destination, int startIndex, int length)
        {
            CopyToManaged(source, destination, startIndex, length);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Copy(IntPtr source, IntPtr[] destination, int startIndex, int length)
        {
            CopyToManaged(source, destination, startIndex, length);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Copy(IntPtr source, float[] destination, int startIndex, int length)
        {
            CopyToManaged(source, destination, startIndex, length);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Copy(byte[] source, int startIndex, IntPtr destination, int length)
        {
            CopyToNative(source, startIndex, destination, length);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Copy(char[] source, int startIndex, IntPtr destination, int length)
        {
            CopyToNative(source, startIndex, destination, length);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Copy(double[] source, int startIndex, IntPtr destination, int length)
        {
            CopyToNative(source, startIndex, destination, length);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Copy(short[] source, int startIndex, IntPtr destination, int length)
        {
            CopyToNative(source, startIndex, destination, length);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Copy(int[] source, int startIndex, IntPtr destination, int length)
        {
            CopyToNative(source, startIndex, destination, length);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Copy(long[] source, int startIndex, IntPtr destination, int length)
        {
            CopyToNative(source, startIndex, destination, length);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Copy(IntPtr[] source, int startIndex, IntPtr destination, int length)
        {
            CopyToNative(source, startIndex, destination, length);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Copy(float[] source, int startIndex, IntPtr destination, int length)
        {
            CopyToNative(source, startIndex, destination, length);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void CopyToManaged(IntPtr source, object destination, int startIndex, int length);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void CopyToNative(object source, int startIndex, IntPtr destination, int length);
        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern IntPtr CreateAggregatedObject(IntPtr pOuter, object o);
        [DllImport("ole32.dll", PreserveSig=false)]
        private static extern void CreateBindCtx(uint reserved, out IBindCtx ppbc);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static object CreateWrapperOfType(object o, Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }
            if (!t.IsCOMObject)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_TypeNotComObject"), "t");
            }
            if (t.IsGenericType)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_NeedNonGenericType"), "t");
            }
            if (o == null)
            {
                return null;
            }
            if (!o.GetType().IsCOMObject)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_ObjNotComObject"), "o");
            }
            if (o.GetType() == t)
            {
                return o;
            }
            object comObjectData = GetComObjectData(o, t);
            if (comObjectData == null)
            {
                comObjectData = InternalCreateWrapperOfType(o, t);
                if (!SetComObjectData(o, t, comObjectData))
                {
                    comObjectData = GetComObjectData(o, t);
                }
            }
            return comObjectData;
        }

        [MethodImpl(MethodImplOptions.InternalCall), ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern void DestroyStructure(IntPtr ptr, Type structuretype);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void FCallGenerateGuidForType(ref Guid result, Type type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void FCallGetTypeInfoGuid(ref Guid result, ITypeInfo typeInfo);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void FCallGetTypeLibGuid(ref Guid result, ITypeLib pTLB);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void FCallGetTypeLibGuidForAssembly(ref Guid result, Assembly asm);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static int FinalReleaseComObject(object o)
        {
            __ComObject obj2 = null;
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }
            try
            {
                obj2 = (__ComObject) o;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_ObjNotComObject"), "o");
            }
            obj2.FinalReleaseSelf();
            return 0;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void FreeBSTR(IntPtr ptr)
        {
            if (IsNotWin32Atom(ptr))
            {
                Win32Native.SysFreeString(ptr);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void FreeCoTaskMem(IntPtr ptr)
        {
            if (IsNotWin32Atom(ptr))
            {
                Win32Native.CoTaskMemFree(ptr);
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void FreeHGlobal(IntPtr hglobal)
        {
            if (IsNotWin32Atom(hglobal) && (Win32Native.NULL != Win32Native.LocalFree(hglobal)))
            {
                ThrowExceptionForHR(GetHRForLastWin32Error());
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static Guid GenerateGuidForType(Type type)
        {
            Guid result = new Guid();
            FCallGenerateGuidForType(ref result, type);
            return result;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static string GenerateProgIdForType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!RegistrationServices.TypeRequiresRegistrationHelper(type))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_TypeMustBeComCreatable"), "type");
            }
            if (type.IsImport)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_TypeMustNotBeComImport"), "type");
            }
            if (type.IsGenericType)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_NeedNonGenericType"), "type");
            }
            IList<CustomAttributeData> customAttributes = CustomAttributeData.GetCustomAttributes(type);
            for (int i = 0; i < customAttributes.Count; i++)
            {
                if (customAttributes[i].Constructor.DeclaringType == typeof(ProgIdAttribute))
                {
                    CustomAttributeTypedArgument argument = customAttributes[i].ConstructorArguments[0];
                    string str = (string) argument.Value;
                    if (str == null)
                    {
                        str = string.Empty;
                    }
                    return str;
                }
            }
            return type.FullName;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static object GetActiveObject(string progID)
        {
            object ppunk = null;
            Guid guid;
            try
            {
                CLSIDFromProgIDEx(progID, out guid);
            }
            catch (Exception)
            {
                CLSIDFromProgID(progID, out guid);
            }
            GetActiveObject(ref guid, IntPtr.Zero, out ppunk);
            return ppunk;
        }

        [DllImport("oleaut32.dll", PreserveSig=false)]
        private static extern void GetActiveObject(ref Guid rclsid, IntPtr reserved, [MarshalAs(UnmanagedType.Interface)] out object ppunk);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr GetComInterfaceForObject(object o, Type T) => 
            GetComInterfaceForObjectNative(o, T, false);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr GetComInterfaceForObjectInContext(object o, Type t) => 
            GetComInterfaceForObjectNative(o, t, true);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern IntPtr GetComInterfaceForObjectNative(object o, Type t, bool onlyInContext);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static object GetComObjectData(object obj, object key)
        {
            __ComObject obj2 = null;
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            try
            {
                obj2 = (__ComObject) obj;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_ObjNotComObject"), "obj");
            }
            return obj2.GetData(key);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static int GetComSlotForMethodInfo(MemberInfo m)
        {
            if (m == null)
            {
                throw new ArgumentNullException("m");
            }
            if (!(m is RuntimeMethodInfo))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeMethodInfo"), "m");
            }
            if (!m.DeclaringType.IsInterface)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustBeInterfaceMethod"), "m");
            }
            if (m.DeclaringType.IsGenericType)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_NeedNonGenericType"), "m");
            }
            return InternalGetComSlotForMethodInfo(((RuntimeMethodInfo) m).GetMethodHandle());
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static Delegate GetDelegateForFunctionPointer(IntPtr ptr, Type t)
        {
            if (ptr == IntPtr.Zero)
            {
                throw new ArgumentNullException("ptr");
            }
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }
            if (!(t is RuntimeType))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "t");
            }
            if (t.IsGenericType)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_NeedNonGenericType"), "t");
            }
            Type baseType = t.BaseType;
            if ((baseType == null) || ((baseType != typeof(Delegate)) && (baseType != typeof(MulticastDelegate))))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "t");
            }
            return GetDelegateForFunctionPointerInternal(ptr, t);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Delegate GetDelegateForFunctionPointerInternal(IntPtr ptr, Type t);
        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern int GetEndComSlot(Type t);
        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern int GetExceptionCode();
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static Exception GetExceptionForHR(int errorCode)
        {
            if (errorCode < 0)
            {
                return GetExceptionForHRInternal(errorCode, Win32Native.NULL);
            }
            return null;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static Exception GetExceptionForHR(int errorCode, IntPtr errorInfo)
        {
            if (errorCode < 0)
            {
                return GetExceptionForHRInternal(errorCode, errorInfo);
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Exception GetExceptionForHRInternal(int errorCode, IntPtr errorInfo);
        [MethodImpl(MethodImplOptions.InternalCall), ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern IntPtr GetExceptionPointers();
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr GetFunctionPointerForDelegate(Delegate d)
        {
            if (d == null)
            {
                throw new ArgumentNullException("d");
            }
            return GetFunctionPointerForDelegateInternal(d);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern IntPtr GetFunctionPointerForDelegateInternal(Delegate d);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr GetHINSTANCE(Module m) => 
            m?.GetHINSTANCE();

        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern int GetHRForException(Exception e);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static int GetHRForLastWin32Error()
        {
            int num = GetLastWin32Error();
            if ((num & 0x80000000L) == 0x80000000L)
            {
                return num;
            }
            return ((num & 0xffff) | -2147024896);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr GetIDispatchForObject(object o) => 
            GetIDispatchForObjectNative(o, false);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr GetIDispatchForObjectInContext(object o) => 
            GetIDispatchForObjectNative(o, true);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern IntPtr GetIDispatchForObjectNative(object o, bool onlyInContext);
        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern IntPtr GetITypeInfoForType(Type t);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr GetIUnknownForObject(object o) => 
            GetIUnknownForObjectNative(o, false);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr GetIUnknownForObjectInContext(object o) => 
            GetIUnknownForObjectNative(o, true);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern IntPtr GetIUnknownForObjectNative(object o, bool onlyInContext);
        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern int GetLastWin32Error();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Type GetLoadedTypeForGUID(ref Guid guid);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("The GetManagedThunkForUnmanagedMethodPtr method has been deprecated and will be removed in a future release.", false), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern IntPtr GetManagedThunkForUnmanagedMethodPtr(IntPtr pfnMethodToWrap, IntPtr pbSignature, int cbSignature);
        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern MemberInfo GetMethodInfoForComSlot(Type t, int slot, ref ComMemberType memberType);
        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern void GetNativeVariantForObject(object obj, IntPtr pDstNativeVariant);
        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern object GetObjectForIUnknown(IntPtr pUnk);
        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern object GetObjectForNativeVariant(IntPtr pSrcNativeVariant);
        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern object[] GetObjectsForNativeVariants(IntPtr aSrcNativeVariant, int cVars);
        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern int GetStartComSlot(Type t);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int GetSystemMaxDBCSCharSize();
        [Obsolete("The GetThreadFromFiberCookie method has been deprecated.  Use the hosting API to perform this operation.", false), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static Thread GetThreadFromFiberCookie(int cookie)
        {
            if (cookie == 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_ArgumentZero"), "cookie");
            }
            return InternalGetThreadFromFiberCookie(cookie);
        }

        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern object GetTypedObjectForIUnknown(IntPtr pUnk, Type t);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static Type GetTypeForITypeInfo(IntPtr piTypeInfo)
        {
            ITypeInfo typeInfo = null;
            ITypeLib ppTLB = null;
            Type loadedTypeForGUID = null;
            Assembly assembly = null;
            int pIndex = 0;
            if (piTypeInfo == Win32Native.NULL)
            {
                return null;
            }
            typeInfo = (ITypeInfo) GetObjectForIUnknown(piTypeInfo);
            loadedTypeForGUID = GetLoadedTypeForGUID(ref GetTypeInfoGuid(typeInfo));
            if (loadedTypeForGUID != null)
            {
                return loadedTypeForGUID;
            }
            try
            {
                typeInfo.GetContainingTypeLib(out ppTLB, out pIndex);
            }
            catch (COMException)
            {
                ppTLB = null;
            }
            if (ppTLB != null)
            {
                string fullName = TypeLibConverter.GetAssemblyNameFromTypelib(ppTLB, null, null, null, null, AssemblyNameFlags.None).FullName;
                Assembly[] assemblies = Thread.GetDomain().GetAssemblies();
                int length = assemblies.Length;
                for (int i = 0; i < length; i++)
                {
                    if (string.Compare(assemblies[i].FullName, fullName, StringComparison.Ordinal) == 0)
                    {
                        assembly = assemblies[i];
                    }
                }
                if (assembly == null)
                {
                    assembly = new TypeLibConverter().ConvertTypeLibToAssembly(ppTLB, GetTypeLibName(ppTLB) + ".dll", TypeLibImporterFlags.None, new ImporterCallback(), null, null, null, null);
                }
                loadedTypeForGUID = assembly.GetType(GetTypeLibName(ppTLB) + "." + GetTypeInfoName(typeInfo), true, false);
                if ((loadedTypeForGUID != null) && !loadedTypeForGUID.IsVisible)
                {
                    loadedTypeForGUID = null;
                }
                return loadedTypeForGUID;
            }
            return typeof(object);
        }

        internal static Guid GetTypeInfoGuid(ITypeInfo typeInfo)
        {
            Guid result = new Guid();
            FCallGetTypeInfoGuid(ref result, typeInfo);
            return result;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static string GetTypeInfoName(ITypeInfo typeInfo)
        {
            string strName = null;
            string strDocString = null;
            int dwHelpContext = 0;
            string strHelpFile = null;
            if (typeInfo == null)
            {
                throw new ArgumentNullException("typeInfo");
            }
            typeInfo.GetDocumentation(-1, out strName, out strDocString, out dwHelpContext, out strHelpFile);
            return strName;
        }

        [Obsolete("Use System.Runtime.InteropServices.Marshal.GetTypeInfoName(ITypeInfo pTLB) instead. http://go.microsoft.com/fwlink/?linkid=14202&ID=0000011.", false), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static string GetTypeInfoName(UCOMITypeInfo pTI) => 
            GetTypeInfoName((ITypeInfo) pTI);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static Guid GetTypeLibGuid(ITypeLib typelib)
        {
            Guid result = new Guid();
            FCallGetTypeLibGuid(ref result, typelib);
            return result;
        }

        [Obsolete("Use System.Runtime.InteropServices.Marshal.GetTypeLibGuid(ITypeLib pTLB) instead. http://go.microsoft.com/fwlink/?linkid=14202&ID=0000011.", false), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static Guid GetTypeLibGuid(UCOMITypeLib pTLB) => 
            GetTypeLibGuid((ITypeLib) pTLB);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static Guid GetTypeLibGuidForAssembly(Assembly asm)
        {
            Guid result = new Guid();
            FCallGetTypeLibGuidForAssembly(ref result, asm?.InternalAssembly);
            return result;
        }

        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern int GetTypeLibLcid(ITypeLib typelib);
        [Obsolete("Use System.Runtime.InteropServices.Marshal.GetTypeLibLcid(ITypeLib pTLB) instead. http://go.microsoft.com/fwlink/?linkid=14202&ID=0000011.", false), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static int GetTypeLibLcid(UCOMITypeLib pTLB) => 
            GetTypeLibLcid((ITypeLib) pTLB);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static string GetTypeLibName(ITypeLib typelib)
        {
            string strName = null;
            string strDocString = null;
            int dwHelpContext = 0;
            string strHelpFile = null;
            if (typelib == null)
            {
                throw new ArgumentNullException("typelib");
            }
            typelib.GetDocumentation(-1, out strName, out strDocString, out dwHelpContext, out strHelpFile);
            return strName;
        }

        [Obsolete("Use System.Runtime.InteropServices.Marshal.GetTypeLibName(ITypeLib pTLB) instead. http://go.microsoft.com/fwlink/?linkid=14202&ID=0000011.", false), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static string GetTypeLibName(UCOMITypeLib pTLB) => 
            GetTypeLibName((ITypeLib) pTLB);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void GetTypeLibVersion(ITypeLib typeLibrary, out int major, out int minor);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void GetTypeLibVersionForAssembly(Assembly inputAssembly, out int majorVersion, out int minorVersion)
        {
            _GetTypeLibVersionForAssembly(inputAssembly?.InternalAssembly, out majorVersion, out minorVersion);
        }

        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern object GetUniqueObjectForIUnknown(IntPtr unknown);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("The GetUnmanagedThunkForManagedMethodPtr method has been deprecated and will be removed in a future release.", false), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern IntPtr GetUnmanagedThunkForManagedMethodPtr(IntPtr pfnMethodToWrap, IntPtr pbSignature, int cbSignature);
        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        private static extern object InternalCreateWrapperOfType(object o, Type t);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void InternalFinalReleaseComObject(object o);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int InternalGetComSlotForMethodInfo(RuntimeMethodHandle m);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Thread InternalGetThreadFromFiberCookie(int cookie);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int InternalNumParamBytes(IntPtr m);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void InternalPrelink(IntPtr m);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int InternalReleaseComObject(object o);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool InternalSwitchCCW(object oldtp, object newtp);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern object InternalWrapIUnknownWithComObject(IntPtr i);
        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern bool IsComObject(object o);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static bool IsNotWin32Atom(IntPtr ptr)
        {
            long num = (long) ptr;
            return (0L != (num & ((long) HIWORDMASK)));
        }

        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern bool IsTypeVisibleFromCom(Type t);
        private static bool IsWin32Atom(IntPtr ptr)
        {
            long num = (long) ptr;
            return (0L == (num & ((long) HIWORDMASK)));
        }

        private static RuntimeTypeHandle LoadLicenseManager()
        {
            Type type = Assembly.Load("System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089").GetType("System.ComponentModel.LicenseManager");
            if ((type != null) && type.IsVisible)
            {
                return type.TypeHandle;
            }
            return RuntimeTypeHandle.EmptyHandle;
        }

        [DllImport("ole32.dll", PreserveSig=false)]
        private static extern void MkParseDisplayName(IBindCtx pbc, [MarshalAs(UnmanagedType.LPWStr)] string szUserName, out uint pchEaten, out IMoniker ppmk);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static int NumParamBytes(MethodInfo m)
        {
            if (m == null)
            {
                throw new ArgumentNullException("m");
            }
            if (!(m is RuntimeMethodInfo))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeMethodInfo"));
            }
            return InternalNumParamBytes(m.GetMethodHandle().Value);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr OffsetOf(Type t, string fieldName)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }
            FieldInfo field = t.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_OffsetOfFieldNotFound", new object[] { t.FullName }), "fieldName");
            }
            if (!(field is RuntimeFieldInfo))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeFieldInfo"), "fieldName");
            }
            return OffsetOfHelper(((RuntimeFieldInfo) field).GetFieldHandle().Value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern IntPtr OffsetOfHelper(IntPtr f);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void Prelink(MethodInfo m)
        {
            if (m == null)
            {
                throw new ArgumentNullException("m");
            }
            if (!(m is RuntimeMethodInfo))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeMethodInfo"));
            }
            InternalPrelink(m.MethodHandle.Value);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void PrelinkAll(Type c)
        {
            if (c == null)
            {
                throw new ArgumentNullException("c");
            }
            MethodInfo[] methods = c.GetMethods();
            if (methods != null)
            {
                for (int i = 0; i < methods.Length; i++)
                {
                    Prelink(methods[i]);
                }
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static string PtrToStringAnsi(IntPtr ptr)
        {
            if (Win32Native.NULL == ptr)
            {
                return null;
            }
            if (IsWin32Atom(ptr))
            {
                return null;
            }
            int capacity = Win32Native.lstrlenA(ptr);
            if (capacity == 0)
            {
                return string.Empty;
            }
            StringBuilder pdst = new StringBuilder(capacity);
            Win32Native.CopyMemoryAnsi(pdst, ptr, new IntPtr(1 + capacity));
            return pdst.ToString();
        }

        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern string PtrToStringAnsi(IntPtr ptr, int len);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static string PtrToStringAuto(IntPtr ptr)
        {
            if (Win32Native.NULL == ptr)
            {
                return null;
            }
            if (IsWin32Atom(ptr))
            {
                return null;
            }
            StringBuilder dst = new StringBuilder(Win32Native.lstrlen(ptr));
            Win32Native.lstrcpy(dst, ptr);
            return dst.ToString();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static string PtrToStringAuto(IntPtr ptr, int len)
        {
            if (SystemDefaultCharSize != 1)
            {
                return PtrToStringUni(ptr, len);
            }
            return PtrToStringAnsi(ptr, len);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static string PtrToStringBSTR(IntPtr ptr) => 
            PtrToStringUni(ptr, Win32Native.SysStringLen(ptr));

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static string PtrToStringUni(IntPtr ptr)
        {
            if (Win32Native.NULL == ptr)
            {
                return null;
            }
            if (IsWin32Atom(ptr))
            {
                return null;
            }
            int capacity = Win32Native.lstrlenW(ptr);
            StringBuilder pdst = new StringBuilder(capacity);
            Win32Native.CopyMemoryUni(pdst, ptr, new IntPtr(2 * (1 + capacity)));
            return pdst.ToString();
        }

        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern string PtrToStringUni(IntPtr ptr, int len);
        [ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void PtrToStructure(IntPtr ptr, object structure)
        {
            PtrToStructureHelper(ptr, structure, false);
        }

        [ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static object PtrToStructure(IntPtr ptr, Type structureType)
        {
            if (ptr == Win32Native.NULL)
            {
                return null;
            }
            if (structureType == null)
            {
                throw new ArgumentNullException("structureType");
            }
            if (structureType.IsGenericType)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_NeedNonGenericType"), "structureType");
            }
            object structure = Activator.InternalCreateInstanceWithNoMemberAccessCheck(structureType, true);
            PtrToStructureHelper(ptr, structure, true);
            return structure;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void PtrToStructureHelper(IntPtr ptr, object structure, bool allowValueClasses);
        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern int QueryInterface(IntPtr pUnk, ref Guid iid, out IntPtr ppv);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static byte ReadByte(IntPtr ptr) => 
            ReadByte(ptr, 0);

        [DllImport("mscoree.dll", EntryPoint="ND_RU1")]
        public static extern byte ReadByte(IntPtr ptr, int ofs);
        [DllImport("mscoree.dll", EntryPoint="ND_RU1")]
        public static extern byte ReadByte([In, MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static short ReadInt16(IntPtr ptr) => 
            ReadInt16(ptr, 0);

        [DllImport("mscoree.dll", EntryPoint="ND_RI2")]
        public static extern short ReadInt16(IntPtr ptr, int ofs);
        [DllImport("mscoree.dll", EntryPoint="ND_RI2")]
        public static extern short ReadInt16([In, MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static int ReadInt32(IntPtr ptr) => 
            ReadInt32(ptr, 0);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("mscoree.dll", EntryPoint="ND_RI4")]
        public static extern int ReadInt32(IntPtr ptr, int ofs);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("mscoree.dll", EntryPoint="ND_RI4")]
        public static extern int ReadInt32([In, MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static long ReadInt64(IntPtr ptr) => 
            ReadInt64(ptr, 0);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("mscoree.dll", EntryPoint="ND_RI8")]
        public static extern long ReadInt64(IntPtr ptr, int ofs);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("mscoree.dll", EntryPoint="ND_RI8")]
        public static extern long ReadInt64([In, MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr ReadIntPtr(IntPtr ptr) => 
            ((IntPtr) ReadInt32(ptr, 0));

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr ReadIntPtr(IntPtr ptr, int ofs) => 
            ((IntPtr) ReadInt32(ptr, ofs));

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr ReadIntPtr([In, MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs) => 
            ((IntPtr) ReadInt32(ptr, ofs));

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr ReAllocCoTaskMem(IntPtr pv, int cb)
        {
            IntPtr ptr = Win32Native.CoTaskMemRealloc(pv, cb);
            if ((ptr == Win32Native.NULL) && (cb != 0))
            {
                throw new OutOfMemoryException();
            }
            return ptr;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr ReAllocHGlobal(IntPtr pv, IntPtr cb)
        {
            IntPtr ptr = Win32Native.LocalReAlloc(pv, cb, 2);
            if (ptr == Win32Native.NULL)
            {
                throw new OutOfMemoryException();
            }
            return ptr;
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern int Release(IntPtr pUnk);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static int ReleaseComObject(object o)
        {
            __ComObject obj2 = null;
            try
            {
                obj2 = (__ComObject) o;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_ObjNotComObject"), "o");
            }
            return obj2.ReleaseSelf();
        }

        [Obsolete("This API did not perform any operation and will be removed in future versions of the CLR.", false), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void ReleaseThreadCache()
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr SecureStringToBSTR(SecureString s) => 
            s?.ToBSTR();

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr SecureStringToCoTaskMemAnsi(SecureString s) => 
            s?.ToAnsiStr(false);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr SecureStringToCoTaskMemUnicode(SecureString s) => 
            s?.ToUniStr(false);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr SecureStringToGlobalAllocAnsi(SecureString s) => 
            s?.ToAnsiStr(true);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr SecureStringToGlobalAllocUnicode(SecureString s) => 
            s?.ToUniStr(true);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static bool SetComObjectData(object obj, object key, object data)
        {
            __ComObject obj2 = null;
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            try
            {
                obj2 = (__ComObject) obj;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_ObjNotComObject"), "obj");
            }
            return obj2.SetData(key, data);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal static extern void SetLastWin32Error(int error);
        [MethodImpl(MethodImplOptions.InternalCall), ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern int SizeOf(object structure);
        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern int SizeOf(Type t);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr StringToBSTR(string s)
        {
            if (s == null)
            {
                return Win32Native.NULL;
            }
            if ((s.Length + 1) < s.Length)
            {
                throw new ArgumentOutOfRangeException("s");
            }
            IntPtr ptr = Win32Native.SysAllocStringLen(s, s.Length);
            if (ptr == Win32Native.NULL)
            {
                throw new OutOfMemoryException();
            }
            return ptr;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr StringToCoTaskMemAnsi(string s)
        {
            if (s == null)
            {
                return Win32Native.NULL;
            }
            int cb = (s.Length + 1) * SystemMaxDBCSCharSize;
            if (cb < s.Length)
            {
                throw new ArgumentOutOfRangeException("s");
            }
            IntPtr pdst = Win32Native.CoTaskMemAlloc(cb);
            if (pdst == Win32Native.NULL)
            {
                throw new OutOfMemoryException();
            }
            Win32Native.CopyMemoryAnsi(pdst, s, new IntPtr(cb));
            return pdst;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr StringToCoTaskMemAuto(string s)
        {
            if (s == null)
            {
                return Win32Native.NULL;
            }
            int cb = (s.Length + 1) * SystemDefaultCharSize;
            if (cb < s.Length)
            {
                throw new ArgumentOutOfRangeException("s");
            }
            IntPtr dst = Win32Native.CoTaskMemAlloc(cb);
            if (dst == Win32Native.NULL)
            {
                throw new OutOfMemoryException();
            }
            Win32Native.lstrcpy(dst, s);
            return dst;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr StringToCoTaskMemUni(string s)
        {
            if (s == null)
            {
                return Win32Native.NULL;
            }
            int cb = (s.Length + 1) * 2;
            if (cb < s.Length)
            {
                throw new ArgumentOutOfRangeException("s");
            }
            IntPtr pdst = Win32Native.CoTaskMemAlloc(cb);
            if (pdst == Win32Native.NULL)
            {
                throw new OutOfMemoryException();
            }
            Win32Native.CopyMemoryUni(pdst, s, new IntPtr(cb));
            return pdst;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr StringToHGlobalAnsi(string s)
        {
            if (s == null)
            {
                return Win32Native.NULL;
            }
            int num = (s.Length + 1) * SystemMaxDBCSCharSize;
            if (num < s.Length)
            {
                throw new ArgumentOutOfRangeException("s");
            }
            IntPtr sizetdwBytes = new IntPtr(num);
            IntPtr pdst = Win32Native.LocalAlloc_NoSafeHandle(0, sizetdwBytes);
            if (pdst == Win32Native.NULL)
            {
                throw new OutOfMemoryException();
            }
            Win32Native.CopyMemoryAnsi(pdst, s, sizetdwBytes);
            return pdst;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr StringToHGlobalAuto(string s)
        {
            if (SystemDefaultCharSize != 1)
            {
                return StringToHGlobalUni(s);
            }
            return StringToHGlobalAnsi(s);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static IntPtr StringToHGlobalUni(string s)
        {
            if (s == null)
            {
                return Win32Native.NULL;
            }
            int num = (s.Length + 1) * 2;
            if (num < s.Length)
            {
                throw new ArgumentOutOfRangeException("s");
            }
            IntPtr sizetdwBytes = new IntPtr(num);
            IntPtr pdst = Win32Native.LocalAlloc_NoSafeHandle(0, sizetdwBytes);
            if (pdst == Win32Native.NULL)
            {
                throw new OutOfMemoryException();
            }
            Win32Native.CopyMemoryUni(pdst, s, sizetdwBytes);
            return pdst;
        }

        [MethodImpl(MethodImplOptions.InternalCall), ComVisible(true), ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern void StructureToPtr(object structure, IntPtr ptr, bool fDeleteOld);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void ThrowExceptionForHR(int errorCode)
        {
            if (errorCode < 0)
            {
                ThrowExceptionForHRInternal(errorCode, Win32Native.NULL);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void ThrowExceptionForHR(int errorCode, IntPtr errorInfo)
        {
            if (errorCode < 0)
            {
                ThrowExceptionForHRInternal(errorCode, errorInfo);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void ThrowExceptionForHRInternal(int errorCode, IntPtr errorInfo);
        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static extern IntPtr UnsafeAddrOfPinnedArrayElement(Array arr, int index);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void WriteByte(IntPtr ptr, byte val)
        {
            WriteByte(ptr, 0, val);
        }

        [DllImport("mscoree.dll", EntryPoint="ND_WU1")]
        public static extern void WriteByte(IntPtr ptr, int ofs, byte val);
        [DllImport("mscoree.dll", EntryPoint="ND_WU1")]
        public static extern void WriteByte([In, Out, MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs, byte val);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void WriteInt16(IntPtr ptr, char val)
        {
            WriteInt16(ptr, 0, (short) val);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void WriteInt16(IntPtr ptr, short val)
        {
            WriteInt16(ptr, 0, val);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void WriteInt16(IntPtr ptr, int ofs, char val)
        {
            WriteInt16(ptr, ofs, (short) val);
        }

        [DllImport("mscoree.dll", EntryPoint="ND_WI2")]
        public static extern void WriteInt16(IntPtr ptr, int ofs, short val);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void WriteInt16([In, Out] object ptr, int ofs, char val)
        {
            WriteInt16(ptr, ofs, (short) val);
        }

        [DllImport("mscoree.dll", EntryPoint="ND_WI2")]
        public static extern void WriteInt16([In, Out, MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs, short val);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void WriteInt32(IntPtr ptr, int val)
        {
            WriteInt32(ptr, 0, val);
        }

        [DllImport("mscoree.dll", EntryPoint="ND_WI4")]
        public static extern void WriteInt32(IntPtr ptr, int ofs, int val);
        [DllImport("mscoree.dll", EntryPoint="ND_WI4")]
        public static extern void WriteInt32([In, Out, MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs, int val);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void WriteInt64(IntPtr ptr, long val)
        {
            WriteInt64(ptr, 0, val);
        }

        [DllImport("mscoree.dll", EntryPoint="ND_WI8")]
        public static extern void WriteInt64(IntPtr ptr, int ofs, long val);
        [DllImport("mscoree.dll", EntryPoint="ND_WI8")]
        public static extern void WriteInt64([In, Out, MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs, long val);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void WriteIntPtr(IntPtr ptr, IntPtr val)
        {
            WriteInt32(ptr, 0, (int) val);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void WriteIntPtr(IntPtr ptr, int ofs, IntPtr val)
        {
            WriteInt32(ptr, ofs, (int) val);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void WriteIntPtr([In, Out, MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs, IntPtr val)
        {
            WriteInt32(ptr, ofs, (int) val);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void ZeroFreeBSTR(IntPtr s)
        {
            Win32Native.ZeroMemory(s, (uint) (Win32Native.SysStringLen(s) * 2));
            FreeBSTR(s);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void ZeroFreeCoTaskMemAnsi(IntPtr s)
        {
            Win32Native.ZeroMemory(s, (uint) Win32Native.lstrlenA(s));
            FreeCoTaskMem(s);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void ZeroFreeCoTaskMemUnicode(IntPtr s)
        {
            Win32Native.ZeroMemory(s, (uint) (Win32Native.lstrlenW(s) * 2));
            FreeCoTaskMem(s);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void ZeroFreeGlobalAllocAnsi(IntPtr s)
        {
            Win32Native.ZeroMemory(s, (uint) Win32Native.lstrlenA(s));
            FreeHGlobal(s);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void ZeroFreeGlobalAllocUnicode(IntPtr s)
        {
            Win32Native.ZeroMemory(s, (uint) (Win32Native.lstrlenW(s) * 2));
            FreeHGlobal(s);
        }
    }
}

