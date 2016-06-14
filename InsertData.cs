using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace UpdateSQLPassword
{
    public class InsertData
    {
        public static void InsertDBData(object[] obj, string str)
        {
            try
            {
                // Get Length of Object pass from View Page
                int objectLength = obj.Length;

                // Create Object to get store procedure parameters
                object[] storedProcParameters = new object[objectLength];
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[Constants.CONNECTION_STRING_KEY].ConnectionString.ToString()))
                {
                    // Check Connection Open or Close
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    // Start Code to get SQL parameter from Stored Procedure

                    SqlCommand command = new SqlCommand();
                    command.Connection = con;
                    command.CommandText = str;
                    command.CommandType = CommandType.StoredProcedure;

                    SqlCommandBuilder.DeriveParameters(command);

                    for (int i = 0; i < command.Parameters.Count - 1; i++)
                    {
                        storedProcParameters[i] = command.Parameters[i + 1].ParameterName.ToString();
                    }
                    // End code to get SQL parameter from Stored Procedure
                    // Start Code to Insert data into table using Stored Procedure
                    using (SqlCommand cmd = new SqlCommand(str, con))
                    {

                        for (int i = 0; i < obj.Length; i++)
                        {
                            SqlParameter sp = new SqlParameter();
                            sp.ParameterName = storedProcParameters[i].ToString();
                            sp.Value = obj[i];
                            cmd.Parameters.Add(sp);
                        }

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                    }
                }
                //End Code to Insert data into table using stored procedure
            }

            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
