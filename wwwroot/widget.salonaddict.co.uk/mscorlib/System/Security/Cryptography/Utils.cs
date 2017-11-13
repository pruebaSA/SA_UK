namespace System.Security.Cryptography
{
    using Microsoft.Win32;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.AccessControl;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Permissions;
    using System.Threading;

    internal static class Utils
    {
        private static int _defaultRsaProviderType = -1;
        private static RNGCryptoServiceProvider _rng = null;
        private static SafeProvHandle _safeDssProvHandle = null;
        private static SafeProvHandle _safeProvHandle = null;
        private static int s_fipsAlgorithmPolicy = -1;
        private static int s_hasEnhProv = -1;
        private static object s_InternalSyncObject;
        private static int s_win2KCrypto = -1;

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void _AcquireCSP(CspParameters param, ref SafeProvHandle hProv);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void _CreateCSP(CspParameters param, bool randomKeyContainer, ref SafeProvHandle hProv);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void _CreateHash(SafeProvHandle hProv, int algid, ref SafeHashHandle hKey);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern byte[] _CryptDeriveKey(SafeProvHandle hProv, int algid, int algidHash, byte[] password, int dwFlags, byte[] IV);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int _DecryptData(SafeKeyHandle hKey, byte[] data, int ib, int cb, ref byte[] outputBuffer, int outputOffset, PaddingMode PaddingMode, bool fDone);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern byte[] _DecryptKey(SafeKeyHandle hPubKey, byte[] key, int dwFlags);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern byte[] _DecryptPKWin2KEnh(SafeKeyHandle hPubKey, byte[] key, bool fOAEP, out int hr);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int _EncryptData(SafeKeyHandle hKey, byte[] data, int ib, int cb, ref byte[] outputBuffer, int outputOffset, PaddingMode PaddingMode, bool fDone);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern byte[] _EncryptKey(SafeKeyHandle hPubKey, byte[] key);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern byte[] _EncryptPKWin2KEnh(SafeKeyHandle hPubKey, byte[] key, bool fOAEP, out int hr);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern byte[] _EndHash(SafeHashHandle hHash);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern byte[] _ExportCspBlob(SafeKeyHandle hKey, int blobType);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void _ExportKey(SafeKeyHandle hKey, int blobType, object cspObject);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void _GenerateKey(SafeProvHandle hProv, int algid, CspProviderFlags flags, int keySize, ref SafeKeyHandle hKey);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void _GetBytes(SafeProvHandle hProv, byte[] randomBytes);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool _GetEnforceFipsPolicySetting();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern byte[] _GetKeyParameter(SafeKeyHandle hKey, uint paramID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern byte[] _GetKeySetSecurityInfo(SafeProvHandle hProv, SecurityInfos securityInfo, out int error);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void _GetNonZeroBytes(SafeProvHandle hProv, byte[] randomBytes);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool _GetPersistKeyInCsp(SafeProvHandle hProv);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern object _GetProviderParameter(SafeProvHandle hProv, int keyNumber, uint paramID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string _GetRandomKeyContainer();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int _GetUserKey(SafeProvHandle hProv, int keyNumber, ref SafeKeyHandle hKey);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void _HashData(SafeHashHandle hHash, byte[] data, int ibStart, int cbSize);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void _ImportBulkKey(SafeProvHandle hProv, int algid, bool useSalt, byte[] key, ref SafeKeyHandle hKey);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int _ImportCspBlob(byte[] keyBlob, SafeProvHandle hProv, CspProviderFlags flags, ref SafeKeyHandle hKey);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void _ImportKey(SafeProvHandle hCSP, int keyNumber, CspProviderFlags flags, object cspObject, ref SafeKeyHandle hKey);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int _OpenCSP(CspParameters param, uint flags, ref SafeProvHandle hProv);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool _ProduceLegacyHmacValues();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool _SearchForAlgorithm(SafeProvHandle hProv, int algID, int keyLength);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void _SetKeyParamDw(SafeKeyHandle hKey, int param, int dwValue);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void _SetKeyParamRgb(SafeKeyHandle hKey, int param, byte[] value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int _SetKeySetSecurityInfo(SafeProvHandle hProv, SecurityInfos securityInfo, byte[] sd);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void _SetPersistKeyInCsp(SafeProvHandle hProv, bool fPersistKeyInCsp);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void _SetProviderParameter(SafeProvHandle hProv, int keyNumber, uint paramID, IntPtr pbData);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void _ShowLegacyHmacWarning();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern byte[] _SignValue(SafeKeyHandle hKey, int keyNumber, int calgKey, int calgHash, byte[] hash, int dwFlags);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool _VerifySign(SafeKeyHandle hKey, int calgKey, int calgHash, byte[] hash, byte[] signature, int dwFlags);
        internal static SafeProvHandle AcquireProvHandle(CspParameters parameters)
        {
            if (parameters == null)
            {
                parameters = new CspParameters(DefaultRsaProviderType);
            }
            SafeProvHandle invalidHandle = SafeProvHandle.InvalidHandle;
            if (Win2KCrypto == 1)
            {
                _AcquireCSP(parameters, ref invalidHandle);
                return invalidHandle;
            }
            if ((parameters.KeyContainerName == null) && ((parameters.Flags & CspProviderFlags.UseDefaultKeyContainer) == CspProviderFlags.NoFlags))
            {
                parameters.KeyContainerName = _GetRandomKeyContainer();
            }
            return CreateProvHandle(parameters, true);
        }

        internal static unsafe void BlockCopy(byte* src, int srcOffset, byte* dst, int dstOffset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                dst[dstOffset + i] = src[srcOffset + i];
            }
        }

        internal static unsafe void BlockCopy(byte[] src, int srcOffset, int* dst, int dstOffset, int count)
        {
            fixed (byte* numRef = src)
            {
                BlockCopy(numRef, srcOffset, (byte*) dst, dstOffset, count);
            }
        }

        internal static unsafe void BlockCopy(int* src, int srcOffset, int[] dst, int dstOffset, int count)
        {
            fixed (int* numRef = &(dst[dstOffset]))
            {
                BlockCopy((byte*) (src + srcOffset), srcOffset, (byte*) numRef, 0, count);
            }
        }

        internal static bool CompareBigIntArrays(byte[] lhs, byte[] rhs)
        {
            if (lhs == null)
            {
                return (rhs == null);
            }
            int index = 0;
            int num2 = 0;
            while ((index < lhs.Length) && (lhs[index] == 0))
            {
                index++;
            }
            while ((num2 < rhs.Length) && (rhs[num2] == 0))
            {
                num2++;
            }
            int num3 = lhs.Length - index;
            if ((rhs.Length - num2) != num3)
            {
                return false;
            }
            for (int i = 0; i < num3; i++)
            {
                if (lhs[index + i] != rhs[num2 + i])
                {
                    return false;
                }
            }
            return true;
        }

        internal static int ConvertByteArrayToInt(byte[] input)
        {
            int num = 0;
            for (int i = 0; i < input.Length; i++)
            {
                num *= 0x100;
                num += input[i];
            }
            return num;
        }

        internal static byte[] ConvertIntToByteArray(int dwInput)
        {
            byte[] buffer = new byte[8];
            int index = 0;
            if (dwInput == 0)
            {
                return new byte[1];
            }
            int num = dwInput;
            while (num > 0)
            {
                int num2 = num % 0x100;
                buffer[index] = (byte) num2;
                num = (num - num2) / 0x100;
                index++;
            }
            byte[] buffer2 = new byte[index];
            for (int i = 0; i < index; i++)
            {
                buffer2[i] = buffer[(index - i) - 1];
            }
            return buffer2;
        }

        internal static void ConvertIntToByteArray(uint dwInput, ref byte[] counter)
        {
            uint num = dwInput;
            int num3 = 0;
            Array.Clear(counter, 0, counter.Length);
            if (dwInput != 0)
            {
                while (num > 0)
                {
                    uint num2 = num % 0x100;
                    counter[3 - num3] = (byte) num2;
                    num = (num - num2) / 0x100;
                    num3++;
                }
            }
        }

        internal static SafeProvHandle CreateProvHandle(CspParameters parameters, bool randomKeyContainer)
        {
            SafeProvHandle invalidHandle = SafeProvHandle.InvalidHandle;
            int hr = _OpenCSP(parameters, 0, ref invalidHandle);
            KeyContainerPermission permission = new KeyContainerPermission(KeyContainerPermissionFlags.NoFlags);
            if (hr != 0)
            {
                if (((parameters.Flags & CspProviderFlags.UseExistingKey) != CspProviderFlags.NoFlags) || ((hr != -2146893799) && (hr != -2146893802)))
                {
                    throw new CryptographicException(hr);
                }
                if (!randomKeyContainer)
                {
                    KeyContainerPermissionAccessEntry accessEntry = new KeyContainerPermissionAccessEntry(parameters, KeyContainerPermissionFlags.Create);
                    permission.AccessEntries.Add(accessEntry);
                    permission.Demand();
                }
                _CreateCSP(parameters, randomKeyContainer, ref invalidHandle);
                return invalidHandle;
            }
            if (!randomKeyContainer)
            {
                KeyContainerPermissionAccessEntry entry2 = new KeyContainerPermissionAccessEntry(parameters, KeyContainerPermissionFlags.Open);
                permission.AccessEntries.Add(entry2);
                permission.Demand();
            }
            return invalidHandle;
        }

        internal static string DiscardWhiteSpaces(string inputBuffer) => 
            DiscardWhiteSpaces(inputBuffer, 0, inputBuffer.Length);

        internal static string DiscardWhiteSpaces(string inputBuffer, int inputOffset, int inputCount)
        {
            int num;
            int num2 = 0;
            for (num = 0; num < inputCount; num++)
            {
                if (char.IsWhiteSpace(inputBuffer[inputOffset + num]))
                {
                    num2++;
                }
            }
            char[] chArray = new char[inputCount - num2];
            num2 = 0;
            for (num = 0; num < inputCount; num++)
            {
                if (!char.IsWhiteSpace(inputBuffer[inputOffset + num]))
                {
                    chArray[num2++] = inputBuffer[inputOffset + num];
                }
            }
            return new string(chArray);
        }

        internal static unsafe void DWORDFromBigEndian(uint* x, int digits, byte* block)
        {
            int index = 0;
            for (int i = 0; index < digits; i += 4)
            {
                x[index] = (uint) ((((block[i] << 0x18) | (block[i + 1] << 0x10)) | (block[i + 2] << 8)) | block[i + 3]);
                index++;
            }
        }

        internal static unsafe void DWORDFromLittleEndian(uint* x, int digits, byte* block)
        {
            int index = 0;
            for (int i = 0; index < digits; i += 4)
            {
                x[index] = (uint) (((block[i] | (block[i + 1] << 8)) | (block[i + 2] << 0x10)) | (block[i + 3] << 0x18));
                index++;
            }
        }

        internal static void DWORDToBigEndian(byte[] block, uint[] x, int digits)
        {
            int index = 0;
            for (int i = 0; index < digits; i += 4)
            {
                block[i] = (byte) ((x[index] >> 0x18) & 0xff);
                block[i + 1] = (byte) ((x[index] >> 0x10) & 0xff);
                block[i + 2] = (byte) ((x[index] >> 8) & 0xff);
                block[i + 3] = (byte) (x[index] & 0xff);
                index++;
            }
        }

        internal static void DWORDToLittleEndian(byte[] block, uint[] x, int digits)
        {
            int index = 0;
            for (int i = 0; index < digits; i += 4)
            {
                block[i] = (byte) (x[index] & 0xff);
                block[i + 1] = (byte) ((x[index] >> 8) & 0xff);
                block[i + 2] = (byte) ((x[index] >> 0x10) & 0xff);
                block[i + 3] = (byte) ((x[index] >> 0x18) & 0xff);
                index++;
            }
        }

        internal static byte[] ExportCspBlobHelper(bool includePrivateParameters, CspParameters parameters, SafeKeyHandle safeKeyHandle)
        {
            if (includePrivateParameters)
            {
                KeyContainerPermission permission = new KeyContainerPermission(KeyContainerPermissionFlags.NoFlags);
                KeyContainerPermissionAccessEntry accessEntry = new KeyContainerPermissionAccessEntry(parameters, KeyContainerPermissionFlags.Export);
                permission.AccessEntries.Add(accessEntry);
                permission.Demand();
            }
            return _ExportCspBlob(safeKeyHandle, includePrivateParameters ? 7 : 6);
        }

        internal static byte[] FixupKeyParity(byte[] key)
        {
            byte[] buffer = new byte[key.Length];
            for (int i = 0; i < key.Length; i++)
            {
                buffer[i] = (byte) (key[i] & 0xfe);
                byte num2 = (byte) ((buffer[i] & 15) ^ (buffer[i] >> 4));
                byte num3 = (byte) ((num2 & 3) ^ (num2 >> 2));
                if (((byte) ((num3 & 1) ^ (num3 >> 1))) == 0)
                {
                    buffer[i] = (byte) (buffer[i] | 1);
                }
            }
            return buffer;
        }

        internal static byte[] GenerateRandom(int keySize)
        {
            byte[] data = new byte[keySize];
            StaticRandomNumberGenerator.GetBytes(data);
            return data;
        }

        internal static void GetKeyPairHelper(CspAlgorithmType keyType, CspParameters parameters, bool randomKeyContainer, int dwKeySize, ref SafeProvHandle safeProvHandle, ref SafeKeyHandle safeKeyHandle)
        {
            SafeProvHandle hProv = CreateProvHandle(parameters, randomKeyContainer);
            if (parameters.CryptoKeySecurity != null)
            {
                KeyContainerPermission permission = new KeyContainerPermission(KeyContainerPermissionFlags.NoFlags);
                KeyContainerPermissionAccessEntry accessEntry = new KeyContainerPermissionAccessEntry(parameters, KeyContainerPermissionFlags.ChangeAcl);
                permission.AccessEntries.Add(accessEntry);
                permission.Demand();
                SetKeySetSecurityInfo(hProv, parameters.CryptoKeySecurity, parameters.CryptoKeySecurity.ChangedAccessControlSections);
            }
            if (parameters.ParentWindowHandle != IntPtr.Zero)
            {
                _SetProviderParameter(hProv, parameters.KeyNumber, 10, parameters.ParentWindowHandle);
            }
            else if (parameters.KeyPassword != null)
            {
                IntPtr pbData = Marshal.SecureStringToCoTaskMemAnsi(parameters.KeyPassword);
                try
                {
                    _SetProviderParameter(hProv, parameters.KeyNumber, 11, pbData);
                }
                finally
                {
                    if (pbData != IntPtr.Zero)
                    {
                        Marshal.ZeroFreeCoTaskMemAnsi(pbData);
                    }
                }
            }
            safeProvHandle = hProv;
            SafeKeyHandle invalidHandle = SafeKeyHandle.InvalidHandle;
            int hr = _GetUserKey(safeProvHandle, parameters.KeyNumber, ref invalidHandle);
            if (hr != 0)
            {
                if (((parameters.Flags & CspProviderFlags.UseExistingKey) != CspProviderFlags.NoFlags) || (hr != -2146893811))
                {
                    throw new CryptographicException(hr);
                }
                _GenerateKey(safeProvHandle, parameters.KeyNumber, parameters.Flags, dwKeySize, ref invalidHandle);
            }
            byte[] buffer = _GetKeyParameter(invalidHandle, 9);
            int num2 = ((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 0x10)) | (buffer[3] << 0x18);
            if ((((keyType == CspAlgorithmType.Rsa) && (num2 != 0xa400)) && (num2 != 0x2400)) || ((keyType == CspAlgorithmType.Dss) && (num2 != 0x2200)))
            {
                invalidHandle.Dispose();
                throw new CryptographicException(Environment.GetResourceString("Cryptography_CSP_WrongKeySpec"));
            }
            safeKeyHandle = invalidHandle;
        }

        internal static CryptoKeySecurity GetKeySetSecurityInfo(SafeProvHandle hProv, AccessControlSections accessControlSections)
        {
            int num;
            if (Win2KCrypto != 1)
            {
                throw new PlatformNotSupportedException(Environment.GetResourceString("PlatformNotSupported_RequiresNT"));
            }
            SecurityInfos securityInfo = 0;
            Privilege privilege = null;
            if ((accessControlSections & AccessControlSections.Owner) != AccessControlSections.None)
            {
                securityInfo |= SecurityInfos.Owner;
            }
            if ((accessControlSections & AccessControlSections.Group) != AccessControlSections.None)
            {
                securityInfo |= SecurityInfos.Group;
            }
            if ((accessControlSections & AccessControlSections.Access) != AccessControlSections.None)
            {
                securityInfo |= SecurityInfos.DiscretionaryAcl;
            }
            byte[] binaryForm = null;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                if ((accessControlSections & AccessControlSections.Audit) != AccessControlSections.None)
                {
                    securityInfo |= SecurityInfos.SystemAcl;
                    privilege = new Privilege("SeSecurityPrivilege");
                    privilege.Enable();
                }
                binaryForm = _GetKeySetSecurityInfo(hProv, securityInfo, out num);
            }
            finally
            {
                if (privilege != null)
                {
                    privilege.Revert();
                }
            }
            if ((num == 0) && ((binaryForm == null) || (binaryForm.Length == 0)))
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NoSecurityDescriptor"));
            }
            switch (num)
            {
                case 8:
                    throw new OutOfMemoryException();

                case 5:
                    throw new UnauthorizedAccessException();

                case 0x522:
                    throw new PrivilegeNotHeldException("SeSecurityPrivilege");
            }
            if (num != 0)
            {
                throw new CryptographicException(num);
            }
            return new CryptoKeySecurity(new CommonSecurityDescriptor(false, false, new RawSecurityDescriptor(binaryForm, 0), true));
        }

        internal static bool HasAlgorithm(int dwCalg, int dwKeySize)
        {
            lock (InternalSyncObject)
            {
                return _SearchForAlgorithm(StaticProvHandle, dwCalg, dwKeySize);
            }
        }

        internal static void ImportCspBlobHelper(CspAlgorithmType keyType, byte[] keyBlob, bool publicOnly, ref CspParameters parameters, bool randomKeyContainer, ref SafeProvHandle safeProvHandle, ref SafeKeyHandle safeKeyHandle)
        {
            if ((safeKeyHandle != null) && !safeKeyHandle.IsClosed)
            {
                safeKeyHandle.Dispose();
            }
            safeKeyHandle = SafeKeyHandle.InvalidHandle;
            if (publicOnly)
            {
                parameters.KeyNumber = _ImportCspBlob(keyBlob, (keyType == CspAlgorithmType.Dss) ? StaticDssProvHandle : StaticProvHandle, CspProviderFlags.NoFlags, ref safeKeyHandle);
            }
            else
            {
                KeyContainerPermission permission = new KeyContainerPermission(KeyContainerPermissionFlags.NoFlags);
                KeyContainerPermissionAccessEntry accessEntry = new KeyContainerPermissionAccessEntry(parameters, KeyContainerPermissionFlags.Import);
                permission.AccessEntries.Add(accessEntry);
                permission.Demand();
                if (safeProvHandle == null)
                {
                    safeProvHandle = CreateProvHandle(parameters, randomKeyContainer);
                }
                parameters.KeyNumber = _ImportCspBlob(keyBlob, safeProvHandle, parameters.Flags, ref safeKeyHandle);
            }
        }

        internal static byte[] Int(uint i)
        {
            byte[] bytes = BitConverter.GetBytes(i);
            byte[] buffer2 = new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
            if (!BitConverter.IsLittleEndian)
            {
                return bytes;
            }
            return buffer2;
        }

        internal static HashAlgorithm ObjToHashAlgorithm(object hashAlg)
        {
            if (hashAlg == null)
            {
                throw new ArgumentNullException("hashAlg");
            }
            HashAlgorithm algorithm = null;
            if (hashAlg is string)
            {
                algorithm = (HashAlgorithm) CryptoConfig.CreateFromName((string) hashAlg);
                if (algorithm == null)
                {
                    string name = X509Utils._GetFriendlyNameFromOid((string) hashAlg);
                    if (name != null)
                    {
                        algorithm = (HashAlgorithm) CryptoConfig.CreateFromName(name);
                    }
                }
            }
            else if (hashAlg is HashAlgorithm)
            {
                algorithm = (HashAlgorithm) hashAlg;
            }
            else if (hashAlg is Type)
            {
                algorithm = (HashAlgorithm) CryptoConfig.CreateFromName(hashAlg.ToString());
            }
            if (algorithm == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidValue"));
            }
            return algorithm;
        }

        internal static string ObjToOidValue(object hashAlg)
        {
            if (hashAlg == null)
            {
                throw new ArgumentNullException("hashAlg");
            }
            string str = null;
            if (hashAlg is string)
            {
                str = CryptoConfig.MapNameToOID((string) hashAlg);
                if (str == null)
                {
                    str = (string) hashAlg;
                }
            }
            else if (hashAlg is HashAlgorithm)
            {
                str = CryptoConfig.MapNameToOID(hashAlg.GetType().ToString());
            }
            else if (hashAlg is Type)
            {
                str = CryptoConfig.MapNameToOID(hashAlg.ToString());
            }
            if (str == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidValue"));
            }
            return str;
        }

        internal static unsafe void QuadWordFromBigEndian(ulong* x, int digits, byte* block)
        {
            int index = 0;
            for (int i = 0; index < digits; i += 8)
            {
                x[index] = (ulong) ((((((((block[i] << 0x38) | (block[i + 1] << 0x30)) | (block[i + 2] << 40)) | (block[i + 3] << 0x20)) | (block[i + 4] << 0x18)) | (block[i + 5] << 0x10)) | (block[i + 6] << 8)) | block[i + 7]);
                index++;
            }
        }

        internal static void QuadWordToBigEndian(byte[] block, ulong[] x, int digits)
        {
            int index = 0;
            for (int i = 0; index < digits; i += 8)
            {
                block[i] = (byte) ((x[index] >> 0x38) & ((ulong) 0xffL));
                block[i + 1] = (byte) ((x[index] >> 0x30) & ((ulong) 0xffL));
                block[i + 2] = (byte) ((x[index] >> 40) & ((ulong) 0xffL));
                block[i + 3] = (byte) ((x[index] >> 0x20) & ((ulong) 0xffL));
                block[i + 4] = (byte) ((x[index] >> 0x18) & ((ulong) 0xffL));
                block[i + 5] = (byte) ((x[index] >> 0x10) & ((ulong) 0xffL));
                block[i + 6] = (byte) ((x[index] >> 8) & ((ulong) 0xffL));
                block[i + 7] = (byte) (x[index] & ((ulong) 0xffL));
                index++;
            }
        }

        internal static byte[] RsaOaepDecrypt(RSA rsa, HashAlgorithm hash, PKCS1MaskGenerationMethod mgf, byte[] encryptedData)
        {
            int num = rsa.KeySize / 8;
            byte[] src = null;
            try
            {
                src = rsa.DecryptValue(encryptedData);
            }
            catch (CryptographicException)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_OAEPDecoding"));
            }
            int num2 = hash.HashSize / 8;
            int dstOffset = num - src.Length;
            if ((dstOffset < 0) || (dstOffset >= num2))
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_OAEPDecoding"));
            }
            byte[] dst = new byte[num2];
            Buffer.InternalBlockCopy(src, 0, dst, dstOffset, dst.Length - dstOffset);
            byte[] buffer3 = new byte[(src.Length - dst.Length) + dstOffset];
            Buffer.InternalBlockCopy(src, dst.Length - dstOffset, buffer3, 0, buffer3.Length);
            byte[] buffer4 = mgf.GenerateMask(buffer3, dst.Length);
            int index = 0;
            for (index = 0; index < dst.Length; index++)
            {
                dst[index] = (byte) (dst[index] ^ buffer4[index]);
            }
            buffer4 = mgf.GenerateMask(dst, buffer3.Length);
            for (index = 0; index < buffer3.Length; index++)
            {
                buffer3[index] = (byte) (buffer3[index] ^ buffer4[index]);
            }
            hash.ComputeHash(new byte[0]);
            byte[] buffer5 = hash.Hash;
            index = 0;
            while (index < num2)
            {
                if (buffer3[index] != buffer5[index])
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_OAEPDecoding"));
                }
                index++;
            }
            while (index < buffer3.Length)
            {
                if (buffer3[index] == 1)
                {
                    break;
                }
                if (buffer3[index] != 0)
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_OAEPDecoding"));
                }
                index++;
            }
            if (index == buffer3.Length)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_OAEPDecoding"));
            }
            index++;
            byte[] buffer6 = new byte[buffer3.Length - index];
            Buffer.InternalBlockCopy(buffer3, index, buffer6, 0, buffer6.Length);
            return buffer6;
        }

        internal static byte[] RsaOaepEncrypt(RSA rsa, HashAlgorithm hash, PKCS1MaskGenerationMethod mgf, RandomNumberGenerator rng, byte[] data)
        {
            int num = rsa.KeySize / 8;
            int count = hash.HashSize / 8;
            if (((data.Length + 2) + (2 * count)) > num)
            {
                throw new CryptographicException(string.Format(null, Environment.GetResourceString("Cryptography_Padding_EncDataTooBig"), new object[] { (num - 2) - (2 * count) }));
            }
            hash.ComputeHash(new byte[0]);
            byte[] dst = new byte[num - count];
            Buffer.InternalBlockCopy(hash.Hash, 0, dst, 0, count);
            dst[(dst.Length - data.Length) - 1] = 1;
            Buffer.InternalBlockCopy(data, 0, dst, dst.Length - data.Length, data.Length);
            byte[] buffer2 = new byte[count];
            rng.GetBytes(buffer2);
            byte[] buffer3 = mgf.GenerateMask(buffer2, dst.Length);
            for (int i = 0; i < dst.Length; i++)
            {
                dst[i] = (byte) (dst[i] ^ buffer3[i]);
            }
            buffer3 = mgf.GenerateMask(dst, count);
            for (int j = 0; j < buffer2.Length; j++)
            {
                buffer2[j] = (byte) (buffer2[j] ^ buffer3[j]);
            }
            byte[] buffer4 = new byte[num];
            Buffer.InternalBlockCopy(buffer2, 0, buffer4, 0, buffer2.Length);
            Buffer.InternalBlockCopy(dst, 0, buffer4, buffer2.Length, dst.Length);
            return rsa.EncryptValue(buffer4);
        }

        internal static byte[] RsaPkcs1Padding(RSA rsa, byte[] oid, byte[] hash)
        {
            int num = rsa.KeySize / 8;
            byte[] dst = new byte[num];
            byte[] buffer2 = new byte[(oid.Length + 8) + hash.Length];
            buffer2[0] = 0x30;
            int num2 = buffer2.Length - 2;
            buffer2[1] = (byte) num2;
            buffer2[2] = 0x30;
            num2 = oid.Length + 2;
            buffer2[3] = (byte) num2;
            Buffer.InternalBlockCopy(oid, 0, buffer2, 4, oid.Length);
            buffer2[4 + oid.Length] = 5;
            buffer2[(4 + oid.Length) + 1] = 0;
            buffer2[(4 + oid.Length) + 2] = 4;
            buffer2[(4 + oid.Length) + 3] = (byte) hash.Length;
            Buffer.InternalBlockCopy(hash, 0, buffer2, oid.Length + 8, hash.Length);
            int dstOffset = num - buffer2.Length;
            if (dstOffset <= 2)
            {
                throw new CryptographicUnexpectedOperationException(Environment.GetResourceString("Cryptography_InvalidOID"));
            }
            dst[0] = 0;
            dst[1] = 1;
            for (int i = 2; i < (dstOffset - 1); i++)
            {
                dst[i] = 0xff;
            }
            dst[dstOffset - 1] = 0;
            Buffer.InternalBlockCopy(buffer2, 0, dst, dstOffset, buffer2.Length);
            return dst;
        }

        internal static CspParameters SaveCspParameters(CspAlgorithmType keyType, CspParameters userParameters, CspProviderFlags defaultFlags, ref bool randomKeyContainer)
        {
            CspParameters parameters;
            if (userParameters == null)
            {
                parameters = new CspParameters((keyType == CspAlgorithmType.Dss) ? 13 : DefaultRsaProviderType, null, null, defaultFlags);
            }
            else
            {
                ValidateCspFlags(userParameters.Flags);
                parameters = new CspParameters(userParameters);
            }
            if (parameters.KeyNumber == -1)
            {
                parameters.KeyNumber = (keyType == CspAlgorithmType.Dss) ? 2 : 1;
            }
            else if ((parameters.KeyNumber == 0x2200) || (parameters.KeyNumber == 0x2400))
            {
                parameters.KeyNumber = 2;
            }
            else if (parameters.KeyNumber == 0xa400)
            {
                parameters.KeyNumber = 1;
            }
            randomKeyContainer = false;
            if ((parameters.KeyContainerName == null) && ((parameters.Flags & CspProviderFlags.UseDefaultKeyContainer) == CspProviderFlags.NoFlags))
            {
                parameters.KeyContainerName = _GetRandomKeyContainer();
                randomKeyContainer = true;
            }
            return parameters;
        }

        internal static void SetKeySetSecurityInfo(SafeProvHandle hProv, CryptoKeySecurity cryptoKeySecurity, AccessControlSections accessControlSections)
        {
            if (Win2KCrypto != 1)
            {
                throw new PlatformNotSupportedException(Environment.GetResourceString("PlatformNotSupported_RequiresNT"));
            }
            SecurityInfos securityInfo = 0;
            Privilege privilege = null;
            if (((accessControlSections & AccessControlSections.Owner) != AccessControlSections.None) && (cryptoKeySecurity._securityDescriptor.Owner != null))
            {
                securityInfo |= SecurityInfos.Owner;
            }
            if (((accessControlSections & AccessControlSections.Group) != AccessControlSections.None) && (cryptoKeySecurity._securityDescriptor.Group != null))
            {
                securityInfo |= SecurityInfos.Group;
            }
            if ((accessControlSections & AccessControlSections.Audit) != AccessControlSections.None)
            {
                securityInfo |= SecurityInfos.SystemAcl;
            }
            if (((accessControlSections & AccessControlSections.Access) != AccessControlSections.None) && cryptoKeySecurity._securityDescriptor.IsDiscretionaryAclPresent)
            {
                securityInfo |= SecurityInfos.DiscretionaryAcl;
            }
            if (securityInfo != 0)
            {
                int hr = 0;
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    if ((securityInfo & SecurityInfos.SystemAcl) != 0)
                    {
                        privilege = new Privilege("SeSecurityPrivilege");
                        privilege.Enable();
                    }
                    byte[] securityDescriptorBinaryForm = cryptoKeySecurity.GetSecurityDescriptorBinaryForm();
                    if ((securityDescriptorBinaryForm != null) && (securityDescriptorBinaryForm.Length > 0))
                    {
                        hr = _SetKeySetSecurityInfo(hProv, securityInfo, securityDescriptorBinaryForm);
                    }
                }
                finally
                {
                    if (privilege != null)
                    {
                        privilege.Revert();
                    }
                }
                switch (hr)
                {
                    case 5:
                    case 0x51b:
                    case 0x51c:
                        throw new UnauthorizedAccessException();

                    case 0x522:
                        throw new PrivilegeNotHeldException("SeSecurityPrivilege");

                    case 6:
                        throw new NotSupportedException(Environment.GetResourceString("AccessControl_InvalidHandle"));
                }
                if (hr != 0)
                {
                    throw new CryptographicException(hr);
                }
            }
        }

        private static void ValidateCspFlags(CspProviderFlags flags)
        {
            if ((flags & CspProviderFlags.UseExistingKey) != CspProviderFlags.NoFlags)
            {
                CspProviderFlags flags2 = CspProviderFlags.UseUserProtectedKey | CspProviderFlags.UseArchivableKey | CspProviderFlags.UseNonExportableKey;
                if ((flags & flags2) != CspProviderFlags.NoFlags)
                {
                    throw new ArgumentException(Environment.GetResourceString("Argument_InvalidFlag"));
                }
            }
            if ((flags & CspProviderFlags.UseUserProtectedKey) != CspProviderFlags.NoFlags)
            {
                if (!Environment.UserInteractive)
                {
                    throw new InvalidOperationException(Environment.GetResourceString("Cryptography_NotInteractive"));
                }
                new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
            }
        }

        internal static int DefaultRsaProviderType
        {
            get
            {
                if (_defaultRsaProviderType == -1)
                {
                    _defaultRsaProviderType = ((Environment.OSVersion.Version.Major > 5) || ((Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor >= 1))) ? 0x18 : 1;
                }
                return _defaultRsaProviderType;
            }
        }

        internal static int FipsAlgorithmPolicy
        {
            [RegistryPermission(SecurityAction.Assert, Unrestricted=true)]
            get
            {
                if (s_fipsAlgorithmPolicy == -1)
                {
                    if (!_GetEnforceFipsPolicySetting())
                    {
                        s_fipsAlgorithmPolicy = 0;
                    }
                    else if (Environment.OSVersion.Version.Major >= 6)
                    {
                        bool flag;
                        uint num = Win32Native.BCryptGetFipsAlgorithmMode(out flag);
                        if (((num != 0) && (num != 0xc0000034)) || flag)
                        {
                            s_fipsAlgorithmPolicy = 1;
                        }
                        else
                        {
                            s_fipsAlgorithmPolicy = 0;
                        }
                    }
                    else
                    {
                        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\Lsa", false))
                        {
                            if (key != null)
                            {
                                object obj2 = key.GetValue("FIPSAlgorithmPolicy");
                                if (obj2 != null)
                                {
                                    s_fipsAlgorithmPolicy = (int) obj2;
                                }
                            }
                        }
                    }
                }
                return s_fipsAlgorithmPolicy;
            }
        }

        internal static int HasEnhProv
        {
            get
            {
                if (s_hasEnhProv == -1)
                {
                    s_hasEnhProv = HasAlgorithm(0xa400, 0x800) ? 1 : 0;
                }
                return s_hasEnhProv;
            }
        }

        private static object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, obj2, null);
                }
                return s_InternalSyncObject;
            }
        }

        internal static SafeProvHandle StaticDssProvHandle
        {
            get
            {
                if (_safeDssProvHandle == null)
                {
                    lock (InternalSyncObject)
                    {
                        if (_safeDssProvHandle == null)
                        {
                            SafeProvHandle handle = AcquireProvHandle(new CspParameters(13));
                            Thread.MemoryBarrier();
                            _safeDssProvHandle = handle;
                        }
                    }
                }
                return _safeDssProvHandle;
            }
        }

        internal static SafeProvHandle StaticProvHandle
        {
            get
            {
                if (_safeProvHandle == null)
                {
                    lock (InternalSyncObject)
                    {
                        if (_safeProvHandle == null)
                        {
                            SafeProvHandle handle = AcquireProvHandle(new CspParameters(DefaultRsaProviderType));
                            Thread.MemoryBarrier();
                            _safeProvHandle = handle;
                        }
                    }
                }
                return _safeProvHandle;
            }
        }

        internal static RNGCryptoServiceProvider StaticRandomNumberGenerator
        {
            get
            {
                if (_rng == null)
                {
                    _rng = new RNGCryptoServiceProvider();
                }
                return _rng;
            }
        }

        internal static int Win2KCrypto
        {
            get
            {
                if (s_win2KCrypto == -1)
                {
                    Win32Native.OSVERSIONINFO ver = new Win32Native.OSVERSIONINFO();
                    s_win2KCrypto = ((Win32Native.GetVersionEx(ver) && (ver.PlatformId == 2)) && (ver.MajorVersion >= 5)) ? 1 : 0;
                }
                return s_win2KCrypto;
            }
        }
    }
}

