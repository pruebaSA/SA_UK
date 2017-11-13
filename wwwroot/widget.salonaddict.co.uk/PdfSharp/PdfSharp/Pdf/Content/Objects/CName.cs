namespace PdfSharp.Pdf.Content.Objects
{
    using PdfSharp;
    using PdfSharp.Pdf.Content;
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("({Name})")]
    public class CName : CObject
    {
        private string name;

        public CName()
        {
            this.name = "/";
        }

        public CName(string name)
        {
            this.Name = name;
        }

        public CName Clone() => 
            ((CName) this.Copy());

        protected override CObject Copy() => 
            base.Copy();

        public override string ToString() => 
            this.name;

        internal override void WriteObject(ContentWriter writer)
        {
            writer.WriteRaw(this.ToString() + " ");
        }

        public string Name
        {
            get => 
                this.name;
            set
            {
                if ((this.name == null) || (this.name.Length == 0))
                {
                    throw new ArgumentNullException("name");
                }
                if (this.name[0] != '/')
                {
                    throw new ArgumentException(PSSR.NameMustStartWithSlash);
                }
                this.name = value;
            }
        }
    }
}

