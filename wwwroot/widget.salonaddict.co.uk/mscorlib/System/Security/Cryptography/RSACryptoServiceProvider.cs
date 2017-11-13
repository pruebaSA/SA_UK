namespace System.Security.Cryptography
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Permissions;

    [ComVisible(true)]
    public sealed class RSACryptoServiceProvider : RSA, ICspAsymmetricAlgorithm
    {
        private int _dwKeySize;
        private CspParameters _parameters;
        private bool _randomKeyContainer;
        private SafeKeyHandle _safeKeyHandle;
        private SafeProvHandle _safeProvHandle;
        private const uint RandomKeyContainerFlag = 0x80000000;
        private static CspProviderFlags s_UseMachineKeyStore;

        public RSACryptoServiceProvider() : this(0, new CspParameters(Utils.DefaultRsaProviderType, null, null, s_UseMachineKeyStore), true)
        {
        }

        public RSACryptoServiceProvider(int dwKeySize) : this(dwKeySize, new CspParameters(Utils.DefaultRsaProviderType, null, null, s_UseMachineKeyStore), false)
        {
        }

        public RSACryptoServiceProvider(CspParameters parameters) : this(0, parameters, true)
        {
        }

        public RSACryptoServiceProvider(int dwKeySize, CspParameters parameters) : this(dwKeySize, parameters, false)
        {
        }

        private RSACryptoServiceProvider(int dwKeySize, CspParameters parameters, bool useDefaultKeySize)
        {
            if (dwKeySize < 0)
            {
                throw new ArgumentOutOfRangeException("dwKeySize", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            bool flag = (parameters.Flags & -2147483648) != CspProviderFlags.NoFlags;
            parameters.Flags &= 0x7fffffff;
            this._parameters = Utils.SaveCspParameters(CspAlgorithmType.Rsa, parameters, s_UseMachineKeyStore, ref this._randomKeyContainer);
            if ((this._parameters.KeyNumber == 2) || (Utils.HasEnhProv == 1))
            {
                base.LegalKeySizesValue = new KeySizes[] { new KeySizes(0x180, 0x4000, 8) };
                if (useDefaultKeySize)
                {
                    this._dwKeySize = 0x400;
                }
            }
            else
            {
                base.LegalKeySizesValue = new KeySizes[] { new KeySizes(0x180, 0x200, 8) };
                if (useDefaultKeySize)
                {
                    this._dwKeySize = 0x200;
                }
            }
            if (!useDefaultKeySize)
            {
                this._dwKeySize = dwKeySize;
            }
            if (!this._randomKeyContainer || Environment.GetCompatibilityFlag(CompatibilityFlag.EagerlyGenerateRandomAsymmKeys))
            {
                this.GetKeyPair();
            }
            this._randomKeyContainer |= flag;
        }

        public byte[] Decrypt(byte[] rgb, bool fOAEP)
        {
            if (rgb == null)
            {
                throw new ArgumentNullException("rgb");
            }
            this.GetKeyPair();
            if (rgb.Length > (this.KeySize / 8))
            {
                throw new CryptographicException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Cryptography_Padding_DecDataTooBig"), new object[] { this.KeySize / 8 }));
            }
            if (!this._randomKeyContainer)
            {
                KeyContainerPermission permission = new KeyContainerPermission(KeyContainerPermissionFlags.NoFlags);
                KeyContainerPermissionAccessEntry accessEntry = new KeyContainerPermissionAccessEntry(this._parameters, KeyContainerPermissionFlags.Decrypt);
                permission.AccessEntries.Add(accessEntry);
                permission.Demand();
            }
            byte[] buffer = null;
            int hr = 0;
            if (fOAEP)
            {
                if ((Utils.HasEnhProv != 1) || (Utils.Win2KCrypto != 1))
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_Padding_Win2KEnhOnly"));
                }
                buffer = Utils._DecryptPKWin2KEnh(this._safeKeyHandle, rgb, true, out hr);
                if (hr != 0)
                {
                    throw new CryptographicException(hr);
                }
                return buffer;
            }
            buffer = Utils._DecryptPKWin2KEnh(this._safeKeyHandle, rgb, false, out hr);
            if (hr != 0)
            {
                buffer = Utils._DecryptKey(this._safeKeyHandle, rgb, 0);
            }
            return buffer;
        }

        public override byte[] DecryptValue(byte[] rgb)
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_Method"));
        }

        protected override void Dispose(bool disposing)
        {
            if ((this._safeKeyHandle != null) && !this._safeKeyHandle.IsClosed)
            {
                this._safeKeyHandle.Dispose();
            }
            if ((this._safeProvHandle != null) && !this._safeProvHandle.IsClosed)
            {
                this._safeProvHandle.Dispose();
            }
        }

        public byte[] Encrypt(byte[] rgb, bool fOAEP)
        {
            if (rgb == null)
            {
                throw new ArgumentNullException("rgb");
            }
            this.GetKeyPair();
            byte[] buffer = null;
            int hr = 0;
            if (fOAEP)
            {
                if ((Utils.HasEnhProv != 1) || (Utils.Win2KCrypto != 1))
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_Padding_Win2KEnhOnly"));
                }
                buffer = Utils._EncryptPKWin2KEnh(this._safeKeyHandle, rgb, true, out hr);
                if (hr != 0)
                {
                    throw new CryptographicException(hr);
                }
                return buffer;
            }
            buffer = Utils._EncryptPKWin2KEnh(this._safeKeyHandle, rgb, false, out hr);
            if (hr != 0)
            {
                buffer = Utils._EncryptKey(this._safeKeyHandle, rgb);
            }
            return buffer;
        }

        public override byte[] EncryptValue(byte[] rgb)
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_Method"));
        }

        [ComVisible(false)]
        public byte[] ExportCspBlob(bool includePrivateParameters)
        {
            this.GetKeyPair();
            return Utils.ExportCspBlobHelper(includePrivateParameters, this._parameters, this._safeKeyHandle);
        }

        public override RSAParameters ExportParameters(bool includePrivateParameters)
        {
            this.GetKeyPair();
            if (includePrivateParameters)
            {
                KeyContainerPermission permission = new KeyContainerPermission(KeyContainerPermissionFlags.NoFlags);
                KeyContainerPermissionAccessEntry accessEntry = new KeyContainerPermissionAccessEntry(this._parameters, KeyContainerPermissionFlags.Export);
                permission.AccessEntries.Add(accessEntry);
                permission.Demand();
            }
            RSACspObject cspObject = new RSACspObject();
            int blobType = includePrivateParameters ? 7 : 6;
            Utils._ExportKey(this._safeKeyHandle, blobType, cspObject);
            return RSAObjectToStruct(cspObject);
        }

        private void GetKeyPair()
        {
            if (this._safeKeyHandle == null)
            {
                lock (this)
                {
                    if (this._safeKeyHandle == null)
                    {
                        Utils.GetKeyPairHelper(CspAlgorithmType.Rsa, this._parameters, this._randomKeyContainer, this._dwKeySize, ref this._safeProvHandle, ref this._safeKeyHandle);
                    }
                }
            }
        }

        [ComVisible(false)]
        public void ImportCspBlob(byte[] keyBlob)
        {
            Utils.ImportCspBlobHelper(CspAlgorithmType.Rsa, keyBlob, IsPublic(keyBlob), ref this._parameters, this._randomKeyContainer, ref this._safeProvHandle, ref this._safeKeyHandle);
        }

        public override void ImportParameters(RSAParameters parameters)
        {
            RSACspObject cspObject = RSAStructToObject(parameters);
            if ((this._safeKeyHandle != null) && !this._safeKeyHandle.IsClosed)
            {
                this._safeKeyHandle.Dispose();
            }
            this._safeKeyHandle = SafeKeyHandle.InvalidHandle;
            if (IsPublic(parameters))
            {
                Utils._ImportKey(Utils.StaticProvHandle, 0xa400, CspProviderFlags.NoFlags, cspObject, ref this._safeKeyHandle);
            }
            else
            {
                KeyContainerPermission permission = new KeyContainerPermission(KeyContainerPermissionFlags.NoFlags);
                KeyContainerPermissionAccessEntry accessEntry = new KeyContainerPermissionAccessEntry(this._parameters, KeyContainerPermissionFlags.Import);
                permission.AccessEntries.Add(accessEntry);
                permission.Demand();
                if (this._safeProvHandle == null)
                {
                    this._safeProvHandle = Utils.CreateProvHandle(this._parameters, this._randomKeyContainer);
                }
                Utils._ImportKey(this._safeProvHandle, 0xa400, this._parameters.Flags, cspObject, ref this._safeKeyHandle);
            }
        }

        private static bool IsPublic(RSAParameters rsaParams) => 
            (rsaParams.P == null);

        private static bool IsPublic(byte[] keyBlob)
        {
            if (keyBlob == null)
            {
                throw new ArgumentNullException("keyBlob");
            }
            if (keyBlob[0] != 6)
            {
                return false;
            }
            return (((keyBlob[11] == 0x31) && (keyBlob[10] == 0x41)) && ((keyBlob[9] == 0x53) && (keyBlob[8] == 0x52)));
        }

        private static RSAParameters RSAObjectToStruct(RSACspObject rsaCspObject) => 
            new RSAParameters { 
                Exponent = rsaCspObject.Exponent,
                Modulus = rsaCspObject.Modulus,
                P = rsaCspObject.P,
                Q = rsaCspObject.Q,
                DP = rsaCspObject.DP,
                DQ = rsaCspObject.DQ,
                InverseQ = rsaCspObject.InverseQ,
                D = rsaCspObject.D
            };

        private static RSACspObject RSAStructToObject(RSAParameters rsaParams) => 
            new RSACspObject { 
                Exponent = rsaParams.Exponent,
                Modulus = rsaParams.Modulus,
                P = rsaParams.P,
                Q = rsaParams.Q,
                DP = rsaParams.DP,
                DQ = rsaParams.DQ,
                InverseQ = rsaParams.InverseQ,
                D = rsaParams.D
            };

        public byte[] SignData(Stream inputStream, object halg)
        {
            string str = Utils.ObjToOidValue(halg);
            byte[] rgbHash = Utils.ObjToHashAlgorithm(halg).ComputeHash(inputStream);
            return this.SignHash(rgbHash, str);
        }

        public byte[] SignData(byte[] buffer, object halg)
        {
            string str = Utils.ObjToOidValue(halg);
            byte[] rgbHash = Utils.ObjToHashAlgorithm(halg).ComputeHash(buffer);
            return this.SignHash(rgbHash, str);
        }

        public byte[] SignData(byte[] buffer, int offset, int count, object halg)
        {
            string str = Utils.ObjToOidValue(halg);
            byte[] rgbHash = Utils.ObjToHashAlgorithm(halg).ComputeHash(buffer, offset, count);
            return this.SignHash(rgbHash, str);
        }

        public byte[] SignHash(byte[] rgbHash, string str)
        {
            if (rgbHash == null)
            {
                throw new ArgumentNullException("rgbHash");
            }
            if (this.PublicOnly)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_CSP_NoPrivateKey"));
            }
            int calgHash = X509Utils.OidToAlgId(str);
            this.GetKeyPair();
            if (!this._randomKeyContainer)
            {
                KeyContainerPermission permission = new KeyContainerPermission(KeyContainerPermissionFlags.NoFlags);
                KeyContainerPermissionAccessEntry accessEntry = new KeyContainerPermissionAccessEntry(this._parameters, KeyContainerPermissionFlags.Sign);
                permission.AccessEntries.Add(accessEntry);
                permission.Demand();
            }
            return Utils._SignValue(this._safeKeyHandle, this._parameters.KeyNumber, 0x2400, calgHash, rgbHash, 0);
        }

        public bool VerifyData(byte[] buffer, object halg, byte[] signature)
        {
            string str = Utils.ObjToOidValue(halg);
            byte[] rgbHash = Utils.ObjToHashAlgorithm(halg).ComputeHash(buffer);
            return this.VerifyHash(rgbHash, str, signature);
        }

        internal bool VerifyHash(byte[] rgbHash, int calgHash, byte[] rgbSignature)
        {
            if (rgbHash == null)
            {
                throw new ArgumentNullException("rgbHash");
            }
            if (rgbSignature == null)
            {
                throw new ArgumentNullException("rgbSignature");
            }
            this.GetKeyPair();
            return Utils._VerifySign(this._safeKeyHandle, 0x2400, calgHash, rgbHash, rgbSignature, 0);
        }

        public bool VerifyHash(byte[] rgbHash, string str, byte[] rgbSignature)
        {
            if (rgbHash == null)
            {
                throw new ArgumentNullException("rgbHash");
            }
            if (rgbSignature == null)
            {
                throw new ArgumentNullException("rgbSignature");
            }
            int calgHash = X509Utils.OidToAlgId(str, OidGroup.HashAlgorithm);
            return this.VerifyHash(rgbHash, calgHash, rgbSignature);
        }

        [ComVisible(false)]
        public System.Security.Cryptography.CspKeyContainerInfo CspKeyContainerInfo
        {
            get
            {
                this.GetKeyPair();
                return new System.Security.Cryptography.CspKeyContainerInfo(this._parameters, this._randomKeyContainer);
            }
        }

        public override string KeyExchangeAlgorithm
        {
            get
            {
                if (this._parameters.KeyNumber == 1)
                {
                    return "RSA-PKCS1-KeyEx";
                }
                return null;
            }
        }

        public override int KeySize
        {
            get
            {
                this.GetKeyPair();
                byte[] buffer = Utils._GetKeyParameter(this._safeKeyHandle, 1);
                this._dwKeySize = ((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 0x10)) | (buffer[3] << 0x18);
                return this._dwKeySize;
            }
        }

        public bool PersistKeyInCsp
        {
            get
            {
                if (this._safeProvHandle == null)
                {
                    lock (this)
                    {
                        if (this._safeProvHandle == null)
                        {
                            this._safeProvHandle = Utils.CreateProvHandle(this._parameters, this._randomKeyContainer);
                        }
                    }
                }
                return Utils._GetPersistKeyInCsp(this._safeProvHandle);
            }
            set
            {
                bool persistKeyInCsp = this.PersistKeyInCsp;
                if (value != persistKeyInCsp)
                {
                    KeyContainerPermission permission = new KeyContainerPermission(KeyContainerPermissionFlags.NoFlags);
                    if (!value)
                    {
                        KeyContainerPermissionAccessEntry accessEntry = new KeyContainerPermissionAccessEntry(this._parameters, KeyContainerPermissionFlags.Delete);
                        permission.AccessEntries.Add(accessEntry);
                    }
                    else
                    {
                        KeyContainerPermissionAccessEntry entry2 = new KeyContainerPermissionAccessEntry(this._parameters, KeyContainerPermissionFlags.Create);
                        permission.AccessEntries.Add(entry2);
                    }
                    permission.Demand();
                    Utils._SetPersistKeyInCsp(this._safeProvHandle, value);
                }
            }
        }

        [ComVisible(false)]
        public bool PublicOnly
        {
            get
            {
                this.GetKeyPair();
                return (Utils._GetKeyParameter(this._safeKeyHandle, 2)[0] == 1);
            }
        }

        public override string SignatureAlgorithm =>
            "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

        public static bool UseMachineKeyStore
        {
            get => 
                (s_UseMachineKeyStore == CspProviderFlags.UseMachineKeyStore);
            set
            {
                s_UseMachineKeyStore = value ? CspProviderFlags.UseMachineKeyStore : CspProviderFlags.NoFlags;
            }
        }
    }
}

