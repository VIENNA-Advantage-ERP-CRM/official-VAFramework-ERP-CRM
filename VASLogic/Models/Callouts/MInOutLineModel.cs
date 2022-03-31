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
            result["M_Locator_ID"] = ioLine.GetM_Locator_ID().ToString();
            result["MovementQty"] = ioLine.GetMovementQty().ToString();
            result["C_Project_ID"] = ioLine.GetC_Project_ID().ToString();
            result["C_Campaign_ID"] = ioLine.GetC_Campaign_ID().ToString();
            result["M_Product_ID"] = ioLine.GetM_Product_ID().ToString();
            result["M_AttributeSetInstance_ID"] = ioLine.GetM_AttributeSetInstance_ID().ToString();
            result["C_UOM_ID"] = ioLine.GetC_UOM_ID().ToString();
            result["IsDropShip"] = ioLine.IsDropShip() ? "Y" : "N";

            // JID_1310: On Selection of Shipment line on Customer/Vendor RMA. System should check Total Delivred - Total Return Qty From Sales PO line and Balance  show in qty field
            decimal qtyRMA = Util.GetValueOfDecimal(DB.ExecuteScalar(@"SELECT SUM(QtyEntered) FROM C_Order o INNER JOIN C_OrderLine ol ON o.C_Order_ID = ol.C_Order_ID                            
                            WHERE ol.Orig_InOutLine_ID = " + Orig_InOutLine_ID
                            + @" AND ol.Isactive = 'Y' AND o.docstatus NOT IN ('RE' , 'VO')", null, null));
            decimal QtyNotDelivered = ioLine.GetQtyEntered() - qtyRMA;

            result["QtyEntered"] = QtyNotDelivered.ToString();

            //retlst.Add(retValue);
            return result;
        }

        /// <summary>
        /// Get Invoice Line ID
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="fields">parameter</param>
        /// <returns>Invoice Line ID</returns>
        public int GetInvoiceLine(Ctx ctx, string fields)
        {
            int invLine_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_InvoiceLine_ID FROM C_InvoiceLine WHERE M_InOutLine_ID = "
                + Util.GetValueOfInt(fields), null, null));
            return invLine_ID;
        }
    }
}