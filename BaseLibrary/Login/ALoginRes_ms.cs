using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VAdvantage.Login
{
    public class ALoginRes_ms : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_ms()
        {
            contents.Add("Connection", "Hubungan");
            contents.Add("Defaults", "Defaults");
            contents.Add("Login", "Vienna Login");
            contents.Add("File", "Fail");
            contents.Add("Exit", "Keluar");
            contents.Add("Help", "Tolong");
            contents.Add("About", "Tentang");
            contents.Add("Host", "Host");
            contents.Add("Database", "Pangkalan Data");
            contents.Add("User", "ID Pengguna");
            contents.Add("EnterUser", "Masukkan ID Pengguna");
            contents.Add("Password", "Kata Laluan");
            contents.Add("EnterPassword", "Masukkan Kata Laluan Applikasi");
            contents.Add("Language", "Bahasa");
            contents.Add("SelectLanguage", "Pilih Bahasa Anda");
            contents.Add("Role", "Role");
            contents.Add("Client", "Pengguna");
            contents.Add("Organization", "Organisasi");
            contents.Add("Date", "Tarikh");
            contents.Add("Warehouse", "Warehouse");
            contents.Add("Printer", "Printer");
            contents.Add("Connected", "Telah dihubungi");
            contents.Add("NotConnected", "Tiday dapat dihubungi");
            contents.Add("DatabaseNotFound", "Pangkalan Data tidak dijumpai");
            contents.Add("UserPwdError", "Pengguna tidak padan dengan kata laluan");
            contents.Add("RoleNotFound", "Role not found/complete");
            contents.Add("Authorized", "Authorized");
            contents.Add("Ok", "Ok");
            contents.Add("Cancel", "Batal");
            contents.Add("VersionConflict", "Bertentangan versi:");
            contents.Add("VersionInfo", "Pelayan <> Pengguna");
            contents.Add("PleaseUpgrade", "Please run the update program");
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
