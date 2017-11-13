namespace PdfSharp.Pdf.Security
{
    using System;

    [Flags]
    internal enum PdfUserAccessPermission
    {
        PermitAccessibilityExtractContent = 0x200,
        PermitAll = -3,
        PermitAnnotations = 0x20,
        PermitAssembleDocument = 0x400,
        PermitExtractContent = 0x10,
        PermitFormsFill = 0x100,
        PermitFullQualityPrint = 0x800,
        PermitModifyDocument = 8,
        PermitPrint = 4
    }
}

