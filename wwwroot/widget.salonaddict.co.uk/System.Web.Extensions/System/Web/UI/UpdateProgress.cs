namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Threading;
    using System.Web;
    using System.Web.Resources;

    [PersistChildren(false), ToolboxBitmap(typeof(EmbeddedResourceFinder), "System.Web.Resources.UpdateProgress.bmp"), ToolboxItemFilter("System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", ToolboxItemFilterType.Require), DefaultProperty("AssociatedUpdatePanelID"), Designer("System.Web.UI.Design.UpdateProgressDesigner, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"), ParseChildren(true), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class UpdateProgress : Control, IScriptControl
    {
        private string _associatedUpdatePanelID;
        private int _displayAfter = 500;
        private bool _dynamicLayout = true;
        private ITemplate _progressTemplate;
        private Control _progressTemplateContainer;

        protected internal override void CreateChildControls()
        {
            if (this._progressTemplate != null)
            {
                this._progressTemplateContainer = new Control();
                this._progressTemplate.InstantiateIn(this._progressTemplateContainer);
                this.Controls.Add(this._progressTemplateContainer);
            }
        }

        public override void DataBind()
        {
            this.EnsureChildControls();
            base.DataBind();
        }

        protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            if (((this.Page == null) || !this.ScriptManager.SupportsPartialRendering) || !this.Visible)
            {
                yield break;
            }
            ScriptControlDescriptor iteratorVariable0 = new ScriptControlDescriptor("Sys.UI._UpdateProgress", this.ClientID);
            string clientID = null;
            if (!string.IsNullOrEmpty(this.AssociatedUpdatePanelID))
            {
                UpdatePanel panel = ControlUtil.FindTargetControl(this.AssociatedUpdatePanelID, this, true) as UpdatePanel;
                if (panel == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.UpdateProgress_NoUpdatePanel, new object[] { this.AssociatedUpdatePanelID }));
                }
                clientID = panel.ClientID;
            }
            iteratorVariable0.AddProperty("associatedUpdatePanelId", clientID);
            iteratorVariable0.AddProperty("dynamicLayout", this.DynamicLayout);
            iteratorVariable0.AddProperty("displayAfter", this.DisplayAfter);
            yield return iteratorVariable0;
        }

        protected virtual IEnumerable<ScriptReference> GetScriptReferences() => 
            new <GetScriptReferences>d__0(-2) { <>4__this = this };

        protected internal override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.ScriptManager.RegisterScriptControl<UpdateProgress>(this);
        }

        protected internal override void Render(HtmlTextWriter writer)
        {
            this.EnsureChildControls();
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
            if (this._dynamicLayout)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
            }
            else
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Visibility, "hidden");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "block");
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            base.Render(writer);
            writer.RenderEndTag();
            if (!base.DesignMode)
            {
                this.ScriptManager.RegisterScriptDescriptors(this);
            }
        }

        IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors() => 
            this.GetScriptDescriptors();

        IEnumerable<ScriptReference> IScriptControl.GetScriptReferences() => 
            this.GetScriptReferences();

        [DefaultValue(""), Category("Behavior"), IDReferenceProperty(typeof(UpdatePanel)), ResourceDescription("UpdateProgress_AssociatedUpdatePanelID"), TypeConverter("System.Web.UI.Design.UpdateProgressAssociatedUpdatePanelIDConverter")]
        public string AssociatedUpdatePanelID
        {
            get
            {
                if (this._associatedUpdatePanelID == null)
                {
                    return string.Empty;
                }
                return this._associatedUpdatePanelID;
            }
            set
            {
                this._associatedUpdatePanelID = value;
            }
        }

        public override ControlCollection Controls
        {
            get
            {
                this.EnsureChildControls();
                return base.Controls;
            }
        }

        [ResourceDescription("UpdateProgress_DisplayAfter"), Category("Behavior"), DefaultValue(500)]
        public int DisplayAfter
        {
            get => 
                this._displayAfter;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(AtlasWeb.UpdateProgress_DisplayAfterInvalid);
                }
                this._displayAfter = value;
            }
        }

        [ResourceDescription("UpdateProgress_DynamicLayout"), Category("Behavior"), DefaultValue(true)]
        public bool DynamicLayout
        {
            get => 
                this._dynamicLayout;
            set
            {
                this._dynamicLayout = value;
            }
        }

        [ResourceDescription("UpdateProgress_ProgressTemplate"), PersistenceMode(PersistenceMode.InnerProperty), Browsable(false)]
        public ITemplate ProgressTemplate
        {
            get => 
                this._progressTemplate;
            set
            {
                this._progressTemplate = value;
            }
        }

        private System.Web.UI.ScriptManager ScriptManager
        {
            get
            {
                System.Web.UI.ScriptManager current = System.Web.UI.ScriptManager.GetCurrent(this.Page);
                if (current == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.Common_ScriptManagerRequired, new object[] { this.ID }));
                }
                return current;
            }
        }


        [CompilerGenerated]
        private sealed class <GetScriptReferences>d__0 : IEnumerable<ScriptReference>, IEnumerable, IEnumerator<ScriptReference>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private ScriptReference <>2__current;
            public UpdateProgress <>4__this;
            private int <>l__initialThreadId;

            [DebuggerHidden]
            public <GetScriptReferences>d__0(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private bool MoveNext()
            {
                if (this.<>1__state == 0)
                {
                    this.<>1__state = -1;
                }
                return false;
            }

            [DebuggerHidden]
            IEnumerator<ScriptReference> IEnumerable<ScriptReference>.GetEnumerator()
            {
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    return this;
                }
                return new UpdateProgress.<GetScriptReferences>d__0(0) { <>4__this = this.<>4__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<System.Web.UI.ScriptReference>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
            }

            ScriptReference IEnumerator<ScriptReference>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

