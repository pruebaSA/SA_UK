namespace PdfSharp.Internal
{
    using PdfSharp;
    using PdfSharp.Drawing;
    using System;

    internal static class Calc
    {
        public const double Deg2Rad = 0.017453292519943295;
        public const double πHalf = 1.5707963267948966;

        public static XSize PageSizeToSize(PageSize value)
        {
            switch (value)
            {
                case PageSize.A0:
                    return new XSize(2380.0, 3368.0);

                case PageSize.A1:
                    return new XSize(1684.0, 2380.0);

                case PageSize.A2:
                    return new XSize(1190.0, 1684.0);

                case PageSize.A3:
                    return new XSize(842.0, 1190.0);

                case PageSize.A4:
                    return new XSize(595.0, 842.0);

                case PageSize.A5:
                    return new XSize(420.0, 595.0);

                case PageSize.B4:
                    return new XSize(729.0, 1032.0);

                case PageSize.B5:
                    return new XSize(516.0, 729.0);

                case PageSize.Quarto:
                    return new XSize(610.0, 780.0);

                case PageSize.Executive:
                    return new XSize(540.0, 720.0);

                case PageSize.Letter:
                    return new XSize(612.0, 792.0);

                case PageSize.Legal:
                    return new XSize(612.0, 1008.0);

                case PageSize.Ledger:
                    return new XSize(1224.0, 792.0);

                case PageSize.Tabloid:
                    return new XSize(792.0, 1224.0);

                case PageSize.Folio:
                    return new XSize(612.0, 936.0);

                case PageSize.Statement:
                    return new XSize(396.0, 612.0);

                case PageSize.Size10x14:
                    return new XSize(720.0, 1008.0);
            }
            throw new ArgumentException("Invalid PageSize.");
        }
    }
}

