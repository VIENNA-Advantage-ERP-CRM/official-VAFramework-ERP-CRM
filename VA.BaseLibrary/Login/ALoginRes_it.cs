using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace VAdvantage.Login
{
    public class ALoginRes_it_IT : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_it_IT()
        {
            //contents.Add( "Connection",       "Connection" );
            contents.Add("Connection", "Connessione");
            //contents.Add( "Defaults",         "Defaults" );
            contents.Add("Defaults", "Defaults"); //Need to be checked
            //contents.Add( "Login",            "Vienna Login" );
            contents.Add("Login", "Vienna Login");
            //contents.Add( "File",             "File" );
            contents.Add("File", "File");
            //contents.Add( "Exit",             "Exit" );
            contents.Add("Exit", "Esci");
            //contents.Add( "Help",             "Help" );
            contents.Add("Help", "Aiuto");
            //contents.Add( "About",            "About" );
            contents.Add("About", "Informazioni");
            //contents.Add( "Host",             "Host" );
            contents.Add("Host", "Host");
            //contents.Add( "Database",         "Database" );
            contents.Add("Database", "Database");
            //contents.Add( "User",             "User ID" ); //Need to be checked. Leave "User ID" ?
            contents.Add("User", "Identificativo Utente");
            //contents.Add( "EnterUser",        "Enter Application User ID" );
            contents.Add("EnterUser", "Identificativo Utente Applicazione");
            //contents.Add( "Password",         "Password" );
            contents.Add("Password", "Password");
            //contents.Add( "EnterPassword",    "Enter Application password" );
            contents.Add("EnterPassword", "Inserimento password Applicazione");
            //contents.Add( "Language",         "Language" );
            contents.Add("Language", "Linguaggio");
            //contents.Add( "SelectLanguage",   "Select your language" );
            contents.Add("SelectLanguage", "Selezionate il vostro linguaggio");
            //contents.Add( "Role",             "Role" );
            contents.Add("Role", "Ruolo");
            //contents.Add( "Client",           "Client" ); //Need to be checked. Everybody agree with the SAP translation ?
            contents.Add("Client", "Mandante");
            //contents.Add( "Organization",     "Organization" );
            contents.Add("Organization", "Organizzazione");
            //contents.Add( "Date",             "Date" );
            contents.Add("Date", "Data");
            //contents.Add( "Warehouse",        "Warehouse" );
            contents.Add("Warehouse", "Magazzino");
            //contents.Add( "Printer",          "Printer" );
            contents.Add("Printer", "Stampante");
            //contents.Add( "Connected",        "Connected" );
            contents.Add("Connected", "Connesso");
            //contents.Add( "NotConnected",     "Not Connected" );
            contents.Add("NotConnected", "Non Connesso");
            //contents.Add( "DatabaseNotFound", "Database not found" );
            contents.Add("DatabaseNotFound", "Database non trovato");
            //contents.Add( "UserPwdError",     "User does not match password" );
            contents.Add("UserPwdError", "L'Utente non corrisponde alla password");
            //contents.Add( "RoleNotFound",     "Role not found" );
            contents.Add("RoleNotFound", "Ruolo non trovato");
            //contents.Add( "Authorized",       "Authorized" );
            contents.Add("Authorized", "Authorizzato");
            //contents.Add( "Ok",               "Ok" );
            contents.Add("Ok", "Ok");
            //contents.Add( "Cancel",           "Cancel" );
            contents.Add("Cancel", "Cancella");
            //contents.Add( "VersionConflict",  "Version Conflict:" );
            contents.Add("VersionConflict", "Conflitto di Versione:");
            //contents.Add( "VersionInfo",      "Server <> Client" );
            contents.Add("VersionInfo", "Server <> Client");
            //contents.Add( "PleaseUpgrade",    "Please run the update program" }
            contents.Add("PleaseUpgrade", "Prego lanciare il programma di update");
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
