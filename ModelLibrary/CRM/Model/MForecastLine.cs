using System;
using System.Net;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MForecastLine : X_C_ForecastLine
    {
        public MForecastLine(Ctx ctx, int C_ForecastLine_ID, Trx trxName) :
           base(ctx, C_ForecastLine_ID, null)
        {

        }

        public MForecastLine(Ctx ctx, DataRow dr, Trx trxName) :
           base(ctx, dr, trxName)
        {

        }

        protected override bool AfterDelete(bool success)
        {
            if (!success)
                return success;
            string sql = "update C_Forecast set GrandTotal = (SELECT COALESCE(SUM(TotalPrice),0) FROM C_ForecastLine WHERE isactive = 'Y' and C_Forecast_ID= " + GetC_Forecast_ID() + ") where C_Forecast_ID = " + GetC_Forecast_ID();
            int count = DB.ExecuteQuery(sql, null, Get_Trx());
            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;
            // Develop By Deekshant For margin amount and total amount
            if (VAdvantage.Utility.Env.IsModuleInstalled("VA077_"))
            {
                double amount = Util.GetValueOfDouble(Get_Value("VA077_PurchasePrice"));
                double qty = Util.GetValueOfDouble(GetQtyEntered());
                double total = amount * qty;
                double margin = Util.GetValueOfDouble(GetPriceStd()) - Util.GetValueOfDouble(total);
                string query = "UPDATE C_ForeCastLine SET VA077_PurchaseAmount=" + total + " , VA077_MarginAmt=" + margin + " WHERE C_ForeCastLine_ID=" + GetC_ForecastLine_ID();
                int res = Util.GetValueOfInt(DB.ExecuteQuery(query, null, Get_Trx()));
                if (res <= 0)
                {
                    Get_Trx().Rollback();
                    return false;
                }
            }
            string sql = "update C_Forecast set GrandTotal = (SELECT COALESCE(SUM(TotalPrice),0) FROM C_ForecastLine WHERE isactive = 'Y' and C_Forecast_ID= " + GetC_Forecast_ID() + ") where C_Forecast_ID = " + GetC_Forecast_ID();
            int count = DB.ExecuteQuery(sql, null, Get_Trx());

            if (!newRecord)
            {
                MForecastLineHistory LineHistory = new MForecastLineHistory(GetCtx(), 0, Get_Trx());
                LineHistory.SetAD_Client_ID(GetAD_Client_ID());
                LineHistory.SetAD_Org_ID(GetAD_Org_ID());
                LineHistory.SetC_ForecastLine_ID(GetC_ForecastLine_ID());
                LineHistory.SetLine(Util.GetValueOfInt(Get_ValueOld("Line")));
                LineHistory.SetC_Order_ID(Util.GetValueOfInt(Get_ValueOld("C_Order_ID")));
                LineHistory.SetC_OrderLine_ID(Util.GetValueOfInt(Get_ValueOld("C_OrderLine_ID")));
                LineHistory.SetC_Project_ID(Util.GetValueOfInt(Get_ValueOld("C_Project_ID")));
                LineHistory.SetC_ProjectLine_ID(Util.GetValueOfInt(Get_ValueOld("C_ProjectLine_ID")));
                LineHistory.SetC_Charge_ID(GetC_Charge_ID());
                LineHistory.SetM_Product_ID(GetM_Product_ID());
                LineHistory.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(Get_ValueOld("M_AttributeSetInstance_ID")));
                LineHistory.SetIsBOM(IsBOM());
                LineHistory.SetM_BOM_ID(Util.GetValueOfInt(Get_ValueOld("M_BOM_ID")));
                LineHistory.SetBOMUse(Util.GetValueOfString(Get_ValueOld("BOMUse")));
                LineHistory.SetC_UOM_ID(Util.GetValueOfInt(Get_ValueOld("C_UOM_ID")));
                LineHistory.SetBaseQuantity(Util.GetValueOfDecimal(Get_ValueOld("BaseQty")));
                LineHistory.SetQtyEntered(Util.GetValueOfDecimal(Get_ValueOld("QtyEntered")));
                LineHistory.SetUnitPrice(Util.GetValueOfDecimal(Get_ValueOld("UnitPrice")));
                LineHistory.SetTotalPrice(Util.GetValueOfDecimal(Get_ValueOld("TotalPrice")));
                LineHistory.SetDescription(Util.GetValueOfString(Get_ValueOld("Description")));
                if (Env.IsModuleInstalled("VAMFG_"))
                {
                    LineHistory.Set_Value("VAMFG_M_Routing_ID", Get_ValueOld("VAMFG_M_Routing_ID"));
                }
                if (!LineHistory.Save())
                {
                    ValueNamePair vp = VLogger.RetrieveError();
                    if (vp != null)
                    {
                        string val = vp.GetName();
                        log.SaveWarning("", Msg.GetMsg(GetCtx(), "NotSaveLineHistory") + val);
                    }
                    else
                    {
                        log.SaveWarning("", Msg.GetMsg(GetCtx(), "NotSaveLineHistory"));
                    }
                }

            }
            return true;
        }

        /// <summary>
        /// Add to Description
        /// </summary>
        /// <param name="description">text</param>
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
            {
                SetDescription(description);
            }
            else
            {
                SetDescription(desc + " | " + description);
            }
        }
    }
}
