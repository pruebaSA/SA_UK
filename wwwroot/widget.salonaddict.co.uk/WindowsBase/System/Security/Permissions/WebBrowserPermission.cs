namespace System.Security.Permissions
{
    using System;
    using System.Security;
    using System.Windows;

    [Serializable]
    public sealed class WebBrowserPermission : CodeAccessPermission, IUnrestrictedPermission
    {
        private WebBrowserPermissionLevel _webBrowserPermissionLevel;

        public WebBrowserPermission()
        {
            this._webBrowserPermissionLevel = WebBrowserPermissionLevel.Safe;
        }

        public WebBrowserPermission(PermissionState state)
        {
            if (state == PermissionState.Unrestricted)
            {
                this._webBrowserPermissionLevel = WebBrowserPermissionLevel.Unrestricted;
            }
            else
            {
                if (state != PermissionState.None)
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidPermissionState"));
                }
                this._webBrowserPermissionLevel = WebBrowserPermissionLevel.None;
            }
        }

        public WebBrowserPermission(WebBrowserPermissionLevel webBrowserPermissionLevel)
        {
            VerifyWebBrowserPermissionLevel(webBrowserPermissionLevel);
            this._webBrowserPermissionLevel = webBrowserPermissionLevel;
        }

        public override IPermission Copy() => 
            new WebBrowserPermission(this._webBrowserPermissionLevel);

        public override void FromXml(SecurityElement securityElement)
        {
            if (securityElement == null)
            {
                throw new ArgumentNullException("securityElement");
            }
            string str = securityElement.Attribute("class");
            if ((str == null) || (str.IndexOf(base.GetType().FullName, StringComparison.Ordinal) == -1))
            {
                throw new ArgumentNullException("securityElement");
            }
            string str2 = securityElement.Attribute("Unrestricted");
            if ((str2 != null) && bool.Parse(str2))
            {
                this._webBrowserPermissionLevel = WebBrowserPermissionLevel.Unrestricted;
            }
            else
            {
                this._webBrowserPermissionLevel = WebBrowserPermissionLevel.None;
                string str3 = securityElement.Attribute("Level");
                if (str3 == null)
                {
                    throw new ArgumentException(System.Windows.SR.Get("BadXml", new object[] { "level" }));
                }
                this._webBrowserPermissionLevel = (WebBrowserPermissionLevel) Enum.Parse(typeof(WebBrowserPermissionLevel), str3);
            }
        }

        public override IPermission Intersect(IPermission target)
        {
            if (target == null)
            {
                return null;
            }
            WebBrowserPermission permission = target as WebBrowserPermission;
            if (permission == null)
            {
                throw new ArgumentException(System.Windows.SR.Get("TargetNotWebBrowserPermissionLevel"));
            }
            WebBrowserPermissionLevel webBrowserPermissionLevel = (this._webBrowserPermissionLevel < permission._webBrowserPermissionLevel) ? this._webBrowserPermissionLevel : permission._webBrowserPermissionLevel;
            if (webBrowserPermissionLevel == WebBrowserPermissionLevel.None)
            {
                return null;
            }
            return new WebBrowserPermission(webBrowserPermissionLevel);
        }

        public override bool IsSubsetOf(IPermission target)
        {
            if (target == null)
            {
                return (this._webBrowserPermissionLevel == WebBrowserPermissionLevel.None);
            }
            WebBrowserPermission permission = target as WebBrowserPermission;
            if (permission == null)
            {
                throw new ArgumentException(System.Windows.SR.Get("TargetNotWebBrowserPermissionLevel"));
            }
            return (this._webBrowserPermissionLevel <= permission._webBrowserPermissionLevel);
        }

        public bool IsUnrestricted() => 
            (this._webBrowserPermissionLevel == WebBrowserPermissionLevel.Unrestricted);

        public override SecurityElement ToXml()
        {
            SecurityElement element = new SecurityElement("IPermission");
            element.AddAttribute("class", base.GetType().AssemblyQualifiedName);
            element.AddAttribute("version", "1");
            if (this.IsUnrestricted())
            {
                element.AddAttribute("Unrestricted", bool.TrueString);
                return element;
            }
            element.AddAttribute("Level", this._webBrowserPermissionLevel.ToString());
            return element;
        }

        public override IPermission Union(IPermission target)
        {
            if (target == null)
            {
                return this.Copy();
            }
            WebBrowserPermission permission = target as WebBrowserPermission;
            if (permission == null)
            {
                throw new ArgumentException(System.Windows.SR.Get("TargetNotWebBrowserPermissionLevel"));
            }
            WebBrowserPermissionLevel webBrowserPermissionLevel = (this._webBrowserPermissionLevel > permission._webBrowserPermissionLevel) ? this._webBrowserPermissionLevel : permission._webBrowserPermissionLevel;
            if (webBrowserPermissionLevel == WebBrowserPermissionLevel.None)
            {
                return null;
            }
            return new WebBrowserPermission(webBrowserPermissionLevel);
        }

        internal static void VerifyWebBrowserPermissionLevel(WebBrowserPermissionLevel level)
        {
            if ((level < WebBrowserPermissionLevel.None) || (level > WebBrowserPermissionLevel.Unrestricted))
            {
                throw new ArgumentException(System.Windows.SR.Get("InvalidPermissionLevel"));
            }
        }

        public WebBrowserPermissionLevel Level
        {
            get => 
                this._webBrowserPermissionLevel;
            set
            {
                VerifyWebBrowserPermissionLevel(value);
                this._webBrowserPermissionLevel = value;
            }
        }
    }
}

