using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace VAdvantage.Login
{
    public class ALoginRes_sv_SE : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_sv_SE()
        {
            contents.Add("Connection", "Anslutning");
            contents.Add("Defaults", "Standardinställningar");
            contents.Add("Login", "Vienna inloggning");
            contents.Add("File", "Fil");
            contents.Add("Exit", "Avsluta");
            contents.Add("Help", "Hjälp");
            contents.Add("About", "Om");
            contents.Add("Host", "Värddatorn");
            contents.Add("Database", "Databas");
            contents.Add("User", "Användarnamn");
            contents.Add("EnterUser", "Ange program användarnamn");
            contents.Add("Password", "Lösenord");
            contents.Add("EnterPassword", "Ange program lösenord");
            contents.Add("Language", "Språk");
            contents.Add("SelectLanguage", "Välj ditt språk");
            contents.Add("Role", "Roll");
            contents.Add("Client", "Klient");
            contents.Add("Organization", "Organisation");
            contents.Add("Date", "Datum");
            contents.Add("Warehouse", "Lager");
            contents.Add("Printer", "Skrivare");
            contents.Add("Connected", "Anslutad");
            contents.Add("NotConnected", "Ej ansluten");
            contents.Add("DatabaseNotFound", "Hittade inte databasen");
            contents.Add("UserPwdError", "User does not match password");
            contents.Add("RoleNotFound", "Hittade inte rollen");
            contents.Add("Authorized", "Auktoriserad");
            contents.Add("Ok", "Ok");
            contents.Add("Cancel", "Avbryt");
            contents.Add("VersionConflict", "Versionskonflikt:");
            contents.Add("VersionInfo", "Server <> Klient");
            contents.Add("PleaseUpgrade", "Kör updateringsprogram");
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
