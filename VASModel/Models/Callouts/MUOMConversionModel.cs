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
            int VAM_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int VAB_UOM_To_ID = Util.GetValueOfInt(paramValue[1].ToString());
            Decimal? QtyEntered = Util.GetValueOfDecimal(paramValue[2].ToString());
            //End Assign parameter value  


            return MVABUOMConversion.ConvertProductTo(ctx, VAM_Product_ID,
                    VAB_UOM_To_ID, QtyEntered);

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
            int VAM_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int VAB_UOM_To_ID = Util.GetValueOfInt(paramValue[1].ToString());
            Decimal? PriceStd = Util.GetValueOfDecimal(paramValue[2].ToString());
            //End Assign parameter value             

            return MVABUOMConversion.ConvertProductFrom(ctx, VAM_Product_ID,
                    VAB_UOM_To_ID, PriceStd);

        }
    }
}