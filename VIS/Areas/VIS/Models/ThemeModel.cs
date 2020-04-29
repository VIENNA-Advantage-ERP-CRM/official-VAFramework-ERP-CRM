using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using System.Web;
using VAdvantage.Utility;
using VIS.DataContracts;

namespace VIS.Models
{
    public class ThemeModel
    {

        internal static List<ThemeData>  GetThemeData()
        {
            List<ThemeData> tml = new List<ThemeData>();
            string sql = "SELECT Name, PrimaryColor, OnPrimaryColor, SecondaryColor,OnSecondaryColor, IsDefault, AD_Theme_ID FROM AD_Theme";
            DataSet ds = DBase.DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables.Count>0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    ThemeData tm = new ThemeData();
                    tm.Id = Convert.ToInt32(dr["AD_Theme_ID"]);
                    tm.IsDefault = dr["IsDefault"].ToString()== "Y";
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
    }
}