namespace PdfSharp.Pdf
{
    using System;

    public class KeysBase
    {
        internal static DictionaryMeta CreateMeta(Type type) => 
            new DictionaryMeta(type);
    }
}

