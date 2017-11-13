namespace System.Configuration
{
    using System;
    using System.Threading;

    internal sealed class UriSectionInternal
    {
        private static object classSyncObject;
        private UriIdnScope idn;
        private bool iriParsing;

        internal UriSectionInternal(UriSection section)
        {
            this.idn = section.Idn.Enabled;
            this.iriParsing = section.IriParsing.Enabled;
        }

        internal static UriSectionInternal GetSection()
        {
            lock (ClassSyncObject)
            {
                UriSection section = System.Configuration.PrivilegedConfigurationManager.GetSection(CommonConfigurationStrings.UriSectionPath) as UriSection;
                if (section == null)
                {
                    return null;
                }
                return new UriSectionInternal(section);
            }
        }

        internal static object ClassSyncObject
        {
            get
            {
                if (classSyncObject == null)
                {
                    Interlocked.CompareExchange(ref classSyncObject, new object(), null);
                }
                return classSyncObject;
            }
        }

        internal UriIdnScope Idn =>
            this.idn;

        internal bool IriParsing =>
            this.iriParsing;
    }
}

