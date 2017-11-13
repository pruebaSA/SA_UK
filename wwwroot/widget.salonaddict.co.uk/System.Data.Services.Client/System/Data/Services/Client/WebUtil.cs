namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;

    internal static class WebUtil
    {
        private static bool? dataServiceCollectionAvailable = null;

        internal static void ApplyHeadersToRequest(Dictionary<string, string> headers, HttpWebRequest request, bool ignoreAcceptHeader)
        {
            foreach (KeyValuePair<string, string> pair in headers)
            {
                if (string.Equals(pair.Key, "Accept", StringComparison.Ordinal))
                {
                    if (!ignoreAcceptHeader)
                    {
                        request.Accept = pair.Value;
                    }
                }
                else if (string.Equals(pair.Key, "Content-Type", StringComparison.Ordinal))
                {
                    request.ContentType = pair.Value;
                }
                else
                {
                    request.Headers[pair.Key] = pair.Value;
                }
            }
        }

        internal static long CopyStream(Stream input, Stream output, ref byte[] refBuffer)
        {
            long num = 0L;
            byte[] buffer = refBuffer;
            if (buffer == null)
            {
                refBuffer = buffer = new byte[0x3e8];
            }
            int count = 0;
            while (input.CanRead && (0 < (count = input.Read(buffer, 0, buffer.Length))))
            {
                output.Write(buffer, 0, count);
                num += count;
            }
            return num;
        }

        internal static Type GetDataServiceCollectionOfT(params Type[] typeArguments)
        {
            if (DataServiceCollectionAvailable)
            {
                return GetDataServiceCollectionOfTType().MakeGenericType(typeArguments);
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Type GetDataServiceCollectionOfTType() => 
            typeof(DataServiceCollection<>);

        internal static void GetHttpWebResponse(InvalidOperationException exception, ref HttpWebResponse response)
        {
            if (response == null)
            {
                WebException exception2 = exception as WebException;
                if (exception2 != null)
                {
                    response = (HttpWebResponse) exception2.Response;
                }
            }
        }

        internal static bool IsDataServiceCollectionType(Type t) => 
            (DataServiceCollectionAvailable && (t == GetDataServiceCollectionOfTType()));

        internal static bool SuccessStatusCode(HttpStatusCode status) => 
            ((HttpStatusCode.OK <= status) && (status < HttpStatusCode.MultipleChoices));

        internal static Dictionary<string, string> WrapResponseHeaders(HttpWebResponse response)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>(EqualityComparer<string>.Default);
            if (response != null)
            {
                foreach (string str in response.Headers.AllKeys)
                {
                    dictionary.Add(str, response.Headers[str]);
                }
            }
            return dictionary;
        }

        private static bool DataServiceCollectionAvailable
        {
            get
            {
                if (!dataServiceCollectionAvailable.HasValue)
                {
                    try
                    {
                        dataServiceCollectionAvailable = new bool?(GetDataServiceCollectionOfTType() != null);
                    }
                    catch (FileNotFoundException)
                    {
                        dataServiceCollectionAvailable = false;
                    }
                }
                return dataServiceCollectionAvailable.Value;
            }
        }
    }
}

