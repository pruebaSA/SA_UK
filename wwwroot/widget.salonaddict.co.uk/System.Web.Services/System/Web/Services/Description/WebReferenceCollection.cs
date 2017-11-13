namespace System.Web.Services.Description
{
    using System;
    using System.Collections;
    using System.Reflection;

    public sealed class WebReferenceCollection : CollectionBase
    {
        public int Add(WebReference webReference) => 
            base.List.Add(webReference);

        public bool Contains(WebReference webReference) => 
            base.List.Contains(webReference);

        public void CopyTo(WebReference[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        public int IndexOf(WebReference webReference) => 
            base.List.IndexOf(webReference);

        public void Insert(int index, WebReference webReference)
        {
            base.List.Insert(index, webReference);
        }

        public void Remove(WebReference webReference)
        {
            base.List.Remove(webReference);
        }

        public WebReference this[int index]
        {
            get => 
                ((WebReference) base.List[index]);
            set
            {
                base.List[index] = value;
            }
        }
    }
}

