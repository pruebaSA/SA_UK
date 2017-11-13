namespace System.Xml.Serialization
{
    using System;

    public class XmlTypeMapping : XmlMapping
    {
        internal XmlTypeMapping(TypeScope scope, ElementAccessor accessor) : base(scope, accessor)
        {
        }

        internal TypeMapping Mapping =>
            base.Accessor.Mapping;

        public string TypeFullName =>
            this.Mapping.TypeDesc.FullName;

        public string TypeName =>
            this.Mapping.TypeDesc.Name;

        public string XsdTypeName =>
            this.Mapping.TypeName;

        public string XsdTypeNamespace =>
            this.Mapping.Namespace;
    }
}

