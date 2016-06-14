using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace UpdateSQLPassword
{
    class UpdateSQLPassword
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Constants.TOOL_STARTED_MESSAGE);
            try
            {
                string loginName = Convert.ToString(ConfigurationManager.AppSettings[Constants.LOGIN_NAME_KEY]);
                if (!string.IsNullOrEmpty(loginName))
                {
                    // Generate new random password
                    string newPassword = GenerateNewPassword();

                    // Get the old password from database
                    string oldPassword = GetCurrentPassword(loginName).Trim();
                    try
                    {
                        if (!string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(oldPassword))
                        {
                            // Update password of the SQL login
                            bool updateStatus = UpdatePassword(loginName, oldPassword, newPassword);
                            if (updateStatus)
                            {
                                // Add updated password in LoginDetails table
                                bool status = LogNewPassWord(newPassword, loginName);

                                // Call method to send an eamil
                                SendMail.SendEmail(newPassword);

                                string message = string.Format(Constants.SUCCESS_MESSAGE, oldPassword, newPassword, DateTime.Now);
                                // Add detailed log in LogInformation table
                                UpdateLogTable(message);
                            }
                            else
                            {
                                UpdateLogTable(Constants.FAILED_MESSAGE);
                                Console.WriteLine(Constants.FAILED_MESSAGE);
                                Console.WriteLine(Constants.STOP_TOOL_MESSAGE);
                                Console.ReadKey();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        UpdateLogTable(e.Message + e.InnerException);
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                        Console.WriteLine(Constants.STOP_TOOL_MESSAGE);
                        Console.ReadKey();
                    }
                }
                else
                {
                    Console.WriteLine(Constants.LOGIN_NAME_MISSING_MESSAGE);
                    Console.WriteLine(Constants.STOP_TOOL_MESSAGE);
                    Console.ReadKey();
                }
                Console.WriteLine(Constants.TOOL_COMPLETED);
                Console.ReadLine();
            }

            catch (Exception e)
            {
                UpdateLogTable(e.Message + e.InnerException);
                Console.WriteLine(Constants.LOGIN_NAME_MISSING_MESSAGE);
                Console.WriteLine(Constants.STOP_TOOL_MESSAGE);
                Console.ReadKey();
            }
        }

        /// <summary>Update log table to maintain logging information
        /// <param name="message">String message which we want to log in Database</param>
        /// </summary>
        private static void UpdateLogTable(string message)
        {
            try
            {
                object[] obj = new object[2];
                obj[0] = message;
                obj[1] = Convert.ToString(DateTime.Now);
                InsertData.InsertDBData(obj, Constants.INSERT_LOG_STORED_PROC);
            }
            catch (Exception e)
            {
                UpdateLogTable(e.Message + e.InnerException);
            }
        }

        /// <summary>Add new password in database for a given login name.
        /// <param name="newPassword">String message which we want to log in Database</param>
        /// <para name="loginName">login name for which password needs to be updated.</para>
        /// </summary>
        private static bool LogNewPassWord(string newPassword, string loginName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Convert.ToString(ConfigurationManager.ConnectionStrings[Constants.CONNECTION_STRING_KEY].ConnectionString)))
                {
                    if (con.State.Equals(ConnectionState.Closed))
                    {
                        con.Open();
                    }
                    using (SqlCommand myCommand = new SqlCommand())
                    {
                        myCommand.Connection = con;
                        myCommand.CommandText = Constants.ADD_NEW_PASSWORD_STORED_PROC;
                        myCommand.CommandType = CommandType.StoredProcedure;
                        myCommand.Parameters.Add(Constants.NEW_PASSWORD_PARMETER, SqlDbType.NChar).Value = newPassword;
                        myCommand.Parameters.Add(Constants.LOGIN_NAME_PARMETER, SqlDbType.NChar).Value = loginName;
                        myCommand.ExecuteReader();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                UpdateLogTable(e.Message + e.InnerException);
                return false;
            }
        }

        /// <summary>Get the current password for a given login name.
        /// <para name="loginName">login name for which password needs to be updated.</para>
        /// </summary>
        private static string GetCurrentPassword(string loginName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Convert.ToString(ConfigurationManager.ConnectionStrings[Constants.CONNECTION_STRING_KEY].ConnectionString)))
                {
                    if (con.State.Equals(ConnectionState.Closed))
                    {
                        con.Open();
                    }

                    // Execute 'BackupCurrentPassword' to take a back up of current password column
                    using (SqlCommand myCommand = new SqlCommand())
                    {
                        myCommand.Connection = con;
                        myCommand.CommandText = Constants.BACKUP_CURRENT_PASSWORD_STORED_PROC;
                        myCommand.CommandType = CommandType.StoredProcedure;
                        myCommand.Parameters.Add(Constants.LOGIN_NAME_PARMETER, SqlDbType.NChar).Value = loginName;
                        myCommand.ExecuteReader();
                    }

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = Constants.GET_CURRENT_PASSWORD_STORED_PROC;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(Constants.LOGIN_NAME_PARMETER, SqlDbType.NChar).Value = loginName;
                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            string currentPassword = Convert.ToString(dr[Constants.OLD_PASSWORD_FIELD]);
                            return currentPassword;
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                UpdateLogTable(e.Message + e.InnerException);
                return string.Empty;
            }

        }

        /// <summary>Generate a random password..
        /// </summary>
        private static string GenerateNewPassword()
        {
            try
            {
                int length = 10;
                string chars = Constants.ALLOWED_PASSWORD_CHARS;
                var random = new Random();
                return new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            catch (Exception e)
            {
                UpdateLogTable(e.Message + e.InnerException);
                return string.Empty;
            }
        }

        /// <summary>Update password for the given SQL server login name
        /// <para name="loginName">login name for which password needs to be updated.</para>
        /// <param name="newPassword">New password</param>
        /// <param name="oldPassword">Current password</param>
        /// </summary>
        private static bool UpdatePassword(string loginName, string oldPassword, string newPassword)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Convert.ToString(ConfigurationManager.ConnectionStrings[Constants.CONNECTION_STRING_KEY].ConnectionString)))
                {

                    // Check Connection Open or Close
                    if (con.State.Equals(ConnectionState.Closed))
                    {
                        con.Open();
                    }
                    string alterQuery = "ALTER LOGIN " + loginName + " WITH PASSWORD = '" + newPassword + "' OLD_PASSWORD = '" + oldPassword + "'";
                    using (SqlCommand cmd = new SqlCommand(alterQuery, con))
                    {
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                UpdateLogTable(e.Message + e.InnerException);
                return false;
            }
        }
    }
}
