using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DBase;

namespace VIS.Models
{
    public class MInOutLineModel
    {
        /// <summary>
        /// GetMInOutLine
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetMInOutLine(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            //Assign parameter value
            int Orig_InOutLine_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value
            MInOutLine ioLine = new MInOutLine(ctx, Orig_InOutLine_ID, null);
            Dictionary<string, string> result = new Dictionary<string, string>();
            result["VAM_Locator_ID"] = ioLine.GetVAM_Locator_ID().ToString();
            result["MovementQty"] = ioLine.GetMovementQty().ToString();
            result["VAB_Project_ID"] = ioLine.GetVAB_Project_ID().ToString();
            result["VAB_Promotion_ID"] = ioLine.GetVAB_Promotion_ID().ToString();
            result["VAM_Product_ID"] = ioLine.GetVAM_Product_ID().ToString();
            result["VAM_PFeature_SetInstance_ID"] = ioLine.GetVAM_PFeature_SetInstance_ID().ToString();
            result["VAB_UOM_ID"] = ioLine.GetVAB_UOM_ID().ToString();
            result["IsDropShip"] = ioLine.IsDropShip() ? "Y" : "N";

            // JID_1310: On Selection of Shipment line on Customer/Vendor RMA. System should check Total Delivred - Total Return Qty From Sales PO line and Balance  show in qty field
            decimal qtyRMA = Util.GetValueOfDecimal(DB.ExecuteScalar(@"SELECT SUM(QtyEntered) FROM VAB_Order o INNER JOIN VAB_OrderLine ol ON o.VAB_Order_ID = ol.VAB_Order_ID                            
                            WHERE ol.Orig_InOutLine_ID = " + Orig_InOutLine_ID
                            + @" AND ol.Isactive = 'Y' AND o.docstatus NOT IN ('RE' , 'VO')", null, null));
            decimal QtyNotDelivered = ioLine.GetQtyEntered() - qtyRMA;

            result["QtyEntered"] = QtyNotDelivered.ToString();

            //retlst.Add(retValue);
            return result;
        }
    }
}