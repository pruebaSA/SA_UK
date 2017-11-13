namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("59f4c2a3-d3d7-4a31-b6e4-6ab3177c50b9")]
    internal interface IServiceTransactionConfig
    {
        void ConfigureTransaction(TransactionConfig transactionConfig);
        void IsolationLevel(int option);
        void TransactionTimeout(uint ulTimeoutSec);
        void BringYourOwnTransaction([MarshalAs(UnmanagedType.LPWStr)] string szTipURL);
        void NewTransactionDescription([MarshalAs(UnmanagedType.LPWStr)] string szTxDesc);
        void ConfigureBYOT(IntPtr pITxByot);
    }
}

