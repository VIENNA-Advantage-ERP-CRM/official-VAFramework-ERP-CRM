using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;


namespace VAdvantage.Login
{
    public class ALoginRes : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();

        public ALoginRes()
        {
            contents.Add("Connection", "Connection");
            contents.Add("Defaults", "Defaults");
            contents.Add("Login", "Vienna Login");
            contents.Add("File", "&File");
            contents.Add("Exit", "Exit");
            contents.Add("Help", "&Help");
            contents.Add("About", "About");
            contents.Add("Host", "&Server");
            contents.Add("Database", "Database");
            contents.Add("User", "&User ID");
            contents.Add("EnterUser", "Enter Application User ID");
            contents.Add("Password", "&Password");
            contents.Add("EnterPassword", "Enter Application Password");
            contents.Add("Language", "&Language");
            contents.Add("SelectLanguage", "Select your language");
            contents.Add("Role", "&Role");
            contents.Add("Client", "&Client");
            contents.Add("Organization", "&Organization");
            contents.Add("Date", "&Date");
            contents.Add("Warehouse", "&Warehouse");
            contents.Add("Printer", "Prin&ter");
            contents.Add("Connected", "Connected");
            contents.Add("NotConnected", "Not Connected");
            contents.Add("DatabaseNotFound", "Database not found");
            contents.Add("UserPwdError", "User does not match password");
            contents.Add("RoleNotFound", "Role not found/complete");
            contents.Add("Authorized", "Authorized");
            contents.Add("Ok", "&Ok");
            contents.Add("Cancel", "&Cancel");
            contents.Add("VersionConflict", "Version Conflict:");
            contents.Add("VersionInfo", "Server <> Client");
            contents.Add("PleaseUpgrade", "Please download new Version from Server");
            contents.Add("Configure", "Configure Application");
            contents.Add("GetDefaultPort", "Get Default Port for database");
            contents.Add("DatabaseName", "Databae Name");
            contents.Add("DatabaseType", "Database Type");
            contents.Add("TestConnection", "Test Connection");
            contents.Add("Success", "Success");
            contents.Add("Failed", "Failed");
            contents.Add("AppsType", "Application Type");
            contents.Add("AppsHost", "Application Host");
            contents.Add("AppsPort", "Application Port");
            contents.Add("TestApps", "Test Application Server");
            contents.Add("CConnectionDialog", "VAdvantage Connection");
            contents.Add("ServerNotActive", "Test Application");
            contents.Add("ConnectionError", "VAdvantage Connection");
            contents.Add("GoodMorning", "Good Morning");
            contents.Add("GoodAfternoon", "Good Afternoon");
            contents.Add("GoodEvening", "Good Evening");
        }


        /// <summary>
        /// Get Resource text
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>resource text</returns>
        public override string GetString(string key)
        {
            if (!contents.ContainsKey(key.ToString()))
                return key;
            return contents[key].ToString();
        }

    }
}
