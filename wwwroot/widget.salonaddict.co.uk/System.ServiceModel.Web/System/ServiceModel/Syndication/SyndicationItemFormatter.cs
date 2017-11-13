﻿namespace System.ServiceModel.Syndication
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Xml;

    [DataContract]
    public abstract class SyndicationItemFormatter
    {
        private SyndicationItem item;

        protected SyndicationItemFormatter()
        {
            this.item = null;
        }

        protected SyndicationItemFormatter(SyndicationItem itemToWrite)
        {
            if (itemToWrite == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("itemToWrite");
            }
            this.item = itemToWrite;
        }

        public abstract bool CanRead(XmlReader reader);
        internal static void CreateBufferIfRequiredAndWriteNode(ref XmlBuffer buffer, ref XmlDictionaryWriter extWriter, XmlDictionaryReader reader, int maxExtensionSize)
        {
            SyndicationFeedFormatter.CreateBufferIfRequiredAndWriteNode(ref buffer, ref extWriter, reader, maxExtensionSize);
        }

        protected static SyndicationCategory CreateCategory(SyndicationItem item) => 
            SyndicationFeedFormatter.CreateCategory(item);

        protected abstract SyndicationItem CreateItemInstance();
        internal static SyndicationItem CreateItemInstance(Type itemType)
        {
            if (itemType.Equals(typeof(SyndicationItem)))
            {
                return new SyndicationItem();
            }
            return (SyndicationItem) Activator.CreateInstance(itemType);
        }

        protected static SyndicationLink CreateLink(SyndicationItem item) => 
            SyndicationFeedFormatter.CreateLink(item);

        protected static SyndicationPerson CreatePerson(SyndicationItem item) => 
            SyndicationFeedFormatter.CreatePerson(item);

        internal static void LoadElementExtensions(XmlBuffer buffer, XmlDictionaryWriter writer, SyndicationCategory category)
        {
            SyndicationFeedFormatter.LoadElementExtensions(buffer, writer, category);
        }

        internal static void LoadElementExtensions(XmlBuffer buffer, XmlDictionaryWriter writer, SyndicationItem item)
        {
            SyndicationFeedFormatter.LoadElementExtensions(buffer, writer, item);
        }

        internal static void LoadElementExtensions(XmlBuffer buffer, XmlDictionaryWriter writer, SyndicationLink link)
        {
            SyndicationFeedFormatter.LoadElementExtensions(buffer, writer, link);
        }

        internal static void LoadElementExtensions(XmlBuffer buffer, XmlDictionaryWriter writer, SyndicationPerson person)
        {
            SyndicationFeedFormatter.LoadElementExtensions(buffer, writer, person);
        }

        protected static void LoadElementExtensions(XmlReader reader, SyndicationCategory category, int maxExtensionSize)
        {
            SyndicationFeedFormatter.LoadElementExtensions(reader, category, maxExtensionSize);
        }

        protected static void LoadElementExtensions(XmlReader reader, SyndicationItem item, int maxExtensionSize)
        {
            SyndicationFeedFormatter.LoadElementExtensions(reader, item, maxExtensionSize);
        }

        protected static void LoadElementExtensions(XmlReader reader, SyndicationLink link, int maxExtensionSize)
        {
            SyndicationFeedFormatter.LoadElementExtensions(reader, link, maxExtensionSize);
        }

        protected static void LoadElementExtensions(XmlReader reader, SyndicationPerson person, int maxExtensionSize)
        {
            SyndicationFeedFormatter.LoadElementExtensions(reader, person, maxExtensionSize);
        }

        public abstract void ReadFrom(XmlReader reader);
        protected internal virtual void SetItem(SyndicationItem item)
        {
            if (item == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
            }
            this.item = item;
        }

        public override string ToString() => 
            string.Format(CultureInfo.CurrentCulture, "{0}, SyndicationVersion={1}", new object[] { base.GetType(), this.Version });

        protected static bool TryParseAttribute(string name, string ns, string value, SyndicationCategory category, string version) => 
            SyndicationFeedFormatter.TryParseAttribute(name, ns, value, category, version);

        protected static bool TryParseAttribute(string name, string ns, string value, SyndicationItem item, string version) => 
            SyndicationFeedFormatter.TryParseAttribute(name, ns, value, item, version);

        protected static bool TryParseAttribute(string name, string ns, string value, SyndicationLink link, string version) => 
            SyndicationFeedFormatter.TryParseAttribute(name, ns, value, link, version);

        protected static bool TryParseAttribute(string name, string ns, string value, SyndicationPerson person, string version) => 
            SyndicationFeedFormatter.TryParseAttribute(name, ns, value, person, version);

        protected static bool TryParseContent(XmlReader reader, SyndicationItem item, string contentType, string version, out SyndicationContent content) => 
            SyndicationFeedFormatter.TryParseContent(reader, item, contentType, version, out content);

        protected static bool TryParseElement(XmlReader reader, SyndicationCategory category, string version) => 
            SyndicationFeedFormatter.TryParseElement(reader, category, version);

        protected static bool TryParseElement(XmlReader reader, SyndicationItem item, string version) => 
            SyndicationFeedFormatter.TryParseElement(reader, item, version);

        protected static bool TryParseElement(XmlReader reader, SyndicationLink link, string version) => 
            SyndicationFeedFormatter.TryParseElement(reader, link, version);

        protected static bool TryParseElement(XmlReader reader, SyndicationPerson person, string version) => 
            SyndicationFeedFormatter.TryParseElement(reader, person, version);

        protected static void WriteAttributeExtensions(XmlWriter writer, SyndicationCategory category, string version)
        {
            SyndicationFeedFormatter.WriteAttributeExtensions(writer, category, version);
        }

        protected static void WriteAttributeExtensions(XmlWriter writer, SyndicationItem item, string version)
        {
            SyndicationFeedFormatter.WriteAttributeExtensions(writer, item, version);
        }

        protected static void WriteAttributeExtensions(XmlWriter writer, SyndicationLink link, string version)
        {
            SyndicationFeedFormatter.WriteAttributeExtensions(writer, link, version);
        }

        protected static void WriteAttributeExtensions(XmlWriter writer, SyndicationPerson person, string version)
        {
            SyndicationFeedFormatter.WriteAttributeExtensions(writer, person, version);
        }

        protected void WriteElementExtensions(XmlWriter writer, SyndicationCategory category, string version)
        {
            SyndicationFeedFormatter.WriteElementExtensions(writer, category, version);
        }

        protected static void WriteElementExtensions(XmlWriter writer, SyndicationItem item, string version)
        {
            SyndicationFeedFormatter.WriteElementExtensions(writer, item, version);
        }

        protected void WriteElementExtensions(XmlWriter writer, SyndicationLink link, string version)
        {
            SyndicationFeedFormatter.WriteElementExtensions(writer, link, version);
        }

        protected void WriteElementExtensions(XmlWriter writer, SyndicationPerson person, string version)
        {
            SyndicationFeedFormatter.WriteElementExtensions(writer, person, version);
        }

        public abstract void WriteTo(XmlWriter writer);

        public SyndicationItem Item =>
            this.item;

        public abstract string Version { get; }
    }
}

