namespace System.CodeDom
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [Serializable, ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true)]
    public class CodeDirectiveCollection : CollectionBase
    {
        public CodeDirectiveCollection()
        {
        }

        public CodeDirectiveCollection(CodeDirectiveCollection value)
        {
            this.AddRange(value);
        }

        public CodeDirectiveCollection(CodeDirective[] value)
        {
            this.AddRange(value);
        }

        public int Add(CodeDirective value) => 
            base.List.Add(value);

        public void AddRange(CodeDirective[] value)
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

        public void AddRange(CodeDirectiveCollection value)
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

        public bool Contains(CodeDirective value) => 
            base.List.Contains(value);

        public void CopyTo(CodeDirective[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        public int IndexOf(CodeDirective value) => 
            base.List.IndexOf(value);

        public void Insert(int index, CodeDirective value)
        {
            base.List.Insert(index, value);
        }

        public void Remove(CodeDirective value)
        {
            base.List.Remove(value);
        }

        public CodeDirective this[int index]
        {
            get => 
                ((CodeDirective) base.List[index]);
            set
            {
                base.List[index] = value;
            }
        }
    }
}

