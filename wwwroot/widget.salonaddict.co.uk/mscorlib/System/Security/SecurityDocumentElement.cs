namespace System.Security
{
    using System;

    [Serializable]
    internal sealed class SecurityDocumentElement : ISecurityElementFactory
    {
        private SecurityDocument m_document;
        private int m_position;

        internal SecurityDocumentElement(SecurityDocument document, int position)
        {
            this.m_document = document;
            this.m_position = position;
        }

        string ISecurityElementFactory.Attribute(string attributeName) => 
            this.m_document.GetAttributeForElement(this.m_position, attributeName);

        object ISecurityElementFactory.Copy() => 
            new SecurityDocumentElement(this.m_document, this.m_position);

        SecurityElement ISecurityElementFactory.CreateSecurityElement() => 
            this.m_document.GetElement(this.m_position, true);

        string ISecurityElementFactory.GetTag() => 
            this.m_document.GetTagForElement(this.m_position);
    }
}

