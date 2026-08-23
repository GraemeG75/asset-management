using System.Data;
using AssetManagement.Core.Services;
using Microsoft.Data.Sqlite;

namespace AssetManagement.Infrastructure.Services
{
    public class SqliteConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        private readonly IDbConnection? _existingConnection;

        public SqliteConnectionFactory(string connectionString = "Data Source=AssetManagementDb;Mode=Memory;Cache=Shared")
        {
            _connectionString = connectionString;
        }

        public SqliteConnectionFactory(IDbConnection existingConnection)
        {
            _existingConnection = existingConnection;
            _connectionString = existingConnection.ConnectionString;
        }

        public IDbConnection CreateConnection()
        {
            if (_existingConnection != null)
            {
                if (_existingConnection.State != ConnectionState.Open)
                {
                    _existingConnection.Open();
                }
                return new NonDisposingDbConnection(_existingConnection);
            }

            SqliteConnection connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }

        private class NonDisposingDbConnection : IDbConnection
        {
            private readonly IDbConnection _inner;

            public NonDisposingDbConnection(IDbConnection inner)
            {
                _inner = inner;
            }

#pragma warning disable CS8767
            public string ConnectionString { get => _inner.ConnectionString ?? string.Empty; set => _inner.ConnectionString = value; }
#pragma warning restore CS8767
            public int ConnectionTimeout => _inner.ConnectionTimeout;
            public string Database => _inner.Database;
            public ConnectionState State => _inner.State;

            public IDbTransaction BeginTransaction() => _inner.BeginTransaction();
            public IDbTransaction BeginTransaction(IsolationLevel il) => _inner.BeginTransaction(il);
            public void ChangeDatabase(string databaseName) => _inner.ChangeDatabase(databaseName);
            public void Close() { /* Do not close shared connection */ }
            public IDbCommand CreateCommand() => _inner.CreateCommand();
            public void Open() => _inner.Open();
            public void Dispose() { /* Do not dispose shared connection */ }
        }
    }
}
