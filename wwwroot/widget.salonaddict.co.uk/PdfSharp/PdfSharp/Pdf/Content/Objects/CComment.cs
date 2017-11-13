namespace PdfSharp.Pdf.Content.Objects
{
    using PdfSharp.Pdf.Content;
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("({Text})")]
    public class CComment : CObject
    {
        private string text;

        public CComment Clone() => 
            ((CComment) this.Copy());

        protected override CObject Copy() => 
            base.Copy();

        public override string ToString() => 
            ("% " + this.text);

        internal override void WriteObject(ContentWriter writer)
        {
            writer.WriteLineRaw(this.ToString());
        }

        public string Text
        {
            get => 
                this.text;
            set
            {
                this.text = value;
            }
        }
    }
}

