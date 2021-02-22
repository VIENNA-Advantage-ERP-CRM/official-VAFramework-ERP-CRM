using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MVAMProductModel
    {
        public Dictionary<string, string> GetProduct(Ctx ctx,string fields)
        {
            string[] paramValue = fields.Split(',');

            //Assign parameter value
            int VAM_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int VAM_Warehouse_ID = 0;
            if (paramValue.Length > 1)
            {
                VAM_Warehouse_ID = Util.GetValueOfInt(paramValue[1].ToString());
            }
            //End Assign parameter value

            MVAMProduct product = MVAMProduct.Get(ctx, VAM_Product_ID);
            Dictionary<string, string> result = new Dictionary<string, string>();
            result["VAB_UOM_ID"] = product.GetVAB_UOM_ID().ToString();
            result["IsStocked"] = product.IsStocked() ? "Y" : "N";
            if (VAM_Product_ID > 0)
            {
                if(VAM_Warehouse_ID>0)
                result["VAM_Locator_ID"] = MVAMProductLocator.GetFirstVAM_Locator_ID(product, VAM_Warehouse_ID).ToString();
            }
            //if (product.GetVAM_ProductCategory_ID() > 0)
            //{
            //    result["VAM_ProductCategory_ID"] = product.GetVAM_ProductCategory_ID().ToString();
            //}
            //else
            //{
            //    result["VAM_ProductCategory_ID"] = "0";
            //}
            //result["VAM_Product_ID"] = product.GetVAM_Product_ID().ToString();
            return result;
        }
        public string GetProductType(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');

            //Assign parameter value
            int VAM_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
          
             MVAMProduct prod = new MVAMProduct(ctx, VAM_Product_ID, null);
             return prod.GetProductType(); ;
            
            
        }  
        /// <summary>
        /// Get UPM Precision
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public int GetUOMPrecision(Ctx ctx,string fields)
        {
            string[] paramValue = fields.Split(',');
            int VAM_Product_ID;
            VAM_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
            return MVAMProduct.Get(ctx, VAM_Product_ID).GetUOMPrecision();
            
        }
        /// <summary>
        /// Get C
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public int GetVAB_UOM_ID(Ctx ctx,string fields)
        {
            string[] paramValue = fields.Split(',');
            int VAM_Product_ID;
            VAM_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
            return MVAMProduct.Get(ctx, VAM_Product_ID).GetVAB_UOM_ID();
           
        }

        //Added By amit
        public int GetTaxCategory(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int VAM_Product_ID;
            VAM_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
            return MVAMProduct.Get(ctx, VAM_Product_ID).GetVAB_TaxCategory_ID();
        }
        //End

        /// <summary>
        /// Get VAB_Rev_Recognition_ID
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="fields">VAM_Product_ID</param>
        /// <returns>VAB_Rev_Recognition_ID</returns>
        public int GetRevenuRecognition(Ctx ctx, string fields)
        {
            string sql = "SELECT VAB_Rev_Recognition_ID FROM VAM_Product WHERE IsActive = 'Y' AND VAM_Product_ID = " + Util.GetValueOfInt(fields);
            return Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
        }
    }
}