using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Model;
using System.Data;

namespace VAdvantage.Process
{
    class CreateTaxLines: SvrProcess
    {
        string qry = "";
        //DateTime? stDate,eDate;
        Decimal rate = 0, taxAmount = 0;
        int acct_Comb = 0;
        DataSet ds = null;
        MIncomeTax tax = null;
        MIncomeTaxLines tLine = null;
        protected override string DoIt()
        {
            DB.ExecuteQuery("DELETE FROM C_IncomeTaxLines WHERE C_IncomeTax_ID=" + GetRecord_ID());
            tax = new MIncomeTax(GetCtx(), GetRecord_ID(), Get_Trx());
            //qry = "SELECT ct.Rate,ta.T_Due_Acct,ev.value,ev.name FROM C_IncomeTax tx INNER JOIN C_Tax ct ON (tx.C_Tax_ID=ct.C_Tax_ID) INNER JOIN C_Tax_Acct ta ON (tx.C_Tax_ID = ta.C_Tax_ID) inner join c_validCombination ac on(ta.T_Due_Acct=ac.c_validCombination_id) inner join VAB_Acct_Element ev on(ac.Account_ID=ev.VAB_Acct_Element_id) WHERE tx.C_IncomeTax_ID=" + GetRecord_ID() + " and tx.vaf_client_id=" + GetVAF_Client_ID();
            qry = "SELECT ct.Rate,ta.IncomeSummary_Acct,ev.value,ev.name FROM C_IncomeTax tx INNER JOIN C_Tax ct ON (tx.C_Tax_ID=ct.C_Tax_ID) INNER JOIN VAB_AccountBook_GL ta ON (tx.VAF_Client_ID = ta.VAF_Client_ID) inner join c_validCombination ac on(ta.IncomeSummary_Acct=ac.c_validCombination_id) inner join VAB_Acct_Element ev on(ac.Account_ID=ev.VAB_Acct_Element_id) WHERE tx.C_IncomeTax_ID=" + GetRecord_ID() + " and tx.vaf_client_id=" + GetVAF_Client_ID();
            ds=DB.ExecuteDataset(qry,null,Get_Trx());
            if(ds !=null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    rate = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["Rate"]);
                    taxAmount = tax.GetProfitBeforeTax() * rate / 100;
                    acct_Comb = Util.GetValueOfInt(ds.Tables[0].Rows[0]["IncomeSummary_Acct"]);
                    tLine = new MIncomeTaxLines(GetCtx(), 0, Get_Trx());
                    tLine.SetVAF_Client_ID(GetVAF_Client_ID());
                    tLine.SetVAF_Org_ID(GetCtx().GetVAF_Org_ID());
                    tLine.SetC_IncomeTax_ID(GetRecord_ID());
                    tLine.SetC_Tax_ID(tax.GetC_Tax_ID());
                    tLine.SetC_IncomeTax_Acct(acct_Comb);
                    tLine.SetC_ProfitAndLoss_ID(tax.GetC_ProfitAndLoss_ID());
                    tLine.SetIncomeTaxAmount(taxAmount);
                    tLine.SetLedgerCode(Util.GetValueOfString(ds.Tables[0].Rows[0]["value"]));
                    tLine.SetLedgerName(Util.GetValueOfString(ds.Tables[0].Rows[0]["name"]));
                    if (!tLine.Save())
                    {
                        ds.Dispose();
                        return GetRetrievedError(tLine, "TaxLinesNotSaved");
                        //return Msg.GetMsg(GetCtx(), "TaxLinesNotSaved");
                    }

                    tax.SetIncomeTaxAmount(taxAmount);
                    tax.SetProfitAfterTax(tax.GetProfitBeforeTax() - taxAmount);
                    if (!tax.Save())
                    {
                        ds.Dispose();
                        Rollback();
                        return GetRetrievedError(tax, "TaxNotSaved");
                        //return Msg.GetMsg(GetCtx(), "TaxNotSaved");
                    }
                    ds.Dispose();
                    return Msg.GetMsg(GetCtx(), "LinesGenerated");
                }
            }
            return Msg.GetMsg(GetCtx(), "RecordNotFound"); 
        }


        protected override void Prepare()
        {
            
        }
    }
}
