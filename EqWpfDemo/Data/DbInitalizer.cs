using System;
using System.IO;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;

namespace EqWpfDemo
{

    public class DbInitializer
    {
        private string _connectionString;
        private SqlConnection _connection;
        private string _scriptFilePath;


        private DbInitializer()
        {
            _scriptFilePath = _scriptFilePath = Path.Combine(Environment.CurrentDirectory, "App_Data", "EqDemoDb.sql"); 
        }

        public DbInitializer(string connectionString): this()
        {
            _connectionString = connectionString;
            _connection = new SqlConnection(_connectionString);
        }

        public DbInitializer(SqlConnection connection): this()
        {
            _connectionString = connection.ConnectionString;
            _connection = connection;
        }

        public void EnsureCreated()
        {
            bool dbExists = false;
            try
            {
                _connection.Open();
                _connection.Close();
                dbExists = true;
            }
            catch
            {

            }

            if (!dbExists)
            {
                CreateDb();
                FillDb();
            }
        }

        private void CreateDb()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(_connectionString) { InitialCatalog = "master" };
            connectionStringBuilder.Remove("AttachDBFilename");

            using (var masterConnnection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                masterConnnection.Open();

                var createDbCommand = masterConnnection.CreateCommand();
                createDbCommand.CommandText = "CREATE DATABASE " + _connection.Database;

                createDbCommand.ExecuteScalar();
                masterConnnection.Close();
            }

            Task.Delay(2000).Wait();

            TryToOpenNewDb();
        }

        private void TryToOpenNewDb()
        {
            int N = 0;
            Exception lastException = null;
            do
            {
                try
                {
                    _connection.Open();
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    Task.Delay(2000).Wait();
                }
                N++;
            }
            while (_connection.State != ConnectionState.Open && N < 3);

            if (_connection.State != ConnectionState.Open)
            {
                throw lastException;
            }
        }

        private void FillDb()
        {
            string script = System.IO.File.ReadAllText(_scriptFilePath);

            var fillDbCommand = _connection.CreateCommand();

            fillDbCommand.CommandText = script;

            fillDbCommand.ExecuteNonQuery();
        }
    }
 
}