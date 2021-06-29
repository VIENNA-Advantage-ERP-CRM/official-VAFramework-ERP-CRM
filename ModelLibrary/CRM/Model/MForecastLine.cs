
/********************************************************
 * Module Name    :    VA Framework
 * Purpose        :    TeamForecast Line Model
 * Employee Code  :    209
 * Updated Date   :    26-April-2021
  ******************************************************/
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
        private static VLogger log = VLogger.GetVLogger(typeof(MForecastLine).FullName);

        public MForecastLine(Ctx ctx, int C_ForecastLine_ID, Trx trxName) :
           base(ctx, C_ForecastLine_ID, null)
        {

        }

        public MForecastLine(Ctx ctx, DataRow dr, Trx trxName) :
           base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Create New Instance Forecast Line
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="trx">Transaction</param>
        /// <param name="C_Forecast_ID">Team Forecast</param>
        /// <param name="M_Product_ID">Product</param>
        public MForecastLine(Ctx ctx, Trx trx, int C_Forecast_ID, int M_Product_ID)
            : base(ctx, 0, trx)
        {
            int LineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(Line), 0)+10  FROM C_ForecastLine WHERE " +
                "C_Forecast_ID=" + C_Forecast_ID, null, trx));
            SetC_Forecast_ID(C_Forecast_ID);
            SetM_Product_ID(M_Product_ID);
            SetLine(LineNo);
        }

        /// <summary>
        /// BeforeSave
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns><True/returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (Env.IsModuleInstalled("VAMFG_") && Util.GetValueOfInt(GetM_BOM_ID()) == 0)
            {
                //fetch BOM, BOMUSE, Routing of selected Product
                string sql = @"SELECT BOM.M_BOM_ID ,BOM.BOMUse,Routing.VAMFG_M_Routing_ID FROM M_Product p 
                    INNER JOIN M_BOM  BOM ON p.M_product_ID = BOM.M_Product_ID 
                    LEFT JOIN VAMFG_M_Routing Routing ON Routing.M_product_ID=p.M_product_ID AND Routing.VAMFG_IsDefault='Y'
                    WHERE p.M_Product_ID=" + GetM_Product_ID() + " AND p.ISBOM = 'Y'" + " AND BOM.IsActive='Y'";

                DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
                if (ds != null && ds.Tables[0].Rows.Count == 1)
                {
                    SetBOMUse(Util.GetValueOfString(ds.Tables[0].Rows[0]["BOMUse"]));
                    Set_Value("VAMFG_M_Routing_ID", Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAMFG_M_Routing_ID"]));
                    SetM_BOM_ID(Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_BOM_ID"]));
                }
            }
            return true;
        }

        /// <summary>
        /// Before Delete Constraints/logics
        /// </summary>
        /// <returns>true, when success</returns>
        protected override bool BeforeDelete()
        {
            // when document is other than Drafted stage, then we cannot delete documnet line
            string docStatus = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT DocStatus FROM C_Forecast 
                                WHERE C_Forecast_ID = " + GetC_Forecast_ID(), null, Get_Trx()));
            if (!docStatus.Equals(MForecast.DOCSTATUS_Drafted))
            {
                log.SaveError("CannotDeleteTrx", "");
                return false;
            }
            return true;
        }

        /// <summary>
        /// After Delete Constraints/logics
        /// </summary>
        /// <param name="success"></param>
        /// <returns>true, when success</returns>
        protected override bool AfterDelete(bool success)
        {
            if (!success)
                return success;
            string sql = "update C_Forecast set GrandTotal = (SELECT COALESCE(SUM(TotalPrice),0) FROM C_ForecastLine WHERE isactive = 'Y' and C_Forecast_ID= " + GetC_Forecast_ID() + ") where C_Forecast_ID = " + GetC_Forecast_ID();
            int count = DB.ExecuteQuery(sql, null, Get_Trx());
            return true;
        }

        /// <summary>
        /// After Save Constraints/logics
        /// </summary>
        /// <param name="newRecord">newRecord</param>
        /// <param name="success">True or False</param>
        /// <returns>true when success</returns>
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
                LineHistory.Set_Value("AD_OrgTrx_ID", Get_ValueOld("AD_OrgTrx_ID"));
                LineHistory.SetC_ForecastLine_ID(GetC_ForecastLine_ID());
                LineHistory.SetLine(Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT NVL(MAX(Line), 0) + 10 FROM C_ForecastLineHistory
                            WHERE C_ForecastLine_ID = " + GetC_ForecastLine_ID())));
                LineHistory.SetC_Order_ID(Util.GetValueOfInt(Get_ValueOld("C_Order_ID")));
                LineHistory.SetC_OrderLine_ID(Util.GetValueOfInt(Get_ValueOld("C_OrderLine_ID")));
                LineHistory.SetC_Project_ID(Util.GetValueOfInt(Get_ValueOld("C_Project_ID")));
                LineHistory.SetC_ProjectLine_ID(Util.GetValueOfInt(Get_ValueOld("C_ProjectLine_ID")));
                LineHistory.SetC_Charge_ID(Util.GetValueOfInt(Get_ValueOld("C_Charge_ID")));
                LineHistory.SetM_Product_ID(Util.GetValueOfInt(Get_ValueOld("M_Product_ID")));
                LineHistory.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(Get_ValueOld("M_AttributeSetInstance_ID")));
                LineHistory.SetIsBOM(IsBOM());
                LineHistory.SetM_BOM_ID(Util.GetValueOfInt(Get_ValueOld("M_BOM_ID")));
                LineHistory.SetBOMUse(Util.GetValueOfString(Get_ValueOld("BOMUse")).Equals("") ? null : Util.GetValueOfString(Get_ValueOld("BOMUse")));
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="trx">Transaction</param>
        /// <param name="C_Forecast_ID">Team Forecast</param>
        /// <param name="M_Product_ID">Product</param>
        /// <returns></returns>
        public static MForecastLine GetOrCreate(Ctx ctx, Trx trx, int C_Forecast_ID, int M_Product_ID, string ProductCategories)
        {
            MForecastLine retValue = null;
            if (!string.IsNullOrEmpty(ProductCategories))
            {

                String sql = "SELECT * FROM C_ForecastLine " +
                             " WHERE NVL(M_Product_ID,0)=" + M_Product_ID +
                             " AND C_Forecast_ID=" + C_Forecast_ID +
                             " AND NVL(C_OrderLine_ID,0)=0 AND NVL(C_ProjectLine_ID,0)=0 ";

                DataTable dt = null;
                IDataReader idr = null;
                try
                {
                    idr = DB.ExecuteReader(sql, null, trx);
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        retValue = new MForecastLine(ctx, dr, trx);
                    }
                }
                catch (Exception ex)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    log.Log(Level.SEVERE, sql, ex);
                }
                finally
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    dt = null;
                }
            }
            if (retValue == null)
            {
                retValue = new MForecastLine(ctx, trx, C_Forecast_ID, M_Product_ID);
            }
            return retValue;
        }

    }
}
