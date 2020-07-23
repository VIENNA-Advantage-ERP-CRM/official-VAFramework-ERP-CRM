using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace VAdvantage.Login
{
    public class ALoginRes_vi_VN : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_vi_VN()
        {
            contents.Add("Connection", "K\u1EBFt n\u1ED1i");
            contents.Add("Defaults", "M\u1EB7c nhiên");
            contents.Add("Login", "\u0110\u0103ng nh\u1EADp");
            contents.Add("File", "H\u1EC7 th\u1ED1ng");
            contents.Add("Exit", "Exit");
            contents.Add("Help", "Giúp \u0111\u1EE1");
            contents.Add("About", "Gi\u1EDBi thi\u1EC7u");
            contents.Add("Host", "Máy ch\u1EE7");
            contents.Add("Database", "C\u01A1 s\u1EDF d\u1EEF li\u1EC7u");
            contents.Add("User", "Tên ng\u01B0\u1EDDi dùng");
            contents.Add("EnterUser", "Hãy nh\u1EADp tên ng\u01B0\u1EDDi dùng");
            contents.Add("Password", "M\u1EADt kh\u1EA9u");
            contents.Add("EnterPassword", "Hãy nh\u1EADp m\u1EADt kh\u1EA9u");
            contents.Add("Language", "Ngôn ng\u1EEF");
            contents.Add("SelectLanguage", "Hãy ch\u1ECDn ngôn ng\u1EEF");
            contents.Add("Role", "Vai trò");
            contents.Add("Client", "Công ty");
            contents.Add("Organization", "\u0110\u01A1n v\u1ECB");
            contents.Add("Date", "Ngày");
            contents.Add("Warehouse", "Kho hàng");
            contents.Add("Printer", "Máy in");
            contents.Add("Connected", "\u0110ã k\u1EBFt n\u1ED1i");
            contents.Add("NotConnected", "Ch\u01B0a k\u1EBFt n\u1ED1i \u0111\u01B0\u1EE3c");
            contents.Add("DatabaseNotFound", "Không tìm th\u1EA5y CSDL");
            contents.Add("UserPwdError", "Ng\u01B0\u1EDDi dùng và m\u1EADt kh\u1EA9u không kh\u1EDBp nhau");
            contents.Add("RoleNotFound", "Không tìm th\u1EA5y vai trò này");
            contents.Add("Authorized", "\u0110ã \u0111\u01B0\u1EE3c phép");
            contents.Add("Ok", "\u0110\u1ED3ng ý");
            contents.Add("Cancel", "H\u1EE7y");
            contents.Add("VersionConflict", "X\u1EA3y ra tranh ch\u1EA5p phiên b\u1EA3n:");
            contents.Add("VersionInfo", "Thông tin v\u1EC1 phiên b\u1EA3n");
            contents.Add("PleaseUpgrade", "Vui lòng nâng c\u1EA5p ch\u01B0\u01A1ng trình");
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
