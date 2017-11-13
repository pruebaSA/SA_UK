namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Linq.Provider;
    using System.Data.SqlClient;
    using System.Transactions;

    internal class SqlConnectionManager : IConnectionManager
    {
        private bool autoClose;
        private DbConnection connection;
        private SqlInfoMessageEventHandler infoMessagehandler;
        private int maxUsers;
        private IProvider provider;
        private System.Transactions.Transaction systemTransaction;
        private DbTransaction transaction;
        private List<IConnectionUser> users;

        internal SqlConnectionManager(IProvider provider, DbConnection con, int maxUsers)
        {
            this.provider = provider;
            this.connection = con;
            this.maxUsers = maxUsers;
            this.infoMessagehandler = new SqlInfoMessageEventHandler(this.OnInfoMessage);
            this.users = new List<IConnectionUser>(maxUsers);
        }

        private void AddInfoMessageHandler()
        {
            SqlConnection connection = this.connection as SqlConnection;
            if (connection != null)
            {
                connection.InfoMessage += this.infoMessagehandler;
            }
        }

        private void BootUser(IConnectionUser user)
        {
            bool autoClose = this.autoClose;
            this.autoClose = false;
            int index = this.users.IndexOf(user);
            if (index >= 0)
            {
                this.users.RemoveAt(index);
            }
            user.CompleteUse();
            this.autoClose = autoClose;
        }

        internal void ClearConnection()
        {
            while (this.users.Count > 0)
            {
                this.BootUser(this.users[0]);
            }
        }

        private void CloseConnection()
        {
            if ((this.connection != null) && (this.connection.State != ConnectionState.Closed))
            {
                this.connection.Close();
            }
            this.RemoveInfoMessageHandler();
            this.autoClose = false;
        }

        internal void DisposeConnection()
        {
            if (this.autoClose)
            {
                this.CloseConnection();
            }
        }

        private void OnInfoMessage(object sender, SqlInfoMessageEventArgs args)
        {
            if (this.provider.Log != null)
            {
                this.provider.Log.WriteLine(Strings.LogGeneralInfoMessage(args.Source, args.Message));
            }
        }

        private void OnTransactionCompleted(object sender, TransactionEventArgs args)
        {
            if ((this.users.Count == 0) && this.autoClose)
            {
                this.CloseConnection();
            }
        }

        public void ReleaseConnection(IConnectionUser user)
        {
            if (user == null)
            {
                throw Error.ArgumentNull("user");
            }
            int index = this.users.IndexOf(user);
            if (index >= 0)
            {
                this.users.RemoveAt(index);
            }
            if (((this.users.Count == 0) && this.autoClose) && ((this.transaction == null) && (System.Transactions.Transaction.Current == null)))
            {
                this.CloseConnection();
            }
        }

        private void RemoveInfoMessageHandler()
        {
            SqlConnection connection = this.connection as SqlConnection;
            if (connection != null)
            {
                connection.InfoMessage -= this.infoMessagehandler;
            }
        }

        public DbConnection UseConnection(IConnectionUser user)
        {
            if (user == null)
            {
                throw Error.ArgumentNull("user");
            }
            if (this.connection.State == ConnectionState.Closed)
            {
                this.connection.Open();
                this.autoClose = true;
                this.AddInfoMessageHandler();
                if (System.Transactions.Transaction.Current != null)
                {
                    System.Transactions.Transaction.Current.TransactionCompleted += new TransactionCompletedEventHandler(this.OnTransactionCompleted);
                }
            }
            if (((this.transaction == null) && (System.Transactions.Transaction.Current != null)) && (System.Transactions.Transaction.Current != this.systemTransaction))
            {
                this.ClearConnection();
                this.systemTransaction = System.Transactions.Transaction.Current;
                this.connection.EnlistTransaction(System.Transactions.Transaction.Current);
            }
            if (this.users.Count == this.maxUsers)
            {
                this.BootUser(this.users[0]);
            }
            this.users.Add(user);
            return this.connection;
        }

        internal bool AutoClose
        {
            get => 
                this.autoClose;
            set
            {
                this.autoClose = value;
            }
        }

        internal DbConnection Connection =>
            this.connection;

        internal int MaxUsers =>
            this.maxUsers;

        internal DbTransaction Transaction
        {
            get => 
                this.transaction;
            set
            {
                if (value != this.transaction)
                {
                    if ((value != null) && (this.connection != value.Connection))
                    {
                        throw Error.TransactionDoesNotMatchConnection();
                    }
                    this.transaction = value;
                }
            }
        }
    }
}

