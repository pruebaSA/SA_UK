namespace MS.Internal.ComponentModel
{
    using System;
    using System.Windows;

    internal class PropertyChangeTracker : Expression
    {
        private DependencyObject _object;
        private DependencyProperty _property;
        internal EventHandler Changed;

        internal PropertyChangeTracker(DependencyObject obj, DependencyProperty property) : base(ExpressionMode.SupportsUnboundSources)
        {
            this._object = obj;
            this._property = property;
            base.ChangeSources(this._object, this._property, new DependencySource[] { new DependencySource(obj, property) });
        }

        internal void Close()
        {
            this._object = null;
            this._property = null;
            base.ChangeSources(null, null, null);
        }

        internal override void OnPropertyInvalidation(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            DependencyProperty property = args.Property;
            if (((this._object == d) && (this._property == property)) && (this.Changed != null))
            {
                this.Changed(this._object, EventArgs.Empty);
            }
        }

        internal bool CanClose =>
            (this.Changed == null);
    }
}

