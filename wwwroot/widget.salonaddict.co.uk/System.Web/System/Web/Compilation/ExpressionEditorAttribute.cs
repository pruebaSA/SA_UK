namespace System.Web.Compilation
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ExpressionEditorAttribute : Attribute
    {
        private string _editorTypeName;

        public ExpressionEditorAttribute(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException("typeName");
            }
            this._editorTypeName = typeName;
        }

        public ExpressionEditorAttribute(Type type) : this(type?.AssemblyQualifiedName)
        {
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            ExpressionEditorAttribute attribute = obj as ExpressionEditorAttribute;
            return ((attribute != null) && (attribute.EditorTypeName == this.EditorTypeName));
        }

        public override int GetHashCode() => 
            this.EditorTypeName.GetHashCode();

        public string EditorTypeName =>
            this._editorTypeName;
    }
}

