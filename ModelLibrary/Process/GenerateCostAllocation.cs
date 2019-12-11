using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using VAdvantage.Model;
namespace VAdvantage.Process
{
  public  class GenerateCostAllocation :SvrProcess
    {
       protected override void Prepare()
       {
           
       }
       protected override string DoIt()
       {
           MAllocationLine objMAllocationLine = new MAllocationLine(GetCtx(), GetRecord_ID(), Get_Trx());
           if (objMAllocationLine.GetM_CostAllocation_ID() == 0)
           {
               MPayment objMPayment = new MPayment(GetCtx(), objMAllocationLine.GetC_Payment_ID(), Get_Trx());
               string AlloctionMsg="";
               //if (objMPayment.GenerateCostAllocation(objMPayment.GetDocumentNo(), GetAD_Client_ID(), Get_Trx(), GetAD_Org_ID(), out AlloctionMsg))
               //{
               //    return "Done";
               //}
               //else
               //{
                   return AlloctionMsg;
              // }
           }
           else
           {
               return "Cost Allocation record already generated";
           }
          
       }
    }
}
