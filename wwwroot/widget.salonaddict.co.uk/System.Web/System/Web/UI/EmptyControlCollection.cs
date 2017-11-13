﻿namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class EmptyControlCollection : ControlCollection
    {
        public EmptyControlCollection(Control owner) : base(owner)
        {
        }

        public override void Add(Control child)
        {
            this.ThrowNotSupportedException();
        }

        public override void AddAt(int index, Control child)
        {
            this.ThrowNotSupportedException();
        }

        private void ThrowNotSupportedException()
        {
            throw new HttpException(System.Web.SR.GetString("Control_does_not_allow_children", new object[] { base.Owner.GetType().ToString() }));
        }
    }
}

