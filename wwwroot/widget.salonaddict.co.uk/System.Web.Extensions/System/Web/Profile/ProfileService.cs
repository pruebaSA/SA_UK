namespace System.Web.Profile
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Web;
    using System.Web.ApplicationServices;
    using System.Web.Script.Serialization;
    using System.Web.Script.Services;
    using System.Web.Services;

    [ScriptService]
    internal sealed class ProfileService
    {
        private static System.Web.Script.Serialization.JavaScriptSerializer _javaScriptSerializer;

        [WebMethod]
        public Dictionary<string, object> GetAllPropertiesForCurrentUser(bool authenticatedUserOnly)
        {
            ApplicationServiceHelper.EnsureProfileServiceEnabled();
            HttpContext current = HttpContext.Current;
            if (authenticatedUserOnly)
            {
                ApplicationServiceHelper.EnsureAuthenticated(current);
            }
            return GetProfile(current, null);
        }

        private static Dictionary<string, object> GetProfile(HttpContext context, IEnumerable<string> properties)
        {
            ProfileBase profile = context.Profile;
            if (profile == null)
            {
                return null;
            }
            Dictionary<string, object> profileAllowedGet = ApplicationServiceHelper.ProfileAllowedGet;
            if ((profileAllowedGet == null) || (profileAllowedGet.Count == 0))
            {
                return new Dictionary<string, object>(0);
            }
            Dictionary<string, object> dictionary2 = null;
            if (properties == null)
            {
                dictionary2 = new Dictionary<string, object>(profileAllowedGet.Count, StringComparer.OrdinalIgnoreCase);
                foreach (KeyValuePair<string, object> pair in profileAllowedGet)
                {
                    string key = pair.Key;
                    dictionary2.Add(key, profile[key]);
                }
                return dictionary2;
            }
            dictionary2 = new Dictionary<string, object>(profileAllowedGet.Count, StringComparer.OrdinalIgnoreCase);
            foreach (string str2 in properties)
            {
                if (profileAllowedGet.ContainsKey(str2))
                {
                    dictionary2.Add(str2, profile[str2]);
                }
            }
            return dictionary2;
        }

        [WebMethod]
        public Dictionary<string, object> GetPropertiesForCurrentUser(IEnumerable<string> properties, bool authenticatedUserOnly)
        {
            ApplicationServiceHelper.EnsureProfileServiceEnabled();
            HttpContext current = HttpContext.Current;
            if (authenticatedUserOnly)
            {
                ApplicationServiceHelper.EnsureAuthenticated(current);
            }
            return GetProfile(current, properties);
        }

        [WebMethod]
        public Collection<ProfilePropertyMetadata> GetPropertiesMetadata()
        {
            ApplicationServiceHelper.EnsureProfileServiceEnabled();
            return ApplicationServiceHelper.GetProfilePropertiesMetadata();
        }

        private static Collection<string> SetProfile(HttpContext context, IDictionary<string, object> values)
        {
            Collection<string> collection = new Collection<string>();
            if ((values != null) && (values.Count != 0))
            {
                ProfileBase profile = context.Profile;
                Dictionary<string, object> profileAllowedSet = ApplicationServiceHelper.ProfileAllowedSet;
                bool flag = false;
                foreach (KeyValuePair<string, object> pair in values)
                {
                    string key = pair.Key;
                    if (((profile != null) && (profileAllowedSet != null)) && profileAllowedSet.ContainsKey(key))
                    {
                        SettingsProperty property = ProfileBase.Properties[key];
                        if (((property != null) && !property.IsReadOnly) && (!profile.IsAnonymous || ((bool) property.Attributes["AllowAnonymous"])))
                        {
                            object obj2;
                            Type propertyType = property.PropertyType;
                            if (ObjectConverter.TryConvertObjectToType(pair.Value, propertyType, JavaScriptSerializer, out obj2))
                            {
                                profile[key] = obj2;
                                flag = true;
                                continue;
                            }
                        }
                    }
                    collection.Add(key);
                }
                if (flag)
                {
                    profile.Save();
                }
            }
            return collection;
        }

        [WebMethod]
        public Collection<string> SetPropertiesForCurrentUser(IDictionary<string, object> values, bool authenticatedUserOnly)
        {
            ApplicationServiceHelper.EnsureProfileServiceEnabled();
            HttpContext current = HttpContext.Current;
            if (authenticatedUserOnly)
            {
                ApplicationServiceHelper.EnsureAuthenticated(current);
            }
            return SetProfile(current, values);
        }

        private static System.Web.Script.Serialization.JavaScriptSerializer JavaScriptSerializer
        {
            get
            {
                if (_javaScriptSerializer == null)
                {
                    HttpContext current = HttpContext.Current;
                    _javaScriptSerializer = WebServiceData.GetWebServiceData(current, current.Request.FilePath).Serializer;
                }
                return _javaScriptSerializer;
            }
        }
    }
}

