namespace System.Data.Services.Client
{
    using System;
    using System.Data.Services.Common;
    using System.Reflection;
    using System.Xml;

    internal sealed class EpmSyndicationContentSerializer : EpmContentSerializerBase, IDisposable
    {
        private bool authorInfoPresent;
        private bool authorNamePresent;
        private bool updatedPresent;

        internal EpmSyndicationContentSerializer(EpmTargetTree tree, object element, XmlWriter target) : base(tree, true, element, target)
        {
        }

        private void CreateAuthor(bool createNull)
        {
            if (!this.authorInfoPresent)
            {
                if (createNull)
                {
                    base.Target.WriteStartElement("author", "http://www.w3.org/2005/Atom");
                    base.Target.WriteElementString("name", "http://www.w3.org/2005/Atom", string.Empty);
                    base.Target.WriteEndElement();
                }
                else
                {
                    base.Target.WriteStartElement("author", "http://www.w3.org/2005/Atom");
                }
                this.authorInfoPresent = true;
            }
        }

        private void CreateUpdated()
        {
            if (!this.updatedPresent)
            {
                base.Target.WriteElementString("updated", "http://www.w3.org/2005/Atom", XmlConvert.ToString(DateTime.UtcNow, XmlDateTimeSerializationMode.RoundtripKind));
            }
        }

        public void Dispose()
        {
            this.CreateAuthor(true);
            this.CreateUpdated();
        }

        private void FinishAuthor()
        {
            if (!this.authorNamePresent)
            {
                base.Target.WriteElementString("name", "http://www.w3.org/2005/Atom", string.Empty);
                this.authorNamePresent = true;
            }
            base.Target.WriteEndElement();
        }

        protected override void Serialize(EpmTargetPathSegment targetSegment, EpmSerializationKind kind)
        {
            if (!targetSegment.HasContent)
            {
                if (targetSegment.SegmentName == "author")
                {
                    this.CreateAuthor(false);
                    base.Serialize(targetSegment, kind);
                    this.FinishAuthor();
                }
                else if (targetSegment.SegmentName == "contributor")
                {
                    base.Target.WriteStartElement("contributor", "http://www.w3.org/2005/Atom");
                    base.Serialize(targetSegment, kind);
                    base.Target.WriteEndElement();
                }
            }
            else
            {
                object propertyValue;
                string contentType;
                Action<string> contentWriter;
                EntityPropertyMappingInfo epmInfo = targetSegment.EpmInfo;
                try
                {
                    propertyValue = ClientType.ReadPropertyValue(base.Element, epmInfo.ActualType, epmInfo.Attribute.SourcePath);
                }
                catch (TargetInvocationException)
                {
                    throw;
                }
                switch (epmInfo.Attribute.TargetTextContentKind)
                {
                    case SyndicationTextContentKind.Html:
                    {
                        contentType = "html";
                        XmlWriter target = base.Target;
                        contentWriter = new Action<string>(target.WriteString);
                        break;
                    }
                    case SyndicationTextContentKind.Xhtml:
                    {
                        contentType = "xhtml";
                        XmlWriter writer2 = base.Target;
                        contentWriter = new Action<string>(writer2.WriteRaw);
                        break;
                    }
                    default:
                    {
                        contentType = "text";
                        XmlWriter writer3 = base.Target;
                        contentWriter = new Action<string>(writer3.WriteString);
                        break;
                    }
                }
                Action<string, bool, bool> action = delegate (string c, bool nonTextPossible, bool atomDateConstruct) {
                    this.Target.WriteStartElement(c, "http://www.w3.org/2005/Atom");
                    if (nonTextPossible)
                    {
                        this.Target.WriteAttributeString("type", string.Empty, contentType);
                    }
                    string str = (propertyValue != null) ? ClientConvert.ToString(propertyValue, atomDateConstruct) : (atomDateConstruct ? ClientConvert.ToString(DateTime.MinValue, atomDateConstruct) : string.Empty);
                    contentWriter(str);
                    this.Target.WriteEndElement();
                };
                switch (epmInfo.Attribute.TargetSyndicationItem)
                {
                    case SyndicationItemProperty.AuthorEmail:
                    case SyndicationItemProperty.ContributorEmail:
                        action("email", false, false);
                        return;

                    case SyndicationItemProperty.AuthorName:
                    case SyndicationItemProperty.ContributorName:
                        action("name", false, false);
                        this.authorNamePresent = true;
                        return;

                    case SyndicationItemProperty.AuthorUri:
                    case SyndicationItemProperty.ContributorUri:
                        action("uri", false, false);
                        return;

                    case SyndicationItemProperty.Updated:
                        action("updated", false, true);
                        this.updatedPresent = true;
                        return;

                    case SyndicationItemProperty.Published:
                        action("published", false, true);
                        return;

                    case SyndicationItemProperty.Rights:
                        action("rights", true, false);
                        return;

                    case SyndicationItemProperty.Summary:
                        action("summary", true, false);
                        return;

                    case SyndicationItemProperty.Title:
                        action("title", true, false);
                        return;
                }
            }
        }
    }
}

