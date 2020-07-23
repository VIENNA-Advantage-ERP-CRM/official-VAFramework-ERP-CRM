using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace VAdvantage.Login
{
    public class ALoginRes_id_ID : LanguageProcess
    {
        private System.Collections.Hashtable contents = new Hashtable();
        public ALoginRes_id_ID()
        {
            contents.Add("Connection", "Koneksi");
            contents.Add("Defaults", "Konfigurasi Dasar");
            contents.Add("Login", "Login Vienna");
            contents.Add("File", "File");
            contents.Add("Exit", "Keluar");
            contents.Add("Help", "Tolong");
            contents.Add("About", "Tentang");
            contents.Add("Host", "Pusat");
            contents.Add("Database", "Database");
            contents.Add("User", "ID Pengguna");
            contents.Add("EnterUser", "Masukkan ID pengguna");
            contents.Add("Password", "Kata Sandi");
            contents.Add("EnterPassword", "Masukkan kata sandi applikasi");
            contents.Add("Language", "Pilihan Bahasa");
            contents.Add("SelectLanguage", "Pilihlah bahasa yang disukai");
            contents.Add("Role", "Jabatan");
            contents.Add("Client", "Klien");
            contents.Add("Organization", "Organisasi");
            contents.Add("Date", "Tanggal");
            contents.Add("Warehouse", "Gudang");
            contents.Add("Printer", "Pencetak");
            contents.Add("Connected", "Sistem telah terkoneksi");
            contents.Add("NotConnected", "Sistem tidak terkoneksi!");
            contents.Add("DatabaseNotFound", "Database tidak ditemukan!");
            contents.Add("UserPwdError", "Nama ID pengguna anda tidak sesuai dengan kata sandi");
            contents.Add("RoleNotFound", "Jabatan tidak ditemukan");
            contents.Add("Authorized", "Anda telah diotorisasi");
            contents.Add("Ok", "Ok");
            contents.Add("Cancel", "Batal");
            contents.Add("VersionConflict", "Konflik Versi");
            contents.Add("VersionInfo", "Info Versi");
            contents.Add("PleaseUpgrade", "Mohon hubungi partner Vienna anda untuk upgrade");
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
