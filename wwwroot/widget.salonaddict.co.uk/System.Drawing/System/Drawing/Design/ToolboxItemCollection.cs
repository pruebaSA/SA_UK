﻿namespace System.Drawing.Design
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
    public sealed class ToolboxItemCollection : ReadOnlyCollectionBase
    {
        public ToolboxItemCollection(ToolboxItemCollection value)
        {
            base.InnerList.AddRange(value);
        }

        public ToolboxItemCollection(ToolboxItem[] value)
        {
            base.InnerList.AddRange(value);
        }

        public bool Contains(ToolboxItem value) => 
            base.InnerList.Contains(value);

        public void CopyTo(ToolboxItem[] array, int index)
        {
            base.InnerList.CopyTo(array, index);
        }

        public int IndexOf(ToolboxItem value) => 
            base.InnerList.IndexOf(value);

        public ToolboxItem this[int index] =>
            ((ToolboxItem) base.InnerList[index]);
    }
}

