using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DataContracts;

namespace VIS.Models
{
    public class ThemeModel
    {

        internal static List<ThemeData> GetThemeData()
        {
            List<ThemeData> tml = new List<ThemeData>();
            string sql = "SELECT Name, PrimaryColor, OnPrimaryColor, SecondaryColor,OnSecondaryColor, IsDefault, VAF_Theme_ID FROM VAF_Theme";
            DataSet ds = DBase.DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    ThemeData tm = new ThemeData();
                    tm.Id = Convert.ToInt32(dr["VAF_Theme_ID"]);
                    tm.IsDefault = dr["IsDefault"].ToString() == "Y";
                    tm.Primary = dr["PrimaryColor"].ToString();
                    tm.OnPrimary = dr["OnPrimaryColor"].ToString();
                    tm.Seconadary = dr["SecondaryColor"].ToString();
                    tm.OnSecondary = dr["OnSecondaryColor"].ToString();
                    tm.Name = dr["Name"].ToString();
                    tml.Add(tm);
                }
            }
            return tml;
        }

        internal int SaveTheme(ThemeData thd)
        {
            if (thd.IsDefault)
            {
                DBase.DB.ExecuteQuery("UPDATE VAF_Theme SET IsDefault='N'", null, null);
            }

            X_VAF_Theme xtheme = new X_VAF_Theme(new Ctx(), 0, null);
            xtheme.SetName(thd.Name);
            xtheme.SetIsDefault(thd.IsDefault);
            xtheme.SetIsActive(true);
            xtheme.SetPrimaryColor(thd.Primary);
            xtheme.SetOnPrimaryColor(thd.OnPrimary);
            xtheme.SetSecondaryColor(thd.Seconadary);
            xtheme.SetOnSecondaryColor(thd.OnSecondary);
            if (xtheme.Save())
                return xtheme.Get_ID();
            else
                return -1;
        }

        internal bool SetDefaultTheme(int id)
        {
            DBase.DB.ExecuteQuery("UPDATE VAF_Theme SET IsDefault='N'", null, null);
            DBase.DB.ExecuteQuery("UPDATE VAF_Theme SET IsDefault='Y' WHERE VAF_Theme_ID = " + id, null, null);
            return true;
        }

        internal bool Delete(int id)
        {
            DBase.DB.ExecuteQuery("DELETE FROM VAF_Theme WHERE VAF_Theme_ID = " + id, null, null);
            return true;
        }

        internal bool UpdateForUser(int id, int uid)
        {
            DBase.DB.ExecuteQuery("UPDATE AD_User SET VAF_Theme_ID =  " + id + " WHERE AD_User_ID = " + uid, null, null);
            return true;
        }
        internal bool UpdateForClient(int id, int tid)
        {
            DBase.DB.ExecuteQuery("UPDATE VAF_Client SET VAF_Theme_ID = " + id + " WHERE VAF_Client_ID = " + tid, null, null);
            return true;
        }
    }
}