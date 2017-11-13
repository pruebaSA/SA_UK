﻿namespace System.Drawing.Design
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
    public sealed class CategoryNameCollection : ReadOnlyCollectionBase
    {
        public CategoryNameCollection(CategoryNameCollection value)
        {
            base.InnerList.AddRange(value);
        }

        public CategoryNameCollection(string[] value)
        {
            base.InnerList.AddRange(value);
        }

        public bool Contains(string value) => 
            base.InnerList.Contains(value);

        public void CopyTo(string[] array, int index)
        {
            base.InnerList.CopyTo(array, index);
        }

        public int IndexOf(string value) => 
            base.InnerList.IndexOf(value);

        public string this[int index] =>
            ((string) base.InnerList[index]);
    }
}

