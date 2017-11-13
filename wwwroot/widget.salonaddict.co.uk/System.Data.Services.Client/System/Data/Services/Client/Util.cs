namespace System.Data.Services.Client
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Xml;

    internal static class Util
    {
        internal const string CodeGeneratorToolName = "System.Data.Services.Design";
        internal static readonly Version DataServiceVersion1 = new Version(1, 0);
        internal static readonly Version DataServiceVersion2 = new Version(2, 0);
        internal static readonly Version DataServiceVersionEmpty = new Version(0, 0);
        internal static readonly char[] ForwardSlash = new char[] { '/' };
        internal static readonly Version MaxResponseVersion = DataServiceVersion2;
        internal static readonly Version[] SupportedResponseVersions = new Version[] { DataServiceVersion1, DataServiceVersion2 };
        internal const string VersionSuffix = ";NetFx";
        private static char[] whitespaceForTracing = new char[] { '\r', '\n', ' ', ' ', ' ', ' ', ' ' };

        internal static object ActivatorCreateInstance(Type type, params object[] arguments) => 
            Activator.CreateInstance(type, arguments);

        internal static bool AreSame(string value1, string value2) => 
            (value1 == value2);

        internal static bool AreSame(XmlReader reader, string localName, string namespaceUri) => 
            ((((XmlNodeType.Element == reader.NodeType) || (XmlNodeType.EndElement == reader.NodeType)) && AreSame(reader.LocalName, localName)) && AreSame(reader.NamespaceURI, namespaceUri));

        internal static void CheckArgumentNotEmpty(string value, string parameterName)
        {
            CheckArgumentNull<string>(value, parameterName);
            if (value.Length == 0)
            {
                throw Error.Argument(Strings.Util_EmptyString, parameterName);
            }
        }

        internal static void CheckArgumentNotEmpty<T>(T[] value, string parameterName) where T: class
        {
            CheckArgumentNull<T[]>(value, parameterName);
            if (value.Length == 0)
            {
                throw Error.Argument(Strings.Util_EmptyArray, parameterName);
            }
            for (int i = 0; i < value.Length; i++)
            {
                if (object.ReferenceEquals(value[i], null))
                {
                    throw Error.Argument(Strings.Util_NullArrayElement, parameterName);
                }
            }
        }

        internal static T CheckArgumentNull<T>(T value, string parameterName) where T: class
        {
            if (value == null)
            {
                throw Error.ArgumentNull(parameterName);
            }
            return value;
        }

        internal static MergeOption CheckEnumerationValue(MergeOption value, string parameterName)
        {
            switch (value)
            {
                case MergeOption.AppendOnly:
                case MergeOption.OverwriteChanges:
                case MergeOption.PreserveChanges:
                case MergeOption.NoTracking:
                    return value;
            }
            throw Error.ArgumentOutOfRange(parameterName);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static object ConstructorInvoke(ConstructorInfo constructor, object[] arguments) => 
            constructor?.Invoke(arguments);

        internal static bool ContainsReference<T>(T[] array, T value) where T: class => 
            (0 <= IndexOfReference<T>(array, value));

        internal static Uri CreateUri(string value, UriKind kind)
        {
            if (value != null)
            {
                return new Uri(value, kind);
            }
            return null;
        }

        internal static Uri CreateUri(Uri baseUri, Uri requestUri)
        {
            CheckArgumentNull<Uri>(requestUri, "requestUri");
            if (!requestUri.IsAbsoluteUri)
            {
                if (baseUri.OriginalString.EndsWith("/", StringComparison.Ordinal))
                {
                    if (requestUri.OriginalString.StartsWith("/", StringComparison.Ordinal))
                    {
                        requestUri = new Uri(baseUri, CreateUri(requestUri.OriginalString.TrimStart(ForwardSlash), UriKind.Relative));
                        return requestUri;
                    }
                    requestUri = new Uri(baseUri, requestUri);
                    return requestUri;
                }
                requestUri = CreateUri(baseUri.OriginalString + "/" + requestUri.OriginalString.TrimStart(ForwardSlash), UriKind.Absolute);
            }
            return requestUri;
        }

        [Conditional("DEBUG")]
        internal static void DebugInjectFault(string state)
        {
        }

        internal static string DereferenceIdentity(string uri) => 
            uri;

        internal static void Dispose<T>(ref T disposable) where T: class, IDisposable
        {
            Dispose<T>((T) disposable);
            disposable = default(T);
        }

        internal static void Dispose<T>(T disposable) where T: class, IDisposable
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        internal static bool DoesNullAttributeSayTrue(XmlReader reader)
        {
            string attribute = reader.GetAttribute("null", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            return ((attribute != null) && XmlConvert.ToBoolean(attribute));
        }

        internal static bool DoNotHandleException(Exception ex)
        {
            if (ex == null)
            {
                return false;
            }
            return (((ex is StackOverflowException) || (ex is OutOfMemoryException)) || (ex is ThreadAbortException));
        }

        internal static Type GetTypeAllowingNull(Type type)
        {
            if (!TypeAllowsNull(type))
            {
                return typeof(Nullable<>).MakeGenericType(new Type[] { type });
            }
            return type;
        }

        internal static char[] GetWhitespaceForTracing(int depth)
        {
            char[] whitespaceForTracing = Util.whitespaceForTracing;
            while (whitespaceForTracing.Length <= depth)
            {
                char[] chArray2 = new char[2 * whitespaceForTracing.Length];
                chArray2[0] = '\r';
                chArray2[1] = '\n';
                for (int i = 2; i < chArray2.Length; i++)
                {
                    chArray2[i] = ' ';
                }
                Interlocked.CompareExchange<char[]>(ref Util.whitespaceForTracing, chArray2, whitespaceForTracing);
                whitespaceForTracing = chArray2;
            }
            return whitespaceForTracing;
        }

        internal static int IndexOfReference<T>(T[] array, T value) where T: class
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (object.ReferenceEquals(array[i], value))
                {
                    return i;
                }
            }
            return -1;
        }

        internal static bool IsKnownClientExcption(Exception ex) => 
            (((ex is DataServiceClientException) || (ex is DataServiceQueryException)) || (ex is DataServiceRequestException));

        private static bool IsNullableType(Type type) => 
            (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));

        internal static T NullCheck<T>(T value, InternalError errorcode) where T: class
        {
            if (object.ReferenceEquals(value, null))
            {
                Error.ThrowInternalError(errorcode);
            }
            return value;
        }

        internal static string ReferenceIdentity(string uri) => 
            uri;

        internal static void SetNextLinkForCollection(object collection, DataServiceQueryContinuation continuation)
        {
            foreach (PropertyInfo info in collection.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (((info.Name == "Continuation") && info.CanWrite) && typeof(DataServiceQueryContinuation).IsAssignableFrom(info.PropertyType))
                {
                    info.SetValue(collection, continuation, null);
                }
            }
        }

        [Conditional("TRACE")]
        internal static void TraceElement(XmlReader reader, TextWriter writer)
        {
            if (writer != null)
            {
                writer.Write(GetWhitespaceForTracing(2 + reader.Depth), 0, 2 + reader.Depth);
                writer.Write("<{0}", reader.Name);
                if (reader.MoveToFirstAttribute())
                {
                    do
                    {
                        writer.Write(" {0}=\"{1}\"", reader.Name, reader.Value);
                    }
                    while (reader.MoveToNextAttribute());
                    reader.MoveToElement();
                }
                writer.Write(reader.IsEmptyElement ? " />" : ">");
            }
        }

        [Conditional("TRACE")]
        internal static void TraceEndElement(XmlReader reader, TextWriter writer, bool indent)
        {
            if (writer != null)
            {
                if (indent)
                {
                    writer.Write(GetWhitespaceForTracing(2 + reader.Depth), 0, 2 + reader.Depth);
                }
                writer.Write("</{0}>", reader.Name);
            }
        }

        [Conditional("TRACE")]
        internal static void TraceText(TextWriter writer, string value)
        {
            if (writer != null)
            {
                writer.Write(value);
            }
        }

        internal static bool TypeAllowsNull(Type type)
        {
            if (type.IsValueType)
            {
                return IsNullableType(type);
            }
            return true;
        }

        internal static string UriToString(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
            {
                return uri.OriginalString;
            }
            return uri.AbsoluteUri;
        }
    }
}

