namespace System.Security.Cryptography
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;

    [ComVisible(true)]
    public class PasswordDeriveBytes : DeriveBytes
    {
        private byte[] _baseValue;
        private CspParameters _cspParams;
        private byte[] _extra;
        private int _extraCount;
        private HashAlgorithm _hash;
        private string _hashName;
        private int _iterations;
        private byte[] _password;
        private int _prefix;
        private SafeProvHandle _safeProvHandle;
        private byte[] _salt;

        public PasswordDeriveBytes(string strPassword, byte[] rgbSalt) : this(strPassword, rgbSalt, new CspParameters())
        {
        }

        public PasswordDeriveBytes(byte[] password, byte[] salt) : this(password, salt, new CspParameters())
        {
        }

        public PasswordDeriveBytes(string strPassword, byte[] rgbSalt, CspParameters cspParams) : this(strPassword, rgbSalt, "SHA1", 100, cspParams)
        {
        }

        public PasswordDeriveBytes(byte[] password, byte[] salt, CspParameters cspParams) : this(password, salt, "SHA1", 100, cspParams)
        {
        }

        public PasswordDeriveBytes(string strPassword, byte[] rgbSalt, string strHashName, int iterations) : this(strPassword, rgbSalt, strHashName, iterations, new CspParameters())
        {
        }

        public PasswordDeriveBytes(byte[] password, byte[] salt, string hashName, int iterations) : this(password, salt, hashName, iterations, new CspParameters())
        {
        }

        public PasswordDeriveBytes(string strPassword, byte[] rgbSalt, string strHashName, int iterations, CspParameters cspParams) : this(new UTF8Encoding(false).GetBytes(strPassword), rgbSalt, strHashName, iterations, cspParams)
        {
        }

        public PasswordDeriveBytes(byte[] password, byte[] salt, string hashName, int iterations, CspParameters cspParams)
        {
            this.IterationCount = iterations;
            this.Salt = salt;
            this.HashName = hashName;
            this._password = password;
            this._cspParams = cspParams;
        }

        private byte[] ComputeBaseValue()
        {
            this._hash.Initialize();
            this._hash.TransformBlock(this._password, 0, this._password.Length, this._password, 0);
            if (this._salt != null)
            {
                this._hash.TransformBlock(this._salt, 0, this._salt.Length, this._salt, 0);
            }
            this._hash.TransformFinalBlock(new byte[0], 0, 0);
            this._baseValue = this._hash.Hash;
            this._hash.Initialize();
            for (int i = 1; i < (this._iterations - 1); i++)
            {
                this._hash.ComputeHash(this._baseValue);
                this._baseValue = this._hash.Hash;
            }
            return this._baseValue;
        }

        private byte[] ComputeBytes(int cb)
        {
            int dstOffset = 0;
            this._hash.Initialize();
            int count = this._hash.HashSize / 8;
            byte[] dst = new byte[(((cb + count) - 1) / count) * count];
            CryptoStream cs = new CryptoStream(Stream.Null, this._hash, CryptoStreamMode.Write);
            this.HashPrefix(cs);
            cs.Write(this._baseValue, 0, this._baseValue.Length);
            cs.Close();
            Buffer.InternalBlockCopy(this._hash.Hash, 0, dst, dstOffset, count);
            for (dstOffset += count; cb > dstOffset; dstOffset += count)
            {
                this._hash.Initialize();
                cs = new CryptoStream(Stream.Null, this._hash, CryptoStreamMode.Write);
                this.HashPrefix(cs);
                cs.Write(this._baseValue, 0, this._baseValue.Length);
                cs.Close();
                Buffer.InternalBlockCopy(this._hash.Hash, 0, dst, dstOffset, count);
            }
            return dst;
        }

        public byte[] CryptDeriveKey(string algname, string alghashname, int keySize, byte[] rgbIV)
        {
            if (keySize < 0)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidKeySize"));
            }
            int algidHash = X509Utils.OidToAlgId(alghashname);
            if (algidHash == 0)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_PasswordDerivedBytes_InvalidAlgorithm"));
            }
            int algid = X509Utils.OidToAlgId(algname);
            if (algid == 0)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_PasswordDerivedBytes_InvalidAlgorithm"));
            }
            if (rgbIV == null)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_PasswordDerivedBytes_InvalidIV"));
            }
            return Utils._CryptDeriveKey(this.ProvHandle, algid, algidHash, this._password, keySize << 0x10, rgbIV);
        }

        [Obsolete("Rfc2898DeriveBytes replaces PasswordDeriveBytes for deriving key material from a password and is preferred in new applications.")]
        public override byte[] GetBytes(int cb)
        {
            int srcOffset = 0;
            byte[] dst = new byte[cb];
            if (this._baseValue == null)
            {
                this.ComputeBaseValue();
            }
            else if (this._extra != null)
            {
                srcOffset = this._extra.Length - this._extraCount;
                if (srcOffset >= cb)
                {
                    Buffer.InternalBlockCopy(this._extra, this._extraCount, dst, 0, cb);
                    if (srcOffset > cb)
                    {
                        this._extraCount += cb;
                        return dst;
                    }
                    this._extra = null;
                    return dst;
                }
                Buffer.InternalBlockCopy(this._extra, srcOffset, dst, 0, srcOffset);
                this._extra = null;
            }
            byte[] src = this.ComputeBytes(cb - srcOffset);
            Buffer.InternalBlockCopy(src, 0, dst, srcOffset, cb - srcOffset);
            if ((src.Length + srcOffset) > cb)
            {
                this._extra = src;
                this._extraCount = cb - srcOffset;
            }
            return dst;
        }

        private void HashPrefix(CryptoStream cs)
        {
            int index = 0;
            byte[] buffer = new byte[] { 0x30, 0x30, 0x30 };
            if (this._prefix > 0x3e7)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_PasswordDerivedBytes_TooManyBytes"));
            }
            if (this._prefix >= 100)
            {
                buffer[0] = (byte) (buffer[0] + ((byte) (this._prefix / 100)));
                index++;
            }
            if (this._prefix >= 10)
            {
                buffer[index] = (byte) (buffer[index] + ((byte) ((this._prefix % 100) / 10)));
                index++;
            }
            if (this._prefix > 0)
            {
                buffer[index] = (byte) (buffer[index] + ((byte) (this._prefix % 10)));
                index++;
                cs.Write(buffer, 0, index);
            }
            this._prefix++;
        }

        public override void Reset()
        {
            this._prefix = 0;
            this._extra = null;
            this._baseValue = null;
        }

        public string HashName
        {
            get => 
                this._hashName;
            set
            {
                if (this._baseValue != null)
                {
                    throw new CryptographicException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Cryptography_PasswordDerivedBytes_ValuesFixed"), new object[] { "HashName" }));
                }
                this._hashName = value;
                this._hash = (HashAlgorithm) CryptoConfig.CreateFromName(this._hashName);
            }
        }

        public int IterationCount
        {
            get => 
                this._iterations;
            set
            {
                if (this._baseValue != null)
                {
                    throw new CryptographicException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Cryptography_PasswordDerivedBytes_ValuesFixed"), new object[] { "IterationCount" }));
                }
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
                }
                this._iterations = value;
            }
        }

        private SafeProvHandle ProvHandle
        {
            get
            {
                if (this._safeProvHandle == null)
                {
                    lock (this)
                    {
                        if (this._safeProvHandle == null)
                        {
                            SafeProvHandle handle = Utils.AcquireProvHandle(this._cspParams);
                            Thread.MemoryBarrier();
                            this._safeProvHandle = handle;
                        }
                    }
                }
                return this._safeProvHandle;
            }
        }

        public byte[] Salt
        {
            get
            {
                if (this._salt == null)
                {
                    return null;
                }
                return (byte[]) this._salt.Clone();
            }
            set
            {
                if (this._baseValue != null)
                {
                    throw new CryptographicException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Cryptography_PasswordDerivedBytes_ValuesFixed"), new object[] { "Salt" }));
                }
                if (value == null)
                {
                    this._salt = null;
                }
                else
                {
                    this._salt = (byte[]) value.Clone();
                }
            }
        }
    }
}

