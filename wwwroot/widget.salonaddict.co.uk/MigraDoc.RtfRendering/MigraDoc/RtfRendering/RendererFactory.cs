namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal sealed class RendererFactory
    {
        internal static RendererBase CreateRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer)
        {
            switch (domObj.GetType().Name)
            {
                case "Style":
                    return new StyleRenderer(domObj, docRenderer);

                case "ParagraphFormat":
                    return new ParagraphFormatRenderer(domObj, docRenderer);

                case "Font":
                    return new FontRenderer(domObj, docRenderer);

                case "Borders":
                    return new BordersRenderer(domObj, docRenderer);

                case "Border":
                    return new BorderRenderer(domObj, docRenderer);

                case "TabStops":
                    return new TabStopsRenderer(domObj, docRenderer);

                case "TabStop":
                    return new TabStopRenderer(domObj, docRenderer);

                case "Section":
                    return new SectionRenderer(domObj, docRenderer);

                case "PageSetup":
                    return new PageSetupRenderer(domObj, docRenderer);

                case "Paragraph":
                    return new ParagraphRenderer(domObj, docRenderer);

                case "Text":
                    return new TextRenderer(domObj, docRenderer);

                case "FormattedText":
                    return new FormattedTextRenderer(domObj, docRenderer);

                case "Character":
                    return new CharacterRenderer(domObj, docRenderer);

                case "BookmarkField":
                    return new BookmarkFieldRenderer(domObj, docRenderer);

                case "PageField":
                    return new PageFieldRenderer(domObj, docRenderer);

                case "PageRefField":
                    return new PageRefFieldRenderer(domObj, docRenderer);

                case "NumPagesField":
                    return new NumPagesFieldRenderer(domObj, docRenderer);

                case "SectionField":
                    return new SectionFieldRenderer(domObj, docRenderer);

                case "SectionPagesField":
                    return new SectionPagesFieldRenderer(domObj, docRenderer);

                case "InfoField":
                    return new InfoFieldRenderer(domObj, docRenderer);

                case "DateField":
                    return new DateFieldRenderer(domObj, docRenderer);

                case "Hyperlink":
                    return new HyperlinkRenderer(domObj, docRenderer);

                case "Footnote":
                    return new FootnoteRenderer(domObj, docRenderer);

                case "ListInfo":
                    return new ListInfoRenderer(domObj, docRenderer);

                case "Image":
                    return new ImageRenderer(domObj, docRenderer);

                case "TextFrame":
                    return new TextFrameRenderer(domObj, docRenderer);

                case "Chart":
                    return new ChartRenderer(domObj, docRenderer);

                case "HeadersFooters":
                    return new HeadersFootersRenderer(domObj, docRenderer);

                case "HeaderFooter":
                    return new HeaderFooterRenderer(domObj, docRenderer);

                case "PageBreak":
                    return new PageBreakRenderer(domObj, docRenderer);

                case "Table":
                    return new TableRenderer(domObj, docRenderer);

                case "Row":
                    return new RowRenderer(domObj, docRenderer);

                case "Cell":
                    return new CellRenderer(domObj, docRenderer);
            }
            return null;
        }
    }
}

