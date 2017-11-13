namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing.Design;
    using System.Security.Permissions;
    using System.Web.UI.Design.Util;
    using System.Web.UI.WebControls;
    using System.Windows.Forms;

    public class DataPagerFieldTypeEditor : UITypeEditor
    {
        [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            DataPager instance = context.Instance as DataPager;
            if (instance == null)
            {
                return null;
            }
            IDesignerHost host = (IDesignerHost) provider.GetService(typeof(IDesignerHost));
            DataPagerDesigner controlDesigner = (DataPagerDesigner) host.GetDesigner(instance);
            IComponentChangeService service = (IComponentChangeService) provider.GetService(typeof(IComponentChangeService));
            DataPagerFieldsEditor form = new DataPagerFieldsEditor(controlDesigner);
            if ((UIServiceHelper.ShowDialog(provider, form) == DialogResult.OK) && (service != null))
            {
                service.OnComponentChanged(instance, null, null, null);
            }
            return value;
        }

        [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => 
            UITypeEditorEditStyle.Modal;
    }
}

