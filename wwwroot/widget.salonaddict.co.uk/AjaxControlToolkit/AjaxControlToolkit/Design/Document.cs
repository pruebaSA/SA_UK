namespace AjaxControlToolkit.Design
{
    using System;

    internal class Document
    {
        private object _reference;
        private static Type _referencedType;

        public Document() : this(null)
        {
        }

        public Document(object reference)
        {
            this._reference = ReferencedType.IsInstanceOfType(reference) ? reference : null;
        }

        public AjaxControlToolkit.Design.ProjectItem ProjectItem
        {
            get
            {
                object reference = ReferencedType.GetProperty("ProjectItem").GetValue(this._reference, new object[0]);
                if (reference == null)
                {
                    return null;
                }
                return new AjaxControlToolkit.Design.ProjectItem(reference);
            }
        }

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
                    _referencedType = ReferencedAssemblies.EnvDTE.GetType("EnvDTE.Document");
                    if (_referencedType == null)
                    {
                        throw new InvalidOperationException("Failed to load type 'EnvDTE.Document' from assembly 'EnvDTE, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.");
                    }
                }
                return _referencedType;
            }
        }
    }
}

