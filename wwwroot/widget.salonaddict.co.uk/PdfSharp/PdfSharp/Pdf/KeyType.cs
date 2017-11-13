namespace PdfSharp.Pdf
{
    using System;

    [Flags]
    internal enum KeyType
    {
        Array = 8,
        ArrayOrDictionary = 0x30,
        ArrayOrNameOrString = 0x60,
        Boolean = 3,
        Date = 6,
        Dictionary = 9,
        Function = 12,
        FunctionOrName = 0x70,
        Inheritable = 0x400,
        Integer = 4,
        MustBeIndirect = 0x1000,
        MustNotBeIndirect = 0x2000,
        Name = 1,
        NameOrArray = 0x10,
        NameOrDictionary = 0x20,
        NumberTree = 11,
        Optional = 0x100,
        Real = 5,
        Rectangle = 7,
        Required = 0x200,
        Stream = 10,
        StreamOrArray = 0x40,
        StreamOrName = 80,
        String = 2,
        TextString = 13,
        TypeMask = 0xff,
        Various = 0x80
    }
}

