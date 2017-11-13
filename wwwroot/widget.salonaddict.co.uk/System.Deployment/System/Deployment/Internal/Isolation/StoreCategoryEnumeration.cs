﻿namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Collections;

    internal class StoreCategoryEnumeration : IEnumerator
    {
        private System.Deployment.Internal.Isolation.STORE_CATEGORY _current;
        private System.Deployment.Internal.Isolation.IEnumSTORE_CATEGORY _enum;
        private bool _fValid;

        public StoreCategoryEnumeration(System.Deployment.Internal.Isolation.IEnumSTORE_CATEGORY pI)
        {
            this._enum = pI;
        }

        private System.Deployment.Internal.Isolation.STORE_CATEGORY GetCurrent()
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
            System.Deployment.Internal.Isolation.STORE_CATEGORY[] rgElements = new System.Deployment.Internal.Isolation.STORE_CATEGORY[1];
            uint num = this._enum.Next(1, rgElements);
            if (num == 1)
            {
                this._current = rgElements[0];
            }
            return (this._fValid = num == 1);
        }

        public void Reset()
        {
            this._fValid = false;
            this._enum.Reset();
        }

        public System.Deployment.Internal.Isolation.STORE_CATEGORY Current =>
            this.GetCurrent();

        object IEnumerator.Current =>
            this.GetCurrent();
    }
}

