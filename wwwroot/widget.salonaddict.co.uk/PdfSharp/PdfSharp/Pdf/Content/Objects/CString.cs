namespace PdfSharp.Pdf.Content.Objects
{
    using PdfSharp.Pdf.Content;
    using System;
    using System.Diagnostics;
    using System.Text;

    [DebuggerDisplay("({Value})")]
    public class CString : CObject
    {
        private string value;

        public CString Clone() => 
            ((CString) this.Copy());

        protected override CObject Copy() => 
            base.Copy();

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("(");
            int length = this.value.Length;
            for (int i = 0; i < length; i++)
            {
                char ch = this.value[i];
                switch (ch)
                {
                    case '\b':
                    {
                        builder.Append(@"\b");
                        continue;
                    }
                    case '\t':
                    {
                        builder.Append(@"\t");
                        continue;
                    }
                    case '\n':
                    {
                        builder.Append(@"\n");
                        continue;
                    }
                    case '\f':
                    {
                        builder.Append(@"\f");
                        continue;
                    }
                    case '\r':
                    {
                        builder.Append(@"\r");
                        continue;
                    }
                    case '(':
                    {
                        builder.Append(@"\(");
                        continue;
                    }
                    case ')':
                    {
                        builder.Append(@"\)");
                        continue;
                    }
                    case '\\':
                    {
                        builder.Append(@"\\");
                        continue;
                    }
                }
                builder.Append(ch);
            }
            builder.Append(')');
            return builder.ToString();
        }

        internal override void WriteObject(ContentWriter writer)
        {
            writer.WriteRaw(this.ToString());
        }

        public string Value
        {
            get => 
                this.value;
            set
            {
                this.value = value;
            }
        }
    }
}

