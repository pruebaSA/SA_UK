namespace System.Data.Services.Common
{
    using System;
    using System.Data.Services.Client;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
    public sealed class EntityPropertyMappingAttribute : Attribute
    {
        private const string AtomNamespacePrefix = "atom";
        private readonly bool keepInContent;
        private readonly string sourcePath;
        private readonly string targetNamespacePrefix;
        private readonly string targetNamespaceUri;
        private readonly string targetPath;
        private readonly SyndicationItemProperty targetSyndicationItem;
        private readonly SyndicationTextContentKind targetTextContentKind;

        public EntityPropertyMappingAttribute(string sourcePath, SyndicationItemProperty targetSyndicationItem, SyndicationTextContentKind targetTextContentKind, bool keepInContent)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("sourcePath"));
            }
            this.sourcePath = sourcePath;
            this.targetPath = SyndicationItemPropertyToPath(targetSyndicationItem);
            this.targetSyndicationItem = targetSyndicationItem;
            this.targetTextContentKind = targetTextContentKind;
            this.targetNamespacePrefix = "atom";
            this.targetNamespaceUri = "http://www.w3.org/2005/Atom";
            this.keepInContent = keepInContent;
        }

        public EntityPropertyMappingAttribute(string sourcePath, string targetPath, string targetNamespacePrefix, string targetNamespaceUri, bool keepInContent)
        {
            Uri uri;
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("sourcePath"));
            }
            this.sourcePath = sourcePath;
            if (string.IsNullOrEmpty(targetPath))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("targetPath"));
            }
            if (targetPath[0] == '@')
            {
                throw new ArgumentException(Strings.EpmTargetTree_InvalidTargetPath(targetPath));
            }
            this.targetPath = targetPath;
            this.targetSyndicationItem = SyndicationItemProperty.CustomProperty;
            this.targetTextContentKind = SyndicationTextContentKind.Plaintext;
            this.targetNamespacePrefix = targetNamespacePrefix;
            if (string.IsNullOrEmpty(targetNamespaceUri))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("targetNamespaceUri"));
            }
            this.targetNamespaceUri = targetNamespaceUri;
            if (!Uri.TryCreate(targetNamespaceUri, UriKind.Absolute, out uri))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_TargetNamespaceUriNotValid(targetNamespaceUri));
            }
            this.keepInContent = keepInContent;
        }

        internal static string SyndicationItemPropertyToPath(SyndicationItemProperty targetSyndicationItem)
        {
            switch (targetSyndicationItem)
            {
                case SyndicationItemProperty.AuthorEmail:
                    return "author/email";

                case SyndicationItemProperty.AuthorName:
                    return "author/name";

                case SyndicationItemProperty.AuthorUri:
                    return "author/uri";

                case SyndicationItemProperty.ContributorEmail:
                    return "contributor/email";

                case SyndicationItemProperty.ContributorName:
                    return "contributor/name";

                case SyndicationItemProperty.ContributorUri:
                    return "contributor/uri";

                case SyndicationItemProperty.Updated:
                    return "updated";

                case SyndicationItemProperty.Published:
                    return "published";

                case SyndicationItemProperty.Rights:
                    return "rights";

                case SyndicationItemProperty.Summary:
                    return "summary";

                case SyndicationItemProperty.Title:
                    return "title";
            }
            throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("targetSyndicationItem"));
        }

        public bool KeepInContent =>
            this.keepInContent;

        public string SourcePath =>
            this.sourcePath;

        public string TargetNamespacePrefix =>
            this.targetNamespacePrefix;

        public string TargetNamespaceUri =>
            this.targetNamespaceUri;

        public string TargetPath =>
            this.targetPath;

        public SyndicationItemProperty TargetSyndicationItem =>
            this.targetSyndicationItem;

        public SyndicationTextContentKind TargetTextContentKind =>
            this.targetTextContentKind;
    }
}

