namespace System.Data.Common.EntitySql
{
    using System;

    internal abstract class AstNode
    {
        private ErrorContext _errCtx;

        internal AstNode()
        {
            this._errCtx = new ErrorContext();
        }

        internal AstNode(string query, int inputPosition)
        {
            this._errCtx = new ErrorContext();
            this._errCtx.QueryText = query;
            this._errCtx.InputPosition = inputPosition;
        }

        internal ErrorContext ErrCtx
        {
            get => 
                this._errCtx;
            set
            {
                this._errCtx = value;
            }
        }
    }
}

