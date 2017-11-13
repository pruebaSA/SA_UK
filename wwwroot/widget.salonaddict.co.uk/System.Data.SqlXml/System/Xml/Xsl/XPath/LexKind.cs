namespace System.Xml.Xsl.XPath
{
    using System;

    internal enum LexKind
    {
        At = 0x40,
        Axis = 0x61,
        Bang = 0x21,
        Comma = 0x2c,
        Dollar = 0x24,
        Dot = 0x2e,
        DotDot = 0x44,
        Eof = 0x45,
        Eq = 0x3d,
        Ge = 0x47,
        Gt = 0x3e,
        LBrace = 0x7b,
        LBracket = 0x5b,
        Le = 0x4c,
        LParens = 40,
        Lt = 60,
        Minus = 0x2d,
        Name = 110,
        Ne = 0x4e,
        Number = 100,
        Plus = 0x2b,
        RBrace = 0x7d,
        RBracket = 0x5d,
        RParens = 0x29,
        Slash = 0x2f,
        SlashSlash = 0x53,
        Star = 0x2a,
        String = 0x73,
        Union = 0x7c,
        Unknown = 0x55
    }
}

