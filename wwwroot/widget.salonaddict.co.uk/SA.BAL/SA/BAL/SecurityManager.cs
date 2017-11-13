namespace SA.BAL
{
    using System;

    public class SecurityManager : ISecurityManager
    {
        public string DecryptUserPassword(string value, byte[] key)
        {
            byte[] buffer = Convert.FromBase64String(value);
            SymmetricUtility utility = new SymmetricUtility(key, SymmetricUtility.AlgorithmType.TripleDES);
            return utility.Decrypt(buffer);
        }

        public string EncryptUserPassword(string clearText, byte[] key)
        {
            SymmetricUtility utility = new SymmetricUtility(key, SymmetricUtility.AlgorithmType.TripleDES);
            return Convert.ToBase64String(utility.Encrypt(clearText));
        }

        public string GenerateHMAC(string value, byte[] key)
        {
            HMACUtility utility = new HMACUtility(key);
            return utility.ComputeHash(value);
        }

        public bool VerifyHMAC(string value, string signature, byte[] key)
        {
            HMACUtility utility = new HMACUtility(key);
            return (utility.ComputeHash(value) == signature);
        }
    }
}

