namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Runtime.InteropServices;

    internal class CacheForPrimitiveTypes
    {
        private List<PrimitiveType>[] _primitiveTypeMap = new List<PrimitiveType>[15];

        internal void Add(PrimitiveType type)
        {
            List<PrimitiveType> list = EntityUtil.CheckArgumentOutOfRange<List<PrimitiveType>>(this._primitiveTypeMap, (int) type.PrimitiveTypeKind, "primitiveTypeKind");
            if (list == null)
            {
                list = new List<PrimitiveType> {
                    type
                };
                this._primitiveTypeMap[(int) type.PrimitiveTypeKind] = list;
            }
            else
            {
                list.Add(type);
            }
        }

        private static Facet[] CreateInitialFacets(FacetDescription[] facetDescriptions)
        {
            Facet[] facetArray = new Facet[facetDescriptions.Length];
            for (int i = 0; i < facetDescriptions.Length; i++)
            {
                string facetName = facetDescriptions[i].FacetName;
                if (facetName != null)
                {
                    if (facetName != "MaxLength")
                    {
                        if (facetName == "Unicode")
                        {
                            goto Label_0074;
                        }
                        if (facetName == "FixedLength")
                        {
                            goto Label_008B;
                        }
                        if (facetName == "Precision")
                        {
                            goto Label_00A2;
                        }
                        if (facetName == "Scale")
                        {
                            goto Label_00B4;
                        }
                    }
                    else
                    {
                        facetArray[i] = Facet.Create(facetDescriptions[i], TypeUsage.DefaultMaxLengthFacetValue);
                    }
                }
                continue;
            Label_0074:
                facetArray[i] = Facet.Create(facetDescriptions[i], TypeUsage.DefaultUnicodeFacetValue);
                continue;
            Label_008B:
                facetArray[i] = Facet.Create(facetDescriptions[i], TypeUsage.DefaultFixedLengthFacetValue);
                continue;
            Label_00A2:
                facetArray[i] = Facet.Create(facetDescriptions[i], TypeUsage.DefaultPrecisionFacetValue);
                continue;
            Label_00B4:
                facetArray[i] = Facet.Create(facetDescriptions[i], TypeUsage.DefaultScaleFacetValue);
            }
            return facetArray;
        }

        internal ReadOnlyCollection<PrimitiveType> GetTypes()
        {
            List<PrimitiveType> list = new List<PrimitiveType>();
            foreach (List<PrimitiveType> list2 in this._primitiveTypeMap)
            {
                if (list2 != null)
                {
                    list.AddRange(list2);
                }
            }
            return list.AsReadOnly();
        }

        internal bool TryGetType(PrimitiveTypeKind primitiveTypeKind, IEnumerable<Facet> facets, out PrimitiveType type)
        {
            type = null;
            List<PrimitiveType> list = EntityUtil.CheckArgumentOutOfRange<List<PrimitiveType>>(this._primitiveTypeMap, (int) primitiveTypeKind, "primitiveTypeKind");
            if ((list == null) || (0 >= list.Count))
            {
                return false;
            }
            if (list.Count == 1)
            {
                type = list[0];
                return true;
            }
            if (facets == null)
            {
                FacetDescription[] initialFacetDescriptions = EdmProviderManifest.GetInitialFacetDescriptions(primitiveTypeKind);
                if (initialFacetDescriptions == null)
                {
                    type = list[0];
                    return true;
                }
                facets = CreateInitialFacets(initialFacetDescriptions);
            }
            bool flag = false;
            foreach (Facet facet in facets)
            {
                if (((primitiveTypeKind == PrimitiveTypeKind.String) || (primitiveTypeKind == PrimitiveTypeKind.Binary)) && (((facet.Value != null) && (facet.Name == "MaxLength")) && Helper.IsUnboundedFacetValue(facet)))
                {
                    flag = true;
                }
            }
            int num = 0;
            foreach (PrimitiveType type2 in list)
            {
                if (flag)
                {
                    if (type == null)
                    {
                        type = type2;
                        num = Helper.GetFacet(type2.FacetDescriptions, "MaxLength").MaxValue.Value;
                    }
                    else
                    {
                        int num2 = Helper.GetFacet(type2.FacetDescriptions, "MaxLength").MaxValue.Value;
                        if (num2 > num)
                        {
                            type = type2;
                            num = num2;
                        }
                    }
                }
                else
                {
                    type = type2;
                    break;
                }
            }
            return true;
        }
    }
}

