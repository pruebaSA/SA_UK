namespace PdfSharp.Pdf.Security
{
    using PdfSharp.Pdf;
    using System;

    public abstract class PdfSecurityHandler : PdfDictionary
    {
        internal PdfSecurityHandler(PdfDictionary dict) : base(dict)
        {
        }

        internal PdfSecurityHandler(PdfDocument document) : base(document)
        {
        }

        internal class Keys : KeysBase
        {
            [KeyInfo(KeyType.Optional | KeyType.Dictionary)]
            public const string CF = "/CF";
            [KeyInfo("1.6", KeyType.Optional | KeyType.Name)]
            public const string EFF = "/EFF";
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string Filter = "/Filter";
            [KeyInfo("1.4", KeyType.Optional | KeyType.Integer)]
            public const string Length = "/Length";
            [KeyInfo("1.5", KeyType.Optional | KeyType.Name)]
            public const string StmF = "/StmF";
            [KeyInfo("1.5", KeyType.Optional | KeyType.Name)]
            public const string StrF = "/StrF";
            [KeyInfo("1.3", KeyType.Optional | KeyType.Name)]
            public const string SubFilter = "/SubFilter";
            [KeyInfo(KeyType.Optional | KeyType.Integer)]
            public const string V = "/V";
        }
    }
}

