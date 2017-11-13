namespace System.IdentityModel.Selectors
{
    using Microsoft.InfoCards;
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("infocardapi.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        public static extern int Decrypt(InternalRefCountedHandle nativeCryptoHandle, bool fOAEP, [MarshalAs(UnmanagedType.U4)] int cbInData, SafeHandle pInData, [MarshalAs(UnmanagedType.U4)] out int pcbOutData, out GlobalAllocSafeHandle pOutData);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("infocardapi.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        public static extern int Encrypt(InternalRefCountedHandle nativeCryptoHandle, bool fOAEP, [MarshalAs(UnmanagedType.U4)] int cbInData, SafeHandle pInData, [MarshalAs(UnmanagedType.U4)] out int pcbOutData, out GlobalAllocSafeHandle pOutData);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("infocardapi.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        public static extern int GenerateDerivedKey(InternalRefCountedHandle nativeCryptoHandle, int cbLabel, SafeHandle pLabel, int cbNonce, SafeHandle pNonce, int derivedKeyLength, int offset, [MarshalAs(UnmanagedType.LPWStr)] string derivationAlgUri, out int cbDerivedKey, out GlobalAllocSafeHandle pDerivedKey);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("infocardapi.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        public static extern int GetCryptoTransform(InternalRefCountedHandle nativeCryptoHandle, int mode, int padding, int feedbackSize, int direction, int cbIV, SafeHandle pIV, out InternalRefCountedHandle nativeTransformHandle);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("infocardapi.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        public static extern int GetKeyedHash(InternalRefCountedHandle nativeCryptoHandle, out InternalRefCountedHandle nativeHashHandle);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("infocardapi.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        public static extern int GetToken(int cPolicyChain, SafeHandle pPolicyChain, out SafeTokenHandle securityToken, out InternalRefCountedHandle pCryptoHandle);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("infocardapi.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        public static extern int HashCore(InternalRefCountedHandle nativeCryptoHandle, int cbInData, SafeHandle pInData);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("infocardapi.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        public static extern int HashFinal(InternalRefCountedHandle nativeCryptoHandle, int cbInData, SafeHandle pInData, out int cbOutData, out GlobalAllocSafeHandle pOutData);
        [DllImport("infocardapi.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        public static extern int ImportInformationCard([MarshalAs(UnmanagedType.LPWStr)] string nativeFileName);
        [DllImport("infocardapi.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        public static extern int ManageCardSpace();
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("infocardapi.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        public static extern int SignHash(InternalRefCountedHandle nativeCryptoHandle, [MarshalAs(UnmanagedType.U4)] int cbHash, SafeHandle pInData, SafeHandle pHashAlgOid, [MarshalAs(UnmanagedType.U4)] out int pcbSig, out GlobalAllocSafeHandle pSig);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("infocardapi.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        public static extern int TransformBlock(InternalRefCountedHandle nativeCryptoHandle, int cbInData, SafeHandle pInData, out int cbOutData, out GlobalAllocSafeHandle pOutData);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("infocardapi.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        public static extern int TransformFinalBlock(InternalRefCountedHandle nativeCryptoHandle, int cbInData, SafeHandle pInData, out int cbOutData, out GlobalAllocSafeHandle pOutData);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("infocardapi.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        public static extern int VerifyHash(InternalRefCountedHandle nativeCryptoHandle, [MarshalAs(UnmanagedType.U4)] int cbHash, SafeHandle pInData, SafeHandle pHashAlgOid, [MarshalAs(UnmanagedType.U4)] int pcbSig, SafeHandle pSig, out bool verified);
    }
}

