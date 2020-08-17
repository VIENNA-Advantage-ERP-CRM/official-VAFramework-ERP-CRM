using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace VAdvantage.Login
{
    public class ALoginRes_ar_TN : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_ar_TN()
        {
            contents.Add("Connection", "\u0631\u0628\u0637");
            contents.Add("Defaults", "\u0627\u0650\u0641\u062a\u0631\u0627\u0636\u064a");
            contents.Add("Login", "\u0639\u0628\u0648\u0631 \u0643\u0645\u0628\u064a\u0631");
            contents.Add("File", "\u0645\u0644\u0641");
            contents.Add("Exit", "\u062e\u0631\u0648\u062c");
            contents.Add("Help", "\u0645\u0633\u0627\u0639\u062f\u0629");
            contents.Add("About", "\u0646\u0628\u0630\u0629");
            contents.Add("Host", "\u0645\u0648\u0632\u0639");
            contents.Add("Database", "\u0642\u0627\u0639\u062f\u0629 \u0628\u064a\u0627\u0646\u0627\u062a");
            contents.Add("User", "\u0645\u0639\u0631\u0651\u0641 \u0627\u0644\u0645\u0633\u062a\u0639\u0645\u0644");
            contents.Add("EnterUser", "\u0623\u062f\u062e\u0644 \u0645\u0639\u0631\u0651\u0641 \u0645\u0633\u062a\u0639\u0645\u0644 \u0627\u0644\u062a\u0637\u0628\u064a\u0642");
            contents.Add("Password", "\u0643\u0644\u0645\u0629 \u0627\u0644\u0633\u0631");
            contents.Add("EnterPassword", "\u0623\u062f\u062e\u0644 \u0643\u0644\u0645\u0629 \u0633\u0631 \u0627\u0644\u062a\u0637\u0628\u064a\u0642");
            contents.Add("Language", "\u0627\u0644\u0644\u0651\u064f\u063a\u0629");
            contents.Add("SelectLanguage", "\u0627\u0650\u062e\u062a\u0631 \u0644\u063a\u062a\u0643");
            contents.Add("Role", "\u0627\u0644\u062f\u0651\u064e\u0648\u0631");
            contents.Add("Client", "\u0648\u0643\u064a\u0644");
            contents.Add("Organization", "\u0627\u0644\u0645\u0624\u0633\u0651\u064e\u0633\u0629");
            contents.Add("Date", "\u0627\u0644\u062a\u0627\u0631\u064a\u062e");
            contents.Add("Warehouse", "\u0627\u0644\u0645\u062e\u0632\u0646");
            contents.Add("Printer", "\u0627\u0644\u0622\u0644\u0629 \u0627\u0644\u0637\u0651\u064e\u0627\u0628\u0639\u0629");
            contents.Add("Connected", "\u0645\u0631\u062a\u0628\u0637");
            contents.Add("NotConnected", "\u063a\u064a\u0631 \u0645\u0631\u062a\u0628\u0637");
            contents.Add("DatabaseNotFound", "\u0642\u0627\u0639\u062f\u0629 \u0627\u0644\u0628\u064a\u0627\u0646\u0627\u062a \u0645\u0641\u0642\u0648\u062f\u0629");
            contents.Add("UserPwdError", "\u0627\u0644\u0645\u0633\u062a\u0639\u0645\u0644 \u0644\u0627 \u064a\u0648\u0627\u0626\u0645 \u0643\u0644\u0645\u0629 \u0627\u0644\u0633\u0631");
            contents.Add("RoleNotFound", "\u0627\u0644\u062f\u0651\u064e\u0648\u0631 \u0645\u0641\u0642\u0648\u062f");
            contents.Add("Authorized", "\u0645\u0633\u0645\u0648\u062d \u0644\u0647");
            contents.Add("Ok", "\u0646\u0639\u0645");
            contents.Add("Cancel", "\u0625\u0644\u063a\u0627\u0621");
            contents.Add("VersionConflict", "\u062a\u0636\u0627\u0631\u0628 \u0635\u064a\u063a");
            contents.Add("VersionInfo", "\u0645\u0648\u0632\u0639 <> \u062d\u0631\u064a\u0641");
            contents.Add("PleaseUpgrade", "\u0627\u0644\u0631\u0651\u064e\u062c\u0627\u0621 \u0623\u062c\u0631 \u0628\u0631\u0646\u0627\u0645\u062c \u0627\u0644\u062a\u062d\u064a\u064a\u0646");
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
