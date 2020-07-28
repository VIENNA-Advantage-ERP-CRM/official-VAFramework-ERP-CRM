using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VAdvantage.Login
{
    public class ALoginRes_nb_NO : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();

        public ALoginRes_nb_NO()
        {
            contents.Add("Connection", "Forbindelse");
            contents.Add("Defaults", "Vanlige");
            contents.Add("Login", "Vienna Loginn");
            contents.Add("File", "Fil");
            contents.Add("Exit", "Avslutt");
            contents.Add("Help", "Hjelp");
            contents.Add("About", "Om");
            contents.Add("Host", "Maskin");
            contents.Add("Database", "Database");
            contents.Add("User", "Bruker ID");
            contents.Add("EnterUser", "Skriv  Applikasjon Bruker ID");
            contents.Add("Password", "Passord");
            contents.Add("EnterPassword", "Skriv Applikasjon Passordet");
            contents.Add("Language", "Språk");
            contents.Add("SelectLanguage", "Velg ønsket Språk");
            contents.Add("Role", "Rolle");
            contents.Add("Client", "Klient");
            contents.Add("Organization", "Organisasjon");
            contents.Add("Date", "Dato");
            contents.Add("Warehouse", "Varehus");
            contents.Add("Printer", "Skriver");
            contents.Add("Connected", "Oppkoblett");
            contents.Add("NotConnected", "Ikke Oppkoblet");
            contents.Add("DatabaseNotFound", "Database ikke funnet");
            contents.Add("UserPwdError", "Bruker passer ikke til passordet");
            contents.Add("RoleNotFound", "Role not found/complete");
            contents.Add("Authorized", "Autorisert");
            contents.Add("Ok", "Ok");
            contents.Add("Cancel", "Avbryt");
            contents.Add("VersionConflict", "Versions Konflikt:");
            contents.Add("VersionInfo", "Server <> Klient");
            contents.Add("PleaseUpgrade", "Vennligst kjør oppdaterings programet");
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
