namespace PdfSharp.Forms
{
    using PdfSharp.Drawing;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    public class PagePreview : UserControl
    {
        private System.Windows.Forms.BorderStyle borderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        private readonly PagePreviewCanvas canvas;
        private Container components;
        internal Color desktopColor = SystemColors.ControlDark;
        private readonly HScrollBar hScrollBar;
        private Color pageColor = Color.GhostWhite;
        private SizeF pageSize = PageSizeConverter.ToSize(PdfSharp.PageSize.A4).ToSizeF();
        private Point posOffset;
        private readonly RectangleF printableArea;
        private RenderEvent renderEvent;
        internal bool showPage = true;
        private bool showScrollbars = true;
        private Size virtualCanvas;
        private Rectangle virtualPage;
        private readonly VScrollBar vScrollBar;
        private PdfSharp.Forms.Zoom zoom;
        private int zoomPercent;

        public event EventHandler ZoomChanged;

        public PagePreview()
        {
            this.canvas = new PagePreviewCanvas(this);
            base.Controls.Add(this.canvas);
            this.hScrollBar = new HScrollBar();
            this.hScrollBar.Visible = this.showScrollbars;
            this.hScrollBar.Scroll += new ScrollEventHandler(this.OnScroll);
            this.hScrollBar.ValueChanged += new EventHandler(this.OnValueChanged);
            base.Controls.Add(this.hScrollBar);
            this.vScrollBar = new VScrollBar();
            this.vScrollBar.Visible = this.showScrollbars;
            this.vScrollBar.Scroll += new ScrollEventHandler(this.OnScroll);
            this.vScrollBar.ValueChanged += new EventHandler(this.OnValueChanged);
            base.Controls.Add(this.vScrollBar);
            this.InitializeComponent();
            this.zoom = PdfSharp.Forms.Zoom.FullPage;
            this.printableArea = new RectangleF();
            this.printableArea.GetType();
            this.posOffset = new Point();
            this.virtualPage = new Rectangle();
        }

        internal void CalculatePreviewDimension()
        {
            bool flag;
            this.CalculatePreviewDimension(out flag);
        }

        internal void CalculatePreviewDimension(out bool zoomChanged)
        {
            Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr hdc = graphics.GetHdc();
            DeviceInfos infos = DeviceInfos.GetInfos(hdc);
            graphics.ReleaseHdc(hdc);
            graphics.Dispose();
            int logicalDpiX = infos.LogicalDpiX;
            int logicalDpiY = infos.LogicalDpiY;
            Rectangle clientRectangle = this.canvas.ClientRectangle;
            PdfSharp.Forms.Zoom zoom = this.zoom;
            int zoomPercent = this.zoomPercent;
            switch (this.zoom)
            {
                case PdfSharp.Forms.Zoom.OriginalSize:
                    this.zoomPercent = (int) (0.5 + (200f / (infos.ScaleX + infos.ScaleY)));
                    this.zoomPercent = (int) (0.5 + (100f / infos.ScaleX));
                    break;

                case PdfSharp.Forms.Zoom.FullPage:
                {
                    int num4 = (int) ((7200f * (clientRectangle.Width - 6)) / (this.pageSize.Width * logicalDpiX));
                    int num5 = (int) ((7200f * (clientRectangle.Height - 6)) / (this.pageSize.Height * logicalDpiY));
                    this.zoomPercent = Math.Min(num4, num5);
                    break;
                }
                case PdfSharp.Forms.Zoom.TextFit:
                case PdfSharp.Forms.Zoom.BestFit:
                    this.zoomPercent = (int) ((7200f * (clientRectangle.Width - 6)) / (this.pageSize.Width * logicalDpiX));
                    break;

                default:
                    this.zoomPercent = (int) this.zoom;
                    break;
            }
            this.zoomPercent = Math.Max(Math.Min(this.zoomPercent, 800), 10);
            if (this.zoom > ~PdfSharp.Forms.Zoom.BestFit)
            {
                this.zoom = (PdfSharp.Forms.Zoom) this.zoomPercent;
            }
            this.virtualPage.X = 2;
            this.virtualPage.Y = 2;
            this.virtualPage.Width = (int) (((this.pageSize.Width * logicalDpiX) * this.zoomPercent) / 7200f);
            this.virtualPage.Height = (int) (((this.pageSize.Height * logicalDpiY) * this.zoomPercent) / 7200f);
            this.virtualCanvas = new Size(this.virtualPage.Width + 6, this.virtualPage.Height + 6);
            if (this.virtualCanvas.Width < clientRectangle.Width)
            {
                this.virtualCanvas.Width = clientRectangle.Width;
                this.virtualPage.X = 2 + (((clientRectangle.Width - 6) - this.virtualPage.Width) / 2);
            }
            if (this.virtualCanvas.Height < clientRectangle.Height)
            {
                this.virtualCanvas.Height = clientRectangle.Height;
                this.virtualPage.Y = 2 + (((clientRectangle.Height - 6) - this.virtualPage.Height) / 2);
            }
            zoomChanged = (zoom != this.zoom) || (zoomPercent != this.zoomPercent);
            if (zoomChanged)
            {
                this.OnZoomChanged(new EventArgs());
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

        [Conditional("DEBUG")]
        private void DrawDash(Graphics gfx, Rectangle rect)
        {
            Pen pen = new Pen(Color.GreenYellow, 1f) {
                DashStyle = DashStyle.Dash
            };
            gfx.DrawRectangle(pen, rect);
        }

        private void InitializeComponent()
        {
            base.Name = "PagePreview";
            base.Size = new Size(0xe4, 0xfc);
        }

        private void LayoutChildren()
        {
            base.Invalidate();
            Rectangle clientRectangle = base.ClientRectangle;
            switch (this.borderStyle)
            {
                case System.Windows.Forms.BorderStyle.FixedSingle:
                    clientRectangle.Inflate(-1, -1);
                    break;

                case System.Windows.Forms.BorderStyle.Fixed3D:
                    clientRectangle.Inflate(-2, -2);
                    break;
            }
            int x = clientRectangle.X;
            int y = clientRectangle.Y;
            int width = clientRectangle.Width;
            int height = clientRectangle.Height;
            int num5 = 0;
            int num6 = 0;
            if ((this.showScrollbars && (this.vScrollBar != null)) && (this.hScrollBar != null))
            {
                num5 = this.vScrollBar.Width;
                num6 = this.hScrollBar.Height;
                this.vScrollBar.Location = new Point((x + width) - num5, y);
                this.vScrollBar.Size = new Size(num5, height - num6);
                this.hScrollBar.Location = new Point(x, (y + height) - num6);
                this.hScrollBar.Size = new Size(width - num5, num6);
            }
            if (this.canvas != null)
            {
                this.canvas.Location = new Point(x, y);
                this.canvas.Size = new Size(width - num5, height - num6);
            }
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
            this.canvas.Invalidate();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            this.LayoutChildren();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Rectangle clientRectangle = base.ClientRectangle;
            int num = 0;
            switch (this.borderStyle)
            {
                case System.Windows.Forms.BorderStyle.FixedSingle:
                    graphics.DrawRectangle(SystemPens.WindowFrame, clientRectangle.X, clientRectangle.Y, clientRectangle.Width - 1, clientRectangle.Height - 1);
                    num = 1;
                    break;

                case System.Windows.Forms.BorderStyle.Fixed3D:
                    ControlPaint.DrawBorder3D(graphics, clientRectangle, Border3DStyle.Sunken);
                    num = 2;
                    break;
            }
            if (this.showScrollbars)
            {
                int verticalScrollBarWidth = SystemInformation.VerticalScrollBarWidth;
                int horizontalScrollBarHeight = SystemInformation.HorizontalScrollBarHeight;
                graphics.FillRectangle(new SolidBrush(this.BackColor), (clientRectangle.Width - verticalScrollBarWidth) - num, (clientRectangle.Height - horizontalScrollBarHeight) - num, verticalScrollBarWidth, horizontalScrollBarHeight);
            }
        }

        private void OnScroll(object obj, ScrollEventArgs e)
        {
            ScrollBar bar = obj as ScrollBar;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.CalculatePreviewDimension();
            this.SetScrollBarRange();
        }

        private void OnValueChanged(object obj, EventArgs e)
        {
            ScrollBar bar = obj as ScrollBar;
            if (bar != null)
            {
                if (bar == this.hScrollBar)
                {
                    this.posOffset.X = bar.Value;
                }
                else if (bar == this.vScrollBar)
                {
                    this.posOffset.Y = bar.Value;
                }
            }
            this.canvas.Invalidate();
        }

        protected virtual void OnZoomChanged(EventArgs e)
        {
            if (this.ZoomChanged != null)
            {
                this.ZoomChanged(this, e);
            }
        }

        internal void PaintBackground(Graphics gfx)
        {
            gfx.SmoothingMode = SmoothingMode.None;
            gfx.TranslateTransform((float) -this.posOffset.X, (float) -this.posOffset.Y);
            gfx.SetClip(new Rectangle(this.virtualPage.X, this.virtualPage.Y, this.virtualPage.Width + 3, this.virtualPage.Height + 3), CombineMode.Exclude);
            gfx.SetClip(new Rectangle((this.virtualPage.X + this.virtualPage.Width) + 1, this.virtualPage.Y, 2, 2), CombineMode.Union);
            gfx.SetClip(new Rectangle(this.virtualPage.X, (this.virtualPage.Y + this.virtualPage.Height) + 1, 2, 2), CombineMode.Union);
            gfx.Clear(this.desktopColor);
            gfx.ResetClip();
            SolidBrush brush = new SolidBrush(this.pageColor);
            gfx.FillRectangle(brush, (int) (this.virtualPage.X + 1), (int) (this.virtualPage.Y + 1), (int) (this.virtualPage.Width - 1), (int) (this.virtualPage.Height - 1));
            Pen windowText = SystemPens.WindowText;
            Brush controlDarkDark = SystemBrushes.ControlDarkDark;
            gfx.DrawRectangle(windowText, this.virtualPage);
            gfx.FillRectangle(controlDarkDark, (this.virtualPage.X + this.virtualPage.Width) + 1, this.virtualPage.Y + 2, 2, this.virtualPage.Height + 1);
            gfx.FillRectangle(controlDarkDark, this.virtualPage.X + 2, (this.virtualPage.Y + this.virtualPage.Height) + 1, this.virtualPage.Width + 1, 2);
        }

        internal bool RenderPage(Graphics gfx)
        {
            gfx.TranslateTransform((float) -this.posOffset.X, (float) -this.posOffset.Y);
            gfx.SetClip(new Rectangle(this.virtualPage.X + 1, this.virtualPage.Y + 1, this.virtualPage.Width - 1, this.virtualPage.Height - 1));
            float scaleX = ((float) this.virtualPage.Width) / this.pageSize.Width;
            float scaleY = ((float) this.virtualPage.Height) / this.pageSize.Height;
            Matrix matrix = new Matrix();
            matrix.Translate((float) this.virtualPage.X, (float) this.virtualPage.Y);
            matrix.Translate((float) -this.posOffset.X, (float) -this.posOffset.Y);
            matrix.Scale(scaleX, scaleY);
            gfx.Transform = matrix;
            if (this.renderEvent != null)
            {
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                XGraphics graphics = XGraphics.FromGraphics(gfx, new XSize((double) this.pageSize.Width, (double) this.pageSize.Height));
                try
                {
                    this.renderEvent(graphics);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Exception");
                }
            }
            return true;
        }

        public void SetRenderEvent(RenderEvent renderEvent)
        {
            this.renderEvent = renderEvent;
            base.Invalidate();
        }

        private void SetScrollBarRange()
        {
            Size size = this.canvas.ClientRectangle.Size;
            int num = this.virtualCanvas.Width - size.Width;
            int num2 = this.virtualCanvas.Height - size.Height;
            if (this.ShowScrollbars && (this.hScrollBar != null))
            {
                if (this.posOffset.X > num)
                {
                    this.hScrollBar.Value = this.posOffset.X = num;
                }
                if (num > 0)
                {
                    this.hScrollBar.Minimum = 0;
                    this.hScrollBar.Maximum = this.virtualCanvas.Width;
                    this.hScrollBar.SmallChange = size.Width / 10;
                    this.hScrollBar.LargeChange = size.Width;
                    this.hScrollBar.Enabled = true;
                }
                else
                {
                    this.hScrollBar.Minimum = 0;
                    this.hScrollBar.Maximum = 0;
                    this.hScrollBar.Enabled = false;
                }
            }
            if (this.ShowScrollbars && (this.vScrollBar != null))
            {
                if (this.posOffset.Y > num2)
                {
                    this.vScrollBar.Value = this.posOffset.Y = num2;
                }
                if (num2 > 0)
                {
                    this.vScrollBar.Minimum = 0;
                    this.vScrollBar.Maximum = this.virtualCanvas.Height;
                    this.vScrollBar.SmallChange = size.Height / 10;
                    this.vScrollBar.LargeChange = size.Height;
                    this.vScrollBar.Enabled = true;
                }
                else
                {
                    this.vScrollBar.Minimum = 0;
                    this.vScrollBar.Maximum = 0;
                    this.vScrollBar.Enabled = false;
                }
            }
        }

        [DefaultValue(2), Description("Determines the style of the border."), Category("Preview Properties")]
        public System.Windows.Forms.BorderStyle BorderStyle
        {
            get => 
                this.borderStyle;
            set
            {
                if (!Enum.IsDefined(typeof(System.Windows.Forms.BorderStyle), value))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(System.Windows.Forms.BorderStyle));
                }
                if (value != this.borderStyle)
                {
                    this.borderStyle = value;
                    this.LayoutChildren();
                }
            }
        }

        [Category("Preview Properties"), Description("The color of the desktop.")]
        public Color DesktopColor
        {
            get => 
                this.desktopColor;
            set
            {
                if (value != this.desktopColor)
                {
                    this.desktopColor = value;
                    base.Invalidate();
                }
            }
        }

        [Description("The background color of the page."), Category("Preview Properties")]
        public Color PageColor
        {
            get => 
                this.pageColor;
            set
            {
                if (value != this.pageColor)
                {
                    this.pageColor = value;
                    base.Invalidate();
                }
            }
        }

        [Description("Determines the size (in points) of the page."), Category("Preview Properties")]
        public XSize PageSize
        {
            get => 
                new XSize((double) ((int) this.pageSize.Width), (double) ((int) this.pageSize.Height));
            set
            {
                this.pageSize = new SizeF((float) value.Width, (float) value.Height);
                this.CalculatePreviewDimension();
                base.Invalidate();
            }
        }

        public Size PageSizeF
        {
            get
            {
                int width = Convert.ToInt32(this.pageSize.Width);
                return new Size(width, Convert.ToInt32(this.pageSize.Height));
            }
            set
            {
                this.pageSize = (SizeF) value;
                this.CalculatePreviewDimension();
                base.Invalidate();
            }
        }

        [Category("Preview Properties"), DefaultValue(true), Description("Determines whether the page visible.")]
        public bool ShowPage
        {
            get => 
                this.showPage;
            set
            {
                if (value != this.showPage)
                {
                    this.showPage = value;
                    this.canvas.Invalidate();
                }
            }
        }

        [DefaultValue(true), Category("Preview Properties"), Description("Determines whether the scrollbars are visible.")]
        public bool ShowScrollbars
        {
            get => 
                this.showScrollbars;
            set
            {
                if (value != this.showScrollbars)
                {
                    this.showScrollbars = value;
                    this.hScrollBar.Visible = value;
                    this.vScrollBar.Visible = value;
                    this.LayoutChildren();
                }
            }
        }

        [Description("Determines the zoom of the page."), Category("Preview Properties"), DefaultValue(-3)]
        public PdfSharp.Forms.Zoom Zoom
        {
            get => 
                this.zoom;
            set
            {
                if (((value < PdfSharp.Forms.Zoom.Mininum) || (value > PdfSharp.Forms.Zoom.Maximum)) && !Enum.IsDefined(typeof(PdfSharp.Forms.Zoom), value))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(PdfSharp.Forms.Zoom));
                }
                if (value != this.zoom)
                {
                    this.zoom = value;
                    this.CalculatePreviewDimension();
                    this.SetScrollBarRange();
                    this.canvas.Invalidate();
                }
            }
        }

        public int ZoomPercent
        {
            get => 
                this.zoomPercent;
            set
            {
                if ((value < 10) || (value > 800))
                {
                    throw new ArgumentOutOfRangeException("value", value, $"Value must between {10} and {800}.");
                }
                if (value != this.zoomPercent)
                {
                    this.zoom = (PdfSharp.Forms.Zoom) value;
                    this.zoomPercent = value;
                    this.CalculatePreviewDimension();
                    this.SetScrollBarRange();
                    this.canvas.Invalidate();
                }
            }
        }

        public delegate void RenderEvent(XGraphics gfx);
    }
}

