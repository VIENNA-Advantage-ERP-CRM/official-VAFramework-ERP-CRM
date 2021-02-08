using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MStorageModel
    {
        /// <summary>
        /// GetQtyAvailable
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Decimal? GetQtyAvailable(Ctx ctx,string fields)
        {
            string[] paramValue = fields.Split(',');

            //Assign parameter value
            int VAM_Warehouse_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int VAM_Product_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(paramValue[2].ToString());
            //End Assign parameter value

            return MStorage.GetQtyAvailable(VAM_Warehouse_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, null);
           
        }
    }
}