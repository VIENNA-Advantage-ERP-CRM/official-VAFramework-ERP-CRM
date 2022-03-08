using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace VAdvantage.Login
{
    public class ALoginRes_es : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_es()
        {
            contents.Add("Connection", "Conexión");
            contents.Add("Defaults", "Valores por Defecto");
            contents.Add("Login", "Login");
            contents.Add("File", "Archivo");
            contents.Add("Exit", "Salir");
            contents.Add("Help", "Ayuda");
            contents.Add("About", "Acerca de");
            contents.Add("Host", "Servidor");
            contents.Add("Database", "Base de datos");
            contents.Add("User", "ID de usuario");
            contents.Add("EnterUser", "ID de aplicacion de usuario");
            contents.Add("Password", "Contraseña");
            contents.Add("EnterPassword", "Ingrese Contraseña");
            contents.Add("Language", "Lenguaje");
            contents.Add("SelectLanguage", "Seleccione su lenguaje");
            contents.Add("Role", "Perfil");
            contents.Add("Client", "Cliente");
            contents.Add("Organization", "Organización");
            contents.Add("Date", "Fecha");
            contents.Add("Warehouse", "Depósito");
            contents.Add("Printer", "Impresora");
            contents.Add("Connected", "Conectado");
            contents.Add("NotConnected", "No Conectado");
            contents.Add("DatabaseNotFound", "Base de datos no encontrada");
            contents.Add("UserPwdError", "Usuario-Contraseña no coincide");
            contents.Add("RoleNotFound", "Perfil no encontrado");
            contents.Add("Authorized", "Autorizado");
            contents.Add("Ok", "Aceptar");
            contents.Add("Cancel", "Cancelar");
            contents.Add("VersionConflict", "Conflicto de versión:");
            contents.Add("VersionInfo", "Servidor <> Cliente");
            contents.Add("PleaseUpgrade", "Favor de ejecutar Programa de actualización");
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
