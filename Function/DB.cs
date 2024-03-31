using System.Data;
using System.Data.SqlClient;

namespace Packing.Function
{
    public class DB
    {
        public DataTable GetDataTable(string str)
        {
            string connstr = "Data Source=119.59.117.165;Initial Catalog=vms_packing; User ID=dev;Password=devISR@2022;encrypt=false";
            SqlConnection cnn;
            cnn = new SqlConnection(connstr);
            cnn.Open();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(str, cnn);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            cnn.Close();
            cnn.Dispose();
            return dataTable;
        }
    }
}
