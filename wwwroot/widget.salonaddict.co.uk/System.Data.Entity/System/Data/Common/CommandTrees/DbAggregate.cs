namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Metadata.Edm;

    public abstract class DbAggregate
    {
        private ExpressionList _args;
        private DbCommandTree _commandTree;
        private TypeUsage _type;

        internal DbAggregate(DbCommandTree commandTree)
        {
            this._commandTree = EntityUtil.CheckArgumentNull<DbCommandTree>(commandTree, "commandTree");
        }

        internal ExpressionList ArgumentList
        {
            get => 
                this._args;
            set
            {
                this._args = value;
            }
        }

        public IList<DbExpression> Arguments =>
            this._args;

        internal DbCommandTree CommandTree =>
            this._commandTree;

        public TypeUsage ResultType
        {
            get => 
                this._type;
            internal set
            {
                this._type = value;
            }
        }
    }
}

