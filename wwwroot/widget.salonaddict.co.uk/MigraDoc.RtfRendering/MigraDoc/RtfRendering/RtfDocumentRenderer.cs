namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;
    using System.Collections;
    using System.Drawing;
    using System.IO;
    using System.Text;

    public class RtfDocumentRenderer : RendererBase
    {
        private ArrayList colorList = new ArrayList();
        private MigraDoc.DocumentObjectModel.Document document;
        private ArrayList fontList = new ArrayList();
        private ArrayList listList = new ArrayList();
        private string workingDirectory;

        private void CollectTables(DocumentObject dom)
        {
            ValueDescriptorCollection valueDescriptors = Meta.GetMeta(dom).ValueDescriptors;
            int count = valueDescriptors.Count;
            for (int i = 0; i < count; i++)
            {
                ValueDescriptor descriptor = valueDescriptors[i];
                if (!descriptor.IsRefOnly && !descriptor.IsNull(dom))
                {
                    if (descriptor.ValueType == typeof(MigraDoc.DocumentObjectModel.Color))
                    {
                        MigraDoc.DocumentObjectModel.Color item = (MigraDoc.DocumentObjectModel.Color) descriptor.GetValue(dom, GV.ReadWrite);
                        item = item.GetMixedTransparencyColor();
                        if (!this.colorList.Contains(item))
                        {
                            this.colorList.Add(item);
                        }
                    }
                    else if (descriptor.ValueType == typeof(MigraDoc.DocumentObjectModel.Font))
                    {
                        MigraDoc.DocumentObjectModel.Font font = descriptor.GetValue(dom, GV.ReadWrite) as MigraDoc.DocumentObjectModel.Font;
                        if (!font.IsNull("Name") && !this.fontList.Contains(font.Name))
                        {
                            this.fontList.Add(font.Name);
                        }
                    }
                    else if (descriptor.ValueType == typeof(ListInfo))
                    {
                        ListInfo info = descriptor.GetValue(dom, GV.ReadWrite) as ListInfo;
                        if (!this.listList.Contains(info))
                        {
                            this.listList.Add(info);
                        }
                    }
                    if (typeof(DocumentObject).IsAssignableFrom(descriptor.ValueType))
                    {
                        this.CollectTables(descriptor.GetValue(dom, GV.ReadWrite) as DocumentObject);
                        if (typeof(DocumentObjectCollection).IsAssignableFrom(descriptor.ValueType))
                        {
                            DocumentObjectCollection objects = descriptor.GetValue(dom, GV.ReadWrite) as DocumentObjectCollection;
                            if (objects != null)
                            {
                                foreach (DocumentObject obj2 in objects)
                                {
                                    if (obj2 != null)
                                    {
                                        this.CollectTables(obj2);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal int GetColorIndex(MigraDoc.DocumentObjectModel.Color color)
        {
            MigraDoc.DocumentObjectModel.Color mixedTransparencyColor = color.GetMixedTransparencyColor();
            int index = this.colorList.IndexOf(mixedTransparencyColor);
            if (index < 0)
            {
                throw new ArgumentException("Color does not exist in this document's color table.", "color");
            }
            return index;
        }

        internal int GetFontIndex(string fontName)
        {
            if (!this.fontList.Contains(fontName))
            {
                throw new ArgumentException("Font does not exist in this document's font table.", "fontName");
            }
            return this.fontList.IndexOf(fontName);
        }

        internal int GetStyleIndex(string styleName) => 
            this.document.Styles.GetIndex(styleName);

        private void Prepare()
        {
            this.fontList.Clear();
            this.fontList.Add("Symbol");
            this.fontList.Add("Wingdings");
            this.fontList.Add("Courier New");
            this.colorList.Clear();
            this.colorList.Add(Colors.Black);
            this.listList.Clear();
            ListInfoRenderer.Clear();
            ListInfoOverrideRenderer.Clear();
            this.CollectTables(this.document);
        }

        internal override void Render()
        {
            throw new InvalidOperationException();
        }

        public void Render(MigraDoc.DocumentObjectModel.Document document, Stream stream, string workingDirectory)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            if (document.UseCmykColor)
            {
                throw new InvalidOperationException("Cannot create RTF document with CMYK colors.");
            }
            StreamWriter textWriter = null;
            try
            {
                textWriter = new StreamWriter(stream, Encoding.Default);
                this.document = document;
                base.docObject = document;
                this.workingDirectory = workingDirectory;
                base.rtfWriter = new MigraDoc.RtfRendering.RtfWriter(textWriter);
                this.WriteDocument();
            }
            finally
            {
                if (textWriter != null)
                {
                    textWriter.Flush();
                    textWriter.Close();
                }
            }
        }

        public void Render(MigraDoc.DocumentObjectModel.Document doc, string file, string workingDirectory)
        {
            StreamWriter textWriter = null;
            try
            {
                this.document = doc;
                base.docObject = doc;
                this.workingDirectory = workingDirectory;
                string path = file;
                if (workingDirectory != null)
                {
                    path = Path.Combine(workingDirectory, file);
                }
                textWriter = new StreamWriter(path, false, Encoding.Default);
                base.rtfWriter = new MigraDoc.RtfRendering.RtfWriter(textWriter);
                this.WriteDocument();
            }
            finally
            {
                if (textWriter != null)
                {
                    textWriter.Flush();
                    textWriter.Close();
                }
            }
        }

        private void RenderColorTable()
        {
            if (this.colorList.Count != 0)
            {
                base.rtfWriter.StartContent();
                base.rtfWriter.WriteControl("colortbl");
                foreach (object obj2 in this.colorList)
                {
                    MigraDoc.DocumentObjectModel.Color color = (MigraDoc.DocumentObjectModel.Color) obj2;
                    base.rtfWriter.WriteControl("red", (int) color.R);
                    base.rtfWriter.WriteControl("green", (int) color.G);
                    base.rtfWriter.WriteControl("blue", (int) color.B);
                    base.rtfWriter.WriteSeparator();
                }
                base.rtfWriter.EndContent();
            }
        }

        private void RenderDocumentArea()
        {
            this.RenderInfo();
            this.RenderDocumentFormat();
            this.RenderGlobalPorperties();
            foreach (Section section in this.document.Sections)
            {
                RendererFactory.CreateRenderer(section, this).Render();
            }
        }

        private void RenderDocumentFormat()
        {
            base.Translate("DefaultTabStop", "deftab");
            base.Translate("FootnoteNumberingRule", "ftn");
            base.Translate("FootnoteLocation", "ftn", RtfUnit.Undefined, "bj", false);
            base.Translate("FootnoteNumberStyle", "ftnn");
            base.Translate("FootnoteStartingNumber", "ftnstart");
        }

        private void RenderFontTable()
        {
            if (this.fontList.Count != 0)
            {
                base.rtfWriter.StartContent();
                base.rtfWriter.WriteControl("fonttbl");
                for (int i = 0; i < this.fontList.Count; i++)
                {
                    base.rtfWriter.StartContent();
                    string familyName = (string) this.fontList[i];
                    base.rtfWriter.WriteControl("f", i);
                    System.Drawing.Font font = new System.Drawing.Font(familyName, 12f);
                    base.rtfWriter.WriteControl("fcharset", font.GdiCharSet);
                    base.rtfWriter.WriteText(familyName);
                    base.rtfWriter.WriteSeparator();
                    base.rtfWriter.EndContent();
                }
                base.rtfWriter.EndContent();
            }
        }

        private void RenderGlobalPorperties()
        {
            base.rtfWriter.WriteControl("viewkind", 4);
            base.rtfWriter.WriteControl("uc", 1);
            base.rtfWriter.WriteControl("lnbrkrule");
            base.rtfWriter.WriteControl("fet", 0);
            base.rtfWriter.WriteControl("facingp");
            base.rtfWriter.WriteControl("htmautsp");
            Section first = this.document.Sections.First as Section;
            if (((first != null) && !first.PageSetup.IsNull("MirrorMargins")) && first.PageSetup.MirrorMargins)
            {
                base.rtfWriter.WriteControl("margmirror");
            }
        }

        private void RenderHeader()
        {
            base.rtfWriter.WriteControl("rtf", 1);
            base.rtfWriter.WriteControl("ansi");
            base.rtfWriter.WriteControl("ansicpg", 0x4e4);
            base.rtfWriter.WriteControl("deff", 0);
            this.RenderFontTable();
            this.RenderColorTable();
            this.RenderStyles();
            this.RenderListTable();
        }

        private void RenderInfo()
        {
            if (!this.document.IsNull("Info"))
            {
                base.rtfWriter.StartContent();
                base.rtfWriter.WriteControl("info");
                DocumentInfo info = this.document.Info;
                if (!info.IsNull("Title"))
                {
                    base.rtfWriter.StartContent();
                    base.rtfWriter.WriteControl("title");
                    base.rtfWriter.WriteText(info.Title);
                    base.rtfWriter.EndContent();
                }
                if (!info.IsNull("Subject"))
                {
                    base.rtfWriter.StartContent();
                    base.rtfWriter.WriteControl("subject");
                    base.rtfWriter.WriteText(info.Subject);
                    base.rtfWriter.EndContent();
                }
                if (!info.IsNull("Author"))
                {
                    base.rtfWriter.StartContent();
                    base.rtfWriter.WriteControl("author");
                    base.rtfWriter.WriteText(info.Author);
                    base.rtfWriter.EndContent();
                }
                if (!info.IsNull("Keywords"))
                {
                    base.rtfWriter.StartContent();
                    base.rtfWriter.WriteControl("keywords");
                    base.rtfWriter.WriteText(info.Keywords);
                    base.rtfWriter.EndContent();
                }
                base.rtfWriter.EndContent();
            }
        }

        private void RenderListTable()
        {
            if (this.listList.Count != 0)
            {
                base.rtfWriter.StartContent();
                base.rtfWriter.WriteControlWithStar("listtable");
                foreach (object obj2 in this.listList)
                {
                    ListInfo domObj = (ListInfo) obj2;
                    new ListInfoRenderer(domObj, this).Render();
                }
                base.rtfWriter.EndContent();
                base.rtfWriter.StartContent();
                base.rtfWriter.WriteControlWithStar("listoverridetable");
                foreach (object obj3 in this.listList)
                {
                    ListInfo info2 = (ListInfo) obj3;
                    new ListInfoOverrideRenderer(info2, this).Render();
                }
                base.rtfWriter.EndContent();
            }
        }

        internal void RenderSectionProperties()
        {
            base.Translate("FootnoteNumberingRule", "sftn");
            base.Translate("FootnoteLocation", "sftn", RtfUnit.Undefined, "bj", false);
            base.Translate("FootnoteNumberStyle", "sftnn");
            base.Translate("FootnoteStartingNumber", "sftnstart");
        }

        private void RenderStyles()
        {
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("stylesheet");
            foreach (Style style in this.document.Styles)
            {
                RendererFactory.CreateRenderer(style, this).Render();
            }
            base.rtfWriter.EndContent();
        }

        public string RenderToString(MigraDoc.DocumentObjectModel.Document document, string workingDirectory)
        {
            string str;
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            if (document.UseCmykColor)
            {
                throw new InvalidOperationException("Cannot create RTF document with CMYK colors.");
            }
            this.document = document;
            base.docObject = document;
            this.workingDirectory = workingDirectory;
            StringWriter textWriter = null;
            try
            {
                textWriter = new StringWriter();
                base.rtfWriter = new MigraDoc.RtfRendering.RtfWriter(textWriter);
                this.WriteDocument();
                textWriter.Flush();
                str = textWriter.GetStringBuilder().ToString();
            }
            finally
            {
                if (textWriter != null)
                {
                    textWriter.Close();
                }
            }
            return str;
        }

        private void WriteDocument()
        {
            new RtfFlattenVisitor().Visit(this.document);
            this.Prepare();
            base.rtfWriter.StartContent();
            this.RenderHeader();
            this.RenderDocumentArea();
            base.rtfWriter.EndContent();
        }

        internal MigraDoc.DocumentObjectModel.Document Document =>
            this.document;

        internal MigraDoc.RtfRendering.RtfWriter RtfWriter =>
            base.rtfWriter;

        internal string WorkingDirectory =>
            this.workingDirectory;
    }
}

