namespace System.ComponentModel.Design.Serialization
{
    using System;

    [Obsolete("This attribute has been deprecated. Use DesignerSerializerAttribute instead.  For example, to specify a root designer for CodeDom, use DesignerSerializerAttribute(...,typeof(TypeCodeDomSerializer)).  http://go.microsoft.com/fwlink/?linkid=14202"), AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
    public sealed class RootDesignerSerializerAttribute : Attribute
    {
        private bool reloadable;
        private string serializerBaseTypeName;
        private string serializerTypeName;
        private string typeId;

        public RootDesignerSerializerAttribute(string serializerTypeName, string baseSerializerTypeName, bool reloadable)
        {
            this.serializerTypeName = serializerTypeName;
            this.serializerBaseTypeName = baseSerializerTypeName;
            this.reloadable = reloadable;
        }

        public RootDesignerSerializerAttribute(string serializerTypeName, Type baseSerializerType, bool reloadable)
        {
            this.serializerTypeName = serializerTypeName;
            this.serializerBaseTypeName = baseSerializerType.AssemblyQualifiedName;
            this.reloadable = reloadable;
        }

        public RootDesignerSerializerAttribute(Type serializerType, Type baseSerializerType, bool reloadable)
        {
            this.serializerTypeName = serializerType.AssemblyQualifiedName;
            this.serializerBaseTypeName = baseSerializerType.AssemblyQualifiedName;
            this.reloadable = reloadable;
        }

        public bool Reloadable =>
            this.reloadable;

        public string SerializerBaseTypeName =>
            this.serializerBaseTypeName;

        public string SerializerTypeName =>
            this.serializerTypeName;

        public override object TypeId
        {
            get
            {
                if (this.typeId == null)
                {
                    string serializerBaseTypeName = this.serializerBaseTypeName;
                    int index = serializerBaseTypeName.IndexOf(',');
                    if (index != -1)
                    {
                        serializerBaseTypeName = serializerBaseTypeName.Substring(0, index);
                    }
                    this.typeId = base.GetType().FullName + serializerBaseTypeName;
                }
                return this.typeId;
            }
        }
    }
}

