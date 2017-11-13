namespace System.Data.Linq.Mapping
{
    using System;

    public sealed class AttributeMappingSource : MappingSource
    {
        protected override MetaModel CreateModel(Type dataContextType)
        {
            if (dataContextType == null)
            {
                throw Error.ArgumentNull("dataContextType");
            }
            return new AttributedMetaModel(this, dataContextType);
        }
    }
}

