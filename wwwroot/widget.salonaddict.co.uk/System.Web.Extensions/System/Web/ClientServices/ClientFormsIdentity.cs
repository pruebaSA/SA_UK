namespace System.Web.ClientServices
{
    using System;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Principal;
    using System.Web.Security;

    public class ClientFormsIdentity : IIdentity, IDisposable
    {
        private CookieContainer _AuthenticationCookies;
        private string _AuthenticationType;
        private bool _Disposed;
        private bool _IsAuthenticated;
        private string _Name;
        private SecureString _Password;
        private MembershipProvider _Provider;

        public ClientFormsIdentity(string name, string password, MembershipProvider provider, string authenticationType, bool isAuthenticated, CookieContainer authenticationCookies)
        {
            this._Name = name;
            this._AuthenticationType = authenticationType;
            this._IsAuthenticated = isAuthenticated;
            this._AuthenticationCookies = authenticationCookies;
            this._Password = GetSecureStringFromString(password);
            this._Provider = provider;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && (this._Password != null))
            {
                this._Password.Dispose();
            }
            this._Disposed = true;
        }

        private static SecureString GetSecureStringFromString(string password)
        {
            char[] chArray = password.ToCharArray();
            SecureString str = new SecureString();
            for (int i = 0; i < chArray.Length; i++)
            {
                str.AppendChar(chArray[i]);
            }
            str.MakeReadOnly();
            return str;
        }

        private static string GetStringFromSecureString(SecureString securePass)
        {
            string str;
            IntPtr zero = IntPtr.Zero;
            try
            {
                zero = Marshal.SecureStringToBSTR(securePass);
                str = Marshal.PtrToStringBSTR(zero);
            }
            finally
            {
                if (zero != IntPtr.Zero)
                {
                    Marshal.FreeBSTR(zero);
                }
            }
            return str;
        }

        public void RevalidateUser()
        {
            if (this._Disposed)
            {
                throw new ObjectDisposedException(base.GetType().FullName);
            }
            this._Provider.ValidateUser(this._Name, GetStringFromSecureString(this._Password));
        }

        public CookieContainer AuthenticationCookies =>
            this._AuthenticationCookies;

        public string AuthenticationType =>
            this._AuthenticationType;

        public bool IsAuthenticated =>
            this._IsAuthenticated;

        public string Name =>
            this._Name;

        public MembershipProvider Provider =>
            this._Provider;
    }
}

