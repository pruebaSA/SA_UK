namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class EditorPartCollection : ReadOnlyCollectionBase
    {
        public static readonly EditorPartCollection Empty = new EditorPartCollection();

        public EditorPartCollection()
        {
        }

        public EditorPartCollection(ICollection editorParts)
        {
            this.Initialize(null, editorParts);
        }

        public EditorPartCollection(EditorPartCollection existingEditorParts, ICollection editorParts)
        {
            this.Initialize(existingEditorParts, editorParts);
        }

        internal int Add(EditorPart value) => 
            base.InnerList.Add(value);

        public bool Contains(EditorPart editorPart) => 
            base.InnerList.Contains(editorPart);

        public void CopyTo(EditorPart[] array, int index)
        {
            base.InnerList.CopyTo(array, index);
        }

        public int IndexOf(EditorPart editorPart) => 
            base.InnerList.IndexOf(editorPart);

        private void Initialize(EditorPartCollection existingEditorParts, ICollection editorParts)
        {
            if (existingEditorParts != null)
            {
                foreach (EditorPart part in existingEditorParts)
                {
                    base.InnerList.Add(part);
                }
            }
            if (editorParts != null)
            {
                foreach (object obj2 in editorParts)
                {
                    if (obj2 == null)
                    {
                        throw new ArgumentException(System.Web.SR.GetString("Collection_CantAddNull"), "editorParts");
                    }
                    if (!(obj2 is EditorPart))
                    {
                        throw new ArgumentException(System.Web.SR.GetString("Collection_InvalidType", new object[] { "EditorPart" }), "editorParts");
                    }
                    base.InnerList.Add(obj2);
                }
            }
        }

        public EditorPart this[int index] =>
            ((EditorPart) base.InnerList[index]);
    }
}

