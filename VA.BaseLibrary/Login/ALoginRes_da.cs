using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VAdvantage.Login
{
    public class ALoginRes_da_DK : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_da_DK()
        {
            contents.Add("Connection", "Forbindelse");
            contents.Add("Defaults", "Basis");
            contents.Add("Login", "Vienna: Log pÃ¥");
            contents.Add("File", "Fil");
            contents.Add("Exit", "Afslut");
            contents.Add("Help", "HjÃ¦lp");
            contents.Add("About", "Om");
            contents.Add("Host", "VÃ¦rt");
            contents.Add("Database", "Database");
            contents.Add("User", "Bruger-ID");
            contents.Add("EnterUser", "Angiv bruger-ID til program");
            contents.Add("Password", "Adgangskode");
            contents.Add("EnterPassword", "Angiv adgangskode til program");
            contents.Add("Language", "Sprog");
            contents.Add("SelectLanguage", "VÃ¦lg sprog");
            contents.Add("Role", "Rolle");
            contents.Add("Client", "Firma");
            contents.Add("Organization", "Organisation");
            contents.Add("Date", "Dato");
            contents.Add("Warehouse", "Lager");
            contents.Add("Printer", "Printer");
            contents.Add("Connected", "Forbindelse OK");
            contents.Add("NotConnected", "Ingen forbindelse");
            contents.Add("DatabaseNotFound", "Database blev ikke fundet");
            contents.Add("UserPwdError", "Forkert bruger til adgangskode");
            contents.Add("RoleNotFound", "Rolle blev ikke fundet/afsluttet");
            contents.Add("Authorized", "Tilladelse OK");
            contents.Add("Ok", "OK");
            contents.Add("Cancel", "AnnullÃ©r");
            contents.Add("VersionConflict", "Konflikt:");
            contents.Add("VersionInfo", "Server <> Klient");
            contents.Add("PleaseUpgrade", "KÃ¸r opdateringsprogram");
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
