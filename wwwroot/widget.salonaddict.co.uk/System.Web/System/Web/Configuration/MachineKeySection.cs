namespace System.Web.Configuration
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class MachineKeySection : ConfigurationSection
    {
        private bool _AutogenKey;
        private MachineKeyValidation _cachedValidation;
        private byte[] _DecryptionKey;
        private static readonly ConfigurationProperty _propCompatibilityMode = new ConfigurationProperty("compatibilityMode", typeof(MachineKeyCompatibilityMode), MachineKeyCompatibilityMode.Framework20SP1, null, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propDecryption = new ConfigurationProperty("decryption", typeof(string), "Auto", StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propDecryptionKey = new ConfigurationProperty("decryptionKey", typeof(string), "AutoGenerate,IsolateApps", StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propValidation = new ConfigurationProperty("validation", typeof(MachineKeyValidation), MachineKeyValidation.SHA1, new MachineKeyValidationConverter(), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propValidationKey = new ConfigurationProperty("validationKey", typeof(string), "AutoGenerate,IsolateApps", StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.None);
        private bool _validationIsCached;
        private byte[] _ValidationKey;
        private const int BLOCK_SIZE = 0x40;
        private bool DataInitialized;
        private const int HASH_SIZE = 20;
        private static char[] s_acharval;
        private static byte[] s_ahexval;
        private static MachineKeyCompatibilityMode s_compatMode;
        private static MachineKeySection s_config;
        private static object s_initLock = new object();
        private static byte[] s_inner = null;
        private static Stack s_oDecryptorStackDecryption;
        private static Stack s_oDecryptorStackValidation;
        private static Stack s_oEncryptorStackDecryption;
        private static Stack s_oEncryptorStackValidation;
        private static SymmetricAlgorithm s_oSymAlgoDecryption;
        private static SymmetricAlgorithm s_oSymAlgoValidation;
        private static byte[] s_outer = null;
        private static RNGCryptoServiceProvider s_randomNumberGenerator;
        private static byte[] s_validationKey;

        static MachineKeySection()
        {
            _properties.Add(_propValidationKey);
            _properties.Add(_propDecryptionKey);
            _properties.Add(_propValidation);
            _properties.Add(_propDecryption);
            _properties.Add(_propCompatibilityMode);
        }

        internal static unsafe string ByteArrayToHexString(byte[] buf, int iLen)
        {
            char[] chArray = s_acharval;
            if (chArray == null)
            {
                chArray = new char[0x10];
                int length = chArray.Length;
                while (--length >= 0)
                {
                    if (length < 10)
                    {
                        chArray[length] = (char) (0x30 + length);
                    }
                    else
                    {
                        chArray[length] = (char) (0x41 + (length - 10));
                    }
                }
                s_acharval = chArray;
            }
            if (buf == null)
            {
                return null;
            }
            if (iLen == 0)
            {
                iLen = buf.Length;
            }
            char[] chArray2 = new char[iLen * 2];
            fixed (char* chRef = chArray2)
            {
                fixed (char* chRef2 = chArray)
                {
                    fixed (byte* numRef = buf)
                    {
                        char* chPtr = chRef;
                        for (byte* numPtr = numRef; --iLen >= 0; numPtr++)
                        {
                            chPtr++;
                            chPtr[0] = chRef2[(numPtr[0] & 240) >> 4];
                            chPtr++;
                            chPtr[0] = chRef2[numPtr[0] & 15];
                        }
                    }
                }
            }
            return new string(chArray2);
        }

        private void ConfigureEncryptionObject()
        {
            using (new ApplicationImpersonationContext())
            {
                s_validationKey = this.ValidationKeyInternal;
                byte[] decryptionKeyInternal = this.DecryptionKeyInternal;
                SetInnerOuterKeys(s_validationKey);
                this.DestroyKeys();
                switch (this.Decryption)
                {
                    case "3DES":
                        s_oSymAlgoDecryption = new TripleDESCryptoServiceProvider();
                        break;

                    case "DES":
                        s_oSymAlgoDecryption = new DESCryptoServiceProvider();
                        break;

                    case "AES":
                        s_oSymAlgoDecryption = new RijndaelManaged();
                        break;

                    default:
                        if (decryptionKeyInternal.Length == 8)
                        {
                            s_oSymAlgoDecryption = new DESCryptoServiceProvider();
                        }
                        else
                        {
                            s_oSymAlgoDecryption = new RijndaelManaged();
                        }
                        break;
                }
                switch (this.Validation)
                {
                    case MachineKeyValidation.TripleDES:
                        if (decryptionKeyInternal.Length != 8)
                        {
                            break;
                        }
                        s_oSymAlgoValidation = new DESCryptoServiceProvider();
                        goto Label_00DC;

                    case MachineKeyValidation.AES:
                        s_oSymAlgoValidation = new RijndaelManaged();
                        goto Label_00DC;

                    default:
                        goto Label_00DC;
                }
                s_oSymAlgoValidation = new TripleDESCryptoServiceProvider();
            Label_00DC:
                if (s_oSymAlgoValidation != null)
                {
                    this.SetKeyOnSymAlgorithm(s_oSymAlgoValidation, decryptionKeyInternal);
                    s_oEncryptorStackValidation = new Stack();
                    s_oDecryptorStackValidation = new Stack();
                }
                this.SetKeyOnSymAlgorithm(s_oSymAlgoDecryption, decryptionKeyInternal);
                s_oEncryptorStackDecryption = new Stack();
                s_oDecryptorStackDecryption = new Stack();
                DestroyByteArray(decryptionKeyInternal);
            }
        }

        internal static void DestroyByteArray(byte[] buf)
        {
            if ((buf != null) && (buf.Length >= 1))
            {
                for (int i = 0; i < buf.Length; i++)
                {
                    buf[i] = 0;
                }
            }
        }

        internal void DestroyKeys()
        {
            DestroyByteArray(this._ValidationKey);
            DestroyByteArray(this._DecryptionKey);
        }

        internal static byte[] EncryptOrDecryptData(bool fEncrypt, byte[] buf, byte[] modifier, int start, int length) => 
            EncryptOrDecryptData(fEncrypt, buf, modifier, start, length, IVType.Random, false);

        internal static byte[] EncryptOrDecryptData(bool fEncrypt, byte[] buf, byte[] modifier, int start, int length, bool useValidationSymAlgo) => 
            EncryptOrDecryptData(fEncrypt, buf, modifier, start, length, IVType.Random, useValidationSymAlgo);

        internal static byte[] EncryptOrDecryptData(bool fEncrypt, byte[] buf, byte[] modifier, int start, int length, IVType ivType) => 
            EncryptOrDecryptData(fEncrypt, buf, modifier, start, length, ivType, false);

        internal static byte[] EncryptOrDecryptData(bool fEncrypt, byte[] buf, byte[] modifier, int start, int length, IVType ivType, bool useValidationSymAlgo) => 
            EncryptOrDecryptData(fEncrypt, buf, modifier, start, length, ivType, useValidationSymAlgo, !AppSettings.UseLegacyEncryption);

        internal static byte[] EncryptOrDecryptData(bool fEncrypt, byte[] buf, byte[] modifier, int start, int length, IVType ivType, bool useValidationSymAlgo, bool signData)
        {
            byte[] buffer8;
            try
            {
                byte[] buffer4;
                EnsureConfig();
                if (!fEncrypt && signData)
                {
                    if ((start != 0) || (length != buf.Length))
                    {
                        byte[] dst = new byte[length];
                        Buffer.BlockCopy(buf, start, dst, 0, length);
                        buf = dst;
                        start = 0;
                    }
                    buf = GetUnHashedData(buf);
                    if (buf == null)
                    {
                        throw new HttpException(System.Web.SR.GetString("Unable_to_validate_data"));
                    }
                    length = buf.Length;
                }
                MemoryStream stream = new MemoryStream();
                ICryptoTransform cryptoTransform = GetCryptoTransform(fEncrypt, useValidationSymAlgo);
                CryptoStream stream2 = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write);
                bool flag = signData || ((ivType != IVType.None) && (CompatMode > MachineKeyCompatibilityMode.Framework20SP1));
                if (fEncrypt && flag)
                {
                    byte[] buffer = null;
                    switch (ivType)
                    {
                        case IVType.Random:
                            buffer = GetIVRandom(useValidationSymAlgo);
                            break;

                        case IVType.Hash:
                            buffer = GetIVHash(buf, useValidationSymAlgo);
                            break;
                    }
                    stream2.Write(buffer, 0, buffer.Length);
                }
                stream2.Write(buf, start, length);
                if (fEncrypt && (modifier != null))
                {
                    stream2.Write(modifier, 0, modifier.Length);
                }
                stream2.FlushFinalBlock();
                byte[] src = stream.ToArray();
                stream2.Close();
                ReturnCryptoTransform(fEncrypt, cryptoTransform, useValidationSymAlgo);
                if (!fEncrypt && flag)
                {
                    int iVLength = GetIVLength(useValidationSymAlgo);
                    int count = src.Length - iVLength;
                    if (count < 0)
                    {
                        throw new Exception();
                    }
                    buffer4 = new byte[count];
                    Buffer.BlockCopy(src, iVLength, buffer4, 0, count);
                }
                else
                {
                    buffer4 = src;
                }
                if ((!fEncrypt && (modifier != null)) && (modifier.Length > 0))
                {
                    bool flag2 = false;
                    for (int i = 0; i < modifier.Length; i++)
                    {
                        if (buffer4[(buffer4.Length - modifier.Length) + i] != modifier[i])
                        {
                            flag2 = true;
                        }
                    }
                    if (flag2)
                    {
                        throw new HttpException(System.Web.SR.GetString("Unable_to_validate_data"));
                    }
                    byte[] buffer5 = new byte[buffer4.Length - modifier.Length];
                    Buffer.BlockCopy(buffer4, 0, buffer5, 0, buffer5.Length);
                    buffer4 = buffer5;
                }
                if (fEncrypt && signData)
                {
                    byte[] buffer6 = HashData(buffer4, null, 0, buffer4.Length);
                    byte[] buffer7 = new byte[buffer4.Length + buffer6.Length];
                    Buffer.BlockCopy(buffer4, 0, buffer7, 0, buffer4.Length);
                    Buffer.BlockCopy(buffer6, 0, buffer7, buffer4.Length, buffer6.Length);
                    buffer4 = buffer7;
                }
                buffer8 = buffer4;
            }
            catch
            {
                throw new HttpException(System.Web.SR.GetString("Unable_to_validate_data"));
            }
            return buffer8;
        }

        private static void EnsureConfig()
        {
            if (s_config == null)
            {
                lock (s_initLock)
                {
                    if (s_config == null)
                    {
                        MachineKeySection machineKey = RuntimeConfig.GetAppConfig().MachineKey;
                        machineKey.ConfigureEncryptionObject();
                        s_config = machineKey;
                        s_compatMode = machineKey.CompatibilityMode;
                    }
                }
            }
        }

        private static ICryptoTransform GetCryptoTransform(bool fEncrypt, bool useValidationSymAlgo)
        {
            Stack stack;
            if (useValidationSymAlgo)
            {
                stack = fEncrypt ? s_oEncryptorStackValidation : s_oDecryptorStackValidation;
            }
            else
            {
                stack = fEncrypt ? s_oEncryptorStackDecryption : s_oDecryptorStackDecryption;
            }
            lock (stack)
            {
                if (stack.Count > 0)
                {
                    return (ICryptoTransform) stack.Pop();
                }
            }
            if (useValidationSymAlgo)
            {
                lock (s_oSymAlgoValidation)
                {
                    return (fEncrypt ? s_oSymAlgoValidation.CreateEncryptor() : s_oSymAlgoValidation.CreateDecryptor());
                }
            }
            lock (s_oSymAlgoDecryption)
            {
                return (fEncrypt ? s_oSymAlgoDecryption.CreateEncryptor() : s_oSymAlgoDecryption.CreateDecryptor());
            }
        }

        internal static byte[] GetDecodedData(byte[] buf, byte[] modifier, int start, int length, ref int dataLength)
        {
            EnsureConfig();
            if ((s_config.Validation == MachineKeyValidation.TripleDES) || (s_config.Validation == MachineKeyValidation.AES))
            {
                buf = EncryptOrDecryptData(false, buf, modifier, start, length, true);
                if ((buf == null) || (buf.Length < 20))
                {
                    throw new HttpException(System.Web.SR.GetString("Unable_to_validate_data"));
                }
                length = buf.Length;
                start = 0;
            }
            if (((length < 20) || (start < 0)) || (start >= length))
            {
                throw new HttpException(System.Web.SR.GetString("Unable_to_validate_data"));
            }
            byte[] buffer = HashData(buf, modifier, start, length - 20);
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != buf[((start + length) - 20) + i])
                {
                    throw new HttpException(System.Web.SR.GetString("Unable_to_validate_data"));
                }
            }
            dataLength = length - 20;
            return buf;
        }

        internal static byte[] GetEncodedData(byte[] buf, byte[] modifier, int start, ref int length)
        {
            byte[] buffer2;
            EnsureConfig();
            byte[] src = HashData(buf, modifier, start, length);
            if (((buf.Length - start) - length) >= src.Length)
            {
                Buffer.BlockCopy(src, 0, buf, start + length, src.Length);
                buffer2 = buf;
            }
            else
            {
                buffer2 = new byte[length + src.Length];
                Buffer.BlockCopy(buf, start, buffer2, 0, length);
                Buffer.BlockCopy(src, 0, buffer2, length, src.Length);
                start = 0;
            }
            length += src.Length;
            if ((s_config.Validation == MachineKeyValidation.TripleDES) || (s_config.Validation == MachineKeyValidation.AES))
            {
                buffer2 = EncryptOrDecryptData(true, buffer2, modifier, start, length, true);
                length = buffer2.Length;
            }
            return buffer2;
        }

        private static byte[] GetHMACSHA1Hash(byte[] buf, byte[] modifier, int start, int length)
        {
            if (((length < 0) || (buf == null)) || (length > buf.Length))
            {
                throw new ArgumentException(System.Web.SR.GetString("InvalidArgumentValue", new object[] { "length" }));
            }
            if ((start < 0) || (start >= length))
            {
                throw new ArgumentException(System.Web.SR.GetString("InvalidArgumentValue", new object[] { "start" }));
            }
            byte[] hash = new byte[20];
            Marshal.ThrowExceptionForHR(System.Web.UnsafeNativeMethods.GetHMACSHA1Hash(buf, start, length, modifier, (modifier == null) ? 0 : modifier.Length, s_inner, s_inner.Length, s_outer, s_outer.Length, hash, hash.Length));
            return hash;
        }

        private static byte[] GetIVHash(byte[] buf, bool useValidationSymAlgo)
        {
            int iVLength = GetIVLength(useValidationSymAlgo);
            int num2 = iVLength;
            int dstOffset = 0;
            byte[] dst = new byte[iVLength];
            byte[] data = buf;
            while (dstOffset < iVLength)
            {
                byte[] hash = new byte[20];
                Marshal.ThrowExceptionForHR(System.Web.UnsafeNativeMethods.GetSHA1Hash(data, data.Length, hash, hash.Length));
                data = hash;
                int count = Math.Min(20, num2);
                Buffer.BlockCopy(data, 0, dst, dstOffset, count);
                dstOffset += count;
                num2 -= count;
            }
            return dst;
        }

        private static int GetIVLength(bool useValidationSymAlgo)
        {
            SymmetricAlgorithm algorithm = useValidationSymAlgo ? s_oSymAlgoValidation : s_oSymAlgoDecryption;
            int keySize = algorithm.KeySize;
            if ((keySize % 8) != 0)
            {
                keySize += 8 - (keySize % 8);
            }
            return (keySize / 8);
        }

        private static byte[] GetIVRandom(bool useValidationSymAlgo)
        {
            byte[] data = new byte[GetIVLength(useValidationSymAlgo)];
            if (s_randomNumberGenerator == null)
            {
                s_randomNumberGenerator = new RNGCryptoServiceProvider();
            }
            s_randomNumberGenerator.GetBytes(data);
            return data;
        }

        internal static byte[] GetUnHashedData(byte[] bufHashed)
        {
            if (!VerifyHashedData(bufHashed))
            {
                return null;
            }
            byte[] dst = new byte[bufHashed.Length - 20];
            Buffer.BlockCopy(bufHashed, 0, dst, 0, dst.Length);
            return dst;
        }

        internal static string HashAndBase64EncodeString(string s)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(s);
            return Convert.ToBase64String(HashData(bytes, null, 0, bytes.Length));
        }

        internal static byte[] HashData(byte[] buf, byte[] modifier, int start, int length)
        {
            EnsureConfig();
            byte[] src = null;
            if (s_config.Validation == MachineKeyValidation.MD5)
            {
                src = MD5HashForData(buf, modifier, start, length);
            }
            else
            {
                src = GetHMACSHA1Hash(buf, modifier, start, length);
            }
            if (src.Length < 20)
            {
                byte[] dst = new byte[20];
                Buffer.BlockCopy(src, 0, dst, 0, src.Length);
                src = dst;
            }
            return src;
        }

        internal static byte[] HexStringToByteArray(string str)
        {
            if ((str.Length & 1) == 1)
            {
                return null;
            }
            byte[] buffer = s_ahexval;
            if (buffer == null)
            {
                buffer = new byte[0x67];
                int index = buffer.Length;
                while (--index >= 0)
                {
                    if ((0x30 <= index) && (index <= 0x39))
                    {
                        buffer[index] = (byte) (index - 0x30);
                    }
                    else
                    {
                        if ((0x61 <= index) && (index <= 0x66))
                        {
                            buffer[index] = (byte) ((index - 0x61) + 10);
                            continue;
                        }
                        if ((0x41 <= index) && (index <= 70))
                        {
                            buffer[index] = (byte) ((index - 0x41) + 10);
                        }
                    }
                }
                s_ahexval = buffer;
            }
            byte[] buffer2 = new byte[str.Length / 2];
            int num2 = 0;
            int num3 = 0;
            int length = buffer2.Length;
            while (--length >= 0)
            {
                int num5;
                int num6;
                try
                {
                    num5 = buffer[str[num2++]];
                }
                catch (ArgumentNullException)
                {
                    num5 = 0;
                    return null;
                }
                catch (ArgumentException)
                {
                    num5 = 0;
                    return null;
                }
                catch (IndexOutOfRangeException)
                {
                    num5 = 0;
                    return null;
                }
                try
                {
                    num6 = buffer[str[num2++]];
                }
                catch (ArgumentNullException)
                {
                    num6 = 0;
                    return null;
                }
                catch (ArgumentException)
                {
                    num6 = 0;
                    return null;
                }
                catch (IndexOutOfRangeException)
                {
                    num6 = 0;
                    return null;
                }
                buffer2[num3++] = (byte) ((num5 << 4) + num6);
            }
            return buffer2;
        }

        private static byte[] MD5HashForData(byte[] buf, byte[] modifier, int start, int length)
        {
            MD5 md = MD5.Create();
            int num = length + s_validationKey.Length;
            if (modifier != null)
            {
                num += modifier.Length;
            }
            byte[] dst = new byte[num];
            Buffer.BlockCopy(buf, start, dst, 0, length);
            if (modifier != null)
            {
                Buffer.BlockCopy(modifier, 0, dst, length, modifier.Length);
                length += modifier.Length;
            }
            Buffer.BlockCopy(s_validationKey, 0, dst, length, s_validationKey.Length);
            return md.ComputeHash(dst);
        }

        protected override void Reset(ConfigurationElement parentElement)
        {
            MachineKeySection section = parentElement as MachineKeySection;
            base.Reset(parentElement);
        }

        private static void ReturnCryptoTransform(bool fEncrypt, ICryptoTransform ct, bool useValidationSymAlgo)
        {
            Stack stack;
            if (useValidationSymAlgo)
            {
                stack = fEncrypt ? s_oEncryptorStackValidation : s_oDecryptorStackValidation;
            }
            else
            {
                stack = fEncrypt ? s_oEncryptorStackDecryption : s_oDecryptorStackDecryption;
            }
            lock (stack)
            {
                if (stack.Count <= 100)
                {
                    stack.Push(ct);
                }
            }
        }

        private void RuntimeDataInitialize()
        {
            if (!this.DataInitialized)
            {
                byte[] data = null;
                bool flag = false;
                string validationKey = this.ValidationKey;
                string appDomainAppVirtualPath = HttpRuntime.AppDomainAppVirtualPath;
                if (appDomainAppVirtualPath == null)
                {
                    appDomainAppVirtualPath = Process.GetCurrentProcess().MainModule.ModuleName;
                    if (this.ValidationKey.Contains("AutoGenerate") || this.DecryptionKey.Contains("AutoGenerate"))
                    {
                        flag = true;
                        data = new byte[0x58];
                        new RNGCryptoServiceProvider().GetBytes(data);
                    }
                }
                bool flag2 = System.Web.Util.StringUtil.StringEndsWith(validationKey, ",IsolateApps");
                if (flag2)
                {
                    validationKey = validationKey.Substring(0, validationKey.Length - ",IsolateApps".Length);
                }
                if (validationKey == "AutoGenerate")
                {
                    this._ValidationKey = new byte[0x40];
                    if (flag)
                    {
                        Buffer.BlockCopy(data, 0, this._ValidationKey, 0, 0x40);
                    }
                    else
                    {
                        Buffer.BlockCopy(HttpRuntime.s_autogenKeys, 0, this._ValidationKey, 0, 0x40);
                    }
                }
                else
                {
                    if ((validationKey.Length > 0x80) || (validationKey.Length < 40))
                    {
                        throw new ConfigurationErrorsException(System.Web.SR.GetString("Unable_to_get_cookie_authentication_validation_key", new object[] { validationKey.Length.ToString(CultureInfo.InvariantCulture) }), base.ElementInformation.Properties["validationKey"].Source, base.ElementInformation.Properties["validationKey"].LineNumber);
                    }
                    this._ValidationKey = HexStringToByteArray(validationKey);
                    if (this._ValidationKey == null)
                    {
                        throw new ConfigurationErrorsException(System.Web.SR.GetString("Invalid_validation_key"), base.ElementInformation.Properties["validationKey"].Source, base.ElementInformation.Properties["validationKey"].LineNumber);
                    }
                }
                if (flag2)
                {
                    int hashCode = StringComparer.InvariantCultureIgnoreCase.GetHashCode(appDomainAppVirtualPath);
                    this._ValidationKey[0] = (byte) (hashCode & 0xff);
                    this._ValidationKey[1] = (byte) ((hashCode & 0xff00) >> 8);
                    this._ValidationKey[2] = (byte) ((hashCode & 0xff0000) >> 0x10);
                    this._ValidationKey[3] = (byte) ((hashCode & 0xff000000L) >> 0x18);
                }
                validationKey = this.DecryptionKey;
                flag2 = System.Web.Util.StringUtil.StringEndsWith(validationKey, ",IsolateApps");
                if (flag2)
                {
                    validationKey = validationKey.Substring(0, validationKey.Length - ",IsolateApps".Length);
                }
                if (validationKey == "AutoGenerate")
                {
                    this._DecryptionKey = new byte[0x18];
                    if (flag)
                    {
                        Buffer.BlockCopy(data, 0x40, this._DecryptionKey, 0, 0x18);
                    }
                    else
                    {
                        Buffer.BlockCopy(HttpRuntime.s_autogenKeys, 0x40, this._DecryptionKey, 0, 0x18);
                    }
                    this._AutogenKey = true;
                }
                else
                {
                    this._AutogenKey = false;
                    if ((validationKey.Length % 2) != 0)
                    {
                        throw new ConfigurationErrorsException(System.Web.SR.GetString("Invalid_decryption_key"), base.ElementInformation.Properties["decryptionKey"].Source, base.ElementInformation.Properties["decryptionKey"].LineNumber);
                    }
                    this._DecryptionKey = HexStringToByteArray(validationKey);
                    if (this._DecryptionKey == null)
                    {
                        throw new ConfigurationErrorsException(System.Web.SR.GetString("Invalid_decryption_key"), base.ElementInformation.Properties["decryptionKey"].Source, base.ElementInformation.Properties["decryptionKey"].LineNumber);
                    }
                }
                if (flag2)
                {
                    int num2 = StringComparer.InvariantCultureIgnoreCase.GetHashCode(appDomainAppVirtualPath);
                    this._DecryptionKey[0] = (byte) (num2 & 0xff);
                    this._DecryptionKey[1] = (byte) ((num2 & 0xff00) >> 8);
                    this._DecryptionKey[2] = (byte) ((num2 & 0xff0000) >> 0x10);
                    this._DecryptionKey[3] = (byte) ((num2 & 0xff000000L) >> 0x18);
                }
                this.DataInitialized = true;
            }
        }

        private static void SetInnerOuterKeys(byte[] validationKey)
        {
            byte[] hash = null;
            int num2;
            if (validationKey.Length > 0x40)
            {
                hash = new byte[20];
                Marshal.ThrowExceptionForHR(System.Web.UnsafeNativeMethods.GetSHA1Hash(validationKey, validationKey.Length, hash, hash.Length));
            }
            if (s_inner == null)
            {
                s_inner = new byte[0x40];
            }
            if (s_outer == null)
            {
                s_outer = new byte[0x40];
            }
            for (num2 = 0; num2 < 0x40; num2++)
            {
                s_inner[num2] = 0x36;
                s_outer[num2] = 0x5c;
            }
            for (num2 = 0; num2 < validationKey.Length; num2++)
            {
                s_inner[num2] = (byte) (s_inner[num2] ^ validationKey[num2]);
                s_outer[num2] = (byte) (s_outer[num2] ^ validationKey[num2]);
            }
        }

        private void SetKeyOnSymAlgorithm(SymmetricAlgorithm symAlgo, byte[] dKey)
        {
            try
            {
                if ((dKey.Length == 0x18) && (symAlgo is DESCryptoServiceProvider))
                {
                    byte[] dst = new byte[8];
                    Buffer.BlockCopy(dKey, 0, dst, 0, 8);
                    symAlgo.Key = dst;
                    DestroyByteArray(dst);
                }
                else
                {
                    symAlgo.Key = dKey;
                }
                symAlgo.GenerateIV();
                symAlgo.IV = new byte[symAlgo.IV.Length];
            }
            catch (Exception exception)
            {
                throw new ConfigurationErrorsException(System.Web.SR.GetString("Bad_machine_key", new object[] { exception.Message }), base.ElementInformation.Properties["decryptionKey"].Source, base.ElementInformation.Properties["decryptionKey"].LineNumber);
            }
        }

        internal static bool VerifyHashedData(byte[] bufHashed)
        {
            EnsureConfig();
            if (bufHashed.Length <= 20)
            {
                return false;
            }
            byte[] buffer = HashData(bufHashed, null, 0, bufHashed.Length - 20);
            if ((buffer == null) || (buffer.Length != 20))
            {
                return false;
            }
            int num = bufHashed.Length - 20;
            bool flag = false;
            for (int i = 0; i < 20; i++)
            {
                if (buffer[i] != bufHashed[num + i])
                {
                    flag = true;
                }
            }
            return !flag;
        }

        internal bool AutogenKey
        {
            get
            {
                this.RuntimeDataInitialize();
                return this._AutogenKey;
            }
        }

        [ConfigurationProperty("compatibilityMode", DefaultValue=0)]
        public MachineKeyCompatibilityMode CompatibilityMode
        {
            get => 
                ((MachineKeyCompatibilityMode) base[_propCompatibilityMode]);
            set
            {
                base[_propCompatibilityMode] = value;
            }
        }

        internal static MachineKeyCompatibilityMode CompatMode
        {
            get
            {
                EnsureConfig();
                return s_compatMode;
            }
        }

        [StringValidator(MinLength=1), ConfigurationProperty("decryption", DefaultValue="Auto"), TypeConverter(typeof(WhiteSpaceTrimStringConverter))]
        public string Decryption
        {
            get
            {
                string str = base[_propDecryption] as string;
                if (str == null)
                {
                    return "Auto";
                }
                if (((str != "Auto") && (str != "AES")) && ((str != "3DES") && (str != "DES")))
                {
                    throw new ConfigurationErrorsException(System.Web.SR.GetString("Wrong_decryption_enum"), base.ElementInformation.Properties["decryption"].Source, base.ElementInformation.Properties["decryption"].LineNumber);
                }
                return str;
            }
            set
            {
                if (((value != "AES") && (value != "3DES")) && ((value != "Auto") && (value != "DES")))
                {
                    throw new ConfigurationErrorsException(System.Web.SR.GetString("Wrong_decryption_enum"), base.ElementInformation.Properties["decryption"].Source, base.ElementInformation.Properties["decryption"].LineNumber);
                }
                base[_propDecryption] = value;
            }
        }

        [StringValidator(MinLength=1), ConfigurationProperty("decryptionKey", DefaultValue="AutoGenerate,IsolateApps"), TypeConverter(typeof(WhiteSpaceTrimStringConverter))]
        public string DecryptionKey
        {
            get => 
                ((string) base[_propDecryptionKey]);
            set
            {
                base[_propDecryptionKey] = value;
            }
        }

        internal byte[] DecryptionKeyInternal
        {
            get
            {
                this.RuntimeDataInitialize();
                return (byte[]) this._DecryptionKey.Clone();
            }
        }

        internal static bool IsDecryptionKeyAutogenerated
        {
            get
            {
                EnsureConfig();
                return s_config.AutogenKey;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("validation", DefaultValue=1), TypeConverter(typeof(MachineKeyValidationConverter))]
        public MachineKeyValidation Validation
        {
            get
            {
                if (!this._validationIsCached)
                {
                    this._cachedValidation = (MachineKeyValidation) base[_propValidation];
                    this._validationIsCached = true;
                }
                return this._cachedValidation;
            }
            set
            {
                base[_propValidation] = value;
                this._cachedValidation = value;
                this._validationIsCached = true;
            }
        }

        [ConfigurationProperty("validationKey", DefaultValue="AutoGenerate,IsolateApps"), StringValidator(MinLength=1), TypeConverter(typeof(WhiteSpaceTrimStringConverter))]
        public string ValidationKey
        {
            get => 
                ((string) base[_propValidationKey]);
            set
            {
                base[_propValidationKey] = value;
            }
        }

        internal byte[] ValidationKeyInternal
        {
            get
            {
                this.RuntimeDataInitialize();
                return (byte[]) this._ValidationKey.Clone();
            }
        }
    }
}

