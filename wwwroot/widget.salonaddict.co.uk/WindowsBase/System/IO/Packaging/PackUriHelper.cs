namespace System.IO.Packaging
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Windows;

    public static class PackUriHelper
    {
        private static readonly Uri _defaultUri = new Uri("http://defaultcontainer/");
        private static readonly Uri _packageRootUri = new Uri("/", UriKind.Relative);
        private static readonly string _relationshipPartExtensionName = ".rels";
        private static readonly string _relationshipPartSegmentName = "_rels";
        private static readonly char[] _specialCharacters = new char[] { '%', '@', ',', '?' };
        internal static readonly char BackwardSlashChar = '\\';
        internal static readonly char ForwardSlashChar = '/';
        public static readonly string UriSchemePack = "pack";

        [SecurityTreatAsSafe, SecurityCritical]
        static PackUriHelper()
        {
            if (!UriParser.IsKnownScheme(UriSchemePack))
            {
                try
                {
                    new SecurityPermission(SecurityPermissionFlag.Infrastructure).Assert();
                    UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority), UriSchemePack, -1);
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
            }
        }

        public static int ComparePackUri(Uri firstPackUri, Uri secondPackUri)
        {
            int num;
            Uri uri;
            Uri uri2;
            Uri uri3;
            Uri uri4;
            if ((firstPackUri == null) || (secondPackUri == null))
            {
                return CompareUsingSystemUri(firstPackUri, secondPackUri);
            }
            ValidateAndGetPackUriComponents(firstPackUri, out uri, out uri3);
            ValidateAndGetPackUriComponents(secondPackUri, out uri2, out uri4);
            if ((uri.Scheme == UriSchemePack) && (uri2.Scheme == UriSchemePack))
            {
                num = ComparePackUri(uri, uri2);
            }
            else
            {
                num = CompareUsingSystemUri(uri, uri2);
            }
            if (num == 0)
            {
                num = ComparePartUri(uri3, uri4);
            }
            return num;
        }

        public static int ComparePartUri(Uri firstPartUri, Uri secondPartUri)
        {
            if (firstPartUri != null)
            {
                firstPartUri = ValidatePartUri(firstPartUri);
            }
            if (secondPartUri != null)
            {
                secondPartUri = ValidatePartUri(secondPartUri);
            }
            if ((firstPartUri != null) && (secondPartUri != null))
            {
                return ((IComparable<ValidatedPartUri>) firstPartUri).CompareTo((ValidatedPartUri) secondPartUri);
            }
            return CompareUsingSystemUri(firstPartUri, secondPartUri);
        }

        private static int CompareUsingSystemUri(Uri firstUri, Uri secondUri) => 
            Uri.Compare(firstUri, secondUri, UriComponents.HttpRequestUrl | UriComponents.UserInfo, UriFormat.UriEscaped, StringComparison.Ordinal);

        public static Uri Create(Uri packageUri) => 
            Create(packageUri, null, null);

        public static Uri Create(Uri packageUri, Uri partUri) => 
            Create(packageUri, partUri, null);

        public static Uri Create(Uri packageUri, Uri partUri, string fragment)
        {
            packageUri = ValidatePackageUri(packageUri);
            if (partUri != null)
            {
                partUri = ValidatePartUri(partUri);
            }
            if ((fragment != null) && ((fragment == string.Empty) || (fragment[0] != '#')))
            {
                throw new ArgumentException(System.Windows.SR.Get("FragmentMustStartWithHash"));
            }
            string components = packageUri.GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped);
            if (packageUri.Fragment.Length != 0)
            {
                components = components.Substring(0, components.IndexOf('#'));
            }
            components = EscapeSpecialCharacters(components).Replace('/', ',');
            Uri baseUri = new Uri(UriSchemePack + Uri.SchemeDelimiter + components);
            if (partUri != null)
            {
                baseUri = new Uri(baseUri, partUri);
            }
            if (fragment != null)
            {
                baseUri = new Uri(baseUri.GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped) + fragment);
            }
            return new Uri(baseUri.GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped));
        }

        public static Uri CreatePartUri(Uri partUri)
        {
            if (partUri == null)
            {
                throw new ArgumentNullException("partUri");
            }
            ThrowIfAbsoluteUri(partUri);
            string components = partUri.GetComponents(UriComponents.SerializationInfoString, UriFormat.SafeUnescaped);
            ThrowIfPartNameStartsWithTwoSlashes(components);
            ThrowIfFragmentPresent(components);
            Uri uri = new Uri(_defaultUri, partUri);
            string stringForPartUriFromAnyUri = GetStringForPartUriFromAnyUri(uri);
            if (stringForPartUriFromAnyUri == string.Empty)
            {
                throw new ArgumentException(System.Windows.SR.Get("PartUriIsEmpty"));
            }
            ThrowIfPartNameEndsWithSlash(stringForPartUriFromAnyUri);
            return new ValidatedPartUri(stringForPartUriFromAnyUri);
        }

        private static string EscapeSpecialCharacters(string path)
        {
            foreach (char ch in _specialCharacters)
            {
                string str = ch.ToString();
                if (path.Contains(str))
                {
                    path = path.Replace(str, Uri.HexEscape(ch));
                }
            }
            return path;
        }

        private static ArgumentException GetExceptionIfAbsoluteUri(Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                return new ArgumentException(System.Windows.SR.Get("URIShouldNotBeAbsolute"));
            }
            return null;
        }

        private static ArgumentException GetExceptionIfFragmentPresent(string partName)
        {
            if (partName.Contains("#"))
            {
                return new ArgumentException(System.Windows.SR.Get("PartUriCannotHaveAFragment"));
            }
            return null;
        }

        private static ArgumentException GetExceptionIfPartNameEndsWithSlash(string partName)
        {
            if ((partName.Length > 0) && (partName[partName.Length - 1] == '/'))
            {
                return new ArgumentException(System.Windows.SR.Get("PartUriShouldNotEndWithForwardSlash"));
            }
            return null;
        }

        private static ArgumentException GetExceptionIfPartNameStartsWithTwoSlashes(string partName)
        {
            if (((partName.Length > 1) && (partName[0] == '/')) && (partName[1] == '/'))
            {
                return new ArgumentException(System.Windows.SR.Get("PartUriShouldNotStartWithTwoForwardSlashes"));
            }
            return null;
        }

        private static Exception GetExceptionIfPartUriInvalid(Uri partUri, out string partUriString)
        {
            partUriString = string.Empty;
            if (partUri == null)
            {
                return new ArgumentNullException("partUri");
            }
            Exception exceptionIfAbsoluteUri = null;
            exceptionIfAbsoluteUri = GetExceptionIfAbsoluteUri(partUri);
            if (exceptionIfAbsoluteUri != null)
            {
                return exceptionIfAbsoluteUri;
            }
            string stringForPartUriFromAnyUri = GetStringForPartUriFromAnyUri(partUri);
            if (stringForPartUriFromAnyUri == string.Empty)
            {
                return new ArgumentException(System.Windows.SR.Get("PartUriIsEmpty"));
            }
            if (stringForPartUriFromAnyUri[0] != '/')
            {
                return new ArgumentException(System.Windows.SR.Get("PartUriShouldStartWithForwardSlash"));
            }
            exceptionIfAbsoluteUri = GetExceptionIfPartNameStartsWithTwoSlashes(stringForPartUriFromAnyUri);
            if (exceptionIfAbsoluteUri != null)
            {
                return exceptionIfAbsoluteUri;
            }
            exceptionIfAbsoluteUri = GetExceptionIfPartNameEndsWithSlash(stringForPartUriFromAnyUri);
            if (exceptionIfAbsoluteUri != null)
            {
                return exceptionIfAbsoluteUri;
            }
            exceptionIfAbsoluteUri = GetExceptionIfFragmentPresent(stringForPartUriFromAnyUri);
            if (exceptionIfAbsoluteUri != null)
            {
                return exceptionIfAbsoluteUri;
            }
            string components = new Uri(_defaultUri, stringForPartUriFromAnyUri).GetComponents(UriComponents.KeepDelimiter | UriComponents.Path, UriFormat.UriEscaped);
            if (string.CompareOrdinal(partUri.ToString().ToUpperInvariant(), components.ToUpperInvariant()) != 0)
            {
                return new ArgumentException(System.Windows.SR.Get("InvalidPartUri"));
            }
            partUriString = stringForPartUriFromAnyUri;
            return null;
        }

        public static Uri GetNormalizedPartUri(Uri partUri)
        {
            if (partUri == null)
            {
                throw new ArgumentNullException("partUri");
            }
            if (!(partUri is ValidatedPartUri))
            {
                partUri = ValidatePartUri(partUri);
            }
            return ((ValidatedPartUri) partUri).NormalizedPartUri;
        }

        public static Uri GetPackageUri(Uri packUri)
        {
            Uri uri;
            Uri uri2;
            ValidateAndGetPackUriComponents(packUri, out uri, out uri2);
            return uri;
        }

        private static Uri GetPackageUriComponent(Uri packUri)
        {
            Uri uri = new Uri(Uri.UnescapeDataString(packUri.GetComponents(UriComponents.HostAndPort, UriFormat.UriEscaped).Replace(',', '/')));
            if (uri.Fragment != string.Empty)
            {
                throw new ArgumentException(System.Windows.SR.Get("InnerPackageUriHasFragment"));
            }
            return uri;
        }

        public static Uri GetPartUri(Uri packUri)
        {
            Uri uri;
            Uri uri2;
            ValidateAndGetPackUriComponents(packUri, out uri, out uri2);
            return uri2;
        }

        private static ValidatedPartUri GetPartUriComponent(Uri packUri)
        {
            string stringForPartUriFromAnyUri = GetStringForPartUriFromAnyUri(packUri);
            if (stringForPartUriFromAnyUri == string.Empty)
            {
                return null;
            }
            return ValidatePartUri(new Uri(stringForPartUriFromAnyUri, UriKind.Relative));
        }

        public static Uri GetRelationshipPartUri(Uri partUri)
        {
            if (partUri == null)
            {
                throw new ArgumentNullException("partUri");
            }
            if (Uri.Compare(partUri, PackageRootUri, UriComponents.SerializationInfoString, UriFormat.UriEscaped, StringComparison.Ordinal) == 0)
            {
                return PackageRelationship.ContainerRelationshipPartName;
            }
            partUri = ValidatePartUri(partUri);
            if (IsRelationshipPartUri(partUri))
            {
                throw new ArgumentException(System.Windows.SR.Get("RelationshipPartUriNotExpected"));
            }
            string partUriString = ((ValidatedPartUri) partUri).PartUriString;
            string fileName = Path.GetFileName(partUriString);
            return new ValidatedPartUri((Path.Combine(Path.Combine(partUriString.Substring(0, partUriString.Length - fileName.Length), _relationshipPartSegmentName), fileName) + _relationshipPartExtensionName).Replace(BackwardSlashChar, ForwardSlashChar), true);
        }

        public static Uri GetRelativeUri(Uri sourcePartUri, Uri targetPartUri)
        {
            if (sourcePartUri == null)
            {
                throw new ArgumentNullException("sourcePartUri");
            }
            if (targetPartUri == null)
            {
                throw new ArgumentNullException("targetPartUri");
            }
            sourcePartUri = new Uri(_defaultUri, ValidatePartUri(sourcePartUri));
            targetPartUri = new Uri(_defaultUri, ValidatePartUri(targetPartUri));
            return sourcePartUri.MakeRelativeUri(targetPartUri);
        }

        public static Uri GetSourcePartUriFromRelationshipPartUri(Uri relationshipPartUri)
        {
            if (relationshipPartUri == null)
            {
                throw new ArgumentNullException("relationshipPartUri");
            }
            relationshipPartUri = ValidatePartUri(relationshipPartUri);
            if (!IsRelationshipPartUri(relationshipPartUri))
            {
                throw new ArgumentException(System.Windows.SR.Get("RelationshipPartUriExpected"));
            }
            if (ComparePartUri(PackageRelationship.ContainerRelationshipPartName, relationshipPartUri) == 0)
            {
                return PackageRootUri;
            }
            string partUriString = ((ValidatedPartUri) relationshipPartUri).PartUriString;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(partUriString);
            partUriString = partUriString.Substring(0, ((partUriString.Length - fileNameWithoutExtension.Length) - _relationshipPartExtensionName.Length) - 1);
            return new ValidatedPartUri(Path.Combine(partUriString.Substring(0, partUriString.Length - _relationshipPartSegmentName.Length), fileNameWithoutExtension).Replace(BackwardSlashChar, ForwardSlashChar), false);
        }

        internal static string GetStringForPartUri(Uri partUri)
        {
            if (!(partUri is ValidatedPartUri))
            {
                partUri = ValidatePartUri(partUri);
            }
            return ((ValidatedPartUri) partUri).PartUriString;
        }

        private static string GetStringForPartUriFromAnyUri(Uri partUri)
        {
            Uri uri;
            if (!partUri.IsAbsoluteUri)
            {
                uri = new Uri(partUri.GetComponents(UriComponents.SerializationInfoString, UriFormat.SafeUnescaped), UriKind.Relative);
            }
            else
            {
                uri = new Uri(partUri.GetComponents(UriComponents.KeepDelimiter | UriComponents.Path, UriFormat.SafeUnescaped), UriKind.Relative);
            }
            string components = uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped);
            if (IsPartNameEmpty(components))
            {
                return string.Empty;
            }
            return components;
        }

        internal static bool IsPackUri(Uri uri) => 
            ((uri != null) && (string.Compare(uri.Scheme, UriSchemePack, StringComparison.OrdinalIgnoreCase) == 0));

        private static bool IsPartNameEmpty(string partName)
        {
            if ((partName.Length != 0) && ((partName.Length != 1) || (partName[0] != '/')))
            {
                return false;
            }
            return true;
        }

        public static bool IsRelationshipPartUri(Uri partUri)
        {
            if (partUri == null)
            {
                throw new ArgumentNullException("partUri");
            }
            if (!(partUri is ValidatedPartUri))
            {
                partUri = ValidatePartUri(partUri);
            }
            return ((ValidatedPartUri) partUri).IsRelationshipPartUri;
        }

        public static Uri ResolvePartUri(Uri sourcePartUri, Uri targetUri)
        {
            Uri uri;
            if (sourcePartUri == null)
            {
                throw new ArgumentNullException("sourcePartUri");
            }
            if (targetUri == null)
            {
                throw new ArgumentNullException("targetUri");
            }
            ThrowIfAbsoluteUri(sourcePartUri);
            ThrowIfAbsoluteUri(targetUri);
            if (sourcePartUri == PackageRootUri)
            {
                uri = new Uri(_defaultUri, targetUri);
            }
            else
            {
                uri = new Uri(new Uri(_defaultUri, ValidatePartUri(sourcePartUri)), targetUri);
            }
            return new Uri(uri.AbsolutePath, UriKind.Relative);
        }

        private static void ThrowIfAbsoluteUri(Uri uri)
        {
            Exception exceptionIfAbsoluteUri = GetExceptionIfAbsoluteUri(uri);
            if (exceptionIfAbsoluteUri != null)
            {
                throw exceptionIfAbsoluteUri;
            }
        }

        private static void ThrowIfFragmentPresent(string partName)
        {
            Exception exceptionIfFragmentPresent = GetExceptionIfFragmentPresent(partName);
            if (exceptionIfFragmentPresent != null)
            {
                throw exceptionIfFragmentPresent;
            }
        }

        private static void ThrowIfPartNameEndsWithSlash(string partName)
        {
            Exception exceptionIfPartNameEndsWithSlash = GetExceptionIfPartNameEndsWithSlash(partName);
            if (exceptionIfPartNameEndsWithSlash != null)
            {
                throw exceptionIfPartNameEndsWithSlash;
            }
        }

        private static void ThrowIfPartNameStartsWithTwoSlashes(string partName)
        {
            Exception exceptionIfPartNameStartsWithTwoSlashes = GetExceptionIfPartNameStartsWithTwoSlashes(partName);
            if (exceptionIfPartNameStartsWithTwoSlashes != null)
            {
                throw exceptionIfPartNameStartsWithTwoSlashes;
            }
        }

        internal static bool TryValidatePartUri(Uri partUri, out ValidatedPartUri validatedPartUri)
        {
            string str;
            if (partUri is ValidatedPartUri)
            {
                validatedPartUri = (ValidatedPartUri) partUri;
                return true;
            }
            if (GetExceptionIfPartUriInvalid(partUri, out str) != null)
            {
                validatedPartUri = null;
                return false;
            }
            validatedPartUri = new ValidatedPartUri(str);
            return true;
        }

        internal static void ValidateAndGetPackUriComponents(Uri packUri, out Uri packageUri, out Uri partUri)
        {
            packUri = ValidatePackUri(packUri);
            packageUri = GetPackageUriComponent(packUri);
            partUri = GetPartUriComponent(packUri);
        }

        private static Uri ValidatePackageUri(Uri packageUri)
        {
            if (packageUri == null)
            {
                throw new ArgumentNullException("packageUri");
            }
            if (!packageUri.IsAbsoluteUri)
            {
                throw new ArgumentException(System.Windows.SR.Get("UriShouldBeAbsolute"));
            }
            return packageUri;
        }

        private static Uri ValidatePackUri(Uri packUri)
        {
            if (packUri == null)
            {
                throw new ArgumentNullException("packUri");
            }
            if (!packUri.IsAbsoluteUri)
            {
                throw new ArgumentException(System.Windows.SR.Get("UriShouldBeAbsolute"));
            }
            if (packUri.Scheme != UriSchemePack)
            {
                throw new ArgumentException(System.Windows.SR.Get("UriShouldBePackScheme"));
            }
            return packUri;
        }

        internal static ValidatedPartUri ValidatePartUri(Uri partUri)
        {
            string str;
            if (partUri is ValidatedPartUri)
            {
                return (ValidatedPartUri) partUri;
            }
            Exception exceptionIfPartUriInvalid = GetExceptionIfPartUriInvalid(partUri, out str);
            if (exceptionIfPartUriInvalid != null)
            {
                throw exceptionIfPartUriInvalid;
            }
            return new ValidatedPartUri(str);
        }

        internal static Uri PackageRootUri =>
            _packageRootUri;

        internal sealed class ValidatedPartUri : Uri, IComparable<PackUriHelper.ValidatedPartUri>, IEquatable<PackUriHelper.ValidatedPartUri>
        {
            private static readonly Uri _containerRelationshipNormalizedPartUri = new PackUriHelper.ValidatedPartUri("/_RELS/.RELS", true, false, true);
            private static readonly char[] _forwardSlashSeparator = new char[] { '/' };
            private bool _isNormalized;
            private bool _isRelationshipPartUri;
            private PackUriHelper.ValidatedPartUri _normalizedPartUri;
            private string _normalizedPartUriString;
            private string _partUriExtension;
            private string _partUriString;
            private static readonly string _relationshipPartUpperCaseExtension = ".RELS";
            private static readonly string _relationshipPartUpperCaseSegmentName = "_RELS";
            private static readonly string _relsrelsUpperCaseExtension = (_relationshipPartUpperCaseExtension + _relationshipPartUpperCaseExtension);

            internal ValidatedPartUri(string partUriString) : this(partUriString, false, true, false)
            {
            }

            internal ValidatedPartUri(string partUriString, bool isRelationshipUri) : this(partUriString, false, false, isRelationshipUri)
            {
            }

            private ValidatedPartUri(string partUriString, bool isNormalized, bool computeIsRelationship, bool isRelationshipPartUri) : base(partUriString, UriKind.Relative)
            {
                this._partUriString = partUriString;
                this._isNormalized = isNormalized;
                if (computeIsRelationship)
                {
                    this._isRelationshipPartUri = this.IsRelationshipUri();
                }
                else
                {
                    this._isRelationshipPartUri = isRelationshipPartUri;
                }
            }

            private int Compare(PackUriHelper.ValidatedPartUri otherPartUri)
            {
                if (otherPartUri == null)
                {
                    return 1;
                }
                return string.CompareOrdinal(this.NormalizedPartUriString, otherPartUri.NormalizedPartUriString);
            }

            private PackUriHelper.ValidatedPartUri GetNormalizedPartUri()
            {
                if (this.IsNormalized)
                {
                    return this;
                }
                return new PackUriHelper.ValidatedPartUri(this._normalizedPartUriString, true, false, this.IsRelationshipPartUri);
            }

            private string GetNormalizedPartUriString()
            {
                if (this._isNormalized)
                {
                    return this._partUriString;
                }
                return this._partUriString.ToUpperInvariant();
            }

            private bool IsRelationshipUri()
            {
                bool flag = false;
                if (!this.NormalizedPartUriString.EndsWith(_relationshipPartUpperCaseExtension, StringComparison.Ordinal))
                {
                    return false;
                }
                if (PackUriHelper.ComparePartUri(_containerRelationshipNormalizedPartUri, this) == 0)
                {
                    return true;
                }
                string[] strArray = this.NormalizedPartUriString.Split(_forwardSlashSeparator);
                if ((strArray.Length >= 3) && (strArray[strArray.Length - 1].Length > PackUriHelper._relationshipPartExtensionName.Length))
                {
                    flag = string.CompareOrdinal(strArray[strArray.Length - 2], _relationshipPartUpperCaseSegmentName) == 0;
                }
                if (((strArray.Length > 3) && flag) && (strArray[strArray.Length - 1].EndsWith(_relsrelsUpperCaseExtension, StringComparison.Ordinal) && (string.CompareOrdinal(strArray[strArray.Length - 3], _relationshipPartUpperCaseSegmentName) == 0)))
                {
                    throw new ArgumentException(System.Windows.SR.Get("NotAValidRelationshipPartUri"));
                }
                return flag;
            }

            int IComparable<PackUriHelper.ValidatedPartUri>.CompareTo(PackUriHelper.ValidatedPartUri otherPartUri) => 
                this.Compare(otherPartUri);

            bool IEquatable<PackUriHelper.ValidatedPartUri>.Equals(PackUriHelper.ValidatedPartUri otherPartUri) => 
                (this.Compare(otherPartUri) == 0);

            internal bool IsNormalized =>
                this._isNormalized;

            internal bool IsRelationshipPartUri =>
                this._isRelationshipPartUri;

            internal PackUriHelper.ValidatedPartUri NormalizedPartUri
            {
                get
                {
                    if (this._normalizedPartUri == null)
                    {
                        this._normalizedPartUri = this.GetNormalizedPartUri();
                    }
                    return this._normalizedPartUri;
                }
            }

            internal string NormalizedPartUriString
            {
                get
                {
                    if (this._normalizedPartUriString == null)
                    {
                        this._normalizedPartUriString = this.GetNormalizedPartUriString();
                    }
                    return this._normalizedPartUriString;
                }
            }

            internal string PartUriExtension
            {
                get
                {
                    if (this._partUriExtension == null)
                    {
                        this._partUriExtension = Path.GetExtension(this._partUriString);
                        if (this._partUriExtension.Length > 0)
                        {
                            this._partUriExtension = this._partUriExtension.Substring(1);
                        }
                    }
                    return this._partUriExtension;
                }
            }

            internal string PartUriString =>
                this._partUriString;
        }
    }
}

