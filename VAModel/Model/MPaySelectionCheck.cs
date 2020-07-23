using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.Process;
using System.Data;
using java.io;
using System.Data.SqlClient;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MPaySelectionCheck : X_C_PaySelectionCheck
    {

        /**
         * 	Get Check for Payment
         *	@param ctx context
         *	@param C_Payment_ID id
         *	@param trxName transaction
         *	@return pay selection check for payment or null
         */
        public static MPaySelectionCheck GetOfPayment(Ctx ctx, int C_Payment_ID, Trx trxName)
        {
            MPaySelectionCheck retValue = null;
            String sql = "SELECT * FROM C_PaySelectionCheck WHERE C_Payment_ID=" + C_Payment_ID;
            int count = 0;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    MPaySelectionCheck psc = new MPaySelectionCheck(ctx, dr, trxName);
                    if (retValue == null)
                        retValue = psc;
                    else if (!retValue.IsProcessed() && psc.IsProcessed())
                        retValue = psc;
                    count++;
                }

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            if (count > 1)
            {
                _log.Warning("More then one for C_Payment_ID=" + C_Payment_ID);
            }
            return retValue;
        }

        /**
         * 	Create Check for Payment
         *	@param ctx context
         *	@param C_Payment_ID id
         *	@param trxName transaction
         *	@return pay selection check for payment or null
         */
        public static MPaySelectionCheck CreateForPayment(Ctx ctx, int C_Payment_ID, Trx trxName)
        {
            if (C_Payment_ID == 0)
                return null;
            MPayment payment = new MPayment(ctx, C_Payment_ID, null);
            //	Map Payment Rule <- Tender Type
            String PaymentRule = PAYMENTRULE_Check;
            if (payment.GetTenderType().Equals(X_C_Payment.TENDERTYPE_CreditCard))
                PaymentRule = PAYMENTRULE_CreditCard;
            else if (payment.GetTenderType().Equals(X_C_Payment.TENDERTYPE_DirectDebit))
                PaymentRule = PAYMENTRULE_DirectDebit;
            else if (payment.GetTenderType().Equals(X_C_Payment.TENDERTYPE_DirectDeposit))
                PaymentRule = PAYMENTRULE_DirectDeposit;
            //	else if (payment.GetTenderType().Equals(MPayment.TENDERTYPE_Check))
            //		PaymentRule = MPaySelectionCheck.PAYMENTRULE_Check;

            //	Create new PaySelection
            MPaySelection ps = new MPaySelection(ctx, 0, trxName);
            ps.SetC_BankAccount_ID(payment.GetC_BankAccount_ID());
            ps.SetName(Msg.Translate(ctx, "C_Payment_ID") + ": " + payment.GetDocumentNo());
            ps.SetDescription(payment.GetDescription());
            ps.SetPayDate(payment.GetDateTrx());
            ps.SetTotalAmt(payment.GetPayAmt());
            ps.SetIsApproved(true);
            ps.Save();

            //	Create new PaySelection Line
            MPaySelectionLine psl = null;
            if (payment.GetC_Invoice_ID() != 0)
            {
                psl = new MPaySelectionLine(ps, 10, PaymentRule);
                psl.SetC_Invoice_ID(payment.GetC_Invoice_ID());
                psl.SetIsSOTrx(payment.IsReceipt());
                psl.SetOpenAmt(Decimal.Add(payment.GetPayAmt(), payment.GetDiscountAmt()));
                psl.SetPayAmt(payment.GetPayAmt());
                psl.SetDiscountAmt(payment.GetDiscountAmt());
                psl.SetDifferenceAmt(Env.ZERO);
                psl.Save();
            }

            //	Create new PaySelection Check
            MPaySelectionCheck psc = new MPaySelectionCheck(ps, PaymentRule);
            psc.SetC_BPartner_ID(payment.GetC_BPartner_ID());
            psc.SetC_Payment_ID(payment.GetC_Payment_ID());
            psc.SetIsReceipt(payment.IsReceipt());
            psc.SetPayAmt(payment.GetPayAmt());
            psc.SetDiscountAmt(payment.GetDiscountAmt());
            psc.SetQty(1);
            psc.SetDocumentNo(payment.GetDocumentNo());
            psc.SetProcessed(true);
            psc.Save();

            //	Update optional Line
            if (psl != null)
            {
                psl.SetC_PaySelectionCheck_ID(psc.GetC_PaySelectionCheck_ID());
                psl.SetProcessed(true);
                psl.Save();
            }

            //	Indicate Done
            ps.SetProcessed(true);
            ps.Save();
            return psc;
        }

        /**
         *  Get Checks of Payment Selection
         *
         *  @param C_PaySelection_ID Payment Selection
         *  @param PaymentRule Payment Rule
         *  @param startDocumentNo start document no
         *	@param trxName transaction
         *  @return array of checks
         */
        static public MPaySelectionCheck[] Get(int C_PaySelection_ID,
            String PaymentRule, int startDocumentNo, Trx trxName)
        {
            _log.Fine("C_PaySelection_ID=" + C_PaySelection_ID
                + ", PaymentRule=" + PaymentRule + ", startDocumentNo=" + startDocumentNo);
            List<MPaySelectionCheck> list = new List<MPaySelectionCheck>();

            int docNo = startDocumentNo;
            String sql = "SELECT * FROM C_PaySelectionCheck "
                + "WHERE C_PaySelection_ID=" + C_PaySelection_ID + " AND PaymentRule='"
                + PaymentRule + "'";
            DataTable dt;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    MPaySelectionCheck check = new MPaySelectionCheck(Env.GetContext(), dr, trxName);
                    //	Set new Check Document No - saved in confirmPrint
                    check.SetDocumentNo((docNo++).ToString());
                    list.Add(check);
                }

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            //  convert to Array
            MPaySelectionCheck[] retValue = new MPaySelectionCheck[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        /**
         *  Export to File
         *  @param checks array of checks
         *  @param file file to export checks
         *  @return number of lines
         */
        public static int ExportToFile(MPaySelectionCheck[] checks, File file)
        {
            if (checks == null || checks.Length == 0)
                return 0;
            //  Must be a file
            if (file.isDirectory())
            {
                _log.Log(Level.WARNING, "File is directory - " + file.getAbsolutePath());
                return 0;
            }
            //  delete if exists
            try
            {
                if (file.exists())
                    file.delete();
            }
            catch (Exception e)
            {
                _log.Log(Level.WARNING, "Could not delete - " + file.getAbsolutePath(), e);
            }

            char x = '"';      //  ease
            int noLines = 0;
            StringBuilder line = null;
            try
            {
                FileWriter fw = new FileWriter(file);

                //  write header
                line = new StringBuilder();
                line.Append(x).Append("Value").Append(x).Append(",")
                    .Append(x).Append("Name").Append(x).Append(",")
                    .Append(x).Append("Contact").Append(x).Append(",")
                    .Append(x).Append("Addr1").Append(x).Append(",")
                    .Append(x).Append("Addr2").Append(x).Append(",")
                    .Append(x).Append("City").Append(x).Append(",")
                    .Append(x).Append("State").Append(x).Append(",")
                    .Append(x).Append("ZIP").Append(x).Append(",")
                    .Append(x).Append("Country").Append(x).Append(",")
                    .Append(x).Append("ReferenceNo").Append(x).Append(",")
                    .Append(x).Append("BPRoutingNo").Append(x).Append(",")
                    .Append(x).Append("BPAccountNo").Append(x).Append(",")
                    .Append(x).Append("BPAName").Append(x).Append(",")
                    .Append(x).Append("BPACity").Append(x).Append(",")
                    .Append(x).Append("BPBBAN").Append(x).Append(",")
                    .Append(x).Append("BPIBAN").Append(x).Append(",")
                    .Append(x).Append("BAName").Append(x).Append(",")
                    .Append(x).Append("BARoutingNo").Append(x).Append(",")
                    .Append(x).Append("BASwiftCode").Append(x).Append(",")
                    .Append(x).Append("DocumentNo").Append(x).Append(",")
                    .Append(x).Append("PayDate").Append(x).Append(",")
                    .Append(x).Append("Currency").Append(x).Append(",")
                    .Append(x).Append("PayAmount").Append(x).Append(",")
                    .Append(x).Append("Comment").Append(x)
                    .Append(Env.NL);
                fw.write(line.ToString());
                noLines++;

                //  write lines
                for (int i = 0; i < checks.Length; i++)
                {
                    MPaySelectionCheck mpp = checks[i];
                    if (mpp == null)
                        continue;
                    //  BPartner Info
                    String[] bp = GetBPartnerInfo(mpp.GetC_BPartner_ID());
                    //  TarGet BankAccount Info
                    String[] bpba = GetBPBankAccountInfo(mpp.GetC_BP_BankAccount_ID());

                    //  Comment - list of invoice document no
                    StringBuilder comment = new StringBuilder();
                    MPaySelectionLine[] psls = mpp.GetPaySelectionLines(false);
                    for (int l = 0; l < psls.Length; l++)
                    {
                        if (l > 0)
                            comment.Append(", ");
                        comment.Append(psls[l].GetInvoice().GetDocumentNo());
                    }
                    line = new StringBuilder();
                    line.Append(x).Append(bp[BP_VALUE]).Append(x).Append(",")   // Value
                        .Append(x).Append(bp[BP_NAME]).Append(x).Append(",")    // Name
                        .Append(x).Append(bp[BP_CONTACT]).Append(x).Append(",") // Contact
                        .Append(x).Append(bp[BP_ADDR1]).Append(x).Append(",")   // Addr1
                        .Append(x).Append(bp[BP_ADDR2]).Append(x).Append(",")   // Addr2
                        .Append(x).Append(bp[BP_CITY]).Append(x).Append(",")    // City
                        .Append(x).Append(bp[BP_REGION]).Append(x).Append(",")  // State
                        .Append(x).Append(bp[BP_POSTAL]).Append(x).Append(",")  // ZIP
                        .Append(x).Append(bp[BP_COUNTRY]).Append(x).Append(",") // Country
                        .Append(x).Append(bp[BP_REFNO]).Append(x).Append(",")   // ReferenceNo
                        .Append(x).Append(bpba[BPBA_RoutingNo]).Append(x).Append(",")   // Routing No (as of BPBankAccount
                        .Append(x).Append(bpba[BPBA_AccountNo]).Append(x).Append(",")   // AccountNo
                        .Append(x).Append(bpba[BPBA_AName]).Append(x).Append(",")       // Account Name
                        .Append(x).Append(bpba[BPBA_ACity]).Append(x).Append(",")       // Account City
                        .Append(x).Append(bpba[BPBA_BBAN]).Append(x).Append(",")        // BBAN
                        .Append(x).Append(bpba[BPBA_IBAN]).Append(x).Append(",")        // IBAN
                        .Append(x).Append(bpba[BA_Name]).Append(x).Append(",")          // Bank Name
                        .Append(x).Append(bpba[BA_RoutingNo]).Append(x).Append(",")     // Bank RoutingNo
                        .Append(x).Append(bpba[BA_SwitftCode]).Append(x).Append(",")    // SwiftCode
                        //  Payment Info
                        .Append(x).Append(mpp.GetDocumentNo()).Append(x).Append(",")    // DocumentNo
                        .Append(mpp.GetParent().GetPayDate()).Append(",")               // PayDate
                        .Append(x).Append(MCurrency.GetISO_Code(Env.GetContext(), mpp.GetParent().GetC_Currency_ID())).Append(x).Append(",")    // Currency
                        .Append(mpp.GetPayAmt()).Append(",")                // PayAmount
                        .Append(x).Append(comment.ToString()).Append(x)     // Comment
                        .Append(Env.NL);
                    fw.write(line.ToString());
                    noLines++;
                }   //  write line

                fw.flush();
                fw.close();
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, "", e);
            }

            return noLines;
        }

        /**
         *  Get Customer/Vendor Info.
         *  Based on BP_ static variables
         *  @param C_BPartner_ID BPartner
         *  @return info array
         */
        private static String[] GetBPartnerInfo(int C_BPartner_ID)
        {
            String[] bp = new String[10];

            String sql = "SELECT bp.Value, bp.Name, c.Name AS Contact, "
                + "a.Address1, a.Address2, a.City, r.Name AS Region, a.Postal, "
                + "cc.Name AS Country, bp.ReferenceNo "
                /*//jz use SQL standard outer join
                + "FROM C_BPartner bp, AD_User c, C_BPartner_Location l, C_Location a, C_Region r, C_Country cc "
                + "WHERE bp.C_BPartner_ID=?"        // #1
                + " AND bp.C_BPartner_ID=c.C_BPartner_ID(+)"
                + " AND bp.C_BPartner_ID=l.C_BPartner_ID"
                + " AND l.C_Location_ID=a.C_Location_ID"
                + " AND a.C_Region_ID=r.C_Region_ID(+)"
                + " AND a.C_Country_ID=cc.C_Country_ID "
                */
                + "FROM C_BPartner bp "
                + "LEFT OUTER JOIN AD_User c ON (bp.C_BPartner_ID=c.C_BPartner_ID) "
                + "INNER JOIN C_BPartner_Location l ON (bp.C_BPartner_ID=l.C_BPartner_ID) "
                + "INNER JOIN C_Location a ON (l.C_Location_ID=a.C_Location_ID) "
                + "LEFT OUTER JOIN C_Region r ON (a.C_Region_ID=r.C_Region_ID) "
                + "INNER JOIN C_Country cc ON (a.C_Country_ID=cc.C_Country_ID) "
                + "WHERE bp.C_BPartner_ID= " + C_BPartner_ID          // #1
                + " ORDER BY l.IsBillTo DESC";

            IDataReader idr = null;
            try
            {
                //
                idr = DataBase.DB.ExecuteReader(sql, null, null);

                if (idr.Read())
                {
                    bp[BP_VALUE] = idr.GetString(0);
                    if (bp[BP_VALUE] == null)
                        bp[BP_VALUE] = "";
                    bp[BP_NAME] = idr.GetString(1);
                    if (bp[BP_NAME] == null)
                        bp[BP_NAME] = "";
                    bp[BP_CONTACT] = idr.GetString(2);
                    if (bp[BP_CONTACT] == null)
                        bp[BP_CONTACT] = "";
                    bp[BP_ADDR1] = idr.GetString(3);
                    if (bp[BP_ADDR1] == null)
                        bp[BP_ADDR1] = "";
                    bp[BP_ADDR2] = idr.GetString(4);
                    if (bp[BP_ADDR2] == null)
                        bp[BP_ADDR2] = "";
                    bp[BP_CITY] = idr.GetString(5);
                    if (bp[BP_CITY] == null)
                        bp[BP_CITY] = "";
                    bp[BP_REGION] = idr.GetString(6);
                    if (bp[BP_REGION] == null)
                        bp[BP_REGION] = "";
                    bp[BP_POSTAL] = idr.GetString(7);
                    if (bp[BP_POSTAL] == null)
                        bp[BP_POSTAL] = "";
                    bp[BP_COUNTRY] = idr.GetString(8);
                    if (bp[BP_COUNTRY] == null)
                        bp[BP_COUNTRY] = "";
                    bp[BP_REFNO] = idr.GetString(9);
                    if (bp[BP_REFNO] == null)
                        bp[BP_REFNO] = "";
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }

            return bp;
        }


        /** BankAccount Info Index for RoutingNo       */
        private const int BPBA_RoutingNo = 0;
        /** BankAccount Info Index for AccountNo       */
        private const int BPBA_AccountNo = 1;
        /** BankAccount Info Index for AccountName     */
        private const int BPBA_AName = 2;
        /** BankAccount Info Index for AccountCity     */
        private const int BPBA_ACity = 3;
        /** BankAccount Info Index for BBAN            */
        private const int BPBA_BBAN = 4;
        /** BankAccount Info Index for IBAN            */
        private const int BPBA_IBAN = 5;
        /** BankAccount Info Index for Bank Name       */
        private const int BA_Name = 6;
        /** BankAccount Info Index for Bank RoutingNo  */
        private const int BA_RoutingNo = 7;
        /** BankAccount Info Index for Bank SwiftCode  */
        private const int BA_SwitftCode = 8;

        /**
         *  Get Bank Account Info for tarGet Accpimt.
         *  Based on BP_ static variables
         *  @param C_BPartner_ID BPartner
         *  @return info array
         */
        private static String[] GetBPBankAccountInfo(int C_BP_BankAccount_ID)
        {
            String[] bp = new String[10];

            String sql = "SELECT bpba.RoutingNo, bpba.AccountNo, bpba.A_Name, bpba.A_City, bpba.BBAN, "
                + "bpba.IBAN, ba.Name, ba.RoutingNo, ba.SwiftCode "
                /*//jz use SQL standard outer join
                + "FROM C_BPartner bp, AD_User c, C_BPartner_Location l, C_Location a, C_Region r, C_Country cc "
                + "WHERE bp.C_BPartner_ID=?"        // #1
                + " AND bp.C_BPartner_ID=c.C_BPartner_ID(+)"
                + " AND bp.C_BPartner_ID=l.C_BPartner_ID"
                + " AND l.C_Location_ID=a.C_Location_ID"
                + " AND a.C_Region_ID=r.C_Region_ID(+)"
                + " AND a.C_Country_ID=cc.C_Country_ID "
                */
                + "FROM C_BP_BankAccount bpba "
                + "LEFT OUTER JOIN C_Bank ba ON (bpba.C_Bank_ID = ba.C_Bank_ID) "
                + "WHERE bpba.C_BP_BankAccount_ID=" + C_BP_BankAccount_ID;        // #1

            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                if (idr.Read())
                {
                    bp[BPBA_RoutingNo] = idr.GetString(1);
                    if (bp[BPBA_RoutingNo] == null)
                        bp[BPBA_RoutingNo] = "";
                    bp[BPBA_AccountNo] = idr.GetString(2);
                    if (bp[BPBA_AccountNo] == null)
                        bp[BPBA_AccountNo] = "";
                    bp[BPBA_AName] = idr.GetString(3);
                    if (bp[BPBA_AName] == null)
                        bp[BPBA_AName] = "";
                    bp[BPBA_ACity] = idr.GetString(4);
                    if (bp[BPBA_ACity] == null)
                        bp[BPBA_ACity] = "";
                    bp[BPBA_BBAN] = idr.GetString(5);
                    if (bp[BPBA_BBAN] == null)
                        bp[BPBA_BBAN] = "";
                    bp[BPBA_IBAN] = idr.GetString(6);
                    if (bp[BPBA_IBAN] == null)
                        bp[BPBA_IBAN] = "";
                    bp[BA_Name] = idr.GetString(7);
                    if (bp[BA_Name] == null)
                        bp[BA_Name] = "";
                    bp[BA_RoutingNo] = idr.GetString(8);
                    if (bp[BA_RoutingNo] == null)
                        bp[BA_RoutingNo] = "";
                    bp[BA_SwitftCode] = idr.GetString(9);
                    if (bp[BA_SwitftCode] == null)
                        bp[BA_SwitftCode] = "";
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }

            return bp;
        }

        /// <summary>
        /// Confirm Print. Create Payments the first time
        /// </summary>
        /// <param name="checks">checks</param>
        /// <param name="batch">batch</param>
        /// <returns>last Document number or 0 if nothing printed</returns>
        public static int ConfirmPrint(MPaySelectionCheck[] checks, MPaymentBatch batch)
        {
            int lastDocumentNo = 0;
            int VA009_PaymentMethod_ID = 0;
            MPaySelectionCheck check = null;
            MPayment payment = null;
            MPaySelectionLine[] psls = null;
            string error = "";
            for (int i = 0; i < checks.Length; i++)
            {
                check = checks[i];
                payment = new MPayment(check.GetCtx(), check.GetC_Payment_ID(), null);

                //	Existing Payment
                if (check.GetC_Payment_ID() != 0)
                {
                    //	Update check number
                    if (check.GetPaymentRule().Equals(PAYMENTRULE_Check))
                    {
                        payment.SetCheckNo(check.GetDocumentNo());
                        payment.SetCheckDate(check.GetParent().GetPayDate());           // Set Check date from Payment selection date
                        if (!payment.Save())
                        {
                            error = "Payment not saved: " + payment;
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null)
                            {
                                error += ", " + pp.GetName();
                            }
                            _log.Log(Level.SEVERE, error);
                        }
                    }
                }
                else	//	New Payment
                {
                    if (Env.IsModuleInstalled("VA009_"))
                    {
                        #region IF VA009 (VA Payment management is installed)

                        payment = new MPayment(check.GetCtx(), 0, null);
                        payment.SetAD_Org_ID(check.GetAD_Org_ID());
                        payment.SetC_DocType_ID(false);
                        //
                        if (check.GetPaymentRule().Equals(PAYMENTRULE_Check))
                        {
                            payment.SetBankCheck(check.GetParent().GetC_BankAccount_ID(), false, check.GetDocumentNo());
                            payment.SetCheckDate(check.GetParent().GetPayDate());               // Set Check date from Payment selection date
                        }
                        else if (check.GetPaymentRule().Equals(PAYMENTRULE_CreditCard))
                            payment.SetTenderType(X_C_Payment.TENDERTYPE_CreditCard);
                        else if (check.GetPaymentRule().Equals(PAYMENTRULE_DirectDeposit)
                            || check.GetPaymentRule().Equals(PAYMENTRULE_DirectDebit))
                            payment.SetBankACH(check);
                        else
                        {
                            _log.Log(Level.SEVERE, "Unsupported Payment Rule=" + check.GetPaymentRule());
                            continue;
                        }
                        payment.SetTrxType(X_C_Payment.TRXTYPE_CreditPayment);
                        payment.SetAmount(check.GetParent().GetC_Currency_ID(), check.GetPayAmt());
                        payment.SetDiscountAmt(check.GetDiscountAmt());
                        payment.SetDateTrx(check.GetParent().GetPayDate());
                        payment.SetDateAcct(check.GetParent().GetPayDate());
                        payment.SetC_BPartner_ID(check.GetC_BPartner_ID());

                        //to set VA009_PaymentMethod_ID on payment from Print Export Form
                        VA009_PaymentMethod_ID = Util.GetValueOfInt(check.Get_Value("VA009_PaymentMethod_ID"));
                        if (VA009_PaymentMethod_ID == 0)
                        {
                            VA009_PaymentMethod_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT VA009_PaymentMethod_ID FROM VA009_PaymentMethod WHERE IsActive='Y' 
                                AND VA009_PaymentBaseType='S' AND rownum=1"));
                        }
                        if (VA009_PaymentMethod_ID > 0)
                        {
                            payment.SetVA009_PaymentMethod_ID(VA009_PaymentMethod_ID);
                        }

                        //	Link to Batch
                        if (batch != null)
                        {
                            if (batch.GetC_PaymentBatch_ID() == 0)
                                batch.Save();	//	new
                            payment.SetC_PaymentBatch_ID(batch.GetC_PaymentBatch_ID());
                        }

                        //	Link to Invoice
                        psls = check.GetPaySelectionLines(false);
                        _log.Fine("confirmPrint - " + check + " (#SelectionLines=" + psls.Length + ")");
                        if (check.GetQty() == 1 && psls != null && psls.Length == 1)
                        {
                            MPaySelectionLine psl = psls[0];
                            _log.Fine("Map to Invoice " + psl);
                            //
                            payment.SetC_Invoice_ID(psl.GetC_Invoice_ID());
                            if (psl.Get_ColumnIndex("C_InvoicePaySchedule_ID") > 0)
                            {
                                payment.SetC_InvoicePaySchedule_ID(psl.GetC_InvoicePaySchedule_ID());
                            }
                            payment.SetDiscountAmt(psl.GetDiscountAmt());
                            if (psl.GetDifferenceAmt() > 0)
                                payment.SetWriteOffAmt(psl.GetDifferenceAmt());
                            else
                                payment.SetOverUnderAmt(psl.GetDifferenceAmt());
                        }
                        else
                        {
                            payment.SetDiscountAmt(Env.ZERO);
                        }
                        payment.SetWriteOffAmt(Env.ZERO);

                        if (psls.Length == 1)
                        {
                            MPaySelectionLine psl = psls[0];
                            if (psl.Get_ColumnIndex("C_InvoicePaySchedule_ID") > 0)
                            {
                                payment.SetC_InvoicePaySchedule_ID(psl.GetC_InvoicePaySchedule_ID());
                            }
                            payment.SetC_Invoice_ID(psl.GetC_Invoice_ID());
                            payment.SetDiscountAmt(psl.GetDiscountAmt());
                            if (psl.GetDifferenceAmt() > 0)
                                payment.SetWriteOffAmt(psl.GetDifferenceAmt());
                            else
                                payment.SetOverUnderAmt(psl.GetDifferenceAmt());
                        }

                        if (!payment.Save())
                        {
                            error = "Payment not saved: " + payment;
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null)
                            {
                                error += ", " + pp.GetName();
                            }
                            _log.Log(Level.SEVERE, error);
                        }
                        else
                        {

                            #region Payment Allocation Code
                            //MPaySelectionLine[] psl = check.GetPaySelectionLinesAgainsPaySelection();
                            if (psls.Length > 1)
                            {
                                for (int j = 0; j < psls.Length; j++)
                                {
                                    MPaymentAllocate PayAlocate = new MPaymentAllocate(check.GetCtx(), 0, null);
                                    PayAlocate.SetC_Payment_ID(payment.GetC_Payment_ID());
                                    PayAlocate.SetC_Invoice_ID(psls[j].GetC_Invoice_ID());
                                    PayAlocate.SetC_InvoicePaySchedule_ID(psls[j].GetC_InvoicePaySchedule_ID());
                                    PayAlocate.SetDiscountAmt(psls[j].GetDiscountAmt());
                                    PayAlocate.SetAmount(psls[j].GetPayAmt());
                                    PayAlocate.SetInvoiceAmt(psls[j].GetOpenAmt());
                                    PayAlocate.SetAD_Client_ID(psls[j].GetAD_Client_ID());
                                    PayAlocate.SetAD_Org_ID(psls[j].GetAD_Org_ID());
                                    PayAlocate.SetWriteOffAmt(0);
                                    PayAlocate.SetOverUnderAmt(0);
                                    if (!PayAlocate.Save())
                                    {
                                        //_log.Log(Level.SEVERE, Msg.GetMsg(check.GetCtx(), "VA009_PymentAllocateNotSaved") + payment);
                                        error = Msg.GetMsg(check.GetCtx(), "VA009_PymentAllocateNotSaved") + payment;
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        if (pp != null)
                                        {
                                            error += ", " + pp.GetName();
                                        }
                                        _log.Log(Level.SEVERE, error);
                                        break;
                                    }
                                }
                            }
                            #endregion
                        }
                        //
                        int C_Payment_ID = payment.Get_ID();
                        if (C_Payment_ID < 1)
                        {
                            _log.Log(Level.SEVERE, "Payment not created=" + check);
                        }
                        else
                        {
                            check.SetC_Payment_ID(C_Payment_ID);
                            check.Save();	//	Payment process needs it
                            //	Should start WF
                            payment.ProcessIt(DocActionVariables.ACTION_COMPLETE);
                            if (!payment.Save())
                            {
                                error = "Payment not saved: " + payment;
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null)
                                {
                                    error += ", " + pp.GetName();
                                }
                                _log.Log(Level.SEVERE, error);
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        payment = new MPayment(check.GetCtx(), 0, null);
                        payment.SetAD_Org_ID(check.GetAD_Org_ID());
                        payment.SetC_DocType_ID(false);
                        //
                        if (check.GetPaymentRule().Equals(PAYMENTRULE_Check))
                        {
                            payment.SetBankCheck(check.GetParent().GetC_BankAccount_ID(), false, check.GetDocumentNo());
                            payment.SetCheckDate(check.GetParent().GetPayDate());                   // Set Check date from Payment selection date
                        }
                        else if (check.GetPaymentRule().Equals(PAYMENTRULE_CreditCard))
                            payment.SetTenderType(X_C_Payment.TENDERTYPE_CreditCard);
                        else if (check.GetPaymentRule().Equals(PAYMENTRULE_DirectDeposit)
                            || check.GetPaymentRule().Equals(PAYMENTRULE_DirectDebit))
                            payment.SetBankACH(check);
                        else
                        {
                            _log.Log(Level.SEVERE, "Unsupported Payment Rule=" + check.GetPaymentRule());
                            continue;
                        }
                        payment.SetTrxType(X_C_Payment.TRXTYPE_CreditPayment);
                        payment.SetAmount(check.GetParent().GetC_Currency_ID(), check.GetPayAmt());
                        payment.SetDiscountAmt(check.GetDiscountAmt());
                        payment.SetDateTrx(check.GetParent().GetPayDate());
                        payment.SetC_BPartner_ID(check.GetC_BPartner_ID());
                        //	Link to Batch
                        if (batch != null)
                        {
                            if (batch.GetC_PaymentBatch_ID() == 0)
                                batch.Save();	//	new
                            payment.SetC_PaymentBatch_ID(batch.GetC_PaymentBatch_ID());
                        }
                        //	Link to Invoice
                        psls = check.GetPaySelectionLines(false);
                        _log.Fine("confirmPrint - " + check + " (#SelectionLines=" + psls.Length + ")");
                        if (check.GetQty() == 1 && psls != null && psls.Length == 1)
                        {
                            MPaySelectionLine psl = psls[0];
                            _log.Fine("Map to Invoice " + psl);
                            //
                            payment.SetC_Invoice_ID(psl.GetC_Invoice_ID());
                            payment.SetDiscountAmt(psl.GetDiscountAmt());
                            payment.SetWriteOffAmt(psl.GetDifferenceAmt());
                            Decimal overUnder = Decimal.Subtract(Decimal.Subtract(Decimal.Subtract(psl.GetOpenAmt(), psl.GetPayAmt()), psl.GetDiscountAmt()), psl.GetDifferenceAmt());
                            //Decimal overUnder = psl.GetOpenAmt().subtract(psl.GetPayAmt())                                                                                                                                                                                                                                                                             
                            //	.subtract(psl.GetDiscountAmt()).subtract(psl.GetDifferenceAmt());
                            payment.SetOverUnderAmt(overUnder);
                        }
                        else
                            payment.SetDiscountAmt(Env.ZERO);
                        payment.SetWriteOffAmt(Env.ZERO);
                        if (!payment.Save())
                        {
                            error = "Payment not saved: " + payment;
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null)
                            {
                                error += ", " + pp.GetName();
                            }
                            _log.Log(Level.SEVERE, error);
                        }
                        else
                        {
                            #region Payment Allocation Code
                            //MPaySelectionLine[] psl = check.GetPaySelectionLinesAgainsPaySelection();
                            if (psls.Length > 1)
                            {
                                for (int j = 0; j < psls.Length; j++)
                                {
                                    MPaymentAllocate PayAlocate = new MPaymentAllocate(check.GetCtx(), 0, null);
                                    PayAlocate.SetC_Payment_ID(payment.GetC_Payment_ID());
                                    PayAlocate.SetC_Invoice_ID(psls[j].GetC_Invoice_ID());
                                    PayAlocate.SetC_InvoicePaySchedule_ID(psls[j].GetC_InvoicePaySchedule_ID());
                                    PayAlocate.SetDiscountAmt(psls[j].GetDiscountAmt());
                                    PayAlocate.SetAmount(psls[j].GetPayAmt());
                                    PayAlocate.SetInvoiceAmt(psls[j].GetOpenAmt());
                                    PayAlocate.SetAD_Client_ID(psls[j].GetAD_Client_ID());
                                    PayAlocate.SetAD_Org_ID(psls[j].GetAD_Org_ID());
                                    PayAlocate.SetWriteOffAmt(0);
                                    PayAlocate.SetOverUnderAmt(0);
                                    if (!PayAlocate.Save())
                                    {
                                        //_log.Log(Level.SEVERE, Msg.GetMsg(check.GetCtx(), "VA009_PymentAllocateNotSaved") + payment);
                                        error = Msg.GetMsg(check.GetCtx(), "VA009_PymentAllocateNotSaved") + payment;
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        if (pp != null)
                                        {
                                            error += ", " + pp.GetName();
                                        }
                                        _log.Log(Level.SEVERE, error);
                                        break;
                                    }
                                }
                            }
                            #endregion
                        }
                        //
                        int C_Payment_ID = payment.Get_ID();
                        if (C_Payment_ID < 1)
                        {
                            _log.Log(Level.SEVERE, "Payment not created=" + check);
                        }
                        else
                        {
                            check.SetC_Payment_ID(C_Payment_ID);
                            check.Save();	//	Payment process needs it
                            //	Should start WF
                            payment.ProcessIt(DocActionVariables.ACTION_COMPLETE);
                            if (!payment.Save())
                            {
                                error = "Payment not saved: " + payment;
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null)
                                {
                                    error += ", " + pp.GetName();
                                }
                                _log.Log(Level.SEVERE, error);
                            }
                        }
                    }
                }//	new Payment

                //	Get Check Document No
                try
                {
                    int no = int.Parse(check.GetDocumentNo());
                    if (lastDocumentNo < no)
                        lastDocumentNo = no;
                }
                catch (FormatException ex)
                {
                    _log.Log(Level.SEVERE, "DocumentNo=" + check.GetDocumentNo(), ex);
                }
                check.SetIsPrinted(true);
                check.SetProcessed(true);
                if (!check.Save())
                {
                    _log.Log(Level.SEVERE, "Check not saved: " + check);
                }
            }	//	all checks

            _log.Fine("Last Document No = " + lastDocumentNo);
            return lastDocumentNo;
        }

        /** Logger								*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MPaySelectionCheck).FullName);
        //static private CLogger	s_log = CLogger.GetCLogger (MPaySelectionCheck.class);

        /** BPartner Info Index for Value       */
        private const int BP_VALUE = 0;
        /** BPartner Info Index for Name        */
        private const int BP_NAME = 1;
        /** BPartner Info Index for Contact Name    */
        private const int BP_CONTACT = 2;
        /** BPartner Info Index for Address 1   */
        private const int BP_ADDR1 = 3;
        /** BPartner Info Index for Address 2   */
        private const int BP_ADDR2 = 4;
        /** BPartner Info Index for City        */
        private const int BP_CITY = 5;
        /** BPartner Info Index for Region      */
        private const int BP_REGION = 6;
        /** BPartner Info Index for Postal Code */
        private const int BP_POSTAL = 7;
        /** BPartner Info Index for Country     */
        private const int BP_COUNTRY = 8;
        /** BPartner Info Index for Reference No    */
        private const int BP_REFNO = 9;


        /**
         *	Constructor
         *  @param ctx context
         *  @param C_PaySelectionCheck_ID C_PaySelectionCheck_ID
         *	@param trxName transaction
         */
        public MPaySelectionCheck(Ctx ctx, int C_PaySelectionCheck_ID, Trx trxName) :
            base(ctx, C_PaySelectionCheck_ID, trxName)
        {
            if (C_PaySelectionCheck_ID == 0)
            {
                //	SetC_PaySelection_ID (0);
                //	SetC_BPartner_ID (0);
                //	SetPaymentRule (null);
                SetPayAmt(Env.ZERO);
                SetDiscountAmt(Env.ZERO);
                SetIsPrinted(false);
                SetIsReceipt(false);
                SetQty(0);
            }
        }

        /**
         *	Load Constructor
         *  @param ctx context
         *  @param idr result Set
         *	@param trxName transaction
         */
        public MPaySelectionCheck(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
        }

        /**
         * 	Create from Line
         *	@param line payment selection
         *	@param PaymentRule payment rule
         */
        public MPaySelectionCheck(MPaySelectionLine line, String PaymentRule)
            : this(line.GetCtx(), 0, line.Get_TrxName())
        {
            SetClientOrg(line);
            SetC_PaySelection_ID(line.GetC_PaySelection_ID());
            int C_BPartner_ID = line.GetInvoice().GetC_BPartner_ID();
            SetC_BPartner_ID(C_BPartner_ID);
            //
            if (X_C_Order.PAYMENTRULE_DirectDebit.Equals(PaymentRule))
            {
                MBPBankAccount[] bas = MBPBankAccount.GetOfBPartner(line.GetCtx(), C_BPartner_ID);
                for (int i = 0; i < bas.Length; i++)
                {
                    MBPBankAccount account = bas[i];
                    if (account.IsDirectDebit())
                    {
                        SetC_BP_BankAccount_ID(account.GetC_BP_BankAccount_ID());
                        break;
                    }
                }
            }
            else if (X_C_Order.PAYMENTRULE_DirectDeposit.Equals(PaymentRule))
            {
                MBPBankAccount[] bas = MBPBankAccount.GetOfBPartner(line.GetCtx(), C_BPartner_ID);
                for (int i = 0; i < bas.Length; i++)
                {
                    MBPBankAccount account = bas[i];
                    if (account.IsDirectDeposit())
                    {
                        SetC_BP_BankAccount_ID(account.GetC_BP_BankAccount_ID());
                        break;
                    }
                }
            }
            SetPaymentRule(PaymentRule);
            //
            SetIsReceipt(line.IsSOTrx());
            SetPayAmt(line.GetPayAmt());
            SetDiscountAmt(line.GetDiscountAmt());
            SetQty(1);
        }

        /**
         * 	Create from Pay Selection
         *	@param ps payment selection
         *	@param PaymentRule payment rule
         */
        public MPaySelectionCheck(MPaySelection ps, String PaymentRule)
            : this(ps.GetCtx(), 0, ps.Get_TrxName())
        {
            SetClientOrg(ps);
            SetC_PaySelection_ID(ps.GetC_PaySelection_ID());
            SetPaymentRule(PaymentRule);
        }


        /**	Parent					*/
        private MPaySelection _parent = null;
        /**	Payment Selection lines of this check	*/
        private MPaySelectionLine[] _lines = null;


        /**
         * 	Add Payment Selection Line
         *	@param line line
         */
        public void AddLine(MPaySelectionLine line)
        {
            if (GetC_BPartner_ID() != line.GetInvoice().GetC_BPartner_ID())
                throw new ArgumentException("Line for fifferent BPartner");
            //
            if (IsReceipt() == line.IsSOTrx())
            {
                SetPayAmt(Decimal.Add(GetPayAmt(), line.GetPayAmt()));
                SetDiscountAmt(Decimal.Add(GetDiscountAmt(), line.GetDiscountAmt()));
            }
            else
            {
                SetPayAmt(Decimal.Subtract(GetPayAmt(), line.GetPayAmt()));
                SetDiscountAmt(Decimal.Subtract(GetDiscountAmt(), line.GetDiscountAmt()));
            }
            SetQty(GetQty() + 1);
        }

        /**
         * 	Get Parent
         *	@return parent
         */
        public MPaySelection GetParent()
        {
            if (_parent == null)
                _parent = new MPaySelection(GetCtx(), GetC_PaySelection_ID(), Get_TrxName());
            return _parent;
        }

        /**
         * 	Is this a valid Prepared Payment
         *	@return true if valid
         */
        public Boolean IsValid()
        {
            if (GetC_BP_BankAccount_ID() != 0)
                return true;
            return !IsDirect();
        }

        /**
         * 	Is this a direct Debit or Deposit
         *	@return true if direct
         */
        public Boolean IsDirect()
        {
            return (X_C_Order.PAYMENTRULE_DirectDeposit.Equals(GetPaymentRule())
                || X_C_Order.PAYMENTRULE_DirectDebit.Equals(GetPaymentRule()));
        }

        /**
         * 	String Representation
         * 	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MPaymentCheck[");
            sb.Append(Get_ID()).Append("-").Append(GetDocumentNo())
                .Append("-").Append(GetPayAmt())
                .Append(",PaymetRule=").Append(GetPaymentRule())
                .Append(",Qty=").Append(GetQty())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Get Payment Selection Lines of this check
         *	@param requery requery
         * 	@return array of peyment selection lines
         */
        public MPaySelectionLine[] GetPaySelectionLines(Boolean requery)
        {
            if (_lines != null && !requery)
                return _lines;
            List<MPaySelectionLine> list = new List<MPaySelectionLine>();
            String sql = "SELECT * FROM C_PaySelectionLine WHERE C_PaySelectionCheck_ID=" + GetC_PaySelectionCheck_ID() + " ORDER BY Line";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MPaySelectionLine(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            //
            _lines = new MPaySelectionLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }


        //public MPaySelectionLine[] GetPaySelectionLinesAgainsPaySelection()
        //{
        //    List<MPaySelectionLine> list = new List<MPaySelectionLine>();
        //    String sql = "SELECT * FROM C_PaySelectionLine WHERE C_PaySelection_ID=" + GetC_PaySelection_ID() + " ORDER BY Line";
        //    DataTable dt = null;
        //    IDataReader idr = null;
        //    try
        //    {
        //        idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
        //        dt = new DataTable();
        //        dt.Load(idr);
        //        idr.Close();

        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            list.Add(new MPaySelectionLine(GetCtx(), dr, Get_TrxName()));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        if (idr != null)
        //        {
        //            idr.Close();
        //        }
        //        log.Log(Level.SEVERE, sql, e);
        //    }
        //    finally
        //    {
        //        if (idr != null)
        //        {
        //            idr.Close();
        //        }
        //        dt = null;
        //    }
        //    //
        //    _lines = new MPaySelectionLine[list.Count];
        //    _lines = list.ToArray();
        //    return _lines;
        //}

    }
}
