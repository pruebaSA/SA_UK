namespace System.Data.Services.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal sealed class EpmTargetTree
    {
        private int countOfNonContentProperties;

        internal EpmTargetTree()
        {
            this.SyndicationRoot = new EpmTargetPathSegment();
            this.NonSyndicationRoot = new EpmTargetPathSegment();
        }

        internal void Add(EntityPropertyMappingInfo epmInfo)
        {
            string targetPath = epmInfo.Attribute.TargetPath;
            bool isSyndication = epmInfo.Attribute.TargetSyndicationItem != SyndicationItemProperty.CustomProperty;
            string namespaceUri = epmInfo.Attribute.TargetNamespaceUri;
            string targetNamespacePrefix = epmInfo.Attribute.TargetNamespacePrefix;
            EpmTargetPathSegment parentSegment = isSyndication ? this.SyndicationRoot : this.NonSyndicationRoot;
            IList<EpmTargetPathSegment> subSegments = parentSegment.SubSegments;
            string[] strArray = targetPath.Split(new char[] { '/' });
            for (int i = 0; i < strArray.Length; i++)
            {
                string targetSegment = strArray[i];
                if (targetSegment.Length == 0)
                {
                    throw new InvalidOperationException(System.Data.Services.Client.Strings.EpmTargetTree_InvalidTargetPath(targetPath));
                }
                if ((targetSegment[0] == '@') && (i != (strArray.Length - 1)))
                {
                    throw new InvalidOperationException(System.Data.Services.Client.Strings.EpmTargetTree_AttributeInMiddle(targetSegment));
                }
                EpmTargetPathSegment segment2 = subSegments.SingleOrDefault<EpmTargetPathSegment>(delegate (EpmTargetPathSegment segment) {
                    if (segment.SegmentName != targetSegment)
                    {
                        return false;
                    }
                    if (!isSyndication)
                    {
                        return segment.SegmentNamespaceUri == namespaceUri;
                    }
                    return true;
                });
                if (segment2 != null)
                {
                    parentSegment = segment2;
                }
                else
                {
                    parentSegment = new EpmTargetPathSegment(targetSegment, namespaceUri, targetNamespacePrefix, parentSegment);
                    if (targetSegment[0] == '@')
                    {
                        subSegments.Insert(0, parentSegment);
                    }
                    else
                    {
                        subSegments.Add(parentSegment);
                    }
                }
                subSegments = parentSegment.SubSegments;
            }
            if (parentSegment.HasContent)
            {
                throw new ArgumentException(System.Data.Services.Client.Strings.EpmTargetTree_DuplicateEpmAttrsWithSameTargetName(GetPropertyNameFromEpmInfo(parentSegment.EpmInfo), parentSegment.EpmInfo.DefiningType.Name, parentSegment.EpmInfo.Attribute.SourcePath, epmInfo.Attribute.SourcePath));
            }
            if (!epmInfo.Attribute.KeepInContent)
            {
                this.countOfNonContentProperties++;
            }
            parentSegment.EpmInfo = epmInfo;
            if (HasMixedContent(this.NonSyndicationRoot, false))
            {
                throw new InvalidOperationException(System.Data.Services.Client.Strings.EpmTargetTree_InvalidTargetPath(targetPath));
            }
        }

        private static string GetPropertyNameFromEpmInfo(EntityPropertyMappingInfo epmInfo)
        {
            if (epmInfo.Attribute.TargetSyndicationItem != SyndicationItemProperty.CustomProperty)
            {
                return epmInfo.Attribute.TargetSyndicationItem.ToString();
            }
            return epmInfo.Attribute.TargetPath;
        }

        private static bool HasMixedContent(EpmTargetPathSegment currentSegment, bool ancestorHasContent)
        {
            foreach (EpmTargetPathSegment segment in from s in currentSegment.SubSegments
                where !s.IsAttribute
                select s)
            {
                if (segment.HasContent && ancestorHasContent)
                {
                    return true;
                }
                if (HasMixedContent(segment, segment.HasContent || ancestorHasContent))
                {
                    return true;
                }
            }
            return false;
        }

        internal void Remove(EntityPropertyMappingInfo epmInfo)
        {
            string targetPath = epmInfo.Attribute.TargetPath;
            bool isSyndication = epmInfo.Attribute.TargetSyndicationItem != SyndicationItemProperty.CustomProperty;
            string namespaceUri = epmInfo.Attribute.TargetNamespaceUri;
            EpmTargetPathSegment item = isSyndication ? this.SyndicationRoot : this.NonSyndicationRoot;
            List<EpmTargetPathSegment> subSegments = item.SubSegments;
            string[] strArray = targetPath.Split(new char[] { '/' });
            for (int i = 0; i < strArray.Length; i++)
            {
                string targetSegment = strArray[i];
                if (targetSegment.Length == 0)
                {
                    throw new InvalidOperationException(System.Data.Services.Client.Strings.EpmTargetTree_InvalidTargetPath(targetPath));
                }
                if ((targetSegment[0] == '@') && (i != (strArray.Length - 1)))
                {
                    throw new InvalidOperationException(System.Data.Services.Client.Strings.EpmTargetTree_AttributeInMiddle(targetSegment));
                }
                EpmTargetPathSegment segment2 = subSegments.FirstOrDefault<EpmTargetPathSegment>(delegate (EpmTargetPathSegment segment) {
                    if (segment.SegmentName != targetSegment)
                    {
                        return false;
                    }
                    if (!isSyndication)
                    {
                        return segment.SegmentNamespaceUri == namespaceUri;
                    }
                    return true;
                });
                if (segment2 != null)
                {
                    item = segment2;
                }
                else
                {
                    return;
                }
                subSegments = item.SubSegments;
            }
            if (item.HasContent)
            {
                if (!item.EpmInfo.Attribute.KeepInContent)
                {
                    this.countOfNonContentProperties--;
                }
                do
                {
                    EpmTargetPathSegment parentSegment = item.ParentSegment;
                    parentSegment.SubSegments.Remove(item);
                    item = parentSegment;
                }
                while (((item.ParentSegment != null) && !item.HasContent) && (item.SubSegments.Count == 0));
            }
        }

        internal bool IsV1Compatible =>
            (this.countOfNonContentProperties == 0);

        internal EpmTargetPathSegment NonSyndicationRoot { get; private set; }

        internal EpmTargetPathSegment SyndicationRoot { get; private set; }
    }
}

