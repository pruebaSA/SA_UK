﻿namespace System.CodeDom
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true)]
    public class CodeLinePragma
    {
        private string fileName;
        private int lineNumber;

        public CodeLinePragma()
        {
        }

        public CodeLinePragma(string fileName, int lineNumber)
        {
            this.FileName = fileName;
            this.LineNumber = lineNumber;
        }

        public string FileName
        {
            get
            {
                if (this.fileName != null)
                {
                    return this.fileName;
                }
                return string.Empty;
            }
            set
            {
                this.fileName = value;
            }
        }

        public int LineNumber
        {
            get => 
                this.lineNumber;
            set
            {
                this.lineNumber = value;
            }
        }
    }
}

