namespace System.Data.Objects.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Runtime.InteropServices;

    internal class ObjectFullSpanRewriter : ObjectSpanRewriter
    {
        private Stack<SpanPathInfo> _currentSpanPath;

        internal ObjectFullSpanRewriter(DbExpression toRewrite, Span span) : base(toRewrite)
        {
            this._currentSpanPath = new Stack<SpanPathInfo>();
            EntityType entityType = null;
            if (!TryGetEntityType(base.Query.ResultType, out entityType))
            {
                throw EntityUtil.InvalidOperation(Strings.ObjectQuery_Span_IncludeRequiresEntityOrEntityCollection);
            }
            SpanPathInfo parentInfo = new SpanPathInfo(entityType);
            foreach (Span.SpanPath path in span.SpanList)
            {
                this.AddSpanPath(parentInfo, path.Navigations);
            }
            this._currentSpanPath.Push(parentInfo);
        }

        private void AddSpanPath(SpanPathInfo parentInfo, List<string> navPropNames)
        {
            this.ConvertSpanPath(parentInfo, navPropNames, 0);
        }

        private void ConvertSpanPath(SpanPathInfo parentInfo, List<string> navPropNames, int pos)
        {
            NavigationProperty item = null;
            if (!parentInfo.DeclaringType.NavigationProperties.TryGetValue(navPropNames[pos], true, out item))
            {
                throw EntityUtil.InvalidOperation(Strings.ObjectQuery_Span_NoNavProp(parentInfo.DeclaringType.FullName, navPropNames[pos]));
            }
            if (parentInfo.Children == null)
            {
                parentInfo.Children = new Dictionary<NavigationProperty, SpanPathInfo>();
            }
            SpanPathInfo info = null;
            if (!parentInfo.Children.TryGetValue(item, out info))
            {
                info = new SpanPathInfo(EntityTypeFromResultType(item));
                parentInfo.Children[item] = info;
            }
            if (pos < (navPropNames.Count - 1))
            {
                this.ConvertSpanPath(info, navPropNames, pos + 1);
            }
        }

        internal override ObjectSpanRewriter.SpanTrackingInfo CreateEntitySpanTrackingInfo(DbExpression expression, EntityType entityType)
        {
            ObjectSpanRewriter.SpanTrackingInfo info = new ObjectSpanRewriter.SpanTrackingInfo();
            SpanPathInfo info2 = this._currentSpanPath.Peek();
            if (info2.Children != null)
            {
                int num = 1;
                foreach (KeyValuePair<NavigationProperty, SpanPathInfo> pair in info2.Children)
                {
                    if (info.ColumnDefinitions == null)
                    {
                        info = base.InitializeTrackingInfo(base.RelationshipSpan);
                    }
                    DbExpression expression2 = expression.CommandTree.CreatePropertyExpression(pair.Key, expression.Clone());
                    this._currentSpanPath.Push(pair.Value);
                    expression2 = base.Rewrite(expression2);
                    this._currentSpanPath.Pop();
                    info.ColumnDefinitions.Add(new KeyValuePair<string, DbExpression>(info.ColumnNames.Next(), expression2));
                    AssociationEndMember navigationPropertyTargetEnd = this.GetNavigationPropertyTargetEnd(pair.Key);
                    info.SpannedColumns[num] = navigationPropertyTargetEnd;
                    if (base.RelationshipSpan)
                    {
                        info.FullSpannedEnds[navigationPropertyTargetEnd] = true;
                    }
                    num++;
                }
            }
            return info;
        }

        private static EntityType EntityTypeFromResultType(NavigationProperty navProp)
        {
            EntityType entityType = null;
            TryGetEntityType(navProp.TypeUsage, out entityType);
            return entityType;
        }

        private AssociationEndMember GetNavigationPropertyTargetEnd(NavigationProperty property) => 
            base.Metadata.GetItem<AssociationType>(property.RelationshipType.FullName, 1).AssociationEndMembers[property.ToEndMember.Name];

        private static bool TryGetEntityType(TypeUsage resultType, out EntityType entityType)
        {
            if (BuiltInTypeKind.EntityType == resultType.EdmType.BuiltInTypeKind)
            {
                entityType = (EntityType) resultType.EdmType;
                return true;
            }
            if (BuiltInTypeKind.CollectionType == resultType.EdmType.BuiltInTypeKind)
            {
                EdmType edmType = ((CollectionType) resultType.EdmType).TypeUsage.EdmType;
                if (BuiltInTypeKind.EntityType == edmType.BuiltInTypeKind)
                {
                    entityType = (EntityType) edmType;
                    return true;
                }
            }
            entityType = null;
            return false;
        }

        private class SpanPathInfo
        {
            internal Dictionary<NavigationProperty, ObjectFullSpanRewriter.SpanPathInfo> Children;
            internal EntityType DeclaringType;

            internal SpanPathInfo(EntityType declaringType)
            {
                this.DeclaringType = declaringType;
            }
        }
    }
}

