namespace System.Data.Common
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Reflection;

    public static class DbProviderFactories
    {
        private static DataSet _configTable;
        private static ConnectionState _initState;
        private static object _lockobj = new object();
        private const string AssemblyQualifiedName = "AssemblyQualifiedName";
        private const string Instance = "Instance";
        private const string InvariantName = "InvariantName";

        private static DataSet GetConfigTable()
        {
            Initialize();
            return _configTable;
        }

        public static DbProviderFactory GetFactory(DataRow providerRow)
        {
            ADP.CheckArgumentNull(providerRow, "providerRow");
            DataColumn column = providerRow.Table.Columns["AssemblyQualifiedName"];
            if (column != null)
            {
                string str = providerRow[column] as string;
                if (!ADP.IsEmpty(str))
                {
                    Type type = Type.GetType(str);
                    if (type == null)
                    {
                        throw ADP.ConfigProviderNotInstalled();
                    }
                    FieldInfo field = type.GetField("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
                    if ((field != null) && field.FieldType.IsSubclassOf(typeof(DbProviderFactory)))
                    {
                        object obj2 = field.GetValue(null);
                        if (obj2 != null)
                        {
                            return (DbProviderFactory) obj2;
                        }
                    }
                    throw ADP.ConfigProviderInvalid();
                }
            }
            throw ADP.ConfigProviderMissing();
        }

        public static DbProviderFactory GetFactory(string providerInvariantName)
        {
            ADP.CheckArgumentLength(providerInvariantName, "providerInvariantName");
            DataSet configTable = GetConfigTable();
            DataTable table = configTable?.Tables["DbProviderFactories"];
            if (table != null)
            {
                DataRow providerRow = table.Rows.Find(providerInvariantName);
                if (providerRow != null)
                {
                    return GetFactory(providerRow);
                }
            }
            throw ADP.ConfigProviderNotFound();
        }

        public static DataTable GetFactoryClasses()
        {
            DataSet configTable = GetConfigTable();
            DataTable table = configTable?.Tables["DbProviderFactories"];
            if (table != null)
            {
                return table.Copy();
            }
            return DbProviderFactoriesConfigurationHandler.CreateProviderDataTable();
        }

        private static void Initialize()
        {
            if (ConnectionState.Open != _initState)
            {
                lock (_lockobj)
                {
                    switch (_initState)
                    {
                        case ConnectionState.Closed:
                            break;

                        case ConnectionState.Open:
                        case ConnectionState.Connecting:
                            return;

                        default:
                            return;
                    }
                    _initState = ConnectionState.Connecting;
                    try
                    {
                        _configTable = System.Configuration.PrivilegedConfigurationManager.GetSection("system.data") as DataSet;
                    }
                    finally
                    {
                        _initState = ConnectionState.Open;
                    }
                }
            }
        }
    }
}

