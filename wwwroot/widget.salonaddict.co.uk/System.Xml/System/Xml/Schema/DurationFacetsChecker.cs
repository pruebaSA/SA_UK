namespace System.Xml.Schema
{
    using System;
    using System.Collections;

    internal class DurationFacetsChecker : FacetsChecker
    {
        internal override Exception CheckValueFacets(object value, XmlSchemaDatatype datatype)
        {
            TimeSpan span = (TimeSpan) datatype.ValueConverter.ChangeType(value, typeof(TimeSpan));
            return this.CheckValueFacets(span, datatype);
        }

        internal override Exception CheckValueFacets(TimeSpan value, XmlSchemaDatatype datatype)
        {
            RestrictionFacets restriction = datatype.Restriction;
            RestrictionFlags flags = (restriction != null) ? restriction.Flags : ((RestrictionFlags) 0);
            if (((flags & RestrictionFlags.MaxInclusive) != 0) && (TimeSpan.Compare(value, (TimeSpan) restriction.MaxInclusive) > 0))
            {
                return new XmlSchemaException("Sch_MaxInclusiveConstraintFailed", string.Empty);
            }
            if (((flags & RestrictionFlags.MaxExclusive) != 0) && (TimeSpan.Compare(value, (TimeSpan) restriction.MaxExclusive) >= 0))
            {
                return new XmlSchemaException("Sch_MaxExclusiveConstraintFailed", string.Empty);
            }
            if (((flags & RestrictionFlags.MinInclusive) != 0) && (TimeSpan.Compare(value, (TimeSpan) restriction.MinInclusive) < 0))
            {
                return new XmlSchemaException("Sch_MinInclusiveConstraintFailed", string.Empty);
            }
            if (((flags & RestrictionFlags.MinExclusive) != 0) && (TimeSpan.Compare(value, (TimeSpan) restriction.MinExclusive) <= 0))
            {
                return new XmlSchemaException("Sch_MinExclusiveConstraintFailed", string.Empty);
            }
            if (((flags & RestrictionFlags.Enumeration) != 0) && !this.MatchEnumeration(value, restriction.Enumeration))
            {
                return new XmlSchemaException("Sch_EnumerationConstraintFailed", string.Empty);
            }
            return null;
        }

        private bool MatchEnumeration(TimeSpan value, ArrayList enumeration)
        {
            foreach (TimeSpan span in enumeration)
            {
                if (TimeSpan.Compare(value, span) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        internal override bool MatchEnumeration(object value, ArrayList enumeration, XmlSchemaDatatype datatype) => 
            this.MatchEnumeration((TimeSpan) value, enumeration);
    }
}

