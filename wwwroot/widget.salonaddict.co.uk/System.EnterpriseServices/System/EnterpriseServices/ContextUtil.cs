namespace System.EnterpriseServices
{
    using System;
    using System.EnterpriseServices.Thunk;
    using System.Runtime.InteropServices;
    using System.Transactions;

    public sealed class ContextUtil
    {
        internal static readonly Guid GUID_JitActivationPolicy = new Guid("ecabaeb2-7f19-11d2-978e-0000f8757e2a");
        internal static readonly Guid GUID_TransactionProperty = new Guid("ecabaeb1-7f19-11d2-978e-0000f8757e2a");

        private ContextUtil()
        {
        }

        public static void DisableCommit()
        {
            Platform.Assert(Platform.MTS, "ContextUtil.DisableCommit");
            ContextThunk.DisableCommit();
        }

        public static void EnableCommit()
        {
            Platform.Assert(Platform.MTS, "ContextUtil.EnableCommit");
            ContextThunk.EnableCommit();
        }

        public static object GetNamedProperty(string name)
        {
            Platform.Assert(Platform.W2K, "ContextUtil.GetNamedProperty");
            return ((IGetContextProperties) ObjectContext).GetProperty(name);
        }

        public static bool IsCallerInRole(string role)
        {
            Platform.Assert(Platform.MTS, "ContextUtil.IsCallerInRole");
            return ((System.EnterpriseServices.IObjectContext) ObjectContext).IsCallerInRole(role);
        }

        public static bool IsDefaultContext() => 
            ContextThunk.IsDefaultContext();

        public static void SetAbort()
        {
            Platform.Assert(Platform.MTS, "ContextUtil.SetAbort");
            ContextThunk.SetAbort();
        }

        public static void SetComplete()
        {
            Platform.Assert(Platform.MTS, "ContextUtil.SetComplete");
            ContextThunk.SetComplete();
        }

        public static void SetNamedProperty(string name, object value)
        {
            Platform.Assert(Platform.W2K, "ContextUtil.SetNamedProperty");
            ((IContextProperties) ObjectContext).SetProperty(name, value);
        }

        public static Guid ActivityId
        {
            get
            {
                Platform.Assert(Platform.W2K, "ContextUtil.ActivityId");
                return ((System.EnterpriseServices.IObjectContextInfo) ObjectContext).GetActivityId();
            }
        }

        public static Guid ApplicationId
        {
            get
            {
                Platform.Assert(Platform.Whistler, "ContextUtil.ApplicationId");
                return ((IObjectContextInfo2) ObjectContext).GetApplicationId();
            }
        }

        public static Guid ApplicationInstanceId
        {
            get
            {
                Platform.Assert(Platform.Whistler, "ContextUtil.ApplicationInstanceId");
                return ((IObjectContextInfo2) ObjectContext).GetApplicationInstanceId();
            }
        }

        public static Guid ContextId
        {
            get
            {
                Platform.Assert(Platform.W2K, "ContextUtil.ContextId");
                return ((System.EnterpriseServices.IObjectContextInfo) ObjectContext).GetContextId();
            }
        }

        public static bool DeactivateOnReturn
        {
            get
            {
                Platform.Assert(Platform.W2K, "ContextUtil.DeactivateOnReturn");
                return ContextThunk.GetDeactivateOnReturn();
            }
            set
            {
                Platform.Assert(Platform.W2K, "ContextUtil.DeactivateOnReturn");
                ContextThunk.SetDeactivateOnReturn(value);
            }
        }

        public static bool IsInTransaction =>
            ContextThunk.IsInTransaction();

        public static bool IsSecurityEnabled
        {
            get
            {
                Platform.Assert(Platform.MTS, "ContextUtil.IsSecurityEnabled");
                try
                {
                    return ((System.EnterpriseServices.IObjectContext) ObjectContext).IsSecurityEnabled();
                }
                catch
                {
                    return false;
                }
            }
        }

        public static TransactionVote MyTransactionVote
        {
            get
            {
                Platform.Assert(Platform.W2K, "ContextUtil.MyTransactionVote");
                return (TransactionVote) ContextThunk.GetMyTransactionVote();
            }
            set
            {
                Platform.Assert(Platform.W2K, "ContextUtil.MyTransactionVote");
                ContextThunk.SetMyTransactionVote((int) value);
            }
        }

        internal static object ObjectContext
        {
            get
            {
                Platform.Assert(Platform.MTS, "ContextUtil.ObjectContext");
                System.EnterpriseServices.IObjectContext pCtx = null;
                int objectContext = Util.GetObjectContext(out pCtx);
                if (objectContext == 0)
                {
                    return pCtx;
                }
                if ((objectContext == Util.E_NOINTERFACE) || (objectContext == Util.CONTEXT_E_NOCONTEXT))
                {
                    throw new COMException(Resource.FormatString("Err_NoContext"), Util.CONTEXT_E_NOCONTEXT);
                }
                Marshal.ThrowExceptionForHR(objectContext);
                return null;
            }
        }

        public static Guid PartitionId
        {
            get
            {
                Platform.Assert(Platform.Whistler, "ContextUtil.PartitionId");
                return ((IObjectContextInfo2) ObjectContext).GetPartitionId();
            }
        }

        internal static object SafeObjectContext
        {
            get
            {
                Platform.Assert(Platform.MTS, "ContextUtil.ObjectContext");
                System.EnterpriseServices.IObjectContext pCtx = null;
                int objectContext = Util.GetObjectContext(out pCtx);
                if (objectContext == 0)
                {
                    return pCtx;
                }
                if ((objectContext != Util.E_NOINTERFACE) && (objectContext != Util.CONTEXT_E_NOCONTEXT))
                {
                    Marshal.ThrowExceptionForHR(objectContext);
                }
                return null;
            }
        }

        public static System.Transactions.Transaction SystemTransaction
        {
            get
            {
                Platform.Assert(Platform.W2K, "ContextUtil.SystemTransaction");
                object ppTx = null;
                TxInfo pTxInfo = new TxInfo();
                if (!ContextThunk.GetTransactionProxyOrTransaction(ref ppTx, pTxInfo))
                {
                    return null;
                }
                if (pTxInfo.isDtcTransaction)
                {
                    return TransactionInterop.GetTransactionFromDtcTransaction((IDtcTransaction) ppTx);
                }
                if (ppTx == null)
                {
                    TransactionProxy pTransactionProxy = new TransactionProxy((DtcIsolationLevel) pTxInfo.IsolationLevel, pTxInfo.timeout);
                    Guid guid = ContextThunk.RegisterTransactionProxy(pTransactionProxy);
                    pTransactionProxy.SetOwnerGuid(guid);
                    return pTransactionProxy.SystemTransaction;
                }
                TransactionProxy proxy2 = ppTx as TransactionProxy;
                if (proxy2 != null)
                {
                    return proxy2.SystemTransaction;
                }
                IDtcTransaction transaction = ContextThunk.GetTransaction() as IDtcTransaction;
                System.Transactions.Transaction transactionFromDtcTransaction = TransactionInterop.GetTransactionFromDtcTransaction(transaction);
                Marshal.ReleaseComObject(ppTx);
                return transactionFromDtcTransaction;
            }
        }

        public static object Transaction
        {
            get
            {
                Platform.Assert(Platform.W2K, "ContextUtil.Transaction");
                return ContextThunk.GetTransaction();
            }
        }

        public static Guid TransactionId
        {
            get
            {
                Platform.Assert(Platform.W2K, "ContextUtil.TransactionId");
                return ContextThunk.GetTransactionId();
            }
        }
    }
}

