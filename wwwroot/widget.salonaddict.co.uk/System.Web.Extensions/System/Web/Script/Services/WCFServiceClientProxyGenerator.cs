namespace System.Web.Script.Services
{
    using System;
    using System.Globalization;
    using System.ServiceModel.Description;
    using System.Text;

    internal class WCFServiceClientProxyGenerator : ClientProxyGenerator
    {
        private string _path;
        private const string DataContractXsdBaseNamespace = "http://schemas.datacontract.org/2004/07/";
        private const int MaxIdentifierLength = 0x1ff;

        internal WCFServiceClientProxyGenerator(string path, bool debugMode)
        {
            this._path = path;
            base._debugMode = debugMode;
        }

        private static void AddToNamespace(StringBuilder builder, string fragment)
        {
            if (fragment != null)
            {
                bool flag = true;
                for (int i = 0; (i < fragment.Length) && (builder.Length < 0x1ff); i++)
                {
                    char c = fragment[i];
                    if (IsValid(c))
                    {
                        if (flag && !IsValidStart(c))
                        {
                            builder.Append("_");
                        }
                        builder.Append(c);
                        flag = false;
                    }
                    else if ((((c == '.') || (c == '/')) || (c == ':')) && ((builder.Length == 1) || ((builder.Length > 1) && (builder[builder.Length - 1] != '.'))))
                    {
                        builder.Append('.');
                        flag = true;
                    }
                }
            }
        }

        internal static string GetClientProxyScript(Type contractType, string path, bool debugMode)
        {
            WebServiceData webServiceData = WebServiceData.GetWebServiceData(ContractDescription.GetContract(contractType));
            WCFServiceClientProxyGenerator generator = new WCFServiceClientProxyGenerator(path, debugMode);
            return generator.GetClientProxyScript(webServiceData);
        }

        protected override string GetClientTypeNamespace(string ns)
        {
            if (string.IsNullOrEmpty(ns))
            {
                return string.Empty;
            }
            Uri result = null;
            StringBuilder builder = new StringBuilder();
            if (Uri.TryCreate(ns, UriKind.RelativeOrAbsolute, out result))
            {
                if (!result.IsAbsoluteUri)
                {
                    AddToNamespace(builder, result.OriginalString);
                }
                else
                {
                    string absoluteUri = result.AbsoluteUri;
                    if (absoluteUri.StartsWith("http://schemas.datacontract.org/2004/07/", StringComparison.Ordinal))
                    {
                        AddToNamespace(builder, absoluteUri.Substring("http://schemas.datacontract.org/2004/07/".Length));
                    }
                    else
                    {
                        string host = result.Host;
                        if (host != null)
                        {
                            AddToNamespace(builder, host);
                        }
                        string pathAndQuery = result.PathAndQuery;
                        if (pathAndQuery != null)
                        {
                            AddToNamespace(builder, pathAndQuery);
                        }
                    }
                }
            }
            if (builder.Length == 0)
            {
                return string.Empty;
            }
            int length = builder.Length;
            if (builder[builder.Length - 1] == '.')
            {
                length--;
            }
            length = Math.Min(0x1ff, length);
            return builder.ToString(0, length);
        }

        protected override string GetProxyPath() => 
            this._path;

        protected override string GetProxyTypeName(WebServiceData data) => 
            this.GetClientTypeNamespace(data.TypeData.TypeName);

        private static bool IsValid(char c)
        {
            switch (char.GetUnicodeCategory(c))
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.SpacingCombiningMark:
                case UnicodeCategory.DecimalDigitNumber:
                case UnicodeCategory.ConnectorPunctuation:
                    return true;
            }
            return false;
        }

        private static bool IsValidStart(char c) => 
            (char.GetUnicodeCategory(c) != UnicodeCategory.DecimalDigitNumber);
    }
}

