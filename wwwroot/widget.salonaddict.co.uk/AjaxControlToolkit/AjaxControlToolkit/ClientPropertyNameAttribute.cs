namespace AjaxControlToolkit
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ClientPropertyNameAttribute : Attribute
    {
        private string _propertyName;

        public ClientPropertyNameAttribute()
        {
        }

        public ClientPropertyNameAttribute(string propertyName)
        {
            this._propertyName = propertyName;
        }

        public override bool IsDefaultAttribute() => 
            string.IsNullOrEmpty(this.PropertyName);

        public string PropertyName =>
            this._propertyName;
    }
}

