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
        private int _AD_Org_ID = 0;
        /** Default Bank Account			*/
        private int _C_BankAccount_ID = 0;
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
                if (name.Equals("AD_Org_ID"))
                    _AD_Org_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("C_BankAccount_ID"))
                    _C_BankAccount_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
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
            log.Info("C_BankAccount_ID" + _C_BankAccount_ID);
            MBankAccount ba = MBankAccount.Get(GetCtx(), _C_BankAccount_ID);
            if (_C_BankAccount_ID == 0 || ba.Get_ID() != _C_BankAccount_ID)
                throw new Exception("@NotFound@ @C_BankAccount_ID@ - " + _C_BankAccount_ID);
            if (_AD_Org_ID != ba.GetAD_Org_ID() && ba.GetAD_Org_ID() != 0)
                _AD_Org_ID = ba.GetAD_Org_ID();
            log.Info("AD_Org_ID=" + _AD_Org_ID);

            StringBuilder sql = null;
            int no = 0;
            String clientCheck = " AND AD_Client_ID=" + ba.GetAD_Client_ID();

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
                  + "SET AD_Client_ID = COALESCE (AD_Client_ID,").Append(ba.GetAD_Client_ID()).Append("),"
                  + " AD_Org_ID = COALESCE (AD_Org_ID,").Append(_AD_Org_ID).Append("),");
            sql.Append(" IsActive = COALESCE (IsActive, 'Y'),"
                  + " Created = COALESCE (Created, SysDate),"
                  + " CreatedBy = COALESCE (CreatedBy, 0),"
                  + " Updated = COALESCE (Updated, SysDate),"
                  + " UpdatedBy = COALESCE (UpdatedBy, 0),"
                  + " I_ErrorMsg = NULL,"
                  + " I_IsImported = 'N' "
                  + "WHERE I_IsImported<>'Y' OR I_IsImported IS NULL OR AD_Client_ID IS NULL OR AD_Org_ID IS NULL OR AD_Client_ID=0 OR AD_Org_ID=0");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("Reset=" + no);

            String ts = DataBase.DB.IsPostgreSQL() ? "COALESCE(I_ErrorMsg,'')" : "I_ErrorMsg";  //java bug, it could not be used directly
            sql = new StringBuilder("UPDATE I_Payment o "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Org, '"
                + "WHERE (AD_Org_ID IS NULL OR AD_Org_ID=0"
                + " OR EXISTS (SELECT * FROM AD_Org oo WHERE o.AD_Org_ID=oo.AD_Org_ID AND (oo.IsSummary='Y' OR oo.IsActive='N')))"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Org=" + no);

            //	Set Bank Account
            sql = new StringBuilder("UPDATE I_Payment i "
                + "SET C_BankAccount_ID="
                + "( "
                + " SELECT C_BankAccount_ID "
                + " FROM C_BankAccount a, C_Bank b "
                + " WHERE b.IsOwnBank='Y' "
                + " AND a.AD_Client_ID=i.AD_Client_ID "
                + " AND a.C_Bank_ID=b.C_Bank_ID "
                + " AND a.AccountNo=i.BankAccountNo "
                + " AND b.RoutingNo=i.RoutingNo "
                + " OR b.SwiftCode=i.RoutingNo "
                + ") "
                + "WHERE i.C_BankAccount_ID IS NULL "
                + "AND i.I_IsImported<>'Y' "
                + "OR i.I_IsImported IS NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Bank Account (With Routing No)=" + no);
            //
            sql = new StringBuilder("UPDATE I_Payment i "
                + "SET C_BankAccount_ID="
                + "( "
                + " SELECT C_BankAccount_ID "
                + " FROM C_BankAccount a, C_Bank b "
                + " WHERE b.IsOwnBank='Y' "
                + " AND a.C_Bank_ID=b.C_Bank_ID "
                + " AND a.AccountNo=i.BankAccountNo "
                + " AND a.AD_Client_ID=i.AD_Client_ID "
                + ") "
                + "WHERE i.C_BankAccount_ID IS NULL "
                + "AND i.I_isImported<>'Y' "
                + "OR i.I_isImported IS NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Bank Account (Without Routing No)=" + no);
            //
            sql = new StringBuilder("UPDATE I_Payment i "
                + "SET C_BankAccount_ID=(SELECT C_BankAccount_ID FROM C_BankAccount a WHERE a.C_BankAccount_ID=").Append(_C_BankAccount_ID);
            sql.Append(" and a.AD_Client_ID=i.AD_Client_ID) "
                + "WHERE i.C_BankAccount_ID IS NULL "
                + "AND i.BankAccountNo IS NULL "
                + "AND i.I_isImported<>'Y' "
                + "OR i.I_isImported IS NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Bank Account=" + no);
            //	
            sql = new StringBuilder("UPDATE I_Payment "
                + "SET I_isImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Bank Account, ' "
                + "WHERE C_BankAccount_ID IS NULL "
                + "AND I_isImported<>'Y' "
                + "OR I_isImported IS NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Bank Account=" + no);

            //	Set Currency
            sql = new StringBuilder("UPDATE I_Payment i "
                + "SET C_Currency_ID=(SELECT C_Currency_ID FROM C_Currency c"
                + " WHERE i.ISO_Code=c.ISO_Code AND c.AD_Client_ID IN (0,i.AD_Client_ID)) "
                + "WHERE C_Currency_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Set Currency=" + no);
            //
            sql = new StringBuilder("UPDATE I_Payment i "
                //jz	+ "SET i.C_Currency_ID=(SELECT C_Currency_ID FROM C_BankAccount WHERE C_BankAccount_ID=i.C_BankAccount_ID) "
                    + "SET C_Currency_ID=(SELECT C_Currency_ID FROM C_BankAccount WHERE C_BankAccount_ID=i.C_BankAccount_ID) "
                + "WHERE i.C_Currency_ID IS NULL "
                + "AND i.ISO_Code IS NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Set Currency=" + no);
            //
            sql = new StringBuilder("UPDATE I_Payment "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No Currency,' "
                + "WHERE C_Currency_ID IS NULL "
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
                  + "SET C_Invoice_ID=(SELECT MAX(C_Invoice_ID) FROM C_Invoice ii"
                  + " WHERE i.InvoiceDocumentNo=ii.DocumentNo AND i.AD_Client_ID=ii.AD_Client_ID) "
                  + "WHERE C_Invoice_ID IS NULL AND InvoiceDocumentNo IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Fine("Set Invoice from DocumentNo=" + no);

            //	BPartner
            sql = new StringBuilder("UPDATE I_Payment i "
                  + "SET C_BPartner_ID=(SELECT MAX(C_BPartner_ID) FROM C_BPartner bp"
                  + " WHERE i.BPartnerValue=bp.Value AND i.AD_Client_ID=bp.AD_Client_ID) "
                  + "WHERE C_BPartner_ID IS NULL AND BPartnerValue IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Fine("Set BP from Value=" + no);

            sql = new StringBuilder("UPDATE I_Payment i "
                  + "SET C_BPartner_ID=(SELECT MAX(C_BPartner_ID) FROM C_Invoice ii"
                  + " WHERE i.C_Invoice_ID=ii.C_Invoice_ID AND i.AD_Client_ID=ii.AD_Client_ID) "
                  + "WHERE C_BPartner_ID IS NULL AND C_Invoice_ID IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Fine("Set BP from Invoice=" + no);

            sql = new StringBuilder("UPDATE I_Payment "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No BPartner,' "
                + "WHERE C_BPartner_ID IS NULL "
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
                    + " INNER JOIN C_Payment p ON (i.C_Payment_ID=p.C_Payment_ID) "
                    + "WHERE i.C_Invoice_ID IS NOT NULL "
                    + " AND p.C_Invoice_ID IS NOT NULL "
                    + " AND p.C_Invoice_ID<>i.C_Invoice_ID) ")
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
                    + " INNER JOIN C_Payment p ON (i.C_Payment_ID=p.C_Payment_ID) "
                    + "WHERE i.C_BPartner_ID IS NOT NULL "
                    + " AND p.C_BPartner_ID IS NOT NULL "
                    + " AND p.C_BPartner_ID<>i.C_BPartner_ID) ")
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
                    + " INNER JOIN C_Invoice v ON (i.C_Invoice_ID=v.C_Invoice_ID) "
                    + "WHERE i.C_BPartner_ID IS NOT NULL "
                    + " AND v.C_BPartner_ID IS NOT NULL "
                    + " AND v.C_BPartner_ID<>i.C_BPartner_ID) ")
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
                    + " INNER JOIN C_Invoice v ON (i.C_Invoice_ID=v.C_Invoice_ID)"
                    + " INNER JOIN C_Payment p ON (i.C_Payment_ID=p.C_Payment_ID) "
                    + "WHERE p.C_Invoice_ID<>v.C_Invoice_ID"
                    + " AND v.C_BPartner_ID<>p.C_BPartner_ID) ")
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
                  + "SET C_DocType_ID=(SELECT C_DocType_ID FROM C_DocType d WHERE d.Name=i.DocTypeName"
                  + " AND d.DocBaseType IN ('ARR','APP') AND i.AD_Client_ID=d.AD_Client_ID) "
                  + "WHERE C_DocType_ID IS NULL AND DocTypeName IS NOT NULL AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Fine("Set DocType=" + no);
            sql = new StringBuilder("UPDATE I_Payment "
                  + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid DocTypeName, ' "
                  + "WHERE C_DocType_ID IS NULL AND DocTypeName IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid DocTypeName=" + no);
            sql = new StringBuilder("UPDATE I_Payment "
                  + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No DocType, ' "
                  + "WHERE C_DocType_ID IS NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No DocType=" + no);

            Commit();

            //Import Bank Statement
            sql = new StringBuilder("SELECT * FROM I_Payment"
                + " WHERE I_IsImported='N'"
                + " ORDER BY C_BankAccount_ID, CheckNo, DateTrx, R_AuthCode");

            MBankAccount account = null;
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
                    if (account == null || account.GetC_BankAccount_ID() != imp.GetC_BankAccount_ID())
                    {
                        account = MBankAccount.Get(_ctx, imp.GetC_BankAccount_ID());
                        log.Info("New Account=" + account.GetAccountNo());
                    }

                    //	New Payment
                    MPayment payment = new MPayment(_ctx, 0, Get_TrxName());
                    payment.SetAD_Org_ID(imp.GetAD_Org_ID());
                    payment.SetDocumentNo(imp.GetDocumentNo());
                    payment.SetPONum(imp.GetPONum());

                    payment.SetTrxType(imp.GetTrxType());
                    payment.SetTenderType(imp.GetTenderType());

                    payment.SetC_BankAccount_ID(imp.GetC_BankAccount_ID());
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
                    payment.SetC_BPartner_ID(imp.GetC_BPartner_ID());
                    payment.SetC_Invoice_ID(imp.GetC_Invoice_ID());
                    payment.SetC_DocType_ID(imp.GetC_DocType_ID());
                    payment.SetC_Currency_ID(imp.GetC_Currency_ID());
                    //	payment.setC_ConversionType_ID(imp.getC_ConversionType_ID());
                    payment.SetC_Charge_ID(imp.GetC_Charge_ID());
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
                        imp.SetC_Payment_ID(payment.GetC_Payment_ID());
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
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsert), "@C_Payment_ID@: @Inserted@");
            return "";
        }	//	doIt

    }
}
