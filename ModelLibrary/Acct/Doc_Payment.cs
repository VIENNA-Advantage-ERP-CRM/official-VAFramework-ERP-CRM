/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Doc_Payment
 * Purpose        : Post Invoice Documents.
 *                  <pre>
 *                   Table:              C_Payment (335)
 *                   Document Types      ARP, APP
 *                   </pre>
 * Class Used     : Doc
 * Chronological    Development
 * Raghunandan      19-Jan-2010
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using System.Data.SqlClient;
using VAdvantage.Acct;

namespace VAdvantage.Acct
{
    public class Doc_Payment : Doc
    {
        //	Tender Type			
        private String _TenderType = null;
        // Prepayment			
        private bool _Prepayment = false;
        // Bank Account			
        private int _VAB_Bank_Acct_ID = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public Doc_Payment(MAcctSchema[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(MPayment), idr, null, trxName)
        {

        }
        public Doc_Payment(MAcctSchema[] ass, DataRow dr, Trx trxName)
            : base(ass, typeof(MPayment), dr, null, trxName)
        {

        }

        /// <summary>
        /// Load Specific Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            MPayment pay = (MPayment)GetPO();
            SetDateDoc(pay.GetDateTrx());
            _TenderType = pay.GetTenderType();
            _Prepayment = pay.IsPrepayment();
            _VAB_Bank_Acct_ID = pay.GetVAB_Bank_Acct_ID();
            //	Amount
            SetAmount(Doc.AMTTYPE_Gross, pay.GetPayAmt());
            return null;
        }

        /// <summary>
        /// Get Source Currency Balance - always zero
        /// </summary>
        /// <returns>Zero (always balanced)</returns>
        public override Decimal GetBalance()
        {
            Decimal retValue = Env.ZERO;
            //log.Config( ToString() + " Balance=" + retValue);
            return retValue;
        }

        /// <summary>
        /// Create Facts (the accounting logic) for
        ///  ARP, APP.
        ///<pre>
        ///  ARP
        ///      BankInTransit   DR
        ///      UnallocatedCash         CR
        ///      or Charge/C_Prepayment
        ///  APP
        ///      PaymentSelect   DR
        ///      or Charge/V_Prepayment
        ///      BankInTransit           CR
        ///  CashBankTransfer
        ///      -
        ///  </pre>
        /// </summary>
        /// <param name="as1"></param>
        /// <returns>fact</returns>
        public override List<Fact> CreateFacts(MAcctSchema as1)
        {
            //  create Fact Header
            Fact fact = new Fact(this, as1, Fact.POST_Actual);
            //	Cash Transfer
            if ("X".Equals(_TenderType))
            {
                List<Fact> facts = new List<Fact>();
                facts.Add(fact);
                return facts;
            }

            int VAF_Org_ID = GetBank_Org_ID();		//	Bank Account Org	
            if (GetDocumentType().Equals(MDocBaseType.DOCBASETYPE_ARRECEIPT))
            {
                // Work done by Bharat for the posting of ED008 Module

                bool addPost = false;
                //Check For Module
                Tuple<String, String, String> aInfo = null;
                if (Env.HasModulePrefix("ED008_", out aInfo))
                {
                    addPost = true;
                }
                else
                {
                    addPost = false;
                }

                if (addPost == true)
                {
                    // Tender Type RIBA
                    if ("R".Equals(_TenderType))
                    {
                        MAccount acct = null;
                        MAccount portAcct = null;
                        int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_RIBA_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=" + GetVAB_Bank_Acct_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                        if (validComID > 0)
                        {
                            acct = MAccount.Get(Env.GetCtx(), validComID);
                        }

                        if (acct == null)
                        {
                            validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_RIBA_Acct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                            acct = MAccount.Get(Env.GetCtx(), validComID);
                        }
                        FactLine fl = fact.CreateLine(null, acct, GetVAB_Currency_ID(), GetAmount(), null);

                        int ComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_Portfolio_Acct FROM C_BP_Customer_Acct WHERE VAB_BusinessPartner_ID=" + GetVAB_BusinessPartner_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                        if (ComID > 0)
                        {
                            portAcct = MAccount.Get(Env.GetCtx(), ComID);
                        }

                        if (portAcct == null)
                        {
                            ComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_Portfolio_Acct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                            portAcct = MAccount.Get(Env.GetCtx(), ComID);
                        }
                        fl = fact.CreateLine(null, portAcct, GetVAB_Currency_ID(), null, GetAmount());
                    }
                    // Tender Type MAV
                    else if ("M".Equals(_TenderType))
                    {
                        MAccount acct = null;
                        MAccount portAcct = null;
                        int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_MAV_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=" + GetVAB_Bank_Acct_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                        if (validComID > 0)
                        {
                            acct = MAccount.Get(Env.GetCtx(), validComID);
                        }

                        if (acct == null)
                        {
                            validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_MAV_Acct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                            acct = MAccount.Get(Env.GetCtx(), validComID);
                        }
                        FactLine fl = fact.CreateLine(null, acct,
                        GetVAB_Currency_ID(), GetAmount(), null);

                        int ComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_Portfolio_Acct FROM C_BP_Customer_Acct WHERE VAB_BusinessPartner_ID=" + GetVAB_BusinessPartner_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                        if (ComID > 0)
                        {
                            portAcct = MAccount.Get(Env.GetCtx(), ComID);
                        }

                        if (portAcct == null)
                        {
                            ComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_Portfolio_Acct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                            portAcct = MAccount.Get(Env.GetCtx(), ComID);
                        }
                        fl = fact.CreateLine(null, portAcct,
                        GetVAB_Currency_ID(), null, GetAmount());
                    }
                    // Tender Type BOE
                    else if ("E".Equals(_TenderType))
                    {
                        MAccount acct = null;
                        MAccount portAcct = null;
                        string boeType = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT ED008_BOEType FROM C_Payment WHERE C_Payment_ID = " + Get_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                        int boeID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED008_BOE_ID FROM C_Payment WHERE C_Payment_ID = " + Get_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                        if ("T".Equals(boeType))
                        {
                            int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_BOE_Acct FROM ED008_BOEAccounting WHERE ED008_BOE_ID=" + boeID + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                            if (validComID > 0)
                            {
                                acct = MAccount.Get(Env.GetCtx(), validComID);
                            }

                            if (acct == null)
                            {
                                validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_BOE_Acct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                                acct = MAccount.Get(Env.GetCtx(), validComID);
                            }
                            FactLine fl = fact.CreateLine(null, acct, GetVAB_Currency_ID(), GetAmount(), null);

                            int ComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_BOETransit_Acct FROM ED008_BOEAccounting WHERE ED008_BOE_ID=" + boeID + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                            if (ComID > 0)
                            {
                                portAcct = MAccount.Get(Env.GetCtx(), ComID);
                            }

                            if (portAcct == null)
                            {
                                ComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_BOETransit_Acct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                                portAcct = MAccount.Get(Env.GetCtx(), ComID);
                            }
                            fl = fact.CreateLine(null, portAcct, GetVAB_Currency_ID(), null, GetAmount());
                        }

                        else if ("R".Equals(boeType))
                        {
                            FactLine fl = fact.CreateLine(null, GetAccount(Doc.ACCTTYPE_BankInTransit, as1), GetVAB_Currency_ID(), GetAmount(), null);

                            int ComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_BOE_Acct FROM ED008_BOEAccounting WHERE ED008_BOE_ID=" + boeID + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                            if (ComID > 0)
                            {
                                portAcct = MAccount.Get(Env.GetCtx(), ComID);
                            }

                            if (portAcct == null)
                            {
                                ComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_BOE_Acct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                                portAcct = MAccount.Get(Env.GetCtx(), ComID);
                            }
                            fl = fact.CreateLine(null, portAcct, GetVAB_Currency_ID(), null, GetAmount());
                        }
                    }
                    // Tender Type RID
                    else if ("I".Equals(_TenderType))
                    {
                        MAccount acct = null;
                        MAccount portAcct = null;
                        int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_RID_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=" + GetVAB_Bank_Acct_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                        if (validComID > 0)
                        {
                            acct = MAccount.Get(Env.GetCtx(), validComID);
                        }

                        if (acct == null)
                        {
                            validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_RID_Acct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                            acct = MAccount.Get(Env.GetCtx(), validComID);
                        }
                        FactLine fl = fact.CreateLine(null, acct,
                        GetVAB_Currency_ID(), GetAmount(), null);

                        int ComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_Portfolio_Acct FROM C_BP_Customer_Acct WHERE VAB_BusinessPartner_ID=" + GetVAB_BusinessPartner_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                        if (ComID > 0)
                        {
                            portAcct = MAccount.Get(Env.GetCtx(), ComID);
                        }

                        if (portAcct == null)
                        {
                            ComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_Portfolio_Acct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                            portAcct = MAccount.Get(Env.GetCtx(), ComID);
                        }
                        fl = fact.CreateLine(null, portAcct,
                        GetVAB_Currency_ID(), null, GetAmount());
                    }

                    else
                    {
                        //	Asset
                        FactLine fl = fact.CreateLine(null, GetAccount(Doc.ACCTTYPE_BankInTransit, as1),
                            GetVAB_Currency_ID(), GetAmount(), null);
                        if (fl != null && VAF_Org_ID != 0)
                        {
                            fl.SetVAF_Org_ID(VAF_Org_ID);
                        }
                        //	
                        MAccount acct = null;
                        if (GetVAB_Charge_ID() != 0)
                        {
                            acct = MCharge.GetAccount(GetVAB_Charge_ID(), as1, GetAmount());
                        }
                        else if (_Prepayment)
                        {
                            acct = GetAccount(Doc.ACCTTYPE_C_Prepayment, as1);
                        }
                        else
                        {
                            acct = GetAccount(Doc.ACCTTYPE_UnallocatedCash, as1);
                        }
                        fl = fact.CreateLine(null, acct, GetVAB_Currency_ID(), null, GetAmount());
                        if (fl != null && VAF_Org_ID != 0
                            && GetVAB_Charge_ID() == 0)		//	don't overwrite charge
                        {
                            fl.SetVAF_Org_ID(VAF_Org_ID);
                        }
                    }
                }
                // Default Posting Logic
                else
                {
                    //	Asset
                    FactLine fl = fact.CreateLine(null, GetAccount(Doc.ACCTTYPE_BankInTransit, as1),
                        GetVAB_Currency_ID(), GetAmount(), null);
                    if (fl != null && VAF_Org_ID != 0)
                    {
                        fl.SetVAF_Org_ID(VAF_Org_ID);
                    }
                    //	
                    MAccount acct = null;
                    if (GetVAB_Charge_ID() != 0)
                    {
                        acct = MCharge.GetAccount(GetVAB_Charge_ID(), as1, GetAmount());
                    }
                    else if (_Prepayment)
                    {
                        acct = GetAccount(Doc.ACCTTYPE_C_Prepayment, as1);
                    }
                    else
                    {
                        acct = GetAccount(Doc.ACCTTYPE_UnallocatedCash, as1);
                    }
                    fl = fact.CreateLine(null, acct,
                        GetVAB_Currency_ID(), null, GetAmount());
                    if (fl != null && VAF_Org_ID != 0
                        && GetVAB_Charge_ID() == 0)		//	don't overwrite charge
                    {
                        fl.SetVAF_Org_ID(VAF_Org_ID);
                    }
                }
            }
            //  APP
            else if (GetDocumentType().Equals(MDocBaseType.DOCBASETYPE_APPAYMENT))
            {
                // Work Done For Ed008 Module
                bool addPost = false;
                //Check For Module
                Tuple<String, String, String> aInfo = null;
                if (Env.HasModulePrefix("ED008_", out aInfo) || Env.HasModulePrefix("ED010_", out aInfo))
                {
                    addPost = true;
                }

                if (addPost == true)
                {
                    MAccount acct = null;
                    MAccount portAcct = null;
                    string boeType = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT ED008_BOEType FROM C_Payment WHERE C_Payment_ID = " + Get_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                    int boeID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED008_BOE_ID FROM C_Payment WHERE C_Payment_ID = " + Get_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                    if ("E".Equals(_TenderType))
                    {
                        if ("T".Equals(boeType))
                        {
                            int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_BOETransit_Acct FROM ED008_BOEAccounting WHERE ED008_BOE_ID=" + boeID + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                            if (validComID > 0)
                            {
                                acct = MAccount.Get(Env.GetCtx(), validComID);
                            }

                            if (acct == null)
                            {
                                validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_BOETransit_Acct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                                acct = MAccount.Get(Env.GetCtx(), validComID);
                            }
                            FactLine f2 = fact.CreateLine(null, acct, GetVAB_Currency_ID(), GetAmount(), null);

                            int ComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_BOE_Acct FROM ED008_BOEAccounting WHERE ED008_BOE_ID=" + boeID + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                            if (ComID > 0)
                            {
                                portAcct = MAccount.Get(Env.GetCtx(), ComID);
                            }

                            if (portAcct == null)
                            {
                                ComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_BOE_Acct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                                portAcct = MAccount.Get(Env.GetCtx(), ComID);
                            }
                            f2 = fact.CreateLine(null, portAcct, GetVAB_Currency_ID(), null, GetAmount());
                        }
                    }
                    else
                    {
                        FactLine fl = fact.CreateLine(null, GetAccount(Doc.ACCTTYPE_BankInTransit, as1), GetVAB_Currency_ID(), null, GetAmount());
                        if (fl != null && VAF_Org_ID != 0
                            && GetVAB_Charge_ID() == 0)		//	don't overwrite charge
                            fl.SetVAF_Org_ID(VAF_Org_ID);

                        DataSet ds = null;
                        DataSet dsSSCode = null;
                        decimal amount = 0, WithholdingAmt = 0, sscAmt = 0, payAmt = 0;
                        payAmt = GetAmount();

                        string sql = @"SELECT cl.m_product_id, cl.linenetamt,prd.ed010_sscode_id,holddet.ed010_appliedpercentage,holddet.ed010_actualpercentage,withAcct.Withholding_Acct
                        FROM C_Payment pay INNER JOIN VAB_Invoice inv ON pay.VAB_Invoice_id = inv.VAB_Invoice_id inner join VAB_InvoiceLine cl  on pay.VAB_Invoice_id=cl.VAB_Invoice_id inner join VAB_BusinessPartner cb 
                        on pay.VAB_BusinessPartner_id=cb.VAB_BusinessPartner_id INNER JOIN m_product prd ON prd.m_product_ID = cl.m_product_ID LEFT JOIN c_withholding hold ON hold.C_WithHolding_Id = prd.c_withholding_id
                        LEFT JOIN ed010_withholdingdetails holddet ON holddet.C_WithHolding_Id = hold.C_WithHolding_Id left join C_Withholding_Acct withAcct on hold.C_WithHolding_Id=withAcct.C_WithHolding_Id
                        where cb.ED010_IsWithholding='Y' and cb.ED010_IsSSCode='Y' and pay.ED010_WithholdingAmt > 0 AND " + GlobalVariable.TO_DATE(DateTime.Now.ToLocalTime(), true) +
                            @"BETWEEN holddet.ED010_FromDate AND holddet.ED010_ToDate and pay.VAF_Org_ID = " + GetCtx().GetVAF_Org_ID() + " and pay.c_payment_id = " + Get_ID();

                        ds = new DataSet();
                        ds = DB.ExecuteDataset(sql, null, null);
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    MAccount withAcct = null;
                                    int validID = 0;
                                    amount = (Decimal.Divide(Decimal.Multiply(Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["linenetamt"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ed010_appliedpercentage"])), 100));
                                    WithholdingAmt = (Decimal.Divide(Decimal.Multiply(amount, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ed010_actualpercentage"])), 100));

                                    validID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["Withholding_Acct"]);
                                    if (validID > 0)
                                    {
                                        withAcct = MAccount.Get(Env.GetCtx(), validID);
                                    }
                                    if (withAcct == null)
                                    {
                                        validID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT Withholding_Acct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                                        withAcct = MAccount.Get(Env.GetCtx(), validID);
                                    }
                                    fl = fact.CreateLine(null, withAcct, GetVAB_Currency_ID(), null, Decimal.Round(WithholdingAmt, 2));
                                    if (fl != null)
                                    {
                                        payAmt += Decimal.Round(WithholdingAmt, 2);
                                    }
                                    if (fl != null && VAF_Org_ID != 0)
                                    {
                                        fl.SetVAF_Org_ID(VAF_Org_ID);
                                    }
                                }
                            }
                        }
                        ds.Dispose();

                        sql = @"SELECT cl.m_product_id,cb.VAB_BusinessPartner_id,cl.linenetamt,prd.ed010_sscode_id,sdet.ed010_percentagetype,sdet.ed010_maxamt,sdet.ed010_minamt,sdet.ed010_socialsecurityprcnt,sdet.ed010_orgpercentage,
                        sdet.ed010_vendorpercentage,sscAcct.ED000_SecCodeAcct FROM C_Payment pay INNER JOIN VAB_Invoice inv ON pay.VAB_Invoice_id = inv.VAB_Invoice_id inner join VAB_InvoiceLine cl  on pay.VAB_Invoice_id=cl.VAB_Invoice_id inner join VAB_BusinessPartner cb 
                        on pay.VAB_BusinessPartner_id=cb.VAB_BusinessPartner_id INNER JOIN m_product prd ON prd.m_product_ID = cl.m_product_ID LEFT JOIN ed010_sscode scode ON scode.ed010_sscode_ID = prd.ed010_sscode_id
                        LEFT JOIN ed010_sscodedetails sdet ON sdet.ed010_sscode_ID = scode.ed010_sscode_ID left join ED010_SSCode_Acct sscAcct on scode.ED010_SSCode_ID=sscAcct.ED010_SSCode_ID
                        where cb.ED010_IsWithholding='Y' and cb.ED010_IsSSCode='Y' and pay.ED010_WithholdingAmt > 0 AND " + GlobalVariable.TO_DATE(DateTime.Now.ToLocalTime(), true) +
                            @"BETWEEN sdet.ED010_FromDate AND sdet.ED010_ToDate AND pay.VAF_Org_ID = " + GetCtx().GetVAF_Org_ID() + " and pay.c_payment_id = " + Get_ID();

                        dsSSCode = new DataSet();
                        dsSSCode = DB.ExecuteDataset(sql, null, null);
                        if (dsSSCode.Tables.Count > 0)
                        {
                            if (dsSSCode.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dsSSCode.Tables[0].Rows.Count; i++)
                                {
                                    MAccount sscAcct = null;
                                    int vlID = 0;
                                    sql = @"SELECT SUM(grandtotal) FROM VAB_Invoice WHERE IsActive = 'Y' AND  docstatus = 'CO' AND VAB_BusinessPartner_Id = " + Util.GetValueOfInt(dsSSCode.Tables[0].Rows[i]["VAB_BusinessPartner_id"]);
                                    decimal result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                                    if (result <= Util.GetValueOfDecimal(dsSSCode.Tables[0].Rows[i]["ed010_maxamt"]) && result >= Util.GetValueOfDecimal(dsSSCode.Tables[0].Rows[i]["ed010_minamt"]))
                                    {
                                        if (Util.GetValueOfString(dsSSCode.Tables[0].Rows[i]["ed010_percentagetype"]) == "S")
                                        {
                                            sscAmt = (Decimal.Divide(Decimal.Multiply(Util.GetValueOfDecimal(dsSSCode.Tables[0].Rows[i]["linenetamt"]), Util.GetValueOfDecimal(dsSSCode.Tables[0].Rows[i]["ed010_socialsecurityprcnt"])), 100));
                                        }
                                        else if (Util.GetValueOfString(dsSSCode.Tables[0].Rows[i]["ed010_percentagetype"]) == "M")
                                        {
                                            //  amount = (Decimal.Divide(Decimal.Multiply(Util.GetValueOfDecimal(dsSSCode.Tables[0].Rows[i]["linenetamt"]), Util.GetValueOfDecimal(dsSSCode.Tables[0].Rows[i]["ed010_vendorpercentage"])), 100));
                                            sscAmt = (Decimal.Divide(Decimal.Multiply(Util.GetValueOfDecimal(dsSSCode.Tables[0].Rows[i]["linenetamt"]), Util.GetValueOfDecimal(dsSSCode.Tables[0].Rows[i]["ed010_orgpercentage"])), 100));
                                        }
                                    }

                                    vlID = Util.GetValueOfInt(dsSSCode.Tables[0].Rows[i]["ED000_SecCodeAcct"]);
                                    if (vlID > 0)
                                    {
                                        sscAcct = MAccount.Get(Env.GetCtx(), vlID);
                                    }
                                    if (sscAcct == null)
                                    {
                                        vlID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_SecCodeAcct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                                        sscAcct = MAccount.Get(Env.GetCtx(), vlID);
                                    }
                                    fl = fact.CreateLine(null, sscAcct, GetVAB_Currency_ID(), null, Decimal.Round(sscAmt, 2));
                                    if (fl != null)
                                    {
                                        payAmt += Decimal.Round(sscAmt, 2);
                                    }
                                    if (fl != null && VAF_Org_ID != 0)
                                    {
                                        fl.SetVAF_Org_ID(VAF_Org_ID);
                                    }
                                }
                            }
                        }
                        dsSSCode.Dispose();

                        fl = fact.CreateLine(null, GetAccount(Doc.ACCTTYPE_PaymentSelect, as1), GetVAB_Currency_ID(), payAmt, null);
                        if (fl != null && VAF_Org_ID != 0 && GetVAB_Charge_ID() == 0)		//	don't overwrite charge
                            fl.SetVAF_Org_ID(VAF_Org_ID);
                    }
                }

                // Default Posting Logic
                else
                {
                    MAccount acct = null;
                    if (GetVAB_Charge_ID() != 0)
                    {
                        acct = MCharge.GetAccount(GetVAB_Charge_ID(), as1, GetAmount());
                    }
                    else if (_Prepayment)
                    {
                        acct = GetAccount(Doc.ACCTTYPE_V_Prepayment, as1);
                    }
                    else
                    {
                        acct = GetAccount(Doc.ACCTTYPE_PaymentSelect, as1);
                    }
                    FactLine fl = fact.CreateLine(null, acct,
                        GetVAB_Currency_ID(), GetAmount(), null);
                    if (fl != null && VAF_Org_ID != 0
                        && GetVAB_Charge_ID() == 0)		//	don't overwrite charge
                        fl.SetVAF_Org_ID(VAF_Org_ID);

                    //	Asset
                    fl = fact.CreateLine(null, GetAccount(Doc.ACCTTYPE_BankInTransit, as1),
                        GetVAB_Currency_ID(), null, GetAmount());
                    if (fl != null && VAF_Org_ID != 0)
                    {
                        fl.SetVAF_Org_ID(VAF_Org_ID);
                    }
                }
            }
            else
            {
                _error = "DocumentType unknown: " + GetDocumentType();
                log.Log(Level.SEVERE, _error);
                fact = null;
            }
            //
            List<Fact> facts1 = new List<Fact>();
            facts1.Add(fact);
            return facts1;
        }

        /// <summary>
        /// Get VAF_Org_ID from Bank Account
        /// </summary>
        /// <returns>VAF_Org_ID or 0</returns>
        private int GetBank_Org_ID()
        {
            if (_VAB_Bank_Acct_ID == 0)
            {
                return 0;
            }
            //
            MBankAccount ba = MBankAccount.Get(GetCtx(), _VAB_Bank_Acct_ID);
            return ba.GetVAF_Org_ID();
        }

    }
}
