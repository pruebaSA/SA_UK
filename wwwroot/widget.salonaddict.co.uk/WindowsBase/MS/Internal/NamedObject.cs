namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Globalization;

    [FriendAccessAllowed]
    internal class NamedObject
    {
        private string _name;

        public NamedObject(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(name);
            }
            this._name = name;
        }

        public override string ToString()
        {
            if (this._name[0] != '{')
            {
                this._name = string.Format(CultureInfo.InvariantCulture, "{{{0}}}", new object[] { this._name });
            }
            return this._name;
        }
    }
}

