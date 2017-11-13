namespace System.Xml.Serialization
{
    using System;
    using System.Collections;
    using System.Reflection;

    public class XmlArrayItemAttributes : CollectionBase
    {
        public int Add(XmlArrayItemAttribute attribute) => 
            base.List.Add(attribute);

        public bool Contains(XmlArrayItemAttribute attribute) => 
            base.List.Contains(attribute);

        public void CopyTo(XmlArrayItemAttribute[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        public int IndexOf(XmlArrayItemAttribute attribute) => 
            base.List.IndexOf(attribute);

        public void Insert(int index, XmlArrayItemAttribute attribute)
        {
            base.List.Insert(index, attribute);
        }

        public void Remove(XmlArrayItemAttribute attribute)
        {
            base.List.Remove(attribute);
        }

        public XmlArrayItemAttribute this[int index]
        {
            get => 
                ((XmlArrayItemAttribute) base.List[index]);
            set
            {
                base.List[index] = value;
            }
        }
    }
}

