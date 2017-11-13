namespace System.IO.Packaging
{
    using MS.Internal;
    using MS.Internal.IO.Packaging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;

    public abstract class PackagePart
    {
        private System.IO.Packaging.CompressionOption _compressionOption;
        private System.IO.Packaging.Package _container;
        private MS.Internal.ContentType _contentType;
        private bool _deleted;
        private bool _disposed;
        private bool _isRelationshipPart;
        private InternalRelationshipCollection _relationships;
        private List<Stream> _requestedStreams;
        private PackUriHelper.ValidatedPartUri _uri;

        protected PackagePart(System.IO.Packaging.Package package, System.Uri partUri) : this(package, partUri, null, System.IO.Packaging.CompressionOption.NotCompressed)
        {
        }

        protected PackagePart(System.IO.Packaging.Package package, System.Uri partUri, string contentType) : this(package, partUri, contentType, System.IO.Packaging.CompressionOption.NotCompressed)
        {
        }

        protected PackagePart(System.IO.Packaging.Package package, System.Uri partUri, string contentType, System.IO.Packaging.CompressionOption compressionOption)
        {
            this._compressionOption = System.IO.Packaging.CompressionOption.NotCompressed;
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }
            if (partUri == null)
            {
                throw new ArgumentNullException("partUri");
            }
            System.IO.Packaging.Package.ThrowIfCompressionOptionInvalid(compressionOption);
            this._uri = PackUriHelper.ValidatePartUri(partUri);
            this._container = package;
            if (contentType == null)
            {
                this._contentType = null;
            }
            else
            {
                this._contentType = new MS.Internal.ContentType(contentType);
            }
            this._requestedStreams = null;
            this._compressionOption = compressionOption;
            this._isRelationshipPart = PackUriHelper.IsRelationshipPartUri(partUri);
        }

        private void CheckInvalidState()
        {
            this.ThrowIfPackagePartDeleted();
            this.ThrowIfParentContainerClosed();
        }

        private void CleanUpRequestedStreamsList()
        {
            if (this._requestedStreams != null)
            {
                for (int i = this._requestedStreams.Count - 1; i >= 0; i--)
                {
                    if (this.IsStreamClosed(this._requestedStreams[i]))
                    {
                        this._requestedStreams.RemoveAt(i);
                    }
                }
            }
        }

        internal void ClearRelationships()
        {
            if (this._relationships != null)
            {
                this._relationships.Clear();
            }
        }

        internal void Close()
        {
            if (!this._disposed)
            {
                try
                {
                    if (this._requestedStreams != null)
                    {
                        if (!this._deleted)
                        {
                            foreach (Stream stream in this._requestedStreams)
                            {
                                stream.Close();
                            }
                        }
                        this._requestedStreams.Clear();
                    }
                    else if (this._container.InStreamingCreation)
                    {
                        this.GetStream(FileMode.CreateNew, this._container.FileOpenAccess).Close();
                    }
                }
                finally
                {
                    this._requestedStreams = null;
                    this._relationships = null;
                    this._container = null;
                    this._disposed = true;
                }
            }
        }

        internal void CloseRelationships()
        {
            if (!this._deleted)
            {
                if (this._container.InStreamingCreation)
                {
                    this.CloseStreamingRelationships();
                }
                else
                {
                    this.FlushRelationships();
                }
            }
        }

        private void CloseStreamingRelationships()
        {
            if (!this._container.InStreamingCreation)
            {
                throw new IOException(System.Windows.SR.Get("MethodAvailableOnlyInStreamingCreation", new object[] { "CloseRelationships" }));
            }
            if (this._relationships != null)
            {
                this._relationships.CloseInStreamingCreationMode();
            }
        }

        public PackageRelationship CreateRelationship(System.Uri targetUri, TargetMode targetMode, string relationshipType) => 
            this.CreateRelationship(targetUri, targetMode, relationshipType, null);

        public PackageRelationship CreateRelationship(System.Uri targetUri, TargetMode targetMode, string relationshipType, string id)
        {
            this.CheckInvalidState();
            this._container.ThrowIfReadOnly();
            this.EnsureRelationships();
            return this._relationships.Add(targetUri, targetMode, relationshipType, id);
        }

        public void DeleteRelationship(string id)
        {
            this.CheckInvalidState();
            this._container.ThrowIfReadOnly();
            this._container.ThrowIfInStreamingCreation("DeleteRelationship");
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            InternalRelationshipCollection.ThrowIfInvalidXsdId(id);
            this.EnsureRelationships();
            this._relationships.Delete(id);
        }

        private void EnsureRelationships()
        {
            if (this._relationships == null)
            {
                this.ThrowIfRelationship();
                this._relationships = new InternalRelationshipCollection(this);
            }
        }

        internal void Flush()
        {
            if (this._requestedStreams != null)
            {
                foreach (Stream stream in this._requestedStreams)
                {
                    if (stream.CanWrite)
                    {
                        stream.Flush();
                    }
                }
            }
        }

        internal void FlushRelationships()
        {
            if ((this._relationships != null) && (this._container.FileOpenAccess != FileAccess.Read))
            {
                this._relationships.Flush();
            }
        }

        protected virtual string GetContentTypeCore()
        {
            throw new NotSupportedException(System.Windows.SR.Get("GetContentTypeCoreNotImplemented"));
        }

        public PackageRelationship GetRelationship(string id)
        {
            PackageRelationship relationshipHelper = this.GetRelationshipHelper(id);
            if (relationshipHelper == null)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("PackagePartRelationshipDoesNotExist"));
            }
            return relationshipHelper;
        }

        private PackageRelationship GetRelationshipHelper(string id)
        {
            this.CheckInvalidState();
            this._container.ThrowIfWriteOnly();
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
            this.CheckInvalidState();
            this._container.ThrowIfWriteOnly();
            if (relationshipType == null)
            {
                throw new ArgumentNullException("relationshipType");
            }
            InternalRelationshipCollection.ThrowIfInvalidRelationshipType(relationshipType);
            return this.GetRelationshipsHelper(relationshipType);
        }

        private PackageRelationshipCollection GetRelationshipsHelper(string filterString)
        {
            this.CheckInvalidState();
            this._container.ThrowIfWriteOnly();
            this.EnsureRelationships();
            return new PackageRelationshipCollection(this._relationships, filterString);
        }

        public Stream GetStream()
        {
            this.CheckInvalidState();
            return this.GetStream(FileMode.OpenOrCreate, this._container.FileOpenAccess);
        }

        public Stream GetStream(FileMode mode)
        {
            this.CheckInvalidState();
            return this.GetStream(mode, this._container.FileOpenAccess);
        }

        public Stream GetStream(FileMode mode, FileAccess access)
        {
            this.CheckInvalidState();
            this.ThrowIfOpenAccessModesAreIncompatible(mode, access);
            Stream streamCore = this.GetStreamCore(mode, access);
            if (streamCore == null)
            {
                throw new IOException(System.Windows.SR.Get("NullStreamReturned"));
            }
            if (this._requestedStreams == null)
            {
                this._requestedStreams = new List<Stream>();
            }
            this.CleanUpRequestedStreamsList();
            this._requestedStreams.Add(streamCore);
            return streamCore;
        }

        protected abstract Stream GetStreamCore(FileMode mode, FileAccess access);
        private bool IsStreamClosed(Stream s) => 
            ((!s.CanRead && !s.CanSeek) && !s.CanWrite);

        public bool RelationshipExists(string id) => 
            (this.GetRelationshipHelper(id) != null);

        private void ThrowIfOpenAccessModesAreIncompatible(FileMode mode, FileAccess access)
        {
            System.IO.Packaging.Package.ThrowIfFileModeInvalid(mode);
            System.IO.Packaging.Package.ThrowIfFileAccessInvalid(access);
            if ((access == FileAccess.Read) && (((mode == FileMode.Create) || (mode == FileMode.CreateNew)) || ((mode == FileMode.Truncate) || (mode == FileMode.Append))))
            {
                throw new IOException(System.Windows.SR.Get("UnsupportedCombinationOfModeAccess"));
            }
            if (((this._container.FileOpenAccess == FileAccess.Read) && (access != FileAccess.Read)) || ((this._container.FileOpenAccess == FileAccess.Write) && (access != FileAccess.Write)))
            {
                throw new IOException(System.Windows.SR.Get("ContainerAndPartModeIncompatible"));
            }
        }

        private void ThrowIfPackagePartDeleted()
        {
            if (this._deleted)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("PackagePartDeleted"));
            }
        }

        private void ThrowIfParentContainerClosed()
        {
            if (this._container == null)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("ParentContainerClosed"));
            }
        }

        private void ThrowIfRelationship()
        {
            if (this.IsRelationshipPart)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("RelationshipPartsCannotHaveRelationships"));
            }
        }

        public System.IO.Packaging.CompressionOption CompressionOption
        {
            get
            {
                this.CheckInvalidState();
                return this._compressionOption;
            }
        }

        public string ContentType
        {
            get
            {
                this.CheckInvalidState();
                return this._contentType?.ToString();
            }
        }

        internal bool IsClosed =>
            this._disposed;

        internal bool IsDeleted
        {
            get => 
                this._deleted;
            set
            {
                this._deleted = value;
            }
        }

        internal bool IsRelationshipPart =>
            this._isRelationshipPart;

        public System.IO.Packaging.Package Package
        {
            get
            {
                this.CheckInvalidState();
                return this._container;
            }
        }

        public System.Uri Uri
        {
            get
            {
                this.CheckInvalidState();
                return this._uri;
            }
        }

        internal MS.Internal.ContentType ValidatedContentType =>
            this._contentType;
    }
}

