namespace System.Data.Common.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    internal sealed class Memoizer<TArg, TResult>
    {
        private readonly Func<TArg, TResult> _function;
        private readonly ReaderWriterLockSlim _lock;
        private readonly Dictionary<TArg, Result<TArg, TResult>> _resultCache;

        internal Memoizer(Func<TArg, TResult> function, IEqualityComparer<TArg> argComparer)
        {
            EntityUtil.CheckArgumentNull<Func<TArg, TResult>>(function, "function");
            this._function = function;
            this._resultCache = new Dictionary<TArg, Result<TArg, TResult>>(argComparer);
            this._lock = new ReaderWriterLockSlim();
        }

        internal TResult Evaluate(TArg arg)
        {
            Result<TArg, TResult> result;
            bool flag;
            Func<TResult> createValueDelegate = null;
            this._lock.EnterReadLock();
            try
            {
                flag = this._resultCache.TryGetValue(arg, out result);
            }
            finally
            {
                this._lock.ExitReadLock();
            }
            if (!flag)
            {
                this._lock.EnterWriteLock();
                try
                {
                    if (!this._resultCache.TryGetValue(arg, out result))
                    {
                        if (createValueDelegate == null)
                        {
                            createValueDelegate = () => ((System.Data.Common.Utils.Memoizer<TArg, TResult>) this)._function(arg);
                        }
                        result = new Result<TArg, TResult>(createValueDelegate);
                        this._resultCache.Add(arg, result);
                    }
                }
                finally
                {
                    this._lock.ExitWriteLock();
                }
            }
            return result.GetValue();
        }

        private class Result
        {
            private Func<TResult> _delegate;
            private TResult _value;

            internal Result(Func<TResult> createValueDelegate)
            {
                this._delegate = createValueDelegate;
            }

            internal TResult GetValue()
            {
                if (this._delegate == null)
                {
                    return this._value;
                }
                lock (((System.Data.Common.Utils.Memoizer<TArg, TResult>.Result) this))
                {
                    if (this._delegate != null)
                    {
                        this._value = this._delegate();
                        this._delegate = null;
                    }
                    return this._value;
                }
            }
        }
    }
}

