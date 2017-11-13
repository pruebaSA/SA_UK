namespace System.Security.Cryptography.X509Certificates
{
    using Microsoft.Win32;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Cryptography;
    using System.Security.Permissions;
    using System.Security.Util;
    using System.Text;

    [Serializable, ComVisible(true)]
    public class X509Certificate : IDeserializationCallback, ISerializable
    {
        private const string m_format = "X509";
        private string m_issuerName;
        private DateTime m_notAfter;
        private DateTime m_notBefore;
        private string m_publicKeyOid;
        private byte[] m_publicKeyParameters;
        private byte[] m_publicKeyValue;
        private byte[] m_rawData;
        private SafeCertContextHandle m_safeCertContext;
        private byte[] m_serialNumber;
        private string m_subjectName;
        private byte[] m_thumbprint;

        public X509Certificate()
        {
            this.m_safeCertContext = SafeCertContextHandle.InvalidHandle;
        }

        public X509Certificate(byte[] data)
        {
            this.m_safeCertContext = SafeCertContextHandle.InvalidHandle;
            if ((data != null) && (data.Length != 0))
            {
                this.LoadCertificateFromBlob(data, null, X509KeyStorageFlags.DefaultKeySet);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public X509Certificate(IntPtr handle)
        {
            this.m_safeCertContext = SafeCertContextHandle.InvalidHandle;
            if (handle == IntPtr.Zero)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_InvalidHandle"), "handle");
            }
            X509Utils._DuplicateCertContext(handle, ref this.m_safeCertContext);
        }

        public X509Certificate(X509Certificate cert)
        {
            this.m_safeCertContext = SafeCertContextHandle.InvalidHandle;
            if (cert == null)
            {
                throw new ArgumentNullException("cert");
            }
            if (cert.m_safeCertContext.pCertContext != IntPtr.Zero)
            {
                X509Utils._DuplicateCertContext(cert.m_safeCertContext.pCertContext, ref this.m_safeCertContext);
            }
            GC.KeepAlive(cert.m_safeCertContext);
        }

        public X509Certificate(string fileName)
        {
            this.m_safeCertContext = SafeCertContextHandle.InvalidHandle;
            this.LoadCertificateFromFile(fileName, null, X509KeyStorageFlags.DefaultKeySet);
        }

        public X509Certificate(SerializationInfo info, StreamingContext context)
        {
            this.m_safeCertContext = SafeCertContextHandle.InvalidHandle;
            byte[] rawData = (byte[]) info.GetValue("RawData", typeof(byte[]));
            if (rawData != null)
            {
                this.LoadCertificateFromBlob(rawData, null, X509KeyStorageFlags.DefaultKeySet);
            }
        }

        public X509Certificate(byte[] rawData, SecureString password)
        {
            this.m_safeCertContext = SafeCertContextHandle.InvalidHandle;
            this.LoadCertificateFromBlob(rawData, password, X509KeyStorageFlags.DefaultKeySet);
        }

        public X509Certificate(string fileName, SecureString password)
        {
            this.m_safeCertContext = SafeCertContextHandle.InvalidHandle;
            this.LoadCertificateFromFile(fileName, password, X509KeyStorageFlags.DefaultKeySet);
        }

        public X509Certificate(byte[] rawData, string password)
        {
            this.m_safeCertContext = SafeCertContextHandle.InvalidHandle;
            this.LoadCertificateFromBlob(rawData, password, X509KeyStorageFlags.DefaultKeySet);
        }

        public X509Certificate(string fileName, string password)
        {
            this.m_safeCertContext = SafeCertContextHandle.InvalidHandle;
            this.LoadCertificateFromFile(fileName, password, X509KeyStorageFlags.DefaultKeySet);
        }

        public X509Certificate(byte[] rawData, SecureString password, X509KeyStorageFlags keyStorageFlags)
        {
            this.m_safeCertContext = SafeCertContextHandle.InvalidHandle;
            this.LoadCertificateFromBlob(rawData, password, keyStorageFlags);
        }

        public X509Certificate(string fileName, SecureString password, X509KeyStorageFlags keyStorageFlags)
        {
            this.m_safeCertContext = SafeCertContextHandle.InvalidHandle;
            this.LoadCertificateFromFile(fileName, password, keyStorageFlags);
        }

        public X509Certificate(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags)
        {
            this.m_safeCertContext = SafeCertContextHandle.InvalidHandle;
            this.LoadCertificateFromBlob(rawData, password, keyStorageFlags);
        }

        public X509Certificate(string fileName, string password, X509KeyStorageFlags keyStorageFlags)
        {
            this.m_safeCertContext = SafeCertContextHandle.InvalidHandle;
            this.LoadCertificateFromFile(fileName, password, keyStorageFlags);
        }

        public static X509Certificate CreateFromCertFile(string filename) => 
            new X509Certificate(filename);

        public static X509Certificate CreateFromSignedFile(string filename) => 
            new X509Certificate(filename);

        [ComVisible(false)]
        public override bool Equals(object obj)
        {
            if (!(obj is X509Certificate))
            {
                return false;
            }
            X509Certificate other = (X509Certificate) obj;
            return this.Equals(other);
        }

        public virtual bool Equals(X509Certificate other)
        {
            if (other == null)
            {
                return false;
            }
            if (this.m_safeCertContext.IsInvalid)
            {
                return other.m_safeCertContext.IsInvalid;
            }
            if (!this.Issuer.Equals(other.Issuer))
            {
                return false;
            }
            if (!this.SerialNumber.Equals(other.SerialNumber))
            {
                return false;
            }
            return true;
        }

        [ComVisible(false)]
        public virtual byte[] Export(X509ContentType contentType) => 
            this.ExportHelper(contentType, null);

        public virtual byte[] Export(X509ContentType contentType, SecureString password) => 
            this.ExportHelper(contentType, password);

        [ComVisible(false)]
        public virtual byte[] Export(X509ContentType contentType, string password) => 
            this.ExportHelper(contentType, password);

        private byte[] ExportHelper(X509ContentType contentType, object password)
        {
            switch (contentType)
            {
                case X509ContentType.Cert:
                case X509ContentType.SerializedCert:
                    break;

                case X509ContentType.Pfx:
                    new KeyContainerPermission(KeyContainerPermissionFlags.Export | KeyContainerPermissionFlags.Open).Demand();
                    break;

                default:
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_X509_InvalidContentType"));
            }
            IntPtr zero = IntPtr.Zero;
            byte[] buffer = null;
            SafeCertStoreHandle safeCertStoreHandle = X509Utils.ExportCertToMemoryStore(this);
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                zero = X509Utils.PasswordToCoTaskMemUni(password);
                buffer = X509Utils._ExportCertificatesToBlob(safeCertStoreHandle, contentType, zero);
            }
            finally
            {
                if (zero != IntPtr.Zero)
                {
                    Marshal.ZeroFreeCoTaskMemUnicode(zero);
                }
                safeCertStoreHandle.Dispose();
            }
            if (buffer == null)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_X509_ExportFailed"));
            }
            return buffer;
        }

        public virtual byte[] GetCertHash()
        {
            this.SetThumbprint();
            return (byte[]) this.m_thumbprint.Clone();
        }

        public virtual string GetCertHashString()
        {
            this.SetThumbprint();
            return Hex.EncodeHexString(this.m_thumbprint);
        }

        public virtual string GetEffectiveDateString() => 
            this.NotBefore.ToString();

        public virtual string GetExpirationDateString() => 
            this.NotAfter.ToString();

        public virtual string GetFormat() => 
            "X509";

        public override int GetHashCode()
        {
            if (this.m_safeCertContext.IsInvalid)
            {
                return 0;
            }
            this.SetThumbprint();
            int num = 0;
            for (int i = 0; (i < this.m_thumbprint.Length) && (i < 4); i++)
            {
                num = (num << 8) | this.m_thumbprint[i];
            }
            return num;
        }

        [Obsolete("This method has been deprecated.  Please use the Issuer property instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public virtual string GetIssuerName()
        {
            if (this.m_safeCertContext.IsInvalid)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidHandle"), "m_safeCertContext");
            }
            return X509Utils._GetIssuerName(this.m_safeCertContext, true);
        }

        public virtual string GetKeyAlgorithm()
        {
            if (this.m_safeCertContext.IsInvalid)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidHandle"), "m_safeCertContext");
            }
            if (this.m_publicKeyOid == null)
            {
                this.m_publicKeyOid = X509Utils._GetPublicKeyOid(this.m_safeCertContext);
            }
            return this.m_publicKeyOid;
        }

        public virtual byte[] GetKeyAlgorithmParameters()
        {
            if (this.m_safeCertContext.IsInvalid)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidHandle"), "m_safeCertContext");
            }
            if (this.m_publicKeyParameters == null)
            {
                this.m_publicKeyParameters = X509Utils._GetPublicKeyParameters(this.m_safeCertContext);
            }
            return (byte[]) this.m_publicKeyParameters.Clone();
        }

        public virtual string GetKeyAlgorithmParametersString()
        {
            if (this.m_safeCertContext.IsInvalid)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidHandle"), "m_safeCertContext");
            }
            return Hex.EncodeHexString(this.GetKeyAlgorithmParameters());
        }

        [Obsolete("This method has been deprecated.  Please use the Subject property instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public virtual string GetName()
        {
            if (this.m_safeCertContext.IsInvalid)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidHandle"), "m_safeCertContext");
            }
            return X509Utils._GetSubjectInfo(this.m_safeCertContext, 2, true);
        }

        public virtual byte[] GetPublicKey()
        {
            if (this.m_safeCertContext.IsInvalid)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidHandle"), "m_safeCertContext");
            }
            if (this.m_publicKeyValue == null)
            {
                this.m_publicKeyValue = X509Utils._GetPublicKeyValue(this.m_safeCertContext);
            }
            return (byte[]) this.m_publicKeyValue.Clone();
        }

        public virtual string GetPublicKeyString() => 
            Hex.EncodeHexString(this.GetPublicKey());

        public virtual byte[] GetRawCertData() => 
            this.RawData;

        public virtual string GetRawCertDataString() => 
            Hex.EncodeHexString(this.GetRawCertData());

        public virtual byte[] GetSerialNumber()
        {
            if (this.m_safeCertContext.IsInvalid)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidHandle"), "m_safeCertContext");
            }
            if (this.m_serialNumber == null)
            {
                this.m_serialNumber = X509Utils._GetSerialNumber(this.m_safeCertContext);
            }
            return (byte[]) this.m_serialNumber.Clone();
        }

        public virtual string GetSerialNumberString() => 
            this.SerialNumber;

        [ComVisible(false), PermissionSet(SecurityAction.InheritanceDemand, Unrestricted=true), PermissionSet(SecurityAction.LinkDemand, Unrestricted=true)]
        public virtual void Import(byte[] rawData)
        {
            this.Reset();
            this.LoadCertificateFromBlob(rawData, null, X509KeyStorageFlags.DefaultKeySet);
        }

        [ComVisible(false), PermissionSet(SecurityAction.LinkDemand, Unrestricted=true), PermissionSet(SecurityAction.InheritanceDemand, Unrestricted=true)]
        public virtual void Import(string fileName)
        {
            this.Reset();
            this.LoadCertificateFromFile(fileName, null, X509KeyStorageFlags.DefaultKeySet);
        }

        [PermissionSet(SecurityAction.InheritanceDemand, Unrestricted=true), PermissionSet(SecurityAction.LinkDemand, Unrestricted=true)]
        public virtual void Import(byte[] rawData, SecureString password, X509KeyStorageFlags keyStorageFlags)
        {
            this.Reset();
            this.LoadCertificateFromBlob(rawData, password, keyStorageFlags);
        }

        [PermissionSet(SecurityAction.InheritanceDemand, Unrestricted=true), PermissionSet(SecurityAction.LinkDemand, Unrestricted=true)]
        public virtual void Import(string fileName, SecureString password, X509KeyStorageFlags keyStorageFlags)
        {
            this.Reset();
            this.LoadCertificateFromFile(fileName, password, keyStorageFlags);
        }

        [ComVisible(false), PermissionSet(SecurityAction.InheritanceDemand, Unrestricted=true), PermissionSet(SecurityAction.LinkDemand, Unrestricted=true)]
        public virtual void Import(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags)
        {
            this.Reset();
            this.LoadCertificateFromBlob(rawData, password, keyStorageFlags);
        }

        [ComVisible(false), PermissionSet(SecurityAction.LinkDemand, Unrestricted=true), PermissionSet(SecurityAction.InheritanceDemand, Unrestricted=true)]
        public virtual void Import(string fileName, string password, X509KeyStorageFlags keyStorageFlags)
        {
            this.Reset();
            this.LoadCertificateFromFile(fileName, password, keyStorageFlags);
        }

        private void LoadCertificateFromBlob(byte[] rawData, object password, X509KeyStorageFlags keyStorageFlags)
        {
            if ((rawData == null) || (rawData.Length == 0))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_EmptyOrNullArray"), "rawData");
            }
            if ((X509Utils.MapContentType(X509Utils._QueryCertBlobType(rawData)) == X509ContentType.Pfx) && ((keyStorageFlags & X509KeyStorageFlags.PersistKeySet) == X509KeyStorageFlags.PersistKeySet))
            {
                new KeyContainerPermission(KeyContainerPermissionFlags.Create).Demand();
            }
            uint dwFlags = X509Utils.MapKeyStorageFlags(keyStorageFlags);
            IntPtr zero = IntPtr.Zero;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                zero = X509Utils.PasswordToCoTaskMemUni(password);
                X509Utils._LoadCertFromBlob(rawData, zero, dwFlags, (keyStorageFlags & X509KeyStorageFlags.PersistKeySet) != X509KeyStorageFlags.DefaultKeySet, ref this.m_safeCertContext);
            }
            finally
            {
                if (zero != IntPtr.Zero)
                {
                    Marshal.ZeroFreeCoTaskMemUnicode(zero);
                }
            }
        }

        private void LoadCertificateFromFile(string fileName, object password, X509KeyStorageFlags keyStorageFlags)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            string fullPathInternal = Path.GetFullPathInternal(fileName);
            new FileIOPermission(FileIOPermissionAccess.Read, fullPathInternal).Demand();
            if ((X509Utils.MapContentType(X509Utils._QueryCertFileType(fileName)) == X509ContentType.Pfx) && ((keyStorageFlags & X509KeyStorageFlags.PersistKeySet) == X509KeyStorageFlags.PersistKeySet))
            {
                new KeyContainerPermission(KeyContainerPermissionFlags.Create).Demand();
            }
            uint dwFlags = X509Utils.MapKeyStorageFlags(keyStorageFlags);
            IntPtr zero = IntPtr.Zero;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                zero = X509Utils.PasswordToCoTaskMemUni(password);
                X509Utils._LoadCertFromFile(fileName, zero, dwFlags, (keyStorageFlags & X509KeyStorageFlags.PersistKeySet) != X509KeyStorageFlags.DefaultKeySet, ref this.m_safeCertContext);
            }
            finally
            {
                if (zero != IntPtr.Zero)
                {
                    Marshal.ZeroFreeCoTaskMemUnicode(zero);
                }
            }
        }

        [ComVisible(false), PermissionSet(SecurityAction.InheritanceDemand, Unrestricted=true), PermissionSet(SecurityAction.LinkDemand, Unrestricted=true)]
        public virtual void Reset()
        {
            this.m_subjectName = null;
            this.m_issuerName = null;
            this.m_serialNumber = null;
            this.m_publicKeyParameters = null;
            this.m_publicKeyValue = null;
            this.m_publicKeyOid = null;
            this.m_rawData = null;
            this.m_thumbprint = null;
            this.m_notBefore = DateTime.MinValue;
            this.m_notAfter = DateTime.MinValue;
            if (!this.m_safeCertContext.IsInvalid)
            {
                this.m_safeCertContext.Dispose();
                this.m_safeCertContext = SafeCertContextHandle.InvalidHandle;
            }
        }

        private void SetThumbprint()
        {
            if (this.m_safeCertContext.IsInvalid)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidHandle"), "m_safeCertContext");
            }
            if (this.m_thumbprint == null)
            {
                this.m_thumbprint = X509Utils._GetThumbprint(this.m_safeCertContext);
            }
        }

        void IDeserializationCallback.OnDeserialization(object sender)
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (this.m_safeCertContext.IsInvalid)
            {
                info.AddValue("RawData", null);
            }
            else
            {
                info.AddValue("RawData", this.RawData);
            }
        }

        public override string ToString() => 
            this.ToString(false);

        public virtual string ToString(bool fVerbose)
        {
            if (!fVerbose || this.m_safeCertContext.IsInvalid)
            {
                return base.GetType().FullName;
            }
            StringBuilder builder = new StringBuilder();
            builder.Append("[Subject]" + Environment.NewLine + "  ");
            builder.Append(this.Subject);
            builder.Append(Environment.NewLine + Environment.NewLine + "[Issuer]" + Environment.NewLine + "  ");
            builder.Append(this.Issuer);
            builder.Append(Environment.NewLine + Environment.NewLine + "[Serial Number]" + Environment.NewLine + "  ");
            builder.Append(this.SerialNumber);
            builder.Append(Environment.NewLine + Environment.NewLine + "[Not Before]" + Environment.NewLine + "  ");
            builder.Append(this.NotBefore);
            builder.Append(Environment.NewLine + Environment.NewLine + "[Not After]" + Environment.NewLine + "  ");
            builder.Append(this.NotAfter);
            builder.Append(Environment.NewLine + Environment.NewLine + "[Thumbprint]" + Environment.NewLine + "  ");
            builder.Append(this.GetCertHashString());
            builder.Append(Environment.NewLine);
            return builder.ToString();
        }

        internal SafeCertContextHandle CertContext =>
            this.m_safeCertContext;

        [ComVisible(false)]
        public IntPtr Handle =>
            this.m_safeCertContext.pCertContext;

        public string Issuer
        {
            get
            {
                if (this.m_safeCertContext.IsInvalid)
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidHandle"), "m_safeCertContext");
                }
                if (this.m_issuerName == null)
                {
                    this.m_issuerName = X509Utils._GetIssuerName(this.m_safeCertContext, false);
                }
                return this.m_issuerName;
            }
        }

        private DateTime NotAfter
        {
            get
            {
                if (this.m_safeCertContext.IsInvalid)
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidHandle"), "m_safeCertContext");
                }
                if (this.m_notAfter == DateTime.MinValue)
                {
                    Win32Native.FILE_TIME fileTime = new Win32Native.FILE_TIME();
                    X509Utils._GetDateNotAfter(this.m_safeCertContext, ref fileTime);
                    this.m_notAfter = DateTime.FromFileTime(fileTime.ToTicks());
                }
                return this.m_notAfter;
            }
        }

        private DateTime NotBefore
        {
            get
            {
                if (this.m_safeCertContext.IsInvalid)
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidHandle"), "m_safeCertContext");
                }
                if (this.m_notBefore == DateTime.MinValue)
                {
                    Win32Native.FILE_TIME fileTime = new Win32Native.FILE_TIME();
                    X509Utils._GetDateNotBefore(this.m_safeCertContext, ref fileTime);
                    this.m_notBefore = DateTime.FromFileTime(fileTime.ToTicks());
                }
                return this.m_notBefore;
            }
        }

        private byte[] RawData
        {
            get
            {
                if (this.m_safeCertContext.IsInvalid)
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidHandle"), "m_safeCertContext");
                }
                if (this.m_rawData == null)
                {
                    this.m_rawData = X509Utils._GetCertRawData(this.m_safeCertContext);
                }
                return (byte[]) this.m_rawData.Clone();
            }
        }

        private string SerialNumber
        {
            get
            {
                if (this.m_safeCertContext.IsInvalid)
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidHandle"), "m_safeCertContext");
                }
                if (this.m_serialNumber == null)
                {
                    this.m_serialNumber = X509Utils._GetSerialNumber(this.m_safeCertContext);
                }
                return Hex.EncodeHexStringFromInt(this.m_serialNumber);
            }
        }

        public string Subject
        {
            get
            {
                if (this.m_safeCertContext.IsInvalid)
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidHandle"), "m_safeCertContext");
                }
                if (this.m_subjectName == null)
                {
                    this.m_subjectName = X509Utils._GetSubjectInfo(this.m_safeCertContext, 2, false);
                }
                return this.m_subjectName;
            }
        }
    }
}

