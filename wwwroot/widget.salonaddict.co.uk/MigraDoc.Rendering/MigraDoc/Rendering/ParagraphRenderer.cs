namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Fields;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.Rendering.Resources;
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class ParagraphRenderer : Renderer
    {
        private int currentBlankCount;
        private Hyperlink currentHyperlink;
        private ParagraphIterator currentLeaf;
        private XUnit currentLineWidth;
        private XPen currentUnderlinePen;
        private VerticalLineInfo currentVerticalInfo;
        private XUnit currentWordsWidth;
        private XUnit currentXPosition;
        private XUnit currentYPosition;
        private ParagraphIterator endLeaf;
        private Area formattingArea;
        private XRect hyperlinkRect;
        private Hashtable imageRenderInfos;
        private bool isFirstLine;
        private bool isLastLine;
        private DocumentObject lastTab;
        private bool lastTabPassed;
        private XUnit lastTabPosition;
        private XUnit minWidth;
        private Paragraph paragraph;
        private Phase phase;
        private bool reMeasureLine;
        private XUnit savedBlankWidth;
        private XUnit savedWordWidth;
        private ParagraphIterator startLeaf;
        private static XStringFormat stringFormat;
        private int tabIdx;
        private ArrayList tabOffsets;
        private XUnit underlineStartPos;

        internal ParagraphRenderer(XGraphics gfx, Paragraph paragraph, FieldInfos fieldInfos) : base(gfx, paragraph, fieldInfos)
        {
            this.savedBlankWidth = 0;
            this.savedWordWidth = 0;
            this.currentVerticalInfo = new VerticalLineInfo();
            this.minWidth = 0;
            this.paragraph = paragraph;
            ParagraphRenderInfo info = new ParagraphRenderInfo {
                paragraph = this.paragraph
            };
            ((ParagraphFormatInfo) info.FormatInfo).widowControl = this.paragraph.Format.WidowControl;
            base.renderInfo = info;
        }

        internal ParagraphRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos) : base(gfx, renderInfo, fieldInfos)
        {
            this.savedBlankWidth = 0;
            this.savedWordWidth = 0;
            this.currentVerticalInfo = new VerticalLineInfo();
            this.minWidth = 0;
            this.paragraph = (Paragraph) renderInfo.DocumentObject;
        }

        private VerticalLineInfo CalcCurrentVerticalInfo() => 
            this.CalcVerticalInfo(this.CurrentFont);

        private RenderInfo CalcImageRenderInfo(Image image)
        {
            Renderer renderer = Renderer.Create(base.gfx, base.documentRenderer, image, base.fieldInfos);
            renderer.Format(new Rectangle(0, 0, 1.7976931348623157E+308, 1.7976931348623157E+308), null);
            return renderer.RenderInfo;
        }

        private VerticalLineInfo CalcVerticalInfo(XFont font)
        {
            LineSpacingRule lineSpacingRule = this.paragraph.Format.LineSpacingRule;
            XUnit height = 0;
            XUnit descent = FontHandler.GetDescent(font);
            descent = Math.Max((double) this.currentVerticalInfo.descent, (double) descent);
            XUnit unit3 = font.GetHeight();
            RenderInfo currentImageRenderInfo = this.CurrentImageRenderInfo;
            if (currentImageRenderInfo != null)
            {
                unit3 = (((double) unit3) - ((double) FontHandler.GetAscent(font))) + ((double) currentImageRenderInfo.LayoutInfo.ContentArea.Height);
            }
            XUnit inherentlineSpace = Math.Max((double) this.currentVerticalInfo.inherentlineSpace, (double) unit3);
            switch (lineSpacingRule)
            {
                case LineSpacingRule.Single:
                    height = unit3;
                    break;

                case LineSpacingRule.OnePtFive:
                    height = 1.5 * ((double) unit3);
                    break;

                case LineSpacingRule.Double:
                    height = 2.0 * ((double) unit3);
                    break;

                case LineSpacingRule.AtLeast:
                    height = Math.Max((double) unit3, (double) this.paragraph.Format.LineSpacing);
                    break;

                case LineSpacingRule.Exactly:
                    height = new XUnit((double) this.paragraph.Format.LineSpacing);
                    inherentlineSpace = this.paragraph.Format.LineSpacing.Point;
                    break;

                case LineSpacingRule.Multiple:
                    height = ((double) this.paragraph.Format.LineSpacing) * ((double) unit3);
                    break;
            }
            height = Math.Max((double) this.currentVerticalInfo.height, (double) height);
            if (((double) base.MaxElementHeight) > 0.0)
            {
                height = Math.Min(((double) base.MaxElementHeight) - ((double) Renderer.Tolerance), (double) height);
            }
            return new VerticalLineInfo(height, descent, inherentlineSpace);
        }

        private void EndHyperlink(Hyperlink hyperlink, XUnit right, XUnit bottom)
        {
            this.hyperlinkRect.Width = ((double) right) - this.hyperlinkRect.X;
            this.hyperlinkRect.Height = ((double) bottom) - this.hyperlinkRect.Y;
            PdfPage pdfPage = base.gfx.PdfPage;
            if (pdfPage != null)
            {
                XRect rect = base.gfx.Transformer.WorldToDefaultPage(this.hyperlinkRect);
                switch (hyperlink.Type)
                {
                    case HyperlinkType.Local:
                    {
                        int physicalPageNumber = base.fieldInfos.GetPhysicalPageNumber(hyperlink.Name);
                        if (physicalPageNumber > 0)
                        {
                            pdfPage.AddDocumentLink(new PdfRectangle(rect), physicalPageNumber);
                        }
                        break;
                    }
                    case HyperlinkType.Web:
                        pdfPage.AddWebLink(new PdfRectangle(rect), hyperlink.Name);
                        break;

                    case HyperlinkType.File:
                        pdfPage.AddFileLink(new PdfRectangle(rect), hyperlink.Name);
                        break;
                }
                this.hyperlinkRect = new XRect();
            }
        }

        private void EndUnderline(XPen pen, XUnit xPosition)
        {
            XUnit unit = ((double) this.CurrentBaselinePosition) + (0.33 * ((double) this.currentVerticalInfo.descent));
            base.gfx.DrawLine(pen, (double) this.underlineStartPos, (double) unit, (double) xPosition, (double) unit);
        }

        private void FinishLayoutInfo()
        {
            LayoutInfo layoutInfo = base.renderInfo.LayoutInfo;
            ParagraphFormat format = this.paragraph.Format;
            ParagraphFormatInfo formatInfo = (ParagraphFormatInfo) base.renderInfo.FormatInfo;
            layoutInfo.MinWidth = this.minWidth;
            layoutInfo.KeepTogether = format.KeepTogether;
            if (formatInfo.IsComplete)
            {
                int num = 1;
                if (formatInfo.widowControl)
                {
                    num = 3;
                }
                if (formatInfo.LineCount <= num)
                {
                    layoutInfo.KeepTogether = true;
                }
            }
            if (formatInfo.IsStarting)
            {
                layoutInfo.MarginTop = format.SpaceBefore.Point;
                layoutInfo.PageBreakBefore = format.PageBreakBefore;
            }
            else
            {
                layoutInfo.MarginTop = 0;
                layoutInfo.PageBreakBefore = false;
            }
            if (formatInfo.IsEnding)
            {
                layoutInfo.MarginBottom = this.paragraph.Format.SpaceAfter.Point;
                layoutInfo.KeepWithNext = this.paragraph.Format.KeepWithNext;
            }
            else
            {
                layoutInfo.MarginBottom = 0;
                layoutInfo.KeepWithNext = false;
            }
            if (formatInfo.LineCount > 0)
            {
                XUnit height = formatInfo.GetFirstLineInfo().vertical.height;
                if ((formatInfo.isStarting && this.paragraph.Format.WidowControl) && (formatInfo.LineCount >= 2))
                {
                    height = ((double) height) + ((double) formatInfo.GetLineInfo(1).vertical.height);
                }
                layoutInfo.StartingHeight = height;
                XUnit unit2 = formatInfo.GetLastLineInfo().vertical.height;
                if ((formatInfo.IsEnding && this.paragraph.Format.WidowControl) && (formatInfo.LineCount >= 2))
                {
                    unit2 = ((double) unit2) + ((double) formatInfo.GetLineInfo(formatInfo.LineCount - 2).vertical.height);
                }
                layoutInfo.TrailingHeight = unit2;
            }
        }

        internal override void Format(Area area, FormatInfo previousFormatInfo)
        {
            ParagraphFormatInfo formatInfo = (ParagraphFormatInfo) base.renderInfo.FormatInfo;
            if (!this.InitFormat(area, previousFormatInfo))
            {
                formatInfo.isStarting = false;
            }
            else
            {
                formatInfo.isEnding = true;
                FormatResult result = FormatResult.Continue;
                while (this.currentLeaf != null)
                {
                    FormatResult newArea = this.FormatElement(this.currentLeaf.Current);
                    switch (newArea)
                    {
                        case FormatResult.Ignore:
                            this.currentLeaf = this.currentLeaf.GetNextLeaf();
                            break;

                        case FormatResult.Continue:
                            result = newArea;
                            this.currentLeaf = this.currentLeaf.GetNextLeaf();
                            break;

                        case FormatResult.NewLine:
                            result = newArea;
                            this.StoreLineInformation();
                            if (!this.StartNewLine())
                            {
                                newArea = FormatResult.NewArea;
                                formatInfo.isEnding = false;
                            }
                            break;
                    }
                    if (newArea == FormatResult.NewArea)
                    {
                        result = newArea;
                        formatInfo.isEnding = false;
                        break;
                    }
                }
                if (formatInfo.IsEnding && (result != FormatResult.NewLine))
                {
                    this.StoreLineInformation();
                }
                formatInfo.imageRenderInfos = this.imageRenderInfos;
                this.FinishLayoutInfo();
            }
        }

        private FormatResult FormatAsWord(XUnit width)
        {
            VerticalLineInfo info = this.CalcCurrentVerticalInfo();
            Rectangle fittingRect = this.formattingArea.GetFittingRect(this.currentYPosition, ((double) info.height) + ((double) this.BottomBorderOffset));
            if (fittingRect == null)
            {
                return FormatResult.NewArea;
            }
            if ((((double) this.currentXPosition) + ((double) width)) <= (((((double) fittingRect.X) + ((double) fittingRect.Width)) - ((double) this.RightIndent)) + ((double) Renderer.Tolerance)))
            {
                this.savedWordWidth = width;
                this.currentXPosition = ((double) this.currentXPosition) + ((double) width);
                if (!this.IgnoreHorizontalGrowth)
                {
                    this.currentWordsWidth = ((double) this.currentWordsWidth) + ((double) width);
                }
                if ((((double) this.savedBlankWidth) > 0.0) && !this.IgnoreHorizontalGrowth)
                {
                    this.currentBlankCount++;
                }
                if (!this.IgnoreHorizontalGrowth)
                {
                    this.currentLineWidth = ((double) this.currentLineWidth) + (((double) width) + ((double) this.PopSavedBlankWidth()));
                }
                this.currentVerticalInfo = info;
                this.minWidth = Math.Max((double) this.minWidth, (double) width);
                return FormatResult.Continue;
            }
            this.savedWordWidth = width;
            return FormatResult.NewLine;
        }

        private FormatResult FormatBlank()
        {
            if (this.IgnoreBlank())
            {
                return FormatResult.Ignore;
            }
            this.savedWordWidth = 0;
            XUnit blankWidth = this.MeasureString(" ");
            VerticalLineInfo info = this.CalcCurrentVerticalInfo();
            Rectangle fittingRect = this.formattingArea.GetFittingRect(this.currentYPosition, ((double) info.height) + ((double) this.BottomBorderOffset));
            if (fittingRect == null)
            {
                return FormatResult.NewArea;
            }
            if ((((double) blankWidth) + ((double) this.currentXPosition)) <= ((((double) fittingRect.X) + ((double) fittingRect.Width)) + ((double) Renderer.Tolerance)))
            {
                this.currentXPosition = ((double) this.currentXPosition) + ((double) blankWidth);
                this.currentVerticalInfo = info;
                this.SaveBlankWidth(blankWidth);
                return FormatResult.Continue;
            }
            return FormatResult.NewLine;
        }

        private FormatResult FormatBookmarkField(BookmarkField bookmarkField)
        {
            base.fieldInfos.AddBookmark(bookmarkField.Name);
            return FormatResult.Ignore;
        }

        private FormatResult FormatCharacter(Character character)
        {
            switch (character.SymbolName)
            {
                case ((SymbolName) (-251658239)):
                case ((SymbolName) (-251658238)):
                case ((SymbolName) (-251658237)):
                case ((SymbolName) (-251658236)):
                    return this.FormatSpace(character);

                case ((SymbolName) (-234881023)):
                    return this.FormatTab();

                case ((SymbolName) (-201326591)):
                    return this.FormatLineBreak();
            }
            return this.FormatSymbol(character);
        }

        private FormatResult FormatDateField(DateField dateField)
        {
            this.reMeasureLine = true;
            string word = DateTime.Now.ToString(dateField.Format);
            return this.FormatWord(word);
        }

        private FormatResult FormatElement(DocumentObject docObj)
        {
            switch (docObj.GetType().Name)
            {
                case "Text":
                    if (!this.IsBlank(docObj))
                    {
                        if (this.IsSoftHyphen(docObj))
                        {
                            return this.FormatSoftHyphen();
                        }
                        return this.FormatText((Text) docObj);
                    }
                    return this.FormatBlank();

                case "Character":
                    return this.FormatCharacter((Character) docObj);

                case "DateField":
                    return this.FormatDateField((DateField) docObj);

                case "InfoField":
                    return this.FormatInfoField((InfoField) docObj);

                case "NumPagesField":
                    return this.FormatNumPagesField((NumPagesField) docObj);

                case "PageField":
                    return this.FormatPageField((PageField) docObj);

                case "SectionField":
                    return this.FormatSectionField((SectionField) docObj);

                case "SectionPagesField":
                    return this.FormatSectionPagesField((SectionPagesField) docObj);

                case "BookmarkField":
                    return this.FormatBookmarkField((BookmarkField) docObj);

                case "PageRefField":
                    return this.FormatPageRefField((PageRefField) docObj);

                case "Image":
                    return this.FormatImage((Image) docObj);
            }
            return FormatResult.Continue;
        }

        private FormatResult FormatImage(Image image)
        {
            XUnit width = this.CurrentImageRenderInfo.LayoutInfo.ContentArea.Width;
            return this.FormatAsWord(width);
        }

        private FormatResult FormatInfoField(InfoField infoField)
        {
            string documentInfo = this.GetDocumentInfo(infoField.Name);
            if (documentInfo != "")
            {
                return this.FormatWord(documentInfo);
            }
            return FormatResult.Continue;
        }

        private FormatResult FormatLineBreak()
        {
            if (this.phase != Phase.Rendering)
            {
                this.currentLeaf = this.currentLeaf.GetNextLeaf();
            }
            this.savedWordWidth = 0;
            return FormatResult.NewLine;
        }

        private void FormatListSymbol()
        {
            string str;
            XFont font;
            if (this.GetListSymbol(out str, out font))
            {
                this.currentVerticalInfo = this.CalcVerticalInfo(font);
                this.currentXPosition = ((double) this.currentXPosition) + base.gfx.MeasureString(str, font, StringFormat).Width;
                this.FormatTab();
            }
        }

        private FormatResult FormatNumPagesField(NumPagesField numPagesField)
        {
            this.reMeasureLine = true;
            string fieldValue = this.GetFieldValue(numPagesField);
            return this.FormatWord(fieldValue);
        }

        private FormatResult FormatPageField(PageField pageField)
        {
            this.reMeasureLine = true;
            string fieldValue = this.GetFieldValue(pageField);
            return this.FormatWord(fieldValue);
        }

        private FormatResult FormatPageRefField(PageRefField pageRefField)
        {
            this.reMeasureLine = true;
            string fieldValue = this.GetFieldValue(pageRefField);
            return this.FormatWord(fieldValue);
        }

        private FormatResult FormatSectionField(SectionField sectionField)
        {
            this.reMeasureLine = true;
            string fieldValue = this.GetFieldValue(sectionField);
            return this.FormatWord(fieldValue);
        }

        private FormatResult FormatSectionPagesField(SectionPagesField sectionPagesField)
        {
            this.reMeasureLine = true;
            string fieldValue = this.GetFieldValue(sectionPagesField);
            return this.FormatWord(fieldValue);
        }

        private FormatResult FormatSoftHyphen()
        {
            ParagraphIterator iterator3;
            int num;
            XUnit unit;
            XUnit unit2;
            XUnit unit3;
            XUnit unit4;
            if (this.currentLeaf.Current == this.startLeaf.Current)
            {
                return FormatResult.Continue;
            }
            ParagraphIterator nextLeaf = this.currentLeaf.GetNextLeaf();
            ParagraphIterator previousLeaf = this.currentLeaf.GetPreviousLeaf();
            if (!this.IsWordLikeElement(previousLeaf.Current) || !this.IsWordLikeElement(nextLeaf.Current))
            {
                return FormatResult.Continue;
            }
            this.SaveBeforeProbing(out iterator3, out num, out unit3, out unit, out unit2, out unit4);
            this.currentLeaf = nextLeaf;
            FormatResult result = this.FormatElement(nextLeaf.Current);
            this.RestoreAfterProbing(iterator3, num, unit3, unit, unit2, unit4);
            if (result == FormatResult.Continue)
            {
                return FormatResult.Continue;
            }
            this.RestoreAfterProbing(iterator3, num, unit3, unit, unit2, unit4);
            Rectangle fittingRect = this.formattingArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height);
            XUnit unit5 = this.MeasureString("-");
            if (((((double) unit) + ((double) unit5)) <= ((((double) fittingRect.X) + ((double) fittingRect.Width)) + ((double) Renderer.Tolerance))) || (previousLeaf.Current == this.startLeaf.Current))
            {
                if (!this.IgnoreHorizontalGrowth)
                {
                    this.currentWordsWidth = ((double) this.currentWordsWidth) + ((double) unit5);
                    this.currentLineWidth = ((double) this.currentLineWidth) + ((double) unit5);
                }
                this.currentLeaf = nextLeaf;
                return FormatResult.NewLine;
            }
            this.currentWordsWidth = ((double) this.currentWordsWidth) - ((double) this.savedWordWidth);
            this.currentLineWidth = ((double) this.currentLineWidth) - ((double) this.savedWordWidth);
            this.currentLineWidth = ((double) this.currentLineWidth) - ((double) this.GetPreviousBlankWidth(previousLeaf));
            this.currentLeaf = previousLeaf;
            return FormatResult.NewLine;
        }

        private FormatResult FormatSpace(Character character)
        {
            XUnit spaceWidth = this.GetSpaceWidth(character);
            return this.FormatAsWord(spaceWidth);
        }

        private FormatResult FormatSymbol(Character character) => 
            this.FormatWord(GetSymbol(character));

        private FormatResult FormatTab()
        {
            if (this.paragraph.Format.Alignment == ParagraphAlignment.Justify)
            {
                this.reMeasureLine = true;
            }
            TabStop nextTabStop = this.GetNextTabStop();
            this.savedWordWidth = 0;
            if (nextTabStop != null)
            {
                bool notFitting = false;
                XUnit currentXPosition = this.currentXPosition;
                switch (nextTabStop.Alignment)
                {
                    case TabAlignment.Left:
                        this.currentXPosition = this.ProbeAfterLeftAlignedTab(nextTabStop.Position.Point, out notFitting);
                        break;

                    case TabAlignment.Center:
                        this.currentXPosition = this.ProbeAfterCenterAlignedTab(nextTabStop.Position.Point, out notFitting);
                        break;

                    case TabAlignment.Right:
                        this.currentXPosition = this.ProbeAfterRightAlignedTab(nextTabStop.Position.Point, out notFitting);
                        break;

                    case TabAlignment.Decimal:
                        this.currentXPosition = this.ProbeAfterDecimalAlignedTab(nextTabStop.Position.Point, out notFitting);
                        break;
                }
                if (!notFitting)
                {
                    if (!this.IgnoreHorizontalGrowth)
                    {
                        this.currentLineWidth = ((double) this.currentLineWidth) + (((double) this.currentXPosition) - ((double) currentXPosition));
                    }
                    this.tabOffsets.Add(new TabOffset(nextTabStop.Leader, ((double) this.currentXPosition) - ((double) currentXPosition)));
                    if (this.currentLeaf != null)
                    {
                        this.lastTab = this.currentLeaf.Current;
                    }
                }
                if (!notFitting)
                {
                    return FormatResult.Continue;
                }
            }
            return FormatResult.NewLine;
        }

        private FormatResult FormatText(Text text) => 
            this.FormatWord(text.Content);

        private FormatResult FormatWord(string word)
        {
            XUnit width = this.MeasureString(word);
            return this.FormatAsWord(width);
        }

        private string GetDocumentInfo(string name)
        {
            foreach (string str2 in Enum.GetNames(typeof(InfoFieldType)))
            {
                if (string.Compare(name, str2, true) == 0)
                {
                    return this.paragraph.Document.Info.GetValue(str2).ToString();
                }
            }
            return "";
        }

        private string GetFieldValue(DocumentObject field)
        {
            if (field is NumericFieldBase)
            {
                int number = -1;
                if (field is PageRefField)
                {
                    PageRefField field2 = (PageRefField) field;
                    number = base.fieldInfos.GetShownPageNumber(field2.Name);
                    if (number <= 0)
                    {
                        if (this.phase == Phase.Formatting)
                        {
                            return "XX";
                        }
                        return Messages.BookmarkNotDefined(field2.Name);
                    }
                }
                else if (field is SectionField)
                {
                    number = base.fieldInfos.section;
                    if (number <= 0)
                    {
                        return "XX";
                    }
                }
                else if (field is PageField)
                {
                    number = base.fieldInfos.displayPageNr;
                    if (number <= 0)
                    {
                        return "XX";
                    }
                }
                else if (field is NumPagesField)
                {
                    number = base.fieldInfos.numPages;
                    if (number <= 0)
                    {
                        return "XXX";
                    }
                }
                else if (field is SectionPagesField)
                {
                    number = base.fieldInfos.sectionPages;
                    if (number <= 0)
                    {
                        return "XX";
                    }
                }
                return NumberFormatter.Format(number, ((NumericFieldBase) field).Format);
            }
            if (field is DateField)
            {
                if (base.fieldInfos.date == DateTime.MinValue)
                {
                    DateTime now = DateTime.Now;
                }
                return base.fieldInfos.date.ToString(((DateField) field).Format);
            }
            if (field is InfoField)
            {
                return this.GetDocumentInfo(((InfoField) field).Name);
            }
            return "";
        }

        private Hyperlink GetHyperlink()
        {
            for (DocumentObject obj3 = DocumentRelations.GetParent(DocumentRelations.GetParent(this.currentLeaf.Current)); !(obj3 is Paragraph); obj3 = DocumentRelations.GetParent(DocumentRelations.GetParent(obj3)))
            {
                if (obj3 is Hyperlink)
                {
                    return (Hyperlink) obj3;
                }
            }
            return null;
        }

        internal static XUnit GetLineHeight(ParagraphFormat format, XGraphics gfx, DocumentRenderer renderer)
        {
            XFont font = FontHandler.FontToXFont(format.Font, renderer.PrivateFonts, gfx.MUH, gfx.MFEH);
            XUnit height = font.GetHeight();
            switch (format.LineSpacingRule)
            {
                case LineSpacingRule.Single:
                    return height;

                case LineSpacingRule.OnePtFive:
                    return (1.5 * ((double) height));

                case LineSpacingRule.Double:
                    return (2.0 * ((double) height));

                case LineSpacingRule.AtLeast:
                    return Math.Max(format.LineSpacing.Point, font.GetHeight(gfx));

                case LineSpacingRule.Exactly:
                    return format.LineSpacing.Point;

                case LineSpacingRule.Multiple:
                    return (double) (((float) format.LineSpacing) * ((float) format.Font.Size));
            }
            return height;
        }

        private bool GetListSymbol(out string symbol, out XFont font)
        {
            font = null;
            symbol = null;
            ParagraphFormatInfo formatInfo = (ParagraphFormatInfo) base.renderInfo.FormatInfo;
            if (this.phase != Phase.Formatting)
            {
                if ((formatInfo.listFont != null) && (formatInfo.listSymbol != null))
                {
                    font = formatInfo.listFont;
                    symbol = formatInfo.listSymbol;
                    return true;
                }
            }
            else
            {
                ParagraphFormat format = this.paragraph.Format;
                if (!format.IsNull("ListInfo"))
                {
                    ListInfo listInfo = format.ListInfo;
                    double size = (double) format.Font.Size;
                    XFontStyle xStyle = FontHandler.GetXStyle(format.Font);
                    switch (listInfo.ListType)
                    {
                        case ListType.BulletList1:
                            symbol = "\x00b7";
                            font = new XFont("Symbol", size, xStyle);
                            break;

                        case ListType.BulletList2:
                            symbol = "o";
                            font = new XFont("Courier New", size, xStyle);
                            break;

                        case ListType.BulletList3:
                            symbol = "\x00a7";
                            font = new XFont("Wingdings", size, xStyle);
                            break;

                        case ListType.NumberList1:
                            symbol = base.documentRenderer.NextListNumber(listInfo) + ".";
                            font = FontHandler.FontToXFont(format.Font, base.documentRenderer.PrivateFonts, base.gfx.MUH, base.gfx.MFEH);
                            break;

                        case ListType.NumberList2:
                            symbol = base.documentRenderer.NextListNumber(listInfo) + ")";
                            font = FontHandler.FontToXFont(format.Font, base.documentRenderer.PrivateFonts, base.gfx.MUH, base.gfx.MFEH);
                            break;

                        case ListType.NumberList3:
                            symbol = NumberFormatter.Format(base.documentRenderer.NextListNumber(listInfo), "alphabetic") + ")";
                            font = FontHandler.FontToXFont(format.Font, base.documentRenderer.PrivateFonts, base.gfx.MUH, base.gfx.MFEH);
                            break;
                    }
                    formatInfo.listFont = font;
                    formatInfo.listSymbol = symbol;
                    return true;
                }
            }
            return false;
        }

        private TabStop GetNextTabStop()
        {
            ParagraphFormat format = this.paragraph.Format;
            TabStops tabStops = format.TabStops;
            XUnit point = 0;
            foreach (TabStop stop in tabStops)
            {
                if (stop.Position.Point > ((((double) this.formattingArea.Width) - ((double) this.RightIndent)) + ((double) Renderer.Tolerance)))
                {
                    break;
                }
                if ((stop.Position.Point + ((double) this.formattingArea.X)) > (((double) this.currentXPosition) + ((double) Renderer.Tolerance)))
                {
                    return stop;
                }
                point = stop.Position.Point;
            }
            if ((((float) format.FirstLineIndent) < 0f) || (!format.IsNull("ListInfo") && (((float) format.ListInfo.NumberPosition) < ((float) format.LeftIndent))))
            {
                XUnit unit2 = format.LeftIndent.Point;
                if (this.isFirstLine && (((double) this.currentXPosition) < (((double) unit2) + ((double) this.formattingArea.X))))
                {
                    return new TabStop(unit2.Point);
                }
            }
            XUnit unit3 = "1.25cm";
            if (!this.paragraph.Document.IsNull("DefaultTabstop"))
            {
                unit3 = this.paragraph.Document.DefaultTabStop.Point;
            }
            for (XUnit unit4 = unit3; (((double) unit4) + ((double) this.formattingArea.X)) <= (((double) this.formattingArea.Width) - ((double) this.RightIndent)); unit4 = ((double) unit4) + ((double) unit3))
            {
                if ((((double) unit4) > ((double) point)) && ((((double) unit4) + ((double) this.formattingArea.X)) > (((double) this.currentXPosition) + ((double) Renderer.Tolerance))))
                {
                    return new TabStop(unit4.Point);
                }
            }
            return null;
        }

        private string GetOutlineTitle()
        {
            ParagraphIterator firstLeaf = new ParagraphIterator(this.paragraph.Elements);
            firstLeaf = firstLeaf.GetFirstLeaf();
            bool flag = true;
            string str = "";
            while (firstLeaf != null)
            {
                DocumentObject current = firstLeaf.Current;
                if (!flag && ((this.IsBlank(current) || this.IsTab(current)) || this.IsLineBreak(current)))
                {
                    str = str + " ";
                    flag = true;
                }
                else if (current is Text)
                {
                    str = str + ((Text) current).Content;
                    flag = false;
                }
                else if (this.IsRenderedField(current))
                {
                    str = str + this.GetFieldValue(current);
                    flag = false;
                }
                else if (this.IsSymbol(current))
                {
                    str = str + GetSymbol((Character) current);
                    flag = false;
                }
                if (str.Length > 0x40)
                {
                    return str;
                }
                firstLeaf = firstLeaf.GetNextLeaf();
            }
            return str;
        }

        private XUnit GetPreviousBlankWidth(ParagraphIterator beforeIter)
        {
            XUnit currentWordDistance = 0;
            ParagraphIterator currentLeaf = this.currentLeaf;
            this.currentLeaf = beforeIter.GetPreviousLeaf();
            while (this.currentLeaf != null)
            {
                if (this.currentLeaf.Current is BookmarkField)
                {
                    this.currentLeaf = this.currentLeaf.GetPreviousLeaf();
                }
                else
                {
                    if (this.IsBlank(this.currentLeaf.Current) && !this.IgnoreBlank())
                    {
                        currentWordDistance = this.CurrentWordDistance;
                    }
                    break;
                }
            }
            this.currentLeaf = currentLeaf;
            return currentWordDistance;
        }

        private Area GetShadingArea()
        {
            Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
            ParagraphFormat format = this.paragraph.Format;
            XUnit x = ((double) contentArea.X) + ((double) format.LeftIndent);
            if (((float) format.FirstLineIndent) < 0f)
            {
                x = ((double) x) + ((double) format.FirstLineIndent);
            }
            XUnit y = contentArea.Y;
            XUnit unit3 = ((double) contentArea.Y) + ((double) contentArea.Height);
            XUnit unit4 = ((double) contentArea.X) + ((double) contentArea.Width);
            unit4 = ((double) unit4) - ((double) format.RightIndent);
            if (!this.paragraph.Format.IsNull("Borders"))
            {
                Borders borders = format.Borders;
                BordersRenderer renderer = new BordersRenderer(borders, base.gfx);
                if (base.renderInfo.FormatInfo.IsStarting)
                {
                    y = ((double) y) + ((double) renderer.GetWidth(BorderType.Top));
                }
                if (base.renderInfo.FormatInfo.IsEnding)
                {
                    unit3 = ((double) unit3) - ((double) renderer.GetWidth(BorderType.Bottom));
                }
                x = ((double) x) - ((double) borders.DistanceFromLeft);
                unit4 = ((double) unit4) + ((double) borders.DistanceFromRight);
            }
            return new Rectangle(x, y, ((double) unit4) - ((double) x), ((double) unit3) - ((double) y));
        }

        private XUnit GetSpaceWidth(Character character)
        {
            XUnit unit = 0;
            switch (character.SymbolName)
            {
                case ((SymbolName) (-251658239)):
                    unit = this.MeasureString(" ");
                    break;

                case ((SymbolName) (-251658238)):
                    unit = this.MeasureString("n");
                    break;

                case ((SymbolName) (-251658237)):
                    unit = this.MeasureString("m");
                    break;

                case ((SymbolName) (-251658236)):
                    unit = 0.25 * ((double) this.MeasureString("m"));
                    break;
            }
            return (((double) unit) * character.Count);
        }

        private static string GetSymbol(Character character)
        {
            char ch;
            switch (character.SymbolName)
            {
                case ((SymbolName) (-134217727)):
                    ch = '€';
                    break;

                case ((SymbolName) (-134217726)):
                    ch = '\x00a9';
                    break;

                case ((SymbolName) (-134217725)):
                    ch = '™';
                    break;

                case ((SymbolName) (-134217724)):
                    ch = '\x00ae';
                    break;

                case ((SymbolName) (-134217723)):
                    ch = '•';
                    break;

                case ((SymbolName) (-134217722)):
                    ch = '\x00ac';
                    break;

                case ((SymbolName) (-134217721)):
                    ch = '—';
                    break;

                case ((SymbolName) (-134217720)):
                    ch = '–';
                    break;

                default:
                {
                    char ch2 = character.Char;
                    ch = Encoding.Default.GetChars(new byte[] { (byte) ch2 })[0];
                    break;
                }
            }
            string str = "";
            str = str + ch;
            int count = character.Count;
            while (--count > 0)
            {
                str = str + ch;
            }
            return str;
        }

        private XPen GetUnderlinePen(bool isWord)
        {
            Font currentDomFont = this.CurrentDomFont;
            Underline underline = currentDomFont.Underline;
            if (underline == Underline.None)
            {
                return null;
            }
            if ((underline == Underline.Words) && !isWord)
            {
                return null;
            }
            XPen pen = new XPen(ColorHelper.ToXColor(currentDomFont.Color, this.paragraph.Document.UseCmykColor), (double) (((float) currentDomFont.Size) / 16f));
            switch (currentDomFont.Underline)
            {
                case Underline.Dotted:
                    pen.DashStyle = XDashStyle.Dot;
                    return pen;

                case Underline.Dash:
                    pen.DashStyle = XDashStyle.Dash;
                    return pen;

                case Underline.DotDash:
                    pen.DashStyle = XDashStyle.DashDot;
                    return pen;

                case Underline.DotDotDash:
                    pen.DashStyle = XDashStyle.DashDotDot;
                    return pen;
            }
            pen.DashStyle = XDashStyle.Solid;
            return pen;
        }

        private void HandleNonFittingLine()
        {
            if (this.currentLeaf != null)
            {
                if (((double) this.savedWordWidth) > 0.0)
                {
                    this.currentWordsWidth = this.savedWordWidth;
                    this.currentLineWidth = this.savedWordWidth;
                }
                this.currentLeaf = this.currentLeaf.GetNextLeaf();
                this.currentYPosition = ((double) this.currentYPosition) + ((double) this.currentVerticalInfo.height);
                this.currentVerticalInfo = new VerticalLineInfo();
            }
        }

        private bool IgnoreBlank()
        {
            if (this.currentLeaf != this.startLeaf)
            {
                if ((this.endLeaf != null) && (this.currentLeaf.Current == this.endLeaf.Current))
                {
                    return true;
                }
                ParagraphIterator nextLeaf = this.currentLeaf.GetNextLeaf();
                while ((nextLeaf != null) && (this.IsBlank(nextLeaf.Current) || (nextLeaf.Current is BookmarkField)))
                {
                    nextLeaf = nextLeaf.GetNextLeaf();
                }
                if (nextLeaf == null)
                {
                    return true;
                }
                if (this.IsTab(nextLeaf.Current))
                {
                    return true;
                }
                ParagraphIterator previousLeaf = this.currentLeaf.GetPreviousLeaf();
                DocumentObject current = previousLeaf?.Current;
                while ((current != null) && (current is BookmarkField))
                {
                    previousLeaf = previousLeaf.GetPreviousLeaf();
                    if (previousLeaf != null)
                    {
                        current = previousLeaf.Current;
                    }
                    else
                    {
                        current = null;
                    }
                }
                if ((current != null) && !this.IsBlank(current))
                {
                    return this.IsTab(current);
                }
            }
            return true;
        }

        private bool InitFormat(Area area, FormatInfo previousFormatInfo)
        {
            this.phase = Phase.Formatting;
            this.tabOffsets = new ArrayList();
            ParagraphFormatInfo info = (ParagraphFormatInfo) previousFormatInfo;
            if ((previousFormatInfo == null) || (info.LineCount == 0))
            {
                ((ParagraphFormatInfo) base.renderInfo.FormatInfo).isStarting = true;
                this.currentLeaf = new ParagraphIterator(this.paragraph.Elements).GetFirstLeaf();
                this.isFirstLine = true;
            }
            else
            {
                this.currentLeaf = info.GetLastLineInfo().endIter.GetNextLeaf();
                this.isFirstLine = false;
                ((ParagraphFormatInfo) base.renderInfo.FormatInfo).isStarting = false;
            }
            this.startLeaf = this.currentLeaf;
            this.currentVerticalInfo = this.CalcCurrentVerticalInfo();
            this.currentYPosition = ((double) area.Y) + ((double) this.TopBorderOffset);
            this.formattingArea = area;
            Rectangle fittingRect = this.formattingArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height);
            if (fittingRect == null)
            {
                return false;
            }
            this.currentXPosition = ((double) fittingRect.X) + ((double) this.LeftIndent);
            if (this.isFirstLine)
            {
                this.FormatListSymbol();
            }
            return true;
        }

        private void InitRendering()
        {
            this.phase = Phase.Rendering;
            ParagraphFormatInfo formatInfo = (ParagraphFormatInfo) base.renderInfo.FormatInfo;
            if (formatInfo.LineCount != 0)
            {
                this.isFirstLine = formatInfo.IsStarting;
                LineInfo firstLineInfo = formatInfo.GetFirstLineInfo();
                Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
                this.currentYPosition = ((double) contentArea.Y) + ((double) this.TopBorderOffset);
                Rectangle fittingRect = contentArea.GetFittingRect(this.currentYPosition, firstLineInfo.vertical.height);
                if (fittingRect != null)
                {
                    this.currentXPosition = fittingRect.X;
                }
                this.currentLineWidth = 0;
            }
        }

        private bool IsBlank(DocumentObject docObj) => 
            ((docObj is Text) && (((Text) docObj).Content == " "));

        private bool IsLineBreak(DocumentObject docObj) => 
            ((docObj is Character) && (((Character) docObj).SymbolName == ((SymbolName) (-201326591))));

        private bool IsPlainText(DocumentObject docObj) => 
            ((docObj is Text) && (!this.IsSoftHyphen(docObj) && !this.IsBlank(docObj)));

        private bool IsRenderedField(DocumentObject docObj) => 
            ((docObj is NumericFieldBase) || ((docObj is DocumentInfo) || (docObj is DateField)));

        private bool IsSoftHyphen(DocumentObject docObj)
        {
            Text text = docObj as Text;
            return ((text != null) && (text.Content == "\x00ad"));
        }

        private bool IsSpaceCharacter(DocumentObject docObj)
        {
            if (docObj is Character)
            {
                switch (((Character) docObj).SymbolName)
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

        private bool IsSymbol(DocumentObject docObj) => 
            ((docObj is Character) && ((!this.IsSpaceCharacter(docObj) && !this.IsTab(docObj)) && !this.IsLineBreak(docObj)));

        private bool IsTab(DocumentObject docObj) => 
            ((docObj is Character) && (((Character) docObj).SymbolName == ((SymbolName) (-234881023))));

        private bool IsWordLikeElement(DocumentObject docObj) => 
            (this.IsPlainText(docObj) || (this.IsRenderedField(docObj) || this.IsSymbol(docObj)));

        private XUnit MeasureString(string word)
        {
            XFont currentFont = this.CurrentFont;
            XUnit width = base.gfx.MeasureString(word, currentFont, StringFormat).Width;
            Font currentDomFont = this.CurrentDomFont;
            if (!currentDomFont.Subscript && !currentDomFont.Superscript)
            {
                return width;
            }
            return (((double) width) * FontHandler.GetSubSuperScaling(currentFont));
        }

        private TabOffset NextTabOffset()
        {
            TabOffset offset = (this.tabOffsets.Count > this.tabIdx) ? ((TabOffset) this.tabOffsets[this.tabIdx]) : new TabOffset(TabLeader.Spaces, 0);
            this.tabIdx++;
            return offset;
        }

        private XUnit PopSavedBlankWidth()
        {
            XUnit savedBlankWidth = this.savedBlankWidth;
            this.savedBlankWidth = 0;
            return savedBlankWidth;
        }

        private XUnit ProbeAfterCenterAlignedTab(XUnit tabStopPosition, out bool notFitting)
        {
            ParagraphIterator iterator;
            int num;
            XUnit unit;
            XUnit unit2;
            XUnit unit3;
            XUnit unit4;
            this.SaveBeforeProbing(out iterator, out num, out unit3, out unit, out unit2, out unit4);
            XUnit unit5 = unit;
            notFitting = this.ProbeAfterTab();
            if (!notFitting && ((((double) unit) + (((double) this.currentLineWidth) / 2.0)) <= (((double) this.formattingArea.X) + ((double) tabStopPosition))))
            {
                Rectangle fittingRect = this.formattingArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height);
                if (((((double) this.formattingArea.X) + ((double) tabStopPosition)) + (((double) this.currentLineWidth) / 2.0)) > ((((double) fittingRect.X) + ((double) fittingRect.Width)) - ((double) this.RightIndent)))
                {
                    unit5 = ((((double) fittingRect.X) + ((double) fittingRect.Width)) - ((double) this.RightIndent)) - ((double) this.currentLineWidth);
                }
                else
                {
                    unit5 = (((double) this.formattingArea.X) + ((double) tabStopPosition)) - (((double) this.currentLineWidth) / 2.0);
                }
            }
            this.RestoreAfterProbing(iterator, num, unit3, unit, unit2, unit4);
            return unit5;
        }

        private XUnit ProbeAfterDecimalAlignedTab(XUnit tabStopPosition, out bool notFitting)
        {
            notFitting = false;
            ParagraphIterator currentLeaf = this.currentLeaf;
            if (this.IsTab(this.currentLeaf.Current))
            {
                this.currentLeaf = this.currentLeaf.GetNextLeaf();
            }
            if (this.currentLeaf == null)
            {
                this.currentLeaf = currentLeaf;
                return (((double) this.currentXPosition) + ((double) tabStopPosition));
            }
            VerticalLineInfo info = this.CalcCurrentVerticalInfo();
            if (this.formattingArea.GetFittingRect(this.currentYPosition, info.height) == null)
            {
                notFitting = true;
                this.currentLeaf = currentLeaf;
                return this.currentXPosition;
            }
            if (this.IsPlainText(this.currentLeaf.Current))
            {
                Text current = (Text) this.currentLeaf.Current;
                string content = current.Content;
                int length = current.Content.LastIndexOfAny(new char[] { ',', '.' });
                if (length > 0)
                {
                    content = content.Substring(0, length);
                }
                XUnit unit = this.MeasureString(content);
                notFitting = (((double) this.currentXPosition) + ((double) unit)) >= ((((double) this.formattingArea.X) + ((double) this.formattingArea.Width)) + ((double) Renderer.Tolerance));
                if (!notFitting)
                {
                    return ((((double) this.formattingArea.X) + ((double) tabStopPosition)) - ((double) unit));
                }
                return this.currentXPosition;
            }
            this.currentLeaf = currentLeaf;
            return this.ProbeAfterRightAlignedTab(tabStopPosition, out notFitting);
        }

        private XUnit ProbeAfterLeftAlignedTab(XUnit tabStopPosition, out bool notFitting)
        {
            ParagraphIterator iterator;
            int num;
            XUnit unit;
            XUnit unit2;
            XUnit unit3;
            XUnit unit4;
            this.SaveBeforeProbing(out iterator, out num, out unit3, out unit, out unit2, out unit4);
            XUnit unit5 = unit;
            this.currentXPosition = ((double) this.formattingArea.X) + tabStopPosition.Point;
            notFitting = this.ProbeAfterTab();
            if (!notFitting)
            {
                unit5 = ((double) this.formattingArea.X) + ((double) tabStopPosition);
            }
            this.RestoreAfterProbing(iterator, num, unit3, unit, unit2, unit4);
            return unit5;
        }

        private XUnit ProbeAfterRightAlignedTab(XUnit tabStopPosition, out bool notFitting)
        {
            ParagraphIterator iterator;
            int num;
            XUnit unit;
            XUnit unit2;
            XUnit unit3;
            XUnit unit4;
            this.SaveBeforeProbing(out iterator, out num, out unit3, out unit, out unit2, out unit4);
            XUnit unit5 = unit;
            notFitting = this.ProbeAfterTab();
            if (!notFitting && ((((double) unit) + ((double) this.currentLineWidth)) <= (((double) this.formattingArea.X) + ((double) tabStopPosition))))
            {
                unit5 = (((double) this.formattingArea.X) + ((double) tabStopPosition)) - ((double) this.currentLineWidth);
            }
            this.RestoreAfterProbing(iterator, num, unit3, unit, unit2, unit4);
            return unit5;
        }

        private bool ProbeAfterTab()
        {
            this.currentLineWidth = 0;
            this.currentBlankCount = 0;
            if ((this.currentLeaf != null) && this.IsTab(this.currentLeaf.Current))
            {
                this.currentLeaf = this.currentLeaf.GetNextLeaf();
            }
            bool flag = false;
            while (((this.currentLeaf != null) && !this.IsLineBreak(this.currentLeaf.Current)) && !this.IsTab(this.currentLeaf.Current))
            {
                if (this.FormatElement(this.currentLeaf.Current) != FormatResult.Continue)
                {
                    break;
                }
                flag = flag || this.IsWordLikeElement(this.currentLeaf.Current);
                this.currentLeaf = this.currentLeaf.GetNextLeaf();
            }
            return ((((this.currentLeaf != null) && !this.IsLineBreak(this.currentLeaf.Current)) && !this.IsTab(this.currentLeaf.Current)) && !flag);
        }

        private void RealizeHyperlink(XUnit width)
        {
            XUnit currentYPosition = this.currentYPosition;
            XUnit currentXPosition = this.currentXPosition;
            XUnit bottom = ((double) currentYPosition) + ((double) this.currentVerticalInfo.height);
            XUnit right = ((double) currentXPosition) + ((double) width);
            Hyperlink hyperlink = this.GetHyperlink();
            if (this.currentHyperlink != hyperlink)
            {
                if (this.currentHyperlink != null)
                {
                    this.EndHyperlink(this.currentHyperlink, currentXPosition, bottom);
                }
                if (hyperlink != null)
                {
                    this.StartHyperlink(currentXPosition, currentYPosition);
                }
                this.currentHyperlink = hyperlink;
            }
            if (this.currentLeaf.Current == this.endLeaf.Current)
            {
                if (this.currentHyperlink != null)
                {
                    this.EndHyperlink(this.currentHyperlink, right, bottom);
                }
                this.currentHyperlink = null;
            }
        }

        private void ReMeasureLine(ref LineInfo lineInfo)
        {
            ParagraphIterator iterator;
            int num;
            XUnit unit;
            XUnit unit2;
            XUnit unit3;
            XUnit unit4;
            this.SaveBeforeProbing(out iterator, out num, out unit3, out unit, out unit2, out unit4);
            bool lastTabPassed = this.lastTabPassed;
            this.currentLeaf = lineInfo.startIter;
            this.endLeaf = lineInfo.endIter;
            this.formattingArea = base.renderInfo.LayoutInfo.ContentArea;
            this.tabOffsets = new ArrayList();
            this.currentLineWidth = 0;
            this.currentWordsWidth = 0;
            Rectangle fittingRect = this.formattingArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height);
            if (fittingRect == null)
            {
                base.GetType();
            }
            if (fittingRect != null)
            {
                this.currentXPosition = ((double) fittingRect.X) + ((double) this.LeftIndent);
                this.FormatListSymbol();
                bool flag2 = true;
                while (flag2 && (this.currentLeaf != null))
                {
                    if (this.currentLeaf.Current == lineInfo.lastTab)
                    {
                        this.lastTabPassed = true;
                    }
                    this.FormatElement(this.currentLeaf.Current);
                    if ((this.currentLeaf != null) && (this.currentLeaf.Current != this.endLeaf.Current))
                    {
                        this.currentLeaf = this.currentLeaf.GetNextLeaf();
                    }
                }
                lineInfo.lineWidth = this.currentLineWidth;
                lineInfo.wordsWidth = this.currentWordsWidth;
                lineInfo.blankCount = this.currentBlankCount;
                lineInfo.tabOffsets = this.tabOffsets;
                lineInfo.reMeasureLine = false;
                this.lastTabPassed = lastTabPassed;
            }
            this.RestoreAfterProbing(iterator, num, unit3, unit, unit2, unit4);
        }

        internal override void Render()
        {
            this.InitRendering();
            if ((this.paragraph.Format.OutlineLevel >= OutlineLevel.Level1) && (base.gfx.PdfPage != null))
            {
                base.documentRenderer.AddOutline((int) this.paragraph.Format.OutlineLevel, this.GetOutlineTitle(), base.gfx.PdfPage);
            }
            this.RenderShading();
            this.RenderBorders();
            ParagraphFormatInfo formatInfo = (ParagraphFormatInfo) base.renderInfo.FormatInfo;
            for (int i = 0; i < formatInfo.LineCount; i++)
            {
                LineInfo lineInfo = formatInfo.GetLineInfo(i);
                this.isLastLine = i == (formatInfo.LineCount - 1);
                this.lastTabPosition = 0;
                if (lineInfo.reMeasureLine)
                {
                    this.ReMeasureLine(ref lineInfo);
                }
                this.RenderLine(lineInfo);
            }
        }

        private void RenderBlank()
        {
            if (!this.IgnoreBlank())
            {
                XUnit currentWordDistance = this.CurrentWordDistance;
                this.RenderUnderline(currentWordDistance, false);
                this.RealizeHyperlink(currentWordDistance);
                this.currentXPosition = ((double) this.currentXPosition) + ((double) currentWordDistance);
            }
            else
            {
                this.RenderUnderline(0, false);
                this.RealizeHyperlink(0);
            }
        }

        private void RenderBookmarkField()
        {
            this.RenderUnderline(0, false);
        }

        private void RenderBorders()
        {
            if (!this.paragraph.Format.IsNull("Borders"))
            {
                Area shadingArea = this.GetShadingArea();
                XUnit x = shadingArea.X;
                XUnit y = shadingArea.Y;
                XUnit top = ((double) shadingArea.Y) + ((double) shadingArea.Height);
                XUnit left = ((double) shadingArea.X) + ((double) shadingArea.Width);
                BordersRenderer renderer = new BordersRenderer(this.paragraph.Format.Borders, base.gfx);
                XUnit width = renderer.GetWidth(BorderType.Left);
                if (((double) width) > 0.0)
                {
                    x = ((double) x) - ((double) width);
                    renderer.RenderVertically(BorderType.Left, x, y, ((double) top) - ((double) y));
                }
                width = renderer.GetWidth(BorderType.Right);
                if (((double) width) > 0.0)
                {
                    renderer.RenderVertically(BorderType.Right, left, y, ((double) top) - ((double) y));
                    left = ((double) left) + ((double) width);
                }
                width = renderer.GetWidth(BorderType.Top);
                if (base.renderInfo.FormatInfo.IsStarting && (((double) width) > 0.0))
                {
                    y = ((double) y) - ((double) width);
                    renderer.RenderHorizontally(BorderType.Top, x, y, ((double) left) - ((double) x));
                }
                width = renderer.GetWidth(BorderType.Bottom);
                if (base.renderInfo.FormatInfo.IsEnding && (((double) width) > 0.0))
                {
                    renderer.RenderHorizontally(BorderType.Bottom, x, top, ((double) left) - ((double) x));
                }
            }
        }

        private void RenderCharacter(Character character)
        {
            switch (character.SymbolName)
            {
                case ((SymbolName) (-251658239)):
                case ((SymbolName) (-251658238)):
                case ((SymbolName) (-251658237)):
                case ((SymbolName) (-251658236)):
                    this.RenderSpace(character);
                    return;

                case ((SymbolName) (-234881023)):
                    this.RenderTab();
                    return;

                case ((SymbolName) (-201326591)):
                    this.RenderLinebreak();
                    return;
            }
            this.RenderSymbol(character);
        }

        private void RenderDateField(DateField dateField)
        {
            this.RenderWord(base.fieldInfos.date.ToString(dateField.Format));
        }

        private void RenderElement(DocumentObject docObj)
        {
            switch (docObj.GetType().Name)
            {
                case "Text":
                    if (!this.IsBlank(docObj))
                    {
                        if (this.IsSoftHyphen(docObj))
                        {
                            this.RenderSoftHyphen();
                            return;
                        }
                        this.RenderText((Text) docObj);
                        return;
                    }
                    this.RenderBlank();
                    return;

                case "Character":
                    this.RenderCharacter((Character) docObj);
                    return;

                case "DateField":
                    this.RenderDateField((DateField) docObj);
                    return;

                case "InfoField":
                    this.RenderInfoField((InfoField) docObj);
                    return;

                case "NumPagesField":
                    this.RenderNumPagesField((NumPagesField) docObj);
                    return;

                case "PageField":
                    this.RenderPageField((PageField) docObj);
                    return;

                case "SectionField":
                    this.RenderSectionField((SectionField) docObj);
                    return;

                case "SectionPagesField":
                    this.RenderSectionPagesField((SectionPagesField) docObj);
                    return;

                case "BookmarkField":
                    this.RenderBookmarkField();
                    return;

                case "PageRefField":
                    this.RenderPageRefField((PageRefField) docObj);
                    return;

                case "Image":
                    this.RenderImage((Image) docObj);
                    return;
            }
        }

        private void RenderImage(Image image)
        {
            RenderInfo currentImageRenderInfo = this.CurrentImageRenderInfo;
            XUnit currentBaselinePosition = this.CurrentBaselinePosition;
            Area contentArea = currentImageRenderInfo.LayoutInfo.ContentArea;
            currentBaselinePosition = ((double) currentBaselinePosition) - ((double) contentArea.Height);
            base.RenderByInfos(this.currentXPosition, currentBaselinePosition, new RenderInfo[] { currentImageRenderInfo });
            this.RenderUnderline(contentArea.Width, true);
            this.RealizeHyperlink(contentArea.Width);
            this.currentXPosition = ((double) this.currentXPosition) + ((double) contentArea.Width);
        }

        private void RenderInfoField(InfoField infoField)
        {
            this.RenderWord(this.GetDocumentInfo(infoField.Name));
        }

        private void RenderLine(LineInfo lineInfo)
        {
            this.currentVerticalInfo = lineInfo.vertical;
            this.currentLeaf = lineInfo.startIter;
            this.startLeaf = lineInfo.startIter;
            this.endLeaf = lineInfo.endIter;
            this.currentBlankCount = lineInfo.blankCount;
            this.currentLineWidth = lineInfo.lineWidth;
            this.currentWordsWidth = lineInfo.wordsWidth;
            this.currentXPosition = this.StartXPosition;
            this.tabOffsets = lineInfo.tabOffsets;
            this.lastTabPassed = lineInfo.lastTab == null;
            this.lastTab = lineInfo.lastTab;
            this.tabIdx = 0;
            bool flag = this.currentLeaf == null;
            if (this.isFirstLine)
            {
                this.RenderListSymbol();
            }
            while (!flag)
            {
                if (this.currentLeaf.Current == lineInfo.endIter.Current)
                {
                    flag = true;
                }
                if (this.currentLeaf.Current == lineInfo.lastTab)
                {
                    this.lastTabPassed = true;
                }
                this.RenderElement(this.currentLeaf.Current);
                this.currentLeaf = this.currentLeaf.GetNextLeaf();
            }
            this.currentYPosition = ((double) this.currentYPosition) + ((double) lineInfo.vertical.height);
            this.isFirstLine = false;
        }

        private void RenderLinebreak()
        {
            this.RenderUnderline(0, false);
            this.RealizeHyperlink(0);
        }

        private void RenderListSymbol()
        {
            string str;
            XFont font;
            if (this.GetListSymbol(out str, out font))
            {
                XBrush brush = FontHandler.FontColorToXBrush(this.paragraph.Format.Font);
                base.gfx.DrawString(str, font, brush, (double) this.currentXPosition, (double) this.CurrentBaselinePosition);
                this.currentXPosition = ((double) this.currentXPosition) + base.gfx.MeasureString(str, font, StringFormat).Width;
                TabOffset offset = this.NextTabOffset();
                this.currentXPosition = ((double) this.currentXPosition) + ((double) offset.offset);
                this.lastTabPosition = this.currentXPosition;
            }
        }

        private void RenderNumPagesField(NumPagesField numPagesField)
        {
            this.RenderWord(this.GetFieldValue(numPagesField));
        }

        private void RenderPageField(PageField pageField)
        {
            this.RenderWord(this.GetFieldValue(pageField));
        }

        private void RenderPageRefField(PageRefField pageRefField)
        {
            this.RenderWord(this.GetFieldValue(pageRefField));
        }

        private void RenderSectionField(SectionField sectionField)
        {
            this.RenderWord(this.GetFieldValue(sectionField));
        }

        private void RenderSectionPagesField(SectionPagesField sectionPagesField)
        {
            this.RenderWord(this.GetFieldValue(sectionPagesField));
        }

        private void RenderShading()
        {
            if (!this.paragraph.Format.IsNull("Shading"))
            {
                ShadingRenderer renderer = new ShadingRenderer(base.gfx, this.paragraph.Format.Shading);
                Area shadingArea = this.GetShadingArea();
                renderer.Render(shadingArea.X, shadingArea.Y, shadingArea.Width, shadingArea.Height);
            }
        }

        private void RenderSoftHyphen()
        {
            if (this.currentLeaf.Current == this.endLeaf.Current)
            {
                this.RenderWord("-");
            }
        }

        private void RenderSpace(Character character)
        {
            this.currentXPosition = ((double) this.currentXPosition) + ((double) this.GetSpaceWidth(character));
        }

        private void RenderSymbol(Character character)
        {
            string symbol = GetSymbol(character);
            string word = symbol;
            for (int i = 1; i < character.Count; i++)
            {
                word = word + symbol;
            }
            this.RenderWord(word);
        }

        private void RenderTab()
        {
            TabOffset tabOffset = this.NextTabOffset();
            this.RenderUnderline(tabOffset.offset, false);
            this.RenderTabLeader(tabOffset);
            this.RealizeHyperlink(tabOffset.offset);
            this.currentXPosition = ((double) this.currentXPosition) + ((double) tabOffset.offset);
            if (this.currentLeaf.Current == this.lastTab)
            {
                this.lastTabPosition = this.currentXPosition;
            }
        }

        private void RenderTabLeader(TabOffset tabOffset)
        {
            string word = " ";
            switch (tabOffset.leader)
            {
                case TabLeader.Dots:
                    word = ".";
                    break;

                case TabLeader.Dashes:
                    word = "-";
                    break;

                case TabLeader.Lines:
                case TabLeader.Heavy:
                    word = "_";
                    break;

                case TabLeader.MiddleDot:
                    word = "\x00b7";
                    break;

                default:
                    return;
            }
            XUnit unit = this.MeasureString(word);
            XUnit currentXPosition = this.currentXPosition;
            string s = "";
            while ((((double) currentXPosition) + ((double) unit)) <= (((double) this.currentXPosition) + ((double) tabOffset.offset)))
            {
                s = s + word;
                currentXPosition = ((double) currentXPosition) + ((double) unit);
            }
            Font currentDomFont = this.CurrentDomFont;
            XFont currentFont = this.CurrentFont;
            if (currentDomFont.Subscript || currentDomFont.Superscript)
            {
                currentFont = FontHandler.ToSubSuperFont(currentFont);
            }
            base.gfx.DrawString(s, currentFont, this.CurrentBrush, (double) this.currentXPosition, (double) this.CurrentBaselinePosition);
        }

        private void RenderText(Text text)
        {
            this.RenderWord(text.Content);
        }

        private void RenderUnderline(XUnit width, bool isWord)
        {
            XPen underlinePen = this.GetUnderlinePen(isWord);
            if (this.UnderlinePenChanged(underlinePen))
            {
                if (this.currentUnderlinePen != null)
                {
                    this.EndUnderline(this.currentUnderlinePen, this.currentXPosition);
                }
                if (underlinePen != null)
                {
                    this.StartUnderline(this.currentXPosition);
                }
                this.currentUnderlinePen = underlinePen;
            }
            if (this.currentLeaf.Current == this.endLeaf.Current)
            {
                if (this.currentUnderlinePen != null)
                {
                    this.EndUnderline(this.currentUnderlinePen, ((double) this.currentXPosition) + ((double) width));
                }
                this.currentUnderlinePen = null;
            }
        }

        private void RenderWord(string word)
        {
            Font currentDomFont = this.CurrentDomFont;
            XFont currentFont = this.CurrentFont;
            if (currentDomFont.Subscript || currentDomFont.Superscript)
            {
                currentFont = FontHandler.ToSubSuperFont(currentFont);
            }
            base.gfx.DrawString(word, currentFont, this.CurrentBrush, (double) this.currentXPosition, (double) this.CurrentBaselinePosition);
            XUnit width = this.MeasureString(word);
            this.RenderUnderline(width, true);
            this.RealizeHyperlink(width);
            this.currentXPosition = ((double) this.currentXPosition) + ((double) width);
        }

        private void RestoreAfterProbing(ParagraphIterator paragraphIter, int blankCount, XUnit wordsWidth, XUnit xPosition, XUnit lineWidth, XUnit blankWidth)
        {
            this.currentLeaf = paragraphIter;
            this.currentBlankCount = blankCount;
            this.currentXPosition = xPosition;
            this.currentLineWidth = lineWidth;
            this.currentWordsWidth = wordsWidth;
            this.savedBlankWidth = blankWidth;
        }

        private void SaveBeforeProbing(out ParagraphIterator paragraphIter, out int blankCount, out XUnit wordsWidth, out XUnit xPosition, out XUnit lineWidth, out XUnit blankWidth)
        {
            paragraphIter = this.currentLeaf;
            blankCount = this.currentBlankCount;
            xPosition = this.currentXPosition;
            lineWidth = this.currentLineWidth;
            wordsWidth = this.currentWordsWidth;
            blankWidth = this.savedBlankWidth;
        }

        private void SaveBlankWidth(XUnit blankWidth)
        {
            this.savedBlankWidth = blankWidth;
        }

        private void StartHyperlink(XUnit left, XUnit top)
        {
            this.hyperlinkRect = new XRect((double) left, (double) top, 0.0, 0.0);
        }

        private bool StartNewLine()
        {
            this.tabOffsets = new ArrayList();
            this.lastTab = null;
            this.lastTabPosition = 0;
            this.currentYPosition = ((double) this.currentYPosition) + ((double) this.currentVerticalInfo.height);
            if (this.formattingArea.GetFittingRect(this.currentYPosition, ((double) this.currentVerticalInfo.height) + ((double) this.BottomBorderOffset)) == null)
            {
                return false;
            }
            this.isFirstLine = false;
            this.currentXPosition = this.StartXPosition;
            this.currentVerticalInfo = new VerticalLineInfo();
            this.currentVerticalInfo = this.CalcCurrentVerticalInfo();
            this.startLeaf = this.currentLeaf;
            this.currentBlankCount = 0;
            this.currentWordsWidth = 0;
            this.currentLineWidth = 0;
            return true;
        }

        private void StartUnderline(XUnit xPosition)
        {
            this.underlineStartPos = xPosition;
        }

        private void StoreLineInformation()
        {
            this.PopSavedBlankWidth();
            XUnit topBorderOffset = this.TopBorderOffset;
            Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
            if (((double) topBorderOffset) > 0.0)
            {
                contentArea = this.formattingArea.GetFittingRect(this.formattingArea.Y, topBorderOffset);
            }
            if (contentArea == null)
            {
                contentArea = this.formattingArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height);
            }
            else
            {
                contentArea = contentArea.Unite(this.formattingArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height));
            }
            XUnit bottomBorderOffset = this.BottomBorderOffset;
            if (((double) bottomBorderOffset) > 0.0)
            {
                contentArea = contentArea.Unite(this.formattingArea.GetFittingRect(((double) this.currentYPosition) + ((double) this.currentVerticalInfo.height), bottomBorderOffset));
            }
            LineInfo lineInfo = new LineInfo {
                vertical = this.currentVerticalInfo
            };
            if ((this.startLeaf != null) && (this.startLeaf == this.currentLeaf))
            {
                this.HandleNonFittingLine();
            }
            lineInfo.lastTab = this.lastTab;
            base.renderInfo.LayoutInfo.ContentArea = contentArea;
            lineInfo.startIter = this.startLeaf;
            if (this.currentLeaf == null)
            {
                lineInfo.endIter = new ParagraphIterator(this.paragraph.Elements).GetLastLeaf();
            }
            else
            {
                lineInfo.endIter = this.currentLeaf.GetPreviousLeaf();
            }
            lineInfo.blankCount = this.currentBlankCount;
            lineInfo.wordsWidth = this.currentWordsWidth;
            lineInfo.lineWidth = this.currentLineWidth;
            lineInfo.tabOffsets = this.tabOffsets;
            lineInfo.reMeasureLine = this.reMeasureLine;
            this.savedWordWidth = 0;
            this.reMeasureLine = false;
            ((ParagraphFormatInfo) base.renderInfo.FormatInfo).AddLineInfo(lineInfo);
        }

        private bool UnderlinePenChanged(XPen pen)
        {
            if ((pen == null) && (this.currentUnderlinePen == null))
            {
                return false;
            }
            return (((pen == null) && (this.currentUnderlinePen != null)) || (((pen != null) && (this.currentUnderlinePen == null)) || ((pen.Color != this.currentUnderlinePen.Color) || (pen.Width != this.currentUnderlinePen.Width))));
        }

        private XUnit BottomBorderOffset
        {
            get
            {
                XUnit unit = 0;
                if ((((this.phase == Phase.Formatting) && ((this.currentLeaf == null) || this.IsLastVisibleLeaf)) || ((this.phase == Phase.Rendering) && this.isLastLine)) && !this.paragraph.Format.IsNull("Borders"))
                {
                    unit = ((double) unit) + ((double) this.paragraph.Format.Borders.DistanceFromBottom);
                    BordersRenderer renderer = new BordersRenderer(this.paragraph.Format.Borders, base.gfx);
                    unit = ((double) unit) + ((double) renderer.GetWidth(BorderType.Bottom));
                }
                return unit;
            }
        }

        private XUnit CurrentBaselinePosition
        {
            get
            {
                VerticalLineInfo currentVerticalInfo = this.currentVerticalInfo;
                XUnit currentYPosition = this.currentYPosition;
                Font currentDomFont = this.CurrentDomFont;
                XFont currentFont = this.CurrentFont;
                if (currentDomFont.Subscript)
                {
                    currentYPosition = ((double) currentYPosition) + ((double) currentVerticalInfo.inherentlineSpace);
                    return (((double) currentYPosition) - (FontHandler.GetSubSuperScaling(this.CurrentFont) * ((double) FontHandler.GetDescent(currentFont))));
                }
                if (currentDomFont.Superscript)
                {
                    return (((double) currentYPosition) + (FontHandler.GetSubSuperScaling(this.CurrentFont) * (currentFont.GetHeight() - ((double) FontHandler.GetDescent(currentFont)))));
                }
                return (((double) currentYPosition) + (((double) currentVerticalInfo.inherentlineSpace) - ((double) currentVerticalInfo.descent)));
            }
        }

        private XBrush CurrentBrush
        {
            get
            {
                if (this.currentLeaf != null)
                {
                    return FontHandler.FontColorToXBrush(this.CurrentDomFont);
                }
                return null;
            }
        }

        private Font CurrentDomFont
        {
            get
            {
                if (this.currentLeaf != null)
                {
                    DocumentObject parent = DocumentRelations.GetParent(DocumentRelations.GetParent(this.currentLeaf.Current));
                    if (parent is FormattedText)
                    {
                        return ((FormattedText) parent).Font;
                    }
                    if (parent is Hyperlink)
                    {
                        return ((Hyperlink) parent).Font;
                    }
                }
                return this.paragraph.Format.Font;
            }
        }

        private XFont CurrentFont =>
            FontHandler.FontToXFont(this.CurrentDomFont, base.documentRenderer.PrivateFonts, base.gfx.MUH, base.gfx.MFEH);

        private RenderInfo CurrentImageRenderInfo
        {
            get
            {
                if ((this.currentLeaf == null) || !(this.currentLeaf.Current is Image))
                {
                    return null;
                }
                Image current = (Image) this.currentLeaf.Current;
                if ((this.imageRenderInfos != null) && this.imageRenderInfos.ContainsKey(current))
                {
                    return (RenderInfo) this.imageRenderInfos[current];
                }
                if (this.imageRenderInfos == null)
                {
                    this.imageRenderInfos = new Hashtable();
                }
                RenderInfo info = this.CalcImageRenderInfo(current);
                this.imageRenderInfos.Add(current, info);
                return info;
            }
        }

        private XUnit CurrentWordDistance
        {
            get
            {
                if ((((this.phase != Phase.Rendering) || (this.paragraph.Format.Alignment != ParagraphAlignment.Justify)) || (!this.lastTabPassed || (this.currentBlankCount < 1))) || (this.isLastLine && base.renderInfo.FormatInfo.IsEnding))
                {
                    return this.MeasureString(" ");
                }
                Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
                XUnit width = contentArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height).Width;
                if (((double) this.lastTabPosition) > 0.0)
                {
                    width = ((double) width) - (((double) this.lastTabPosition) - ((double) contentArea.X));
                }
                else
                {
                    width = ((double) width) - ((double) this.LeftIndent);
                }
                width = ((double) width) - ((double) this.RightIndent);
                return ((((double) width) - ((double) this.currentWordsWidth)) / ((double) this.currentBlankCount));
            }
        }

        private bool IgnoreHorizontalGrowth =>
            (((this.phase == Phase.Rendering) && (this.paragraph.Format.Alignment == ParagraphAlignment.Justify)) && !this.lastTabPassed);

        internal override LayoutInfo InitialLayoutInfo =>
            new LayoutInfo { 
                PageBreakBefore=this.paragraph.Format.PageBreakBefore,
                MarginTop=this.paragraph.Format.SpaceBefore.Point,
                MarginBottom=this.paragraph.Format.SpaceAfter.Point,
                MarginRight=0,
                MarginLeft=0,
                KeepTogether=this.paragraph.Format.KeepTogether,
                KeepWithNext=this.paragraph.Format.KeepWithNext
            };

        private bool IsLastVisibleLeaf =>
            this.currentLeaf.IsLastLeaf;

        private XUnit LeftIndent
        {
            get
            {
                ParagraphFormat format = this.paragraph.Format;
                XUnit point = format.LeftIndent.Point;
                if (!this.isFirstLine)
                {
                    return point;
                }
                if (!format.IsNull("ListInfo"))
                {
                    if (!format.ListInfo.IsNull("NumberPosition"))
                    {
                        return format.ListInfo.NumberPosition.Point;
                    }
                    if (format.IsNull("FirstLineIndent"))
                    {
                        return 0;
                    }
                }
                return (((double) point) + this.paragraph.Format.FirstLineIndent.Point);
            }
        }

        private XUnit RightIndent =>
            this.paragraph.Format.RightIndent.Point;

        private XUnit StartXPosition
        {
            get
            {
                XUnit unit = 0;
                if (this.phase == Phase.Formatting)
                {
                    return (((double) this.formattingArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height).X) + ((double) this.LeftIndent));
                }
                Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
                XUnit x = contentArea.X;
                XUnit width = contentArea.Width;
                Rectangle fittingRect = contentArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height);
                if (fittingRect != null)
                {
                    x = fittingRect.X;
                    width = fittingRect.Width;
                }
                switch (this.paragraph.Format.Alignment)
                {
                    case ParagraphAlignment.Left:
                    case ParagraphAlignment.Justify:
                        unit = x;
                        return (((double) unit) + ((double) this.LeftIndent));

                    case ParagraphAlignment.Center:
                        return (((double) x) + ((((((double) width) + ((double) this.LeftIndent)) - ((double) this.RightIndent)) - ((double) this.currentLineWidth)) / 2.0));

                    case ParagraphAlignment.Right:
                        unit = (((double) x) + ((double) width)) - ((double) this.RightIndent);
                        return (((double) unit) - ((double) this.currentLineWidth));
                }
                return unit;
            }
        }

        private static XStringFormat StringFormat
        {
            get
            {
                if (stringFormat == null)
                {
                    stringFormat = XStringFormats.Default;
                    stringFormat.FormatFlags |= XStringFormatFlags.MeasureTrailingSpaces;
                }
                return stringFormat;
            }
        }

        private XUnit TopBorderOffset
        {
            get
            {
                XUnit unit = 0;
                if (this.isFirstLine && !this.paragraph.Format.IsNull("Borders"))
                {
                    unit = ((double) unit) + ((double) this.paragraph.Format.Borders.DistanceFromTop);
                    if (!this.paragraph.Format.IsNull("Borders"))
                    {
                        BordersRenderer renderer = new BordersRenderer(this.paragraph.Format.Borders, base.gfx);
                        unit = ((double) unit) + ((double) renderer.GetWidth(BorderType.Top));
                    }
                }
                return unit;
            }
        }

        private enum FormatResult
        {
            Ignore,
            Continue,
            NewLine,
            NewArea
        }

        private enum Phase
        {
            Formatting,
            Rendering
        }
    }
}

