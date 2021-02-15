/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : InvoiceNGL
 * Purpose        : Invoice Not realized Gain & Loss.The actual data shown is VAT_InvoiceGL_v
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           15-Jan-2010
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
    public class InvoiceNGL : ProcessEngine.SvrProcess
    {
        /**	Mandatory Acct Schema			*/
        private int _VAB_AccountBook_ID = 0;
        /** Mandatory Conversion Type		*/
        private int _VAB_CurrencyTypeReval_ID = 0;
        /** Revaluation Date				*/
        private DateTime? _DateReval = null;
        /** Only AP/AR Transactions			*/
        private String _APAR = "A";
        private static String ONLY_AP = "P";
        private static String ONLY_AR = "R";
        /** Report all Currencies			*/
        private Boolean _IsAllCurrencies = false;
        /** Optional Invoice Currency		*/
        private int _VAB_Currency_ID = 0;
        /** GL Document Type				*/
        private int _VAB_DocTypesReval_ID = 0;

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
                else if (name.Equals("VAB_AccountBook_ID"))
                {
                    _VAB_AccountBook_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAB_CurrencyTypeReval_ID"))
                {
                    _VAB_CurrencyTypeReval_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("DateReval"))
                {
                    _DateReval = (DateTime?)para[i].GetParameter();
                }
                else if (name.Equals("APAR"))
                {
                    _APAR = (String)para[i].GetParameter();
                }
                else if (name.Equals("IsAllCurrencies"))
                {
                    _IsAllCurrencies = "Y".Equals((String)para[i].GetParameter());
                }
                else if (name.Equals("VAB_Currency_ID"))
                {
                    _VAB_Currency_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAB_DocTypesReval_ID"))
                {
                    _VAB_DocTypesReval_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }	//	prepare

        /// <summary>
        /// Process
        /// </summary>
        /// <returns> info</returns>
        protected override String DoIt()
        {
            if (_IsAllCurrencies)
            {
                _VAB_Currency_ID = 0;
            }
            log.Info("VAB_AccountBook_ID=" + _VAB_AccountBook_ID
                + ",VAB_CurrencyTypeReval_ID=" + _VAB_CurrencyTypeReval_ID
                + ",DateReval=" + _DateReval
                + ", APAR=" + _APAR
                + ", IsAllCurrencies=" + _IsAllCurrencies
                + ",VAB_Currency_ID=" + _VAB_Currency_ID
                + ", VAB_DocTypes_ID=" + _VAB_DocTypesReval_ID);

            //	Parameter
            if (_DateReval == null)
            {
                _DateReval = DateTime.Now; //new Timestamp(System.currentTimeMillis());
            }
            //	Delete - just to be sure
            String sql = "DELETE FROM VAT_InvoiceGL WHERE VAF_JInstance_ID=" + GetVAF_JInstance_ID();  //jz FROM
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no > 0)
            {
                log.Info("Deleted #" + no);
            }

            //	Insert Trx
            String dateStr = DataBase.DB.TO_DATE(_DateReval, true);
            sql = "INSERT INTO VAT_InvoiceGL (VAF_Client_ID, VAF_Org_ID, IsActive, Created,CreatedBy, Updated,UpdatedBy,"
             + " VAF_JInstance_ID, VAB_Invoice_ID, GrandTotal, OpenAmt, "
             + " Actual_Acct_Detail_ID, AmtSourceBalance, AmtAcctBalance, "
             + " AmtRevalDr, AmtRevalCr, VAB_DocTypesReval_ID, IsAllCurrencies, "
             + " DateReval, VAB_CurrencyTypeReval_ID, AmtRevalDrDiff, AmtRevalCrDiff, APAR) "
                //	--
             + "SELECT i.VAF_Client_ID, i.VAF_Org_ID, i.IsActive, i.Created,i.CreatedBy, i.Updated,i.UpdatedBy,"
             + GetVAF_JInstance_ID() + ", i.VAB_Invoice_ID, i.GrandTotal, invoiceOpen(i.VAB_Invoice_ID, 0), "
             + " fa.Actual_Acct_Detail_ID, fa.AmtSourceDr-fa.AmtSourceCr, fa.AmtAcctDr-fa.AmtAcctCr, "
                //	AmtRevalDr, AmtRevalCr,
             + " currencyConvert(fa.AmtSourceDr, i.VAB_Currency_ID, a.VAB_Currency_ID, " + dateStr + ", " + _VAB_CurrencyTypeReval_ID + ", i.VAF_Client_ID, i.VAF_Org_ID),"
             + " currencyConvert(fa.AmtSourceCr, i.VAB_Currency_ID, a.VAB_Currency_ID, " + dateStr + ", " + _VAB_CurrencyTypeReval_ID + ", i.VAF_Client_ID, i.VAF_Org_ID),"
             + (_VAB_DocTypesReval_ID == 0 || _VAB_DocTypesReval_ID==-1 ? "NULL" : Utility.Util.GetValueOfString(_VAB_DocTypesReval_ID)) + ", "
             + (_IsAllCurrencies ? "'Y'," : "'N',")
             + dateStr + ", " + _VAB_CurrencyTypeReval_ID + ", 0, 0, '" + _APAR + "' "
                //
             + "FROM VAB_Invoice_v i"
             + " INNER JOIN Actual_Acct_Detail fa ON (fa.VAF_TableView_ID=318 AND fa.Record_ID=i.VAB_Invoice_ID"
                 + " AND (i.GrandTotal=fa.AmtSourceDr OR i.GrandTotal=fa.AmtSourceCr))"
             + " INNER JOIN VAB_AccountBook a ON (fa.VAB_AccountBook_ID=a.VAB_AccountBook_ID) "
             + "WHERE i.IsPaid='N'"
             + " AND EXISTS (SELECT * FROM VAB_Acct_Element ev "
                 + "WHERE ev.VAB_Acct_Element_ID=fa.Account_ID AND (ev.AccountType='A' OR ev.AccountType='L'))"
             + " AND fa.VAB_AccountBook_ID=" + _VAB_AccountBook_ID;
            if (!_IsAllCurrencies)
            {
                sql += " AND i.VAB_Currency_ID<>a.VAB_Currency_ID";
            }
            if (ONLY_AR.Equals(_APAR))
            {
                sql += " AND i.IsSOTrx='Y'";
            }
            else if (ONLY_AP.Equals(_APAR))
            {
                sql += " AND i.IsSOTrx='N'";
            }
            if (!_IsAllCurrencies && _VAB_Currency_ID != 0 && _VAB_Currency_ID!=-1)
            {
                sql += " AND i.VAB_Currency_ID=" + _VAB_Currency_ID;
            }

            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Info("Inserted #" + no);
            }
            else if (VLogMgt.IsLevelFiner())
            {
                log.Warning("Inserted #" + no + " - " + sql);
            }
            else
            {
                log.Warning("Inserted #" + no);
            }

            //	Calculate Difference
            sql = "UPDATE VAT_InvoiceGL gl "
                + "SET (AmtRevalDrDiff,AmtRevalCrDiff)="
                    + "(SELECT gl.AmtRevalDr-fa.AmtAcctDr, gl.AmtRevalCr-fa.AmtAcctCr "
                    + "FROM Actual_Acct_Detail fa "
                    + "WHERE gl.Actual_Acct_Detail_ID=fa.Actual_Acct_Detail_ID) "
                + "WHERE VAF_JInstance_ID=" + GetVAF_JInstance_ID();
            int noT = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (noT > 0)
            {
                log.Config("Difference #" + noT);
            }

            //	Percentage
            sql = "UPDATE VAT_InvoiceGL SET PercentGL = 100 "
                + "WHERE GrandTotal=OpenAmt AND VAF_JInstance_ID=" + GetVAF_JInstance_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no > 0)
            {
                log.Info("Not Paid #" + no);
            }

            sql = "UPDATE VAT_InvoiceGL SET PercentGL = ROUND(OpenAmt*100/GrandTotal,6) "
                + "WHERE GrandTotal<>OpenAmt AND GrandTotal <> 0 AND VAF_JInstance_ID=" + GetVAF_JInstance_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no > 0)
            {
                log.Info("Partial Paid #" + no);
            }

            sql = "UPDATE VAT_InvoiceGL SET AmtRevalDr = AmtRevalDr * PercentGL/100,"
                + " AmtRevalCr = AmtRevalCr * PercentGL/100,"
                + " AmtRevalDrDiff = AmtRevalDrDiff * PercentGL/100,"
                + " AmtRevalCrDiff = AmtRevalCrDiff * PercentGL/100 "
                + "WHERE PercentGL <> 100 AND VAF_JInstance_ID=" + GetVAF_JInstance_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no > 0)
            {
                log.Config("Partial Calc #" + no);
            }

            //	Create Document
            String info = "";
            if (_VAB_DocTypesReval_ID != 0 && _VAB_DocTypesReval_ID!=-1)
            {
                if (_VAB_Currency_ID != 0)
                {
                    log.Warning("Can create Journal only for all currencies");
                }
                else
                {
                    info = CreateGLJournal();
                }
            }
            return "#" + noT + info;
        }	//	doIt

        /// <summary>
        /// Create GL Journal
        /// </summary>
        /// <returns>document info</returns>
        private String CreateGLJournal()
        {
            List<X_VAT_InvoiceGL> list = new List<X_VAT_InvoiceGL>();
            String sql = "SELECT * FROM VAT_InvoiceGL "
                + "WHERE VAF_JInstance_ID=" + GetVAF_JInstance_ID()
                + " ORDER BY VAF_Org_ID";
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                while (idr.Read())
                {
                    list.Add(new X_VAT_InvoiceGL(GetCtx(), idr, Get_TrxName()));
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            if (list.Count == 0)
            {
                return " - No Records found";
            }

            //
            MVABAccountBook aas = MVABAccountBook.Get(GetCtx(), _VAB_AccountBook_ID);
            MVABAccountBookDefault asDefaultAccts = MVABAccountBookDefault.Get(GetCtx(), _VAB_AccountBook_ID);
            MVAGLGroup cat = MVAGLGroup.GetDefaultSystem(GetCtx());
            if (cat == null)
            {
                MVABDocTypes docType = MVABDocTypes.Get(GetCtx(), _VAB_DocTypesReval_ID);
                cat = MVAGLGroup.Get(GetCtx(), docType.GetVAGL_Group_ID());
            }
            //
            MJournalBatch batch = new MJournalBatch(GetCtx(), 0, Get_TrxName());
            batch.SetDescription(GetName());
            batch.SetVAB_DocTypes_ID(_VAB_DocTypesReval_ID);
            batch.SetDateDoc(DateTime.Now);// new Timestamp(System.currentTimeMillis()));
            batch.SetDateAcct(_DateReval);
            batch.SetVAB_Currency_ID(aas.GetVAB_Currency_ID());
            if (!batch.Save())
            {
                return GetRetrievedError(batch, "Could not create Batch");
                //return " - Could not create Batch";
            }
            //
            MJournal journal = null;
            Decimal? drTotal = Env.ZERO;
            Decimal? crTotal = Env.ZERO;
            int VAF_Org_ID = 0;
            for (int i = 0; i < list.Count; i++)
            {
                X_VAT_InvoiceGL gl = list[i];//.get(i);
                if (Env.Signum(gl.GetAmtRevalDrDiff()) == 0 && Env.Signum(gl.GetAmtRevalCrDiff()) == 0)
                {
                    continue;
                }
                MInvoice invoice = new MInvoice(GetCtx(), gl.GetVAB_Invoice_ID(), null);
                if (invoice.GetVAB_Currency_ID() == aas.GetVAB_Currency_ID())
                {
                    continue;
                }
                //
                if (journal == null)
                {
                    journal = new MJournal(batch);
                    journal.SetVAB_AccountBook_ID(aas.GetVAB_AccountBook_ID());
                    journal.SetVAB_Currency_ID(aas.GetVAB_Currency_ID());
                    journal.SetVAB_CurrencyType_ID(_VAB_CurrencyTypeReval_ID);
                    MVAFOrg org = MVAFOrg.Get(GetCtx(), gl.GetVAF_Org_ID());
                    journal.SetDescription(GetName() + " - " + org.GetName());
                    journal.SetVAGL_Group_ID(cat.GetVAGL_Group_ID());
                    if (!journal.Save())
                    {
                        return GetRetrievedError(journal, "Could not create Journal");
                        //return " - Could not create Journal";
                    }
                }
                //
                MJournalLine line = new MJournalLine(journal);
                line.SetLine((i + 1) * 10);
                line.SetDescription(invoice.GetSummary());
                //
                MActualAcctDetail fa = new MActualAcctDetail(GetCtx(), gl.GetActual_Acct_Detail_ID(), null);
                line.SetVAB_Acct_ValidParameter_ID(MVABAccount.Get(fa));
                Decimal? dr = gl.GetAmtRevalDrDiff();
                Decimal? cr = gl.GetAmtRevalCrDiff();
                drTotal = Decimal.Add(drTotal.Value, dr.Value);
                crTotal = Decimal.Add(crTotal.Value, cr.Value);
                line.SetAmtSourceDr(dr.Value);
                line.SetAmtAcctDr(dr.Value);
                line.SetAmtSourceCr(cr.Value);
                line.SetAmtAcctCr(cr.Value);
                line.Save();
                //
                if (VAF_Org_ID == 0)		//	invoice org id
                {
                    VAF_Org_ID = gl.GetVAF_Org_ID();
                }
                //	Change in Org
                if (VAF_Org_ID != gl.GetVAF_Org_ID())
                {
                    CreateBalancing(asDefaultAccts, journal, drTotal.Value, crTotal.Value, VAF_Org_ID, (i + 1) * 10);
                    //
                    VAF_Org_ID = gl.GetVAF_Org_ID();
                    drTotal = Env.ZERO;
                    crTotal = Env.ZERO;
                    journal = null;
                }
            }
            CreateBalancing(asDefaultAccts, journal, drTotal.Value, crTotal.Value, VAF_Org_ID, (list.Count + 1) * 10);

            return " - " + batch.GetDocumentNo() + " #" + list.Count;
        }	//	createGLJournal

        /// <summary>
        ///  Create Balancing Entry
        /// </summary>
        /// <param name="asDefaultAccts">acct schema default accounts</param>
        /// <param name="journal">journal</param>
        /// <param name="drTotal">dr</param>
        /// <param name="crTotal">cr</param>
        /// <param name="VAF_Org_ID">org</param>
        /// <param name="lineNo">lineno base line no</param>
        private void CreateBalancing(MVABAccountBookDefault asDefaultAccts, MJournal journal,
            Decimal drTotal, Decimal crTotal, int VAF_Org_ID, int lineNo)
        {
            if (journal == null)
            {
                throw new ArgumentException("Jornal is null");
            }
            //		CR Entry = Gain
            if (Env.Signum(drTotal) != 0)
            {
                MJournalLine line = new MJournalLine(journal);
                line.SetLine(lineNo + 1);
                MVABAccount bas = MVABAccount.Get(GetCtx(), asDefaultAccts.GetUnrealizedGain_Acct());
                MVABAccount acct = MVABAccount.Get(GetCtx(), asDefaultAccts.GetVAF_Client_ID(), VAF_Org_ID,
                    asDefaultAccts.GetVAB_AccountBook_ID(), bas.GetAccount_ID(), bas.GetVAB_SubAcct_ID(),
                    bas.GetVAM_Product_ID(), bas.GetVAB_BusinessPartner_ID(), bas.GetVAF_OrgTrx_ID(),
                    bas.GetC_LocFrom_ID(), bas.GetC_LocTo_ID(), bas.GetVAB_SalesRegionState_ID(),
                    bas.GetVAB_Project_ID(), bas.GetVAB_Promotion_ID(), bas.GetVAB_BillingCode_ID(),
                    bas.GetUser1_ID(), bas.GetUser2_ID(), bas.GetUserElement1_ID(), bas.GetUserElement2_ID());
                line.SetDescription(Msg.GetElement(GetCtx(), "UnrealizedGain_Acct"));
                line.SetVAB_Acct_ValidParameter_ID(acct.GetVAB_Acct_ValidParameter_ID());
                line.SetAmtSourceCr(drTotal);
                line.SetAmtAcctCr(drTotal);
                line.Save();
            }
            //	DR Entry = Loss
            if (Env.Signum(crTotal) != 0)
            {
                MJournalLine line = new MJournalLine(journal);
                line.SetLine(lineNo + 2);
                MVABAccount bas = MVABAccount.Get(GetCtx(), asDefaultAccts.GetUnrealizedLoss_Acct());
                MVABAccount acct = MVABAccount.Get(GetCtx(), asDefaultAccts.GetVAF_Client_ID(), VAF_Org_ID,
                    asDefaultAccts.GetVAB_AccountBook_ID(), bas.GetAccount_ID(), bas.GetVAB_SubAcct_ID(),
                    bas.GetVAM_Product_ID(), bas.GetVAB_BusinessPartner_ID(), bas.GetVAF_OrgTrx_ID(),
                    bas.GetC_LocFrom_ID(), bas.GetC_LocTo_ID(), bas.GetVAB_SalesRegionState_ID(),
                    bas.GetVAB_Project_ID(), bas.GetVAB_Promotion_ID(), bas.GetVAB_BillingCode_ID(),
                    bas.GetUser1_ID(), bas.GetUser2_ID(), bas.GetUserElement1_ID(), bas.GetUserElement2_ID());
                line.SetDescription(Msg.GetElement(GetCtx(), "UnrealizedLoss_Acct"));
                line.SetVAB_Acct_ValidParameter_ID(acct.GetVAB_Acct_ValidParameter_ID());
                line.SetAmtSourceDr(crTotal);
                line.SetAmtAcctDr(crTotal);
                line.Save();
            }
        }	//	createBalancing

    }	//	InvoiceNGL

}
