namespace System.Data.Services.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Linq;

    internal sealed class EpmSourceTree
    {
        private readonly EpmTargetTree epmTargetTree;
        private readonly EpmSourcePathSegment root = new EpmSourcePathSegment("");

        internal EpmSourceTree(EpmTargetTree epmTargetTree)
        {
            this.epmTargetTree = epmTargetTree;
        }

        internal void Add(EntityPropertyMappingInfo epmInfo)
        {
            string sourcePath = epmInfo.Attribute.SourcePath;
            EpmSourcePathSegment root = this.Root;
            IList<EpmSourcePathSegment> subProperties = root.SubProperties;
            EpmSourcePathSegment segment2 = null;
            Func<EpmSourcePathSegment, bool> predicate = null;
            foreach (string propertyName in sourcePath.Split(new char[] { '/' }))
            {
                if (propertyName.Length == 0)
                {
                    throw new InvalidOperationException(System.Data.Services.Client.Strings.EpmSourceTree_InvalidSourcePath(epmInfo.DefiningType.Name, sourcePath));
                }
                if (predicate == null)
                {
                    predicate = e => e.PropertyName == propertyName;
                }
                segment2 = subProperties.SingleOrDefault<EpmSourcePathSegment>(predicate);
                if (segment2 != null)
                {
                    root = segment2;
                }
                else
                {
                    root = new EpmSourcePathSegment(propertyName);
                    subProperties.Add(root);
                }
                subProperties = root.SubProperties;
            }
            if (segment2 != null)
            {
                if (segment2.EpmInfo.DefiningType.Name == epmInfo.DefiningType.Name)
                {
                    throw new InvalidOperationException(System.Data.Services.Client.Strings.EpmSourceTree_DuplicateEpmAttrsWithSameSourceName(epmInfo.Attribute.SourcePath, epmInfo.DefiningType.Name));
                }
                this.epmTargetTree.Remove(segment2.EpmInfo);
            }
            root.EpmInfo = epmInfo;
            this.epmTargetTree.Add(epmInfo);
        }

        internal EpmSourcePathSegment Root =>
            this.root;
    }
}

