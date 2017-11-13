namespace AjaxControlToolkit.Design
{
    using System;

    internal sealed class ExtenderVisiblePropertyAttribute : Attribute
    {
        private bool _value;
        public static ExtenderVisiblePropertyAttribute Default = No;
        public static ExtenderVisiblePropertyAttribute No = new ExtenderVisiblePropertyAttribute(false);
        public static ExtenderVisiblePropertyAttribute Yes = new ExtenderVisiblePropertyAttribute(true);

        public ExtenderVisiblePropertyAttribute(bool value)
        {
            this._value = value;
        }

        public override bool IsDefaultAttribute() => 
            !this._value;

        public bool Value =>
            this._value;
    }
}

