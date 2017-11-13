namespace MigraDoc.DocumentObjectModel.Visitors
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using MigraDoc.DocumentObjectModel.Tables;
    using System;

    public abstract class VisitorBase : DocumentObjectVisitor
    {
        protected void FlattenAxis(Axis axis)
        {
            if (axis != null)
            {
                LineFormat refLineFormat = new LineFormat {
                    width = 0.15
                };
                if (axis.hasMajorGridlines.Value && (axis.majorGridlines != null))
                {
                    this.FlattenLineFormat(axis.majorGridlines.lineFormat, refLineFormat);
                }
                if (axis.hasMinorGridlines.Value && (axis.minorGridlines != null))
                {
                    this.FlattenLineFormat(axis.minorGridlines.lineFormat, refLineFormat);
                }
                refLineFormat.width = 0.4;
                if (axis.lineFormat != null)
                {
                    this.FlattenLineFormat(axis.lineFormat, refLineFormat);
                }
            }
        }

        protected void FlattenBorder(Border border, Border refBorder)
        {
            if (border.visible.IsNull)
            {
                border.visible = refBorder.visible;
            }
            if (border.width.IsNull)
            {
                border.width = refBorder.width;
            }
            if (border.style.IsNull)
            {
                border.style = refBorder.style;
            }
            if (border.color.IsNull)
            {
                border.color = refBorder.color;
            }
        }

        protected void FlattenBorders(Borders borders, Borders refBorders)
        {
            if (borders.visible.IsNull)
            {
                borders.visible = refBorders.visible;
            }
            if (borders.width.IsNull)
            {
                borders.width = refBorders.width;
            }
            if (borders.style.IsNull)
            {
                borders.style = refBorders.style;
            }
            if (borders.color.IsNull)
            {
                borders.color = refBorders.color;
            }
            if (borders.distanceFromBottom.IsNull)
            {
                borders.distanceFromBottom = refBorders.distanceFromBottom;
            }
            if (borders.distanceFromRight.IsNull)
            {
                borders.distanceFromRight = refBorders.distanceFromRight;
            }
            if (borders.distanceFromLeft.IsNull)
            {
                borders.distanceFromLeft = refBorders.distanceFromLeft;
            }
            if (borders.distanceFromTop.IsNull)
            {
                borders.distanceFromTop = refBorders.distanceFromTop;
            }
            if (refBorders.left != null)
            {
                this.FlattenBorder(borders.Left, refBorders.left);
                this.FlattenedBorderFromBorders(borders.left, borders);
            }
            if (refBorders.right != null)
            {
                this.FlattenBorder(borders.Right, refBorders.right);
                this.FlattenedBorderFromBorders(borders.right, borders);
            }
            if (refBorders.top != null)
            {
                this.FlattenBorder(borders.Top, refBorders.top);
                this.FlattenedBorderFromBorders(borders.top, borders);
            }
            if (refBorders.bottom != null)
            {
                this.FlattenBorder(borders.Bottom, refBorders.bottom);
                this.FlattenedBorderFromBorders(borders.bottom, borders);
            }
        }

        protected void FlattenDataLabel(DataLabel dataLabel)
        {
        }

        protected Border FlattenedBorderFromBorders(Border border, Borders parentBorders)
        {
            if (border == null)
            {
                border = new Border(parentBorders);
            }
            if (border.visible.IsNull)
            {
                border.visible = parentBorders.visible;
            }
            if (border.style.IsNull)
            {
                border.style = parentBorders.style;
            }
            if (border.width.IsNull)
            {
                border.width = parentBorders.width;
            }
            if (border.color.IsNull)
            {
                border.color = parentBorders.color;
            }
            return border;
        }

        protected void FlattenFillFormat(FillFormat fillFormat)
        {
        }

        protected void FlattenFont(Font font, Font refFont)
        {
            if (font.name.IsNull)
            {
                font.name = refFont.name;
            }
            if (font.size.IsNull)
            {
                font.size = refFont.size;
            }
            if (font.color.IsNull)
            {
                font.color = refFont.color;
            }
            if (font.underline.IsNull)
            {
                font.underline = refFont.underline;
            }
            if (font.bold.IsNull)
            {
                font.bold = refFont.bold;
            }
            if (font.italic.IsNull)
            {
                font.italic = refFont.italic;
            }
            if (font.superscript.IsNull)
            {
                font.superscript = refFont.superscript;
            }
            if (font.subscript.IsNull)
            {
                font.subscript = refFont.subscript;
            }
        }

        protected void FlattenHeaderFooter(HeaderFooter headerFooter, bool isHeader)
        {
        }

        protected void FlattenLineFormat(LineFormat lineFormat, LineFormat refLineFormat)
        {
            if ((refLineFormat != null) && lineFormat.width.IsNull)
            {
                lineFormat.width = refLineFormat.width;
            }
        }

        protected void FlattenListInfo(ListInfo listInfo, ListInfo refListInfo)
        {
            if (listInfo.continuePreviousList.IsNull)
            {
                listInfo.continuePreviousList = refListInfo.continuePreviousList;
            }
            if (listInfo.listType.IsNull)
            {
                listInfo.listType = refListInfo.listType;
            }
            if (listInfo.numberPosition.IsNull)
            {
                listInfo.numberPosition = refListInfo.numberPosition;
            }
        }

        protected void FlattenPageSetup(PageSetup pageSetup, PageSetup refPageSetup)
        {
            if (pageSetup.pageWidth.IsNull && pageSetup.pageHeight.IsNull)
            {
                if (pageSetup.pageFormat.IsNull)
                {
                    pageSetup.pageWidth = refPageSetup.pageWidth;
                    pageSetup.pageHeight = refPageSetup.pageHeight;
                    pageSetup.pageFormat = refPageSetup.pageFormat;
                }
                else
                {
                    PageSetup.GetPageSize(pageSetup.PageFormat, out pageSetup.pageWidth, out pageSetup.pageHeight);
                }
            }
            else
            {
                Unit unit;
                if (pageSetup.pageWidth.IsNull)
                {
                    if (pageSetup.pageFormat.IsNull)
                    {
                        pageSetup.pageHeight = refPageSetup.pageHeight;
                    }
                    else
                    {
                        PageSetup.GetPageSize(pageSetup.PageFormat, out unit, out pageSetup.pageHeight);
                    }
                }
                else if (pageSetup.pageHeight.IsNull)
                {
                    if (pageSetup.pageFormat.IsNull)
                    {
                        pageSetup.pageWidth = refPageSetup.pageWidth;
                    }
                    else
                    {
                        PageSetup.GetPageSize(pageSetup.PageFormat, out pageSetup.pageWidth, out unit);
                    }
                }
            }
            if (pageSetup.sectionStart.IsNull)
            {
                pageSetup.sectionStart = refPageSetup.sectionStart;
            }
            if (pageSetup.orientation.IsNull)
            {
                pageSetup.orientation = refPageSetup.orientation;
            }
            if (pageSetup.topMargin.IsNull)
            {
                pageSetup.topMargin = refPageSetup.topMargin;
            }
            if (pageSetup.bottomMargin.IsNull)
            {
                pageSetup.bottomMargin = refPageSetup.bottomMargin;
            }
            if (pageSetup.leftMargin.IsNull)
            {
                pageSetup.leftMargin = refPageSetup.leftMargin;
            }
            if (pageSetup.rightMargin.IsNull)
            {
                pageSetup.rightMargin = refPageSetup.rightMargin;
            }
            if (pageSetup.headerDistance.IsNull)
            {
                pageSetup.headerDistance = refPageSetup.headerDistance;
            }
            if (pageSetup.footerDistance.IsNull)
            {
                pageSetup.footerDistance = refPageSetup.footerDistance;
            }
            if (pageSetup.oddAndEvenPagesHeaderFooter.IsNull)
            {
                pageSetup.oddAndEvenPagesHeaderFooter = refPageSetup.oddAndEvenPagesHeaderFooter;
            }
            if (pageSetup.differentFirstPageHeaderFooter.IsNull)
            {
                pageSetup.differentFirstPageHeaderFooter = refPageSetup.differentFirstPageHeaderFooter;
            }
            if (pageSetup.mirrorMargins.IsNull)
            {
                pageSetup.mirrorMargins = refPageSetup.mirrorMargins;
            }
            if (pageSetup.horizontalPageBreak.IsNull)
            {
                pageSetup.horizontalPageBreak = refPageSetup.horizontalPageBreak;
            }
        }

        protected void FlattenParagraphFormat(ParagraphFormat format, ParagraphFormat refFormat)
        {
            if (format.alignment.IsNull)
            {
                format.alignment = refFormat.alignment;
            }
            if (format.firstLineIndent.IsNull)
            {
                format.firstLineIndent = refFormat.firstLineIndent;
            }
            if (format.leftIndent.IsNull)
            {
                format.leftIndent = refFormat.leftIndent;
            }
            if (format.rightIndent.IsNull)
            {
                format.rightIndent = refFormat.rightIndent;
            }
            if (format.spaceBefore.IsNull)
            {
                format.spaceBefore = refFormat.spaceBefore;
            }
            if (format.spaceAfter.IsNull)
            {
                format.spaceAfter = refFormat.spaceAfter;
            }
            if (format.lineSpacingRule.IsNull)
            {
                format.lineSpacingRule = refFormat.lineSpacingRule;
            }
            if (format.lineSpacing.IsNull)
            {
                format.lineSpacing = refFormat.lineSpacing;
            }
            if (format.widowControl.IsNull)
            {
                format.widowControl = refFormat.widowControl;
            }
            if (format.keepTogether.IsNull)
            {
                format.keepTogether = refFormat.keepTogether;
            }
            if (format.keepWithNext.IsNull)
            {
                format.keepWithNext = refFormat.keepWithNext;
            }
            if (format.pageBreakBefore.IsNull)
            {
                format.pageBreakBefore = refFormat.pageBreakBefore;
            }
            if (format.outlineLevel.IsNull)
            {
                format.outlineLevel = refFormat.outlineLevel;
            }
            if (format.font == null)
            {
                if (refFormat.font != null)
                {
                    format.font = refFormat.font.Clone();
                    format.font.parent = format;
                }
            }
            else if (refFormat.font != null)
            {
                this.FlattenFont(format.font, refFormat.font);
            }
            if (format.shading == null)
            {
                if (refFormat.shading != null)
                {
                    format.shading = refFormat.shading.Clone();
                    format.shading.parent = format;
                }
            }
            else if (refFormat.shading != null)
            {
                this.FlattenShading(format.shading, refFormat.shading);
            }
            if (format.borders == null)
            {
                format.borders = refFormat.borders;
            }
            else if (refFormat.borders != null)
            {
                this.FlattenBorders(format.borders, refFormat.borders);
            }
            if (refFormat.tabStops != null)
            {
                this.FlattenTabStops(format.TabStops, refFormat.tabStops);
            }
            if (refFormat.listInfo != null)
            {
                this.FlattenListInfo(format.ListInfo, refFormat.listInfo);
            }
        }

        protected void FlattenPlotArea(PlotArea plotArea)
        {
        }

        protected void FlattenShading(Shading shading, Shading refShading)
        {
            if (shading.visible.IsNull)
            {
                shading.visible = refShading.visible;
            }
            if (shading.color.IsNull)
            {
                shading.color = refShading.color;
            }
        }

        protected void FlattenTabStops(TabStops tabStops, TabStops refTabStops)
        {
            if (!tabStops.fClearAll)
            {
                foreach (TabStop stop in refTabStops)
                {
                    if ((tabStops.GetTabStopAt(stop.Position) == null) && stop.AddTab)
                    {
                        tabStops.AddTabStop(stop.Position, stop.Alignment, stop.Leader);
                    }
                }
            }
            for (int i = 0; i < tabStops.Count; i++)
            {
                TabStop stop2 = tabStops[i];
                if (!stop2.AddTab)
                {
                    tabStops.RemoveObjectAt(i);
                }
            }
            tabStops.fClearAll = true;
        }

        private DocumentObject GetDocumentElementHolder(DocumentObject docObj)
        {
            DocumentElements parent = (DocumentElements) docObj.parent;
            return parent.parent;
        }

        private ParagraphFormat ParagraphFormatFromStyle(Style style)
        {
            if (style.Type == StyleType.Character)
            {
                Document document = style.Document;
                ParagraphFormat format = style.paragraphFormat.Clone();
                this.FlattenParagraphFormat(format, document.Styles.Normal.ParagraphFormat);
                return format;
            }
            return style.paragraphFormat;
        }

        public override void Visit(DocumentObject documentObject)
        {
            IVisitable visitable = documentObject as IVisitable;
            if (visitable != null)
            {
                visitable.AcceptVisitor(this, true);
            }
        }

        internal override void VisitCell(Cell cell)
        {
        }

        internal override void VisitChart(Chart chart)
        {
            Document document = chart.Document;
            if (chart.style.IsNull)
            {
                chart.style.Value = "Normal";
            }
            Style style = document.Styles[chart.style.Value];
            if (chart.format == null)
            {
                chart.format = style.paragraphFormat.Clone();
                chart.format.parent = chart;
            }
            else
            {
                this.FlattenParagraphFormat(chart.format, style.paragraphFormat);
            }
            this.FlattenLineFormat(chart.lineFormat, null);
            this.FlattenFillFormat(chart.fillFormat);
            this.FlattenAxis(chart.xAxis);
            this.FlattenAxis(chart.yAxis);
            this.FlattenAxis(chart.zAxis);
            this.FlattenPlotArea(chart.plotArea);
            this.FlattenDataLabel(chart.dataLabel);
        }

        internal override void VisitColumns(Columns columns)
        {
            foreach (Column column in columns)
            {
                if (column.width.IsNull)
                {
                    column.width = columns.width;
                }
                if (column.width.IsNull)
                {
                    column.width = "2.5cm";
                }
            }
        }

        internal override void VisitDocument(Document document)
        {
        }

        internal override void VisitDocumentElements(DocumentElements elements)
        {
        }

        internal override void VisitFootnote(Footnote footnote)
        {
            Document document = footnote.Document;
            ParagraphFormat refFormat = null;
            Style style = document.styles[footnote.style.Value];
            if (style != null)
            {
                refFormat = this.ParagraphFormatFromStyle(style);
            }
            else
            {
                footnote.Style = "Footnote";
                refFormat = document.styles[footnote.Style].paragraphFormat;
            }
            if (footnote.format == null)
            {
                footnote.format = refFormat.Clone();
                footnote.format.parent = footnote;
            }
            else
            {
                this.FlattenParagraphFormat(footnote.format, refFormat);
            }
        }

        internal override void VisitHeaderFooter(HeaderFooter headerFooter)
        {
            string str;
            ParagraphFormat paragraphFormat;
            Document document = headerFooter.Document;
            if (headerFooter.IsHeader)
            {
                str = "Header";
            }
            else
            {
                str = "Footer";
            }
            Style style = document.styles[headerFooter.style.Value];
            if (style != null)
            {
                paragraphFormat = this.ParagraphFormatFromStyle(style);
            }
            else
            {
                paragraphFormat = document.styles[str].paragraphFormat;
                headerFooter.Style = str;
            }
            if (headerFooter.format == null)
            {
                headerFooter.format = paragraphFormat.Clone();
                headerFooter.format.parent = headerFooter;
            }
            else
            {
                this.FlattenParagraphFormat(headerFooter.format, paragraphFormat);
            }
        }

        internal override void VisitHeadersFooters(HeadersFooters headersFooters)
        {
        }

        internal override void VisitLegend(Legend legend)
        {
            ParagraphFormat paragraphFormat;
            if (!legend.style.IsNull)
            {
                Style style = legend.Document.Styles[legend.Style];
                if (style == null)
                {
                    style = legend.Document.Styles["InvalidStyleName"];
                }
                paragraphFormat = style.paragraphFormat;
            }
            else
            {
                TextArea documentElementHolder = (TextArea) this.GetDocumentElementHolder(legend);
                legend.style = documentElementHolder.style;
                paragraphFormat = documentElementHolder.format;
            }
            if (legend.format == null)
            {
                legend.Format = paragraphFormat.Clone();
            }
            else
            {
                this.FlattenParagraphFormat(legend.format, paragraphFormat);
            }
        }

        internal override void VisitParagraph(Paragraph paragraph)
        {
            Document document = paragraph.Document;
            ParagraphFormat refFormat = null;
            DocumentObject documentElementHolder = this.GetDocumentElementHolder(paragraph);
            Style style = document.styles[paragraph.style.Value];
            if (style != null)
            {
                refFormat = this.ParagraphFormatFromStyle(style);
            }
            else if (documentElementHolder is Cell)
            {
                paragraph.style = ((Cell) documentElementHolder).style;
                refFormat = ((Cell) documentElementHolder).format;
            }
            else if (documentElementHolder is HeaderFooter)
            {
                HeaderFooter footer = (HeaderFooter) documentElementHolder;
                if (footer.IsHeader)
                {
                    paragraph.Style = "Header";
                    refFormat = document.styles["Header"].paragraphFormat;
                }
                else
                {
                    paragraph.Style = "Footer";
                    refFormat = document.styles["Footer"].paragraphFormat;
                }
                if (footer.format != null)
                {
                    this.FlattenParagraphFormat(paragraph.Format, footer.format);
                }
            }
            else if (documentElementHolder is Footnote)
            {
                paragraph.Style = "Footnote";
                refFormat = document.styles["Footnote"].paragraphFormat;
            }
            else if (documentElementHolder is TextArea)
            {
                paragraph.style = ((TextArea) documentElementHolder).style;
                refFormat = ((TextArea) documentElementHolder).format;
            }
            else
            {
                if (paragraph.style.Value != "")
                {
                    paragraph.Style = "InvalidStyleName";
                }
                else
                {
                    paragraph.Style = "Normal";
                }
                refFormat = document.styles[paragraph.Style].paragraphFormat;
            }
            if (paragraph.format == null)
            {
                paragraph.format = refFormat.Clone();
                paragraph.format.parent = paragraph;
            }
            else
            {
                this.FlattenParagraphFormat(paragraph.format, refFormat);
            }
        }

        internal override void VisitRow(Row row)
        {
            foreach (Cell cell in row.Cells)
            {
                if (cell.verticalAlignment.IsNull)
                {
                    cell.verticalAlignment = row.verticalAlignment;
                }
            }
        }

        internal override void VisitRows(Rows rows)
        {
            foreach (Row row in rows)
            {
                if (row.height.IsNull)
                {
                    row.height = rows.height;
                }
                if (row.heightRule.IsNull)
                {
                    row.heightRule = rows.heightRule;
                }
                if (row.verticalAlignment.IsNull)
                {
                    row.verticalAlignment = rows.verticalAlignment;
                }
            }
        }

        internal override void VisitSection(Section section)
        {
            Section section2 = section.PreviousSection();
            PageSetup defaultPageSetup = PageSetup.DefaultPageSetup;
            if (section2 != null)
            {
                defaultPageSetup = section2.pageSetup;
                if (!section.Headers.HasHeaderFooter(HeaderFooterIndex.Primary))
                {
                    section.Headers.primary = section2.Headers.primary;
                }
                if (!section.Headers.HasHeaderFooter(HeaderFooterIndex.EvenPage))
                {
                    section.Headers.evenPage = section2.Headers.evenPage;
                }
                if (!section.Headers.HasHeaderFooter(HeaderFooterIndex.FirstPage))
                {
                    section.Headers.firstPage = section2.Headers.firstPage;
                }
                if (!section.Footers.HasHeaderFooter(HeaderFooterIndex.Primary))
                {
                    section.Footers.primary = section2.Footers.primary;
                }
                if (!section.Footers.HasHeaderFooter(HeaderFooterIndex.EvenPage))
                {
                    section.Footers.evenPage = section2.Footers.evenPage;
                }
                if (!section.Footers.HasHeaderFooter(HeaderFooterIndex.FirstPage))
                {
                    section.Footers.firstPage = section2.Footers.firstPage;
                }
            }
            if (section.pageSetup == null)
            {
                section.pageSetup = defaultPageSetup;
            }
            else
            {
                this.FlattenPageSetup(section.pageSetup, defaultPageSetup);
            }
        }

        internal override void VisitSections(Sections sections)
        {
        }

        internal override void VisitStyle(Style style)
        {
            Style baseStyle = style.GetBaseStyle();
            if ((baseStyle != null) && (baseStyle.paragraphFormat != null))
            {
                if (style.paragraphFormat == null)
                {
                    style.paragraphFormat = baseStyle.paragraphFormat;
                }
                else
                {
                    this.FlattenParagraphFormat(style.paragraphFormat, baseStyle.paragraphFormat);
                }
            }
        }

        internal override void VisitStyles(Styles styles)
        {
        }

        internal override void VisitTable(Table table)
        {
            ParagraphFormat paragraphFormat;
            Document document = table.Document;
            if (table.leftPadding.IsNull)
            {
                table.leftPadding = Unit.FromMillimeter(1.2);
            }
            if (table.rightPadding.IsNull)
            {
                table.rightPadding = Unit.FromMillimeter(1.2);
            }
            Style style = document.styles[table.style.Value];
            if (style != null)
            {
                paragraphFormat = this.ParagraphFormatFromStyle(style);
            }
            else
            {
                table.Style = "Normal";
                paragraphFormat = document.styles.Normal.paragraphFormat;
            }
            if (table.format == null)
            {
                table.format = paragraphFormat.Clone();
                table.format.parent = table;
            }
            else
            {
                this.FlattenParagraphFormat(table.format, paragraphFormat);
            }
            int count = table.Rows.Count;
            int num2 = table.Columns.Count;
            for (int i = 0; i < num2; i++)
            {
                ParagraphFormat format;
                Column column = table.Columns[i];
                style = document.styles[column.style.Value];
                if (style != null)
                {
                    format = this.ParagraphFormatFromStyle(style);
                }
                else
                {
                    column.style = table.style;
                    format = table.Format;
                }
                if (column.format == null)
                {
                    column.format = format.Clone();
                    column.format.parent = column;
                    if ((column.format.shading == null) && (table.format.shading != null))
                    {
                        column.format.shading = table.format.shading;
                    }
                }
                else
                {
                    this.FlattenParagraphFormat(column.format, format);
                }
                if (column.leftPadding.IsNull)
                {
                    column.leftPadding = table.leftPadding;
                }
                if (column.rightPadding.IsNull)
                {
                    column.rightPadding = table.rightPadding;
                }
                if (column.shading == null)
                {
                    column.shading = table.shading;
                }
                else if (table.shading != null)
                {
                    this.FlattenShading(column.shading, table.shading);
                }
                if (column.borders == null)
                {
                    column.borders = table.borders;
                }
                else if (table.borders != null)
                {
                    this.FlattenBorders(column.borders, table.borders);
                }
            }
            for (int j = 0; j < count; j++)
            {
                ParagraphFormat format3;
                Row row = table.Rows[j];
                style = document.styles[row.style.Value];
                if (style != null)
                {
                    format3 = this.ParagraphFormatFromStyle(style);
                }
                else
                {
                    row.style = table.style;
                    format3 = table.Format;
                }
                for (int k = 0; k < num2; k++)
                {
                    Column column2 = table.Columns[k];
                    Cell cell = row[k];
                    Style style2 = document.styles[cell.style.Value];
                    if (style2 != null)
                    {
                        ParagraphFormat refFormat = this.ParagraphFormatFromStyle(style2);
                        if (cell.format == null)
                        {
                            cell.format = refFormat;
                        }
                        else
                        {
                            this.FlattenParagraphFormat(cell.format, refFormat);
                        }
                    }
                    else
                    {
                        if (row.format != null)
                        {
                            this.FlattenParagraphFormat(cell.Format, row.format);
                        }
                        if (style != null)
                        {
                            cell.style = row.style;
                            this.FlattenParagraphFormat(cell.Format, format3);
                        }
                        else
                        {
                            cell.style = column2.style;
                            this.FlattenParagraphFormat(cell.Format, column2.format);
                        }
                    }
                    if ((cell.format.shading == null) && (table.format.shading != null))
                    {
                        cell.format.shading = table.format.shading;
                    }
                    if (cell.shading == null)
                    {
                        cell.shading = row.shading;
                    }
                    else if (row.shading != null)
                    {
                        this.FlattenShading(cell.shading, row.shading);
                    }
                    if (cell.shading == null)
                    {
                        cell.shading = column2.shading;
                    }
                    else if (column2.shading != null)
                    {
                        this.FlattenShading(cell.shading, column2.shading);
                    }
                    if (cell.borders == null)
                    {
                        cell.borders = row.borders;
                    }
                    else if (row.borders != null)
                    {
                        this.FlattenBorders(cell.borders, row.borders);
                    }
                    if (cell.borders == null)
                    {
                        cell.borders = column2.borders;
                    }
                    else if (column2.borders != null)
                    {
                        this.FlattenBorders(cell.borders, column2.borders);
                    }
                }
                if (row.format == null)
                {
                    row.format = format3.Clone();
                    row.format.parent = row;
                    if ((row.format.shading == null) && (table.format.shading != null))
                    {
                        row.format.shading = table.format.shading;
                    }
                }
                else
                {
                    this.FlattenParagraphFormat(row.format, format3);
                }
                if (row.topPadding.IsNull)
                {
                    row.topPadding = table.topPadding;
                }
                if (row.bottomPadding.IsNull)
                {
                    row.bottomPadding = table.bottomPadding;
                }
                if (row.shading == null)
                {
                    row.shading = table.shading;
                }
                else if (table.shading != null)
                {
                    this.FlattenShading(row.shading, table.shading);
                }
                if (row.borders == null)
                {
                    row.borders = table.borders;
                }
                else if (table.borders != null)
                {
                    this.FlattenBorders(row.borders, table.borders);
                }
            }
        }

        internal override void VisitTextArea(TextArea textArea)
        {
            if ((textArea != null) && (textArea.elements != null))
            {
                ParagraphFormat paragraphFormat;
                Document document = textArea.Document;
                if (!textArea.style.IsNull)
                {
                    Style style = textArea.Document.Styles[textArea.Style];
                    if (style == null)
                    {
                        style = textArea.Document.Styles["InvalidStyleName"];
                    }
                    paragraphFormat = style.paragraphFormat;
                }
                else
                {
                    Chart parent = (Chart) textArea.parent;
                    paragraphFormat = parent.format;
                    textArea.style = parent.style;
                }
                if (textArea.format == null)
                {
                    textArea.Format = paragraphFormat.Clone();
                }
                else
                {
                    this.FlattenParagraphFormat(textArea.format, paragraphFormat);
                }
                this.FlattenFillFormat(textArea.fillFormat);
                this.FlattenLineFormat(textArea.lineFormat, null);
            }
        }

        internal override void VisitTextFrame(TextFrame textFrame)
        {
            if (textFrame.height.IsNull)
            {
                textFrame.height = Unit.FromInch(1.0);
            }
            if (textFrame.width.IsNull)
            {
                textFrame.width = Unit.FromInch(1.0);
            }
        }
    }
}

