/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ImportBankStatement
 * Purpose        : Import Bank Statement from I_BankStatement
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
    public class ImportBankStatement : ProcessEngine.SvrProcess
    {
        /**	Client to be imported to		*/
        private int _AD_Client_ID = 0;
        /**	Organization to be imported to	*/
        private int _AD_Org_ID = 0;
        /** Default Bank Account			*/
        private int _C_BankAccount_ID = 0;
        /**	Delete old Imported				*/
        private bool _deleteOldImported = false;

        /** Properties						*/
        private Ctx _ctx;

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("AD_Client_ID"))
                    _AD_Client_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("AD_Org_ID"))
                    _AD_Org_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("C_BankAccount_ID"))
                    _C_BankAccount_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("DeleteOldImported"))
                    _deleteOldImported = "Y".Equals(para[i].GetParameter());
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
            _ctx = GetCtx();
        }	//	prepare


        /// <summary>
        /// perform process
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            log.Info("AD_Org_ID=" + _AD_Org_ID + ", C_BankAccount_ID" + _C_BankAccount_ID);
            StringBuilder sql = null;
            int no = 0;
            String clientCheck = " AND AD_Client_ID=" + _AD_Client_ID;

            //	****	Prepare	****

            //	Delete Old Imported
            if (_deleteOldImported)
            {
                sql = new StringBuilder("DELETE FROM I_BankStatement "
                      + "WHERE I_IsImported='Y'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Delete Old Impored =" + no);
            }

            //	Set Client, Org, IsActive, Created/Updated
            sql = new StringBuilder("UPDATE I_BankStatement "
                  + "SET AD_Client_ID = COALESCE (AD_Client_ID,").Append(_AD_Client_ID).Append("),"
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
            sql = new StringBuilder("UPDATE I_BankStatement o "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Org, '"
                + "WHERE (AD_Org_ID IS NULL OR AD_Org_ID=0"
                + " OR EXISTS (SELECT * FROM AD_Org oo WHERE o.AD_Org_ID=oo.AD_Org_ID AND (oo.IsSummary='Y' OR oo.IsActive='N')))"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Org=" + no);

            //	Set Bank Account
            sql = new StringBuilder("UPDATE I_BankStatement i "
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
            sql = new StringBuilder("UPDATE I_BankStatement i "
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
            sql = new StringBuilder("UPDATE I_BankStatement i "
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
            sql = new StringBuilder("UPDATE I_BankStatement "
                + "SET I_isImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Bank Account, ' "
                + "WHERE C_BankAccount_ID IS NULL "
                + "AND I_isImported<>'Y' "
                + "OR I_isImported IS NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Bank Account=" + no);

            //	Set Currency
            sql = new StringBuilder("UPDATE I_BankStatement i "
                + "SET C_Currency_ID=(SELECT C_Currency_ID FROM C_Currency c"
                + " WHERE i.ISO_Code=c.ISO_Code AND c.AD_Client_ID IN (0,i.AD_Client_ID)) "
                + "WHERE C_Currency_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Set Currency=" + no);
            //
            sql = new StringBuilder("UPDATE I_BankStatement i "
                //jz	+ "SET i.C_Currency_ID=(SELECT C_Currency_ID FROM C_BankAccount WHERE C_BankAccount_ID=i.C_BankAccount_ID) "
                    + "SET C_Currency_ID=(SELECT C_Currency_ID FROM C_BankAccount WHERE C_BankAccount_ID=i.C_BankAccount_ID) "
                + "WHERE i.C_Currency_ID IS NULL "
                + "AND i.ISO_Code IS NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Set Currency=" + no);
            //
            sql = new StringBuilder("UPDATE I_BankStatement "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Currency,' "
                + "WHERE C_Currency_ID IS NULL "
                + "AND I_IsImported<>'E' "
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Currency=" + no);


            //	Set Amount
            sql = new StringBuilder("UPDATE I_BankStatement "
               + "SET ChargeAmt=0 "
               + "WHERE ChargeAmt IS NULL "
               + "AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Charge Amount=" + no);
            //
            sql = new StringBuilder("UPDATE I_BankStatement "
               + "SET InterestAmt=0 "
               + "WHERE InterestAmt IS NULL "
               + "AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Interest Amount=" + no);
            //
            sql = new StringBuilder("UPDATE I_BankStatement "
               + "SET TrxAmt=StmtAmt - InterestAmt - ChargeAmt "
               + "WHERE TrxAmt IS NULL "
               + "AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Transaction Amount=" + no);
            //
            sql = new StringBuilder("UPDATE I_BankStatement "
                + "SET I_isImported='E', I_ErrorMsg=" + ts + "||'Err=Invalid Amount, ' "
                + "WHERE TrxAmt + ChargeAmt + InterestAmt <> StmtAmt "
                + "AND I_isImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Invaid Amount=" + no);

            //	Set Valuta Date
            sql = new StringBuilder("UPDATE I_BankStatement "
                + "SET ValutaDate=StatementLineDate "
                + "WHERE ValutaDate IS NULL "
                + "AND I_isImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Valuta Date=" + no);

            //	Check Payment<->Invoice combination
            sql = new StringBuilder("UPDATE I_BankStatement "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'Err=Invalid Payment<->Invoice, ' "
                + "WHERE I_BankStatement_ID IN "
                    + "(SELECT I_BankStatement_ID "
                    + "FROM I_BankStatement i"
                    + " INNER JOIN C_Payment p ON (i.C_Payment_ID=p.C_Payment_ID) "
                    + "WHERE i.C_Invoice_ID IS NOT NULL "
                    + " AND p.C_Invoice_ID IS NOT NULL "
                    + " AND p.C_Invoice_ID<>i.C_Invoice_ID) ")
                .Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Payment<->Invoice Mismatch=" + no);

            //	Check Payment<->BPartner combination
            sql = new StringBuilder("UPDATE I_BankStatement "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'Err=Invalid Payment<->BPartner, ' "
                + "WHERE I_BankStatement_ID IN "
                    + "(SELECT I_BankStatement_ID "
                    + "FROM I_BankStatement i"
                    + " INNER JOIN C_Payment p ON (i.C_Payment_ID=p.C_Payment_ID) "
                    + "WHERE i.C_BPartner_ID IS NOT NULL "
                    + " AND p.C_BPartner_ID IS NOT NULL "
                    + " AND p.C_BPartner_ID<>i.C_BPartner_ID) ")
                .Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Payment<->BPartner Mismatch=" + no);

            //	Check Invoice<->BPartner combination
            sql = new StringBuilder("UPDATE I_BankStatement "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'Err=Invalid Invoice<->BPartner, ' "
                + "WHERE I_BankStatement_ID IN "
                    + "(SELECT I_BankStatement_ID "
                    + "FROM I_BankStatement i"
                    + " INNER JOIN C_Invoice v ON (i.C_Invoice_ID=v.C_Invoice_ID) "
                    + "WHERE i.C_BPartner_ID IS NOT NULL "
                    + " AND v.C_BPartner_ID IS NOT NULL "
                    + " AND v.C_BPartner_ID<>i.C_BPartner_ID) ")
                .Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Invoice<->BPartner Mismatch=" + no);

            //	Check Invoice.BPartner<->Payment.BPartner combination
            sql = new StringBuilder("UPDATE I_BankStatement "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'Err=Invalid Invoice.BPartner<->Payment.BPartner, ' "
                + "WHERE I_BankStatement_ID IN "
                    + "(SELECT I_BankStatement_ID "
                    + "FROM I_BankStatement i"
                    + " INNER JOIN C_Invoice v ON (i.C_Invoice_ID=v.C_Invoice_ID)"
                    + " INNER JOIN C_Payment p ON (i.C_Payment_ID=p.C_Payment_ID) "
                    + "WHERE p.C_Invoice_ID<>v.C_Invoice_ID"
                    + " AND v.C_BPartner_ID<>p.C_BPartner_ID) ")
                .Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Info("Invoice.BPartner<->Payment.BPartner Mismatch=" + no);

            //	Detect Duplicates
            sql = new StringBuilder("SELECT i.I_BankStatement_ID, l.C_BankStatementLine_ID, i.EftTrxID "
               + "FROM I_BankStatement i, C_BankStatement s, C_BankStatementLine l "
               + "WHERE i.I_isImported='N' "
               + "AND s.C_BankStatement_ID=l.C_BankStatement_ID "
               + "AND i.EftTrxID IS NOT NULL AND "
                //	Concatinate EFT Info
               + "(l.EftTrxID||l.EftAmt||l.EftStatementLineDate||l.EftValutaDate||l.EftTrxType||l.EftCurrency||l.EftReference||s.EftStatementReference "
               + "||l.EftCheckNo||l.EftMemo||l.EftPayee||l.EftPayeeAccount) "
               + "= "
               + "(i.EftTrxID||i.EftAmt||i.EftStatementLineDate||i.EftValutaDate||i.EftTrxType||i.EftCurrency||i.EftReference||i.EftStatementReference "
               + "||i.EftCheckNo||i.EftMemo||i.EftPayee||i.EftPayeeAccount) ");

            StringBuilder updateSql = new StringBuilder("UPDATE I_Bankstatement "
                    + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'Err=Duplicate['||@param1||']' "
                    + "WHERE I_BankStatement_ID=@param2").Append(clientCheck);
            //PreparedStatement pupdt = DataBase.prepareStatement(updateSql.ToString(), get_TrxName());

            //PreparedStatement pstmtDuplicates = null;
            IDataReader idr = null;
            SqlParameter[] param = new SqlParameter[2];
            no = 0;
            try
            {
                //pstmtDuplicates = DataBase.prepareStatement(sql.ToString(), get_TrxName());
                //ResultSet rs = pstmtDuplicates.executeQuery();
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                while (idr.Read())
                {
                    String Info = "Line_ID=" + Utility.Util.GetValueOfInt(idr[1])// s.getInt(2)		//	l.C_BankStatementLine_ID
                    + ",EDTTrxID=" + Utility.Util.GetValueOfString(idr[2]);// rs.getString(3);			//	i.EftTrxID
                    //pupdt.setString(1, Info);	
                    param[0] = new SqlParameter("@param1", Info);
                    //pupdt.setInt(2, rs.getInt(1));	//	i.I_BankStatement_ID
                    param[1] = new SqlParameter("@param2", Utility.Util.GetValueOfInt(idr[0]));
                    //pupdt.executeUpdate();
                    DataBase.DB.ExecuteQuery(updateSql.ToString(), param, Get_TrxName());
                    no++;
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "DetectDuplicates " + e.Message);
            }
            if (no != 0)
                log.Info("Duplicates=" + no);

            Commit();

            //Import Bank Statement
            sql = new StringBuilder("SELECT * FROM I_BankStatement"
                + " WHERE I_IsImported='N'"
                + " ORDER BY C_BankAccount_ID, Name, EftStatementDate, EftStatementReference");

            MBankStatement statement = null;
            MBankAccount account = null;
            //PreparedStatement pstmt = null;
            int lineNo = 10;
            int noInsert = 0;
            int noInsertLine = 0;
            try
            {
                //pstmt = DataBase.prepareStatement(sql.ToString(), get_TrxName());
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());

                while (idr.Read())
                {
                    X_I_BankStatement imp = new X_I_BankStatement(_ctx, idr, Get_TrxName());
                    //	Get the bank account for the first statement
                    if (account == null)
                    {
                        account = MBankAccount.Get(_ctx, imp.GetC_BankAccount_ID());
                        statement = null;
                        log.Info("New Statement, Account=" + account.GetAccountNo());
                    }
                    //	Create a new Bank Statement for every account
                    else if (account.GetC_BankAccount_ID() != imp.GetC_BankAccount_ID())
                    {
                        account = MBankAccount.Get(_ctx, imp.GetC_BankAccount_ID());
                        statement = null;
                        log.Info("New Statement, Account=" + account.GetAccountNo());
                    }
                    //	Create a new Bank Statement for every statement name
                    else if ((statement.GetName() != null) && (imp.GetName() != null))
                    {
                        if (!statement.GetName().Equals(imp.GetName()))
                        {
                            statement = null;
                            log.Info("New Statement, Statement Name=" + imp.GetName());
                        }
                    }
                    //	Create a new Bank Statement for every statement reference
                    else if ((statement.GetEftStatementReference() != null) && (imp.GetEftStatementReference() != null))
                    {
                        if (!statement.GetEftStatementReference().Equals(imp.GetEftStatementReference()))
                        {
                            statement = null;
                            log.Info("New Statement, Statement Reference=" + imp.GetEftStatementReference());
                        }
                    }
                    //	Create a new Bank Statement for every statement date
                    else if ((statement.GetStatementDate() != null) && (imp.GetStatementDate() != null))
                    {
                        if (!statement.GetStatementDate().Equals(imp.GetStatementDate()))
                        {
                            statement = null;
                            log.Info("New Statement, Statement Date=" + imp.GetStatementDate());
                        }
                    }

                    //	New Statement
                    if (statement == null)
                    {
                        statement = new MBankStatement(account);
                        statement.SetEndingBalance(Env.ZERO);

                        //	Copy statement data
                        if (imp.GetName() != null)
                        {
                            statement.SetName(imp.GetName());
                        }
                        if (imp.GetStatementDate() != null)
                        {
                            statement.SetStatementDate(imp.GetStatementDate());
                        }
                        statement.SetDescription(imp.GetDescription());
                        statement.SetEftStatementReference(imp.GetEftStatementReference());
                        statement.SetEftStatementDate(imp.GetEftStatementDate());
                        if (statement.Save())
                        {
                            noInsert++;
                        }
                        lineNo = 10;
                    }

                    //	New StatementLine
                    MBankStatementLine line = new MBankStatementLine(statement, lineNo);

                    //	Copy statement line data
                    //line.setC_BPartner_ID(imp.getC_BPartner_ID());
                    //line.setC_Invoice_ID(imp.getC_Invoice_ID());
                    line.SetReferenceNo(imp.GetReferenceNo());
                    line.SetDescription(imp.GetLineDescription());
                    line.SetStatementLineDate(imp.GetStatementLineDate());
                    line.SetDateAcct(imp.GetStatementLineDate());
                    line.SetValutaDate(imp.GetValutaDate());
                    line.SetIsReversal(imp.IsReversal());
                    line.SetC_Currency_ID(imp.GetC_Currency_ID());
                    line.SetTrxAmt(imp.GetTrxAmt());
                    line.SetStmtAmt(imp.GetStmtAmt());
                    if (imp.GetC_Charge_ID() != 0)
                    {
                        line.SetC_Charge_ID(imp.GetC_Charge_ID());
                    }
                    line.SetInterestAmt(imp.GetInterestAmt());
                    line.SetChargeAmt(imp.GetChargeAmt());
                    line.SetMemo(imp.GetMemo());
                    if (imp.GetC_Payment_ID() != 0)
                    {
                        line.SetC_Payment_ID(imp.GetC_Payment_ID());
                    }

                    //	Copy statement line reference data
                    line.SetEftTrxID(imp.GetEftTrxID());
                    line.SetEftTrxType(imp.GetEftTrxType());
                    line.SetEftCheckNo(imp.GetEftCheckNo());
                    line.SetEftReference(imp.GetEftReference());
                    line.SetEftMemo(imp.GetEftMemo());
                    line.SetEftPayee(imp.GetEftPayee());
                    line.SetEftPayeeAccount(imp.GetEftPayeeAccount());
                    line.SetEftStatementLineDate(imp.GetEftStatementLineDate());
                    line.SetEftValutaDate(imp.GetEftValutaDate());
                    line.SetEftCurrency(imp.GetEftCurrency());
                    line.SetEftAmt(imp.GetEftAmt());

                    //	Save statement line
                    if (line.Save())
                    {
                        imp.SetC_BankStatement_ID(statement.GetC_BankStatement_ID());
                        imp.SetC_BankStatementLine_ID(line.GetC_BankStatementLine_ID());
                        imp.SetI_IsImported(X_I_BankStatement.I_ISIMPORTED_Yes);
                        imp.SetProcessed(true);
                        imp.Save();
                        noInsertLine++;
                        lineNo += 10;
                    }
                    line = null;

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
            sql = new StringBuilder("UPDATE I_BankStatement "
                + "SET I_IsImported='N', Updated=SysDate "
                + "WHERE I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, Utility.Util.GetValueOfDecimal(no), "@Errors@");
            //
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsert), "@C_BankStatement_ID@: @Inserted@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsertLine), "@C_BankStatementLine_ID@: @Inserted@");
            return "";

        }	//	doIt

    }
}
