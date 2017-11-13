namespace System.Data.Services.Design.Common
{
    using System;
    using System.Data.Services.Design;

    internal static class EDesignUtil
    {
        internal static T CheckArgumentNull<T>(T value, string parameterName) where T: class
        {
            if (value == null)
            {
                throw Error.ArgumentNull(parameterName);
            }
            return value;
        }

        internal static DataServiceCodeVersion CheckDataServiceCodeVersionArgument(DataServiceCodeVersion value, string paramName)
        {
            if ((value != DataServiceCodeVersion.V1) && (value != DataServiceCodeVersion.V2))
            {
                throw Error.ArgumentOutOfRange(paramName);
            }
            return value;
        }

        internal static LanguageOption CheckLanguageOptionArgument(LanguageOption value, string paramName)
        {
            if ((value != LanguageOption.GenerateCSharpCode) && (value != LanguageOption.GenerateVBCode))
            {
                throw Error.ArgumentOutOfRange(paramName);
            }
            return value;
        }

        internal static void CheckStringArgument(string value, string parameterName)
        {
            CheckArgumentNull<string>(value, parameterName);
            if (value.Length == 0)
            {
                throw InvalidStringArgument(parameterName);
            }
        }

        internal static InvalidOperationException InvalidOperation(string error) => 
            new InvalidOperationException(error);

        internal static ArgumentException InvalidStringArgument(string parameterName) => 
            new ArgumentException(Strings.InvalidStringArgument(parameterName));
    }
}

