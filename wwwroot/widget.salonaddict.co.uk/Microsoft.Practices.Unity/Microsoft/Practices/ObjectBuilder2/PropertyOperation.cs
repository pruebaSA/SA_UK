namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Globalization;

    public abstract class PropertyOperation : BuildOperation
    {
        private readonly string propertyName;

        protected PropertyOperation(Type typeBeingConstructed, string propertyName) : base(typeBeingConstructed)
        {
            this.propertyName = propertyName;
        }

        protected abstract string GetDescriptionFormat();
        public override string ToString() => 
            string.Format(CultureInfo.CurrentCulture, this.GetDescriptionFormat(), new object[] { base.TypeBeingConstructed.Name, this.propertyName });

        public string PropertyName =>
            this.propertyName;
    }
}

