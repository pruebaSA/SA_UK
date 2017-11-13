namespace System.Web.UI.Design
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Web.Resources.Design;
    using System.Web.UI;

    public class UpdateProgressDesigner : ControlDesigner
    {
        private const string ContentTemplatePropertyName = "ProgressTemplate";

        public override string GetDesignTimeHtml(DesignerRegionCollection regions)
        {
            if (regions == null)
            {
                return base.GetDesignTimeHtml(regions);
            }
            EditableDesignerRegion region = new UpdateProgressDesignerRegion(base.Component, this.Template, this.TemplateDescriptor, this.TemplateDefinition) {
                Description = AtlasWebDesign.UpdateProgressDesigner_RegionDescription
            };
            regions.Add(region);
            return DesignUtil.GetContainerDesignTimeHtml(false);
        }

        public override string GetEditableDesignerRegionContent(EditableDesignerRegion region)
        {
            UpdateProgressDesignerRegion region2 = region as UpdateProgressDesignerRegion;
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

        public override void SetEditableDesignerRegionContent(EditableDesignerRegion region, string content)
        {
            UpdateProgressDesignerRegion region2 = region as UpdateProgressDesignerRegion;
            if (region2 != null)
            {
                IDesignerHost service = (IDesignerHost) base.Component.Site.GetService(typeof(IDesignerHost));
                ITemplate template = ControlParser.ParseTemplate(service, content);
                region2.PropertyDescriptor.SetValue(region2.Object, template);
                region2.Template = template;
            }
        }

        private ITemplate Template =>
            this.UpdateProgress.ProgressTemplate;

        private System.Web.UI.Design.TemplateDefinition TemplateDefinition =>
            new System.Web.UI.Design.TemplateDefinition(this, "ProgressTemplate", base.Component, "ProgressTemplate");

        private PropertyDescriptor TemplateDescriptor =>
            TypeDescriptor.GetProperties(base.Component)["ProgressTemplate"];

        private System.Web.UI.UpdateProgress UpdateProgress =>
            ((System.Web.UI.UpdateProgress) base.Component);

        protected override bool UsePreviewControl =>
            true;

        private class UpdateProgressDesignerRegion : TemplatedEditableDesignerRegion
        {
            private object _object;
            private System.ComponentModel.PropertyDescriptor _propertyDescriptor;
            private ITemplate _template;

            public UpdateProgressDesignerRegion(object obj, ITemplate template, System.ComponentModel.PropertyDescriptor descriptor, TemplateDefinition definition) : base(definition)
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

