namespace Accessibility
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, Guid("7852B78D-1CFD-41C1-A615-9C0C85960B5F"), InterfaceType((short) 1), ComConversionLoss]
    public interface IAccIdentity
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetIdentityString([In] uint dwIDChild, [Out] IntPtr ppIDString, out uint pdwIDStringLen);
    }
}

