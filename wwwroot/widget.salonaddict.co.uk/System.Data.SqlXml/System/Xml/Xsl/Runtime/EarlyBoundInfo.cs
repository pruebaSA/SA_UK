namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Reflection;

    internal sealed class EarlyBoundInfo
    {
        private ConstructorInfo constrInfo;
        private string namespaceUri;

        public EarlyBoundInfo(string namespaceUri, Type ebType)
        {
            this.namespaceUri = namespaceUri;
            this.constrInfo = ebType.GetConstructor(Type.EmptyTypes);
        }

        public object CreateObject() => 
            this.constrInfo.Invoke(new object[0]);

        public override bool Equals(object obj)
        {
            EarlyBoundInfo info = obj as EarlyBoundInfo;
            if (info == null)
            {
                return false;
            }
            return ((this.namespaceUri == info.namespaceUri) && (this.constrInfo == info.constrInfo));
        }

        public override int GetHashCode() => 
            this.namespaceUri.GetHashCode();

        public Type EarlyBoundType =>
            this.constrInfo.DeclaringType;

        public string NamespaceUri =>
            this.namespaceUri;
    }
}

