namespace LinqToSqlShared.Mapping
{
    using System;
    using System.Collections.Generic;

    internal class TypeMapping
    {
        private TypeMapping baseType;
        private List<TypeMapping> derivedTypes = new List<TypeMapping>();
        private string inheritanceCode;
        private bool isInheritanceDefault;
        private List<MemberMapping> members = new List<MemberMapping>();
        private string name;

        internal TypeMapping()
        {
        }

        internal TypeMapping BaseType
        {
            get => 
                this.baseType;
            set
            {
                this.baseType = value;
            }
        }

        internal List<TypeMapping> DerivedTypes =>
            this.derivedTypes;

        internal string InheritanceCode
        {
            get => 
                this.inheritanceCode;
            set
            {
                this.inheritanceCode = value;
            }
        }

        internal bool IsInheritanceDefault
        {
            get => 
                this.isInheritanceDefault;
            set
            {
                this.isInheritanceDefault = value;
            }
        }

        internal List<MemberMapping> Members =>
            this.members;

        internal string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }

        internal string XmlIsInheritanceDefault
        {
            get
            {
                if (!this.isInheritanceDefault)
                {
                    return null;
                }
                return "true";
            }
            set
            {
                this.isInheritanceDefault = (value != null) ? bool.Parse(value) : false;
            }
        }
    }
}

