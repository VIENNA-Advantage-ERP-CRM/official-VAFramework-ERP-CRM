using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MDiscountSchemaModel
    {
        public string GetDiscountType(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int M_DiscountSchema_ID;
            M_DiscountSchema_ID = Util.GetValueOfInt(paramValue[0].ToString());
            MDiscountSchema discountschema = new MDiscountSchema(ctx, M_DiscountSchema_ID, null);
            return discountschema.GetDiscountType();
        }

        public Dictionary<String, String> GetDiscount(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int M_DiscountSchema_ID;
            M_DiscountSchema_ID = Util.GetValueOfInt(paramValue[0].ToString());
            MDiscountSchema discountschema = new MDiscountSchema(ctx, M_DiscountSchema_ID, null);
            Dictionary<String, String> retDic = new Dictionary<string, string>();
            retDic["AD_Org_ID"] = Util.GetValueOfString(discountschema.GetAD_Org_ID());
            retDic["DiscountType"] = discountschema.GetDiscountType();
            if (discountschema.GetFlatDiscount() > 0)
            {
                retDic["FlatDiscount"] = Util.GetValueOfString(discountschema.GetFlatDiscount());
            }
            else
            {
                retDic["FlatDiscount"] = "0";
            }
            retDic["IsBPartnerFlatDiscount"] = Util.GetValueOfString(discountschema.IsBPartnerFlatDiscount());
            retDic["IsQuantityBased"] = Util.GetValueOfString(discountschema.IsQuantityBased());
            retDic["M_DiscountSchema_ID"] = Util.GetValueOfString(discountschema.GetM_DiscountSchema_ID());
            retDic["Name"] = discountschema.GetName();
            return retDic;
        }
    }
}