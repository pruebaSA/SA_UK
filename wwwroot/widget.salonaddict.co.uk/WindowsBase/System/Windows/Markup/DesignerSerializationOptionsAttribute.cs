namespace System.Windows.Markup
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple=false)]
    public sealed class DesignerSerializationOptionsAttribute : Attribute
    {
        private System.Windows.Markup.DesignerSerializationOptions _designerSerializationOptions;

        public DesignerSerializationOptionsAttribute(System.Windows.Markup.DesignerSerializationOptions designerSerializationOptions)
        {
            if (System.Windows.Markup.DesignerSerializationOptions.SerializeAsAttribute != designerSerializationOptions)
            {
                throw new InvalidEnumArgumentException(System.Windows.SR.Get("Enum_Invalid", new object[] { "DesignerSerializationOptions" }));
            }
            this._designerSerializationOptions = designerSerializationOptions;
        }

        public System.Windows.Markup.DesignerSerializationOptions DesignerSerializationOptions =>
            this._designerSerializationOptions;
    }
}

