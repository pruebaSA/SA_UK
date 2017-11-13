namespace SA.BAL
{
    using System;

    public interface ISecurityManager
    {
        string DecryptUserPassword(string value, byte[] key);
        string EncryptUserPassword(string clearText, byte[] key);
        string GenerateHMAC(string value, byte[] key);
        bool VerifyHMAC(string value, string signature, byte[] key);
    }
}

