﻿namespace System.Windows.Forms
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    internal class ToolStripRendererSwitcher
    {
        private System.Type currentRendererType;
        private ToolStripRenderMode defaultRenderMode;
        private ToolStripRenderer renderer;
        private BitVector32 state;
        private static readonly int stateAttachedRendererChanged = BitVector32.CreateMask(stateUseDefaultRenderer);
        private static readonly int stateUseDefaultRenderer = BitVector32.CreateMask();

        public event EventHandler RendererChanged;

        public ToolStripRendererSwitcher(Control owner)
        {
            this.currentRendererType = typeof(System.Type);
            this.state = new BitVector32();
            this.defaultRenderMode = ToolStripRenderMode.ManagerRenderMode;
            this.state[stateUseDefaultRenderer] = true;
            this.state[stateAttachedRendererChanged] = false;
            owner.Disposed += new EventHandler(this.OnControlDisposed);
            owner.VisibleChanged += new EventHandler(this.OnControlVisibleChanged);
            if (owner.Visible)
            {
                this.OnControlVisibleChanged(owner, EventArgs.Empty);
            }
        }

        public ToolStripRendererSwitcher(Control owner, ToolStripRenderMode defaultRenderMode) : this(owner)
        {
            this.defaultRenderMode = defaultRenderMode;
            this.RenderMode = defaultRenderMode;
        }

        private void OnControlDisposed(object sender, EventArgs e)
        {
            if (this.state[stateAttachedRendererChanged])
            {
                ToolStripManager.RendererChanged -= new EventHandler(this.OnDefaultRendererChanged);
                this.state[stateAttachedRendererChanged] = false;
            }
        }

        private void OnControlVisibleChanged(object sender, EventArgs e)
        {
            Control control = sender as Control;
            if (control != null)
            {
                if (control.Visible)
                {
                    if (!this.state[stateAttachedRendererChanged])
                    {
                        ToolStripManager.RendererChanged += new EventHandler(this.OnDefaultRendererChanged);
                        this.state[stateAttachedRendererChanged] = true;
                    }
                }
                else if (this.state[stateAttachedRendererChanged])
                {
                    ToolStripManager.RendererChanged -= new EventHandler(this.OnDefaultRendererChanged);
                    this.state[stateAttachedRendererChanged] = false;
                }
            }
        }

        private void OnDefaultRendererChanged(object sender, EventArgs e)
        {
            if (this.state[stateUseDefaultRenderer])
            {
                this.OnRendererChanged(e);
            }
        }

        private void OnRendererChanged(EventArgs e)
        {
            if (this.RendererChanged != null)
            {
                this.RendererChanged(this, e);
            }
        }

        public void ResetRenderMode()
        {
            this.RenderMode = this.defaultRenderMode;
        }

        public bool ShouldSerializeRenderMode() => 
            ((this.RenderMode != this.defaultRenderMode) && (this.RenderMode != ToolStripRenderMode.Custom));

        public ToolStripRenderer Renderer
        {
            get
            {
                if (this.RenderMode == ToolStripRenderMode.ManagerRenderMode)
                {
                    return ToolStripManager.Renderer;
                }
                this.state[stateUseDefaultRenderer] = false;
                if (this.renderer == null)
                {
                    this.Renderer = ToolStripManager.CreateRenderer(this.RenderMode);
                }
                return this.renderer;
            }
            set
            {
                if (this.renderer != value)
                {
                    this.state[stateUseDefaultRenderer] = value == null;
                    this.renderer = value;
                    this.currentRendererType = (this.renderer != null) ? this.renderer.GetType() : typeof(System.Type);
                    this.OnRendererChanged(EventArgs.Empty);
                }
            }
        }

        public ToolStripRenderMode RenderMode
        {
            get
            {
                if (this.state[stateUseDefaultRenderer])
                {
                    return ToolStripRenderMode.ManagerRenderMode;
                }
                if ((this.renderer == null) || this.renderer.IsAutoGenerated)
                {
                    if (this.currentRendererType == ToolStripManager.ProfessionalRendererType)
                    {
                        return ToolStripRenderMode.Professional;
                    }
                    if (this.currentRendererType == ToolStripManager.SystemRendererType)
                    {
                        return ToolStripRenderMode.System;
                    }
                }
                return ToolStripRenderMode.Custom;
            }
            set
            {
                if (!System.Windows.Forms.ClientUtils.IsEnumValid(value, (int) value, 0, 3))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(ToolStripRenderMode));
                }
                if (value == ToolStripRenderMode.Custom)
                {
                    throw new NotSupportedException(System.Windows.Forms.SR.GetString("ToolStripRenderModeUseRendererPropertyInstead"));
                }
                if (value == ToolStripRenderMode.ManagerRenderMode)
                {
                    if (!this.state[stateUseDefaultRenderer])
                    {
                        this.state[stateUseDefaultRenderer] = true;
                        this.OnRendererChanged(EventArgs.Empty);
                    }
                }
                else
                {
                    this.state[stateUseDefaultRenderer] = false;
                    this.Renderer = ToolStripManager.CreateRenderer(value);
                }
            }
        }
    }
}

