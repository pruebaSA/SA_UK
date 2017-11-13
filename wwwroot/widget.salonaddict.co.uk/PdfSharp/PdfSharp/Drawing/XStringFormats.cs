namespace PdfSharp.Drawing
{
    using System;

    public static class XStringFormats
    {
        public static XStringFormat BottomCenter =>
            new XStringFormat { 
                Alignment=XStringAlignment.Center,
                LineAlignment=XLineAlignment.Far
            };

        public static XStringFormat Center =>
            new XStringFormat { 
                Alignment=XStringAlignment.Center,
                LineAlignment=XLineAlignment.Center
            };

        public static XStringFormat Default =>
            new XStringFormat { LineAlignment=XLineAlignment.BaseLine };

        public static XStringFormat TopCenter =>
            new XStringFormat { 
                Alignment=XStringAlignment.Center,
                LineAlignment=XLineAlignment.Near
            };

        public static XStringFormat TopLeft =>
            new XStringFormat { 
                Alignment=XStringAlignment.Near,
                LineAlignment=XLineAlignment.Near
            };
    }
}

