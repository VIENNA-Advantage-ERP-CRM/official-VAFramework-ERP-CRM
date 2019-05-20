using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MProductCategoryModel
    {
        /// <summary>
        /// GetProductCategory
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string,string> GetProductCategory(Ctx ctx, string fields)
        {          
            string[] paramValue = fields.Split(',');
            int M_Product_Category_ID;

            M_Product_Category_ID = Util.GetValueOfInt(paramValue[0].ToString());
            MProductCategory pc = new MProductCategory(ctx, M_Product_Category_ID, null);
            //   mTab.setValue("IsPurchasedToOrder", pc.IsPurchasedToOrder());
            bool IsPurchasedToOrder = false;//= pc.IsPurchasedToOrder();  //Temporay Commented BY sarab 
            Dictionary<string, string> retDic = new Dictionary<string, string>();
            retDic[""] = IsPurchasedToOrder.ToString();
            return retDic;

        }
    }
}