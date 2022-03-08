using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VAdvantage.Login
{
    public class ALoginRes_fi_FI : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_fi_FI()
        {
            contents.Add("Connection", "Yhteys");
            contents.Add("Defaults", "Oletusarvot");
            contents.Add("Login", "Vienna Login");
            contents.Add("File", "Tiedosto");
            contents.Add("Exit", "Poistu");
            contents.Add("Help", "Ohje");
            contents.Add("About", "About");
            contents.Add("Host", "Host");
            contents.Add("Database", "Tietokanta");
            contents.Add("User", "Käyttäjätunnus");
            contents.Add("EnterUser", "Anna sovelluksen käyttäjätunnus");
            contents.Add("Password", "Salasana");
            contents.Add("EnterPassword", "Anna sovelluksen salasana");
            contents.Add("Language", "Kieli");
            contents.Add("SelectLanguage", "Valitse kieli");
            contents.Add("Role", "Rooli");
            contents.Add("Client", "Client");
            contents.Add("Organization", "Organisaatio");
            contents.Add("Date", "Päivämäärä");
            contents.Add("Warehouse", "Tietovarasto");
            contents.Add("Printer", "Tulostin");
            contents.Add("Connected", "Yhdistetty");
            contents.Add("NotConnected", "Ei yhteyttä");
            contents.Add("DatabaseNotFound", "Tietokantaa ei löydy");
            contents.Add("UserPwdError", "Käyttäjätunnus ja salasana eivät vastaa toisiaan");
            contents.Add("RoleNotFound", "Roolia ei löydy tai se ei ole täydellinen");
            contents.Add("Authorized", "Valtuutettu");
            contents.Add("Ok", "Hyväksy");
            contents.Add("Cancel", "Peruuta");
            contents.Add("VersionConflict", "Versioristiriita:");
            contents.Add("VersionInfo", "Server <> Client");
            contents.Add("PleaseUpgrade", "Ole hyvä ja aja päivitysohjelma");
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
