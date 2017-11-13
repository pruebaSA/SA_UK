namespace AjaxControlToolkit.Design
{
    using System;
    using System.Reflection;

    internal static class ReferencedAssemblies
    {
        private static Assembly _EnvDTE;
        private static Assembly _EnvDTE80;
        private static Assembly _VsWebSite;

        public static Assembly EnvDTE
        {
            get
            {
                if (_EnvDTE == null)
                {
                    _EnvDTE = Assembly.Load("EnvDTE, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
                }
                return _EnvDTE;
            }
        }

        public static Assembly EnvDTE80
        {
            get
            {
                if (_EnvDTE80 == null)
                {
                    _EnvDTE80 = Assembly.Load("EnvDTE80, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
                }
                return _EnvDTE80;
            }
        }

        public static Assembly VsWebSite
        {
            get
            {
                if (_VsWebSite == null)
                {
                    _VsWebSite = Assembly.Load("VsWebSite.Interop, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
                }
                return _VsWebSite;
            }
        }
    }
}

