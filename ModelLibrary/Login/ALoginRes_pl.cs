using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace VAdvantage.Login
{
    public class ALoginRes_pl_PL : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_pl_PL()
        {
            contents.Add("Connection", "Po\u0142\u0105czenie");
            contents.Add("Defaults", "Domy\u015blne");
            contents.Add("Login", "Logowanie");
            contents.Add("File", "Plik");
            contents.Add("Exit", "Zako\u0144cz");
            contents.Add("Help", "Pomoc");
            contents.Add("About", "O aplikacji");
            contents.Add("Host", "Host");
            contents.Add("Database", "Baza danych");
            contents.Add("User", "U\u017cytkownik");
            contents.Add("EnterUser", "Wprowad\u017a Identyfikator U\u017cytkownika Aplikacji");
            contents.Add("Password", "Has\u0142o");
            contents.Add("EnterPassword", "Wprowad\u017a Has\u0142o Aplikacji");
            contents.Add("Language", "J\u0119zyk");
            contents.Add("SelectLanguage", "Wybierz j\u0119zyk");
            contents.Add("Role", "Funkcja");
            contents.Add("Client", "Klient");
            contents.Add("Organization", "Organizacja");
            contents.Add("Date", "Data");
            contents.Add("Warehouse", "Magazyn");
            contents.Add("Printer", "Drukarka");
            contents.Add("Connected", "Po\u0142\u0105czony");
            contents.Add("NotConnected", "Nie Po\u0142\u0105czony");
            contents.Add("DatabaseNotFound", "Nie znaleziono bazy danych");
            contents.Add("UserPwdError", "Has\u0142o nie odpowiada U\u017cytkownikowi");
            contents.Add("RoleNotFound", "Nie znaleziono zasady");
            contents.Add("Authorized", "Autoryzowany");
            contents.Add("Ok", "Ok");
            contents.Add("Cancel", "Anuluj");
            contents.Add("VersionConflict", "Konflikt wersji:");
            contents.Add("VersionInfo", "Serwer <> Klienta");
            contents.Add("PleaseUpgrade", "Uruchom now\u0105 wersj\u0119 programu !");
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
