using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace VAdvantage.Login
{
    public class ALoginRes_pt_BR : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_pt_BR()
        {
            contents.Add("Connection", "Conexão");
            contents.Add("Defaults", "Padrões");
            contents.Add("Login", "Vienna Login");
            contents.Add("File", "Arquivo");
            contents.Add("Exit", "Sair");
            contents.Add("Help", "Ajuda");
            contents.Add("About", "Sobre");
            contents.Add("Host", "Servidor");
            contents.Add("Database", "Banco de Dados");
            contents.Add("User", "ID Usuário");
            contents.Add("EnterUser", "Entre com o ID Usuário da Aplicação");
            contents.Add("Password", "Senha");
            contents.Add("EnterPassword", "Entre com a Senha da Aplicação");
            contents.Add("Language", "Idioma");
            contents.Add("SelectLanguage", "Selecione o idioma");
            contents.Add("Role", "Regra");
            contents.Add("Client", "Cliente");
            contents.Add("Organization", "Organização");
            contents.Add("Date", "Data");
            contents.Add("Warehouse", "Depósito");
            contents.Add("Printer", "Impressora");
            contents.Add("Connected", "Conectado");
            contents.Add("NotConnected", "Não conectado");
            contents.Add("DatabaseNotFound", "Banco de Dados não encontrado");
            contents.Add("UserPwdError", "Usuário/Senha inválidos");
            contents.Add("RoleNotFound", "Regra não encontrada/incorreta");
            contents.Add("Authorized", "Autorizado");
            contents.Add("Ok", "Ok");
            contents.Add("Cancel", "Cancelar");
            contents.Add("VersionConflict", "Conflito de Versões:");
            contents.Add("VersionInfo", "Servidor <> Cliente");
            contents.Add("PleaseUpgrade", "Favor executar o programa de atualização");
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
