using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MUOMConversionModel
    {
        /// <summary>
        /// Convert Product To
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Decimal? ConvertProductTo(Ctx ctx, string fields)
        {
            string[] paramValue = fields.ToString().Split(',');

            //Assign parameter value
            int M_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int C_UOM_To_ID = Util.GetValueOfInt(paramValue[1].ToString());
            Decimal? QtyEntered = Util.GetValueOfDecimal(paramValue[2].ToString());
            //End Assign parameter value  


            return MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                    C_UOM_To_ID, QtyEntered);

        }
        /// <summary>
        /// Convert Product From
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Decimal? ConvertProductFrom(Ctx ctx, string fields)
        {
            string[] paramValue = fields.ToString().Split(',');

            //Assign parameter value
            int M_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int C_UOM_To_ID = Util.GetValueOfInt(paramValue[1].ToString());
            Decimal? PriceStd = Util.GetValueOfDecimal(paramValue[2].ToString());
            //End Assign parameter value             

            return MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                    C_UOM_To_ID, PriceStd);

        }
    }
}