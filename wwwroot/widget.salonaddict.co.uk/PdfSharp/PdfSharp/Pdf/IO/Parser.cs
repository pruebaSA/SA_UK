namespace PdfSharp.Pdf.IO
{
    using PdfSharp;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Advanced;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    internal sealed class Parser
    {
        private PdfDocument document;
        private Lexer lexer;
        private ShiftStack stack;

        public Parser(PdfDocument document)
        {
            this.document = document;
            this.lexer = document.lexer;
            this.stack = new ShiftStack();
        }

        public Parser(PdfDocument document, Stream pdf)
        {
            this.document = document;
            this.lexer = new Lexer(pdf);
            this.stack = new ShiftStack();
        }

        private int GetStreamLength(PdfDictionary dict)
        {
            if (dict.Elements["/F"] != null)
            {
                throw new NotImplementedException("File streams are not yet implemented.");
            }
            PdfItem item = dict.Elements["/Length"];
            if (item is PdfInteger)
            {
                return Convert.ToInt32(item);
            }
            if (!(item is PdfReference))
            {
                throw new InvalidOperationException("Cannot retrieve stream length.");
            }
            ParserState state = this.SaveState();
            object obj2 = this.ReadObject(null, ((PdfReference) item).ObjectID, false);
            this.RestoreState(state);
            int num = ((PdfIntegerObject) obj2).Value;
            dict.Elements["/Length"] = new PdfInteger(num);
            return num;
        }

        public int MoveToObject(PdfObjectID objectID)
        {
            int position = this.document.irefTable[objectID].Position;
            return (this.lexer.Position = position);
        }

        internal static DateTime ParseDateTime(string date, DateTime errorValue)
        {
            DateTime time = errorValue;
            try
            {
                if (date.StartsWith("D:"))
                {
                    int length = date.Length;
                    int year = 0;
                    int month = 0;
                    int day = 0;
                    int hour = 0;
                    int minute = 0;
                    int second = 0;
                    int hours = 0;
                    int minutes = 0;
                    char ch = 'Z';
                    if (length >= 10)
                    {
                        year = int.Parse(date.Substring(2, 4));
                        month = int.Parse(date.Substring(6, 2));
                        day = int.Parse(date.Substring(8, 2));
                        if (length >= 0x10)
                        {
                            hour = int.Parse(date.Substring(10, 2));
                            minute = int.Parse(date.Substring(12, 2));
                            second = int.Parse(date.Substring(14, 2));
                            if ((length >= 0x17) && ((ch = date[0x10]) != 'Z'))
                            {
                                hours = int.Parse(date.Substring(0x11, 2));
                                minutes = int.Parse(date.Substring(20, 2));
                            }
                        }
                    }
                    time = new DateTime(year, month, day, hour, minute, second);
                    if (ch != 'Z')
                    {
                        TimeSpan span = new TimeSpan(hours, minutes, 0);
                        if (ch == '+')
                        {
                            time.Add(span);
                            return time;
                        }
                        time.Subtract(span);
                    }
                    return time;
                }
                time = DateTime.Parse(date);
            }
            catch
            {
            }
            return time;
        }

        private void ParseObject(PdfSharp.Pdf.IO.Symbol stop)
        {
            PdfSharp.Pdf.IO.Symbol symbol;
            while ((symbol = this.ScanNextToken()) != PdfSharp.Pdf.IO.Symbol.Eof)
            {
                PdfReference reference;
                if (symbol == stop)
                {
                    return;
                }
                switch (symbol)
                {
                    case PdfSharp.Pdf.IO.Symbol.Comment:
                    {
                        continue;
                    }
                    case PdfSharp.Pdf.IO.Symbol.Null:
                    {
                        this.stack.Shift(PdfNull.Value);
                        continue;
                    }
                    case PdfSharp.Pdf.IO.Symbol.Integer:
                    {
                        this.stack.Shift(new PdfInteger(this.lexer.TokenToInteger));
                        continue;
                    }
                    case PdfSharp.Pdf.IO.Symbol.UInteger:
                    {
                        this.stack.Shift(new PdfUInteger(this.lexer.TokenToUInteger));
                        continue;
                    }
                    case PdfSharp.Pdf.IO.Symbol.Real:
                    {
                        this.stack.Shift(new PdfReal(this.lexer.TokenToReal));
                        continue;
                    }
                    case PdfSharp.Pdf.IO.Symbol.Boolean:
                    {
                        this.stack.Shift(new PdfBoolean(this.lexer.TokenToBoolean));
                        continue;
                    }
                    case PdfSharp.Pdf.IO.Symbol.String:
                    {
                        this.stack.Shift(new PdfString(this.lexer.Token, PdfStringFlags.RawEncoding));
                        continue;
                    }
                    case PdfSharp.Pdf.IO.Symbol.HexString:
                    {
                        this.stack.Shift(new PdfString(this.lexer.Token, PdfStringFlags.HexLiteral));
                        continue;
                    }
                    case PdfSharp.Pdf.IO.Symbol.UnicodeString:
                    {
                        this.stack.Shift(new PdfString(this.lexer.Token, PdfStringFlags.Unicode));
                        continue;
                    }
                    case PdfSharp.Pdf.IO.Symbol.UnicodeHexString:
                    {
                        this.stack.Shift(new PdfString(this.lexer.Token, PdfStringFlags.HexLiteral | PdfStringFlags.Unicode));
                        continue;
                    }
                    case PdfSharp.Pdf.IO.Symbol.Name:
                    {
                        this.stack.Shift(new PdfName(this.lexer.Token));
                        continue;
                    }
                    case PdfSharp.Pdf.IO.Symbol.BeginStream:
                        throw new NotImplementedException();

                    case PdfSharp.Pdf.IO.Symbol.BeginArray:
                    {
                        PdfArray array = new PdfArray(this.document);
                        this.ReadArray(array, false);
                        this.stack.Shift(array);
                        continue;
                    }
                    case PdfSharp.Pdf.IO.Symbol.BeginDictionary:
                    {
                        PdfDictionary dict = new PdfDictionary(this.document);
                        this.ReadDictionary(dict, false);
                        this.stack.Shift(dict);
                        continue;
                    }
                    case PdfSharp.Pdf.IO.Symbol.R:
                    {
                        PdfObjectID objectID = new PdfObjectID(this.stack.GetInteger(-2), this.stack.GetInteger(-1));
                        reference = this.document.irefTable[objectID];
                        if (reference != null)
                        {
                            goto Label_0221;
                        }
                        if (!this.document.irefTable.IsUnderConstruction)
                        {
                            break;
                        }
                        reference = new PdfReference(objectID, 0);
                        this.stack.Reduce(reference, 2);
                        continue;
                    }
                    default:
                        goto Label_027F;
                }
                this.stack.Reduce(PdfNull.Value, 2);
                continue;
            Label_0221:
                this.stack.Reduce(reference, 2);
                continue;
            Label_027F:
                string text1 = this.lexer.Token;
            }
            throw new PdfReaderException("Unexpected end of file.");
        }

        public PdfArray ReadArray(PdfArray array, bool includeReferences)
        {
            if (array == null)
            {
                array = new PdfArray(this.document);
            }
            int sP = this.stack.SP;
            this.ParseObject(PdfSharp.Pdf.IO.Symbol.EndArray);
            int length = this.stack.SP - sP;
            PdfItem[] itemArray = this.stack.ToArray(sP, length);
            this.stack.Reduce(length);
            for (int i = 0; i < length; i++)
            {
                PdfItem item = itemArray[i];
                if (includeReferences && (item is PdfReference))
                {
                    item = this.ReadReference((PdfReference) item, includeReferences);
                }
                array.Elements.Add(item);
            }
            return array;
        }

        internal PdfDictionary ReadDictionary(PdfDictionary dict, bool includeReferences)
        {
            if (dict == null)
            {
                dict = new PdfDictionary(this.document);
            }
            DictionaryMeta meta = dict.Meta;
            int sP = this.stack.SP;
            this.ParseObject(PdfSharp.Pdf.IO.Symbol.EndDictionary);
            int length = this.stack.SP - sP;
            PdfItem[] itemArray = this.stack.ToArray(sP, length);
            this.stack.Reduce(length);
            for (int i = 0; i < length; i += 2)
            {
                PdfItem item = itemArray[i];
                if (!(item is PdfName))
                {
                    throw new PdfReaderException("name expected");
                }
                string str = ((PdfName) item).ToString();
                item = itemArray[i + 1];
                if (includeReferences && (item is PdfReference))
                {
                    item = this.ReadReference((PdfReference) item, includeReferences);
                }
                dict.Elements[str] = item;
            }
            return dict;
        }

        private int ReadInteger() => 
            this.ReadInteger(false);

        private int ReadInteger(bool canBeIndirect)
        {
            switch (this.lexer.ScanNextToken())
            {
                case PdfSharp.Pdf.IO.Symbol.Integer:
                    return this.lexer.TokenToInteger;

                case PdfSharp.Pdf.IO.Symbol.R:
                {
                    int position = this.lexer.Position;
                    this.ReadObjectID(null);
                    int num2 = this.ReadInteger();
                    this.ReadSymbol(PdfSharp.Pdf.IO.Symbol.EndObj);
                    this.lexer.Position = position;
                    return num2;
                }
            }
            throw new PdfReaderException(PSSR.UnexpectedToken(this.lexer.Token));
        }

        private string ReadName()
        {
            string str;
            if (this.ScanNextToken(out str) != PdfSharp.Pdf.IO.Symbol.Name)
            {
                throw new PdfReaderException(PSSR.UnexpectedToken(str));
            }
            return str;
        }

        public static PdfObject ReadObject(PdfDocument owner, PdfObjectID objectID)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }
            Parser parser = new Parser(owner);
            return parser.ReadObject(null, objectID, false);
        }

        public PdfObject ReadObject(PdfObject pdfObject, PdfObjectID objectID, bool includeReferences)
        {
            PdfArray array;
            this.MoveToObject(objectID);
            int objectNumber = this.ReadInteger();
            int generationNumber = this.ReadInteger();
            objectNumber = objectID.ObjectNumber;
            generationNumber = objectID.GenerationNumber;
            this.ReadSymbol(PdfSharp.Pdf.IO.Symbol.Obj);
            PdfSharp.Pdf.IO.Symbol symbol = this.ScanNextToken();
            switch (symbol)
            {
                case PdfSharp.Pdf.IO.Symbol.Null:
                    pdfObject = new PdfNullObject(this.document);
                    pdfObject.SetObjectID(objectNumber, generationNumber);
                    this.ReadSymbol(PdfSharp.Pdf.IO.Symbol.EndObj);
                    return pdfObject;

                case PdfSharp.Pdf.IO.Symbol.Integer:
                    pdfObject = new PdfIntegerObject(this.document, this.lexer.TokenToInteger);
                    pdfObject.SetObjectID(objectNumber, generationNumber);
                    this.ReadSymbol(PdfSharp.Pdf.IO.Symbol.EndObj);
                    return pdfObject;

                case PdfSharp.Pdf.IO.Symbol.UInteger:
                    pdfObject = new PdfUIntegerObject(this.document, this.lexer.TokenToUInteger);
                    pdfObject.SetObjectID(objectNumber, generationNumber);
                    this.ReadSymbol(PdfSharp.Pdf.IO.Symbol.EndObj);
                    return pdfObject;

                case PdfSharp.Pdf.IO.Symbol.Real:
                    pdfObject = new PdfRealObject(this.document, this.lexer.TokenToReal);
                    pdfObject.SetObjectID(objectNumber, generationNumber);
                    this.ReadSymbol(PdfSharp.Pdf.IO.Symbol.EndObj);
                    return pdfObject;

                case PdfSharp.Pdf.IO.Symbol.Boolean:
                    pdfObject = new PdfBooleanObject(this.document, string.Compare(this.lexer.Token, bool.TrueString, true) == 0);
                    pdfObject.SetObjectID(objectNumber, generationNumber);
                    this.ReadSymbol(PdfSharp.Pdf.IO.Symbol.EndObj);
                    return pdfObject;

                case PdfSharp.Pdf.IO.Symbol.String:
                    pdfObject = new PdfStringObject(this.document, this.lexer.Token);
                    pdfObject.SetObjectID(objectNumber, generationNumber);
                    this.ReadSymbol(PdfSharp.Pdf.IO.Symbol.EndObj);
                    return pdfObject;

                case PdfSharp.Pdf.IO.Symbol.Name:
                    pdfObject = new PdfNameObject(this.document, this.lexer.Token);
                    pdfObject.SetObjectID(objectNumber, generationNumber);
                    this.ReadSymbol(PdfSharp.Pdf.IO.Symbol.EndObj);
                    return pdfObject;

                case PdfSharp.Pdf.IO.Symbol.Keyword:
                    throw new NotImplementedException("Keyword");

                case PdfSharp.Pdf.IO.Symbol.BeginArray:
                    if (pdfObject != null)
                    {
                        array = (PdfArray) pdfObject;
                        break;
                    }
                    array = new PdfArray(this.document);
                    break;

                case PdfSharp.Pdf.IO.Symbol.BeginDictionary:
                    PdfDictionary dictionary;
                    if (pdfObject != null)
                    {
                        dictionary = (PdfDictionary) pdfObject;
                    }
                    else
                    {
                        dictionary = new PdfDictionary(this.document);
                    }
                    pdfObject = this.ReadDictionary(dictionary, includeReferences);
                    pdfObject.SetObjectID(objectNumber, generationNumber);
                    goto Label_023E;

                default:
                    throw new NotImplementedException("unknown token \"" + symbol + "\"");
            }
            pdfObject = this.ReadArray(array, includeReferences);
            pdfObject.SetObjectID(objectNumber, generationNumber);
        Label_023E:
            symbol = this.ScanNextToken();
            if (symbol == PdfSharp.Pdf.IO.Symbol.BeginStream)
            {
                PdfDictionary dict = (PdfDictionary) pdfObject;
                int streamLength = this.GetStreamLength(dict);
                PdfDictionary.PdfStream stream = new PdfDictionary.PdfStream(this.lexer.ReadStream(streamLength), dict);
                dict.Stream = stream;
                this.ReadSymbol(PdfSharp.Pdf.IO.Symbol.EndStream);
                symbol = this.ScanNextToken();
            }
            if (symbol != PdfSharp.Pdf.IO.Symbol.EndObj)
            {
                throw new PdfReaderException(PSSR.UnexpectedToken(this.lexer.Token));
            }
            return pdfObject;
        }

        private void ReadObjectID(PdfObject obj)
        {
            int objectNumber = this.ReadInteger();
            int generationNumber = this.ReadInteger();
            this.ReadSymbol(PdfSharp.Pdf.IO.Symbol.Obj);
            if (obj != null)
            {
                obj.SetObjectID(objectNumber, generationNumber);
            }
        }

        private PdfItem ReadReference(PdfReference iref, bool includeReferences)
        {
            throw new NotImplementedException("ReadReference");
        }

        private PdfSharp.Pdf.IO.Symbol ReadSymbol(PdfSharp.Pdf.IO.Symbol symbol)
        {
            PdfSharp.Pdf.IO.Symbol symbol2 = this.lexer.ScanNextToken();
            if (symbol != symbol2)
            {
                throw new PdfReaderException(PSSR.UnexpectedToken(this.lexer.Token));
            }
            return symbol2;
        }

        private PdfSharp.Pdf.IO.Symbol ReadToken(string token)
        {
            PdfSharp.Pdf.IO.Symbol symbol = this.lexer.ScanNextToken();
            if (token != this.lexer.Token)
            {
                throw new PdfReaderException(PSSR.UnexpectedToken(this.lexer.Token));
            }
            return symbol;
        }

        internal PdfTrailer ReadTrailer()
        {
            PdfTrailer trailer;
            int pdfLength = this.lexer.PdfLength;
            int index = this.lexer.ReadRawString(pdfLength - 0x83, 130).IndexOf("startxref");
            this.lexer.Position = (pdfLength - 0x83) + index;
            this.ReadSymbol(PdfSharp.Pdf.IO.Symbol.StartXRef);
            this.lexer.Position = this.ReadInteger();
        Label_005E:
            trailer = this.ReadXRefTableAndTrailer(this.document.irefTable);
            if (this.document.trailer == null)
            {
                this.document.trailer = trailer;
            }
            int integer = trailer.Elements.GetInteger("/Prev");
            if (integer != 0)
            {
                this.lexer.Position = integer;
                goto Label_005E;
            }
            return this.document.trailer;
        }

        private PdfTrailer ReadXRefTableAndTrailer(PdfReferenceTable xrefTable)
        {
            PdfSharp.Pdf.IO.Symbol symbol;
            if (this.ScanNextToken() == PdfSharp.Pdf.IO.Symbol.Integer)
            {
                throw new PdfReaderException(PSSR.CannotHandleXRefStreams);
            }
            while (true)
            {
                symbol = this.ScanNextToken();
                if (symbol != PdfSharp.Pdf.IO.Symbol.Integer)
                {
                    break;
                }
                int tokenToInteger = this.lexer.TokenToInteger;
                int num2 = this.ReadInteger();
                for (int i = tokenToInteger; i < (tokenToInteger + num2); i++)
                {
                    int position = this.ReadInteger();
                    int generationNumber = this.ReadInteger();
                    this.ReadSymbol(PdfSharp.Pdf.IO.Symbol.Keyword);
                    string token = this.lexer.Token;
                    if ((i != 0) && (token == "n"))
                    {
                        PdfObjectID objectID = new PdfObjectID(i, generationNumber);
                        if (!xrefTable.Contains(objectID))
                        {
                            xrefTable.Add(new PdfReference(objectID, position));
                        }
                    }
                }
            }
            if (symbol != PdfSharp.Pdf.IO.Symbol.Trailer)
            {
                throw new PdfReaderException(PSSR.UnexpectedToken(this.lexer.Token));
            }
            this.ReadSymbol(PdfSharp.Pdf.IO.Symbol.BeginDictionary);
            PdfTrailer dict = new PdfTrailer(this.document);
            this.ReadDictionary(dict, false);
            return dict;
        }

        private void RestoreState(ParserState state)
        {
            this.lexer.Position = state.Position;
            this.lexer.Symbol = state.Symbol;
        }

        private ParserState SaveState() => 
            new ParserState { 
                Position = this.lexer.Position,
                Symbol = this.lexer.Symbol
            };

        private PdfSharp.Pdf.IO.Symbol ScanNextToken() => 
            this.lexer.ScanNextToken();

        private PdfSharp.Pdf.IO.Symbol ScanNextToken(out string token)
        {
            PdfSharp.Pdf.IO.Symbol symbol = this.lexer.ScanNextToken();
            token = this.lexer.Token;
            return symbol;
        }

        public PdfSharp.Pdf.IO.Symbol Symbol =>
            this.lexer.Symbol;

        private class ParserState
        {
            public int Position;
            public PdfSharp.Pdf.IO.Symbol Symbol;
        }
    }
}

