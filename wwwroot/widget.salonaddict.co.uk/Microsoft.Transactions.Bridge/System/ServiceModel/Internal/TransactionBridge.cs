namespace System.ServiceModel.Internal
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Bridge.Configuration;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.Threading;

    [Guid("eec5dcca-05dc-4b46-8af7-2881c1635aea"), ComVisible(true), ProgId("")]
    public class TransactionBridge : ITransactionBridge
    {
        private object bridgeConfig;
        private TransactionBridgeSection config;
        private List<TransactionManager> transactionManagers;

        public TransactionBridge()
        {
            PropagationProtocolsTracing.TraceVerbose("Transaction Bridge Created");
            this.transactionManagers = new List<TransactionManager>();
        }

        public void Init(object bridgeConfig)
        {
            this.bridgeConfig = bridgeConfig;
            PropagationProtocolsTracing.TraceVerbose("Initializing...");
            try
            {
                this.config = TransactionBridgeSection.GetSection();
            }
            catch (Exception exception)
            {
                PropagationProtocolsTracing.TraceError("Error reading configuration: " + exception);
                throw;
            }
            this.config.Protocols.AssertBothWsatProtocolVersions();
            try
            {
                PropagationProtocolsTracing.TraceVerbose("Reading transaction managers from configuration...");
                if (!IsAssemblyMicrosoftSigned(this.config.TransactionManagerType))
                {
                    PropagationProtocolsTracing.TraceVerbose("Transaction manager type has wrong signature: " + this.config.TransactionManagerType);
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(Microsoft.Transactions.SR.GetString("TransactionManagerTypeWrongSignature")));
                }
                PropagationProtocolsTracing.TraceVerbose("Loading transaction manager " + this.config.TransactionManagerType);
                Type type = Type.GetType(this.config.TransactionManagerType);
                if (type == null)
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(Microsoft.Transactions.SR.GetString("CouldNotLoadTM", new object[] { this.config.TransactionManagerType })));
                }
                PropagationProtocolsTracing.TraceVerbose("Initializing transaction managers...");
                try
                {
                    foreach (ProtocolElement element in this.config.Protocols)
                    {
                        TransactionManager item = (TransactionManager) Activator.CreateInstance(type);
                        item.Initialize(element.Type, this.bridgeConfig);
                        this.transactionManagers.Add(item);
                    }
                }
                catch
                {
                    this.transactionManagers.Clear();
                    throw;
                }
            }
            catch (Exception exception2)
            {
                System.ServiceModel.DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Error);
                PropagationProtocolsTracing.TraceError("Error initializing: " + exception2);
                throw;
            }
        }

        internal static bool IsAssemblyMicrosoftSigned(string assemblyQualifiedTypeName)
        {
            string[] strArray = assemblyQualifiedTypeName.Split(new char[] { ',' }, 2);
            if (strArray.Length == 2)
            {
                byte[] publicKeyToken = new AssemblyName(strArray[1].Trim()).GetPublicKeyToken();
                if (publicKeyToken != null)
                {
                    string strA = string.Empty;
                    foreach (byte num in publicKeyToken)
                    {
                        strA = strA + num.ToString("x2", CultureInfo.InvariantCulture);
                    }
                    return (string.Compare(strA, "b03f5f7f11d50a3a", StringComparison.OrdinalIgnoreCase) == 0);
                }
            }
            return false;
        }

        private void RecoverWorkItem(object obj)
        {
            try
            {
                PropagationProtocolsTracing.TraceVerbose("Recovering...");
                ((TransactionManager) obj).Recover();
            }
            catch (PluggableProtocolException exception)
            {
                System.ServiceModel.DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Error);
                PropagationProtocolsTracing.TraceError("Error recovering: " + exception);
            }
            catch (Exception exception2)
            {
                System.ServiceModel.DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Critical);
                PropagationProtocolsTracing.TraceError("Unknown error recovering: " + exception2);
                TransactionBridgeRecoveryFailureRecord.TraceAndLog(exception2);
                System.ServiceModel.DiagnosticUtility.InvokeFinalHandler(exception2);
            }
        }

        public void Shutdown()
        {
        }

        public void Start()
        {
            List<TransactionManager> list = new List<TransactionManager>();
            try
            {
                PropagationProtocolsTracing.TraceVerbose("Starting...");
                foreach (TransactionManager manager in this.transactionManagers)
                {
                    manager.Start();
                    list.Add(manager);
                    if (!ThreadPool.QueueUserWorkItem(System.ServiceModel.DiagnosticUtility.Utility.ThunkCallback(new WaitCallback(this.RecoverWorkItem)), manager))
                    {
                        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TransactionBridgeException(Microsoft.Transactions.SR.GetString("CouldNotQueueStartUserWorkItem")));
                    }
                }
            }
            catch (Exception exception)
            {
                foreach (TransactionManager manager2 in list)
                {
                    manager2.Stop();
                }
                System.ServiceModel.DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Error);
                PropagationProtocolsTracing.TraceError("Error starting: " + exception);
                throw;
            }
        }
    }
}

