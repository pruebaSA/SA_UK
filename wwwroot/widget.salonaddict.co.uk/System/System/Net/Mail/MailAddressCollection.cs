namespace System.Net.Mail
{
    using System;
    using System.Collections.ObjectModel;
    using System.Net.Mime;
    using System.Text;

    public class MailAddressCollection : Collection<MailAddress>
    {
        public void Add(string addresses)
        {
            if (addresses == null)
            {
                throw new ArgumentNullException("addresses");
            }
            if (addresses == string.Empty)
            {
                throw new ArgumentException(SR.GetString("net_emptystringcall", new object[] { "addresses" }), "addresses");
            }
            this.ParseValue(addresses);
        }

        protected override void InsertItem(int index, MailAddress item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            base.InsertItem(index, item);
        }

        internal void ParseValue(string addresses)
        {
            for (int i = 0; i < addresses.Length; i++)
            {
                MailAddress item = MailBnfHelper.ReadMailAddress(addresses, ref i);
                if (item == null)
                {
                    return;
                }
                base.Add(item);
                if (!MailBnfHelper.SkipCFWS(addresses, ref i))
                {
                    break;
                }
                if (addresses[i] != ',')
                {
                    return;
                }
            }
        }

        protected override void SetItem(int index, MailAddress item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            base.SetItem(index, item);
        }

        internal string ToEncodedString()
        {
            bool flag = true;
            StringBuilder builder = new StringBuilder();
            foreach (MailAddress address in this)
            {
                if (!flag)
                {
                    builder.Append(", ");
                }
                builder.Append(address.ToEncodedString());
                flag = false;
            }
            return builder.ToString();
        }

        public override string ToString()
        {
            bool flag = true;
            StringBuilder builder = new StringBuilder();
            foreach (MailAddress address in this)
            {
                if (!flag)
                {
                    builder.Append(", ");
                }
                builder.Append(address.ToString());
                flag = false;
            }
            return builder.ToString();
        }
    }
}

