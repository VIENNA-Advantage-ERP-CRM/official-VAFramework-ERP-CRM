using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace VAdvantage.Login
{
    public class ALoginRes_hr_HR : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_hr_HR()
        {
            contents.Add("Connection", "Veza");
            contents.Add("Defaults", "Uobièajeno");
            contents.Add("Login", "Vienna Login");
            contents.Add("File", "Datoteka");
            contents.Add("Exit", "Izlaz");
            contents.Add("Help", "Pomoæ");
            contents.Add("About", "O programu");
            contents.Add("Host", "Host");
            contents.Add("Database", "Baza podataka");
            contents.Add("User", "Korisnik");
            contents.Add("EnterUser", "Unos korisnika");
            contents.Add("Password", "Lozinka");
            contents.Add("EnterPassword", "Unos lozinke");
            contents.Add("Language", "Jezika");
            contents.Add("SelectLanguage", "Izbor jezika");
            contents.Add("Role", "Uloga");
            contents.Add("Client", "Klijent");
            contents.Add("Organization", "Organizacija");
            contents.Add("Date", "Datum");
            contents.Add("Warehouse", "Skladište");
            contents.Add("Printer", "Pisac");
            contents.Add("Connected", "Spojeno");
            contents.Add("NotConnected", "Nije spojeno");
            contents.Add("DatabaseNotFound", "Baza podataka nije pronadena");
            contents.Add("UserPwdError", "Lozinka ne odgovara korisniku");
            contents.Add("RoleNotFound", "Uloga nije pronadena");
            contents.Add("Authorized", "Autoriziran");
            contents.Add("Ok", "U redu");
            contents.Add("Cancel", "Otkazati");
            contents.Add("VersionConflict", "Konflikt verzija");
            contents.Add("VersionInfo", "Server <> Klijent");
            contents.Add("PleaseUpgrade", "Molim pokrenite nadogradnju programa");
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
