namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public abstract class PropertySelectorBase<TResolutionAttribute> : IPropertySelectorPolicy, IBuilderPolicy where TResolutionAttribute: Attribute
    {
        protected PropertySelectorBase()
        {
        }

        protected abstract IDependencyResolverPolicy CreateResolver(PropertyInfo property);
        private SelectedProperty CreateSelectedProperty(IBuilderContext context, IPolicyList resolverPolicyDestination, PropertyInfo property)
        {
            string key = Guid.NewGuid().ToString();
            SelectedProperty property2 = new SelectedProperty(property, key);
            resolverPolicyDestination.Set<IDependencyResolverPolicy>(this.CreateResolver(property), key);
            DependencyResolverTrackerPolicy.TrackKey(context.PersistentPolicies, context.BuildKey, key);
            return property2;
        }

        public virtual IEnumerable<SelectedProperty> SelectProperties(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            Type type = context.BuildKey.Type;
            foreach (PropertyInfo iteratorVariable1 in type.GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance))
            {
                if (((iteratorVariable1.GetIndexParameters().Length == 0) && iteratorVariable1.CanWrite) && iteratorVariable1.IsDefined(typeof(TResolutionAttribute), false))
                {
                    yield return this.CreateSelectedProperty(context, resolverPolicyDestination, iteratorVariable1);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <SelectProperties>d__0 : IEnumerable<SelectedProperty>, IEnumerable, IEnumerator<SelectedProperty>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private SelectedProperty <>2__current;
            public IBuilderContext <>3__context;
            public IPolicyList <>3__resolverPolicyDestination;
            public PropertySelectorBase<TResolutionAttribute> <>4__this;
            public PropertyInfo[] <>7__wrap4;
            public int <>7__wrap5;
            private int <>l__initialThreadId;
            public PropertyInfo <prop>5__2;
            public Type <t>5__1;
            public IBuilderContext context;
            public IPolicyList resolverPolicyDestination;

            [DebuggerHidden]
            public <SelectProperties>d__0(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void <>m__Finally3()
            {
                this.<>1__state = -1;
            }

            private bool MoveNext()
            {
                try
                {
                    switch (this.<>1__state)
                    {
                        case 0:
                            this.<>1__state = -1;
                            this.<t>5__1 = this.context.BuildKey.Type;
                            this.<>1__state = 1;
                            this.<>7__wrap4 = this.<t>5__1.GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance);
                            this.<>7__wrap5 = 0;
                            while (this.<>7__wrap5 < this.<>7__wrap4.Length)
                            {
                                this.<prop>5__2 = this.<>7__wrap4[this.<>7__wrap5];
                                if (((this.<prop>5__2.GetIndexParameters().Length != 0) || !this.<prop>5__2.CanWrite) || !this.<prop>5__2.IsDefined(typeof(TResolutionAttribute), false))
                                {
                                    goto Label_00F6;
                                }
                                this.<>2__current = this.<>4__this.CreateSelectedProperty(this.context, this.resolverPolicyDestination, this.<prop>5__2);
                                this.<>1__state = 2;
                                return true;
                            Label_00EE:
                                this.<>1__state = 1;
                            Label_00F6:
                                this.<>7__wrap5++;
                            }
                            this.<>m__Finally3();
                            break;

                        case 2:
                            goto Label_00EE;
                    }
                    return false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
            }

            [DebuggerHidden]
            IEnumerator<SelectedProperty> IEnumerable<SelectedProperty>.GetEnumerator()
            {
                PropertySelectorBase<TResolutionAttribute>.<SelectProperties>d__0 d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (PropertySelectorBase<TResolutionAttribute>.<SelectProperties>d__0) this;
                }
                else
                {
                    d__ = new PropertySelectorBase<TResolutionAttribute>.<SelectProperties>d__0(0) {
                        <>4__this = this.<>4__this
                    };
                }
                d__.context = this.<>3__context;
                d__.resolverPolicyDestination = this.<>3__resolverPolicyDestination;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Microsoft.Practices.ObjectBuilder2.SelectedProperty>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
                switch (this.<>1__state)
                {
                    case 1:
                    case 2:
                        this.<>m__Finally3();
                        break;
                }
            }

            SelectedProperty IEnumerator<SelectedProperty>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

