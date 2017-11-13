namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.ServiceModel;

    [DataContract]
    internal sealed class BaseUriWithWildcard
    {
        [DataMember]
        private Uri baseAddress;
        [DataMember]
        private System.ServiceModel.HostNameComparisonMode hostNameComparisonMode;
        private const int HttpsUriDefaultPort = 0x1bb;
        private const int HttpUriDefaultPort = 80;
        private const string plus = "+";
        private const char segmentDelimiter = '/';
        private const string star = "*";

        internal BaseUriWithWildcard(Uri baseAddress, System.ServiceModel.HostNameComparisonMode hostNameComparisonMode)
        {
            this.baseAddress = baseAddress;
            this.hostNameComparisonMode = hostNameComparisonMode;
        }

        private BaseUriWithWildcard(string protocol, int defaultPort, string binding, int segmentCount, string path)
        {
            string[] strArray = SplitBinding(binding);
            if (strArray.Length != segmentCount)
            {
                string sampleBinding = GetSampleBinding(protocol);
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new UriFormatException(System.ServiceModel.SR.GetString("Hosting_MisformattedBinding", new object[] { binding, protocol, sampleBinding })));
            }
            int index = segmentCount - 1;
            string host = this.ParseHostAndHostNameComparisonMode(strArray[index]);
            int result = -1;
            if (--index >= 0)
            {
                string str3 = strArray[index].Trim();
                if (!string.IsNullOrEmpty(str3) && !int.TryParse(str3, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new UriFormatException(System.ServiceModel.SR.GetString("Hosting_MisformattedPort", new object[] { protocol, binding, str3 })));
                }
                if (result == defaultPort)
                {
                    result = -1;
                }
            }
            try
            {
                if (path != null)
                {
                    this.baseAddress = new UriBuilder(protocol, host, result, path).Uri;
                }
                else
                {
                    this.baseAddress = new UriBuilder(protocol, host, result, HostingEnvironmentWrapper.ApplicationVirtualPath).Uri;
                }
            }
            catch (Exception exception)
            {
                if (System.ServiceModel.DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                if (System.ServiceModel.DiagnosticUtility.ShouldTraceError)
                {
                    System.ServiceModel.DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Error);
                }
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new UriFormatException(System.ServiceModel.SR.GetString("Hosting_MisformattedBindingData", new object[] { binding, protocol })));
            }
        }

        internal static BaseUriWithWildcard CreateHttpsUri(string binding)
        {
            if (binding == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("binding"));
            }
            return CreateHttpsUri(binding, null);
        }

        private static BaseUriWithWildcard CreateHttpsUri(string binding, string path) => 
            new BaseUriWithWildcard(Uri.UriSchemeHttps, 0x1bb, binding, 3, path);

        internal static BaseUriWithWildcard CreateHttpUri(string binding)
        {
            if (binding == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("binding"));
            }
            return CreateHttpUri(binding, null);
        }

        private static BaseUriWithWildcard CreateHttpUri(string binding, string path) => 
            new BaseUriWithWildcard(Uri.UriSchemeHttp, 80, binding, 3, path);

        internal static BaseUriWithWildcard CreateMsmqFormatNameUri(string binding)
        {
            if (binding == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("binding"));
            }
            return CreateMsmqFormatNameUri(binding, null);
        }

        private static BaseUriWithWildcard CreateMsmqFormatNameUri(string binding, string path) => 
            new BaseUriWithWildcard(MsmqUri.FormatNameAddressTranslator.Scheme, -1, binding, 1, path);

        internal static BaseUriWithWildcard CreateMsmqUri(string binding)
        {
            if (binding == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("binding"));
            }
            return CreateMsmqUri(binding, null);
        }

        private static BaseUriWithWildcard CreateMsmqUri(string binding, string path) => 
            new BaseUriWithWildcard(MsmqUri.NetMsmqAddressTranslator.Scheme, -1, binding, 1, path);

        internal static BaseUriWithWildcard CreatePipeUri(string binding)
        {
            if (binding == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("binding"));
            }
            return CreatePipeUri(binding, null);
        }

        private static BaseUriWithWildcard CreatePipeUri(string binding, string path) => 
            new BaseUriWithWildcard(Uri.UriSchemeNetPipe, -1, binding, 1, path);

        internal static BaseUriWithWildcard CreateTcpUri(string binding)
        {
            if (binding == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("binding"));
            }
            return CreateTcpUri(binding, null);
        }

        private static BaseUriWithWildcard CreateTcpUri(string binding, string path) => 
            new BaseUriWithWildcard(Uri.UriSchemeNetTcp, 0x328, binding, 2, path);

        internal static BaseUriWithWildcard CreateUri(string protocol, string binding, string path)
        {
            if (protocol == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("protocol"));
            }
            if (binding == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("binding"));
            }
            if (path == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("path"));
            }
            if (protocol.Equals(Uri.UriSchemeHttp))
            {
                return CreateHttpUri(binding, path);
            }
            if (protocol.Equals(Uri.UriSchemeHttps))
            {
                return CreateHttpsUri(binding, path);
            }
            if (protocol.Equals(Uri.UriSchemeNetTcp))
            {
                return CreateTcpUri(binding, path);
            }
            if (protocol.Equals(Uri.UriSchemeNetPipe))
            {
                return CreatePipeUri(binding, path);
            }
            if (protocol.Equals(MsmqUri.NetMsmqAddressTranslator.Scheme))
            {
                return CreateMsmqUri(binding, path);
            }
            if (!protocol.Equals(MsmqUri.FormatNameAddressTranslator.Scheme))
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new UriFormatException(System.ServiceModel.SR.GetString("Hosting_NotSupportedProtocol", new object[] { binding })));
            }
            return CreateMsmqFormatNameUri(binding, path);
        }

        public override bool Equals(object other)
        {
            BaseUriWithWildcard wildcard = other as BaseUriWithWildcard;
            if (((wildcard == null) || (this.GetHashCode() != wildcard.GetHashCode())) || (this.HostNameComparisonMode != wildcard.HostNameComparisonMode))
            {
                return false;
            }
            if (this.BaseAddress.Scheme != wildcard.BaseAddress.Scheme)
            {
                return false;
            }
            if ((this.BaseAddress.Port != wildcard.BaseAddress.Port) && (((this.baseAddress.Port == 0x328) && (wildcard.baseAddress.Port != -1)) || ((this.baseAddress.Port == -1) && (wildcard.baseAddress.Port != 0x328))))
            {
                return false;
            }
            if (this.HostNameComparisonMode != System.ServiceModel.HostNameComparisonMode.Exact)
            {
                string components = this.BaseAddress.GetComponents(UriComponents.KeepDelimiter | UriComponents.Path, UriFormat.Unescaped);
                string str2 = wildcard.BaseAddress.GetComponents(UriComponents.KeepDelimiter | UriComponents.Path, UriFormat.Unescaped);
                return components.Equals(str2);
            }
            return this.BaseAddress.Equals(wildcard.BaseAddress);
        }

        public override int GetHashCode() => 
            ((this.baseAddress.Port ^ this.baseAddress.PathAndQuery.GetHashCode()) ^ this.HostNameComparisonMode);

        private static string GetSampleBinding(string protocol)
        {
            if (string.Compare(protocol, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ":80:";
            }
            if (string.Compare(protocol, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return ":443:";
            }
            if (string.Compare(protocol, Uri.UriSchemeNetTcp, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return "808:*";
            }
            if (((string.Compare(protocol, Uri.UriSchemeNetPipe, StringComparison.OrdinalIgnoreCase) != 0) && (string.Compare(protocol, MsmqUri.NetMsmqAddressTranslator.Scheme, StringComparison.OrdinalIgnoreCase) != 0)) && (string.Compare(protocol, MsmqUri.FormatNameAddressTranslator.Scheme, StringComparison.OrdinalIgnoreCase) != 0))
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new UriFormatException(System.ServiceModel.SR.GetString("Hosting_NotSupportedProtocol", new object[] { protocol })));
            }
            return "*";
        }

        internal bool IsBaseOf(Uri fullAddress)
        {
            if (this.baseAddress.Scheme != fullAddress.Scheme)
            {
                return false;
            }
            if (this.baseAddress.Port != fullAddress.Port)
            {
                return false;
            }
            if ((this.HostNameComparisonMode == System.ServiceModel.HostNameComparisonMode.Exact) && (string.Compare(this.baseAddress.Host, fullAddress.Host, StringComparison.OrdinalIgnoreCase) != 0))
            {
                return false;
            }
            string components = this.baseAddress.GetComponents(UriComponents.KeepDelimiter | UriComponents.Path, UriFormat.Unescaped);
            string strA = fullAddress.GetComponents(UriComponents.KeepDelimiter | UriComponents.Path, UriFormat.Unescaped);
            if (components.Length > strA.Length)
            {
                return false;
            }
            if (((components.Length < strA.Length) && (components[components.Length - 1] != '/')) && (strA[components.Length] != '/'))
            {
                return false;
            }
            return (string.Compare(strA, 0, components, 0, components.Length, StringComparison.OrdinalIgnoreCase) == 0);
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            UriSchemeKeyedCollection.ValidateBaseAddress(this.baseAddress, "context");
            if (!HostNameComparisonModeHelper.IsDefined(this.HostNameComparisonMode))
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("context", System.ServiceModel.SR.GetString("Hosting_BaseUriDeserializedNotValid"));
            }
        }

        private string ParseHostAndHostNameComparisonMode(string host)
        {
            if (string.IsNullOrEmpty(host) || host.Equals("*"))
            {
                this.hostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.WeakWildcard;
                host = DnsCache.MachineName;
                return host;
            }
            if (host.Equals("+"))
            {
                this.hostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
                host = DnsCache.MachineName;
                return host;
            }
            this.hostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.Exact;
            return host;
        }

        private static string[] SplitBinding(string binding)
        {
            bool flag = false;
            string[] strArray = null;
            List<int> list = null;
            for (int i = 0; i < binding.Length; i++)
            {
                if (flag && (binding[i] == ']'))
                {
                    flag = false;
                }
                else if (binding[i] == '[')
                {
                    flag = true;
                }
                else if (!flag && (binding[i] == ':'))
                {
                    if (list == null)
                    {
                        list = new List<int>();
                    }
                    list.Add(i);
                }
            }
            if (list == null)
            {
                return new string[] { binding };
            }
            strArray = new string[list.Count + 1];
            int startIndex = 0;
            for (int j = 0; j < strArray.Length; j++)
            {
                if (j < list.Count)
                {
                    int num4 = list[j];
                    strArray[j] = binding.Substring(startIndex, num4 - startIndex);
                    startIndex = num4 + 1;
                }
                else if (startIndex < binding.Length)
                {
                    strArray[j] = binding.Substring(startIndex, binding.Length - startIndex);
                }
                else
                {
                    strArray[j] = string.Empty;
                }
            }
            return strArray;
        }

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[] { this.HostNameComparisonMode, this.BaseAddress });

        internal Uri BaseAddress =>
            this.baseAddress;

        internal System.ServiceModel.HostNameComparisonMode HostNameComparisonMode =>
            this.hostNameComparisonMode;
    }
}

