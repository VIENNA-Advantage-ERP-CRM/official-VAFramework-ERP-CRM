/********************************************************
 * Class Name     : MJournal
 * Purpose        : GL Journal Model
 * Class Used     : X_VAGL_JRNL,DocAction 
 * Chronological    Development
 * Deepak           21-Nov-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;
using BaseLibrary.Engine;

namespace VAdvantage.Model
{
    public class MJournal : X_VAGL_JRNL, DocAction
    {

        /** Is record save from GL Voucher form **/
        private bool _isSaveFromForm;
        //	Process Message 			
        private String _processMsg = null;
        private ValueNamePair pp = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAGL_JRNL_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MJournal(Ctx ctx, int VAGL_JRNL_ID, Trx trxName)
            : base(ctx, VAGL_JRNL_ID, trxName)
        {
            //super (ctx, VAGL_JRNL_ID, trxName);
            if (VAGL_JRNL_ID == 0)
            {
                //	setVAGL_JRNL_ID (0);		//	PK
                //	setVAB_AccountBook_ID (0);
                //	setVAB_Currency_ID (0);
                //	setVAB_DocTypes_ID (0);
                //	setVAB_YearPeriod_ID (0);
                //
                SetCurrencyRate(Env.ONE);
                //	setVAB_CurrencyType_ID(0);
                SetDateAcct(DateTime.Now.Date);// Timestamp(Comm.currentTimeMillis()));
                SetDateDoc((DateTime.Now.Date));//new Timestamp(System.currentTimeMillis()));
                //	setDescription (null);
                SetDocAction(DOCACTION_Complete);
                SetDocStatus(DOCSTATUS_Drafted);
                //	setDocumentNo (null);
                //	setVAGL_Group_ID (0);
                SetPostingType(POSTINGTYPE_Actual);
                SetTotalCr(Env.ZERO);
                SetTotalDr(Env.ZERO);
                SetIsApproved(false);
                SetIsPrinted(false);
                SetPosted(false);
                SetProcessed(false);
            }
        }	//	MJournal

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MJournal(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MJournal


        public MJournal(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            //super(ctx, rs, trxName);
        }

        /// <summary>
        /// Parent Constructor.
        /// </summary>
        /// <param name="parent">parent batch</param>
        public MJournal(MJournalBatch parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            //this (parent.getCtx(), 0, parent.get_TrxName());
            SetClientOrg(parent);
            SetVAGL_BatchJRNL_ID(parent.GetVAGL_BatchJRNL_ID());
            SetVAB_DocTypes_ID(parent.GetVAB_DocTypes_ID());
            SetPostingType(parent.GetPostingType());
            //
            SetDateDoc(parent.GetDateDoc());
            SetVAB_YearPeriod_ID(parent.GetVAB_YearPeriod_ID());
            SetDateAcct(parent.GetDateAcct());
            SetVAB_Currency_ID(parent.GetVAB_Currency_ID());
        }	//	MJournal

        /// <summary>
        /// Copy Constructor.Dos not copy: Dates/Period
        /// </summary>
        /// <param name="original">original</param>
        public MJournal(MJournal original)
            : this(original.GetCtx(), 0, original.Get_TrxName())
        {
            //this (original.GetCtx(), 0, original.get_TrxName())
            SetClientOrg(original);
            SetVAGL_BatchJRNL_ID(original.GetVAGL_BatchJRNL_ID());
            //
            SetVAB_AccountBook_ID(original.GetVAB_AccountBook_ID());
            SetVAGL_Budget_ID(original.GetVAGL_Budget_ID());
            SetVAGL_Group_ID(original.GetVAGL_Group_ID());
            SetPostingType(original.GetPostingType());
            SetDescription(original.GetDescription());
            SetVAB_DocTypes_ID(original.GetVAB_DocTypes_ID());
            SetControlAmt(original.GetControlAmt());
            //
            SetVAB_Currency_ID(original.GetVAB_Currency_ID());
            SetVAB_CurrencyType_ID(original.GetVAB_CurrencyType_ID());
            SetCurrencyRate(original.GetCurrencyRate());

            //	setDateDoc(original.getDateDoc());
            //	setDateAcct(original.getDateAcct());
            //	setVAB_YearPeriod_ID(original.getVAB_YearPeriod_ID());
        }	//	MJournal


        /// <summary>
        /// Overwrite Client/Org if required
        /// </summary>
        /// <param name="VAF_Client_ID">client</param>
        /// <param name="VAF_Org_ID"> org</param>
        public new void SetClientOrg(int VAF_Client_ID, int VAF_Org_ID)
        {
            //super.setClientOrg(VAF_Client_ID, VAF_Org_ID);
            base.SetClientOrg(VAF_Client_ID, VAF_Org_ID);
        }	//	setClientOrg

        /// <summary>
        /// Get Period
        /// </summary>
        /// <returns>period or null</returns>
        public MPeriod GetPeriod()
        {
            int VAB_YearPeriod_ID = GetVAB_YearPeriod_ID();
            if (VAB_YearPeriod_ID != 0)
            {
                return MPeriod.Get(GetCtx(), VAB_YearPeriod_ID);
            }
            return null;
        }	//	getPeriod

        /// <summary>
        /// Set Doc Date - Callout.Sets also acct date and period
        /// </summary>
        /// <param name="oldDateDoc">old</param>
        /// <param name="newDateDoc">new</param>
        /// <param name="windowNo">window</param>
        public void SetDateDoc(String oldDateDoc,
               String newDateDoc, int windowNo)
        {
            if (newDateDoc == null || newDateDoc.Length == 0)
            {
                return;
            }
            DateTime? dateDoc = PO.ConvertToTimestamp(newDateDoc);
            if (dateDoc == null)
            {
                return;
            }
            SetDateDoc(dateDoc);
            SetDateAcct(dateDoc.Value);
        }	//	setDateDoc

        /// <summary>
        /// Set Acct Date - Callout.	Sets Period
        /// </summary>
        /// <param name="oldDateAcct">old</param>
        /// <param name="newDateAcct">new</param>
        /// <param name="windowNo">window no</param>
        public void SetDateAcct(String oldDateAcct,
               String newDateAcct, int windowNo)
        {
            if (newDateAcct == null || newDateAcct.Length == 0)
            {
                return;
            }
            DateTime? dateAcct = PO.ConvertToTimestamp(newDateAcct);
            if (dateAcct == null)
            {
                return;
            }
            SetDateAcct(dateAcct.Value);
        }	//	setDateAcct

        /// <summary>
        /// Set Period - Callout.Set Acct Date if required
        /// </summary>
        /// <param name="oldVAB_YearPeriod_ID">old</param>
        /// <param name="newVAB_YearPeriod_ID">new</param>
        /// <param name="windowNo">window no</param>
        public void SetVAB_YearPeriod_ID(String oldVAB_YearPeriod_ID,
               String newVAB_YearPeriod_ID, int windowNo)
        {
            if (newVAB_YearPeriod_ID == null || newVAB_YearPeriod_ID.Length == 0)
            {
                return;
            }
            int VAB_YearPeriod_ID = Utility.Util.GetValueOfInt(newVAB_YearPeriod_ID);
            if (VAB_YearPeriod_ID == 0)
            {
                return;
            }
            SetVAB_YearPeriod_ID(VAB_YearPeriod_ID);
        }	//	setVAB_YearPeriod_ID

        /// <summary>
        /// Set Accounting Date.Set also Period if not set earlier
        /// </summary>
        /// <param name="DateAcct">date</param>
        public void SetDateAcct(DateTime DateAcct)
        {
            //super.setDateAcct(DateAcct);
            base.SetDateAcct(DateAcct);
            if (GetVAB_YearPeriod_ID() != 0)	//	previously set
            {
                SetRate();
                return;
            }
            int VAB_YearPeriod_ID = MPeriod.GetVAB_YearPeriod_ID(GetCtx(), DateAcct);
            if (VAB_YearPeriod_ID == 0)
            {
                log.Warning("Period not found");
            }
            else
            {
                base.SetVAB_YearPeriod_ID(VAB_YearPeriod_ID);
                SetRate();
            }
        }	//	setDateAcct

        /// <summary>
        /// Set Period
        /// </summary>
        /// <param name="VAB_YearPeriod_ID">period</param>
        public new void SetVAB_YearPeriod_ID(int VAB_YearPeriod_ID)
        {
            //super.setVAB_YearPeriod_ID(VAB_YearPeriod_ID);
            base.SetVAB_YearPeriod_ID(VAB_YearPeriod_ID);
            if (VAB_YearPeriod_ID == 0)
            {
                return;
            }
            DateTime? dateAcct = GetDateAcct();
            //
            MPeriod period = GetPeriod();
            if (period != null)
            {
                if (period.IsStandardPeriod()
                    && !period.IsInPeriod(dateAcct))
                    base.SetDateAcct(period.GetEndDate());
            }
        }	//	setVAB_YearPeriod_ID


        /// <summary>
        /// Set Currency Info
        /// </summary>
        /// <param name="VAB_Currency_ID">currenct</param>
        /// <param name="VAB_CurrencyType_ID">type</param>
        /// <param name="CurrencyRate">rate</param>
        public void SetCurrency(int VAB_Currency_ID, int VAB_CurrencyType_ID, Decimal CurrencyRate)
        {
            if (VAB_Currency_ID != 0)
            {
                SetVAB_Currency_ID(VAB_Currency_ID);
            }
            if (VAB_CurrencyType_ID != 0)
            {
                SetVAB_CurrencyType_ID(VAB_CurrencyType_ID);
            }
            if (CurrencyRate.CompareTo(Env.ZERO) == 0)
            {
                SetCurrencyRate(CurrencyRate);
            }
        }	//	setCurrency


        /// <summary>
        /// set callout
        /// </summary>
        /// <param name="oldVAB_CurrencyType_ID">old</param>
        /// <param name="newVAB_CurrencyType_ID">new</param>
        /// <param name="windowNo">window no</param>
        public void SetVAB_CurrencyType_ID(String oldVAB_CurrencyType_ID,
               String newVAB_CurrencyType_ID, int windowNo)
        {
            if (newVAB_CurrencyType_ID == null || newVAB_CurrencyType_ID.Length == 0)
            {
                return;
            }
            int VAB_CurrencyType_ID = Utility.Util.GetValueOfInt(newVAB_CurrencyType_ID);
            if (VAB_CurrencyType_ID == 0)
            {
                return;
            }
            SetVAB_CurrencyType_ID(VAB_CurrencyType_ID);
            SetRate();
        }	//	setVAB_CurrencyType_ID

        /// <summary>
        /// set currency callout
        /// /// </summary>
        /// <param name="oldVAB_Currency_ID">old</param>
        /// <param name="newVAB_Currency_ID">new</param>
        /// <param name="windowNo">window no</param>
        public void SetVAB_Currency_ID(String oldVAB_Currency_ID,
               String newVAB_Currency_ID, int windowNo)
        {
            if (newVAB_Currency_ID == null || newVAB_Currency_ID.Length == 0)
            {
                return;
            }
            int VAB_Currency_ID = Utility.Util.GetValueOfInt(newVAB_Currency_ID);
            if (VAB_Currency_ID == 0)
            {
                return;
            }
            SetVAB_Currency_ID(VAB_Currency_ID);
            SetRate();
        }	//	setVAB_Currency_ID

        /// <summary>
        ///	Set Rate
        /// </summary>
        private void SetRate()
        {
            //  Source info
            int VAB_Currency_ID = GetVAB_Currency_ID();
            int VAB_CurrencyType_ID = GetVAB_CurrencyType_ID();
            if (VAB_Currency_ID == 0 || VAB_CurrencyType_ID == 0)
            {
                return;
            }
            DateTime? DateAcct = GetDateAcct();
            if (DateAcct == null)
            {
                DateAcct = DateTime.Now;// new Timestamp(System.currentTimeMillis());
            }
            //
            int VAB_AccountBook_ID = GetVAB_AccountBook_ID();
            MVABAccountBook a = MVABAccountBook.Get(GetCtx(), VAB_AccountBook_ID);
            int VAF_Client_ID = GetVAF_Client_ID();
            int VAF_Org_ID = GetVAF_Org_ID();

            Decimal? CurrencyRate = (Decimal?)MVABExchangeRate.GetRate(VAB_Currency_ID, a.GetVAB_Currency_ID(),
                DateAcct, VAB_CurrencyType_ID, VAF_Client_ID, VAF_Org_ID);
            log.Fine("rate = " + CurrencyRate);
            //if (CurrencyRate.Value == null)
            //{
            //    CurrencyRate = Env.ZERO;
            //}
            SetCurrencyRate(CurrencyRate.Value);
        }	//	setRate

        /// <summary>
        /// Get Journal Lines
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>Array of lines</returns>
        public MJournalLine[] GetLines(Boolean requery)
        {
            //ArrayList<MJournalLine> list = new ArrayList<MJournalLine>();
            List<MJournalLine> list = new List<MJournalLine>();
            String sql = "SELECT * FROM VAGL_JRNLLine WHERE VAGL_JRNL_ID=@Param1 ORDER BY Line";
            //PreparedStatement pstmt = null;
            SqlParameter[] Param = new SqlParameter[1];
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                //pstmt = DataBase.prepareStatement(sql, get_TrxName());
                //pstmt.setInt(1, getVAGL_JRNL_ID());
                Param[0] = new SqlParameter("@Param1", GetVAGL_JRNL_ID());

                idr = DataBase.DB.ExecuteReader(sql, Param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                //while (rs.next())
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MJournalLine(GetCtx(), dr, Get_TrxName()));
                }
                dt = null;
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                if (dt != null)
                {
                    dt = null;
                }
                log.Log(Level.SEVERE, "getLines", ex);
            }
            //
            MJournalLine[] retValue = new MJournalLine[list.Count];
            //list.toArray(retValue);
            retValue = list.ToArray();
            return retValue;
        }	//	getLines

        /// <summary>
        /// Copy Lines from other Journal
        /// </summary>
        /// <param name="fromJournal">Journal</param>
        /// <param name="dateAcct">date used - if null original</param>
        /// <param name="typeCR">type of copying (C)orrect=negate - (R)everse=flip dr/cr - otherwise just copy</param>
        /// <returns>number of lines copied</returns>
        public int CopyLinesFrom(MJournal fromJournal, DateTime? dateAcct, char typeCR)
        {
            if (IsProcessed() || fromJournal == null)
            {
                return 0;
            }
            int count = 0;
            int lineCount = 0;

            MJournalLine[] fromLines = fromJournal.GetLines(false);
            for (int i = 0; i < fromLines.Length; i++)
            {
                MJournalLine toLine = new MJournalLine(GetCtx(), 0, fromJournal.Get_TrxName());
                PO.CopyValues(fromLines[i], toLine, GetVAF_Client_ID(), GetVAF_Org_ID());
                toLine.SetVAGL_JRNL_ID(GetVAGL_JRNL_ID());
                //
                if (dateAcct != null)
                {
                    toLine.SetDateAcct(dateAcct);
                }
                //	Amounts
                if (typeCR == 'C')			//	correct
                {
                    // toLine.SetAmtSourceDr(fromLines[i].GetAmtSourceDr().negate());
                    toLine.SetAmtSourceDr(Decimal.Negate(fromLines[i].GetAmtSourceDr()));
                    toLine.SetAmtSourceCr(Decimal.Negate(fromLines[i].GetAmtSourceCr()));//.negate());
                }
                else if (typeCR == 'R')		//	reverse
                {
                    toLine.SetAmtSourceDr(fromLines[i].GetAmtSourceCr());
                    toLine.SetAmtSourceCr(fromLines[i].GetAmtSourceDr());
                }
                toLine.SetIsGenerated(true);
                toLine.SetProcessed(false);

                // // Set Orignal Document Reference on Reversal Document
                if (Get_ColumnIndex("IsReversal") > 0 && IsReversal())
                {
                    if (toLine.Get_ColumnIndex("ReversalDoc_ID") > 0)
                    {
                        toLine.SetReversalDoc_ID(fromLines[i].GetVAGL_JRNLLine_ID());
                    }
                }

                if (toLine.Save())
                {
                    count++;
                    lineCount += toLine.CopyLinesFrom(fromLines[i], toLine.GetVAGL_JRNLLine_ID(), typeCR);
                }
            }
            if (fromLines.Length != count)
            {
                log.Log(Level.SEVERE, "Line difference - JournalLines=" + fromLines.Length + " <> Saved=" + count);
            }

            return count;
        }	//	copyLinesFrom


        // Mainsh 18/7/2016...
        public int CopyLines(MJournal fromJournal, char typeCR)
        {
            DateTime? dateAcct = GetDateAcct();
            if (IsProcessed() || fromJournal == null)
            {
                return 0;
            }
            int count = 0;
            int lineCount = 0;

            MJournalLine[] fromLines = fromJournal.GetLines(false);
            for (int i = 0; i < fromLines.Length; i++)
            {
                MJournalLine toLine = new MJournalLine(GetCtx(), 0, fromJournal.Get_TrxName());
                PO.CopyValues(fromLines[i], toLine, GetVAF_Client_ID(), GetVAF_Org_ID());
                toLine.SetVAGL_JRNL_ID(GetVAGL_JRNL_ID());
                //
                //if (dateAcct != null)
                //{
                //    toLine.SetDateAcct(dateAcct);
                //}
                //	Amounts
                //if (typeCR == 'C')			//	correct
                //{
                //    // toLine.SetAmtSourceDr(fromLines[i].GetAmtSourceDr().negate());
                //    toLine.SetAmtSourceDr(Decimal.Negate(fromLines[i].GetAmtSourceDr()));
                //    toLine.SetAmtSourceCr(Decimal.Negate(fromLines[i].GetAmtSourceCr()));//.negate());
                //}
                //else if (typeCR == 'R')		//	reverse
                //{
                toLine.SetAmtSourceDr(fromLines[i].GetAmtSourceDr());
                toLine.SetAmtSourceCr(fromLines[i].GetAmtSourceCr());
                // }

                toLine.SetDescription(fromLines[i].GetDescription());
                toLine.SetVAB_Currency_ID(fromLines[i].GetVAB_Currency_ID());
                toLine.SetIsGenerated(true);
                toLine.SetProcessed(false);
                toLine.SetQty(fromLines[i].GetQty());
                toLine.SetElementType(fromLines[i].GetElementType());


                if (toLine.Save(fromJournal.Get_TrxName()))
                {
                    count++;
                    lineCount += toLine.CopyLinesFrom(fromLines[i], toLine.GetVAGL_JRNLLine_ID());
                }
            }
            if (fromLines.Length != count)
            {
                log.Log(Level.SEVERE, "Line difference - JournalLines=" + fromLines.Length + " <> Saved=" + count);
            }

            return count;
        }	//	copyLinesFrom
        //end

        //added by To Create Journal Lines Arpit Rai 15th Dec,2016
        public int CopyJLines(MJournal fromJournal, DateTime? dateAcct)
        {

            if (IsProcessed() || fromJournal == null)
            {
                return 0;
            }
            int count = 0;
            int lineCount = 0;

            MJournalLine[] fromLines = fromJournal.GetLines(false);
            for (int i = 0; i < fromLines.Length; i++)
            {
                MJournalLine toLine = new MJournalLine(GetCtx(), 0, fromJournal.Get_TrxName());
                PO.CopyValues(fromLines[i], toLine, GetVAF_Client_ID(), GetVAF_Org_ID());
                toLine.SetVAGL_JRNL_ID(GetVAGL_JRNL_ID());
                if (dateAcct != null)
                {
                    toLine.SetDateAcct(dateAcct);
                }
                else
                {
                    toLine.SetDateAcct(DateTime.Now);
                }
                toLine.SetAmtSourceDr(fromLines[i].GetAmtSourceDr());
                toLine.SetAmtSourceCr(fromLines[i].GetAmtSourceCr());
                toLine.SetDescription(fromLines[i].GetDescription());
                toLine.SetVAB_Currency_ID(fromLines[i].GetVAB_Currency_ID());
                toLine.SetIsGenerated(true);
                toLine.SetProcessed(false);
                toLine.SetQty(fromLines[i].GetQty());
                toLine.SetElementType(fromLines[i].GetElementType());
                if (toLine.Save(fromJournal.Get_TrxName()))
                {
                    count++;
                    lineCount += toLine.CopyLinesFrom(fromLines[i], toLine.GetVAGL_JRNLLine_ID());
                }
            }
            if (fromLines.Length != count)
            {
                log.Log(Level.SEVERE, "Line difference - JournalLines=" + fromLines.Length + " <> Saved=" + count);
            }

            return count;
        }
        //End Here


        /// <summary>
        /// Set Processed.
        /// </summary>
        /// <param name="processed">Propergate to Lines/Taxes</param>
        public new void SetProcessed(Boolean processed)
        {
            //super.setProcessed (processed);
            base.SetProcessed(processed);
            if (Get_ID() == 0)
            {
                return;
            }
            String sql = "UPDATE VAGL_JRNLLine SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE VAGL_JRNL_ID=" + GetVAGL_JRNL_ID();
            int noLine = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            log.Fine(processed + " - Lines=" + noLine);
        }	//	setProcessed


        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            //	Imported Journals may not have date
            if (GetDateDoc() == null)
            {
                if (GetDateAcct() == null)
                {
                    //SetDateDoc(new Timestamp(System.currentTimeMillis()));
                    SetDateDoc(DateTime.Now);
                }
                else
                {
                    SetDateDoc(GetDateAcct());
                }
            }
            if (GetDateAcct() == null)
            {
                SetDateAcct(GetDateDoc());
            }


            // set currency of selected accounting schema
            MVABAccountBook acctSchema = MVABAccountBook.Get(GetCtx(), GetVAB_AccountBook_ID());
            int VAB_Currency_ID = acctSchema.GetVAB_Currency_ID();
            SetVAB_Currency_ID(VAB_Currency_ID);
            SetCurrencyRate(1);

            // rounding control amount based on standard precision defined on currency
            SetControlAmt(Decimal.Round(GetControlAmt(), acctSchema.GetStdPrecision()));

            return true;
        }	//	beforeSave


        /// <summary>
        /// After Save.
        /// </summary>
        /// <param name="newRecord">true if new record</param>
        /// <param name="success">true if success</param>
        /// <returns>success</returns>
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (!success)
            {
                return success;
            }
            // create Assigned Accounting Schema Entry for assigned org only - in case of new record
            if (newRecord && !GetIsFormData())
            {
                CreateAssignAccountingSchemaRecord();
            }

            return UpdateBatch();
        }	//	afterSave

        /// <summary>
        /// This function is used to create record on Assign Accounting Schema Tab for Posting
        /// </summary>
        private void CreateAssignAccountingSchemaRecord()
        {
            // default conversion type 
            int C_DefaultCurrencyType_ID = MVABCurrencyType.GetDefault(GetVAF_Client_ID());

            // selected accounting schema currency
            int selectedAcctSchemaCurrency = MVABAccountBook.Get(GetCtx(), GetVAB_AccountBook_ID()).GetVAB_Currency_ID();

            // this query return a record of assigned org accounting schema having same chart of account
            String sql = @"SELECT DISTINCT CA.VAB_ACCOUNTBOOK_ID , Ca.VAB_Currency_ID , " + C_DefaultCurrencyType_ID + @" AS VAB_CurrencyType_ID , 
  CURRENCYRATE(" + selectedAcctSchemaCurrency + @" , Ca.VAB_Currency_ID , " + GlobalVariable.TO_DATE(GetDateAcct(), true) +
  @" , " + C_DefaultCurrencyType_ID + @" , " + GetVAF_Client_ID() + " , " + GetVAF_Org_ID() + @") AS Rate
FROM VAB_AccountBook CA INNER JOIN FRPT_AssignedOrg AO ON CA.VAB_AccountBook_ID = AO.VAB_AccountBook_ID
INNER JOIN VAB_AccountBook_Element ASE ON (CA.VAB_AccountBook_ID = ASE.VAB_AccountBook_ID AND ElementType = 'AC')
WHERE CA.ISACTIVE = 'Y' AND ASE.IsActive = 'Y' AND AO.IsActive = 'Y' AND AO.VAF_Org_ID IN(0," + GetVAF_Org_ID() + @")
AND ASE.VAB_Element_ID = (SELECT VAB_Element_ID FROM VAB_AccountBook_Element WHERE ElementType = 'AC' AND IsActive = 'Y' AND VAB_AccountBook_ID = " + GetVAB_AccountBook_ID() + @" )
AND CA.VAB_AccountBook_ID != " + GetVAB_AccountBook_ID();
            sql = MVAFRole.GetDefault(GetCtx()).AddAccessSQL(sql, "VAB_AccountBook", true, true);

            DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
            MAssignAcctSchema assignAcctSchema = null;
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    // create new record 
                    assignAcctSchema = new MAssignAcctSchema(GetCtx(), 0, Get_Trx());
                    assignAcctSchema.SetVAF_Client_ID(GetVAF_Client_ID());
                    assignAcctSchema.SetVAF_Org_ID(GetVAF_Org_ID());
                    assignAcctSchema.SetVAGL_JRNL_ID(GetVAGL_JRNL_ID());
                    assignAcctSchema.SetVAB_AccountBook_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_ACCOUNTBOOK_ID"]));
                    assignAcctSchema.SetVAB_Currency_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Currency_ID"]));
                    assignAcctSchema.SetVAB_CurrencyType_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_CurrencyType_ID"]));
                    assignAcctSchema.SetCurrencyRate(Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Rate"]));
                    if (!assignAcctSchema.Save())
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        log.Severe(@"Error Occured when try to save record on Assigned Accounting Schema.
                                        Error Name : " + (pp != null && !String.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : ""));
                    }
                }
            }
        }

        /// <summary>
        /// setter property for checking data to be manual creation of through process
        /// </summary>
        /// <param name="isformData">true, if data saved through process</param>
        public void SetIsFormData(bool isformData)
        {
            _isSaveFromForm = isformData;
        }

        /// <summary>
        /// getter property for checking data saved from procss or manual
        /// </summary>
        /// <returns></returns>
        public bool GetIsFormData()
        {
            return _isSaveFromForm;
        }

        /// <summary>
        /// After Delete
        /// </summary>
        /// <param name="success"> true if deleted</param>
        /// <returns> true if success</returns>
        protected override Boolean AfterDelete(Boolean success)
        {
            if (!success)
            {
                return success;
            }
            return UpdateBatch();
        }	//	afterDelete

        /// <summary>
        /// Update Batch total
        /// </summary>
        /// <returns>true if ok</returns>
        private Boolean UpdateBatch()
        {
            // Manish 18/7/2016 ..  check VAGL_BatchJRNL_id is in window or not.
            string sqlquery = @"SELECT VAGL_BatchJRNL_id FROM VAGL_JRNL WHERE VAGL_JRNL_id =" + GetVAGL_JRNL_ID();
            int nooo = Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sqlquery, null, null));
            if (nooo <= 0)
            {
                return true;
            }
            // end 18/7/2016

            String sql = "UPDATE VAGL_BatchJRNL jb"
                + " SET (TotalDr, TotalCr) = (SELECT SUM(TotalDr), SUM(TotalCr)" //jz hard coded ", "
                    + " FROM VAGL_JRNL j WHERE j.IsActive='Y' AND jb.VAGL_BatchJRNL_ID=j.VAGL_BatchJRNL_ID) "
                + "WHERE VAGL_BatchJRNL_ID=" + GetVAGL_BatchJRNL_ID();
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 1)
            {
                log.Warning("afterSave - Update Batch #" + no);
            }
            return no == 1;
        }	//	updateBatch


        /// <summary>
        ///	Process document
        /// </summary>
        /// <param name="processAction"> document action</param>
        /// <returns>true if performed</returns>
        public Boolean ProcessIt(String processAction)
        {
            m_processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }	//	process

        /**	Process Message 			*/
        private String m_processMsg = null;
        /**	Just Prepared Flag			*/
        private Boolean m_justPrepared = false;

        /// <summary>
        ///	Unlock Document.
        /// </summary>
        /// <returns> true if success </returns>
        public Boolean UnlockIt()
        {
            log.Info(ToString());
            SetProcessing(false);
            return true;
        }	//	unlockIt

        /// <summary>
        /// Invalidate Document
        /// </summary>
        /// <returns> true if success </returns>
        public Boolean InvalidateIt()
        {
            log.Info(ToString());
            return true;
        }	//	invalidateIt

        /// <summary>
        /// Prepare Document
        /// </summary>
        /// <returns>new status (In Progress or Invalid) </returns>
        public String PrepareIt()
        {
            log.Info(ToString());
            m_processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (m_processMsg != null)
            {
                return DocActionVariables.STATUS_INVALID;
            }
            MDocType dt = MDocType.Get(GetCtx(), GetVAB_DocTypes_ID());

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType(), GetVAF_Org_ID()))
            {
                m_processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // JID_0521 - Restrict if debit and credit amount is not equal.-Mohit-12-jun-2019.
            if (GetTotalCr() != GetTotalDr())
            {
                m_processMsg = Msg.GetMsg(GetCtx(), "DBAndCRAmtNotEqual");
                return DocActionVariables.STATUS_INVALID;
            }

            //	Lines
            MJournalLine[] lines = GetLines(true);
            if (lines.Length == 0)
            {
                m_processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }

            //Manish 18/7/2016 .. Because if line dimention (Amount) column sum is not equals to debit or credit value complete process will not done. 
            Decimal journalDRAndCR = 0;
            string getlinevalues = "SELECT NVL(ElementType,null) AS ElementType,AmtSourceDr,AmtAcctCr,AmtSourceCr,VAGL_JRNLLine_ID FROM VAGL_JRNLLine where VAGL_JRNL_ID=" + Get_Value("VAGL_JRNL_ID");
            DataSet dts = DB.ExecuteDataset(getlinevalues, null, null);

            if (dts != null && dts.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dts.Tables[0].Rows.Count; i++)
                {
                    journalDRAndCR = 0;

                    if (dts.Tables[0].Rows[i]["ElementType"].ToString() == "")
                    {
                        continue;
                    }

                    if (Util.GetValueOfDecimal(dts.Tables[0].Rows[i]["AmtSourceDr"]) > 0)
                    {
                        journalDRAndCR = Util.GetValueOfDecimal(dts.Tables[0].Rows[i]["AmtSourceDr"]);
                    }
                    else
                    {
                        if (Util.GetValueOfDecimal(dts.Tables[0].Rows[i]["AmtSourceCr"]) > 0)
                        {
                            journalDRAndCR = Util.GetValueOfDecimal(dts.Tables[0].Rows[i]["AmtSourceCr"]);
                        }
                    }

                    string getlineval = "SELECT SUM(amount) FROM VAGL_LineDimension where VAGL_JRNLLine_ID=" + Convert.ToInt32(dts.Tables[0].Rows[i]["VAGL_JRNLLine_ID"]);
                    Decimal count = Util.GetValueOfDecimal(DB.ExecuteScalar(getlineval));

                    if (journalDRAndCR != count)
                    {
                        m_processMsg = "@AmountNotMatch@";
                        return DocActionVariables.STATUS_INVALID;
                    }
                }
            }





            //	Add up Amounts
            Decimal AmtSourceDr = Env.ZERO;
            Decimal AmtSourceCr = Env.ZERO;
            for (int i = 0; i < lines.Length; i++)
            {
                MJournalLine line = lines[i];
                if (!IsActive())
                {
                    continue;
                }
                //
                if (line.IsDocControlled())
                {
                    m_processMsg = "@DocControlledError@ - @Line@=" + line.GetLine()
                        + " - " + line.GetAccountElementValue();
                    return DocActionVariables.STATUS_INVALID;
                }
                //
                AmtSourceDr = Decimal.Add(AmtSourceDr, line.GetAmtAcctDr());
                AmtSourceCr = Decimal.Add(AmtSourceCr, line.GetAmtAcctCr());
            }
            SetTotalDr(AmtSourceDr);
            SetTotalCr(AmtSourceCr);

            //	Control Amount
            if (Env.ZERO.CompareTo(GetControlAmt()) != 0
                && GetControlAmt().CompareTo(GetTotalDr()) < 0)
            {
                m_processMsg = "@ControlAmtError@";
                return DocActionVariables.STATUS_INVALID;
            }

            //	Unbalanced Jornal & Not Suspense
            if (AmtSourceDr.CompareTo(AmtSourceCr) != 0)
            {
                MVABAccountBookGL gl = MVABAccountBookGL.Get(GetCtx(), GetVAB_AccountBook_ID());
                if (gl == null || !gl.IsUseSuspenseBalancing())
                {
                    m_processMsg = "@UnbalancedJornal@";
                    return DocActionVariables.STATUS_INVALID;
                }
            }

            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            m_justPrepared = true;
            return DocActionVariables.STATUS_INPROGRESS;
        }	//	prepareIt

        /// <summary>
        /// Approve Document
        /// </summary>
        /// <returns>true if success </returns>
        public Boolean ApproveIt()
        {
            log.Info(ToString());
            SetIsApproved(true);
            return true;
        }	//	approveIt

        /// <summary>
        /// Reject Approval
        /// </summary>
        /// <returns> true if success </returns>
        public Boolean RejectIt()
        {
            log.Info(ToString());
            SetIsApproved(false);
            return true;
        }	//	rejectIt

        /// <summary>
        ///	Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public String CompleteIt()
        {
            //	Re-Check
            if (!m_justPrepared)
            {
                String status = PrepareIt();
                if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                {
                    return status;
                }
            }

            // JID_1290: Set the document number from completed document sequence after completed (if needed)
            SetCompletedDocumentNo();

            //	Implicit Approval
            if (!IsApproved())
            {
                ApproveIt();
            }
            log.Info(ToString());
            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                m_processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }
            //
            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            return DocActionVariables.STATUS_COMPLETED;
        }	//	completeIt

        /// <summary>
        /// Set the document number from Completed Document Sequence after completed
        /// </summary>
        private void SetCompletedDocumentNo()
        {
            // if Reversal document then no need to get Document no from Completed sequence
            if (Get_ColumnIndex("IsReversal") > 0 && IsReversal())
            {
                return;
            }

            MDocType dt = MDocType.Get(GetCtx(), GetVAB_DocTypes_ID());

            // if Overwrite Date on Complete checkbox is true.
            if (dt.IsOverwriteDateOnComplete())
            {
                SetDateDoc(DateTime.Now.Date);
                if (GetDateAcct().Value.Date < GetDateDoc().Value.Date)
                {
                    SetDateAcct(GetDateDoc());

                    //	Std Period open?
                    if (!MPeriod.IsOpen(GetCtx(), GetDateDoc(), dt.GetDocBaseType(), GetVAF_Org_ID()))
                    {
                        throw new Exception("@PeriodClosed@");
                    }
                }
            }

            // if Overwrite Sequence on Complete checkbox is true.
            if (dt.IsOverwriteSeqOnComplete())
            {
                // Set Drafted Document No into Temp Document No.
                if (Get_ColumnIndex("TempDocumentNo") > 0)
                {
                    SetTempDocumentNo(GetDocumentNo());
                }

                // Get current next from Completed document sequence defined on Document type
                String value = MVAFRecordSeq.GetDocumentNo(GetVAB_DocTypes_ID(), Get_TrxName(), GetCtx(), true, this);
                if (value != null)
                {
                    SetDocumentNo(value);
                }
            }
        }

        /// <summary>
        ///	Void Document.
        /// </summary>
        /// <returns>true if success </returns>
        public Boolean VoidIt()
        {
            log.Info(ToString());
            if (DOCSTATUS_Drafted.Equals(GetDocStatus())
                || DOCSTATUS_Invalid.Equals(GetDocStatus()))
            {
                //Bug 124 in Devops :  set Amount as ZERO on GL journal , 
                DB.ExecuteQuery(@"Update VAGL_JRNL SET TotalDr = 0, TotalCr = 0
                                    WHERE VAGL_JRNL_ID =  " + GetVAGL_JRNL_ID(), null, Get_Trx());

                //Bug 124 in Devops :  set Amount as ZERO on GL journal Line, 
                DB.ExecuteQuery(@"Update VAGL_JRNLLine SET AmtSourceDr = 0, AmtSourceCr = 0, AmtAcctDr = 0, AmtAcctCr = 0 
                                    WHERE VAGL_JRNL_ID =  " + GetVAGL_JRNL_ID(), null, Get_Trx());

                //Bug 124 in Devops :  set Amount as ZERO on GL journal Line, 
                DB.ExecuteQuery(@"Update VAGL_LineDimension  SET Amount = 0
                                    WHERE VAGL_JRNLLine_ID IN (SELECT VAGL_JRNLLine_ID FROM VAGL_JRNLLine 
                                    WHERE VAGL_JRNL_ID =   " + GetVAGL_JRNL_ID() + ") ", null, Get_Trx());

                SetProcessed(true);
                SetDocAction(DOCACTION_None);
                return true;
            }
            return false;
        }	//	voidIt

        /// <summary>
        /// Close Document.Cancel not delivered Qunatities
        /// </summary>
        /// <returns>true if success </returns>
        public Boolean CloseIt()
        {
            log.Info(ToString());
            if (DOCSTATUS_Completed.Equals(GetDocStatus()))
            {
                SetProcessed(true);
                SetDocAction(DOCACTION_None);
                return true;
            }
            return false;
        }	//	closeIt

        /// <summary>
        /// Reverse Correction (in same batch).As if nothing happened - same date
        /// </summary>
        /// <returns> true if success </returns>
        public Boolean ReverseCorrectIt()
        {
            return ReverseCorrectIt(GetVAGL_BatchJRNL_ID()) != null;
        }	//	reverseCorrectIt

        /// <summary>
        /// Reverse Correction.As if nothing happened - same date
        /// </summary>
        /// <param name="VAGL_BatchJRNL_ID">batch</param>
        /// <returns>reversed journal or null</returns>
        public MJournal ReverseCorrectIt(int VAGL_BatchJRNL_ID)
        {
            log.Info(ToString());
            //If any journal line is allocated then the Journal will not Reverted
            string sql = "SELECT Count(IsAllocated) AS IsAllocated FROM VAGL_JRNLLine WHERE VAGL_JRNL_ID = " + GetVAGL_JRNL_ID() + " AND IsAllocated = 'Y'";
            if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) > 0)
            {
                m_processMsg = Msg.GetMsg(GetCtx(), "DeleteAllowcationFirst");
                return null;
            }
            //	Journal
            MJournal reverse = new MJournal(this);
            reverse.SetVAGL_BatchJRNL_ID(VAGL_BatchJRNL_ID);
            reverse.SetDateDoc(GetDateDoc());
            reverse.SetVAB_YearPeriod_ID(GetVAB_YearPeriod_ID());
            reverse.SetDateAcct(GetDateAcct());
            reverse.SetIsFormData(true);
            //	Reverse indicator
            String description = reverse.GetDescription();
            if (description == null)
            {
                description = "** " + GetDocumentNo() + " **";
            }
            else
            {
                description += " ** " + GetDocumentNo() + " **";
            }
            reverse.SetDescription(description);

            if (reverse.Get_ColumnIndex("ReversalDoc_ID") > 0 && reverse.Get_ColumnIndex("IsReversal") > 0)
            {
                // set Reversal property for identifying, record is reversal or not during saving or for other actions
                reverse.SetIsReversal(true);
                // Set Orignal Document Reference
                reverse.SetReversalDoc_ID(GetVAGL_JRNL_ID());
            }

            // for reversal document set Temp Document No to empty
            if (reverse.Get_ColumnIndex("TempDocumentNo") > 0)
            {
                reverse.SetTempDocumentNo("");
            }

            if (!reverse.Save())
            {
                return null;
            }
            else
            {
                // Create record on Assign Accounting Schema Tab if any
                _processMsg = CopyAssignAccountingSchema(GetVAGL_JRNL_ID(), reverse.GetVAGL_JRNL_ID(), reverse.Get_TrxName());
                if (!String.IsNullOrEmpty(_processMsg))
                {
                    return null;
                }
            }

            //	Lines
            if (reverse.CopyLinesFrom(this, null, 'C') > 0)
            {
                if (!reverse.ProcessIt(DocActionVariables.ACTION_COMPLETE))
                {
                    _processMsg = "Reversal ERROR: " + reverse.GetProcessMsg();
                    return null;
                }
                reverse.SetProcessing(false);
                reverse.SetDocStatus(DOCSTATUS_Reversed);
                reverse.SetDocAction(DOCACTION_None);
                reverse.Save(Get_TrxName());
            }

            //JID_0889: show on void full message Reversal Document created
            _processMsg = Msg.GetMsg(GetCtx(), "VIS_DocumentReversed") + reverse.GetDocumentNo();
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return reverse;
        }	//	reverseCorrectionIt

        /// <summary>
        /// This process is used to copy assign accounting schema record from olg Gl Journal record to new journal record
        /// </summary>
        /// <param name="oldjournal_Id">old GL journal ID</param>
        /// <param name="newJournal_Id">new GL Journal ID</param>
        /// <param name="trxName">current Transaction</param>
        /// <returns>if not saved, then return error message</returns>
        public String CopyAssignAccountingSchema(int oldjournal_Id, int newJournal_Id, Trx trxName)
        {
            if (oldjournal_Id <= 0 && newJournal_Id <= 0)
            {
                return "";
            }
            MAssignAcctSchema[] fromLines = GetAssignAcctSchemaLines();
            for (int i = 0; i < fromLines.Length; i++)
            {
                MAssignAcctSchema toAssignAcctSchemaLine = new MAssignAcctSchema(GetCtx(), 0, trxName);
                PO.CopyValues(fromLines[i], toAssignAcctSchemaLine, GetVAF_Client_ID(), GetVAF_Org_ID());
                toAssignAcctSchemaLine.SetVAGL_JRNL_ID(newJournal_Id);
                if (!toAssignAcctSchemaLine.Save())
                {
                    pp = VLogger.RetrieveError();
                    if (!String.IsNullOrEmpty(pp.GetName()))
                        _processMsg = Msg.GetMsg(GetCtx(), "NotCreatedAssignAcctSchema") + ", " + pp.GetName();
                    else
                        _processMsg = Msg.GetMsg(GetCtx(), "NotCreatedAssignAcctSchema"); //Could not create Assign Accounting Schmea
                    return _processMsg;
                }
            }
            return "";
        }

        /// <summary>
        /// Get lines of Assign Accounting Schema against GL journal
        /// </summary>
        /// <returns>array of assign accounting schema</returns>
        public MAssignAcctSchema[] GetAssignAcctSchemaLines()
        {
            List<MAssignAcctSchema> list = new List<MAssignAcctSchema>();
            String sql = "SELECT * FROM VAGL_AssignAcctSchema WHERE VAGL_JRNL_ID=@Param1";
            SqlParameter[] Param = new SqlParameter[1];
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                Param[0] = new SqlParameter("@Param1", GetVAGL_JRNL_ID());

                idr = DataBase.DB.ExecuteReader(sql, Param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MAssignAcctSchema(GetCtx(), dr, Get_TrxName()));
                }
                dt = null;
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                if (dt != null)
                {
                    dt = null;
                }
                log.Log(Level.SEVERE, "getLines", ex);
            }
            //
            MAssignAcctSchema[] retValue = new MAssignAcctSchema[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Reverse Accrual (sane batch).	Flip Dr/Cr - Use Today's date
        /// </summary>
        /// <returns> true if success </returns>
        public Boolean ReverseAccrualIt()
        {
            return ReverseAccrualIt(GetVAGL_BatchJRNL_ID()) != null;
        }	//	reverseAccrualIt

        /// <summary>
        ///	Reverse Accrual.	Flip Dr/Cr - Use Today's date
        /// </summary>
        /// <param name="VAGL_BatchJRNL_ID">reversal batch</param>
        /// <returns> journal or null </returns>
        public MJournal ReverseAccrualIt(int VAGL_BatchJRNL_ID)
        {
            log.Info(ToString());
            //	Journal
            MJournal reverse = new MJournal(this);
            reverse.SetVAGL_BatchJRNL_ID(VAGL_BatchJRNL_ID);
            reverse.SetDateDoc(DateTime.Now);
            reverse.Set_ValueNoCheck("VAB_YearPeriod_ID", null);		//	reset
            reverse.SetDateAcct(reverse.GetDateDoc());
            //	Reverse indicator
            String description = reverse.GetDescription();
            if (description == null)
            {
                description = "** " + GetDocumentNo() + " **";
            }
            else
            {
                description += " ** " + GetDocumentNo() + " **";
            }
            reverse.SetDescription(description);
            if (!reverse.Save())
            {
                return null;
            }

            //	Lines
            reverse.CopyLinesFrom(this, reverse.GetDateAcct(), 'R');
            //
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return reverse;
        }	//	reverseAccrualIt

        /// <summary>
        /// Re-activate
        /// </summary>
        /// <returns>true if success </returns>
        public Boolean ReActivateIt()
        {
            log.Info(ToString());
            return false;
        }	//	reActivateIt


        /// <summary>
        /// Get Summary
        /// </summary>
        /// <returns>Summary of Document</returns>
        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDocumentNo());
            //	: Total Lines = 123.00 (#1)
            sb.Append(": ")
                .Append(Msg.Translate(GetCtx(), "TotalDr")).Append("=").Append(GetTotalDr())
                .Append(" ")
                .Append(Msg.Translate(GetCtx(), "TotalCR")).Append("=").Append(GetTotalCr())
                .Append(" (#").Append(GetLines(false).Length).Append(")");
            //	 - Description
            if (GetDescription() != null && GetDescription().Length > 0)
            {
                sb.Append(" - ").Append(GetDescription());
            }
            return sb.ToString();
        }	//	getSummary

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns> info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MJournal[");
            sb.Append(Get_ID()).Append(",").Append(GetDescription())
                .Append(",DR=").Append(GetTotalDr())
                .Append(",CR=").Append(GetTotalCr())
                .Append("]");
            return sb.ToString();
        }	//	toString

        /// <summary>
        /// Get Document Info

        /// </summary>
        /// <returns>document info (untranslated)</returns>
        public String GetDocumentInfo()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetVAB_DocTypes_ID());
            return dt.GetName() + " " + GetDocumentNo();
        }	//	getDocumentInfo

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF()
        {
            try
            {
                string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                                    + ".pdf"; //.pdf
                string filePath = Path.GetTempPath() + fileName;

                FileInfo temp = new FileInfo(filePath);
                if (!temp.Exists)
                {
                    return CreatePDF(temp);
                }
            }
            catch (Exception e)
            {
                log.Severe("Could not create PDF - " + e.Message);
            }
            return null;
        }
        /**
         * 	Create PDF file
         *	@param file output file
         *	@return file if success
         */
        public FileInfo CreatePDF(FileInfo file)
        {
            //	ReportEngine re = ReportEngine.get (getCtx(), ReportEngine.INVOICE, getVAB_Invoice_ID());
            //	if (re == null)
            return null;
            //	return re.getPDF(file);
        }	//	createPDF


        /// <summary>
        /// Get Process Message
        /// </summary>
        /// <returns>clear text error message</returns>
        public String GetProcessMsg()
        {
            return m_processMsg;
        }	//	getProcessMsg

        /// <summary>
        /// Get Document Owner (Responsible)
        /// </summary>
        /// <returns>VAF_UserContact_ID (Created)</returns>
        public int GetDoc_User_ID()
        {
            return GetCreatedBy();
        }	//	getDoc_User_ID

        /// <summary>
        /// Get Document Approval Amount
        /// </summary>
        /// <returns>DR amount</returns>
        public Decimal GetApprovalAmt()
        {
            return GetTotalDr();
        }

        #region DocAction Members


        public Env.QueryParams GetLineOrgsQueryInfo()
        {
            return null;
        }

        public DateTime? GetDocumentDate()
        {
            return null;
        }

        public string GetDocBaseType()
        {
            return null;
        }


        public void SetProcessMsg(string processMsg)
        {
            _processMsg = processMsg;
        }



        #endregion
        //Added by Arpit on 15th Dec,2016
        public static MJournal CopyFrom(Ctx ctx, int VAGL_JRNL_ID, DateTime? dateDoc, Trx trxName)
        {
            MJournal from = new MJournal(ctx, VAGL_JRNL_ID, trxName);
            if (from.GetVAGL_JRNL_ID() == 0)
            {
                throw new ArgumentException("From Journal Batch not found VAGL_BatchJRNL_ID=" + VAGL_JRNL_ID);
            }
            //
            MJournal to = new MJournal(ctx, 0, trxName);
            PO.CopyValues(from, to, from.GetVAF_Client_ID(), from.GetVAF_Org_ID());
            to.Set_ValueNoCheck("DocumentNo", null);
            // to.Set_ValueNoCheck("VAB_YearPeriod_ID", null);
            to.SetDateAcct(dateDoc);
            to.SetDateDoc(dateDoc);
            to.SetDocStatus(DOCSTATUS_Drafted);
            to.SetDocAction(DOCACTION_Complete);
            to.SetIsApproved(false);
            to.SetProcessed(false);
            from.GetType();
            //
            if (!to.Save())
            {
                throw new Exception("Could not create GL Journal ");
            }

            if (to.CopyJLines(from, dateDoc) == 0)
            {
                throw new Exception("Could not create GL Journal Details");
            }

            return to;
        }	//	copyFrom
        //End Here
    }	//	MJournal

}
