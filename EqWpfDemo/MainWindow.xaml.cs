using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Win32;

using Korzh.EasyQuery;
using Korzh.EasyQuery.Wpf;
using Korzh.EasyQuery.Db;
using Korzh.EasyQuery.Services;

namespace EqWpfDemo {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window{            
        private readonly string _modelFileName;
        private readonly SqlConnection _connection;

        public MainWindow() {
            string directory = System.IO.Directory.GetCurrentDirectory();
            _connection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=EqDemoDb07;Trusted_Connection=True;");
            InitializeComponent();
                        
            DataContext = this;
            DbModel dataModel = new DbModel();
            //_modelFileName = System.IO.Path.Combine(directory, "App_Data\\NWindSQL.xml");
            _modelFileName = System.IO.Path.Combine(directory, "App_Data\\NWindSQL.json");
            try {
                //dataModel.LoadFromXmlFile(_modelFileName);
                dataModel.LoadFromJsonFile(_modelFileName);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }

            query = new DbQuery(dataModel);      

            AddHandler(ListXElement.ListRequestEvent, new ListXElement.ListRequestEventHandler(queryPanel_ListRequest));

            AddHandler(SimpleConditionRow.ValueRequestEvent, new ValueRequestEventHandler(queryPanel_CustomValueRequest));

            queryPanel.SortEntities = XSortOrder.Ascending;

            queryColumnsPanel.SortEntities = XSortOrder.Ascending;
            sortColumnsPanel.SortEntities = XSortOrder.Ascending;
            entitiesPanel.SortEntities = XSortOrder.Ascending;
            query.ConditionsChanged += query_ConditionsChanged;
            query.ColumnsChanged += query_ColumnsChanged;
            PanelExport.Visibility = Visibility.Collapsed;

            entitiesPanel.SortEntities = XSortOrder.Ascending;

            textBoxEntityFilter.TextChanged += TextBoxEntityFilter_TextChanged;
            entitiesPanel.ItemAdding += EntitiesPanel_ItemAdding;
        }

        private void TextBoxEntityFilter_TextChanged(object sender, TextChangedEventArgs e) {
            entitiesPanel.FilterByText(textBoxEntityFilter.Text);
        }

        private void EntitiesPanel_ItemAdding(object sender, ItemAddingEventArgs e) {
            //set e.Accept to true only for those item which you want to leave in the tree
        }

        void query_ColumnsChanged(object sender, ColumnsChangeEventArgs e) {
            SetSql();
        }

        void query_ConditionsChanged(object sender, ConditionsChangeEventArgs e) {
            SetSql();
        }

        void SetSql() {
            SqlQueryBuilder builder = new SqlQueryBuilder((DbQuery)queryPanel.Query);
            builder.Formats.SetDefaultFormats(FormatType.MsSqlServer);
            builder.Formats.OrderByStyle = OrderByStyles.Aliases;
            builder.Formats.DateFormat = "MM/dd/yyyy";
            builder.Formats.DateTimeFormat = "MM/dd/yyyy HH:mm";

            if (!builder.CanBuild) return;
            builder.BuildSQL();
            string sql = builder.Result.SQL;
            textBoxSql.Text = sql;
            buttonExecute.IsEnabled = !string.IsNullOrEmpty(sql);
        }

        private DbQuery query;
        public Query Query {
            get {                
                return query;
            }
        } 

        private Stream LoadEmbededResource(string resourceFileName) {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            return a.GetManifestResourceStream("EqWpfDemo.res." + resourceFileName);
        }

        private void CreateFileByResource(string fileName) {
            using (Stream s = LoadEmbededResource(fileName)) {
                using (var fileStream = File.Create(fileName)) {
                    s.CopyTo(fileStream);
                    fileStream.Flush();
                }
            }
        }

        private void CheckConnection() {
            if (_connection.State != System.Data.ConnectionState.Open)
                _connection.Open();
        }


        private void Execute_Click(object sender, RoutedEventArgs e) {            
            try {
                string sql = textBoxSql.Text;
                CheckConnection();
                var resultDA = new SqlDataAdapter(sql, _connection);

                DataSet ResultDS = new DataSet();
                resultDA.Fill(ResultDS, "Result");
                datGrid.ItemsSource = ResultDS.Tables[0].DefaultView;   
                
                _connection.Close();
                PanelExport.Visibility = Visibility.Visible;
            }
            catch (Exception error) {
                //if some error occurs just show the error message 
                MessageBox.Show(error.Message);
            }
            finally {
                _connection.Close();
            }
        }

        private void TextBoxSql_TextChanged(object sender, TextChangedEventArgs e)
        {
            buttonExecute.IsEnabled = !string.IsNullOrEmpty(textBoxSql.Text);
        }


        private void Clear_Click(object sender, RoutedEventArgs e) {
            Clear();
            PanelExport.Visibility = Visibility.Collapsed;
        }

        private void Clear() {
            queryPanel.Query.Clear();
            textBoxSql.Clear();
            datGrid.ItemsSource = null;
            buttonExecute.IsEnabled = false;
        }

        private void queryPanel_ListRequest(object sender, ListRequestEventArgs e) {
            string listName = e.ListName;
            if (listName == "SQL") {
                GetSqlList(e.SQL, e.ListItems);
            }
            else if (listName == "RegionList") {
                e.ListItems.Clear();
                e.ListItems.Add("British Columbia", "BC");
                e.ListItems.Add("Colorado", "CO");
                e.ListItems.Add("Oregon", "OR");
                e.ListItems.Add("Washington", "WA");
            }
        }



        private void GetSqlList(string sql, ValueItemList items) {
            CheckConnection();
            var resultDA = new SqlDataAdapter(sql, _connection);

            DataSet tempDS = new DataSet();
            try {
                resultDA.Fill(tempDS, "Result");

                StringWriter strWriter = new StringWriter();
                tempDS.WriteXml(strWriter);
                items.LoadFromXml(strWriter.ToString());
            }
            catch (Exception) {
                items.Clear();
            }
            
        }

        /// <summary>
        /// An example of CUSTOM value editor processing. 
        /// This an event handler for SimpleConditionRow.ValueRequestEvent which is raised when we click on element with "CUSTOM" value editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void queryPanel_CustomValueRequest(object sender, ValueRequestEventArgs e) {
            var postalCodeDlg = new PostalCodeDialog();
            postalCodeDlg.Owner = this;

            bool? res = postalCodeDlg.ShowDialog();

            if (res != null && res.Value) {
                var district = postalCodeDlg.SelectedDistrict;
                e.Value = district.Code;
                e.Text = district.Name;
            }
        }


        private void Load_Click(object sender, RoutedEventArgs e) {
            try {
                OpenFileDialog openFileDlg = new OpenFileDialog();
                openFileDlg.Filter = "JSON files (*.json)|*.json|XML files (*.xml)|*.xml";
                openFileDlg.FilterIndex = 1;
                bool? result = openFileDlg.ShowDialog();
                if (result == true) {
                    if (openFileDlg.FilterIndex == 1) {
                        queryPanel.Query.LoadFromJsonFile(openFileDlg.FileName);
                    }
                    else {
                        queryPanel.Query.LoadFromXmlFile(openFileDlg.FileName);
                    }
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Specified query file is not valid:\n" + ex.Message);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e) {
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Filter = "JSON files (*.json)|*.json|XML files (*.xml)|*.xml";
            saveFileDlg.FilterIndex = 1;
            saveFileDlg.AddExtension = true;
            bool? result = saveFileDlg.ShowDialog();
            if (result == true) {
                if (saveFileDlg.FilterIndex == 1) {
                    queryPanel.Query.SaveToJsonFile(saveFileDlg.FileName);
                }
                else {
                    queryPanel.Query.SaveToXmlFile(saveFileDlg.FileName);
                }
            }
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e) {
            try {
                DataTable resultDt = ((DataView)datGrid.ItemsSource).ToTable();
                SaveFileDialog saveFileDlg = new SaveFileDialog();
                saveFileDlg.Filter = "xls files (*.xls)|*.xls";
                saveFileDlg.DefaultExt = "xls";
                saveFileDlg.FilterIndex = 2;
                saveFileDlg.RestoreDirectory = true;
                bool? result = saveFileDlg.ShowDialog();
                if (result == true) {
                    var exporter = new ExcelHtmlDataExporter();
                    using (var streamWriter = File.OpenWrite(saveFileDlg.FileName))
                        exporter.Export(resultDt.CreateDataReader(), streamWriter);
                }
            }
            catch (Exception error) {
                //if some error occurs just show the error message 
                MessageBox.Show(error.Message);
            }
        }

        private void ExportToCsv_Click(object sender, RoutedEventArgs e) {
            try {
                DataTable resultDt = ((DataView)datGrid.ItemsSource).ToTable();
                SaveFileDialog saveFileDlg = new SaveFileDialog();
                saveFileDlg.Filter = "csv files (*.csv)|*.csv";
                saveFileDlg.DefaultExt = "csv";
                saveFileDlg.FilterIndex = 2;
                saveFileDlg.RestoreDirectory = true;
                bool? result = saveFileDlg.ShowDialog();
                if (result == true) {
                    var exporter = new CsvDataExporter();
                    using (var streamWriter = File.OpenWrite(saveFileDlg.FileName))
                        exporter.Export(resultDt.CreateDataReader(), streamWriter);
                }
            }
            catch (Exception error) {
                //if some error occurs just show the error message 
                MessageBox.Show(error.Message);
            }
        }
    }
}
