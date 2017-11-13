﻿namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    internal class Com2AboutBoxPropertyDescriptor : Com2PropertyDescriptor
    {
        private TypeConverter converter;
        private UITypeEditor editor;

        public Com2AboutBoxPropertyDescriptor() : base(-552, "About", new Attribute[] { new DispIdAttribute(-552), DesignerSerializationVisibilityAttribute.Hidden, new DescriptionAttribute(System.Windows.Forms.SR.GetString("AboutBoxDesc")), new ParenthesizePropertyNameAttribute(true) }, true, typeof(string), null, false)
        {
        }

        public override bool CanResetValue(object component) => 
            false;

        public override object GetEditor(System.Type editorBaseType)
        {
            if ((editorBaseType == typeof(UITypeEditor)) && (this.editor == null))
            {
                this.editor = new AboutBoxUITypeEditor();
            }
            return this.editor;
        }

        public override object GetValue(object component) => 
            "";

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            throw new ArgumentException();
        }

        public override bool ShouldSerializeValue(object component) => 
            false;

        public override System.Type ComponentType =>
            typeof(UnsafeNativeMethods.IDispatch);

        public override TypeConverter Converter
        {
            get
            {
                if (this.converter == null)
                {
                    this.converter = new TypeConverter();
                }
                return this.converter;
            }
        }

        public override bool IsReadOnly =>
            true;

        public override System.Type PropertyType =>
            typeof(string);

        public class AboutBoxUITypeEditor : UITypeEditor
        {
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                object instance = context.Instance;
                if (Marshal.IsComObject(instance) && (instance is UnsafeNativeMethods.IDispatch))
                {
                    UnsafeNativeMethods.IDispatch dispatch = (UnsafeNativeMethods.IDispatch) instance;
                    NativeMethods.tagEXCEPINFO pExcepInfo = new NativeMethods.tagEXCEPINFO();
                    Guid empty = Guid.Empty;
                    dispatch.Invoke(-552, ref empty, SafeNativeMethods.GetThreadLCID(), 1, new NativeMethods.tagDISPPARAMS(), null, pExcepInfo, null);
                }
                return value;
            }

            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => 
                UITypeEditorEditStyle.Modal;
        }
    }
}

