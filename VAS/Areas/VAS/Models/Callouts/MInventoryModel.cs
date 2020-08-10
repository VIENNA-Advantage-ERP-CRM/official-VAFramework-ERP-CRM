using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MInventoryModel
    {
        /// <summary>
        /// GetMInventory
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetMInventory(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int M_Inventory_ID;

            //Assign parameter value
            M_Inventory_ID = Util.GetValueOfInt(paramValue[0].ToString());
            MInventory inv = new MInventory(ctx, M_Inventory_ID, null);
            DateTime? MovementDate = inv.GetMovementDate();
            int AD_Org_ID = inv.GetAD_Org_ID();

            Dictionary<string, string> retDic = new Dictionary<string, string>();
            retDic["MovementDate"] = MovementDate.ToString();
            retDic["AD_Org_ID"] = AD_Org_ID.ToString();
            return retDic;
        }
    }
}