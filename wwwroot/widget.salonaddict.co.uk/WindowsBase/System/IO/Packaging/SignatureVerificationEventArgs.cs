namespace System.IO.Packaging
{
    using System;

    public class SignatureVerificationEventArgs : EventArgs
    {
        private System.IO.Packaging.VerifyResult _result;
        private PackageDigitalSignature _signature;

        internal SignatureVerificationEventArgs(PackageDigitalSignature signature, System.IO.Packaging.VerifyResult result)
        {
            if (signature == null)
            {
                throw new ArgumentNullException("signature");
            }
            if ((result < System.IO.Packaging.VerifyResult.Success) || (result > System.IO.Packaging.VerifyResult.NotSigned))
            {
                throw new ArgumentOutOfRangeException("result");
            }
            this._signature = signature;
            this._result = result;
        }

        public PackageDigitalSignature Signature =>
            this._signature;

        public System.IO.Packaging.VerifyResult VerifyResult =>
            this._result;
    }
}

