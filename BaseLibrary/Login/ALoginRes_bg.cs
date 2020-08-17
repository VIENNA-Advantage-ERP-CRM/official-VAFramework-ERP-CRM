using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace VAdvantage.Login
{
    public class ALoginRes_bg_BG : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();

        public ALoginRes_bg_BG()
        {

            contents.Add("Connection", "\u0412\u0440\u044a\u0437\u043a\u0430");
            contents.Add("Defaults", "\u041f\u043e \u043f\u043e\u0434\u0440\u0430\u0437\u0431\u0438\u0440\u0430\u043d\u0435");
            contents.Add("Login", "\u0421\u0432\u044a\u0440\u0437\u0432\u0430\u043d\u0435 \u0438 \u043e\u0442\u043e\u0440\u0438\u0437\u0430\u0446\u0438\u044f");
            contents.Add("File", "\u0424\u0430\u0439\u043b");
            contents.Add("Exit", "\u0418\u0437\u0445\u043e\u0434");
            contents.Add("Help", "\u041f\u043e\u043c\u043e\u0449");
            contents.Add("About", "\u0417\u0430 ..");
            contents.Add("Host", "\u0421\u044a\u0440\u0432\u0435\u0440");
            contents.Add("Database", "\u0411\u0430\u0437\u0430 \u0434\u0430\u043d\u043d\u0438");
            contents.Add("User", "\u041f\u043e\u0442\u0440\u0435\u0431\u0438\u0442\u0435\u043b");
            contents.Add("EnterUser", "\u0412\u044a\u0432\u0435\u0434\u0435\u0442\u0435 \u043f\u043e\u0442\u0440\u0435\u0431\u0438\u0442\u0435\u043b \u043d\u0430 \u043f\u0440\u0438\u043b\u043e\u0436\u0435\u043d\u0438\u0435\u0442\u043e");
            contents.Add("Password", "\u041f\u0430\u0440\u043e\u043b\u0430");
            contents.Add("EnterPassword", "\u0412\u044a\u0432\u0435\u0434\u0435\u0442\u0435 \u043f\u0430\u0440\u043e\u043b\u0430 \u0437\u0430 \u043f\u043e\u0442\u0440\u0435\u0431\u0438\u0442\u0435\u043b\u044f \u043d\u0430 \u043f\u0440\u0438\u043b\u043e\u0436\u0435\u043d\u0438\u0435\u0442\u043e");
            contents.Add("Language", "\u0415\u0437\u0438\u043a");
            contents.Add("SelectLanguage", "\u0418\u0437\u0431\u0435\u0440\u0435\u0442\u0435 \u0412\u0430\u0448\u0438\u044f \u0435\u0437\u0438\u043a");
            contents.Add("Role", "\u041f\u0440\u0438\u0432\u0438\u043b\u0435\u0433\u0438\u0438");
            contents.Add("Client", "\u041a\u043b\u0438\u0435\u043d\u0442(\u0445\u043e\u043b\u0434\u0438\u043d\u0433)");
            contents.Add("Organization", "\u041e\u0440\u0433\u0430\u043d\u0438\u0437\u0430\u0446\u0438\u044f(\u0444\u0438\u0440\u043c\u0430)");
            contents.Add("Date", "\u0414\u0430\u0442\u0430");
            contents.Add("Warehouse", "\u0421\u043a\u043b\u0430\u0434");
            contents.Add("Printer", "\u041f\u0440\u0438\u043d\u0442\u0435\u0440");
            contents.Add("Connected", "\u0412\u0440\u044a\u0437\u043a\u0430\u0442\u0430 \u0435 \u043e\u0441\u044a\u0449\u0435\u0441\u0442\u0432\u0435\u043d\u0430");
            contents.Add("NotConnected", "\u0412\u0440\u044a\u0437\u043a\u0430\u0442\u0430 \u043d\u0435 \u0435 \u043e\u0441\u044a\u0449\u0435\u0441\u0442\u0432\u0435\u043d\u0430");
            contents.Add("DatabaseNotFound", "\u041b\u0438\u043f\u0441\u0432\u0430 \u0431\u0430\u0437\u0430 \u0434\u0430\u043d\u043d\u0438");
            contents.Add("UserPwdError", "\u041f\u043e\u0442\u0440\u0435\u0431\u0438\u0442\u0435\u043b\u044f \u0438/\u0438\u043b\u0438 \u043f\u0430\u0440\u043e\u043b\u0430\u0442\u0430 \u0441\u0430 \u0433\u0440\u0435\u0448\u043d\u0438");
            contents.Add("RoleNotFound", "\u041b\u0438\u043f\u0441\u0432\u0430\u0449\u0438 \u043f\u0440\u0438\u0432\u0438\u043b\u0435\u0433\u0438\u0438");
            contents.Add("Authorized", "\u041e\u0442\u043e\u0440\u0438\u0437\u0430\u0446\u0438\u044f\u0442\u0430 \u0435 \u0443\u0441\u043f\u0435\u0448\u043d\u0430");
            contents.Add("Ok", "\u0414\u0430");
            contents.Add("Cancel", "\u041e\u0442\u043a\u0430\u0437");
            contents.Add("VersionConflict", "\u041a\u043e\u043d\u0444\u043b\u0438\u043a\u0442 \u043d\u0430 \u0432\u0435\u0440\u0441\u0438\u0438\u0442\u0435:");
            contents.Add("VersionInfo", "\u0421\u044a\u0440\u0432\u0435\u0440 <> \u041a\u043b\u0438\u0435\u043d\u0442");
            contents.Add("PleaseUpgrade", "\u041c\u043e\u043b\u044f \u0441\u0442\u0430\u0440\u0442\u0438\u0440\u0430\u0439\u0442\u0435 \u043f\u0440\u043e\u0433\u0440\u0430\u043c\u0430 \u0437\u0430 \u043e\u0431\u043d\u043e\u0432\u044f\u0432\u0430\u043d\u0435 \u043d\u0430 \u0432\u0435\u0440\u0441\u0438\u044f\u0442\u0430");
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
