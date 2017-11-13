namespace MigraDoc.DocumentObjectModel.IO
{
    using System;

    internal enum TokenType
    {
        None,
        Identifier,
        KeyWord,
        IntegerLiteral,
        RealLiteral,
        CharacterLiteral,
        StringLiteral,
        OperatorOrPunctuator,
        Text
    }
}

