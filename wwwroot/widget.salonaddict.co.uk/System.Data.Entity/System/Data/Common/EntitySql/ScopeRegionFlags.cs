namespace System.Data.Common.EntitySql
{
    using System;
    using System.Collections.Generic;

    internal sealed class ScopeRegionFlags
    {
        private HashSet<string> _groupAggregateNames = new HashSet<string>();
        private int _groupAggregateNestingCount;
        private Dictionary<MethodExpr, GroupAggregateInfo> _groupAggregatesInfo = new Dictionary<MethodExpr, GroupAggregateInfo>();
        private bool _isImplicitGroup;
        private bool _isInGroupScope;
        private bool _isInsideGroupAggregate;
        private bool _isInsideJoinOnPredicate;
        private System.Data.Common.EntitySql.ScopeEntryKind _scopeEntryKind = System.Data.Common.EntitySql.ScopeEntryKind.SourceVar;
        private System.Data.Common.EntitySql.SemanticResolver.ScopeViewKind _scopeViewKind;
        private TreePathTagger _treePathTagger = new TreePathTagger();
        private bool _wasNestedGroupAggregateReferredByInnerExpressions;
        private bool _wasResolutionCorrelated;

        internal void AddGroupAggregateInfo(MethodExpr astMethodNode, GroupAggregateInfo groupAggrInfo)
        {
            this._groupAggregatesInfo.Add(astMethodNode, groupAggrInfo);
        }

        internal void AddGroupAggregateToScopeFlags(string groupAggregateName)
        {
            this._groupAggregateNames.Add(groupAggregateName);
        }

        internal bool ContainsGroupAggregate(string groupAggregateName) => 
            this._groupAggregateNames.Contains(groupAggregateName);

        internal void DecrementGroupAggregateNestingCount()
        {
            this._groupAggregateNestingCount--;
        }

        internal void IncrementGroupAggregateNestingCount()
        {
            this._groupAggregateNestingCount++;
        }

        internal void ResetGroupAggregateNestingCount()
        {
            this._groupAggregateNestingCount = 0;
        }

        internal int GroupAggregateNestingCount =>
            this._groupAggregateNestingCount;

        internal Dictionary<MethodExpr, GroupAggregateInfo> GroupAggregatesInfo =>
            this._groupAggregatesInfo;

        internal bool IsImplicitGroup
        {
            get => 
                this._isImplicitGroup;
            set
            {
                this._isImplicitGroup = value;
            }
        }

        internal bool IsInGroupScope
        {
            get => 
                this._isInGroupScope;
            set
            {
                this._isInGroupScope = value;
            }
        }

        internal bool IsInsideGroupAggregate
        {
            get => 
                this._isInsideGroupAggregate;
            set
            {
                this._isInsideGroupAggregate = value;
            }
        }

        internal bool IsInsideJoinOnPredicate
        {
            get => 
                this._isInsideJoinOnPredicate;
            set
            {
                this._isInsideJoinOnPredicate = value;
            }
        }

        internal TreePathTagger PathTagger =>
            this._treePathTagger;

        internal System.Data.Common.EntitySql.ScopeEntryKind ScopeEntryKind
        {
            get => 
                this._scopeEntryKind;
            set
            {
                this._scopeEntryKind = value;
            }
        }

        internal System.Data.Common.EntitySql.SemanticResolver.ScopeViewKind ScopeViewKind
        {
            get => 
                this._scopeViewKind;
            set
            {
                this._scopeViewKind = value;
            }
        }

        internal bool WasNestedGroupAggregateReferredByInnerExpressions
        {
            get => 
                this._wasNestedGroupAggregateReferredByInnerExpressions;
            set
            {
                this._wasNestedGroupAggregateReferredByInnerExpressions = value;
            }
        }

        internal bool WasResolutionCorrelated
        {
            get => 
                this._wasResolutionCorrelated;
            set
            {
                this._wasResolutionCorrelated = value;
            }
        }
    }
}

