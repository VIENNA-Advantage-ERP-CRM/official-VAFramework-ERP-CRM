using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MInventoryLineModel
    {
        /// <summary>
        /// GetMInventoryLine
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string,string> GetMInventoryLine(Ctx ctx, string fields)
        {          
            string[] paramValue = fields.Split(',');
            int M_InventoryLine_ID;

            //Assign parameter value
            M_InventoryLine_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value
            MInventoryLine iLine = new MInventoryLine(ctx, M_InventoryLine_ID, null);
            int M_Product_ID = iLine.GetM_Product_ID();
            int M_Locator_ID = iLine.GetM_Locator_ID();           

            Dictionary<string, string> retDic =new Dictionary<string, string>();
            retDic["M_Product_ID"] = M_Product_ID.ToString();
            retDic["M_Locator_ID"] = M_Locator_ID.ToString();
            
            return retDic;
           }

        // Added by mohit to get product UOM- 12 June 2018
        /// <summary>
        /// Get product UOM
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public string GetproductUOM(Ctx ctx, string field)
        {            
            MProduct Product = new MProduct(ctx, Util.GetValueOfInt(field), null);
            return Product.GetC_UOM_ID().ToString();
        }
      
    }
}