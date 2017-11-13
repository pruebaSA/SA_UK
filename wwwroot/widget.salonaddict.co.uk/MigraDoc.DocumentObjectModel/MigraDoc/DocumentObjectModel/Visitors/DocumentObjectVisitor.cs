namespace MigraDoc.DocumentObjectModel.Visitors
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using MigraDoc.DocumentObjectModel.Tables;
    using System;

    public abstract class DocumentObjectVisitor
    {
        protected DocumentObjectVisitor()
        {
        }

        public abstract void Visit(DocumentObject documentObject);
        internal virtual void VisitCell(Cell cell)
        {
        }

        internal virtual void VisitChart(Chart chart)
        {
        }

        internal virtual void VisitColumns(Columns columns)
        {
        }

        internal virtual void VisitDocument(Document document)
        {
        }

        internal virtual void VisitDocumentElements(DocumentElements elements)
        {
        }

        internal virtual void VisitDocumentObjectCollection(DocumentObjectCollection elements)
        {
        }

        internal virtual void VisitFont(Font font)
        {
        }

        internal virtual void VisitFootnote(Footnote footnote)
        {
        }

        internal virtual void VisitFormattedText(FormattedText formattedText)
        {
        }

        internal virtual void VisitHeaderFooter(HeaderFooter headerFooter)
        {
        }

        internal virtual void VisitHeadersFooters(HeadersFooters headersFooters)
        {
        }

        internal virtual void VisitHyperlink(Hyperlink hyperlink)
        {
        }

        internal virtual void VisitImage(Image image)
        {
        }

        internal virtual void VisitLegend(Legend legend)
        {
        }

        internal virtual void VisitParagraph(Paragraph paragraph)
        {
        }

        internal virtual void VisitParagraphFormat(ParagraphFormat paragraphFormat)
        {
        }

        internal virtual void VisitRow(Row row)
        {
        }

        internal virtual void VisitRows(Rows rows)
        {
        }

        internal virtual void VisitSection(Section section)
        {
        }

        internal virtual void VisitSections(Sections sections)
        {
        }

        internal virtual void VisitShading(Shading shading)
        {
        }

        internal virtual void VisitStyle(Style style)
        {
        }

        internal virtual void VisitStyles(Styles styles)
        {
        }

        internal virtual void VisitTable(Table table)
        {
        }

        internal virtual void VisitTextArea(TextArea textArea)
        {
        }

        internal virtual void VisitTextFrame(TextFrame textFrame)
        {
        }
    }
}

