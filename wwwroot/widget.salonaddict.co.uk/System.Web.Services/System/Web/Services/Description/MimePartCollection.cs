namespace System.Web.Services.Description
{
    using System;
    using System.Collections;
    using System.Reflection;

    public sealed class MimePartCollection : CollectionBase
    {
        public int Add(MimePart mimePart) => 
            base.List.Add(mimePart);

        public bool Contains(MimePart mimePart) => 
            base.List.Contains(mimePart);

        public void CopyTo(MimePart[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        public int IndexOf(MimePart mimePart) => 
            base.List.IndexOf(mimePart);

        public void Insert(int index, MimePart mimePart)
        {
            base.List.Insert(index, mimePart);
        }

        public void Remove(MimePart mimePart)
        {
            base.List.Remove(mimePart);
        }

        public MimePart this[int index]
        {
            get => 
                ((MimePart) base.List[index]);
            set
            {
                base.List[index] = value;
            }
        }
    }
}

