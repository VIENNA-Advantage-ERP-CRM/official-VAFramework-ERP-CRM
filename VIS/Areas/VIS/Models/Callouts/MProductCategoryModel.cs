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
        public Dictionary<string, string> GetProductCategory(Ctx ctx, string fields)
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

        public Dictionary<string, string> GetCategoryData(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int M_Product_Category_ID = 0;
            M_Product_Category_ID = Util.GetValueOfInt(paramValue[0].ToString());
            MProductCategory pc = new MProductCategory(ctx, M_Product_Category_ID, null);
            Dictionary<string, string> retDic = new Dictionary<string, string>();
            if (pc.Get_ColumnIndex("ProductType") > 0)
            {
                retDic["ProductType"] = pc.GetProductType();
            }
            if (pc.Get_ColumnIndex("M_AttributeSet_ID") > 0)
            {
                retDic["M_AttributeSet_ID"] = Util.GetValueOfString(pc.GetM_AttributeSet_ID());
            }
            if (pc.Get_ColumnIndex("C_TaxCategory_ID") > 0)
            {
                retDic["C_TaxCategory_ID"] = Util.GetValueOfString(pc.GetC_TaxCategory_ID());
            }
            retDic["A_Asset_Group_ID"] = Util.GetValueOfString(pc.GetA_Asset_Group_ID());
            if (pc.GetA_Asset_Group_ID() > 0)
            {
                MAssetGroup astGrp = new MAssetGroup(ctx, pc.GetA_Asset_Group_ID(), null);
                if (astGrp.Get_ColumnIndex("VA038_AmortizationTemplate_ID") > 0)
                {
                    retDic["VA038_AmortizationTemplate_ID"] = Util.GetValueOfString(astGrp.Get_Value("VA038_AmortizationTemplate_ID"));
                }
            }
            return retDic;
        }
    }
}