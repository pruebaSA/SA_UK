namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [Serializable, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class SharedPersonalizationStateInfo : PersonalizationStateInfo
    {
        private int _countOfPersonalizations;
        private int _sizeOfPersonalizations;

        public SharedPersonalizationStateInfo(string path, DateTime lastUpdatedDate, int size, int sizeOfPersonalizations, int countOfPersonalizations) : base(path, lastUpdatedDate, size)
        {
            PersonalizationProviderHelper.CheckNegativeInteger(sizeOfPersonalizations, "sizeOfPersonalizations");
            PersonalizationProviderHelper.CheckNegativeInteger(countOfPersonalizations, "countOfPersonalizations");
            this._sizeOfPersonalizations = sizeOfPersonalizations;
            this._countOfPersonalizations = countOfPersonalizations;
        }

        public int CountOfPersonalizations =>
            this._countOfPersonalizations;

        public int SizeOfPersonalizations =>
            this._sizeOfPersonalizations;
    }
}

