namespace System.Runtime.InteropServices
{
    using System;

    [Serializable, Obsolete("Use System.Runtime.InteropServices.ComTypes.LIBFLAGS instead. http://go.microsoft.com/fwlink/?linkid=14202", false), Flags]
    public enum LIBFLAGS : short
    {
        LIBFLAG_FCONTROL = 2,
        LIBFLAG_FHASDISKIMAGE = 8,
        LIBFLAG_FHIDDEN = 4,
        LIBFLAG_FRESTRICTED = 1
    }
}

