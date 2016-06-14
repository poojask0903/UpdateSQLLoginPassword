using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateSQLPassword
{
    public static class Constants
    {
        // Console messages region
        # region 
        public static string TOOL_STARTED_MESSAGE = "Tool started....to change the password.";
        public static string FAILED_MESSAGE = "Failed while updating password.";
        public static string LOGIN_NAME_MISSING_MESSAGE = "Please add login name in configuration file.";
        public static string MISSING_API_KEY_VALUE = "Please provide API Key value in configuration file.";
        public static string MISSING_DOMAIN_KEY_VALUE = "Please provide domain value in configurtion file.";
        public static string MISSING_FROM_EMAIL_KEY_VALUE = "Please provide From: email value in configuration file.";
        public static string MISSING_TO_EMAIL_KEY_VALUE = "Please provide To: email value in configuration file.";
        public static string STOP_TOOL_MESSAGE = "Please enter to stop the tool.";
        public static string MISSING_SUBJECT_MESSAGE = "Please add email subject in configuration file.";
        public static string TOOL_COMPLETED = "Tool Completed. Please press enter key.";
        #endregion

        // region for app.config keys
        # region
        public static string LOGIN_NAME_KEY = "LoginName";
        public static string CONNECTION_STRING_KEY = "DefaultConnection";

        # endregion

        // Stored Procedure regions
        # region
        public static string INSERT_LOG_STORED_PROC = "InsertLogInformation";
        public static string ADD_NEW_PASSWORD_STORED_PROC = "AddNewPassword";
        public static string NEW_PASSWORD_PARMETER = "@NewPassword";
        public static string LOGIN_NAME_PARMETER = "@LoginName";
        public static string BACKUP_CURRENT_PASSWORD_STORED_PROC = "BackupCurrentPassword";
        public static string GET_CURRENT_PASSWORD_STORED_PROC = "GetCurrentPassword";
        public static string OLD_PASSWORD_FIELD = "OldPassword";
        #endregion
    
        public static string ALLOWED_PASSWORD_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public static string SUCCESS_MESSAGE = "Successfully updated password. Updated [LoginDetails] table. Updated password from {0} to {1} on {2}.";

        // Email configuration region
        #region
        public static string MAIL_BODY = "<p>Hello All:</P>"+ 
            "<p>Successfully updtated password for SQL Server <Name of the SQL Server>.</p>" + 
            "Updated password is - {0}." +
            "<p><i>*** This is an automatically generated email, please do not reply ***</i></p>";

        #endregion 
    }
}
