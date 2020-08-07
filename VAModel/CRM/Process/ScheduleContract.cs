using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VAdvantage.Logging;
//using ViennaAdvantage.Model;
using VAdvantage.DataBase;
using System.Data;

/* Process: Create Contract Schedule 
 * Writer :Arpit Singh
 * Date   : 31/1/12 
 */
namespace VAdvantage.Process
{
    class ScheduleContract : SvrProcess
    {
        int contractID;
        protected override void Prepare()
        {
            contractID = GetRecord_ID();
        }

        protected override String DoIt()
        {
            VAdvantage.Model.X_C_Contract contract = new VAdvantage.Model.X_C_Contract(GetCtx(), contractID, null);
            DateTime start = (DateTime)contract.GetBillStartDate();
            DateTime end = (DateTime)contract.GetEndDate();
            int frequency = contract.GetC_Frequency_ID();

            string Sql = "Select NoofMonths from C_Frequency where C_Frequency_ID=" + frequency;
            int months = Util.GetValueOfInt(DB.ExecuteScalar(Sql, null, null));
            int count = Util.GetValueOfInt(contract.GetTotalInvoice());

            for (int i = 1; i <= count; i++)
            {

                VAdvantage.Model.X_C_ContractSchedule CSchedule = new VAdvantage.Model.X_C_ContractSchedule(GetCtx(), 0, null);
                CSchedule.SetC_Contract_ID(contractID);
                CSchedule.SetC_BPartner_ID(contract.GetC_BPartner_ID());

                CSchedule.SetFROMDATE(start);
                CSchedule.SetProcessed(true);

                CSchedule.SetUnitsDelivered(contract.GetQtyEntered());
                CSchedule.SetFROMDATE(start);
                DateTime toDate = start.AddMonths(months);
                toDate = toDate.AddDays(-1);
                CSchedule.SetEndDate(toDate);
                start = start.AddMonths(months);
                // if (i != count)
                // {

                // CSchedule.SetNoOfDays(days);
                // }
                // else
                // {
                //     CSchedule.SetEndDate(end);
                //     CSchedule.SetNoOfDays((end - start).Days+1);
                // }
                CSchedule.SetM_Product_ID(contract.GetM_Product_ID());
                CSchedule.SetTotalAmt(contract.GetLineNetAmt());
                CSchedule.SetGrandTotal(contract.GetGrandTotal());
                CSchedule.SetTaxAmt(contract.GetTaxAmt());
                CSchedule.SetC_UOM_ID(contract.GetC_UOM_ID());
                CSchedule.SetPriceEntered(contract.GetPriceEntered());
                if (CSchedule.Save())
                {
                    contract.SetProcessed(true);
                    contract.SetScheduleContract("Y");
                    contract.Save();
                }

            }

            return Msg.GetMsg(GetCtx(), "ContractScheduledDone ");

        }

        //protected override String DoIt()
        //{
        //    X_C_Contract contract = new X_C_Contract(GetCtx(), contractID, null);
        //    DateTime start = (DateTime)contract.GetStartDate();
        //    DateTime end = (DateTime)contract.GetEndDate();
        //    int frequency = contract.GetC_Frequency_ID();

        //    string Sql = "Select NoofDays from C_Frequency where C_Frequency_ID=" + frequency;
        //    int days = Util.GetValueOfInt(DB.ExecuteScalar(Sql, null, null));
       
        //    int count = Util.GetValueOfInt(contract.GetTotalInvoice());

        //    for (int i = 1; i <= count; i++)
        //    {

        //        X_C_ContractSchedule CSchedule = new X_C_ContractSchedule(GetCtx(), 0, null);
        //        CSchedule.SetC_Contract_ID(contractID);
        //        CSchedule.SetC_BPartner_ID(contract.GetC_BPartner_ID());

        //        CSchedule.SetFROMDATE(start);
        //        CSchedule.SetProcessed(true);

        //        CSchedule.SetUnitsDelivered(contract.GetQtyEntered());
        //        CSchedule.SetFROMDATE(start);
        //        CSchedule.SetEndDate(start.AddDays(days - 1));
        //        start = start.AddDays(days);
        //        // if (i != count)
        //        // {

        //        // CSchedule.SetNoOfDays(days);
        //        // }
        //        // else
        //        // {
        //        //     CSchedule.SetEndDate(end);
        //        // CSchedule.SetNoOfDays((end - start).Days+1);
        //        // }
        //        CSchedule.SetM_Product_ID(contract.GetM_Product_ID());
        //        CSchedule.SetTotalAmt(contract.GetLineNetAmt());
        //        CSchedule.SetGrandTotal(contract.GetGrandTotal());
        //        CSchedule.SetTaxAmt(contract.GetTaxAmt());
        //        CSchedule.SetC_UOM_ID(contract.GetC_UOM_ID());
        //        CSchedule.SetPriceEntered(contract.GetPriceEntered());
        //        if (CSchedule.Save())
        //        {
        //            contract.SetProcessed(true);
        //            contract.SetScheduleContract("Y");
        //            contract.Save();
        //        }

        //    }





        //    return Msg.GetMsg(GetCtx(), "ContractScheduledDone ");

        //}

    }
}
