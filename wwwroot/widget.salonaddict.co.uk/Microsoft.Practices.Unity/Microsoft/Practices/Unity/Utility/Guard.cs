namespace Microsoft.Practices.Unity.Utility
{
    using Microsoft.Practices.Unity.Properties;
    using System;
    using System.Globalization;

    public static class Guard
    {
        public static void ArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void ArgumentNotNullOrEmpty(string argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
            if (argumentValue.Length == 0)
            {
                throw new ArgumentException(Resources.ArgumentMustNotBeEmpty, argumentName);
            }
        }

        private static string GetTypeName(object assignmentInstance)
        {
            try
            {
                return assignmentInstance.GetType().FullName;
            }
            catch (Exception)
            {
                return Resources.UnknownType;
            }
        }

        public static void InstanceIsAssignable(Type assignmentTargetType, object assignmentInstance, string argumentName)
        {
            if (assignmentTargetType == null)
            {
                throw new ArgumentNullException("assignmentTargetType");
            }
            if (assignmentInstance == null)
            {
                throw new ArgumentNullException("assignmentInstance");
            }
            if (!assignmentTargetType.IsInstanceOfType(assignmentInstance))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.TypesAreNotAssignable, new object[] { assignmentTargetType, GetTypeName(assignmentInstance) }), argumentName);
            }
        }

        public static void TypeIsAssignable(Type assignmentTargetType, Type assignmentValueType, string argumentName)
        {
            if (assignmentTargetType == null)
            {
                throw new ArgumentNullException("assignmentTargetType");
            }
            if (assignmentValueType == null)
            {
                throw new ArgumentNullException("assignmentValueType");
            }
            if (!assignmentTargetType.IsAssignableFrom(assignmentValueType))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.TypesAreNotAssignable, new object[] { assignmentTargetType, assignmentValueType }), argumentName);
            }
        }
    }
}

