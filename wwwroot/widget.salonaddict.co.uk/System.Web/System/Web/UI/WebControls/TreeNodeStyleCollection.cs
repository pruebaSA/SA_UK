namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class TreeNodeStyleCollection : StateManagedCollection
    {
        private static readonly Type[] knownTypes = new Type[] { typeof(TreeNodeStyle) };

        internal TreeNodeStyleCollection()
        {
        }

        public int Add(TreeNodeStyle style) => 
            ((IList) this).Add(style);

        public bool Contains(TreeNodeStyle style) => 
            ((IList) this).Contains(style);

        public void CopyTo(TreeNodeStyle[] styleArray, int index)
        {
            base.CopyTo(styleArray, index);
        }

        protected override object CreateKnownType(int index) => 
            new TreeNodeStyle();

        protected override Type[] GetKnownTypes() => 
            knownTypes;

        public int IndexOf(TreeNodeStyle style) => 
            ((IList) this).IndexOf(style);

        public void Insert(int index, TreeNodeStyle style)
        {
            ((IList) this).Insert(index, style);
        }

        protected override void OnInsert(int index, object value)
        {
            base.OnInsert(index, value);
            if (!(value is TreeNodeStyle))
            {
                throw new ArgumentException(System.Web.SR.GetString("TreeNodeStyleCollection_InvalidArgument"), "value");
            }
            TreeNodeStyle style = (TreeNodeStyle) value;
            style.Font.Underline = style.Font.Underline;
        }

        public void Remove(TreeNodeStyle style)
        {
            ((IList) this).Remove(style);
        }

        public void RemoveAt(int index)
        {
            ((IList) this).RemoveAt(index);
        }

        protected override void SetDirtyObject(object o)
        {
            if (o is TreeNodeStyle)
            {
                ((TreeNodeStyle) o).SetDirty();
            }
        }

        public TreeNodeStyle this[int i]
        {
            get => 
                ((TreeNodeStyle) this[i]);
            set
            {
                this[i] = value;
            }
        }
    }
}

