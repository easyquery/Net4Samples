
Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Threading.Tasks

Public Class DbInitializer

    Private _connectionString As String
    Private _connection As SqlConnection
    Private _scriptFilePath As String

    Public Sub New(connectionString As String)
        _connectionString = connectionString
        _connection = New SqlConnection(_connectionString)
        _scriptFilePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data"), "EqDemoDb.sql")
    End Sub


    Public Sub EnsureCreated()
        Dim dbExists As Boolean = False
        Try
            _connection.Open()
            _connection.Close()
            dbExists = True
            Exit Try
        Catch
            Exit Try

        End Try


        If Not dbExists Then
            CreateDb()
            FillDb()
        End If

    End Sub

    Private Sub CreateDb()

        Dim connectionStringBuilder As SqlConnectionStringBuilder = New SqlConnectionStringBuilder(_connectionString)
        connectionStringBuilder.InitialCatalog = "master"
        connectionStringBuilder.Remove("AttachDBFilename")

        Using masterConnnection As SqlConnection = New SqlConnection(connectionStringBuilder.ConnectionString)
            masterConnnection.Open()

            Dim createDbCommand As SqlCommand = masterConnnection.CreateCommand()
            createDbCommand.CommandText = "CREATE DATABASE " + _connection.Database

            createDbCommand.ExecuteScalar()
            masterConnnection.Close()
        End Using

        Task.Delay(2000).Wait()

        TryToOpenNewDb()
    End Sub

    Private Sub FillDb()

        Dim script As String = System.IO.File.ReadAllText(_scriptFilePath)

        Dim fillDbCommand As SqlCommand = _connection.CreateCommand()

        fillDbCommand.CommandText = script

        fillDbCommand.ExecuteNonQuery()
    End Sub


    Private Sub TryToOpenNewDb()
        Dim N = 0
        Dim lastException As Exception = Nothing

        Do
            Try
                _connection.Open()
                Exit Try

            Catch ex As Exception

                lastException = ex
                Task.Delay(2000).Wait()
                N += 1

                Exit Try

            End Try


        Loop While Not _connection.State.Equals(ConnectionState.Open) And N < 3

        If Not _connection.State.Equals(ConnectionState.Open) Then
            Throw lastException
        End If


    End Sub

End Class
