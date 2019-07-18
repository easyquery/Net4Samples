using System;
using System.IO;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;

namespace EqWinFormsDemo
{
    public class DbInitializer
    {
        private SqlConnection _connection;
        private string _scriptFilePath;

        public DbInitializer(SqlConnection connection, string dataFolder = null)
        {
            if (dataFolder == null) {               
                dataFolder = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
            }
            _connection = connection;
            _scriptFilePath = Path.Combine(dataFolder, "EqDemoDb.sql");
        }

        public void EnsureCreated()
        {
            bool dbExists = false;
            try
            {
                _connection.Open();
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
            var connectionStringBuilder = new SqlConnectionStringBuilder(_connection.ConnectionString) {
                InitialCatalog = "master"
            };
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