namespace MS.Internal.IO.Packaging
{
    using MS.Internal;
    using System;
    using System.IO;
    using System.IO.Packaging;
    using System.Security.Cryptography.X509Certificates;
    using System.Windows;

    internal class CertificatePart
    {
        private X509Certificate2 _certificate;
        private static readonly MS.Internal.ContentType _certificatePartContentType = new MS.Internal.ContentType("application/vnd.openxmlformats-package.digital-signature-certificate");
        private static readonly string _certificatePartNameExtension = ".cer";
        private static readonly string _certificatePartNamePrefix = "/package/services/digital-signature/certificate/";
        private static readonly string _certificatePartRelationshipType = "http://schemas.openxmlformats.org/package/2006/relationships/digital-signature/certificate";
        private static long _maximumCertificateStreamLength = 0x40000L;
        private PackagePart _part;

        internal CertificatePart(Package container, System.Uri partName)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            if (partName == null)
            {
                throw new ArgumentNullException("partName");
            }
            partName = PackUriHelper.ValidatePartUri(partName);
            if (container.PartExists(partName))
            {
                this._part = container.GetPart(partName);
                if (!this._part.ValidatedContentType.AreTypeAndSubTypeEqual(_certificatePartContentType))
                {
                    throw new FileFormatException(System.Windows.SR.Get("CertificatePartContentTypeMismatch"));
                }
            }
            else
            {
                this._part = container.CreatePart(partName, _certificatePartContentType.ToString());
            }
        }

        internal X509Certificate2 GetCertificate()
        {
            if (this._certificate == null)
            {
                using (Stream stream = this._part.GetStream())
                {
                    if (stream.Length > 0L)
                    {
                        if (stream.Length > _maximumCertificateStreamLength)
                        {
                            throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                        }
                        byte[] buffer = new byte[stream.Length];
                        PackagingUtilities.ReliableRead(stream, buffer, 0, (int) stream.Length);
                        this._certificate = new X509Certificate2(buffer);
                    }
                }
            }
            return this._certificate;
        }

        internal void SetCertificate(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }
            this._certificate = certificate;
            byte[] rawCertData = this._certificate.GetRawCertData();
            using (Stream stream = this._part.GetStream(FileMode.Create, FileAccess.Write))
            {
                stream.Write(rawCertData, 0, rawCertData.Length);
            }
        }

        internal static MS.Internal.ContentType ContentType =>
            _certificatePartContentType;

        internal static string PartNameExtension =>
            _certificatePartNameExtension;

        internal static string PartNamePrefix =>
            _certificatePartNamePrefix;

        internal static string RelationshipType =>
            _certificatePartRelationshipType;

        internal System.Uri Uri =>
            this._part.Uri;
    }
}

