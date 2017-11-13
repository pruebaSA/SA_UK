namespace System.Data.Services.Design
{
    using System;
    using System.Data.Services.Design.Common;

    internal static class EntityUtil
    {
        internal static void CheckArgumentNull<T>(T value, string parameterName) where T: class
        {
            EDesignUtil.CheckArgumentNull<T>(value, parameterName);
        }

        internal static void CheckStringArgument(string value, string parameterName)
        {
            EDesignUtil.CheckStringArgument(value, parameterName);
        }
    }
}

