namespace System.Windows.Threading
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal class ExceptionWrapper
    {
        public event CatchHandler Catch;

        public event FilterHandler Filter;

        private bool CatchException(object source, Exception e, Delegate catchHandler)
        {
            if (catchHandler != null)
            {
                if (catchHandler is DispatcherOperationCallback)
                {
                    ((DispatcherOperationCallback) catchHandler)(null);
                }
                else
                {
                    catchHandler.DynamicInvoke(null);
                }
            }
            return ((this.Catch != null) && this.Catch(source, e));
        }

        private bool FilterException(object source, Exception e)
        {
            bool flag = null != this.Catch;
            if (this.Filter != null)
            {
                flag = this.Filter(source, e);
            }
            return flag;
        }

        private object InternalRealCall(Delegate callback, object args, bool isSingleParameter)
        {
            bool flag = true;
            object obj2 = null;
            if (!isSingleParameter && (args == null))
            {
                if (callback is Dispatcher.ShutdownCallback)
                {
                    flag = false;
                    ((Dispatcher.ShutdownCallback) callback)();
                }
            }
            else
            {
                bool flag2 = isSingleParameter;
                object arg = args;
                if (!isSingleParameter && (args != null))
                {
                    object[] objArray = (object[]) args;
                    if (objArray.Length == 1)
                    {
                        flag2 = true;
                        arg = objArray[0];
                    }
                }
                if (flag2)
                {
                    if (callback is DispatcherOperationCallback)
                    {
                        flag = false;
                        obj2 = ((DispatcherOperationCallback) callback)(arg);
                    }
                    else if (callback is SendOrPostCallback)
                    {
                        flag = false;
                        ((SendOrPostCallback) callback)(arg);
                    }
                }
            }
            if (!flag)
            {
                return obj2;
            }
            if (isSingleParameter)
            {
                args = new object[] { args };
            }
            return callback.DynamicInvoke((object[]) args);
        }

        public object TryCatchWhen(object source, Delegate callback, object args, bool isSingleParameter, Delegate catchHandler)
        {
            object objectValue = null;
            Exception exception;
            try
            {
                objectValue = RuntimeHelpers.GetObjectValue(this.InternalRealCall(callback, RuntimeHelpers.GetObjectValue(args), isSingleParameter));
            }
            catch (Exception exception1) when (this.FilterException(RuntimeHelpers.GetObjectValue(source), exception))
            {
                Exception exception2 = exception = exception1;
                if (!this.CatchException(RuntimeHelpers.GetObjectValue(source), exception, catchHandler))
                {
                    throw;
                }
            }
            return objectValue;
        }

        public delegate bool CatchHandler(object source, Exception e);

        public delegate bool FilterHandler(object source, Exception e);
    }
}

