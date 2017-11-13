namespace System.Xml.Serialization
{
    using System;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization.Advanced;

    internal class TypeDesc
    {
        private TypeDesc arrayElementTypeDesc;
        private TypeDesc arrayTypeDesc;
        private TypeDesc baseTypeDesc;
        private string cSharpName;
        private XmlSchemaType dataType;
        private System.Exception exception;
        private MappedTypeDesc extendedType;
        private TypeFlags flags;
        private string formatterName;
        private string fullName;
        private bool isMixed;
        private bool isXsdType;
        private TypeKind kind;
        private string name;
        private TypeDesc nullableTypeDesc;
        private System.Type type;
        private int weight;

        internal TypeDesc(string name, string fullName, TypeKind kind, TypeDesc baseTypeDesc, TypeFlags flags) : this(name, fullName, null, kind, baseTypeDesc, flags, null)
        {
        }

        internal TypeDesc(System.Type type, bool isXsdType, XmlSchemaType dataType, string formatterName, TypeFlags flags) : this(type.Name, type.FullName, dataType, TypeKind.Primitive, null, flags, formatterName)
        {
            this.isXsdType = isXsdType;
            this.type = type;
        }

        internal TypeDesc(string name, string fullName, XmlSchemaType dataType, TypeKind kind, TypeDesc baseTypeDesc, TypeFlags flags) : this(name, fullName, dataType, kind, baseTypeDesc, flags, null)
        {
        }

        internal TypeDesc(string name, string fullName, XmlSchemaType dataType, TypeKind kind, TypeDesc baseTypeDesc, TypeFlags flags, string formatterName)
        {
            this.name = name.Replace('+', '.');
            this.fullName = fullName.Replace('+', '.');
            this.kind = kind;
            this.baseTypeDesc = baseTypeDesc;
            this.flags = flags;
            this.isXsdType = kind == TypeKind.Primitive;
            if (this.isXsdType)
            {
                this.weight = 1;
            }
            else if (kind == TypeKind.Enum)
            {
                this.weight = 2;
            }
            else if (this.kind == TypeKind.Root)
            {
                this.weight = -1;
            }
            else
            {
                this.weight = (baseTypeDesc == null) ? 0 : (baseTypeDesc.Weight + 1);
            }
            this.dataType = dataType;
            this.formatterName = formatterName;
        }

        internal TypeDesc(System.Type type, string name, string fullName, TypeKind kind, TypeDesc baseTypeDesc, TypeFlags flags, TypeDesc arrayElementTypeDesc) : this(name, fullName, null, kind, baseTypeDesc, flags, null)
        {
            this.arrayElementTypeDesc = arrayElementTypeDesc;
            this.type = type;
        }

        internal void CheckNeedConstructor()
        {
            if ((!this.IsValueType && !this.IsAbstract) && !this.HasDefaultConstructor)
            {
                this.flags |= TypeFlags.Unsupported;
                this.exception = new InvalidOperationException(Res.GetString("XmlConstructorInaccessible", new object[] { this.FullName }));
            }
        }

        internal void CheckSupported()
        {
            if (this.IsUnsupported)
            {
                if (this.Exception != null)
                {
                    throw this.Exception;
                }
                throw new NotSupportedException(Res.GetString("XmlSerializerUnsupportedType", new object[] { this.FullName }));
            }
            if (this.baseTypeDesc != null)
            {
                this.baseTypeDesc.CheckSupported();
            }
            if (this.arrayElementTypeDesc != null)
            {
                this.arrayElementTypeDesc.CheckSupported();
            }
        }

        internal TypeDesc CreateArrayTypeDesc()
        {
            if (this.arrayTypeDesc == null)
            {
                this.arrayTypeDesc = new TypeDesc(null, this.name + "[]", this.fullName + "[]", TypeKind.Array, null, TypeFlags.Reference | (this.flags & TypeFlags.UseReflection), this);
            }
            return this.arrayTypeDesc;
        }

        internal TypeDesc CreateMappedTypeDesc(MappedTypeDesc extension) => 
            new TypeDesc(extension.Name, extension.Name, null, this.kind, this.baseTypeDesc, this.flags, null) { 
                isXsdType = this.isXsdType,
                isMixed = this.isMixed,
                extendedType = extension,
                dataType = this.dataType
            };

        internal static TypeDesc FindCommonBaseTypeDesc(TypeDesc[] typeDescs)
        {
            if (typeDescs.Length == 0)
            {
                return null;
            }
            TypeDesc baseTypeDesc = null;
            int num = 0x7fffffff;
            for (int i = 0; i < typeDescs.Length; i++)
            {
                int weight = typeDescs[i].Weight;
                if (weight < num)
                {
                    num = weight;
                    baseTypeDesc = typeDescs[i];
                }
            }
            while (baseTypeDesc != null)
            {
                int index = 0;
                while (index < typeDescs.Length)
                {
                    if (!typeDescs[index].IsDerivedFrom(baseTypeDesc))
                    {
                        break;
                    }
                    index++;
                }
                if (index == typeDescs.Length)
                {
                    return baseTypeDesc;
                }
                baseTypeDesc = baseTypeDesc.BaseTypeDesc;
            }
            return baseTypeDesc;
        }

        internal TypeDesc GetNullableTypeDesc(System.Type type)
        {
            if (this.IsOptionalValue)
            {
                return this;
            }
            if (this.nullableTypeDesc == null)
            {
                this.nullableTypeDesc = new TypeDesc("NullableOf" + this.name, "System.Nullable`1[" + this.fullName + "]", null, TypeKind.Struct, this, this.flags | TypeFlags.OptionalValue, this.formatterName);
                this.nullableTypeDesc.type = type;
            }
            return this.nullableTypeDesc;
        }

        internal bool IsDerivedFrom(TypeDesc baseTypeDesc)
        {
            for (TypeDesc desc = this; desc != null; desc = desc.BaseTypeDesc)
            {
                if (desc == baseTypeDesc)
                {
                    return true;
                }
            }
            return baseTypeDesc.IsRoot;
        }

        public override string ToString() => 
            this.fullName;

        internal TypeDesc ArrayElementTypeDesc
        {
            get => 
                this.arrayElementTypeDesc;
            set
            {
                this.arrayElementTypeDesc = value;
            }
        }

        internal string ArrayLengthName
        {
            get
            {
                if (this.kind != TypeKind.Array)
                {
                    return "Count";
                }
                return "Length";
            }
        }

        internal TypeDesc BaseTypeDesc
        {
            get => 
                this.baseTypeDesc;
            set
            {
                this.baseTypeDesc = value;
                this.weight = (this.baseTypeDesc == null) ? 0 : (this.baseTypeDesc.Weight + 1);
            }
        }

        internal bool CanBeAttributeValue =>
            ((this.flags & TypeFlags.CanBeAttributeValue) != TypeFlags.None);

        internal bool CanBeElementValue =>
            ((this.flags & TypeFlags.CanBeElementValue) != TypeFlags.None);

        internal bool CanBeTextValue =>
            ((this.flags & TypeFlags.CanBeTextValue) != TypeFlags.None);

        internal bool CannotNew
        {
            get
            {
                if (this.HasDefaultConstructor)
                {
                    return this.ConstructorInaccessible;
                }
                return true;
            }
        }

        internal bool CollapseWhitespace =>
            ((this.flags & TypeFlags.CollapseWhitespace) != TypeFlags.None);

        internal bool ConstructorInaccessible =>
            ((this.flags & TypeFlags.CtorInaccessible) != TypeFlags.None);

        internal string CSharpName
        {
            get
            {
                if (this.cSharpName == null)
                {
                    this.cSharpName = (this.type == null) ? CodeIdentifier.GetCSharpName(this.fullName) : CodeIdentifier.GetCSharpName(this.type);
                }
                return this.cSharpName;
            }
        }

        internal XmlSchemaType DataType =>
            this.dataType;

        internal System.Exception Exception
        {
            get => 
                this.exception;
            set
            {
                this.exception = value;
            }
        }

        internal MappedTypeDesc ExtendedType =>
            this.extendedType;

        internal TypeFlags Flags =>
            this.flags;

        internal string FormatterName =>
            this.formatterName;

        internal string FullName =>
            this.fullName;

        internal bool HasCustomFormatter =>
            ((this.flags & TypeFlags.HasCustomFormatter) != TypeFlags.None);

        internal bool HasDefaultConstructor =>
            ((this.flags & TypeFlags.HasDefaultConstructor) != TypeFlags.None);

        internal bool HasDefaultSupport =>
            ((this.flags & TypeFlags.IgnoreDefault) == TypeFlags.None);

        internal bool HasIsEmpty =>
            ((this.flags & TypeFlags.HasIsEmpty) != TypeFlags.None);

        internal bool IsAbstract =>
            ((this.flags & TypeFlags.Abstract) != TypeFlags.None);

        internal bool IsAmbiguousDataType =>
            ((this.flags & TypeFlags.AmbiguousDataType) != TypeFlags.None);

        internal bool IsArray =>
            (this.kind == TypeKind.Array);

        internal bool IsArrayLike
        {
            get
            {
                if ((this.kind != TypeKind.Array) && (this.kind != TypeKind.Collection))
                {
                    return (this.kind == TypeKind.Enumerable);
                }
                return true;
            }
        }

        internal bool IsClass =>
            (this.kind == TypeKind.Class);

        internal bool IsCollection =>
            (this.kind == TypeKind.Collection);

        internal bool IsEnum =>
            (this.kind == TypeKind.Enum);

        internal bool IsEnumerable =>
            (this.kind == TypeKind.Enumerable);

        internal bool IsGenericInterface =>
            ((this.flags & TypeFlags.GenericInterface) != TypeFlags.None);

        internal bool IsMappedType =>
            (this.extendedType != null);

        internal bool IsMixed
        {
            get
            {
                if (!this.isMixed)
                {
                    return this.CanBeTextValue;
                }
                return true;
            }
            set
            {
                this.isMixed = value;
            }
        }

        internal bool IsNullable =>
            !this.IsValueType;

        internal bool IsOptionalValue =>
            ((this.flags & TypeFlags.OptionalValue) != TypeFlags.None);

        internal bool IsPrimitive =>
            (this.kind == TypeKind.Primitive);

        internal bool IsPrivateImplementation =>
            ((this.flags & TypeFlags.UsePrivateImplementation) != TypeFlags.None);

        internal bool IsRoot =>
            (this.kind == TypeKind.Root);

        internal bool IsSpecial =>
            ((this.flags & TypeFlags.Special) != TypeFlags.None);

        internal bool IsStructLike
        {
            get
            {
                if (this.kind != TypeKind.Struct)
                {
                    return (this.kind == TypeKind.Class);
                }
                return true;
            }
        }

        internal bool IsUnsupported =>
            ((this.flags & TypeFlags.Unsupported) != TypeFlags.None);

        internal bool IsValueType =>
            ((this.flags & TypeFlags.Reference) == TypeFlags.None);

        internal bool IsVoid =>
            (this.kind == TypeKind.Void);

        internal bool IsXsdType =>
            this.isXsdType;

        internal TypeKind Kind =>
            this.kind;

        internal string Name =>
            this.name;

        internal System.Type Type =>
            this.type;

        internal bool UseReflection =>
            ((this.flags & TypeFlags.UseReflection) != TypeFlags.None);

        internal int Weight =>
            this.weight;

        internal bool XmlEncodingNotRequired =>
            ((this.flags & TypeFlags.XmlEncodingNotRequired) != TypeFlags.None);
    }
}

