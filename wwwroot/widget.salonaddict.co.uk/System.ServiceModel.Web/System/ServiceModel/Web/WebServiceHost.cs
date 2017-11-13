namespace System.ServiceModel.Web
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.Web;
    using System.Web.Configuration;

    public class WebServiceHost : ServiceHost
    {
        private static bool isAspNetAuthenticationModeDetermined;
        private static bool isWindowsAuthentication;

        public WebServiceHost()
        {
        }

        public WebServiceHost(object singletonInstance, params Uri[] baseAddresses) : base(singletonInstance, baseAddresses)
        {
        }

        public WebServiceHost(Type serviceType, params Uri[] baseAddresses) : base(serviceType, baseAddresses)
        {
        }

        internal static void AddAutomaticWebHttpBindingEndpoints(ServiceHost host, IDictionary<string, ContractDescription> implementedContracts, string multipleContractsErrorMessage)
        {
            AuthenticationSchemes authenticationSchemes;
            if (ServiceHostingEnvironment.IsHosted)
            {
                authenticationSchemes = GetAuthenticationSchemes(host.BaseAddresses[0]);
            }
            else
            {
                authenticationSchemes = AuthenticationSchemes.None;
            }
            Type implementedContract = null;
            foreach (Uri uri in host.BaseAddresses)
            {
                string scheme = uri.Scheme;
                if (object.ReferenceEquals(scheme, Uri.UriSchemeHttp) || object.ReferenceEquals(scheme, Uri.UriSchemeHttps))
                {
                    bool flag = false;
                    foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
                    {
                        if ((endpoint.Address != null) && EndpointAddress.UriEquals(endpoint.Address.Uri, uri, true, false))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        if (implementedContract == null)
                        {
                            if (implementedContracts.Count != 1)
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(multipleContractsErrorMessage));
                            }
                            foreach (ContractDescription description in implementedContracts.Values)
                            {
                                implementedContract = description.ContractType;
                                break;
                            }
                        }
                        WebHttpBinding binding = new WebHttpBinding();
                        if (object.ReferenceEquals(scheme, Uri.UriSchemeHttps))
                        {
                            binding.Security.Mode = WebHttpSecurityMode.Transport;
                        }
                        else if ((authenticationSchemes != AuthenticationSchemes.None) && (authenticationSchemes != AuthenticationSchemes.Anonymous))
                        {
                            binding.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;
                        }
                        else
                        {
                            binding.Security.Mode = WebHttpSecurityMode.None;
                        }
                        ServiceEndpoint serviceEndpoint = host.AddServiceEndpoint(implementedContract, binding, uri);
                        if (ServiceHostingEnvironment.IsHosted)
                        {
                            SetBindingCredentialBasedOnHostedEnvironment(serviceEndpoint, authenticationSchemes);
                        }
                    }
                }
            }
        }

        internal static AuthenticationSchemes GetAuthenticationSchemes(Uri baseAddress)
        {
            string str3;
            string fileName = VirtualPathUtility.GetFileName(baseAddress.AbsolutePath);
            string currentVirtualPath = ServiceHostingEnvironment.CurrentVirtualPath;
            if ((currentVirtualPath != null) && currentVirtualPath.EndsWith("/", StringComparison.Ordinal))
            {
                str3 = currentVirtualPath + fileName;
            }
            else
            {
                str3 = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[] { currentVirtualPath, fileName });
            }
            return HostedTransportConfigurationManager.MetabaseSettings.GetAuthenticationSchemes(str3);
        }

        private static bool IsRawContentMapperCompatibleClientOperation(OperationDescription operation, ref int numStreamOperations)
        {
            if ((operation.Messages.Count > 1) & !IsResponseStreamOrVoid(operation, ref numStreamOperations))
            {
                return false;
            }
            return true;
        }

        private static bool IsRawContentMapperCompatibleDispatchOperation(OperationDescription operation, ref int numStreamOperations)
        {
            UriTemplateDispatchFormatter throwAway = new UriTemplateDispatchFormatter(operation, null, new QueryStringConverter(), operation.DeclaringContract.Name, new Uri("http://localhost"));
            int num = throwAway.pathMapping.Count + throwAway.queryMapping.Count;
            bool isRequestCompatible = false;
            if (num > 0)
            {
                int tmp = 0;
                WebHttpBehavior.HideRequestUriTemplateParameters(operation, throwAway, delegate {
                    isRequestCompatible = IsRequestStreamOrVoid(operation, ref tmp);
                });
                numStreamOperations += tmp;
            }
            else
            {
                isRequestCompatible = IsRequestStreamOrVoid(operation, ref numStreamOperations);
            }
            return isRequestCompatible;
        }

        private static bool IsRequestStreamOrVoid(OperationDescription operation, ref int numStreamOperations)
        {
            MessageDescription message = operation.Messages[0];
            if (!WebHttpBehavior.IsTypedMessage(message) && !WebHttpBehavior.IsUntypedMessage(message))
            {
                if (message.Body.Parts.Count == 0)
                {
                    return true;
                }
                if (message.Body.Parts.Count == 1)
                {
                    if (IsStreamPart(message.Body.Parts[0].Type))
                    {
                        numStreamOperations++;
                        return true;
                    }
                    if (IsVoidPart(message.Body.Parts[0].Type))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsResponseStreamOrVoid(OperationDescription operation, ref int numStreamOperations)
        {
            if (operation.Messages.Count < 1)
            {
                return true;
            }
            MessageDescription message = operation.Messages[1];
            if ((!WebHttpBehavior.IsTypedMessage(message) && !WebHttpBehavior.IsUntypedMessage(message)) && (message.Body.Parts.Count == 0))
            {
                if ((message.Body.ReturnValue == null) || IsVoidPart(message.Body.ReturnValue.Type))
                {
                    return true;
                }
                if (IsStreamPart(message.Body.ReturnValue.Type))
                {
                    numStreamOperations++;
                    return true;
                }
            }
            return false;
        }

        private static bool IsStreamPart(Type type) => 
            (type == typeof(Stream));

        private static bool IsVoidPart(Type type)
        {
            if (type != null)
            {
                return (type == typeof(void));
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool IsWindowsAuthenticationConfigured()
        {
            if (!isAspNetAuthenticationModeDetermined)
            {
                AuthenticationSection section = (AuthenticationSection) WebConfigurationManager.GetSection("system.web/authentication");
                if (section != null)
                {
                    isWindowsAuthentication = section.Mode == AuthenticationMode.Windows;
                }
                isAspNetAuthenticationModeDetermined = true;
            }
            return isWindowsAuthentication;
        }

        protected override void OnOpening()
        {
            base.OnOpening();
            if (base.Description != null)
            {
                ServiceDebugBehavior behavior = base.Description.Behaviors.Find<ServiceDebugBehavior>();
                if (behavior != null)
                {
                    behavior.HttpHelpPageEnabled = false;
                    behavior.HttpsHelpPageEnabled = false;
                }
                ServiceMetadataBehavior behavior2 = base.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (behavior2 != null)
                {
                    behavior2.HttpGetEnabled = false;
                    behavior2.HttpsGetEnabled = false;
                }
                AddAutomaticWebHttpBindingEndpoints(this, base.ImplementedContracts, SR2.GetString(SR2.HttpTransferServiceHostMultipleContracts, new object[] { base.Description.Name }));
                foreach (ServiceEndpoint endpoint in base.Description.Endpoints)
                {
                    if ((endpoint.Binding != null) && (endpoint.Binding.CreateBindingElements().Find<WebMessageEncodingBindingElement>() != null))
                    {
                        SetRawContentTypeMapperIfNecessary(endpoint, true);
                        if (endpoint.Behaviors.Find<WebHttpBehavior>() == null)
                        {
                            endpoint.Behaviors.Add(new WebHttpBehavior());
                        }
                    }
                }
            }
        }

        private static void SetBindingCredentialBasedOnHostedEnvironment(ServiceEndpoint serviceEndpoint, AuthenticationSchemes supportedSchemes)
        {
            if (ServiceHostingEnvironment.IsSimpleApplicationHost && (supportedSchemes == (AuthenticationSchemes.Anonymous | AuthenticationSchemes.Ntlm)))
            {
                if (IsWindowsAuthenticationConfigured())
                {
                    supportedSchemes = AuthenticationSchemes.Ntlm;
                }
                else
                {
                    supportedSchemes = AuthenticationSchemes.Anonymous;
                }
            }
            WebHttpBinding binding = serviceEndpoint.Binding as WebHttpBinding;
            switch (supportedSchemes)
            {
                case AuthenticationSchemes.Digest:
                    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Digest;
                    return;

                case AuthenticationSchemes.Negotiate:
                case AuthenticationSchemes.IntegratedWindowsAuthentication:
                    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                    return;

                case AuthenticationSchemes.Ntlm:
                    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
                    return;

                case AuthenticationSchemes.Basic:
                    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                    return;

                case AuthenticationSchemes.Anonymous:
                    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                    return;
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.HttpTransferServiceHostBadAuthSchemes, new object[] { supportedSchemes })));
        }

        internal static void SetRawContentTypeMapperIfNecessary(ServiceEndpoint endpoint, bool isDispatch)
        {
            Binding binding = endpoint.Binding;
            ContractDescription contract = endpoint.Contract;
            if (binding != null)
            {
                CustomBinding binding2 = new CustomBinding(binding);
                WebMessageEncodingBindingElement element = binding2.Elements.Find<WebMessageEncodingBindingElement>();
                if ((element != null) && (element.ContentTypeMapper == null))
                {
                    bool flag = true;
                    int numStreamOperations = 0;
                    foreach (OperationDescription description2 in contract.Operations)
                    {
                        if (!(isDispatch ? IsRawContentMapperCompatibleDispatchOperation(description2, ref numStreamOperations) : IsRawContentMapperCompatibleClientOperation(description2, ref numStreamOperations)))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag && (numStreamOperations > 0))
                    {
                        element.ContentTypeMapper = RawContentTypeMapper.Instance;
                        endpoint.Binding = binding2;
                    }
                }
            }
        }
    }
}

