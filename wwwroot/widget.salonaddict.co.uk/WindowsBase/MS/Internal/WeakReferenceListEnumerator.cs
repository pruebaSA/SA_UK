namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Windows;

    [StructLayout(LayoutKind.Sequential), FriendAccessAllowed]
    internal struct WeakReferenceListEnumerator : IEnumerator
    {
        private int _i;
        private ArrayList _List;
        private object _StrongReference;
        public WeakReferenceListEnumerator(ArrayList List)
        {
            this._i = 0;
            this._List = List;
            this._StrongReference = null;
        }

        object IEnumerator.Current =>
            this.Current;
        public object Current
        {
            get
            {
                if (this._StrongReference == null)
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("Enumerator_VerifyContext"));
                }
                return this._StrongReference;
            }
        }
        public bool MoveNext()
        {
            object target = null;
            while (this._i < this._List.Count)
            {
                WeakReference reference = (WeakReference) this._List[this._i++];
                target = reference.Target;
                if (target != null)
                {
                    break;
                }
            }
            this._StrongReference = target;
            return (null != target);
        }

        public void Reset()
        {
            this._i = 0;
            this._StrongReference = null;
        }
    }
}

