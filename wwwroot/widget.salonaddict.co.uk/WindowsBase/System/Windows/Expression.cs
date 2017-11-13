namespace System.Windows
{
    using MS.Internal.WindowsBase;
    using System;
    using System.ComponentModel;

    [TypeConverter(typeof(ExpressionConverter))]
    public class Expression
    {
        private InternalFlags _flags;
        [FriendAccessAllowed]
        internal static readonly object NoValue = new object();

        internal Expression() : this(ExpressionMode.None)
        {
        }

        internal Expression(ExpressionMode mode)
        {
            this._flags = InternalFlags.None;
            switch (mode)
            {
                case ExpressionMode.None:
                    return;

                case ExpressionMode.NonSharable:
                    this._flags |= InternalFlags.NonShareable;
                    return;

                case ExpressionMode.ForwardsInvalidations:
                    this._flags |= InternalFlags.ForwardsInvalidations;
                    this._flags |= InternalFlags.NonShareable;
                    return;

                case ExpressionMode.SupportsUnboundSources:
                    this._flags |= InternalFlags.ForwardsInvalidations;
                    this._flags |= InternalFlags.NonShareable;
                    this._flags |= InternalFlags.SupportsUnboundSources;
                    return;
            }
            throw new ArgumentException(System.Windows.SR.Get("UnknownExpressionMode"));
        }

        internal void ChangeSources(DependencyObject d, DependencyProperty dp, DependencySource[] newSources)
        {
            if ((d == null) && !this.ForwardsInvalidations)
            {
                throw new ArgumentNullException("d");
            }
            if ((dp == null) && !this.ForwardsInvalidations)
            {
                throw new ArgumentNullException("dp");
            }
            if (this.Shareable)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("ShareableExpressionsCannotChangeSources"));
            }
            DependencyObject.ValidateSources(d, newSources, this);
            if (this.ForwardsInvalidations)
            {
                DependencyObject.ChangeExpressionSources(this, null, null, newSources);
            }
            else
            {
                DependencyObject.ChangeExpressionSources(this, d, dp, newSources);
            }
        }

        [FriendAccessAllowed]
        internal virtual Expression Copy(DependencyObject targetObject, DependencyProperty targetDP) => 
            this;

        internal virtual DependencySource[] GetSources() => 
            null;

        internal virtual object GetValue(DependencyObject d, DependencyProperty dp) => 
            DependencyProperty.UnsetValue;

        internal void MarkAttached()
        {
            this._flags |= InternalFlags.Attached;
        }

        internal virtual void OnAttach(DependencyObject d, DependencyProperty dp)
        {
        }

        internal virtual void OnDetach(DependencyObject d, DependencyProperty dp)
        {
        }

        internal virtual void OnPropertyInvalidation(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
        }

        internal virtual bool SetValue(DependencyObject d, DependencyProperty dp, object value) => 
            false;

        internal bool Attachable
        {
            get
            {
                if (!this.Shareable)
                {
                    return !this.HasBeenAttached;
                }
                return true;
            }
        }

        internal bool ForwardsInvalidations =>
            ((this._flags & InternalFlags.ForwardsInvalidations) != InternalFlags.None);

        internal bool HasBeenAttached =>
            ((this._flags & InternalFlags.Attached) != InternalFlags.None);

        internal bool Shareable =>
            ((this._flags & InternalFlags.NonShareable) == InternalFlags.None);

        internal bool SupportsUnboundSources =>
            ((this._flags & InternalFlags.SupportsUnboundSources) != InternalFlags.None);

        [Flags]
        private enum InternalFlags
        {
            Attached = 8,
            ForwardsInvalidations = 2,
            None = 0,
            NonShareable = 1,
            SupportsUnboundSources = 4
        }
    }
}

