namespace System.Data.Common.Utils
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.EntityClient;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;

    internal static class CommandHelper
    {
        internal static void ConsumeReader(DbDataReader reader)
        {
            if ((reader != null) && !reader.IsClosed)
            {
                while (reader.NextResult())
                {
                }
            }
        }

        internal static EdmFunction FindFunctionImport(MetadataWorkspace workspace, string containerName, string functionImportName)
        {
            EntityContainer container;
            if (!workspace.TryGetEntityContainer(containerName, DataSpace.CSpace, out container))
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_UnableToFindFunctionImportContainer(containerName));
            }
            EdmFunction function = null;
            foreach (EdmFunction function2 in container.FunctionImports)
            {
                if (function2.Name == functionImportName)
                {
                    function = function2;
                    break;
                }
            }
            if (function == null)
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_UnableToFindFunctionImport(containerName, functionImportName));
            }
            return function;
        }

        internal static EntityTransaction GetEntityTransaction(EntityCommand entityCommand)
        {
            EntityTransaction transaction = entityCommand.Transaction;
            if ((transaction != null) && (transaction != entityCommand.Connection.CurrentTransaction))
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_InvalidTransactionForCommand);
            }
            return entityCommand.Connection.CurrentTransaction;
        }

        internal static void ParseFunctionImportCommandText(string commandText, string defaultContainerName, out string containerName, out string functionImportName)
        {
            string[] strArray = commandText.Split(new char[] { '.' });
            containerName = null;
            functionImportName = null;
            if (2 == strArray.Length)
            {
                containerName = strArray[0].Trim();
                functionImportName = strArray[1].Trim();
            }
            else if ((1 == strArray.Length) && (defaultContainerName != null))
            {
                containerName = defaultContainerName;
                functionImportName = strArray[0].Trim();
            }
            if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(functionImportName))
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_InvalidStoredProcedureCommandText);
            }
        }

        internal static void SetEntityParameterValues(EntityCommand entityCommand, DbCommand storeProviderCommand)
        {
            foreach (DbParameter parameter in storeProviderCommand.Parameters)
            {
                if ((parameter.Direction & ParameterDirection.Output) != ((ParameterDirection) 0))
                {
                    int index = entityCommand.Parameters.IndexOf(parameter.ParameterName);
                    if (0 <= index)
                    {
                        EntityParameter parameter2 = entityCommand.Parameters[index];
                        parameter2.Value = parameter.Value;
                    }
                }
            }
        }

        internal static void SetStoreProviderCommandState(EntityCommand entityCommand, EntityTransaction entityTransaction, DbCommand storeProviderCommand)
        {
            storeProviderCommand.CommandTimeout = entityCommand.CommandTimeout;
            storeProviderCommand.Connection = entityCommand.Connection.StoreConnection;
            storeProviderCommand.Transaction = entityTransaction?.StoreTransaction;
            storeProviderCommand.UpdatedRowSource = entityCommand.UpdatedRowSource;
        }
    }
}

