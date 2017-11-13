namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [ConfigurationCollection(typeof(CustomError), AddItemName="error", CollectionType=ConfigurationElementCollectionType.BasicMap), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class CustomErrorCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public void Add(CustomError customError)
        {
            this.BaseAdd(customError);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new CustomError();

        public CustomError Get(int index) => 
            ((CustomError) base.BaseGet(index));

        public CustomError Get(string statusCode) => 
            ((CustomError) base.BaseGet(int.Parse(statusCode, CultureInfo.InvariantCulture)));

        protected override object GetElementKey(ConfigurationElement element) => 
            ((CustomError) element).StatusCode;

        public string GetKey(int index)
        {
            int num = (int) base.BaseGetKey(index);
            return num.ToString(CultureInfo.InvariantCulture);
        }

        public void Remove(string statusCode)
        {
            base.BaseRemove(int.Parse(statusCode, CultureInfo.InvariantCulture));
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        public void Set(CustomError customError)
        {
            base.BaseAdd(customError, false);
        }

        public string[] AllKeys
        {
            get
            {
                object[] objArray = base.BaseGetAllKeys();
                string[] strArray = new string[objArray.Length];
                for (int i = 0; i < objArray.Length; i++)
                {
                    strArray[i] = ((int) objArray[i]).ToString(CultureInfo.InvariantCulture);
                }
                return strArray;
            }
        }

        public override ConfigurationElementCollectionType CollectionType =>
            ConfigurationElementCollectionType.BasicMap;

        protected override string ElementName =>
            "error";

        public CustomError this[string statusCode] =>
            ((CustomError) base.BaseGet(int.Parse(statusCode, CultureInfo.InvariantCulture)));

        public CustomError this[int index]
        {
            get => 
                ((CustomError) base.BaseGet(index));
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

