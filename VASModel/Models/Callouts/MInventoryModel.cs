using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MVAMInventoryModel
    {
        /// <summary>
        /// GetMVAMInventory
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetMVAMInventory(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int VAM_Inventory_ID;

            //Assign parameter value
            VAM_Inventory_ID = Util.GetValueOfInt(paramValue[0].ToString());
            MVAMInventory inv = new MVAMInventory(ctx, VAM_Inventory_ID, null);
            DateTime? MovementDate = inv.GetMovementDate();
            int VAF_Org_ID = inv.GetVAF_Org_ID();

            Dictionary<string, string> retDic = new Dictionary<string, string>();
            retDic["MovementDate"] = MovementDate.ToString();
            retDic["VAF_Org_ID"] = VAF_Org_ID.ToString();
            return retDic;
        }
    }
}