namespace AjaxControlToolkit.Design
{
    using System;

    internal class ProjectItem
    {
        private object _reference;
        private static Type _referencedType;

        public ProjectItem() : this(null)
        {
        }

        public ProjectItem(object reference)
        {
            this._reference = ReferencedType.IsInstanceOfType(reference) ? reference : null;
        }

        public FileCodeModel2 FileCodeModel
        {
            get
            {
                object reference = ReferencedType.GetProperty("FileCodeModel").GetValue(this._reference, new object[0]);
                if (reference == null)
                {
                    return null;
                }
                return new FileCodeModel2(reference);
            }
        }

        public object Object =>
            ReferencedType.GetProperty("Object").GetValue(this._reference, new object[0]);

        public object Reference
        {
            get => 
                this._reference;
            set
            {
                this._reference = value;
            }
        }

        public static Type ReferencedType
        {
            get
            {
                if (_referencedType == null)
                {
                    _referencedType = ReferencedAssemblies.EnvDTE.GetType("EnvDTE.ProjectItem");
                    if (_referencedType == null)
                    {
                        throw new InvalidOperationException("Failed to load type 'EnvDTE.ProjectItem' from assembly 'EnvDTE, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.");
                    }
                }
                return _referencedType;
            }
        }
    }
}

