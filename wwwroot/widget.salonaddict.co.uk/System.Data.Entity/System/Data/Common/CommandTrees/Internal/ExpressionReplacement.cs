namespace System.Data.Common.CommandTrees.Internal
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees;

    internal class ExpressionReplacement
    {
        private DbExpression _replacement;
        private DbExpression _toReplace;
        private bool _visit;

        internal ExpressionReplacement()
        {
        }

        public DbExpression Current
        {
            get => 
                this._toReplace;
            internal set
            {
                this._toReplace = value;
            }
        }

        public DbExpression Replacement
        {
            get => 
                this._replacement;
            set
            {
                if (value == null)
                {
                    throw EntityUtil.ArgumentNull("Replacement");
                }
                this._replacement = value;
            }
        }

        public bool VisitReplacement
        {
            get => 
                this._visit;
            set
            {
                this._visit = value;
            }
        }
    }
}

