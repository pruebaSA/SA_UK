namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Metadata.Edm;

    public abstract class DbExpression
    {
        private DbCommandTree _commandTree;
        private DbExpressionKind _kind;
        private TypeUsage _type;
        internal readonly int ObjectId = Interlocked.Increment(ref s_instanceCount);
        private static int s_instanceCount;

        internal DbExpression(DbCommandTree commandTree, DbExpressionKind kind)
        {
            CheckExpressionKind(kind);
            this._commandTree = EntityUtil.CheckArgumentNull<DbCommandTree>(commandTree, "commandTree");
            this._kind = kind;
        }

        public abstract void Accept(DbExpressionVisitor visitor);
        public abstract TResultType Accept<TResultType>(DbExpressionVisitor<TResultType> visitor);
        internal static void CheckExpressionKind(DbExpressionKind kind)
        {
            if ((kind < DbExpressionKind.All) || (DbExpressionKind.VariableReference < kind))
            {
                throw EntityUtil.InvalidEnumerationValue(typeof(DbExpressionKind), (int) kind);
            }
        }

        internal DbExpression Clone()
        {
            using (new EntityBid.ScopeAuto("<cqt.DbExpression.Clone|API> %d#", this.ObjectId))
            {
                return ExpressionCopier.Copy(this.CommandTree, this);
            }
        }

        internal static int GetExpressionKind(DbExpression expression)
        {
            if (expression != null)
            {
                return (int) expression.ExpressionKind;
            }
            return -1;
        }

        internal static int GetObjectId(DbExpression expression)
        {
            if (expression != null)
            {
                return expression.ObjectId;
            }
            return -1;
        }

        internal string Print() => 
            new ExpressionPrinter().Print(this);

        internal static void TraceInfo(DbExpression expression)
        {
            if ((expression != null) && EntityBid.AdvancedOn)
            {
                EntityBid.PutStr(expression.Print());
                EntityBid.Trace("\n");
            }
        }

        internal DbCommandTree CommandTree =>
            this._commandTree;

        public DbExpressionKind ExpressionKind =>
            this._kind;

        public TypeUsage ResultType
        {
            get => 
                this._type;
            internal set
            {
                TypeUsage usage = CommandTreeTypeHelper.SetResultAsNullable(value);
                this._type = usage;
            }
        }
    }
}

