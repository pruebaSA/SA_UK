namespace System.Data.Linq.Provider
{
    using System;

    internal static class DataManipulation
    {
        public static int Delete<TEntity>(TEntity item)
        {
            throw new NotImplementedException();
        }

        public static int Delete<TEntity>(TEntity item, Func<TEntity, bool> check)
        {
            throw new NotImplementedException();
        }

        public static int Insert<TEntity>(TEntity item)
        {
            throw new NotImplementedException();
        }

        public static TResult Insert<TEntity, TResult>(TEntity item, Func<TEntity, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }

        public static int Update<TEntity>(TEntity item)
        {
            throw new NotImplementedException();
        }

        public static int Update<TEntity>(TEntity item, Func<TEntity, bool> check)
        {
            throw new NotImplementedException();
        }

        public static TResult Update<TEntity, TResult>(TEntity item, Func<TEntity, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }

        public static TResult Update<TEntity, TResult>(TEntity item, Func<TEntity, bool> check, Func<TEntity, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }
    }
}

