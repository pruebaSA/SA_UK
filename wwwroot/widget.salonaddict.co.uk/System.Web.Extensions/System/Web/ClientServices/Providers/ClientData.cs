namespace System.Web.ClientServices.Providers
{
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Xml;

    internal class ClientData
    {
        private const string _IsolatedDir = "System.Web.Extensions.ClientServices.ClientData";
        private const int _NumStoredValues = 13;
        private static string[] _StoredValueNames = new string[] { "LastLoggedInUserName", "LastLoggedInDateUtc", "PasswordHash", "PasswordSalt", "Roles", "RolesCachedDateUtc", "SettingsNames", "SettingsStoredAs", "SettingsValues", "SettingsNeedReset", "SettingsCacheIsMoreFresh", "CookieNames", "CookieValues" };
        private object[] _StoredValues;
        private string FileName;
        private bool UsingIsolatedStorage;

        private ClientData()
        {
            this._StoredValues = new object[] { "", DateTime.UtcNow.AddYears(-1), string.Empty, string.Empty, new string[0], DateTime.UtcNow.AddYears(-1), new string[0], new string[0], new string[0], false, false, new string[0], new string[0] };
            this.FileName = string.Empty;
        }

        private ClientData(XmlReader reader)
        {
            this._StoredValues = new object[] { "", DateTime.UtcNow.AddYears(-1), string.Empty, string.Empty, new string[0], DateTime.UtcNow.AddYears(-1), new string[0], new string[0], new string[0], false, false, new string[0], new string[0] };
            this.FileName = string.Empty;
            reader.ReadStartElement("ClientData");
            for (int i = 0; i < 13; i++)
            {
                reader.ReadStartElement(_StoredValueNames[i]);
                if (this._StoredValues[i] is string)
                {
                    this._StoredValues[i] = reader.ReadContentAsString();
                }
                else if (this._StoredValues[i] is DateTime)
                {
                    long fileTime = long.Parse(reader.ReadContentAsString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    this._StoredValues[i] = DateTime.FromFileTimeUtc(fileTime);
                }
                else if (this._StoredValues[i] is bool)
                {
                    string str2 = reader.ReadContentAsString();
                    this._StoredValues[i] = string.IsNullOrEmpty(str2) ? ((object) 0) : ((object) (str2 == "1"));
                }
                else
                {
                    this._StoredValues[i] = ReadStringArray(reader);
                }
                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }

        internal static ClientData Load(string username, bool useIsolatedStorage)
        {
            ClientData data = null;
            string path = null;
            if (useIsolatedStorage)
            {
                path = @"System.Web.Extensions.ClientServices.ClientData\" + SqlHelper.GetPartialDBFileName(username, ".clientdata");
                try
                {
                    using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForAssembly())
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(path, FileMode.Open, file))
                        {
                            using (XmlReader reader = XmlReader.Create(stream))
                            {
                                data = new ClientData(reader);
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            else
            {
                path = SqlHelper.GetFullDBFileName(username, ".clientdata");
                try
                {
                    if (File.Exists(path))
                    {
                        using (FileStream stream2 = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            using (XmlReader reader2 = XmlReader.Create(stream2))
                            {
                                data = new ClientData(reader2);
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            if (data == null)
            {
                data = new ClientData();
            }
            data.UsingIsolatedStorage = useIsolatedStorage;
            data.FileName = path;
            return data;
        }

        private static string[] ReadStringArray(XmlReader reader)
        {
            StringCollection strings = new StringCollection();
            while (reader.IsStartElement())
            {
                reader.ReadStartElement("item");
                strings.Add(reader.ReadContentAsString());
                reader.ReadEndElement();
            }
            string[] array = new string[strings.Count];
            strings.CopyTo(array, 0);
            return array;
        }

        internal void Save()
        {
            if (!this.UsingIsolatedStorage)
            {
                using (XmlWriter writer = XmlWriter.Create(this.FileName))
                {
                    this.Save(writer);
                    return;
                }
            }
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (file.GetDirectoryNames("System.Web.Extensions.ClientServices.ClientData").Length == 0)
                {
                    file.CreateDirectory("System.Web.Extensions.ClientServices.ClientData");
                }
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(this.FileName, FileMode.Create, file))
                {
                    using (XmlWriter writer2 = XmlWriter.Create(stream))
                    {
                        this.Save(writer2);
                    }
                }
            }
        }

        private void Save(XmlWriter writer)
        {
            writer.WriteStartElement("ClientData");
            for (int i = 0; i < 13; i++)
            {
                writer.WriteStartElement(_StoredValueNames[i]);
                if (this._StoredValues[i] == null)
                {
                    writer.WriteValue(string.Empty);
                }
                else if (this._StoredValues[i] is string)
                {
                    writer.WriteValue(this._StoredValues[i]);
                }
                else if (this._StoredValues[i] is bool)
                {
                    writer.WriteValue(((bool) this._StoredValues[i]) ? "1" : "0");
                }
                else if (this._StoredValues[i] is DateTime)
                {
                    DateTime time = (DateTime) this._StoredValues[i];
                    writer.WriteValue(time.ToFileTimeUtc().ToString("X", CultureInfo.InvariantCulture));
                }
                else
                {
                    WriteStringArray(writer, (string[]) this._StoredValues[i]);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.Flush();
        }

        private static void WriteStringArray(XmlWriter writer, string[] arrToWrite)
        {
            if (arrToWrite.Length == 0)
            {
                writer.WriteValue(string.Empty);
            }
            for (int i = 0; i < arrToWrite.Length; i++)
            {
                writer.WriteStartElement("item");
                writer.WriteValue((arrToWrite[i] == null) ? string.Empty : arrToWrite[i]);
                writer.WriteEndElement();
            }
        }

        internal string[] CookieNames
        {
            get => 
                ((string[]) this._StoredValues[11]);
            set
            {
                this._StoredValues[11] = value;
            }
        }

        internal string[] CookieValues
        {
            get => 
                ((string[]) this._StoredValues[12]);
            set
            {
                this._StoredValues[12] = value;
            }
        }

        internal DateTime LastLoggedInDateUtc
        {
            get => 
                ((DateTime) this._StoredValues[1]);
            set
            {
                this._StoredValues[1] = value;
            }
        }

        internal string LastLoggedInUserName
        {
            get => 
                ((string) this._StoredValues[0]);
            set
            {
                this._StoredValues[0] = value;
            }
        }

        internal string PasswordHash
        {
            get => 
                ((string) this._StoredValues[2]);
            set
            {
                this._StoredValues[2] = value;
            }
        }

        internal string PasswordSalt
        {
            get => 
                ((string) this._StoredValues[3]);
            set
            {
                this._StoredValues[3] = value;
            }
        }

        internal string[] Roles
        {
            get => 
                ((string[]) this._StoredValues[4]);
            set
            {
                this._StoredValues[4] = value;
            }
        }

        internal DateTime RolesCachedDateUtc
        {
            get => 
                ((DateTime) this._StoredValues[5]);
            set
            {
                this._StoredValues[5] = value;
            }
        }

        internal bool SettingsCacheIsMoreFresh
        {
            get => 
                ((bool) this._StoredValues[10]);
            set
            {
                this._StoredValues[10] = value;
            }
        }

        internal string[] SettingsNames
        {
            get => 
                ((string[]) this._StoredValues[6]);
            set
            {
                this._StoredValues[6] = value;
            }
        }

        internal bool SettingsNeedReset
        {
            get => 
                ((bool) this._StoredValues[9]);
            set
            {
                this._StoredValues[9] = value;
            }
        }

        internal string[] SettingsStoredAs
        {
            get => 
                ((string[]) this._StoredValues[7]);
            set
            {
                this._StoredValues[7] = value;
            }
        }

        internal string[] SettingsValues
        {
            get => 
                ((string[]) this._StoredValues[8]);
            set
            {
                this._StoredValues[8] = value;
            }
        }

        internal enum ClientDateStoreOrderEnum
        {
            LastLoggedInUserName,
            LastLoggedInDateUtc,
            PasswordHash,
            PasswordSalt,
            Roles,
            RolesCachedDateUtc,
            SettingsNames,
            SettingsStoredAs,
            SettingsValues,
            SettingsNeedReset,
            SettingsCacheIsMoreFresh,
            CookieNames,
            CookieValues
        }
    }
}

