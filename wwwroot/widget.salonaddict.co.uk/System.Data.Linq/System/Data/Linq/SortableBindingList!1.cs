namespace System.Data.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Xml.Linq;

    internal class SortableBindingList<T> : BindingList<T>
    {
        private bool isSorted;
        private ListSortDirection sortDirection;
        private PropertyDescriptor sortProperty;

        internal SortableBindingList(IList<T> list) : base(list)
        {
        }

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            if (PropertyComparer<T>.IsAllowable(prop.PropertyType))
            {
                ((List<T>) base.Items).Sort(new PropertyComparer<T>(prop, direction));
                this.sortDirection = direction;
                this.sortProperty = prop;
                this.isSorted = true;
                this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        protected override void RemoveSortCore()
        {
            this.isSorted = false;
            this.sortProperty = null;
        }

        protected override bool IsSortedCore =>
            this.isSorted;

        protected override ListSortDirection SortDirectionCore =>
            this.sortDirection;

        protected override PropertyDescriptor SortPropertyCore =>
            this.sortProperty;

        protected override bool SupportsSortingCore =>
            true;

        internal class PropertyComparer : Comparer<T>
        {
            private IComparer comparer;
            private ListSortDirection direction;
            private PropertyDescriptor prop;
            private bool useToString;

            internal PropertyComparer(PropertyDescriptor prop, ListSortDirection direction)
            {
                if (prop.ComponentType != typeof(T))
                {
                    throw new MissingMemberException(typeof(T).Name, prop.Name);
                }
                this.prop = prop;
                this.direction = direction;
                if (SortableBindingList<T>.PropertyComparer.OkWithIComparable(prop.PropertyType))
                {
                    PropertyInfo property = typeof(Comparer<>).MakeGenericType(new Type[] { prop.PropertyType }).GetProperty("Default");
                    this.comparer = (IComparer) property.GetValue(null, null);
                    this.useToString = false;
                }
                else if (SortableBindingList<T>.PropertyComparer.OkWithToString(prop.PropertyType))
                {
                    this.comparer = StringComparer.CurrentCultureIgnoreCase;
                    this.useToString = true;
                }
            }

            public override int Compare(T x, T y)
            {
                object obj2 = this.prop.GetValue(x);
                object obj3 = this.prop.GetValue(y);
                if (this.useToString)
                {
                    obj2 = obj2?.ToString();
                    obj3 = obj3?.ToString();
                }
                if (this.direction == ListSortDirection.Ascending)
                {
                    return this.comparer.Compare(obj2, obj3);
                }
                return this.comparer.Compare(obj3, obj2);
            }

            public static bool IsAllowable(Type t)
            {
                if (!SortableBindingList<T>.PropertyComparer.OkWithToString(t))
                {
                    return SortableBindingList<T>.PropertyComparer.OkWithIComparable(t);
                }
                return true;
            }

            protected static bool OkWithIComparable(Type t) => 
                ((t.GetInterface("IComparable") != null) || (t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(Nullable<>))));

            protected static bool OkWithToString(Type t)
            {
                if (!t.Equals(typeof(XNode)))
                {
                    return t.IsSubclassOf(typeof(XNode));
                }
                return true;
            }
        }
    }
}

