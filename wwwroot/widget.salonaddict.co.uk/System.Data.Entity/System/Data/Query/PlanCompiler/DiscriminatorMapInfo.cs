namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;

    internal class DiscriminatorMapInfo
    {
        internal ExplicitDiscriminatorMap DiscriminatorMap;
        internal bool IncludesSubTypes;
        internal EntityTypeBase RootEntityType;

        internal DiscriminatorMapInfo(EntityTypeBase rootEntityType, bool includesSubTypes, ExplicitDiscriminatorMap discriminatorMap)
        {
            this.RootEntityType = rootEntityType;
            this.IncludesSubTypes = includesSubTypes;
            this.DiscriminatorMap = discriminatorMap;
        }

        internal void Merge(EntityTypeBase neededRootEntityType, bool includesSubtypes, ExplicitDiscriminatorMap discriminatorMap)
        {
            if ((this.RootEntityType != neededRootEntityType) || (this.IncludesSubTypes != includesSubtypes))
            {
                if (!this.IncludesSubTypes || !includesSubtypes)
                {
                    this.DiscriminatorMap = null;
                }
                if (TypeSemantics.IsSubTypeOf(this.RootEntityType, neededRootEntityType))
                {
                    this.RootEntityType = neededRootEntityType;
                    this.DiscriminatorMap = discriminatorMap;
                }
                if (!TypeSemantics.IsSubTypeOf(neededRootEntityType, this.RootEntityType))
                {
                    this.DiscriminatorMap = null;
                }
            }
        }
    }
}

