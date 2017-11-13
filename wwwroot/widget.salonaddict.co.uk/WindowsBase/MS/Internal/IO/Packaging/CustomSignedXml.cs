namespace MS.Internal.IO.Packaging
{
    using Microsoft.Win32;
    using System;
    using System.Globalization;
    using System.Security;
    using System.Security.AccessControl;
    using System.Security.Cryptography.Xml;
    using System.Security.Permissions;
    using System.Threading;
    using System.Windows;
    using System.Xml;

    internal class CustomSignedXml : SignedXml
    {
        private const string _NetFxSecurityFullKeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\Security";
        private const string _NetFxSecurityKey = @"SOFTWARE\Microsoft\.NETFramework\Security";
        private const string _XAdESNameSpace = "http://uri.etsi.org/01903/v1.2.2#";
        private const string _XAdESTargetType = "http://uri.etsi.org/01903/v1.2.2#SignedProperties";
        private static bool? s_allowAmbiguousReferenceTarget = null;
        private static bool s_readRequireNCNameIdentifier = false;
        private static bool s_requireNCNameIdentifier = true;

        [SecurityTreatAsSafe, SecurityCritical]
        private static bool AllowAmbiguousReferenceTargets()
        {
            if (!s_allowAmbiguousReferenceTarget.HasValue)
            {
                bool flag = GetNetFxSecurityRegistryValue("SignedXmlAllowAmbiguousReferenceTargets", 0L) != 0L;
                s_allowAmbiguousReferenceTarget = new bool?(flag);
            }
            return s_allowAmbiguousReferenceTarget.Value;
        }

        public override XmlElement GetIdElement(XmlDocument document, string idValue)
        {
            XmlElement idElement = base.GetIdElement(document, idValue);
            if (idElement != null)
            {
                return idElement;
            }
            if (RequireNCNameIdentifier())
            {
                try
                {
                    XmlConvert.VerifyNCName(idValue);
                }
                catch (XmlException)
                {
                    return null;
                }
            }
            return SelectNodeByIdFromObjects(base.m_signature, idValue);
        }

        [SecurityCritical]
        private static long GetNetFxSecurityRegistryValue(string regValueName, long defaultValue)
        {
            new RegistryPermission(RegistryPermissionAccess.Read, AccessControlActions.View, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\Security").Assert();
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework\Security", false))
                {
                    if (key == null)
                    {
                        return defaultValue;
                    }
                    object obj2 = key.GetValue(regValueName);
                    if (obj2 == null)
                    {
                        return defaultValue;
                    }
                    RegistryValueKind valueKind = key.GetValueKind(regValueName);
                    if ((valueKind != RegistryValueKind.DWord) && (valueKind != RegistryValueKind.QWord))
                    {
                        return defaultValue;
                    }
                    return Convert.ToInt64(obj2, CultureInfo.InvariantCulture);
                }
            }
            catch (SecurityException)
            {
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
            return defaultValue;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private static bool RequireNCNameIdentifier()
        {
            if (!s_readRequireNCNameIdentifier)
            {
                bool flag = GetNetFxSecurityRegistryValue("SignedXmlRequireNCNameIdentifier", 1L) != 0L;
                s_requireNCNameIdentifier = flag;
                Thread.MemoryBarrier();
                s_readRequireNCNameIdentifier = true;
            }
            return s_requireNCNameIdentifier;
        }

        private static XmlElement SelectNodeByIdFromObjects(System.Security.Cryptography.Xml.Signature signature, string idValue)
        {
            XmlElement xml = null;
            foreach (DataObject obj2 in signature.ObjectList)
            {
                if (string.CompareOrdinal(idValue, obj2.Id) == 0)
                {
                    if (xml != null)
                    {
                        throw new XmlException(System.Windows.SR.Get("DuplicateObjectId"));
                    }
                    xml = obj2.GetXml();
                }
            }
            if (xml == null)
            {
                xml = SelectSubObjectNodeForXAdES(signature, idValue);
            }
            return xml;
        }

        private static XmlElement SelectSubObjectNodeForXAdES(System.Security.Cryptography.Xml.Signature signature, string idValue)
        {
            foreach (Reference reference in signature.SignedInfo.References)
            {
                if (((string.CompareOrdinal(reference.Type, "http://uri.etsi.org/01903/v1.2.2#SignedProperties") == 0) && (reference.Uri.Length > 0)) && ((reference.Uri[0] == '#') && (string.CompareOrdinal(reference.Uri.Substring(1), idValue) == 0)))
                {
                    return SelectSubObjectNodeForXAdESInDataObjects(signature, idValue);
                }
            }
            return null;
        }

        private static XmlElement SelectSubObjectNodeForXAdESInDataObjects(System.Security.Cryptography.Xml.Signature signature, string idValue)
        {
            XmlElement element = null;
            bool flag = false;
            foreach (DataObject obj2 in signature.ObjectList)
            {
                if (string.CompareOrdinal(obj2.Id, XTable.Get(XTable.ID.OpcAttrValue)) != 0)
                {
                    XmlNodeList list = obj2.GetXml().SelectNodes(".//*[@Id='" + idValue + "']");
                    if (list.Count > 0)
                    {
                        if (!AllowAmbiguousReferenceTargets() && ((list.Count > 1) || flag))
                        {
                            throw new XmlException(System.Windows.SR.Get("DuplicateObjectId"));
                        }
                        flag = true;
                        XmlNode node = list[0] as XmlElement;
                        if (node != null)
                        {
                            XmlNode parentNode = node;
                            while ((parentNode != null) && (parentNode.NamespaceURI.Length == 0))
                            {
                                parentNode = parentNode.ParentNode;
                            }
                            if ((parentNode != null) && (string.CompareOrdinal(parentNode.NamespaceURI, "http://uri.etsi.org/01903/v1.2.2#") == 0))
                            {
                                element = node as XmlElement;
                            }
                        }
                    }
                }
            }
            return element;
        }
    }
}

