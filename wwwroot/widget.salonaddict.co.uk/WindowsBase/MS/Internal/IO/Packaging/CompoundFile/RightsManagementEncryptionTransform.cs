namespace MS.Internal.IO.Packaging.CompoundFile
{
    using MS.Internal.IO.Packaging;
    using MS.Internal.Utility;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Packaging;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.RightsManagement;
    using System.Text;
    using System.Windows;

    internal class RightsManagementEncryptionTransform : IDataTransform
    {
        private System.Security.RightsManagement.CryptoProvider _cryptoProvider;
        private bool _fixedSettings;
        private PublishLicense _publishLicense;
        private byte[] _publishLicenseHeaderExtraBytes;
        private VersionedStreamOwner _publishLicenseStream;
        private static readonly UnicodeEncoding _unicodeEncoding = new UnicodeEncoding(false, false);
        private StorageInfo _useLicenseStorage;
        private static readonly char[] Base32EncodingTable = new char[] { 
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '2', '3', '4', '5', '6', '7',
            '='
        };
        private static readonly VersionPair CurrentFeatureVersion = new VersionPair(1, 0);
        private const string FeatureName = "Microsoft.Metadata.DRMTransform";
        private const string LicenseStreamNamePrefix = "EUL-";
        private static readonly int LicenseStreamNamePrefixLength = "EUL-".Length;
        private const int MaxPublishLicenseHeaderLen = 0x1000;
        private static readonly VersionPair MinimumReaderVersion = new VersionPair(1, 0);
        private static readonly VersionPair MinimumUpdaterVersion = new VersionPair(1, 0);
        private static readonly byte[] Padding = new byte[3];
        private const int PublishLicenseLengthMax = 0xf4240;
        private const int SizeofByte = 1;
        private const int UseLicenseLengthMax = 0xf4240;
        private static readonly int UseLicenseStreamLengthMin = ((2 * ContainerUtilities.Int32Size) + 1);
        private const int UserNameLengthMax = 0x3e8;

        internal RightsManagementEncryptionTransform(TransformEnvironment transformEnvironment)
        {
            Stream primaryInstanceData = transformEnvironment.GetPrimaryInstanceData();
            this._useLicenseStorage = transformEnvironment.GetInstanceDataStorage();
            this._publishLicenseStream = new VersionedStreamOwner(primaryInstanceData, new FormatVersion("Microsoft.Metadata.DRMTransform", MinimumReaderVersion, MinimumUpdaterVersion, CurrentFeatureVersion));
        }

        private static char[] Base32EncodeWithoutPadding(byte[] bytes)
        {
            int length = bytes.Length;
            int num2 = length * 8;
            int num3 = num2 / 5;
            if ((num2 % 5) != 0)
            {
                num3++;
            }
            char[] chArray = new char[num3];
            for (int i = 0; i < num3; i++)
            {
                int num5 = i * 5;
                int index = 0;
                for (int j = num5; ((j - num5) < 5) && (j < num2); j++)
                {
                    int num8 = j / 8;
                    int num9 = j % 8;
                    if ((bytes[num8] & (((int) 1) << num9)) != 0)
                    {
                        index += ((int) 1) << (j - num5);
                    }
                }
                chArray[i] = Base32EncodingTable[index];
            }
            return chArray;
        }

        internal void DeleteUseLicense(ContentUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            this.EnumUseLicenseStreams(new UseLicenseStreamCallback(this.DeleteUseLicenseForUser), user);
        }

        private void DeleteUseLicenseForUser(RightsManagementEncryptionTransform rmet, StreamInfo si, object param, ref bool stop)
        {
            ContentUser userObj = param as ContentUser;
            if (userObj == null)
            {
                throw new ArgumentException(System.Windows.SR.Get("CallbackParameterInvalid"), "param");
            }
            ContentUser user2 = null;
            using (Stream stream = si.GetStream(FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
                {
                    user2 = rmet.LoadUserFromStream(reader);
                }
            }
            if (user2.GenericEquals(userObj))
            {
                si.Delete();
            }
        }

        internal void EnumUseLicenseStreams(UseLicenseStreamCallback callback, object param)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            bool stop = false;
            foreach (StreamInfo info in this._useLicenseStorage.GetStreams())
            {
                if (string.CompareOrdinal("EUL-".ToUpperInvariant(), 0, info.Name.ToUpperInvariant(), 0, LicenseStreamNamePrefixLength) == 0)
                {
                    callback(this, info, param, ref stop);
                    if (stop)
                    {
                        return;
                    }
                }
            }
        }

        internal IDictionary<ContentUser, UseLicense> GetEmbeddedUseLicenses()
        {
            UserUseLicenseDictionaryLoader loader = new UserUseLicenseDictionaryLoader(this);
            return new ReadOnlyDictionary<ContentUser, UseLicense>(loader.LoadedDictionary);
        }

        internal PublishLicense LoadPublishLicense()
        {
            if (this._publishLicenseStream.Length <= 0L)
            {
                return null;
            }
            this._publishLicenseStream.Seek(0L, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(this._publishLicenseStream, Encoding.UTF8);
            int num = reader.ReadInt32();
            if (num < ContainerUtilities.Int32Size)
            {
                throw new FileFormatException(System.Windows.SR.Get("PublishLicenseStreamCorrupt"));
            }
            if (num > 0x1000)
            {
                throw new FileFormatException(System.Windows.SR.Get("PublishLicenseStreamHeaderTooLong", new object[] { num, 0x1000 }));
            }
            int count = num - ContainerUtilities.Int32Size;
            if (count > 0)
            {
                this._publishLicenseHeaderExtraBytes = new byte[count];
                if (PackagingUtilities.ReliableRead(this._publishLicenseStream, this._publishLicenseHeaderExtraBytes, 0, count) != count)
                {
                    throw new FileFormatException(System.Windows.SR.Get("PublishLicenseStreamCorrupt"));
                }
            }
            this._publishLicense = new PublishLicense(ReadLengthPrefixedString(reader, Encoding.UTF8, 0xf4240));
            return this._publishLicense;
        }

        internal UseLicense LoadUseLicense(ContentUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            LoadUseLicenseForUserParams param = new LoadUseLicenseForUserParams(user);
            this.EnumUseLicenseStreams(new UseLicenseStreamCallback(this.LoadUseLicenseForUser), param);
            return param.UseLicense;
        }

        internal UseLicense LoadUseLicenseAndUserFromStream(BinaryReader utf8Reader, out ContentUser user)
        {
            AuthenticationType type;
            string str3;
            utf8Reader.BaseStream.Seek(0L, SeekOrigin.Begin);
            if (utf8Reader.ReadInt32() < UseLicenseStreamLengthMin)
            {
                throw new FileFormatException(System.Windows.SR.Get("UseLicenseStreamCorrupt"));
            }
            byte[] bytes = Convert.FromBase64String(ReadLengthPrefixedString(utf8Reader, Encoding.UTF8, 0x3e8));
            string typePrefixedUserName = new string(_unicodeEncoding.GetChars(bytes));
            ParseTypePrefixedUserName(typePrefixedUserName, out type, out str3);
            user = new ContentUser(str3, type);
            return new UseLicense(ReadLengthPrefixedString(utf8Reader, Encoding.UTF8, 0xf4240));
        }

        private void LoadUseLicenseForUser(RightsManagementEncryptionTransform rmet, StreamInfo si, object param, ref bool stop)
        {
            LoadUseLicenseForUserParams @params = param as LoadUseLicenseForUserParams;
            if (@params == null)
            {
                throw new ArgumentException(System.Windows.SR.Get("CallbackParameterInvalid"), "param");
            }
            ContentUser userObj = @params.User;
            using (Stream stream = si.GetStream(FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
                {
                    if (rmet.LoadUserFromStream(reader).GenericEquals(userObj))
                    {
                        @params.UseLicense = rmet.LoadUseLicenseFromStream(reader);
                        stop = true;
                    }
                }
            }
        }

        private UseLicense LoadUseLicenseFromStream(BinaryReader utf8Reader)
        {
            utf8Reader.BaseStream.Seek(0L, SeekOrigin.Begin);
            if (utf8Reader.ReadInt32() < UseLicenseStreamLengthMin)
            {
                throw new FileFormatException(System.Windows.SR.Get("UseLicenseStreamCorrupt"));
            }
            ReadLengthPrefixedString(utf8Reader, Encoding.UTF8, 0x3e8);
            return new UseLicense(ReadLengthPrefixedString(utf8Reader, Encoding.UTF8, 0xf4240));
        }

        private ContentUser LoadUserFromStream(BinaryReader utf8Reader)
        {
            AuthenticationType type;
            string str3;
            utf8Reader.BaseStream.Seek(0L, SeekOrigin.Begin);
            if (utf8Reader.ReadInt32() < UseLicenseStreamLengthMin)
            {
                throw new FileFormatException(System.Windows.SR.Get("UseLicenseStreamCorrupt"));
            }
            byte[] bytes = Convert.FromBase64String(ReadLengthPrefixedString(utf8Reader, Encoding.UTF8, 0x3e8));
            string typePrefixedUserName = new string(_unicodeEncoding.GetChars(bytes));
            ParseTypePrefixedUserName(typePrefixedUserName, out type, out str3);
            return new ContentUser(str3, type);
        }

        private static string MakeTypePrefixedUserName(ContentUser user)
        {
            StringBuilder builder = new StringBuilder(9 + user.Name.Length);
            builder.Append(user.AuthenticationType.ToString());
            builder.Append(':');
            builder.Append(user.Name);
            return builder.ToString();
        }

        private static string MakeUseLicenseStreamName() => 
            ("EUL-" + new string(Base32EncodeWithoutPadding(Guid.NewGuid().ToByteArray())));

        private static void ParseTypePrefixedUserName(string typePrefixedUserName, out AuthenticationType authenticationType, out string userName)
        {
            authenticationType = AuthenticationType.Windows;
            int index = typePrefixedUserName.IndexOf(':');
            if ((index < 1) || (index >= (typePrefixedUserName.Length - 1)))
            {
                throw new FileFormatException(System.Windows.SR.Get("InvalidTypePrefixedUserName"));
            }
            userName = typePrefixedUserName.Substring(index + 1);
            string x = typePrefixedUserName.Substring(0, index);
            bool flag = false;
            if (((IEqualityComparer) ContainerUtilities.StringCaseInsensitiveComparer).Equals(x, Enum.GetName(typeof(AuthenticationType), AuthenticationType.Windows)))
            {
                authenticationType = AuthenticationType.Windows;
                flag = true;
            }
            else if (((IEqualityComparer) ContainerUtilities.StringCaseInsensitiveComparer).Equals(x, Enum.GetName(typeof(AuthenticationType), AuthenticationType.Passport)))
            {
                authenticationType = AuthenticationType.Passport;
                flag = true;
            }
            if (!flag)
            {
                throw new FileFormatException(System.Windows.SR.Get("InvalidAuthenticationTypeString", new object[] { typePrefixedUserName }));
            }
        }

        private static string ReadLengthPrefixedString(BinaryReader reader, Encoding encoding, int maxLength)
        {
            int count = reader.ReadInt32();
            if (count > maxLength)
            {
                throw new FileFormatException(System.Windows.SR.Get("ExcessiveLengthPrefix", new object[] { count, maxLength }));
            }
            byte[] bytes = reader.ReadBytes(count);
            if (bytes.Length != count)
            {
                throw new FileFormatException(System.Windows.SR.Get("InvalidStringFormat"));
            }
            string str = encoding.GetString(bytes);
            SkipDwordPadding(bytes.Length, reader);
            return str;
        }

        internal void SavePublishLicense(PublishLicense publishLicense)
        {
            if (publishLicense == null)
            {
                throw new ArgumentNullException("publishLicense");
            }
            if (this._fixedSettings)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("CannotChangePublishLicense"));
            }
            this._publishLicenseStream.Seek(0L, SeekOrigin.Begin);
            BinaryWriter writer = new BinaryWriter(this._publishLicenseStream, Encoding.UTF8);
            int num = ContainerUtilities.Int32Size;
            if (this._publishLicenseHeaderExtraBytes != null)
            {
                num += this._publishLicenseHeaderExtraBytes.Length;
            }
            writer.Write(num);
            if (this._publishLicenseHeaderExtraBytes != null)
            {
                this._publishLicenseStream.Write(this._publishLicenseHeaderExtraBytes, 0, this._publishLicenseHeaderExtraBytes.Length);
            }
            WriteByteLengthPrefixedDwordPaddedString(publishLicense.ToString(), writer, Encoding.UTF8);
            writer.Flush();
            this._publishLicense = publishLicense;
        }

        internal void SaveUseLicense(ContentUser user, UseLicense useLicense)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (useLicense == null)
            {
                throw new ArgumentNullException("useLicense");
            }
            if ((user.AuthenticationType != AuthenticationType.Windows) && (user.AuthenticationType != AuthenticationType.Passport))
            {
                throw new ArgumentException(System.Windows.SR.Get("OnlyPassportOrWindowsAuthenticatedUsersAreAllowed"), "user");
            }
            this.EnumUseLicenseStreams(new UseLicenseStreamCallback(this.DeleteUseLicenseForUser), user);
            this.SaveUseLicenseForUser(user, useLicense);
        }

        internal void SaveUseLicenseForUser(ContentUser user, UseLicense useLicense)
        {
            string streamName = MakeUseLicenseStreamName();
            StreamInfo info = new StreamInfo(this._useLicenseStorage, streamName);
            using (Stream stream = info.Create())
            {
                using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8))
                {
                    string s = MakeTypePrefixedUserName(user);
                    string str3 = Convert.ToBase64String(_unicodeEncoding.GetBytes(s));
                    byte[] bytes = Encoding.UTF8.GetBytes(str3);
                    int length = bytes.Length;
                    int num2 = ((2 * ContainerUtilities.Int32Size) + length) + ContainerUtilities.CalculateDWordPadBytesLength(length);
                    writer.Write(num2);
                    writer.Write(length);
                    writer.Write(bytes, 0, length);
                    WriteDwordPadding(length, writer);
                    WriteByteLengthPrefixedDwordPaddedString(useLicense.ToString(), writer, Encoding.UTF8);
                }
            }
        }

        private static void SkipDwordPadding(int length, BinaryReader reader)
        {
            int num = length % ContainerUtilities.Int32Size;
            if ((num != 0) && (reader.ReadBytes(ContainerUtilities.Int32Size - num).Length != (ContainerUtilities.Int32Size - num)))
            {
                throw new FileFormatException(System.Windows.SR.Get("InvalidStringFormat"));
            }
        }

        Stream IDataTransform.GetTransformedStream(Stream encodedStream, IDictionary transformContext)
        {
            this._fixedSettings = true;
            return new VersionedStream(new RightsManagementEncryptedStream(encodedStream, this._cryptoProvider), this._publishLicenseStream);
        }

        private static void WriteByteLengthPrefixedDwordPaddedString(string s, BinaryWriter writer, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(s);
            int length = bytes.Length;
            writer.Write(length);
            writer.Write(bytes);
            WriteDwordPadding(length, writer);
        }

        private static void WriteDwordPadding(int length, BinaryWriter writer)
        {
            int num = length % ContainerUtilities.Int32Size;
            if (num != 0)
            {
                writer.Write(Padding, 0, ContainerUtilities.Int32Size - num);
            }
        }

        internal static string ClassTransformIdentifier =>
            "{C73DFACD-061F-43B0-8B64-0C620D2A8B50}";

        internal System.Security.RightsManagement.CryptoProvider CryptoProvider
        {
            get => 
                this._cryptoProvider;
            set
            {
                if (this._fixedSettings)
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("CannotChangeCryptoProvider"));
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (!value.CanEncrypt && !value.CanDecrypt)
                {
                    throw new ArgumentException(System.Windows.SR.Get("CryptoProviderIsNotReady"), "value");
                }
                this._cryptoProvider = value;
            }
        }

        public bool FixedSettings =>
            this._fixedSettings;

        public bool IsReady
        {
            get
            {
                if (this._cryptoProvider == null)
                {
                    return false;
                }
                if (!this._cryptoProvider.CanDecrypt)
                {
                    return this._cryptoProvider.CanEncrypt;
                }
                return true;
            }
        }

        public object TransformIdentifier =>
            ClassTransformIdentifier;

        internal int TransformIdentifierType =>
            1;

        private class LoadUseLicenseForUserParams
        {
            private System.Security.RightsManagement.UseLicense _useLicense;
            private ContentUser _user;

            internal LoadUseLicenseForUserParams(ContentUser user)
            {
                this._user = user;
                this._useLicense = null;
            }

            internal System.Security.RightsManagement.UseLicense UseLicense
            {
                get => 
                    this._useLicense;
                set
                {
                    this._useLicense = value;
                }
            }

            internal ContentUser User =>
                this._user;
        }

        internal delegate void UseLicenseStreamCallback(RightsManagementEncryptionTransform rmet, StreamInfo si, object param, ref bool stop);
    }
}

