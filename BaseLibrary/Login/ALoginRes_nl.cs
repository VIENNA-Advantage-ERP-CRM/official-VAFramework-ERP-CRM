using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace VAdvantage.Login
{
    public class ALoginRes_nl_NL : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_nl_NL()
        {
            contents.Add("Connection", "Verbinding");
            contents.Add("Defaults", "Standaard");
            contents.Add("Login", "Aanmelden bij Vienna");
            contents.Add("File", "Bestand");
            contents.Add("Exit", "Afsluiten");
            contents.Add("Help", "Help");
            contents.Add("About", "Info");
            contents.Add("Host", "Server");
            contents.Add("Database", "Database");
            contents.Add("User", "Gebruikersnaam");
            contents.Add("EnterUser", "Voer uw gebruikersnaam in");
            contents.Add("Password", "Wachtwoord");
            contents.Add("EnterPassword", "Voer uw wachtwoord in");
            contents.Add("Language", "Taal");
            contents.Add("SelectLanguage", "Selecteer uw taal");
            contents.Add("Role", "Rol");
            contents.Add("Client", "Client");
            contents.Add("Organization", "Organisatie");
            contents.Add("Date", "Datum");
            contents.Add("Warehouse", "Magazijn");
            contents.Add("Printer", "Printer");
            contents.Add("Connected", "Verbonden");
            contents.Add("NotConnected", "Niet verbonden");
            contents.Add("DatabaseNotFound", "Database niet gevonden");
            contents.Add("UserPwdError", "Foute gebruikersnaam of wachtwoord");
            contents.Add("RoleNotFound", "Rol niet gevonden of incompleet");
            contents.Add("Authorized", "Geautoriseerd");
            contents.Add("Ok", "Ok");
            contents.Add("Cancel", "Annuleren");
            contents.Add("VersionConflict", "Versie Conflict:");
            contents.Add("VersionInfo", "Server <> Client");
            contents.Add("PleaseUpgrade", "Uw Vienna installatie dient te worden bijgewerkt.");

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
