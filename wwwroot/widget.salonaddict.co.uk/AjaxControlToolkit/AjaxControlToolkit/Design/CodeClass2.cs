namespace AjaxControlToolkit.Design
{
    using System;

    internal class CodeClass2
    {
        private object _reference;
        private static Type _referencedType;

        public CodeClass2() : this(null)
        {
        }

        public CodeClass2(object reference)
        {
            this._reference = ReferencedType.IsInstanceOfType(reference) ? reference : null;
        }

        public CodeFunction2 AddFunction(string Name, vsCMFunction Kind, object Type, object Position, vsCMAccess Access, object Location)
        {
            object reference = ReferencedType.GetMethod("AddFunction").Invoke(this._reference, new object[] { Name, Kind?.Reference, Type, Position, Access?.Reference, Location });
            if (reference == null)
            {
                return null;
            }
            return new CodeFunction2(reference);
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
                    _referencedType = ReferencedAssemblies.EnvDTE80.GetType("EnvDTE80.CodeClass2");
                    if (_referencedType == null)
                    {
                        throw new InvalidOperationException("Failed to load type 'EnvDTE80.CodeClass2' from assembly 'EnvDTE80, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.");
                    }
                }
                return _referencedType;
            }
        }
    }
}

