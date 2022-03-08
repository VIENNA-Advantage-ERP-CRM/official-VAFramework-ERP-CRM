using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace VAdvantage.Login
{
    public class ALoginRes_de_DE : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_de_DE()
        {
            contents.Add("Connection", "Verbindung");
            contents.Add("Defaults", "Standard Werte");
            contents.Add("Login", "Vienna Anmeldung");
            contents.Add("File", "Datei");
            contents.Add("Exit", "Beenden");
            contents.Add("Help", "Hilfe");
            contents.Add("About", "Über");
            contents.Add("Host", "Rechner");
            contents.Add("Database", "Datenbank");
            contents.Add("User", "Benutzer");
            contents.Add("EnterUser", "Answendungs-Benutzer eingeben");
            contents.Add("Password", "Kennwort");
            contents.Add("EnterPassword", "Anwendungs-Kennwort eingeben");
            contents.Add("Language", "Sprache");
            contents.Add("SelectLanguage", "Anwendungs-Sprache auswählen");
            contents.Add("Role", "Rolle");
            contents.Add("Client", "Mandant");
            contents.Add("Organization", "Organisation");
            contents.Add("Date", "Datum");
            contents.Add("Warehouse", "Lager");
            contents.Add("Printer", "Drucker");
            contents.Add("Connected", "Verbunden");
            contents.Add("NotConnected", "Nicht verbunden");
            contents.Add("DatabaseNotFound", "Datenbank nicht gefunden");
            contents.Add("UserPwdError", "Benutzer und Kennwort stimmen nicht überein");
            contents.Add("RoleNotFound", "Rolle nicht gefunden/komplett");
            contents.Add("Authorized", "Authorisiert");
            contents.Add("Ok", "Ok");
            contents.Add("Cancel", "Abbruch");
            contents.Add("VersionConflict", "Versions Konflikt:");
            contents.Add("VersionInfo", "Server <> Arbeitsstation");
            contents.Add("PleaseUpgrade", "Bitte das Aktualisierung-Programm (update) starten");
            contents.Add("GoodMorning", "Guten Morgen");
            contents.Add("GoodAfternoon", "Guten Tag");
            contents.Add("GoodEvening", "Guten Abend");
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
