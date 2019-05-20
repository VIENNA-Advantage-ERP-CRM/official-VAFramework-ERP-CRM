using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace VAdvantage.Login
{
    public class ALoginRes_es_UY : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();

        public ALoginRes_es_UY()
        {

            contents.Add("Connection", "Conección");
            contents.Add("Defaults", "Predeterminados");
            contents.Add("Login", "Acceso");
            contents.Add("File", "Archivo");
            contents.Add("Exit", "Salir");
            contents.Add("Help", "Ayuda");
            contents.Add("About", "Acerca de");
            contents.Add("Host", "Huesped");
            contents.Add("Database", "Base de Datos");
            contents.Add("User", "Usuario");
            contents.Add("EnterUser", "Introduzca ID de usuario de aplicaciones");
            contents.Add("Password", "Contraseña");
            contents.Add("EnterPassword", "Introduzca Aplicación Contraseña");
            contents.Add("Language", "Idioma");
            contents.Add("SelectLanguage", "Seleccione su idioma");
            contents.Add("Role", "Rol");
            contents.Add("Client", "Cliente");
            contents.Add("Organization", "Oganización");
            contents.Add("Date", "Fecha");
            contents.Add("Warehouse", "Almacén");
            contents.Add("Printer", "Impresora");
            contents.Add("Connected", "Conectado");
            contents.Add("NotConnected", "No conectado");
            contents.Add("DatabaseNotFound", "Base de datos que no se encuentran");
            contents.Add("UserPwdError", "Usuario no coincide con la contraseña");
            contents.Add("RoleNotFound", "Papel no encontrado / completa");
            contents.Add("Authorized", "Autorizado");
            contents.Add("Ok", "Aceptar");
            contents.Add("Cancel", "Cancelar");
            contents.Add("VersionConflict", "conflicto de versiones");
            contents.Add("VersionInfo", "Servidor <> Cliente");
            contents.Add("PleaseUpgrade", "Por favor, descargue la nueva versión del servidor");

            //New Resource

            contents.Add("Back", "Atrás");
            contents.Add("SelectRole", "Seleccione Rol");
            contents.Add("SelectOrg", "Seleccione Organización");
            contents.Add("SelectClient", "Seleccione Cliente");
            contents.Add("SelectWarehouse", "Seleccione Almacen");
            contents.Add("VerifyUserLanguage", "Verificando usuario y el idioma");

            contents.Add("LoadingPreference", "Cargando Configuaciones");
            contents.Add("Completed", "Completado");
            contents.Add("DefaultLoginLanguage", "Idioma Predeterminado");
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
