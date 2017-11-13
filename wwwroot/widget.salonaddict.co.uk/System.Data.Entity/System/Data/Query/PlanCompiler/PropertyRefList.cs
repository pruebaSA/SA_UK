namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;

    internal class PropertyRefList
    {
        internal static PropertyRefList All = new PropertyRefList(true);
        private bool m_allProperties;
        private Dictionary<PropertyRef, PropertyRef> m_propertyReferences;

        internal PropertyRefList() : this(false)
        {
        }

        private PropertyRefList(bool allProps)
        {
            this.m_propertyReferences = new Dictionary<PropertyRef, PropertyRef>();
            if (allProps)
            {
                this.MakeAllProperties();
            }
        }

        internal void Add(PropertyRef property)
        {
            if (!this.m_allProperties)
            {
                if (property is AllPropertyRef)
                {
                    this.MakeAllProperties();
                }
                else
                {
                    this.m_propertyReferences[property] = property;
                }
            }
        }

        internal void Append(PropertyRefList propertyRefs)
        {
            if (!this.m_allProperties)
            {
                foreach (PropertyRef ref2 in propertyRefs.m_propertyReferences.Keys)
                {
                    this.Add(ref2);
                }
            }
        }

        internal PropertyRefList Clone()
        {
            PropertyRefList list = new PropertyRefList(this.m_allProperties);
            foreach (PropertyRef ref2 in this.m_propertyReferences.Keys)
            {
                list.Add(ref2);
            }
            return list;
        }

        internal bool Contains(PropertyRef p)
        {
            if (!this.m_allProperties)
            {
                return this.m_propertyReferences.ContainsKey(p);
            }
            return true;
        }

        private void MakeAllProperties()
        {
            this.m_allProperties = true;
            this.m_propertyReferences.Clear();
            this.m_propertyReferences.Add(AllPropertyRef.Instance, AllPropertyRef.Instance);
        }

        public override string ToString()
        {
            string str = "{";
            foreach (PropertyRef ref2 in this.m_propertyReferences.Keys)
            {
                str = str + ref2.ToString() + ",";
            }
            return (str + "}");
        }

        internal bool AllProperties =>
            this.m_allProperties;

        internal IEnumerable<PropertyRef> Properties =>
            this.m_propertyReferences.Keys;
    }
}

