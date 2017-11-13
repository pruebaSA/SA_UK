namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class StyleCollection : StateManagedCollection
    {
        private static readonly Type[] knownTypes = new Type[] { typeof(Style) };

        internal StyleCollection()
        {
        }

        public int Add(Style style) => 
            ((IList) this).Add(style);

        public bool Contains(Style style) => 
            ((IList) this).Contains(style);

        public void CopyTo(Style[] styleArray, int index)
        {
            base.CopyTo(styleArray, index);
        }

        protected override object CreateKnownType(int index) => 
            new Style();

        protected override Type[] GetKnownTypes() => 
            knownTypes;

        public int IndexOf(Style style) => 
            ((IList) this).IndexOf(style);

        public void Insert(int index, Style style)
        {
            ((IList) this).Insert(index, style);
        }

        public void Remove(Style style)
        {
            ((IList) this).Remove(style);
        }

        public void RemoveAt(int index)
        {
            ((IList) this).RemoveAt(index);
        }

        protected override void SetDirtyObject(object o)
        {
            if (o is Style)
            {
                ((Style) o).SetDirty();
            }
        }

        public Style this[int i]
        {
            get => 
                ((Style) this[i]);
            set
            {
                this[i] = value;
            }
        }
    }
}

