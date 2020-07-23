using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VAdvantage.Login
{
    public class ALoginRes_zh_HK : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_zh_HK()
        {
            contents.Add("Connection", "连接");
            contents.Add("Defaults", "默认");
            contents.Add("Login", "Vienna 登录");
            contents.Add("File", "档案");
            contents.Add("Exit", "离开");
            contents.Add("Help", "帮助");
            contents.Add("About", "关於");
            contents.Add("Host", "服务器");
            contents.Add("Database", "数据库");
            contents.Add("User", "用户ID");
            contents.Add("EnterUser", "输入应用用户ID");
            contents.Add("Password", "密码");
            contents.Add("EnterPassword", "输入应用密码");
            contents.Add("Language", "语言");
            contents.Add("SelectLanguage", "请选择语言");
            contents.Add("Role", "角色");
            contents.Add("Client", "客户端");
            contents.Add("Organization", "组织");
            contents.Add("Date", "日期");
            contents.Add("Warehouse", "仓库");
            contents.Add("Printer", "打印机");
            contents.Add("Connected", "已连接");
            contents.Add("NotConnected", "未连接");
            contents.Add("DatabaseNotFound", "未找到数据库");
            contents.Add("UserPwdError", "用户密码不一致");
            contents.Add("RoleNotFound", "未找到角色/角色未完成");
            contents.Add("Authorized", "已授权");
            contents.Add("Ok", "确定");
            contents.Add("Cancel", "取消");
            contents.Add("VersionConflict", "版本冲突:");
            contents.Add("VersionInfo", "服务器 <> 客户端");
            contents.Add("PleaseUpgrade", "请从服务器下载最新版本");

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
