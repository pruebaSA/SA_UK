namespace System.Web.Hosting
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class AppDomainInfoEnum : IAppDomainInfoEnum
    {
        private AppDomainInfo[] _appDomainInfos;
        private int _curPos;

        internal AppDomainInfoEnum(AppDomainInfo[] appDomainInfos)
        {
            this._appDomainInfos = appDomainInfos;
            this._curPos = -1;
        }

        public int Count() => 
            this._appDomainInfos.Length;

        public IAppDomainInfo GetData() => 
            this._appDomainInfos[this._curPos];

        public bool MoveNext()
        {
            this._curPos++;
            if (this._curPos >= this._appDomainInfos.Length)
            {
                return false;
            }
            return true;
        }

        public void Reset()
        {
            this._curPos = -1;
        }
    }
}

