namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Data.Common.Utils;

    internal class TypeReference
    {
        private static CodeTypeReference _byteArray;
        private static CodeTypeReference _dateTime;
        private readonly System.Data.Common.Utils.Memoizer<Type, CodeTypeReference> _forTypeMemoizer;
        private readonly System.Data.Common.Utils.Memoizer<KeyValuePair<string, CodeTypeReference>, CodeTypeReference> _fromStringGenericMemoizer;
        private readonly System.Data.Common.Utils.Memoizer<KeyValuePair<string, bool>, CodeTypeReference> _fromStringMemoizer;
        private static CodeTypeReference _guid;
        private readonly System.Data.Common.Utils.Memoizer<Type, CodeTypeReference> _nullableForTypeMemoizer;
        private static CodeTypeReference _objectContext;
        public const string FQMetaDataWorkspaceTypeName = "System.Data.Metadata.Edm.MetadataWorkspace";
        internal static readonly Type ObjectContextBaseClassType = typeof(DataServiceContext);

        internal TypeReference()
        {
            this._forTypeMemoizer = new System.Data.Common.Utils.Memoizer<Type, CodeTypeReference>(new Func<Type, CodeTypeReference>(this.ComputeForType), null);
            this._fromStringMemoizer = new System.Data.Common.Utils.Memoizer<KeyValuePair<string, bool>, CodeTypeReference>(new Func<KeyValuePair<string, bool>, CodeTypeReference>(this.ComputeFromString), null);
            this._nullableForTypeMemoizer = new System.Data.Common.Utils.Memoizer<Type, CodeTypeReference>(new Func<Type, CodeTypeReference>(this.ComputeNullableForType), null);
            this._fromStringGenericMemoizer = new System.Data.Common.Utils.Memoizer<KeyValuePair<string, CodeTypeReference>, CodeTypeReference>(new Func<KeyValuePair<string, CodeTypeReference>, CodeTypeReference>(this.ComputeFromStringGeneric), null);
        }

        public CodeTypeReference AdoFrameworkGenericClass(string name, CodeTypeReference typeParameter) => 
            this.FrameworkGenericClass("System.Data.Services.Client", name, typeParameter);

        public CodeTypeReference AdoFrameworkType(string name) => 
            this.FromString("System.Data.Services.Client." + name, true);

        private CodeTypeReference ComputeForType(Type type) => 
            new CodeTypeReference(type, CodeTypeReferenceOptions.GlobalReference);

        private CodeTypeReference ComputeFromString(KeyValuePair<string, bool> arguments)
        {
            string key = arguments.Key;
            if (arguments.Value)
            {
                return new CodeTypeReference(key, CodeTypeReferenceOptions.GlobalReference);
            }
            return new CodeTypeReference(key);
        }

        private CodeTypeReference ComputeFromStringGeneric(KeyValuePair<string, CodeTypeReference> arguments)
        {
            string key = arguments.Key;
            CodeTypeReference reference = arguments.Value;
            CodeTypeReference reference2 = this.ComputeFromString(new KeyValuePair<string, bool>(key, true));
            reference2.TypeArguments.Add(reference);
            return reference2;
        }

        private CodeTypeReference ComputeNullableForType(Type innerType)
        {
            CodeTypeReference reference = new CodeTypeReference(typeof(Nullable<>), CodeTypeReferenceOptions.GlobalReference);
            reference.TypeArguments.Add(this.ForType(innerType));
            return reference;
        }

        public CodeTypeReference ForType(Type type) => 
            this._forTypeMemoizer.Evaluate(type);

        public CodeTypeReference ForType(Type generic, params CodeTypeReference[] argument)
        {
            CodeTypeReference reference = new CodeTypeReference(generic, CodeTypeReferenceOptions.GlobalReference);
            if ((argument != null) && (0 < argument.Length))
            {
                reference.TypeArguments.AddRange(argument);
            }
            return reference;
        }

        public CodeTypeReference FrameworkGenericClass(string fullname, CodeTypeReference typeParameter) => 
            this._fromStringGenericMemoizer.Evaluate(new KeyValuePair<string, CodeTypeReference>(fullname, typeParameter));

        public CodeTypeReference FrameworkGenericClass(string namespaceName, string name, CodeTypeReference typeParameter) => 
            this.FrameworkGenericClass(namespaceName + "." + name, typeParameter);

        public CodeTypeReference FromString(string type) => 
            this.FromString(type, false);

        public CodeTypeReference FromString(string type, bool addGlobalQualifier) => 
            this._fromStringMemoizer.Evaluate(new KeyValuePair<string, bool>(type, addGlobalQualifier));

        public CodeTypeReference NullableForType(Type innerType) => 
            this._nullableForTypeMemoizer.Evaluate(innerType);

        public CodeTypeReference ByteArray
        {
            get
            {
                if (_byteArray == null)
                {
                    _byteArray = this.ForType(typeof(byte[]));
                }
                return _byteArray;
            }
        }

        public CodeTypeReference DateTime
        {
            get
            {
                if (_dateTime == null)
                {
                    _dateTime = this.ForType(typeof(System.DateTime));
                }
                return _dateTime;
            }
        }

        public CodeTypeReference Guid
        {
            get
            {
                if (_guid == null)
                {
                    _guid = this.ForType(typeof(System.Guid));
                }
                return _guid;
            }
        }

        public CodeTypeReference ObjectContext
        {
            get
            {
                if (_objectContext == null)
                {
                    _objectContext = this.AdoFrameworkType("DataServiceContext");
                }
                return _objectContext;
            }
        }
    }
}

