namespace System.Xml.Serialization
{
    using System;
    using System.Collections;
    using System.Reflection;

    public class XmlAnyElementAttributes : CollectionBase
    {
        public int Add(XmlAnyElementAttribute attribute) => 
            base.List.Add(attribute);

        public bool Contains(XmlAnyElementAttribute attribute) => 
            base.List.Contains(attribute);

        public void CopyTo(XmlAnyElementAttribute[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        public int IndexOf(XmlAnyElementAttribute attribute) => 
            base.List.IndexOf(attribute);

        public void Insert(int index, XmlAnyElementAttribute attribute)
        {
            base.List.Insert(index, attribute);
        }

        public void Remove(XmlAnyElementAttribute attribute)
        {
            base.List.Remove(attribute);
        }

        public XmlAnyElementAttribute this[int index]
        {
            get => 
                ((XmlAnyElementAttribute) base.List[index]);
            set
            {
                base.List[index] = value;
            }
        }
    }
}

