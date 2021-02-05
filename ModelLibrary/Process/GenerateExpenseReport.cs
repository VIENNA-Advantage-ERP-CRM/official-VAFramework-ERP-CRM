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
//////using System.Windows.Forms;
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
            VAdvantage.Model.X_VAS_ExpenseReport tExp = new VAdvantage.Model.X_VAS_ExpenseReport(GetCtx(), 0, Get_TrxName());
            tExp.SetVAF_Client_ID(GetCtx().GetVAF_Client_ID());
            tExp.SetVAF_Org_ID(GetCtx().GetVAF_Org_ID());
            tExp.SetDateReport(Util.GetValueOfDateTime(System.DateTime.Now));

            sql = "select VAM_PriceList_ID from VAM_PriceList where isdefault ='Y' and issopricelist = 'Y' and isactive= 'Y' and vaf_org_id = " + GetCtx().GetVAF_Org_ID();
            int VAM_PriceList_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
            if (VAM_PriceList_ID == 0)
            {
                log.SaveError("PriceListNotFound", "PriceListNotFound");
                return 1;
            }

            tExp.SetVAM_PriceList_ID(VAM_PriceList_ID);
            tExp.SetVAM_Warehouse_ID(1000030);
            tExp.SetVAB_BusinessPartner_ID(1001904);
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
            VAdvantage.Model.X_VAS_ExpenseReportLine tLine = new VAdvantage.Model.X_VAS_ExpenseReportLine(GetCtx(), 0, Get_TrxName());
            tLine.SetVAF_Client_ID(GetCtx().GetVAF_Client_ID());
            tLine.SetVAF_Org_ID(GetCtx().GetVAF_Org_ID());
            tLine.SetVAS_ExpenseReport_ID(S_TimeExpence_ID);
            tLine.SetDateExpense(rTime.GetDate1());
            tLine.SetVAM_Product_ID(rTime.GetVAM_Product_ID());
            tLine.SetVAB_OrderLine_ID(rTime.GetVAB_OrderLine_ID());
            tLine.SetVAB_UOM_ID(rTime.GetVAB_UOM_ID());
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
