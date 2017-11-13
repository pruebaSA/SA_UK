namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class StaticPartialCachingControl : BasePartialCachingControl
    {
        private BuildMethod _buildMethod;

        public StaticPartialCachingControl(string ctrlID, string guid, int duration, string varyByParams, string varyByControls, string varyByCustom, BuildMethod buildMethod) : this(ctrlID, guid, duration, varyByParams, varyByControls, varyByCustom, null, buildMethod)
        {
        }

        public StaticPartialCachingControl(string ctrlID, string guid, int duration, string varyByParams, string varyByControls, string varyByCustom, string sqlDependency, BuildMethod buildMethod)
        {
            base._ctrlID = ctrlID;
            base.Duration = new TimeSpan(0, 0, duration);
            base.SetVaryByParamsCollectionFromString(varyByParams);
            if (varyByControls != null)
            {
                base._varyByControlsCollection = varyByControls.Split(new char[] { ';' });
            }
            base._varyByCustom = varyByCustom;
            base._guid = guid;
            this._buildMethod = buildMethod;
            base._sqlDependency = sqlDependency;
        }

        public static void BuildCachedControl(Control parent, string ctrlID, string guid, int duration, string varyByParams, string varyByControls, string varyByCustom, BuildMethod buildMethod)
        {
            BuildCachedControl(parent, ctrlID, guid, duration, varyByParams, varyByControls, varyByCustom, null, buildMethod);
        }

        public static void BuildCachedControl(Control parent, string ctrlID, string guid, int duration, string varyByParams, string varyByControls, string varyByCustom, string sqlDependency, BuildMethod buildMethod)
        {
            StaticPartialCachingControl control = new StaticPartialCachingControl(ctrlID, guid, duration, varyByParams, varyByControls, varyByCustom, sqlDependency, buildMethod);
            ((IParserAccessor) parent).AddParsedSubObject(control);
        }

        internal override Control CreateCachedControl() => 
            this._buildMethod();
    }
}

