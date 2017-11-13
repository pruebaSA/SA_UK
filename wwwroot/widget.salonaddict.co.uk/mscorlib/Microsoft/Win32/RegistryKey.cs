namespace Microsoft.Win32
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.AccessControl;
    using System.Security.Permissions;
    using System.Text;

    [ComVisible(true)]
    public sealed class RegistryKey : MarshalByRefObject, IDisposable
    {
        private static readonly int _SystemDefaultCharSize;
        private RegistryKeyPermissionCheck checkMode;
        private const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x2000;
        private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
        private const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x200;
        private SafeRegistryHandle hkey;
        internal static readonly IntPtr HKEY_CLASSES_ROOT = new IntPtr(-2147483648);
        internal static readonly IntPtr HKEY_CURRENT_CONFIG = new IntPtr(-2147483643);
        internal static readonly IntPtr HKEY_CURRENT_USER = new IntPtr(-2147483647);
        internal static readonly IntPtr HKEY_DYN_DATA = new IntPtr(-2147483642);
        internal static readonly IntPtr HKEY_LOCAL_MACHINE = new IntPtr(-2147483646);
        internal static readonly IntPtr HKEY_PERFORMANCE_DATA = new IntPtr(-2147483644);
        internal static readonly IntPtr HKEY_USERS = new IntPtr(-2147483645);
        private static readonly string[] hkeyNames = new string[] { "HKEY_CLASSES_ROOT", "HKEY_CURRENT_USER", "HKEY_LOCAL_MACHINE", "HKEY_USERS", "HKEY_PERFORMANCE_DATA", "HKEY_CURRENT_CONFIG", "HKEY_DYN_DATA" };
        private string keyName;
        private const int MaxKeyLength = 0xff;
        private bool remoteKey;
        private int state;
        private const int STATE_DIRTY = 1;
        private const int STATE_PERF_DATA = 8;
        private const int STATE_SYSTEMKEY = 2;
        private const int STATE_WRITEACCESS = 4;

        static RegistryKey()
        {
            sbyte[] ptr = new sbyte[4];
            ptr[0] = 0x41;
            ptr[1] = 0x41;
            _SystemDefaultCharSize = 3 - Win32Native.lstrlen(ptr);
        }

        private RegistryKey(SafeRegistryHandle hkey, bool writable) : this(hkey, writable, false, false, false)
        {
        }

        private RegistryKey(SafeRegistryHandle hkey, bool writable, bool systemkey, bool remoteKey, bool isPerfData)
        {
            this.hkey = hkey;
            this.keyName = "";
            this.remoteKey = remoteKey;
            if (systemkey)
            {
                this.state |= 2;
            }
            if (writable)
            {
                this.state |= 4;
            }
            if (isPerfData)
            {
                this.state |= 8;
            }
        }

        private RegistryValueKind CalculateValueKind(object value)
        {
            if (value is int)
            {
                return RegistryValueKind.DWord;
            }
            if (!(value is Array))
            {
                return RegistryValueKind.String;
            }
            if (value is byte[])
            {
                return RegistryValueKind.Binary;
            }
            if (!(value is string[]))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_RegSetBadArrType", new object[] { value.GetType().Name }));
            }
            return RegistryValueKind.MultiString;
        }

        private void CheckKeyReadPermission()
        {
            if (this.checkMode == RegistryKeyPermissionCheck.Default)
            {
                new RegistryPermission(RegistryPermissionAccess.Read, this.keyName + @"\.").Demand();
            }
        }

        private void CheckOpenSubKeyPermission(string subkeyName, RegistryKeyPermissionCheck subKeyCheck)
        {
            if ((subKeyCheck == RegistryKeyPermissionCheck.Default) && (this.checkMode == RegistryKeyPermissionCheck.Default))
            {
                this.CheckSubKeyReadPermission(subkeyName);
            }
            this.CheckSubTreePermission(subkeyName, subKeyCheck);
        }

        private void CheckOpenSubKeyPermission(string subkeyName, bool subKeyWritable)
        {
            if (this.checkMode == RegistryKeyPermissionCheck.Default)
            {
                this.CheckSubKeyReadPermission(subkeyName);
            }
            if (subKeyWritable && (this.checkMode == RegistryKeyPermissionCheck.ReadSubTree))
            {
                this.CheckSubTreeReadWritePermission(subkeyName);
            }
        }

        private void CheckSubKeyCreatePermission(string subkeyName)
        {
            if (this.remoteKey)
            {
                CheckUnmanagedCodePermission();
            }
            else if (this.checkMode == RegistryKeyPermissionCheck.Default)
            {
                new RegistryPermission(RegistryPermissionAccess.Create, this.keyName + @"\" + subkeyName + @"\.").Demand();
            }
        }

        private void CheckSubKeyReadPermission(string subkeyName)
        {
            if (this.remoteKey)
            {
                CheckUnmanagedCodePermission();
            }
            else
            {
                new RegistryPermission(RegistryPermissionAccess.Read, this.keyName + @"\" + subkeyName + @"\.").Demand();
            }
        }

        private void CheckSubKeyWritePermission(string subkeyName)
        {
            if (this.remoteKey)
            {
                CheckUnmanagedCodePermission();
            }
            else if (this.checkMode == RegistryKeyPermissionCheck.Default)
            {
                new RegistryPermission(RegistryPermissionAccess.Write, this.keyName + @"\" + subkeyName + @"\.").Demand();
            }
        }

        private void CheckSubTreePermission(string subkeyName, RegistryKeyPermissionCheck subKeyCheck)
        {
            if (subKeyCheck == RegistryKeyPermissionCheck.ReadSubTree)
            {
                if (this.checkMode == RegistryKeyPermissionCheck.Default)
                {
                    this.CheckSubTreeReadPermission(subkeyName);
                }
            }
            else if ((subKeyCheck == RegistryKeyPermissionCheck.ReadWriteSubTree) && (this.checkMode != RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                this.CheckSubTreeReadWritePermission(subkeyName);
            }
        }

        private void CheckSubTreeReadPermission(string subkeyName)
        {
            if (this.remoteKey)
            {
                CheckUnmanagedCodePermission();
            }
            else if (this.checkMode == RegistryKeyPermissionCheck.Default)
            {
                new RegistryPermission(RegistryPermissionAccess.Read, this.keyName + @"\" + subkeyName + @"\").Demand();
            }
        }

        private void CheckSubTreeReadWritePermission(string subkeyName)
        {
            if (this.remoteKey)
            {
                CheckUnmanagedCodePermission();
            }
            else
            {
                new RegistryPermission(RegistryPermissionAccess.Write | RegistryPermissionAccess.Read, this.keyName + @"\" + subkeyName).Demand();
            }
        }

        private void CheckSubTreeWritePermission(string subkeyName)
        {
            if (this.remoteKey)
            {
                CheckUnmanagedCodePermission();
            }
            else if (this.checkMode == RegistryKeyPermissionCheck.Default)
            {
                new RegistryPermission(RegistryPermissionAccess.Write, this.keyName + @"\" + subkeyName + @"\").Demand();
            }
        }

        private static void CheckUnmanagedCodePermission()
        {
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
        }

        private void CheckValueCreatePermission(string valueName)
        {
            if (this.remoteKey)
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
            }
            else if (this.checkMode == RegistryKeyPermissionCheck.Default)
            {
                new RegistryPermission(RegistryPermissionAccess.Create, this.keyName + @"\" + valueName).Demand();
            }
        }

        private void CheckValueReadPermission(string valueName)
        {
            if (this.checkMode == RegistryKeyPermissionCheck.Default)
            {
                new RegistryPermission(RegistryPermissionAccess.Read, this.keyName + @"\" + valueName).Demand();
            }
        }

        private void CheckValueWritePermission(string valueName)
        {
            if (this.remoteKey)
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
            }
            else if (this.checkMode == RegistryKeyPermissionCheck.Default)
            {
                new RegistryPermission(RegistryPermissionAccess.Write, this.keyName + @"\" + valueName).Demand();
            }
        }

        public void Close()
        {
            this.Dispose(true);
        }

        private bool ContainsRegistryValue(string name)
        {
            int lpType = 0;
            int lpcbData = 0;
            return (Win32Native.RegQueryValueEx(this.hkey, name, null, ref lpType, (byte[]) null, ref lpcbData) == 0);
        }

        public RegistryKey CreateSubKey(string subkey) => 
            this.CreateSubKey(subkey, this.checkMode);

        [ComVisible(false)]
        public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck) => 
            this.CreateSubKey(subkey, permissionCheck, null);

        [ComVisible(false)]
        public unsafe RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistrySecurity registrySecurity)
        {
            ValidateKeyName(subkey);
            ValidateKeyMode(permissionCheck);
            this.EnsureWriteable();
            subkey = FixupName(subkey);
            if (!this.remoteKey)
            {
                RegistryKey key = this.InternalOpenSubKey(subkey, permissionCheck != RegistryKeyPermissionCheck.ReadSubTree);
                if (key != null)
                {
                    this.CheckSubKeyWritePermission(subkey);
                    this.CheckSubTreePermission(subkey, permissionCheck);
                    key.checkMode = permissionCheck;
                    return key;
                }
            }
            this.CheckSubKeyCreatePermission(subkey);
            Win32Native.SECURITY_ATTRIBUTES structure = null;
            if (registrySecurity != null)
            {
                structure = new Win32Native.SECURITY_ATTRIBUTES();
                structure.nLength = Marshal.SizeOf(structure);
                byte[] securityDescriptorBinaryForm = registrySecurity.GetSecurityDescriptorBinaryForm();
                byte* pDest = stackalloc byte[1 * securityDescriptorBinaryForm.Length];
                Buffer.memcpy(securityDescriptorBinaryForm, 0, pDest, 0, securityDescriptorBinaryForm.Length);
                structure.pSecurityDescriptor = pDest;
            }
            int lpdwDisposition = 0;
            SafeRegistryHandle hkResult = null;
            int errorCode = Win32Native.RegCreateKeyEx(this.hkey, subkey, 0, null, 0, GetRegistryKeyAccess(permissionCheck != RegistryKeyPermissionCheck.ReadSubTree), structure, out hkResult, out lpdwDisposition);
            if ((errorCode == 0) && !hkResult.IsInvalid)
            {
                RegistryKey key2 = new RegistryKey(hkResult, permissionCheck != RegistryKeyPermissionCheck.ReadSubTree, false, this.remoteKey, false);
                this.CheckSubTreePermission(subkey, permissionCheck);
                key2.checkMode = permissionCheck;
                if (subkey.Length == 0)
                {
                    key2.keyName = this.keyName;
                    return key2;
                }
                key2.keyName = this.keyName + @"\" + subkey;
                return key2;
            }
            if (errorCode != 0)
            {
                this.Win32Error(errorCode, this.keyName + @"\" + subkey);
            }
            return null;
        }

        public void DeleteSubKey(string subkey)
        {
            this.DeleteSubKey(subkey, true);
        }

        public void DeleteSubKey(string subkey, bool throwOnMissingSubKey)
        {
            ValidateKeyName(subkey);
            this.EnsureWriteable();
            subkey = FixupName(subkey);
            this.CheckSubKeyWritePermission(subkey);
            RegistryKey key = this.InternalOpenSubKey(subkey, false);
            if (key != null)
            {
                try
                {
                    if (key.InternalSubKeyCount() > 0)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_RegRemoveSubKey);
                    }
                }
                finally
                {
                    key.Close();
                }
                int errorCode = Win32Native.RegDeleteKey(this.hkey, subkey);
                switch (errorCode)
                {
                    case 0:
                        return;

                    case 2:
                        if (throwOnMissingSubKey)
                        {
                            ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSubKeyAbsent);
                            return;
                        }
                        return;
                }
                this.Win32Error(errorCode, null);
            }
            else if (throwOnMissingSubKey)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSubKeyAbsent);
            }
        }

        public void DeleteSubKeyTree(string subkey)
        {
            ValidateKeyName(subkey);
            if ((subkey.Length == 0) && this.IsSystemKey())
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegKeyDelHive);
            }
            this.EnsureWriteable();
            subkey = FixupName(subkey);
            this.CheckSubTreeWritePermission(subkey);
            RegistryKey key = this.InternalOpenSubKey(subkey, true);
            if (key != null)
            {
                try
                {
                    if (key.InternalSubKeyCount() > 0)
                    {
                        string[] subKeyNames = key.InternalGetSubKeyNames();
                        for (int i = 0; i < subKeyNames.Length; i++)
                        {
                            key.DeleteSubKeyTreeInternal(subKeyNames[i]);
                        }
                    }
                }
                finally
                {
                    key.Close();
                }
                int errorCode = Win32Native.RegDeleteKey(this.hkey, subkey);
                if (errorCode != 0)
                {
                    this.Win32Error(errorCode, null);
                }
            }
            else
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSubKeyAbsent);
            }
        }

        private void DeleteSubKeyTreeInternal(string subkey)
        {
            RegistryKey key = this.InternalOpenSubKey(subkey, true);
            if (key != null)
            {
                try
                {
                    if (key.InternalSubKeyCount() > 0)
                    {
                        string[] subKeyNames = key.InternalGetSubKeyNames();
                        for (int i = 0; i < subKeyNames.Length; i++)
                        {
                            key.DeleteSubKeyTreeInternal(subKeyNames[i]);
                        }
                    }
                }
                finally
                {
                    key.Close();
                }
                int errorCode = Win32Native.RegDeleteKey(this.hkey, subkey);
                if (errorCode != 0)
                {
                    this.Win32Error(errorCode, null);
                }
            }
            else
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSubKeyAbsent);
            }
        }

        public void DeleteValue(string name)
        {
            this.DeleteValue(name, true);
        }

        public void DeleteValue(string name, bool throwOnMissingValue)
        {
            if (name == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.name);
            }
            this.EnsureWriteable();
            this.CheckValueWritePermission(name);
            int num = Win32Native.RegDeleteValue(this.hkey, name);
            if (((num == 2) || (num == 0xce)) && throwOnMissingValue)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSubKeyValueAbsent);
            }
        }

        private void Dispose(bool disposing)
        {
            if (this.hkey != null)
            {
                bool flag = this.IsPerfDataKey();
                if (!this.IsSystemKey() || flag)
                {
                    try
                    {
                        this.hkey.Dispose();
                    }
                    catch (IOException)
                    {
                    }
                    if (flag)
                    {
                        this.hkey = new SafeRegistryHandle(HKEY_PERFORMANCE_DATA, !IsWin9x());
                    }
                    else
                    {
                        this.hkey = null;
                    }
                }
            }
        }

        private void EnsureNotDisposed()
        {
            if (this.hkey == null)
            {
                ThrowHelper.ThrowObjectDisposedException(this.keyName, ExceptionResource.ObjectDisposed_RegKeyClosed);
            }
        }

        private void EnsureWriteable()
        {
            this.EnsureNotDisposed();
            if (!this.IsWritable())
            {
                ThrowHelper.ThrowUnauthorizedAccessException(ExceptionResource.UnauthorizedAccess_RegistryNoWrite);
            }
        }

        internal static string FixupName(string name)
        {
            if (name.IndexOf('\\') == -1)
            {
                return name;
            }
            StringBuilder path = new StringBuilder(name);
            FixupPath(path);
            int num = path.Length - 1;
            if (path[num] == '\\')
            {
                path.Length = num;
            }
            return path.ToString();
        }

        private static void FixupPath(StringBuilder path)
        {
            int num2;
            int length = path.Length;
            bool flag = false;
            char ch = 0xffff;
            for (num2 = 1; num2 < (length - 1); num2++)
            {
                if (path[num2] == '\\')
                {
                    num2++;
                    while (num2 < length)
                    {
                        if (path[num2] != '\\')
                        {
                            break;
                        }
                        path[num2] = ch;
                        num2++;
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                num2 = 0;
                int num3 = 0;
                while (num2 < length)
                {
                    if (path[num2] == ch)
                    {
                        num2++;
                    }
                    else
                    {
                        path[num3] = path[num2];
                        num2++;
                        num3++;
                    }
                }
                path.Length += num3 - num2;
            }
        }

        public void Flush()
        {
            if ((this.hkey != null) && this.IsDirty())
            {
                Win32Native.RegFlushKey(this.hkey);
            }
        }

        public RegistrySecurity GetAccessControl() => 
            this.GetAccessControl(AccessControlSections.Group | AccessControlSections.Owner | AccessControlSections.Access);

        public RegistrySecurity GetAccessControl(AccessControlSections includeSections)
        {
            this.EnsureNotDisposed();
            return new RegistrySecurity(this.hkey, this.keyName, includeSections);
        }

        internal static RegistryKey GetBaseKey(IntPtr hKey)
        {
            int index = ((int) hKey) & 0xfffffff;
            bool isPerfData = hKey == HKEY_PERFORMANCE_DATA;
            return new RegistryKey(new SafeRegistryHandle(hKey, isPerfData && !IsWin9x()), true, true, false, isPerfData) { 
                checkMode = RegistryKeyPermissionCheck.Default,
                keyName = hkeyNames[index]
            };
        }

        private static int GetRegistryKeyAccess(RegistryKeyPermissionCheck mode)
        {
            switch (mode)
            {
                case RegistryKeyPermissionCheck.Default:
                case RegistryKeyPermissionCheck.ReadSubTree:
                    return 0x20019;

                case RegistryKeyPermissionCheck.ReadWriteSubTree:
                    return 0x2001f;
            }
            return 0;
        }

        private static int GetRegistryKeyAccess(bool isWritable)
        {
            if (!isWritable)
            {
                return 0x20019;
            }
            return 0x2001f;
        }

        public string[] GetSubKeyNames()
        {
            this.CheckKeyReadPermission();
            return this.InternalGetSubKeyNames();
        }

        private RegistryKeyPermissionCheck GetSubKeyPermissonCheck(bool subkeyWritable)
        {
            if (this.checkMode == RegistryKeyPermissionCheck.Default)
            {
                return this.checkMode;
            }
            if (subkeyWritable)
            {
                return RegistryKeyPermissionCheck.ReadWriteSubTree;
            }
            return RegistryKeyPermissionCheck.ReadSubTree;
        }

        public object GetValue(string name)
        {
            this.CheckValueReadPermission(name);
            return this.InternalGetValue(name, null, false, true);
        }

        public object GetValue(string name, object defaultValue)
        {
            this.CheckValueReadPermission(name);
            return this.InternalGetValue(name, defaultValue, false, true);
        }

        [ComVisible(false)]
        public object GetValue(string name, object defaultValue, RegistryValueOptions options)
        {
            if ((options < RegistryValueOptions.None) || (options > RegistryValueOptions.DoNotExpandEnvironmentNames))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", new object[] { (int) options }), "options");
            }
            bool doNotExpand = options == RegistryValueOptions.DoNotExpandEnvironmentNames;
            this.CheckValueReadPermission(name);
            return this.InternalGetValue(name, defaultValue, doNotExpand, true);
        }

        [ComVisible(false)]
        public RegistryValueKind GetValueKind(string name)
        {
            this.CheckValueReadPermission(name);
            this.EnsureNotDisposed();
            int lpType = 0;
            int lpcbData = 0;
            int errorCode = Win32Native.RegQueryValueEx(this.hkey, name, null, ref lpType, (byte[]) null, ref lpcbData);
            if (errorCode != 0)
            {
                this.Win32Error(errorCode, null);
            }
            if (!Enum.IsDefined(typeof(RegistryValueKind), lpType))
            {
                return RegistryValueKind.Unknown;
            }
            return (RegistryValueKind) lpType;
        }

        public string[] GetValueNames()
        {
            this.CheckKeyReadPermission();
            this.EnsureNotDisposed();
            int num = this.InternalValueCount();
            string[] strArray = new string[num];
            if (num > 0)
            {
                StringBuilder lpValueName = new StringBuilder(0x100);
                for (int i = 0; i < num; i++)
                {
                    int capacity = lpValueName.Capacity;
                    int errorCode = Win32Native.RegEnumValue(this.hkey, i, lpValueName, ref capacity, Win32Native.NULL, null, null, null);
                    if (((errorCode == 0xea) && !this.IsPerfDataKey()) && this.remoteKey)
                    {
                        int[] lpcbData = new int[1];
                        byte[] lpData = new byte[5];
                        lpcbData[0] = 5;
                        errorCode = Win32Native.RegEnumValueA(this.hkey, i, lpValueName, ref capacity, Win32Native.NULL, null, lpData, lpcbData);
                        if (errorCode == 0xea)
                        {
                            lpcbData[0] = 0;
                            errorCode = Win32Native.RegEnumValueA(this.hkey, i, lpValueName, ref capacity, Win32Native.NULL, null, null, lpcbData);
                        }
                    }
                    if ((errorCode != 0) && (!this.IsPerfDataKey() || (errorCode != 0xea)))
                    {
                        this.Win32Error(errorCode, null);
                    }
                    strArray[i] = lpValueName.ToString();
                }
            }
            return strArray;
        }

        internal string[] InternalGetSubKeyNames()
        {
            this.EnsureNotDisposed();
            int num = this.InternalSubKeyCount();
            string[] strArray = new string[num];
            if (num > 0)
            {
                StringBuilder lpName = new StringBuilder(0x100);
                for (int i = 0; i < num; i++)
                {
                    int capacity = lpName.Capacity;
                    int errorCode = Win32Native.RegEnumKeyEx(this.hkey, i, lpName, out capacity, null, null, null, null);
                    if (errorCode != 0)
                    {
                        this.Win32Error(errorCode, null);
                    }
                    strArray[i] = lpName.ToString();
                }
            }
            return strArray;
        }

        internal object InternalGetValue(string name, object defaultValue, bool doNotExpand, bool checkSecurity)
        {
            IList list;
            if (checkSecurity)
            {
                this.EnsureNotDisposed();
            }
            object obj2 = defaultValue;
            int lpType = 0;
            int lpcbData = 0;
            int num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref lpType, (byte[]) null, ref lpcbData);
            if (num3 != 0)
            {
                if (this.IsPerfDataKey())
                {
                    int num6;
                    int num4 = 0xfde8;
                    int num5 = num4;
                    byte[] lpData = new byte[num4];
                    while (0xea == (num6 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref lpType, lpData, ref num5)))
                    {
                        num4 *= 2;
                        num5 = num4;
                        lpData = new byte[num4];
                    }
                    if (num6 != 0)
                    {
                        this.Win32Error(num6, name);
                    }
                    return lpData;
                }
                if (num3 != 0xea)
                {
                    return obj2;
                }
            }
            switch (lpType)
            {
                case 0:
                case 6:
                case 8:
                case 9:
                case 10:
                    return obj2;

                case 1:
                {
                    if (_SystemDefaultCharSize == 1)
                    {
                        byte[] buffer3 = new byte[lpcbData];
                        num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref lpType, buffer3, ref lpcbData);
                        return Encoding.Default.GetString(buffer3, 0, buffer3.Length - 1);
                    }
                    StringBuilder builder = new StringBuilder(lpcbData / 2);
                    num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref lpType, builder, ref lpcbData);
                    return builder.ToString();
                }
                case 2:
                {
                    if (_SystemDefaultCharSize == 1)
                    {
                        byte[] buffer4 = new byte[lpcbData];
                        num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref lpType, buffer4, ref lpcbData);
                        string str = Encoding.Default.GetString(buffer4, 0, buffer4.Length - 1);
                        if (doNotExpand)
                        {
                            return str;
                        }
                        return Environment.ExpandEnvironmentVariables(str);
                    }
                    StringBuilder builder2 = new StringBuilder(lpcbData / 2);
                    num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref lpType, builder2, ref lpcbData);
                    if (!doNotExpand)
                    {
                        return Environment.ExpandEnvironmentVariables(builder2.ToString());
                    }
                    return builder2.ToString();
                }
                case 3:
                case 5:
                {
                    byte[] buffer2 = new byte[lpcbData];
                    num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref lpType, buffer2, ref lpcbData);
                    return buffer2;
                }
                case 4:
                {
                    int num8 = 0;
                    num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref lpType, ref num8, ref lpcbData);
                    return num8;
                }
                case 7:
                {
                    bool flag = _SystemDefaultCharSize != 1;
                    list = new ArrayList();
                    if (!flag)
                    {
                        byte[] buffer5 = new byte[lpcbData];
                        num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref lpType, buffer5, ref lpcbData);
                        int index = 0;
                        int num13 = buffer5.Length;
                        while ((num3 == 0) && (index < num13))
                        {
                            int num14 = index;
                            while ((num14 < num13) && (buffer5[num14] != 0))
                            {
                                num14++;
                            }
                            if (num14 < num13)
                            {
                                if ((num14 - index) > 0)
                                {
                                    list.Add(Encoding.Default.GetString(buffer5, index, num14 - index));
                                }
                                else if (num14 != (num13 - 1))
                                {
                                    list.Add(string.Empty);
                                }
                            }
                            else
                            {
                                list.Add(Encoding.Default.GetString(buffer5, index, num13 - index));
                            }
                            index = num14 + 1;
                        }
                        break;
                    }
                    char[] chArray = new char[lpcbData / 2];
                    num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref lpType, chArray, ref lpcbData);
                    int startIndex = 0;
                    int length = chArray.Length;
                    while ((num3 == 0) && (startIndex < length))
                    {
                        int num11 = startIndex;
                        while ((num11 < length) && (chArray[num11] != '\0'))
                        {
                            num11++;
                        }
                        if (num11 < length)
                        {
                            if ((num11 - startIndex) > 0)
                            {
                                list.Add(new string(chArray, startIndex, num11 - startIndex));
                            }
                            else if (num11 != (length - 1))
                            {
                                list.Add(string.Empty);
                            }
                        }
                        else
                        {
                            list.Add(new string(chArray, startIndex, length - startIndex));
                        }
                        startIndex = num11 + 1;
                    }
                    break;
                }
                case 11:
                {
                    long num7 = 0L;
                    num3 = Win32Native.RegQueryValueEx(this.hkey, name, null, ref lpType, ref num7, ref lpcbData);
                    return num7;
                }
                default:
                    return obj2;
            }
            obj2 = new string[list.Count];
            list.CopyTo((Array) obj2, 0);
            return obj2;
        }

        internal RegistryKey InternalOpenSubKey(string name, bool writable)
        {
            ValidateKeyName(name);
            this.EnsureNotDisposed();
            int registryKeyAccess = GetRegistryKeyAccess(writable);
            SafeRegistryHandle hkResult = null;
            if ((Win32Native.RegOpenKeyEx(this.hkey, name, 0, registryKeyAccess, out hkResult) == 0) && !hkResult.IsInvalid)
            {
                return new RegistryKey(hkResult, writable, false, this.remoteKey, false) { keyName = this.keyName + @"\" + name };
            }
            return null;
        }

        private RegistryKey InternalOpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck, int rights)
        {
            ValidateKeyName(name);
            ValidateKeyMode(permissionCheck);
            ValidateKeyRights(rights);
            this.EnsureNotDisposed();
            name = FixupName(name);
            this.CheckOpenSubKeyPermission(name, permissionCheck);
            SafeRegistryHandle hkResult = null;
            int num = Win32Native.RegOpenKeyEx(this.hkey, name, 0, rights, out hkResult);
            if ((num == 0) && !hkResult.IsInvalid)
            {
                return new RegistryKey(hkResult, permissionCheck == RegistryKeyPermissionCheck.ReadWriteSubTree, false, this.remoteKey, false) { 
                    keyName = this.keyName + @"\" + name,
                    checkMode = permissionCheck
                };
            }
            if ((num == 5) || (num == 0x542))
            {
                ThrowHelper.ThrowSecurityException(ExceptionResource.Security_RegistryPermission);
            }
            return null;
        }

        internal int InternalSubKeyCount()
        {
            this.EnsureNotDisposed();
            int lpcSubKeys = 0;
            int lpcValues = 0;
            int errorCode = Win32Native.RegQueryInfoKey(this.hkey, null, null, Win32Native.NULL, ref lpcSubKeys, null, null, ref lpcValues, null, null, null, null);
            if (errorCode != 0)
            {
                this.Win32Error(errorCode, null);
            }
            return lpcSubKeys;
        }

        internal int InternalValueCount()
        {
            this.EnsureNotDisposed();
            int lpcValues = 0;
            int lpcSubKeys = 0;
            int errorCode = Win32Native.RegQueryInfoKey(this.hkey, null, null, Win32Native.NULL, ref lpcSubKeys, null, null, ref lpcValues, null, null, null, null);
            if (errorCode != 0)
            {
                this.Win32Error(errorCode, null);
            }
            return lpcValues;
        }

        private bool IsDirty() => 
            ((this.state & 1) != 0);

        private bool IsPerfDataKey() => 
            ((this.state & 8) != 0);

        private bool IsSystemKey() => 
            ((this.state & 2) != 0);

        private static bool IsWin9x() => 
            ((Environment.OSInfo & Environment.OSName.Win9x) != Environment.OSName.Invalid);

        private bool IsWritable() => 
            ((this.state & 4) != 0);

        public static RegistryKey OpenRemoteBaseKey(RegistryHive hKey, string machineName)
        {
            if (machineName == null)
            {
                throw new ArgumentNullException("machineName");
            }
            int index = ((int) hKey) & 0xfffffff;
            if (((index < 0) || (index >= hkeyNames.Length)) || ((((long) hKey) & 0xfffffff0L) != 0x80000000L))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_RegKeyOutOfRange"));
            }
            CheckUnmanagedCodePermission();
            SafeRegistryHandle result = null;
            int errorCode = Win32Native.RegConnectRegistry(machineName, new SafeRegistryHandle(new IntPtr((int) hKey), false), out result);
            if (errorCode == 0x45a)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_DllInitFailure"));
            }
            if (errorCode != 0)
            {
                Win32ErrorStatic(errorCode, null);
            }
            if (result.IsInvalid)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_RegKeyNoRemoteConnect", new object[] { machineName }));
            }
            return new RegistryKey(result, true, false, true, ((IntPtr) ((long) hKey)) == HKEY_PERFORMANCE_DATA) { 
                checkMode = RegistryKeyPermissionCheck.Default,
                keyName = hkeyNames[index]
            };
        }

        public RegistryKey OpenSubKey(string name) => 
            this.OpenSubKey(name, false);

        [ComVisible(false)]
        public RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck)
        {
            ValidateKeyMode(permissionCheck);
            return this.InternalOpenSubKey(name, permissionCheck, GetRegistryKeyAccess(permissionCheck));
        }

        public RegistryKey OpenSubKey(string name, bool writable)
        {
            ValidateKeyName(name);
            this.EnsureNotDisposed();
            name = FixupName(name);
            this.CheckOpenSubKeyPermission(name, writable);
            SafeRegistryHandle hkResult = null;
            int num = Win32Native.RegOpenKeyEx(this.hkey, name, 0, GetRegistryKeyAccess(writable), out hkResult);
            if ((num == 0) && !hkResult.IsInvalid)
            {
                return new RegistryKey(hkResult, writable, false, this.remoteKey, false) { 
                    checkMode = this.GetSubKeyPermissonCheck(writable),
                    keyName = this.keyName + @"\" + name
                };
            }
            if ((num == 5) || (num == 0x542))
            {
                ThrowHelper.ThrowSecurityException(ExceptionResource.Security_RegistryPermission);
            }
            return null;
        }

        [ComVisible(false)]
        public RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck, RegistryRights rights) => 
            this.InternalOpenSubKey(name, permissionCheck, (int) rights);

        public void SetAccessControl(RegistrySecurity registrySecurity)
        {
            this.EnsureWriteable();
            if (registrySecurity == null)
            {
                throw new ArgumentNullException("registrySecurity");
            }
            registrySecurity.Persist(this.hkey, this.keyName);
        }

        private void SetDirty()
        {
            this.state |= 1;
        }

        public void SetValue(string name, object value)
        {
            this.SetValue(name, value, RegistryValueKind.Unknown);
        }

        [ComVisible(false)]
        public unsafe void SetValue(string name, object value, RegistryValueKind valueKind)
        {
            if (value == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }
            if ((name != null) && (name.Length > 0xff))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_RegKeyStrLenBug"));
            }
            if (!Enum.IsDefined(typeof(RegistryValueKind), valueKind))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_RegBadKeyKind"), "valueKind");
            }
            this.EnsureWriteable();
            if (!this.remoteKey && this.ContainsRegistryValue(name))
            {
                this.CheckValueWritePermission(name);
            }
            else
            {
                this.CheckValueCreatePermission(name);
            }
            if (valueKind == RegistryValueKind.Unknown)
            {
                valueKind = this.CalculateValueKind(value);
            }
            int errorCode = 0;
            try
            {
                string str;
                string[] strArray;
                bool flag;
                int num2;
                int num3;
                int num4;
                byte[] buffer3;
                byte[] buffer5;
                switch (valueKind)
                {
                    case RegistryValueKind.String:
                    case RegistryValueKind.ExpandString:
                    {
                        str = value.ToString();
                        if (_SystemDefaultCharSize != 1)
                        {
                            break;
                        }
                        byte[] bytes = Encoding.Default.GetBytes(str);
                        byte[] destinationArray = new byte[bytes.Length + 1];
                        Array.Copy(bytes, 0, destinationArray, 0, bytes.Length);
                        errorCode = Win32Native.RegSetValueEx(this.hkey, name, 0, valueKind, destinationArray, destinationArray.Length);
                        goto Label_039E;
                    }
                    case RegistryValueKind.Binary:
                        goto Label_0314;

                    case RegistryValueKind.DWord:
                    {
                        int lpData = Convert.ToInt32(value, CultureInfo.InvariantCulture);
                        errorCode = Win32Native.RegSetValueEx(this.hkey, name, 0, RegistryValueKind.DWord, ref lpData, 4);
                        goto Label_039E;
                    }
                    case RegistryValueKind.MultiString:
                        strArray = (string[]) ((string[]) value).Clone();
                        flag = _SystemDefaultCharSize != 1;
                        num2 = 0;
                        if (!flag)
                        {
                            goto Label_0193;
                        }
                        num3 = 0;
                        goto Label_0183;

                    case RegistryValueKind.QWord:
                    {
                        long num7 = Convert.ToInt64(value, CultureInfo.InvariantCulture);
                        errorCode = Win32Native.RegSetValueEx(this.hkey, name, 0, RegistryValueKind.QWord, ref num7, 8);
                        goto Label_039E;
                    }
                    default:
                        goto Label_039E;
                }
                errorCode = Win32Native.RegSetValueEx(this.hkey, name, 0, valueKind, str, (str.Length * 2) + 2);
                goto Label_039E;
            Label_015C:
                if (strArray[num3] == null)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSetStrArrNull);
                }
                num2 += (strArray[num3].Length + 1) * 2;
                num3++;
            Label_0183:
                if (num3 < strArray.Length)
                {
                    goto Label_015C;
                }
                num2 += 2;
                goto Label_01D0;
            Label_0193:
                num4 = 0;
                while (num4 < strArray.Length)
                {
                    if (strArray[num4] == null)
                    {
                        ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSetStrArrNull);
                    }
                    num2 += Encoding.Default.GetByteCount(strArray[num4]) + 1;
                    num4++;
                }
                num2++;
            Label_01D0:
                buffer3 = new byte[num2];
                try
                {
                    fixed (byte* numRef = buffer3)
                    {
                        IntPtr dest = new IntPtr((void*) numRef);
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            if (flag)
                            {
                                string.InternalCopy(strArray[i], dest, strArray[i].Length * 2);
                                dest = new IntPtr(((long) dest) + (strArray[i].Length * 2));
                                *((short*) dest.ToPointer()) = 0;
                                dest = new IntPtr(((long) dest) + 2L);
                            }
                            else
                            {
                                byte[] src = Encoding.Default.GetBytes(strArray[i]);
                                Buffer.memcpy(src, 0, (byte*) dest.ToPointer(), 0, src.Length);
                                dest = new IntPtr(((long) dest) + src.Length);
                                *((sbyte*) dest.ToPointer()) = 0;
                                dest = new IntPtr(((long) dest) + 1L);
                            }
                        }
                        if (flag)
                        {
                            *((short*) dest.ToPointer()) = 0;
                            dest = new IntPtr(((long) dest) + 2L);
                        }
                        else
                        {
                            *((sbyte*) dest.ToPointer()) = 0;
                            dest = new IntPtr(((long) dest) + 1L);
                        }
                        errorCode = Win32Native.RegSetValueEx(this.hkey, name, 0, RegistryValueKind.MultiString, buffer3, num2);
                        goto Label_039E;
                    }
                }
                finally
                {
                    numRef = null;
                }
            Label_0314:
                buffer5 = (byte[]) value;
                errorCode = Win32Native.RegSetValueEx(this.hkey, name, 0, RegistryValueKind.Binary, buffer5, buffer5.Length);
            }
            catch (OverflowException)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSetMismatchedKind);
            }
            catch (InvalidOperationException)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSetMismatchedKind);
            }
            catch (FormatException)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSetMismatchedKind);
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegSetMismatchedKind);
            }
        Label_039E:
            if (errorCode == 0)
            {
                this.SetDirty();
            }
            else
            {
                this.Win32Error(errorCode, null);
            }
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }

        public override string ToString()
        {
            this.EnsureNotDisposed();
            return this.keyName;
        }

        private static void ValidateKeyMode(RegistryKeyPermissionCheck mode)
        {
            if ((mode < RegistryKeyPermissionCheck.Default) || (mode > RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidRegistryKeyPermissionCheck, ExceptionArgument.mode);
            }
        }

        private static void ValidateKeyName(string name)
        {
            if (name == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.name);
            }
            int index = name.IndexOf(@"\", StringComparison.OrdinalIgnoreCase);
            int startIndex = 0;
            while (index != -1)
            {
                if ((index - startIndex) > 0xff)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegKeyStrLenBug);
                }
                startIndex = index + 1;
                index = name.IndexOf(@"\", startIndex, StringComparison.OrdinalIgnoreCase);
            }
            if ((name.Length - startIndex) > 0xff)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RegKeyStrLenBug);
            }
        }

        private static void ValidateKeyRights(int rights)
        {
            if ((rights & -983104) != 0)
            {
                ThrowHelper.ThrowSecurityException(ExceptionResource.Security_RegistryPermission);
            }
        }

        internal void Win32Error(int errorCode, string str)
        {
            switch (errorCode)
            {
                case 2:
                    throw new IOException(Environment.GetResourceString("Arg_RegKeyNotFound"), errorCode);

                case 5:
                    if (str != null)
                    {
                        throw new UnauthorizedAccessException(Environment.GetResourceString("UnauthorizedAccess_RegistryKeyGeneric_Key", new object[] { str }));
                    }
                    throw new UnauthorizedAccessException();

                case 6:
                    this.hkey.SetHandleAsInvalid();
                    this.hkey = null;
                    break;

                case 0xea:
                    if (this.remoteKey)
                    {
                        return;
                    }
                    break;
            }
            throw new IOException(Win32Native.GetMessage(errorCode), errorCode);
        }

        internal static void Win32ErrorStatic(int errorCode, string str)
        {
            if (errorCode != 5)
            {
                throw new IOException(Win32Native.GetMessage(errorCode), errorCode);
            }
            if (str != null)
            {
                throw new UnauthorizedAccessException(Environment.GetResourceString("UnauthorizedAccess_RegistryKeyGeneric_Key", new object[] { str }));
            }
            throw new UnauthorizedAccessException();
        }

        public string Name
        {
            get
            {
                this.EnsureNotDisposed();
                return this.keyName;
            }
        }

        public int SubKeyCount
        {
            get
            {
                this.CheckKeyReadPermission();
                return this.InternalSubKeyCount();
            }
        }

        public int ValueCount
        {
            get
            {
                this.CheckKeyReadPermission();
                return this.InternalValueCount();
            }
        }
    }
}

