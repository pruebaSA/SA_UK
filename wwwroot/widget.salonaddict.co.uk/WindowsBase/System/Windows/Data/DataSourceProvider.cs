namespace System.Windows.Data
{
    using MS.Internal;
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Threading;

    public abstract class DataSourceProvider : INotifyPropertyChanged, ISupportInitialize
    {
        private object _data;
        private int _deferLevel;
        private System.Windows.Threading.Dispatcher _dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
        private Exception _error;
        private bool _initialLoadCalled;
        private bool _isInitialLoadEnabled = true;
        private static readonly DispatcherOperationCallback UpdateWithNewResultCallback = new DispatcherOperationCallback(DataSourceProvider.UpdateWithNewResult);

        public event EventHandler DataChanged;

        protected event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged;

        protected DataSourceProvider()
        {
        }

        protected virtual void BeginInit()
        {
            this._deferLevel++;
        }

        protected virtual void BeginQuery()
        {
        }

        public virtual IDisposable DeferRefresh()
        {
            this._deferLevel++;
            return new DeferHelper(this);
        }

        private void EndDefer()
        {
            this._deferLevel--;
            if (this._deferLevel == 0)
            {
                this.Refresh();
            }
        }

        protected virtual void EndInit()
        {
            this.EndDefer();
        }

        public void InitialLoad()
        {
            if (this.IsInitialLoadEnabled && !this._initialLoadCalled)
            {
                this._initialLoadCalled = true;
                this.BeginQuery();
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        protected void OnQueryFinished(object newData)
        {
            this.OnQueryFinished(newData, null, null, null);
        }

        protected virtual void OnQueryFinished(object newData, Exception error, DispatcherOperationCallback completionWork, object callbackArguments)
        {
            Invariant.Assert(this.Dispatcher != null);
            if (this.Dispatcher.CheckAccess())
            {
                this.UpdateWithNewResult(error, newData, completionWork, callbackArguments);
            }
            else
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, UpdateWithNewResultCallback, new object[] { this, error, newData, completionWork, callbackArguments });
            }
        }

        public void Refresh()
        {
            this._initialLoadCalled = true;
            this.BeginQuery();
        }

        void ISupportInitialize.BeginInit()
        {
            this.BeginInit();
        }

        void ISupportInitialize.EndInit()
        {
            this.EndInit();
        }

        private static object UpdateWithNewResult(object arg)
        {
            object[] objArray = (object[]) arg;
            Invariant.Assert(objArray.Length == 5);
            DataSourceProvider provider = (DataSourceProvider) objArray[0];
            Exception error = (Exception) objArray[1];
            object newData = objArray[2];
            DispatcherOperationCallback completionWork = (DispatcherOperationCallback) objArray[3];
            object callbackArgs = objArray[4];
            provider.UpdateWithNewResult(error, newData, completionWork, callbackArgs);
            return null;
        }

        private void UpdateWithNewResult(Exception error, object newData, DispatcherOperationCallback completionWork, object callbackArgs)
        {
            bool flag = this._error != error;
            this._error = error;
            if (error != null)
            {
                newData = null;
                this._initialLoadCalled = false;
            }
            this._data = newData;
            if (completionWork != null)
            {
                completionWork(callbackArgs);
            }
            if (this.DataChanged != null)
            {
                this.DataChanged(this, EventArgs.Empty);
            }
            if (flag)
            {
                this.OnPropertyChanged(new PropertyChangedEventArgs("Error"));
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Data =>
            this._data;

        protected System.Windows.Threading.Dispatcher Dispatcher
        {
            get => 
                this._dispatcher;
            set
            {
                if (this._dispatcher != value)
                {
                    this._dispatcher = value;
                }
            }
        }

        public Exception Error =>
            this._error;

        [DefaultValue(true)]
        public bool IsInitialLoadEnabled
        {
            get => 
                this._isInitialLoadEnabled;
            set
            {
                this._isInitialLoadEnabled = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("IsInitialLoadEnabled"));
            }
        }

        protected bool IsRefreshDeferred =>
            ((this._deferLevel > 0) || (!this.IsInitialLoadEnabled && !this._initialLoadCalled));

        private class DeferHelper : IDisposable
        {
            private DataSourceProvider _provider;

            public DeferHelper(DataSourceProvider provider)
            {
                this._provider = provider;
            }

            public void Dispose()
            {
                if (this._provider != null)
                {
                    this._provider.EndDefer();
                    this._provider = null;
                }
            }
        }
    }
}

