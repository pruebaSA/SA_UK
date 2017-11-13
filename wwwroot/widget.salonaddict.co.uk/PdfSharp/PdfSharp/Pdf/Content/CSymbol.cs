namespace PdfSharp.Pdf.Content
{
    using System;

    internal enum CSymbol
    {
        None,
        Comment,
        Integer,
        Real,
        String,
        HexString,
        UnicodeString,
        UnicodeHexString,
        Name,
        Operator,
        BeginArray,
        EndArray,
        Eof
    }
}

