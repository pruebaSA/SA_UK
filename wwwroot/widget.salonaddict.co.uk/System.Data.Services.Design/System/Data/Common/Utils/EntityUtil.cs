namespace System.Data.Common.Utils
{
    using System;
    using System.Data.Services.Design;

    internal static class EntityUtil
    {
        internal static void CheckArgumentNull<T>(T value, string parameterName) where T: class
        {
            System.Data.Services.Design.EntityUtil.CheckArgumentNull<T>(value, parameterName);
        }
    }
}

