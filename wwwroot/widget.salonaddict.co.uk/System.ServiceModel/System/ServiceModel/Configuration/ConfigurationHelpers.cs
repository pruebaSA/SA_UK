namespace System.ServiceModel.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Security.Permissions;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Web.Configuration;

    internal static class ConfigurationHelpers
    {
        internal static BindingCollectionElement GetAssociatedBindingCollectionElement(ContextInformation evaluationContext, string bindingCollectionName)
        {
            BindingCollectionElement element = null;
            BindingsSection associatedSection = (BindingsSection) GetAssociatedSection(evaluationContext, ConfigurationStrings.BindingsSectionGroupPath);
            if (associatedSection != null)
            {
                try
                {
                    element = associatedSection[bindingCollectionName];
                }
                catch (KeyNotFoundException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigBindingExtensionNotFound", new object[] { GetBindingsSectionPath(bindingCollectionName) })));
                }
                catch (NullReferenceException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigBindingExtensionNotFound", new object[] { GetBindingsSectionPath(bindingCollectionName) })));
                }
            }
            return element;
        }

        internal static object GetAssociatedSection(ContextInformation evalContext, string sectionPath)
        {
            object section = null;
            if (evalContext != null)
            {
                section = evalContext.GetSection(sectionPath);
            }
            else
            {
                if (ServiceHostingEnvironment.IsHosted)
                {
                    section = GetSectionFromWebConfigurationManager(sectionPath);
                }
                else
                {
                    section = ConfigurationManager.GetSection(sectionPath);
                }
                if (DiagnosticUtility.ShouldTraceVerbose)
                {
                    TraceUtility.TraceEvent(TraceEventType.Verbose, TraceCode.GetConfigurationSection, new StringTraceRecord("ConfigurationSection", sectionPath), null, null);
                }
            }
            if (section == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigSectionNotFound", new object[] { sectionPath })));
            }
            return section;
        }

        internal static BindingCollectionElement GetBindingCollectionElement(string bindingCollectionName) => 
            GetAssociatedBindingCollectionElement(null, bindingCollectionName);

        internal static string GetBindingsSectionPath(string sectionName) => 
            (ConfigurationStrings.BindingsSectionGroupPath + "/" + sectionName);

        internal static ContextInformation GetEvaluationContext(IConfigurationContextProviderInternal provider)
        {
            if (provider != null)
            {
                try
                {
                    return provider.GetEvaluationContext();
                }
                catch (ConfigurationErrorsException)
                {
                }
            }
            return null;
        }

        internal static ContextInformation GetOriginalEvaluationContext(IConfigurationContextProviderInternal provider)
        {
            if (provider != null)
            {
                try
                {
                    return provider.GetOriginalEvaluationContext();
                }
                catch (ConfigurationErrorsException)
                {
                }
            }
            return null;
        }

        internal static object GetSection(string sectionPath) => 
            GetAssociatedSection(null, sectionPath);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static object GetSectionFromWebConfigurationManager(string sectionPath)
        {
            if (ServiceHostingEnvironment.CurrentVirtualPath != null)
            {
                return WebConfigurationManager.GetSection(sectionPath, ServiceHostingEnvironment.CurrentVirtualPath);
            }
            return WebConfigurationManager.GetSection(sectionPath);
        }

        internal static string GetSectionPath(string sectionName) => 
            ("system.serviceModel" + "/" + sectionName);

        [SecurityCritical]
        internal static void SetIsPresent(ConfigurationElement element)
        {
            SetIsPresentWithAssert(element.GetType().GetProperty("ElementPresent", BindingFlags.NonPublic | BindingFlags.Instance), element, true);
        }

        [SecurityCritical, ReflectionPermission(SecurityAction.Assert, MemberAccess=true)]
        private static void SetIsPresentWithAssert(PropertyInfo elementPresent, ConfigurationElement element, bool value)
        {
            elementPresent.SetValue(element, value, null);
        }

        [SecurityCritical]
        internal static BindingCollectionElement UnsafeGetAssociatedBindingCollectionElement(ContextInformation evaluationContext, string bindingCollectionName)
        {
            BindingCollectionElement element = null;
            BindingsSection section = (BindingsSection) UnsafeGetAssociatedSection(evaluationContext, ConfigurationStrings.BindingsSectionGroupPath);
            if (section != null)
            {
                try
                {
                    element = section[bindingCollectionName];
                }
                catch (KeyNotFoundException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigBindingExtensionNotFound", new object[] { GetBindingsSectionPath(bindingCollectionName) })));
                }
                catch (NullReferenceException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigBindingExtensionNotFound", new object[] { GetBindingsSectionPath(bindingCollectionName) })));
                }
            }
            return element;
        }

        [SecurityCritical]
        internal static object UnsafeGetAssociatedSection(ContextInformation evalContext, string sectionPath)
        {
            object obj2 = null;
            if (evalContext != null)
            {
                obj2 = UnsafeGetSectionFromContext(evalContext, sectionPath);
            }
            else
            {
                if (ServiceHostingEnvironment.IsHosted)
                {
                    if (ServiceHostingEnvironment.CurrentVirtualPath != null)
                    {
                        obj2 = UnsafeGetSectionFromWebConfigurationManager(sectionPath, ServiceHostingEnvironment.CurrentVirtualPath);
                    }
                    else
                    {
                        obj2 = UnsafeGetSectionFromWebConfigurationManager(sectionPath);
                    }
                }
                else
                {
                    obj2 = UnsafeGetSectionFromConfigurationManager(sectionPath);
                }
                if (DiagnosticUtility.ShouldTraceVerbose)
                {
                    TraceUtility.TraceEvent(TraceEventType.Verbose, TraceCode.GetConfigurationSection, new StringTraceRecord("ConfigurationSection", sectionPath), null, null);
                }
            }
            if (obj2 == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigSectionNotFound", new object[] { sectionPath })));
            }
            return obj2;
        }

        [SecurityCritical]
        internal static BindingCollectionElement UnsafeGetBindingCollectionElement(string bindingCollectionName) => 
            UnsafeGetAssociatedBindingCollectionElement(null, bindingCollectionName);

        [SecurityCritical]
        internal static object UnsafeGetSection(string sectionPath) => 
            UnsafeGetAssociatedSection(null, sectionPath);

        [SecurityCritical, ConfigurationPermission(SecurityAction.Assert, Unrestricted=true)]
        private static object UnsafeGetSectionFromConfigurationManager(string sectionPath) => 
            ConfigurationManager.GetSection(sectionPath);

        [SecurityCritical, ConfigurationPermission(SecurityAction.Assert, Unrestricted=true)]
        internal static object UnsafeGetSectionFromContext(ContextInformation evalContext, string sectionPath) => 
            evalContext.GetSection(sectionPath);

        [MethodImpl(MethodImplOptions.NoInlining), SecurityCritical, ConfigurationPermission(SecurityAction.Assert, Unrestricted=true)]
        internal static object UnsafeGetSectionFromWebConfigurationManager(string sectionPath) => 
            WebConfigurationManager.GetSection(sectionPath);

        [MethodImpl(MethodImplOptions.NoInlining), SecurityCritical, ConfigurationPermission(SecurityAction.Assert, Unrestricted=true)]
        private static object UnsafeGetSectionFromWebConfigurationManager(string sectionPath, string virtualPath) => 
            WebConfigurationManager.GetSection(sectionPath, virtualPath);
    }
}

