namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows.Forms.Layout;

    [System.Windows.Forms.SRDescription("DescriptionFlowLayoutPanel"), ClassInterface(ClassInterfaceType.AutoDispatch), ProvideProperty("FlowBreak", typeof(Control)), ComVisible(true), DefaultProperty("FlowDirection"), Designer("System.Windows.Forms.Design.FlowLayoutPanelDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), Docking(DockingBehavior.Ask)]
    public class FlowLayoutPanel : Panel, IExtenderProvider
    {
        private FlowLayoutSettings _flowLayoutSettings;

        public FlowLayoutPanel()
        {
            this._flowLayoutSettings = FlowLayout.CreateSettings(this);
        }

        [DefaultValue(false), DisplayName("FlowBreak")]
        public bool GetFlowBreak(Control control) => 
            this._flowLayoutSettings.GetFlowBreak(control);

        [DisplayName("FlowBreak")]
        public void SetFlowBreak(Control control, bool value)
        {
            this._flowLayoutSettings.SetFlowBreak(control, value);
        }

        bool IExtenderProvider.CanExtend(object obj)
        {
            Control control = obj as Control;
            return ((control != null) && (control.Parent == this));
        }

        [System.Windows.Forms.SRCategory("CatLayout"), DefaultValue(0), Localizable(true), System.Windows.Forms.SRDescription("FlowPanelFlowDirectionDescr")]
        public System.Windows.Forms.FlowDirection FlowDirection
        {
            get => 
                this._flowLayoutSettings.FlowDirection;
            set
            {
                this._flowLayoutSettings.FlowDirection = value;
            }
        }

        public override System.Windows.Forms.Layout.LayoutEngine LayoutEngine =>
            FlowLayout.Instance;

        [System.Windows.Forms.SRCategory("CatLayout"), System.Windows.Forms.SRDescription("FlowPanelWrapContentsDescr"), Localizable(true), DefaultValue(true)]
        public bool WrapContents
        {
            get => 
                this._flowLayoutSettings.WrapContents;
            set
            {
                this._flowLayoutSettings.WrapContents = value;
            }
        }
    }
}

