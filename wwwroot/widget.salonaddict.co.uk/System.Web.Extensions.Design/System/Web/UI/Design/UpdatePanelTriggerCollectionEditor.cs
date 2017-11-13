namespace System.Web.UI.Design
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Web.UI;

    public class UpdatePanelTriggerCollectionEditor : CollectionEditorBase
    {
        private static readonly Type[] TriggerTypes = new Type[] { typeof(AsyncPostBackTrigger), typeof(PostBackTrigger) };

        public UpdatePanelTriggerCollectionEditor(Type type) : base(type)
        {
        }

        protected override bool CanSelectMultipleInstances() => 
            false;

        protected override Type CreateCollectionItemType() => 
            typeof(UpdatePanelTrigger);

        protected override Type[] CreateNewItemTypes() => 
            TriggerTypes;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            UpdatePanelDesigner designer = null;
            if (context != null)
            {
                UpdatePanel component = null;
                component = context.Instance as UpdatePanel;
                IDesignerHost service = null;
                if (provider != null)
                {
                    service = (IDesignerHost) provider.GetService(typeof(IDesignerHost));
                    if (service != null)
                    {
                        designer = service.GetDesigner(component) as UpdatePanelDesigner;
                    }
                }
            }
            if (designer != null)
            {
                designer.EnterTriggerEditor();
            }
            value = base.EditValue(context, provider, value);
            if (designer != null)
            {
                designer.ExitTriggerEditor();
            }
            return value;
        }

        private sealed class UpdatePanelTrigger : System.Web.UI.UpdatePanelTrigger
        {
            private int _dummyProperty;

            protected internal override bool HasTriggered() => 
                false;

            public int DummyProperty
            {
                get => 
                    this._dummyProperty;
                set
                {
                    this._dummyProperty = value;
                }
            }
        }
    }
}

