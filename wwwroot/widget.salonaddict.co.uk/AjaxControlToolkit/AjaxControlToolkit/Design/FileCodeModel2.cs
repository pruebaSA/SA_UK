namespace AjaxControlToolkit.Design
{
    using System;

    internal class FileCodeModel2
    {
        private object _reference;
        private static Type _referencedType;

        public FileCodeModel2() : this(null)
        {
        }

        public FileCodeModel2(object reference)
        {
            this._reference = ReferencedType.IsInstanceOfType(reference) ? reference : null;
        }

        public AjaxControlToolkit.Design.CodeElements CodeElements
        {
            get
            {
                object reference = ReferencedType.GetProperty("CodeElements").GetValue(this._reference, new object[0]);
                if (reference == null)
                {
                    return null;
                }
                return new AjaxControlToolkit.Design.CodeElements(reference);
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
                    _referencedType = ReferencedAssemblies.EnvDTE80.GetType("EnvDTE80.FileCodeModel2");
                    if (_referencedType == null)
                    {
                        throw new InvalidOperationException("Failed to load type 'EnvDTE80.FileCodeModel2' from assembly 'EnvDTE80, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.");
                    }
                }
                return _referencedType;
            }
        }
    }
}

