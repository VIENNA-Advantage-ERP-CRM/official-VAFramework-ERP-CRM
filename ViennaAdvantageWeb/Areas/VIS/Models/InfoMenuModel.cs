using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class InfoMenuModel
    {
        //public Dictionary<int, string> InfoWinMenu
        //{
        //    get;
        //    set;
        //}
        public List<InfoWin> InfoWinMenu
        {
            get;
            set;
        }
        public String HTML
        {
            get;
            set;
        }
        public void FIllInfoWinMenu(Ctx ctx)
        {

            bool isBaseLanguage = VAdvantage.Utility.Env.IsBaseLanguage(ctx, "C_Currency");
            string sql = "";
            if (isBaseLanguage)
            {
                sql = "SELECT Name, AD_InfoWindow_ID FROM AD_InfoWindow WHERE IsActive='Y'";
            }
            else
            {
                sql = @" SELECT WT.Name, W.AD_InfoWindow_ID FROM AD_InfoWindow W
                            INNER JOIN AD_InfoWindow_Trl WT ON (W.AD_InfoWindow_ID=WT.AD_InfoWindow_ID AND WT.ad_language='"+ctx.GetAD_Language()+@"')
                            WHERE W.IsActive='Y'";
            }
           
            DataSet ds = VAdvantage.DataBase.DB.ExecuteDataset(sql);
            List<InfoWin> menu = new List<InfoWin>();
            InfoWin item = null;
           // StringBuilder menu = new StringBuilder();
            //menu.Append("<SELECT multiple='multiple' style='height:300px'>");
            //menu.Append("<ul>");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
             //   menu.Append("<OPTION>" + ds.Tables[0].Rows[i]["Name"] + "_" + ds.Tables[0].Rows[i]["AD_InfoWindow_ID"] + "</OPTION>");
                //menu.Append("<li>" + ds.Tables[0].Rows[i]["Name"] + "_" + ds.Tables[0].Rows[i]["AD_InfoWindow_ID"] + "</li>");
                item = new InfoWin();
                item.Name = (ds.Tables[0].Rows[i]["Name"]).ToString();
                item.AD_InfoWindow_ID= Convert.ToInt32(ds.Tables[0].Rows[i]["AD_InfoWindow_ID"]);
                ////menu.Add((Convert.ToInt32(ds.Tables[0].Rows[i]["AD_InfoWindow_ID"])), (ds.Tables[0].Rows[i]["Name"]).ToString());
                menu.Add(item);
             }
            //menu.Append("</SELECT>");
            //menu.Append("</ul>");
            InfoWinMenu = menu;
            //HTML = menu.ToString();
        }
    }
    public class InfoWin
    {
        public string Name
        {
            get;
            set;
        }
        public int AD_InfoWindow_ID
        {
            get;
            set;
        }

    }
}