namespace PdfSharp.Pdf.Content
{
    using PdfSharp;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Content.Objects;
    using System;
    using System.Runtime.InteropServices;

    internal sealed class CParser
    {
        private CLexer lexer;
        private CSequence operands;
        private PdfPage page;

        public CParser(PdfPage page)
        {
            this.operands = new CSequence();
            this.page = page;
            byte[] content = page.Contents.CreateSingleContent().Stream.Value;
            this.lexer = new CLexer(content);
        }

        public CParser(byte[] content)
        {
            this.operands = new CSequence();
            this.lexer = new CLexer(content);
        }

        private COperator CreateOperator()
        {
            COperator @operator = OpCodes.OperatorFromName(this.lexer.Token);
            if (@operator.OpCode.OpCodeName == OpCodeName.ID)
            {
                this.lexer.ScanInlineImage();
            }
            @operator.Operands.Add(this.operands);
            return @operator;
        }

        private void ParseObject(CSequence sequence, CSymbol stop)
        {
            CSymbol symbol;
            while ((symbol = this.ScanNextToken()) != CSymbol.Eof)
            {
                if (symbol == stop)
                {
                    return;
                }
                switch (symbol)
                {
                    case CSymbol.Integer:
                    {
                        CInteger integer = new CInteger {
                            Value = this.lexer.TokenToInteger
                        };
                        this.operands.Add(integer);
                        break;
                    }
                    case CSymbol.Real:
                    {
                        CReal real = new CReal {
                            Value = this.lexer.TokenToReal
                        };
                        this.operands.Add(real);
                        break;
                    }
                    case CSymbol.String:
                    case CSymbol.HexString:
                    case CSymbol.UnicodeString:
                    case CSymbol.UnicodeHexString:
                    {
                        CString str = new CString {
                            Value = this.lexer.Token
                        };
                        this.operands.Add(str);
                        break;
                    }
                    case CSymbol.Name:
                    {
                        CName name = new CName {
                            Name = this.lexer.Token
                        };
                        this.operands.Add(name);
                        break;
                    }
                    case CSymbol.Operator:
                    {
                        COperator @operator = this.CreateOperator();
                        this.operands.Clear();
                        sequence.Add(@operator);
                        break;
                    }
                    case CSymbol.BeginArray:
                    {
                        CArray array = new CArray();
                        this.ParseObject(array, CSymbol.EndArray);
                        array.Add(this.operands);
                        this.operands.Clear();
                        this.operands.Add((CObject) array);
                        break;
                    }
                    case CSymbol.EndArray:
                        throw new ContentReaderException("Unexpected: ']'");
                }
            }
        }

        public CSequence ReadContent()
        {
            CSequence sequence = new CSequence();
            this.ParseObject(sequence, CSymbol.Eof);
            return sequence;
        }

        private CSymbol ReadSymbol(CSymbol symbol)
        {
            CSymbol symbol2 = this.lexer.ScanNextToken();
            if (symbol != symbol2)
            {
                throw new ContentReaderException(PSSR.UnexpectedToken(this.lexer.Token));
            }
            return symbol2;
        }

        private CSymbol ScanNextToken() => 
            this.lexer.ScanNextToken();

        private CSymbol ScanNextToken(out string token)
        {
            CSymbol symbol = this.lexer.ScanNextToken();
            token = this.lexer.Token;
            return symbol;
        }

        public CSymbol Symbol =>
            this.lexer.Symbol;
    }
}

