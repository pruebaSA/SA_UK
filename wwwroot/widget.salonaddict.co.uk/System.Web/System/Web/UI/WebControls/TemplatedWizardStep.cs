namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [ParseChildren(true), Themeable(true), ControlBuilder(typeof(WizardStepControlBuilder)), ToolboxItem(false), PersistChildren(false), Bindable(false), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class TemplatedWizardStep : WizardStepBase
    {
        private Control _contentContainer;
        private ITemplate _contentTemplate;
        private Control _navigationContainer;
        private ITemplate _navigationTemplate;

        [PersistenceMode(PersistenceMode.InnerProperty), WebSysDescription("TemplatedWizardStep_ContentTemplate"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false), TemplateContainer(typeof(Wizard)), DefaultValue((string) null)]
        public virtual ITemplate ContentTemplate
        {
            get => 
                this._contentTemplate;
            set
            {
                this._contentTemplate = value;
                if ((this.Owner != null) && (base.ControlState > ControlState.Constructed))
                {
                    this.Owner.RequiresControlsRecreation();
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control ContentTemplateContainer
        {
            get => 
                this._contentContainer;
            internal set
            {
                this._contentContainer = value;
            }
        }

        [WebSysDescription("TemplatedWizardStep_CustomNavigationTemplate"), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(Wizard)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DefaultValue((string) null), Browsable(false)]
        public virtual ITemplate CustomNavigationTemplate
        {
            get => 
                this._navigationTemplate;
            set
            {
                this._navigationTemplate = value;
                if ((this.Owner != null) && (base.ControlState > ControlState.Constructed))
                {
                    this.Owner.RequiresControlsRecreation();
                }
            }
        }

        [Bindable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control CustomNavigationTemplateContainer
        {
            get => 
                this._navigationContainer;
            internal set
            {
                this._navigationContainer = value;
            }
        }

        [Browsable(true)]
        public override string SkinID
        {
            get => 
                base.SkinID;
            set
            {
                base.SkinID = value;
            }
        }
    }
}

