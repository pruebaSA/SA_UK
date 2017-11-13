namespace MS.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    internal static class TraceLevelStore
    {
        private static Dictionary<Key, PresentationTraceLevel> _dictionary = new Dictionary<Key, PresentationTraceLevel>();

        internal static PresentationTraceLevel GetTraceLevel(object element)
        {
            PresentationTraceLevel none;
            if ((element == null) || (_dictionary.Count == 0))
            {
                return PresentationTraceLevel.None;
            }
            lock (_dictionary)
            {
                Key key = new Key(element);
                if (!_dictionary.TryGetValue(key, out none))
                {
                    none = PresentationTraceLevel.None;
                }
            }
            return none;
        }

        internal static void SetTraceLevel(object element, PresentationTraceLevel traceLevel)
        {
            if (element != null)
            {
                lock (_dictionary)
                {
                    Key key = new Key(element, true);
                    if (traceLevel > PresentationTraceLevel.None)
                    {
                        _dictionary[key] = traceLevel;
                    }
                    else
                    {
                        _dictionary.Remove(key);
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Key
        {
            private object _element;
            private int _hashcode;
            internal Key(object element, bool useWeakRef)
            {
                this._element = new WeakReference(element);
                this._hashcode = element.GetHashCode();
            }

            internal Key(object element)
            {
                this._element = element;
                this._hashcode = element.GetHashCode();
            }

            public override int GetHashCode() => 
                this._hashcode;

            public override bool Equals(object o)
            {
                if (!(o is TraceLevelStore.Key))
                {
                    return false;
                }
                TraceLevelStore.Key key = (TraceLevelStore.Key) o;
                if (this._hashcode != key._hashcode)
                {
                    return false;
                }
                WeakReference reference = this._element as WeakReference;
                object obj2 = (reference != null) ? reference.Target : this._element;
                reference = key._element as WeakReference;
                object obj3 = (reference != null) ? reference.Target : key._element;
                if ((obj2 != null) && (obj3 != null))
                {
                    return (obj2 == obj3);
                }
                return (this._element == key._element);
            }

            public static bool operator ==(TraceLevelStore.Key key1, TraceLevelStore.Key key2) => 
                key1.Equals(key2);

            public static bool operator !=(TraceLevelStore.Key key1, TraceLevelStore.Key key2) => 
                !key1.Equals(key2);
        }
    }
}

