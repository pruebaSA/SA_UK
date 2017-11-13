namespace MS.Internal.IO.Packaging.CompoundFile
{
    using MS.Internal;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Packaging;
    using System.Security.RightsManagement;
    using System.Text;

    internal class UserUseLicenseDictionaryLoader
    {
        private Dictionary<ContentUser, UseLicense> _dict = new Dictionary<ContentUser, UseLicense>(ContentUser._contentUserComparer);
        private UTF8Encoding _utf8Encoding = new UTF8Encoding();

        internal UserUseLicenseDictionaryLoader(RightsManagementEncryptionTransform rmet)
        {
            Invariant.Assert(rmet != null);
            this.Load(rmet);
        }

        private void AddUseLicenseFromStreamToDictionary(RightsManagementEncryptionTransform rmet, StreamInfo si, object param, ref bool stop)
        {
            using (Stream stream = si.GetStream(FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream, this._utf8Encoding))
                {
                    ContentUser user;
                    UseLicense license = rmet.LoadUseLicenseAndUserFromStream(reader, out user);
                    this._dict.Add(user, license);
                }
            }
        }

        private void Load(RightsManagementEncryptionTransform rmet)
        {
            rmet.EnumUseLicenseStreams(new RightsManagementEncryptionTransform.UseLicenseStreamCallback(this.AddUseLicenseFromStreamToDictionary), null);
        }

        internal Dictionary<ContentUser, UseLicense> LoadedDictionary =>
            this._dict;
    }
}

