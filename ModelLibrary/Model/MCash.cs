/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MCash
 * Purpose        : Cash Journal Model
 * Class Used     : X_C_Cash, DocAction
 * Chronological    Development
 * Raghunandan     23-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.ProcessEngine;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;
using VAdvantage.Print;

namespace VAdvantage.Model
{
    public class MCash : X_C_Cash, DocAction
    {
        #region variables
        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MCash).FullName);
        /**	Lines					*/
        private MCashLine[] _lines = null;
        /** CashBook				*/
        private MCashBook _book = null;
        /**	Process Message 			*/
        private String _processMsg = null;
        /**	Just Prepared Flag			*/
        private bool _justPrepared = false;

        // used for locking query - checking schedule is paid or not on completion
        // when multiple user try to pay agaisnt same schedule from different scenarion at that tym lock record
        static readonly object objLock = new object();

        Tuple<String, String, String> mInfo = null;
        #endregion

        /**
         * 	Get Cash Journal for currency, org and date
         *	@param ctx context
         *	@param C_Currency_ID currency
         *	@param AD_Org_ID org
         *	@param dateAcct date
         *	@param trxName transaction
         *	@return cash
         */
        public static MCash Get(Ctx ctx, int AD_Org_ID, DateTime? dateAcct, int C_Currency_ID, Trx trxName)
        {
            MCash retValue = null;
            //	Existing Journal
            String sql = "SELECT * FROM C_Cash c "
                + "WHERE c.AD_Org_ID=" + AD_Org_ID						//	#1
                + " AND TRUNC(c.StatementDate, 'DD')=@sdate"			//	#2
                + " AND c.Processed='N'"
                + " AND EXISTS (SELECT * FROM C_CashBook cb "
                    + "WHERE c.C_CashBook_ID=cb.C_CashBook_ID AND cb.AD_Org_ID=c.AD_Org_ID"
                    + " AND cb.C_Currency_ID=" + C_Currency_ID + ")";			//	#3
            DataTable dt = null;
            SqlParameter[] param = null;
            IDataReader idr = null;
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@sdate", TimeUtil.GetDay(dateAcct));

                idr = DB.ExecuteReader(sql, param, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MCash(ctx, dr, trxName);
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
            finally { dt = null; }

            if (retValue != null)
                return retValue;

            //	Get CashBook
            MCashBook cb = MCashBook.Get(ctx, AD_Org_ID, C_Currency_ID);
            if (cb == null)
            {
                _log.Warning("No CashBook for AD_Org_ID=" + AD_Org_ID + ", C_Currency_ID=" + C_Currency_ID);
                return null;
            }

            //	Create New Journal
            retValue = new MCash(cb, dateAcct);
            sql = @"SELECT SUM(completedbalance + runningbalance)AS BegningBalance FROM c_cashbook WHERE IsActive = 'Y' AND  c_cashbook_id =" + cb.GetC_CashBook_ID();
            decimal beginingBalance = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, trxName));

            //Added By Manjot -Changes done for Target Doctype Cash Journal
            Int32 DocType_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_Doctype_ID FROM C_Doctype WHERE DocBaseType='CMC' AND AD_Client_ID='" + ctx.GetAD_Client_ID() + "' AND AD_Org_ID IN('0','" + ctx.GetAD_Org_ID() + "') ORDER BY  AD_Org_ID DESC"));
            if (DocType_ID != 0)
            {
                retValue.SetC_DocType_ID(DocType_ID);
            }
            // Manjot

            retValue.SetBeginningBalance(beginingBalance);
            retValue.Save(trxName);
            return retValue;
        }

        // new function added to gt cash journal according to terminal's cashbook
        // Added by Vivek on 14/07/2017 as per discussion with Lokesh sir
        /**
         * 	Get Cash Journal for currency, org and date
         *	@param ctx context
         *	@param C_Currency_ID currency
         *	@param AD_Org_ID org
         *	@param dateAcct date
         *	@param trxName transaction
         *	@return cash
         */
        public static MCash Get(Ctx ctx, int AD_Org_ID, DateTime? dateAcct, int C_Currency_ID, int VAPOS_Terminal_ID, Trx trxName)
        {
            MCash retValue = null;
            int cashbook_ID = 0;
            MCashBook cb = null;
            cashbook_ID = Util.GetValueOfInt(DB.ExecuteScalar("Select C_CashBook_ID From VAPOS_POSTerminal Where VAPOS_POSTerminal_ID=" + VAPOS_Terminal_ID));

            //	Existing Journal from cashbook
            String sql = "SELECT * FROM C_Cash c "
                + "WHERE c.AD_Org_ID=" + AD_Org_ID						//	#1
                + " AND TRUNC(c.StatementDate, 'DD')=@sdate"			//	#2
                + " AND c.Processed='N'"
                + " AND EXISTS (SELECT * FROM C_CashBook cb "
                    + "WHERE c.C_CashBook_ID=" + cashbook_ID + " AND cb.AD_Org_ID=c.AD_Org_ID"
                    + " AND cb.C_Currency_ID=" + C_Currency_ID + ")";			//	#3
            DataTable dt = null;
            SqlParameter[] param = null;
            IDataReader idr = null;
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@sdate", TimeUtil.GetDay(dateAcct));

                idr = DB.ExecuteReader(sql, param, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MCash(ctx, dr, trxName);
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
            finally { dt = null; }

            if (retValue != null)
                return retValue;

            // get cashbook 
            cb = new MCashBook(ctx, cashbook_ID, trxName);

            if (cb == null)
            {
                _log.Warning("No CashBook for AD_Org_ID=" + AD_Org_ID + ", C_Currency_ID=" + C_Currency_ID);
                return null;
            }

            //	Create New Journal
            retValue = new MCash(cb, dateAcct);
            retValue.Save(trxName);
            return retValue;
        }

        /**
         * 	Get Cash Journal for CashBook and date
         *	@param ctx context
         *	@param C_CashBook_ID cashbook
         *	@param dateAcct date
         *	@param trxName transaction
         *	@return cash
         */
        public static MCash Get(Ctx ctx, int C_CashBook_ID, DateTime? dateAcct, Trx trxName)
        {



            MCash retValue = null;
            //	Existing Journal
            String sql = "SELECT * FROM C_Cash c "
                + "WHERE c.C_CashBook_ID=" + C_CashBook_ID					//	#1
                + " AND TRUNC(c.StatementDate,'DD')=@sdate"			//	#2
                + " AND c.Processed='N'";
            DataTable dt = null;
            SqlParameter[] param = null;
            IDataReader idr = null;
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@sdate", TimeUtil.GetDay(dateAcct));
                idr = DB.ExecuteReader(sql, param, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MCash(ctx, dr, trxName);
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
            finally { dt = null; }

            if (retValue != null)
                return retValue;

            //	Get CashBook
            MCashBook cb = new MCashBook(ctx, C_CashBook_ID, trxName);
            if (cb.Get_ID() == 0)
            {
                _log.Warning("Not found C_CashBook_ID=" + C_CashBook_ID);
                return null;
            }

            //	Create New Journal
            retValue = new MCash(cb, dateAcct);
            sql = @"SELECT SUM(completedbalance + runningbalance)AS BegningBalance FROM c_cashbook WHERE IsActive = 'Y' AND  c_cashbook_id =" + cb.GetC_CashBook_ID();
            decimal beginingBalance = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, trxName));

            //Added By Manjot -Changes done for Target Doctype Cash Journal
            Int32 DocType_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_Doctype_ID FROM C_Doctype WHERE DocBaseType='CMC' AND AD_Client_ID='" + ctx.GetAD_Client_ID() + "' AND AD_Org_ID IN('0','" + ctx.GetAD_Org_ID() + "') ORDER BY  AD_Org_ID DESC"));
            if (DocType_ID != 0)
            {
                retValue.SetC_DocType_ID(DocType_ID);
            }
            // Manjot

            retValue.SetBeginningBalance(beginingBalance);
            retValue.Save(trxName);
            return retValue;
        }

        /// <summary>
        /// Get MCash Model against cash book id ,date acoount ,and shift date 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_CashBook_ID"></param>
        /// <param name="dateAcct"></param>
        /// <param name="trxName"></param>
        /// <param name="shift"></param>
        /// <param name="ShiftDate"></param>
        /// <returns></returns>
        public static MCash GetCash(Ctx ctx, int C_CashBook_ID, DateTime? dateAccount, Trx trxName, int shift, DateTime? ShiftDate)
        {
            DateTime convertedDate = Convert.ToDateTime(ShiftDate);
            DateTime localDate = convertedDate.ToLocalTime();

            DateTime dateAcct = Convert.ToDateTime(dateAccount).ToLocalTime();


            MCash retValue = null;
            String sql = "";
            //	Existing Journal with out shift
            if (shift == 0)
            {
                sql = "SELECT * FROM C_Cash c "
                + "WHERE c.C_CashBook_ID=" + C_CashBook_ID					//	#1
                + " AND TRUNC(c.StatementDate,'DD')=@sdate"			//	#2
                + " AND c.Processed='N' and VAPOS_SHIFTDATE is null";

            }
            //	Existing Journal with shift Details
            else
            {
                sql = "SELECT * FROM C_Cash c "
                    + "WHERE c.C_CashBook_ID=" + C_CashBook_ID					//	#1
                    + " AND TRUNC(c.StatementDate,'DD')=@sdate"			//	#2
                    + " AND c.Processed='N' And VAPOS_SHIFTDETAILS_ID=" + shift
                    ;
            }
            DataTable dt = null;
            SqlParameter[] param = null;
            IDataReader idr = null;
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@sdate", TimeUtil.GetDay(dateAcct));
                idr = DB.ExecuteReader(sql, param, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MCash(ctx, dr, trxName);
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
            finally { dt = null; }

            if (retValue != null)
                return retValue;

            //	Get CashBook
            MCashBook cb = new MCashBook(ctx, C_CashBook_ID, trxName);
            if (cb.Get_ID() == 0)
            {
                _log.Warning("Not found C_CashBook_ID=" + C_CashBook_ID);
                return null;
            }

            //	Create New Journal
            retValue = new MCash(cb, dateAcct);
            sql = @"SELECT SUM(completedbalance + runningbalance)AS BegningBalance FROM c_cashbook WHERE IsActive = 'Y' AND  c_cashbook_id =" + cb.GetC_CashBook_ID();
            decimal beginingBalance = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, trxName));

            //Added By Manjot -Changes done for Target Doctype Cash Journal
            Int32 DocType_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_Doctype_ID FROM C_Doctype WHERE DocBaseType='CMC' AND AD_Client_ID='" + ctx.GetAD_Client_ID() + "' AND AD_Org_ID IN('0','" + ctx.GetAD_Org_ID() + "') ORDER BY  AD_Org_ID DESC"));
            if (DocType_ID != 0)
            {
                retValue.SetC_DocType_ID(DocType_ID);
            }
            // Manjot

            retValue.SetBeginningBalance(beginingBalance);
            if (shift != 0)
            {
                retValue.SetVAPOS_ShiftDetails_ID(shift);
                retValue.SetVAPOS_ShiftDate(localDate);
            }
            retValue.Save(trxName);
            return retValue;
        }

        //Amit 10-9-2014 - Correspity Work
        public static MCash Get(Ctx ctx, int AD_Org_ID, DateTime? dateAcct, int C_Currency_ID, int C_CashBook_ID, int C_DocType_ID, Trx trxName)
        {
            MCash retValue = null;
            //	Existing Journal
            String sql = "SELECT * FROM C_Cash c "
                + "WHERE c.AD_Org_ID=" + AD_Org_ID						//	#1
                + " AND TRUNC(c.StatementDate, 'DD')=@sdate"			//	#2
                + " AND c.Processed='N'"
                + " AND EXISTS (SELECT * FROM C_CashBook cb "
                    + "WHERE c.C_CashBook_ID=" + C_CashBook_ID + " AND cb.AD_Org_ID=c.AD_Org_ID)";			//	#3
            DataTable dt = null;
            SqlParameter[] param = null;
            IDataReader idr = null;
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@sdate", TimeUtil.GetDay(dateAcct));

                idr = DB.ExecuteReader(sql, param, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MCash(ctx, dr, trxName);
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
            finally { dt = null; }

            if (retValue != null)
                return retValue;

            //	Get CashBook
            //MCashBook cb = MCashBook.Get(ctx, AD_Org_ID, C_Currency_ID);
            //if (cb == null)
            //{
            //    _log.Warning("No CashBook for AD_Org_ID=" + AD_Org_ID + ", C_Currency_ID=" + C_Currency_ID);
            //    return null;
            //}
            MCashBook cb = new MCashBook(ctx, C_CashBook_ID, trxName);
            if (cb == null)
            {
                _log.Warning("No CashBook");
                return null;
            }

            //Get Closing Balance of Last Record of that cashbook
            //sql = @"SELECT EndingBalance FROM C_Cash WHERE IsActive    = 'Y' AND C_CashBook_ID = "+ C_CashBook_ID
            //         + " AND c_cash_id = (SELECT MAX(C_Cash_ID) FROM C_Cash WHERE IsActive = 'Y' AND C_CashBook_ID = " + C_CashBook_ID+ ")";
            sql = @"SELECT SUM(completedbalance + runningbalance)AS BegningBalance FROM c_cashbook WHERE IsActive = 'Y' AND  c_cashbook_id =" + C_CashBook_ID;
            decimal beginingBalance = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, trxName));


            //	Create New Journal
            retValue = new MCash(cb, dateAcct);
            retValue.SetC_DocType_ID(C_DocType_ID);
            retValue.SetBeginningBalance(beginingBalance);
            retValue.Save(trxName);
            return retValue;
        }
        //Amit

        // Added By Amit 31-7-2015 VAMRP
        //Modify By Deepak For KC HR Module Not Used
        public static MCash Get(Ctx ctx, int C_CashBook_ID, DateTime? statementDate, DateTime? dateAcct, Trx trxName)
        {
            MCash retValue = null;
            //	Existing Journal
            String sql = "SELECT * FROM C_Cash c "
                + "WHERE c.C_CashBook_ID=@param1"					//	#1
                + " AND TRUNC(c.StatementDate,'DD')=@param2 AND TRUNC(c.DATEACCT,'DD')=@param3"			//	#2
                + " AND c.Processed='N'";
            SqlParameter[] param = new SqlParameter[3];
            IDataReader idr = null;
            try
            {
                param[0] = new SqlParameter("@param1", C_CashBook_ID);
                param[1] = new SqlParameter("@param2", TimeUtil.GetDay(statementDate));
                param[2] = new SqlParameter("@param3", TimeUtil.GetDay(dateAcct));
                idr = DB.ExecuteReader(sql, param, trxName);
                if (idr.Read())
                    retValue = new MCash(ctx, idr, trxName);
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                _log.Log(Level.SEVERE, sql, e);
            }


            if (retValue != null)
                return retValue;

            //	Get CashBook
            MCashBook cb = new MCashBook(ctx, C_CashBook_ID, trxName);
            if (cb.Get_ID() == 0)
            {
                _log.Warning("Not found C_CashBook_ID=" + C_CashBook_ID);
                return null;
            }

            //	Create New Journal
            retValue = new MCash(cb, statementDate);
            retValue.Save(trxName);
            return retValue;
        }
        //End Amit

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param C_Cash_ID id
         *	@param trxName transaction
         */
        public MCash(Ctx ctx, int C_Cash_ID, Trx trxName)
            : base(ctx, C_Cash_ID, trxName)
        {
            if (C_Cash_ID == 0)
            {
                //	setC_CashBook_ID (0);		//	FK
                SetBeginningBalance(Env.ZERO);
                SetEndingBalance(Env.ZERO);
                SetStatementDifference(Env.ZERO);
                SetDocAction(DOCACTION_Complete);
                SetDocStatus(DOCSTATUS_Drafted);
                //
                DateTime today = TimeUtil.GetDay(DateTime.Now);
                SetStatementDate(today);	// @#Date@
                SetDateAcct(today);	// @#Date@
                //String name = DisplayType.getDateFormat(DisplayType.Date).format(today) + " " + MOrg.Get(ctx, GetAD_Org_ID()).GetValue();
                String name = String.Format("{0:d}", today) + " " + MOrg.Get(ctx, GetAD_Org_ID()).GetValue();
                SetName(name);
                SetIsApproved(false);
                SetPosted(false);	// N
                SetProcessed(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MCash(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        // Added By Amit 31-7-2015
        /**
        * 	Load Constructor
        *	@param ctx context
        *	@param dr result set
        *	@param trxName transaction
        */
        public MCash(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }


        /**
         * 	Parent Constructor
         *	@param cb cash book
         *	@param today date - if null today
         */
        public MCash(MCashBook cb, DateTime? today)
            : this(cb.GetCtx(), 0, cb.Get_TrxName())
        {
            SetClientOrg(cb);
            SetC_CashBook_ID(cb.GetC_CashBook_ID());
            if (today != null)
            {
                SetStatementDate(today);
                SetDateAcct(today);
                //String name = DisplayType.getDateFormat(DisplayType.Date).format(today) + " " + cb.GetName();
                String name = String.Format("{0:d}", today) + " " + cb.GetName();
                SetName(name);
            }
            _book = cb;
        }



        /**
         * 	Get Lines
         *	@param requery requery
         *	@return lines
         */
        public MCashLine[] GetLines(bool requery)
        {
            if (_lines != null && !requery)
                return _lines;
            List<MCashLine> list = new List<MCashLine>();
            String sql = "SELECT * FROM C_CashLine WHERE C_Cash_ID=" + GetC_Cash_ID() + " ORDER BY Line";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MCashLine(GetCtx(), dr, Get_TrxName()));
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
            finally { dt = null; }

            _lines = new MCashLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /**
         * 	Get Cash Book
         *	@return cash book
         */
        public MCashBook GetCashBook()
        {
            if (_book == null)
                _book = MCashBook.Get(GetCtx(), GetC_CashBook_ID());
            return _book;
        }

        /**
         * 	Get Document No 
         *	@return name
         */
        public String GetDocumentNo()
        {
            return GetName();
        }

        /**
         * 	Get Document Info
         *	@return document info (untranslated)
         */
        public String GetDocumentInfo()
        {
            return Msg.GetElement(GetCtx(), "C_Cash_ID") + " " + GetDocumentNo();
        }

        ///**
        // * 	Create PDF
        // *	@return File or null
        // */
        //public File createPDF ()
        //{
        //    try
        //    {
        //        File temp = File.createTempFile(get_TableName()+get_ID()+"_", ".pdf");
        //        return createPDF (temp);
        //    }
        //    catch (Exception e)
        //    {
        //        //log.severe("Could not create PDF - " + e.getMessage());
        //    }
        //    return null;
        //}	//	getPDF

        ///**
        // * 	Create PDF file
        // *	@param file output file
        // *	@return file if success
        // */
        //public File createPDF (File file)
        //{
        ////	ReportEngine re = ReportEngine.Get (GetCtx(), ReportEngine.INVOICE, getC_Invoice_ID());
        ////	if (re == null)
        //        return null;
        ////	return re.getPDF(file);
        //}	

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF()
        {
            //try
            //{
            //    string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
            //                        + ".txt"; //.pdf
            //    string filePath = Path.GetTempPath() + fileName;

            //    //File temp = File.createTempFile(Get_TableName() + Get_ID() + "_", ".pdf");
            //    //FileStream fOutStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

            //    FileInfo temp = new FileInfo(filePath);
            //    if (!temp.Exists)
            //    {
            //        return CreatePDF(temp);
            //    }
            //}
            //catch (Exception e)
            //{
            //    log.Severe("Could not create PDF - " + e.Message);
            //}
            //return null;






            try
            {
                //string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                String fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo() + ".pdf";
                string filePath = Path.Combine(GlobalVariable.PhysicalPath, "TempDownload", fileName);

                int processID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Process_Id FROM AD_Process WHERE VALUE='CashJournalReport'", null, null));
                MPInstance instance = new MPInstance(GetCtx(), processID, GetC_Cash_ID());
                instance.Save();
                ProcessInfo pi = new ProcessInfo("", processID, Get_Table_ID(), GetC_Cash_ID());
                pi.SetAD_PInstance_ID(instance.GetAD_PInstance_ID());

                ProcessCtl ctl = new ProcessCtl();
                //ctl.IsArabicReportFromOutside = false;
                byte[] report = null;
                ReportEngine_N re = null;
                Dictionary<string, object> d = ctl.Process(pi, GetCtx(), out report, out re);

                File.WriteAllBytes(filePath, report);
                return new FileInfo(filePath);
                //re.GetView();
                //bool b = re.CreatePDF(filePath);

                //File temp = File.createTempFile(Get_TableName() + Get_ID() + "_", ".pdf");
                //FileStream fOutStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                //FileInfo temp = new FileInfo(filePath);

                //if (!temp.Exists)
                //{
                //    b = re.CreatePDF(filePath);
                //    if (b)
                //    {
                //        return new FileInfo(filePath);
                //    }
                //    return null;
                //}
                //else
                //    return temp;
            }
            catch (Exception e)
            {
                log.Severe("Could not create PDF - " + e.Message);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public FileInfo CreatePDF(FileInfo file)
        {
            ////ReportEngine re = ReportEngine.Get(GetCtx(), ReportEngine.ORDER, GetC_Order_ID());
            ////if (re == null)
            ////    return null;
            ////return re.getPDF(file);

            //Create a file to write to.
            using (StreamWriter sw = file.CreateText())
            {
                sw.WriteLine("Hello");
                sw.WriteLine("And");
                sw.WriteLine("Welcome");
            }

            return file;

        }

        /**
         * 	Set StatementDate - Callout
         *	@param oldStatementDate old
         *	@param newStatementDate new
         *	@param windowNo window no
         */
        //@UICallout 
        public void SetStatementDate(String oldStatementDate, String newStatementDate, int windowNo)
        {
            if (newStatementDate == null || newStatementDate.Length == 0)
                return;
            DateTime statementDate = Convert.ToDateTime(PO.ConvertToTimestamp(newStatementDate));
            if (statementDate == null)
                return;
            SetStatementDate(statementDate);
        }

        /**
         *	Set Statement Date and Acct Date
         */
        public void SetStatementDate(DateTime? statementDate)
        {
            base.SetStatementDate(statementDate);
            base.SetDateAcct(statementDate);
        }

        /**
         * 	Before Save
         *	@param newRecord
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            SetAD_Org_ID(GetCashBook().GetAD_Org_ID());
            if (GetAD_Org_ID() == 0)
            {
                log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@AD_Org_ID@"));
                return false;
            }

            // if cash journal is open against same cashbook, then not to save new record fo same cashbook
            int no = 0;
            if (newRecord || Is_ValueChanged("C_CashBook_ID"))
            {
                no = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(C_Cash_ID) FROM C_Cash WHERE IsActive = 'Y' AND DocStatus NOT IN ('CO' , 'CL', 'VO')  
                                                           AND C_CashBook_ID = " + GetC_CashBook_ID(), null, Get_Trx()));
                if (no > 0)
                {
                    log.SaveError("Warning", Msg.GetMsg(GetCtx(), "VIS_CantOpenNewCashBook"));
                    return false;
                }
            }

            //JID_1326: System should not allow to save Cash Journal with previous date, statement date should be equal or greater than previous created Cash Journal record with same CashBook. 
            no = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(C_Cash_ID) FROM C_Cash WHERE IsActive = 'Y' AND DocStatus != 'VO' AND StatementDate > "
                + GlobalVariable.TO_DATE(GetStatementDate(), true) + " AND C_CashBook_ID = " + GetC_CashBook_ID() + " AND C_Cash_ID != " + Get_ID(), null, Get_Trx()));
            if (no > 0)
            {
                log.SaveError("VIS_CashStatementDate", "");
                return false;
            }

            //	Calculate End Balance
            SetEndingBalance(Decimal.Add(GetBeginningBalance(), GetStatementDifference()));

            // set Currency on the basis of CashBook
            try
            {
                SetC_Currency_ID(Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_Currency_ID FROM C_CashBook WHERE C_CashBook_ID = " + GetC_CashBook_ID())));
            }
            catch { }

            // JID_1280 : restrict to change the date account in case line with currency different than header is present.
            // restrict to change the date account/ statement date / cashbook / documnet type when line exist
            if (!newRecord && (Is_ValueChanged("DateAcct") || Is_ValueChanged("StatementDate") ||
                              Is_ValueChanged("C_CashBook_ID") || Is_ValueChanged("C_DocType_ID")))
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(C_CashLine_ID) FROM C_CashLine WHERE IsActive='Y' AND C_Cash_ID=" + GetC_Cash_ID())) > 0)
                {
                    log.SaveError("VIS_CantChange", "");
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true, if it can be deleted</returns>
        protected override bool BeforeDelete()
        {
            try
            {
                // If Processed is true then, it will give delete error
                if (IsProcessed())
                {
                    return false;
                }

                // Get all Casgh Lines
                GetLines(false);
                for (int i = 0; i < _lines.Length; i++)
                {
                    // JID_1275: "If we delete cash journal system do not update 'Running Balance"" field on cashbook also on cashbook line."
                    if (!_lines[i].Delete(true))
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /**
         * 	Process document
         *	@param processAction document action
         *	@return true if performed
         */
        public bool ProcessIt(String processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }

        /**
         * 	Unlock Document.
         * 	@return true if success 
         */
        public bool UnlockIt()
        {
            log.Info(ToString());
            SetProcessing(false);
            return true;
        }

        /**
         * 	Invalidate Document
         * 	@return true if success 
         */
        public bool InvalidateIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        /**
         *	Prepare Document
         * 	@return new status (In Progress or Invalid) 
         */
        public String PrepareIt()
        {
            log.Info(ToString());
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), MDocBaseType.DOCBASETYPE_CASHJOURNAL, GetAD_Org_ID()))
            {
                _processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateAcct(), GetAD_Org_ID()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }


            MCashLine[] lines = GetLines(false);
            if (lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }
            //	Add up Amounts
            Decimal difference = Env.ZERO;
            int C_Currency_ID = GetC_Currency_ID();
            for (int i = 0; i < lines.Length; i++)
            {
                MCashLine line = lines[i];

                if (!line.IsActive())
                    continue;

                Decimal amt = line.GetAmount();

                if (C_Currency_ID == line.GetC_Currency_ID())
                    difference = Decimal.Add(difference, amt);
                else
                {
                    amt = MConversionRate.Convert(GetCtx(), line.GetAmount(),
                        line.GetC_Currency_ID(), C_Currency_ID, GetDateAcct(), line.GetC_ConversionType_ID(),
                        GetAD_Client_ID(), GetAD_Org_ID());
                    if (amt == 0)
                    {
                        //_processMsg = "No Conversion Rate found - @C_CashLine_ID@= " + line.GetLine();

                        MConversionType conv = MConversionType.Get(GetCtx(), line.GetC_ConversionType_ID());
                        _processMsg = Msg.GetMsg(GetCtx(), "NoConversion") + MCurrency.GetISO_Code(GetCtx(), line.GetC_Currency_ID()) + Msg.GetMsg(GetCtx(), "ToCashBookCurr")
                            + MCurrency.GetISO_Code(GetCtx(), C_Currency_ID) + " - " + Msg.GetMsg(GetCtx(), "ConversionType") + conv.GetName();
                        return DocActionVariables.STATUS_INVALID;
                    }
                    difference = Decimal.Add(difference, amt);
                }

                // Check applied here for lines whether credit allowed and credit limit not crossed // Lokesh 16 July 2019
                if (line.GetVSS_PAYMENTTYPE() == X_C_CashLine.VSS_PAYMENTTYPE_Payment && line.GetC_BPartner_ID() > 0)
                {
                    MBPartner bp = new MBPartner(GetCtx(), line.GetC_BPartner_ID(), Get_Trx());
                    string retMsg = "";
                    bool crdAll = bp.IsCreditAllowed(line.GetC_BPartner_Location_ID(), Decimal.Subtract(0, amt), out retMsg);
                    if (!crdAll)
                    {
                        if (bp.ValidateCreditValidation("D", line.GetC_BPartner_Location_ID()))
                        {
                            _processMsg = retMsg + " :: " + GetDocumentNo() + "_" + line.GetLine();
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                }
            }
            SetStatementDifference(difference);
            //	setEndingBalance(getBeginningBalance().add(getStatementDifference()));
            //
            _justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /**
         * 	Approve Document
         * 	@return true if success 
         */
        public bool ApproveIt()
        {
            log.Info(ToString());
            SetIsApproved(true);
            return true;
        }

        /**
         * 	Reject Approval
         * 	@return true if success 
         */
        public bool RejectIt()
        {
            log.Info(ToString());
            SetIsApproved(false);
            return true;
        }

        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>return new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public String CompleteIt()
        {
            ValueNamePair pp = null;
            //	Re-Check
            if (!_justPrepared)
            {
                String status = PrepareIt();
                if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                    return status;
            }

            //9-6-2016 if ending balance < 0 then not to be completed
            if (GetEndingBalance() < 0)
            {
                //_processMsg = Msg.GetMsg(GetCtx(), "BegBalCanNotLessThanZero");
                _processMsg = Msg.GetMsg(GetCtx(), "Ending should not be in negative value");
                return DocActionVariables.STATUS_INVALID;
            }
            //
            int OrderPayScheduleExists = 0;
            // check payment is received or not against any cash line which is created with the reerence of invoice & its schedule
            // if yes, then not able to complete this record
            if (Env.IsModuleInstalled("VA009_"))
            {
                // used for locking query - checking schedule is paid or not on completion
                // when multiple user try to pay agaisnt same schedule from different scenarion at that tym lock record
                lock (objLock)
                {
                    try
                    {
                        //VA230:To check Order Pay Schedule reference column exists or not
                        if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VA009_OrderPaySchedule_ID) FROM C_CashLine WHERE C_Cash_ID = " + GetC_Cash_ID(), null, null)) > 0)
                            OrderPayScheduleExists = 1;
                    }
                    catch (Exception ex)
                    {
                        OrderPayScheduleExists = 0;
                    }
                    if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(C.C_Cash_ID) FROM C_Cash c INNER JOIN C_CashLine cl ON c.c_cash_id = cl.c_cash_id 
                                                           INNER JOIN C_InvoicePaySchedule cs ON cs.C_InvoicePaySchedule_ID = cl.C_InvoicePaySchedule_ID
                                                           INNER JOIN C_Invoice inv ON inv.C_Invoice_ID = cl.C_Invoice_ID AND inv.DocStatus NOT IN ('RE' , 'VO') 
                                         WHERE cl.CashType = 'I' AND cs.DueAmt > 0  AND (nvl(cs.c_payment_id,0) != 0 or nvl(cs.c_cashline_id , 0) != 0 OR cs.VA009_IsPaid = 'Y')
                                         AND cl.IsActive = 'Y' AND c.c_cash_id = " + GetC_Cash_ID(), null, Get_Trx())) > 0)
                    {
                        String schedule = string.Empty;
                        string sql = "";
                        try
                        {
                            //used DBFunctionCollection method to get the query which will execute SQL as well as PostGre.
                            sql = DBFunctionCollection.CashLineRefInvScheduleDuePaidOrNot(GetC_Cash_ID());
                            if (!string.IsNullOrEmpty(sql))
                                schedule = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));
                        }
                        catch (Exception ex)
                        {
                            //ex to ex.Message to get Explanation about the Exception.
                            //log.Log(Level.SEVERE, sql, ex);
                            log.Log(Level.SEVERE, sql, ex.Message);
                        }

                        _processMsg = Msg.GetMsg(GetCtx(), "VIS_PayAlreadyDoneforInvoiceSchedule") + schedule;
                        return DocActionVariables.STATUS_INVALID;
                    }
                    //VA230:Check order pay schedule paid status
                    if (OrderPayScheduleExists == 1 && Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(C.C_Cash_ID) FROM C_Cash c INNER JOIN C_CashLine cl ON c.c_cash_id = cl.c_cash_id 
                                                           INNER JOIN VA009_OrderPaySchedule cs ON cs.VA009_OrderPaySchedule_ID = cl.VA009_OrderPaySchedule_ID
                                                           INNER JOIN C_Order ord ON ord.C_Order_ID = cl.C_Order_ID AND ord.DocStatus NOT IN ('RE' , 'VO', 'CL') 
                                         WHERE cl.CashType = 'O' AND cs.DueAmt > 0  AND (nvl(cs.c_payment_id,0) != 0 or nvl(cs.c_cashline_id , 0) != 0 OR cs.VA009_IsPaid = 'Y')
                                         AND cl.IsActive = 'Y' AND c.c_cash_id = " + GetC_Cash_ID(), null, Get_Trx())) > 0)
                    {
                        String schedule = string.Empty;
                        string sql = "";
                        try
                        {
                            //used DBFunctionCollection method to get the query which will execute SQL as well as PostGre.
                            sql = DBFunctionCollection.CashLineRefOrerPayScheduleDuePaidOrNot(GetC_Cash_ID());
                            if (!string.IsNullOrEmpty(sql))
                                schedule = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));
                        }
                        catch (Exception ex)
                        {
                            //ex to ex.Message to get Explanation about the Exception.
                            //log.Log(Level.SEVERE, sql, ex);
                            log.Log(Level.SEVERE, sql, ex.Message);
                        }

                        _processMsg = Msg.GetMsg(GetCtx(), "VA009_OrderScheduleAlreadyPaid") + schedule;
                        return DocActionVariables.STATUS_INVALID;
                    }
                }
            }

            //	Implicit Approval
            if (!IsApproved())
                ApproveIt();
            //
            log.Info(ToString());

            //********************** Vikas 1 Dec 2015  Issue assigned by Surya Sir*******************************************
            // View Allocation Header Created only When Any Cashline CashType is "Invoice"
            MCashLine[] Cashlines = GetLines(false);
            int CashTyp_Invc = 0;
            for (int i = 0; i < Cashlines.Length; i++)
            {
                MCashLine Cashline = Cashlines[i];
                // if record is inactive then not to consider 
                if (!Cashline.IsActive())
                    continue;
                if (MCashLine.CASHTYPE_Invoice.Equals(Cashline.GetCashType()))
                {
                    CashTyp_Invc++;
                }
            }
            //	Allocation Header
            MAllocationHdr alloc = new MAllocationHdr(GetCtx(), false,
               GetDateAcct(), GetC_Currency_ID(),
               Msg.Translate(GetCtx(), "C_Cash_ID") + ": " + GetName(), Get_TrxName());
            alloc.SetAD_Org_ID(GetAD_Org_ID());
            if (CashTyp_Invc > 0)
            {
                if (!alloc.Save())
                {
                    pp = VLogger.RetrieveError();
                    if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                        _processMsg = Msg.GetMsg(GetCtx(), "VIS_AllocationHdrNotSaved") + ", " + pp.GetName();
                    else
                        _processMsg = Msg.GetMsg(GetCtx(), "VIS_AllocationHdrNotSaved"); //"Could not create Allocation Hdr";
                    return DocActionVariables.STATUS_INVALID;
                }
            }

            //*****************************END*********************************************************************************//
            MCashLine[] lines = GetLines(false);
            MInvoice invoice = null; // is created for getting detail from Invoice Header
            DataSet dsPaymentMethod = null;
            DataSet dsPaymentBaseType = null;
            if (OrderPayScheduleExists == 1)
            {
                //VA230:Get Payment Method having PaymentBaseType Cash only
                dsPaymentMethod = DB.ExecuteDataset(@"SELECT VA009_PaymentMethod_ID,VA009_PaymentMode, VA009_PaymentType, VA009_PaymentTrigger,VA009_PaymentBaseType FROM VA009_PaymentMethod
                                          WHERE IsActive = 'Y' AND VA009_PaymentBaseType='B' AND  AD_Client_ID = " + GetAD_Client_ID() +
                                          " AND AD_Org_ID IN(0," + GetAD_Org_ID() + ") ORDER BY AD_Org_ID DESC", null, Get_Trx());
                //Get distinct OrderPaySchedule ids from Cash lines
                List<int> ids = lines.Select(x => x.GetVA009_OrderPaySchedule_ID()).Distinct().ToList();
                if (ids.Count > 0)
                {
                    dsPaymentBaseType = GetOrderPaySchedulePaymentBaseTypeData(ids);
                }
            }
            for (int i = 0; i < lines.Length; i++)
            {
                MCashLine line = lines[i];

                if (Util.GetValueOfInt(line.GetC_InvoicePaySchedule_ID()) != 0)
                {
                    MInvoicePaySchedule paySch = new MInvoicePaySchedule(GetCtx(), Util.GetValueOfInt(line.GetC_InvoicePaySchedule_ID()), Get_TrxName());
                    if (paySch != null) //if schedule not found or deleted 
                    {
                        paySch.SetC_CashLine_ID(line.GetC_CashLine_ID());
                        if (!paySch.Save())
                        {
                            pp = VLogger.RetrieveError();
                            if (pp != null)
                                _processMsg = pp.GetName();
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                }
                //VA230:Check if Order Payment Schedule Column exists
                else if (line.Get_ColumnIndex("VA009_OrderPaySchedule_ID") >= 0 && Util.GetValueOfInt(line.GetVA009_OrderPaySchedule_ID()) != 0)
                {
                    Decimal basePaidAmt = line.GetAmount();

                    if (GetCtx().GetContextAsInt("$C_Currency_ID") != line.GetC_Currency_ID())
                    {
                        basePaidAmt = MConversionRate.Convert(GetCtx(), basePaidAmt, line.GetC_Currency_ID(), GetCtx().GetContextAsInt("$C_Currency_ID"), GetDateAcct(), line.GetC_ConversionType_ID(), line.GetAD_Client_ID(), line.GetAD_Org_ID());
                    }

                    StringBuilder sql = new StringBuilder();
                    //VA230:Update paid amount and payment method related detail
                    sql.Append("UPDATE VA009_OrderPaySchedule SET VA009_IsPaid='Y',C_CashLine_ID=" + line.GetC_CashLine_ID() +
                                    @" , VA009_PaidAmntInvce = " + Math.Abs(line.GetAmount()) +
                                    @" , VA009_PaidAmnt = " + Math.Abs(basePaidAmt) +
                                    @" , VA009_ExecutionStatus = 'I' ");
                    if (dsPaymentMethod != null && dsPaymentMethod.Tables.Count > 0 && dsPaymentMethod.Tables[0].Rows.Count > 0)
                    {
                        if (dsPaymentBaseType != null && dsPaymentBaseType.Tables.Count > 0 && dsPaymentBaseType.Tables[0].Rows.Count > 0)
                        {
                            DataRow[] dr = dsPaymentBaseType.Tables[0].Select("VA009_OrderPaySchedule_ID=" + Util.GetValueOfInt(line.GetVA009_OrderPaySchedule_ID()));
                            if (dr != null && dr.Length > 0)
                            {
                                //VA230:If existing paymentbasetype not equal to cash journal paymentbasetype (not cash type)
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentBaseType"]))
                            && Util.GetValueOfString(dr[0]["VA009_PaymentBaseType"]) != Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentBaseType"]))
                                {
                                    if (!String.IsNullOrEmpty(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentMode"])))
                                        sql.Append(",VA009_PaymentMode='" + Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentMode"]) + "'");
                                    if (!String.IsNullOrEmpty(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentType"])))
                                        sql.Append(",VA009_PaymentType='" + Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentType"]) + "'");
                                    if (!String.IsNullOrEmpty(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentType"])))
                                        sql.Append(",VA009_PaymentTrigger='" + Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentTrigger"]) + "'");
                                    if (Util.GetValueOfInt(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]) > 0)
                                        sql.Append(",VA009_PaymentMethod_ID=" + Util.GetValueOfInt(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]));
                                }
                            }
                        }
                    }
                    sql.Append(" WHERE VA009_OrderPaySchedule_ID=" + line.GetVA009_OrderPaySchedule_ID());
                    //VA230:Update CashLineID Reference and Paid Status on Order Pay Schedule
                    if (Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, Get_TrxName())) <= 0)
                    {
                        _processMsg = Msg.GetMsg(GetCtx(), "VA009_OrderPayScheduleNotUpdated");
                        log.Log(Level.SEVERE, "Order pay schedule not updated for CashLineId = " + line.GetC_CashLine_ID() +
                                   " Query is: " + sql.ToString());
                        return DocActionVariables.STATUS_INVALID;
                    }
                }
                else
                {
                    int[] InvoicePaySchedule_ID = MInvoicePaySchedule.GetAllIDs("C_InvoicePaySchedule", "C_Invoice_ID = " + line.GetC_Invoice_ID() + @" AND C_InvoicePaySchedule_ID NOT IN 
                    (SELECT NVL(C_InvoicePaySchedule_ID,0) FROM C_InvoicePaySchedule WHERE C_Payment_ID IN (SELECT NVL(C_Payment_ID,0) FROM C_InvoicePaySchedule) UNION 
                    SELECT NVL(C_InvoicePaySchedule_ID,0) FROM C_InvoicePaySchedule  WHERE C_Cashline_ID IN (SELECT NVL(C_Cashline_ID,0) FROM C_InvoicePaySchedule))", Get_TrxName());

                    foreach (int invocePay in InvoicePaySchedule_ID)
                    {
                        MInvoicePaySchedule paySch = new MInvoicePaySchedule(GetCtx(), invocePay, Get_TrxName());
                        if (paySch != null)      //if schedule not found or deleted 
                        {
                            paySch.SetC_CashLine_ID(line.GetC_CashLine_ID());
                            paySch.Save();
                        }
                    }
                }

                if (MCashLine.CASHTYPE_Invoice.Equals(line.GetCashType()))
                {
                    bool differentCurrency = GetC_Currency_ID() != line.GetC_Currency_ID();
                    MAllocationHdr hdr = alloc;
                    if (differentCurrency)
                    {
                        hdr = new MAllocationHdr(GetCtx(), false,
                            GetDateAcct(), line.GetC_Currency_ID(),
                            Msg.Translate(GetCtx(), "C_Cash_ID") + ": " + GetName(), Get_TrxName());
                        hdr.SetAD_Org_ID(GetAD_Org_ID());
                        // Update conversion type from invoice to view allocation (required for posting)
                        invoice = MInvoice.Get(GetCtx(), line.GetC_Invoice_ID());
                        if (hdr.Get_ColumnIndex("C_ConversionType_ID") > 0 && invoice != null && invoice.Get_ID() > 0)
                        {
                            hdr.SetC_ConversionType_ID(invoice.GetC_ConversionType_ID());
                        }
                        if (!hdr.Save())
                        {
                            pp = VLogger.RetrieveError();
                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                _processMsg = Msg.GetMsg(GetCtx(), "VIS_AllocationHdrNotSaved") + ", " + pp.GetName();
                            else
                                _processMsg = Msg.GetMsg(GetCtx(), "VIS_AllocationHdrNotSaved");//"Could not create Allocation Hdr";
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                    else if (hdr.Get_ColumnIndex("C_ConversionType_ID") > 0 && hdr.GetC_ConversionType_ID() == 0)
                    {
                        // when invoice having same currency as on cash journal then Update conversion type from invoice to view allocation (required for posting)
                        invoice = MInvoice.Get(GetCtx(), line.GetC_Invoice_ID());
                        if (hdr.Get_ColumnIndex("C_ConversionType_ID") > 0 && invoice != null && invoice.Get_ID() > 0)
                        {
                            hdr.SetC_ConversionType_ID(invoice.GetC_ConversionType_ID());
                        }
                        if (!hdr.Save())
                        {
                            pp = VLogger.RetrieveError();
                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                _processMsg = Msg.GetMsg(GetCtx(), "VIS_AllocationHdrNotSaved") + ", " + pp.GetName();
                            else
                                _processMsg = Msg.GetMsg(GetCtx(), "VIS_AllocationHdrNotSaved"); //"Could not create Allocation Hdr";
                            return DocActionVariables.STATUS_INVALID;
                        }
                        else
                        {
                            // update reference vice versa
                            alloc = hdr;
                        }
                    }
                    //	Allocation Line
                    MAllocationLine aLine = new MAllocationLine(hdr, line.GetAmount(),
                        line.GetDiscountAmt(), line.GetWriteOffAmt(), line.GetOverUnderAmt());
                    aLine.SetC_Invoice_ID(line.GetC_Invoice_ID());
                    aLine.SetC_CashLine_ID(line.GetC_CashLine_ID());
                    if (Env.IsModuleInstalled("VA009_"))
                    {
                        aLine.SetC_InvoicePaySchedule_ID(line.GetC_InvoicePaySchedule_ID());
                    }

                    // Added by Amit 31-7-2015 VAMRP
                    // Change : Set Transaction Date on View Allocation window
                    if (Env.HasModulePrefix("VAMRP_", out mInfo))
                    {
                        aLine.SetDateTrx(GetStatementDate());
                    }
                    // End Amit

                    if (!aLine.Save())
                    {
                        _processMsg = "Could not create Allocation Line";
                        return DocActionVariables.STATUS_INVALID;
                    }
                    if (differentCurrency)
                    {
                        //	Should start WF
                        hdr.ProcessIt(DocActionVariables.ACTION_COMPLETE);
                        hdr.Save();
                    }
                }
                else if (MCashLine.CASHTYPE_BankAccountTransfer.Equals(line.GetCashType()))
                {
                    //	Payment just as intermediate info
                    MPayment pay = new MPayment(GetCtx(), 0, Get_TrxName());
                    pay.SetAD_Org_ID(GetAD_Org_ID());
                    String documentNo = GetName();
                    pay.SetDocumentNo(documentNo);
                    pay.SetR_PnRef(documentNo);
                    pay.Set_Value("TrxType", "X");		//	Transfer
                    pay.Set_Value("TenderType", "X");
                    //                    
                    pay.SetC_BankAccount_ID(line.GetC_BankAccount_ID());

                    pay.SetC_DocType_ID(!line.GetVSS_PAYMENTTYPE().Equals(MCashLine.VSS_PAYMENTTYPE_Receipt));

                    pay.SetDateTrx(GetStatementDate());
                    pay.SetDateAcct(GetDateAcct());

                    // JID_1285: On Cash Journal completion while creating the payment it should create Payment always in Positive Amount in case of Bank Transfer.
                    pay.SetAmount(line.GetC_Currency_ID(), Math.Abs(line.GetAmount()));	//	Transfer

                    // JID_1244: On Cash Journal completion while creating the payment it should copy the check number, check date, MICR no and check valid date fields on the payment window
                    // line.GetVSS_PAYMENTTYPE() == MCashLine.VSS_PAYMENTTYPE_Receipt && 
                    if (line.GetTransferType() == MCashLine.TRANSFERTYPE_Check)
                    {
                        pay.SetAccountNo(line.GetAccountNo());
                        pay.SetRoutingNo(line.GetRoutingNo());
                        pay.SetCheckNo(line.GetCheckNo());
                        pay.SetCheckDate(line.GetCheckDate());
                        pay.SetMicr(line.GetMicr());
                        pay.SetValidMonths(line.GetValidMonths());
                    }

                    // JID_1244: On Payment Window Need to set Payment Method on Completion of Cash Journal  IF Check than "Check" if cash than "Direct Deposit"
                    if (Env.IsModuleInstalled("VA009_"))
                    {
                        string qry = "SELECT MAX(VA009_PaymentMethod_ID) FROM VA009_PaymentMethod WHERE IsActive = 'Y' AND AD_Client_ID = " + GetAD_Client_ID() + " AND VA009_PaymentBaseType = " +
                            (line.GetTransferType() == MCashLine.TRANSFERTYPE_Check ? "'S'" : "'T'") + " AND AD_Org_ID IN (0, " + GetAD_Org_ID() + ")";
                        int paymethod_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, Get_TrxName()));
                        pay.SetVA009_PaymentMethod_ID(paymethod_ID);
                    }

                    // JID_1244: If Bank account currency is other than cash journal currency. whatever conversion type is set, same need to set on payment window when create bank transafer
                    pay.SetC_ConversionType_ID(line.GetC_ConversionType_ID());

                    //JID_1244: Need to set Cash Line refercne on payment window, when payment is created from bank account transfer. Also this field need to make read only.
                    pay.Set_Value("C_CashLine_ID", line.Get_ID());

                    pay.SetDescription(line.GetDescription());
                    pay.SetDocStatus(MPayment.DOCSTATUS_Closed);
                    pay.SetDocAction(MPayment.DOCACTION_None);
                    pay.SetPosted(true);
                    pay.SetIsAllocated(true);	//	Has No Allocation!
                    pay.SetProcessed(true);
                    if (!pay.Save())
                    {
                        //_processMsg = "Could not create Payment";
                        // JID_1244: Need to correct message. If txn date is greater than system date on completion of cash journal systen is giving error
                        pp = VLogger.RetrieveError();
                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                            _processMsg = Msg.GetMsg(GetCtx(), "PaymentNotCreated") + ", " + pp.GetName();
                        else
                            _processMsg = Msg.GetMsg(GetCtx(), "PaymentNotCreated");
                        return DocActionVariables.STATUS_INVALID;
                    }

                    #region To Set Unmatched Balance On Bank Account To Balance the effect of Bank Statement
                    //....Arpit..Asked by Ashish Gandhi
                    else
                    {
                        MBankAccount bankAcct = new MBankAccount(GetCtx(), line.GetC_BankAccount_ID(), Get_TrxName());
                        if (line.GetC_Currency_ID() == bankAcct.GetC_Currency_ID()) //If same currency as Bank Account
                            bankAcct.SetUnMatchedBalance(Decimal.Add(bankAcct.GetUnMatchedBalance(), Decimal.Negate(line.GetAmount())));
                        else
                        {    //Currency conversion in Case of different currencies
                            decimal? amt = Util.GetValueOfDecimal(MConversionRate.Convert(GetCtx(), line.GetAmount(), line.GetC_Currency_ID(), bankAcct.GetC_Currency_ID(), GetDateAcct(),
                                line.GetC_ConversionType_ID(), line.GetAD_Client_ID(), line.GetAD_Org_ID()));
                            if (amt == null || amt == 0)
                            {
                                //_processMsg = "Could not convert C_Currency_ID=" + line.GetC_Currency_ID()
                                //    + " to Bank Account C_Currency_ID=" + bankAcct.GetC_Currency_ID();                                

                                MConversionType conv = MConversionType.Get(GetCtx(), line.GetC_ConversionType_ID());
                                _processMsg = Msg.GetMsg(GetCtx(), "NoConversion") + MCurrency.GetISO_Code(GetCtx(), line.GetC_Currency_ID()) + Msg.GetMsg(GetCtx(), "ToBankCurrency")
                                    + MCurrency.GetISO_Code(GetCtx(), bankAcct.GetC_Currency_ID()) + " - " + Msg.GetMsg(GetCtx(), "ConversionType") + conv.GetName();
                                return DocActionVariables.STATUS_INVALID;
                            }
                            bankAcct.SetUnMatchedBalance(Decimal.Add(bankAcct.GetUnMatchedBalance(), Decimal.Negate(Util.GetValueOfDecimal(amt))));
                        }
                        if (!bankAcct.Save(Get_TrxName()))
                        {
                            _processMsg = "Could not Update Unmatched Balance";
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                    //Arpit
                    #endregion
                }
                // Added to Update Open Balance of Business Partner
                // Code commented while open amount is updating twice for Business partner by VIvek on 28/11/2017
                //else if (MCashLine.CASHTYPE_BusinessPartner.Equals(line.GetCashType()))
                //{
                //    if (line.GetC_BPartner_ID() != 0)
                //    {
                //        string msg = SetBPOpenbalence(line);
                //        if (msg != "")
                //        {
                //            _processMsg = msg;
                //            return DocActionVariables.STATUS_INVALID;
                //        }
                //    }
                //}
            }

            // Added by Amit 31-7-2015 VAMRP
            if (Env.HasModulePrefix("VAMRP_", out mInfo))
            {
                // IsPaid();
            }
            // End Amit

            //	Should start WF
            // Vikas View  Allocation Header Created only When Any Cashline CashType is "Invoice"
            if (CashTyp_Invc > 0)
            {
                //AllocationHdr have Lines then it will processit otherwise it will Delete the AllocationHdr.
                int count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(C_AllocationLine_ID) FROM C_AllocationLine " +
                    "WHERE C_AllocationHdr_ID=" + alloc.GetC_AllocationHdr_ID(), null, Get_Trx()));
                if (count > 0)
                {
                    alloc.ProcessIt(DocActionVariables.ACTION_COMPLETE);
                    alloc.Save();
                }
                else
                {
                    alloc.Delete(true, Get_Trx());
                }
            }
            // Vikas End
            //Added By Bharat 07/11/2015 (to Update Open Balance of Business Partner)
            for (int i = 0; i < lines.Length; i++)
            {
                MCashLine line = lines[i];

                if (Util.GetValueOfInt(line.GetC_InvoicePaySchedule_ID()) != 0)
                {

                }
                else
                {

                }
                //VA230:Update Open Balance in case when selected cash type is Invoice or Order
                if (MCashLine.CASHTYPE_Invoice.Equals(line.GetCashType()) || MCashLine.CASHTYPE_Order.Equals(line.GetCashType()))
                {
                    if (line.GetC_BPartner_ID() != 0)
                    {
                        string msg = SetBPOpenbalence(line);
                        if (msg != "")
                        {
                            _processMsg = msg;
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                }
                else if (MCashLine.CASHTYPE_BankAccountTransfer.Equals(line.GetCashType()))
                {

                }
                // Added to Update Open Balance of Business Partner
                else if (MCashLine.CASHTYPE_BusinessPartner.Equals(line.GetCashType()))
                {
                    if (line.GetC_BPartner_ID() != 0)
                    {
                        string msg = SetBPOpenbalence(line);
                        if (msg != "")
                        {
                            _processMsg = msg;
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                }
            }
            /* TODO - need to verify in Std functionality 
             * bpparrtener open balance updated in other class or not 
             */
            //try
            //{
            //    // Change here to set Business partner Open balance
            //    UpdateOpenBalances(alloc.GetC_AllocationHdr_ID());
            //}
            //catch (Exception ex)
            //{
            //    log.Info("Open Balances not updated ==>> " + ex.Message);
            //}   

            // update allocation as True where Cash type = Invoice
            int no = Util.GetValueOfInt(DB.ExecuteQuery("UPDATE C_CashLine SET IsAllocated = 'Y' WHERE CashType = 'I' AND C_Cash_ID = " + GetC_Cash_ID(), null, Get_Trx()));

            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }
            //
            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            if (!UpdateCompletedBalance())
            {
                _processMsg = "Could not update Header";
                return VAdvantage.Process.DocActionVariables.STATUS_INVALID;
            }

            return VAdvantage.Process.DocActionVariables.STATUS_COMPLETED;
        }

        //Update open balances after completion of Cash Journals
        public void UpdateOpenBalances(int HdrID)
        {
            MAllocationLine[] allocLines = GetLinesAllocHeader(HdrID);

            if (allocLines != null && allocLines.Length > 0)
            {
                HashSet<int> bps = new HashSet<int>();
                for (int i = 0; i < allocLines.Length; i++)
                {
                    MAllocationLine line = allocLines[i];
                    bps.Add(line.GetC_BPartner_ID());	//	reverse
                }
                UpdateBP(bps);
            }
        }

        /// <summary>
        /// Update Open Balance of BP's
        /// </summary>
        /// <param name="bps">list of business partners</param>
        private void UpdateBP(HashSet<int> bps)
        {
            log.Info("#" + bps.Count);
            IEnumerator<int> it = bps.GetEnumerator();
            it.Reset();
            while (it.MoveNext())
            {
                int C_BPartner_ID = it.Current;
                MBPartner bp = new MBPartner(GetCtx(), C_BPartner_ID, Get_TrxName());
                bp.SetTotalOpenBalance();		//	recalculates from scratch
                //	bp.setSOCreditStatus();			//	called automatically
                if (bp.Save())
                {
                    log.Fine(bp.ToString());
                }
                else
                {
                    log.Log(Level.SEVERE, "BP not updated - " + bp);
                }
            }
        }

        /// <summary>
        /// Get Lines
        /// </summary>
        /// <param name="requery">if true requery</param>
        /// <returns>lines</returns>
        public MAllocationLine[] GetLinesAllocHeader(int allocID)
        {
            MAllocationLine[] _Allocline = null;

            String sql = "SELECT * FROM C_AllocationLine WHERE C_AllocationHdr_ID=" + allocID;
            List<MAllocationLine> list = new List<MAllocationLine>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MAllocationLine line = new MAllocationLine(GetCtx(), dr, Get_TrxName());
                        //line.SetParent(this);
                        list.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            //
            _Allocline = new MAllocationLine[list.Count];
            _Allocline = list.ToArray();
            return _Allocline;
        }

        //Added By Bharat 07/11/2015 (to Update Open Balance of Business Partner)
        private string SetBPOpenbalence(MCashLine line)
        {
            string errorMsg = "";
            Decimal? UpdatedBal = 0;
            try
            {
                MBPartner bp = new MBPartner(GetCtx(), line.GetC_BPartner_ID(), Get_TrxName());

                // JID_0556 :: // Change by Lokesh Chauhan to validate watch % from BP Group, 
                // if it is 0 on BP Group then default to 90 // 12 July 2019
                //MBPGroup bpg = new MBPGroup(GetCtx(), bp.GetC_BP_Group_ID(), Get_Trx());
                //Decimal? watchPerBP = bp.GetCreditWatchPercent();
                ////preference check for credit watch percentage
                //if (watchPerBP == 0)
                //{
                //    Decimal? watchPer = bpg.GetCreditWatchPercent();
                //    if (watchPer == 0)
                //    {
                //        watchPer = 90;
                //    }
                //}

                if (bp.GetCreditStatusSettingOn() == "CH")
                {
                    //Arpit Dated 30th Nov,2017
                    // Commented Code because the amount which is of cash Line is converted into Header currency and then update into Business Partner
                    // Decimal? cashAmt = VAdvantage.Model.MConversionRate.ConvertBase(GetCtx(), Decimal.Add(Decimal.Add(line.GetAmount(), line.GetDiscountAmt()), line.GetWriteOffAmt()),
                    //GetC_Currency_ID(), GetDateAcct(), 0, GetAD_Client_ID(), GetAD_Org_ID());
                    Decimal? cashAmt = VAdvantage.Model.MConversionRate.ConvertBase(GetCtx(), Decimal.Add(Decimal.Add(line.GetAmount(), line.GetDiscountAmt()), line.GetWriteOffAmt()),
                                       line.GetC_Currency_ID(), GetDateAcct(), line.GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());
                    //Arpit
                    // not checking if cashamt is less than zero
                    // Done by Vivek on 12/07/2017 as per discussion with Mandeep sir 
                    if (cashAmt == null || cashAmt == 0)
                    {
                        //JID_0821: If Cashbook currency conversion is not found. On completion of cash journal System give error "IN".
                        MConversionType conv = MConversionType.Get(GetCtx(), line.GetC_ConversionType_ID());
                        errorMsg = Msg.GetMsg(GetCtx(), "NoConversion") + MCurrency.GetISO_Code(GetCtx(), line.GetC_Currency_ID()) + Msg.GetMsg(GetCtx(), "ToBaseCurrency")
                            + MCurrency.GetISO_Code(GetCtx(), MClient.Get(GetCtx()).GetC_Currency_ID()) + " - " + Msg.GetMsg(GetCtx(), "ConversionType") + conv.GetName();
                        return errorMsg;
                    }
                    UpdatedBal = Decimal.Subtract((Decimal)bp.GetTotalOpenBalance(), (Decimal)cashAmt);
                    // changed here to set credit Used only in case of Receipt not in payment
                    if (line.GetVSS_PAYMENTTYPE() == X_C_CashLine.VSS_PAYMENTTYPE_Receipt)
                    {
                        Decimal? newCreditAmt = bp.GetSO_CreditUsed();
                        if (newCreditAmt == null)
                            newCreditAmt = Decimal.Negate((Decimal)cashAmt);
                        else
                            newCreditAmt = Decimal.Subtract((Decimal)newCreditAmt, (Decimal)cashAmt);
                        //
                        log.Fine("TotalOpenBalance=" + bp.GetTotalOpenBalance(false) + "(" + cashAmt
                            + ", Credit=" + bp.GetSO_CreditUsed() + "->" + newCreditAmt
                            + ", Balance=" + bp.GetTotalOpenBalance(false) + " -> " + UpdatedBal);
                        bp.SetSO_CreditUsed((Decimal)newCreditAmt);
                    }
                    bp.SetTotalOpenBalance(Convert.ToDecimal(UpdatedBal));
                    bp.SetSOCreditStatus();
                    if (!bp.Save(Get_TrxName()))
                    {
                        errorMsg = "Could not update Business Partner";
                        return errorMsg;
                    }
                }
                //Arpit to update Business partner Location --set Total Open balance and SO credit Used
                if (bp.GetCreditStatusSettingOn() == "CL")
                {
                    Decimal? cashAmt = VAdvantage.Model.MConversionRate.ConvertBase(GetCtx(), Decimal.Add(Decimal.Add(line.GetAmount(), line.GetDiscountAmt()), line.GetWriteOffAmt()),
                                       line.GetC_Currency_ID(), GetDateAcct(), line.GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());
                    if (cashAmt == null || cashAmt == 0)
                    {
                        //_processMsg = "Could not convert C_Currency_ID=" + GetC_Currency_ID()
                        //        + " to base C_Currency_ID=" + MClient.Get(GetCtx()).GetC_Currency_ID();
                        //return DocActionVariables.STATUS_INVALID;

                        //JID_0821: If Cashbook currency conversion is not found. On completion of cash journal System give error "IN".
                        MConversionType conv = MConversionType.Get(GetCtx(), line.GetC_ConversionType_ID());
                        errorMsg = Msg.GetMsg(GetCtx(), "NoConversion") + MCurrency.GetISO_Code(GetCtx(), line.GetC_Currency_ID()) + Msg.GetMsg(GetCtx(), "ToBaseCurrency")
                            + MCurrency.GetISO_Code(GetCtx(), MClient.Get(GetCtx()).GetC_Currency_ID()) + " - " + Msg.GetMsg(GetCtx(), "ConversionType") + conv.GetName();
                        return errorMsg;
                    }
                    UpdatedBal = Decimal.Subtract((Decimal)bp.GetTotalOpenBalance(), (Decimal)cashAmt);

                    // changed here to set credit Used only in case of Receipt not in payment
                    if (line.GetVSS_PAYMENTTYPE() == X_C_CashLine.VSS_PAYMENTTYPE_Receipt)
                    {
                        Decimal? newCreditAmt = bp.GetSO_CreditUsed();
                        if (newCreditAmt == null)
                            newCreditAmt = Decimal.Negate((Decimal)cashAmt);
                        else
                            newCreditAmt = Decimal.Subtract((Decimal)newCreditAmt, (Decimal)cashAmt);
                        //
                        log.Fine("TotalOpenBalance=" + bp.GetTotalOpenBalance(false) + "(" + cashAmt
                            + ", Credit=" + bp.GetSO_CreditUsed() + "->" + newCreditAmt
                            + ", Balance=" + bp.GetTotalOpenBalance(false) + " -> " + UpdatedBal);
                        bp.SetSO_CreditUsed((Decimal)newCreditAmt);
                    }

                    bp.SetTotalOpenBalance(Convert.ToDecimal(UpdatedBal));
                    bp.SetSOCreditStatus();
                    if (bp.Save(Get_TrxName()))
                    {
                        MBPartnerLocation loc = null;
                        if (line.GetC_Invoice_ID() > 0)
                        {
                            MInvoice inv = new MInvoice(GetCtx(), line.GetC_Invoice_ID(), Get_TrxName());
                            loc = new MBPartnerLocation(GetCtx(), inv.GetC_BPartner_Location_ID(), Get_TrxName());
                        }
                        else
                        {
                            loc = new MBPartnerLocation(GetCtx(), line.GetC_BPartner_Location_ID(), Get_TrxName());
                        }
                        //	Total Balance
                        Decimal? newBalance = loc.GetTotalOpenBalance();
                        if (newBalance == null)
                            newBalance = Env.ZERO;
                        newBalance = Decimal.Subtract((Decimal)newBalance, (Decimal)cashAmt);
                        // changed here to set credit Used only in case of Receipt not in payment
                        if (line.GetVSS_PAYMENTTYPE() == "R") //R-receipt , P-payment
                        {
                            Decimal? newCreditAmt = loc.GetSO_CreditUsed();
                            if (newCreditAmt == null)
                                newCreditAmt = Decimal.Negate((Decimal)cashAmt);
                            else
                                newCreditAmt = Decimal.Subtract((Decimal)newCreditAmt, (Decimal)cashAmt);
                            //
                            log.Fine("TotalOpenBalance=" + loc.GetTotalOpenBalance() + "(" + cashAmt
                                + ", Credit=" + loc.GetSO_CreditUsed() + "->" + newCreditAmt
                                + ", Balance=" + loc.GetTotalOpenBalance() + " -> " + newBalance);
                            loc.SetSO_CreditUsed((Decimal)newCreditAmt);
                        }	//	SO
                        else
                        {
                            //In case of Payment against Customer- e.g. Credit memo against a customer
                            if (bp.IsCustomer())
                            {
                                log.Fine("Payment Amount =(" + cashAmt
                                    + ") Balance=" + loc.GetTotalOpenBalance() + " -> " + newBalance);

                                Decimal? newCreditAmt = loc.GetSO_CreditUsed();
                                if (newCreditAmt == null)
                                {
                                    newCreditAmt = 0;
                                }
                                newCreditAmt = Decimal.Subtract((Decimal)newCreditAmt, (Decimal)cashAmt);
                                loc.SetSO_CreditUsed((Decimal)newCreditAmt);
                            }
                        }
                        loc.SetTotalOpenBalance(Convert.ToDecimal(newBalance));
                        loc.SetSOCreditStatus();
                        if (!loc.Save(Get_Trx()))
                        {
                            _processMsg = "Could not update Business Partner Location";
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                    else
                    {
                        errorMsg = "Could not update Business Partner";
                        return errorMsg;
                    }
                }
                //Arpit
                #region Code commented For invoice because it is already handled in above lines //Arpit
                /*
                else
                {
                    MInvoice inv = new MInvoice(GetCtx(), line.GetC_Invoice_ID(), Get_TrxName());
                    MBPartnerLocation loc = new MBPartnerLocation(GetCtx(), inv.GetC_BPartner_Location_ID(), Get_TrxName());
                    //Arpit Dated 30th Nov,2017
                    // Commented Code because the amount which is of cash Line is converted into Header currency and then update into Business Partner
                    //  Decimal? cashAmt = VAdvantage.Model.MConversionRate.ConvertBase(GetCtx(), Decimal.Add(Decimal.Add(line.GetAmount(), line.GetDiscountAmt()), line.GetWriteOffAmt()),
                    //GetC_Currency_ID(), GetDateAcct(), 0, GetAD_Client_ID(), GetAD_Org_ID());
                    Decimal? cashAmt = VAdvantage.Model.MConversionRate.ConvertBase(GetCtx(), Decimal.Add(Decimal.Add(line.GetAmount(), line.GetDiscountAmt()), line.GetWriteOffAmt()),
                                       line.GetC_Currency_ID(), GetDateAcct(), 0, GetAD_Client_ID(), GetAD_Org_ID());
                    //Arpit
                    Decimal? newCreditAmt = 0;
                    if (cashAmt > 0)
                    {
                        UpdatedBal = Decimal.Subtract((Decimal)loc.GetTotalOpenBalance(), (Decimal)cashAmt);

                        newCreditAmt = loc.GetSO_CreditUsed();
                        if (newCreditAmt == null)
                            newCreditAmt = Decimal.Negate((Decimal)cashAmt);
                        else
                            newCreditAmt = Decimal.Subtract((Decimal)newCreditAmt, (Decimal)cashAmt);
                        //
                        log.Fine("TotalOpenBalance=" + loc.GetTotalOpenBalance() + "(" + cashAmt
                            + ", Credit=" + loc.GetSO_CreditUsed() + "->" + newCreditAmt
                            + ", Balance=" + loc.GetTotalOpenBalance() + " -> " + UpdatedBal);
                        loc.SetSO_CreditUsed((Decimal)newCreditAmt);
                    }
                    else
                    {
                        UpdatedBal = Decimal.Subtract((Decimal)loc.GetTotalOpenBalance(), (Decimal)cashAmt);
                        log.Fine("Payment Amount =" + line.GetAmount() + "(" + cashAmt
                            + ") Balance=" + loc.GetTotalOpenBalance() + " -> " + UpdatedBal);
                    }
                    loc.SetTotalOpenBalance(Convert.ToDecimal(UpdatedBal));
                    if (!loc.Save(Get_TrxName()))
                    {
                        errorMsg = "Could not update Business Partner Location";
                        return errorMsg;

                    }
                    Decimal? bptotalopenbal = Decimal.Negate((Decimal)cashAmt) + bp.GetTotalOpenBalance();
                    Decimal? bpSOcreditUsed = Decimal.Negate((Decimal)cashAmt) + bp.GetSO_CreditUsed();
                    bp.SetSO_CreditUsed(bpSOcreditUsed);
                    bp.SetTotalOpenBalance(bptotalopenbal);
                    if (bp.GetSO_CreditLimit() > 0)
                    {
                        if (((bpSOcreditUsed / bp.GetSO_CreditLimit()) * 100) <= 90)
                        {
                            bp.SetSOCreditStatus("O");
                        }
                    }
                    if (loc.GetSO_CreditLimit() > 0)
                    {
                        if (((newCreditAmt / loc.GetSO_CreditLimit()) * 100) <= 90)
                        {
                            loc.SetSOCreditStatus("O");
                        }
                    }
                    if (!bp.Save())
                    {
                        _processMsg = "Could not update Business Partner";
                        return DocActionVariables.STATUS_INVALID;
                    }
                    //loc.SetSOCreditStatus();

                }
                     * */
                #endregion

            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, "Exception found while updating Open Balance of Business Partner", ex);
            }
            return errorMsg;

        }

        // Added By Amit 31-7-2015 VAMRP Not Used
        private void IsPaid()
        {
            string sql = "select c_invoice_id from c_cashline where c_cash_id =" + GetC_Cash_ID();
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, Get_TrxName());
                ArrayList inv = new ArrayList();
                while (idr.Read())
                {
                    inv.Add(idr[0]);
                }
                idr = null;

                for (int i = 0; i < inv.Count; i++)
                {
                    sql = "select sum(amount),sum(writeoffamt),sum(discountamt) from c_cashline where c_invoice_id = " + Util.GetValueOfInt(inv[i]);
                    Decimal cashAmt = 0;
                    idr = DB.ExecuteReader(sql, null, Get_TrxName());
                    if (idr.Read())
                    {
                        cashAmt = Decimal.Add(Decimal.Add(Util.GetValueOfDecimal(idr[0]), Util.GetValueOfDecimal(idr[1])), Util.GetValueOfDecimal(idr[2]));
                    }
                    idr = null;
                    sql = "select sum(payamt), sum(writeoffamt), sum(discountamt) from c_payment where c_invoice_id =" + Util.GetValueOfInt(inv[i]);
                    Decimal totalPayAmount = 0;
                    idr = DB.ExecuteReader(sql, null, Get_TrxName());
                    if (idr.Read())
                    {
                        totalPayAmount = Decimal.Add(Decimal.Add(Util.GetValueOfDecimal(idr[0]), Util.GetValueOfDecimal(idr[1])), Util.GetValueOfDecimal(idr[2]));
                    }
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                    sql = "select grandtotal from c_invoice where c_invoice_id = " + Util.GetValueOfInt(inv[i]);
                    Decimal GT = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
                    Decimal total = Decimal.Add(cashAmt, totalPayAmount);
                    if (total >= GT)
                    {
                        sql = "update c_invoice set ispaid = 'Y' where c_invoice_id = " + Util.GetValueOfInt(inv[i]);
                        int res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_TrxName()));
                    }
                }
            }
            catch
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
        }
        // End Amit

        private bool UpdateCompletedBalance()
        {
            MCashBook cashbook = new MCashBook(GetCtx(), GetC_CashBook_ID(), Get_TrxName());
            cashbook.SetCompletedBalance(Decimal.Add(cashbook.GetCompletedBalance(), GetStatementDifference()));
            cashbook.SetRunningBalance(Decimal.Subtract(cashbook.GetRunningBalance(), GetStatementDifference()));

            if (!cashbook.Save())
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Void Document.
        /// </summary>
        /// <returns>true if success</returns>
        public bool VoidIt()
        {
            log.Info(ToString());

            MCashBook cashbook = new MCashBook(GetCtx(), GetC_CashBook_ID(), Get_TrxName());
            int C_CASHBOOKLINE_ID = 0;

            string sql = "SELECT C_CASHBOOKLINE_ID FROM C_CASHBOOKLINE WHERE C_CASHBOOK_ID="
                            + GetC_CashBook_ID() + " AND DATEACCT="
                            + DB.TO_DATE(GetDateAcct()) + " AND AD_ORG_ID=" + GetAD_Org_ID();

            C_CASHBOOKLINE_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));

            MCashbookLine cashbookLine = new MCashbookLine(GetCtx(), C_CASHBOOKLINE_ID, Get_TrxName());
            if (C_CASHBOOKLINE_ID == 0)
            {
                cashbookLine.SetC_CashBook_ID(GetC_CashBook_ID());
                cashbookLine.SetAD_Org_ID(cashbook.GetAD_Org_ID());
                cashbookLine.SetAD_Client_ID(GetAD_Client_ID());
                cashbookLine.SetEndingBalance(Decimal.Subtract(cashbook.GetCompletedBalance(), GetStatementDifference()));
            }
            else
            {
                cashbookLine.SetEndingBalance(Decimal.Subtract(cashbookLine.GetEndingBalance(), GetStatementDifference()));
            }
            cashbookLine.SetDateAcct(GetDateAcct());
            cashbookLine.SetStatementDifference(Decimal.Subtract(cashbookLine.GetStatementDifference(), GetStatementDifference()));

            if (!cashbookLine.Save())
            {
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null && !string.IsNullOrEmpty(pp.GetName()))
                    _processMsg = Msg.GetMsg(GetCtx(), "CashbookLineNotSaved") + " - " + pp.GetName();
                else
                    _processMsg = Msg.GetMsg(GetCtx(), "CashbookLineNotSaved");
                return false;
            }

            cashbook.SetRunningBalance(Decimal.Subtract(cashbook.GetRunningBalance(), GetStatementDifference()));

            if (!cashbook.Save())
            {
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null && !string.IsNullOrEmpty(pp.GetName()))
                    _processMsg = Msg.GetMsg(GetCtx(), "CashbookNotSaved") + " - " + pp.GetName();
                else
                    _processMsg = Msg.GetMsg(GetCtx(), "CashbookNotSaved");
                return false;
            }

            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /**
         * 	Close Document.
         * 	Cancel not delivered Qunatities
         * 	@return true if success 
         */
        public bool CloseIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_None);
            return true;
        }

        /**
         * 	Reverse Correction
         * 	@return true if success 
         */
        public bool ReverseCorrectIt()
        {
            log.Info(ToString());
            return false;
        }

        /**
         * 	Reverse Accrual - none
         * 	@return true if success 
         */
        public bool ReverseAccrualIt()
        {
            log.Info(ToString());
            return false;
        }

        /** 
         * 	Re-activate
         * 	@return true if success 
         */
        public bool ReActivateIt()
        {
            log.Info(ToString());
            SetProcessed(false);
            if (ReverseCorrectIt())
                return true;
            return false;
        }

        /**
         * 	Set Processed
         *	@param processed processed
         */
        public void SetProcessed(bool processed)
        {
            base.SetProcessed(processed);
            String sql = "UPDATE C_CashLine SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE C_Cash_ID=" + GetC_Cash_ID();
            int noLine = DB.ExecuteQuery(sql, null, Get_TrxName());
            _lines = null;
            log.Fine(processed + " - Lines=" + noLine);
        }

        /**
         * 	String Representation
         *	@return info
         */
        public String ToString()
        {
            StringBuilder sb = new StringBuilder("MCash[");
            sb.Append(Get_ID())
                .Append("-").Append(GetName())
                .Append(", Balance=").Append(GetBeginningBalance())
                .Append("->").Append(GetEndingBalance())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Get Summary
         *	@return Summary of Document
         */
        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetName());
            //	: Total Lines = 123.00 (#1)
            sb.Append(": ")
                .Append(Msg.Translate(GetCtx(), "BeginningBalance")).Append("=").Append(GetBeginningBalance())
                .Append(",")
                .Append(Msg.Translate(GetCtx(), "EndingBalance")).Append("=").Append(GetEndingBalance())
                .Append(" (#").Append(GetLines(false).Length).Append(")");
            //	 - Description
            if (GetDescription() != null && GetDescription().Length > 0)
                sb.Append(" - ").Append(GetDescription());
            return sb.ToString();
        }

        /**
         * 	Get Process Message
         *	@return clear text error message
         */
        public String GetProcessMsg()
        {
            return _processMsg;
        }

        /**
         * 	Get Document Owner (Responsible)
         *	@return AD_User_ID
         */
        public int GetDoc_User_ID()
        {
            return GetCreatedBy();
        }

        /**
         * 	Get Document Approval Amount
         *	@return amount difference
         */
        public Decimal GetApprovalAmt()
        {
            return GetStatementDifference();
        }

        /**
         * 	Get Currency
         *	@return Currency
         */
        public int GetC_Currency_ID()
        {
            return GetCashBook().GetC_Currency_ID();
        }



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
        /// <summary>
        /// Get OrderPaySchedule ids and PaymentBaseType of selected PaymentMethod on OrderPaySchedule
        /// Author:VA230
        /// </summary>
        /// <param name="ids">OrderPaySchedule ids list</param>
        /// <returns>Order Pay Schedule Dataset</returns>
        public static DataSet GetOrderPaySchedulePaymentBaseTypeData(List<int> ids)
        {
            decimal totalPages = ids.Count();
            //to fixed 999 ids per page
            totalPages = Math.Ceiling(totalPages / 999);

            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT PM.VA009_PaymentBaseType,PS.VA009_OrderPaySchedule_ID FROM VA009_PaymentMethod PM 
                                    INNER JOIN VA009_OrderPaySchedule PS ON PS.VA009_PaymentMethod_ID=PM.VA009_PaymentMethod_ID");
            List<string> schedule_Ids = new List<string>();
            //loop through each page
            for (int i = 0; i <= totalPages - 1; i++)
            {
                //get comma seperated product ids max 999
                schedule_Ids.Add(string.Join(",", ids.Select(r => r.ToString()).Skip(i * 999).Take(999)));
            }
            if (schedule_Ids.Count > 0)
            {
                //append product in sql statement use OR keyword when records are more than 999
                for (int i = 0; i < schedule_Ids.Count; i++)
                {
                    if (i == 0)
                    {
                        sql.Append(@" AND (");
                    }
                    else
                    {
                        sql.Append(" OR ");
                    }
                    sql.Append(" PS.VA009_OrderPaySchedule_ID IN (" + schedule_Ids[i] + @")");
                    if (i == schedule_Ids.Count - 1)
                    {
                        sql.Append(" ) ");
                    }
                }
            }
            DataSet ds = DB.ExecuteDataset(sql.ToString());
            return ds;
        }
    }
}
