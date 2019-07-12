using System;
using System.IO;
using System.Data.SqlClient;

namespace EqAspNet4Demo
{
    public class DbIntializer
    {

        private readonly SqlConnection _connection;

        private readonly string _scriptFilePath;

        public DbIntializer(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _scriptFilePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data"), "EqDemoDb.sql");
        }

        public void AddTestData()
        {
            try {
                _connection.Open();

                if (IsEmptyDb()) {
                    FillDb();
                }

            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
            finally {
                _connection.Close();
            }

        }

        private bool IsEmptyDb()
        {
            var fillDbCommand = _connection.CreateCommand();
            fillDbCommand.CommandText = "SELECT TOP(1) CategoryID FROM dbo.Categories";

            object someId = fillDbCommand.ExecuteScalar();

            return someId == null;
        }

        private void FillDb()
        {
            var fillDbCommand = _connection.CreateCommand();

            fillDbCommand.CommandText = System.IO.File.ReadAllText(_scriptFilePath);
            fillDbCommand.CommandTimeout = 300;

            fillDbCommand.ExecuteNonQuery();
        }
    }
}