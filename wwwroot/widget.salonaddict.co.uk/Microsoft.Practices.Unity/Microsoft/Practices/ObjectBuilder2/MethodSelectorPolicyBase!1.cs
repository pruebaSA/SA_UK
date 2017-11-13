namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public abstract class MethodSelectorPolicyBase<TMarkerAttribute> : IMethodSelectorPolicy, IBuilderPolicy where TMarkerAttribute: Attribute
    {
        protected MethodSelectorPolicyBase()
        {
        }

        protected abstract IDependencyResolverPolicy CreateResolver(ParameterInfo parameter);
        private SelectedMethod CreateSelectedMethod(IBuilderContext context, IPolicyList resolverPolicyDestination, MethodInfo method)
        {
            SelectedMethod method2 = new SelectedMethod(method);
            foreach (ParameterInfo info in method.GetParameters())
            {
                string buildKey = Guid.NewGuid().ToString();
                IDependencyResolverPolicy policy = this.CreateResolver(info);
                resolverPolicyDestination.Set<IDependencyResolverPolicy>(policy, buildKey);
                DependencyResolverTrackerPolicy.TrackKey(context.PersistentPolicies, context.BuildKey, buildKey);
                method2.AddParameterKey(buildKey);
            }
            return method2;
        }

        public virtual IEnumerable<SelectedMethod> SelectMethods(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            Type type = context.BuildKey.Type;
            foreach (MethodInfo iteratorVariable1 in type.GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance))
            {
                if (iteratorVariable1.IsDefined(typeof(TMarkerAttribute), false))
                {
                    yield return this.CreateSelectedMethod(context, resolverPolicyDestination, iteratorVariable1);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <SelectMethods>d__0 : IEnumerable<SelectedMethod>, IEnumerable, IEnumerator<SelectedMethod>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private SelectedMethod <>2__current;
            public IBuilderContext <>3__context;
            public IPolicyList <>3__resolverPolicyDestination;
            public MethodSelectorPolicyBase<TMarkerAttribute> <>4__this;
            public MethodInfo[] <>7__wrap4;
            public int <>7__wrap5;
            private int <>l__initialThreadId;
            public MethodInfo <method>5__2;
            public Type <t>5__1;
            public IBuilderContext context;
            public IPolicyList resolverPolicyDestination;

            [DebuggerHidden]
            public <SelectMethods>d__0(int <>1__state)
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
                            this.<>7__wrap4 = this.<t>5__1.GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
                            this.<>7__wrap5 = 0;
                            while (this.<>7__wrap5 < this.<>7__wrap4.Length)
                            {
                                this.<method>5__2 = this.<>7__wrap4[this.<>7__wrap5];
                                if (!this.<method>5__2.IsDefined(typeof(TMarkerAttribute), false))
                                {
                                    goto Label_00D1;
                                }
                                this.<>2__current = this.<>4__this.CreateSelectedMethod(this.context, this.resolverPolicyDestination, this.<method>5__2);
                                this.<>1__state = 2;
                                return true;
                            Label_00C9:
                                this.<>1__state = 1;
                            Label_00D1:
                                this.<>7__wrap5++;
                            }
                            this.<>m__Finally3();
                            break;

                        case 2:
                            goto Label_00C9;
                    }
                    return false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
            }

            [DebuggerHidden]
            IEnumerator<SelectedMethod> IEnumerable<SelectedMethod>.GetEnumerator()
            {
                MethodSelectorPolicyBase<TMarkerAttribute>.<SelectMethods>d__0 d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (MethodSelectorPolicyBase<TMarkerAttribute>.<SelectMethods>d__0) this;
                }
                else
                {
                    d__ = new MethodSelectorPolicyBase<TMarkerAttribute>.<SelectMethods>d__0(0) {
                        <>4__this = this.<>4__this
                    };
                }
                d__.context = this.<>3__context;
                d__.resolverPolicyDestination = this.<>3__resolverPolicyDestination;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Microsoft.Practices.ObjectBuilder2.SelectedMethod>.GetEnumerator();

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

            SelectedMethod IEnumerator<SelectedMethod>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

