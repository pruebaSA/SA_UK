namespace System.ComponentModel
{
    using System;
    using System.ComponentModel.Design;
    using System.ComponentModel.Design.Serialization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComVisible(true), Designer("System.Windows.Forms.Design.ComponentDocumentDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(IRootDesigner)), RootDesignerSerializer("System.ComponentModel.Design.Serialization.RootCodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true), Designer("System.ComponentModel.Design.ComponentDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(IDesigner)), TypeConverter(typeof(ComponentConverter))]
    public interface IComponent : IDisposable
    {
        event EventHandler Disposed;

        ISite Site { get; set; }
    }
}

