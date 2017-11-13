namespace System.ServiceModel.Syndication
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract]
    public abstract class SyndicationFeedFormatter
    {
        private SyndicationFeed feed;

        protected SyndicationFeedFormatter()
        {
            this.feed = null;
        }

        protected SyndicationFeedFormatter(SyndicationFeed feedToWrite)
        {
            if (feedToWrite == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("feedToWrite");
            }
            this.feed = feedToWrite;
        }

        public abstract bool CanRead(XmlReader reader);
        internal static void CloseBuffer(XmlBuffer buffer, XmlDictionaryWriter extWriter)
        {
            if (buffer != null)
            {
                extWriter.WriteEndElement();
                buffer.CloseSection();
                buffer.Close();
            }
        }

        internal static void CreateBufferIfRequiredAndWriteNode(ref XmlBuffer buffer, ref XmlDictionaryWriter extWriter, XmlReader reader, int maxExtensionSize)
        {
            if (buffer == null)
            {
                buffer = new XmlBuffer(maxExtensionSize);
                extWriter = buffer.OpenSection(XmlDictionaryReaderQuotas.Max);
                extWriter.WriteStartElement("extensionWrapper");
            }
            extWriter.WriteNode(reader, false);
        }

        protected internal static SyndicationCategory CreateCategory(SyndicationFeed feed)
        {
            if (feed == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("feed");
            }
            return GetNonNullValue<SyndicationCategory>(feed.CreateCategory(), SR2.FeedCreatedNullCategory);
        }

        protected internal static SyndicationCategory CreateCategory(SyndicationItem item)
        {
            if (item == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
            }
            return GetNonNullValue<SyndicationCategory>(item.CreateCategory(), SR2.ItemCreatedNullCategory);
        }

        protected abstract SyndicationFeed CreateFeedInstance();
        internal static SyndicationFeed CreateFeedInstance(Type feedType)
        {
            if (feedType.Equals(typeof(SyndicationFeed)))
            {
                return new SyndicationFeed();
            }
            return (SyndicationFeed) Activator.CreateInstance(feedType);
        }

        protected internal static SyndicationItem CreateItem(SyndicationFeed feed)
        {
            if (feed == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("feed");
            }
            return GetNonNullValue<SyndicationItem>(feed.CreateItem(), SR2.FeedCreatedNullItem);
        }

        protected internal static SyndicationLink CreateLink(SyndicationFeed feed)
        {
            if (feed == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("feed");
            }
            return GetNonNullValue<SyndicationLink>(feed.CreateLink(), SR2.FeedCreatedNullPerson);
        }

        protected internal static SyndicationLink CreateLink(SyndicationItem item)
        {
            if (item == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
            }
            return GetNonNullValue<SyndicationLink>(item.CreateLink(), SR2.ItemCreatedNullPerson);
        }

        protected internal static SyndicationPerson CreatePerson(SyndicationFeed feed)
        {
            if (feed == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("feed");
            }
            return GetNonNullValue<SyndicationPerson>(feed.CreatePerson(), SR2.FeedCreatedNullPerson);
        }

        protected internal static SyndicationPerson CreatePerson(SyndicationItem item)
        {
            if (item == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
            }
            return GetNonNullValue<SyndicationPerson>(item.CreatePerson(), SR2.ItemCreatedNullPerson);
        }

        private static T GetNonNullValue<T>(T value, string errorMsg)
        {
            if (value == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(errorMsg, new object[0])));
            }
            return value;
        }

        internal static void LoadElementExtensions(XmlBuffer buffer, XmlDictionaryWriter writer, SyndicationCategory category)
        {
            if (category == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("category");
            }
            CloseBuffer(buffer, writer);
            category.LoadElementExtensions(buffer);
        }

        internal static void LoadElementExtensions(XmlBuffer buffer, XmlDictionaryWriter writer, SyndicationFeed feed)
        {
            if (feed == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("feed");
            }
            CloseBuffer(buffer, writer);
            feed.LoadElementExtensions(buffer);
        }

        internal static void LoadElementExtensions(XmlBuffer buffer, XmlDictionaryWriter writer, SyndicationItem item)
        {
            if (item == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
            }
            CloseBuffer(buffer, writer);
            item.LoadElementExtensions(buffer);
        }

        internal static void LoadElementExtensions(XmlBuffer buffer, XmlDictionaryWriter writer, SyndicationLink link)
        {
            if (link == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("link");
            }
            CloseBuffer(buffer, writer);
            link.LoadElementExtensions(buffer);
        }

        internal static void LoadElementExtensions(XmlBuffer buffer, XmlDictionaryWriter writer, SyndicationPerson person)
        {
            if (person == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("person");
            }
            CloseBuffer(buffer, writer);
            person.LoadElementExtensions(buffer);
        }

        protected internal static void LoadElementExtensions(XmlReader reader, SyndicationCategory category, int maxExtensionSize)
        {
            if (category == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("category");
            }
            category.LoadElementExtensions(reader, maxExtensionSize);
        }

        protected internal static void LoadElementExtensions(XmlReader reader, SyndicationFeed feed, int maxExtensionSize)
        {
            if (feed == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("feed");
            }
            feed.LoadElementExtensions(reader, maxExtensionSize);
        }

        protected internal static void LoadElementExtensions(XmlReader reader, SyndicationItem item, int maxExtensionSize)
        {
            if (item == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
            }
            item.LoadElementExtensions(reader, maxExtensionSize);
        }

        protected internal static void LoadElementExtensions(XmlReader reader, SyndicationLink link, int maxExtensionSize)
        {
            if (link == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("link");
            }
            link.LoadElementExtensions(reader, maxExtensionSize);
        }

        protected internal static void LoadElementExtensions(XmlReader reader, SyndicationPerson person, int maxExtensionSize)
        {
            if (person == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("person");
            }
            person.LoadElementExtensions(reader, maxExtensionSize);
        }

        internal static void MoveToStartElement(XmlReader reader)
        {
            if (!reader.IsStartElement())
            {
                XmlExceptionHelper.ThrowStartElementExpected(XmlDictionaryReader.CreateDictionaryReader(reader));
            }
        }

        public abstract void ReadFrom(XmlReader reader);
        protected internal virtual void SetFeed(SyndicationFeed feed)
        {
            if (feed == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("feed");
            }
            this.feed = feed;
        }

        public override string ToString() => 
            string.Format(CultureInfo.CurrentCulture, "{0}, SyndicationVersion={1}", new object[] { base.GetType(), this.Version });

        internal static void TraceFeedReadBegin()
        {
            if (System.ServiceModel.DiagnosticUtility.ShouldTraceInformation)
            {
                System.ServiceModel.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SyndicationReadFeedBegin, SR2.GetString(SR2.TraceCodeSyndicationFeedReadBegin, new object[0]));
            }
        }

        internal static void TraceFeedReadEnd()
        {
            if (System.ServiceModel.DiagnosticUtility.ShouldTraceInformation)
            {
                System.ServiceModel.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SyndicationReadFeedEnd, SR2.GetString(SR2.TraceCodeSyndicationFeedReadEnd, new object[0]));
            }
        }

        internal static void TraceFeedWriteBegin()
        {
            if (System.ServiceModel.DiagnosticUtility.ShouldTraceInformation)
            {
                System.ServiceModel.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SyndicationWriteFeedBegin, SR2.GetString(SR2.TraceCodeSyndicationFeedWriteBegin, new object[0]));
            }
        }

        internal static void TraceFeedWriteEnd()
        {
            if (System.ServiceModel.DiagnosticUtility.ShouldTraceInformation)
            {
                System.ServiceModel.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SyndicationWriteFeedEnd, SR2.GetString(SR2.TraceCodeSyndicationFeedWriteEnd, new object[0]));
            }
        }

        internal static void TraceItemReadBegin()
        {
            if (System.ServiceModel.DiagnosticUtility.ShouldTraceInformation)
            {
                System.ServiceModel.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SyndicationReadItemBegin, SR2.GetString(SR2.TraceCodeSyndicationItemReadBegin, new object[0]));
            }
        }

        internal static void TraceItemReadEnd()
        {
            if (System.ServiceModel.DiagnosticUtility.ShouldTraceInformation)
            {
                System.ServiceModel.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SyndicationReadItemEnd, SR2.GetString(SR2.TraceCodeSyndicationItemReadEnd, new object[0]));
            }
        }

        internal static void TraceItemWriteBegin()
        {
            if (System.ServiceModel.DiagnosticUtility.ShouldTraceInformation)
            {
                System.ServiceModel.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SyndicationWriteItemBegin, SR2.GetString(SR2.TraceCodeSyndicationItemWriteBegin, new object[0]));
            }
        }

        internal static void TraceItemWriteEnd()
        {
            if (System.ServiceModel.DiagnosticUtility.ShouldTraceInformation)
            {
                System.ServiceModel.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SyndicationWriteItemEnd, SR2.GetString(SR2.TraceCodeSyndicationItemWriteEnd, new object[0]));
            }
        }

        internal static void TraceSyndicationElementIgnoredOnRead(XmlReader reader)
        {
            if (System.ServiceModel.DiagnosticUtility.ShouldTraceInformation)
            {
                System.ServiceModel.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SyndicationProtocolElementIgnoredOnRead, SR2.GetString(SR2.TraceCodeSyndicationProtocolElementIgnoredOnRead, new object[] { reader.NodeType, reader.LocalName, reader.NamespaceURI }));
            }
        }

        protected internal static bool TryParseAttribute(string name, string ns, string value, SyndicationCategory category, string version)
        {
            if (category == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("category");
            }
            return (FeedUtils.IsXmlns(name, ns) || category.TryParseAttribute(name, ns, value, version));
        }

        protected internal static bool TryParseAttribute(string name, string ns, string value, SyndicationFeed feed, string version)
        {
            if (feed == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("feed");
            }
            return (FeedUtils.IsXmlns(name, ns) || feed.TryParseAttribute(name, ns, value, version));
        }

        protected internal static bool TryParseAttribute(string name, string ns, string value, SyndicationItem item, string version)
        {
            if (item == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
            }
            return (FeedUtils.IsXmlns(name, ns) || item.TryParseAttribute(name, ns, value, version));
        }

        protected internal static bool TryParseAttribute(string name, string ns, string value, SyndicationLink link, string version)
        {
            if (link == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("link");
            }
            return (FeedUtils.IsXmlns(name, ns) || link.TryParseAttribute(name, ns, value, version));
        }

        protected internal static bool TryParseAttribute(string name, string ns, string value, SyndicationPerson person, string version)
        {
            if (person == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("person");
            }
            return (FeedUtils.IsXmlns(name, ns) || person.TryParseAttribute(name, ns, value, version));
        }

        protected internal static bool TryParseContent(XmlReader reader, SyndicationItem item, string contentType, string version, out SyndicationContent content) => 
            item.TryParseContent(reader, contentType, version, out content);

        protected internal static bool TryParseElement(XmlReader reader, SyndicationCategory category, string version) => 
            category?.TryParseElement(reader, version);

        protected internal static bool TryParseElement(XmlReader reader, SyndicationFeed feed, string version) => 
            feed?.TryParseElement(reader, version);

        protected internal static bool TryParseElement(XmlReader reader, SyndicationItem item, string version) => 
            item?.TryParseElement(reader, version);

        protected internal static bool TryParseElement(XmlReader reader, SyndicationLink link, string version) => 
            link?.TryParseElement(reader, version);

        protected internal static bool TryParseElement(XmlReader reader, SyndicationPerson person, string version) => 
            person?.TryParseElement(reader, version);

        protected internal static void WriteAttributeExtensions(XmlWriter writer, SyndicationCategory category, string version)
        {
            if (category == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("category");
            }
            category.WriteAttributeExtensions(writer, version);
        }

        protected internal static void WriteAttributeExtensions(XmlWriter writer, SyndicationFeed feed, string version)
        {
            if (feed == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("feed");
            }
            feed.WriteAttributeExtensions(writer, version);
        }

        protected internal static void WriteAttributeExtensions(XmlWriter writer, SyndicationItem item, string version)
        {
            if (item == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
            }
            item.WriteAttributeExtensions(writer, version);
        }

        protected internal static void WriteAttributeExtensions(XmlWriter writer, SyndicationLink link, string version)
        {
            if (link == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("link");
            }
            link.WriteAttributeExtensions(writer, version);
        }

        protected internal static void WriteAttributeExtensions(XmlWriter writer, SyndicationPerson person, string version)
        {
            if (person == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("person");
            }
            person.WriteAttributeExtensions(writer, version);
        }

        protected internal static void WriteElementExtensions(XmlWriter writer, SyndicationCategory category, string version)
        {
            if (category == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("category");
            }
            category.WriteElementExtensions(writer, version);
        }

        protected internal static void WriteElementExtensions(XmlWriter writer, SyndicationFeed feed, string version)
        {
            if (feed == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("feed");
            }
            feed.WriteElementExtensions(writer, version);
        }

        protected internal static void WriteElementExtensions(XmlWriter writer, SyndicationItem item, string version)
        {
            if (item == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
            }
            item.WriteElementExtensions(writer, version);
        }

        protected internal static void WriteElementExtensions(XmlWriter writer, SyndicationLink link, string version)
        {
            if (link == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("link");
            }
            link.WriteElementExtensions(writer, version);
        }

        protected internal static void WriteElementExtensions(XmlWriter writer, SyndicationPerson person, string version)
        {
            if (person == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("person");
            }
            person.WriteElementExtensions(writer, version);
        }

        public abstract void WriteTo(XmlWriter writer);

        public SyndicationFeed Feed =>
            this.feed;

        public abstract string Version { get; }
    }
}

