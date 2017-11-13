﻿namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel.Design;
    using System.Reflection;
    using System.Security.Permissions;

    [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
    public class TableRowsCollectionEditor : CollectionEditor
    {
        public TableRowsCollectionEditor(Type type) : base(type)
        {
        }

        protected override bool CanSelectMultipleInstances() => 
            false;

        protected override object CreateInstance(Type itemType) => 
            Activator.CreateInstance(itemType, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, null, null);
    }
}

