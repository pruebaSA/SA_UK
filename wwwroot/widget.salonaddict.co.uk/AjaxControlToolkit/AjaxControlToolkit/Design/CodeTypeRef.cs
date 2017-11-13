namespace AjaxControlToolkit.Design
{
    using System;

    internal class CodeTypeRef
    {
        private object _reference;
        private static Type _referencedType;

        public CodeTypeRef() : this(null)
        {
        }

        public CodeTypeRef(object reference)
        {
            this._reference = ReferencedType.IsInstanceOfType(reference) ? reference : null;
        }

        public string AsFullName =>
            ((string) ReferencedType.GetProperty("AsFullName").GetValue(this._reference, new object[0]));

        public CodeTypeRef ElementType
        {
            get
            {
                object reference = ReferencedType.GetProperty("ElementType").GetValue(this._reference, new object[0]);
                if (reference == null)
                {
                    return null;
                }
                return new CodeTypeRef(reference);
            }
            set
            {
                ReferencedType.GetProperty("ElementType").SetValue(this._reference, value?.Reference, new object[0]);
            }
        }

        public int Rank
        {
            get => 
                ((int) ReferencedType.GetProperty("Rank").GetValue(this._reference, new object[0]));
            set
            {
                ReferencedType.GetProperty("Rank").SetValue(this._reference, value, new object[0]);
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
                    _referencedType = ReferencedAssemblies.EnvDTE.GetType("EnvDTE.CodeTypeRef");
                    if (_referencedType == null)
                    {
                        throw new InvalidOperationException("Failed to load type 'EnvDTE.CodeTypeRef' from assembly 'EnvDTE, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.");
                    }
                }
                return _referencedType;
            }
        }

        public vsCMTypeRef TypeKind
        {
            get
            {
                object reference = ReferencedType.GetProperty("TypeKind").GetValue(this._reference, new object[0]);
                if (reference == null)
                {
                    return null;
                }
                return new vsCMTypeRef(reference);
            }
        }
    }
}

