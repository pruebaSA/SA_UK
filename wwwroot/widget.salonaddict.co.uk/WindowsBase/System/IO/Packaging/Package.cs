namespace System.IO.Packaging
{
    using MS.Internal;
    using MS.Internal.IO.Packaging;
    using MS.Internal.WindowsBase;
    using MS.Utility;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public abstract class Package : IDisposable
    {
        private static readonly FileAccess _defaultFileAccess = FileAccess.ReadWrite;
        private static readonly FileMode _defaultFileMode = FileMode.OpenOrCreate;
        private static readonly FileShare _defaultFileShare = FileShare.None;
        private static readonly FileAccess _defaultStreamAccess = FileAccess.Read;
        private static readonly FileMode _defaultStreamMode = FileMode.Open;
        private bool _disposed;
        private bool _inStreamingCreation;
        private FileAccess _openFileAccess;
        private PartBasedPackageProperties _packageProperties;
        private PackagePartCollection _partCollection;
        private SortedList<PackUriHelper.ValidatedPartUri, PackagePart> _partList;
        private InternalRelationshipCollection _relationships;

        protected Package(FileAccess openFileAccess) : this(openFileAccess, false)
        {
        }

        protected Package(FileAccess openFileAccess, bool streaming)
        {
            ThrowIfFileAccessInvalid(openFileAccess);
            this._openFileAccess = openFileAccess;
            this._partList = new SortedList<PackUriHelper.ValidatedPartUri, PackagePart>();
            this._partCollection = null;
            this._disposed = false;
            this._inStreamingCreation = (openFileAccess == FileAccess.Write) && streaming;
        }

        private void AddIfNoPrefixCollisionDetected(PackUriHelper.ValidatedPartUri partUri, PackagePart part)
        {
            this._partList.Add(partUri, part);
            int num = this._partList.IndexOfKey(partUri);
            Invariant.Assert(num >= 0, "Given uri must be present in the dictionary");
            string normalizedPartUriString = partUri.NormalizedPartUriString;
            string str2 = null;
            string str3 = null;
            if (num > 0)
            {
                str2 = this._partList.Keys[num - 1].NormalizedPartUriString;
            }
            if (num < (this._partList.Count - 1))
            {
                str3 = this._partList.Keys[num + 1].NormalizedPartUriString;
            }
            if ((((str2 != null) && normalizedPartUriString.StartsWith(str2, StringComparison.Ordinal)) && ((normalizedPartUriString.Length > str2.Length) && (normalizedPartUriString[str2.Length] == PackUriHelper.ForwardSlashChar))) || (((str3 != null) && str3.StartsWith(normalizedPartUriString, StringComparison.Ordinal)) && ((str3.Length > normalizedPartUriString.Length) && (str3[normalizedPartUriString.Length] == PackUriHelper.ForwardSlashChar))))
            {
                this._partList.Remove(partUri);
                throw new InvalidOperationException(System.Windows.SR.Get("PartNamePrefixExists"));
            }
        }

        private void ClearRelationships()
        {
            if (this._relationships != null)
            {
                this._relationships.Clear();
            }
        }

        public void Close()
        {
            ((IDisposable) this).Dispose();
        }

        internal void ClosePackageRelationships()
        {
            this.ThrowIfNotInStreamingCreation("ClosePackageRelationships");
            if (this._relationships != null)
            {
                this._relationships.CloseInStreamingCreationMode();
            }
        }

        public PackagePart CreatePart(Uri partUri, string contentType) => 
            this.CreatePart(partUri, contentType, CompressionOption.NotCompressed);

        public PackagePart CreatePart(Uri partUri, string contentType, CompressionOption compressionOption)
        {
            this.ThrowIfObjectDisposed();
            this.ThrowIfReadOnly();
            if (partUri == null)
            {
                throw new ArgumentNullException("partUri");
            }
            if (contentType == null)
            {
                throw new ArgumentNullException("contentType");
            }
            ThrowIfCompressionOptionInvalid(compressionOption);
            PackUriHelper.ValidatedPartUri key = PackUriHelper.ValidatePartUri(partUri);
            if (this._partList.ContainsKey(key))
            {
                throw new InvalidOperationException(System.Windows.SR.Get("PartAlreadyExists"));
            }
            this.AddIfNoPrefixCollisionDetected(key, null);
            PackagePart part = this.CreatePartCore(key, contentType, compressionOption);
            this._partList[key] = part;
            return part;
        }

        protected abstract PackagePart CreatePartCore(Uri partUri, string contentType, CompressionOption compressionOption);
        public PackageRelationship CreateRelationship(Uri targetUri, TargetMode targetMode, string relationshipType) => 
            this.CreateRelationship(targetUri, targetMode, relationshipType, null);

        public PackageRelationship CreateRelationship(Uri targetUri, TargetMode targetMode, string relationshipType, string id)
        {
            this.ThrowIfObjectDisposed();
            this.ThrowIfReadOnly();
            this.EnsureRelationships();
            return this._relationships.Add(targetUri, targetMode, relationshipType, id);
        }

        public void DeletePart(Uri partUri)
        {
            this.ThrowIfObjectDisposed();
            this.ThrowIfReadOnly();
            this.ThrowIfInStreamingCreation("DeletePart");
            if (partUri == null)
            {
                throw new ArgumentNullException("partUri");
            }
            PackUriHelper.ValidatedPartUri key = PackUriHelper.ValidatePartUri(partUri);
            if (this._partList.ContainsKey(key))
            {
                key = (PackUriHelper.ValidatedPartUri) this._partList[key].Uri;
                this._partList[key].IsDeleted = true;
                this._partList[key].Close();
                this.DeletePartCore(key);
                this._partList.Remove(key);
            }
            else
            {
                this.DeletePartCore(key);
            }
            if (PackUriHelper.IsRelationshipPartUri(key))
            {
                Uri sourcePartUriFromRelationshipPartUri = PackUriHelper.GetSourcePartUriFromRelationshipPartUri(key);
                if (Uri.Compare(sourcePartUriFromRelationshipPartUri, PackUriHelper.PackageRootUri, UriComponents.SerializationInfoString, UriFormat.UriEscaped, StringComparison.Ordinal) == 0)
                {
                    this.ClearRelationships();
                }
                else if (this.PartExists(sourcePartUriFromRelationshipPartUri))
                {
                    this.GetPart(sourcePartUriFromRelationshipPartUri).ClearRelationships();
                }
            }
            else
            {
                this.DeletePart(PackUriHelper.GetRelationshipPartUri(key));
            }
        }

        protected abstract void DeletePartCore(Uri partUri);
        public void DeleteRelationship(string id)
        {
            this.ThrowIfObjectDisposed();
            this.ThrowIfReadOnly();
            this.ThrowIfInStreamingCreation("DeleteRelationship");
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            InternalRelationshipCollection.ThrowIfInvalidXsdId(id);
            this.EnsureRelationships();
            this._relationships.Delete(id);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed && disposing)
            {
                this._partList.Clear();
                if (this._packageProperties != null)
                {
                    this._packageProperties.Dispose();
                    this._packageProperties = null;
                }
                this._partList = null;
                this._partCollection = null;
                this._relationships = null;
                this._disposed = true;
            }
        }

        private bool DoClose(PackagePart p)
        {
            if (!p.IsClosed)
            {
                if (PackUriHelper.IsRelationshipPartUri(p.Uri) && (PackUriHelper.ComparePartUri(p.Uri, PackageRelationship.ContainerRelationshipPartName) != 0))
                {
                    PackagePart part;
                    PackUriHelper.ValidatedPartUri sourcePartUriFromRelationshipPartUri = (PackUriHelper.ValidatedPartUri) PackUriHelper.GetSourcePartUriFromRelationshipPartUri(p.Uri);
                    if (this._partList.TryGetValue(sourcePartUriFromRelationshipPartUri, out part))
                    {
                        part.Close();
                    }
                }
                p.Close();
            }
            return true;
        }

        private bool DoCloseRelationshipsXml(PackagePart p)
        {
            if (!p.IsRelationshipPart)
            {
                p.CloseRelationships();
            }
            return true;
        }

        private bool DoFlush(PackagePart p)
        {
            p.Flush();
            return true;
        }

        private void DoOperationOnEachPart(PartOperation operation)
        {
            if (this._partList.Count > 0)
            {
                int num = 0;
                PackUriHelper.ValidatedPartUri[] uriArray = new PackUriHelper.ValidatedPartUri[this._partList.Keys.Count];
                foreach (PackUriHelper.ValidatedPartUri uri in this._partList.Keys)
                {
                    uriArray[num++] = uri;
                }
                for (int i = 0; i < this._partList.Keys.Count; i++)
                {
                    PackagePart part;
                    if (this._partList.TryGetValue(uriArray[i], out part) && !operation(part))
                    {
                        return;
                    }
                }
            }
        }

        private bool DoWriteRelationshipsXml(PackagePart p)
        {
            if (!p.IsRelationshipPart)
            {
                p.FlushRelationships();
            }
            return true;
        }

        private void EnsureRelationships()
        {
            if (this._relationships == null)
            {
                this._relationships = new InternalRelationshipCollection(this);
            }
        }

        public void Flush()
        {
            this.ThrowIfObjectDisposed();
            this.ThrowIfReadOnly();
            if (this._packageProperties != null)
            {
                this._packageProperties.Flush();
            }
            if (this.InStreamingCreation)
            {
                this.FlushPackageRelationships();
            }
            else
            {
                this.FlushRelationships();
            }
            this.DoOperationOnEachPart(new PartOperation(this.DoWriteRelationshipsXml));
            this.DoOperationOnEachPart(new PartOperation(this.DoFlush));
            this.FlushCore();
        }

        protected abstract void FlushCore();
        internal void FlushPackageRelationships()
        {
            this.ThrowIfNotInStreamingCreation("FlushPackageRelationships");
            if (this._relationships != null)
            {
                this._relationships.Flush();
            }
        }

        private void FlushRelationships()
        {
            if ((this._relationships != null) && (this._openFileAccess != FileAccess.Read))
            {
                this._relationships.Flush();
            }
        }

        public PackagePart GetPart(Uri partUri)
        {
            PackagePart partHelper = this.GetPartHelper(partUri);
            if (partHelper == null)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("PartDoesNotExist"));
            }
            return partHelper;
        }

        protected abstract PackagePart GetPartCore(Uri partUri);
        private PackagePart GetPartHelper(Uri partUri)
        {
            this.ThrowIfObjectDisposed();
            this.ThrowIfWriteOnly();
            if (partUri == null)
            {
                throw new ArgumentNullException("partUri");
            }
            PackUriHelper.ValidatedPartUri key = PackUriHelper.ValidatePartUri(partUri);
            if (this._partList.ContainsKey(key))
            {
                return this._partList[key];
            }
            PackagePart partCore = this.GetPartCore(key);
            if (partCore != null)
            {
                this.AddIfNoPrefixCollisionDetected(key, partCore);
            }
            return partCore;
        }

        public PackagePartCollection GetParts()
        {
            this.ThrowIfObjectDisposed();
            this.ThrowIfWriteOnly();
            if (this._partCollection == null)
            {
                PackagePart[] partsCore = this.GetPartsCore();
                Dictionary<PackUriHelper.ValidatedPartUri, PackagePart> dictionary = new Dictionary<PackUriHelper.ValidatedPartUri, PackagePart>(partsCore.Length);
                for (int i = 0; i < partsCore.Length; i++)
                {
                    PackUriHelper.ValidatedPartUri key = (PackUriHelper.ValidatedPartUri) partsCore[i].Uri;
                    if (dictionary.ContainsKey(key))
                    {
                        throw new FileFormatException(System.Windows.SR.Get("BadPackageFormat"));
                    }
                    dictionary.Add(key, partsCore[i]);
                    if (!this._partList.ContainsKey(key))
                    {
                        this.AddIfNoPrefixCollisionDetected(key, partsCore[i]);
                    }
                }
                this._partCollection = new PackagePartCollection(this._partList);
            }
            return this._partCollection;
        }

        protected abstract PackagePart[] GetPartsCore();
        public PackageRelationship GetRelationship(string id)
        {
            PackageRelationship relationshipHelper = this.GetRelationshipHelper(id);
            if (relationshipHelper == null)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("PackageRelationshipDoesNotExist"));
            }
            return relationshipHelper;
        }

        private PackageRelationship GetRelationshipHelper(string id)
        {
            this.ThrowIfObjectDisposed();
            this.ThrowIfWriteOnly();
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            InternalRelationshipCollection.ThrowIfInvalidXsdId(id);
            this.EnsureRelationships();
            return this._relationships.GetRelationship(id);
        }

        public PackageRelationshipCollection GetRelationships() => 
            this.GetRelationshipsHelper(null);

        public PackageRelationshipCollection GetRelationshipsByType(string relationshipType)
        {
            this.ThrowIfObjectDisposed();
            this.ThrowIfWriteOnly();
            if (relationshipType == null)
            {
                throw new ArgumentNullException("relationshipType");
            }
            InternalRelationshipCollection.ThrowIfInvalidRelationshipType(relationshipType);
            return this.GetRelationshipsHelper(relationshipType);
        }

        private PackageRelationshipCollection GetRelationshipsHelper(string filterString)
        {
            this.ThrowIfObjectDisposed();
            this.ThrowIfWriteOnly();
            this.EnsureRelationships();
            return new PackageRelationshipCollection(this._relationships, filterString);
        }

        public static Package Open(Stream stream) => 
            Open(stream, _defaultStreamMode, _defaultStreamAccess);

        public static Package Open(string path) => 
            Open(path, _defaultFileMode, _defaultFileAccess, _defaultFileShare);

        public static Package Open(Stream stream, FileMode packageMode) => 
            Open(stream, packageMode, _defaultFileAccess);

        public static Package Open(string path, FileMode packageMode) => 
            Open(path, packageMode, _defaultFileAccess, _defaultFileShare);

        public static Package Open(Stream stream, FileMode packageMode, FileAccess packageAccess) => 
            Open(stream, packageMode, packageAccess, false);

        public static Package Open(string path, FileMode packageMode, FileAccess packageAccess) => 
            Open(path, packageMode, packageAccess, _defaultFileShare);

        [FriendAccessAllowed]
        internal static Package Open(Stream stream, FileMode packageMode, FileAccess packageAccess, bool streaming)
        {
            EventTrace.NormalTraceEvent(EventTraceGuidId.DRXOPENPACKAGEGUID, 1);
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            ValidateStreamingAccess(packageMode, packageAccess, null, streaming);
            Package package = new ZipPackage(ValidateModeAndAccess(stream, packageMode, packageAccess), packageMode, packageAccess, streaming);
            if (!package._inStreamingCreation && ((package.FileOpenAccess == FileAccess.ReadWrite) || (package.FileOpenAccess == FileAccess.Read)))
            {
                package.GetParts();
            }
            EventTrace.NormalTraceEvent(EventTraceGuidId.DRXOPENPACKAGEGUID, 2);
            return package;
        }

        public static Package Open(string path, FileMode packageMode, FileAccess packageAccess, FileShare packageShare) => 
            Open(path, packageMode, packageAccess, packageShare, false);

        internal static Package Open(string path, FileMode packageMode, FileAccess packageAccess, FileShare packageShare, bool streaming)
        {
            EventTrace.NormalTraceEvent(EventTraceGuidId.DRXOPENPACKAGEGUID, 1);
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            ThrowIfFileModeInvalid(packageMode);
            ThrowIfFileAccessInvalid(packageAccess);
            ValidateStreamingAccess(packageMode, packageAccess, new FileShare?(packageShare), streaming);
            FileInfo info = new FileInfo(path);
            Package package = new ZipPackage(info.FullName, packageMode, packageAccess, packageShare, streaming);
            if (!package._inStreamingCreation && ((package.FileOpenAccess == FileAccess.ReadWrite) || (package.FileOpenAccess == FileAccess.Read)))
            {
                package.GetParts();
            }
            EventTrace.NormalTraceEvent(EventTraceGuidId.DRXOPENPACKAGEGUID, 2);
            return package;
        }

        public virtual bool PartExists(Uri partUri) => 
            (this.GetPartHelper(partUri) != null);

        public bool RelationshipExists(string id) => 
            (this.GetRelationshipHelper(id) != null);

        void IDisposable.Dispose()
        {
            if (!this._disposed)
            {
                try
                {
                    if (this._packageProperties != null)
                    {
                        this._packageProperties.Close();
                    }
                    if (this.InStreamingCreation)
                    {
                        this.ClosePackageRelationships();
                    }
                    else
                    {
                        this.FlushRelationships();
                    }
                    this.DoOperationOnEachPart(new PartOperation(this.DoCloseRelationshipsXml));
                    this.DoOperationOnEachPart(new PartOperation(this.DoClose));
                    this.Dispose(true);
                }
                finally
                {
                    this._disposed = true;
                }
                GC.SuppressFinalize(this);
            }
        }

        internal static void ThrowIfCompressionOptionInvalid(CompressionOption compressionOption)
        {
            if ((compressionOption < CompressionOption.NotCompressed) || (compressionOption > CompressionOption.SuperFast))
            {
                throw new ArgumentOutOfRangeException("compressionOption");
            }
        }

        internal static void ThrowIfFileAccessInvalid(FileAccess access)
        {
            if ((access < FileAccess.Read) || (access > FileAccess.ReadWrite))
            {
                throw new ArgumentOutOfRangeException("access");
            }
        }

        internal static void ThrowIfFileModeInvalid(FileMode mode)
        {
            if ((mode < FileMode.CreateNew) || (mode > FileMode.Append))
            {
                throw new ArgumentOutOfRangeException("mode");
            }
        }

        internal void ThrowIfInStreamingCreation(string methodName)
        {
            if (this._inStreamingCreation)
            {
                throw new IOException(System.Windows.SR.Get("OperationIsNotSupportedInStreamingProduction", new object[] { methodName }));
            }
        }

        internal void ThrowIfNotInStreamingCreation(string methodName)
        {
            if (!this.InStreamingCreation)
            {
                throw new IOException(System.Windows.SR.Get("MethodAvailableOnlyInStreamingCreation", new object[] { methodName }));
            }
        }

        private void ThrowIfObjectDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(null, System.Windows.SR.Get("ObjectDisposed"));
            }
        }

        internal void ThrowIfReadOnly()
        {
            if (this._openFileAccess == FileAccess.Read)
            {
                throw new IOException(System.Windows.SR.Get("CannotModifyReadOnlyContainer"));
            }
        }

        internal void ThrowIfWriteOnly()
        {
            if (this._openFileAccess == FileAccess.Write)
            {
                throw new IOException(System.Windows.SR.Get("CannotRetrievePartsOfWriteOnlyContainer"));
            }
        }

        private static Stream ValidateModeAndAccess(Stream s, FileMode mode, FileAccess access)
        {
            ThrowIfFileModeInvalid(mode);
            ThrowIfFileAccessInvalid(access);
            if (!s.CanWrite && ((access == FileAccess.ReadWrite) || (access == FileAccess.Write)))
            {
                throw new IOException(System.Windows.SR.Get("IncompatibleModeOrAccess"));
            }
            if (!s.CanRead && ((access == FileAccess.ReadWrite) || (access == FileAccess.Read)))
            {
                throw new IOException(System.Windows.SR.Get("IncompatibleModeOrAccess"));
            }
            if ((!s.CanRead || !s.CanWrite) || ((access != FileAccess.Read) && (access != FileAccess.Write)))
            {
                return s;
            }
            return new RestrictedStream(s, access);
        }

        private static void ValidateStreamingAccess(FileMode packageMode, FileAccess packageAccess, FileShare? packageShare, bool streaming)
        {
            if (streaming)
            {
                if ((packageMode != FileMode.Create) && (packageMode != FileMode.CreateNew))
                {
                    throw new NotSupportedException(System.Windows.SR.Get("StreamingModeNotSupportedForConsumption"));
                }
                if (packageAccess != FileAccess.Write)
                {
                    throw new IOException(System.Windows.SR.Get("StreamingPackageProductionImpliesWriteOnlyAccess"));
                }
                if ((packageShare.HasValue && (((FileShare) packageShare) != FileShare.Read)) && (((FileShare) packageShare) != FileShare.None))
                {
                    throw new IOException(System.Windows.SR.Get("StreamingPackageProductionRequiresSingleWriter"));
                }
            }
        }

        public FileAccess FileOpenAccess
        {
            get
            {
                this.ThrowIfObjectDisposed();
                return this._openFileAccess;
            }
        }

        internal bool InStreamingCreation =>
            this._inStreamingCreation;

        public System.IO.Packaging.PackageProperties PackageProperties
        {
            get
            {
                this.ThrowIfObjectDisposed();
                if (this._packageProperties == null)
                {
                    this._packageProperties = new PartBasedPackageProperties(this);
                }
                return this._packageProperties;
            }
        }

        internal delegate bool PartOperation(PackagePart p);

        private sealed class RestrictedStream : Stream
        {
            private bool _canRead;
            private bool _canWrite;
            private bool _disposed;
            private Stream _stream;

            internal RestrictedStream(Stream stream, FileAccess access)
            {
                if (stream == null)
                {
                    throw new ArgumentNullException("stream");
                }
                this._stream = stream;
                if (access == FileAccess.Read)
                {
                    this._canRead = true;
                    this._canWrite = false;
                }
                else if (access == FileAccess.Write)
                {
                    this._canRead = false;
                    this._canWrite = true;
                }
            }

            protected override void Dispose(bool disposing)
            {
                try
                {
                    if (disposing && !this._disposed)
                    {
                        this._stream.Close();
                    }
                }
                finally
                {
                    this._disposed = true;
                    base.Dispose(disposing);
                }
            }

            public override void Flush()
            {
                this.ThrowIfStreamDisposed();
                if (this._canWrite)
                {
                    this._stream.Flush();
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                this.ThrowIfStreamDisposed();
                if (!this._canRead)
                {
                    throw new NotSupportedException(System.Windows.SR.Get("WriteOnlyStream"));
                }
                return this._stream.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                this.ThrowIfStreamDisposed();
                return this._stream.Seek(offset, origin);
            }

            public override void SetLength(long newLength)
            {
                this.ThrowIfStreamDisposed();
                if (!this._canWrite)
                {
                    throw new NotSupportedException(System.Windows.SR.Get("ReadOnlyStream"));
                }
                this._stream.SetLength(newLength);
            }

            private void ThrowIfStreamDisposed()
            {
                if (this._disposed)
                {
                    throw new ObjectDisposedException(null, System.Windows.SR.Get("StreamObjectDisposed"));
                }
            }

            public override void Write(byte[] buf, int offset, int count)
            {
                this.ThrowIfStreamDisposed();
                if (!this._canWrite)
                {
                    throw new NotSupportedException(System.Windows.SR.Get("ReadOnlyStream"));
                }
                this._stream.Write(buf, offset, count);
            }

            public override bool CanRead =>
                (!this._disposed && this._canRead);

            public override bool CanSeek =>
                (!this._disposed && this._stream.CanSeek);

            public override bool CanWrite =>
                (!this._disposed && this._canWrite);

            public override long Length
            {
                get
                {
                    this.ThrowIfStreamDisposed();
                    return this._stream.Length;
                }
            }

            public override long Position
            {
                get
                {
                    this.ThrowIfStreamDisposed();
                    return this._stream.Position;
                }
                set
                {
                    this.ThrowIfStreamDisposed();
                    this._stream.Position = value;
                }
            }
        }
    }
}

