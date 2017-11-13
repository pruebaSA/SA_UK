namespace System.Web.UI.Design
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Web.Resources.Design;
    using System.Web.UI;

    public class UpdatePanelDesigner : ControlDesigner
    {
        private bool _triggerEditorOpen;
        private const string ContentTemplatePropertyName = "ContentTemplate";

        internal void EnterTriggerEditor()
        {
            this._triggerEditorOpen = true;
        }

        internal void ExitTriggerEditor()
        {
            this._triggerEditorOpen = false;
            this.OnComponentChanged(this, new ComponentChangedEventArgs(this.UpdatePanel, null, null, null));
        }

        public override string GetDesignTimeHtml(DesignerRegionCollection regions)
        {
            this.UpdatePanel.ClearContent();
            if (regions == null)
            {
                return base.GetDesignTimeHtml(regions);
            }
            EditableDesignerRegion region = new UpdatePanelDesignerRegion(this.CurrentObject, this.CurrentTemplate, this.CurrentTemplateDescriptor, this.TemplateDefinition) {
                Description = AtlasWebDesign.UpdatePanelDesigner_RegionDescription
            };
            regions.Add(region);
            return DesignUtil.GetContainerDesignTimeHtml(this.UpdatePanel.RenderMode == UpdatePanelRenderMode.Inline);
        }

        public override string GetEditableDesignerRegionContent(EditableDesignerRegion region)
        {
            UpdatePanelDesignerRegion region2 = region as UpdatePanelDesignerRegion;
            if (region2 != null)
            {
                ITemplate template = region2.Template;
                if (template != null)
                {
                    IDesignerHost service = (IDesignerHost) base.Component.Site.GetService(typeof(IDesignerHost));
                    return ControlPersister.PersistTemplate(template, service);
                }
            }
            return base.GetEditableDesignerRegionContent(region);
        }

        public override void Initialize(IComponent component)
        {
            ControlDesigner.VerifyInitializeArgument(component, typeof(System.Web.UI.UpdatePanel));
            base.Initialize(component);
        }

        public override void OnComponentChanged(object sender, ComponentChangedEventArgs ce)
        {
            if (!this._triggerEditorOpen)
            {
                base.OnComponentChanged(sender, ce);
            }
        }

        public override void SetEditableDesignerRegionContent(EditableDesignerRegion region, string content)
        {
            UpdatePanelDesignerRegion region2 = region as UpdatePanelDesignerRegion;
            if (region2 != null)
            {
                IDesignerHost service = (IDesignerHost) base.Component.Site.GetService(typeof(IDesignerHost));
                ITemplate template = ControlParser.ParseTemplate(service, content);
                region2.PropertyDescriptor.SetValue(region2.Object, template);
                region2.Template = template;
            }
        }

        private object CurrentObject =>
            base.Component;

        private ITemplate CurrentTemplate =>
            this.UpdatePanel.ContentTemplate;

        private PropertyDescriptor CurrentTemplateDescriptor =>
            TypeDescriptor.GetProperties(base.Component)["ContentTemplate"];

        private System.Web.UI.Design.TemplateDefinition TemplateDefinition =>
            new System.Web.UI.Design.TemplateDefinition(this, "ContentTemplate", base.Component, "ContentTemplate");

        private System.Web.UI.UpdatePanel UpdatePanel =>
            ((System.Web.UI.UpdatePanel) base.Component);

        protected override bool UsePreviewControl =>
            true;

        private class UpdatePanelDesignerRegion : TemplatedEditableDesignerRegion
        {
            private object _object;
            private System.ComponentModel.PropertyDescriptor _propertyDescriptor;
            private ITemplate _template;

            public UpdatePanelDesignerRegion(object obj, ITemplate template, System.ComponentModel.PropertyDescriptor descriptor, TemplateDefinition definition) : base(definition)
            {
                this._template = template;
                this._object = obj;
                this._propertyDescriptor = descriptor;
                base.EnsureSize = true;
            }

            public object Object =>
                this._object;

            public System.ComponentModel.PropertyDescriptor PropertyDescriptor =>
                this._propertyDescriptor;

            public ITemplate Template
            {
                get => 
                    this._template;
                set
                {
                    this._template = value;
                }
            }
        }
    }
}

