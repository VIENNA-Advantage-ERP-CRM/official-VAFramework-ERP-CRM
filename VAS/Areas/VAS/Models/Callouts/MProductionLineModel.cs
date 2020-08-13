using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MProductionLineModel
    {
        public Dictionary<string, decimal> GetChargeAmount(string charge_ID)
        {
            Dictionary<string, decimal> retDic = new Dictionary<string, decimal>();
            try
            {
                string sql = "Select ChargeAmt FROM C_Charge WHERE IsActive='Y' AND C_Charge_ID=" + Util.GetValueOfInt(charge_ID);
                decimal amount = Util.GetValueOfDecimal(DB.ExecuteScalar(sql));
                retDic["ChargeAmt"] = amount;
            }
            catch
            {
                retDic = null;
            }
            return retDic;
        }

        /// <summary>
        /// when we select BOM on production Plan, if attribute defined on BOM then need to set the same Attributesetinstance
        /// </summary>
        /// <param name="fields">M_BOM_ID</param>
        /// <returns>M_Attributesetinstance_ID</returns>
        public int GetAttributeSetInstance(Ctx ctx, string M_BOM_ID)
        {
            MBOM bom = MBOM.Get(ctx, Convert.ToInt32(M_BOM_ID));
            return bom.GetM_AttributeSetInstance_ID();
        }

        /// <summary>
        /// when we select either Product or M_AttributeSetInstance_ID on production Plan, pick respective BOM based on respective input
        /// </summary>
        /// <param name="fields">M_Product_ID , M_AttributeSetInstance_ID</param>
        /// <returns>M_BOM_ID</returns>
        public int GetBOM(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            // when product not found then return BOM ID as ZERO
            if (String.IsNullOrEmpty(paramValue[0]))
                return 0;
            int BOM_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT M_BOM_ID FROM M_BOM WHERE IsActive = 'Y' AND M_Product_ID = "
                 + Convert.ToInt32(paramValue[0]) + " AND NVL(M_AttributeSetInstance_ID, 0) = "
                 + Convert.ToInt32(paramValue[1])));
            return BOM_ID;
        }
    }
}