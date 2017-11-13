namespace MigraDoc.DocumentObjectModel.Visitors
{
    using MigraDoc.DocumentObjectModel;
    using System;
    using System.Collections;

    public class PdfFlattenVisitor : VisitorBase
    {
        protected Font GetParentFont(DocumentObject obj)
        {
            DocumentObject parent = DocumentRelations.GetParent(DocumentRelations.GetParent(obj));
            if (parent is Paragraph)
            {
                return ((Paragraph) parent).Format.font;
            }
            return (parent.GetValue("Font") as Font);
        }

        internal override void VisitDocumentElements(DocumentElements elements)
        {
            SortedList list = new SortedList();
            for (int i = 0; i < elements.Count; i++)
            {
                Paragraph paragraph = elements[i] as Paragraph;
                if (paragraph != null)
                {
                    Paragraph[] paragraphArray = paragraph.SplitOnParaBreak();
                    if (paragraphArray != null)
                    {
                        list.Add(i, paragraphArray);
                    }
                }
            }
            int num2 = 0;
            for (int j = 0; j < list.Count; j++)
            {
                int key = (int) list.GetKey(j);
                Paragraph[] byIndex = (Paragraph[]) list.GetByIndex(j);
                foreach (Paragraph paragraph2 in byIndex)
                {
                    elements.InsertObject(key + num2, paragraph2);
                    num2++;
                }
                elements.RemoveObjectAt(key + num2);
                num2--;
            }
        }

        internal override void VisitDocumentObjectCollection(DocumentObjectCollection elements)
        {
            ArrayList list = new ArrayList();
            if (elements is ParagraphElements)
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i] is Text)
                    {
                        list.Add(i);
                    }
                }
            }
            int[] numArray = (int[]) list.ToArray(typeof(int));
            if (numArray != null)
            {
                int num2 = 0;
                foreach (int num3 in numArray)
                {
                    Text text = (Text) elements[num3 + num2];
                    string content = "";
                    foreach (char ch in text.Content)
                    {
                        switch (ch)
                        {
                            case '-':
                            {
                                elements.InsertObject(num3 + num2, new Text(content + ch));
                                num2++;
                                content = "";
                                continue;
                            }
                            case '\x00ad':
                            {
                                if (content != "")
                                {
                                    elements.InsertObject(num3 + num2, new Text(content));
                                    num2++;
                                    content = "";
                                }
                                elements.InsertObject(num3 + num2, new Text("\x00ad"));
                                num2++;
                                content = "";
                                continue;
                            }
                            case '\t':
                            case '\n':
                            case '\r':
                            case ' ':
                            {
                                if (content != "")
                                {
                                    elements.InsertObject(num3 + num2, new Text(content));
                                    num2++;
                                    content = "";
                                }
                                elements.InsertObject(num3 + num2, new Text(" "));
                                num2++;
                                continue;
                            }
                        }
                        content = content + ch;
                    }
                    if (content != "")
                    {
                        elements.InsertObject(num3 + num2, new Text(content));
                        num2++;
                    }
                    elements.RemoveObjectAt(num3 + num2);
                    num2--;
                }
            }
        }

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
            Font parentFont = this.GetParentFont(formattedText);
            if (formattedText.font == null)
            {
                formattedText.Font = parentFont.Clone();
            }
            else if (parentFont != null)
            {
                base.FlattenFont(formattedText.font, parentFont);
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
            base.FlattenFont(hyperlink.font, this.GetParentFont(hyperlink));
        }
    }
}

