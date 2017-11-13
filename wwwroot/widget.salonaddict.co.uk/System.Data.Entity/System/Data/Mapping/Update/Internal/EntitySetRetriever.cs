namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.CommandTrees;
    using System.Data.Metadata.Edm;

    internal class EntitySetRetriever : BasicExpressionVisitor
    {
        private List<EntitySet> _entitySets = new List<EntitySet>();

        private EntitySetRetriever()
        {
        }

        internal static IEnumerable<EntitySet> GetEntitySets(DbExpression tree)
        {
            EntitySetRetriever visitor = new EntitySetRetriever();
            if (tree != null)
            {
                tree.Accept(visitor);
            }
            return visitor._entitySets;
        }

        public override void Visit(DbScanExpression e)
        {
            base.Visit(e);
            EntitySet target = e.Target as EntitySet;
            if (target != null)
            {
                this._entitySets.Add(target);
            }
        }
    }
}

