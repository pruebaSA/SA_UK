namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class MenuItemStyleCollection : StateManagedCollection
    {
        private static readonly Type[] knownTypes = new Type[] { typeof(MenuItemStyle) };

        internal MenuItemStyleCollection()
        {
        }

        public int Add(MenuItemStyle style) => 
            ((IList) this).Add(style);

        public bool Contains(MenuItemStyle style) => 
            ((IList) this).Contains(style);

        public void CopyTo(MenuItemStyle[] styleArray, int index)
        {
            base.CopyTo(styleArray, index);
        }

        protected override object CreateKnownType(int index) => 
            new MenuItemStyle();

        protected override Type[] GetKnownTypes() => 
            knownTypes;

        public int IndexOf(MenuItemStyle style) => 
            ((IList) this).IndexOf(style);

        public void Insert(int index, MenuItemStyle style)
        {
            ((IList) this).Insert(index, style);
        }

        protected override void OnInsert(int index, object value)
        {
            base.OnInsert(index, value);
            if (!(value is MenuItemStyle))
            {
                throw new ArgumentException(System.Web.SR.GetString("MenuItemStyleCollection_InvalidArgument"), "value");
            }
            MenuItemStyle style = (MenuItemStyle) value;
            style.Font.Underline = style.Font.Underline;
        }

        public void Remove(MenuItemStyle style)
        {
            ((IList) this).Remove(style);
        }

        public void RemoveAt(int index)
        {
            ((IList) this).RemoveAt(index);
        }

        protected override void SetDirtyObject(object o)
        {
            if (o is MenuItemStyle)
            {
                ((MenuItemStyle) o).SetDirty();
            }
        }

        public MenuItemStyle this[int i]
        {
            get => 
                ((MenuItemStyle) this[i]);
            set
            {
                this[i] = value;
            }
        }
    }
}

