namespace MigraDoc.DocumentObjectModel.Visitors
{
    using MigraDoc.DocumentObjectModel;
    using System;

    public class RtfFlattenVisitor : VisitorBase
    {
        internal override void VisitFormattedText(FormattedText formattedText)
        {
            Document document = formattedText.Document;
            ParagraphFormat paragraphFormat = null;
            Style style = document.styles[formattedText.style.Value];
            if (style != null)
            {
                paragraphFormat = style.paragraphFormat;
            }
            else if (formattedText.style.Value != "")
            {
                paragraphFormat = document.styles["InvalidStyleName"].paragraphFormat;
            }
            if (paragraphFormat != null)
            {
                if (formattedText.font == null)
                {
                    formattedText.Font = paragraphFormat.font.Clone();
                }
                else if (paragraphFormat.font != null)
                {
                    base.FlattenFont(formattedText.font, paragraphFormat.font);
                }
            }
        }

        internal override void VisitHyperlink(Hyperlink hyperlink)
        {
            Font refFont = hyperlink.Document.Styles["Hyperlink"].Font;
            if (hyperlink.font == null)
            {
                hyperlink.Font = refFont.Clone();
            }
            else
            {
                base.FlattenFont(hyperlink.font, refFont);
            }
        }
    }
}

