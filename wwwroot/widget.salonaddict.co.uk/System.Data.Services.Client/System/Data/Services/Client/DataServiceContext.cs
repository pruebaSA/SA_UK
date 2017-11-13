namespace System.Data.Services.Client
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using System.Xml.Linq;

    public class DataServiceContext
    {
        private bool applyingChanges;
        private readonly Uri baseUri;
        private readonly Uri baseUriWithSlash;
        private Dictionary<LinkDescriptor, LinkDescriptor> bindings = new Dictionary<LinkDescriptor, LinkDescriptor>(LinkDescriptor.EquivalenceComparer);
        private ICredentials credentials;
        private string dataNamespace;
        private Dictionary<object, EntityDescriptor> entityDescriptors = new Dictionary<object, EntityDescriptor>(EqualityComparer<object>.Default);
        private Dictionary<string, EntityDescriptor> identityToDescriptor;
        private bool ignoreMissingProperties;
        private bool ignoreResourceNotFoundException;
        private System.Data.Services.Client.MergeOption mergeOption;
        private static readonly string NewLine = Environment.NewLine;
        private uint nextChange;
        private bool postTunneling;
        private Func<Type, string> resolveName;
        private Func<string, Type> resolveType;
        private SaveChangesOptions saveChangesDefaultOptions;
        private int timeout;
        private Uri typeScheme;

        internal event EventHandler<SaveChangesEventArgs> ChangesSaved;

        public event EventHandler<ReadingWritingEntityEventArgs> ReadingEntity;

        public event EventHandler<SendingRequestEventArgs> SendingRequest;

        public event EventHandler<ReadingWritingEntityEventArgs> WritingEntity;

        public DataServiceContext(Uri serviceRoot)
        {
            Util.CheckArgumentNull<Uri>(serviceRoot, "serviceRoot");
            if ((!serviceRoot.IsAbsoluteUri || !Uri.IsWellFormedUriString(serviceRoot.OriginalString, UriKind.Absolute)) || ((!string.IsNullOrEmpty(serviceRoot.Query) || !string.IsNullOrEmpty(serviceRoot.Fragment)) || ((serviceRoot.Scheme != "http") && (serviceRoot.Scheme != "https"))))
            {
                throw System.Data.Services.Client.Error.Argument(System.Data.Services.Client.Strings.Context_BaseUri, "serviceRoot");
            }
            this.baseUri = serviceRoot;
            this.baseUriWithSlash = serviceRoot;
            if (!serviceRoot.OriginalString.EndsWith("/", StringComparison.Ordinal))
            {
                this.baseUriWithSlash = Util.CreateUri(serviceRoot.OriginalString + "/", UriKind.Absolute);
            }
            this.mergeOption = System.Data.Services.Client.MergeOption.AppendOnly;
            this.DataNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices";
            this.UsePostTunneling = false;
            this.typeScheme = new Uri("http://schemas.microsoft.com/ado/2007/08/dataservices/scheme");
        }

        public void AddLink(object source, string sourceProperty, object target)
        {
            this.EnsureRelatable(source, sourceProperty, target, EntityStates.Added);
            LinkDescriptor key = new LinkDescriptor(source, sourceProperty, target);
            if (this.bindings.ContainsKey(key))
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_RelationAlreadyContained);
            }
            key.State = EntityStates.Added;
            this.bindings.Add(key, key);
            this.IncrementChange(key);
        }

        public void AddObject(string entitySetName, object entity)
        {
            this.ValidateEntitySetName(ref entitySetName);
            ValidateEntityType(entity);
            EntityDescriptor descriptor = new EntityDescriptor(null, null, null, entity, null, null, entitySetName, null, EntityStates.Added);
            try
            {
                this.entityDescriptors.Add(entity, descriptor);
            }
            catch (ArgumentException)
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_EntityAlreadyContained);
            }
            this.IncrementChange(descriptor);
        }

        public void AddRelatedObject(object source, string sourceProperty, object target)
        {
            Util.CheckArgumentNull<object>(source, "source");
            Util.CheckArgumentNotEmpty(sourceProperty, "propertyName");
            Util.CheckArgumentNull<object>(target, "target");
            ValidateEntityType(source);
            EntityDescriptor parentEntity = this.EnsureContained(source, "source");
            if (parentEntity.State == EntityStates.Deleted)
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_AddRelatedObjectSourceDeleted);
            }
            ClientType.ClientProperty property = ClientType.Create(source.GetType()).GetProperty(sourceProperty, false);
            if (property.IsKnownType || (property.CollectionType == null))
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_AddRelatedObjectCollectionOnly);
            }
            ClientType type2 = ClientType.Create(target.GetType());
            ValidateEntityType(target);
            if (!ClientType.Create(property.CollectionType).ElementType.IsAssignableFrom(type2.ElementType))
            {
                throw System.Data.Services.Client.Error.Argument(System.Data.Services.Client.Strings.Context_RelationNotRefOrCollection, "target");
            }
            EntityDescriptor descriptor2 = new EntityDescriptor(null, null, null, target, parentEntity, sourceProperty, null, null, EntityStates.Added);
            try
            {
                this.entityDescriptors.Add(target, descriptor2);
            }
            catch (ArgumentException)
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_EntityAlreadyContained);
            }
            LinkDescriptor relatedEnd = descriptor2.GetRelatedEnd();
            relatedEnd.State = EntityStates.Added;
            this.bindings.Add(relatedEnd, relatedEnd);
            this.IncrementChange(descriptor2);
        }

        internal void AttachIdentity(string identity, Uri selfLink, Uri editLink, object entity, string etag)
        {
            this.EnsureIdentityToResource();
            EntityDescriptor resource = this.entityDescriptors[entity];
            this.DetachResourceIdentity(resource);
            if (resource.IsDeepInsert)
            {
                LinkDescriptor descriptor2 = this.bindings[resource.GetRelatedEnd()];
                descriptor2.State = EntityStates.Unchanged;
            }
            resource.ETag = etag;
            resource.Identity = identity;
            resource.SelfLink = selfLink;
            resource.EditLink = editLink;
            resource.State = EntityStates.Unchanged;
            this.identityToDescriptor[identity] = resource;
        }

        public void AttachLink(object source, string sourceProperty, object target)
        {
            this.AttachLink(source, sourceProperty, target, System.Data.Services.Client.MergeOption.NoTracking);
        }

        internal void AttachLink(object source, string sourceProperty, object target, System.Data.Services.Client.MergeOption linkMerge)
        {
            this.EnsureRelatable(source, sourceProperty, target, EntityStates.Unchanged);
            LinkDescriptor descriptor = null;
            LinkDescriptor key = new LinkDescriptor(source, sourceProperty, target);
            if (this.bindings.TryGetValue(key, out descriptor))
            {
                switch (linkMerge)
                {
                    case System.Data.Services.Client.MergeOption.OverwriteChanges:
                        key = descriptor;
                        break;

                    case System.Data.Services.Client.MergeOption.PreserveChanges:
                        if (((EntityStates.Added == descriptor.State) || (EntityStates.Unchanged == descriptor.State)) || ((EntityStates.Modified == descriptor.State) && (descriptor.Target != null)))
                        {
                            key = descriptor;
                        }
                        break;

                    case System.Data.Services.Client.MergeOption.NoTracking:
                        throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_RelationAlreadyContained);
                }
            }
            else if ((null != ClientType.Create(source.GetType()).GetProperty(sourceProperty, false).CollectionType) || ((descriptor = this.DetachReferenceLink(source, sourceProperty, target, linkMerge)) == null))
            {
                this.bindings.Add(key, key);
                this.IncrementChange(key);
            }
            else if ((linkMerge != System.Data.Services.Client.MergeOption.AppendOnly) && ((System.Data.Services.Client.MergeOption.PreserveChanges != linkMerge) || (EntityStates.Modified != descriptor.State)))
            {
                key = descriptor;
            }
            key.State = EntityStates.Unchanged;
        }

        internal void AttachLocation(object entity, string location)
        {
            Uri uri = new Uri(location, UriKind.Absolute);
            string str = Util.ReferenceIdentity(Util.UriToString(uri));
            this.EnsureIdentityToResource();
            EntityDescriptor resource = this.entityDescriptors[entity];
            this.DetachResourceIdentity(resource);
            if (resource.IsDeepInsert)
            {
                LinkDescriptor descriptor2 = this.bindings[resource.GetRelatedEnd()];
                descriptor2.State = EntityStates.Unchanged;
            }
            resource.Identity = str;
            resource.EditLink = uri;
            this.identityToDescriptor[str] = resource;
        }

        public void AttachTo(string entitySetName, object entity)
        {
            this.AttachTo(entitySetName, entity, null);
        }

        public void AttachTo(string entitySetName, object entity, string etag)
        {
            this.ValidateEntitySetName(ref entitySetName);
            Uri editLink = GenerateEditLinkUri(this.baseUriWithSlash, entitySetName, entity);
            EntityDescriptor descriptor = new EntityDescriptor(Util.ReferenceIdentity(editLink.AbsoluteUri), null, editLink, entity, null, null, null, etag, EntityStates.Unchanged);
            this.InternalAttachEntityDescriptor(descriptor, true);
        }

        public IAsyncResult BeginExecute<T>(DataServiceQueryContinuation<T> continuation, AsyncCallback callback, object state)
        {
            Util.CheckArgumentNull<DataServiceQueryContinuation<T>>(continuation, "continuation");
            return new DataServiceRequest<T>(continuation.CreateQueryComponents(), continuation.Plan).BeginExecute(this, this, callback, state);
        }

        public IAsyncResult BeginExecute<TElement>(Uri requestUri, AsyncCallback callback, object state)
        {
            requestUri = Util.CreateUri(this.baseUriWithSlash, requestUri);
            QueryComponents queryComponents = new QueryComponents(requestUri, Util.DataServiceVersionEmpty, typeof(TElement), null, null);
            return new DataServiceRequest<TElement>(queryComponents, null).BeginExecute(this, this, callback, state);
        }

        public IAsyncResult BeginExecuteBatch(AsyncCallback callback, object state, params DataServiceRequest[] queries)
        {
            Util.CheckArgumentNotEmpty<DataServiceRequest>(queries, "queries");
            SaveResult result = new SaveResult(this, "ExecuteBatch", queries, SaveChangesOptions.Batch, callback, state, true);
            result.BatchBeginRequest(false);
            return result;
        }

        public IAsyncResult BeginGetReadStream(object entity, DataServiceRequestArgs args, AsyncCallback callback, object state)
        {
            GetReadStreamResult result = this.CreateGetReadStreamResult(entity, args, callback, state);
            result.Begin();
            return result;
        }

        public IAsyncResult BeginLoadProperty(object entity, string propertyName, AsyncCallback callback, object state) => 
            this.BeginLoadProperty(entity, propertyName, (Uri) null, callback, state);

        public IAsyncResult BeginLoadProperty(object entity, string propertyName, DataServiceQueryContinuation continuation, AsyncCallback callback, object state)
        {
            Util.CheckArgumentNull<DataServiceQueryContinuation>(continuation, "continuation");
            LoadPropertyResult result = this.CreateLoadPropertyRequest(entity, propertyName, callback, state, null, continuation);
            result.BeginExecute();
            return result;
        }

        public IAsyncResult BeginLoadProperty(object entity, string propertyName, Uri nextLinkUri, AsyncCallback callback, object state)
        {
            LoadPropertyResult result = this.CreateLoadPropertyRequest(entity, propertyName, callback, state, nextLinkUri, null);
            result.BeginExecute();
            return result;
        }

        public IAsyncResult BeginSaveChanges(AsyncCallback callback, object state) => 
            this.BeginSaveChanges(this.SaveChangesDefaultOptions, callback, state);

        public IAsyncResult BeginSaveChanges(SaveChangesOptions options, AsyncCallback callback, object state)
        {
            ValidateSaveChangesOptions(options);
            SaveResult result = new SaveResult(this, "SaveChanges", null, options, callback, state, true);
            bool replaceOnUpdate = IsFlagSet(options, SaveChangesOptions.ReplaceOnUpdate);
            if (IsFlagSet(options, SaveChangesOptions.Batch))
            {
                result.BatchBeginRequest(replaceOnUpdate);
                return result;
            }
            result.BeginNextChange(replaceOnUpdate);
            return result;
        }

        public void CancelRequest(IAsyncResult asyncResult)
        {
            Util.CheckArgumentNull<IAsyncResult>(asyncResult, "asyncResult");
            BaseAsyncResult result = asyncResult as BaseAsyncResult;
            if ((result == null) || (this != result.Source))
            {
                object context = null;
                DataServiceQuery source = null;
                if (result != null)
                {
                    source = result.Source as DataServiceQuery;
                    if (source != null)
                    {
                        DataServiceQueryProvider provider = source.Provider as DataServiceQueryProvider;
                        if (provider != null)
                        {
                            context = provider.Context;
                        }
                    }
                }
                if (this != context)
                {
                    throw System.Data.Services.Client.Error.Argument(System.Data.Services.Client.Strings.Context_DidNotOriginateAsync, "asyncResult");
                }
            }
            if (!result.IsCompletedInternally)
            {
                result.SetAborted();
                WebRequest abortable = result.Abortable;
                if (abortable != null)
                {
                    abortable.Abort();
                }
            }
        }

        private static bool CanHandleResponseVersion(string responseVersion)
        {
            if (!string.IsNullOrEmpty(responseVersion))
            {
                KeyValuePair<Version, string> pair;
                if (!HttpProcessUtility.TryReadVersion(responseVersion, out pair))
                {
                    return false;
                }
                if (!Util.SupportedResponseVersions.Contains<Version>(pair.Key))
                {
                    return false;
                }
            }
            return true;
        }

        private GetReadStreamResult CreateGetReadStreamResult(object entity, DataServiceRequestArgs args, AsyncCallback callback, object state)
        {
            EntityDescriptor descriptor = this.EnsureContained(entity, "entity");
            Util.CheckArgumentNull<DataServiceRequestArgs>(args, "args");
            Uri mediaResourceUri = descriptor.GetMediaResourceUri(this.baseUriWithSlash);
            if (mediaResourceUri == null)
            {
                throw new ArgumentException(System.Data.Services.Client.Strings.Context_EntityNotMediaLinkEntry, "entity");
            }
            HttpWebRequest request = this.CreateRequest(mediaResourceUri, "GET", true, null, null, false);
            WebUtil.ApplyHeadersToRequest(args.Headers, request, false);
            return new GetReadStreamResult(this, "GetReadStream", request, callback, state);
        }

        private LoadPropertyResult CreateLoadPropertyRequest(object entity, string propertyName, AsyncCallback callback, object state, Uri requestUri, DataServiceQueryContinuation continuation)
        {
            ProjectionPlan plan;
            Version dataServiceVersionEmpty;
            EntityDescriptor descriptor = this.EnsureContained(entity, "entity");
            Util.CheckArgumentNotEmpty(propertyName, "propertyName");
            ClientType type = ClientType.Create(entity.GetType());
            if (EntityStates.Added == descriptor.State)
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_NoLoadWithInsertEnd);
            }
            ClientType.ClientProperty property = type.GetProperty(propertyName, false);
            if (continuation == null)
            {
                plan = null;
            }
            else
            {
                plan = continuation.Plan;
                requestUri = continuation.NextLinkUri;
            }
            bool allowAnyType = (type.MediaDataMember != null) && (propertyName == type.MediaDataMember.PropertyName);
            if (requestUri == null)
            {
                Uri uri;
                if (allowAnyType)
                {
                    uri = Util.CreateUri("$value", UriKind.Relative);
                }
                else
                {
                    uri = Util.CreateUri(propertyName + ((property.CollectionType != null) ? "()" : string.Empty), UriKind.Relative);
                }
                requestUri = Util.CreateUri(descriptor.GetResourceUri(this.baseUriWithSlash, true), uri);
                dataServiceVersionEmpty = Util.DataServiceVersion1;
            }
            else
            {
                dataServiceVersionEmpty = Util.DataServiceVersionEmpty;
            }
            HttpWebRequest request = this.CreateRequest(requestUri, "GET", allowAnyType, null, dataServiceVersionEmpty, false);
            return new LoadPropertyResult(entity, propertyName, this, request, callback, state, DataServiceRequest.GetInstance(property.PropertyType, requestUri), plan);
        }

        public DataServiceQuery<T> CreateQuery<T>(string entitySetName)
        {
            Util.CheckArgumentNotEmpty(entitySetName, "entitySetName");
            this.ValidateEntitySetName(ref entitySetName);
            return new DataServiceQuery<T>.DataServiceOrderedQuery(new ResourceSetExpression(typeof(IOrderedQueryable<T>), null, Expression.Constant(entitySetName), typeof(T), null, CountOption.None, null, null), new DataServiceQueryProvider(this));
        }

        private HttpWebRequest CreateRequest(LinkDescriptor binding)
        {
            if (binding.ContentGeneratedForSave)
            {
                return null;
            }
            EntityDescriptor sourceResource = this.entityDescriptors[binding.Source];
            EntityDescriptor descriptor2 = (binding.Target != null) ? this.entityDescriptors[binding.Target] : null;
            if (sourceResource.Identity == null)
            {
                binding.ContentGeneratedForSave = true;
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_LinkResourceInsertFailure, sourceResource.SaveError);
            }
            if ((descriptor2 != null) && (descriptor2.Identity == null))
            {
                binding.ContentGeneratedForSave = true;
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_LinkResourceInsertFailure, descriptor2.SaveError);
            }
            return this.CreateRequest(this.CreateRequestUri(sourceResource, binding), GetLinkHttpMethod(binding), false, "application/xml", Util.DataServiceVersion1, false);
        }

        private HttpWebRequest CreateRequest(EntityDescriptor box, EntityStates state, bool replaceOnUpdate)
        {
            string entityHttpMethod = GetEntityHttpMethod(state, replaceOnUpdate);
            Uri resourceUri = box.GetResourceUri(this.baseUriWithSlash, false);
            Version requestVersion = ClientType.Create(box.Entity.GetType()).EpmIsV1Compatible ? Util.DataServiceVersion1 : Util.DataServiceVersion2;
            HttpWebRequest request = this.CreateRequest(resourceUri, entityHttpMethod, false, "application/atom+xml", requestVersion, false);
            if ((box.ETag != null) && ((EntityStates.Deleted == state) || (EntityStates.Modified == state)))
            {
                request.Headers.Set(HttpRequestHeader.IfMatch, box.ETag);
            }
            return request;
        }

        internal HttpWebRequest CreateRequest(Uri requestUri, string method, bool allowAnyType, string contentType, Version requestVersion, bool sendChunked)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(requestUri);
            if (this.Credentials != null)
            {
                request.Credentials = this.Credentials;
            }
            if (this.timeout != 0)
            {
                TimeSpan span = new TimeSpan(0, 0, this.timeout);
                request.Timeout = (int) Math.Min(2147483647.0, span.TotalMilliseconds);
            }
            request.KeepAlive = true;
            request.UserAgent = "Microsoft ADO.NET Data Services";
            if ((this.UsePostTunneling && !object.ReferenceEquals("POST", method)) && !object.ReferenceEquals("GET", method))
            {
                request.Headers["X-HTTP-Method"] = method;
                request.Method = "POST";
            }
            else
            {
                request.Method = method;
            }
            if ((requestVersion != null) && (requestVersion.Major > 0))
            {
                request.Headers["DataServiceVersion"] = requestVersion.ToString() + ";NetFx";
            }
            request.Headers["MaxDataServiceVersion"] = Util.MaxResponseVersion.ToString() + ";NetFx";
            if (sendChunked)
            {
                request.SendChunked = true;
            }
            if (this.SendingRequest != null)
            {
                WebHeaderCollection requestHeaders = request.Headers;
                SendingRequestEventArgs e = new SendingRequestEventArgs(request, requestHeaders);
                this.SendingRequest(this, e);
                if (!object.ReferenceEquals(e.Request, request))
                {
                    request = (HttpWebRequest) e.Request;
                }
            }
            request.Accept = allowAnyType ? "*/*" : "application/atom+xml,application/xml";
            request.Headers[HttpRequestHeader.AcceptCharset] = "UTF-8";
            bool flag = false;
            bool flag2 = true;
            if (!object.ReferenceEquals("GET", method))
            {
                request.ContentType = contentType;
                if (object.ReferenceEquals("DELETE", method))
                {
                    request.ContentLength = 0L;
                }
                flag = true;
                if (this.UsePostTunneling && !object.ReferenceEquals("POST", method))
                {
                    request.Headers["X-HTTP-Method"] = method;
                    method = "POST";
                    flag2 = false;
                }
            }
            request.AllowWriteStreamBuffering = flag;
            ICollection<string> allKeys = request.Headers.AllKeys;
            if (allKeys.Contains("If-Match"))
            {
                request.Headers.Remove(HttpRequestHeader.IfMatch);
            }
            if (flag2 && allKeys.Contains("X-HTTP-Method"))
            {
                request.Headers.Remove("X-HTTP-Method");
            }
            request.Method = method;
            return request;
        }

        private void CreateRequestBatch(LinkDescriptor binding, StreamWriter text)
        {
            string absoluteUri;
            EntityDescriptor sourceResource = this.entityDescriptors[binding.Source];
            if (sourceResource.Identity != null)
            {
                absoluteUri = this.CreateRequestUri(sourceResource, binding).AbsoluteUri;
            }
            else
            {
                Uri uri = this.CreateRequestRelativeUri(binding);
                absoluteUri = "$" + sourceResource.ChangeOrder.ToString(CultureInfo.InvariantCulture) + "/" + uri.OriginalString;
            }
            WriteOperationRequestHeaders(text, GetLinkHttpMethod(binding), absoluteUri, Util.DataServiceVersion1);
            text.WriteLine("{0}: {1}", "Content-ID", binding.ChangeOrder);
            if ((EntityStates.Added == binding.State) || ((EntityStates.Modified == binding.State) && (binding.Target != null)))
            {
                text.WriteLine("{0}: {1}", "Content-Type", "application/xml");
            }
        }

        private void CreateRequestBatch(EntityDescriptor box, StreamWriter text, bool replaceOnUpdate)
        {
            Uri resourceUri = box.GetResourceUri(this.baseUriWithSlash, false);
            Version requestVersion = ClientType.Create(box.Entity.GetType()).EpmIsV1Compatible ? Util.DataServiceVersion1 : Util.DataServiceVersion2;
            WriteOperationRequestHeaders(text, GetEntityHttpMethod(box.State, replaceOnUpdate), resourceUri.AbsoluteUri, requestVersion);
            text.WriteLine("{0}: {1}", "Content-ID", box.ChangeOrder);
            if (EntityStates.Deleted != box.State)
            {
                text.WriteLine("{0}: {1}", "Content-Type", "application/atom+xml;type=entry");
            }
            if ((box.ETag != null) && ((EntityStates.Deleted == box.State) || (EntityStates.Modified == box.State)))
            {
                text.WriteLine("{0}: {1}", "If-Match", box.ETag);
            }
        }

        private Stream CreateRequestData(EntityDescriptor box, bool newline)
        {
            MemoryStream stream = null;
            XmlWriter writer;
            bool flag;
            EntityStates state = box.State;
            if (state != EntityStates.Added)
            {
                if (state == EntityStates.Deleted)
                {
                    goto Label_0028;
                }
                if (state != EntityStates.Modified)
                {
                    System.Data.Services.Client.Error.ThrowInternalError(InternalError.UnvalidatedEntityState);
                    goto Label_0028;
                }
            }
            stream = new MemoryStream();
        Label_0028:
            if (stream == null)
            {
                return stream;
            }
            XDocument document = null;
            if (this.WritingEntity != null)
            {
                document = new XDocument();
                writer = document.CreateWriter();
            }
            else
            {
                writer = XmlUtil.CreateXmlWriterAndWriteProcessingInstruction(stream, HttpProcessUtility.EncodingUtf8NoPreamble);
            }
            ClientType type = ClientType.Create(box.Entity.GetType());
            string serverTypeName = this.GetServerTypeName(box);
            writer.WriteStartElement("entry", "http://www.w3.org/2005/Atom");
            writer.WriteAttributeString("d", "http://www.w3.org/2000/xmlns/", this.DataNamespace);
            writer.WriteAttributeString("m", "http://www.w3.org/2000/xmlns/", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            if (!string.IsNullOrEmpty(serverTypeName))
            {
                writer.WriteStartElement("category", "http://www.w3.org/2005/Atom");
                writer.WriteAttributeString("scheme", this.typeScheme.OriginalString);
                writer.WriteAttributeString("term", serverTypeName);
                writer.WriteEndElement();
            }
            if (type.HasEntityPropertyMappings)
            {
                using (EpmSyndicationContentSerializer serializer = new EpmSyndicationContentSerializer(type.EpmTargetTree, box.Entity, writer))
                {
                    serializer.Serialize();
                    goto Label_0176;
                }
            }
            writer.WriteElementString("title", "http://www.w3.org/2005/Atom", string.Empty);
            writer.WriteStartElement("author", "http://www.w3.org/2005/Atom");
            writer.WriteElementString("name", "http://www.w3.org/2005/Atom", string.Empty);
            writer.WriteEndElement();
            writer.WriteElementString("updated", "http://www.w3.org/2005/Atom", XmlConvert.ToString(DateTime.UtcNow, XmlDateTimeSerializationMode.RoundtripKind));
        Label_0176:
            if (EntityStates.Modified == box.State)
            {
                writer.WriteElementString("id", Util.DereferenceIdentity(box.Identity));
            }
            else
            {
                writer.WriteElementString("id", "http://www.w3.org/2005/Atom", string.Empty);
            }
            if (EntityStates.Added == box.State)
            {
                this.CreateRequestDataLinks(box, writer);
            }
            if (!type.IsMediaLinkEntry && !box.IsMediaLinkEntry)
            {
                writer.WriteStartElement("content", "http://www.w3.org/2005/Atom");
                writer.WriteAttributeString("type", "application/xml");
            }
            writer.WriteStartElement("properties", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            this.WriteContentProperties(writer, type, box.Entity, type.HasEntityPropertyMappings ? type.EpmSourceTree.Root : null, out flag);
            writer.WriteEndElement();
            if (!type.IsMediaLinkEntry && !box.IsMediaLinkEntry)
            {
                writer.WriteEndElement();
            }
            if (type.HasEntityPropertyMappings)
            {
                using (EpmCustomContentSerializer serializer2 = new EpmCustomContentSerializer(type.EpmTargetTree, box.Entity, writer))
                {
                    serializer2.Serialize();
                }
            }
            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
            if (this.WritingEntity != null)
            {
                ReadingWritingEntityEventArgs e = new ReadingWritingEntityEventArgs(box.Entity, document.Root);
                this.WritingEntity(this, e);
                XmlWriterSettings settings = XmlUtil.CreateXmlWriterSettings(HttpProcessUtility.EncodingUtf8NoPreamble);
                settings.ConformanceLevel = ConformanceLevel.Auto;
                using (XmlWriter writer2 = XmlWriter.Create(stream, settings))
                {
                    document.Save(writer2);
                }
            }
            if (newline)
            {
                for (int i = 0; i < NewLine.Length; i++)
                {
                    stream.WriteByte((byte) NewLine[i]);
                }
            }
            stream.Position = 0L;
            return stream;
        }

        private MemoryStream CreateRequestData(LinkDescriptor binding, bool newline)
        {
            string originalString;
            MemoryStream stream = new MemoryStream();
            XmlWriter writer = XmlUtil.CreateXmlWriterAndWriteProcessingInstruction(stream, HttpProcessUtility.EncodingUtf8NoPreamble);
            EntityDescriptor descriptor = this.entityDescriptors[binding.Target];
            writer.WriteStartElement("uri", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            if (descriptor.Identity != null)
            {
                originalString = descriptor.GetResourceUri(this.baseUriWithSlash, false).OriginalString;
            }
            else
            {
                originalString = "$" + descriptor.ChangeOrder.ToString(CultureInfo.InvariantCulture);
            }
            writer.WriteValue(originalString);
            writer.WriteEndElement();
            writer.Flush();
            if (newline)
            {
                for (int i = 0; i < NewLine.Length; i++)
                {
                    stream.WriteByte((byte) NewLine[i]);
                }
            }
            stream.Position = 0L;
            return stream;
        }

        private void CreateRequestDataLinks(EntityDescriptor box, XmlWriter writer)
        {
            ClientType type = null;
            foreach (LinkDescriptor descriptor in this.RelatedLinks(box))
            {
                string str;
                descriptor.ContentGeneratedForSave = true;
                if (type == null)
                {
                    type = ClientType.Create(box.Entity.GetType());
                }
                if (type.GetProperty(descriptor.SourceProperty, false).CollectionType != null)
                {
                    str = "application/atom+xml;type=feed";
                }
                else
                {
                    str = "application/atom+xml;type=entry";
                }
                string str2 = Util.UriToString(this.entityDescriptors[descriptor.Target].EditLink);
                writer.WriteStartElement("link", "http://www.w3.org/2005/Atom");
                writer.WriteAttributeString("href", str2);
                writer.WriteAttributeString("rel", "http://schemas.microsoft.com/ado/2007/08/dataservices/related/" + descriptor.SourceProperty);
                writer.WriteAttributeString("type", str);
                writer.WriteEndElement();
            }
        }

        private Uri CreateRequestRelativeUri(LinkDescriptor binding)
        {
            if ((null != ClientType.Create(binding.Source.GetType()).GetProperty(binding.SourceProperty, false).CollectionType) && (EntityStates.Added != binding.State))
            {
                EntityDescriptor descriptor = this.entityDescriptors[binding.Target];
                Uri uri2 = this.BaseUriWithSlash.MakeRelativeUri(GenerateEditLinkUri(this.BaseUriWithSlash, binding.SourceProperty, descriptor.Entity));
                return Util.CreateUri("$links/" + uri2.OriginalString, UriKind.Relative);
            }
            return Util.CreateUri("$links/" + binding.SourceProperty, UriKind.Relative);
        }

        private Uri CreateRequestUri(EntityDescriptor sourceResource, LinkDescriptor binding) => 
            Util.CreateUri(sourceResource.GetResourceUri(this.baseUriWithSlash, false), this.CreateRequestRelativeUri(binding));

        public void DeleteLink(object source, string sourceProperty, object target)
        {
            bool flag = this.EnsureRelatable(source, sourceProperty, target, EntityStates.Deleted);
            LinkDescriptor descriptor = null;
            LinkDescriptor key = new LinkDescriptor(source, sourceProperty, target);
            if (this.bindings.TryGetValue(key, out descriptor) && (EntityStates.Added == descriptor.State))
            {
                this.DetachExistingLink(descriptor, false);
            }
            else
            {
                if (flag)
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_NoRelationWithInsertEnd);
                }
                if (descriptor == null)
                {
                    this.bindings.Add(key, key);
                    descriptor = key;
                }
                if (EntityStates.Deleted != descriptor.State)
                {
                    descriptor.State = EntityStates.Deleted;
                    this.IncrementChange(descriptor);
                }
            }
        }

        public void DeleteObject(object entity)
        {
            Util.CheckArgumentNull<object>(entity, "entity");
            EntityDescriptor descriptor = null;
            if (!this.entityDescriptors.TryGetValue(entity, out descriptor))
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_EntityNotContained);
            }
            EntityStates state = descriptor.State;
            if (EntityStates.Added == state)
            {
                this.DetachResource(descriptor);
            }
            else if (EntityStates.Deleted != state)
            {
                descriptor.State = EntityStates.Deleted;
                this.IncrementChange(descriptor);
            }
        }

        public bool Detach(object entity)
        {
            Util.CheckArgumentNull<object>(entity, "entity");
            EntityDescriptor descriptor = null;
            return (this.entityDescriptors.TryGetValue(entity, out descriptor) && this.DetachResource(descriptor));
        }

        private void DetachExistingLink(LinkDescriptor existingLink, bool targetDelete)
        {
            if (existingLink.Target != null)
            {
                EntityDescriptor descriptor = this.entityDescriptors[existingLink.Target];
                if (descriptor.IsDeepInsert && !targetDelete)
                {
                    EntityDescriptor parentForInsert = descriptor.ParentForInsert;
                    if (object.ReferenceEquals(descriptor.ParentEntity, existingLink.Source) && ((parentForInsert.State != EntityStates.Deleted) || (parentForInsert.State != EntityStates.Detached)))
                    {
                        throw new InvalidOperationException(System.Data.Services.Client.Strings.Context_ChildResourceExists);
                    }
                }
            }
            if (this.bindings.Remove(existingLink))
            {
                existingLink.State = EntityStates.Detached;
            }
        }

        public bool DetachLink(object source, string sourceProperty, object target)
        {
            LinkDescriptor descriptor;
            Util.CheckArgumentNull<object>(source, "source");
            Util.CheckArgumentNotEmpty(sourceProperty, "sourceProperty");
            LinkDescriptor key = new LinkDescriptor(source, sourceProperty, target);
            if (!this.bindings.TryGetValue(key, out descriptor))
            {
                return false;
            }
            this.DetachExistingLink(descriptor, false);
            return true;
        }

        private LinkDescriptor DetachReferenceLink(object source, string sourceProperty, object target, System.Data.Services.Client.MergeOption linkMerge)
        {
            LinkDescriptor existingLink = this.GetLinks(source, sourceProperty).FirstOrDefault<LinkDescriptor>();
            if (existingLink != null)
            {
                if (((target == existingLink.Target) || (linkMerge == System.Data.Services.Client.MergeOption.AppendOnly)) || ((System.Data.Services.Client.MergeOption.PreserveChanges == linkMerge) && (EntityStates.Modified == existingLink.State)))
                {
                    return existingLink;
                }
                this.DetachExistingLink(existingLink, false);
            }
            return null;
        }

        private bool DetachResource(EntityDescriptor resource)
        {
            foreach (LinkDescriptor descriptor in this.bindings.Values.Where<LinkDescriptor>(new Func<LinkDescriptor, bool>(resource.IsRelatedEntity)).ToList<LinkDescriptor>())
            {
                this.DetachExistingLink(descriptor, (descriptor.Target == resource.Entity) && (resource.State == EntityStates.Added));
            }
            resource.ChangeOrder = uint.MaxValue;
            resource.State = EntityStates.Detached;
            this.entityDescriptors.Remove(resource.Entity);
            this.DetachResourceIdentity(resource);
            return true;
        }

        private void DetachResourceIdentity(EntityDescriptor resource)
        {
            EntityDescriptor descriptor = null;
            if (((resource.Identity != null) && this.identityToDescriptor.TryGetValue(resource.Identity, out descriptor)) && object.ReferenceEquals(descriptor, resource))
            {
                this.identityToDescriptor.Remove(resource.Identity);
            }
        }

        public IEnumerable<TElement> EndExecute<TElement>(IAsyncResult asyncResult)
        {
            Util.CheckArgumentNull<IAsyncResult>(asyncResult, "asyncResult");
            return DataServiceRequest.EndExecute<TElement>(this, this, asyncResult);
        }

        public DataServiceResponse EndExecuteBatch(IAsyncResult asyncResult) => 
            BaseAsyncResult.EndExecute<SaveResult>(this, "ExecuteBatch", asyncResult).EndRequest();

        public DataServiceStreamResponse EndGetReadStream(IAsyncResult asyncResult) => 
            BaseAsyncResult.EndExecute<GetReadStreamResult>(this, "GetReadStream", asyncResult).End();

        public QueryOperationResponse EndLoadProperty(IAsyncResult asyncResult) => 
            BaseAsyncResult.EndExecute<LoadPropertyResult>(this, "LoadProperty", asyncResult).LoadProperty();

        public DataServiceResponse EndSaveChanges(IAsyncResult asyncResult)
        {
            DataServiceResponse response = BaseAsyncResult.EndExecute<SaveResult>(this, "SaveChanges", asyncResult).EndRequest();
            if (this.ChangesSaved != null)
            {
                this.ChangesSaved(this, new SaveChangesEventArgs(response));
            }
            return response;
        }

        private EntityDescriptor EnsureContained(object resource, string parameterName)
        {
            Util.CheckArgumentNull<object>(resource, parameterName);
            EntityDescriptor descriptor = null;
            if (!this.entityDescriptors.TryGetValue(resource, out descriptor))
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_EntityNotContained);
            }
            return descriptor;
        }

        private void EnsureIdentityToResource()
        {
            if (this.identityToDescriptor == null)
            {
                Interlocked.CompareExchange<Dictionary<string, EntityDescriptor>>(ref this.identityToDescriptor, new Dictionary<string, EntityDescriptor>(EqualityComparer<string>.Default), null);
            }
        }

        private bool EnsureRelatable(object source, string sourceProperty, object target, EntityStates state)
        {
            EntityDescriptor descriptor = this.EnsureContained(source, "source");
            EntityDescriptor descriptor2 = null;
            if ((target != null) || ((EntityStates.Modified != state) && (EntityStates.Unchanged != state)))
            {
                descriptor2 = this.EnsureContained(target, "target");
            }
            Util.CheckArgumentNotEmpty(sourceProperty, "sourceProperty");
            ClientType.ClientProperty property = ClientType.Create(source.GetType()).GetProperty(sourceProperty, false);
            if (property.IsKnownType)
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_RelationNotRefOrCollection);
            }
            if (((EntityStates.Unchanged == state) && (target == null)) && (property.CollectionType != null))
            {
                descriptor2 = this.EnsureContained(target, "target");
            }
            if (((EntityStates.Added == state) || (EntityStates.Deleted == state)) && (property.CollectionType == null))
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_AddLinkCollectionOnly);
            }
            if ((EntityStates.Modified == state) && (property.CollectionType != null))
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_SetLinkReferenceOnly);
            }
            ClientType type = ClientType.Create(property.CollectionType ?? property.PropertyType);
            if ((target != null) && !type.ElementType.IsInstanceOfType(target))
            {
                throw System.Data.Services.Client.Error.Argument(System.Data.Services.Client.Strings.Context_RelationNotRefOrCollection, "target");
            }
            if (((EntityStates.Added == state) || (EntityStates.Unchanged == state)) && ((descriptor.State == EntityStates.Deleted) || ((descriptor2 != null) && (descriptor2.State == EntityStates.Deleted))))
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_NoRelationWithDeleteEnd);
            }
            if (((EntityStates.Deleted == state) || (EntityStates.Unchanged == state)) && ((descriptor.State == EntityStates.Added) || ((descriptor2 != null) && (descriptor2.State == EntityStates.Added))))
            {
                if (EntityStates.Deleted != state)
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_NoRelationWithInsertEnd);
                }
                return true;
            }
            return false;
        }

        public QueryOperationResponse<T> Execute<T>(DataServiceQueryContinuation<T> continuation)
        {
            Util.CheckArgumentNull<DataServiceQueryContinuation<T>>(continuation, "continuation");
            QueryComponents queryComponents = continuation.CreateQueryComponents();
            DataServiceRequest request = new DataServiceRequest<T>(queryComponents, continuation.Plan);
            return request.Execute<T>(this, queryComponents);
        }

        public IEnumerable<TElement> Execute<TElement>(Uri requestUri)
        {
            requestUri = Util.CreateUri(this.baseUriWithSlash, requestUri);
            QueryComponents queryComponents = new QueryComponents(requestUri, Util.DataServiceVersionEmpty, typeof(TElement), null, null);
            DataServiceRequest request = new DataServiceRequest<TElement>(queryComponents, null);
            return request.Execute<TElement>(this, queryComponents);
        }

        public DataServiceResponse ExecuteBatch(params DataServiceRequest[] queries)
        {
            Util.CheckArgumentNotEmpty<DataServiceRequest>(queries, "queries");
            SaveResult result = new SaveResult(this, "ExecuteBatch", queries, SaveChangesOptions.Batch, null, null, false);
            result.BatchRequest(false);
            return result.EndRequest();
        }

        internal void FireReadingEntityEvent(object entity, XElement data)
        {
            ReadingWritingEntityEventArgs e = new ReadingWritingEntityEventArgs(entity, data);
            this.ReadingEntity(this, e);
        }

        private static Uri GenerateEditLinkUri(Uri baseUriWithSlash, string entitySetName, object entity)
        {
            ValidateEntityTypeHasKeys(entity);
            StringBuilder builder = new StringBuilder();
            builder.Append(baseUriWithSlash.AbsoluteUri);
            builder.Append(entitySetName);
            builder.Append("(");
            string str = string.Empty;
            ClientType.ClientProperty[] propertyArray = ClientType.Create(entity.GetType()).Properties.Where<ClientType.ClientProperty>(new Func<ClientType.ClientProperty, bool>(ClientType.ClientProperty.GetKeyProperty)).ToArray<ClientType.ClientProperty>();
            foreach (ClientType.ClientProperty property in propertyArray)
            {
                string str2;
                builder.Append(str);
                if (1 < propertyArray.Length)
                {
                    builder.Append(property.PropertyName).Append("=");
                }
                object obj2 = property.GetValue(entity);
                if (obj2 == null)
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Serializer_NullKeysAreNotSupported(property.PropertyName));
                }
                if (!ClientConvert.TryKeyPrimitiveToString(obj2, out str2))
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_CannotConvertKey(obj2));
                }
                builder.Append(Uri.EscapeDataString(str2));
                str = ",";
            }
            builder.Append(")");
            return Util.CreateUri(builder.ToString(), UriKind.Absolute);
        }

        public EntityDescriptor GetEntityDescriptor(object entity)
        {
            EntityDescriptor descriptor;
            Util.CheckArgumentNull<object>(entity, "entity");
            if (this.entityDescriptors.TryGetValue(entity, out descriptor))
            {
                return descriptor;
            }
            return null;
        }

        private static string GetEntityHttpMethod(EntityStates state, bool replaceOnUpdate)
        {
            EntityStates states = state;
            if (states == EntityStates.Added)
            {
                return "POST";
            }
            if (states != EntityStates.Deleted)
            {
                if (states != EntityStates.Modified)
                {
                    throw System.Data.Services.Client.Error.InternalError(InternalError.UnvalidatedEntityState);
                }
            }
            else
            {
                return "DELETE";
            }
            if (replaceOnUpdate)
            {
                return "PUT";
            }
            return "MERGE";
        }

        public LinkDescriptor GetLinkDescriptor(object source, string sourceProperty, object target)
        {
            LinkDescriptor descriptor;
            Util.CheckArgumentNull<object>(source, "source");
            Util.CheckArgumentNotEmpty(sourceProperty, "sourceProperty");
            Util.CheckArgumentNull<object>(target, "target");
            if (this.bindings.TryGetValue(new LinkDescriptor(source, sourceProperty, target), out descriptor))
            {
                return descriptor;
            }
            return null;
        }

        private static string GetLinkHttpMethod(LinkDescriptor link)
        {
            if (null == ClientType.Create(link.Source.GetType()).GetProperty(link.SourceProperty, false).CollectionType)
            {
                if (link.Target == null)
                {
                    return "DELETE";
                }
                return "PUT";
            }
            if (EntityStates.Deleted == link.State)
            {
                return "DELETE";
            }
            return "POST";
        }

        internal IEnumerable<LinkDescriptor> GetLinks(object source, string sourceProperty) => 
            (from o in this.bindings.Values
                where (o.Source == source) && (o.SourceProperty == sourceProperty)
                select o);

        public Uri GetMetadataUri() => 
            Util.CreateUri(this.baseUriWithSlash.OriginalString + "$metadata", UriKind.Absolute);

        public DataServiceStreamResponse GetReadStream(object entity)
        {
            DataServiceRequestArgs args = new DataServiceRequestArgs();
            return this.GetReadStream(entity, args);
        }

        public DataServiceStreamResponse GetReadStream(object entity, DataServiceRequestArgs args) => 
            this.CreateGetReadStreamResult(entity, args, null, null).Execute();

        public DataServiceStreamResponse GetReadStream(object entity, string acceptContentType)
        {
            Util.CheckArgumentNotEmpty(acceptContentType, "acceptContentType");
            DataServiceRequestArgs args = new DataServiceRequestArgs {
                AcceptContentType = acceptContentType
            };
            return this.GetReadStream(entity, args);
        }

        public Uri GetReadStreamUri(object entity) => 
            this.EnsureContained(entity, "entity").GetMediaResourceUri(this.baseUriWithSlash);

        internal static DataServiceClientException GetResponseText(Func<Stream> getResponseStream, HttpStatusCode statusCode)
        {
            string str = null;
            using (Stream stream = getResponseStream())
            {
                if ((stream != null) && stream.CanRead)
                {
                    str = new StreamReader(stream).ReadToEnd();
                }
            }
            if (string.IsNullOrEmpty(str))
            {
                str = statusCode.ToString();
            }
            return new DataServiceClientException(str, (int) statusCode);
        }

        internal string GetServerTypeName(EntityDescriptor descriptor)
        {
            if (this.resolveName == null)
            {
                return descriptor.ServerTypeName;
            }
            Type arg = descriptor.Entity.GetType();
            GeneratedCodeAttribute attribute = this.resolveName.Method.GetCustomAttributes(false).OfType<GeneratedCodeAttribute>().FirstOrDefault<GeneratedCodeAttribute>();
            if ((attribute == null) || (attribute.Tool != "System.Data.Services.Design"))
            {
                return (this.resolveName(arg) ?? descriptor.ServerTypeName);
            }
            return (descriptor.ServerTypeName ?? this.resolveName(arg));
        }

        internal static Exception HandleResponse(HttpStatusCode statusCode, string responseVersion, Func<Stream> getResponseStream, bool throwOnFailure)
        {
            InvalidOperationException responseText = null;
            if (!CanHandleResponseVersion(responseVersion))
            {
                responseText = System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_VersionNotSupported(responseVersion, SerializeSupportedVersions()));
            }
            if ((responseText == null) && !WebUtil.SuccessStatusCode(statusCode))
            {
                responseText = GetResponseText(getResponseStream, statusCode);
            }
            if ((responseText != null) && throwOnFailure)
            {
                throw responseText;
            }
            return responseText;
        }

        private void HandleResponseDelete(Descriptor entry)
        {
            if (EntityStates.Deleted != entry.State)
            {
                System.Data.Services.Client.Error.ThrowBatchUnexpectedContent(InternalError.EntityNotDeleted);
            }
            if (entry.IsResource)
            {
                EntityDescriptor resource = (EntityDescriptor) entry;
                this.DetachResource(resource);
            }
            else
            {
                this.DetachExistingLink((LinkDescriptor) entry, false);
            }
        }

        private static void HandleResponsePost(LinkDescriptor entry)
        {
            if ((EntityStates.Added != entry.State) && ((EntityStates.Modified != entry.State) || (entry.Target == null)))
            {
                System.Data.Services.Client.Error.ThrowBatchUnexpectedContent(InternalError.LinkNotAddedState);
            }
            entry.State = EntityStates.Unchanged;
        }

        private void HandleResponsePost(EntityDescriptor entry, MaterializeAtom materializer, Uri editLink, string etag)
        {
            if ((EntityStates.Added != entry.State) && (StreamStates.Added != entry.StreamState))
            {
                System.Data.Services.Client.Error.ThrowBatchUnexpectedContent(InternalError.EntityNotAddedState);
            }
            if (materializer == null)
            {
                string identity = Util.ReferenceIdentity(Util.UriToString(editLink));
                this.AttachIdentity(identity, null, editLink, entry.Entity, etag);
            }
            else
            {
                materializer.SetInsertingObject(entry.Entity);
                using (IEnumerator enumerator = materializer.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        object current = enumerator.Current;
                        if (entry.EditLink == null)
                        {
                            entry.EditLink = editLink;
                        }
                        if (entry.ETag == null)
                        {
                            entry.ETag = etag;
                        }
                    }
                }
            }
            foreach (LinkDescriptor descriptor in this.RelatedLinks(entry))
            {
                if (IncludeLinkState(descriptor.SaveResultWasProcessed) || (descriptor.SaveResultWasProcessed == EntityStates.Added))
                {
                    HandleResponsePost(descriptor);
                }
            }
        }

        private static void HandleResponsePut(Descriptor entry, string etag)
        {
            if (entry.IsResource)
            {
                EntityDescriptor descriptor = (EntityDescriptor) entry;
                if ((EntityStates.Modified != descriptor.State) && (StreamStates.Modified != descriptor.StreamState))
                {
                    System.Data.Services.Client.Error.ThrowBatchUnexpectedContent(InternalError.EntryNotModified);
                }
                if (descriptor.StreamState == StreamStates.Modified)
                {
                    descriptor.StreamETag = etag;
                    descriptor.StreamState = StreamStates.NoStream;
                }
                else
                {
                    descriptor.ETag = etag;
                    descriptor.State = EntityStates.Unchanged;
                }
            }
            else
            {
                LinkDescriptor descriptor2 = (LinkDescriptor) entry;
                if ((EntityStates.Added == entry.State) || (EntityStates.Modified == entry.State))
                {
                    descriptor2.State = EntityStates.Unchanged;
                }
                else if (EntityStates.Detached != entry.State)
                {
                    System.Data.Services.Client.Error.ThrowBatchUnexpectedContent(InternalError.LinkBadState);
                }
            }
        }

        private static bool IncludeLinkState(EntityStates x)
        {
            if (EntityStates.Modified != x)
            {
                return (EntityStates.Unchanged == x);
            }
            return true;
        }

        private void IncrementChange(Descriptor descriptor)
        {
            descriptor.ChangeOrder = ++this.nextChange;
        }

        internal EntityDescriptor InternalAttachEntityDescriptor(EntityDescriptor descriptor, bool failIfDuplicated)
        {
            EntityDescriptor descriptor2;
            EntityDescriptor descriptor3;
            this.EnsureIdentityToResource();
            this.entityDescriptors.TryGetValue(descriptor.Entity, out descriptor2);
            this.identityToDescriptor.TryGetValue(descriptor.Identity, out descriptor3);
            if (failIfDuplicated && (descriptor2 != null))
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_EntityAlreadyContained);
            }
            if (descriptor2 != descriptor3)
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_DifferentEntityAlreadyContained);
            }
            if (descriptor2 == null)
            {
                descriptor2 = descriptor;
                this.IncrementChange(descriptor);
                this.entityDescriptors.Add(descriptor.Entity, descriptor);
                this.identityToDescriptor.Add(descriptor.Identity, descriptor);
            }
            return descriptor2;
        }

        private static bool IsFlagSet(SaveChangesOptions options, SaveChangesOptions flag) => 
            ((options & flag) == flag);

        public QueryOperationResponse LoadProperty(object entity, string propertyName) => 
            this.LoadProperty(entity, propertyName, (Uri) null);

        public QueryOperationResponse LoadProperty(object entity, string propertyName, DataServiceQueryContinuation continuation)
        {
            LoadPropertyResult result = this.CreateLoadPropertyRequest(entity, propertyName, null, null, null, continuation);
            result.Execute();
            return result.LoadProperty();
        }

        public QueryOperationResponse<T> LoadProperty<T>(object entity, string propertyName, DataServiceQueryContinuation<T> continuation)
        {
            LoadPropertyResult result = this.CreateLoadPropertyRequest(entity, propertyName, null, null, null, continuation);
            result.Execute();
            return (QueryOperationResponse<T>) result.LoadProperty();
        }

        public QueryOperationResponse LoadProperty(object entity, string propertyName, Uri nextLinkUri)
        {
            LoadPropertyResult result = this.CreateLoadPropertyRequest(entity, propertyName, null, null, nextLinkUri, null);
            result.Execute();
            return result.LoadProperty();
        }

        private IEnumerable<LinkDescriptor> RelatedLinks(EntityDescriptor box)
        {
            foreach (LinkDescriptor iteratorVariable0 in this.bindings.Values)
            {
                if ((iteratorVariable0.Source == box.Entity) && (iteratorVariable0.Target != null))
                {
                    EntityDescriptor iteratorVariable1 = this.entityDescriptors[iteratorVariable0.Target];
                    if ((IncludeLinkState(iteratorVariable1.SaveResultWasProcessed) || ((iteratorVariable1.SaveResultWasProcessed == 0) && IncludeLinkState(iteratorVariable1.State))) || (((iteratorVariable1.Identity != null) && (iteratorVariable1.ChangeOrder < box.ChangeOrder)) && (((iteratorVariable1.SaveResultWasProcessed == 0) && (EntityStates.Added == iteratorVariable1.State)) || (EntityStates.Added == iteratorVariable1.SaveResultWasProcessed))))
                    {
                        yield return iteratorVariable0;
                    }
                }
            }
        }

        internal string ResolveNameFromType(Type type)
        {
            Func<Type, string> resolveName = this.ResolveName;
            return resolveName?.Invoke(type);
        }

        internal Type ResolveTypeFromName(string wireName, Type userType, bool checkAssignable)
        {
            Type type;
            if (string.IsNullOrEmpty(wireName))
            {
                return userType;
            }
            if (!ClientConvert.ToNamedType(wireName, out type))
            {
                type = null;
                Func<string, Type> resolveType = this.ResolveType;
                if (resolveType != null)
                {
                    type = resolveType(wireName);
                }
                if (type == null)
                {
                    type = ClientType.ResolveFromName(wireName, userType);
                }
                if ((checkAssignable && (type != null)) && !userType.IsAssignableFrom(type))
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Deserialize_Current(userType, type));
                }
            }
            return (type ?? userType);
        }

        public DataServiceResponse SaveChanges() => 
            this.SaveChanges(this.SaveChangesDefaultOptions);

        public DataServiceResponse SaveChanges(SaveChangesOptions options)
        {
            DataServiceResponse response = null;
            ValidateSaveChangesOptions(options);
            SaveResult result = new SaveResult(this, "SaveChanges", null, options, null, null, false);
            bool replaceOnUpdate = IsFlagSet(options, SaveChangesOptions.ReplaceOnUpdate);
            if (IsFlagSet(options, SaveChangesOptions.Batch))
            {
                result.BatchRequest(replaceOnUpdate);
            }
            else
            {
                result.BeginNextChange(replaceOnUpdate);
            }
            response = result.EndRequest();
            if (this.ChangesSaved != null)
            {
                this.ChangesSaved(this, new SaveChangesEventArgs(response));
            }
            return response;
        }

        private int SaveResultProcessed(Descriptor entry)
        {
            entry.SaveResultWasProcessed = entry.State;
            int num = 0;
            if (entry.IsResource && (EntityStates.Added == entry.State))
            {
                foreach (LinkDescriptor descriptor in this.RelatedLinks((EntityDescriptor) entry))
                {
                    if (descriptor.ContentGeneratedForSave)
                    {
                        descriptor.SaveResultWasProcessed = descriptor.State;
                        num++;
                    }
                }
            }
            return num;
        }

        private static string SerializeSupportedVersions()
        {
            StringBuilder builder = new StringBuilder("'").Append(Util.SupportedResponseVersions[0].ToString());
            for (int i = 1; i < Util.SupportedResponseVersions.Length; i++)
            {
                builder.Append("', '");
                builder.Append(Util.SupportedResponseVersions[i].ToString());
            }
            builder.Append("'");
            return builder.ToString();
        }

        public void SetLink(object source, string sourceProperty, object target)
        {
            this.EnsureRelatable(source, sourceProperty, target, EntityStates.Modified);
            LinkDescriptor key = this.DetachReferenceLink(source, sourceProperty, target, System.Data.Services.Client.MergeOption.NoTracking);
            if (key == null)
            {
                key = new LinkDescriptor(source, sourceProperty, target);
                this.bindings.Add(key, key);
            }
            if (EntityStates.Modified != key.State)
            {
                key.State = EntityStates.Modified;
                this.IncrementChange(key);
            }
        }

        public void SetSaveStream(object entity, Stream stream, bool closeStream, DataServiceRequestArgs args)
        {
            EntityDescriptor descriptor = this.EnsureContained(entity, "entity");
            Util.CheckArgumentNull<Stream>(stream, "stream");
            Util.CheckArgumentNull<DataServiceRequestArgs>(args, "args");
            ClientType type = ClientType.Create(entity.GetType());
            if (type.MediaDataMember != null)
            {
                throw new ArgumentException(System.Data.Services.Client.Strings.Context_SetSaveStreamOnMediaEntryProperty(type.ElementTypeName), "entity");
            }
            descriptor.SaveStream = new DataServiceSaveStream(stream, closeStream, args);
            switch (descriptor.State)
            {
                case EntityStates.Unchanged:
                case EntityStates.Modified:
                    descriptor.StreamState = StreamStates.Modified;
                    return;

                case EntityStates.Added:
                    descriptor.StreamState = StreamStates.Added;
                    return;
            }
            throw new DataServiceClientException(System.Data.Services.Client.Strings.DataServiceException_GeneralError);
        }

        public void SetSaveStream(object entity, Stream stream, bool closeStream, string contentType, string slug)
        {
            Util.CheckArgumentNull<string>(contentType, "contentType");
            Util.CheckArgumentNull<string>(slug, "slug");
            DataServiceRequestArgs args = new DataServiceRequestArgs {
                ContentType = contentType,
                Slug = slug
            };
            this.SetSaveStream(entity, stream, closeStream, args);
        }

        public bool TryGetEntity<TEntity>(Uri identity, out TEntity entity) where TEntity: class
        {
            EntityStates states;
            entity = default(TEntity);
            Util.CheckArgumentNull<Uri>(identity, "relativeUri");
            entity = (TEntity) this.TryGetEntity(Util.ReferenceIdentity(Util.UriToString(identity)), null, System.Data.Services.Client.MergeOption.AppendOnly, out states);
            return (null != ((TEntity) entity));
        }

        internal object TryGetEntity(string resourceUri, string etag, System.Data.Services.Client.MergeOption merger, out EntityStates state)
        {
            state = EntityStates.Detached;
            EntityDescriptor descriptor = null;
            if ((this.identityToDescriptor == null) || !this.identityToDescriptor.TryGetValue(resourceUri, out descriptor))
            {
                return null;
            }
            state = descriptor.State;
            if ((etag != null) && (merger != System.Data.Services.Client.MergeOption.AppendOnly))
            {
                descriptor.ETag = etag;
            }
            return descriptor.Entity;
        }

        public bool TryGetUri(object entity, out Uri identity)
        {
            identity = null;
            Util.CheckArgumentNull<object>(entity, "entity");
            EntityDescriptor descriptor = null;
            if (((this.identityToDescriptor != null) && this.entityDescriptors.TryGetValue(entity, out descriptor)) && ((descriptor.Identity != null) && object.ReferenceEquals(descriptor, this.identityToDescriptor[descriptor.Identity])))
            {
                string str = Util.DereferenceIdentity(descriptor.Identity);
                identity = Util.CreateUri(str, UriKind.Absolute);
            }
            return (null != identity);
        }

        public void UpdateObject(object entity)
        {
            Util.CheckArgumentNull<object>(entity, "entity");
            EntityDescriptor descriptor = null;
            if (!this.entityDescriptors.TryGetValue(entity, out descriptor))
            {
                throw System.Data.Services.Client.Error.Argument(System.Data.Services.Client.Strings.Context_EntityNotContained, "entity");
            }
            if (EntityStates.Unchanged == descriptor.State)
            {
                descriptor.State = EntityStates.Modified;
                this.IncrementChange(descriptor);
            }
        }

        private void ValidateEntitySetName(ref string entitySetName)
        {
            Util.CheckArgumentNotEmpty(entitySetName, "entitySetName");
            entitySetName = entitySetName.Trim(Util.ForwardSlash);
            Util.CheckArgumentNotEmpty(entitySetName, "entitySetName");
            Uri requestUri = Util.CreateUri(entitySetName, UriKind.RelativeOrAbsolute);
            if (requestUri.IsAbsoluteUri || !string.IsNullOrEmpty(Util.CreateUri(this.baseUriWithSlash, requestUri).GetComponents(UriComponents.Fragment | UriComponents.Query, UriFormat.SafeUnescaped)))
            {
                throw System.Data.Services.Client.Error.Argument(System.Data.Services.Client.Strings.Context_EntitySetName, "entitySetName");
            }
        }

        private static void ValidateEntityType(object entity)
        {
            Util.CheckArgumentNull<object>(entity, "entity");
            if (!ClientType.Create(entity.GetType()).IsEntityType)
            {
                throw System.Data.Services.Client.Error.Argument(System.Data.Services.Client.Strings.Content_EntityIsNotEntityType, "entity");
            }
        }

        private static void ValidateEntityTypeHasKeys(object entity)
        {
            Util.CheckArgumentNull<object>(entity, "entity");
            if (ClientType.Create(entity.GetType()).KeyCount <= 0)
            {
                throw System.Data.Services.Client.Error.Argument(System.Data.Services.Client.Strings.Content_EntityWithoutKey, "entity");
            }
        }

        private static void ValidateSaveChangesOptions(SaveChangesOptions options)
        {
            if ((options | (SaveChangesOptions.ReplaceOnUpdate | SaveChangesOptions.ContinueOnError | SaveChangesOptions.Batch)) != (SaveChangesOptions.ReplaceOnUpdate | SaveChangesOptions.ContinueOnError | SaveChangesOptions.Batch))
            {
                throw System.Data.Services.Client.Error.ArgumentOutOfRange("options");
            }
            if (IsFlagSet(options, SaveChangesOptions.ContinueOnError | SaveChangesOptions.Batch))
            {
                throw System.Data.Services.Client.Error.ArgumentOutOfRange("options");
            }
        }

        private void WriteContentProperties(XmlWriter writer, ClientType type, object resource, EpmSourcePathSegment currentSegment, out bool propertiesWritten)
        {
            propertiesWritten = false;
            using (IEnumerator<ClientType.ClientProperty> enumerator = type.Properties.GetEnumerator())
            {
                Func<EpmSourcePathSegment, bool> predicate = null;
                ClientType.ClientProperty property;
                while (enumerator.MoveNext())
                {
                    property = enumerator.Current;
                    if ((property != type.MediaDataMember) && ((type.MediaDataMember == null) || (type.MediaDataMember.MimeTypeProperty != property)))
                    {
                        object propertyValue = property.GetValue(resource);
                        if (predicate == null)
                        {
                            predicate = s => s.PropertyName == property.PropertyName;
                        }
                        EpmSourcePathSegment segment = (currentSegment != null) ? currentSegment.SubProperties.SingleOrDefault<EpmSourcePathSegment>(predicate) : null;
                        if (property.IsKnownType)
                        {
                            if (((propertyValue == null) || (segment == null)) || segment.EpmInfo.Attribute.KeepInContent)
                            {
                                WriteContentProperty(writer, this.DataNamespace, property, propertyValue);
                                propertiesWritten = true;
                            }
                        }
                        else if (property.CollectionType == null)
                        {
                            ClientType type2 = ClientType.Create(property.PropertyType);
                            if (!type2.IsEntityType)
                            {
                                XElement element = new XElement(this.DataNamespace + property.PropertyName);
                                bool flag = false;
                                string str = this.ResolveNameFromType(type2.ElementType);
                                if (!string.IsNullOrEmpty(str))
                                {
                                    element.Add(new XAttribute("http://schemas.microsoft.com/ado/2007/08/dataservices/metadata" + "type", str));
                                }
                                if (propertyValue == null)
                                {
                                    element.Add(new XAttribute("http://schemas.microsoft.com/ado/2007/08/dataservices/metadata" + "null", "true"));
                                    flag = true;
                                }
                                else
                                {
                                    using (XmlWriter writer2 = element.CreateWriter())
                                    {
                                        this.WriteContentProperties(writer2, type2, propertyValue, segment, out flag);
                                    }
                                }
                                if (flag)
                                {
                                    element.WriteTo(writer);
                                    propertiesWritten = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void WriteContentProperty(XmlWriter writer, string namespaceName, ClientType.ClientProperty property, object propertyValue)
        {
            writer.WriteStartElement(property.PropertyName, namespaceName);
            string edmType = ClientConvert.GetEdmType(property.PropertyType);
            if (edmType != null)
            {
                writer.WriteAttributeString("type", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", edmType);
            }
            if (propertyValue == null)
            {
                writer.WriteAttributeString("null", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", "true");
                if (property.KeyProperty)
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Serializer_NullKeysAreNotSupported(property.PropertyName));
                }
            }
            else
            {
                string str2 = ClientConvert.ToString(propertyValue, false);
                if (str2.Length == 0)
                {
                    writer.WriteAttributeString("null", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", "false");
                }
                else
                {
                    if (char.IsWhiteSpace(str2[0]) || char.IsWhiteSpace(str2[str2.Length - 1]))
                    {
                        writer.WriteAttributeString("space", "http://www.w3.org/2000/xmlns/", "preserve");
                    }
                    writer.WriteValue(str2);
                }
            }
            writer.WriteEndElement();
        }

        private static void WriteOperationRequestHeaders(StreamWriter writer, string methodName, string uri, Version requestVersion)
        {
            writer.WriteLine("{0}: {1}", "Content-Type", "application/http");
            writer.WriteLine("{0}: {1}", "Content-Transfer-Encoding", "binary");
            writer.WriteLine();
            writer.WriteLine("{0} {1} {2}", methodName, uri, "HTTP/1.1");
            if ((requestVersion != Util.DataServiceVersion1) && (requestVersion != Util.DataServiceVersionEmpty))
            {
                writer.WriteLine("{0}: {1}{2}", "DataServiceVersion", requestVersion, ";NetFx");
            }
        }

        private static void WriteOperationResponseHeaders(StreamWriter writer, int statusCode)
        {
            writer.WriteLine("{0}: {1}", "Content-Type", "application/http");
            writer.WriteLine("{0}: {1}", "Content-Transfer-Encoding", "binary");
            writer.WriteLine();
            writer.WriteLine("{0} {1} {2}", "HTTP/1.1", statusCode, (HttpStatusCode) statusCode);
        }

        public bool ApplyingChanges
        {
            get => 
                this.applyingChanges;
            internal set
            {
                this.applyingChanges = value;
            }
        }

        public Uri BaseUri =>
            this.baseUri;

        internal Uri BaseUriWithSlash =>
            this.baseUriWithSlash;

        public ICredentials Credentials
        {
            get => 
                this.credentials;
            set
            {
                this.credentials = value;
            }
        }

        public string DataNamespace
        {
            get => 
                this.dataNamespace;
            set
            {
                Util.CheckArgumentNull<string>(value, "value");
                this.dataNamespace = value;
            }
        }

        public ReadOnlyCollection<EntityDescriptor> Entities =>
            (from d in this.entityDescriptors.Values
                orderby d.ChangeOrder
                select d).ToList<EntityDescriptor>().AsReadOnly();

        internal bool HasReadingEntityHandlers =>
            (this.ReadingEntity != null);

        public bool IgnoreMissingProperties
        {
            get => 
                this.ignoreMissingProperties;
            set
            {
                this.ignoreMissingProperties = value;
            }
        }

        public bool IgnoreResourceNotFoundException
        {
            get => 
                this.ignoreResourceNotFoundException;
            set
            {
                this.ignoreResourceNotFoundException = value;
            }
        }

        public ReadOnlyCollection<LinkDescriptor> Links =>
            (from l in this.bindings.Values
                orderby l.ChangeOrder
                select l).ToList<LinkDescriptor>().AsReadOnly();

        public System.Data.Services.Client.MergeOption MergeOption
        {
            get => 
                this.mergeOption;
            set
            {
                this.mergeOption = Util.CheckEnumerationValue(value, "MergeOption");
            }
        }

        public Func<Type, string> ResolveName
        {
            get => 
                this.resolveName;
            set
            {
                this.resolveName = value;
            }
        }

        public Func<string, Type> ResolveType
        {
            get => 
                this.resolveType;
            set
            {
                this.resolveType = value;
            }
        }

        public SaveChangesOptions SaveChangesDefaultOptions
        {
            get => 
                this.saveChangesDefaultOptions;
            set
            {
                ValidateSaveChangesOptions(value);
                this.saveChangesDefaultOptions = value;
            }
        }

        public int Timeout
        {
            get => 
                this.timeout;
            set
            {
                if (value < 0)
                {
                    throw System.Data.Services.Client.Error.ArgumentOutOfRange("Timeout");
                }
                this.timeout = value;
            }
        }

        public Uri TypeScheme
        {
            get => 
                this.typeScheme;
            set
            {
                Util.CheckArgumentNull<Uri>(value, "value");
                this.typeScheme = value;
            }
        }

        public bool UsePostTunneling
        {
            get => 
                this.postTunneling;
            set
            {
                this.postTunneling = value;
            }
        }


        internal class DataServiceSaveStream
        {
            private readonly DataServiceRequestArgs args;
            private readonly bool close;
            private readonly System.IO.Stream stream;

            internal DataServiceSaveStream(System.IO.Stream stream, bool close, DataServiceRequestArgs args)
            {
                this.stream = stream;
                this.close = close;
                this.args = args;
            }

            internal void Close()
            {
                if ((this.stream != null) && this.close)
                {
                    this.stream.Close();
                }
            }

            internal DataServiceRequestArgs Args =>
                this.args;

            internal System.IO.Stream Stream =>
                this.stream;
        }

        private class LoadPropertyResult : QueryResult
        {
            private readonly object entity;
            private readonly ProjectionPlan plan;
            private readonly string propertyName;

            internal LoadPropertyResult(object entity, string propertyName, DataServiceContext context, HttpWebRequest request, AsyncCallback callback, object state, DataServiceRequest dataServiceRequest, ProjectionPlan plan) : base(context, "LoadProperty", dataServiceRequest, request, callback, state)
            {
                this.entity = entity;
                this.propertyName = propertyName;
                this.plan = plan;
            }

            internal QueryOperationResponse LoadProperty()
            {
                MaterializeAtom results = null;
                QueryOperationResponse responseWithType;
                DataServiceContext source = (DataServiceContext) base.Source;
                ClientType type = ClientType.Create(this.entity.GetType());
                EntityDescriptor box = source.EnsureContained(this.entity, "entity");
                if (EntityStates.Added == box.State)
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_NoLoadWithInsertEnd);
                }
                ClientType.ClientProperty property = type.GetProperty(this.propertyName, false);
                Type elementType = property.CollectionType ?? property.NullablePropertyType;
                try
                {
                    if (type.MediaDataMember == property)
                    {
                        results = this.ReadPropertyFromRawData(property);
                    }
                    else
                    {
                        results = this.ReadPropertyFromAtom(box, property);
                    }
                    responseWithType = base.GetResponseWithType(results, elementType);
                }
                catch (InvalidOperationException exception)
                {
                    QueryOperationResponse response = base.GetResponseWithType(results, elementType);
                    if (response != null)
                    {
                        response.Error = exception;
                        throw new DataServiceQueryException(System.Data.Services.Client.Strings.DataServiceException_GeneralError, exception, response);
                    }
                    throw;
                }
                return responseWithType;
            }

            private static byte[] ReadByteArrayChunked(Stream responseStream)
            {
                byte[] buffer = null;
                using (MemoryStream stream = new MemoryStream())
                {
                    byte[] buffer2 = new byte[0x1000];
                    int count = 0;
                    int num2 = 0;
                    while (true)
                    {
                        count = responseStream.Read(buffer2, 0, buffer2.Length);
                        if (count <= 0)
                        {
                            break;
                        }
                        stream.Write(buffer2, 0, count);
                        num2 += count;
                    }
                    buffer = new byte[num2];
                    stream.Position = 0L;
                    count = stream.Read(buffer, 0, buffer.Length);
                }
                return buffer;
            }

            private static byte[] ReadByteArrayWithContentLength(Stream responseStream, int totalLength)
            {
                int num2;
                byte[] buffer = new byte[totalLength];
                for (int i = 0; i < totalLength; i += num2)
                {
                    num2 = responseStream.Read(buffer, i, totalLength - i);
                    if (num2 <= 0)
                    {
                        throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_UnexpectedZeroRawRead);
                    }
                }
                return buffer;
            }

            private MaterializeAtom ReadPropertyFromAtom(EntityDescriptor box, ClientType.ClientProperty property)
            {
                MaterializeAtom atom2;
                DataServiceContext source = (DataServiceContext) base.Source;
                bool applyingChanges = source.ApplyingChanges;
                try
                {
                    source.ApplyingChanges = true;
                    bool flag2 = EntityStates.Deleted == box.State;
                    Type type = property.CollectionType ?? property.NullablePropertyType;
                    ClientType type2 = ClientType.Create(type);
                    bool flag3 = false;
                    object entity = this.entity;
                    if (property.CollectionType != null)
                    {
                        entity = property.GetValue(this.entity);
                        if (entity == null)
                        {
                            flag3 = true;
                            if (BindingEntityInfo.IsDataServiceCollection(property.PropertyType))
                            {
                                object[] args = new object[2];
                                args[1] = TrackingMode.None;
                                entity = Activator.CreateInstance(WebUtil.GetDataServiceCollectionOfT(new Type[] { type }), args);
                            }
                            else
                            {
                                entity = Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { type }));
                            }
                        }
                    }
                    Type type3 = property.CollectionType ?? property.NullablePropertyType;
                    IList results = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { type3 }));
                    DataServiceQueryContinuation continuation = null;
                    using (MaterializeAtom atom = base.GetMaterializer(source, this.plan))
                    {
                        int num = 0;
                        foreach (object obj3 in atom)
                        {
                            results.Add(obj3);
                            num++;
                            property.SetValue(entity, obj3, this.propertyName, true);
                            if (((obj3 != null) && (MergeOption.NoTracking != atom.MergeOptionValue)) && type2.IsEntityType)
                            {
                                if (flag2)
                                {
                                    source.DeleteLink(this.entity, this.propertyName, obj3);
                                }
                                else
                                {
                                    source.AttachLink(this.entity, this.propertyName, obj3, atom.MergeOptionValue);
                                }
                            }
                        }
                        continuation = atom.GetContinuation(null);
                        Util.SetNextLinkForCollection(entity, continuation);
                    }
                    if (flag3)
                    {
                        property.SetValue(this.entity, entity, this.propertyName, false);
                    }
                    atom2 = MaterializeAtom.CreateWrapper(results, continuation);
                }
                finally
                {
                    source.ApplyingChanges = applyingChanges;
                }
                return atom2;
            }

            private MaterializeAtom ReadPropertyFromRawData(ClientType.ClientProperty property)
            {
                MaterializeAtom atom;
                DataServiceContext source = (DataServiceContext) base.Source;
                bool applyingChanges = source.ApplyingChanges;
                try
                {
                    source.ApplyingChanges = true;
                    string mime = null;
                    Encoding encoding = null;
                    Type type = property.CollectionType ?? property.NullablePropertyType;
                    IList results = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { type }));
                    HttpProcessUtility.ReadContentType(base.ContentType, out mime, out encoding);
                    using (Stream stream = base.GetResponseStream())
                    {
                        if (property.PropertyType == typeof(byte[]))
                        {
                            int contentLength = (int) base.ContentLength;
                            byte[] buffer = null;
                            if (contentLength >= 0)
                            {
                                buffer = ReadByteArrayWithContentLength(stream, contentLength);
                            }
                            else
                            {
                                buffer = ReadByteArrayChunked(stream);
                            }
                            results.Add(buffer);
                            property.SetValue(this.entity, buffer, this.propertyName, false);
                        }
                        else
                        {
                            StreamReader reader = new StreamReader(stream, encoding);
                            object obj2 = (property.PropertyType == typeof(string)) ? reader.ReadToEnd() : ClientConvert.ChangeType(reader.ReadToEnd(), property.PropertyType);
                            results.Add(obj2);
                            property.SetValue(this.entity, obj2, this.propertyName, false);
                        }
                    }
                    if (property.MimeTypeProperty != null)
                    {
                        property.MimeTypeProperty.SetValue(this.entity, mime, null, false);
                    }
                    atom = MaterializeAtom.CreateWrapper(results);
                }
                finally
                {
                    source.ApplyingChanges = applyingChanges;
                }
                return atom;
            }
        }

        private class SaveResult : BaseAsyncResult
        {
            private readonly string batchBoundary;
            private HttpWebResponse batchResponse;
            private byte[] buildBatchBuffer;
            private StreamWriter buildBatchWriter;
            private readonly List<Descriptor> ChangedEntries;
            private int changesCompleted;
            private string changesetBoundary;
            private bool changesetStarted;
            private readonly DataServiceContext Context;
            private long copiedContentLength;
            private int entryIndex;
            private readonly bool executeAsync;
            private Stream httpWebResponseStream;
            private Stream mediaResourceRequestStream;
            private readonly SaveChangesOptions options;
            private bool processingMediaLinkEntry;
            private bool processingMediaLinkEntryPut;
            private readonly DataServiceRequest[] Queries;
            private PerRequest request;
            private BatchStream responseBatchStream;
            private readonly List<OperationResponse> Responses;
            private System.Data.Services.Client.DataServiceResponse service;

            internal SaveResult(DataServiceContext context, string method, DataServiceRequest[] queries, SaveChangesOptions options, AsyncCallback callback, object state, bool async) : base(context, method, callback, state)
            {
                this.entryIndex = -1;
                this.executeAsync = async;
                this.Context = context;
                this.Queries = queries;
                this.options = options;
                this.Responses = new List<OperationResponse>();
                if (queries == null)
                {
                    this.ChangedEntries = (from o in context.entityDescriptors.Values.Cast<Descriptor>().Union<Descriptor>(context.bindings.Values.Cast<Descriptor>())
                        where o.IsModified && (o.ChangeOrder != uint.MaxValue)
                        orderby o.ChangeOrder
                        select o).ToList<Descriptor>();
                    foreach (Descriptor descriptor in this.ChangedEntries)
                    {
                        descriptor.ContentGeneratedForSave = false;
                        descriptor.SaveResultWasProcessed = 0;
                        descriptor.SaveError = null;
                        if (!descriptor.IsResource)
                        {
                            object target = ((LinkDescriptor) descriptor).Target;
                            if (target != null)
                            {
                                Descriptor descriptor2 = context.entityDescriptors[target];
                                if (EntityStates.Unchanged == descriptor2.State)
                                {
                                    descriptor2.ContentGeneratedForSave = false;
                                    descriptor2.SaveResultWasProcessed = 0;
                                    descriptor2.SaveError = null;
                                }
                            }
                        }
                    }
                }
                else
                {
                    this.ChangedEntries = new List<Descriptor>();
                }
                if (DataServiceContext.IsFlagSet(options, SaveChangesOptions.Batch))
                {
                    this.batchBoundary = "batch_" + Guid.NewGuid().ToString();
                }
                else
                {
                    this.batchBoundary = "batchresponse_" + Guid.NewGuid().ToString();
                    this.DataServiceResponse = new System.Data.Services.Client.DataServiceResponse(null, -1, this.Responses, false);
                }
            }

            private void AsyncEndGetRequestStream(IAsyncResult asyncResult)
            {
                PerRequest request = (asyncResult == null) ? null : (asyncResult.AsyncState as PerRequest);
                try
                {
                    CompleteCheck(request, InternalError.InvalidEndGetRequestCompleted);
                    request.RequestCompletedSynchronously &= asyncResult.CompletedSynchronously;
                    EqualRefCheck(this.request, request, InternalError.InvalidEndGetRequestStream);
                    Stream stream = Util.NullCheck<Stream>(Util.NullCheck<HttpWebRequest>(request.Request, InternalError.InvalidEndGetRequestStreamRequest).EndGetRequestStream(asyncResult), InternalError.InvalidEndGetRequestStreamStream);
                    request.RequestStream = stream;
                    PerRequest.ContentStream requestContentStream = request.RequestContentStream;
                    Util.NullCheck<PerRequest.ContentStream>(requestContentStream, InternalError.InvalidEndGetRequestStreamContent);
                    Util.NullCheck<Stream>(requestContentStream.Stream, InternalError.InvalidEndGetRequestStreamContent);
                    if (requestContentStream.IsKnownMemoryStream)
                    {
                        MemoryStream stream3 = requestContentStream.Stream as MemoryStream;
                        byte[] buffer = stream3.GetBuffer();
                        int position = (int) stream3.Position;
                        int num2 = ((int) stream3.Length) - position;
                        if ((buffer == null) || (num2 == 0))
                        {
                            System.Data.Services.Client.Error.ThrowInternalError(InternalError.InvalidEndGetRequestStreamContentLength);
                        }
                    }
                    request.RequestContentBufferValidLength = -1;
                    Stream stream1 = requestContentStream.Stream;
                    asyncResult = BaseAsyncResult.InvokeAsync(new BaseAsyncResult.Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(stream1.BeginRead), request.RequestContentBuffer, 0, request.RequestContentBuffer.Length, new AsyncCallback(this.AsyncRequestContentEndRead), request);
                    request.RequestCompletedSynchronously &= asyncResult.CompletedSynchronously;
                }
                catch (Exception exception)
                {
                    if (this.HandleFailure(request, exception))
                    {
                        throw;
                    }
                }
                finally
                {
                    this.HandleCompleted(request);
                }
            }

            private void AsyncEndGetResponse(IAsyncResult asyncResult)
            {
                PerRequest request = (asyncResult == null) ? null : (asyncResult.AsyncState as PerRequest);
                try
                {
                    CompleteCheck(request, InternalError.InvalidEndGetResponseCompleted);
                    request.RequestCompletedSynchronously &= asyncResult.CompletedSynchronously;
                    EqualRefCheck(this.request, request, InternalError.InvalidEndGetResponse);
                    HttpWebRequest request2 = Util.NullCheck<HttpWebRequest>(request.Request, InternalError.InvalidEndGetResponseRequest);
                    HttpWebResponse response = null;
                    try
                    {
                        response = (HttpWebResponse) request2.EndGetResponse(asyncResult);
                    }
                    catch (WebException exception)
                    {
                        response = (HttpWebResponse) exception.Response;
                        if (response == null)
                        {
                            throw;
                        }
                    }
                    request.HttpWebResponse = Util.NullCheck<HttpWebResponse>(response, InternalError.InvalidEndGetResponseResponse);
                    if (!DataServiceContext.IsFlagSet(this.options, SaveChangesOptions.Batch))
                    {
                        this.HandleOperationResponse(response);
                    }
                    this.copiedContentLength = 0L;
                    Stream responseStream = response.GetResponseStream();
                    request.ResponseStream = responseStream;
                    if ((responseStream != null) && responseStream.CanRead)
                    {
                        if (this.buildBatchWriter != null)
                        {
                            this.buildBatchWriter.Flush();
                        }
                        if (this.buildBatchBuffer == null)
                        {
                            this.buildBatchBuffer = new byte[0x1f40];
                        }
                        do
                        {
                            asyncResult = BaseAsyncResult.InvokeAsync(new BaseAsyncResult.Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(responseStream.BeginRead), this.buildBatchBuffer, 0, this.buildBatchBuffer.Length, new AsyncCallback(this.AsyncEndRead), request);
                            request.RequestCompletedSynchronously &= asyncResult.CompletedSynchronously;
                        }
                        while (((asyncResult.CompletedSynchronously && !request.RequestCompleted) && !base.IsCompletedInternally) && responseStream.CanRead);
                    }
                    else
                    {
                        request.SetComplete();
                        if (!base.IsCompletedInternally)
                        {
                            this.SaveNextChange(request);
                        }
                    }
                }
                catch (Exception exception2)
                {
                    if (this.HandleFailure(request, exception2))
                    {
                        throw;
                    }
                }
                finally
                {
                    this.HandleCompleted(request);
                }
            }

            private void AsyncEndRead(IAsyncResult asyncResult)
            {
                PerRequest asyncState = asyncResult.AsyncState as PerRequest;
                int count = 0;
                try
                {
                    CompleteCheck(asyncState, InternalError.InvalidEndReadCompleted);
                    asyncState.RequestCompletedSynchronously &= asyncResult.CompletedSynchronously;
                    EqualRefCheck(this.request, asyncState, InternalError.InvalidEndRead);
                    Stream stream = Util.NullCheck<Stream>(asyncState.ResponseStream, InternalError.InvalidEndReadStream);
                    count = stream.EndRead(asyncResult);
                    if (0 < count)
                    {
                        Util.NullCheck<Stream>(this.httpWebResponseStream, InternalError.InvalidEndReadCopy).Write(this.buildBatchBuffer, 0, count);
                        this.copiedContentLength += count;
                        if (!asyncResult.CompletedSynchronously && stream.CanRead)
                        {
                            do
                            {
                                asyncResult = BaseAsyncResult.InvokeAsync(new BaseAsyncResult.Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(stream.BeginRead), this.buildBatchBuffer, 0, this.buildBatchBuffer.Length, new AsyncCallback(this.AsyncEndRead), asyncState);
                                asyncState.RequestCompletedSynchronously &= asyncResult.CompletedSynchronously;
                                if ((!asyncResult.CompletedSynchronously || asyncState.RequestCompleted) || base.IsCompletedInternally)
                                {
                                    return;
                                }
                            }
                            while (stream.CanRead);
                        }
                    }
                    else
                    {
                        asyncState.SetComplete();
                        if (!base.IsCompletedInternally)
                        {
                            this.SaveNextChange(asyncState);
                        }
                    }
                }
                catch (Exception exception)
                {
                    if (this.HandleFailure(asyncState, exception))
                    {
                        throw;
                    }
                }
                finally
                {
                    this.HandleCompleted(asyncState);
                }
            }

            private void AsyncEndWrite(IAsyncResult asyncResult)
            {
                PerRequest request = (asyncResult == null) ? null : (asyncResult.AsyncState as PerRequest);
                try
                {
                    CompleteCheck(request, InternalError.InvalidEndWriteCompleted);
                    request.RequestCompletedSynchronously &= asyncResult.CompletedSynchronously;
                    EqualRefCheck(this.request, request, InternalError.InvalidEndWrite);
                    PerRequest.ContentStream requestContentStream = request.RequestContentStream;
                    Util.NullCheck<PerRequest.ContentStream>(requestContentStream, InternalError.InvalidEndWriteStream);
                    Util.NullCheck<Stream>(requestContentStream.Stream, InternalError.InvalidEndWriteStream);
                    Util.NullCheck<Stream>(request.RequestStream, InternalError.InvalidEndWriteStream).EndWrite(asyncResult);
                    if (!asyncResult.CompletedSynchronously)
                    {
                        Stream stream = requestContentStream.Stream;
                        asyncResult = BaseAsyncResult.InvokeAsync(new BaseAsyncResult.Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(stream.BeginRead), request.RequestContentBuffer, 0, request.RequestContentBuffer.Length, new AsyncCallback(this.AsyncRequestContentEndRead), request);
                        request.RequestCompletedSynchronously &= asyncResult.CompletedSynchronously;
                    }
                }
                catch (Exception exception)
                {
                    if (this.HandleFailure(request, exception))
                    {
                        throw;
                    }
                }
                finally
                {
                    this.HandleCompleted(request);
                }
            }

            private void AsyncRequestContentEndRead(IAsyncResult asyncResult)
            {
                PerRequest request = (asyncResult == null) ? null : (asyncResult.AsyncState as PerRequest);
                try
                {
                    CompleteCheck(request, InternalError.InvalidEndReadCompleted);
                    request.RequestCompletedSynchronously &= asyncResult.CompletedSynchronously;
                    EqualRefCheck(this.request, request, InternalError.InvalidEndRead);
                    PerRequest.ContentStream requestContentStream = request.RequestContentStream;
                    Util.NullCheck<PerRequest.ContentStream>(requestContentStream, InternalError.InvalidEndReadStream);
                    Util.NullCheck<Stream>(requestContentStream.Stream, InternalError.InvalidEndReadStream);
                    Stream stream2 = Util.NullCheck<Stream>(request.RequestStream, InternalError.InvalidEndReadStream);
                    int num = requestContentStream.Stream.EndRead(asyncResult);
                    if (0 < num)
                    {
                        bool flag = request.RequestContentBufferValidLength == -1;
                        request.RequestContentBufferValidLength = num;
                        if (!asyncResult.CompletedSynchronously || flag)
                        {
                            do
                            {
                                asyncResult = BaseAsyncResult.InvokeAsync(new BaseAsyncResult.Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(stream2.BeginWrite), request.RequestContentBuffer, 0, request.RequestContentBufferValidLength, new AsyncCallback(this.AsyncEndWrite), request);
                                request.RequestCompletedSynchronously &= asyncResult.CompletedSynchronously;
                                if ((asyncResult.CompletedSynchronously && !request.RequestCompleted) && !base.IsCompletedInternally)
                                {
                                    Stream stream = requestContentStream.Stream;
                                    asyncResult = BaseAsyncResult.InvokeAsync(new BaseAsyncResult.Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(stream.BeginRead), request.RequestContentBuffer, 0, request.RequestContentBuffer.Length, new AsyncCallback(this.AsyncRequestContentEndRead), request);
                                    request.RequestCompletedSynchronously &= asyncResult.CompletedSynchronously;
                                }
                            }
                            while (((asyncResult.CompletedSynchronously && !request.RequestCompleted) && !base.IsCompletedInternally) && (request.RequestContentBufferValidLength > 0));
                        }
                    }
                    else
                    {
                        request.RequestContentBufferValidLength = 0;
                        request.RequestStream = null;
                        stream2.Close();
                        HttpWebRequest local1 = Util.NullCheck<HttpWebRequest>(request.Request, InternalError.InvalidEndWriteRequest);
                        asyncResult = BaseAsyncResult.InvokeAsync(new Func<AsyncCallback, object, IAsyncResult>(local1.BeginGetResponse), new AsyncCallback(this.AsyncEndGetResponse), request);
                        request.RequestCompletedSynchronously &= asyncResult.CompletedSynchronously;
                    }
                }
                catch (Exception exception)
                {
                    if (this.HandleFailure(request, exception))
                    {
                        throw;
                    }
                }
                finally
                {
                    this.HandleCompleted(request);
                }
            }

            internal void BatchBeginRequest(bool replaceOnUpdate)
            {
                PerRequest state = null;
                try
                {
                    MemoryStream memory = this.GenerateBatchRequest(replaceOnUpdate);
                    if (memory != null)
                    {
                        HttpWebRequest request2 = this.CreateBatchRequest(memory);
                        base.Abortable = request2;
                        this.request = state = new PerRequest();
                        state.Request = request2;
                        state.RequestContentStream = new PerRequest.ContentStream(memory, true);
                        this.httpWebResponseStream = new MemoryStream();
                        IAsyncResult result = BaseAsyncResult.InvokeAsync(new Func<AsyncCallback, object, IAsyncResult>(request2.BeginGetRequestStream), new AsyncCallback(this.AsyncEndGetRequestStream), state);
                        state.RequestCompletedSynchronously &= result.CompletedSynchronously;
                    }
                }
                catch (Exception exception)
                {
                    this.HandleFailure(state, exception);
                    throw;
                }
                finally
                {
                    this.HandleCompleted(state);
                }
            }

            internal void BatchRequest(bool replaceOnUpdate)
            {
                MemoryStream memory = this.GenerateBatchRequest(replaceOnUpdate);
                if ((memory != null) && (0L < memory.Length))
                {
                    HttpWebRequest request = this.CreateBatchRequest(memory);
                    using (Stream stream2 = request.GetRequestStream())
                    {
                        byte[] buffer = memory.GetBuffer();
                        int position = (int) memory.Position;
                        int count = ((int) memory.Length) - position;
                        stream2.Write(buffer, position, count);
                    }
                    HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                    this.batchResponse = response;
                    if (response != null)
                    {
                        this.httpWebResponseStream = response.GetResponseStream();
                    }
                }
            }

            internal void BeginNextChange(bool replaceOnUpdate)
            {
                PerRequest state = null;
                IAsyncResult result = null;
                do
                {
                    HttpWebRequest request2 = null;
                    HttpWebResponse response = null;
                    try
                    {
                        if (this.request != null)
                        {
                            base.SetCompleted();
                            System.Data.Services.Client.Error.ThrowInternalError(InternalError.InvalidBeginNextChange);
                        }
                        base.Abortable = request2 = this.CreateNextRequest(replaceOnUpdate);
                        if ((request2 != null) || (this.entryIndex < this.ChangedEntries.Count))
                        {
                            if (!this.ChangedEntries[this.entryIndex].ContentGeneratedForSave)
                            {
                                PerRequest.ContentStream stream = this.CreateChangeData(this.entryIndex, false);
                                if (this.executeAsync)
                                {
                                    this.request = state = new PerRequest();
                                    state.Request = request2;
                                    if ((stream == null) || (stream.Stream == null))
                                    {
                                        result = BaseAsyncResult.InvokeAsync(new Func<AsyncCallback, object, IAsyncResult>(request2.BeginGetResponse), new AsyncCallback(this.AsyncEndGetResponse), state);
                                    }
                                    else
                                    {
                                        if (stream.IsKnownMemoryStream)
                                        {
                                            request2.ContentLength = stream.Stream.Length - stream.Stream.Position;
                                        }
                                        state.RequestContentStream = stream;
                                        result = BaseAsyncResult.InvokeAsync(new Func<AsyncCallback, object, IAsyncResult>(request2.BeginGetRequestStream), new AsyncCallback(this.AsyncEndGetRequestStream), state);
                                    }
                                    state.RequestCompletedSynchronously &= result.CompletedSynchronously;
                                    base.CompletedSynchronously &= result.CompletedSynchronously;
                                }
                                else
                                {
                                    if ((stream != null) && (stream.Stream != null))
                                    {
                                        if (stream.IsKnownMemoryStream)
                                        {
                                            request2.ContentLength = stream.Stream.Length - stream.Stream.Position;
                                        }
                                        using (Stream stream2 = request2.GetRequestStream())
                                        {
                                            int num;
                                            byte[] buffer = new byte[0x10000];
                                            do
                                            {
                                                num = stream.Stream.Read(buffer, 0, buffer.Length);
                                                if (num > 0)
                                                {
                                                    stream2.Write(buffer, 0, num);
                                                }
                                            }
                                            while (num > 0);
                                        }
                                    }
                                    response = (HttpWebResponse) request2.GetResponse();
                                    if (!this.processingMediaLinkEntry)
                                    {
                                        this.changesCompleted++;
                                    }
                                    this.HandleOperationResponse(response);
                                    this.HandleOperationResponseData(response);
                                    this.HandleOperationEnd();
                                    this.request = null;
                                }
                            }
                        }
                        else
                        {
                            base.SetCompleted();
                            if (base.CompletedSynchronously)
                            {
                                this.HandleCompleted(state);
                            }
                        }
                    }
                    catch (InvalidOperationException exception)
                    {
                        WebUtil.GetHttpWebResponse(exception, ref response);
                        this.HandleOperationException(exception, response);
                        this.HandleCompleted(state);
                    }
                    finally
                    {
                        if (response != null)
                        {
                            response.Close();
                        }
                    }
                }
                while (((state == null) || ((state.RequestCompleted && (result != null)) && result.CompletedSynchronously)) && !base.IsCompletedInternally);
            }

            private HttpWebRequest CheckAndProcessMediaEntryPost(EntityDescriptor entityDescriptor)
            {
                ClientType type = ClientType.Create(entityDescriptor.Entity.GetType());
                if (!type.IsMediaLinkEntry && !entityDescriptor.IsMediaLinkEntry)
                {
                    return null;
                }
                if ((type.MediaDataMember == null) && (entityDescriptor.SaveStream == null))
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_MLEWithoutSaveStream(type.ElementTypeName));
                }
                HttpWebRequest mediaResourceRequest = this.CreateMediaResourceRequest(entityDescriptor.GetResourceUri(this.Context.baseUriWithSlash, false), "POST", type.MediaDataMember == null);
                if (type.MediaDataMember != null)
                {
                    if (type.MediaDataMember.MimeTypeProperty == null)
                    {
                        mediaResourceRequest.ContentType = "application/octet-stream";
                    }
                    else
                    {
                        string str = type.MediaDataMember.MimeTypeProperty.GetValue(entityDescriptor.Entity)?.ToString();
                        if (string.IsNullOrEmpty(str))
                        {
                            throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_NoContentTypeForMediaLink(type.ElementTypeName, type.MediaDataMember.MimeTypeProperty.PropertyName));
                        }
                        mediaResourceRequest.ContentType = str;
                    }
                    object propertyValue = type.MediaDataMember.GetValue(entityDescriptor.Entity);
                    if (propertyValue == null)
                    {
                        mediaResourceRequest.ContentLength = 0L;
                        this.mediaResourceRequestStream = null;
                    }
                    else
                    {
                        byte[] bytes = propertyValue as byte[];
                        if (bytes == null)
                        {
                            string str2;
                            Encoding encoding;
                            HttpProcessUtility.ReadContentType(mediaResourceRequest.ContentType, out str2, out encoding);
                            if (encoding == null)
                            {
                                encoding = Encoding.UTF8;
                                mediaResourceRequest.ContentType = mediaResourceRequest.ContentType + ";charset=UTF-8";
                            }
                            bytes = encoding.GetBytes(ClientConvert.ToString(propertyValue, false));
                        }
                        mediaResourceRequest.ContentLength = bytes.Length;
                        this.mediaResourceRequestStream = new MemoryStream(bytes, 0, bytes.Length, false, true);
                    }
                }
                else
                {
                    this.SetupMediaResourceRequest(mediaResourceRequest, entityDescriptor);
                }
                entityDescriptor.State = EntityStates.Modified;
                return mediaResourceRequest;
            }

            private HttpWebRequest CheckAndProcessMediaEntryPut(EntityDescriptor box)
            {
                if (box.SaveStream == null)
                {
                    return null;
                }
                Uri editMediaResourceUri = box.GetEditMediaResourceUri(this.Context.baseUriWithSlash);
                if (editMediaResourceUri == null)
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Context_SetSaveStreamWithoutEditMediaLink);
                }
                HttpWebRequest mediaResourceRequest = this.CreateMediaResourceRequest(editMediaResourceUri, "PUT", true);
                this.SetupMediaResourceRequest(mediaResourceRequest, box);
                if (box.StreamETag != null)
                {
                    mediaResourceRequest.Headers.Set(HttpRequestHeader.IfMatch, box.StreamETag);
                }
                return mediaResourceRequest;
            }

            private static void CompleteCheck(PerRequest value, InternalError errorcode)
            {
                if ((value == null) || value.RequestCompleted)
                {
                    System.Data.Services.Client.Error.ThrowInternalError(errorcode);
                }
            }

            protected override void CompletedRequest()
            {
                this.buildBatchBuffer = null;
                if (this.buildBatchWriter != null)
                {
                    this.HandleOperationEnd();
                    this.buildBatchWriter.WriteLine("--{0}--", this.batchBoundary);
                    this.buildBatchWriter.Flush();
                    this.httpWebResponseStream.Position = 0L;
                    this.buildBatchWriter = null;
                    this.responseBatchStream = new BatchStream(this.httpWebResponseStream, this.batchBoundary, HttpProcessUtility.EncodingUtf8NoPreamble, false);
                }
            }

            private HttpWebRequest CreateBatchRequest(MemoryStream memory)
            {
                Uri requestUri = Util.CreateUri(this.Context.baseUriWithSlash, Util.CreateUri("$batch", UriKind.Relative));
                string contentType = "multipart/mixed; boundary=" + this.batchBoundary;
                HttpWebRequest request = this.Context.CreateRequest(requestUri, "POST", false, contentType, Util.DataServiceVersion1, false);
                request.ContentLength = memory.Length - memory.Position;
                return request;
            }

            private PerRequest.ContentStream CreateChangeData(int index, bool newline)
            {
                Descriptor descriptor = this.ChangedEntries[index];
                if (descriptor.IsResource)
                {
                    EntityDescriptor box = (EntityDescriptor) descriptor;
                    if (this.processingMediaLinkEntry)
                    {
                        return new PerRequest.ContentStream(this.mediaResourceRequestStream, false);
                    }
                    descriptor.ContentGeneratedForSave = true;
                    return new PerRequest.ContentStream(this.Context.CreateRequestData(box, newline), true);
                }
                descriptor.ContentGeneratedForSave = true;
                LinkDescriptor binding = (LinkDescriptor) descriptor;
                if ((EntityStates.Added != binding.State) && ((EntityStates.Modified != binding.State) || (binding.Target == null)))
                {
                    return null;
                }
                return new PerRequest.ContentStream(this.Context.CreateRequestData(binding, newline), true);
            }

            private HttpWebRequest CreateMediaResourceRequest(Uri requestUri, string method, bool sendChunked) => 
                this.Context.CreateRequest(requestUri, method, false, "*/*", Util.DataServiceVersion1, sendChunked);

            private HttpWebRequest CreateNextRequest(bool replaceOnUpdate)
            {
                HttpWebRequest request;
                if (!this.processingMediaLinkEntry)
                {
                    this.entryIndex++;
                }
                else
                {
                    EntityDescriptor descriptor = (EntityDescriptor) this.ChangedEntries[this.entryIndex];
                    if (this.processingMediaLinkEntryPut && (EntityStates.Unchanged == descriptor.State))
                    {
                        descriptor.ContentGeneratedForSave = true;
                        this.entryIndex++;
                    }
                    this.processingMediaLinkEntry = false;
                    this.processingMediaLinkEntryPut = false;
                    descriptor.CloseSaveStream();
                }
                if (this.entryIndex >= this.ChangedEntries.Count)
                {
                    return null;
                }
                Descriptor descriptor2 = this.ChangedEntries[this.entryIndex];
                if (!descriptor2.IsResource)
                {
                    return this.Context.CreateRequest((LinkDescriptor) descriptor2);
                }
                EntityDescriptor box = (EntityDescriptor) descriptor2;
                if (((EntityStates.Unchanged == descriptor2.State) || (EntityStates.Modified == descriptor2.State)) && ((request = this.CheckAndProcessMediaEntryPut(box)) != null))
                {
                    this.processingMediaLinkEntry = true;
                    this.processingMediaLinkEntryPut = true;
                    return request;
                }
                if ((EntityStates.Added == descriptor2.State) && ((request = this.CheckAndProcessMediaEntryPost(box)) != null))
                {
                    this.processingMediaLinkEntry = true;
                    this.processingMediaLinkEntryPut = false;
                    return request;
                }
                return this.Context.CreateRequest(box, descriptor2.State, replaceOnUpdate);
            }

            internal System.Data.Services.Client.DataServiceResponse EndRequest()
            {
                foreach (EntityDescriptor descriptor in (from e in this.ChangedEntries
                    where e.IsResource
                    select e).Cast<EntityDescriptor>())
                {
                    descriptor.CloseSaveStream();
                }
                if ((this.responseBatchStream != null) || (this.httpWebResponseStream != null))
                {
                    this.HandleBatchResponse();
                }
                return this.DataServiceResponse;
            }

            private static void EqualRefCheck(PerRequest actual, PerRequest expected, InternalError errorcode)
            {
                if (!object.ReferenceEquals(actual, expected))
                {
                    System.Data.Services.Client.Error.ThrowInternalError(errorcode);
                }
            }

            private MemoryStream GenerateBatchRequest(bool replaceOnUpdate)
            {
                this.changesetBoundary = null;
                if (this.Queries == null)
                {
                    if (this.ChangedEntries.Count == 0)
                    {
                        this.DataServiceResponse = new System.Data.Services.Client.DataServiceResponse(null, 0, this.Responses, true);
                        base.SetCompleted();
                        return null;
                    }
                    this.changesetBoundary = "changeset_" + Guid.NewGuid().ToString();
                }
                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);
                if (this.Queries != null)
                {
                    for (int i = 0; i < this.Queries.Length; i++)
                    {
                        Uri uri = Util.CreateUri(this.Context.baseUriWithSlash, this.Queries[i].QueryComponents.Uri);
                        writer.WriteLine("--{0}", this.batchBoundary);
                        DataServiceContext.WriteOperationRequestHeaders(writer, "GET", uri.AbsoluteUri, this.Queries[i].QueryComponents.Version);
                        writer.WriteLine();
                    }
                }
                else if (0 < this.ChangedEntries.Count)
                {
                    writer.WriteLine("--{0}", this.batchBoundary);
                    writer.WriteLine("{0}: {1}; boundary={2}", "Content-Type", "multipart/mixed", this.changesetBoundary);
                    writer.WriteLine();
                    for (int j = 0; j < this.ChangedEntries.Count; j++)
                    {
                        Descriptor descriptor = this.ChangedEntries[j];
                        if (!descriptor.ContentGeneratedForSave)
                        {
                            writer.WriteLine("--{0}", this.changesetBoundary);
                            EntityDescriptor box = descriptor as EntityDescriptor;
                            if (descriptor.IsResource)
                            {
                                if (box.State == EntityStates.Added)
                                {
                                    if (ClientType.Create(box.Entity.GetType()).IsMediaLinkEntry || box.IsMediaLinkEntry)
                                    {
                                        throw System.Data.Services.Client.Error.NotSupported(System.Data.Services.Client.Strings.Context_BatchNotSupportedForMediaLink);
                                    }
                                }
                                else if (((box.State == EntityStates.Unchanged) || (box.State == EntityStates.Modified)) && (box.SaveStream != null))
                                {
                                    throw System.Data.Services.Client.Error.NotSupported(System.Data.Services.Client.Strings.Context_BatchNotSupportedForMediaLink);
                                }
                            }
                            PerRequest.ContentStream stream2 = this.CreateChangeData(j, true);
                            MemoryStream stream3 = null;
                            if (stream2 != null)
                            {
                                stream3 = stream2.Stream as MemoryStream;
                            }
                            if (descriptor.IsResource)
                            {
                                this.Context.CreateRequestBatch(box, writer, replaceOnUpdate);
                            }
                            else
                            {
                                this.Context.CreateRequestBatch((LinkDescriptor) descriptor, writer);
                            }
                            byte[] buffer = null;
                            int offset = 0;
                            int num4 = 0;
                            if (stream3 != null)
                            {
                                buffer = stream3.GetBuffer();
                                offset = (int) stream3.Position;
                                num4 = ((int) stream3.Length) - offset;
                            }
                            if (0 < num4)
                            {
                                writer.WriteLine("{0}: {1}", "Content-Length", num4);
                            }
                            writer.WriteLine();
                            if (0 < num4)
                            {
                                writer.Flush();
                                writer.BaseStream.Write(buffer, offset, num4);
                            }
                        }
                    }
                    writer.WriteLine("--{0}--", this.changesetBoundary);
                }
                writer.WriteLine("--{0}--", this.batchBoundary);
                writer.Flush();
                this.changesetBoundary = null;
                stream.Position = 0L;
                return stream;
            }

            private void HandleBatchResponse()
            {
                Func<Stream> getResponseStream = null;
                string batchBoundary = this.batchBoundary;
                Encoding encoding = Encoding.UTF8;
                Dictionary<string, string> headers = null;
                Exception innerException = null;
                try
                {
                    if (DataServiceContext.IsFlagSet(this.options, SaveChangesOptions.Batch))
                    {
                        if ((this.batchResponse == null) || (HttpStatusCode.NoContent == this.batchResponse.StatusCode))
                        {
                            throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Batch_ExpectedResponse(1));
                        }
                        headers = WebUtil.WrapResponseHeaders(this.batchResponse);
                        if (getResponseStream == null)
                        {
                            getResponseStream = () => this.httpWebResponseStream;
                        }
                        DataServiceContext.HandleResponse(this.batchResponse.StatusCode, this.batchResponse.Headers["DataServiceVersion"], getResponseStream, true);
                        if (!BatchStream.GetBoundaryAndEncodingFromMultipartMixedContentType(this.batchResponse.ContentType, out batchBoundary, out encoding))
                        {
                            string str2;
                            Exception responseText = null;
                            HttpProcessUtility.ReadContentType(this.batchResponse.ContentType, out str2, out encoding);
                            if (string.Equals("text/plain", str2))
                            {
                                responseText = DataServiceContext.GetResponseText(new Func<Stream>(this.batchResponse.GetResponseStream), this.batchResponse.StatusCode);
                            }
                            throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Batch_ExpectedContentType(this.batchResponse.ContentType), responseText);
                        }
                        if (this.httpWebResponseStream == null)
                        {
                            System.Data.Services.Client.Error.ThrowBatchExpectedResponse(InternalError.NullResponseStream);
                        }
                        this.DataServiceResponse = new System.Data.Services.Client.DataServiceResponse(headers, (int) this.batchResponse.StatusCode, this.Responses, true);
                    }
                    bool flag = true;
                    BatchStream batch = null;
                    try
                    {
                        batch = this.responseBatchStream ?? new BatchStream(this.httpWebResponseStream, batchBoundary, encoding, false);
                        this.httpWebResponseStream = null;
                        this.responseBatchStream = null;
                        IEnumerable<OperationResponse> enumerable = this.HandleBatchResponse(batch);
                        if (DataServiceContext.IsFlagSet(this.options, SaveChangesOptions.Batch) && (this.Queries != null))
                        {
                            flag = false;
                            this.responseBatchStream = batch;
                            this.DataServiceResponse = new System.Data.Services.Client.DataServiceResponse((Dictionary<string, string>) this.DataServiceResponse.BatchHeaders, this.DataServiceResponse.BatchStatusCode, enumerable, true);
                        }
                        else
                        {
                            foreach (ChangeOperationResponse response in enumerable)
                            {
                                if ((innerException == null) && (response.Error != null))
                                {
                                    innerException = response.Error;
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (flag && (batch != null))
                        {
                            batch.Close();
                        }
                    }
                }
                catch (InvalidOperationException exception3)
                {
                    innerException = exception3;
                }
                if (innerException != null)
                {
                    if (this.DataServiceResponse == null)
                    {
                        int statusCode = (this.batchResponse == null) ? 500 : ((int) this.batchResponse.StatusCode);
                        this.DataServiceResponse = new System.Data.Services.Client.DataServiceResponse(headers, statusCode, null, DataServiceContext.IsFlagSet(this.options, SaveChangesOptions.Batch));
                    }
                    throw new DataServiceRequestException(System.Data.Services.Client.Strings.DataServiceException_GeneralError, innerException, this.DataServiceResponse);
                }
            }

            private IEnumerable<OperationResponse> HandleBatchResponse(BatchStream batch)
            {
                ChangeOperationResponse iteratorVariable15;
                int iteratorVariable14;
                Exception iteratorVariable13;
                HttpStatusCode iteratorVariable12;
                Dictionary<string, string> contentHeaders;
                if (!batch.CanRead)
                {
                    goto Label_0777;
                }
                Uri editLink = null;
                int iteratorVariable5 = 0;
                int index = 0;
                int iteratorVariable7 = 0;
                this.entryIndex = 0;
                goto Label_0697;
            Label_05EA:
                iteratorVariable15 = new ChangeOperationResponse(contentHeaders, this.ChangedEntries[iteratorVariable14]);
                iteratorVariable15.StatusCode = (int) iteratorVariable12;
                if (iteratorVariable13 != null)
                {
                    iteratorVariable15.Error = iteratorVariable13;
                }
                this.Responses.Add(iteratorVariable15);
                iteratorVariable7++;
                this.entryIndex++;
                yield return iteratorVariable15;
            Label_0697:
                if (batch.MoveNext())
                {
                    string iteratorVariable0;
                    contentHeaders = batch.ContentHeaders;
                    switch (batch.State)
                    {
                        case BatchStreamState.BeginChangeSet:
                            if ((DataServiceContext.IsFlagSet(this.options, SaveChangesOptions.Batch) && (iteratorVariable5 != 0)) || (iteratorVariable7 != 0))
                            {
                                System.Data.Services.Client.Error.ThrowBatchUnexpectedContent(InternalError.UnexpectedBeginChangeSet);
                            }
                            goto Label_0697;

                        case BatchStreamState.EndChangeSet:
                            iteratorVariable5++;
                            iteratorVariable7 = 0;
                            goto Label_0697;

                        case BatchStreamState.GetResponse:
                        {
                            contentHeaders.TryGetValue("Content-Type", out iteratorVariable0);
                            HttpStatusCode statusCode = (HttpStatusCode) (-1);
                            Exception iteratorVariable10 = null;
                            QueryOperationResponse iteratorVariable11 = null;
                            try
                            {
                                statusCode = batch.GetStatusCode();
                                iteratorVariable10 = DataServiceContext.HandleResponse(statusCode, batch.GetResponseVersion(), new Func<Stream>(batch.GetContentStream), false);
                                if (iteratorVariable10 == null)
                                {
                                    DataServiceRequest query = this.Queries[index];
                                    MaterializeAtom results = DataServiceRequest.Materialize(this.Context, query.QueryComponents, null, iteratorVariable0, batch.GetContentStream());
                                    iteratorVariable11 = QueryOperationResponse.GetInstance(query.ElementType, contentHeaders, query, results);
                                }
                            }
                            catch (ArgumentException exception)
                            {
                                iteratorVariable10 = exception;
                            }
                            catch (FormatException exception2)
                            {
                                iteratorVariable10 = exception2;
                            }
                            catch (InvalidOperationException exception3)
                            {
                                iteratorVariable10 = exception3;
                            }
                            if (iteratorVariable11 == null)
                            {
                                if (this.Queries == null)
                                {
                                    throw iteratorVariable10;
                                }
                                DataServiceRequest request2 = this.Queries[index];
                                if (this.Context.ignoreResourceNotFoundException && (statusCode == HttpStatusCode.NotFound))
                                {
                                    iteratorVariable11 = QueryOperationResponse.GetInstance(request2.ElementType, contentHeaders, request2, MaterializeAtom.EmptyResults);
                                }
                                else
                                {
                                    iteratorVariable11 = QueryOperationResponse.GetInstance(request2.ElementType, contentHeaders, request2, MaterializeAtom.EmptyResults);
                                    iteratorVariable11.Error = iteratorVariable10;
                                }
                            }
                            iteratorVariable11.StatusCode = (int) statusCode;
                            index++;
                            yield return iteratorVariable11;
                            goto Label_0697;
                        }
                        case BatchStreamState.ChangeResponse:
                            iteratorVariable12 = batch.GetStatusCode();
                            iteratorVariable13 = DataServiceContext.HandleResponse(iteratorVariable12, batch.GetResponseVersion(), new Func<Stream>(batch.GetContentStream), false);
                            iteratorVariable14 = this.ValidateContentID(contentHeaders);
                            try
                            {
                                string iteratorVariable2;
                                Descriptor iteratorVariable9 = this.ChangedEntries[iteratorVariable14];
                                iteratorVariable7 += this.Context.SaveResultProcessed(iteratorVariable9);
                                if (iteratorVariable13 != null)
                                {
                                    throw iteratorVariable13;
                                }
                                StreamStates streamState = StreamStates.NoStream;
                                if (iteratorVariable9.IsResource)
                                {
                                    EntityDescriptor descriptor = (EntityDescriptor) iteratorVariable9;
                                    streamState = descriptor.StreamState;
                                }
                                if ((streamState == StreamStates.Added) || (iteratorVariable9.State == EntityStates.Added))
                                {
                                    if (iteratorVariable9.IsResource)
                                    {
                                        string iteratorVariable1;
                                        string mime = null;
                                        Encoding encoding = null;
                                        contentHeaders.TryGetValue("Content-Type", out iteratorVariable0);
                                        contentHeaders.TryGetValue("Location", out iteratorVariable1);
                                        contentHeaders.TryGetValue("ETag", out iteratorVariable2);
                                        EntityDescriptor entry = (EntityDescriptor) iteratorVariable9;
                                        if (iteratorVariable1 == null)
                                        {
                                            throw System.Data.Services.Client.Error.NotSupported(System.Data.Services.Client.Strings.Deserialize_NoLocationHeader);
                                        }
                                        editLink = Util.CreateUri(iteratorVariable1, UriKind.Absolute);
                                        Stream stream = batch.GetContentStream();
                                        if (stream != null)
                                        {
                                            HttpProcessUtility.ReadContentType(iteratorVariable0, out mime, out encoding);
                                            if (!string.Equals("application/atom+xml", mime, StringComparison.OrdinalIgnoreCase))
                                            {
                                                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Deserialize_UnknownMimeTypeSpecified(mime));
                                            }
                                            XmlReader reader = XmlUtil.CreateXmlReader(stream, encoding);
                                            QueryComponents queryComponents = new QueryComponents(null, Util.DataServiceVersionEmpty, entry.Entity.GetType(), null, null);
                                            EntityDescriptor descriptor3 = (EntityDescriptor) iteratorVariable9;
                                            MergeOption mergeOption = MergeOption.OverwriteChanges;
                                            if (descriptor3.StreamState == StreamStates.Added)
                                            {
                                                mergeOption = MergeOption.PreserveChanges;
                                            }
                                            try
                                            {
                                                using (MaterializeAtom atom2 = new MaterializeAtom(this.Context, reader, queryComponents, null, mergeOption))
                                                {
                                                    this.Context.HandleResponsePost(entry, atom2, editLink, iteratorVariable2);
                                                }
                                                goto Label_05EA;
                                            }
                                            finally
                                            {
                                                if (descriptor3.StreamState == StreamStates.Added)
                                                {
                                                    descriptor3.State = EntityStates.Modified;
                                                    descriptor3.StreamState = StreamStates.NoStream;
                                                }
                                            }
                                        }
                                        this.Context.HandleResponsePost(entry, null, editLink, iteratorVariable2);
                                    }
                                    else
                                    {
                                        DataServiceContext.HandleResponsePost((LinkDescriptor) iteratorVariable9);
                                    }
                                }
                                else if ((streamState == StreamStates.Modified) || (iteratorVariable9.State == EntityStates.Modified))
                                {
                                    contentHeaders.TryGetValue("ETag", out iteratorVariable2);
                                    DataServiceContext.HandleResponsePut(iteratorVariable9, iteratorVariable2);
                                }
                                else if (iteratorVariable9.State == EntityStates.Deleted)
                                {
                                    this.Context.HandleResponseDelete(iteratorVariable9);
                                }
                            }
                            catch (Exception exception4)
                            {
                                this.ChangedEntries[iteratorVariable14].SaveError = exception4;
                                iteratorVariable13 = exception4;
                            }
                            goto Label_05EA;
                    }
                    System.Data.Services.Client.Error.ThrowBatchExpectedResponse(InternalError.UnexpectedBatchState);
                    goto Label_0697;
                }
                if (((this.Queries == null) && (((iteratorVariable5 == 0) || (0 < index)) || (this.ChangedEntries.Any<Descriptor>(o => (o.ContentGeneratedForSave && (0 == o.SaveResultWasProcessed))) && (!DataServiceContext.IsFlagSet(this.options, SaveChangesOptions.Batch) || (this.ChangedEntries.FirstOrDefault<Descriptor>(o => (null != o.SaveError)) == null))))) || ((this.Queries != null) && (index != this.Queries.Length)))
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Batch_IncompleteResponseCount);
                }
                batch.Dispose();
            Label_0777:;
            }

            private void HandleCompleted(PerRequest pereq)
            {
                if (pereq != null)
                {
                    base.CompletedSynchronously &= pereq.RequestCompletedSynchronously;
                    if (pereq.RequestCompleted)
                    {
                        Interlocked.CompareExchange<PerRequest>(ref this.request, null, pereq);
                        if (DataServiceContext.IsFlagSet(this.options, SaveChangesOptions.Batch))
                        {
                            Interlocked.CompareExchange<HttpWebResponse>(ref this.batchResponse, pereq.HttpWebResponse, null);
                            pereq.HttpWebResponse = null;
                        }
                        pereq.Dispose();
                    }
                }
                base.HandleCompleted();
            }

            private bool HandleFailure(PerRequest pereq, Exception e)
            {
                if (pereq != null)
                {
                    if (base.IsAborted)
                    {
                        pereq.SetAborted();
                    }
                    else
                    {
                        pereq.SetComplete();
                    }
                }
                return base.HandleFailure(e);
            }

            private void HandleOperationEnd()
            {
                if (this.changesetStarted)
                {
                    this.buildBatchWriter.WriteLine();
                    this.buildBatchWriter.WriteLine("--{0}--", this.changesetBoundary);
                    this.changesetStarted = false;
                }
            }

            private void HandleOperationException(Exception e, HttpWebResponse response)
            {
                if (response != null)
                {
                    this.HandleOperationResponse(response);
                    this.HandleOperationResponseData(response);
                    this.HandleOperationEnd();
                }
                else
                {
                    this.HandleOperationStart();
                    DataServiceContext.WriteOperationResponseHeaders(this.buildBatchWriter, 500);
                    this.buildBatchWriter.WriteLine("{0}: {1}", "Content-Type", "text/plain");
                    this.buildBatchWriter.WriteLine("{0}: {1}", "Content-ID", this.ChangedEntries[this.entryIndex].ChangeOrder);
                    this.buildBatchWriter.WriteLine();
                    this.buildBatchWriter.WriteLine(e.ToString());
                    this.HandleOperationEnd();
                }
                this.request = null;
                if (!DataServiceContext.IsFlagSet(this.options, SaveChangesOptions.ContinueOnError))
                {
                    base.SetCompleted();
                    this.processingMediaLinkEntry = false;
                    this.ChangedEntries[this.entryIndex].ContentGeneratedForSave = true;
                }
            }

            private void HandleOperationResponse(HttpWebResponse response)
            {
                this.HandleOperationStart();
                Descriptor descriptor = this.ChangedEntries[this.entryIndex];
                if (descriptor.IsResource)
                {
                    EntityDescriptor descriptor2 = (EntityDescriptor) descriptor;
                    if ((descriptor.State == EntityStates.Added) || (((descriptor.State == EntityStates.Modified) && this.processingMediaLinkEntry) && !this.processingMediaLinkEntryPut))
                    {
                        string location = response.Headers["Location"];
                        if (WebUtil.SuccessStatusCode(response.StatusCode))
                        {
                            if (location == null)
                            {
                                throw System.Data.Services.Client.Error.NotSupported(System.Data.Services.Client.Strings.Deserialize_NoLocationHeader);
                            }
                            this.Context.AttachLocation(descriptor2.Entity, location);
                        }
                    }
                    if (this.processingMediaLinkEntry)
                    {
                        if (!WebUtil.SuccessStatusCode(response.StatusCode))
                        {
                            this.processingMediaLinkEntry = false;
                            if (!this.processingMediaLinkEntryPut)
                            {
                                descriptor.State = EntityStates.Added;
                                this.processingMediaLinkEntryPut = false;
                            }
                            descriptor.ContentGeneratedForSave = true;
                        }
                        else if (response.StatusCode == HttpStatusCode.Created)
                        {
                            descriptor2.ETag = response.Headers["ETag"];
                        }
                    }
                }
                DataServiceContext.WriteOperationResponseHeaders(this.buildBatchWriter, (int) response.StatusCode);
                foreach (string str2 in response.Headers.AllKeys)
                {
                    if ("Content-Length" != str2)
                    {
                        this.buildBatchWriter.WriteLine("{0}: {1}", str2, response.Headers[str2]);
                    }
                }
                this.buildBatchWriter.WriteLine("{0}: {1}", "Content-ID", descriptor.ChangeOrder);
                this.buildBatchWriter.WriteLine();
            }

            private void HandleOperationResponseData(HttpWebResponse response)
            {
                using (Stream stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        this.buildBatchWriter.Flush();
                        if (0L == WebUtil.CopyStream(stream, this.buildBatchWriter.BaseStream, ref this.buildBatchBuffer))
                        {
                            this.HandleOperationResponseNoData();
                        }
                    }
                }
            }

            private void HandleOperationResponseNoData()
            {
                this.buildBatchWriter.Flush();
                Stream baseStream = this.buildBatchWriter.BaseStream;
                baseStream.Position -= DataServiceContext.NewLine.Length;
                this.buildBatchWriter.WriteLine("{0}: {1}", "Content-Length", 0);
                this.buildBatchWriter.WriteLine();
            }

            private void HandleOperationStart()
            {
                this.HandleOperationEnd();
                if (this.httpWebResponseStream == null)
                {
                    this.httpWebResponseStream = new MemoryStream();
                }
                if (this.buildBatchWriter == null)
                {
                    this.buildBatchWriter = new StreamWriter(this.httpWebResponseStream);
                }
                if (this.changesetBoundary == null)
                {
                    this.changesetBoundary = "changesetresponse_" + Guid.NewGuid().ToString();
                }
                this.changesetStarted = true;
                this.buildBatchWriter.WriteLine("--{0}", this.batchBoundary);
                this.buildBatchWriter.WriteLine("{0}: {1}; boundary={2}", "Content-Type", "multipart/mixed", this.changesetBoundary);
                this.buildBatchWriter.WriteLine();
                this.buildBatchWriter.WriteLine("--{0}", this.changesetBoundary);
            }

            private void SaveNextChange(PerRequest pereq)
            {
                if (!pereq.RequestCompleted)
                {
                    System.Data.Services.Client.Error.ThrowInternalError(InternalError.SaveNextChangeIncomplete);
                }
                EqualRefCheck(this.request, pereq, InternalError.InvalidSaveNextChange);
                if (DataServiceContext.IsFlagSet(this.options, SaveChangesOptions.Batch))
                {
                    this.httpWebResponseStream.Position = 0L;
                    this.request = null;
                    base.SetCompleted();
                }
                else
                {
                    if (0L == this.copiedContentLength)
                    {
                        this.HandleOperationResponseNoData();
                    }
                    this.HandleOperationEnd();
                    if (!this.processingMediaLinkEntry)
                    {
                        this.changesCompleted++;
                    }
                    pereq.Dispose();
                    this.request = null;
                    if (!pereq.RequestCompletedSynchronously && !base.IsCompletedInternally)
                    {
                        this.BeginNextChange(DataServiceContext.IsFlagSet(this.options, SaveChangesOptions.ReplaceOnUpdate));
                    }
                }
            }

            private void SetupMediaResourceRequest(HttpWebRequest mediaResourceRequest, EntityDescriptor box)
            {
                this.mediaResourceRequestStream = box.SaveStream.Stream;
                WebUtil.ApplyHeadersToRequest(box.SaveStream.Args.Headers, mediaResourceRequest, true);
            }

            private int ValidateContentID(Dictionary<string, string> contentHeaders)
            {
                string str;
                int result = 0;
                if (!contentHeaders.TryGetValue("Content-ID", out str) || !int.TryParse(str, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
                {
                    System.Data.Services.Client.Error.ThrowBatchUnexpectedContent(InternalError.ChangeResponseMissingContentID);
                }
                for (int i = 0; i < this.ChangedEntries.Count; i++)
                {
                    if (this.ChangedEntries[i].ChangeOrder == result)
                    {
                        return i;
                    }
                }
                System.Data.Services.Client.Error.ThrowBatchUnexpectedContent(InternalError.ChangeResponseUnknownContentID);
                return -1;
            }

            internal System.Data.Services.Client.DataServiceResponse DataServiceResponse
            {
                get => 
                    this.service;
                set
                {
                    this.service = value;
                }
            }


            private sealed class PerRequest
            {
                private byte[] requestContentBuffer;
                private int requestStatus;

                internal PerRequest()
                {
                    this.RequestCompletedSynchronously = true;
                }

                internal void Dispose()
                {
                    Stream responseStream = null;
                    responseStream = this.ResponseStream;
                    if (responseStream != null)
                    {
                        this.ResponseStream = null;
                        responseStream.Dispose();
                    }
                    if (this.RequestContentStream != null)
                    {
                        if ((this.RequestContentStream.Stream != null) && this.RequestContentStream.IsKnownMemoryStream)
                        {
                            this.RequestContentStream.Stream.Dispose();
                        }
                        this.RequestContentStream = null;
                    }
                    responseStream = this.RequestStream;
                    if (responseStream != null)
                    {
                        this.RequestStream = null;
                        try
                        {
                            responseStream.Dispose();
                        }
                        catch (WebException)
                        {
                            if (!this.RequestAborted)
                            {
                                throw;
                            }
                        }
                    }
                    System.Net.HttpWebResponse httpWebResponse = this.HttpWebResponse;
                    if (httpWebResponse != null)
                    {
                        httpWebResponse.Close();
                    }
                    this.Request = null;
                    this.SetComplete();
                }

                internal void SetAborted()
                {
                    Interlocked.Exchange(ref this.requestStatus, 2);
                }

                internal void SetComplete()
                {
                    Interlocked.CompareExchange(ref this.requestStatus, 1, 0);
                }

                internal System.Net.HttpWebResponse HttpWebResponse { get; set; }

                internal HttpWebRequest Request { get; set; }

                internal bool RequestAborted =>
                    (this.requestStatus == 2);

                internal bool RequestCompleted =>
                    (this.requestStatus != 0);

                internal bool RequestCompletedSynchronously { get; set; }

                internal byte[] RequestContentBuffer
                {
                    get
                    {
                        if (this.requestContentBuffer == null)
                        {
                            this.requestContentBuffer = new byte[0x10000];
                        }
                        return this.requestContentBuffer;
                    }
                }

                internal int RequestContentBufferValidLength { get; set; }

                internal ContentStream RequestContentStream { get; set; }

                internal Stream RequestStream { get; set; }

                internal Stream ResponseStream { get; set; }

                internal class ContentStream
                {
                    private readonly bool isKnownMemoryStream;
                    private readonly System.IO.Stream stream;

                    public ContentStream(System.IO.Stream stream, bool isKnownMemoryStream)
                    {
                        this.stream = stream;
                        this.isKnownMemoryStream = isKnownMemoryStream;
                    }

                    public bool IsKnownMemoryStream =>
                        this.isKnownMemoryStream;

                    public System.IO.Stream Stream =>
                        this.stream;
                }
            }
        }
    }
}

