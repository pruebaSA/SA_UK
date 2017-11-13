namespace MigraDoc.DocumentObjectModel.IO
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Fields;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using MigraDoc.DocumentObjectModel.Tables;
    using System;
    using System.Globalization;

    internal class DdlParser
    {
        private DdlReaderErrors errors;
        private DdlScanner scanner;

        internal DdlParser(string ddl, DdlReaderErrors errors) : this("", ddl, errors)
        {
        }

        internal DdlParser(string fileName, string ddl, DdlReaderErrors errors)
        {
            if (errors != null)
            {
                this.errors = errors;
            }
            else
            {
                this.errors = new DdlReaderErrors();
            }
            this.scanner = new DdlScanner(fileName, ddl, errors);
        }

        private void AdjustToNextBlock()
        {
            bool flag = (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft);
            this.ReadCode();
            bool flag2 = false;
            while (!flag2)
            {
                switch (this.Symbol)
                {
                    case MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft:
                    {
                        this.AdjustToNextBlock();
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.BracketRight:
                    {
                        if (flag)
                        {
                            this.ReadCode();
                        }
                        flag2 = true;
                        continue;
                    }
                }
                this.AdjustToNextStatement();
            }
        }

        private void AdjustToNextStatement()
        {
            bool flag = false;
            while (!flag)
            {
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Assign)
                {
                    this.ReadCode();
                }
                else
                {
                    this.ReadCode();
                    flag = true;
                }
            }
        }

        private void AssertCondition(bool cond, DomMsgID error, params object[] args)
        {
            if (!cond)
            {
                this.ThrowParserException(error, args);
            }
        }

        private void AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol symbol)
        {
            if (this.Symbol != symbol)
            {
                this.ThrowParserException(DomMsgID.SymbolExpected, new object[] { KeyWords.NameFromSymbol(symbol), this.Token });
            }
        }

        private void AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol symbol, DomMsgID err)
        {
            if (this.Symbol != symbol)
            {
                this.ThrowParserException(err, new object[] { KeyWords.NameFromSymbol(symbol), this.Token });
            }
        }

        private void AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol symbol, DomMsgID err, params object[] parms)
        {
            if (this.Symbol != symbol)
            {
                this.ThrowParserException(err, new object[] { KeyWords.NameFromSymbol(symbol), parms });
            }
        }

        private string GetSymbolText(MigraDoc.DocumentObjectModel.IO.Symbol docSym) => 
            KeyWords.NameFromSymbol(docSym);

        private bool IsHeaderFooter()
        {
            MigraDoc.DocumentObjectModel.IO.Symbol symbol = this.Symbol;
            if ((((symbol != MigraDoc.DocumentObjectModel.IO.Symbol.Header) && (symbol != MigraDoc.DocumentObjectModel.IO.Symbol.Footer)) && ((symbol != MigraDoc.DocumentObjectModel.IO.Symbol.PrimaryHeader) && (symbol != MigraDoc.DocumentObjectModel.IO.Symbol.PrimaryFooter))) && (((symbol != MigraDoc.DocumentObjectModel.IO.Symbol.EvenPageHeader) && (symbol != MigraDoc.DocumentObjectModel.IO.Symbol.EvenPageFooter)) && (symbol != MigraDoc.DocumentObjectModel.IO.Symbol.FirstPageHeader)))
            {
                return (symbol == MigraDoc.DocumentObjectModel.IO.Symbol.FirstPageFooter);
            }
            return true;
        }

        private bool IsParagraphContent()
        {
            if (this.MoveToParagraphContent())
            {
                if (this.scanner.Char != '\\')
                {
                    return true;
                }
                switch (this.scanner.PeekKeyword())
                {
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Bold:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Italic:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Underline:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.FontSize:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.FontColor:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Font:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Hyperlink:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Tab:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.SoftHyphen:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.LineBreak:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Space:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Field:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Symbol:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Chr:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Footnote:
                        return true;
                }
            }
            return false;
        }

        private bool IsSpaceType(string type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (type == "")
            {
                throw new ArgumentException("type");
            }
            if (Enum.IsDefined(typeof(SymbolName), type))
            {
                switch (((SymbolName) Enum.Parse(typeof(SymbolName), type)))
                {
                    case ((SymbolName) (-251658239)):
                    case ((SymbolName) (-251658238)):
                    case ((SymbolName) (-251658237)):
                    case ((SymbolName) (-251658236)):
                        return true;
                }
            }
            return false;
        }

        private bool IsSymbolType(string type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (type == "")
            {
                throw new ArgumentException("type");
            }
            if (Enum.IsDefined(typeof(SymbolName), type))
            {
                switch (((SymbolName) Enum.Parse(typeof(SymbolName), type)))
                {
                    case ((SymbolName) (-134217727)):
                    case ((SymbolName) (-134217726)):
                    case ((SymbolName) (-134217725)):
                    case ((SymbolName) (-134217724)):
                    case ((SymbolName) (-134217723)):
                    case ((SymbolName) (-134217722)):
                    case ((SymbolName) (-134217721)):
                    case ((SymbolName) (-134217720)):
                    case ((SymbolName) (-134217719)):
                        return true;
                }
            }
            return false;
        }

        private MigraDoc.DocumentObjectModel.IO.Symbol MoveToCode() => 
            this.scanner.MoveToCode();

        internal bool MoveToNextParagraphContentLine(bool rootLevel) => 
            this.scanner.MoveToNextParagraphContentLine(rootLevel);

        internal bool MoveToParagraphContent() => 
            this.scanner.MoveToParagraphContent();

        private void ParseArea(PlotArea area)
        {
            try
            {
                this.ReadCode();
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
                {
                    this.ParseAttributes(area, false);
                    this.ReadCode();
                }
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft)
                {
                    bool flag = true;
                    while (flag)
                    {
                        this.ReadCode();
                        if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight)
                        {
                            flag = false;
                        }
                    }
                    this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
                    this.ReadCode();
                }
            }
            catch (DdlParserException exception)
            {
                this.ReportParserException(exception);
                this.AdjustToNextBlock();
            }
        }

        private void ParseArea(TextArea area)
        {
            try
            {
                this.ReadCode();
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
                {
                    this.ParseAttributes(area, false);
                    this.ReadCode();
                }
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft)
                {
                    if (this.IsParagraphContent())
                    {
                        this.ParseParagraphContent(area.Elements, null);
                    }
                    else
                    {
                        this.ReadCode();
                        bool flag = true;
                        while (flag)
                        {
                            switch (this.Symbol)
                            {
                                case MigraDoc.DocumentObjectModel.IO.Symbol.Image:
                                {
                                    Image image = new Image();
                                    this.ParseImage(image, false);
                                    area.Elements.Add(image);
                                    continue;
                                }
                                case MigraDoc.DocumentObjectModel.IO.Symbol.TextFrame:
                                {
                                    this.ParseTextFrame(area.Elements);
                                    continue;
                                }
                                case MigraDoc.DocumentObjectModel.IO.Symbol.Legend:
                                {
                                    this.ParseLegend(area.AddLegend());
                                    continue;
                                }
                                case MigraDoc.DocumentObjectModel.IO.Symbol.Table:
                                {
                                    this.ParseTable(null, area.AddTable());
                                    continue;
                                }
                                case MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight:
                                {
                                    flag = false;
                                    continue;
                                }
                                case MigraDoc.DocumentObjectModel.IO.Symbol.Paragraph:
                                {
                                    this.ParseParagraph(area.Elements);
                                    continue;
                                }
                            }
                            this.ThrowParserException(DomMsgID.UnexpectedSymbol, new object[] { this.Token });
                        }
                    }
                    this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
                    this.ReadCode();
                }
            }
            catch (DdlParserException exception)
            {
                this.ReportParserException(exception);
                this.AdjustToNextBlock();
            }
        }

        private void ParseAssign(DocumentObject dom, ValueDescriptor vd)
        {
            if (dom == null)
            {
                throw new ArgumentNullException("dom");
            }
            if (vd == null)
            {
                throw new ArgumentNullException("vd");
            }
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Assign)
            {
                this.ReadCode();
            }
            Type valueType = vd.ValueType;
            try
            {
                if (valueType == typeof(string))
                {
                    this.ParseStringAssignment(dom, vd);
                }
                else if (valueType == typeof(int))
                {
                    this.ParseIntegerAssignment(dom, vd);
                }
                else if (valueType == typeof(Unit))
                {
                    this.ParseUnitAssignment(dom, vd);
                }
                else if ((valueType == typeof(double)) || (valueType == typeof(float)))
                {
                    this.ParseRealAssignment(dom, vd);
                }
                else if (valueType == typeof(bool))
                {
                    this.ParseBoolAssignment(dom, vd);
                }
                else if (typeof(Enum).IsAssignableFrom(valueType))
                {
                    this.ParseEnumAssignment(dom, vd);
                }
                else if (valueType == typeof(Color))
                {
                    this.ParseColorAssignment(dom, vd);
                }
                else if (typeof(ValueType).IsAssignableFrom(valueType))
                {
                    this.ParseValueTypeAssignment(dom, vd);
                }
                else if (typeof(DocumentObject).IsAssignableFrom(valueType))
                {
                    this.ParseDocumentObjectAssignment(dom, vd);
                }
                else
                {
                    this.AdjustToNextStatement();
                    this.ThrowParserException(DomMsgID.InvalidType, new object[] { vd.ValueType.Name, vd.ValueName });
                }
            }
            catch (Exception exception)
            {
                this.ReportParserException(exception, DomMsgID.InvalidAssignment, new string[] { vd.ValueName });
            }
        }

        private void ParseAttributeBlock(DocumentObject element)
        {
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft);
            this.ReadCode();
            while (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Identifier)
            {
                this.ParseAttributeStatement(element);
            }
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
            this.ReadCode();
        }

        private void ParseAttributes(DocumentObject element)
        {
            this.ParseAttributes(element, true);
        }

        private void ParseAttributes(DocumentObject element, bool readNextSymbol)
        {
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft);
            this.ReadCode();
            while (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Identifier)
            {
                this.ParseAttributeStatement(element);
            }
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BracketRight);
            if (readNextSymbol)
            {
                this.ReadCode();
            }
        }

        private void ParseAttributeStatement(DocumentObject dom)
        {
            object val = null;
            string name = "";
            try
            {
                TabStops tabStops;
                bool flag;
                TabStop stop;
                name = this.scanner.Token;
                DocumentObject obj3 = dom;
                this.ReadCode();
                while (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Dot)
                {
                    val = obj3.GetValue(name);
                    if (val == null)
                    {
                        val = obj3.CreateValue(name);
                        obj3.SetValue(name, val);
                    }
                    this.AssertCondition(val != null, DomMsgID.InvalidValueName, new object[] { name });
                    obj3 = val as DocumentObject;
                    this.AssertCondition(obj3 != null, DomMsgID.SymbolIsNotAnObject, new object[] { name });
                    this.ReadCode();
                    this.AssertCondition(this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Identifier, DomMsgID.InvalidValueName, new object[] { this.scanner.Token });
                    name = this.scanner.Token;
                    this.AssertCondition(name[0] != '_', DomMsgID.NoAccess, new object[] { this.scanner.Token });
                    this.ReadCode();
                }
                switch (this.Symbol)
                {
                    case MigraDoc.DocumentObjectModel.IO.Symbol.PlusAssign:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.MinusAssign:
                    {
                        if (!(obj3 is ParagraphFormat))
                        {
                            this.ThrowParserException(DomMsgID.SymbolNotAllowed, new object[] { this.scanner.Token });
                        }
                        if (string.Compare(name, "TabStops", true) != 0)
                        {
                            this.ThrowParserException(DomMsgID.InvalidValueForOperation, new object[] { name, this.scanner.Token });
                        }
                        ParagraphFormat format = (ParagraphFormat) obj3;
                        tabStops = format.TabStops;
                        flag = this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.PlusAssign;
                        stop = new TabStop();
                        this.ReadCode();
                        if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft)
                        {
                            this.ParseAttributeBlock(stop);
                        }
                        else
                        {
                            if (((this.Symbol != MigraDoc.DocumentObjectModel.IO.Symbol.StringLiteral) && (this.Symbol != MigraDoc.DocumentObjectModel.IO.Symbol.RealLiteral)) && (this.Symbol != MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral))
                            {
                                break;
                            }
                            Unit token = this.Token;
                            stop.SetValue("Position", token);
                            this.ReadCode();
                        }
                        goto Label_0281;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Assign:
                    {
                        ValueDescriptor vd = obj3.Meta[name];
                        this.AssertCondition(vd != null, DomMsgID.InvalidValueName, new object[] { name });
                        this.ParseAssign(obj3, vd);
                        return;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft:
                        val = obj3.GetValue(name);
                        this.AssertCondition(val != null, DomMsgID.InvalidValueName, new object[] { name });
                        if (val is DocumentObject)
                        {
                            this.ParseAttributeBlock((DocumentObject) val);
                        }
                        else
                        {
                            this.ThrowParserException(DomMsgID.SymbolIsNotAnObject, new object[] { name });
                        }
                        return;

                    default:
                        goto Label_0302;
                }
                this.ThrowParserException(DomMsgID.UnexpectedSymbol, new object[] { this.Token });
            Label_0281:
                if (flag)
                {
                    tabStops.AddTabStop(stop);
                }
                else
                {
                    tabStops.RemoveTabStop(stop.Position);
                }
                return;
            Label_0302:;
                this.ThrowParserException(DomMsgID.SymbolNotAllowed, new object[] { this.scanner.Token });
            }
            catch (DdlParserException exception)
            {
                this.ReportParserException(exception);
                this.AdjustToNextBlock();
            }
            catch (ArgumentException exception2)
            {
                this.ReportParserException(exception2, DomMsgID.InvalidAssignment, new string[] { name });
            }
        }

        private void ParseAxes(Axis axis, MigraDoc.DocumentObjectModel.IO.Symbol symbolAxis)
        {
            try
            {
                this.ReadCode();
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
                {
                    this.ParseAttributes(axis, false);
                    this.ReadCode();
                }
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft)
                {
                    goto Label_0033;
                }
                return;
            Label_002C:
                this.ReadCode();
            Label_0033:
                if (this.Symbol != MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight)
                {
                    goto Label_002C;
                }
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight, DomMsgID.MissingBraceRight, new object[] { this.GetSymbolText(symbolAxis) });
                this.ReadCode();
            }
            catch (DdlParserException exception)
            {
                this.ReportParserException(exception);
                this.AdjustToNextBlock();
            }
        }

        private void ParseBarcode(DocumentElements elements)
        {
            try
            {
                this.ReadCode();
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenLeft, DomMsgID.MissingParenLeft, new object[] { this.GetSymbolText(MigraDoc.DocumentObjectModel.IO.Symbol.Barcode) });
                this.ReadCode();
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.StringLiteral, DomMsgID.UnexpectedSymbol);
                Barcode element = elements.AddBarcode();
                element.SetValue("Code", this.Token);
                this.ReadCode();
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Comma)
                {
                    this.ReadCode();
                    this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Identifier, DomMsgID.IdentifierExpected, new object[] { this.Token });
                    BarcodeType val = (BarcodeType) Enum.Parse(typeof(BarcodeType), this.Token, true);
                    element.SetValue("type", val);
                    this.ReadCode();
                }
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenRight, DomMsgID.MissingParenRight, new object[] { this.GetSymbolText(MigraDoc.DocumentObjectModel.IO.Symbol.Barcode) });
                this.ReadCode();
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
                {
                    this.ParseAttributes(element);
                }
            }
            catch (DdlParserException exception)
            {
                this.ReportParserException(exception);
                this.AdjustToNextBlock();
            }
        }

        private void ParseBoldItalicEtc(FormattedText formattedText, int nestingLevel)
        {
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft);
            this.ParseFormattedText(formattedText.Elements, nestingLevel);
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
        }

        private void ParseBoolAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            this.AssertCondition((this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.True) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.False), DomMsgID.BoolExpected, new object[] { this.scanner.Token });
            dom.SetValue(vd.ValueName, this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.True);
            this.ReadCode();
        }

        private void ParseCell(Cell cell)
        {
            this.ReadCode();
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
            {
                this.ParseAttributes(cell);
            }
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft)
            {
                if (this.IsParagraphContent())
                {
                    this.ParseParagraphContent(cell.Elements, null);
                }
                else
                {
                    this.ReadCode();
                    if (this.Symbol != MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight)
                    {
                        this.ParseDocumentElements(cell.Elements, MigraDoc.DocumentObjectModel.IO.Symbol.Cell);
                    }
                }
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
                this.ReadCode();
            }
        }

        private void ParseChart(DocumentElements elements)
        {
            ChartType line = ChartType.Line;
            Chart element = null;
            try
            {
                this.ReadCode();
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenLeft, DomMsgID.MissingParenLeft, new object[] { this.GetSymbolText(MigraDoc.DocumentObjectModel.IO.Symbol.Chart) });
                this.ReadCode();
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Identifier, DomMsgID.IdentifierExpected, new object[] { this.Token });
                string token = this.Token;
                this.ReadCode();
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenRight, DomMsgID.MissingParenRight, new object[] { this.GetSymbolText(MigraDoc.DocumentObjectModel.IO.Symbol.Chart) });
                try
                {
                    line = (ChartType) Enum.Parse(typeof(ChartType), token, true);
                }
                catch (Exception exception)
                {
                    this.ThrowParserException(exception, DomMsgID.UnknownChartType, new object[] { token });
                }
                element = elements.AddChart(line);
                this.ReadCode();
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
                {
                    this.ParseAttributes(element);
                }
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft, DomMsgID.MissingBraceLeft, new object[] { this.GetSymbolText(MigraDoc.DocumentObjectModel.IO.Symbol.Chart) });
                this.ReadCode();
                bool flag = true;
                while (flag)
                {
                    MigraDoc.DocumentObjectModel.IO.Symbol symbol = this.Symbol;
                    if (symbol != MigraDoc.DocumentObjectModel.IO.Symbol.Eof)
                    {
                        switch (symbol)
                        {
                            case MigraDoc.DocumentObjectModel.IO.Symbol.HeaderArea:
                            {
                                this.ParseArea(element.HeaderArea);
                                continue;
                            }
                            case MigraDoc.DocumentObjectModel.IO.Symbol.FooterArea:
                            {
                                this.ParseArea(element.FooterArea);
                                continue;
                            }
                            case MigraDoc.DocumentObjectModel.IO.Symbol.TopArea:
                            {
                                this.ParseArea(element.TopArea);
                                continue;
                            }
                            case MigraDoc.DocumentObjectModel.IO.Symbol.BottomArea:
                            {
                                this.ParseArea(element.BottomArea);
                                continue;
                            }
                            case MigraDoc.DocumentObjectModel.IO.Symbol.LeftArea:
                            {
                                this.ParseArea(element.LeftArea);
                                continue;
                            }
                            case MigraDoc.DocumentObjectModel.IO.Symbol.RightArea:
                            {
                                this.ParseArea(element.RightArea);
                                continue;
                            }
                            case MigraDoc.DocumentObjectModel.IO.Symbol.PlotArea:
                            {
                                this.ParseArea(element.PlotArea);
                                continue;
                            }
                            case MigraDoc.DocumentObjectModel.IO.Symbol.XAxis:
                            {
                                this.ParseAxes(element.XAxis, this.Symbol);
                                continue;
                            }
                            case MigraDoc.DocumentObjectModel.IO.Symbol.YAxis:
                            {
                                this.ParseAxes(element.YAxis, this.Symbol);
                                continue;
                            }
                            case MigraDoc.DocumentObjectModel.IO.Symbol.ZAxis:
                            {
                                this.ParseAxes(element.ZAxis, this.Symbol);
                                continue;
                            }
                            case MigraDoc.DocumentObjectModel.IO.Symbol.Series:
                            {
                                this.ParseSeries(element.SeriesCollection.AddSeries());
                                continue;
                            }
                            case MigraDoc.DocumentObjectModel.IO.Symbol.XValues:
                            {
                                this.ParseSeries(element.XValues.AddXSeries());
                                continue;
                            }
                            case MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight:
                                goto Label_017B;
                        }
                        goto Label_025C;
                    }
                    this.ThrowParserException(DomMsgID.UnexpectedEndOfFile, new object[0]);
                    continue;
                Label_017B:
                    flag = false;
                    continue;
                Label_025C:;
                    this.ThrowParserException(DomMsgID.UnexpectedSymbol, new object[] { this.Token });
                }
                this.ReadCode();
            }
            catch (DdlParserException exception2)
            {
                this.ReportParserException(exception2);
                this.AdjustToNextBlock();
            }
        }

        private void ParseChr(ParagraphElements elements)
        {
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Chr);
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenLeft);
            char ch = '\0';
            SymbolName symbolType = (SymbolName) 0;
            int count = 1;
            this.ReadCode();
            if (this.TokenType == MigraDoc.DocumentObjectModel.IO.TokenType.IntegerLiteral)
            {
                int tokenValueAsInt = this.scanner.GetTokenValueAsInt();
                if ((tokenValueAsInt >= 1) && (tokenValueAsInt < 0x100))
                {
                    ch = (char) tokenValueAsInt;
                }
                else
                {
                    this.ThrowParserException(DomMsgID.OutOfRange, new object[] { "1 - 255" });
                }
            }
            else
            {
                this.ThrowParserException(DomMsgID.UnexpectedSymbol, new object[] { this.Token });
            }
            this.ReadCode();
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Comma)
            {
                this.ReadCode();
                if (this.TokenType == MigraDoc.DocumentObjectModel.IO.TokenType.IntegerLiteral)
                {
                    count = this.scanner.GetTokenValueAsInt();
                }
                this.ReadCode();
            }
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenRight);
            if (symbolType != ((SymbolName) 0))
            {
                elements.AddCharacter(symbolType, count);
            }
            else
            {
                elements.AddCharacter(ch, count);
            }
        }

        private Color ParseCMYK()
        {
            double num6;
            double num7;
            double num8;
            double num9;
            double num10;
            double num5 = 0.0;
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenLeft);
            this.ReadCode();
            this.AssertCondition((this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.RealLiteral), DomMsgID.NumberExpected, new object[] { this.scanner.Token });
            double tokenValueAsReal = this.scanner.GetTokenValueAsReal();
            this.AssertCondition((tokenValueAsReal >= 0.0) && (tokenValueAsReal <= 100.0), DomMsgID.InvalidRange, new object[] { "0.0 - 100.0" });
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Comma);
            this.ReadCode();
            this.AssertCondition((this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.RealLiteral), DomMsgID.NumberExpected, new object[] { this.scanner.Token });
            double num2 = this.scanner.GetTokenValueAsReal();
            this.AssertCondition((num2 >= 0.0) && (num2 <= 100.0), DomMsgID.InvalidRange, new object[] { "0.0 - 100.0" });
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Comma);
            this.ReadCode();
            this.AssertCondition((this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.RealLiteral), DomMsgID.NumberExpected, new object[] { this.scanner.Token });
            double num3 = this.scanner.GetTokenValueAsReal();
            this.AssertCondition((num3 >= 0.0) && (num3 <= 100.0), DomMsgID.InvalidRange, new object[] { "0.0 - 100.0" });
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Comma);
            this.ReadCode();
            this.AssertCondition((this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.RealLiteral), DomMsgID.NumberExpected, new object[] { this.scanner.Token });
            double num4 = this.scanner.GetTokenValueAsReal();
            this.AssertCondition((num4 >= 0.0) && (num4 <= 100.0), DomMsgID.InvalidRange, new object[] { "0.0 - 100.0" });
            this.ReadCode();
            bool flag = false;
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Comma)
            {
                flag = true;
                this.ReadCode();
                this.AssertCondition((this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.RealLiteral), DomMsgID.NumberExpected, new object[] { this.scanner.Token });
                num5 = this.scanner.GetTokenValueAsReal();
                this.AssertCondition((num5 >= 0.0) && (num5 <= 100.0), DomMsgID.InvalidRange, new object[] { "0.0 - 100.0" });
                this.ReadCode();
            }
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenRight);
            this.ReadCode();
            if (flag)
            {
                num6 = tokenValueAsReal;
                num7 = num2;
                num8 = num3;
                num9 = num4;
                num10 = num5;
            }
            else
            {
                num6 = 100.0;
                num7 = tokenValueAsReal;
                num8 = num2;
                num9 = num3;
                num10 = num4;
            }
            return Color.FromCmyk(num6, num7, num8, num9, num10);
        }

        private Color ParseColor()
        {
            this.MoveToCode();
            Color empty = Color.Empty;
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Identifier)
            {
                string token = this.Token;
                if (token != null)
                {
                    if (token == "RGB")
                    {
                        return this.ParseRGB();
                    }
                    if (token == "CMYK")
                    {
                        return this.ParseCMYK();
                    }
                    if (token == "HSB")
                    {
                        throw new NotImplementedException("ParseColor(HSB)");
                    }
                    if (token == "Lab")
                    {
                        throw new NotImplementedException("ParseColor(Lab)");
                    }
                    if (token == "GRAY")
                    {
                        return this.ParseGray();
                    }
                }
                try
                {
                    empty = Color.Parse(this.Token);
                    this.ReadCode();
                }
                catch (Exception exception)
                {
                    this.ThrowParserException(exception, DomMsgID.InvalidColor, new object[] { this.scanner.Token });
                }
                return empty;
            }
            if ((this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.HexIntegerLiteral))
            {
                empty = new Color(this.scanner.GetTokenValueAsUInt());
                this.ReadCode();
                return empty;
            }
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.StringLiteral)
            {
                throw new NotImplementedException("ParseColor(color-name)");
            }
            this.ThrowParserException(DomMsgID.StringExpected, new object[] { this.scanner.Token });
            return empty;
        }

        private void ParseColorAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            vd.GetValue(dom, GV.ReadWrite);
            Color val = this.ParseColor();
            dom.SetValue(vd.ValueName, val);
        }

        private void ParseColumn(Column column)
        {
            this.ReadCode();
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
            {
                this.ParseAttributes(column);
            }
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft)
            {
                this.ReadCode();
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
                this.ReadCode();
            }
        }

        private void ParseColumns(Table table)
        {
            this.ReadCode();
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
            {
                this.ParseAttributes(table.Columns);
            }
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft);
            this.ReadCode();
            bool flag = true;
            while (flag)
            {
                MigraDoc.DocumentObjectModel.IO.Symbol symbol = this.Symbol;
                if (symbol != MigraDoc.DocumentObjectModel.IO.Symbol.Eof)
                {
                    if (symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight)
                    {
                        goto Label_005A;
                    }
                    if (symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Column)
                    {
                        goto Label_0065;
                    }
                    goto Label_0073;
                }
                this.ThrowParserException(DomMsgID.UnexpectedEndOfFile, new object[0]);
                continue;
            Label_005A:
                flag = false;
                this.ReadCode();
                continue;
            Label_0065:
                this.ParseColumn(table.AddColumn());
                continue;
            Label_0073:
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Column);
            }
        }

        internal Document ParseDocument(Document document)
        {
            if (document == null)
            {
                document = new Document();
            }
            this.MoveToCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Document);
            this.ReadCode();
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
            {
                this.ParseAttributes(document);
            }
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft);
            this.ReadCode();
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Styles)
            {
                this.ParseStyles(document.Styles);
            }
            while (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Section)
            {
                this.ParseSection(document.Sections);
            }
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
            this.ReadCode();
            this.AssertCondition(this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Eof, DomMsgID.EndOfFileExpected, new object[0]);
            return document;
        }

        private DocumentElements ParseDocumentElements(DocumentElements elements, MigraDoc.DocumentObjectModel.IO.Symbol context)
        {
            while (this.TokenType == MigraDoc.DocumentObjectModel.IO.TokenType.KeyWord)
            {
                MigraDoc.DocumentObjectModel.IO.Symbol symbol = this.Symbol;
                if (symbol != MigraDoc.DocumentObjectModel.IO.Symbol.Paragraph)
                {
                    switch (symbol)
                    {
                        case MigraDoc.DocumentObjectModel.IO.Symbol.Image:
                        {
                            this.ParseImage(elements.AddImage(""), false);
                            continue;
                        }
                        case MigraDoc.DocumentObjectModel.IO.Symbol.TextFrame:
                        {
                            this.ParseTextFrame(elements);
                            continue;
                        }
                        case MigraDoc.DocumentObjectModel.IO.Symbol.Chart:
                        {
                            this.ParseChart(elements);
                            continue;
                        }
                        case MigraDoc.DocumentObjectModel.IO.Symbol.PageBreak:
                        {
                            this.ParsePageBreak(elements);
                            continue;
                        }
                        case MigraDoc.DocumentObjectModel.IO.Symbol.Barcode:
                        {
                            this.ParseBarcode(elements);
                            continue;
                        }
                        case MigraDoc.DocumentObjectModel.IO.Symbol.Table:
                            goto Label_004B;
                    }
                    goto Label_0084;
                }
                this.ParseParagraph(elements);
                continue;
            Label_004B:
                this.ParseTable(elements, null);
                continue;
            Label_0084:;
                this.ThrowParserException(DomMsgID.UnexpectedSymbol, new object[] { this.scanner.Token });
            }
            return elements;
        }

        internal DocumentObject ParseDocumentObject()
        {
            DocumentObject obj2 = null;
            this.MoveToCode();
            switch (this.Symbol)
            {
                case MigraDoc.DocumentObjectModel.IO.Symbol.Styles:
                    obj2 = this.ParseStyles(new Styles());
                    break;

                case MigraDoc.DocumentObjectModel.IO.Symbol.Document:
                    obj2 = this.ParseDocument(null);
                    break;

                case MigraDoc.DocumentObjectModel.IO.Symbol.Section:
                    obj2 = this.ParseSection(new Sections());
                    break;

                case MigraDoc.DocumentObjectModel.IO.Symbol.Paragraph:
                    obj2 = new DocumentElements();
                    this.ParseParagraph((DocumentElements) obj2);
                    break;

                case MigraDoc.DocumentObjectModel.IO.Symbol.Table:
                    obj2 = new Table();
                    this.ParseTable(null, (Table) obj2);
                    break;

                case MigraDoc.DocumentObjectModel.IO.Symbol.TextFrame:
                {
                    DocumentElements elements = new DocumentElements();
                    this.ParseTextFrame(elements);
                    obj2 = elements[0];
                    break;
                }
                case MigraDoc.DocumentObjectModel.IO.Symbol.Chart:
                    throw new NotImplementedException();

                default:
                    this.ThrowParserException(DomMsgID.UnexpectedSymbol, new object[0]);
                    break;
            }
            this.ReadCode();
            this.AssertCondition(this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Eof, DomMsgID.EndOfFileExpected, new object[0]);
            return obj2;
        }

        private void ParseDocumentObjectAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            object obj2 = vd.GetValue(dom, GV.ReadWrite);
            DocumentObject obj1 = (DocumentObject) obj2;
            try
            {
                if (this.Symbol != MigraDoc.DocumentObjectModel.IO.Symbol.Null)
                {
                    throw new Exception("Case: TopPosition");
                }
                string valueName = vd.ValueName;
                Type valueType = vd.ValueType;
                if (typeof(Border) == valueType)
                {
                    ((Border) obj2).Clear();
                }
                else if (typeof(Borders) == valueType)
                {
                    ((Borders) obj2).ClearAll();
                }
                else if (typeof(Shading) == valueType)
                {
                    ((Shading) obj2).Clear();
                }
                else if (typeof(TabStops) == valueType)
                {
                    ((TabStops) vd.GetValue(dom, GV.ReadWrite)).ClearAll();
                }
                else
                {
                    this.ThrowParserException(DomMsgID.NullAssignmentNotSupported, new object[] { vd.ValueName });
                }
                this.ReadCode();
            }
            catch (Exception exception)
            {
                this.ReportParserException(exception, DomMsgID.InvalidAssignment, new string[] { vd.ValueName });
            }
        }

        private string ParseElementName()
        {
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenLeft);
            this.ReadCode();
            if (this.Symbol != MigraDoc.DocumentObjectModel.IO.Symbol.StringLiteral)
            {
                this.ThrowParserException(DomMsgID.StringExpected, new object[] { this.Token });
            }
            string token = this.Token;
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenRight);
            return token;
        }

        private void ParseEnumAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Identifier, DomMsgID.IdentifierExpected, new object[] { this.scanner.Token });
            try
            {
                object val = Enum.Parse(vd.ValueType, this.Token, true);
                dom.SetValue(vd.ValueName, val);
            }
            catch (Exception exception)
            {
                this.ThrowParserException(exception, DomMsgID.InvalidEnum, new object[] { this.scanner.Token, vd.ValueName });
            }
            this.ReadCode();
        }

        private void ParseField(ParagraphElements elements, int nestingLevel)
        {
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Field);
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenLeft);
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Identifier);
            string str = this.Token.ToLower();
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenRight);
            DocumentObject element = null;
            switch (str)
            {
                case "date":
                    element = elements.AddDateField();
                    break;

                case "page":
                    element = elements.AddPageField();
                    break;

                case "numpages":
                    element = elements.AddNumPagesField();
                    break;

                case "info":
                    element = elements.AddInfoField(InfoFieldType.Title);
                    break;

                case "sectionpages":
                    element = elements.AddSectionPagesField();
                    break;

                case "section":
                    element = elements.AddSectionField();
                    break;

                case "bookmark":
                    element = elements.AddBookmark("");
                    break;

                case "pageref":
                    element = elements.AddPageRefField("");
                    break;
            }
            this.AssertCondition(element != null, DomMsgID.InvalidFieldType, new object[] { this.Token });
            if (this.scanner.PeekSymbol() == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
            {
                this.ReadCode();
                this.ParseAttributes(element, false);
            }
        }

        private void ParseFont(FormattedText formattedText, int nestingLevel)
        {
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Font);
            this.ReadCode();
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.ParenLeft)
            {
                formattedText.Style = this.ParseElementName();
                this.ReadCode();
            }
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
            {
                this.ParseAttributes(formattedText);
            }
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft);
            this.ParseFormattedText(formattedText.Elements, nestingLevel);
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
        }

        private void ParseFontColor(FormattedText formattedText, int nestingLevel)
        {
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.FontColor);
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenLeft);
            this.ReadCode();
            Color color = this.ParseColor();
            formattedText.Font.Color = color;
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenRight);
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft);
            this.ParseFormattedText(formattedText.Elements, nestingLevel);
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
        }

        private void ParseFontSize(FormattedText formattedText, int nestingLevel)
        {
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.FontSize);
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenLeft);
            this.ReadCode();
            formattedText.Font.Size = this.Token;
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenRight);
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft);
            this.ParseFormattedText(formattedText.Elements, nestingLevel);
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
        }

        private void ParseFootnote(ParagraphElements elements, int nestingLevel)
        {
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Footnote);
            this.ReadCode();
            Footnote element = elements.AddFootnote();
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
            {
                this.ParseAttributes(element);
            }
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft);
            if (this.IsParagraphContent())
            {
                Paragraph paragraph = element.Elements.AddParagraph();
                this.ParseParagraphContent(element.Elements, paragraph);
            }
            else
            {
                this.ReadCode();
                this.ParseDocumentElements(element.Elements, MigraDoc.DocumentObjectModel.IO.Symbol.Footnote);
            }
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
        }

        private void ParseFormattedText(ParagraphElements elements, int nestingLevel)
        {
            this.MoveToParagraphContent();
            bool flag = true;
            bool rootLevel = nestingLevel == 0;
            this.ReadText(rootLevel);
            while (flag)
            {
                switch (this.Symbol)
                {
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Eof:
                    {
                        this.ThrowParserException(DomMsgID.UnexpectedEndOfFile, new object[0]);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Comment:
                    {
                        this.ReadText(rootLevel);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight:
                    {
                        flag = false;
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Bold:
                    {
                        this.ParseBoldItalicEtc(elements.AddFormattedText(TextFormat.Bold), nestingLevel + 1);
                        this.ReadText(rootLevel);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Italic:
                    {
                        this.ParseBoldItalicEtc(elements.AddFormattedText(TextFormat.Italic), nestingLevel + 1);
                        this.ReadText(rootLevel);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Underline:
                    {
                        this.ParseBoldItalicEtc(elements.AddFormattedText(TextFormat.Underline), nestingLevel + 1);
                        this.ReadText(rootLevel);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.FontSize:
                    {
                        this.ParseFontSize(elements.AddFormattedText(), nestingLevel + 1);
                        this.ReadText(rootLevel);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.FontColor:
                    {
                        this.ParseFontColor(elements.AddFormattedText(), nestingLevel + 1);
                        this.ReadText(rootLevel);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Font:
                    {
                        this.ParseFont(elements.AddFormattedText(), nestingLevel + 1);
                        this.ReadText(rootLevel);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Hyperlink:
                    {
                        this.ParseHyperlink(elements, nestingLevel + 1);
                        this.ReadText(rootLevel);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Text:
                    {
                        elements.AddText(this.Token);
                        this.ReadText(rootLevel);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Tab:
                    {
                        this.RemoveTrailingBlank(elements);
                        elements.AddTab();
                        this.scanner.MoveToNonWhiteSpaceOrEol();
                        this.ReadText(rootLevel);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.LineBreak:
                    {
                        this.RemoveTrailingBlank(elements);
                        elements.AddLineBreak();
                        this.scanner.MoveToNonWhiteSpaceOrEol();
                        this.ReadText(rootLevel);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Space:
                    {
                        this.RemoveTrailingBlank(elements);
                        this.ParseSpace(elements, nestingLevel + 1);
                        this.scanner.MoveToNonWhiteSpaceOrEol();
                        this.ReadText(rootLevel);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Field:
                    {
                        this.ParseField(elements, nestingLevel + 1);
                        this.ReadText(rootLevel);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Symbol:
                    {
                        this.ParseSymbol(elements);
                        this.ReadText(rootLevel);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Chr:
                    {
                        this.ParseChr(elements);
                        this.ReadText(rootLevel);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Footnote:
                    {
                        this.ParseFootnote(elements, nestingLevel + 1);
                        this.ReadText(rootLevel);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.EmptyLine:
                    {
                        elements.AddCharacter((SymbolName) (-201326585));
                        this.ReadText(rootLevel);
                        continue;
                    }
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Image:
                    {
                        this.ParseImage(elements.AddImage(""), true);
                        this.ReadText(rootLevel);
                        continue;
                    }
                }
                this.ThrowParserException(DomMsgID.UnexpectedSymbol, new object[] { this.Token });
            }
        }

        private Color ParseGray()
        {
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenLeft);
            this.ReadCode();
            this.AssertCondition((this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.HexIntegerLiteral), DomMsgID.IntegerExpected, new object[] { this.scanner.Token });
            double tokenValueAsReal = this.scanner.GetTokenValueAsReal();
            this.AssertCondition((tokenValueAsReal >= 0.0) && (tokenValueAsReal <= 100.0), DomMsgID.InvalidRange, new object[] { "0.0 - 100.0" });
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenRight);
            this.ReadCode();
            uint num2 = (uint) (((1.0 - (tokenValueAsReal / 100.0)) * 255.0) + 0.5);
            return new Color(((0xff000000 + (num2 << 0x10)) + (num2 << 8)) + num2);
        }

        private void ParseHeaderFooter(Section section)
        {
            if (section == null)
            {
                throw new ArgumentNullException("section");
            }
            HeaderFooter element = null;
            try
            {
                MigraDoc.DocumentObjectModel.IO.Symbol symbol = this.Symbol;
                bool flag = (((symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Header) || (symbol == MigraDoc.DocumentObjectModel.IO.Symbol.PrimaryHeader)) || (symbol == MigraDoc.DocumentObjectModel.IO.Symbol.FirstPageHeader)) || (symbol == MigraDoc.DocumentObjectModel.IO.Symbol.EvenPageHeader);
                element = new HeaderFooter();
                this.ReadCode();
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
                {
                    this.ParseAttributes(element);
                }
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft);
                if (this.IsParagraphContent())
                {
                    Paragraph paragraph = element.Elements.AddParagraph();
                    this.ParseParagraphContent(element.Elements, paragraph);
                }
                else
                {
                    this.ReadCode();
                    this.ParseDocumentElements(element.Elements, MigraDoc.DocumentObjectModel.IO.Symbol.HeaderOrFooter);
                }
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
                this.ReadCode();
                HeadersFooters footers = flag ? section.Headers : section.Footers;
                switch (symbol)
                {
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Header:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.Footer:
                        footers.Primary = element.Clone();
                        footers.EvenPage = element.Clone();
                        footers.FirstPage = element.Clone();
                        return;
                }
                switch (symbol)
                {
                    case MigraDoc.DocumentObjectModel.IO.Symbol.PrimaryHeader:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.PrimaryFooter:
                        footers.Primary = element;
                        return;

                    case MigraDoc.DocumentObjectModel.IO.Symbol.FirstPageHeader:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.FirstPageFooter:
                        footers.FirstPage = element;
                        return;

                    case MigraDoc.DocumentObjectModel.IO.Symbol.EvenPageHeader:
                    case MigraDoc.DocumentObjectModel.IO.Symbol.EvenPageFooter:
                        footers.EvenPage = element;
                        return;

                    case MigraDoc.DocumentObjectModel.IO.Symbol.Footer:
                        return;
                }
            }
            catch (DdlParserException exception)
            {
                this.ReportParserException(exception);
                this.AdjustToNextBlock();
            }
        }

        private void ParseHyperlink(ParagraphElements elements, int nestingLevel)
        {
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Hyperlink);
            this.ReadCode();
            Hyperlink element = elements.AddHyperlink("");
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
            {
                this.ParseAttributes(element);
            }
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft);
            this.ParseFormattedText(element.Elements, nestingLevel);
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
        }

        private void ParseImage(Image image, bool paragraphContent)
        {
            try
            {
                this.MoveToCode();
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Image);
                this.ReadCode();
                if (this.scanner.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.ParenLeft)
                {
                    image.Name = this.ParseElementName();
                }
                if (this.scanner.PeekSymbol() == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
                {
                    this.ReadCode();
                    this.ParseAttributes(image, !paragraphContent);
                }
                else if (!paragraphContent)
                {
                    this.ReadCode();
                }
            }
            catch (DdlParserException exception)
            {
                this.ReportParserException(exception);
                this.AdjustToNextBlock();
            }
        }

        private void ParseIntegerAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            this.AssertCondition(((this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.HexIntegerLiteral)) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.StringLiteral), DomMsgID.IntegerExpected, new object[] { this.Token });
            int val = int.Parse(this.scanner.Token, CultureInfo.InvariantCulture);
            dom.SetValue(vd.ValueName, val);
            this.ReadCode();
        }

        private void ParseLegend(Legend legend)
        {
            try
            {
                this.ReadCode();
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
                {
                    this.ParseAttributes(legend, false);
                    this.ReadCode();
                }
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft)
                {
                    this.AdjustToNextBlock();
                }
            }
            catch (DdlParserException exception)
            {
                this.ReportParserException(exception);
                this.AdjustToNextBlock();
            }
        }

        private void ParsePageBreak(DocumentElements elements)
        {
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.PageBreak);
            elements.AddPageBreak();
            this.ReadCode();
        }

        private void ParseParagraph(DocumentElements elements)
        {
            this.MoveToCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Paragraph);
            Paragraph element = elements.AddParagraph();
            try
            {
                this.ReadCode();
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
                {
                    this.ParseAttributes(element);
                }
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft)
                {
                    this.ParseParagraphContent(elements, element);
                    this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
                    this.ReadCode();
                }
            }
            catch (DdlParserException exception)
            {
                this.ReportParserException(exception);
                this.AdjustToNextBlock();
            }
        }

        private void ParseParagraphContent(DocumentElements elements, Paragraph paragraph)
        {
            Paragraph paragraph2 = paragraph;
            if (paragraph2 == null)
            {
                paragraph2 = elements.AddParagraph();
            }
            while (paragraph2 != null)
            {
                this.ParseFormattedText(paragraph2.Elements, 0);
                if ((this.Symbol != MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight) && (this.Symbol != MigraDoc.DocumentObjectModel.IO.Symbol.Eof))
                {
                    paragraph2 = elements.AddParagraph();
                }
                else
                {
                    paragraph2 = null;
                }
            }
        }

        private void ParsePoint(Point point)
        {
            try
            {
                this.ReadCode();
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
                {
                    this.ParseAttributes(point);
                }
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft, DomMsgID.MissingBraceLeft, new object[] { this.GetSymbolText(MigraDoc.DocumentObjectModel.IO.Symbol.Point) });
                this.ReadCode();
                point.Value = this.scanner.GetTokenValueAsReal();
                this.ReadCode();
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight, DomMsgID.MissingBraceRight, new object[] { this.GetSymbolText(MigraDoc.DocumentObjectModel.IO.Symbol.Point) });
                this.ReadCode();
            }
            catch (DdlParserException exception)
            {
                this.ReportParserException(exception);
                this.AdjustToNextBlock();
            }
        }

        private void ParseRealAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            this.AssertCondition(((this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.RealLiteral) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral)) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.StringLiteral), DomMsgID.RealExpected, new object[] { this.scanner.Token });
            double val = double.Parse(this.scanner.Token, CultureInfo.InvariantCulture);
            dom.SetValue(vd.ValueName, val);
            this.ReadCode();
        }

        private Color ParseRGB()
        {
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenLeft);
            this.ReadCode();
            this.AssertCondition((this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.HexIntegerLiteral), DomMsgID.IntegerExpected, new object[] { this.scanner.Token });
            uint tokenValueAsUInt = this.scanner.GetTokenValueAsUInt();
            this.AssertCondition((tokenValueAsUInt >= 0) && (tokenValueAsUInt <= 0xff), DomMsgID.InvalidRange, new object[] { "0 - 255" });
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Comma);
            this.ReadCode();
            this.AssertCondition((this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.HexIntegerLiteral), DomMsgID.IntegerExpected, new object[] { this.scanner.Token });
            uint num2 = this.scanner.GetTokenValueAsUInt();
            this.AssertCondition((num2 >= 0) && (num2 <= 0xff), DomMsgID.InvalidRange, new object[] { "0 - 255" });
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Comma);
            this.ReadCode();
            this.AssertCondition((this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.HexIntegerLiteral), DomMsgID.IntegerExpected, new object[] { this.scanner.Token });
            uint num3 = this.scanner.GetTokenValueAsUInt();
            this.AssertCondition((num3 >= 0) && (num3 <= 0xff), DomMsgID.InvalidRange, new object[] { "0 - 255" });
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenRight);
            this.ReadCode();
            return new Color(((0xff000000 | (tokenValueAsUInt << 0x10)) | (num2 << 8)) | num3);
        }

        private void ParseRow(Row row)
        {
            this.ReadCode();
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
            {
                this.ParseAttributes(row);
            }
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft)
            {
                this.ReadCode();
                bool flag = true;
                int num = 0;
                int count = row.Cells.Count;
                while (flag)
                {
                    MigraDoc.DocumentObjectModel.IO.Symbol symbol = this.Symbol;
                    if (symbol != MigraDoc.DocumentObjectModel.IO.Symbol.Eof)
                    {
                        if (symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight)
                        {
                            goto Label_0068;
                        }
                        if (symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Cell)
                        {
                            goto Label_0073;
                        }
                        goto Label_0086;
                    }
                    this.ThrowParserException(DomMsgID.UnexpectedEndOfFile, new object[0]);
                    continue;
                Label_0068:
                    flag = false;
                    this.ReadCode();
                    continue;
                Label_0073:
                    this.ParseCell(row[num]);
                    num++;
                    continue;
                Label_0086:;
                    this.ThrowParserException(DomMsgID.UnexpectedSymbol, new object[] { this.Token });
                }
            }
        }

        private void ParseRows(Table table)
        {
            this.ReadCode();
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
            {
                this.ParseAttributes(table.Rows);
            }
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft);
            this.ReadCode();
            bool flag = true;
            while (flag)
            {
                MigraDoc.DocumentObjectModel.IO.Symbol symbol = this.Symbol;
                if (symbol != MigraDoc.DocumentObjectModel.IO.Symbol.Eof)
                {
                    if (symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight)
                    {
                        goto Label_005A;
                    }
                    if (symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Row)
                    {
                        goto Label_0065;
                    }
                    goto Label_0073;
                }
                this.ThrowParserException(DomMsgID.UnexpectedEndOfFile, new object[0]);
                continue;
            Label_005A:
                this.ReadCode();
                flag = false;
                continue;
            Label_0065:
                this.ParseRow(table.AddRow());
                continue;
            Label_0073:
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Row);
            }
        }

        private Section ParseSection(Sections sections)
        {
            this.MoveToCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Section);
            Section element = null;
            try
            {
                element = sections.AddSection();
                this.ReadCode();
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
                {
                    this.ParseAttributes(element);
                }
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft);
                if (this.IsParagraphContent())
                {
                    Paragraph paragraph = element.Elements.AddParagraph();
                    this.ParseParagraphContent(element.Elements, paragraph);
                }
                else
                {
                    this.ReadCode();
                    while (this.IsHeaderFooter())
                    {
                        this.ParseHeaderFooter(element);
                    }
                    this.ParseDocumentElements(element.Elements, MigraDoc.DocumentObjectModel.IO.Symbol.Section);
                }
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
                this.ReadCode();
            }
            catch (DdlParserException exception)
            {
                this.ReportParserException(exception);
                this.AdjustToNextBlock();
            }
            return element;
        }

        private void ParseSeries(Series series)
        {
            try
            {
                this.ReadCode();
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
                {
                    this.ParseAttributes(series);
                }
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft, DomMsgID.MissingBraceLeft, new object[] { this.GetSymbolText(MigraDoc.DocumentObjectModel.IO.Symbol.Series) });
                this.ReadCode();
                bool flag = true;
                bool cond = true;
                while (flag)
                {
                    switch (this.Symbol)
                    {
                        case MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight:
                        {
                            flag = false;
                            continue;
                        }
                        case MigraDoc.DocumentObjectModel.IO.Symbol.Comma:
                        {
                            cond = true;
                            this.ReadCode();
                            continue;
                        }
                        case MigraDoc.DocumentObjectModel.IO.Symbol.Point:
                        {
                            this.AssertCondition(cond, DomMsgID.MissingComma, new object[0]);
                            this.ParsePoint(series.Add((double) 0.0));
                            cond = false;
                            continue;
                        }
                        case MigraDoc.DocumentObjectModel.IO.Symbol.Eof:
                        {
                            this.ThrowParserException(DomMsgID.UnexpectedEndOfFile, new object[0]);
                            continue;
                        }
                        case MigraDoc.DocumentObjectModel.IO.Symbol.Null:
                        {
                            this.AssertCondition(cond, DomMsgID.MissingComma, new object[0]);
                            series.AddBlank();
                            cond = false;
                            this.ReadCode();
                            continue;
                        }
                    }
                    this.AssertCondition(cond, DomMsgID.MissingComma, new object[0]);
                    series.Add(this.scanner.GetTokenValueAsReal());
                    cond = false;
                    this.ReadCode();
                }
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight, DomMsgID.MissingBraceRight, new object[] { this.GetSymbolText(MigraDoc.DocumentObjectModel.IO.Symbol.Series) });
                this.ReadCode();
            }
            catch (DdlParserException exception)
            {
                this.ReportParserException(exception);
                this.AdjustToNextBlock();
            }
        }

        private void ParseSeries(XSeries series)
        {
            try
            {
                this.ReadCode();
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft, DomMsgID.MissingBraceLeft, new object[] { this.GetSymbolText(MigraDoc.DocumentObjectModel.IO.Symbol.Series) });
                bool cond = true;
                bool flag2 = true;
                while (flag2)
                {
                    this.ReadCode();
                    switch (this.Symbol)
                    {
                        case MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight:
                        {
                            flag2 = false;
                            continue;
                        }
                        case MigraDoc.DocumentObjectModel.IO.Symbol.Comma:
                        {
                            cond = true;
                            continue;
                        }
                        case MigraDoc.DocumentObjectModel.IO.Symbol.Null:
                        {
                            this.AssertCondition(cond, DomMsgID.MissingComma, new object[0]);
                            series.AddBlank();
                            cond = false;
                            continue;
                        }
                        case MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral:
                        case MigraDoc.DocumentObjectModel.IO.Symbol.HexIntegerLiteral:
                        case MigraDoc.DocumentObjectModel.IO.Symbol.StringLiteral:
                        case MigraDoc.DocumentObjectModel.IO.Symbol.RealLiteral:
                        {
                            this.AssertCondition(cond, DomMsgID.MissingComma, new object[0]);
                            series.Add(this.Token);
                            cond = false;
                            continue;
                        }
                        case MigraDoc.DocumentObjectModel.IO.Symbol.Eof:
                        {
                            this.ThrowParserException(DomMsgID.UnexpectedEndOfFile, new object[0]);
                            continue;
                        }
                    }
                    this.ThrowParserException(DomMsgID.UnexpectedSymbol, new object[] { this.Token });
                }
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight, DomMsgID.MissingBraceRight, new object[] { this.GetSymbolText(MigraDoc.DocumentObjectModel.IO.Symbol.Series) });
                this.ReadCode();
            }
            catch (DdlParserException exception)
            {
                this.ReportParserException(exception);
                this.AdjustToNextBlock();
            }
        }

        private void ParseSpace(ParagraphElements elements, int nestingLevel)
        {
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Space);
            Character character = elements.AddSpace(1);
            if (this.scanner.PeekSymbol() == MigraDoc.DocumentObjectModel.IO.Symbol.ParenLeft)
            {
                this.ReadCode();
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenLeft);
                this.ReadCode();
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Identifier)
                {
                    string token = this.Token;
                    if (!this.IsSpaceType(token))
                    {
                        this.ThrowParserException(DomMsgID.InvalidEnum, new object[] { token });
                    }
                    character.SymbolName = (SymbolName) Enum.Parse(typeof(SymbolName), token, true);
                    this.ReadCode();
                    if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Comma)
                    {
                        this.ReadCode();
                        this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral);
                        character.Count = this.scanner.GetTokenValueAsInt();
                        this.ReadCode();
                    }
                }
                else if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral)
                {
                    character.Count = this.scanner.GetTokenValueAsInt();
                    this.ReadCode();
                }
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenRight);
            }
        }

        private void ParseStringAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            this.AssertCondition(this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.StringLiteral, DomMsgID.StringExpected, new object[] { this.scanner.Token });
            vd.SetValue(dom, this.Token);
            this.ReadCode();
        }

        private Style ParseStyleDefinition(Styles styles)
        {
            Style element = null;
            try
            {
                string token = this.scanner.Token;
                string styleName = null;
                if ((this.Symbol != MigraDoc.DocumentObjectModel.IO.Symbol.Identifier) && (this.Symbol != MigraDoc.DocumentObjectModel.IO.Symbol.StringLiteral))
                {
                    this.ThrowParserException(DomMsgID.StyleNameExpected, new object[] { token });
                }
                this.ReadCode();
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Colon)
                {
                    this.ReadCode();
                    if ((this.Symbol != MigraDoc.DocumentObjectModel.IO.Symbol.Identifier) && (this.Symbol != MigraDoc.DocumentObjectModel.IO.Symbol.StringLiteral))
                    {
                        this.ThrowParserException(DomMsgID.StyleNameExpected, new object[] { token });
                    }
                    styleName = this.scanner.Token;
                    if (styles.GetIndex(styleName) == -1)
                    {
                        this.ReportParserInfo(DdlErrorLevel.Warning, DomMsgID.UseOfUndefinedBaseStyle, new string[] { styleName });
                        styleName = "InvalidStyleName";
                    }
                    this.ReadCode();
                }
                element = styles[token];
                if (element != null)
                {
                    if (styleName != null)
                    {
                        element.BaseStyle = styleName;
                    }
                }
                else
                {
                    switch (styleName)
                    {
                        case null:
                        case "":
                            styleName = "InvalidStyleName";
                            this.ReportParserInfo(DdlErrorLevel.Warning, DomMsgID.UseOfUndefinedStyle, new string[] { token });
                            break;
                    }
                    element = styles.AddStyle(token, styleName);
                }
                if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft)
                {
                    this.ParseAttributeBlock(element);
                }
            }
            catch (DdlParserException exception)
            {
                this.ReportParserException(exception);
                this.AdjustToNextBlock();
            }
            return element;
        }

        private Styles ParseStyles(Styles styles)
        {
            this.MoveToCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Styles);
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft);
            this.ReadCode();
            while ((this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Identifier) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.StringLiteral))
            {
                this.ParseStyleDefinition(styles);
            }
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
            this.ReadCode();
            return styles;
        }

        private void ParseSymbol(ParagraphElements elements)
        {
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Symbol);
            this.ReadCode();
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenLeft);
            char ch = '\0';
            SymbolName symbolType = (SymbolName) 0;
            int count = 1;
            this.ReadCode();
            if (this.TokenType == MigraDoc.DocumentObjectModel.IO.TokenType.Identifier)
            {
                try
                {
                    if (Enum.IsDefined(typeof(SymbolName), this.Token))
                    {
                        this.AssertCondition(this.IsSymbolType(this.Token), DomMsgID.InvalidSymbolType, new object[] { this.Token });
                        symbolType = (SymbolName) Enum.Parse(typeof(SymbolName), this.Token, true);
                    }
                }
                catch (Exception exception)
                {
                    this.ThrowParserException(exception, DomMsgID.InvalidEnum, new object[] { this.Token });
                }
            }
            else
            {
                this.ThrowParserException(DomMsgID.UnexpectedSymbol, new object[] { this.Token });
            }
            this.ReadCode();
            if (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.Comma)
            {
                this.ReadCode();
                if (this.TokenType == MigraDoc.DocumentObjectModel.IO.TokenType.IntegerLiteral)
                {
                    count = this.scanner.GetTokenValueAsInt();
                }
                this.ReadCode();
            }
            this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.ParenRight);
            if (symbolType != ((SymbolName) 0))
            {
                elements.AddCharacter(symbolType, count);
            }
            else
            {
                elements.AddCharacter(ch, count);
            }
        }

        private void ParseTable(DocumentElements elements, Table table)
        {
            Table element = table;
            try
            {
                if (element == null)
                {
                    element = elements.AddTable();
                }
                this.MoveToCode();
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Table);
                this.ReadCode();
                if (this.scanner.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
                {
                    this.ParseAttributes(element);
                }
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft);
                this.ReadCode();
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Columns);
                this.ParseColumns(element);
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.Rows);
                this.ParseRows(element);
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
                this.ReadCode();
            }
            catch (DdlParserException exception)
            {
                this.ReportParserException(exception);
                this.AdjustToNextBlock();
            }
        }

        private void ParseTextFrame(DocumentElements elements)
        {
            TextFrame element = elements.AddTextFrame();
            try
            {
                this.ReadCode();
                if (this.scanner.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.BracketLeft)
                {
                    this.ParseAttributes(element);
                }
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceLeft);
                if (this.IsParagraphContent())
                {
                    this.ParseParagraphContent(element.Elements, null);
                }
                else
                {
                    this.ReadCode();
                    this.ParseDocumentElements(element.Elements, MigraDoc.DocumentObjectModel.IO.Symbol.TextFrame);
                }
                this.AssertSymbol(MigraDoc.DocumentObjectModel.IO.Symbol.BraceRight);
                this.ReadCode();
            }
            catch (DdlParserException exception)
            {
                this.ReportParserException(exception);
                this.AdjustToNextBlock();
            }
        }

        private void ParseUnitAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            this.AssertCondition(((this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.RealLiteral) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.IntegerLiteral)) || (this.Symbol == MigraDoc.DocumentObjectModel.IO.Symbol.StringLiteral), DomMsgID.RealExpected, new object[] { this.scanner.Token });
            Unit token = this.Token;
            dom.SetValue(vd.ValueName, token);
            this.ReadCode();
        }

        private void ParseValueAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            try
            {
                dom.SetValue(vd.ValueName, this.Token);
            }
            catch (Exception exception)
            {
                this.ThrowParserException(exception, DomMsgID.InvalidEnum, new object[] { this.scanner.Token, vd.ValueName });
            }
            this.ReadCode();
        }

        private void ParseValueTypeAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            object val = vd.GetValue(dom, GV.ReadWrite);
            try
            {
                ((INullableValue) val).SetValue(this.Token);
                dom.SetValue(vd.ValueName, val);
                this.ReadCode();
            }
            catch (Exception exception)
            {
                this.ReportParserException(exception, DomMsgID.InvalidAssignment, new string[] { vd.ValueName });
            }
        }

        private MigraDoc.DocumentObjectModel.IO.Symbol ReadCode() => 
            this.scanner.ReadCode();

        private MigraDoc.DocumentObjectModel.IO.Symbol ReadText(bool rootLevel) => 
            this.scanner.ReadText(rootLevel);

        private void RemoveTrailingBlank(ParagraphElements elements)
        {
            DocumentObject lastObject = elements.LastObject;
            if (lastObject is Text)
            {
                Text text = (Text) lastObject;
                if (text.Content.EndsWith(" "))
                {
                    text.Content = text.Content.Remove(text.Content.Length - 1, 1);
                }
            }
        }

        private void ReportParserException(DdlParserException ex)
        {
            this.errors.AddError(ex.Error);
        }

        private void ReportParserException(DomMsgID error, params string[] parms)
        {
            this.ReportParserException(null, error, parms);
        }

        private void ReportParserException(Exception innerException, DomMsgID errorCode, params string[] parms)
        {
            string errorMessage = "";
            if (innerException != null)
            {
                errorMessage = ": " + innerException.ToString();
            }
            errorMessage = errorMessage + DomSR.FormatMessage(errorCode, parms);
            DdlReaderError error = new DdlReaderError(DdlErrorLevel.Error, errorMessage, (int) errorCode, this.scanner.DocumentFileName, this.scanner.CurrentLine, this.scanner.CurrentLinePos);
            this.errors.AddError(error);
        }

        private void ReportParserInfo(DdlErrorLevel level, DomMsgID errorCode, params string[] parms)
        {
            string errorMessage = DomSR.FormatMessage(errorCode, parms);
            DdlReaderError error = new DdlReaderError(level, errorMessage, (int) errorCode, this.scanner.DocumentFileName, this.scanner.CurrentLine, this.scanner.CurrentLinePos);
            this.errors.AddError(error);
        }

        private void ThrowParserException(DomMsgID errorCode, params object[] parms)
        {
            string errorMessage = DomSR.FormatMessage(errorCode, parms);
            DdlReaderError error = new DdlReaderError(DdlErrorLevel.Error, errorMessage, (int) errorCode, this.scanner.DocumentFileName, this.scanner.CurrentLine, this.scanner.CurrentLinePos);
            throw new DdlParserException(error);
        }

        private void ThrowParserException(Exception innerException, DomMsgID errorCode, params object[] parms)
        {
            throw new DdlParserException(DomSR.FormatMessage(errorCode, parms), innerException);
        }

        private MigraDoc.DocumentObjectModel.IO.Symbol Symbol =>
            this.scanner.Symbol;

        private string Token =>
            this.scanner.Token;

        private MigraDoc.DocumentObjectModel.IO.TokenType TokenType =>
            this.scanner.TokenType;
    }
}

