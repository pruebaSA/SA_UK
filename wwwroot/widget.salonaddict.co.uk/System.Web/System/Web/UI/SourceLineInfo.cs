﻿namespace System.Web.UI
{
    using System;

    internal abstract class SourceLineInfo
    {
        private int _line;
        private string _virtualPath;

        protected SourceLineInfo()
        {
        }

        internal int Line
        {
            get => 
                this._line;
            set
            {
                this._line = value;
            }
        }

        internal string VirtualPath
        {
            get => 
                this._virtualPath;
            set
            {
                this._virtualPath = value;
            }
        }
    }
}

