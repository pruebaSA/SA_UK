namespace System.Windows.Forms
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing.Design;

    [System.Windows.Forms.SRDescription("DescriptionHelpProvider"), ProvideProperty("HelpString", typeof(Control)), ProvideProperty("HelpNavigator", typeof(Control)), ToolboxItemFilter("System.Windows.Forms"), ProvideProperty("HelpKeyword", typeof(Control)), ProvideProperty("ShowHelp", typeof(Control))]
    public class HelpProvider : Component, IExtenderProvider
    {
        private Hashtable boundControls = new Hashtable();
        private string helpNamespace;
        private Hashtable helpStrings = new Hashtable();
        private Hashtable keywords = new Hashtable();
        private Hashtable navigators = new Hashtable();
        private Hashtable showHelp = new Hashtable();
        private object userData;

        public virtual bool CanExtend(object target) => 
            (target is Control);

        [Localizable(true), DefaultValue((string) null), System.Windows.Forms.SRDescription("HelpProviderHelpKeywordDescr")]
        public virtual string GetHelpKeyword(Control ctl) => 
            ((string) this.keywords[ctl]);

        [Localizable(true), System.Windows.Forms.SRDescription("HelpProviderNavigatorDescr"), DefaultValue(-2147483643)]
        public virtual HelpNavigator GetHelpNavigator(Control ctl)
        {
            object obj2 = this.navigators[ctl];
            if (obj2 != null)
            {
                return (HelpNavigator) obj2;
            }
            return HelpNavigator.AssociateIndex;
        }

        [System.Windows.Forms.SRDescription("HelpProviderHelpStringDescr"), DefaultValue((string) null), Localizable(true)]
        public virtual string GetHelpString(Control ctl) => 
            ((string) this.helpStrings[ctl]);

        [Localizable(true), System.Windows.Forms.SRDescription("HelpProviderShowHelpDescr")]
        public virtual bool GetShowHelp(Control ctl)
        {
            object obj2 = this.showHelp[ctl];
            if (obj2 == null)
            {
                return false;
            }
            return (bool) obj2;
        }

        private void OnControlHelp(object sender, HelpEventArgs hevent)
        {
            Control ctl = (Control) sender;
            string helpString = this.GetHelpString(ctl);
            string helpKeyword = this.GetHelpKeyword(ctl);
            HelpNavigator helpNavigator = this.GetHelpNavigator(ctl);
            if (this.GetShowHelp(ctl))
            {
                if (((Control.MouseButtons != MouseButtons.None) && (helpString != null)) && (helpString.Length > 0))
                {
                    Help.ShowPopup(ctl, helpString, hevent.MousePos);
                    hevent.Handled = true;
                }
                if (!hevent.Handled && (this.helpNamespace != null))
                {
                    if ((helpKeyword != null) && (helpKeyword.Length > 0))
                    {
                        Help.ShowHelp(ctl, this.helpNamespace, helpNavigator, helpKeyword);
                        hevent.Handled = true;
                    }
                    if (!hevent.Handled)
                    {
                        Help.ShowHelp(ctl, this.helpNamespace, helpNavigator);
                        hevent.Handled = true;
                    }
                }
                if ((!hevent.Handled && (helpString != null)) && (helpString.Length > 0))
                {
                    Help.ShowPopup(ctl, helpString, hevent.MousePos);
                    hevent.Handled = true;
                }
                if (!hevent.Handled && (this.helpNamespace != null))
                {
                    Help.ShowHelp(ctl, this.helpNamespace);
                    hevent.Handled = true;
                }
            }
        }

        private void OnQueryAccessibilityHelp(object sender, QueryAccessibilityHelpEventArgs e)
        {
            Control ctl = (Control) sender;
            e.HelpString = this.GetHelpString(ctl);
            e.HelpKeyword = this.GetHelpKeyword(ctl);
            e.HelpNamespace = this.HelpNamespace;
        }

        public virtual void ResetShowHelp(Control ctl)
        {
            this.showHelp.Remove(ctl);
        }

        public virtual void SetHelpKeyword(Control ctl, string keyword)
        {
            this.keywords[ctl] = keyword;
            if ((keyword != null) && (keyword.Length > 0))
            {
                this.SetShowHelp(ctl, true);
            }
            this.UpdateEventBinding(ctl);
        }

        public virtual void SetHelpNavigator(Control ctl, HelpNavigator navigator)
        {
            if (!System.Windows.Forms.ClientUtils.IsEnumValid(navigator, (int) navigator, -2147483647, -2147483641))
            {
                throw new InvalidEnumArgumentException("navigator", (int) navigator, typeof(HelpNavigator));
            }
            this.navigators[ctl] = navigator;
            this.SetShowHelp(ctl, true);
            this.UpdateEventBinding(ctl);
        }

        public virtual void SetHelpString(Control ctl, string helpString)
        {
            this.helpStrings[ctl] = helpString;
            if ((helpString != null) && (helpString.Length > 0))
            {
                this.SetShowHelp(ctl, true);
            }
            this.UpdateEventBinding(ctl);
        }

        public virtual void SetShowHelp(Control ctl, bool value)
        {
            this.showHelp[ctl] = value;
            this.UpdateEventBinding(ctl);
        }

        internal virtual bool ShouldSerializeShowHelp(Control ctl) => 
            this.showHelp.ContainsKey(ctl);

        public override string ToString() => 
            (base.ToString() + ", HelpNamespace: " + this.HelpNamespace);

        private void UpdateEventBinding(Control ctl)
        {
            if (this.GetShowHelp(ctl) && !this.boundControls.ContainsKey(ctl))
            {
                ctl.HelpRequested += new HelpEventHandler(this.OnControlHelp);
                ctl.QueryAccessibilityHelp += new QueryAccessibilityHelpEventHandler(this.OnQueryAccessibilityHelp);
                this.boundControls[ctl] = ctl;
            }
            else if (!this.GetShowHelp(ctl) && this.boundControls.ContainsKey(ctl))
            {
                ctl.HelpRequested -= new HelpEventHandler(this.OnControlHelp);
                ctl.QueryAccessibilityHelp -= new QueryAccessibilityHelpEventHandler(this.OnQueryAccessibilityHelp);
                this.boundControls.Remove(ctl);
            }
        }

        [DefaultValue((string) null), Localizable(true), System.Windows.Forms.SRDescription("HelpProviderHelpNamespaceDescr"), Editor("System.Windows.Forms.Design.HelpNamespaceEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public virtual string HelpNamespace
        {
            get => 
                this.helpNamespace;
            set
            {
                this.helpNamespace = value;
            }
        }

        [TypeConverter(typeof(StringConverter)), System.Windows.Forms.SRDescription("ControlTagDescr"), System.Windows.Forms.SRCategory("CatData"), Bindable(true), DefaultValue((string) null), Localizable(false)]
        public object Tag
        {
            get => 
                this.userData;
            set
            {
                this.userData = value;
            }
        }
    }
}

