namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.Xml.Serialization;

    internal class ReferencedCollectionType
    {
        private CollectionCategory m_Category;
        private string m_TypeName;

        [XmlAttribute]
        public CollectionCategory Category
        {
            get => 
                this.m_Category;
            set
            {
                this.m_Category = value;
            }
        }

        [XmlAttribute]
        public string TypeName
        {
            get => 
                this.m_TypeName;
            set
            {
                this.m_TypeName = value;
            }
        }

        public enum CollectionCategory
        {
            [XmlEnum(Name="Dictionary")]
            Dictionary = 2,
            [XmlEnum(Name="List")]
            List = 1,
            [XmlEnum(Name="Unknown")]
            Unknown = 0
        }
    }
}

