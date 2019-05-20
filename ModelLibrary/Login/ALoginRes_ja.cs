using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace VAdvantage.Login
{
    public class ALoginRes_ja_JP : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_ja_JP()
        {
            contents.Add("Connection", "\u63a5\u7d9a");
            contents.Add("Defaults", "\u30c7\u30d5\u30a9\u30eb\u30c8");
            contents.Add("Login", "\u30b3\u30f3\u30d4\u30a7\u30fc\u30ec \u30ed\u30b0\u30a4\u30f3");
            contents.Add("File", "\u30d5\u30a1\u30a4\u30eb");
            contents.Add("Exit", "\u7d42\u4e86");
            contents.Add("Help", "\u30d8\u30eb\u30d7");
            contents.Add("About", "\u60c5\u5831");
            contents.Add("Host", "\u30b5\u30fc\u30d0");
            contents.Add("Database", "\u30c7\u30fc\u30bf\u30d9\u30fc\u30b9");
            contents.Add("User", "\u30e6\u30fc\u30b6\u30fc");
            contents.Add("EnterUser", "\u30a2\u30d7\u30ea\u30b1\u30fc\u30b7\u30e7\u30f3\u306e\u30e6\u30fc\u30b6\u540d\u304a\u5165\u529b\u3057\u3066\u4e0b\u3055\u3044");
            contents.Add("Password", "\u30d1\u30b9\u30ef\u30fc\u30c9");
            contents.Add("EnterPassword", "\u30a2\u30d7\u30ea\u30b1\u30fc\u30b7\u30e7\u30f3\u306e\u30d1\u30b9\u30ef\u30fc\u30c9\u3092\u5165\u529b\u3057\u3066\u4e0b\u3055\u3044");
            contents.Add("Language", "\u8a00\u8a9e");
            contents.Add("SelectLanguage", "\u8a00\u8a9e\u3092\u9078\u629e\u3057\u3066\u4e0b\u3055\u3044");
            contents.Add("Role", "\u5f79\u76ee");
            contents.Add("Client", "\u4f1a\u793e");
            contents.Add("Organization", "\u90e8\u8ab2");
            contents.Add("Date", "\u65e5\u4ed8");
            contents.Add("Warehouse", "\u5009\u5eab");
            contents.Add("Printer", "\u30d7\u30ea\u30f3\u30bf");
            contents.Add("Connected", "\u63a5\u7d9a\u6709\u308a");
            contents.Add("NotConnected", "\u7121\u63a5\u7d9a");
            contents.Add("DatabaseNotFound", "\u30c7\u30fc\u30bf\u30d9\u30fc\u30b9\u3092\u898b\u4ed8\u3051\u3089\u306a\u3044");
            contents.Add("UserPwdError", "\u30e6\u30fc\u30b6\u540d\u3068\u30d1\u30b9\u30ef\u30fc\u30c9\u304c\u5408\u308f\u306a\u3044");
            contents.Add("RoleNotFound", "\u5f79\u540d\u304c\u6709\u308a\u307e\u305b\u3093");
            contents.Add("Authorized", "\u691c\u5b9a\u6e08\u307f");
            contents.Add("Ok", "OK");
            contents.Add("Cancel", "\u30ad\u30e3\u30f3\u30bb\u30eb");
            contents.Add("VersionConflict", "\u30d0\u30fc\u30b8\u30e7\u30f3\u304c\u5408\u308f\u306a\u3044:");
            contents.Add("VersionInfo", "\u30b5\u30fc\u30d0 <> \u30af\u30e9\u30a4\u30a2\u30f3\u30c8");
            contents.Add("PleaseUpgrade", "\u30d0\u30fc\u30b8\u30e7\u30f3\u30a2\u30c3\u30d7\u3057\u3066\u4e0b\u3055\u3044");

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
