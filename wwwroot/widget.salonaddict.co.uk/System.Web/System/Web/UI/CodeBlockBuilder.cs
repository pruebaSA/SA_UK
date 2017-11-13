namespace System.Web.UI
{
    using System;
    using System.Web;

    internal class CodeBlockBuilder : ControlBuilder
    {
        protected CodeBlockType _blockType;
        private int _column;
        protected string _content;

        internal CodeBlockBuilder(CodeBlockType blockType, string content, int lineNumber, int column, VirtualPath virtualPath)
        {
            this._content = content;
            this._blockType = blockType;
            this._column = column;
            base.Line = lineNumber;
            base.VirtualPath = virtualPath;
        }

        public override object BuildObject() => 
            null;

        internal CodeBlockType BlockType =>
            this._blockType;

        internal int Column =>
            this._column;

        internal string Content =>
            this._content;
    }
}

