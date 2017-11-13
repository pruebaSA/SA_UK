namespace System.Data.Linq.Mapping
{
    using System;

    internal static class InheritanceBaseFinder
    {
        internal static MetaType FindBase(MetaType derivedType)
        {
            if (derivedType.Type == typeof(object))
            {
                return null;
            }
            Type baseType = derivedType.Type;
            Type type2 = derivedType.InheritanceRoot.Type;
            MetaTable table = derivedType.Table;
            MetaType inheritanceType = null;
            do
            {
                if ((baseType == typeof(object)) || (baseType == type2))
                {
                    return null;
                }
                baseType = baseType.BaseType;
                inheritanceType = derivedType.InheritanceRoot.GetInheritanceType(baseType);
            }
            while (inheritanceType == null);
            return inheritanceType;
        }
    }
}

