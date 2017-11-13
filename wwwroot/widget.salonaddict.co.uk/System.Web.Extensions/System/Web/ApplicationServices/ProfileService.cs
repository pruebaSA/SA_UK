namespace System.Web.ApplicationServices
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Configuration.Provider;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Web;
    using System.Web.Management;
    using System.Web.Profile;
    using System.Web.Resources;

    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider)), ServiceBehavior(Namespace="http://asp.net/ApplicationServices/v200", InstanceContextMode=InstanceContextMode.Single, ConcurrencyMode=ConcurrencyMode.Multiple), ServiceContract(Namespace="http://asp.net/ApplicationServices/v200"), AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Required), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ProfileService
    {
        private static EventHandler<ValidatingPropertiesEventArgs> _validatingProperties;
        private static object _validatingPropertiesEventHandlerLock = new object();

        public static  event EventHandler<ValidatingPropertiesEventArgs> ValidatingProperties
        {
            add
            {
                lock (_validatingPropertiesEventHandlerLock)
                {
                    _validatingProperties = (EventHandler<ValidatingPropertiesEventArgs>) Delegate.Combine(_validatingProperties, value);
                }
            }
            remove
            {
                lock (_validatingPropertiesEventHandlerLock)
                {
                    _validatingProperties = (EventHandler<ValidatingPropertiesEventArgs>) Delegate.Remove(_validatingProperties, value);
                }
            }
        }

        internal ProfileService()
        {
        }

        [OperationContract]
        public Dictionary<string, object> GetAllPropertiesForCurrentUser(bool authenticatedUserOnly)
        {
            ApplicationServiceHelper.EnsureProfileServiceEnabled();
            if (authenticatedUserOnly)
            {
                ApplicationServiceHelper.EnsureAuthenticated(HttpContext.Current);
            }
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            try
            {
                ProfileBase profileForCurrentUser = GetProfileForCurrentUser(authenticatedUserOnly);
                if (profileForCurrentUser == null)
                {
                    return null;
                }
                Dictionary<string, object> profileAllowedGet = ApplicationServiceHelper.ProfileAllowedGet;
                if ((profileAllowedGet == null) || (profileAllowedGet.Count == 0))
                {
                    return dictionary;
                }
                foreach (KeyValuePair<string, object> pair in profileAllowedGet)
                {
                    string key = pair.Key;
                    SettingsPropertyValue propertyValue = GetPropertyValue(profileForCurrentUser, key);
                    if (propertyValue != null)
                    {
                        dictionary.Add(key, propertyValue.PropertyValue);
                        propertyValue.IsDirty = false;
                    }
                }
            }
            catch (Exception exception)
            {
                this.LogException(exception);
                throw;
            }
            return dictionary;
        }

        private static ProfileBase GetProfileForCurrentUser(bool authenticatedUserOnly)
        {
            HttpContext current = HttpContext.Current;
            IPrincipal currentUser = ApplicationServiceHelper.GetCurrentUser(current);
            string anonymousID = null;
            bool isAuthenticated = false;
            if (((currentUser == null) || (currentUser.Identity == null)) || string.IsNullOrEmpty(currentUser.Identity.Name))
            {
                isAuthenticated = false;
                if ((!authenticatedUserOnly && (current != null)) && !string.IsNullOrEmpty(current.Request.AnonymousID))
                {
                    anonymousID = current.Request.AnonymousID;
                }
            }
            else
            {
                anonymousID = currentUser.Identity.Name;
                isAuthenticated = currentUser.Identity.IsAuthenticated;
            }
            if (isAuthenticated || (!authenticatedUserOnly && !string.IsNullOrEmpty(anonymousID)))
            {
                return ProfileBase.Create(anonymousID, isAuthenticated);
            }
            if (current != null)
            {
                throw new HttpException(AtlasWeb.UserIsNotAuthenticated);
            }
            throw new Exception(AtlasWeb.UserIsNotAuthenticated);
        }

        [OperationContract]
        public Dictionary<string, object> GetPropertiesForCurrentUser(IEnumerable<string> properties, bool authenticatedUserOnly)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }
            ApplicationServiceHelper.EnsureProfileServiceEnabled();
            if (authenticatedUserOnly)
            {
                ApplicationServiceHelper.EnsureAuthenticated(HttpContext.Current);
            }
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            ProfileBase pb = null;
            try
            {
                pb = GetProfileForCurrentUser(authenticatedUserOnly);
            }
            catch (Exception exception)
            {
                this.LogException(exception);
                throw;
            }
            if (pb == null)
            {
                return null;
            }
            Dictionary<string, object> profileAllowedGet = ApplicationServiceHelper.ProfileAllowedGet;
            if ((profileAllowedGet != null) && (profileAllowedGet.Count != 0))
            {
                foreach (string str in properties)
                {
                    if (str == null)
                    {
                        throw new ArgumentNullException("properties");
                    }
                    if (profileAllowedGet.ContainsKey(str))
                    {
                        try
                        {
                            SettingsPropertyValue propertyValue = GetPropertyValue(pb, str);
                            if (propertyValue != null)
                            {
                                dictionary.Add(str, propertyValue.PropertyValue);
                                propertyValue.IsDirty = false;
                            }
                        }
                        catch (Exception exception2)
                        {
                            this.LogException(exception2);
                            throw;
                        }
                    }
                }
            }
            return dictionary;
        }

        [OperationContract]
        public ProfilePropertyMetadata[] GetPropertiesMetadata()
        {
            ProfilePropertyMetadata[] metadataArray2;
            ApplicationServiceHelper.EnsureProfileServiceEnabled();
            try
            {
                Collection<ProfilePropertyMetadata> profilePropertiesMetadata = ApplicationServiceHelper.GetProfilePropertiesMetadata();
                ProfilePropertyMetadata[] array = new ProfilePropertyMetadata[profilePropertiesMetadata.Count];
                profilePropertiesMetadata.CopyTo(array, 0);
                metadataArray2 = array;
            }
            catch (Exception exception)
            {
                this.LogException(exception);
                throw;
            }
            return metadataArray2;
        }

        private static SettingsPropertyValue GetPropertyValue(ProfileBase pb, string name)
        {
            if (ProfileBase.Properties[name] == null)
            {
                return null;
            }
            SettingsPropertyValue value2 = pb.PropertyValues[name];
            if (value2 == null)
            {
                pb.GetPropertyValue(name);
                value2 = pb.PropertyValues[name];
                if (value2 == null)
                {
                    return null;
                }
            }
            return value2;
        }

        private void LogException(Exception e)
        {
            new WebServiceErrorEvent(AtlasWeb.UnhandledExceptionEventLogMessage, this, e).Raise();
        }

        private void OnValidatingProperties(ValidatingPropertiesEventArgs e)
        {
            EventHandler<ValidatingPropertiesEventArgs> handler = _validatingProperties;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        [OperationContract]
        public Collection<string> SetPropertiesForCurrentUser(IDictionary<string, object> values, bool authenticatedUserOnly)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            ApplicationServiceHelper.EnsureProfileServiceEnabled();
            if (authenticatedUserOnly)
            {
                ApplicationServiceHelper.EnsureAuthenticated(HttpContext.Current);
            }
            Collection<string> collection = new Collection<string>();
            try
            {
                ValidatingPropertiesEventArgs e = new ValidatingPropertiesEventArgs(values);
                this.OnValidatingProperties(e);
                Dictionary<string, object> profileAllowedSet = ApplicationServiceHelper.ProfileAllowedSet;
                ProfileBase profileForCurrentUser = GetProfileForCurrentUser(authenticatedUserOnly);
                foreach (KeyValuePair<string, object> pair in values)
                {
                    string key = pair.Key;
                    if (profileForCurrentUser == null)
                    {
                        collection.Add(key);
                    }
                    else if (e.FailedProperties.Contains(key))
                    {
                        collection.Add(key);
                    }
                    else if (profileAllowedSet == null)
                    {
                        collection.Add(key);
                    }
                    else if (!profileAllowedSet.ContainsKey(key))
                    {
                        collection.Add(key);
                    }
                    else
                    {
                        SettingsProperty property = ProfileBase.Properties[key];
                        if (property == null)
                        {
                            collection.Add(key);
                        }
                        else if (property.IsReadOnly || (profileForCurrentUser.IsAnonymous && !((bool) property.Attributes["AllowAnonymous"])))
                        {
                            collection.Add(key);
                        }
                        else if (GetPropertyValue(profileForCurrentUser, pair.Key) == null)
                        {
                            collection.Add(key);
                        }
                        else
                        {
                            try
                            {
                                profileForCurrentUser[key] = pair.Value;
                            }
                            catch (ProviderException)
                            {
                                collection.Add(key);
                            }
                            catch (SettingsPropertyNotFoundException)
                            {
                                collection.Add(key);
                            }
                            catch (SettingsPropertyWrongTypeException)
                            {
                                collection.Add(key);
                            }
                        }
                    }
                }
                profileForCurrentUser.Save();
            }
            catch (Exception exception)
            {
                this.LogException(exception);
                throw;
            }
            return collection;
        }
    }
}

