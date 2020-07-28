using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VAdvantage.Login
{
    public class ALoginRes_zh_CN : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_zh_CN()
        {
            contents.Add("Connection", "\u8fde\u673a");
            contents.Add("Defaults", "\u9ed8\u8ba4\u8bbe\u7f6e");
            contents.Add("Login", "Vienna \u767b\u5f55");
            contents.Add("File", "\u6587\u4ef6");
            contents.Add("Exit", "\u9000\u51fa");
            contents.Add("Help", "\u5e2e\u52a9");
            contents.Add("About", "\u5173\u4e8e");
            contents.Add("Host", "\u4e3b\u673a");
            contents.Add("Database", "\u6570\u636e\u5e93");
            contents.Add("User", "\u7528\u6237\u53f7");
            contents.Add("EnterUser", "\u8bf7\u8f93\u5165\u7528\u6237\u53f7");
            contents.Add("Password", "\u53e3\u4ee4");
            contents.Add("EnterPassword", "\u8bf7\u8f93\u5165\u53e3\u4ee4");
            contents.Add("Language", "\u8bed\u8a00");
            contents.Add("SelectLanguage", "\u9009\u62e9\u8bed\u8a00");
            contents.Add("Role", "\u89d2\u8272");
            contents.Add("Client", "\u5b9e\u4f53");
            contents.Add("Organization", "\u7EC4\u7EC7");
            contents.Add("Date", "\u65e5\u671f");
            contents.Add("Warehouse", "\u4ed3\u5e93");
            contents.Add("Printer", "\u6253\u5370\u673a");
            contents.Add("Connected", "\u5df2\u5728\u7ebf");
            contents.Add("NotConnected", "\u672a\u5728\u7ebf");
            contents.Add("DatabaseNotFound", "\u672a\u627e\u5230\u6570\u636e\u5e93");
            contents.Add("UserPwdError", "\u7528\u6237\u53f7\u53e3\u4ee4\u4e0d\u6b63\u786e");
            contents.Add("RoleNotFound", "\u6ca1\u6709\u6b64\u89d2\u8272");
            contents.Add("Authorized", "\u5df2\u6388\u6743");
            contents.Add("Ok", "\u786e\u5b9a");
            contents.Add("Cancel", "\u64a4\u6d88");
            contents.Add("VersionConflict", "\u7248\u672c\u51b2\u7a81\uff1a");
            contents.Add("VersionInfo", "\u670d\u52a1\u5668\u7aef <> \u5ba2\u6237\u7aef");
            contents.Add("PleaseUpgrade", "\u8bf7\u5347\u7ea7\u7a0b\u5e8f");
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
