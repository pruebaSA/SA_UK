namespace System.Data.Objects.ELinq
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.CommandTrees;
    using System.Data.Objects;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;

    internal sealed class BindingContext
    {
        private readonly Binding _rootContextBinding;
        private readonly List<Binding[]> _scopes;
        internal readonly System.Data.Objects.ObjectContext ObjectContext;

        internal BindingContext()
        {
            this._scopes = new List<Binding[]>();
        }

        internal BindingContext(ParameterExpression rootContextParameter, System.Data.Objects.ObjectContext objectContext, CompiledQueryParameter[] compiledQueryParameters) : this()
        {
            this._rootContextBinding = new Binding(rootContextParameter, null);
            this.ObjectContext = objectContext;
            this.PushBindingScope(new Binding[] { this._rootContextBinding });
            foreach (CompiledQueryParameter parameter in compiledQueryParameters)
            {
                Binding binding = new Binding(parameter.Expression, parameter.ParameterReference);
                this.PushBindingScope(new Binding[] { binding });
            }
        }

        internal bool IsRootContextParameter(ParameterExpression parameter)
        {
            Binding binding;
            return (this.TryGetBinding(parameter, out binding) && (this._rootContextBinding == binding));
        }

        internal void PopBindingScope()
        {
            this._scopes.RemoveAt(this._scopes.Count - 1);
        }

        internal void PushBindingScope(params Binding[] bindings)
        {
            this._scopes.Add(bindings);
        }

        private bool TryGetBinding(Expression parameter, out Binding binding)
        {
            binding = null;
            if (this._scopes.Count != 0)
            {
                for (int i = this._scopes.Count - 1; i >= 0; i--)
                {
                    Binding[] bindingArray = this._scopes[i];
                    foreach (Binding binding2 in bindingArray)
                    {
                        if (parameter == binding2.LinqExpression)
                        {
                            binding = binding2;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal bool TryGetBoundExpression(Expression linqExpression, out DbExpression cqtExpression)
        {
            Binding binding;
            if (this.TryGetBinding(linqExpression, out binding) && (binding != this._rootContextBinding))
            {
                cqtExpression = binding.CqtExpression;
                return true;
            }
            cqtExpression = null;
            return false;
        }
    }
}

