namespace MS.Internal
{
    using System;
    using System.Text;

    internal class AvTraceBuilder
    {
        private StringBuilder _sb;

        public AvTraceBuilder()
        {
            this._sb = new StringBuilder();
        }

        public AvTraceBuilder(string message)
        {
            this._sb = new StringBuilder(message);
        }

        public void Append(string message)
        {
            this._sb.Append(message);
        }

        public void AppendFormat(string message, params object[] args)
        {
            object[] objArray = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                string str = args[i] as string;
                objArray[i] = (str != null) ? str : AvTrace.ToStringHelper(args[i]);
            }
            this._sb.AppendFormat(message, objArray);
        }

        public void AppendFormat(string message, object arg1)
        {
            this._sb.AppendFormat(message, new object[] { AvTrace.ToStringHelper(arg1) });
        }

        public void AppendFormat(string message, string arg1)
        {
            this._sb.AppendFormat(message, new object[] { arg1 });
        }

        public void AppendFormat(string message, object arg1, object arg2)
        {
            this._sb.AppendFormat(message, new object[] { AvTrace.ToStringHelper(arg1), AvTrace.ToStringHelper(arg2) });
        }

        public void AppendFormat(string message, string arg1, string arg2)
        {
            this._sb.AppendFormat(message, new object[] { arg1, arg2 });
        }

        public override string ToString() => 
            this._sb.ToString();
    }
}

