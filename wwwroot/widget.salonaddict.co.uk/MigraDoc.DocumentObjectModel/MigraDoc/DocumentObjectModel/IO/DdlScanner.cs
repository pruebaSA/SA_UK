namespace MigraDoc.DocumentObjectModel.IO
{
    using MigraDoc.DocumentObjectModel;
    using System;
    using System.Globalization;

    internal class DdlScanner
    {
        private char currChar;
        private int ddlLength;
        private bool emptyLine;
        private DdlReaderErrors errors;
        private string m_DocumentFileName;
        private string m_DocumentPath;
        private int m_idx;
        private int m_idxLine;
        private int m_idxLinePos;
        private int m_nCurDocumentIndex;
        private int m_nCurDocumentLine;
        private int m_nCurDocumentLinePos;
        private string m_strDocument;
        private char nextChar;
        private MigraDoc.DocumentObjectModel.IO.Symbol prevSymbol;
        private MigraDoc.DocumentObjectModel.IO.Symbol symbol;
        private string token;
        private MigraDoc.DocumentObjectModel.IO.TokenType tokenType;

        internal DdlScanner(string ddl, DdlReaderErrors errors) : this("", ddl, errors)
        {
        }

        internal DdlScanner(string documentFileName, string ddl, DdlReaderErrors errors)
        {
            this.token = "";
            this.errors = errors;
            this.Init(ddl, documentFileName);
        }

        internal char AppendAndScanNextChar()
        {
            this.token = this.token + this.currChar;
            return this.ScanNextChar();
        }

        internal void AppendAndScanToEol()
        {
            for (char ch = this.ScanNextChar(); ((ch != '\0') && (ch != '\r')) && (ch != '\n'); ch = this.ScanNextChar())
            {
                this.token = this.token + this.currChar;
            }
        }

        internal int GetTokenValueAsInt()
        {
            if (this.symbol == MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral)
            {
                return int.Parse(this.token, CultureInfo.InvariantCulture);
            }
            if (this.symbol == MigraDoc.DocumentObjectModel.IO.Symbol.HexIntegerLiteral)
            {
                return int.Parse(this.token.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            }
            return 0;
        }

        internal double GetTokenValueAsReal() => 
            double.Parse(this.token, CultureInfo.InvariantCulture);

        internal uint GetTokenValueAsUInt()
        {
            if (this.symbol == MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral)
            {
                return uint.Parse(this.token, CultureInfo.InvariantCulture);
            }
            if (this.symbol == MigraDoc.DocumentObjectModel.IO.Symbol.HexIntegerLiteral)
            {
                return uint.Parse(this.token.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            }
            return 0;
        }

        private bool IgnoreLineBreak()
        {
            switch (this.prevSymbol)
            {
                case MigraDoc.DocumentObjectModel.IO.Symbol.Tab:
                case MigraDoc.DocumentObjectModel.IO.Symbol.LineBreak:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Space:
                    return true;
            }
            return false;
        }

        internal bool Init(string document, string documentFileName)
        {
            this.m_DocumentPath = documentFileName;
            this.m_strDocument = document;
            this.ddlLength = this.m_strDocument.Length;
            this.m_idx = 0;
            this.m_idxLine = 1;
            this.m_idxLinePos = 0;
            this.m_DocumentFileName = documentFileName;
            this.m_nCurDocumentIndex = this.m_idx;
            this.m_nCurDocumentLine = this.m_idxLine;
            this.m_nCurDocumentLinePos = this.m_idxLinePos;
            this.ScanNextChar();
            return true;
        }

        internal static bool IsDigit(char ch) => 
            char.IsDigit(ch);

        internal static bool IsDocumentElement(MigraDoc.DocumentObjectModel.IO.Symbol symbol)
        {
            switch (symbol)
            {
                case MigraDoc.DocumentObjectModel.IO.Symbol.Image:
                case MigraDoc.DocumentObjectModel.IO.Symbol.TextFrame:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Chart:
                case MigraDoc.DocumentObjectModel.IO.Symbol.PageBreak:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Barcode:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Paragraph:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Table:
                    return true;
            }
            return false;
        }

        internal static bool IsEof(char ch) => 
            (ch == '\0');

        internal static bool IsFootnoteElement(MigraDoc.DocumentObjectModel.IO.Symbol symbol)
        {
            if (!IsParagraphElement(symbol))
            {
                return false;
            }
            if (symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Footnote)
            {
                return false;
            }
            return true;
        }

        internal static bool IsHeaderFooterElement(MigraDoc.DocumentObjectModel.IO.Symbol symbol)
        {
            if (!IsParagraphElement(symbol))
            {
                if (!IsDocumentElement(symbol))
                {
                    return false;
                }
                if (symbol == MigraDoc.DocumentObjectModel.IO.Symbol.PageBreak)
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool IsHexDigit(char ch) => 
            ((char.IsDigit(ch) || ((ch >= 'A') && (ch <= 'F'))) || ((ch >= 'a') && (ch <= 'f')));

        internal static bool IsIdentifierChar(char ch, bool firstChar)
        {
            if (firstChar)
            {
                return (char.IsLetter(ch) | (ch == '_'));
            }
            return (char.IsLetterOrDigit(ch) | (ch == '_'));
        }

        internal static bool IsLetter(char ch) => 
            char.IsLetter(ch);

        internal static bool IsOctDigit(char ch) => 
            (char.IsDigit(ch) && (ch < '8'));

        internal static bool IsParagraphElement(MigraDoc.DocumentObjectModel.IO.Symbol symbol)
        {
            switch (symbol)
            {
                case MigraDoc.DocumentObjectModel.IO.Symbol.Bold:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Italic:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Underline:
                case MigraDoc.DocumentObjectModel.IO.Symbol.FontSize:
                case MigraDoc.DocumentObjectModel.IO.Symbol.FontColor:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Font:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Hyperlink:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Text:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Blank:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Tab:
                case MigraDoc.DocumentObjectModel.IO.Symbol.SoftHyphen:
                case MigraDoc.DocumentObjectModel.IO.Symbol.LineBreak:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Space:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Field:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Symbol:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Chr:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Image:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Footnote:
                    return true;
            }
            return false;
        }

        internal static bool IsSectionElement(MigraDoc.DocumentObjectModel.IO.Symbol symbol)
        {
            switch (symbol)
            {
                case MigraDoc.DocumentObjectModel.IO.Symbol.Paragraph:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Header:
                case MigraDoc.DocumentObjectModel.IO.Symbol.PrimaryHeader:
                case MigraDoc.DocumentObjectModel.IO.Symbol.FirstPageHeader:
                case MigraDoc.DocumentObjectModel.IO.Symbol.EvenPageHeader:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Footer:
                case MigraDoc.DocumentObjectModel.IO.Symbol.PrimaryFooter:
                case MigraDoc.DocumentObjectModel.IO.Symbol.FirstPageFooter:
                case MigraDoc.DocumentObjectModel.IO.Symbol.EvenPageFooter:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Table:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Image:
                case MigraDoc.DocumentObjectModel.IO.Symbol.TextFrame:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Chart:
                case MigraDoc.DocumentObjectModel.IO.Symbol.PageBreak:
                case MigraDoc.DocumentObjectModel.IO.Symbol.Barcode:
                    return true;
            }
            return false;
        }

        internal static bool IsWhiteSpace(char ch) => 
            char.IsWhiteSpace(ch);

        internal void MoveBeyondEol()
        {
            this.ScanNextChar();
            while ((this.currChar != '\0') && (this.currChar != '\n'))
            {
                this.ScanNextChar();
            }
            this.ScanNextChar();
        }

        internal MigraDoc.DocumentObjectModel.IO.Symbol MoveToCode()
        {
            if ((this.symbol == MigraDoc.DocumentObjectModel.IO.Symbol.None) || (this.symbol == MigraDoc.DocumentObjectModel.IO.Symbol.CR))
            {
                this.ReadCode();
            }
            return this.symbol;
        }

        internal bool MoveToNextParagraphContentLine(bool rootLevel)
        {
            bool flag = true;
            this.ScanNextChar();
            while (flag)
            {
                this.MoveToNonWhiteSpaceOrEol();
                switch (this.currChar)
                {
                    case '/':
                        if (this.nextChar == '/')
                        {
                            this.MoveBeyondEol();
                            continue;
                        }
                        return true;

                    case '}':
                        return false;

                    case '\0':
                    {
                        flag = false;
                        continue;
                    }
                    case '\n':
                        this.ScanNextChar();
                        if (rootLevel)
                        {
                            this.MoveToParagraphContent();
                            return false;
                        }
                        if (this.PeekSymbol() != MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight)
                        {
                            continue;
                        }
                        this.MoveToNonWhiteSpace();
                        return false;
                }
                return true;
            }
            return false;
        }

        internal char MoveToNonWhiteSpace()
        {
            while (this.currChar != '\0')
            {
                switch (this.currChar)
                {
                    case '\t':
                    case '\n':
                    case '\v':
                    case '\r':
                    case ' ':
                    {
                        this.ScanNextChar();
                        continue;
                    }
                }
                return this.currChar;
            }
            return this.currChar;
        }

        internal char MoveToNonWhiteSpaceOrEol()
        {
            while (this.currChar != '\0')
            {
                switch (this.currChar)
                {
                    case '\t':
                    case '\v':
                    case ' ':
                    {
                        this.ScanNextChar();
                        continue;
                    }
                }
                return this.currChar;
            }
            return this.currChar;
        }

        internal bool MoveToParagraphContent()
        {
            while (true)
            {
                this.MoveToNonWhiteSpace();
                if ((this.currChar != '/') || (this.nextChar != '/'))
                {
                    break;
                }
                this.MoveBeyondEol();
            }
            return (this.currChar != '}');
        }

        internal MigraDoc.DocumentObjectModel.IO.Symbol PeekKeyword() => 
            this.PeekKeyword(this.m_idx);

        internal MigraDoc.DocumentObjectModel.IO.Symbol PeekKeyword(int index)
        {
            switch (this.m_strDocument[index])
            {
                case '{':
                case '}':
                case '\\':
                case '(':
                case '-':
                    return MigraDoc.DocumentObjectModel.IO.Symbol.Character;
            }
            string name = @"\";
            int num = index;
            for (int i = this.ddlLength - num; i > 0; i--)
            {
                char ch = this.m_strDocument[num++];
                if (!IsLetter(ch))
                {
                    break;
                }
                name = name + ch;
            }
            return KeyWords.SymbolFromName(name);
        }

        protected MigraDoc.DocumentObjectModel.IO.Symbol PeekPunctuator(int index)
        {
            MigraDoc.DocumentObjectModel.IO.Symbol none = MigraDoc.DocumentObjectModel.IO.Symbol.None;
            char ch2 = this.m_strDocument[index];
            if (ch2 <= '/')
            {
                if (ch2 <= '\n')
                {
                    switch (ch2)
                    {
                        case '\0':
                            return MigraDoc.DocumentObjectModel.IO.Symbol.Eof;

                        case '\n':
                            return MigraDoc.DocumentObjectModel.IO.Symbol.LF;
                    }
                    return none;
                }
                switch (ch2)
                {
                    case ' ':
                        return MigraDoc.DocumentObjectModel.IO.Symbol.Blank;

                    case '!':
                    case '"':
                    case '&':
                    case '\'':
                    case '*':
                        return none;

                    case '#':
                        return MigraDoc.DocumentObjectModel.IO.Symbol.Hash;

                    case '$':
                        return MigraDoc.DocumentObjectModel.IO.Symbol.Dollar;

                    case '%':
                        return MigraDoc.DocumentObjectModel.IO.Symbol.Percent;

                    case '(':
                        return MigraDoc.DocumentObjectModel.IO.Symbol.ParenLeft;

                    case ')':
                        return MigraDoc.DocumentObjectModel.IO.Symbol.ParenRight;

                    case '+':
                        if ((this.ddlLength < (index + 1)) || (this.m_strDocument[index + 1] != '='))
                        {
                            return MigraDoc.DocumentObjectModel.IO.Symbol.Plus;
                        }
                        return MigraDoc.DocumentObjectModel.IO.Symbol.PlusAssign;

                    case ',':
                        return MigraDoc.DocumentObjectModel.IO.Symbol.Comma;

                    case '-':
                        if ((this.ddlLength < (index + 1)) || (this.m_strDocument[index + 1] != '='))
                        {
                            return MigraDoc.DocumentObjectModel.IO.Symbol.Minus;
                        }
                        return MigraDoc.DocumentObjectModel.IO.Symbol.MinusAssign;

                    case '.':
                        return MigraDoc.DocumentObjectModel.IO.Symbol.Dot;

                    case '/':
                        return MigraDoc.DocumentObjectModel.IO.Symbol.Slash;

                    case '\r':
                        return MigraDoc.DocumentObjectModel.IO.Symbol.CR;
                }
                return none;
            }
            if (ch2 <= ']')
            {
                switch (ch2)
                {
                    case ':':
                        return MigraDoc.DocumentObjectModel.IO.Symbol.Colon;

                    case ';':
                        return MigraDoc.DocumentObjectModel.IO.Symbol.Semicolon;

                    case '<':
                    case '>':
                    case '?':
                        return none;

                    case '=':
                        return MigraDoc.DocumentObjectModel.IO.Symbol.Assign;

                    case '@':
                        return MigraDoc.DocumentObjectModel.IO.Symbol.At;

                    case '[':
                        return MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft;

                    case '\\':
                        return MigraDoc.DocumentObjectModel.IO.Symbol.BackSlash;

                    case ']':
                        return MigraDoc.DocumentObjectModel.IO.Symbol.BracketRight;
                }
                return none;
            }
            switch (ch2)
            {
                case '{':
                    return MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft;

                case '|':
                    return none;

                case '}':
                    return MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight;

                case '\x00a4':
                    return MigraDoc.DocumentObjectModel.IO.Symbol.Currency;
            }
            return none;
        }

        internal MigraDoc.DocumentObjectModel.IO.Symbol PeekSymbol()
        {
            int index = this.m_idx - 1;
            int num2 = this.ddlLength - index;
            char ch = '\0';
            while (num2 > 0)
            {
                ch = this.m_strDocument[index++];
                if (!IsWhiteSpace(ch))
                {
                    break;
                }
                num2--;
            }
            if (IsLetter(ch))
            {
                return MigraDoc.DocumentObjectModel.IO.Symbol.Text;
            }
            if (ch == '\\')
            {
                return this.PeekKeyword(index);
            }
            return this.PeekPunctuator(index - 1);
        }

        internal MigraDoc.DocumentObjectModel.IO.Symbol ReadCode()
        {
        Label_0000:
            this.symbol = MigraDoc.DocumentObjectModel.IO.Symbol.None;
            this.tokenType = MigraDoc.DocumentObjectModel.IO.TokenType.None;
            this.token = "";
            this.MoveToNonWhiteSpace();
            this.SaveCurDocumentPos();
            if (this.currChar == '\0')
            {
                this.symbol = MigraDoc.DocumentObjectModel.IO.Symbol.Eof;
                return MigraDoc.DocumentObjectModel.IO.Symbol.Eof;
            }
            if (IsIdentifierChar(this.currChar, true))
            {
                this.symbol = this.ScanIdentifier();
                this.tokenType = MigraDoc.DocumentObjectModel.IO.TokenType.Identifier;
                MigraDoc.DocumentObjectModel.IO.Symbol symbol = KeyWords.SymbolFromName(this.token);
                if (symbol != MigraDoc.DocumentObjectModel.IO.Symbol.None)
                {
                    this.symbol = symbol;
                    this.tokenType = MigraDoc.DocumentObjectModel.IO.TokenType.KeyWord;
                }
            }
            else if (this.currChar == '"')
            {
                this.token = this.token + this.ScanStringLiteral();
                this.symbol = MigraDoc.DocumentObjectModel.IO.Symbol.StringLiteral;
                this.tokenType = MigraDoc.DocumentObjectModel.IO.TokenType.StringLiteral;
            }
            else if ((IsDigit(this.currChar) || ((this.currChar == '-') && IsDigit(this.nextChar))) || ((this.currChar == '+') && IsDigit(this.nextChar)))
            {
                this.symbol = this.ScanNumber(false);
                this.tokenType = (this.symbol == MigraDoc.DocumentObjectModel.IO.Symbol.RealLiteral) ? MigraDoc.DocumentObjectModel.IO.TokenType.RealLiteral : MigraDoc.DocumentObjectModel.IO.TokenType.IntegerLiteral;
            }
            else if ((this.currChar == '.') && IsDigit(this.nextChar))
            {
                this.symbol = this.ScanNumber(true);
                this.tokenType = MigraDoc.DocumentObjectModel.IO.TokenType.RealLiteral;
            }
            else if (this.currChar == '\\')
            {
                this.token = @"\";
                this.symbol = this.ScanKeyword();
                this.tokenType = (this.symbol != MigraDoc.DocumentObjectModel.IO.Symbol.None) ? MigraDoc.DocumentObjectModel.IO.TokenType.KeyWord : MigraDoc.DocumentObjectModel.IO.TokenType.None;
            }
            else
            {
                if ((this.currChar == '/') && (this.nextChar == '/'))
                {
                    this.ScanSingleLineComment();
                    goto Label_0000;
                }
                if ((this.currChar == '@') && (this.nextChar == '"'))
                {
                    this.ScanNextChar();
                    this.token = this.token + this.ScanVerbatimStringLiteral();
                    this.symbol = MigraDoc.DocumentObjectModel.IO.Symbol.StringLiteral;
                    this.tokenType = (this.symbol != MigraDoc.DocumentObjectModel.IO.Symbol.None) ? MigraDoc.DocumentObjectModel.IO.TokenType.StringLiteral : MigraDoc.DocumentObjectModel.IO.TokenType.None;
                }
                else
                {
                    this.symbol = this.ScanPunctuator();
                }
            }
            return this.symbol;
        }

        protected MigraDoc.DocumentObjectModel.IO.Symbol ReadHexNumber()
        {
            this.token = "0x";
            this.ScanNextChar();
            while (this.currChar != '\0')
            {
                if (IsHexDigit(this.currChar))
                {
                    this.AppendAndScanNextChar();
                }
                else
                {
                    if (!IsIdentifierChar(this.currChar, false))
                    {
                        break;
                    }
                    this.AppendAndScanNextChar();
                }
            }
            return MigraDoc.DocumentObjectModel.IO.Symbol.HexIntegerLiteral;
        }

        private MigraDoc.DocumentObjectModel.IO.Symbol ReadPlainText(bool rootLevel)
        {
            bool flag = false;
            bool flag2 = true;
            while (flag2 && (this.currChar != '\0'))
            {
                if (this.currChar == '\\')
                {
                    switch (this.nextChar)
                    {
                        case '-':
                        {
                            this.ScanNextChar();
                            this.currChar = '\x00ad';
                            continue;
                        }
                        case '/':
                        case '\\':
                        case '{':
                        case '}':
                        {
                            this.ScanNextChar();
                            this.AppendAndScanNextChar();
                            continue;
                        }
                    }
                    flag2 = false;
                    continue;
                }
                switch (this.currChar)
                {
                    case '{':
                    {
                        flag2 = false;
                        continue;
                    }
                    case '}':
                    {
                        flag2 = false;
                        continue;
                    }
                    case '/':
                        if (this.nextChar != '/')
                        {
                            goto Label_0111;
                        }
                        this.ScanToEol();
                        break;
                }
                if (this.currChar == '\n')
                {
                    if (this.MoveToNextParagraphContentLine(rootLevel))
                    {
                        if (!this.token.EndsWith(" "))
                        {
                            this.token = this.token + ' ';
                        }
                        continue;
                    }
                    this.emptyLine = this.currChar != '}';
                    break;
                }
            Label_0111:
                if (this.currChar == ' ')
                {
                    if (flag)
                    {
                        this.ScanNextChar();
                        continue;
                    }
                    flag = true;
                }
                else
                {
                    flag = false;
                }
                this.AppendAndScanNextChar();
            }
            this.symbol = MigraDoc.DocumentObjectModel.IO.Symbol.Text;
            this.tokenType = MigraDoc.DocumentObjectModel.IO.TokenType.Text;
            return MigraDoc.DocumentObjectModel.IO.Symbol.Text;
        }

        internal MigraDoc.DocumentObjectModel.IO.Symbol ReadText(bool rootLevel)
        {
            if (this.emptyLine)
            {
                this.emptyLine = false;
                this.symbol = MigraDoc.DocumentObjectModel.IO.Symbol.EmptyLine;
                this.tokenType = MigraDoc.DocumentObjectModel.IO.TokenType.None;
                this.token = "";
                return MigraDoc.DocumentObjectModel.IO.Symbol.EmptyLine;
            }
            this.prevSymbol = this.symbol;
            this.symbol = MigraDoc.DocumentObjectModel.IO.Symbol.None;
            this.tokenType = MigraDoc.DocumentObjectModel.IO.TokenType.None;
            this.token = "";
            this.SaveCurDocumentPos();
            if (this.currChar == '\0')
            {
                this.symbol = MigraDoc.DocumentObjectModel.IO.Symbol.Eof;
                return MigraDoc.DocumentObjectModel.IO.Symbol.Eof;
            }
            if (this.currChar != '\\')
            {
                switch (this.currChar)
                {
                    case '{':
                        this.AppendAndScanNextChar();
                        this.symbol = MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft;
                        this.tokenType = MigraDoc.DocumentObjectModel.IO.TokenType.OperatorOrPunctuator;
                        return MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft;

                    case '}':
                        this.AppendAndScanNextChar();
                        this.symbol = MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight;
                        this.tokenType = MigraDoc.DocumentObjectModel.IO.TokenType.OperatorOrPunctuator;
                        return MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight;
                }
            }
            else
            {
                switch (this.nextChar)
                {
                    case '-':
                    case '/':
                    case '\\':
                    case '{':
                    case '}':
                        return this.ReadPlainText(rootLevel);
                }
                this.token = @"\";
                return this.ScanKeyword();
            }
            if (this.currChar != '\n')
            {
                return this.ReadPlainText(rootLevel);
            }
            if (this.MoveToNextParagraphContentLine(rootLevel))
            {
                this.token = " ";
                if (this.IgnoreLineBreak())
                {
                    this.token = "";
                }
                this.symbol = MigraDoc.DocumentObjectModel.IO.Symbol.Text;
                return MigraDoc.DocumentObjectModel.IO.Symbol.Text;
            }
            if (this.currChar != '}')
            {
                this.symbol = MigraDoc.DocumentObjectModel.IO.Symbol.EmptyLine;
                this.tokenType = MigraDoc.DocumentObjectModel.IO.TokenType.None;
                return MigraDoc.DocumentObjectModel.IO.Symbol.EmptyLine;
            }
            this.AppendAndScanNextChar();
            this.symbol = MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight;
            this.tokenType = MigraDoc.DocumentObjectModel.IO.TokenType.OperatorOrPunctuator;
            return MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight;
        }

        private void SaveCurDocumentPos()
        {
            this.m_nCurDocumentIndex = this.m_idx - 1;
            this.m_nCurDocumentLine = this.m_idxLine;
            this.m_nCurDocumentLinePos = this.m_idxLinePos;
        }

        protected MigraDoc.DocumentObjectModel.IO.Symbol ScanIdentifier()
        {
            for (char ch = this.AppendAndScanNextChar(); IsIdentifierChar(ch, false); ch = this.AppendAndScanNextChar())
            {
            }
            return MigraDoc.DocumentObjectModel.IO.Symbol.Identifier;
        }

        private MigraDoc.DocumentObjectModel.IO.Symbol ScanKeyword()
        {
            char ch = this.ScanNextChar();
            if (ch == '-')
            {
                this.token = this.token + "-";
                this.ScanNextChar();
                return MigraDoc.DocumentObjectModel.IO.Symbol.SoftHyphen;
            }
            if (ch != '(')
            {
                while (!IsEof(ch) && IsIdentifierChar(ch, false))
                {
                    ch = this.AppendAndScanNextChar();
                }
                this.symbol = KeyWords.SymbolFromName(this.token);
                return this.symbol;
            }
            this.token = this.token + "(";
            this.symbol = MigraDoc.DocumentObjectModel.IO.Symbol.Chr;
            return MigraDoc.DocumentObjectModel.IO.Symbol.Chr;
        }

        internal char ScanNextChar()
        {
            if (this.ddlLength <= this.m_idx)
            {
                this.currChar = '\0';
                this.nextChar = '\0';
                goto Label_00D1;
            }
        Label_0021:
            this.currChar = this.m_strDocument[this.m_idx++];
            if (this.ddlLength <= this.m_idx)
            {
                this.nextChar = '\0';
            }
            else
            {
                this.nextChar = this.m_strDocument[this.m_idx];
            }
            this.m_idxLinePos++;
            switch (this.currChar)
            {
                case '\0':
                    this.m_idxLine++;
                    this.m_idxLinePos = 0;
                    break;

                case '\n':
                    this.m_idxLine++;
                    this.m_idxLinePos = 0;
                    break;

                case '\r':
                    if (this.nextChar != '\n')
                    {
                        break;
                    }
                    goto Label_0021;
            }
        Label_00D1:
            return this.currChar;
        }

        protected MigraDoc.DocumentObjectModel.IO.Symbol ScanNumber(bool mantissa)
        {
            char currChar = this.currChar;
            this.token = this.token + this.currChar;
            this.ScanNextChar();
            if ((mantissa || (currChar != '0')) || ((this.currChar != 'x') && (this.currChar != 'X')))
            {
                while (this.currChar != '\0')
                {
                    if (IsDigit(this.currChar))
                    {
                        this.AppendAndScanNextChar();
                    }
                    else
                    {
                        if (mantissa || (this.currChar != '.'))
                        {
                            break;
                        }
                        return this.ScanNumber(true);
                    }
                }
            }
            else
            {
                return this.ReadHexNumber();
            }
            if (!mantissa)
            {
                return MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral;
            }
            return MigraDoc.DocumentObjectModel.IO.Symbol.RealLiteral;
        }

        protected MigraDoc.DocumentObjectModel.IO.Symbol ScanPunctuator()
        {
            MigraDoc.DocumentObjectModel.IO.Symbol none = MigraDoc.DocumentObjectModel.IO.Symbol.None;
            switch (this.currChar)
            {
                case ' ':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.Blank;
                    break;

                case '#':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.Hash;
                    break;

                case '$':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.Dollar;
                    break;

                case '%':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.Percent;
                    break;

                case '(':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.ParenLeft;
                    break;

                case ')':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.ParenRight;
                    break;

                case '+':
                    if (this.nextChar != '=')
                    {
                        none = MigraDoc.DocumentObjectModel.IO.Symbol.Plus;
                        break;
                    }
                    this.token = this.token + this.currChar;
                    this.ScanNextChar();
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.PlusAssign;
                    break;

                case ',':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.Comma;
                    break;

                case '-':
                    if (this.nextChar != '=')
                    {
                        none = MigraDoc.DocumentObjectModel.IO.Symbol.Minus;
                        break;
                    }
                    this.token = this.token + this.currChar;
                    this.ScanNextChar();
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.MinusAssign;
                    break;

                case '.':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.Dot;
                    break;

                case '/':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.Slash;
                    break;

                case '\r':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.CR;
                    break;

                case '\0':
                    return MigraDoc.DocumentObjectModel.IO.Symbol.Eof;

                case '\n':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.LF;
                    break;

                case ':':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.Colon;
                    break;

                case ';':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.Semicolon;
                    break;

                case '=':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.Assign;
                    break;

                case '@':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.At;
                    break;

                case '[':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft;
                    break;

                case '\\':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.BackSlash;
                    break;

                case ']':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.BracketRight;
                    break;

                case '{':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft;
                    break;

                case '}':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight;
                    break;

                case '\x00a4':
                    none = MigraDoc.DocumentObjectModel.IO.Symbol.Currency;
                    break;
            }
            this.token = this.token + this.currChar;
            this.ScanNextChar();
            return none;
        }

        internal MigraDoc.DocumentObjectModel.IO.Symbol ScanSingleLineComment()
        {
            for (char ch = this.ScanNextChar(); (ch != '\0') && (ch != '\n'); ch = this.ScanNextChar())
            {
                this.token = this.token + this.currChar;
            }
            this.ScanNextChar();
            return MigraDoc.DocumentObjectModel.IO.Symbol.Comment;
        }

        protected string ScanStringLiteral()
        {
            string str = "";
            this.ScanNextChar();
            while ((this.currChar != '"') && !IsEof(this.currChar))
            {
                int num;
                string str2;
                if (this.currChar != '\\')
                {
                    goto Label_01D3;
                }
                this.ScanNextChar();
                switch (this.currChar)
                {
                    case '"':
                        str = str + '"';
                        goto Label_0217;

                    case '\'':
                        str = str + '\'';
                        goto Label_0217;

                    case '\\':
                        str = str + '\\';
                        goto Label_0217;

                    case 'r':
                        str = str + '\r';
                        goto Label_0217;

                    case 't':
                        str = str + '\t';
                        goto Label_0217;

                    case 'v':
                        str = str + '\v';
                        goto Label_0217;

                    case 'x':
                        this.ScanNextChar();
                        num = 0;
                        str2 = "0x";
                        goto Label_0188;

                    case 'n':
                        str = str + '\n';
                        goto Label_0217;

                    case 'a':
                        str = str + '\a';
                        goto Label_0217;

                    case 'b':
                        str = str + '\b';
                        goto Label_0217;

                    case 'f':
                        str = str + '\f';
                        goto Label_0217;

                    default:
                        throw new DdlParserException(DdlErrorLevel.Error, DomSR.GetString(DomMsgID.EscapeSequenceNotAllowed), DomMsgID.EscapeSequenceNotAllowed);
                }
            Label_016B:
                num++;
                str2 = str2 + this.currChar;
                this.ScanNextChar();
            Label_0188:
                if (IsHexDigit(this.currChar))
                {
                    goto Label_016B;
                }
                if (num <= 2)
                {
                    str = str + "?????";
                    goto Label_0217;
                }
                throw new DdlParserException(DdlErrorLevel.Error, DomSR.GetString(DomMsgID.EscapeSequenceNotAllowed), DomMsgID.EscapeSequenceNotAllowed);
            Label_01D3:
                if (((this.currChar == '\0') || (this.currChar == '\r')) || (this.currChar == '\n'))
                {
                    throw new DdlParserException(DdlErrorLevel.Error, DomSR.GetString(DomMsgID.NewlineInString), DomMsgID.NewlineInString);
                }
                str = str + this.currChar;
            Label_0217:
                this.ScanNextChar();
            }
            this.ScanNextChar();
            return str;
        }

        internal void ScanToEol()
        {
            while (!IsEof(this.currChar) && (this.currChar != '\n'))
            {
                this.ScanNextChar();
            }
        }

        protected string ScanVerbatimStringLiteral()
        {
            string str = "";
            for (char ch = this.ScanNextChar(); !IsEof(ch); ch = this.ScanNextChar())
            {
                if (ch == '"')
                {
                    if (this.nextChar != '"')
                    {
                        break;
                    }
                    ch = this.ScanNextChar();
                }
                str = str + ch;
            }
            this.ScanNextChar();
            return str;
        }

        internal char Char =>
            this.currChar;

        internal int CurrentLine =>
            this.m_nCurDocumentLine;

        internal int CurrentLinePos =>
            this.m_nCurDocumentLinePos;

        internal string DocumentFileName =>
            this.m_DocumentFileName;

        internal string DocumentPath =>
            this.m_DocumentPath;

        internal char NextChar =>
            this.nextChar;

        internal MigraDoc.DocumentObjectModel.IO.Symbol Symbol =>
            this.symbol;

        internal string Token =>
            this.token;

        internal MigraDoc.DocumentObjectModel.IO.TokenType TokenType =>
            this.tokenType;
    }
}

