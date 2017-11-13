namespace System.IO.Packaging
{
    using System;

    public enum VerifyResult
    {
        Success,
        InvalidSignature,
        CertificateRequired,
        InvalidCertificate,
        ReferenceNotFound,
        NotSigned
    }
}

