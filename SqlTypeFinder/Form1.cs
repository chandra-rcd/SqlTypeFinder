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
            dataGridView1.DataSource = bsGrid;
            pgbProgress.Value = 100;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnFind.Enabled = false;

            try
            {
                ServerConnection srvConn = new ServerConnection();
                srvConn.ServerInstance = txtServerName.Text;
                srvConn.LoginSecure = false;
                srvConn.Login = txtLogin.Text;
                srvConn.Password = txtPassword.Text;
                Server srv = new Server(srvConn);

                pgbProgress.Value = 0;

                Database db = srv.Databases[txtDatabaseName.Text];
                DataTable dataTable = db.EnumObjects(DatabaseObjectTypes.Table);

                var dbColumnsInfo = new List<DbColumInfo>();

                var step = 0;
                var rows = dataTable.Rows;
                var total = rows.Count;

                foreach (DataRow row in rows)
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
                    step++;
                    pgbProgress.Value = step * 100 / total;
                }

                bsGrid.DataSource = dbColumnsInfo.OrderBy(x => x.TableName);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            btnFind.Enabled = true;
        }
    }
}
