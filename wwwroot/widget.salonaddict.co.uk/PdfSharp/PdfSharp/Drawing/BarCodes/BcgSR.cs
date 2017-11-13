namespace PdfSharp.Drawing.BarCodes
{
    using System;

    internal class BcgSR
    {
        internal static string DataMatrixInvalid(int columns, int rows) => 
            string.Format("'{1}'x'{0}' is an invalid ecc200 DataMatrix size.", columns, rows);

        internal static string Invalid2Of5Code(string code) => 
            $"'{code}' is not a valid code for an interleave 2 of 5 bar code. It can only represent an even number of digits.";

        internal static string Invalid3Of9Code(string code) => 
            $"'{code}' is not a valid code for a 3 of 9 standard bar code.";

        internal static string InvalidMarkName(string name) => 
            $"'{name}' is not a valid mark name for this OMR representation.";

        internal static string BarCodeNotSet =>
            "A text must be set before rendering the bar code.";

        internal static string DataMatrixNotSupported =>
            "Zero sizes, odd sizes and other than ecc200 coded DataMatrix is not supported.";

        internal static string DataMatrixNull =>
            "No DataMatrix code is produced.";

        internal static string DataMatrixTooBig =>
            "The given data and encoding combination is too big for the matrix size.";

        internal static string EmptyBarCodeSize =>
            "A non-empty size must be set before rendering the bar code.";

        internal static string Invalid2of5Relation =>
            "Value of relation between thick and thin lines on the interleaved 2 of 5 code must be between 2 and 3.";

        internal static string OmrAlreadyInitialized =>
            "Mark descriptions cannot be set when marks have already been set on OMR.";
    }
}

