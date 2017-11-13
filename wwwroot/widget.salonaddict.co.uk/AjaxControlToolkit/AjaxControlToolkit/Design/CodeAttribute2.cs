namespace AjaxControlToolkit.Design
{
    using System;

    internal class CodeAttribute2
    {
        private object _reference;
        private static Type _referencedType;

        public CodeAttribute2() : this(null)
        {
        }

        public CodeAttribute2(object reference)
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

        public static Type ReferencedType
        {
            get
            {
                if (_referencedType == null)
                {
                    _referencedType = ReferencedAssemblies.EnvDTE80.GetType("EnvDTE80.CodeAttribute2");
                    if (_referencedType == null)
                    {
                        throw new InvalidOperationException("Failed to load type 'EnvDTE80.CodeAttribute2' from assembly 'EnvDTE80, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.");
                    }
                }
                return _referencedType;
            }
        }
    }
}

