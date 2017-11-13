namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;

    [DefaultProperty("Value")]
    public class ToolStripProgressBar : ToolStripControlHost
    {
        internal static readonly object EventRightToLeftLayoutChanged = new object();

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event KeyEventHandler KeyDown
        {
            add
            {
                base.KeyDown += value;
            }
            remove
            {
                base.KeyDown -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event KeyPressEventHandler KeyPress
        {
            add
            {
                base.KeyPress += value;
            }
            remove
            {
                base.KeyPress -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event KeyEventHandler KeyUp
        {
            add
            {
                base.KeyUp += value;
            }
            remove
            {
                base.KeyUp -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event EventHandler LocationChanged
        {
            add
            {
                base.LocationChanged += value;
            }
            remove
            {
                base.LocationChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler OwnerChanged
        {
            add
            {
                base.OwnerChanged += value;
            }
            remove
            {
                base.OwnerChanged -= value;
            }
        }

        [System.Windows.Forms.SRDescription("ControlOnRightToLeftLayoutChangedDescr"), System.Windows.Forms.SRCategory("CatPropertyChanged")]
        public event EventHandler RightToLeftLayoutChanged
        {
            add
            {
                base.Events.AddHandler(EventRightToLeftLayoutChanged, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventRightToLeftLayoutChanged, value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event EventHandler TextChanged
        {
            add
            {
                base.TextChanged += value;
            }
            remove
            {
                base.TextChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event EventHandler Validated
        {
            add
            {
                base.Validated += value;
            }
            remove
            {
                base.Validated -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event CancelEventHandler Validating
        {
            add
            {
                base.Validating += value;
            }
            remove
            {
                base.Validating -= value;
            }
        }

        public ToolStripProgressBar() : base(CreateControlInstance())
        {
        }

        public ToolStripProgressBar(string name) : this()
        {
            base.Name = name;
        }

        private static Control CreateControlInstance() => 
            new System.Windows.Forms.ProgressBar { Size = new Size(100, 15) };

        private void HandleRightToLeftLayoutChanged(object sender, EventArgs e)
        {
            this.OnRightToLeftLayoutChanged(e);
        }

        public void Increment(int value)
        {
            this.ProgressBar.Increment(value);
        }

        protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
        {
            base.RaiseEvent(EventRightToLeftLayoutChanged, e);
        }

        protected override void OnSubscribeControlEvents(Control control)
        {
            System.Windows.Forms.ProgressBar bar = control as System.Windows.Forms.ProgressBar;
            if (bar != null)
            {
                bar.RightToLeftLayoutChanged += new EventHandler(this.HandleRightToLeftLayoutChanged);
            }
            base.OnSubscribeControlEvents(control);
        }

        protected override void OnUnsubscribeControlEvents(Control control)
        {
            System.Windows.Forms.ProgressBar bar = control as System.Windows.Forms.ProgressBar;
            if (bar != null)
            {
                bar.RightToLeftLayoutChanged -= new EventHandler(this.HandleRightToLeftLayoutChanged);
            }
            base.OnUnsubscribeControlEvents(control);
        }

        public void PerformStep()
        {
            this.ProgressBar.PerformStep();
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage
        {
            get => 
                base.BackgroundImage;
            set
            {
                base.BackgroundImage = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout
        {
            get => 
                base.BackgroundImageLayout;
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        protected internal override Padding DefaultMargin
        {
            get
            {
                if ((base.Owner != null) && (base.Owner is StatusStrip))
                {
                    return new Padding(1, 3, 1, 3);
                }
                return new Padding(1, 2, 1, 1);
            }
        }

        protected override Size DefaultSize =>
            new Size(100, 15);

        [System.Windows.Forms.SRDescription("ProgressBarMarqueeAnimationSpeed"), System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(100)]
        public int MarqueeAnimationSpeed
        {
            get => 
                this.ProgressBar.MarqueeAnimationSpeed;
            set
            {
                this.ProgressBar.MarqueeAnimationSpeed = value;
            }
        }

        [RefreshProperties(RefreshProperties.Repaint), DefaultValue(100), System.Windows.Forms.SRDescription("ProgressBarMaximumDescr"), System.Windows.Forms.SRCategory("CatBehavior")]
        public int Maximum
        {
            get => 
                this.ProgressBar.Maximum;
            set
            {
                this.ProgressBar.Maximum = value;
            }
        }

        [System.Windows.Forms.SRDescription("ProgressBarMinimumDescr"), System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(0), RefreshProperties(RefreshProperties.Repaint)]
        public int Minimum
        {
            get => 
                this.ProgressBar.Minimum;
            set
            {
                this.ProgressBar.Minimum = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Windows.Forms.ProgressBar ProgressBar =>
            (base.Control as System.Windows.Forms.ProgressBar);

        [Localizable(true), System.Windows.Forms.SRCategory("CatAppearance"), DefaultValue(false), System.Windows.Forms.SRDescription("ControlRightToLeftLayoutDescr")]
        public virtual bool RightToLeftLayout
        {
            get => 
                this.ProgressBar.RightToLeftLayout;
            set
            {
                this.ProgressBar.RightToLeftLayout = value;
            }
        }

        [System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(10), System.Windows.Forms.SRDescription("ProgressBarStepDescr")]
        public int Step
        {
            get => 
                this.ProgressBar.Step;
            set
            {
                this.ProgressBar.Step = value;
            }
        }

        [System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(0), System.Windows.Forms.SRDescription("ProgressBarStyleDescr")]
        public ProgressBarStyle Style
        {
            get => 
                this.ProgressBar.Style;
            set
            {
                this.ProgressBar.Style = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override string Text
        {
            get => 
                base.Control.Text;
            set
            {
                base.Control.Text = value;
            }
        }

        [DefaultValue(0), System.Windows.Forms.SRCategory("CatBehavior"), Bindable(true), System.Windows.Forms.SRDescription("ProgressBarValueDescr")]
        public int Value
        {
            get => 
                this.ProgressBar.Value;
            set
            {
                this.ProgressBar.Value = value;
            }
        }
    }
}

