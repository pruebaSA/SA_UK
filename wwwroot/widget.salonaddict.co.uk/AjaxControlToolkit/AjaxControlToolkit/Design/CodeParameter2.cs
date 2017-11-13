namespace AjaxControlToolkit.Design
{
    using System;

    internal class CodeParameter2
    {
        private object _reference;
        private static System.Type _referencedType;

        public CodeParameter2() : this(null)
        {
        }

        public CodeParameter2(object reference)
        {
            this._reference = ReferencedType.IsInstanceOfType(reference) ? reference : null;
        }

        public string Name
        {
            get => 
                ((string) ReferencedType.GetProperty("Name").GetValue(this._reference, new object[0]));
            set
            {
                ReferencedType.GetProperty("Name").SetValue(this._reference, value, new object[0]);
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

        public static System.Type ReferencedType
        {
            get
            {
                if (_referencedType == null)
                {
                    _referencedType = ReferencedAssemblies.EnvDTE80.GetType("EnvDTE80.CodeParameter2");
                    if (_referencedType == null)
                    {
                        throw new InvalidOperationException("Failed to load type 'EnvDTE80.CodeParameter2' from assembly 'EnvDTE80, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.");
                    }
                }
                return _referencedType;
            }
        }

        public CodeTypeRef Type
        {
            get
            {
                object reference = ReferencedType.GetProperty("Type").GetValue(this._reference, new object[0]);
                if (reference == null)
                {
                    return null;
                }
                return new CodeTypeRef(reference);
            }
            set
            {
                ReferencedType.GetProperty("Type").SetValue(this._reference, value?.Reference, new object[0]);
            }
        }
    }
}

