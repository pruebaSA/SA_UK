namespace System.Xml.Serialization
{
    using System;
    using System.CodeDom.Compiler;

    public class XmlMemberMapping
    {
        private MemberMapping mapping;

        internal XmlMemberMapping(MemberMapping mapping)
        {
            this.mapping = mapping;
        }

        public string GenerateTypeName(CodeDomProvider codeProvider) => 
            this.mapping.GetTypeName(codeProvider);

        internal System.Xml.Serialization.Accessor Accessor =>
            this.mapping.Accessor;

        public bool Any =>
            this.Accessor.Any;

        public bool CheckSpecified =>
            (this.mapping.CheckSpecified != SpecifiedAccessor.None);

        public string ElementName =>
            System.Xml.Serialization.Accessor.UnescapeName(this.Accessor.Name);

        internal bool IsNullable =>
            this.mapping.IsNeedNullable;

        internal MemberMapping Mapping =>
            this.mapping;

        public string MemberName =>
            this.mapping.Name;

        public string Namespace =>
            this.Accessor.Namespace;

        public string TypeFullName =>
            this.mapping.TypeDesc.FullName;

        public string TypeName =>
            this.Accessor.Mapping?.TypeName;

        public string TypeNamespace =>
            this.Accessor.Mapping?.Namespace;

        public string XsdElementName =>
            this.Accessor.Name;
    }
}

