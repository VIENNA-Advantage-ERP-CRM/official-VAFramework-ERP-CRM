using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace VAdvantage.Login
{
    public class ALoginRes_fr_FR : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_fr_FR()
        {
            contents.Add("Connection", "Connexion");
            contents.Add("Defaults", "Défauts");
            contents.Add("Login", "Login Vienna");
            contents.Add("File", "Fichier");
            contents.Add("Exit", "Sortir");
            contents.Add("Help", "Aide");
            contents.Add("About", "A propos de");
            contents.Add("Host", "Serveur");
            contents.Add("Database", "Base de données");
            contents.Add("User", "Utilisateur");
            contents.Add("EnterUser", "Entrer votre code utilisateur");
            contents.Add("Password", "Mot de passe");
            contents.Add("EnterPassword", "Entrer le mot de passe");
            contents.Add("Language", "Langue");
            contents.Add("SelectLanguage", "Sélectionnez votre langue");
            contents.Add("Role", "Rôle");
            contents.Add("Client", "Société");
            contents.Add("Organization", "Département");
            contents.Add("Date", "Date");
            contents.Add("Warehouse", "Stock");
            contents.Add("Printer", "Imprimante");
            contents.Add("Connected", "Connecté");
            contents.Add("NotConnected", "Non Connecté");
            contents.Add("DatabaseNotFound", "Base de données non trouvée");
            contents.Add("UserPwdError", "L'utilisateur n'a pas entré de mot de passe");
            contents.Add("RoleNotFound", "Rôle non trouvé");
            contents.Add("Authorized", "Autorisé");
            contents.Add("Ok", "Ok");
            contents.Add("Cancel", "Annuler");
            contents.Add("VersionConflict", "Conflit de Version:");
            contents.Add("VersionInfo", "Serveur <> Client");
            contents.Add("PleaseUpgrade", "SVP, mettez à jour le programme");
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
