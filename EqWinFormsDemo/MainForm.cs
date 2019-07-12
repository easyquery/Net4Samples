using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Korzh.EasyQuery;
using Korzh.EasyQuery.Db;
using Korzh.EasyQuery.Services;
using Korzh.EasyQuery.WinForms;

namespace EqWinFormsDemo
{
    public partial class MainForm : Form
    {

        private System.Windows.Forms.OpenFileDialog openFileDlg;
        private System.Windows.Forms.SaveFileDialog saveFileDlg;
        private System.Data.DataSet ResultDS;
        private System.Data.DataTable ResultDataTable;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.GroupBox groupBoxSQL;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.GroupBox groupBoxResultSet;
        private System.Windows.Forms.TextBox teSQL;
        private System.Windows.Forms.DataGrid dataGrid1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Panel panelBG;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btClear;
        private System.Windows.Forms.Button btLoad;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btExecute;
        private System.Windows.Forms.GroupBox groupBoxConditions;
        private System.Windows.Forms.Panel panelQuery;
        private Panel panelColumns;
        private Splitter splitter4;
        private GroupBox groupBoxSorting;
        private SortColumnsPanel SCPanel;
        private GroupBox groupBoxColumns;
        private QueryColumnsPanel QCPanel;
        private QueryPanel QPanel;
        private GroupBox groupBoxEntities;
        private EntitiesPanel EntPanel;
        private Splitter splitter3;

        private DbModel dataModel1;
        private DbQuery query1;

        private ToolTip toolTipExel;
        private ToolTip toolTipCsv;
        private GroupBox panelExport;
        private Button btnExportCsv;
        private Button btnExportExel;

        private readonly string _dataFolder = "App_Data";
        private readonly string _appDirectory;


        private readonly string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=EqDemoDb07;Trusted_Connection=True;";
        private SqlConnection _connection;

        public MainForm()
        {
            _appDirectory = System.IO.Directory.GetCurrentDirectory();
            _dataFolder = System.IO.Path.Combine(_appDirectory, "App_Data");

            InitializeComponent();
            query1.Model = dataModel1;
            QPanel.Query = this.query1;
            EntPanel.ShowFilter = true;
            EntPanel.UpdateModelInfo();
            countryAttr = dataModel1.EntityRoot.FindAttribute(EntityAttrProp.Expression, "Customers.Country");
            PanelExportShow(false);

            InitDataModel();

            this.QCPanel.AllowEditCaptions = true;
            this.QCPanel.AllowSorting = true;
        }

        private void CheckConnection()
        {         
            CheckSqlConnection();
        }


        private void CheckSqlConnection()
        {
            if (_connection == null)
                _connection = new SqlConnection(_connectionString);

            if (_connection.State != ConnectionState.Open)
            {
                try {
                    _connection.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btClear_Click(object sender, System.EventArgs e)
        {
            QPanel.Query.Clear();
            EntPanel.ClearFilter();
            teSQL.Clear();
            dataGrid1.DataSource = null;
            PanelExportShow(false);
        }

        private void btLoad_Click(object sender, System.EventArgs e)
        {
            openFileDlg.Filter = "JSON files (*.json)|*.json|XML files (*.xml)|*.xml";
            openFileDlg.FilterIndex = 1;
            if (openFileDlg.ShowDialog() == DialogResult.OK) {
                if (openFileDlg.FilterIndex == 1) {
                    QPanel.Query.LoadFromJsonFile(openFileDlg.FileName);
                }
                else {
                    QPanel.Query.LoadFromXmlFile(openFileDlg.FileName);
                }
            }
        }

        private void btSave_Click(object sender, System.EventArgs e)
        {
            saveFileDlg.Filter = "JSON files (*.json)|*.json|XML files (*.xml)|*.xml";
            saveFileDlg.FilterIndex = 1;
            saveFileDlg.AddExtension = true;
            saveFileDlg.FileName = System.IO.Path.Combine(_dataFolder, "queries\\query");
            if (saveFileDlg.ShowDialog() == DialogResult.OK) {
                if (saveFileDlg.FilterIndex == 1) {
                    QPanel.Query.SaveToJsonFile(saveFileDlg.FileName);
                }
                else {
                    QPanel.Query.SaveToXmlFile(saveFileDlg.FileName);
                }
            }
        }

        private void btExecute_Click(object sender, System.EventArgs e)
        {

            try {

                ResultDS.Reset();
                var builder = BuildSQL();
                var builderResult = builder.GetResult();

                CheckConnection();

                var command = _connection.CreateCommand();
                command.CommandText = builderResult.Statement;
                foreach (QueryParam param in builderResult.Params) {
                    command.Parameters.Add("@" + param.Id);
                    command.Parameters["@" + param.Id].Value = param.Value;
                }

                var resultDA = new SqlDataAdapter(command);
                resultDA.Fill(ResultDS, "Result");
                dataGrid1.DataSource = ResultDS.Tables[0].DefaultView;

                _connection.Close();
                // panelExport.Visible = true;
                PanelExportShow(true);
            }
            catch (Exception error)
            {
                //if some error occurs just show the error message 
                MessageBox.Show(error.Message);
            }
        }


        private IQueryBuilder BuildSQL()
        {
            teSQL.Clear();
            try {
                SqlQueryBuilder builder = new SqlQueryBuilder(query1);
                builder.Formats.SetDefaultFormats(FormatType.MsAccess);

                if (builder.CanBuild) {
                    builder.BuildSQL();
                    string sql = builder.Result.SQL;
                    teSQL.Text = sql;

                    return builder;
                }

                return null;
            }
            catch (Exception ex) {
                //Simply ignore any possible exception
                MessageBox.Show(ex.Message);
            }

            return null;
        }

        private EntityAttr countryAttr = null;

        private void QPanel_ListRequest(object sender, ListRequestEventArgs e)
        {
            if (e.ListName == "SQL") {
                CheckConnection();

                var sql = e.Data.ToString();
                DataSet tempDS = new DataSet();

                var tempDA = new SqlDataAdapter(sql, _connection);
                tempDA.Fill(tempDS, "Temp");

                var strWriter = new StringWriter();
                tempDS.WriteXml(strWriter);
                e.ResultXml = strWriter.ToString();

                //e.ListItems.Clear();
                //foreach (DataRow row in tempDS.Tables[0].Rows) {
                //    e.ListItems.Add(row[1].ToString(), row[0].ToString());
                //}            
            }
            else if (e.ListName == "RegionList") {
                e.ListItems.Clear();
                string country = query1.GetOneValueForAttr(countryAttr);

                if (country == "Canada" || country == null) {
                    e.ListItems.Add("British Columbia", "BC");
                    e.ListItems.Add("Quebec", "Quebec");
                }

                if (country == "USA" || country == null) {
                    e.ListItems.Add("California", "CA");
                    e.ListItems.Add("Colorado", "CO");
                    e.ListItems.Add("Oregon", "OR");
                    e.ListItems.Add("Washington", "WA");
                }
            }

        }

        private void query1_ColumnsChanged(object sender, ColumnsChangeEventArgs e)
        {
            BuildSQL();
            ResultDS.Reset();
        }

        private void query1_ConditionsChanged(object sender, ConditionsChangeEventArgs e)
        {
            EntityAttr baseAttr = null;
            if (e.Condition != null)
                baseAttr = e.Condition.BaseAttr;

            if (baseAttr != null && baseAttr == countryAttr) {
                QPanel.RefreshList("RegionList");
            }

            BuildSQL();
            ResultDS.Reset();
        }

        private void ResetDataModel()
        {
            query1.Clear();
            dataModel1.LoadFromXmlFile(System.IO.Path.Combine(_dataFolder, "NWindMDB.xml"));
           
            QPanel.UpdateModelInfo();
            QCPanel.UpdateModelInfo();
            EntPanel.UpdateModelInfo();
        }

        private void QPanel_ValueRequest(object sender, ValueRequestEventArgs e)
        {
            MessageBox.Show(e.Data);
        }

        private void QPanel_ConditionRender(object sender, ConditionRenderEventArgs e)
        {
            //for (int i = 0; i < e.Row.Elements.Count; i++) {
            //    e.Row.Elements[i].TextColor = Color.Red;
            //}
        }

        private void CloseConnections()
        {
            if (_connection != null)
                _connection.Close();
        }

        private void InitDataModel()
        {

            //dataModel1.LoadFromXmlFile(System.IO.Path.Combine(_dataFolder, "NwindSQL.xml"));
            dataModel1.LoadFromJsonFile(System.IO.Path.Combine(_dataFolder, "NwindSQL.json"));
                
            QPanel.UpdateModelInfo();
            QCPanel.UpdateModelInfo();
            EntPanel.UpdateModelInfo();
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            try {
                //Save CSV file 
                using (SaveFileDialog saveFileDialog = new SaveFileDialog()) {
                    saveFileDialog.Filter = "csv files (*.csv)|*.csv";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {

                        var exporter = new CsvDataExporter();
                        using (var streamWriter = File.OpenWrite(saveFileDialog.FileName))
                            exporter.Export(ResultDS.CreateDataReader(ResultDS.Tables.Cast<DataTable>().ToArray()), streamWriter);
                    }
                }


            }
            catch (Exception error) {
                //if some error occurs just show the error message 
                MessageBox.Show(error.Message);
            }
        }

        private void btnExportXls_Click(object sender, EventArgs e)
        {
            try {
                //Save xls  file 
                using (SaveFileDialog saveFileDialog = new SaveFileDialog()) {
                    saveFileDialog.Filter = "xls files (*.xls)|*.xls";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog(this) == DialogResult.OK)  {

                        var exporter = new ExcelHtmlDataExporter();
                        using (var streamWriter = File.OpenWrite(saveFileDialog.FileName))
                            exporter.Export(ResultDS.CreateDataReader(ResultDS.Tables.Cast<DataTable>().ToArray()), streamWriter);
                    }
                }


            }
            catch (Exception error) {
                //if some error occurs just show the error message 
                MessageBox.Show(error.Message);
            }
        }

        private void PanelExportShow(bool activ)
        {
            if (activ) {
                this.panelExport.Show();
                this.groupBoxResultSet.Width = this.groupBoxResultSet.Parent.ClientSize.Width - this.panelExport.Width - this.groupBoxResultSet.Left - 4;
            }
            else {
                this.panelExport.Hide();
                this.groupBoxResultSet.Width = this.groupBoxResultSet.Parent.ClientSize.Width - this.groupBoxResultSet.Left - 4;
            }
        }
    }
}
