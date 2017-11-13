namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Xml;

    internal class UsingElement : SchemaElement
    {
        private string _alias;
        private string _namespaceName;

        internal UsingElement(Schema parentElement) : base(parentElement)
        {
        }

        private void HandleAliasAttribute(XmlReader reader)
        {
            this.Alias = base.HandleUndottedNameAttribute(reader, this.Alias);
        }

        protected override bool HandleAttribute(XmlReader reader)
        {
            if (base.HandleAttribute(reader))
            {
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Namespace"))
            {
                this.HandleNamespaceAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Alias"))
            {
                this.HandleAliasAttribute(reader);
                return true;
            }
            return false;
        }

        private void HandleNamespaceAttribute(XmlReader reader)
        {
            ReturnValue<string> value2 = base.HandleDottedNameAttribute(reader, this.NamespaceName, new Func<object, string>(Strings.AlreadyDefined));
            if (value2.Succeeded)
            {
                this.NamespaceName = value2.Value;
            }
        }

        protected override bool ProhibitAttribute(string namespaceUri, string localName) => 
            (base.ProhibitAttribute(namespaceUri, localName) || (((namespaceUri == null) && (localName == "Name")) && false));

        public string Alias
        {
            virtual get => 
                this._alias;
            private set
            {
                this._alias = value;
            }
        }

        public override string FQName =>
            null;

        public string NamespaceName
        {
            virtual get => 
                this._namespaceName;
            private set
            {
                this._namespaceName = value;
            }
        }
    }
}

