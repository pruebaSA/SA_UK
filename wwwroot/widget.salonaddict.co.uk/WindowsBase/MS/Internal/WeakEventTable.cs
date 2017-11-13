namespace MS.Internal
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    internal class WeakEventTable : DispatcherObject
    {
        private bool _cleanupEnabled = true;
        private int _cleanupRequests;
        [ThreadStatic]
        private static WeakEventTable _currentTable;
        private Hashtable _dataTable = new Hashtable();
        private ReaderWriterLockWrapper _lock = new ReaderWriterLockWrapper();
        private Hashtable _managerTable = new Hashtable();

        [SecurityCritical, SecurityTreatAsSafe]
        private WeakEventTable()
        {
            new WeakEventTableShutDownListener(this);
        }

        internal static bool Cleanup() => 
            CurrentWeakEventTable.Purge(false);

        private object CleanupOperation(object arg)
        {
            Interlocked.Exchange(ref this._cleanupRequests, 0);
            if (this.IsCleanupEnabled)
            {
                this.Purge(false);
            }
            return null;
        }

        private void OnShutDown()
        {
            this.Purge(true);
            _currentTable = null;
        }

        private bool Purge(bool purgeAll)
        {
            bool flag = false;
            using (this.WriteLock)
            {
                ICollection keys = this._dataTable.Keys;
                EventKey[] array = new EventKey[keys.Count];
                keys.CopyTo(array, 0);
                for (int i = array.Length - 1; i >= 0; i--)
                {
                    object source = array[i].Source;
                    flag |= array[i].Manager.PurgeInternal(source, this._dataTable[array[i]], purgeAll);
                    if (!purgeAll && (source == null))
                    {
                        this._dataTable.Remove(array[i]);
                    }
                }
                if (purgeAll)
                {
                    this._managerTable.Clear();
                    this._dataTable.Clear();
                }
            }
            return flag;
        }

        internal void Remove(WeakEventManager manager, object source)
        {
            EventKey key = new EventKey(manager, source);
            this._dataTable.Remove(key);
        }

        internal void ScheduleCleanup()
        {
            if (Interlocked.Increment(ref this._cleanupRequests) == 1)
            {
                base.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new DispatcherOperationCallback(this.CleanupOperation), null);
            }
        }

        internal static WeakEventTable CurrentWeakEventTable
        {
            get
            {
                if (_currentTable == null)
                {
                    _currentTable = new WeakEventTable();
                }
                return _currentTable;
            }
        }

        internal bool IsCleanupEnabled
        {
            get => 
                this._cleanupEnabled;
            set
            {
                this._cleanupEnabled = value;
            }
        }

        internal object this[WeakEventManager manager, object source]
        {
            get
            {
                EventKey key = new EventKey(manager, source);
                return this._dataTable[key];
            }
            set
            {
                EventKey key = new EventKey(manager, source, true);
                this._dataTable[key] = value;
            }
        }

        internal WeakEventManager this[Type managerType]
        {
            get => 
                ((WeakEventManager) this._managerTable[managerType]);
            set
            {
                this._managerTable[managerType] = value;
            }
        }

        internal IDisposable ReadLock =>
            this._lock.ReadLock;

        internal IDisposable WriteLock =>
            this._lock.WriteLock;

        [StructLayout(LayoutKind.Sequential)]
        private struct EventKey
        {
            private WeakEventManager _manager;
            private object _source;
            private int _hashcode;
            internal EventKey(WeakEventManager manager, object source, bool useWeakRef)
            {
                this._manager = manager;
                this._source = new WeakReference(source);
                this._hashcode = manager.GetHashCode() + RuntimeHelpers.GetHashCode(source);
            }

            internal EventKey(WeakEventManager manager, object source)
            {
                this._manager = manager;
                this._source = source;
                this._hashcode = manager.GetHashCode() + RuntimeHelpers.GetHashCode(source);
            }

            internal object Source =>
                ((WeakReference) this._source).Target;
            internal WeakEventManager Manager =>
                this._manager;
            public override int GetHashCode() => 
                this._hashcode;

            public override bool Equals(object o)
            {
                if (!(o is WeakEventTable.EventKey))
                {
                    return false;
                }
                WeakEventTable.EventKey key = (WeakEventTable.EventKey) o;
                if ((this._manager != key._manager) || (this._hashcode != key._hashcode))
                {
                    return false;
                }
                WeakReference reference = this._source as WeakReference;
                object obj2 = (reference != null) ? reference.Target : this._source;
                reference = key._source as WeakReference;
                object obj3 = (reference != null) ? reference.Target : key._source;
                if ((obj2 != null) && (obj3 != null))
                {
                    return (obj2 == obj3);
                }
                return (this._source == key._source);
            }

            public static bool operator ==(WeakEventTable.EventKey key1, WeakEventTable.EventKey key2) => 
                key1.Equals(key2);

            public static bool operator !=(WeakEventTable.EventKey key1, WeakEventTable.EventKey key2) => 
                !key1.Equals(key2);
        }

        private sealed class WeakEventTableShutDownListener : ShutDownListener
        {
            [SecurityTreatAsSafe, SecurityCritical]
            public WeakEventTableShutDownListener(WeakEventTable target) : base(target)
            {
            }

            internal override void OnShutDown(object target)
            {
                ((WeakEventTable) target).OnShutDown();
            }
        }
    }
}

