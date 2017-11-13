namespace System.Data.Linq.Mapping
{
    using LinqToSqlShared.Mapping;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Linq;
    using System.Data.Linq.SqlClient;
    using System.Linq;

    internal sealed class MappedRootType : MappedType
    {
        private Dictionary<Type, MetaType> derivedTypes;
        private bool hasInheritance;
        private Dictionary<object, MetaType> inheritanceCodes;
        private MetaType inheritanceDefault;
        private ReadOnlyCollection<MetaType> inheritanceTypes;

        public MappedRootType(MappedMetaModel model, MappedTable table, TypeMapping typeMapping, Type type) : base(model, table, typeMapping, type, null)
        {
            if (typeMapping == null)
            {
                throw System.Data.Linq.Mapping.Error.ArgumentNull("typeMapping");
            }
            if ((typeMapping.InheritanceCode != null) || (typeMapping.DerivedTypes.Count > 0))
            {
                if (this.Discriminator == null)
                {
                    throw System.Data.Linq.Mapping.Error.NoDiscriminatorFound(type.Name);
                }
                this.hasInheritance = true;
                if (!MappingSystem.IsSupportedDiscriminatorType(this.Discriminator.Type))
                {
                    throw System.Data.Linq.Mapping.Error.DiscriminatorClrTypeNotSupported(this.Discriminator.DeclaringType.Name, this.Discriminator.Name, this.Discriminator.Type);
                }
                this.derivedTypes = new Dictionary<Type, MetaType>();
                this.inheritanceCodes = new Dictionary<object, MetaType>();
                this.InitInheritedType(typeMapping, this);
            }
            if ((this.inheritanceDefault == null) && ((base.inheritanceCode != null) || ((this.inheritanceCodes != null) && (this.inheritanceCodes.Count > 0))))
            {
                throw System.Data.Linq.Mapping.Error.InheritanceHierarchyDoesNotDefineDefault(type);
            }
            if (this.derivedTypes != null)
            {
                this.inheritanceTypes = this.derivedTypes.Values.ToList<MetaType>().AsReadOnly();
            }
            else
            {
                this.inheritanceTypes = new MetaType[] { this }.ToList<MetaType>().AsReadOnly();
            }
            this.Validate();
        }

        public override MetaType GetInheritanceType(Type type)
        {
            if (type == this.Type)
            {
                return this;
            }
            MetaType type2 = null;
            if (this.derivedTypes != null)
            {
                this.derivedTypes.TryGetValue(type, out type2);
            }
            return type2;
        }

        private MetaType InitDerivedTypes(TypeMapping typeMap)
        {
            Type type = ((MappedMetaModel) this.Model).FindType(typeMap.Name);
            if (type == null)
            {
                throw System.Data.Linq.Mapping.Error.CouldNotFindRuntimeTypeForMapping(typeMap.Name);
            }
            MappedType type2 = new MappedType(this.Model, this.Table, typeMap, type, this);
            return this.InitInheritedType(typeMap, type2);
        }

        private MetaType InitInheritedType(TypeMapping typeMap, MappedType type)
        {
            this.derivedTypes.Add(type.Type, type);
            if (typeMap.InheritanceCode != null)
            {
                if (this.Discriminator == null)
                {
                    throw System.Data.Linq.Mapping.Error.NoDiscriminatorFound(type.Name);
                }
                if (type.Type.IsAbstract)
                {
                    throw System.Data.Linq.Mapping.Error.AbstractClassAssignInheritanceDiscriminator(type.Type);
                }
                object objB = DBConvert.ChangeType(typeMap.InheritanceCode, this.Discriminator.Type);
                foreach (object obj3 in this.inheritanceCodes.Keys)
                {
                    if ((((objB.GetType() == typeof(string)) && (((string) objB).Trim().Length == 0)) && ((obj3.GetType() == typeof(string)) && (((string) obj3).Trim().Length == 0))) || object.Equals(obj3, objB))
                    {
                        throw System.Data.Linq.Mapping.Error.InheritanceCodeUsedForMultipleTypes(objB);
                    }
                }
                if (type.inheritanceCode != null)
                {
                    throw System.Data.Linq.Mapping.Error.InheritanceTypeHasMultipleDiscriminators(type);
                }
                type.inheritanceCode = objB;
                this.inheritanceCodes.Add(objB, type);
                if (typeMap.IsInheritanceDefault)
                {
                    if (this.inheritanceDefault != null)
                    {
                        throw System.Data.Linq.Mapping.Error.InheritanceTypeHasMultipleDefaults(type);
                    }
                    this.inheritanceDefault = type;
                }
            }
            foreach (TypeMapping mapping in typeMap.DerivedTypes)
            {
                this.InitDerivedTypes(mapping);
            }
            return type;
        }

        private void Validate()
        {
            Dictionary<object, string> dictionary = new Dictionary<object, string>();
            foreach (MetaType type in this.InheritanceTypes)
            {
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
            this.hasInheritance;

        public override bool HasInheritanceCode =>
            (this.InheritanceCode != null);

        public override MetaType InheritanceDefault =>
            this.inheritanceDefault;

        public override ReadOnlyCollection<MetaType> InheritanceTypes =>
            this.inheritanceTypes;
    }
}

