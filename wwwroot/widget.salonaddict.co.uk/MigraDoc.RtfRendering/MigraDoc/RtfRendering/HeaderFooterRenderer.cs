namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;
    using System.Collections.Specialized;

    internal class HeaderFooterRenderer : RendererBase
    {
        private HeaderFooter headerFooter;
        private MigraDoc.DocumentObjectModel.PageSetup pageSetup;
        private HeaderFooterIndex renderAs;

        internal HeaderFooterRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.headerFooter = domObj as HeaderFooter;
        }

        private StringCollection GetHeaderFooterControls()
        {
            StringCollection strings = new StringCollection();
            bool flag = (bool) this.pageSetup.GetValue("DifferentFirstPageHeaderFooter", GV.GetNull);
            bool flag2 = (bool) this.pageSetup.GetValue("OddAndEvenPagesHeaderFooter", GV.GetNull);
            string str = this.headerFooter.IsHeader ? "header" : "footer";
            if (this.renderAs == HeaderFooterIndex.FirstPage)
            {
                if (flag)
                {
                    strings.Add(str + "f");
                }
                return strings;
            }
            if (this.renderAs == HeaderFooterIndex.EvenPage)
            {
                if (flag2)
                {
                    strings.Add(str + "l");
                }
                return strings;
            }
            if (this.renderAs == HeaderFooterIndex.Primary)
            {
                strings.Add(str + "r");
                if (!flag2)
                {
                    strings.Add(str + "l");
                }
            }
            return strings;
        }

        internal override void Render()
        {
            foreach (string str in this.GetHeaderFooterControls())
            {
                base.rtfWriter.StartContent();
                base.rtfWriter.WriteControl(str);
                foreach (DocumentObject obj2 in this.headerFooter.Elements)
                {
                    RendererBase base2 = RendererFactory.CreateRenderer(obj2, base.docRenderer);
                    if (base2 != null)
                    {
                        base2.Render();
                    }
                }
                base.rtfWriter.EndContent();
            }
        }

        internal MigraDoc.DocumentObjectModel.PageSetup PageSetup
        {
            set
            {
                this.pageSetup = value;
            }
        }

        internal HeaderFooterIndex RenderAs
        {
            set
            {
                this.renderAs = value;
            }
        }
    }
}

