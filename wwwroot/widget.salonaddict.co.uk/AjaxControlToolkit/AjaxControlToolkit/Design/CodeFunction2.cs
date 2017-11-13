namespace AjaxControlToolkit.Design
{
    using System;

    internal class CodeFunction2
    {
        private object _reference;
        private static System.Type _referencedType;

        public CodeFunction2() : this(null)
        {
        }

        public CodeFunction2(object reference)
        {
            this._reference = ReferencedType.IsInstanceOfType(reference) ? reference : null;
        }

        public CodeAttribute2 AddAttribute(string Name, string Value, object Position)
        {
            object reference = ReferencedType.GetMethod("AddAttribute").Invoke(this._reference, new object[] { Name, Value, Position });
            if (reference == null)
            {
                return null;
            }
            return new CodeAttribute2(reference);
        }

        public CodeParameter2 AddParameter(string Name, object Type, object Position)
        {
            object reference = ReferencedType.GetMethod("AddParameter").Invoke(this._reference, new object[] { Name, Type, Position });
            if (reference == null)
            {
                return null;
            }
            return new CodeParameter2(reference);
        }

        public vsCMAccess Access
        {
            get
            {
                object reference = ReferencedType.GetProperty("Access").GetValue(this._reference, new object[0]);
                if (reference == null)
                {
                    return null;
                }
                return new vsCMAccess(reference);
            }
            set
            {
                ReferencedType.GetProperty("Access").SetValue(this._reference, value?.Reference, new object[0]);
            }
        }

        public CodeElements Attributes
        {
            get
            {
                object reference = ReferencedType.GetProperty("Attributes").GetValue(this._reference, new object[0]);
                if (reference == null)
                {
                    return null;
                }
                return new CodeElements(reference);
            }
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

        public bool IsShared
        {
            get => 
                ((bool) ReferencedType.GetProperty("IsShared").GetValue(this._reference, new object[0]));
            set
            {
                ReferencedType.GetProperty("IsShared").SetValue(this._reference, value, new object[0]);
            }
        }

        public CodeElements Parameters
        {
            get
            {
                object reference = ReferencedType.GetProperty("Parameters").GetValue(this._reference, new object[0]);
                if (reference == null)
                {
                    return null;
                }
                return new CodeElements(reference);
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
                    _referencedType = ReferencedAssemblies.EnvDTE80.GetType("EnvDTE80.CodeFunction2");
                    if (_referencedType == null)
                    {
                        throw new InvalidOperationException("Failed to load type 'EnvDTE80.CodeFunction2' from assembly 'EnvDTE80, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.");
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

