namespace AjaxControlToolkit
{
    using System;

    public class RatingEventArgs : EventArgs
    {
        private string _callbackResult;
        private string _tag;
        private string _value;

        public RatingEventArgs(string args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            string[] strArray = args.Split(new char[] { ';' });
            if (strArray.Length == 2)
            {
                this._value = strArray[0];
                this._tag = strArray[1];
            }
        }

        public string CallbackResult
        {
            get => 
                this._callbackResult;
            set
            {
                this._callbackResult = value;
            }
        }

        public string Tag =>
            this._tag;

        public string Value =>
            this._value;
    }
}

