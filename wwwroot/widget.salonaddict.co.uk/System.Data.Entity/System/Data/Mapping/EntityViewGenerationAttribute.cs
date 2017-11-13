namespace System.Data.Mapping
{
    using System;
    using System.Data;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public sealed class EntityViewGenerationAttribute : Attribute
    {
        private Type m_viewGenType;

        public EntityViewGenerationAttribute(Type viewGenerationType)
        {
            EntityUtil.CheckArgumentNull<Type>(viewGenerationType, "viewGenType");
            this.m_viewGenType = viewGenerationType;
        }

        public Type ViewGenerationType =>
            this.m_viewGenType;
    }
}

