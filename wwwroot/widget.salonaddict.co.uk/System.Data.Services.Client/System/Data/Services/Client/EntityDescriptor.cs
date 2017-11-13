namespace System.Data.Services.Client
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    [DebuggerDisplay("State = {state}, Uri = {editLink}, Element = {entity.GetType().ToString()}")]
    public sealed class EntityDescriptor : Descriptor
    {
        private Uri editLink;
        private Uri editMediaLink;
        private object entity;
        private string entitySetName;
        private string etag;
        private string identity;
        private bool mediaLinkEntry;
        private EntityDescriptor parentDescriptor;
        private string parentProperty;
        private Uri readStreamLink;
        private DataServiceContext.DataServiceSaveStream saveStream;
        private Uri selfLink;
        private string serverTypeName;
        private string streamETag;
        private StreamStates streamState;

        internal EntityDescriptor(string identity, Uri selfLink, Uri editLink, object entity, EntityDescriptor parentEntity, string parentProperty, string entitySetName, string etag, EntityStates state) : base(state)
        {
            this.identity = identity;
            this.selfLink = selfLink;
            this.editLink = editLink;
            this.parentDescriptor = parentEntity;
            this.parentProperty = parentProperty;
            this.entity = entity;
            this.etag = etag;
            this.entitySetName = entitySetName;
        }

        internal void CloseSaveStream()
        {
            if (this.saveStream != null)
            {
                DataServiceContext.DataServiceSaveStream saveStream = this.saveStream;
                this.saveStream = null;
                saveStream.Close();
            }
        }

        internal Uri GetEditMediaResourceUri(Uri serviceBaseUri)
        {
            if (this.EditStreamUri != null)
            {
                return Util.CreateUri(serviceBaseUri, this.EditStreamUri);
            }
            return null;
        }

        private Uri GetLink(bool queryLink)
        {
            if (queryLink && (this.SelfLink != null))
            {
                return this.SelfLink;
            }
            if (this.EditLink != null)
            {
                return this.EditLink;
            }
            if (!string.IsNullOrEmpty(this.entitySetName))
            {
                return Util.CreateUri(this.entitySetName, UriKind.Relative);
            }
            return Util.CreateUri(this.parentProperty, UriKind.Relative);
        }

        internal Uri GetMediaResourceUri(Uri serviceBaseUri)
        {
            if (this.ReadStreamUri != null)
            {
                return Util.CreateUri(serviceBaseUri, this.ReadStreamUri);
            }
            return null;
        }

        internal LinkDescriptor GetRelatedEnd() => 
            new LinkDescriptor(this.parentDescriptor.entity, this.parentProperty, this.entity);

        internal Uri GetResourceUri(Uri baseUriWithSlash, bool queryLink)
        {
            if (this.parentDescriptor == null)
            {
                return Util.CreateUri(baseUriWithSlash, this.GetLink(queryLink));
            }
            if (this.parentDescriptor.Identity == null)
            {
                return Util.CreateUri(Util.CreateUri(baseUriWithSlash, new Uri("$" + this.parentDescriptor.ChangeOrder.ToString(CultureInfo.InvariantCulture), UriKind.Relative)), Util.CreateUri(this.parentProperty, UriKind.Relative));
            }
            return Util.CreateUri(Util.CreateUri(baseUriWithSlash, this.parentDescriptor.GetLink(queryLink)), this.GetLink(queryLink));
        }

        internal bool IsRelatedEntity(LinkDescriptor related)
        {
            if (this.entity != related.Source)
            {
                return (this.entity == related.Target);
            }
            return true;
        }

        public Uri EditLink
        {
            get => 
                this.editLink;
            internal set
            {
                this.editLink = value;
            }
        }

        public Uri EditStreamUri
        {
            get => 
                this.editMediaLink;
            internal set
            {
                this.editMediaLink = value;
                if (value != null)
                {
                    this.mediaLinkEntry = true;
                }
            }
        }

        public object Entity =>
            this.entity;

        public string ETag
        {
            get => 
                this.etag;
            internal set
            {
                this.etag = value;
            }
        }

        public string Identity
        {
            get => 
                this.identity;
            internal set
            {
                Util.CheckArgumentNotEmpty(value, "Identity");
                this.identity = value;
                this.parentDescriptor = null;
                this.parentProperty = null;
                this.entitySetName = null;
            }
        }

        internal bool IsDeepInsert =>
            (this.parentDescriptor != null);

        internal bool IsMediaLinkEntry =>
            this.mediaLinkEntry;

        internal override bool IsModified =>
            (base.IsModified || (this.saveStream != null));

        internal override bool IsResource =>
            true;

        internal object ParentEntity
        {
            get
            {
                if (this.parentDescriptor == null)
                {
                    return null;
                }
                return this.parentDescriptor.entity;
            }
        }

        public EntityDescriptor ParentForInsert =>
            this.parentDescriptor;

        public string ParentPropertyForInsert =>
            this.parentProperty;

        public Uri ReadStreamUri
        {
            get => 
                this.readStreamLink;
            internal set
            {
                this.readStreamLink = value;
                if (value != null)
                {
                    this.mediaLinkEntry = true;
                }
            }
        }

        internal DataServiceContext.DataServiceSaveStream SaveStream
        {
            get => 
                this.saveStream;
            set
            {
                this.saveStream = value;
                if (value != null)
                {
                    this.mediaLinkEntry = true;
                }
            }
        }

        public Uri SelfLink
        {
            get => 
                this.selfLink;
            internal set
            {
                this.selfLink = value;
            }
        }

        public string ServerTypeName
        {
            get => 
                this.serverTypeName;
            internal set
            {
                this.serverTypeName = value;
            }
        }

        public string StreamETag
        {
            get => 
                this.streamETag;
            internal set
            {
                this.streamETag = value;
            }
        }

        internal StreamStates StreamState
        {
            get => 
                this.streamState;
            set
            {
                this.streamState = value;
            }
        }
    }
}

