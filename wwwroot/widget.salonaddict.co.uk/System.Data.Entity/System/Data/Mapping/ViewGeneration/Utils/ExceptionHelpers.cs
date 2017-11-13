namespace System.Data.Mapping.ViewGeneration.Utils
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Text;

    internal static class ExceptionHelpers
    {
        internal static void CheckAndThrowRes(bool condition, Func<string> formatMessage)
        {
            if (!condition)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(Strings.ViewGen_Internal_Error);
                builder.AppendLine(formatMessage());
                throw EntityUtil.InvalidOperation(builder.ToString());
            }
        }

        internal static void CheckAndThrowResArgs(bool condition, System.Func<object, object, string> formatMessage, object arg1, object arg2)
        {
            if (!condition)
            {
                throw EntityUtil.InvalidOperation(Strings.ViewGen_Internal_Error + formatMessage(arg1, arg2));
            }
        }

        internal static void ThrowMappingException(ErrorLog errorLog, ConfigViewGenerator config)
        {
            InternalMappingException exception = new InternalMappingException(errorLog.ToUserString(), errorLog);
            if (config.IsNormalTracing)
            {
                exception.ErrorLog.PrintTrace();
            }
            throw exception;
        }

        internal static void ThrowMappingException(ErrorLog.Record errorRecord, ConfigViewGenerator config)
        {
            InternalMappingException exception = new InternalMappingException(errorRecord.ToUserString(), errorRecord);
            if (config.IsNormalTracing)
            {
                exception.ErrorLog.PrintTrace();
            }
            throw exception;
        }
    }
}

