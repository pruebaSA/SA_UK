namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Collections;

    internal class StoreCategoryInstanceEnumeration : IEnumerator
    {
        private System.Deployment.Internal.Isolation.STORE_CATEGORY_INSTANCE _current;
        private System.Deployment.Internal.Isolation.IEnumSTORE_CATEGORY_INSTANCE _enum;
        private bool _fValid;

        public StoreCategoryInstanceEnumeration(System.Deployment.Internal.Isolation.IEnumSTORE_CATEGORY_INSTANCE pI)
        {
            this._enum = pI;
        }

        private System.Deployment.Internal.Isolation.STORE_CATEGORY_INSTANCE GetCurrent()
        {
            if (!this._fValid)
            {
                throw new InvalidOperationException();
            }
            return this._current;
        }

        public IEnumerator GetEnumerator() => 
            this;

        public bool MoveNext()
        {
            System.Deployment.Internal.Isolation.STORE_CATEGORY_INSTANCE[] rgInstances = new System.Deployment.Internal.Isolation.STORE_CATEGORY_INSTANCE[1];
            uint num = this._enum.Next(1, rgInstances);
            if (num == 1)
            {
                this._current = rgInstances[0];
            }
            return (this._fValid = num == 1);
        }

        public void Reset()
        {
            this._fValid = false;
            this._enum.Reset();
        }

        public System.Deployment.Internal.Isolation.STORE_CATEGORY_INSTANCE Current =>
            this.GetCurrent();

        object IEnumerator.Current =>
            this.GetCurrent();
    }
}

