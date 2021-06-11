using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MTeamForcastModel
    {
        /// <summary>
        /// Get SuperVisor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public int GetSupervisor(Ctx ctx, int C_Team_ID)
        {
           string sql="SELECT SUpervisor_ID FROM C_Team WHERE C_Team_ID="+ C_Team_ID;
            return Util.GetValueOfInt(DB.ExecuteScalar(sql));       
        }

        /// <summary>
        /// Get BOM Details and Routing
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="M_Product_ID">Product</param>
        /// <writer>209</writer>
        /// <returns></returns>
        public Dictionary<String, Object> GetBOMDetails(Ctx ctx, int M_Product_ID)
        {
            Dictionary<string, object> retDic = null;

            if (Env.IsModuleInstalled("VAMFG_"))
            {
                retDic = new Dictionary<string, object>();
                //fetch BOM, BOMUSE, Routing of selected Product
                string sql = @"SELECT BOM.M_BOM_ID ,BOM.BOMUse,Routing.VAMFG_M_Routing_ID FROM M_Product p 
                    INNER JOIN M_BOM  BOM ON p.M_product_ID = BOM.M_Product_ID 
                    LEFT JOIN VAMFG_M_Routing Routing ON Routing.M_product_ID=p.M_product_ID AND Routing.VAMFG_IsDefault='Y'
                    WHERE p.M_Product_ID=" + M_Product_ID + " AND p.ISBOM = 'Y'" + " AND BOM.IsActive='Y'";
                DataSet ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables[0].Rows.Count == 1)
                {
                    retDic["BOMUse"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["BOMUse"]);
                    retDic["VAMFG_M_Routing_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAMFG_M_Routing_ID"]);
                    retDic["M_BOM_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_BOM_ID"]);
                }
            }
            return retDic;
        }
    }
}