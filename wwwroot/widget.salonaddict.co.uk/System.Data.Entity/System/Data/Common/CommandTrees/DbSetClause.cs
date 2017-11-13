namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Common.Utils;

    public sealed class DbSetClause : DbModificationClause
    {
        private DbExpression _prop;
        private DbExpression _val;

        internal DbSetClause(DbModificationCommandTree commandTree, DbExpression targetProperty, DbExpression sourceValue) : base(commandTree)
        {
            EntityUtil.CheckArgumentNull<DbExpression>(targetProperty, "targetProperty");
            EntityUtil.CheckArgumentNull<DbExpression>(sourceValue, "sourceValue");
            this._prop = targetProperty;
            this._val = sourceValue;
        }

        internal override DbModificationClause Copy(DbModificationCommandTree newTree)
        {
            DbExpression targetProperty = null;
            DbExpression sourceValue = null;
            if (this.Property != null)
            {
                targetProperty = ExpressionCopier.Copy(newTree, this.Property);
            }
            if (this.Value != null)
            {
                sourceValue = ExpressionCopier.Copy(newTree, this.Value);
            }
            return new DbSetClause(newTree, targetProperty, sourceValue);
        }

        internal override void DumpStructure(ExpressionDumper dumper)
        {
            dumper.Begin("DbSetClause");
            if (this.Property != null)
            {
                dumper.Dump(this.Property, "Property");
            }
            if (this.Value != null)
            {
                dumper.Dump(this.Value, "Value");
            }
            dumper.End("DbSetClause");
        }

        internal override TreeNode Print(DbExpressionVisitor<TreeNode> visitor)
        {
            TreeNode node = new TreeNode("DbSetClause", new TreeNode[0]);
            if (this.Property != null)
            {
                node.Children.Add(new TreeNode("Property", new TreeNode[] { this.Property.Accept<TreeNode>(visitor) }));
            }
            if (this.Value != null)
            {
                node.Children.Add(new TreeNode("Value", new TreeNode[] { this.Value.Accept<TreeNode>(visitor) }));
            }
            return node;
        }

        public DbExpression Property =>
            this._prop;

        public DbExpression Value
        {
            get => 
                this._val;
            internal set
            {
                this._val = value;
            }
        }
    }
}

