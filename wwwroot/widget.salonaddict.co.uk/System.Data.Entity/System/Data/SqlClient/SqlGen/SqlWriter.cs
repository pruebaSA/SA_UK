namespace System.Data.SqlClient.SqlGen
{
    using System;
    using System.IO;
    using System.Text;

    internal class SqlWriter : StringWriter
    {
        private bool atBeginningOfLine;
        private int indent;

        public SqlWriter(StringBuilder b) : base(b, CultureInfo.InvariantCulture)
        {
            this.indent = -1;
            this.atBeginningOfLine = true;
        }

        public override void Write(string value)
        {
            if (value == "\r\n")
            {
                base.WriteLine();
                this.atBeginningOfLine = true;
            }
            else
            {
                if (this.atBeginningOfLine)
                {
                    if (this.indent > 0)
                    {
                        base.Write(new string('\t', this.indent));
                    }
                    this.atBeginningOfLine = false;
                }
                base.Write(value);
            }
        }

        public override void WriteLine()
        {
            base.WriteLine();
            this.atBeginningOfLine = true;
        }

        internal int Indent
        {
            get => 
                this.indent;
            set
            {
                this.indent = value;
            }
        }
    }
}

