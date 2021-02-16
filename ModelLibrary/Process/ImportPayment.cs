/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ImportPayment
 * Purpose        : Import Payments
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           12-Feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class ImportPayment : ProcessEngine.SvrProcess
    {
        /**	Organization to be imported to	*/
        private int _VAF_Org_ID = 0;
        /** Default Bank Account			*/
        private int _VAB_Bank_Acct_ID = 0;
        /**	Delete old Imported				*/
        private bool _deleteOldImported = false;

        /** Properties						*/
        private Ctx _ctx;

        /// <summary>
        ///  Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (name.Equals("VAF_Org_ID"))
                    _VAF_Org_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("VAB_Bank_Acct_ID"))
                    _VAB_Bank_Acct_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("DeleteOldImported"))
                    _deleteOldImported = "Y".Equals(para[i].GetParameter());
                //	else if (name.Equals("DocAction"))
                //		m_docAction = (String)para[i].getParameter();
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
            _ctx = GetCtx();
        }	//	prepare

        /// <summary>
        /// doit
        /// </summary>
        /// <returns>Info</returns>
        protected override String DoIt()
        {
            log.Info("VAB_Bank_Acct_ID" + _VAB_Bank_Acct_ID);
            MVABBankAcct ba = MVABBankAcct.Get(GetCtx(), _VAB_Bank_Acct_ID);
            if (_VAB_Bank_Acct_ID == 0 || ba.Get_ID() != _VAB_Bank_Acct_ID)
                throw new Exception("@NotFound@ @VAB_Bank_Acct_ID@ - " + _VAB_Bank_Acct_ID);
            if (_VAF_Org_ID != ba.GetVAF_Org_ID() && ba.GetVAF_Org_ID() != 0)
                _VAF_Org_ID = ba.GetVAF_Org_ID();
            log.Info("VAF_Org_ID=" + _VAF_Org_ID);

            StringBuilder sql = null;
            int no = 0;
            String clientCheck = " AND VAF_Client_ID=" + ba.GetVAF_Client_ID();

            //	****	Prepare	****

            //	Delete Old Imported
            if (_deleteOldImported)
            {
                sql = new StringBuilder("DELETE FROM I_Payment "
                      + "WHERE I_IsImported='Y'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Delete Old Impored =" + no);
            }

            //	Set Client, Org, IsActive, Created/Updated
            sql = new StringBuilder("UPDATE I_Payment "
                  + "SET VAF_Client_ID = COALESCE (VAF_Client_ID,").Append(ba.GetVAF_Client_ID()).Append("),"
                  + " VAF_Org_ID = COALESCE (VAF_Org_ID,").Append(_VAF_Org_ID).Append("),");
            sql.Append(" IsActive = COALESCE (IsActive, 'Y'),"
                  + " Created = COALESCE (Created, SysDate),"
                  + " CreatedBy = COALESCE (CreatedBy, 0),"
                  + " Updated = COALESCE (Updated, SysDate),"
                  + " UpdatedBy = COALESCE (UpdatedBy, 0),"
                  + " I_ErrorMsg = NULL,"
                  + " I_IsImported = 'N' "
                  + "WHERE I_IsImported<>'Y' OR I_IsImported IS NULL OR VAF_Client_ID IS NULL OR VAF_Org_ID IS NULL OR VAF_Client_ID=0 OR VAF_Org_ID=0");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("Reset=" + no);

            String ts = DataBase.DB.IsPostgreSQL() ? "COALESCE(I_ErrorMsg,'')" : "I_ErrorMsg";  //java bug, it could not be used directly
            sql = new StringBuilder("UPDATE I_Payment o "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Org, '"
                + "WHERE (VAF_Org_ID IS NULL OR VAF_Org_ID=0"
                + " OR EXISTS (SELECT * FROM VAF_Org oo WHERE o.VAF_Org_ID=oo.VAF_Org_ID AND (oo.IsSummary='Y' OR oo.IsActive='N')))"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Org=" + no);

            //	Set Bank Account
            sql = new StringBuilder("UPDATE I_Payment i "
                + "SET VAB_Bank_Acct_ID="
                + "( "
                + " SELECT VAB_Bank_Acct_ID "
                + " FROM VAB_Bank_Acct a, VAB_Bank b "
                + " WHERE b.IsOwnBank='Y' "
                + " AND a.VAF_Client_ID=i.VAF_Client_ID "
                + " AND a.VAB_Bank_ID=b.VAB_Bank_ID "
                + " AND a.AccountNo=i.BankAccountNo "
                + " AND b.RoutingNo=i.RoutingNo "
                + " OR b.SwiftCode=i.RoutingNo "
                + ") "
                + "WHERE i.VAB_Bank_Acct_ID IS NULL "
                + "AND i.I_IsImported<>'Y' "
                + "OR i.I_IsImported IS NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Bank Account (With Routing No)=" + no);
            //
            sql = new StringBuilder("UPDATE I_Payment i "
                + "SET VAB_Bank_Acct_ID="
                + "( "
                + " SELECT VAB_Bank_Acct_ID "
                + " FROM VAB_Bank_Acct a, VAB_Bank b "
                + " WHERE b.IsOwnBank='Y' "
                + " AND a.VAB_Bank_ID=b.VAB_Bank_ID "
                + " AND a.AccountNo=i.BankAccountNo "
                + " AND a.VAF_Client_ID=i.VAF_Client_ID "
                + ") "
                + "WHERE i.VAB_Bank_Acct_ID IS NULL "
                + "AND i.I_isImported<>'Y' "
                + "OR i.I_isImported IS NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Bank Account (Without Routing No)=" + no);
            //
            sql = new StringBuilder("UPDATE I_Payment i "
                + "SET VAB_Bank_Acct_ID=(SELECT VAB_Bank_Acct_ID FROM VAB_Bank_Acct a WHERE a.VAB_Bank_Acct_ID=").Append(_VAB_Bank_Acct_ID);
            sql.Append(" and a.VAF_Client_ID=i.VAF_Client_ID) "
                + "WHERE i.VAB_Bank_Acct_ID IS NULL "
                + "AND i.BankAccountNo IS NULL "
                + "AND i.I_isImported<>'Y' "
                + "OR i.I_isImported IS NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Bank Account=" + no);
            //	
            sql = new StringBuilder("UPDATE I_Payment "
                + "SET I_isImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Bank Account, ' "
                + "WHERE VAB_Bank_Acct_ID IS NULL "
                + "AND I_isImported<>'Y' "
                + "OR I_isImported IS NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Bank Account=" + no);

            //	Set Currency
            sql = new StringBuilder("UPDATE I_Payment i "
                + "SET VAB_Currency_ID=(SELECT VAB_Currency_ID FROM VAB_Currency c"
                + " WHERE i.ISO_Code=c.ISO_Code AND c.VAF_Client_ID IN (0,i.VAF_Client_ID)) "
                + "WHERE VAB_Currency_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Set Currency=" + no);
            //
            sql = new StringBuilder("UPDATE I_Payment i "
                //jz	+ "SET i.VAB_Currency_ID=(SELECT VAB_Currency_ID FROM VAB_Bank_Acct WHERE VAB_Bank_Acct_ID=i.VAB_Bank_Acct_ID) "
                    + "SET VAB_Currency_ID=(SELECT VAB_Currency_ID FROM VAB_Bank_Acct WHERE VAB_Bank_Acct_ID=i.VAB_Bank_Acct_ID) "
                + "WHERE i.VAB_Currency_ID IS NULL "
                + "AND i.ISO_Code IS NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Set Currency=" + no);
            //
            sql = new StringBuilder("UPDATE I_Payment "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No Currency,' "
                + "WHERE VAB_Currency_ID IS NULL "
                + "AND I_IsImported<>'E' "
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No Currency=" + no);

            //	Set Amount
            sql = new StringBuilder("UPDATE I_Payment "
                + "SET ChargeAmt=0 "
                + "WHERE ChargeAmt IS NULL "
                + "AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Charge Amount=" + no);
            //
            sql = new StringBuilder("UPDATE I_Payment "
                + "SET TaxAmt=0 "
                + "WHERE TaxAmt IS NULL "
                + "AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Tax Amount=" + no);
            //
            sql = new StringBuilder("UPDATE I_Payment "
                + "SET WriteOffAmt=0 "
                + "WHERE WriteOffAmt IS NULL "
                + "AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("WriteOff Amount=" + no);
            //
            sql = new StringBuilder("UPDATE I_Payment "
                + "SET DiscountAmt=0 "
                + "WHERE DiscountAmt IS NULL "
                + "AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Discount Amount=" + no);
            //

            //	Set Date
            sql = new StringBuilder("UPDATE I_Payment "
                + "SET DateTrx=Created "
                + "WHERE DateTrx IS NULL "
                + "AND I_isImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Trx Date=" + no);

            sql = new StringBuilder("UPDATE I_Payment "
                + "SET DateAcct=DateTrx "
                + "WHERE DateAcct IS NULL "
                + "AND I_isImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Acct Date=" + no);

            //	Invoice
            sql = new StringBuilder("UPDATE I_Payment i "
                  + "SET VAB_Invoice_ID=(SELECT MAX(VAB_Invoice_ID) FROM VAB_Invoice ii"
                  + " WHERE i.InvoiceDocumentNo=ii.DocumentNo AND i.VAF_Client_ID=ii.VAF_Client_ID) "
                  + "WHERE VAB_Invoice_ID IS NULL AND InvoiceDocumentNo IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Fine("Set Invoice from DocumentNo=" + no);

            //	BPartner
            sql = new StringBuilder("UPDATE I_Payment i "
                  + "SET VAB_BusinessPartner_ID=(SELECT MAX(VAB_BusinessPartner_ID) FROM VAB_BusinessPartner bp"
                  + " WHERE i.BPartnerValue=bp.Value AND i.VAF_Client_ID=bp.VAF_Client_ID) "
                  + "WHERE VAB_BusinessPartner_ID IS NULL AND BPartnerValue IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Fine("Set BP from Value=" + no);

            sql = new StringBuilder("UPDATE I_Payment i "
                  + "SET VAB_BusinessPartner_ID=(SELECT MAX(VAB_BusinessPartner_ID) FROM VAB_Invoice ii"
                  + " WHERE i.VAB_Invoice_ID=ii.VAB_Invoice_ID AND i.VAF_Client_ID=ii.VAF_Client_ID) "
                  + "WHERE VAB_BusinessPartner_ID IS NULL AND VAB_Invoice_ID IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Fine("Set BP from Invoice=" + no);

            sql = new StringBuilder("UPDATE I_Payment "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No BPartner,' "
                + "WHERE VAB_BusinessPartner_ID IS NULL "
                + "AND I_IsImported<>'E' "
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No BPartner=" + no);


            //	Check Payment<->Invoice combination
            sql = new StringBuilder("UPDATE I_Payment "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'Err=Invalid Payment<->Invoice, ' "
                + "WHERE I_Payment_ID IN "
                    + "(SELECT I_Payment_ID "
                    + "FROM I_Payment i"
                    + " INNER JOIN VAB_Payment p ON (i.VAB_Payment_ID=p.VAB_Payment_ID) "
                    + "WHERE i.VAB_Invoice_ID IS NOT NULL "
                    + " AND p.VAB_Invoice_ID IS NOT NULL "
                    + " AND p.VAB_Invoice_ID<>i.VAB_Invoice_ID) ")
                .Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Payment<->Invoice Mismatch=" + no);

            //	Check Payment<->BPartner combination
            sql = new StringBuilder("UPDATE I_Payment "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'Err=Invalid Payment<->BPartner, ' "
                + "WHERE I_Payment_ID IN "
                    + "(SELECT I_Payment_ID "
                    + "FROM I_Payment i"
                    + " INNER JOIN VAB_Payment p ON (i.VAB_Payment_ID=p.VAB_Payment_ID) "
                    + "WHERE i.VAB_BusinessPartner_ID IS NOT NULL "
                    + " AND p.VAB_BusinessPartner_ID IS NOT NULL "
                    + " AND p.VAB_BusinessPartner_ID<>i.VAB_BusinessPartner_ID) ")
                .Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Payment<->BPartner Mismatch=" + no);

            //	Check Invoice<->BPartner combination
            sql = new StringBuilder("UPDATE I_Payment "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'Err=Invalid Invoice<->BPartner, ' "
                + "WHERE I_Payment_ID IN "
                    + "(SELECT I_Payment_ID "
                    + "FROM I_Payment i"
                    + " INNER JOIN VAB_Invoice v ON (i.VAB_Invoice_ID=v.VAB_Invoice_ID) "
                    + "WHERE i.VAB_BusinessPartner_ID IS NOT NULL "
                    + " AND v.VAB_BusinessPartner_ID IS NOT NULL "
                    + " AND v.VAB_BusinessPartner_ID<>i.VAB_BusinessPartner_ID) ")
                .Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Invoice<->BPartner Mismatch=" + no);

            //	Check Invoice.BPartner<->Payment.BPartner combination
            sql = new StringBuilder("UPDATE I_Payment "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'Err=Invalid Invoice.BPartner<->Payment.BPartner, ' "
                + "WHERE I_Payment_ID IN "
                    + "(SELECT I_Payment_ID "
                    + "FROM I_Payment i"
                    + " INNER JOIN VAB_Invoice v ON (i.VAB_Invoice_ID=v.VAB_Invoice_ID)"
                    + " INNER JOIN VAB_Payment p ON (i.VAB_Payment_ID=p.VAB_Payment_ID) "
                    + "WHERE p.VAB_Invoice_ID<>v.VAB_Invoice_ID"
                    + " AND v.VAB_BusinessPartner_ID<>p.VAB_BusinessPartner_ID) ")
                .Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Invoice.BPartner<->Payment.BPartner Mismatch=" + no);

            //	TrxType
            sql = new StringBuilder("UPDATE I_Payment "
                + "SET TrxType='S' "	//	MPayment.TRXTYPE_Sales
                + "WHERE TrxType IS NULL "
                + "AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("TrxType Default=" + no);

            //	TenderType
            sql = new StringBuilder("UPDATE I_Payment "
                + "SET TenderType='K' "	//	MPayment.TENDERTYPE_Check
                + "WHERE TenderType IS NULL "
                + "AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("TenderType Default=" + no);

            //	Document Type
            sql = new StringBuilder("UPDATE I_Payment i "
                  + "SET VAB_DocTypes_ID=(SELECT VAB_DocTypes_ID FROM VAB_DocTypes d WHERE d.Name=i.DocTypeName"
                  + " AND d.DocBaseType IN ('ARR','APP') AND i.VAF_Client_ID=d.VAF_Client_ID) "
                  + "WHERE VAB_DocTypes_ID IS NULL AND DocTypeName IS NOT NULL AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Fine("Set DocType=" + no);
            sql = new StringBuilder("UPDATE I_Payment "
                  + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid DocTypeName, ' "
                  + "WHERE VAB_DocTypes_ID IS NULL AND DocTypeName IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid DocTypeName=" + no);
            sql = new StringBuilder("UPDATE I_Payment "
                  + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No DocType, ' "
                  + "WHERE VAB_DocTypes_ID IS NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No DocType=" + no);

            Commit();

            //Import Bank Statement
            sql = new StringBuilder("SELECT * FROM I_Payment"
                + " WHERE I_IsImported='N'"
                + " ORDER BY VAB_Bank_Acct_ID, CheckNo, DateTrx, R_AuthCode");

            MVABBankAcct account = null;
            IDataReader idr = null;
           // int lineNo = 10;
            int noInsert = 0;
            //int noInsertLine = 0;
            try
            {
                //pstmt = DataBase.prepareStatement(sql.ToString(), get_TrxName());
                //ResultSet rs = pstmt.executeQuery();
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());

                while (idr.Read())
                {
                    X_I_Payment imp = new X_I_Payment(_ctx, idr, Get_TrxName());
                    //	Get the bank account
                    if (account == null || account.GetVAB_Bank_Acct_ID() != imp.GetVAB_Bank_Acct_ID())
                    {
                        account = MVABBankAcct.Get(_ctx, imp.GetVAB_Bank_Acct_ID());
                        log.Info("New Account=" + account.GetAccountNo());
                    }

                    //	New Payment
                    MVABPayment payment = new MVABPayment(_ctx, 0, Get_TrxName());
                    payment.SetVAF_Org_ID(imp.GetVAF_Org_ID());
                    payment.SetDocumentNo(imp.GetDocumentNo());
                    payment.SetPONum(imp.GetPONum());

                    payment.SetTrxType(imp.GetTrxType());
                    payment.SetTenderType(imp.GetTenderType());

                    payment.SetVAB_Bank_Acct_ID(imp.GetVAB_Bank_Acct_ID());
                    payment.SetRoutingNo(imp.GetRoutingNo());
                    payment.SetAccountNo(imp.GetAccountNo());
                    payment.SetCheckNo(imp.GetCheckNo());
                    payment.SetMicr(imp.GetMicr());

                    if (imp.GetCreditCardType() != null)
                        payment.SetCreditCardType(imp.GetCreditCardType());
                    payment.SetCreditCardNumber(imp.GetCreditCardNumber());
                    if (imp.GetCreditCardExpMM() != 0)
                        payment.SetCreditCardExpMM(imp.GetCreditCardExpMM());
                    if (imp.GetCreditCardExpYY() != 0)
                        payment.SetCreditCardExpYY(imp.GetCreditCardExpYY());
                    payment.SetCreditCardVV(imp.GetCreditCardVV());
                    payment.SetSwipe(imp.GetSwipe());

                    payment.SetDateAcct(imp.GetDateTrx());
                    payment.SetDateTrx(imp.GetDateTrx());
                    //	payment.setDescription(imp.getDescription());
                    //
                    payment.SetVAB_BusinessPartner_ID(imp.GetVAB_BusinessPartner_ID());
                    payment.SetVAB_Invoice_ID(imp.GetVAB_Invoice_ID());
                    payment.SetVAB_DocTypes_ID(imp.GetVAB_DocTypes_ID());
                    payment.SetVAB_Currency_ID(imp.GetVAB_Currency_ID());
                    //	payment.setVAB_CurrencyType_ID(imp.getVAB_CurrencyType_ID());
                    payment.SetVAB_Charge_ID(imp.GetVAB_Charge_ID());
                    payment.SetChargeAmt(imp.GetChargeAmt());
                    payment.SetTaxAmt(imp.GetTaxAmt());

                    payment.SetPayAmt(imp.GetPayAmt());
                    payment.SetWriteOffAmt(imp.GetWriteOffAmt());
                    payment.SetDiscountAmt(imp.GetDiscountAmt());
                    payment.SetWriteOffAmt(imp.GetWriteOffAmt());

                    //	Copy statement line reference data
                    payment.SetA_City(imp.GetA_City());
                    payment.SetA_Country(imp.GetA_Country());
                    payment.SetA_EMail(imp.GetA_EMail());
                    payment.SetA_Ident_DL(imp.GetA_Ident_DL());
                    payment.SetA_Ident_SSN(imp.GetA_Ident_SSN());
                    payment.SetA_Name(imp.GetA_Name());
                    payment.SetA_State(imp.GetA_State());
                    payment.SetA_Street(imp.GetA_Street());
                    payment.SetA_Zip(imp.GetA_Zip());
                    payment.SetR_AuthCode(imp.GetR_AuthCode());
                    payment.SetR_Info(imp.GetR_Info());
                    payment.SetR_PnRef(imp.GetR_PnRef());
                    payment.SetR_RespMsg(imp.GetR_RespMsg());
                    payment.SetR_Result(imp.GetR_Result());
                    payment.SetOrig_TrxID(imp.GetOrig_TrxID());
                    payment.SetVoiceAuthCode(imp.GetVoiceAuthCode());

                    //	Save patment
                    if (payment.Save())
                    {
                        imp.SetVAB_Payment_ID(payment.GetVAB_Payment_ID());
                        imp.SetI_IsImported(X_I_Payment.I_ISIMPORTED_Yes);
                        imp.SetProcessed(true);
                        imp.Save();
                        noInsert++;
                    }

                }

                //	Close database connection
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql.ToString(), e);
            }

            //	Set Error to indicator to not imported
            sql = new StringBuilder("UPDATE I_Payment "
                + "SET I_IsImported='N', Updated=SysDate "
                + "WHERE I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, Utility.Util.GetValueOfDecimal(no), "@Errors@");
            //
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsert), "@VAB_Payment_ID@: @Inserted@");
            return "";
        }	//	doIt

    }
}
