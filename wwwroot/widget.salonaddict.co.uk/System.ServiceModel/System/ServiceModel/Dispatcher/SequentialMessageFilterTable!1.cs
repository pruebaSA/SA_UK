namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;

    internal class SequentialMessageFilterTable<FilterData> : IMessageFilterTable<FilterData>, IDictionary<MessageFilter, FilterData>, ICollection<KeyValuePair<MessageFilter, FilterData>>, IEnumerable<KeyValuePair<MessageFilter, FilterData>>, IEnumerable
    {
        private Dictionary<MessageFilter, FilterData> filters;

        public SequentialMessageFilterTable()
        {
            this.filters = new Dictionary<MessageFilter, FilterData>();
        }

        public void Add(MessageFilter key, FilterData value)
        {
            this.filters.Add(key, value);
        }

        public void Clear()
        {
            this.filters.Clear();
        }

        public bool ContainsKey(MessageFilter key) => 
            this.filters.ContainsKey(key);

        public bool GetMatchingFilter(Message message, out MessageFilter filter)
        {
            filter = null;
            foreach (KeyValuePair<MessageFilter, FilterData> pair in this.filters)
            {
                if (pair.Key.Match(message))
                {
                    if (filter != null)
                    {
                        Collection<MessageFilter> filters = new Collection<MessageFilter> {
                            filter,
                            pair.Key
                        };
                        throw TraceUtility.ThrowHelperError(new MultipleFilterMatchesException(System.ServiceModel.SR.GetString("FilterMultipleMatches"), null, filters), message);
                    }
                    filter = pair.Key;
                }
            }
            return (filter != null);
        }

        public bool GetMatchingFilter(MessageBuffer buffer, out MessageFilter filter)
        {
            filter = null;
            foreach (KeyValuePair<MessageFilter, FilterData> pair in this.filters)
            {
                if (pair.Key.Match(buffer))
                {
                    if (filter != null)
                    {
                        Collection<MessageFilter> filters = new Collection<MessageFilter> {
                            filter,
                            pair.Key
                        };
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MultipleFilterMatchesException(System.ServiceModel.SR.GetString("FilterMultipleMatches"), null, filters));
                    }
                    filter = pair.Key;
                }
            }
            return (filter != null);
        }

        public bool GetMatchingFilters(Message message, ICollection<MessageFilter> results)
        {
            int count = results.Count;
            foreach (KeyValuePair<MessageFilter, FilterData> pair in this.filters)
            {
                if (pair.Key.Match(message))
                {
                    results.Add(pair.Key);
                }
            }
            return (count != results.Count);
        }

        public bool GetMatchingFilters(MessageBuffer buffer, ICollection<MessageFilter> results)
        {
            int count = results.Count;
            foreach (KeyValuePair<MessageFilter, FilterData> pair in this.filters)
            {
                if (pair.Key.Match(buffer))
                {
                    results.Add(pair.Key);
                }
            }
            return (count != results.Count);
        }

        public bool GetMatchingValue(Message message, out FilterData data)
        {
            bool flag = false;
            MessageFilter key = null;
            data = default(FilterData);
            foreach (KeyValuePair<MessageFilter, FilterData> pair in this.filters)
            {
                if (pair.Key.Match(message))
                {
                    if (flag)
                    {
                        Collection<MessageFilter> filters = new Collection<MessageFilter> {
                            key,
                            pair.Key
                        };
                        throw TraceUtility.ThrowHelperError(new MultipleFilterMatchesException(System.ServiceModel.SR.GetString("FilterMultipleMatches"), null, filters), message);
                    }
                    key = pair.Key;
                    data = pair.Value;
                    flag = true;
                }
            }
            return flag;
        }

        public bool GetMatchingValue(MessageBuffer buffer, out FilterData data)
        {
            bool flag = false;
            MessageFilter key = null;
            data = default(FilterData);
            foreach (KeyValuePair<MessageFilter, FilterData> pair in this.filters)
            {
                if (pair.Key.Match(buffer))
                {
                    if (flag)
                    {
                        Collection<MessageFilter> filters = new Collection<MessageFilter> {
                            key,
                            pair.Key
                        };
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MultipleFilterMatchesException(System.ServiceModel.SR.GetString("FilterMultipleMatches"), null, filters));
                    }
                    key = pair.Key;
                    data = pair.Value;
                    flag = true;
                }
            }
            return flag;
        }

        public bool GetMatchingValues(Message message, ICollection<FilterData> results)
        {
            int count = results.Count;
            foreach (KeyValuePair<MessageFilter, FilterData> pair in this.filters)
            {
                if (pair.Key.Match(message))
                {
                    results.Add(pair.Value);
                }
            }
            return (count != results.Count);
        }

        public bool GetMatchingValues(MessageBuffer buffer, ICollection<FilterData> results)
        {
            int count = results.Count;
            foreach (KeyValuePair<MessageFilter, FilterData> pair in this.filters)
            {
                if (pair.Key.Match(buffer))
                {
                    results.Add(pair.Value);
                }
            }
            return (count != results.Count);
        }

        public bool Remove(MessageFilter key) => 
            this.filters.Remove(key);

        void ICollection<KeyValuePair<MessageFilter, FilterData>>.Add(KeyValuePair<MessageFilter, FilterData> item)
        {
            this.filters.Add(item);
        }

        bool ICollection<KeyValuePair<MessageFilter, FilterData>>.Contains(KeyValuePair<MessageFilter, FilterData> item) => 
            this.filters.Contains(item);

        void ICollection<KeyValuePair<MessageFilter, FilterData>>.CopyTo(KeyValuePair<MessageFilter, FilterData>[] array, int arrayIndex)
        {
            this.filters.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<MessageFilter, FilterData>>.Remove(KeyValuePair<MessageFilter, FilterData> item) => 
            this.filters.Remove(item);

        IEnumerator<KeyValuePair<MessageFilter, FilterData>> IEnumerable<KeyValuePair<MessageFilter, FilterData>>.GetEnumerator() => 
            this.filters.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            ((IEnumerable<KeyValuePair<MessageFilter, FilterData>>) this).GetEnumerator();

        public bool TryGetValue(MessageFilter filter, out FilterData data) => 
            this.filters.TryGetValue(filter, out data);

        public int Count =>
            this.filters.Count;

        public FilterData this[MessageFilter key]
        {
            get => 
                this.filters[key];
            set
            {
                this.filters[key] = value;
            }
        }

        public ICollection<MessageFilter> Keys =>
            this.filters.Keys;

        bool ICollection<KeyValuePair<MessageFilter, FilterData>>.IsReadOnly =>
            false;

        public ICollection<FilterData> Values =>
            ((ICollection<FilterData>) this.filters.Values);
    }
}

