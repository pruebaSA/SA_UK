namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [Browsable(false), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class CreateUserWizardStep : TemplatedWizardStep
    {
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AllowReturn
        {
            get => 
                this.AllowReturnInternal;
            set
            {
                throw new InvalidOperationException(System.Web.SR.GetString("CreateUserWizardStep_AllowReturnCannotBeSet"));
            }
        }

        internal bool AllowReturnInternal
        {
            get
            {
                object obj2 = this.ViewState["AllowReturnInternal"];
                if (obj2 != null)
                {
                    return (bool) obj2;
                }
                return true;
            }
            set
            {
                this.ViewState["AllowReturnInternal"] = value;
            }
        }

        internal override Wizard Owner
        {
            get => 
                base.Owner;
            set
            {
                if (!(value is CreateUserWizard) && (value != null))
                {
                    throw new HttpException(System.Web.SR.GetString("CreateUserWizardStep_OnlyAllowedInCreateUserWizard"));
                }
                base.Owner = value;
            }
        }

        [Browsable(false), Themeable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Filterable(false)]
        public override WizardStepType StepType
        {
            get => 
                base.StepType;
            set
            {
                throw new InvalidOperationException(System.Web.SR.GetString("CreateUserWizardStep_StepTypeCannotBeSet"));
            }
        }

        [Localizable(true), WebSysDefaultValue("CreateUserWizard_DefaultCreateUserTitleText")]
        public override string Title
        {
            get
            {
                string titleInternal = base.TitleInternal;
                if (titleInternal == null)
                {
                    return System.Web.SR.GetString("CreateUserWizard_DefaultCreateUserTitleText");
                }
                return titleInternal;
            }
            set
            {
                base.Title = value;
            }
        }
    }
}

