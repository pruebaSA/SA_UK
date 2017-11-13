namespace PdfSharp.Pdf.Content.Objects
{
    using PdfSharp.Pdf.Content;
    using System;
    using System.Diagnostics;
    using System.Globalization;

    [DebuggerDisplay("({Value})")]
    public class CInteger : CNumber
    {
        private int value;

        public CInteger Clone() => 
            ((CInteger) this.Copy());

        protected override CObject Copy() => 
            base.Copy();

        public override string ToString() => 
            this.value.ToString(CultureInfo.InvariantCulture);

        internal override void WriteObject(ContentWriter writer)
        {
            writer.WriteRaw(this.ToString() + " ");
        }

        public int Value
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

