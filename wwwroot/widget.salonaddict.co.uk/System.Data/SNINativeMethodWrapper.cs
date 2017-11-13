using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

[CLSCompliant(false)]
internal class SNINativeMethodWrapper
{
    internal static int SNI_LocalDBErrorCode = 50;
    internal static int modopt(IsConst) SniMaxComposedSpnLength = SNI_MAX_COMPOSED_SPN;

    internal static unsafe byte[] GetData()
    {
        byte[] destination = null;
        int num;
        IntPtr source = (IntPtr) SqlDependencyProcessDispatcherStorage.NativeGetData(&num);
        if (source != IntPtr.Zero)
        {
            destination = new byte[num];
            Marshal.Copy(source, destination, 0, num);
        }
        return destination;
    }

    internal static _AppDomain GetDefaultAppDomain()
    {
        IntPtr pUnk = (IntPtr) SqlDependencyProcessDispatcherStorage.NativeGetDefaultAppDomain();
        Marshal.Release(pUnk);
        return (Marshal.GetObjectForIUnknown(pUnk) as _AppDomain);
    }

    private static unsafe void MarshalConsumerInfo(ConsumerInfo consumerInfo, SNI_CONSUMER_INFO* modopt(IsImplicitlyDereferenced) native_consumerInfo)
    {
        void* voidPtr;
        void* voidPtr2;
        native_consumerInfo[0] = (SNI_CONSUMER_INFO* modopt(IsImplicitlyDereferenced)) consumerInfo.defaultBufferSize;
        if (null == consumerInfo.readDelegate)
        {
            voidPtr2 = null;
        }
        else
        {
            voidPtr2 = Marshal.GetFunctionPointerForDelegate(consumerInfo.readDelegate).ToPointer();
        }
        native_consumerInfo[8] = (SNI_CONSUMER_INFO* modopt(IsImplicitlyDereferenced)) voidPtr2;
        if (null == consumerInfo.writeDelegate)
        {
            voidPtr = null;
        }
        else
        {
            voidPtr = Marshal.GetFunctionPointerForDelegate(consumerInfo.writeDelegate).ToPointer();
        }
        native_consumerInfo[12] = (SNI_CONSUMER_INFO* modopt(IsImplicitlyDereferenced)) voidPtr;
        native_consumerInfo[4] = (SNI_CONSUMER_INFO* modopt(IsImplicitlyDereferenced)) consumerInfo.key.ToPointer();
    }

    internal static unsafe void SetData(byte[] data)
    {
        ref byte modopt(IsExplicitlyDereferenced) pinned numRef = (ref byte modopt(IsExplicitlyDereferenced)) &(data[0]);
        SqlDependencyProcessDispatcherStorage.NativeSetData(numRef, data.Length);
    }

    internal static unsafe uint SNIAddProvider(SafeHandle pConn, ProviderEnum providerEnum, ref uint info)
    {
        uint num;
        uint num2 = info;
        bool success = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            pConn.DangerousAddRef(ref success);
            Debug.Assert(success, "AddRef Failed!");
            num = SNIAddProvider((SNI_Conn*) pConn.DangerousGetHandle().ToPointer(), (ProviderNum) providerEnum, (void*) &num2);
        }
        finally
        {
            if (success)
            {
                pConn.DangerousRelease();
            }
        }
        info = num2;
        return num;
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static unsafe uint SNIClose(IntPtr pConn)
    {
        uint modopt(IsLong) modopt(CallConvStdcall) *(SNI_Conn*) local = (uint modopt(IsLong) modopt(CallConvStdcall) *(SNI_Conn*)) __unep@?SNIClose@@$$J14YGKPAVSNI_Conn@@@Z;
        return *local(pConn.ToPointer());
    }

    internal static unsafe void SNIGetLastError(SNI_Error error)
    {
        SNI_ERROR sni_error;
        SNIGetLastError(&sni_error);
        error.provider = *((ProviderEnum*) &sni_error);
        error.errorMessage = new char[0x20a];
        int index = 0;
        do
        {
            error.errorMessage[index] = *((char*) ((index * 2) + (&sni_error + 4)));
            index++;
        }
        while (index < 0x105);
        error.nativeError = *((uint*) (&sni_error + 0x210));
        error.sniError = *((uint*) (&sni_error + 0x214));
        IntPtr ptr2 = (IntPtr) *(((int*) (&sni_error + 0x218)));
        error.fileName = Marshal.PtrToStringUni(ptr2);
        IntPtr ptr = (IntPtr) *(((int*) (&sni_error + 540)));
        error.function = Marshal.PtrToStringUni(ptr);
        error.lineNumber = *((uint*) (&sni_error + 0x220));
    }

    internal static uint SNIInitialize() => 
        SNIInitialize(null);

    internal static unsafe uint SNIOpen(ConsumerInfo consumerInfo, string constring, SafeHandle parent, ref IntPtr pConn, [MarshalAs(UnmanagedType.U1)] bool fSync)
    {
        SNI_CONSUMER_INFO sni_consumer_info;
        uint num = 0;
        MarshalConsumerInfo(consumerInfo, &sni_consumer_info);
        SNI_Conn* connPtr = null;
        ref byte modopt(IsExplicitlyDereferenced) pinned numRef = (ref byte modopt(IsExplicitlyDereferenced)) &(Encoding.ASCII.GetBytes(constring)[0]);
        bool success = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            parent.DangerousAddRef(ref success);
            Debug.Assert(success, "AddRef Failed!");
            num = SNIOpen(&sni_consumer_info, numRef, parent.DangerousGetHandle().ToPointer(), &connPtr, (int) fSync);
        }
        finally
        {
            if (success)
            {
                parent.DangerousRelease();
            }
        }
        IntPtr ptr = (IntPtr) connPtr;
        pConn = ptr;
        return num;
    }

    internal static unsafe uint SNIOpenSyncEx(ConsumerInfo consumerInfo, string constring, ref IntPtr pConn, byte[] spnBuffer, byte[] instanceName, [MarshalAs(UnmanagedType.U1)] bool fOverrideCache, [MarshalAs(UnmanagedType.U1)] bool fSync, int timeout, [MarshalAs(UnmanagedType.U1)] bool fParallel)
    {
        SNI_CLIENT_CONSUMER_INFO sni_client_consumer_info;
        SNI_CLIENT_CONSUMER_INFO.{ctor}(&sni_client_consumer_info);
        ref ushort modopt(IsConst) modopt(IsExplicitlyDereferenced) pinned numRef3 = PtrToStringChars(constring);
        byte num = (byte) (null == pConn.ToPointer());
        Debug.Assert((bool) num, "Verrifying variable is really not initallized.");
        SNI_Conn* connPtr = null;
        ref byte modopt(IsExplicitlyDereferenced) pinned numRef2 = (spnBuffer != null) ? ((ref byte modopt(IsExplicitlyDereferenced)) &(spnBuffer[0])) : ((ref byte modopt(IsExplicitlyDereferenced)) 0);
        ref byte modopt(IsExplicitlyDereferenced) pinned numRef = (ref byte modopt(IsExplicitlyDereferenced)) &(instanceName[0]);
        MarshalConsumerInfo(consumerInfo, (SNI_CONSUMER_INFO* modopt(IsImplicitlyDereferenced)) &sni_client_consumer_info);
        *((int*) (&sni_client_consumer_info + 0x20)) = numRef3;
        *((int*) (&sni_client_consumer_info + 0x24)) = 0;
        if (spnBuffer != null)
        {
            *((int*) (&sni_client_consumer_info + 40)) = numRef2;
            *((int*) (&sni_client_consumer_info + 0x2c)) = spnBuffer.Length;
        }
        *((int*) (&sni_client_consumer_info + 0x30)) = numRef;
        *((int*) (&sni_client_consumer_info + 0x34)) = instanceName.Length;
        *((int*) (&sni_client_consumer_info + 0x38)) = fOverrideCache;
        *((int*) (&sni_client_consumer_info + 60)) = fSync;
        *((int*) (&sni_client_consumer_info + 0x40)) = timeout;
        *((int*) (&sni_client_consumer_info + 0x44)) = fParallel;
        IntPtr ptr = (IntPtr) connPtr;
        pConn = ptr;
        return SNIOpenSyncEx(&sni_client_consumer_info, &connPtr);
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    internal static unsafe void SNIPacketAllocate(SafeHandle pConn, IOType ioType, ref IntPtr ret)
    {
        bool success = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            pConn.DangerousAddRef(ref success);
            Debug.Assert(success, "AddRef Failed!");
            SNI_Conn* connPtr = (SNI_Conn*) pConn.DangerousGetHandle().ToPointer();
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                SNI_Packet* modopt(CallConvStdcall) *(SNI_Conn*, uint modopt(IsLong)) local = (SNI_Packet* modopt(CallConvStdcall) *(SNI_Conn*, uint modopt(IsLong))) __unep@?SNIPacketAllocate@@$$J18YGPAVSNI_Packet@@PAVSNI_Conn@@K@Z;
                IntPtr ptr = (IntPtr) *local(connPtr, ioType);
                ret = ptr;
            }
        }
        finally
        {
            if (success)
            {
                pConn.DangerousRelease();
            }
        }
    }

    internal static unsafe IntPtr SNIPacketGetConnection(IntPtr packet) => 
        ((IntPtr) SNIPacketGetConnection((SNI_Packet*) packet.ToPointer()));

    internal static unsafe void SNIPacketGetData(IntPtr packet, ref IntPtr data, ref uint dataSize)
    {
        ref SNI_Packet modopt(IsExplicitlyDereferenced) pinned packetRef = (ref SNI_Packet modopt(IsExplicitlyDereferenced)) packet.ToPointer();
        byte* numPtr = null;
        uint num = 0;
        SNIPacketGetData(packetRef, &numPtr, (uint modopt(IsLong)*) &num);
        IntPtr ptr = (IntPtr) numPtr;
        data = ptr;
        dataSize = num;
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static unsafe void SNIPacketRelease(IntPtr packet)
    {
        ref SNI_Packet modopt(IsExplicitlyDereferenced) pinned packetRef = (ref SNI_Packet modopt(IsExplicitlyDereferenced)) packet.ToPointer();
        void modopt(CallConvStdcall) *(SNI_Packet*) local = (void modopt(CallConvStdcall) *(SNI_Packet*)) __unep@?SNIPacketRelease@@$$J14YGXPAVSNI_Packet@@@Z;
        *local(packetRef);
    }

    internal static unsafe void SNIPacketReset(SafeHandle pConn, IOType ioType, SafeHandle packet)
    {
        bool success = false;
        bool flag = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            pConn.DangerousAddRef(ref success);
            Debug.Assert(success, "AddRef Failed!");
            packet.DangerousAddRef(ref flag);
            Debug.Assert(flag, "AddRef Failed!");
            SNI_Packet* packetPtr = (SNI_Packet*) packet.DangerousGetHandle().ToPointer();
            SNIPacketReset((SNI_Conn*) pConn.DangerousGetHandle().ToPointer(), (uint modopt(IsLong)) ioType, packetPtr);
        }
        finally
        {
            if (success)
            {
                pConn.DangerousRelease();
            }
            if (flag)
            {
                packet.DangerousRelease();
            }
        }
    }

    internal static unsafe void SNIPacketSetData(SafeHandle packet, byte[] data, int length)
    {
        ref byte modopt(IsConst) modopt(IsExplicitlyDereferenced) pinned numRef = (ref byte modopt(IsConst) modopt(IsExplicitlyDereferenced)) &(data[0]);
        RuntimeHelpers.PrepareConstrainedRegions();
        bool success = false;
        try
        {
            packet.DangerousAddRef(ref success);
            Debug.Assert(success, "AddRef Failed!");
            SNIPacketSetData((SNI_Packet*) packet.DangerousGetHandle().ToPointer(), numRef, (uint modopt(IsLong)) length);
        }
        finally
        {
            if (success)
            {
                packet.DangerousRelease();
            }
        }
    }

    [ResourceExposure(ResourceScope.None), ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)]
    internal static int SNIQueryInfo(QTypes qType, ref IntPtr qInfo)
    {
        byte num = (byte) (qType == QTypes.SNI_QUERY_LOCALDB_HMODULE);
        Debug.Assert((bool) num, "qType is unsupported or unknown");
        ref IntPtr modopt(IsExplicitlyDereferenced) pinned ptrRef = qInfo;
        return SNIQueryInfo((uint) qType, ptrRef);
    }

    internal static unsafe int SNIQueryInfo(QTypes qType, ref uint qInfo)
    {
        uint num = qInfo;
        qInfo = num;
        return SNIQueryInfo((uint) qType, (void*) &num);
    }

    internal static unsafe uint SNIReadAsync(SafeHandle pConn, ref IntPtr packet)
    {
        uint num;
        SNI_Packet* packetPtr = null;
        bool success = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            pConn.DangerousAddRef(ref success);
            Debug.Assert(success, "AddRef Failed!");
            num = SNIReadAsync((SNI_Conn*) pConn.DangerousGetHandle().ToPointer(), &packetPtr, null);
        }
        finally
        {
            if (success)
            {
                pConn.DangerousRelease();
            }
        }
        IntPtr ptr = (IntPtr) packetPtr;
        packet = ptr;
        return num;
    }

    internal static unsafe uint SNIReadSync(SafeHandle pConn, ref IntPtr packet, int timeout)
    {
        uint num;
        SNI_Packet* packetPtr = null;
        bool success = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            pConn.DangerousAddRef(ref success);
            Debug.Assert(success, "AddRef Failed!");
            num = SNIReadSync((SNI_Conn*) pConn.DangerousGetHandle().ToPointer(), &packetPtr, timeout);
        }
        finally
        {
            if (success)
            {
                pConn.DangerousRelease();
            }
        }
        IntPtr ptr = (IntPtr) packetPtr;
        packet = ptr;
        return num;
    }

    internal static unsafe uint SNIRemoveProvider(SafeHandle pConn, ProviderEnum providerEnum)
    {
        uint num;
        bool success = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            pConn.DangerousAddRef(ref success);
            Debug.Assert(success, "AddRef Failed!");
            num = SNIRemoveProvider((SNI_Conn*) pConn.DangerousGetHandle().ToPointer(), (ProviderNum) providerEnum);
        }
        finally
        {
            if (success)
            {
                pConn.DangerousRelease();
            }
        }
        return num;
    }

    internal static unsafe uint SNISecGenClientContext(SafeHandle pConnectionObject, byte[] inBuff, uint receivedLength, byte[] OutBuff, ref uint sendLength, byte[] serverUserName)
    {
        uint num2;
        uint num3 = sendLength;
        ref byte modopt(IsExplicitlyDereferenced) pinned numRef3 = (inBuff != null) ? ((ref byte modopt(IsExplicitlyDereferenced)) &(inBuff[0])) : ((ref byte modopt(IsExplicitlyDereferenced)) 0);
        ref byte modopt(IsExplicitlyDereferenced) pinned numRef2 = (ref byte modopt(IsExplicitlyDereferenced)) &(OutBuff[0]);
        ref byte modopt(IsExplicitlyDereferenced) pinned numRef = (ref byte modopt(IsExplicitlyDereferenced)) &(serverUserName[0]);
        bool success = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            int length;
            int num4;
            pConnectionObject.DangerousAddRef(ref success);
            Debug.Assert(success, "AddRef Failed!");
            if (serverUserName == null)
            {
                length = 0;
            }
            else
            {
                length = serverUserName.Length;
            }
            num2 = SNISecGenClientContext((SNI_Conn*) pConnectionObject.DangerousGetHandle().ToPointer(), numRef3, receivedLength, numRef2, (uint modopt(IsLong)*) &num3, &num4, numRef, (uint modopt(IsLong)) length, null, null);
        }
        finally
        {
            if (success)
            {
                pConnectionObject.DangerousRelease();
            }
        }
        sendLength = num3;
        return num2;
    }

    internal static unsafe uint SNISecInitPackage(ref uint maxLength)
    {
        uint num = maxLength;
        maxLength = num;
        return SNISecInitPackage((uint modopt(IsLong)*) &num);
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static unsafe void SNIServerEnumClose(IntPtr handle)
    {
        void modopt(CallConvStdcall) *(void*) local = (void modopt(CallConvStdcall) *(void*)) __unep@?SNIServerEnumClose@@$$J14YGXPAX@Z;
        *local(handle.ToPointer());
    }

    internal static IntPtr SNIServerEnumOpen() => 
        new IntPtr(SNIServerEnumOpen(null, 1));

    internal static unsafe int SNIServerEnumRead(IntPtr handle, char[] wStr, int pcbBuf, ref bool fMore)
    {
        ref ushort modopt(IsExplicitlyDereferenced) pinned numRef = (ref ushort modopt(IsExplicitlyDereferenced)) &(wStr[0]);
        int num2 = (int) fMore;
        byte num = (num2 != 0) ? ((byte) 1) : ((byte) 0);
        fMore = (bool) num;
        return SNIServerEnumRead(handle.ToPointer(), numRef, pcbBuf, &num2);
    }

    internal static unsafe uint SNISetInfo(SafeHandle pConn, QTypes qtype, ref uint qInfo)
    {
        uint num;
        uint num2 = qInfo;
        bool success = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            pConn.DangerousAddRef(ref success);
            Debug.Assert(success, "AddRef Failed!");
            num = SNISetInfo((SNI_Conn*) pConn.DangerousGetHandle().ToPointer(), (uint) qtype, (void*) &num2);
        }
        finally
        {
            if (success)
            {
                pConn.DangerousRelease();
            }
        }
        qInfo = num2;
        return num;
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static unsafe uint SNITerminate() => 
        ((uint) *__unep@?SNITerminate@@$$J10YGKXZ());

    [ResourceExposure(ResourceScope.None), ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)]
    internal static unsafe uint SNIWaitForSSLHandshakeToComplete(SafeHandle pConn, int timeoutMilliseconds)
    {
        uint num;
        bool success = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            pConn.DangerousAddRef(ref success);
            Debug.Assert(success, "AddRef Failed!");
            num = SNIWaitForSSLHandshakeToComplete((SNI_Conn*) pConn.DangerousGetHandle().ToPointer(), (uint modopt(IsLong)) timeoutMilliseconds);
        }
        finally
        {
            if (success)
            {
                pConn.DangerousRelease();
            }
        }
        return num;
    }

    internal static unsafe uint SNIWriteAsync(SafeHandle pConn, SafeHandle packet)
    {
        uint num;
        bool success = false;
        bool flag = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            pConn.DangerousAddRef(ref success);
            Debug.Assert(success, "AddRef Failed!");
            packet.DangerousAddRef(ref flag);
            Debug.Assert(flag, "AddRef Failed!");
            num = SNIWriteAsync((SNI_Conn*) pConn.DangerousGetHandle().ToPointer(), (SNI_Packet*) packet.DangerousGetHandle().ToPointer(), null);
        }
        finally
        {
            if (success)
            {
                pConn.DangerousRelease();
            }
            if (flag)
            {
                packet.DangerousRelease();
            }
        }
        return num;
    }

    internal static unsafe uint SNIWriteSync(SafeHandle pConn, SafeHandle packet)
    {
        uint num;
        bool success = false;
        bool flag = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            pConn.DangerousAddRef(ref success);
            Debug.Assert(success, "AddRef Failed!");
            packet.DangerousAddRef(ref flag);
            Debug.Assert(flag, "AddRef Failed!");
            num = SNIWriteSync((SNI_Conn*) pConn.DangerousGetHandle().ToPointer(), (SNI_Packet*) packet.DangerousGetHandle().ToPointer(), null);
        }
        finally
        {
            if (success)
            {
                pConn.DangerousRelease();
            }
            if (flag)
            {
                packet.DangerousRelease();
            }
        }
        return num;
    }

    [CLSCompliant(false)]
    internal class ConsumerInfo
    {
        internal int defaultBufferSize;
        internal IntPtr key;
        internal SNINativeMethodWrapper.SqlAsyncCallbackDelegate readDelegate;
        internal SNINativeMethodWrapper.SqlAsyncCallbackDelegate writeDelegate;
    }

    internal enum IOType
    {
        READ,
        WRITE
    }

    internal enum ProviderEnum
    {
        HTTP_PROV,
        NP_PROV,
        SESSION_PROV,
        SIGN_PROV,
        SM_PROV,
        SMUX_PROV,
        SSL_PROV,
        TCP_PROV,
        VIA_PROV,
        MAX_PROVS,
        INVALID_PROV
    }

    internal enum QTypes
    {
        SNI_QUERY_CERTIFICATE = 5,
        SNI_QUERY_CLIENT_ENCRYPT_POSSIBLE = 3,
        SNI_QUERY_CONN_BUFSIZE = 1,
        SNI_QUERY_CONN_CONNID = 8,
        SNI_QUERY_CONN_ENCRYPT = 6,
        SNI_QUERY_CONN_INFO = 0,
        SNI_QUERY_CONN_KEY = 2,
        SNI_QUERY_CONN_PARENTCONNID = 9,
        SNI_QUERY_CONN_PROVIDERNUM = 7,
        SNI_QUERY_CONN_SECPKG = 10,
        SNI_QUERY_LOCALDB_HMODULE = 0x18,
        SNI_QUERY_SERVER_ENCRYPT_POSSIBLE = 4
    }

    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode), CLSCompliant(false)]
    internal class SNI_Error
    {
        internal SNINativeMethodWrapper.ProviderEnum provider;
        internal char[] errorMessage;
        internal uint nativeError;
        internal uint sniError;
        internal string fileName;
        internal string function;
        internal uint lineNumber;
    }

    internal delegate void SqlAsyncCallbackDelegate(IntPtr ptr1, IntPtr ptr2, uint num);
}

