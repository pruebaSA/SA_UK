namespace MigraDoc.Rendering.Forms
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.IO;
    using MigraDoc.Rendering;
    using PdfSharp;
    using PdfSharp.Drawing;
    using PdfSharp.Forms;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    public class DocumentPreview : UserControl
    {
        private Container components;
        private string ddl;
        internal Color desktopColor = SystemColors.ControlDark;
        private MigraDoc.DocumentObjectModel.Document document;
        private int page;
        private Color pageColor = Color.GhostWhite;
        private PagePreview preview;
        internal XPrivateFontCollection privateFonts;
        private DocumentRenderer renderer;
        internal bool showPage = true;
        internal bool showScrollbars = true;
        internal int zoomPercen = 100;

        [Description("Occurs when the current page changed."), Category("Preview Properties")]
        public event PagePreviewEventHandler PageChanged;

        [Category("Preview Properties"), Description("Occurs when the zoom factor changed.")]
        public event PagePreviewEventHandler ZoomChanged;

        public DocumentPreview()
        {
            this.InitializeComponent();
            this.preview.ZoomChanged += new EventHandler(this.preview_ZoomChanged);
            this.preview.SetRenderEvent(new PagePreview.RenderEvent(this.RenderPage));
        }

        private void DdlUpdated()
        {
            if (this.ddl != null)
            {
                this.document = DdlReader.DocumentFromString(this.ddl);
                this.renderer = new DocumentRenderer(this.document);
                this.renderer.PrivateFonts = this.privateFonts;
                this.renderer.PrepareDocument();
                this.Page = 1;
                this.preview.Invalidate();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void FirstPage()
        {
            if (this.renderer != null)
            {
                this.Page = 1;
                this.preview.Invalidate();
                this.OnPageChanged(new EventArgs());
            }
        }

        private MigraDoc.Rendering.Forms.Zoom GetNewZoomFactor(int currentZoom, bool larger)
        {
            int[] numArray = new int[] { 
                10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 120, 140, 160, 180, 200, 250,
                300, 350, 400, 450, 500, 600, 700, 800
            };
            if ((currentZoom <= 10) && !larger)
            {
                return MigraDoc.Rendering.Forms.Zoom.Percent10;
            }
            if ((currentZoom >= 800) && larger)
            {
                return MigraDoc.Rendering.Forms.Zoom.Percent800;
            }
            if (larger)
            {
                for (int i = 0; i < numArray.Length; i++)
                {
                    if (currentZoom < numArray[i])
                    {
                        return (MigraDoc.Rendering.Forms.Zoom) numArray[i];
                    }
                }
            }
            else
            {
                for (int j = numArray.Length - 1; j >= 0; j--)
                {
                    if (currentZoom > numArray[j])
                    {
                        return (MigraDoc.Rendering.Forms.Zoom) numArray[j];
                    }
                }
            }
            return MigraDoc.Rendering.Forms.Zoom.Percent100;
        }

        private void InitializeComponent()
        {
            this.preview = new PagePreview();
            base.SuspendLayout();
            this.preview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.preview.DesktopColor = SystemColors.ControlDark;
            this.preview.Dock = DockStyle.Fill;
            this.preview.Location = new Point(0, 0);
            this.preview.Name = "preview";
            this.preview.PageColor = Color.GhostWhite;
            this.preview.PageSize = new Size(0x253, 0x34a);
            this.preview.Size = new Size(200, 200);
            this.preview.TabIndex = 0;
            this.preview.Zoom = PdfSharp.Forms.Zoom.FullPage;
            this.preview.ZoomPercent = 15;
            base.Controls.Add(this.preview);
            base.Name = "PagePreview";
            base.Size = new Size(200, 200);
            base.ResumeLayout(false);
        }

        public void LastPage()
        {
            if (this.renderer != null)
            {
                this.Page = this.PageCount;
                this.preview.Invalidate();
                this.OnPageChanged(new EventArgs());
            }
        }

        public void MakeLarger()
        {
            this.ZoomPercent = (int) this.GetNewZoomFactor(this.ZoomPercent, true);
        }

        public void MakeSmaller()
        {
            this.ZoomPercent = (int) this.GetNewZoomFactor(this.ZoomPercent, false);
        }

        public void NextPage()
        {
            if ((this.renderer != null) && (this.page < this.PageCount))
            {
                this.Page++;
                this.preview.Invalidate();
                this.OnPageChanged(new EventArgs());
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            int delta = e.Delta;
            if (delta > 0)
            {
                this.PrevPage();
            }
            else if (delta < 0)
            {
                this.NextPage();
            }
        }

        protected virtual void OnPageChanged(EventArgs e)
        {
            if (this.PageChanged != null)
            {
                this.PageChanged(this, e);
            }
        }

        protected virtual void OnZoomChanged(EventArgs e)
        {
            if (this.ZoomChanged != null)
            {
                this.ZoomChanged(this, e);
            }
        }

        private void preview_ZoomChanged(object sender, EventArgs e)
        {
            this.OnZoomChanged(e);
        }

        public void PrevPage()
        {
            if ((this.renderer != null) && (this.page > 1))
            {
                this.Page--;
            }
        }

        private void RenderPage(XGraphics gfx)
        {
            if ((this.renderer != null) && (this.renderer != null))
            {
                try
                {
                    this.renderer.RenderPage(gfx, this.page);
                }
                catch
                {
                }
            }
        }

        [Category("Preview Properties"), Description("Determines the style of the border."), DefaultValue(2)]
        public System.Windows.Forms.BorderStyle BorderStyle
        {
            get => 
                this.preview.BorderStyle;
            set
            {
                this.preview.BorderStyle = value;
            }
        }

        public string Ddl
        {
            get => 
                this.ddl;
            set
            {
                this.ddl = value;
                this.DdlUpdated();
            }
        }

        [Category("Preview Properties"), Description("The color of the desktop.")]
        public Color DesktopColor
        {
            get => 
                this.preview.DesktopColor;
            set
            {
                this.preview.DesktopColor = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Document Document
        {
            get => 
                this.document;
            set
            {
                if (value != null)
                {
                    this.document = value;
                    this.renderer = new DocumentRenderer(value);
                    this.renderer.PrepareDocument();
                    this.Page = 1;
                    this.preview.Invalidate();
                }
                else
                {
                    this.document = null;
                    this.renderer = null;
                    this.preview.Invalidate();
                }
            }
        }

        public int Page
        {
            get => 
                this.page;
            set
            {
                try
                {
                    if (this.preview != null)
                    {
                        if (this.page != value)
                        {
                            this.page = value;
                            PageInfo pageInfo = this.renderer.formattedDocument.GetPageInfo(this.page);
                            if (pageInfo.Orientation == PageOrientation.Portrait)
                            {
                                this.preview.PageSize = new Size((int) pageInfo.Width, (int) pageInfo.Height);
                            }
                            else
                            {
                                this.preview.PageSize = new Size((int) pageInfo.Height, (int) pageInfo.Width);
                            }
                            this.preview.Invalidate();
                            this.OnPageChanged(new EventArgs());
                        }
                    }
                    else
                    {
                        this.page = -1;
                    }
                }
                catch
                {
                }
            }
        }

        [Category("Preview Properties"), Description("The background color of the page.")]
        public Color PageColor
        {
            get => 
                this.preview.PageColor;
            set
            {
                this.preview.PageColor = value;
            }
        }

        public int PageCount
        {
            get
            {
                if (this.renderer != null)
                {
                    return this.renderer.FormattedDocument.PageCount;
                }
                return 0;
            }
        }

        [Category("Preview Properties"), Description("Determines the size (in points) of the page.")]
        public Size PageSize
        {
            get => 
                new Size((int) this.preview.PageSize.Width, (int) this.preview.PageSize.Height);
            set
            {
                this.preview.PageSize = value;
            }
        }

        public XPrivateFontCollection PrivateFonts
        {
            get => 
                this.privateFonts;
            set
            {
                this.privateFonts = value;
            }
        }

        public DocumentRenderer Renderer =>
            this.renderer;

        [DefaultValue(true), Description("Determines whether the page visible."), Category("Preview Properties")]
        public bool ShowPage
        {
            get => 
                this.preview.ShowPage;
            set
            {
                this.preview.ShowPage = value;
            }
        }

        [Description("Determines whether the scrollbars are visible."), Category("Preview Properties"), DefaultValue(true)]
        public bool ShowScrollbars
        {
            get => 
                this.preview.ShowScrollbars;
            set
            {
                this.preview.ShowScrollbars = value;
            }
        }

        [Category("Preview Properties"), DefaultValue(-3), Description("Determines the zoom of the page.")]
        public MigraDoc.Rendering.Forms.Zoom Zoom
        {
            get => 
                ((MigraDoc.Rendering.Forms.Zoom) this.preview.Zoom);
            set
            {
                if (this.preview.Zoom != ((PdfSharp.Forms.Zoom) ((int) value)))
                {
                    this.preview.Zoom = (PdfSharp.Forms.Zoom) value;
                    this.OnZoomChanged(new EventArgs());
                }
            }
        }

        [DefaultValue(-3), Description("Determines the zoom of the page."), Category("Preview Properties")]
        public int ZoomPercent
        {
            get => 
                this.preview.ZoomPercent;
            set
            {
                if (this.preview.ZoomPercent != value)
                {
                    this.preview.ZoomPercent = value;
                    this.OnZoomChanged(new EventArgs());
                }
            }
        }
    }
}

