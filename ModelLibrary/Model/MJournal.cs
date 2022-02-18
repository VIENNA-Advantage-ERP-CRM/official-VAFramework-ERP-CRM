/********************************************************
 * Class Name     : MJournal
 * Purpose        : GL Journal Model
 * Class Used     : X_GL_Journal,DocAction 
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
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MJournal : X_GL_Journal, DocAction
    {

        /** Is record save from GL Voucher form **/
        private bool _isSaveFromForm;
        //	Process Message 			
        private String _processMsg = null;
        private ValueNamePair pp = null;
        // Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MJournal).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="GL_Journal_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MJournal(Ctx ctx, int GL_Journal_ID, Trx trxName)
            : base(ctx, GL_Journal_ID, trxName)
        {
            //super (ctx, GL_Journal_ID, trxName);
            if (GL_Journal_ID == 0)
            {
                //	setGL_Journal_ID (0);		//	PK
                //	setC_AcctSchema_ID (0);
                //	setC_Currency_ID (0);
                //	setC_DocType_ID (0);
                //	setC_Period_ID (0);
                //
                SetCurrencyRate(Env.ONE);
                //	setC_ConversionType_ID(0);
                SetDateAcct(DateTime.Now.Date);// Timestamp(Comm.currentTimeMillis()));
                SetDateDoc((DateTime.Now.Date));//new Timestamp(System.currentTimeMillis()));
                //	setDescription (null);
                SetDocAction(DOCACTION_Complete);
                SetDocStatus(DOCSTATUS_Drafted);
                //	setDocumentNo (null);
                //	setGL_Category_ID (0);
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
            SetGL_JournalBatch_ID(parent.GetGL_JournalBatch_ID());
            SetC_DocType_ID(parent.GetC_DocType_ID());
            SetPostingType(parent.GetPostingType());
            //
            SetDateDoc(parent.GetDateDoc());
            SetC_Period_ID(parent.GetC_Period_ID());
            SetDateAcct(parent.GetDateAcct());
            SetC_Currency_ID(parent.GetC_Currency_ID());
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
            SetGL_JournalBatch_ID(original.GetGL_JournalBatch_ID());
            //
            SetC_AcctSchema_ID(original.GetC_AcctSchema_ID());
            SetGL_Budget_ID(original.GetGL_Budget_ID());
            SetGL_Category_ID(original.GetGL_Category_ID());
            SetPostingType(original.GetPostingType());
            SetDescription(original.GetDescription());
            SetC_DocType_ID(original.GetC_DocType_ID());
            SetControlAmt(original.GetControlAmt());
            //
            SetC_Currency_ID(original.GetC_Currency_ID());
            SetC_ConversionType_ID(original.GetC_ConversionType_ID());
            SetCurrencyRate(original.GetCurrencyRate());

            //	setDateDoc(original.getDateDoc());
            //	setDateAcct(original.getDateAcct());
            //	setC_Period_ID(original.getC_Period_ID());
        }	//	MJournal


        /// <summary>
        /// Overwrite Client/Org if required
        /// </summary>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID"> org</param>
        public new void SetClientOrg(int AD_Client_ID, int AD_Org_ID)
        {
            //super.setClientOrg(AD_Client_ID, AD_Org_ID);
            base.SetClientOrg(AD_Client_ID, AD_Org_ID);
        }	//	setClientOrg

        /// <summary>
        /// Get Period
        /// </summary>
        /// <returns>period or null</returns>
        public MPeriod GetPeriod()
        {
            int C_Period_ID = GetC_Period_ID();
            if (C_Period_ID != 0)
            {
                return MPeriod.Get(GetCtx(), C_Period_ID);
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
        /// <param name="oldC_Period_ID">old</param>
        /// <param name="newC_Period_ID">new</param>
        /// <param name="windowNo">window no</param>
        public void SetC_Period_ID(String oldC_Period_ID,
               String newC_Period_ID, int windowNo)
        {
            if (newC_Period_ID == null || newC_Period_ID.Length == 0)
            {
                return;
            }
            int C_Period_ID = Utility.Util.GetValueOfInt(newC_Period_ID);
            if (C_Period_ID == 0)
            {
                return;
            }
            SetC_Period_ID(C_Period_ID);
        }	//	setC_Period_ID

        /// <summary>
        /// Set Accounting Date.Set also Period if not set earlier
        /// </summary>
        /// <param name="DateAcct">date</param>
        public void SetDateAcct(DateTime DateAcct)
        {
            //super.setDateAcct(DateAcct);
            base.SetDateAcct(DateAcct);
            if (GetC_Period_ID() != 0)	//	previously set
            {
                SetRate();
                return;
            }
            int C_Period_ID = MPeriod.GetC_Period_ID(GetCtx(), DateAcct);
            if (C_Period_ID == 0)
            {
                log.Warning("Period not found");
            }
            else
            {
                base.SetC_Period_ID(C_Period_ID);
                SetRate();
            }
        }	//	setDateAcct

        /// <summary>
        /// Set Period
        /// </summary>
        /// <param name="C_Period_ID">period</param>
        public new void SetC_Period_ID(int C_Period_ID)
        {
            //super.setC_Period_ID(C_Period_ID);
            base.SetC_Period_ID(C_Period_ID);
            if (C_Period_ID == 0)
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
        }	//	setC_Period_ID


        /// <summary>
        /// Set Currency Info
        /// </summary>
        /// <param name="C_Currency_ID">currenct</param>
        /// <param name="C_ConversionType_ID">type</param>
        /// <param name="CurrencyRate">rate</param>
        public void SetCurrency(int C_Currency_ID, int C_ConversionType_ID, Decimal CurrencyRate)
        {
            if (C_Currency_ID != 0)
            {
                SetC_Currency_ID(C_Currency_ID);
            }
            if (C_ConversionType_ID != 0)
            {
                SetC_ConversionType_ID(C_ConversionType_ID);
            }
            if (CurrencyRate.CompareTo(Env.ZERO) == 0)
            {
                SetCurrencyRate(CurrencyRate);
            }
        }	//	setCurrency


        /// <summary>
        /// set callout
        /// </summary>
        /// <param name="oldC_ConversionType_ID">old</param>
        /// <param name="newC_ConversionType_ID">new</param>
        /// <param name="windowNo">window no</param>
        public void SetC_ConversionType_ID(String oldC_ConversionType_ID,
               String newC_ConversionType_ID, int windowNo)
        {
            if (newC_ConversionType_ID == null || newC_ConversionType_ID.Length == 0)
            {
                return;
            }
            int C_ConversionType_ID = Utility.Util.GetValueOfInt(newC_ConversionType_ID);
            if (C_ConversionType_ID == 0)
            {
                return;
            }
            SetC_ConversionType_ID(C_ConversionType_ID);
            SetRate();
        }	//	setC_ConversionType_ID

        /// <summary>
        /// set currency callout
        /// /// </summary>
        /// <param name="oldC_Currency_ID">old</param>
        /// <param name="newC_Currency_ID">new</param>
        /// <param name="windowNo">window no</param>
        public void SetC_Currency_ID(String oldC_Currency_ID,
               String newC_Currency_ID, int windowNo)
        {
            if (newC_Currency_ID == null || newC_Currency_ID.Length == 0)
            {
                return;
            }
            int C_Currency_ID = Utility.Util.GetValueOfInt(newC_Currency_ID);
            if (C_Currency_ID == 0)
            {
                return;
            }
            SetC_Currency_ID(C_Currency_ID);
            SetRate();
        }	//	setC_Currency_ID

        /// <summary>
        ///	Set Rate
        /// </summary>
        private void SetRate()
        {
            //  Source info
            int C_Currency_ID = GetC_Currency_ID();
            int C_ConversionType_ID = GetC_ConversionType_ID();
            if (C_Currency_ID == 0 || C_ConversionType_ID == 0)
            {
                return;
            }
            DateTime? DateAcct = GetDateAcct();
            if (DateAcct == null)
            {
                DateAcct = DateTime.Now;// new Timestamp(System.currentTimeMillis());
            }
            //
            int C_AcctSchema_ID = GetC_AcctSchema_ID();
            MAcctSchema a = MAcctSchema.Get(GetCtx(), C_AcctSchema_ID);
            int AD_Client_ID = GetAD_Client_ID();
            int AD_Org_ID = GetAD_Org_ID();

            Decimal? CurrencyRate = (Decimal?)MConversionRate.GetRate(C_Currency_ID, a.GetC_Currency_ID(),
                DateAcct, C_ConversionType_ID, AD_Client_ID, AD_Org_ID);
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
            String sql = "SELECT * FROM GL_JournalLine WHERE GL_Journal_ID=@Param1 ORDER BY Line";
            //PreparedStatement pstmt = null;
            SqlParameter[] Param = new SqlParameter[1];
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                //pstmt = DataBase.prepareStatement(sql, get_TrxName());
                //pstmt.setInt(1, getGL_Journal_ID());
                Param[0] = new SqlParameter("@Param1", GetGL_Journal_ID());

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

            int precision = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT stdprecision FROM C_Currency WHERE C_Currency_ID = 
                            ( SELECT C_Currency_ID FROM GL_journal WHERE GL_Journal_ID=" + GetGL_Journal_ID() + " )", null, Get_Trx()));
            int count = 0;
            int lineCount = 0;
            MJournalLine toLine = null;
            MJournalLine[] fromLines = fromJournal.GetLines(false);
            for (int i = 0; i < fromLines.Length; i++)
            {
                toLine = new MJournalLine(GetCtx(), 0, fromJournal.Get_TrxName());
                PO.CopyValues(fromLines[i], toLine, GetAD_Client_ID(), GetAD_Org_ID());
                toLine.SetGL_Journal_ID(GetGL_Journal_ID());
                toLine.m_precision = precision;
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
                        toLine.SetReversalDoc_ID(fromLines[i].GetGL_JournalLine_ID());
                        toLine._isReverseByProcess = true;
                    }
                }
                if (Get_ColumnIndex("ConditionalFlag") >= 0 && Util.GetValueOfString(Get_Value("ConditionalFlag")).Equals("00"))
                {
                    toLine._isReverseByProcess = true;
                }

                if (!toLine.Save())
                {
                    String error = "";
                    ValueNamePair pp = VLogger.RetrieveError();
                    if (pp != null)
                    {
                        error = pp.GetName();
                        if (String.IsNullOrEmpty(error))
                        {
                            error = pp.GetValue();
                        }
                        _log.Log(Level.SEVERE, String.IsNullOrEmpty(error) ? "Could not create Journal Line" : Msg.GetMsg(toLine.GetCtx(), error));
                    }
                    SetProcessMsg(String.IsNullOrEmpty(error) ? "Could not create Journal Line" : Msg.GetMsg(toLine.GetCtx(), error));
                }
                else
                {
                    count++;
                    if (!String.IsNullOrEmpty(toLine.GetElementType()))
                    {
                        lineCount += toLine.CopyLinesFrom(fromLines[i], toLine.GetGL_JournalLine_ID(), typeCR);
                    }
                }
            }

            // Update Header
            toLine.UpdateJournalTotal();

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
                PO.CopyValues(fromLines[i], toLine, GetAD_Client_ID(), GetAD_Org_ID());
                toLine.SetGL_Journal_ID(GetGL_Journal_ID());
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
                toLine.SetC_Currency_ID(fromLines[i].GetC_Currency_ID());
                toLine.SetIsGenerated(true);
                toLine.SetProcessed(false);
                toLine.SetQty(fromLines[i].GetQty());
                toLine.SetElementType(fromLines[i].GetElementType());


                if (toLine.Save(fromJournal.Get_TrxName()))
                {
                    count++;
                    lineCount += toLine.CopyLinesFrom(fromLines[i], toLine.GetGL_JournalLine_ID());
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
                PO.CopyValues(fromLines[i], toLine, GetAD_Client_ID(), GetAD_Org_ID());
                toLine.SetGL_Journal_ID(GetGL_Journal_ID());
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
                toLine.SetC_Currency_ID(fromLines[i].GetC_Currency_ID());
                toLine.SetIsGenerated(true);
                toLine.SetProcessed(false);
                toLine.SetQty(fromLines[i].GetQty());
                toLine.SetElementType(fromLines[i].GetElementType());
                if (!toLine.Save(fromJournal.Get_TrxName()))
                {
                    String error = "";
                    ValueNamePair pp = VLogger.RetrieveError();
                    if (pp != null)
                    {
                        error = pp.GetName();
                        if (String.IsNullOrEmpty(error))
                        {
                            error = pp.GetValue();
                        }
                        _log.Log(Level.SEVERE, String.IsNullOrEmpty(error) ? "Could not create GL Journal" : Msg.GetMsg(toLine.GetCtx(), error));
                    }
                    SetProcessMsg(String.IsNullOrEmpty(error) ? "Could not create GL Journal" : Msg.GetMsg(toLine.GetCtx(), error));
                }
                else
                {
                    count++;
                    lineCount += toLine.CopyLinesFrom(fromLines[i], toLine.GetGL_JournalLine_ID());
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
            String sql = "UPDATE GL_JournalLine SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE GL_Journal_ID=" + GetGL_Journal_ID();
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
            MAcctSchema acctSchema = MAcctSchema.Get(GetCtx(), GetC_AcctSchema_ID());
            int c_Currency_ID = acctSchema.GetC_Currency_ID();
            SetC_Currency_ID(c_Currency_ID);
            SetCurrencyRate(1);

            // Set Costing based on Accounting Schema
            if (Util.GetValueOfBool(acctSchema.Get_Value("Costing")) && Get_ColumnIndex("Costing") >= 0)
            {
                Set_Value("Costing", Util.GetValueOfBool(acctSchema.Get_Value("Costing")));
            }

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
            int C_DefaultCurrencyType_ID = MConversionType.GetDefault(GetAD_Client_ID());

            // selected accounting schema currency
            int selectedAcctSchemaCurrency = MAcctSchema.Get(GetCtx(), GetC_AcctSchema_ID()).GetC_Currency_ID();

            // this query return a record of assigned org accounting schema having same chart of account
            String sql = @"SELECT DISTINCT CA.C_ACCTSCHEMA_ID , Ca.C_Currency_ID , " + C_DefaultCurrencyType_ID + @" AS C_ConversionType_ID , 
  CURRENCYRATE(" + selectedAcctSchemaCurrency + @" , Ca.C_Currency_ID , " + GlobalVariable.TO_DATE(GetDateAcct(), true) +
  @" , " + C_DefaultCurrencyType_ID + @" , " + GetAD_Client_ID() + " , " + GetAD_Org_ID() + @") AS Rate
FROM C_AcctSchema CA INNER JOIN FRPT_AssignedOrg AO ON CA.C_AcctSchema_ID = AO.C_AcctSchema_ID
INNER JOIN C_AcctSchema_Element ASE ON (CA.C_AcctSchema_ID = ASE.C_AcctSchema_ID AND ElementType = 'AC')
WHERE CA.ISACTIVE = 'Y' AND ASE.IsActive = 'Y' AND AO.IsActive = 'Y' AND AO.AD_Org_ID IN(0," + GetAD_Org_ID() + @")
AND ASE.C_Element_ID = (SELECT C_Element_ID FROM C_AcctSchema_Element WHERE ElementType = 'AC' AND IsActive = 'Y' AND C_AcctSchema_ID = " + GetC_AcctSchema_ID() + @" )
AND CA.C_AcctSchema_ID != " + GetC_AcctSchema_ID();
            sql = MRole.GetDefault(GetCtx()).AddAccessSQL(sql, "C_AcctSchema", true, true);

            DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
            MAssignAcctSchema assignAcctSchema = null;
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    // create new record 
                    assignAcctSchema = new MAssignAcctSchema(GetCtx(), 0, Get_Trx());
                    assignAcctSchema.SetAD_Client_ID(GetAD_Client_ID());
                    assignAcctSchema.SetAD_Org_ID(GetAD_Org_ID());
                    assignAcctSchema.SetGL_Journal_ID(GetGL_Journal_ID());
                    assignAcctSchema.SetC_AcctSchema_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ACCTSCHEMA_ID"]));
                    assignAcctSchema.SetC_Currency_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Currency_ID"]));
                    assignAcctSchema.SetC_ConversionType_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ConversionType_ID"]));
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
            // Manish 18/7/2016 ..  check gl_journalbatch_id is in window or not.
            string sqlquery = @"SELECT gl_journalbatch_id FROM gl_journal WHERE gl_journal_id =" + GetGL_Journal_ID();
            int nooo = Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sqlquery, null, null));
            if (nooo <= 0)
            {
                return true;
            }
            // end 18/7/2016

            String sql = "UPDATE GL_JournalBatch jb"
                + " SET (TotalDr, TotalCr) = (SELECT SUM(TotalDr), SUM(TotalCr)" //jz hard coded ", "
                    + " FROM GL_Journal j WHERE j.IsActive='Y' AND jb.GL_JournalBatch_ID=j.GL_JournalBatch_ID) "
                + "WHERE GL_JournalBatch_ID=" + GetGL_JournalBatch_ID();
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
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType(), GetAD_Org_ID()))
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
            string getlinevalues = @"SELECT gl.AmtSourceDr, gl.AmtSourceCr, gl.GL_JournalLine_ID, SUM(gld.amount) AS LineDimensionAmt  
                                    FROM GL_JournalLine gl INNER JOIN 
                                    GL_LineDimension gld ON gl.GL_JournalLine_ID = gld.GL_JournalLine_ID
                                    where GL_Journal_ID =" + Get_Value("GL_Journal_ID") + @" 
                                    GROUP BY gl.AmtSourceDr,
                                     gl.AmtAcctCr, gl.AmtSourceCr, gl.GL_JournalLine_ID";
            DataSet dts = DB.ExecuteDataset(getlinevalues, null, null);

            if (dts != null && dts.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dts.Tables[0].Rows.Count; i++)
                {
                    journalDRAndCR = 0;

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

                    if (journalDRAndCR != Util.GetValueOfDecimal(dts.Tables[0].Rows[i]["LineDimensionAmt"]))
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
                MAcctSchemaGL gl = MAcctSchemaGL.Get(GetCtx(), GetC_AcctSchema_ID());
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

            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());

            // if Overwrite Date on Complete checkbox is true.
            if (dt.IsOverwriteDateOnComplete())
            {
                SetDateDoc(DateTime.Now.Date);
                if (GetDateAcct().Value.Date < GetDateDoc().Value.Date)
                {
                    SetDateAcct(GetDateDoc());

                    //	Std Period open?
                    if (!MPeriod.IsOpen(GetCtx(), GetDateDoc(), dt.GetDocBaseType(), GetAD_Org_ID()))
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
                String value = MSequence.GetDocumentNo(GetC_DocType_ID(), Get_TrxName(), GetCtx(), true, this);
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
                DB.ExecuteQuery(@"Update GL_Journal SET TotalDr = 0, TotalCr = 0
                                    WHERE GL_Journal_ID =  " + GetGL_Journal_ID(), null, Get_Trx());

                //Bug 124 in Devops :  set Amount as ZERO on GL journal Line, 
                DB.ExecuteQuery(@"Update GL_JournalLine SET AmtSourceDr = 0, AmtSourceCr = 0, AmtAcctDr = 0, AmtAcctCr = 0 
                                    WHERE GL_Journal_ID =  " + GetGL_Journal_ID(), null, Get_Trx());

                //Bug 124 in Devops :  set Amount as ZERO on GL journal Line, 
                DB.ExecuteQuery(@"Update GL_LineDimension  SET Amount = 0
                                    WHERE GL_JournalLine_ID IN (SELECT GL_JournalLine_ID FROM GL_JournalLine 
                                    WHERE GL_Journal_ID =   " + GetGL_Journal_ID() + ") ", null, Get_Trx());

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
            return ReverseCorrectIt(GetGL_JournalBatch_ID()) != null;
        }	//	reverseCorrectIt

        /// <summary>
        /// Reverse Correction.As if nothing happened - same date
        /// </summary>
        /// <param name="GL_JournalBatch_ID">batch</param>
        /// <returns>reversed journal or null</returns>
        public MJournal ReverseCorrectIt(int GL_JournalBatch_ID)
        {
            log.Info(ToString());
            //If any journal line is allocated then the Journal will not Reverted
            string sql = "SELECT Count(IsAllocated) AS IsAllocated FROM GL_JournalLine WHERE GL_Journal_ID = " + GetGL_Journal_ID() + " AND IsAllocated = 'Y'";
            if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) > 0)
            {
                m_processMsg = Msg.GetMsg(GetCtx(), "DeleteAllowcationFirst");
                return null;
            }
            //	Journal
            MJournal reverse = new MJournal(this);
            reverse.SetGL_JournalBatch_ID(GL_JournalBatch_ID);
            reverse.SetDateDoc(GetDateDoc());
            reverse.SetC_Period_ID(GetC_Period_ID());
            reverse.SetDateAcct(GetDateAcct());
            reverse.SetIsFormData(true);
            //	Reverse indicator
            String description = reverse.GetDescription();
            //Set append document number on new reversal record
            if (description == null)
            {
                description = "{->" + GetDocumentNo() + ")";
            }
            else
            {
                description += " | {-> " + GetDocumentNo() + ")";
            }
            reverse.SetDescription(description);

            if (reverse.Get_ColumnIndex("ReversalDoc_ID") > 0 && reverse.Get_ColumnIndex("IsReversal") > 0)
            {
                // set Reversal property for identifying, record is reversal or not during saving or for other actions
                reverse.SetIsReversal(true);
                // Set Orignal Document Reference
                reverse.SetReversalDoc_ID(GetGL_Journal_ID());
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
                _processMsg = CopyAssignAccountingSchema(GetGL_Journal_ID(), reverse.GetGL_Journal_ID(), reverse.Get_TrxName());
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
            //VA230:Append reverse document number in description
            AddDescription("(" + reverse.GetDocumentNo() + "<-)");
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
                PO.CopyValues(fromLines[i], toAssignAcctSchemaLine, GetAD_Client_ID(), GetAD_Org_ID());
                toAssignAcctSchemaLine.SetGL_Journal_ID(newJournal_Id);
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
            String sql = "SELECT * FROM GL_AssignAcctSchema WHERE GL_Journal_ID=@Param1";
            SqlParameter[] Param = new SqlParameter[1];
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                Param[0] = new SqlParameter("@Param1", GetGL_Journal_ID());

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
            return ReverseAccrualIt(GetGL_JournalBatch_ID()) != null;
        }	//	reverseAccrualIt

        /// <summary>
        ///	Reverse Accrual.	Flip Dr/Cr - Use Today's date
        /// </summary>
        /// <param name="GL_JournalBatch_ID">reversal batch</param>
        /// <returns> journal or null </returns>
        public MJournal ReverseAccrualIt(int GL_JournalBatch_ID)
        {
            log.Info(ToString());
            //	Journal
            MJournal reverse = new MJournal(this);
            reverse.SetGL_JournalBatch_ID(GL_JournalBatch_ID);
            reverse.SetDateDoc(DateTime.Now);
            reverse.Set_ValueNoCheck("C_Period_ID", null);		//	reset
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
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
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
            //	ReportEngine re = ReportEngine.get (getCtx(), ReportEngine.INVOICE, getC_Invoice_ID());
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
        /// <returns>AD_User_ID (Created)</returns>
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
        public static MJournal CopyFrom(Ctx ctx, int GL_Journal_ID, DateTime? dateDoc, Trx trxName)
        {
            MJournal from = new MJournal(ctx, GL_Journal_ID, trxName);
            if (from.GetGL_Journal_ID() == 0)
            {
                throw new ArgumentException("From Journal Batch not found GL_JournalBatch_ID=" + GL_Journal_ID);
            }
            //
            MJournal to = new MJournal(ctx, 0, trxName);
            PO.CopyValues(from, to, from.GetAD_Client_ID(), from.GetAD_Org_ID());
            to.Set_ValueNoCheck("DocumentNo", null);
            // to.Set_ValueNoCheck("C_Period_ID", null);
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
                String error = "";
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null)
                {
                    error = pp.GetName();
                    if (String.IsNullOrEmpty(error))
                    {
                        error = pp.GetValue();
                    }
                    _log.Log(Level.SEVERE, String.IsNullOrEmpty(error) ? "Could not create GL Journal" : Msg.GetMsg(to.GetCtx(), error));
                }
                to.SetProcessMsg(String.IsNullOrEmpty(error) ? "Could not create GL Journal" : Msg.GetMsg(to.GetCtx(), error));
                throw new Exception(String.IsNullOrEmpty(error) ? "Could not create GL Journal" : Msg.GetMsg(to.GetCtx(), error));
            }

            if (to.CopyJLines(from, dateDoc) == 0)
            {
                throw new Exception("Could not create GL Journal Details");
            }

            return to;
        }   //	copyFrom
            //End Here

        /// <summary>
        /// Author:VA230
        /// Add description
        /// </summary>
        /// <param name="description">description</param>
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }
    }	//	MJournal

}
