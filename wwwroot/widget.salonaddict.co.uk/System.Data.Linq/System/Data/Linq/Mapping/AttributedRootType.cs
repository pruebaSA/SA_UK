namespace System.Data.Linq.Mapping
{
    using LinqToSqlShared.Mapping;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Linq;
    using System.Data.Linq.SqlClient;
    using System.Linq;

    internal sealed class AttributedRootType : AttributedMetaType
    {
        private Dictionary<object, MetaType> codeMap;
        private MetaType inheritanceDefault;
        private ReadOnlyCollection<MetaType> inheritanceTypes;
        private Dictionary<Type, MetaType> types;

        internal AttributedRootType(AttributedMetaModel model, AttributedMetaTable table, Type type) : base(model, table, type, null)
        {
            InheritanceMappingAttribute[] customAttributes = (InheritanceMappingAttribute[]) type.GetCustomAttributes(typeof(InheritanceMappingAttribute), true);
            if (customAttributes.Length > 0)
            {
                if (this.Discriminator == null)
                {
                    throw System.Data.Linq.Mapping.Error.NoDiscriminatorFound(type);
                }
                if (!MappingSystem.IsSupportedDiscriminatorType(this.Discriminator.Type))
                {
                    throw System.Data.Linq.Mapping.Error.DiscriminatorClrTypeNotSupported(this.Discriminator.DeclaringType.Name, this.Discriminator.Name, this.Discriminator.Type);
                }
                this.types = new Dictionary<Type, MetaType>();
                this.types.Add(type, this);
                this.codeMap = new Dictionary<object, MetaType>();
                foreach (InheritanceMappingAttribute attribute in customAttributes)
                {
                    if (!type.IsAssignableFrom(attribute.Type))
                    {
                        throw System.Data.Linq.Mapping.Error.InheritanceTypeDoesNotDeriveFromRoot(attribute.Type, type);
                    }
                    if (attribute.Type.IsAbstract)
                    {
                        throw System.Data.Linq.Mapping.Error.AbstractClassAssignInheritanceDiscriminator(attribute.Type);
                    }
                    AttributedMetaType type2 = this.CreateInheritedType(type, attribute.Type);
                    if (attribute.Code == null)
                    {
                        throw System.Data.Linq.Mapping.Error.InheritanceCodeMayNotBeNull();
                    }
                    if (type2.inheritanceCode != null)
                    {
                        throw System.Data.Linq.Mapping.Error.InheritanceTypeHasMultipleDiscriminators(attribute.Type);
                    }
                    object objB = DBConvert.ChangeType(attribute.Code, this.Discriminator.Type);
                    foreach (object obj3 in this.codeMap.Keys)
                    {
                        if ((((objB.GetType() == typeof(string)) && (((string) objB).Trim().Length == 0)) && ((obj3.GetType() == typeof(string)) && (((string) obj3).Trim().Length == 0))) || object.Equals(obj3, objB))
                        {
                            throw System.Data.Linq.Mapping.Error.InheritanceCodeUsedForMultipleTypes(objB);
                        }
                    }
                    type2.inheritanceCode = objB;
                    this.codeMap.Add(objB, type2);
                    if (attribute.IsDefault)
                    {
                        if (this.inheritanceDefault != null)
                        {
                            throw System.Data.Linq.Mapping.Error.InheritanceTypeHasMultipleDefaults(type);
                        }
                        this.inheritanceDefault = type2;
                    }
                }
                if (this.inheritanceDefault == null)
                {
                    throw System.Data.Linq.Mapping.Error.InheritanceHierarchyDoesNotDefineDefault(type);
                }
            }
            if (this.types != null)
            {
                this.inheritanceTypes = this.types.Values.ToList<MetaType>().AsReadOnly();
            }
            else
            {
                this.inheritanceTypes = new MetaType[] { this }.ToList<MetaType>().AsReadOnly();
            }
            this.Validate();
        }

        private AttributedMetaType CreateInheritedType(Type root, Type type)
        {
            MetaType type2;
            if (!this.types.TryGetValue(type, out type2))
            {
                type2 = new AttributedMetaType(this.Model, this.Table, type, this);
                this.types.Add(type, type2);
                if ((type != root) && (type.BaseType != typeof(object)))
                {
                    this.CreateInheritedType(root, type.BaseType);
                }
            }
            return (AttributedMetaType) type2;
        }

        public override MetaType GetInheritanceType(Type type)
        {
            if (type == this.Type)
            {
                return this;
            }
            MetaType type2 = null;
            if (this.types != null)
            {
                this.types.TryGetValue(type, out type2);
            }
            return type2;
        }

        private void Validate()
        {
            Dictionary<object, string> dictionary = new Dictionary<object, string>();
            foreach (MetaType type in this.InheritanceTypes)
            {
                if (type != this)
                {
                    TableAttribute[] customAttributes = (TableAttribute[]) type.Type.GetCustomAttributes(typeof(TableAttribute), false);
                    if (customAttributes.Length > 0)
                    {
                        throw System.Data.Linq.Mapping.Error.InheritanceSubTypeIsAlsoRoot(type.Type);
                    }
                }
                foreach (MetaDataMember member in type.PersistentDataMembers)
                {
                    if (member.IsDeclaredBy(type))
                    {
                        if (member.IsDiscriminator && !this.HasInheritance)
                        {
                            throw System.Data.Linq.Mapping.Error.NonInheritanceClassHasDiscriminator(type);
                        }
                        if (!member.IsAssociation && !string.IsNullOrEmpty(member.MappedName))
                        {
                            string str;
                            object key = InheritanceRules.DistinguishedMemberName(member.Member);
                            if (dictionary.TryGetValue(key, out str))
                            {
                                if (str != member.MappedName)
                                {
                                    throw System.Data.Linq.Mapping.Error.MemberMappedMoreThanOnce(member.Member.Name);
                                }
                            }
                            else
                            {
                                dictionary.Add(key, member.MappedName);
                            }
                        }
                    }
                }
            }
        }

        public override bool HasInheritance =>
            (this.types != null);

        public override MetaType InheritanceDefault =>
            this.inheritanceDefault;

        public override ReadOnlyCollection<MetaType> InheritanceTypes =>
            this.inheritanceTypes;
    }
}

