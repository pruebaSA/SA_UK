namespace AjaxControlToolkit.Design
{
    using System;

    internal class CodeElement2
    {
        private object _reference;
        private static Type _referencedType;

        public CodeElement2() : this(null)
        {
        }

        public CodeElement2(object reference)
        {
            this._reference = ReferencedType.IsInstanceOfType(reference) ? reference : null;
        }

        public CodeElements Children
        {
            get
            {
                object reference = ReferencedType.GetProperty("Children").GetValue(this._reference, new object[0]);
                if (reference == null)
                {
                    return null;
                }
                return new CodeElements(reference);
            }
        }

        public string FullName =>
            ((string) ReferencedType.GetProperty("FullName").GetValue(this._reference, new object[0]));

        public bool IsCodeType =>
            ((bool) ReferencedType.GetProperty("IsCodeType").GetValue(this._reference, new object[0]));

        public vsCMElement Kind
        {
            get
            {
                object reference = ReferencedType.GetProperty("Kind").GetValue(this._reference, new object[0]);
                if (reference == null)
                {
                    return null;
                }
                return new vsCMElement(reference);
            }
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
                    _referencedType = ReferencedAssemblies.EnvDTE80.GetType("EnvDTE80.CodeElement2");
                    if (_referencedType == null)
                    {
                        throw new InvalidOperationException("Failed to load type 'EnvDTE80.CodeElement2' from assembly 'EnvDTE80, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.");
                    }
                }
                return _referencedType;
            }
        }
    }
}

