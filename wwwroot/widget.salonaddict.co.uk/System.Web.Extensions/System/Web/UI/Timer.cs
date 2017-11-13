namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Threading;
    using System.Web;
    using System.Web.Resources;

    [Designer("System.Web.UI.Design.TimerDesigner, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"), ToolboxBitmap(typeof(EmbeddedResourceFinder), "System.Web.Resources.Timer.bmp"), ToolboxItemFilter("System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", ToolboxItemFilterType.Require), DefaultEvent("Tick"), DefaultProperty("Interval"), NonVisualControl, SupportsEventValidation, AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class Timer : Control, IPostBackEventHandler, IScriptControl
    {
        private System.Web.UI.IPage _page;
        private System.Web.UI.ScriptManager _scriptManager;
        private bool _stateDirty;
        private static readonly object TickEventKey = new object();

        [ResourceDescription("Timer_TimerTick"), Category("Action")]
        public event EventHandler<EventArgs> Tick
        {
            add
            {
                base.Events.AddHandler(TickEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(TickEventKey, value);
            }
        }

        private string GetJsonState() => 
            ("[" + (this.Enabled ? "true" : "false") + "," + this.Interval.ToString(CultureInfo.InvariantCulture) + "]");

        protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptComponentDescriptor iteratorVariable0 = new ScriptControlDescriptor("Sys.UI._Timer", this.ClientID);
            iteratorVariable0.AddProperty("interval", this.Interval);
            iteratorVariable0.AddProperty("enabled", this.Enabled);
            iteratorVariable0.AddProperty("uniqueID", this.UniqueID);
            yield return iteratorVariable0;
        }

        protected virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            yield return new ScriptReference("MicrosoftAjaxTimer.js", Assembly.GetAssembly(typeof(System.Web.UI.Timer)).FullName);
        }

        protected internal override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.ScriptManager.RegisterScriptControl<System.Web.UI.Timer>(this);
            if (this._stateDirty && this.ScriptManager.IsInAsyncPostBack)
            {
                this._stateDirty = false;
                this.ScriptManager.RegisterDataItem(this, this.GetJsonState(), true);
            }
            this.IPage.ClientScript.GetPostBackEventReference(new PostBackOptions(this, string.Empty));
        }

        protected virtual void OnTick(EventArgs e)
        {
            EventHandler<EventArgs> handler = (EventHandler<EventArgs>) base.Events[TickEventKey];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaisePostBackEvent(string eventArgument)
        {
            if (this.Enabled)
            {
                this.OnTick(EventArgs.Empty);
            }
        }

        protected internal override void Render(HtmlTextWriter writer)
        {
            this.IPage.VerifyRenderingInServerForm(this);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
            writer.AddStyleAttribute(HtmlTextWriterStyle.Visibility, "hidden");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.RenderEndTag();
            if (!base.DesignMode)
            {
                this.ScriptManager.RegisterScriptDescriptors(this);
            }
        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            this.RaisePostBackEvent(eventArgument);
        }

        IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors() => 
            this.GetScriptDescriptors();

        IEnumerable<ScriptReference> IScriptControl.GetScriptReferences() => 
            this.GetScriptReferences();

        [ResourceDescription("Timer_TimerEnable"), DefaultValue(true), Category("Behavior")]
        public bool Enabled
        {
            get
            {
                object obj2 = this.ViewState["Enabled"];
                return ((obj2 == null) || ((bool) obj2));
            }
            set
            {
                if (!this._stateDirty && base.IsTrackingViewState)
                {
                    object obj2 = this.ViewState["Enabled"];
                    this._stateDirty = (obj2 == null) || (value != ((bool) obj2));
                }
                this.ViewState["Enabled"] = value;
            }
        }

        [Category("Behavior"), ResourceDescription("Timer_TimerInterval"), DefaultValue(0xea60)]
        public int Interval
        {
            get
            {
                object obj2 = this.ViewState["Interval"];
                if (obj2 == null)
                {
                    return 0xea60;
                }
                return (int) obj2;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", AtlasWeb.Timer_IntervalMustBeGreaterThanZero);
                }
                if (!this._stateDirty && base.IsTrackingViewState)
                {
                    object obj2 = this.ViewState["Interval"];
                    this._stateDirty = (obj2 == null) || (value != ((int) obj2));
                }
                this.ViewState["Interval"] = value;
            }
        }

        private System.Web.UI.IPage IPage
        {
            get
            {
                if (this._page == null)
                {
                    Page page = this.Page;
                    if (page == null)
                    {
                        throw new InvalidOperationException(AtlasWeb.Common_PageCannotBeNull);
                    }
                    this._page = new PageWrapper(page);
                }
                return this._page;
            }
        }

        internal System.Web.UI.ScriptManager ScriptManager
        {
            get
            {
                if (this._scriptManager == null)
                {
                    Page page = this.Page;
                    if (page == null)
                    {
                        throw new InvalidOperationException(AtlasWeb.Common_PageCannotBeNull);
                    }
                    this._scriptManager = System.Web.UI.ScriptManager.GetCurrent(page);
                    if (this._scriptManager == null)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.Common_ScriptManagerRequired, new object[] { this.ID }));
                    }
                }
                return this._scriptManager;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Visible
        {
            get => 
                base.Visible;
            set
            {
                throw new NotImplementedException();
            }
        }


    }
}

