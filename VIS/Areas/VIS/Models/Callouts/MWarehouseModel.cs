using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MWarehouseModel
    {
        /// <summary>
        /// GetWarehouse
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetWarehouse(Ctx ctx,string fields)
        {
                       
            string[] paramValue = fields.Split(',');
            int M_Warehouse_ID;

            //Assign parameter value
            M_Warehouse_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //Assign parameter value

            MWarehouse wh = MWarehouse.Get(ctx, M_Warehouse_ID);
            Dictionary<string, string> result = new Dictionary<string, string>();
            result["M_Locator_ID"] = wh.GetDefaultM_Locator_ID().ToString();
            return result;
                
        }
    }
}