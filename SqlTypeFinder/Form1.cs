using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SqlTypeFinder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtDatabaseName.Text = "RCDLOYALTY_MPM";
            dataGridView1.DataSource = bsGrid;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ServerConnection srvConn = new ServerConnection();
                srvConn.ServerInstance = txtServerName.Text;
                srvConn.LoginSecure = false;
                srvConn.Login = txtLogin.Text;
                srvConn.Password = txtPassword.Text;
                Server srv = new Server(srvConn);

                Database db = srv.Databases[txtDatabaseName.Text];
                DataTable dataTable = db.EnumObjects(DatabaseObjectTypes.Table);

                var dbColumnsInfo = new List<DbColumInfo>();

                var count = 0;
                foreach (DataRow row in dataTable.Rows)
                {
                    string sSchema = (string)row["Schema"];
                    if (sSchema == "sys" || sSchema == "INFORMATION_SCHEMA")
                        continue;

                    Table table = (Table)srv.GetSmoObject(new Urn((string)row["Urn"]));
                    if (!table.IsSystemObject)
                    {
                        foreach (Column column in table.Columns)
                        {
                            if (column.Nullable)
                            {
                                if (column.DataType.SqlDataType == SqlDataType.Bit)
                                {
                                    dbColumnsInfo.Add(new DbColumInfo
                                    {
                                        TableName = table.Name,
                                        ColumnName = column.Name,
                                        DataType = column.DataType.Name,
                                        AllowNull = true
                                    });
                                }
                                else if (column.DataType.Name.Contains("int"))
                                {
                                    dbColumnsInfo.Add(new DbColumInfo
                                    {
                                        TableName = table.Name,
                                        ColumnName = column.Name,
                                        DataType = column.DataType.Name,
                                        AllowNull = true
                                    });
                                }
                            }
                        }
                    }
                    count++;
                    pgbProgress.Value = count * 100 / dataTable.Rows.Count;
                }

                bsGrid.DataSource = dbColumnsInfo.OrderBy(x => x.TableName);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
