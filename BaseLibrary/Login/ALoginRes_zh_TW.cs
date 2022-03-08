using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace VAdvantage.Login
{
    public class ALoginRes_zh_TW : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_zh_TW()
        {
            contents.Add("Connection", "\u9023\u7dda");
            contents.Add("Defaults", "\u9810\u8a2d\u503c");
            contents.Add("Login", "Vienna \u767b\u5165");
            contents.Add("File", "\u6a94\u6848");
            contents.Add("Exit", "\u96e2\u958b");
            contents.Add("Help", "\u8aaa\u660e");
            contents.Add("About", "\u95dc\u65bc");
            contents.Add("Host", "\u4e3b\u6a5f");
            contents.Add("Database", "\u8cc7\u6599\u5eab");
            contents.Add("User", "\u5e33\u865f");
            contents.Add("EnterUser", "\u8acb\u9375\u5165\u5e33\u865f");
            contents.Add("Password", "\u5bc6\u78bc");
            contents.Add("EnterPassword", "\u8acb\u9375\u5165\u5bc6\u78bc");
            contents.Add("Language", "\u8a9e\u8a00");
            contents.Add("SelectLanguage", "\u9078\u64c7\u8a9e\u8a00");
            contents.Add("Role", "\u89d2\u8272");
            contents.Add("Client", "\u5BE6\u9AD4");
            contents.Add("Organization", "\u7d44\u7e54");
            contents.Add("Date", "\u65e5\u671f");
            contents.Add("Warehouse", "\u5009\u5eab");
            contents.Add("Printer", "\u5370\u8868\u6a5f");
            contents.Add("Connected", "\u5df2\u9023\u7dda");
            contents.Add("NotConnected", "\u672a\u9023\u7dda");
            contents.Add("DatabaseNotFound", "\u627e\u4e0d\u5230\u8cc7\u6599\u5eab");
            contents.Add("UserPwdError", "\u5e33\u865f\u5bc6\u78bc\u4e0d\u6b63\u78ba");
            contents.Add("RoleNotFound", "\u6c92\u6709\u9019\u89d2\u8272");
            contents.Add("Authorized", "\u5df2\u6388\u6b0a");
            contents.Add("Ok", "\u78ba\u5b9a");
            contents.Add("Cancel", "\u53d6\u6d88");
            contents.Add("VersionConflict", "\u7248\u672c\u885d\u7a81:");
            contents.Add("VersionInfo", "\u4f3a\u670d\u7aef <> \u5ba2\u6236\u7aef");
            contents.Add("PleaseUpgrade", "\u8acb\u57f7\u884c\u5347\u7d1a\u7a0b\u5f0f");
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
