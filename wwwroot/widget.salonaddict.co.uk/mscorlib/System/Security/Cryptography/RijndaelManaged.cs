namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public sealed class RijndaelManaged : Rijndael
    {
        public RijndaelManaged()
        {
            if (Utils.FipsAlgorithmPolicy == 1)
            {
                throw new InvalidOperationException(Environment.GetResourceString("Cryptography_NonCompliantFIPSAlgorithm"));
            }
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV) => 
            this.NewEncryptor(rgbKey, base.ModeValue, rgbIV, base.FeedbackSizeValue, RijndaelManagedTransformMode.Decrypt);

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV) => 
            this.NewEncryptor(rgbKey, base.ModeValue, rgbIV, base.FeedbackSizeValue, RijndaelManagedTransformMode.Encrypt);

        public override void GenerateIV()
        {
            base.IVValue = new byte[base.BlockSizeValue / 8];
            Utils.StaticRandomNumberGenerator.GetBytes(base.IVValue);
        }

        public override void GenerateKey()
        {
            base.KeyValue = new byte[base.KeySizeValue / 8];
            Utils.StaticRandomNumberGenerator.GetBytes(base.KeyValue);
        }

        private ICryptoTransform NewEncryptor(byte[] rgbKey, CipherMode mode, byte[] rgbIV, int feedbackSize, RijndaelManagedTransformMode encryptMode)
        {
            if (rgbKey == null)
            {
                rgbKey = new byte[base.KeySizeValue / 8];
                Utils.StaticRandomNumberGenerator.GetBytes(rgbKey);
            }
            if ((mode != CipherMode.ECB) && (rgbIV == null))
            {
                rgbIV = new byte[base.BlockSizeValue / 8];
                Utils.StaticRandomNumberGenerator.GetBytes(rgbIV);
            }
            return new RijndaelManagedTransform(rgbKey, mode, rgbIV, base.BlockSizeValue, feedbackSize, base.PaddingValue, encryptMode);
        }
    }
}

