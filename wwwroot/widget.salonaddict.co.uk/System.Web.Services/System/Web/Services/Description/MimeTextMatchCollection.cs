namespace System.Web.Services.Description
{
    using System;
    using System.Collections;
    using System.Reflection;

    public sealed class MimeTextMatchCollection : CollectionBase
    {
        public int Add(MimeTextMatch match) => 
            base.List.Add(match);

        public bool Contains(MimeTextMatch match) => 
            base.List.Contains(match);

        public void CopyTo(MimeTextMatch[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        public int IndexOf(MimeTextMatch match) => 
            base.List.IndexOf(match);

        public void Insert(int index, MimeTextMatch match)
        {
            base.List.Insert(index, match);
        }

        public void Remove(MimeTextMatch match)
        {
            base.List.Remove(match);
        }

        public MimeTextMatch this[int index]
        {
            get => 
                ((MimeTextMatch) base.List[index]);
            set
            {
                base.List[index] = value;
            }
        }
    }
}

