﻿namespace System.Web.UI.Design
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing.Design;
    using System.Security.Permissions;
    using System.Web.UI;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;

    [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
    public class ExpressionsCollectionEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            System.Web.UI.Control instance = (System.Web.UI.Control) context.Instance;
            IServiceProvider site = instance.Site;
            if (site == null)
            {
                if (instance.Page != null)
                {
                    site = instance.Page.Site;
                }
                if (site == null)
                {
                    site = provider;
                }
            }
            if (site != null)
            {
                DesignerTransaction transaction = ((IDesignerHost) site.GetService(typeof(IDesignerHost))).CreateTransaction("(Expressions)");
                try
                {
                    IComponentChangeService service = (IComponentChangeService) site.GetService(typeof(IComponentChangeService));
                    if (service != null)
                    {
                        try
                        {
                            service.OnComponentChanging(instance, null);
                        }
                        catch (CheckoutException exception)
                        {
                            if (exception != CheckoutException.Canceled)
                            {
                                throw exception;
                            }
                            return value;
                        }
                    }
                    DialogResult cancel = DialogResult.Cancel;
                    try
                    {
                        ExpressionBindingsDialog dialog = new ExpressionBindingsDialog(site, instance);
                        cancel = ((IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService))).ShowDialog(dialog);
                    }
                    finally
                    {
                        if ((cancel == DialogResult.OK) && (service != null))
                        {
                            try
                            {
                                service.OnComponentChanged(instance, null, null, null);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                finally
                {
                    transaction.Commit();
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => 
            UITypeEditorEditStyle.Modal;
    }
}

