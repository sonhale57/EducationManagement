using Hangfire;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SuperbrainManagement.Controllers
{
    public class Connect
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
        public Connect() { }
        /*
        public DataSet ShowAll(String txt)
        {
            MySqlConnection connection;
            MySqlCommand cmd;
            MySqlDataAdapter da;

            connection = null;
            cmd = null;
            DataSet ds = new DataSet();
            da = new MySqlDataAdapter();

            try
            {
                cmd = new MySqlCommand(txt);
                cmd.CommandType = CommandType.Text;

                da.SelectCommand = (MySqlCommand)cmd;

                connection = new MySqlConnection(connectionString);
                cmd.Connection = connection;
                connection.Open();

                // fill the dataset
                da.Fill(ds);
            }
            catch
            {
                throw;  // exception occurred here
            }
            finally
            {
                if (da != null)
                    da.Dispose();
                if (cmd != null)
                    cmd.Dispose();
                // implicitly calls close()
                connection.Dispose();
            }
            return ds;
        }
        */
        public static List<T> Select<T>(string query) where T : new()
        {
            List<T> data = new List<T>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    T item = new T();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        object value = reader[i];

                        // Using reflection to set the property value of the object
                        var property = typeof(T).GetProperty(columnName);
                        if (property != null && value != DBNull.Value)
                        {
                            property.SetValue(item, value);
                        }
                    }

                    data.Add(item);
                }

                reader.Close();
            }

            return data;
        }
        public static DataTable SelectAll(string query)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dataTable.Load(reader); // Load data directly into DataTable

                reader.Close();
            }

            return dataTable;
        }

        public static T SelectSingle<T>(string query) where T : new()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                T item = default(T);

                if (reader.Read())
                {
                    item = new T();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        object value = reader[i];

                        // Using reflection to set the property value of the object
                        var property = typeof(T).GetProperty(columnName);
                        if (property != null && value != DBNull.Value)
                        {
                            property.SetValue(item, value);
                        }
                    }
                }
                reader.Close();
                return item;
            }
        }
    }
}