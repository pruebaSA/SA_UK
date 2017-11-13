namespace System.IO.Packaging
{
    using MS.Internal;
    using MS.Internal.IO.Packaging;
    using MS.Internal.IO.Packaging.CompoundFile;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.RightsManagement;
    using System.Windows;

    public class EncryptedPackageEnvelope : IDisposable
    {
        private const string _dataspaceLabelRMEncryptionNoCompression = "RMEncryptionNoCompression";
        private string _dataSpaceName;
        private const FileAccess _defaultFileAccess = FileAccess.ReadWrite;
        private const FileMode _defaultFileModeForCreate = FileMode.Create;
        private const FileMode _defaultFileModeForOpen = FileMode.Open;
        private const FileShare _defaultFileShare = FileShare.None;
        private bool _disposed;
        private const string _encryptionTransformName = "EncryptionTransform";
        private bool _handedOutPackage;
        private bool _handedOutPackageStream;
        private Package _package;
        private StorageBasedPackageProperties _packageProperties;
        private Stream _packageStream;
        private const string _packageStreamName = "EncryptedPackage";
        private System.IO.Packaging.RightsManagementInformation _rmi;
        private StorageRoot _root;
        private const int STG_E_FILEALREADYEXISTS = -2147286960;

        internal EncryptedPackageEnvelope(Stream envelopeStream)
        {
            if (envelopeStream == null)
            {
                throw new ArgumentNullException("envelopeStream");
            }
            this._root = StorageRoot.CreateOnStream(envelopeStream, FileMode.Open);
            this.InitForOpen();
        }

        internal EncryptedPackageEnvelope(Stream envelopeStream, PublishLicense publishLicense, CryptoProvider cryptoProvider)
        {
            if (envelopeStream == null)
            {
                throw new ArgumentNullException("envelopeStream");
            }
            this.ThrowIfRMEncryptionInfoInvalid(publishLicense, cryptoProvider);
            this._root = StorageRoot.CreateOnStream(envelopeStream, FileMode.Create);
            if (this._root.OpenAccess != FileAccess.ReadWrite)
            {
                throw new NotSupportedException(System.Windows.SR.Get("StreamNeedsReadWriteAccess"));
            }
            this.InitializeRMForCreate(publishLicense, cryptoProvider);
            this.EmbedPackage(null);
        }

        internal EncryptedPackageEnvelope(string envelopeFileName, FileAccess access, FileShare sharing)
        {
            if (envelopeFileName == null)
            {
                throw new ArgumentNullException("envelopeFileName");
            }
            this._root = StorageRoot.Open(envelopeFileName, FileMode.Open, access, sharing);
            this.InitForOpen();
        }

        internal EncryptedPackageEnvelope(string envelopeFileName, PublishLicense publishLicense, CryptoProvider cryptoProvider)
        {
            if (envelopeFileName == null)
            {
                throw new ArgumentNullException("envelopeFileName");
            }
            this.ThrowIfRMEncryptionInfoInvalid(publishLicense, cryptoProvider);
            this._root = StorageRoot.Open(envelopeFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            this.InitializeRMForCreate(publishLicense, cryptoProvider);
            this.EmbedPackage(null);
        }

        internal EncryptedPackageEnvelope(Stream envelopeStream, Stream packageStream, PublishLicense publishLicense, CryptoProvider cryptoProvider)
        {
            if (envelopeStream == null)
            {
                throw new ArgumentNullException("envelopeStream");
            }
            if (packageStream == null)
            {
                throw new ArgumentNullException("packageStream");
            }
            this.ThrowIfRMEncryptionInfoInvalid(publishLicense, cryptoProvider);
            this._root = StorageRoot.CreateOnStream(envelopeStream, FileMode.Create);
            if (this._root.OpenAccess != FileAccess.ReadWrite)
            {
                throw new NotSupportedException(System.Windows.SR.Get("StreamNeedsReadWriteAccess"));
            }
            this.InitializeRMForCreate(publishLicense, cryptoProvider);
            this.EmbedPackage(packageStream);
        }

        internal EncryptedPackageEnvelope(string envelopeFileName, Stream packageStream, PublishLicense publishLicense, CryptoProvider cryptoProvider)
        {
            if (envelopeFileName == null)
            {
                throw new ArgumentNullException("envelopeFileName");
            }
            if (packageStream == null)
            {
                throw new ArgumentNullException("packageStream");
            }
            this.ThrowIfRMEncryptionInfoInvalid(publishLicense, cryptoProvider);
            this._root = StorageRoot.Open(envelopeFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            this.InitializeRMForCreate(publishLicense, cryptoProvider);
            this.EmbedPackage(packageStream);
        }

        private void CheckDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(null, System.Windows.SR.Get("EncryptedPackageEnvelopeDisposed"));
            }
        }

        public void Close()
        {
            this.Dispose();
        }

        private static bool ContainsEncryptedPackageStream(StorageRoot root) => 
            new StreamInfo(root, PackageStreamName).InternalExists();

        public static EncryptedPackageEnvelope Create(Stream envelopeStream, PublishLicense publishLicense, CryptoProvider cryptoProvider) => 
            new EncryptedPackageEnvelope(envelopeStream, publishLicense, cryptoProvider);

        public static EncryptedPackageEnvelope Create(string envelopeFileName, PublishLicense publishLicense, CryptoProvider cryptoProvider) => 
            new EncryptedPackageEnvelope(envelopeFileName, publishLicense, cryptoProvider);

        public static EncryptedPackageEnvelope CreateFromPackage(Stream envelopeStream, Stream packageStream, PublishLicense publishLicense, CryptoProvider cryptoProvider) => 
            new EncryptedPackageEnvelope(envelopeStream, packageStream, publishLicense, cryptoProvider);

        public static EncryptedPackageEnvelope CreateFromPackage(string envelopeFileName, Stream packageStream, PublishLicense publishLicense, CryptoProvider cryptoProvider) => 
            new EncryptedPackageEnvelope(envelopeFileName, packageStream, publishLicense, cryptoProvider);

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    try
                    {
                        if (this._package != null)
                        {
                            this._package.Close();
                        }
                    }
                    finally
                    {
                        this._package = null;
                        try
                        {
                            if (this._packageStream != null)
                            {
                                this._packageStream.Close();
                            }
                        }
                        finally
                        {
                            this._packageStream = null;
                            try
                            {
                                if (this._packageProperties != null)
                                {
                                    this._packageProperties.Dispose();
                                }
                            }
                            finally
                            {
                                this._packageProperties = null;
                                try
                                {
                                    if (this._root != null)
                                    {
                                        this._root.Close();
                                    }
                                }
                                finally
                                {
                                    this._root = null;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                this._disposed = true;
            }
        }

        private void EmbedPackage(Stream packageStream)
        {
            this._packageStream = new StreamInfo(this._root, PackageStreamName).Create(FileMode.Create, this._root.OpenAccess, this._dataSpaceName);
            if (packageStream != null)
            {
                PackagingUtilities.CopyStream(packageStream, this._packageStream, 0x7fffffffffffffffL, 0x1000);
                this._package = Package.Open(this._packageStream, FileMode.Open, this.FileOpenAccess);
            }
            else
            {
                this._package = Package.Open(this._packageStream, FileMode.Create, FileAccess.ReadWrite);
                this._package.Flush();
                this._packageStream.Flush();
            }
        }

        private void EnsurePackageStream()
        {
            if (this._packageStream == null)
            {
                StreamInfo info = new StreamInfo(this._root, PackageStreamName);
                if (!info.InternalExists())
                {
                    throw new FileFormatException(System.Windows.SR.Get("PackageNotFound"));
                }
                this._packageStream = info.GetStream(FileMode.Open, this.FileOpenAccess);
            }
        }

        public void Flush()
        {
            this.CheckDisposed();
            if (this._package != null)
            {
                this._package.Flush();
            }
            if (this._packageStream != null)
            {
                this._packageStream.Flush();
            }
            Invariant.Assert(this._root != null, "The envelope cannot be null");
            this._root.Flush();
        }

        public Package GetPackage()
        {
            this.CheckDisposed();
            Invariant.Assert(!this._handedOutPackageStream, "Copy of package stream has been already handed out");
            if (this._package == null)
            {
                this.EnsurePackageStream();
                FileAccess packageAccess = 0;
                if (this._packageStream.CanRead)
                {
                    packageAccess |= FileAccess.Read;
                }
                if (this._packageStream.CanWrite)
                {
                    packageAccess |= FileAccess.Write;
                }
                packageAccess &= this.FileOpenAccess;
                this._package = Package.Open(this._packageStream, FileMode.Open, packageAccess);
            }
            this._handedOutPackage = true;
            return this._package;
        }

        internal Stream GetPackageStream()
        {
            this.CheckDisposed();
            Invariant.Assert(!this._handedOutPackage, "Copy of package has been already handed out");
            this.EnsurePackageStream();
            this._handedOutPackageStream = true;
            if (this._package != null)
            {
                try
                {
                    this._package.Close();
                }
                finally
                {
                    this._package = null;
                }
            }
            return this._packageStream;
        }

        private void InitForOpen()
        {
            StreamInfo streamInfo = new StreamInfo(this._root, PackageStreamName);
            if (!streamInfo.InternalExists())
            {
                throw new FileFormatException(System.Windows.SR.Get("PackageNotFound"));
            }
            List<IDataTransform> transformsForStreamInfo = this._root.GetDataSpaceManager().GetTransformsForStreamInfo(streamInfo);
            RightsManagementEncryptionTransform rmet = null;
            foreach (IDataTransform transform2 in transformsForStreamInfo)
            {
                string transformIdentifier = transform2.TransformIdentifier as string;
                if ((transformIdentifier != null) && (string.CompareOrdinal(transformIdentifier.ToUpperInvariant(), RightsManagementEncryptionTransform.ClassTransformIdentifier.ToUpperInvariant()) == 0))
                {
                    if (rmet != null)
                    {
                        throw new FileFormatException(System.Windows.SR.Get("MultipleRightsManagementEncryptionTransformFound"));
                    }
                    rmet = transform2 as RightsManagementEncryptionTransform;
                }
            }
            if (rmet == null)
            {
                throw new FileFormatException(System.Windows.SR.Get("RightsManagementEncryptionTransformNotFound"));
            }
            this._rmi = new System.IO.Packaging.RightsManagementInformation(rmet);
        }

        private void InitializeRMForCreate(PublishLicense publishLicense, CryptoProvider cryptoProvider)
        {
            DataSpaceManager dataSpaceManager = this._root.GetDataSpaceManager();
            dataSpaceManager.DefineTransform(RightsManagementEncryptionTransform.ClassTransformIdentifier, EncryptionTransformName);
            string[] transformStack = new string[] { EncryptionTransformName };
            this._dataSpaceName = DataspaceLabelRMEncryptionNoCompression;
            dataSpaceManager.DefineDataSpace(transformStack, this._dataSpaceName);
            RightsManagementEncryptionTransform transformFromName = dataSpaceManager.GetTransformFromName(EncryptionTransformName) as RightsManagementEncryptionTransform;
            this._rmi = new System.IO.Packaging.RightsManagementInformation(transformFromName);
            transformFromName.SavePublishLicense(publishLicense);
            transformFromName.CryptoProvider = cryptoProvider;
        }

        public static bool IsEncryptedPackageEnvelope(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            bool flag = false;
            StorageRoot root = null;
            try
            {
                root = StorageRoot.CreateOnStream(stream, FileMode.Open);
                flag = ContainsEncryptedPackageStream(root);
            }
            catch (IOException exception)
            {
                COMException innerException = exception.InnerException as COMException;
                if ((innerException == null) || (innerException.ErrorCode != -2147286960))
                {
                    throw;
                }
                return false;
            }
            finally
            {
                if (root != null)
                {
                    root.Close();
                }
            }
            return flag;
        }

        public static bool IsEncryptedPackageEnvelope(string fileName)
        {
            bool flag = false;
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            StorageRoot root = null;
            try
            {
                root = StorageRoot.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                flag = ContainsEncryptedPackageStream(root);
            }
            catch (IOException exception)
            {
                COMException innerException = exception.InnerException as COMException;
                if ((innerException == null) || (innerException.ErrorCode != -2147286960))
                {
                    throw;
                }
                return false;
            }
            finally
            {
                if (root != null)
                {
                    root.Close();
                }
            }
            return flag;
        }

        public static EncryptedPackageEnvelope Open(Stream envelopeStream) => 
            new EncryptedPackageEnvelope(envelopeStream);

        public static EncryptedPackageEnvelope Open(string envelopeFileName) => 
            Open(envelopeFileName, FileAccess.ReadWrite, FileShare.None);

        public static EncryptedPackageEnvelope Open(string envelopeFileName, FileAccess access) => 
            Open(envelopeFileName, access, FileShare.None);

        public static EncryptedPackageEnvelope Open(string envelopeFileName, FileAccess access, FileShare sharing) => 
            new EncryptedPackageEnvelope(envelopeFileName, access, sharing);

        private void ThrowIfRMEncryptionInfoInvalid(PublishLicense publishLicense, CryptoProvider cryptoProvider)
        {
            if (publishLicense == null)
            {
                throw new ArgumentNullException("publishLicense");
            }
            if (cryptoProvider == null)
            {
                throw new ArgumentNullException("cryptoProvider");
            }
        }

        internal static string DataspaceLabelRMEncryptionNoCompression =>
            "RMEncryptionNoCompression";

        internal static string EncryptionTransformName =>
            "EncryptionTransform";

        public FileAccess FileOpenAccess
        {
            get
            {
                this.CheckDisposed();
                return this._root.OpenAccess;
            }
        }

        public System.IO.Packaging.PackageProperties PackageProperties
        {
            get
            {
                this.CheckDisposed();
                if (this._packageProperties == null)
                {
                    this._packageProperties = new StorageBasedPackageProperties(this._root);
                }
                return this._packageProperties;
            }
        }

        internal static string PackageStreamName =>
            "EncryptedPackage";

        public System.IO.Packaging.RightsManagementInformation RightsManagementInformation
        {
            get
            {
                this.CheckDisposed();
                return this._rmi;
            }
        }

        public System.IO.Packaging.StorageInfo StorageInfo
        {
            get
            {
                this.CheckDisposed();
                return this._root;
            }
        }
    }
}

