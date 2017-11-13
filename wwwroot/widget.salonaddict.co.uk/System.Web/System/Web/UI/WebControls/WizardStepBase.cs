namespace System.Web.UI.WebControls
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [Bindable(false), ControlBuilder(typeof(WizardStepControlBuilder)), ToolboxItem(false), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class WizardStepBase : System.Web.UI.WebControls.View
    {
        private System.Web.UI.WebControls.Wizard _owner;

        protected WizardStepBase()
        {
        }

        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                base.LoadViewState(savedState);
                if ((this.Owner != null) && ((this.ViewState["Title"] != null) || (this.ViewState["StepType"] != null)))
                {
                    this.Owner.OnWizardStepsChanged();
                }
            }
        }

        protected internal override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if ((this.Owner == null) && !base.DesignMode)
            {
                throw new InvalidOperationException(System.Web.SR.GetString("WizardStep_WrongContainment"));
            }
        }

        protected internal override void RenderChildren(HtmlTextWriter writer)
        {
            if (this.Owner.ShouldRenderChildControl)
            {
                base.RenderChildren(writer);
            }
        }

        [WebCategory("Behavior"), WebSysDescription("WizardStep_AllowReturn"), Themeable(false), Filterable(false), DefaultValue(true)]
        public virtual bool AllowReturn
        {
            get
            {
                object obj2 = this.ViewState["AllowReturn"];
                if (obj2 != null)
                {
                    return (bool) obj2;
                }
                return true;
            }
            set
            {
                this.ViewState["AllowReturn"] = value;
            }
        }

        [Browsable(true)]
        public override bool EnableTheming
        {
            get => 
                base.EnableTheming;
            set
            {
                base.EnableTheming = value;
            }
        }

        public override string ID
        {
            get => 
                base.ID;
            set
            {
                if ((this.Owner != null) && this.Owner.DesignMode)
                {
                    if (!CodeGenerator.IsValidLanguageIndependentIdentifier(value))
                    {
                        throw new ArgumentException(System.Web.SR.GetString("Invalid_identifier", new object[] { value }));
                    }
                    if ((value != null) && value.Equals(this.Owner.ID, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ArgumentException(System.Web.SR.GetString("Id_already_used", new object[] { value }));
                    }
                    foreach (WizardStepBase base2 in this.Owner.WizardSteps)
                    {
                        if (((base2 != this) && (base2.ID != null)) && base2.ID.Equals(value, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new ArgumentException(System.Web.SR.GetString("Id_already_used", new object[] { value }));
                        }
                    }
                }
                base.ID = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), WebCategory("Appearance"), WebSysDescription("WizardStep_Name"), Browsable(false)]
        public virtual string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Title))
                {
                    return this.Title;
                }
                if (!string.IsNullOrEmpty(this.ID))
                {
                    return this.ID;
                }
                return null;
            }
        }

        internal virtual System.Web.UI.WebControls.Wizard Owner
        {
            get => 
                this._owner;
            set
            {
                this._owner = value;
            }
        }

        [WebSysDescription("WizardStep_StepType"), WebCategory("Behavior"), DefaultValue(0)]
        public virtual WizardStepType StepType
        {
            get
            {
                object obj2 = this.ViewState["StepType"];
                if (obj2 != null)
                {
                    return (WizardStepType) obj2;
                }
                return WizardStepType.Auto;
            }
            set
            {
                if ((value < WizardStepType.Auto) || (value > WizardStepType.Step))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (this.StepType != value)
                {
                    this.ViewState["StepType"] = value;
                    if (this.Owner != null)
                    {
                        this.Owner.OnWizardStepsChanged();
                    }
                }
            }
        }

        [Localizable(true), DefaultValue(""), WebSysDescription("WizardStep_Title"), WebCategory("Appearance")]
        public virtual string Title
        {
            get
            {
                string str = (string) this.ViewState["Title"];
                if (str != null)
                {
                    return str;
                }
                return string.Empty;
            }
            set
            {
                if (this.Title != value)
                {
                    this.ViewState["Title"] = value;
                    if (this.Owner != null)
                    {
                        this.Owner.OnWizardStepsChanged();
                    }
                }
            }
        }

        internal string TitleInternal =>
            ((string) this.ViewState["Title"]);

        [WebCategory("Appearance"), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public System.Web.UI.WebControls.Wizard Wizard =>
            this.Owner;
    }
}

