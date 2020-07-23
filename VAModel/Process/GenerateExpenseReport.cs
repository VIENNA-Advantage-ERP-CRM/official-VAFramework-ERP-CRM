/********************************************************
 * Project Name   : ViennaAdvantage
 * Class Name     : GenerateExpenseReport
 * Class Used     : SvrProcess
 * Chronological    Development
 * Lokesh Chauhan   25-Apr-2012
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
////using VAdvantage.Common;
using ViennaAdvantage.Process;
////using System.Windows.Forms;
//using ViennaAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;


using VAdvantage.Process;

namespace ViennaAdvantage.Process
{
    public class GenerateExpenseReport : SvrProcess
    {
        #region Private Variables
        //int value = 0;
        string msg = "";
        string sql = "";
        int C_ResourcePeriod_ID = 0;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected override void Prepare()
        {
            C_ResourcePeriod_ID = Util.GetValueOfInt(GetRecord_ID());
        } //	prepare


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override String DoIt()
        {
            if (C_ResourcePeriod_ID != 0)
            {
                int[] allIds = VAdvantage.Model.X_C_ResourceTime.GetAllIDs("C_ResourceTime", "C_ResourcePeriod_ID = " + C_ResourcePeriod_ID, Get_TrxName());
                int S_TimeExpence_ID = 0;
                for (int i = 0; i < allIds.Length; i++)
                {
                    VAdvantage.Model.X_C_ResourceTime rTime = new VAdvantage.Model.X_C_ResourceTime(GetCtx(), allIds[i], Get_TrxName());
                    if (S_TimeExpence_ID == 0)
                    {
                        int res = GenerateTimeExpense();
                        if (res == 1)
                        {
                            msg = Msg.GetMsg(GetCtx(), "PriceListNotFound");
                            return msg;
                        }
                        else if (res == 5)
                        {
                            msg = Msg.GetMsg(GetCtx(), "WarehouseNotFound");
                            return msg;
                        }
                    }
                    GenerateExpenseReportLine(S_TimeExpence_ID, rTime);
                }
            }
            return msg;
        }



        /// <summary>
        /// Generate Header for Expense Report
        /// </summary>
        private int GenerateTimeExpense()
        {
            VAdvantage.Model.X_S_TimeExpense tExp = new VAdvantage.Model.X_S_TimeExpense(GetCtx(), 0, Get_TrxName());
            tExp.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
            tExp.SetAD_Org_ID(GetCtx().GetAD_Org_ID());
            tExp.SetDateReport(Util.GetValueOfDateTime(System.DateTime.Now));

            sql = "select M_Pricelist_ID from m_Pricelist where isdefault ='Y' and issopricelist = 'Y' and isactive= 'Y' and AD_org_id = " + GetCtx().GetAD_Org_ID();
            int M_Pricelist_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
            if (M_Pricelist_ID == 0)
            {
                log.SaveError("PriceListNotFound", "PriceListNotFound");
                return 1;
            }

            tExp.SetM_PriceList_ID(M_Pricelist_ID);
            tExp.SetM_Warehouse_ID(1000030);
            tExp.SetC_BPartner_ID(1001904);
            if (!tExp.Save(Get_TrxName()))
            {
                log.SaveError("ExpenseReportHeaderNotSaved", "ExpenseReportHeaderNotSaved");
                return 5;
            }
            return 0;
        }

        /// <summary>
        /// Generate Lines 
        /// </summary>
        /// <param name="S_TimeExpence_ID"></param>
        /// <param name="rTime"></param>
        private void GenerateExpenseReportLine(int S_TimeExpence_ID, VAdvantage.Model.X_C_ResourceTime rTime)
        {
            VAdvantage.Model.X_S_TimeExpenseLine tLine = new VAdvantage.Model.X_S_TimeExpenseLine(GetCtx(), 0, Get_TrxName());
            tLine.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
            tLine.SetAD_Org_ID(GetCtx().GetAD_Org_ID());
            tLine.SetS_TimeExpense_ID(S_TimeExpence_ID);
            tLine.SetDateExpense(rTime.GetDate1());
            tLine.SetM_Product_ID(rTime.GetM_Product_ID());
            tLine.SetC_OrderLine_ID(rTime.GetC_OrderLine_ID());
            tLine.SetC_UOM_ID(rTime.GetC_UOM_ID());
            tLine.SetIsTimeReport(true);
            tLine.SetQty(rTime.GetActualHrs());
            tLine.SetActualQty(rTime.GetActualHrs());
            if (!tLine.Save(Get_TrxName()))
            {
                log.SaveError("ExpenseLineNotSaved", "ExpenseLineNotSaved");
                return;
            }
        }
    }
}
