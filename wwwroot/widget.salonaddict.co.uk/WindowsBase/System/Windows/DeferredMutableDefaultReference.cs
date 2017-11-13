namespace System.Windows
{
    using System;

    internal class DeferredMutableDefaultReference : DeferredReference
    {
        private readonly PropertyMetadata _sourceMetadata;
        private readonly DependencyObject _sourceObject;
        private readonly DependencyProperty _sourceProperty;

        internal DeferredMutableDefaultReference(PropertyMetadata metadata, DependencyObject d, DependencyProperty dp)
        {
            this._sourceObject = d;
            this._sourceProperty = dp;
            this._sourceMetadata = metadata;
        }

        internal override object GetValue(BaseValueSourceInternal valueSource) => 
            this._sourceMetadata.GetDefaultValue(this._sourceObject, this._sourceProperty);

        internal override Type GetValueType() => 
            this._sourceProperty.PropertyType;

        internal PropertyMetadata SourceMetadata =>
            this._sourceMetadata;

        protected DependencyObject SourceObject =>
            this._sourceObject;

        protected DependencyProperty SourceProperty =>
            this._sourceProperty;
    }
}

