﻿namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Security.Permissions;
    using System.Windows.Forms.Layout;

    [Designer("System.Windows.Forms.Design.ToolStripMenuItemDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultProperty("DropDownItems")]
    public abstract class ToolStripDropDownItem : ToolStripItem
    {
        private ToolStripDropDown dropDown;
        private static readonly object EventDropDownClosed = new object();
        private static readonly object EventDropDownHide = new object();
        private static readonly object EventDropDownItemClicked = new object();
        private static readonly object EventDropDownOpened = new object();
        private static readonly object EventDropDownShow = new object();
        private ToolStripDropDownDirection toolStripDropDownDirection;

        [System.Windows.Forms.SRDescription("ToolStripDropDownClosedDecr"), System.Windows.Forms.SRCategory("CatAction")]
        public event EventHandler DropDownClosed
        {
            add
            {
                base.Events.AddHandler(EventDropDownClosed, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventDropDownClosed, value);
            }
        }

        [System.Windows.Forms.SRCategory("CatAction")]
        public event ToolStripItemClickedEventHandler DropDownItemClicked
        {
            add
            {
                base.Events.AddHandler(EventDropDownItemClicked, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventDropDownItemClicked, value);
            }
        }

        [System.Windows.Forms.SRCategory("CatAction"), System.Windows.Forms.SRDescription("ToolStripDropDownOpenedDescr")]
        public event EventHandler DropDownOpened
        {
            add
            {
                base.Events.AddHandler(EventDropDownOpened, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventDropDownOpened, value);
            }
        }

        [System.Windows.Forms.SRCategory("CatAction"), System.Windows.Forms.SRDescription("ToolStripDropDownOpeningDescr")]
        public event EventHandler DropDownOpening
        {
            add
            {
                base.Events.AddHandler(EventDropDownShow, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventDropDownShow, value);
            }
        }

        protected ToolStripDropDownItem()
        {
            this.toolStripDropDownDirection = ToolStripDropDownDirection.Default;
        }

        protected ToolStripDropDownItem(string text, Image image, EventHandler onClick) : base(text, image, onClick)
        {
            this.toolStripDropDownDirection = ToolStripDropDownDirection.Default;
        }

        protected ToolStripDropDownItem(string text, Image image, params ToolStripItem[] dropDownItems) : this(text, image, (EventHandler) null)
        {
            if (dropDownItems != null)
            {
                this.DropDownItems.AddRange(dropDownItems);
            }
        }

        protected ToolStripDropDownItem(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
        {
            this.toolStripDropDownDirection = ToolStripDropDownDirection.Default;
        }

        internal virtual void AutoHide(ToolStripItem otherItemBeingSelected)
        {
            this.HideDropDown();
        }

        protected override AccessibleObject CreateAccessibilityInstance() => 
            new ToolStripDropDownItemAccessibleObject(this);

        protected virtual ToolStripDropDown CreateDefaultDropDown() => 
            new ToolStripDropDown(this, true);

        protected override void Dispose(bool disposing)
        {
            if (this.dropDown != null)
            {
                this.dropDown.Opened -= new EventHandler(this.DropDown_Opened);
                this.dropDown.Closed -= new ToolStripDropDownClosedEventHandler(this.DropDown_Closed);
                this.dropDown.ItemClicked -= new ToolStripItemClickedEventHandler(this.DropDown_ItemClicked);
                if (disposing && this.dropDown.IsAutoGenerated)
                {
                    this.dropDown.Dispose();
                    this.dropDown = null;
                }
            }
            base.Dispose(disposing);
        }

        private void DropDown_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this.OnDropDownClosed(EventArgs.Empty);
        }

        private void DropDown_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.OnDropDownItemClicked(e);
        }

        private void DropDown_Opened(object sender, EventArgs e)
        {
            this.OnDropDownOpened(EventArgs.Empty);
        }

        private Rectangle DropDownDirectionToDropDownBounds(ToolStripDropDownDirection dropDownDirection, Rectangle dropDownBounds)
        {
            Point empty = Point.Empty;
            switch (dropDownDirection)
            {
                case ToolStripDropDownDirection.AboveLeft:
                    empty.X = -dropDownBounds.Width + base.Width;
                    empty.Y = -dropDownBounds.Height + 1;
                    break;

                case ToolStripDropDownDirection.AboveRight:
                    empty.Y = -dropDownBounds.Height + 1;
                    break;

                case ToolStripDropDownDirection.BelowLeft:
                    empty.X = -dropDownBounds.Width + base.Width;
                    empty.Y = base.Height - 1;
                    break;

                case ToolStripDropDownDirection.BelowRight:
                    empty.Y = base.Height - 1;
                    break;

                case ToolStripDropDownDirection.Left:
                    empty.X = -dropDownBounds.Width;
                    break;

                case ToolStripDropDownDirection.Right:
                    empty.X = base.Width;
                    if (!base.IsOnDropDown)
                    {
                        empty.X--;
                    }
                    break;
            }
            Point point2 = base.TranslatePoint(Point.Empty, ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ScreenCoords);
            dropDownBounds.Location = new Point(point2.X + empty.X, point2.Y + empty.Y);
            dropDownBounds = WindowsFormsUtils.ConstrainToScreenWorkingAreaBounds(dropDownBounds);
            return dropDownBounds;
        }

        private Rectangle GetDropDownBounds(ToolStripDropDownDirection dropDownDirection)
        {
            Rectangle dropDownBounds = new Rectangle(Point.Empty, this.DropDown.GetSuggestedSize());
            dropDownBounds = this.DropDownDirectionToDropDownBounds(dropDownDirection, dropDownBounds);
            Rectangle b = new Rectangle(base.TranslatePoint(Point.Empty, ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ScreenCoords), this.Size);
            if (Rectangle.Intersect(dropDownBounds, b).Height > 1)
            {
                bool flag = this.RightToLeft == RightToLeft.Yes;
                if (Rectangle.Intersect(dropDownBounds, b).Width > 1)
                {
                    dropDownBounds = this.DropDownDirectionToDropDownBounds(!flag ? ToolStripDropDownDirection.Right : ToolStripDropDownDirection.Left, dropDownBounds);
                }
                if (Rectangle.Intersect(dropDownBounds, b).Width > 1)
                {
                    dropDownBounds = this.DropDownDirectionToDropDownBounds(!flag ? ToolStripDropDownDirection.Left : ToolStripDropDownDirection.Right, dropDownBounds);
                }
            }
            return dropDownBounds;
        }

        public void HideDropDown()
        {
            this.OnDropDownHide(EventArgs.Empty);
            if ((this.dropDown != null) && this.dropDown.Visible)
            {
                this.DropDown.Visible = false;
            }
        }

        protected override void OnBoundsChanged()
        {
            base.OnBoundsChanged();
            if ((this.dropDown != null) && this.dropDown.Visible)
            {
                this.dropDown.Bounds = this.GetDropDownBounds(this.DropDownDirection);
            }
        }

        protected internal virtual void OnDropDownClosed(EventArgs e)
        {
            base.Invalidate();
            if (this.DropDown.OwnerItem == this)
            {
                EventHandler handler = (EventHandler) base.Events[EventDropDownClosed];
                if (handler != null)
                {
                    handler(this, e);
                }
                if (!this.DropDown.IsAutoGenerated)
                {
                    this.DropDown.OwnerItem = null;
                }
            }
        }

        protected virtual void OnDropDownHide(EventArgs e)
        {
            base.Invalidate();
            EventHandler handler = (EventHandler) base.Events[EventDropDownHide];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected internal virtual void OnDropDownItemClicked(ToolStripItemClickedEventArgs e)
        {
            if (this.DropDown.OwnerItem == this)
            {
                ToolStripItemClickedEventHandler handler = (ToolStripItemClickedEventHandler) base.Events[EventDropDownItemClicked];
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }

        protected internal virtual void OnDropDownOpened(EventArgs e)
        {
            if (this.DropDown.OwnerItem == this)
            {
                EventHandler handler = (EventHandler) base.Events[EventDropDownOpened];
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }

        protected virtual void OnDropDownShow(EventArgs e)
        {
            EventHandler handler = (EventHandler) base.Events[EventDropDownShow];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (this.dropDown != null)
            {
                this.dropDown.OnOwnerItemFontChanged(EventArgs.Empty);
            }
        }

        internal override void OnImageScalingSizeChanged(EventArgs e)
        {
            base.OnImageScalingSizeChanged(e);
            if (this.HasDropDown && this.DropDown.IsAutoGenerated)
            {
                this.DropDown.DoLayoutIfHandleCreated(new ToolStripItemEventArgs(this));
            }
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            if (this.HasDropDownItems)
            {
                if (this.DropDown.Visible)
                {
                    LayoutTransaction.DoLayout(this.DropDown, this, PropertyNames.RightToLeft);
                }
                else
                {
                    CommonProperties.xClearPreferredSizeCache(this.DropDown);
                    this.DropDown.LayoutRequired = true;
                }
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected internal override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            if (this.HasDropDownItems)
            {
                return this.DropDown.ProcessCmdKeyInternal(ref m, keyData);
            }
            return base.ProcessCmdKey(ref m, keyData);
        }

        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected internal override bool ProcessDialogKey(Keys keyData)
        {
            Keys keys = keyData & Keys.KeyCode;
            if (this.HasDropDownItems)
            {
                bool flag = !base.IsOnDropDown || base.IsOnOverflow;
                if (flag && (((keys == Keys.Down) || (keys == Keys.Up)) || ((keys == Keys.Enter) || (base.SupportsSpaceKey && (keys == Keys.Space)))))
                {
                    if (this.Enabled || base.DesignMode)
                    {
                        this.ShowDropDown();
                        this.DropDown.SelectNextToolStripItem(null, true);
                    }
                    return true;
                }
                if (!flag)
                {
                    bool flag2 = (this.DropDownDirection & ToolStripDropDownDirection.AboveRight) == ToolStripDropDownDirection.AboveLeft;
                    if ((((keys == Keys.Enter) || (base.SupportsSpaceKey && (keys == Keys.Space))) || (flag2 && (keys == Keys.Left))) || (!flag2 && (keys == Keys.Right)))
                    {
                        if (this.Enabled || base.DesignMode)
                        {
                            this.ShowDropDown();
                            this.DropDown.SelectNextToolStripItem(null, true);
                        }
                        return true;
                    }
                }
            }
            if (base.IsOnDropDown)
            {
                bool flag4 = (this.DropDownDirection & ToolStripDropDownDirection.AboveRight) == ToolStripDropDownDirection.AboveLeft;
                if ((flag4 && (keys == Keys.Right)) || (!flag4 && (keys == Keys.Left)))
                {
                    ToolStripDropDown currentParentDropDown = base.GetCurrentParentDropDown();
                    if ((currentParentDropDown != null) && !currentParentDropDown.IsFirstDropDown)
                    {
                        currentParentDropDown.SelectPreviousToolStrip();
                        return true;
                    }
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        private ToolStripDropDownDirection RTLTranslateDropDownDirection(ToolStripDropDownDirection dropDownDirection, RightToLeft rightToLeft)
        {
            switch (dropDownDirection)
            {
                case ToolStripDropDownDirection.AboveLeft:
                    return ToolStripDropDownDirection.AboveRight;

                case ToolStripDropDownDirection.AboveRight:
                    return ToolStripDropDownDirection.AboveLeft;

                case ToolStripDropDownDirection.BelowLeft:
                    return ToolStripDropDownDirection.BelowRight;

                case ToolStripDropDownDirection.BelowRight:
                    return ToolStripDropDownDirection.BelowLeft;

                case ToolStripDropDownDirection.Left:
                    return ToolStripDropDownDirection.Right;

                case ToolStripDropDownDirection.Right:
                    return ToolStripDropDownDirection.Left;
            }
            if (base.IsOnDropDown)
            {
                if (rightToLeft != RightToLeft.Yes)
                {
                    return ToolStripDropDownDirection.Right;
                }
                return ToolStripDropDownDirection.Left;
            }
            if (rightToLeft != RightToLeft.Yes)
            {
                return ToolStripDropDownDirection.BelowRight;
            }
            return ToolStripDropDownDirection.BelowLeft;
        }

        private bool ShouldSerializeDropDown() => 
            ((this.dropDown != null) && !this.dropDown.IsAutoGenerated);

        private bool ShouldSerializeDropDownDirection() => 
            (this.toolStripDropDownDirection != ToolStripDropDownDirection.Default);

        private bool ShouldSerializeDropDownItems() => 
            ((this.dropDown != null) && this.dropDown.IsAutoGenerated);

        public void ShowDropDown()
        {
            this.ShowDropDown(false);
        }

        internal void ShowDropDown(bool mousePush)
        {
            this.ShowDropDownInternal();
            ToolStripDropDownMenu dropDown = this.dropDown as ToolStripDropDownMenu;
            if (dropDown != null)
            {
                if (!mousePush)
                {
                    dropDown.ResetScrollPosition();
                }
                dropDown.RestoreScrollPosition();
            }
        }

        private void ShowDropDownInternal()
        {
            if ((this.dropDown == null) || !this.dropDown.Visible)
            {
                this.OnDropDownShow(EventArgs.Empty);
            }
            if (((this.dropDown != null) && !this.dropDown.Visible) && (!this.dropDown.IsAutoGenerated || (this.DropDownItems.Count > 0)))
            {
                if (this.DropDown == base.ParentInternal)
                {
                    throw new InvalidOperationException(System.Windows.Forms.SR.GetString("ToolStripShowDropDownInvalidOperation"));
                }
                this.dropDown.OwnerItem = this;
                this.dropDown.Location = this.DropDownLocation;
                this.dropDown.Show();
                base.Invalidate();
            }
        }

        [TypeConverter(typeof(ReferenceConverter)), System.Windows.Forms.SRDescription("ToolStripDropDownDescr"), System.Windows.Forms.SRCategory("CatData")]
        public ToolStripDropDown DropDown
        {
            get
            {
                if (this.dropDown == null)
                {
                    this.DropDown = this.CreateDefaultDropDown();
                    if (!(this is ToolStripOverflowButton))
                    {
                        this.dropDown.SetAutoGeneratedInternal(true);
                    }
                }
                return this.dropDown;
            }
            set
            {
                if (this.dropDown != value)
                {
                    if (this.dropDown != null)
                    {
                        this.dropDown.Opened -= new EventHandler(this.DropDown_Opened);
                        this.dropDown.Closed -= new ToolStripDropDownClosedEventHandler(this.DropDown_Closed);
                        this.dropDown.ItemClicked -= new ToolStripItemClickedEventHandler(this.DropDown_ItemClicked);
                        this.dropDown.UnassignDropDownItem();
                    }
                    this.dropDown = value;
                    if (this.dropDown != null)
                    {
                        this.dropDown.Opened += new EventHandler(this.DropDown_Opened);
                        this.dropDown.Closed += new ToolStripDropDownClosedEventHandler(this.DropDown_Closed);
                        this.dropDown.ItemClicked += new ToolStripItemClickedEventHandler(this.DropDown_ItemClicked);
                        this.dropDown.AssignToDropDownItem();
                    }
                }
            }
        }

        internal virtual Rectangle DropDownButtonArea =>
            this.Bounds;

        [System.Windows.Forms.SRDescription("ToolStripDropDownItemDropDownDirectionDescr"), System.Windows.Forms.SRCategory("CatBehavior"), Browsable(false)]
        public ToolStripDropDownDirection DropDownDirection
        {
            get
            {
                if (this.toolStripDropDownDirection == ToolStripDropDownDirection.Default)
                {
                    ToolStrip parentInternal = base.ParentInternal;
                    if (parentInternal != null)
                    {
                        ToolStripDropDownDirection defaultDropDownDirection = parentInternal.DefaultDropDownDirection;
                        if (this.OppositeDropDownAlign || ((this.RightToLeft != parentInternal.RightToLeft) && (this.RightToLeft != RightToLeft.Inherit)))
                        {
                            defaultDropDownDirection = this.RTLTranslateDropDownDirection(defaultDropDownDirection, this.RightToLeft);
                        }
                        if (base.IsOnDropDown)
                        {
                            Rectangle dropDownBounds = this.GetDropDownBounds(defaultDropDownDirection);
                            Rectangle b = new Rectangle(base.TranslatePoint(Point.Empty, ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ScreenCoords), this.Size);
                            Rectangle rectangle3 = Rectangle.Intersect(dropDownBounds, b);
                            if (rectangle3.Width >= 2)
                            {
                                RightToLeft rightToLeft = (this.RightToLeft == RightToLeft.Yes) ? RightToLeft.No : RightToLeft.Yes;
                                ToolStripDropDownDirection dropDownDirection = this.RTLTranslateDropDownDirection(defaultDropDownDirection, rightToLeft);
                                if (Rectangle.Intersect(this.GetDropDownBounds(dropDownDirection), b).Width < rectangle3.Width)
                                {
                                    defaultDropDownDirection = dropDownDirection;
                                }
                            }
                        }
                        return defaultDropDownDirection;
                    }
                }
                return this.toolStripDropDownDirection;
            }
            set
            {
                switch (value)
                {
                    case ToolStripDropDownDirection.AboveLeft:
                    case ToolStripDropDownDirection.AboveRight:
                    case ToolStripDropDownDirection.BelowLeft:
                    case ToolStripDropDownDirection.BelowRight:
                    case ToolStripDropDownDirection.Left:
                    case ToolStripDropDownDirection.Right:
                    case ToolStripDropDownDirection.Default:
                        if (this.toolStripDropDownDirection != value)
                        {
                            this.toolStripDropDownDirection = value;
                            if (this.HasDropDownItems && this.DropDown.Visible)
                            {
                                this.DropDown.Location = this.DropDownLocation;
                            }
                        }
                        return;
                }
                throw new InvalidEnumArgumentException("value", (int) value, typeof(ToolStripDropDownDirection));
            }
        }

        [System.Windows.Forms.SRCategory("CatData"), System.Windows.Forms.SRDescription("ToolStripDropDownItemsDescr"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolStripItemCollection DropDownItems =>
            this.DropDown.Items;

        protected internal virtual Point DropDownLocation
        {
            get
            {
                if ((base.ParentInternal == null) || !this.HasDropDownItems)
                {
                    return Point.Empty;
                }
                ToolStripDropDownDirection dropDownDirection = this.DropDownDirection;
                return this.GetDropDownBounds(dropDownDirection).Location;
            }
        }

        internal bool HasDropDown =>
            (this.dropDown != null);

        [Browsable(false)]
        public virtual bool HasDropDownItems =>
            ((this.dropDown != null) && this.dropDown.HasVisibleItems);

        internal virtual bool OppositeDropDownAlign =>
            false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public override bool Pressed
        {
            get
            {
                if ((this.dropDown == null) || ((!this.DropDown.AutoClose && base.IsInDesignMode) && (!base.IsInDesignMode || base.IsOnDropDown)))
                {
                    return base.Pressed;
                }
                return ((this.DropDown.OwnerItem == this) && this.DropDown.Visible);
            }
        }
    }
}
