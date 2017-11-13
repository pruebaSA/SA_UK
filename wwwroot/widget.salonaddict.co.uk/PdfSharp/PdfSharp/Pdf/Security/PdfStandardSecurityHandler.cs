namespace PdfSharp.Pdf.Security
{
    using PdfSharp;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Advanced;
    using PdfSharp.Pdf.Internal;
    using PdfSharp.Pdf.IO;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Security.Cryptography;

    public sealed class PdfStandardSecurityHandler : PdfSecurityHandler
    {
        private byte[] encryptionKey;
        private byte[] key;
        private int keySize;
        private MD5 md5;
        private byte[] ownerKey;
        internal string ownerPassword;
        private static byte[] passwordPadding = new byte[] { 
            40, 0xbf, 0x4e, 0x5e, 0x4e, 0x75, 0x8a, 0x41, 100, 0, 0x4e, 0x56, 0xff, 250, 1, 8,
            0x2e, 0x2e, 0, 0xb6, 0xd0, 0x68, 0x3e, 0x80, 0x2f, 12, 0xa9, 0xfe, 100, 0x53, 0x69, 0x7a
        };
        private byte[] state;
        private byte[] userKey;
        internal string userPassword;

        internal PdfStandardSecurityHandler(PdfDictionary dict) : base(dict)
        {
            this.md5 = new MD5CryptoServiceProvider();
            this.state = new byte[0x100];
            this.ownerKey = new byte[0x20];
            this.userKey = new byte[0x20];
        }

        internal PdfStandardSecurityHandler(PdfDocument document) : base(document)
        {
            this.md5 = new MD5CryptoServiceProvider();
            this.state = new byte[0x100];
            this.ownerKey = new byte[0x20];
            this.userKey = new byte[0x20];
        }

        private byte[] ComputeOwnerKey(byte[] userPad, byte[] ownerPad, bool strongEncryption)
        {
            byte[] destinationArray = new byte[0x20];
            byte[] buffer = this.md5.ComputeHash(ownerPad);
            if (strongEncryption)
            {
                byte[] key = new byte[0x10];
                for (int i = 0; i < 50; i++)
                {
                    buffer = this.md5.ComputeHash(buffer);
                }
                Array.Copy(userPad, 0, destinationArray, 0, 0x20);
                for (int j = 0; j < 20; j++)
                {
                    for (int k = 0; k < key.Length; k++)
                    {
                        key[k] = (byte) (buffer[k] ^ j);
                    }
                    this.PrepareRC4Key(key);
                    this.EncryptRC4(destinationArray);
                }
                return destinationArray;
            }
            this.PrepareRC4Key(buffer, 0, 5);
            this.EncryptRC4(userPad, destinationArray);
            return destinationArray;
        }

        [Conditional("DEBUG")]
        private static void DumpBytes(string tag, byte[] bytes)
        {
            string str = tag + ": ";
            for (int i = 0; i < bytes.Length; i++)
            {
                str = str + $"{bytes[i]:X2}";
            }
        }

        private void EncryptArray(PdfArray array)
        {
            int count = array.Elements.Count;
            for (int i = 0; i < count; i++)
            {
                PdfItem item = array.Elements[i];
                PdfString str = item as PdfString;
                if (str != null)
                {
                    this.EncryptString(str);
                }
                else
                {
                    PdfDictionary dict = item as PdfDictionary;
                    if (dict != null)
                    {
                        this.EncryptDictionary(dict);
                    }
                    else
                    {
                        PdfArray array2 = item as PdfArray;
                        if (array2 != null)
                        {
                            this.EncryptArray(array2);
                        }
                    }
                }
            }
        }

        internal byte[] EncryptBytes(byte[] bytes)
        {
            if ((bytes != null) && (bytes.Length != 0))
            {
                this.PrepareKey();
                this.EncryptRC4(bytes);
            }
            return bytes;
        }

        private void EncryptDictionary(PdfDictionary dict)
        {
            PdfName[] keyNames = dict.Elements.KeyNames;
            foreach (KeyValuePair<string, PdfItem> pair in dict.Elements)
            {
                PdfString str = pair.Value as PdfString;
                if (str != null)
                {
                    this.EncryptString(str);
                }
                else
                {
                    PdfDictionary dictionary = pair.Value as PdfDictionary;
                    if (dictionary != null)
                    {
                        this.EncryptDictionary(dictionary);
                    }
                    else
                    {
                        PdfArray array = pair.Value as PdfArray;
                        if (array != null)
                        {
                            this.EncryptArray(array);
                        }
                    }
                }
            }
            if (dict.Stream != null)
            {
                byte[] data = dict.Stream.Value;
                if (data.Length != 0)
                {
                    this.PrepareKey();
                    this.EncryptRC4(data);
                    dict.Stream.Value = data;
                }
            }
        }

        public void EncryptDocument()
        {
            foreach (PdfReference reference in base.document.irefTable.AllReferences)
            {
                if (!object.ReferenceEquals(reference.Value, this))
                {
                    this.EncryptObject(reference.Value);
                }
            }
        }

        internal void EncryptObject(PdfObject value)
        {
            this.SetHashKey(value.ObjectID);
            PdfDictionary dict = value as PdfDictionary;
            if (dict != null)
            {
                this.EncryptDictionary(dict);
            }
            else
            {
                PdfArray array = value as PdfArray;
                if (array != null)
                {
                    this.EncryptArray(array);
                }
                else
                {
                    PdfStringObject obj2;
                    if (((obj2 = value as PdfStringObject) != null) && (obj2.Length != 0))
                    {
                        byte[] encryptionValue = obj2.EncryptionValue;
                        this.PrepareKey();
                        this.EncryptRC4(encryptionValue);
                        obj2.EncryptionValue = encryptionValue;
                    }
                }
            }
        }

        private void EncryptRC4(byte[] data)
        {
            this.EncryptRC4(data, 0, data.Length, data);
        }

        private void EncryptRC4(byte[] inputData, byte[] outputData)
        {
            this.EncryptRC4(inputData, 0, inputData.Length, outputData);
        }

        private void EncryptRC4(byte[] data, int offset, int length)
        {
            this.EncryptRC4(data, offset, length, data);
        }

        private void EncryptRC4(byte[] inputData, int offset, int length, byte[] outputData)
        {
            length += offset;
            int index = 0;
            int num2 = 0;
            for (int i = offset; i < length; i++)
            {
                index = (index + 1) & 0xff;
                num2 = (this.state[index] + num2) & 0xff;
                byte num3 = this.state[index];
                this.state[index] = this.state[num2];
                this.state[num2] = num3;
                outputData[i] = (byte) (inputData[i] ^ this.state[(this.state[index] + this.state[num2]) & 0xff]);
            }
        }

        private void EncryptString(PdfString value)
        {
            if (value.Length != 0)
            {
                byte[] encryptionValue = value.EncryptionValue;
                this.PrepareKey();
                this.EncryptRC4(encryptionValue);
                value.EncryptionValue = encryptionValue;
            }
        }

        private bool EqualsKey(byte[] value, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (this.userKey[i] != value[i])
                {
                    return false;
                }
            }
            return true;
        }

        private void InitEncryptionKey(byte[] documentID, byte[] userPad, byte[] ownerKey, int permissions, bool strongEncryption)
        {
            this.ownerKey = ownerKey;
            this.encryptionKey = new byte[strongEncryption ? 0x10 : 5];
            this.md5.Initialize();
            this.md5.TransformBlock(userPad, 0, userPad.Length, userPad, 0);
            this.md5.TransformBlock(ownerKey, 0, ownerKey.Length, ownerKey, 0);
            byte[] inputBuffer = new byte[] { (byte) permissions, (byte) (permissions >> 8), (byte) (permissions >> 0x10), (byte) (permissions >> 0x18) };
            this.md5.TransformBlock(inputBuffer, 0, 4, inputBuffer, 0);
            this.md5.TransformBlock(documentID, 0, documentID.Length, documentID, 0);
            this.md5.TransformFinalBlock(inputBuffer, 0, 0);
            byte[] hash = this.md5.Hash;
            this.md5.Initialize();
            if (this.encryptionKey.Length == 0x10)
            {
                for (int i = 0; i < 50; i++)
                {
                    hash = this.md5.ComputeHash(hash);
                    this.md5.Initialize();
                }
            }
            Array.Copy(hash, 0, this.encryptionKey, 0, this.encryptionKey.Length);
        }

        private void InitWidhOwnerPassword(byte[] documentID, string ownerPassword, byte[] ownerKey, int permissions, bool strongEncryption)
        {
            byte[] userPad = this.ComputeOwnerKey(ownerKey, PadPassword(ownerPassword), strongEncryption);
            this.InitEncryptionKey(documentID, userPad, ownerKey, permissions, strongEncryption);
            this.SetupUserKey(documentID);
        }

        private void InitWidhUserPassword(byte[] documentID, string userPassword, byte[] ownerKey, int permissions, bool strongEncryption)
        {
            this.InitEncryptionKey(documentID, PadPassword(userPassword), ownerKey, permissions, strongEncryption);
            this.SetupUserKey(documentID);
        }

        private static byte[] PadPassword(string password)
        {
            byte[] destinationArray = new byte[0x20];
            if (password == null)
            {
                Array.Copy(passwordPadding, 0, destinationArray, 0, 0x20);
                return destinationArray;
            }
            int length = password.Length;
            Array.Copy(PdfEncoders.RawEncoding.GetBytes(password), 0, destinationArray, 0, Math.Min(length, 0x20));
            if (length < 0x20)
            {
                Array.Copy(passwordPadding, 0, destinationArray, length, 0x20 - length);
            }
            return destinationArray;
        }

        public void PrepareEncryption()
        {
            PdfInteger integer;
            PdfInteger integer2;
            PdfInteger integer3;
            int permission = (int) this.Permission;
            bool strongEncryption = base.document.securitySettings.DocumentSecurityLevel == PdfDocumentSecurityLevel.Encrypted128Bit;
            if (strongEncryption)
            {
                integer = new PdfInteger(2);
                integer2 = new PdfInteger(0x80);
                integer3 = new PdfInteger(3);
            }
            else
            {
                integer = new PdfInteger(1);
                integer2 = new PdfInteger(40);
                integer3 = new PdfInteger(2);
            }
            if (string.IsNullOrEmpty(this.userPassword))
            {
                this.userPassword = "";
            }
            if (string.IsNullOrEmpty(this.ownerPassword))
            {
                this.ownerPassword = this.userPassword;
            }
            permission |= strongEncryption ? -3904 : -64;
            permission &= -4;
            PdfInteger integer4 = new PdfInteger(permission);
            byte[] userPad = PadPassword(this.userPassword);
            byte[] ownerPad = PadPassword(this.ownerPassword);
            this.md5.Initialize();
            this.ownerKey = this.ComputeOwnerKey(userPad, ownerPad, strongEncryption);
            byte[] bytes = PdfEncoders.RawEncoding.GetBytes(base.document.Internals.FirstDocumentID);
            this.InitWidhUserPassword(bytes, this.userPassword, this.ownerKey, permission, strongEncryption);
            PdfString str = new PdfString(PdfEncoders.RawEncoding.GetString(this.ownerKey));
            PdfString str2 = new PdfString(PdfEncoders.RawEncoding.GetString(this.userKey));
            base.Elements["/Filter"] = new PdfName("/Standard");
            base.Elements["/V"] = integer;
            base.Elements["/Length"] = integer2;
            base.Elements["/R"] = integer3;
            base.Elements["/O"] = str;
            base.Elements["/U"] = str2;
            base.Elements["/P"] = integer4;
        }

        private void PrepareKey()
        {
            this.PrepareRC4Key(this.key, 0, this.keySize);
        }

        private void PrepareRC4Key(byte[] key)
        {
            this.PrepareRC4Key(key, 0, key.Length);
        }

        private void PrepareRC4Key(byte[] key, int offset, int length)
        {
            int num = 0;
            int index = 0;
            for (int i = 0; i < 0x100; i++)
            {
                this.state[i] = (byte) i;
            }
            for (int j = 0; j < 0x100; j++)
            {
                index = ((key[num + offset] + this.state[j]) + index) & 0xff;
                byte num4 = this.state[j];
                this.state[j] = this.state[index];
                this.state[index] = num4;
                num = (num + 1) % length;
            }
        }

        internal void SetHashKey(PdfObjectID id)
        {
            byte[] inputBuffer = new byte[5];
            this.md5.Initialize();
            inputBuffer[0] = (byte) id.ObjectNumber;
            inputBuffer[1] = (byte) (id.ObjectNumber >> 8);
            inputBuffer[2] = (byte) (id.ObjectNumber >> 0x10);
            inputBuffer[3] = (byte) id.GenerationNumber;
            inputBuffer[4] = (byte) (id.GenerationNumber >> 8);
            this.md5.TransformBlock(this.encryptionKey, 0, this.encryptionKey.Length, this.encryptionKey, 0);
            this.md5.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            this.key = this.md5.Hash;
            this.md5.Initialize();
            this.keySize = this.encryptionKey.Length + 5;
            if (this.keySize > 0x10)
            {
                this.keySize = 0x10;
            }
        }

        private void SetupUserKey(byte[] documentID)
        {
            if (this.encryptionKey.Length == 0x10)
            {
                this.md5.TransformBlock(passwordPadding, 0, passwordPadding.Length, passwordPadding, 0);
                this.md5.TransformFinalBlock(documentID, 0, documentID.Length);
                byte[] hash = this.md5.Hash;
                this.md5.Initialize();
                Array.Copy(hash, 0, this.userKey, 0, 0x10);
                for (int i = 0x10; i < 0x20; i++)
                {
                    this.userKey[i] = 0;
                }
                for (int j = 0; j < 20; j++)
                {
                    for (int k = 0; k < this.encryptionKey.Length; k++)
                    {
                        hash[k] = (byte) (this.encryptionKey[k] ^ j);
                    }
                    this.PrepareRC4Key(hash, 0, this.encryptionKey.Length);
                    this.EncryptRC4(this.userKey, 0, 0x10);
                }
            }
            else
            {
                this.PrepareRC4Key(this.encryptionKey);
                this.EncryptRC4(passwordPadding, this.userKey);
            }
        }

        public PasswordValidity ValidatePassword(string inputPassword)
        {
            string name = base.Elements.GetName("/Filter");
            int integer = base.Elements.GetInteger("/V");
            if (((name != "/Standard") || (integer < 1)) || (integer > 3))
            {
                throw new PdfReaderException(PSSR.UnknownEncryption);
            }
            byte[] bytes = PdfEncoders.RawEncoding.GetBytes(this.Owner.Internals.FirstDocumentID);
            byte[] ownerKey = PdfEncoders.RawEncoding.GetBytes(base.Elements.GetString("/O"));
            byte[] buffer3 = PdfEncoders.RawEncoding.GetBytes(base.Elements.GetString("/U"));
            int permissions = base.Elements.GetInteger("/P");
            int num3 = base.Elements.GetInteger("/R");
            if (inputPassword == null)
            {
                inputPassword = "";
            }
            bool strongEncryption = num3 == 3;
            int length = strongEncryption ? 0x10 : 0x20;
            PdfEncoders.RawEncoding.GetBytes(inputPassword);
            this.InitWidhOwnerPassword(bytes, inputPassword, ownerKey, permissions, strongEncryption);
            if (this.EqualsKey(buffer3, length))
            {
                base.document.SecuritySettings.hasOwnerPermissions = true;
                return PasswordValidity.OwnerPassword;
            }
            base.document.SecuritySettings.hasOwnerPermissions = false;
            PdfEncoders.RawEncoding.GetBytes(inputPassword);
            this.InitWidhUserPassword(bytes, inputPassword, ownerKey, permissions, strongEncryption);
            if (!this.EqualsKey(buffer3, length))
            {
                return PasswordValidity.Invalid;
            }
            return PasswordValidity.UserPassword;
        }

        internal override void WriteObject(PdfWriter writer)
        {
            PdfStandardSecurityHandler securityHandler = writer.SecurityHandler;
            writer.SecurityHandler = null;
            base.WriteObject(writer);
            writer.SecurityHandler = securityHandler;
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public string OwnerPassword
        {
            set
            {
                if (base.document.securitySettings.DocumentSecurityLevel == PdfDocumentSecurityLevel.None)
                {
                    base.document.securitySettings.DocumentSecurityLevel = PdfDocumentSecurityLevel.Encrypted128Bit;
                }
                this.ownerPassword = value;
            }
        }

        internal PdfUserAccessPermission Permission
        {
            get
            {
                PdfUserAccessPermission integer = (PdfUserAccessPermission) base.Elements.GetInteger("/P");
                if (integer == 0)
                {
                    integer = -3;
                }
                return integer;
            }
            set
            {
                base.Elements.SetInteger("/P", (int) value);
            }
        }

        public string UserPassword
        {
            set
            {
                if (base.document.securitySettings.DocumentSecurityLevel == PdfDocumentSecurityLevel.None)
                {
                    base.document.securitySettings.DocumentSecurityLevel = PdfDocumentSecurityLevel.Encrypted128Bit;
                }
                this.userPassword = value;
            }
        }

        internal sealed class Keys : PdfSecurityHandler.Keys
        {
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string EncryptMetadata = "/EncryptMetadata";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Required | KeyType.String)]
            public const string O = "/O";
            [KeyInfo(KeyType.Required | KeyType.Integer)]
            public const string P = "/P";
            [KeyInfo(KeyType.Required | KeyType.Integer)]
            public const string R = "/R";
            [KeyInfo(KeyType.Required | KeyType.String)]
            public const string U = "/U";

            public static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfStandardSecurityHandler.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

