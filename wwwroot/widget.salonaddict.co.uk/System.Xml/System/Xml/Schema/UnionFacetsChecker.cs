namespace System.Xml.Schema
{
    using System;
    using System.Collections;

    internal class UnionFacetsChecker : FacetsChecker
    {
        internal override Exception CheckValueFacets(object value, XmlSchemaDatatype datatype)
        {
            RestrictionFacets restriction = datatype.Restriction;
            RestrictionFlags flags = (restriction != null) ? restriction.Flags : ((RestrictionFlags) 0);
            if (((flags & RestrictionFlags.Enumeration) != 0) && !this.MatchEnumeration(value, restriction.Enumeration, datatype))
            {
                return new XmlSchemaException("Sch_EnumerationConstraintFailed", string.Empty);
            }
            return null;
        }

        internal override bool MatchEnumeration(object value, ArrayList enumeration, XmlSchemaDatatype datatype)
        {
            foreach (object obj2 in enumeration)
            {
                if (datatype.Compare(value, obj2) == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

