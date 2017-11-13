namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using MigraDoc.DocumentObjectModel.Tables;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;
    using System.Reflection;

    public class DocumentElements : DocumentObjectCollection, IVisitable
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        public DocumentElements()
        {
        }

        internal DocumentElements(DocumentObject parent) : base(parent)
        {
        }

        public Barcode AddBarcode()
        {
            Barcode barcode = new Barcode();
            this.Add(barcode);
            return barcode;
        }

        public Chart AddChart()
        {
            Chart chart = new Chart {
                Type = ChartType.Line
            };
            this.Add(chart);
            return chart;
        }

        public Chart AddChart(ChartType type)
        {
            Chart chart = this.AddChart();
            chart.Type = type;
            return chart;
        }

        public Image AddImage(string name)
        {
            Image image = new Image {
                Name = name
            };
            this.Add(image);
            return image;
        }

        public Legend AddLegend()
        {
            Legend legend = new Legend();
            this.Add(legend);
            return legend;
        }

        public void AddPageBreak()
        {
            PageBreak @break = new PageBreak();
            this.Add(@break);
        }

        public Paragraph AddParagraph()
        {
            Paragraph paragraph = new Paragraph();
            this.Add(paragraph);
            return paragraph;
        }

        public Paragraph AddParagraph(string text)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.AddText(text);
            this.Add(paragraph);
            return paragraph;
        }

        public Paragraph AddParagraph(string text, string style)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.AddText(text);
            paragraph.Style = style;
            this.Add(paragraph);
            return paragraph;
        }

        public Table AddTable()
        {
            Table table = new Table();
            this.Add(table);
            return table;
        }

        public TextFrame AddTextFrame()
        {
            TextFrame frame = new TextFrame();
            this.Add(frame);
            return frame;
        }

        public DocumentElements Clone() => 
            ((DocumentElements) this.DeepCopy());

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitDocumentElements(this);
            foreach (DocumentObject obj2 in this)
            {
                if (obj2 is IVisitable)
                {
                    ((IVisitable) obj2).AcceptVisitor(visitor, visitChildren);
                }
            }
        }

        internal override void Serialize(Serializer serializer)
        {
            int count = base.Count;
            if ((count == 1) && (this[0] is Paragraph))
            {
                Paragraph paragraph = (Paragraph) this[0];
                if ((paragraph.Style == "") && paragraph.IsNull("Format"))
                {
                    paragraph.SerializeContentOnly = true;
                    paragraph.Serialize(serializer);
                    paragraph.SerializeContentOnly = false;
                    return;
                }
            }
            for (int i = 0; i < count; i++)
            {
                this[i].Serialize(serializer);
            }
        }

        public DocumentObject this[int index] =>
            base[index];

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(DocumentElements));
                }
                return meta;
            }
        }
    }
}

