using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DBase;

namespace VIS.Models
{
    public class MProjectModel
    {
        /// <summary>
        /// GetProjectDetail
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetProjectDetail(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int TaskID = 0, phaseID = 0, projID = 0, ProductID = 0;
            TaskID = Util.GetValueOfInt(paramValue[0].ToString());
            phaseID = Util.GetValueOfInt(paramValue[1].ToString());
            projID = Util.GetValueOfInt(paramValue[2].ToString());
            ProductID = Util.GetValueOfInt(paramValue[3].ToString());
            Dictionary<string, object> result = null;
            string Sql = "SELECT C_Project_ID FROM C_ProjectPhase WHERE C_ProjectPhase_ID IN (SELECT C_ProjectPhase_ID FROM" +
                    " C_ProjectTask WHERE C_ProjectTask_ID = " + TaskID + ")";
            int id = Util.GetValueOfInt(DB.ExecuteScalar(Sql, null, null));
            if (id > 0)
            {
                projID = id;
            }
            else
            {
                Sql = "SELECT C_Project_ID FROM C_ProjectPhase WHERE C_ProjectPhase_ID = " + phaseID;
                id = Util.GetValueOfInt(DB.ExecuteScalar(Sql, null, null));
                if (id > 0)
                {
                    projID = id;
                }
            }

            Sql = "SELECT PriceList, PriceStd, PriceLimit FROM M_ProductPrice WHERE M_PriceList_Version_ID = (SELECT c.M_PriceList_Version_ID FROM C_Project c WHERE c.C_Project_ID = "
                + projID + ")  AND M_Product_ID=" + ProductID;
            DataSet ds = DB.ExecuteDataset(Sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                result = new Dictionary<string, object>();
                result["PriceList"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["PriceLimit"]);
                result["PriceStd"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["PriceLimit"]);
                result["PriceLimit"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["PriceLimit"]);
            }
            return result;
        }

        // Added by Bharat on 23 May 2017
        public Decimal GetProjectPriceLimit(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int TaskID = 0, projID = 0, ProductID = 0;
            TaskID = Util.GetValueOfInt(paramValue[0].ToString());
            projID = Util.GetValueOfInt(paramValue[1].ToString());
            ProductID = Util.GetValueOfInt(paramValue[2].ToString());
            string Sql = "SELECT C_Project_ID FROM C_ProjectPhase WHERE C_ProjectPhase_ID IN (SELECT C_ProjectPhase_ID FROM" +
                    " C_ProjectTask WHERE C_ProjectTask_ID = " + TaskID + ")";
            int id = Util.GetValueOfInt(DB.ExecuteScalar(Sql, null, null));
            if (id > 0)
            {
                projID = id;
            }

            Sql = "SELECT PriceLimit FROM M_ProductPrice WHERE M_PriceList_Version_ID = (SELECT c.M_PriceList_Version_ID FROM  C_Project c WHERE c.C_Project_ID = "
                + projID + ")  AND M_Product_ID=" + ProductID;
            Decimal PriceLimit = Util.GetValueOfDecimal(DB.ExecuteScalar(Sql, null, null));
            return PriceLimit;
        }
    }
}