﻿namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel.Design;
    using System.Design;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web.UI.WebControls;

    [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
    public class WizardStepCollectionEditor : CollectionEditor
    {
        public WizardStepCollectionEditor(Type type) : base(type)
        {
        }

        protected override bool CanSelectMultipleInstances() => 
            false;

        protected override CollectionEditor.CollectionForm CreateCollectionForm()
        {
            CollectionEditor.CollectionForm form = base.CreateCollectionForm();
            form.Text = System.Design.SR.GetString("CollectionEditorCaption", new object[] { "WizardStep" });
            return form;
        }

        protected override object CreateInstance(Type itemType) => 
            Activator.CreateInstance(itemType, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, null, null);

        protected override Type[] CreateNewItemTypes() => 
            new Type[] { typeof(WizardStep), typeof(TemplatedWizardStep) };
    }
}

