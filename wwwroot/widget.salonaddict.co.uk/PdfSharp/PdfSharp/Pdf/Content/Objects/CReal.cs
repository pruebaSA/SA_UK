namespace PdfSharp.Pdf.Content.Objects
{
    using PdfSharp.Pdf.Content;
    using System;
    using System.Diagnostics;
    using System.Globalization;

    [DebuggerDisplay("({Value})")]
    public class CReal : CNumber
    {
        private double value;

        public CReal Clone() => 
            ((CReal) this.Copy());

        protected override CObject Copy() => 
            base.Copy();

        public override string ToString() => 
            this.value.ToString(CultureInfo.InvariantCulture);

        internal override void WriteObject(ContentWriter writer)
        {
            writer.WriteRaw(this.ToString() + " ");
        }

        public double Value
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

