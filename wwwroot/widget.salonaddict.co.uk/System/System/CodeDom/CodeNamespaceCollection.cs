namespace System.CodeDom
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [Serializable, ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true)]
    public class CodeNamespaceCollection : CollectionBase
    {
        public CodeNamespaceCollection()
        {
        }

        public CodeNamespaceCollection(CodeNamespaceCollection value)
        {
            this.AddRange(value);
        }

        public CodeNamespaceCollection(CodeNamespace[] value)
        {
            this.AddRange(value);
        }

        public int Add(CodeNamespace value) => 
            base.List.Add(value);

        public void AddRange(CodeNamespace[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            for (int i = 0; i < value.Length; i++)
            {
                this.Add(value[i]);
            }
        }

        public void AddRange(CodeNamespaceCollection value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            int count = value.Count;
            for (int i = 0; i < count; i++)
            {
                this.Add(value[i]);
            }
        }

        public bool Contains(CodeNamespace value) => 
            base.List.Contains(value);

        public void CopyTo(CodeNamespace[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        public int IndexOf(CodeNamespace value) => 
            base.List.IndexOf(value);

        public void Insert(int index, CodeNamespace value)
        {
            base.List.Insert(index, value);
        }

        public void Remove(CodeNamespace value)
        {
            base.List.Remove(value);
        }

        public CodeNamespace this[int index]
        {
            get => 
                ((CodeNamespace) base.List[index]);
            set
            {
                base.List[index] = value;
            }
        }
    }
}

