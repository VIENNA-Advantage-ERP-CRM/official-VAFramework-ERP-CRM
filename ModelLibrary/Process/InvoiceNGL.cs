/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : InvoiceNGL
 * Purpose        : Invoice Not realized Gain & Loss.The actual data shown is T_InvoiceGL_v
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
        private int _C_AcctSchema_ID = 0;
        /** Mandatory Conversion Type		*/
        private int _C_ConversionTypeReval_ID = 0;
        /** Revaluation Date				*/
        private DateTime? _DateReval = null;
        /** Only AP/AR Transactions			*/
        private String _APAR = "A";
        private static String ONLY_AP = "P";
        private static String ONLY_AR = "R";
        /** Report all Currencies			*/
        private Boolean _IsAllCurrencies = false;
        /** Optional Invoice Currency		*/
        private int _C_Currency_ID = 0;
        /** GL Document Type				*/
        private int _C_DocTypeReval_ID = 0;

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
                else if (name.Equals("C_AcctSchema_ID"))
                {
                    _C_AcctSchema_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_ConversionTypeReval_ID"))
                {
                    _C_ConversionTypeReval_ID = para[i].GetParameterAsInt();
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
                else if (name.Equals("C_Currency_ID"))
                {
                    _C_Currency_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_DocTypeReval_ID"))
                {
                    _C_DocTypeReval_ID = para[i].GetParameterAsInt();
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
                _C_Currency_ID = 0;
            }
            log.Info("C_AcctSchema_ID=" + _C_AcctSchema_ID
                + ",C_ConversionTypeReval_ID=" + _C_ConversionTypeReval_ID
                + ",DateReval=" + _DateReval
                + ", APAR=" + _APAR
                + ", IsAllCurrencies=" + _IsAllCurrencies
                + ",C_Currency_ID=" + _C_Currency_ID
                + ", C_DocType_ID=" + _C_DocTypeReval_ID);

            //	Parameter
            if (_DateReval == null)
            {
                _DateReval = DateTime.Now; //new Timestamp(System.currentTimeMillis());
            }
            //	Delete - just to be sure
            String sql = "DELETE FROM T_InvoiceGL WHERE AD_PInstance_ID=" + GetAD_PInstance_ID();  //jz FROM
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no > 0)
            {
                log.Info("Deleted #" + no);
            }

            //	Insert Trx
            String dateStr = DataBase.DB.TO_DATE(_DateReval, true);
            sql = "INSERT INTO T_InvoiceGL (AD_Client_ID, AD_Org_ID, IsActive, Created,CreatedBy, Updated,UpdatedBy,"
             + " AD_PInstance_ID, C_Invoice_ID, GrandTotal, OpenAmt, "
             + " Fact_Acct_ID, AmtSourceBalance, AmtAcctBalance, "
             + " AmtRevalDr, AmtRevalCr, C_DocTypeReval_ID, IsAllCurrencies, "
             + " DateReval, C_ConversionTypeReval_ID, AmtRevalDrDiff, AmtRevalCrDiff, APAR) "
                //	--
             + "SELECT i.AD_Client_ID, i.AD_Org_ID, i.IsActive, i.Created,i.CreatedBy, i.Updated,i.UpdatedBy,"
             + GetAD_PInstance_ID() + ", i.C_Invoice_ID, i.GrandTotal, invoiceOpen(i.C_Invoice_ID, 0), "
             + " fa.Fact_Acct_ID, fa.AmtSourceDr-fa.AmtSourceCr, fa.AmtAcctDr-fa.AmtAcctCr, "
                //	AmtRevalDr, AmtRevalCr,
             + " currencyConvert(fa.AmtSourceDr, i.C_Currency_ID, a.C_Currency_ID, " + dateStr + ", " + _C_ConversionTypeReval_ID + ", i.AD_Client_ID, i.AD_Org_ID),"
             + " currencyConvert(fa.AmtSourceCr, i.C_Currency_ID, a.C_Currency_ID, " + dateStr + ", " + _C_ConversionTypeReval_ID + ", i.AD_Client_ID, i.AD_Org_ID),"
             + (_C_DocTypeReval_ID == 0 || _C_DocTypeReval_ID==-1 ? "NULL" : Utility.Util.GetValueOfString(_C_DocTypeReval_ID)) + ", "
             + (_IsAllCurrencies ? "'Y'," : "'N',")
             + dateStr + ", " + _C_ConversionTypeReval_ID + ", 0, 0, '" + _APAR + "' "
                //
             + "FROM C_Invoice_v i"
             + " INNER JOIN Fact_Acct fa ON (fa.AD_Table_ID=318 AND fa.Record_ID=i.C_Invoice_ID"
                 + " AND (i.GrandTotal=fa.AmtSourceDr OR i.GrandTotal=fa.AmtSourceCr))"
             + " INNER JOIN C_AcctSchema a ON (fa.C_AcctSchema_ID=a.C_AcctSchema_ID) "
             + "WHERE i.IsPaid='N'"
             + " AND EXISTS (SELECT * FROM C_ElementValue ev "
                 + "WHERE ev.C_ElementValue_ID=fa.Account_ID AND (ev.AccountType='A' OR ev.AccountType='L'))"
             + " AND fa.C_AcctSchema_ID=" + _C_AcctSchema_ID;
            if (!_IsAllCurrencies)
            {
                sql += " AND i.C_Currency_ID<>a.C_Currency_ID";
            }
            if (ONLY_AR.Equals(_APAR))
            {
                sql += " AND i.IsSOTrx='Y'";
            }
            else if (ONLY_AP.Equals(_APAR))
            {
                sql += " AND i.IsSOTrx='N'";
            }
            if (!_IsAllCurrencies && _C_Currency_ID != 0 && _C_Currency_ID!=-1)
            {
                sql += " AND i.C_Currency_ID=" + _C_Currency_ID;
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
            sql = "UPDATE T_InvoiceGL gl "
                + "SET (AmtRevalDrDiff,AmtRevalCrDiff)="
                    + "(SELECT gl.AmtRevalDr-fa.AmtAcctDr, gl.AmtRevalCr-fa.AmtAcctCr "
                    + "FROM Fact_Acct fa "
                    + "WHERE gl.Fact_Acct_ID=fa.Fact_Acct_ID) "
                + "WHERE AD_PInstance_ID=" + GetAD_PInstance_ID();
            int noT = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (noT > 0)
            {
                log.Config("Difference #" + noT);
            }

            //	Percentage
            sql = "UPDATE T_InvoiceGL SET PercentGL = 100 "
                + "WHERE GrandTotal=OpenAmt AND AD_PInstance_ID=" + GetAD_PInstance_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no > 0)
            {
                log.Info("Not Paid #" + no);
            }

            sql = "UPDATE T_InvoiceGL SET PercentGL = ROUND(OpenAmt*100/GrandTotal,6) "
                + "WHERE GrandTotal<>OpenAmt AND GrandTotal <> 0 AND AD_PInstance_ID=" + GetAD_PInstance_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no > 0)
            {
                log.Info("Partial Paid #" + no);
            }

            sql = "UPDATE T_InvoiceGL SET AmtRevalDr = AmtRevalDr * PercentGL/100,"
                + " AmtRevalCr = AmtRevalCr * PercentGL/100,"
                + " AmtRevalDrDiff = AmtRevalDrDiff * PercentGL/100,"
                + " AmtRevalCrDiff = AmtRevalCrDiff * PercentGL/100 "
                + "WHERE PercentGL <> 100 AND AD_PInstance_ID=" + GetAD_PInstance_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no > 0)
            {
                log.Config("Partial Calc #" + no);
            }

            //	Create Document
            String info = "";
            if (_C_DocTypeReval_ID != 0 && _C_DocTypeReval_ID!=-1)
            {
                if (_C_Currency_ID != 0)
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
            List<X_T_InvoiceGL> list = new List<X_T_InvoiceGL>();
            String sql = "SELECT * FROM T_InvoiceGL "
                + "WHERE AD_PInstance_ID=" + GetAD_PInstance_ID()
                + " ORDER BY AD_Org_ID";
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                while (idr.Read())
                {
                    list.Add(new X_T_InvoiceGL(GetCtx(), idr, Get_TrxName()));
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
            MAcctSchema aas = MAcctSchema.Get(GetCtx(), _C_AcctSchema_ID);
            MAcctSchemaDefault asDefaultAccts = MAcctSchemaDefault.Get(GetCtx(), _C_AcctSchema_ID);
            MGLCategory cat = MGLCategory.GetDefaultSystem(GetCtx());
            if (cat == null)
            {
                MDocType docType = MDocType.Get(GetCtx(), _C_DocTypeReval_ID);
                cat = MGLCategory.Get(GetCtx(), docType.GetGL_Category_ID());
            }
            //
            MJournalBatch batch = new MJournalBatch(GetCtx(), 0, Get_TrxName());
            batch.SetDescription(GetName());
            batch.SetC_DocType_ID(_C_DocTypeReval_ID);
            batch.SetDateDoc(DateTime.Now);// new Timestamp(System.currentTimeMillis()));
            batch.SetDateAcct(_DateReval);
            batch.SetC_Currency_ID(aas.GetC_Currency_ID());
            if (!batch.Save())
            {
                return " - Could not create Batch";
            }
            //
            MJournal journal = null;
            Decimal? drTotal = Env.ZERO;
            Decimal? crTotal = Env.ZERO;
            int AD_Org_ID = 0;
            for (int i = 0; i < list.Count; i++)
            {
                X_T_InvoiceGL gl = list[i];//.get(i);
                if (Env.Signum(gl.GetAmtRevalDrDiff()) == 0 && Env.Signum(gl.GetAmtRevalCrDiff()) == 0)
                {
                    continue;
                }
                MInvoice invoice = new MInvoice(GetCtx(), gl.GetC_Invoice_ID(), null);
                if (invoice.GetC_Currency_ID() == aas.GetC_Currency_ID())
                {
                    continue;
                }
                //
                if (journal == null)
                {
                    journal = new MJournal(batch);
                    journal.SetC_AcctSchema_ID(aas.GetC_AcctSchema_ID());
                    journal.SetC_Currency_ID(aas.GetC_Currency_ID());
                    journal.SetC_ConversionType_ID(_C_ConversionTypeReval_ID);
                    MOrg org = MOrg.Get(GetCtx(), gl.GetAD_Org_ID());
                    journal.SetDescription(GetName() + " - " + org.GetName());
                    journal.SetGL_Category_ID(cat.GetGL_Category_ID());
                    if (!journal.Save())
                    {
                        return " - Could not create Journal";
                    }
                }
                //
                MJournalLine line = new MJournalLine(journal);
                line.SetLine((i + 1) * 10);
                line.SetDescription(invoice.GetSummary());
                //
                MFactAcct fa = new MFactAcct(GetCtx(), gl.GetFact_Acct_ID(), null);
                line.SetC_ValidCombination_ID(MAccount.Get(fa));
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
                if (AD_Org_ID == 0)		//	invoice org id
                {
                    AD_Org_ID = gl.GetAD_Org_ID();
                }
                //	Change in Org
                if (AD_Org_ID != gl.GetAD_Org_ID())
                {
                    CreateBalancing(asDefaultAccts, journal, drTotal.Value, crTotal.Value, AD_Org_ID, (i + 1) * 10);
                    //
                    AD_Org_ID = gl.GetAD_Org_ID();
                    drTotal = Env.ZERO;
                    crTotal = Env.ZERO;
                    journal = null;
                }
            }
            CreateBalancing(asDefaultAccts, journal, drTotal.Value, crTotal.Value, AD_Org_ID, (list.Count + 1) * 10);

            return " - " + batch.GetDocumentNo() + " #" + list.Count;
        }	//	createGLJournal

        /// <summary>
        ///  Create Balancing Entry
        /// </summary>
        /// <param name="asDefaultAccts">acct schema default accounts</param>
        /// <param name="journal">journal</param>
        /// <param name="drTotal">dr</param>
        /// <param name="crTotal">cr</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="lineNo">lineno base line no</param>
        private void CreateBalancing(MAcctSchemaDefault asDefaultAccts, MJournal journal,
            Decimal drTotal, Decimal crTotal, int AD_Org_ID, int lineNo)
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
                MAccount bas = MAccount.Get(GetCtx(), asDefaultAccts.GetUnrealizedGain_Acct());
                MAccount acct = MAccount.Get(GetCtx(), asDefaultAccts.GetAD_Client_ID(), AD_Org_ID,
                    asDefaultAccts.GetC_AcctSchema_ID(), bas.GetAccount_ID(), bas.GetC_SubAcct_ID(),
                    bas.GetM_Product_ID(), bas.GetC_BPartner_ID(), bas.GetAD_OrgTrx_ID(),
                    bas.GetC_LocFrom_ID(), bas.GetC_LocTo_ID(), bas.GetC_SalesRegion_ID(),
                    bas.GetC_Project_ID(), bas.GetC_Campaign_ID(), bas.GetC_Activity_ID(),
                    bas.GetUser1_ID(), bas.GetUser2_ID(), bas.GetUserElement1_ID(), bas.GetUserElement2_ID());
                line.SetDescription(Msg.GetElement(GetCtx(), "UnrealizedGain_Acct"));
                line.SetC_ValidCombination_ID(acct.GetC_ValidCombination_ID());
                line.SetAmtSourceCr(drTotal);
                line.SetAmtAcctCr(drTotal);
                line.Save();
            }
            //	DR Entry = Loss
            if (Env.Signum(crTotal) != 0)
            {
                MJournalLine line = new MJournalLine(journal);
                line.SetLine(lineNo + 2);
                MAccount bas = MAccount.Get(GetCtx(), asDefaultAccts.GetUnrealizedLoss_Acct());
                MAccount acct = MAccount.Get(GetCtx(), asDefaultAccts.GetAD_Client_ID(), AD_Org_ID,
                    asDefaultAccts.GetC_AcctSchema_ID(), bas.GetAccount_ID(), bas.GetC_SubAcct_ID(),
                    bas.GetM_Product_ID(), bas.GetC_BPartner_ID(), bas.GetAD_OrgTrx_ID(),
                    bas.GetC_LocFrom_ID(), bas.GetC_LocTo_ID(), bas.GetC_SalesRegion_ID(),
                    bas.GetC_Project_ID(), bas.GetC_Campaign_ID(), bas.GetC_Activity_ID(),
                    bas.GetUser1_ID(), bas.GetUser2_ID(), bas.GetUserElement1_ID(), bas.GetUserElement2_ID());
                line.SetDescription(Msg.GetElement(GetCtx(), "UnrealizedLoss_Acct"));
                line.SetC_ValidCombination_ID(acct.GetC_ValidCombination_ID());
                line.SetAmtSourceDr(crTotal);
                line.SetAmtAcctDr(crTotal);
                line.Save();
            }
        }	//	createBalancing

    }	//	InvoiceNGL

}
