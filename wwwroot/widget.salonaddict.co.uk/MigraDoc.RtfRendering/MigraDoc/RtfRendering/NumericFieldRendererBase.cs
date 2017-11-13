namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Fields;
    using MigraDoc.RtfRendering.Resources;
    using System;
    using System.Diagnostics;

    internal abstract class NumericFieldRendererBase : FieldRenderer
    {
        private NumericFieldBase numericField;

        internal NumericFieldRendererBase(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.numericField = domObj as NumericFieldBase;
        }

        protected void TranslateFormat()
        {
            switch (this.numericField.Format)
            {
                case "":
                    break;

                case "ROMAN":
                    base.rtfWriter.WriteText(@" \*ROMAN");
                    return;

                case "roman":
                    base.rtfWriter.WriteText(@" \*roman");
                    return;

                case "ALPHABETIC":
                    base.rtfWriter.WriteText(@" \*ALPHABETIC");
                    return;

                case "alphabetic":
                    base.rtfWriter.WriteText(@" \*alphabetic");
                    return;

                default:
                    Trace.WriteLine(Messages.InvalidNumericFieldFormat(this.numericField.Format), "warning");
                    break;
            }
        }
    }
}

