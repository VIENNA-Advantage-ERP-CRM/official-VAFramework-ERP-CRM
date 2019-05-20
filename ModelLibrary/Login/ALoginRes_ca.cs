using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VAdvantage.Login
{
    public class ALoginRes_ca_ES : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();

        public ALoginRes_ca_ES()
        {
            contents.Add("Connection", "Connexió");
            contents.Add("Defaults", "Predeterminats");
            contents.Add("Login", "Accés Vienna");
            contents.Add("File", "Fitxer");
            contents.Add("Exit", "Sortir");
            contents.Add("Help", "Ajuda");
            contents.Add("About", "Referent");
            contents.Add("Host", "Servidor");
            contents.Add("Database", "Base de Dades");
            contents.Add("User", "ID Usuari");
            contents.Add("EnterUser", "Entrar ID Usuari Aplicació");
            contents.Add("Password", "Contrasenya");
            contents.Add("EnterPassword", "Entrar Contrasenya Usuari Aplicació");
            contents.Add("Language", "Idioma");
            contents.Add("SelectLanguage", "Seleccioneu el Vostre Idioma");
            contents.Add("Role", "Rol");
            contents.Add("Client", "Client");
            contents.Add("Organization", "Organització");
            contents.Add("Date", "Data");
            contents.Add("Warehouse", "Magatzem");
            contents.Add("Printer", "Impressora");
            contents.Add("Connected", "Connectat");
            contents.Add("NotConnected", "No Connectat");
            contents.Add("DatabaseNotFound", "No s'ha trobat la Base de Dades");
            contents.Add("UserPwdError", "No coincidèix l'Usuari i la Contrasenya");
            contents.Add("RoleNotFound", "Rol no trobat/completat");
            contents.Add("Authorized", "Autoritzat");
            contents.Add("Ok", "D'Acord");
            contents.Add("Cancel", "Cancel.lar");
            contents.Add("VersionConflict", "Conflicte Versions:");
            contents.Add("VersionInfo", "Servidor <> Client");
            contents.Add("PleaseUpgrade", "Sisplau Actualitzeu el Programa");
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
