namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    internal sealed class PdfExtGStateTable : PdfResourceTable
    {
        private Dictionary<string, PdfExtGState> alphaValues;
        private Dictionary<string, PdfExtGState> nonStrokeAlphaValues;
        private Dictionary<string, PdfExtGState> strokeAlphaValues;

        public PdfExtGStateTable(PdfDocument document) : base(document)
        {
            this.alphaValues = new Dictionary<string, PdfExtGState>();
            this.strokeAlphaValues = new Dictionary<string, PdfExtGState>();
            this.nonStrokeAlphaValues = new Dictionary<string, PdfExtGState>();
        }

        public PdfExtGState GetExtGState(double alpha)
        {
            PdfExtGState state;
            string key = MakeKey(alpha);
            if (!this.alphaValues.TryGetValue(key, out state))
            {
                state = new PdfExtGState(base.owner) {
                    Elements = { 
                        ["/CA"] = new PdfReal(alpha),
                        ["/ca"] = new PdfReal(alpha)
                    }
                };
                this.alphaValues[key] = state;
            }
            return state;
        }

        public PdfExtGState GetExtGStateNonStroke(double alpha)
        {
            PdfExtGState state;
            string key = MakeKey(alpha);
            if (!this.nonStrokeAlphaValues.TryGetValue(key, out state))
            {
                state = new PdfExtGState(base.owner) {
                    Elements = { ["/ca"] = new PdfReal(alpha) }
                };
                this.nonStrokeAlphaValues[key] = state;
            }
            return state;
        }

        public PdfExtGState GetExtGStateStroke(double alpha)
        {
            PdfExtGState state;
            string key = MakeKey(alpha);
            if (!this.strokeAlphaValues.TryGetValue(key, out state))
            {
                state = new PdfExtGState(base.owner) {
                    Elements = { ["/CA"] = new PdfReal(alpha) }
                };
                this.strokeAlphaValues[key] = state;
            }
            return state;
        }

        private static string MakeKey(double alpha)
        {
            int num = (int) (1000.0 * alpha);
            return num.ToString(CultureInfo.InvariantCulture);
        }
    }
}

