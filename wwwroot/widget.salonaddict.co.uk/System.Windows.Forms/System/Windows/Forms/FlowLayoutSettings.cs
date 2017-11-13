namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms.Layout;

    [DefaultProperty("FlowDirection")]
    public class FlowLayoutSettings : LayoutSettings
    {
        internal FlowLayoutSettings(IArrangedElement owner) : base(owner)
        {
        }

        public bool GetFlowBreak(object child) => 
            CommonProperties.GetFlowBreak(FlowLayout.Instance.CastToArrangedElement(child));

        public void SetFlowBreak(object child, bool value)
        {
            IArrangedElement element = FlowLayout.Instance.CastToArrangedElement(child);
            if (this.GetFlowBreak(child) != value)
            {
                CommonProperties.SetFlowBreak(element, value);
            }
        }

        [System.Windows.Forms.SRCategory("CatLayout"), System.Windows.Forms.SRDescription("FlowPanelFlowDirectionDescr"), DefaultValue(0)]
        public System.Windows.Forms.FlowDirection FlowDirection
        {
            get => 
                FlowLayout.GetFlowDirection(base.Owner);
            set
            {
                FlowLayout.SetFlowDirection(base.Owner, value);
            }
        }

        public override System.Windows.Forms.Layout.LayoutEngine LayoutEngine =>
            FlowLayout.Instance;

        [System.Windows.Forms.SRDescription("FlowPanelWrapContentsDescr"), DefaultValue(true), System.Windows.Forms.SRCategory("CatLayout")]
        public bool WrapContents
        {
            get => 
                FlowLayout.GetWrapContents(base.Owner);
            set
            {
                FlowLayout.SetWrapContents(base.Owner, value);
            }
        }
    }
}

