namespace System.Data.Common.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class DisposableCollectionWrapper<T> : IDisposable, IEnumerable<T>, IEnumerable where T: IDisposable
    {
        private IEnumerable<T> _enumerable;

        internal DisposableCollectionWrapper(IEnumerable<T> enumerable)
        {
            this._enumerable = enumerable;
        }

        public void Dispose()
        {
            if (this._enumerable != null)
            {
                foreach (T local in this._enumerable)
                {
                    if (local != null)
                    {
                        local.Dispose();
                    }
                }
            }
        }

        public IEnumerator<T> GetEnumerator() => 
            this._enumerable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this._enumerable.GetEnumerator();
    }
}

