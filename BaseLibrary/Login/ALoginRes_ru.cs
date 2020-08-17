using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace VAdvantage.Login
{
    public class ALoginRes_ru_RU : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_ru_RU()
        {
            contents.Add("Connection", "\u0421\u043e\u0435\u0434\u0438\u043d\u0435\u043d\u0438\u0435");
            contents.Add("Defaults", "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u044f \u043f\u043e \u0443\u043c\u043e\u043b\u0447\u0430\u043d\u0438\u044e");
            contents.Add("Login", "\u0412\u0445\u043e\u0434 \u0432 Vienna");
            contents.Add("File", "\u0424\u0430\u0439\u043b");
            contents.Add("Exit", "\u0412\u044b\u0445\u043e\u0434");
            contents.Add("Help", "\u041f\u043e\u043c\u043e\u0449\u044c");
            contents.Add("About", "\u041e \u043f\u0440\u043e\u0433\u0440\u0430\u043c\u043c\u0435");
            contents.Add("Host", "\u0425\u043e\u0441\u0442");
            contents.Add("Database", "\u0411\u0430\u0437\u0430 \u0434\u0430\u043d\u043d\u044b\u0445");
            contents.Add("User", "\u041f\u043e\u043b\u044c\u0437\u043e\u0432\u0430\u0442\u0435\u043b\u044c");
            contents.Add("EnterUser", "\u0412\u0432\u0435\u0434\u0438\u0442\u0435 \u043f\u043e\u043b\u044c\u0437\u043e\u0432\u0430\u0442\u0435\u043b\u044f");
            contents.Add("Password", "\u041f\u0430\u0440\u043e\u043b\u044c");
            contents.Add("EnterPassword", "\u0412\u0432\u0435\u0434\u0438\u0442\u0435 \u043f\u0430\u0440\u043e\u043b\u044c");
            contents.Add("Language", "\u042f\u0437\u044b\u043a");
            contents.Add("SelectLanguage", "\u0412\u044b\u0431\u0435\u0440\u0438\u0442\u0435 \u0432\u0430\u0448 \u044f\u0437\u044b\u043a");
            contents.Add("Role", "\u0420\u043e\u043b\u044c");
            contents.Add("Client", "\u041a\u043b\u0438\u0435\u043d\u0442");
            contents.Add("Organization", "\u041e\u0440\u0433\u0430\u043d\u0438\u0437\u0430\u0446\u0438\u044f");
            contents.Add("Date", "\u0414\u0430\u0442\u0430");
            contents.Add("Warehouse", "\u0421\u043a\u043b\u0430\u0434");
            contents.Add("Printer", "\u041f\u0440\u0438\u043d\u0442\u0435\u0440");
            contents.Add("Connected", "\u0421\u043e\u0435\u0434\u0438\u043d\u0435\u043d\u043e");
            contents.Add("NotConnected", "\u041d\u0435 \u0441\u043e\u0435\u0434\u0438\u043d\u0435\u043d\u043e");
            contents.Add("DatabaseNotFound", "\u0411\u0430\u0437\u0430 \u0434\u0430\u043d\u043d\u044b\u0445 \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d\u0430");
            contents.Add("UserPwdError", "\u041d\u0435 \u0432\u0435\u0440\u043d\u044b\u0439 \u043f\u0430\u0440\u043e\u043b\u044c");
            contents.Add("RoleNotFound", "\u041d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d\u0430 \u0440\u043e\u043b\u044c");
            contents.Add("Authorized", "\u0410\u0432\u0442\u043e\u0440\u0438\u0437\u043e\u0432\u0430\u043d");
            contents.Add("Ok", "\u0414\u0430");
            contents.Add("Cancel", "\u041e\u0442\u043c\u0435\u043d\u0430");
            contents.Add("VersionConflict", "\u041a\u043e\u043d\u0444\u043b\u0438\u043a\u0442 \u0432\u0435\u0440\u0441\u0438\u0439:");
            contents.Add("VersionInfo", "\u0421\u0435\u0440\u0432\u0435\u0440 <> \u041a\u043b\u0438\u0435\u043d\u0442");
            contents.Add("PleaseUpgrade", "\u041f\u043e\u0436\u0430\u043b\u0443\u0439\u0441\u0442\u0430 \u0437\u0430\u043f\u0443\u0441\u0442\u0438\u0442\u0435 \u043f\u0440\u043e\u0433\u0440\u0430\u043c\u043c\u0443 \u043e\u0431\u043d\u043e\u0432\u043b\u0435\u043d\u0438\u044f");
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
