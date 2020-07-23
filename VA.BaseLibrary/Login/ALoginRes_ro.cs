using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VAdvantage.Login
{
    public class ALoginRes_ro_RO : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_ro_RO()
        {
            contents.Add("Connection", "Conexiune");
            contents.Add("Defaults", "Valori implicite");
            contents.Add("Login", "Autentificare");
            contents.Add("File", "Aplica\u0163ie");
            contents.Add("Exit", "Ie\u015fire");
            contents.Add("Help", "Ajutor");
            contents.Add("About", "Despre...");
            contents.Add("Host", "Server");
            contents.Add("Database", "Baz\u0103 de date");
            contents.Add("User", "Utilizator");
            contents.Add("EnterUser", "Introduce\u0163i identificatorul utilizatorului");
            contents.Add("Password", "Parol\u0103");
            contents.Add("EnterPassword", "Introduce\u0163i parola");
            contents.Add("Language", "Limb\u0103");
            contents.Add("SelectLanguage", "Alege\u0163i limba dumneavoastr\u0103");
            contents.Add("Role", "Rol");
            contents.Add("Client", "Titular");
            contents.Add("Organization", "Organiza\u0163ie");
            contents.Add("Date", "Dat\u0103");
            contents.Add("Warehouse", "Depozit");
            contents.Add("Printer", "Imprimant\u0103");
            contents.Add("Connected", "Conectat");
            contents.Add("NotConnected", "Neconectat");
            contents.Add("DatabaseNotFound", "Baza de date nu a fost g\u0103sit\u0103");
            contents.Add("UserPwdError", "Parola nu se potrive\u015fte cu utilizatorul");
            contents.Add("RoleNotFound", "Rolul nu a fost g\u0103sit sau este incomplet");
            contents.Add("Authorized", "Autorizat");
            contents.Add("Ok", "OK");
            contents.Add("Cancel", "Anulare");
            contents.Add("VersionConflict", "Conflict de versiune:");
            contents.Add("VersionInfo", "server <> client");
            contents.Add("PleaseUpgrade", "Rula\u0163i programul de actualizare");

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
