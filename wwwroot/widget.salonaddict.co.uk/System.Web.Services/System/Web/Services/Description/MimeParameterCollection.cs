namespace System.Web.Services.Description
{
    using System;
    using System.Collections;
    using System.Reflection;

    internal class MimeParameterCollection : CollectionBase
    {
        private Type writerType;

        internal int Add(MimeParameter parameter) => 
            base.List.Add(parameter);

        internal bool Contains(MimeParameter parameter) => 
            base.List.Contains(parameter);

        internal void CopyTo(MimeParameter[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        internal int IndexOf(MimeParameter parameter) => 
            base.List.IndexOf(parameter);

        internal void Insert(int index, MimeParameter parameter)
        {
            base.List.Insert(index, parameter);
        }

        internal void Remove(MimeParameter parameter)
        {
            base.List.Remove(parameter);
        }

        internal MimeParameter this[int index]
        {
            get => 
                ((MimeParameter) base.List[index]);
            set
            {
                base.List[index] = value;
            }
        }

        internal Type WriterType
        {
            get => 
                this.writerType;
            set
            {
                this.writerType = value;
            }
        }
    }
}

