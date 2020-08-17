using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace VAdvantage.Login
{
    public class ALoginRes_fa_IR : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_fa_IR()
        {
            contents.Add("Connection", "\u0627\u062a\u0635\u0627\u0644");
            contents.Add("Defaults", "\u0645\u0642\u0627\u062f\u064a\u0631 \u067e\u064a\u0634 \u0641\u0631\u0636");
            contents.Add("Login", "\u0648\u0631\u0648\u062f \u0628\u0647 \u0633\u064a\u0633\u062a\u0645");
            contents.Add("File", "\u0641\u0627\u064a\u0644");
            contents.Add("Exit", "\u062e\u0631\u0648\u062c");
            contents.Add("Help", "\u0631\u0627\u0647\u0646\u0645\u0627\u0626\u06cc");
            contents.Add("About", "\u062f\u0631\u0628\u0627\u0631\u0647");
            contents.Add("Host", "\u0633\u064a\u0633\u062a\u0645 \u0645\u064a\u0632\u0628\u0627\u0646");
            contents.Add("Database", "\u0628\u0627\u0646\u06a9 \u0627\u0637\u0644\u0627\u0639\u0627\u062a");
            contents.Add("User", "\u0645\u0634\u062e\u0635\u0647 \u0627\u0633\u062a\u0641\u0627\u062f\u0647 \u06a9\u0646\u0646\u062f\u0647");
            contents.Add("EnterUser", "\u0645\u0634\u062e\u0635\u0647 \u0627\u0633\u062a\u0641\u0627\u062f\u0647 \u06a9\u0646\u0646\u062f\u0647 \u0631\u0627 \u0648\u0627\u0631\u062f \u06a9\u0646\u064a\u062f");
            contents.Add("Password", "\u06a9\u0644\u0645\u0647 \u0639\u0628\u0648\u0631");
            contents.Add("EnterPassword", "\u06a9\u0644\u0645\u0647 \u0639\u0628\u0648\u0631 \u0631\u0627 \u0648\u0627\u0631\u062f \u06a9\u0646\u064a\u062f");
            contents.Add("Language", "\u0632\u0628\u0627\u0646");
            contents.Add("SelectLanguage", "\u0632\u0628\u0627\u0646 \u0631\u0627 \u0627\u0646\u062a\u062e\u0627\u0628 \u06a9\u0646\u064a\u062f");
            contents.Add("Role", "\u0646\u0642\u0634");
            contents.Add("Client", "\u0645\u0634\u062a\u0631\u06cc");
            contents.Add("Organization", "\u0633\u0627\u0632\u0645\u0627\u0646");
            contents.Add("Date", "\u062a\u0627\u0631\u064a\u062e");
            contents.Add("Warehouse", "\u0627\u0646\u0628\u0627\u0631 \u06a9\u0627\u0644\u0627");
            contents.Add("Printer", "\u0686\u0627\u067e\u06af\u0631");
            contents.Add("Connected", "\u0645\u062a\u0635\u0644 \u0634\u062f\u0647");
            contents.Add("NotConnected", "\u0645\u062a\u0635\u0644 \u0646\u0634\u062f\u0647");
            contents.Add("DatabaseNotFound", "\u0628\u0627\u0646\u06a9 \u0627\u0637\u0644\u0627\u0639\u0627\u062a \u067e\u064a\u062f\u0627 \u0646\u0634\u062f");
            contents.Add("UserPwdError", "\u0645\u0634\u062e\u0635\u0647 \u0627\u0633\u062a\u0641\u0627\u062f\u0647 \u06a9\u0646\u0646\u062f\u0647 \u0648 \u06a9\u0644\u0645\u0647 \u0639\u0628\u0648\u0631 \u0628\u0627 \u0647\u0645 \u062a\u0637\u0627\u0628\u0642 \u0646\u062f\u0627\u0631\u0646\u062f");
            contents.Add("RoleNotFound", "\u0646\u0642\u0634\u06cc \u067e\u064a\u062f\u0627 \u0646\u0634\u062f");
            contents.Add("Authorized", "\u0645\u062c\u0648\u0632 \u062f\u0627\u0631\u062f");
            contents.Add("Ok", "\u062a\u0635\u0648\u064a\u0628");
            contents.Add("Cancel", "\u0644\u063a\u0648");
            contents.Add("VersionConflict", "\u0646\u0633\u062e\u0647 \u0647\u0627 \u0646\u0627\u0633\u0627\u0632\u06af\u0627\u0631\u0627\u0646\u062f");
            contents.Add("VersionInfo", "\u0633\u0631\u0648\u0631 <> \u0645\u0634\u062a\u0631\u06cc");
            contents.Add("PleaseUpgrade", "\u0644\u0637\u0641\u0627 \u0628\u0631\u0646\u0627\u0645\u0647 \u0645\u0631\u0628\u0648\u0637 \u0628\u0647 \u062a\u0635\u064a\u062d \u0646\u0633\u062e\u0647 \u0628\u0631\u0646\u0627\u0645\u0647 \u0631\u0627 \u0627\u062c\u0631\u0627 \u06a9\u0646\u064a\u062f");

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
