namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing.Design;
    using System.Web.UI.Design.Util;
    using System.Web.UI.WebControls;
    using System.Windows.Forms;

    public class DataControlFieldTypeEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            DataBoundControl instance = context.Instance as DataBoundControl;
            if (instance == null)
            {
                return null;
            }
            IDesignerHost host = (IDesignerHost) provider.GetService(typeof(IDesignerHost));
            DataBoundControlDesigner controlDesigner = (DataBoundControlDesigner) host.GetDesigner(instance);
            IComponentChangeService service = (IComponentChangeService) provider.GetService(typeof(IComponentChangeService));
            DataControlFieldsEditor form = new DataControlFieldsEditor(controlDesigner);
            if ((UIServiceHelper.ShowDialog(provider, form) == DialogResult.OK) && (service != null))
            {
                service.OnComponentChanged(instance, null, null, null);
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => 
            UITypeEditorEditStyle.Modal;
    }
}

