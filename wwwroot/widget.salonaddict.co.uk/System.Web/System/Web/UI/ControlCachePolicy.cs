﻿namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ControlCachePolicy
    {
        private static ControlCachePolicy _cachePolicyStub = new ControlCachePolicy();
        private BasePartialCachingControl _pcc;

        internal ControlCachePolicy()
        {
        }

        internal ControlCachePolicy(BasePartialCachingControl pcc)
        {
            this._pcc = pcc;
        }

        private void CheckValidCallingContext()
        {
            if (this._pcc == null)
            {
                throw new HttpException(System.Web.SR.GetString("UC_not_cached"));
            }
            if (this._pcc.ControlState >= ControlState.PreRendered)
            {
                throw new HttpException(System.Web.SR.GetString("UCCachePolicy_unavailable"));
            }
        }

        internal static ControlCachePolicy GetCachePolicyStub() => 
            _cachePolicyStub;

        public void SetExpires(DateTime expirationTime)
        {
            this.CheckValidCallingContext();
            this._pcc._utcExpirationTime = DateTimeUtil.ConvertToUniversalTime(expirationTime);
        }

        public void SetSlidingExpiration(bool useSlidingExpiration)
        {
            this.CheckValidCallingContext();
            this._pcc._useSlidingExpiration = useSlidingExpiration;
        }

        public void SetVaryByCustom(string varyByCustom)
        {
            this.CheckValidCallingContext();
            this._pcc._varyByCustom = varyByCustom;
        }

        public bool Cached
        {
            get
            {
                this.CheckValidCallingContext();
                return !this._pcc._cachingDisabled;
            }
            set
            {
                this.CheckValidCallingContext();
                this._pcc._cachingDisabled = !value;
            }
        }

        public CacheDependency Dependency
        {
            get
            {
                this.CheckValidCallingContext();
                return this._pcc.Dependency;
            }
            set
            {
                this.CheckValidCallingContext();
                this._pcc.Dependency = value;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                this.CheckValidCallingContext();
                return this._pcc.Duration;
            }
            set
            {
                this.CheckValidCallingContext();
                this._pcc.Duration = value;
            }
        }

        public bool SupportsCaching =>
            (this._pcc != null);

        public string VaryByControl
        {
            get
            {
                this.CheckValidCallingContext();
                return this._pcc.VaryByControl;
            }
            set
            {
                this.CheckValidCallingContext();
                this._pcc.VaryByControl = value;
            }
        }

        public HttpCacheVaryByParams VaryByParams
        {
            get
            {
                this.CheckValidCallingContext();
                return this._pcc.VaryByParams;
            }
        }
    }
}

