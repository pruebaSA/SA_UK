namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class ButtonFieldBase : DataControlField
    {
        protected ButtonFieldBase()
        {
        }

        protected override void CopyProperties(DataControlField newField)
        {
            ((ButtonFieldBase) newField).ButtonType = this.ButtonType;
            ((ButtonFieldBase) newField).CausesValidation = this.CausesValidation;
            ((ButtonFieldBase) newField).ValidationGroup = this.ValidationGroup;
            base.CopyProperties(newField);
        }

        [WebCategory("Appearance"), WebSysDescription("ButtonFieldBase_ButtonType"), DefaultValue(2)]
        public virtual System.Web.UI.WebControls.ButtonType ButtonType
        {
            get
            {
                object obj2 = base.ViewState["ButtonType"];
                if (obj2 != null)
                {
                    return (System.Web.UI.WebControls.ButtonType) obj2;
                }
                return System.Web.UI.WebControls.ButtonType.Link;
            }
            set
            {
                if ((value < System.Web.UI.WebControls.ButtonType.Button) || (value > System.Web.UI.WebControls.ButtonType.Link))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                object obj2 = base.ViewState["ButtonType"];
                if ((obj2 == null) || (((System.Web.UI.WebControls.ButtonType) obj2) != value))
                {
                    base.ViewState["ButtonType"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [WebCategory("Behavior"), WebSysDescription("ButtonFieldBase_CausesValidation"), DefaultValue(false)]
        public virtual bool CausesValidation
        {
            get
            {
                object obj2 = base.ViewState["CausesValidation"];
                return ((obj2 != null) && ((bool) obj2));
            }
            set
            {
                object obj2 = base.ViewState["CausesValidation"];
                if ((obj2 == null) || (((bool) obj2) != value))
                {
                    base.ViewState["CausesValidation"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [WebSysDescription("DataControlField_ShowHeader"), WebCategory("Behavior"), DefaultValue(false)]
        public override bool ShowHeader
        {
            get
            {
                object obj2 = base.ViewState["ShowHeader"];
                return ((obj2 != null) && ((bool) obj2));
            }
            set
            {
                object obj2 = base.ViewState["ShowHeader"];
                if ((obj2 == null) || (((bool) obj2) != value))
                {
                    base.ViewState["ShowHeader"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [DefaultValue(""), WebCategory("Behavior"), WebSysDescription("ButtonFieldBase_ValidationGroup")]
        public virtual string ValidationGroup
        {
            get
            {
                object obj2 = base.ViewState["ValidationGroup"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                if (!object.Equals(value, base.ViewState["ValidationGroup"]))
                {
                    base.ViewState["ValidationGroup"] = value;
                    this.OnFieldChanged();
                }
            }
        }
    }
}

