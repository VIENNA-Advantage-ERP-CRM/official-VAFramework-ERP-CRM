using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace VAdvantage.Login
{
    public class ALoginRes_sl_SI : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_sl_SI()
        {
            contents.Add("Connection", "Povezava");
            contents.Add("Defaults", "Privzete vrednosti");
            contents.Add("Login", "Prijava");
            contents.Add("File", "Datoteka");
            contents.Add("Exit", "Izhod");
            contents.Add("Help", "Pomo\u010d");
            contents.Add("About", "O programu");
            contents.Add("Host", "Stre\u017enik");
            contents.Add("Database", "Baza podatkov");
            contents.Add("User", "Uporabnik");
            contents.Add("EnterUser", "Vpi\u0161i uporabnika");
            contents.Add("Password", "Geslo");
            contents.Add("EnterPassword", "Vpi\u0161i geslo");
            contents.Add("Language", "Jezik");
            contents.Add("SelectLanguage", "Izbira jezika");
            contents.Add("Role", "Vloga");
            contents.Add("Client", "Podjetje");
            contents.Add("Organization", "Organizacija");
            contents.Add("Date", "Datum");
            contents.Add("Warehouse", "Skladi\u0161\u010de");
            contents.Add("Printer", "Tiskalnik");
            contents.Add("Connected", "Povezano");
            contents.Add("NotConnected", "Ni povezano");
            contents.Add("DatabaseNotFound", "Ne najdem baze podatkov");
            contents.Add("UserPwdError", "Geslo ni pravilno");
            contents.Add("RoleNotFound", "Ne najdem izbrane vloge");
            contents.Add("Authorized", "Avtoriziran");
            contents.Add("Ok", "V redu");
            contents.Add("Cancel", "Prekli\u010di");
            contents.Add("VersionConflict", "Konflikt verzij");
            contents.Add("VersionInfo", "Stre\u017enik <> Odjemalec");
            contents.Add("PleaseUpgrade", "Prosim nadgradite program");
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
