using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;

namespace Packing.DatabaseConnection
{
    public class ConnectDB
    {
        private string connectionString = "Data Source=192.168.5.240;Initial Catalog=VMS_PACKING;Persist Security Info=True;User ID=isradmin;Password=1qazxsw2";
        SqlConnection connection;
        public ConnectDB()
        {
           connection = new SqlConnection(connectionString);
            
        }


        public DataTable SelectData(string StoredProcedure, SqlCommand commands=null!)
        {

            connection.Open();
            DataTable dataTable = new DataTable();
            if (commands != null)
            {
               commands.Connection = connection;
                commands.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = commands.ExecuteReader();
                    dataTable.Load(reader);
            }
            else
            {
                using (SqlCommand command = new SqlCommand(StoredProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    //// เพิ่มพารามิเตอร์ (ถ้ามี)
                    //command.Parameters.AddWithValue("@param1", value1);
                    //command.Parameters.AddWithValue("@param2", value2);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    dataTable.Load(reader);

                }
            }
             
                return dataTable;
            
        }
    
        public bool SaveData(string StoredProcedure, SqlCommand commands = null!)
        {
            try
            {
                connection.Open();
                if (commands != null)
                {

                    commands.CommandType = System.Data.CommandType.StoredProcedure;
                    return commands.ExecuteNonQuery() > 1;

                }
                else
                {
                    using (SqlCommand command = new SqlCommand("InsertData", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        return command.ExecuteNonQuery() > 1;
                     
                    }


                }
               
            }
            catch (Exception)
            {
                connection.Dispose();
                return false;
            }
               
           
        }
    }
}
