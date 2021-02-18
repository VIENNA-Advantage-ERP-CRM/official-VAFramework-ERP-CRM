﻿/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Doc
 * Purpose        : Posting Document Root.
 * Class Used     : AccountingInterface
 * Chronological    Development
 * Raghunandan      12-Jan-2010
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;

using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

using System.Data.SqlClient;
using VAdvantage.Report;

namespace VAdvantage.Acct
{
    public abstract class Doc : AccountingInterface
    {
        #region Private Variables
        //Document Status         
        public static String STATUS_NotPosted = "N";
        //Document Status         
        public static String STATUS_NotBalanced = "b";
        //Document Status         
        public static String STATUS_NotConvertible = "c";
        //Document Status         
        public static String STATUS_PeriodClosed = "p";
        //Document Status         
        public static String STATUS_InvalidAccount = "i";
        //Document Status         
        public static String STATUS_PostPrepared = "y";
        //Document Status         
        public static String STATUS_Posted = "Y";
        //Document Status         
        public static String STATUS_Error = "E";
        //Document Status         
        public static String STATUS_DocumentError = "e";
        //Static Log						
        protected static VLogger _log = VLogger.GetVLogger(typeof(Doc).FullName);
        //Log	per Document			
        protected VLogger log = null;
        //Accounting Schema Array     
        private MVABAccountBook[] _ass = null;
        // Context						
        private Ctx _ctx = null;
        //Transaction Name			
        private Trx _trx = null;
        // The Document				
        protected PO _po = null;
        // Document Type      		
        private String _DocumentType = null;
        // Document Status      	
        private String _DocStatus = null;
        // Document No      		
        private String _DocumentNo = null;
        // Description      		
        private String _Description = null;
        // GL Category      		
        private int _VAGL_Group_ID = 0;
        //GL Period					
        private MVABYearPeriod _period = null;
        //Period ID					
        private int _VAB_YearPeriod_ID = 0;
        // Location From			
        private int _C_LocFrom_ID = 0;
        // Location To				
        private int _C_LocTo_ID = 0;
        // Accounting Date			
        private DateTime? _DateAcct = null;
        // Document Date			
        private DateTime? _DateDoc = null;
        // Tax Included				
        private bool _TaxIncluded = false;
        //Is (Source) Multi-Currency Document - i.e. the document has different currencies
        //  (if true, the document will not be source balanced)     
        private bool _MultiCurrency = false;
        //BP Sales Region    		
        private int _BP_VAB_SalesRegionState_ID = -1;
        // B Partner	    		
        private int _VAB_BusinessPartner_ID = -1;
        //Bank Account				
        private int _VAB_Bank_Acct_ID = -1;
        //Cach Book					
        private int _VAB_CashBook_ID = -1;
        //Currency					
        private int _VAB_Currency_ID = -1;
        /**********************/
        /** Work Order Class            */
        private int _workorder_id = -1;
        /** Work Center                 */
        protected int _workcenter_id = -1;
        /** Cost Element ID for Work Center Cost */
        protected int _costelement_id = -1;
        /** if the document is reposted */
        protected Boolean _repost = false;
        //Contained Doc Lines		
        protected DocLine[] _lines;
        //Facts                       
        private List<Fact> _fact = null;
        //No Currency in Document Indicator (-1)	
        public static int NO_CURRENCY = -2;
        //Actual Document Status
        protected String _status = null;
        //Error Message			
        public String _error = null;
        //	Account Type - Invoice - Charge 
        public static int ACCTTYPE_Charge = 0;
        //	Account Type - Invoice - AR  
        public static int ACCTTYPE_C_Receivable = 1;
        //	Account Type - Invoice - AP  
        public static int ACCTTYPE_V_Liability = 2;
        //	Account Type - Invoice - AP Service 
        public static int ACCTTYPE_V_Liability_Services = 3;
        //	Account Type - Invoice - AR Service 
        public static int ACCTTYPE_C_Receivable_Services = 4;

        // Account Type - Payment - Unallocated 
        public static int ACCTTYPE_UnallocatedCash = 10;
        // Account Type - Payment - Transfer 
        public static int ACCTTYPE_BankInTransit = 11;
        // Account Type - Payment - Selection 
        public static int ACCTTYPE_PaymentSelect = 12;
        // Account Type - Payment - Prepayment
        public static int ACCTTYPE_C_Prepayment = 13;
        // Account Type - Payment - Prepayment
        public static int ACCTTYPE_V_Prepayment = 14;

        /////******************* Change
        // Account Type - Payment - Prepayment
        public static int ACCTTYPE_E_Prepayment = 15;

        // Account Type - Cash     - Asset 
        public static int ACCTTYPE_CashAsset = 20;
        // Account Type - Cash     - Transfer
        public static int ACCTTYPE_CashTransfer = 21;
        // Account Type - Cash     - Expense 
        public static int ACCTTYPE_CashExpense = 22;
        // Account Type - Cash     - Receipt 
        public static int ACCTTYPE_CashReceipt = 23;
        // Account Type - Cash     - Difference 
        public static int ACCTTYPE_CashDifference = 24;

        // Account Type - Allocation - Discount Expense (AR)
        public static int ACCTTYPE_DiscountExp = 30;
        // Account Type - Allocation - Discount Revenue (AP)
        public static int ACCTTYPE_DiscountRev = 31;
        // Account Type - Allocation  - Write Off 
        public static int ACCTTYPE_WriteOff = 32;

        // Account Type - Bank Statement - Asset  
        public static int ACCTTYPE_BankAsset = 40;
        // Account Type - Bank Statement - Interest Revenue 
        public static int ACCTTYPE_InterestRev = 41;
        // Account Type - Bank Statement - Interest Exp  
        public static int ACCTTYPE_InterestExp = 42;

        // Inventory Accounts  - Differnces	
        public static int ACCTTYPE_InvDifferences = 50;
        // Inventory Accounts - NIR		
        public static int ACCTTYPE_NotInvoicedReceipts = 51;

        // Project Accounts - Assets    
        public static int ACCTTYPE_ProjectAsset = 61;
        // Project Accounts - WIP       
        public static int ACCTTYPE_ProjectWIP = 62;
        // GL Accounts - PPV Offset		
        public static int ACCTTYPE_PPVOffset = 101;
        // GL Accounts - Commitment Offset
        public static int ACCTTYPE_CommitmentOffset = 111;
        //	Amount Type - Invoice - Gross   
        public static int AMTTYPE_Gross = 0;
        //Amount Type - Invoice - Net   
        public static int AMTTYPE_Net = 1;
        //Amount Type - Invoice - Charge   
        public static int AMTTYPE_Charge = 2;
        //Source Amounts (may not all be used)	
        private Decimal[] _Amounts = new Decimal[4];
        //Quantity								
        private Decimal? _qty = null;
        //new types add on 08-March-2011
        /*************************************************************************/
        /** Work Order Material Account */
        public static int ACCTTYPE_WOMaterialAcct = 1773;
        /** Work Order Material Overhead Account */
        public static int ACCTTYPE_WOMaterialOverhdAcct = 1774;
        /** Work Order Resource Account */
        public static int ACCTTYPE_WOResourceAcct = 1775;
        /** Work Center Overhead Account */
        public static int ACCTTYPE_WCOverhdAcct = 1777;
        /** Work Order Material Variance Account */
        public static int ACCTTYPE_WOMaterialVarianceAcct = 7672;
        /** Work Order Material Overhead Variance Account */
        public static int ACCTTYPE_WOMaterialOverhdVarianceAcct = 7673;
        /** Work Order Resource Variance Account */
        public static int ACCTTYPE_WOResoureVarianceAcct = 7674;
        /** Work Order Overhead Variance Account */
        public static int ACCTTYPE_WOOverhdVarianceAcct = 7675;
        /** Work Order Scrap Account */
        public static int ACCTTYPE_WOScrapAcct = 7677;
        /** Overhead Absorption Account */
        public static int ACCTTYPE_OverhdAbsorptionAcct = 7678;  //TODO Check the account number

        #endregion


        /// <summary>
        /// Create Posting document
        /// </summary>
        /// <param name="ass">accounting schema</param>
        /// <param name="VAF_TableView_ID">Table ID of Documents</param>
        /// <param name="Record_ID">record ID to load</param>
        /// <param name="trxName">transaction name</param>
        /// <returns>Document or null</returns>
        public static Doc Get(MVABAccountBook[] ass, int VAF_TableView_ID, int Record_ID, Trx trxName)
        {
            Ctx ctx = ass[0].GetCtx();
            MVABMasterDocType dbt = MVABMasterDocType.GetForTable(ctx, VAF_TableView_ID);
            if (dbt == null)
            {
                _log.Log(Level.SEVERE, "No DocType for VAF_TableView_ID=" + VAF_TableView_ID);
                return null;
            }
            String TableName = dbt.GetTableName();
            if (TableName == null)
            {
                _log.Severe("No TableName for " + dbt);
                return null;
            }
            //
            Doc doc = null;
            StringBuilder sql = new StringBuilder("SELECT * FROM ")
                .Append(TableName)
                .Append(" WHERE ").Append(TableName).Append("_ID=" + Record_ID + " AND Processed='Y'");
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    doc = Get(ass, VAF_TableView_ID, dr, trxName);
                }
                if (dt.Rows.Count <= 0)
                {
                    _log.Severe("Not Found: " + TableName + "_ID=" + Record_ID);
                }
                dt = null;
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                if (dt != null)
                {
                    dt = null;
                }
                _log.Log(Level.SEVERE, sql.ToString(), e);
            }

            return doc;
        }

        /// <summary>
        /// Create Posting document
        /// </summary>
        /// <param name="ass">accounting schema</param>
        /// <param name="VAF_TableView_ID">Table ID of Documents</param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        /// <returns>Document</returns>
        public static Doc Get(MVABAccountBook[] ass, int VAF_TableView_ID, DataRow idr, Trx trxName)
        {
            Ctx ctx = ass[0].GetCtx();
            MVABMasterDocType dbt = MVABMasterDocType.GetForTable(ctx, VAF_TableView_ID);
            if (dbt == null)
            {
                _log.Log(Level.SEVERE, "No DocType for VAF_TableView_ID=" + VAF_TableView_ID);
                return null;
            }
            try
            {
                Doc doc = (Doc)dbt.GetAccountingInstance(ass, idr, trxName);
                return doc;
            }
            catch
            {
                _log.Log(Level.SEVERE, "Error creating Document for " + dbt);
            }
            return null;
        }

        /// <summary>
        /// Post Document
        /// </summary>
        /// <param name="ass">accounting schemata</param>
        /// <param name="VAF_TableView_ID">Transaction table</param>
        /// <param name="Record_ID">Record ID of this document</param>
        /// <param name="force">force posting</param>
        /// <param name="trxName">transaction</param>
        /// <returns>null if the document was posted or error message</returns>
        public static String PostImmediate(MVABAccountBook[] ass, int VAF_TableView_ID, int Record_ID, bool force, Trx trxName)
        {
            Doc doc = Get(ass, VAF_TableView_ID, Record_ID, trxName);
            if (doc != null)
            {
                return doc.Post(force, true);	//	repost
            }
            return "NoDoc";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="clazz"></param>
        /// <param name="idr"></param>
        /// <param name="defaultDocumentType"></param>
        /// <param name="trxName"></param>
        /// ///public Doc (MAcctSchema[] ass, Class<?> clazz, ResultSet idr, String defaultDocumentType, Trx trxName)
        public Doc(MVABAccountBook[] ass, Type clazz, IDataReader idr, String defaultDocumentType, Trx trxName)
        {

            log = VLogger.GetVLogger(this.GetType().FullName);
            _status = STATUS_Error;
            _ass = ass;
            _ctx = new Ctx(_ass[0].GetCtx().GetMap());
            _ctx.SetContext("#VAF_Client_ID", Utility.Util.GetValueOfString(_ass[0].GetVAF_Client_ID()));
            String className = clazz.GetType().Name;//.getName();
            className = className.Substring(className.LastIndexOf('.') + 1);
            try
            {
                //Constructor<?> constructor = clazz.getConstructor(new Class[]{Ctx.class, ResultSet.class, String.class});
                System.Reflection.ConstructorInfo constructor = clazz.GetConstructor(new Type[] { typeof(Ctx), typeof(IDataReader), typeof(Trx) });
                //p_po = (PO)constructor.newInstance(new Object[] { m_ctx, rs, trxName });
                _po = (PO)constructor.Invoke(new object[] { _ctx, idr, trxName });

                //_po = (PO)Activator.CreateInstance(constructor, new Object[] { _ctx, idr, trxName });
            }
            catch (Exception e)
            {
                String msg = className + ": " + e.Message;
                log.Severe(msg);
                throw new ArgumentException(msg);
            }

            //	DocStatus
            int index = _po.Get_ColumnIndex("DocStatus");
            if (index != -1)
            {
                _DocStatus = (String)_po.Get_Value(index);
            }

            //	Document Type
            SetDocumentType(defaultDocumentType);
            _trx = trxName;
            if (_trx == null)
            {
                _trx = Trx.Get("Post" + _DocumentType + _po.Get_ID());
            }
            _po.Set_Trx(_trx);

            //	Amounts
            _Amounts[0] = Env.ZERO;
            _Amounts[1] = Env.ZERO;
            _Amounts[2] = Env.ZERO;
            _Amounts[3] = Env.ZERO;
        }


        public Doc(MVABAccountBook[] ass, Type clazz, DataRow idr, String defaultDocumentType, Trx trxName)
        {

            log = VLogger.GetVLogger(this.GetType().FullName);
            _status = STATUS_Error;
            _ass = ass;
            _ctx = new Ctx(_ass[0].GetCtx().GetMap());
            _ctx.SetContext("#VAF_Client_ID", Utility.Util.GetValueOfString(_ass[0].GetVAF_Client_ID()));
            String className = clazz.GetType().Name;//.getName();
            className = className.Substring(className.LastIndexOf('.') + 1);
            try
            {
                //Constructor<?> constructor = clazz.getConstructor(new Class[]{Ctx.class, ResultSet.class, String.class});
                System.Reflection.ConstructorInfo constructor = clazz.GetConstructor(new Type[] { typeof(Ctx), typeof(DataRow), typeof(Trx) });
                //p_po = (PO)constructor.newInstance(new Object[] { m_ctx, rs, trxName });
                _po = (PO)constructor.Invoke(new object[] { _ctx, idr, trxName });
            }
            catch (Exception e)
            {
                String msg = className + ": " + e.Message;
                log.Severe(msg);
                throw new ArgumentException(msg);
            }

            //	DocStatus
            int index = _po.Get_ColumnIndex("DocStatus");
            if (index != -1)
            {
                _DocStatus = (String)_po.Get_Value(index);
            }

            //	Document Type
            SetDocumentType(defaultDocumentType);
            _trx = trxName;
            if (_trx == null)
            {
                _trx = Trx.Get("Post" + _DocumentType + _po.Get_ID());
            }
            _po.Set_TrxName(_trx);

            //	Amounts
            _Amounts[0] = Env.ZERO;
            _Amounts[1] = Env.ZERO;
            _Amounts[2] = Env.ZERO;
            _Amounts[3] = Env.ZERO;
        }

        /// <summary>
        /// Get Context
        /// </summary>
        /// <returns>context</returns>
        public Ctx GetCtx()
        {
            return _ctx;
        }

        /// <summary>
        /// Get Table Name
        /// </summary>
        /// <returns>table name</returns>
        public String Get_TableName()
        {
            return _po.Get_TableName();
        }

        /// <summary>
        /// Get Table ID
        /// </summary>
        /// <returns>table id</returns>
        public int Get_Table_ID()
        {
            return _po.Get_Table_ID();
        }

        /// <summary>
        /// Get Record_ID
        /// </summary>
        /// <returns>record id</returns>
        public int Get_ID()
        {
            return _po.Get_ID();
        }

        /// <summary>
        /// Get Persistent Object
        /// </summary>
        /// <returns>po</returns>
        protected PO GetPO()
        {
            return _po;
        }

        /// <summary>
        /// Post Document.
        /// <pre>
        /// - try to lock document (Processed='Y' (AND Processing='N' AND Posted='N'))
        /// - if not ok - return false
        /// - postlogic (for all Accounting Schema)
        /// - create Fact lines
        /// - postCommit
        /// - commits Fact lines and Document & sets Processing = 'N'
        /// - if error - create Note
        /// </pre>
        /// </summary>
        /// <param name="force">if true ignore that locked</param>
        /// <param name="repost">if true ignore that already posted</param>
        /// <returns>null if posted error otherwise</returns>
        public String Post(bool force, bool repost)
        {
            if (_DocStatus == null)
            {
                ;// return "No DocStatus for DocumentNo=" + GetDocumentNo();
            }
            else if (_DocStatus.Equals(DocumentEngine.STATUS_COMPLETED)
                || _DocStatus.Equals(DocumentEngine.STATUS_CLOSED)
                || _DocStatus.Equals(DocumentEngine.STATUS_VOIDED)
                || _DocStatus.Equals(DocumentEngine.STATUS_REVERSED))
            {
                ;
            }
            else
            {
                return "Invalid DocStatus='" + _DocStatus + "' for DocumentNo=" + GetDocumentNo();
            }
            //
            if (_po.GetVAF_Client_ID() != _ass[0].GetVAF_Client_ID())
            {
                String error = "VAF_Client_ID Conflict - Document=" + _po.GetVAF_Client_ID()
                    + ", AcctSchema=" + _ass[0].GetVAF_Client_ID();
                log.Severe(error);
                return error;
            }

            //  Lock Record ----
            StringBuilder sql = new StringBuilder("UPDATE ");
            sql.Append(Get_TableName()).Append(" SET Processing='Y' WHERE ")
                .Append(Get_TableName()).Append("_ID=").Append(Get_ID())
                .Append(" AND Processed='Y'"); // AND IsActive='Y'");
            if (!force)
            {
                sql.Append(" AND (Processing='N' OR Processing IS NULL)");
            }
            if (!repost)
            {
                sql.Append(" AND Posted='N'");
            }
            if (DataBase.DB.ExecuteQuery(sql.ToString(), null, null) == 1)	//	outside trx
            {
                log.Info("Locked: " + Get_TableName() + "_ID=" + Get_ID());
            }
            else
            {
                log.Log(Level.WARNING, "Resubmit - Cannot lock " + Get_TableName() + "_ID="
                    + Get_ID() + ", Force=" + force + ",RePost=" + repost);
                if (force)
                {
                    return "Cannot Lock - ReSubmit";
                }
                return "Cannot Lock - ReSubmit or RePost with Force";
            }

            _error = LoadDocumentDetails();
            if (_error != null)	//	LoadError
            {
                _status = STATUS_DocumentError;
                Save(null);
                return _error;
            }

            //  Delete existing Accounting
            if (repost)
            {
                if (IsPosted() && !IsPeriodOpen())	//	already posted - don't delete if period closed
                {
                    log.Log(Level.WARNING, ToString() + " - Period Closed for already posed document");
                    Unlock();
                    return "PeriodClosed";
                }
                if (Get_TableName() != "VAGL_JRNL")
                {
                    //	delete it
                    DeleteAcct(0);
                }
            }
            else if (IsPosted())
            {
                log.Log(Level.WARNING, ToString() + " - Document already posted");
                Unlock();
                return "AlreadyPosted";
            }

            _status = STATUS_NotPosted;

            //  Create Fact per AcctSchema
            _fact = new List<Fact>();

            //  for all Accounting Schema
            bool OK = true;
            try
            {
                for (int i = 0; OK && i < _ass.Length; i++)
                {
                    if (Get_TableName() == "VAGL_JRNL")
                    {
                        DeleteAcct(_ass[i].GetVAB_AccountBook_ID());
                    }
                    //	if acct schema has "only" org, skip
                    bool skip = false;
                    if (_ass[i].GetVAF_OrgOnly_ID() != 0)
                    {
                        if (_ass[i].GetOnlyOrgs() == null)
                            _ass[i].SetOnlyOrgs(MReportTree.GetChildIDs(GetCtx(), 0, MVABAccountBookElement.ELEMENTTYPE_Organization, _ass[i].GetVAF_OrgOnly_ID()));

                        //	Header Level Org
                        skip = _ass[i].IsSkipOrg(GetVAF_Org_ID());
                        //	Line Level Org
                        if (_lines != null)
                        {
                            for (int line = 0; skip && line < _lines.Length; line++)
                            {
                                skip = _ass[i].IsSkipOrg(_lines[line].GetVAF_Org_ID());
                                if (!skip)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (skip)
                    {
                        continue;
                    }
                    //	post
                    log.Info("(" + i + ") " + _po);
                    _status = PostLogic(i);
                    if (!_status.Equals(STATUS_Posted))
                    {
                        OK = false;
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "", e);
                _status = STATUS_Error;
                _error = e.ToString();
                OK = false;
            }

            //  commitFact
            _status = PostCommit(_status);

            //  Create Note
            if (!_status.Equals(STATUS_Posted))
            {
                //  Insert Note
                String VAF_Msg_LableValue = "PostingError-" + _status;
                int VAF_UserContact_ID = _po.GetUpdatedBy();
                MVAFNotice note = new MVAFNotice(GetCtx(), VAF_Msg_LableValue, VAF_UserContact_ID,
                    GetVAF_Client_ID(), GetVAF_Org_ID(), null);
                note.SetRecord(_po.Get_Table_ID(), _po.Get_ID());
                //  Reference
                note.SetReference(ToString());	//	Document
                //	Text
                StringBuilder Text = new StringBuilder(Msg.GetMsg(GetCtx(), VAF_Msg_LableValue));
                if (_error != null)
                {
                    Text.Append(" (").Append(_error).Append(")");
                }

                String cn = typeof(Doc).FullName;//  getClass().getName();

                Text.Append(" - ").Append(cn.Substring(cn.LastIndexOf('.')))
                    .Append(" (").Append(GetDocumentType())
                    .Append(" - DocumentNo=").Append(GetDocumentNo())
                    .Append(", DateAcct=").Append(GetDateAcct().ToString().Substring(0, 10))
                    .Append(", Amount=").Append(GetAmount())
                    .Append(", Sta=").Append(_status)
                    .Append(" - PeriodOpen=").Append(IsPeriodOpen())
                    .Append(", Balanced=").Append(IsBalanced());
                note.SetTextMsg(Text.ToString());
                note.Save();
            }

            //  dispose facts
            for (int i = 0; i < _fact.Count; i++)
            {
                Fact fact = _fact[i];
                if (fact != null)
                {
                    fact.Dispose();
                }
            }
            _lines = null;

            if (_status.Equals(STATUS_Posted))
                return null;
            return _error;
        }

        /// <summary>
        /// Delete Accounting
        /// </summary>
        /// <returns>number of records</returns>
        private int DeleteAcct(int accountingSchema_ID)
        {
            StringBuilder sql = new StringBuilder("DELETE FROM Actual_Acct_Detail WHERE VAF_TableView_ID=")
                .Append(Get_Table_ID())
                .Append(" AND Record_ID=").Append(_po.Get_ID());
            if (accountingSchema_ID > 0)
            {
                sql.Append(" AND VAB_ACCOUNTBOOK_ID=").Append(accountingSchema_ID);
            }
            int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, GetTrx());
            if (no != 0)
            {
                log.Info("#=" + no);
            }
            return no;
        }

        /// <summary>
        /// Posting logic for Accounting Schema index
        /// </summary>
        /// <param name="index">Accounting Schema index</param>
        /// <returns>posting status/error code</returns>
        private String PostLogic(int index)
        {
            log.Info("(" + index + ") " + _po);

            //  rejectUnbalanced
            if (!_ass[index].IsSuspenseBalancing() && !IsBalanced())
            {
                return STATUS_NotBalanced;
            }

            //  rejectUnconvertible
            if (!IsConvertible(_ass[index]))
            {
                return STATUS_NotConvertible;
            }

            //  rejectPeriodClosed
            if (!IsPeriodOpen())
            {
                return STATUS_PeriodClosed;
            }

            //  createFacts
            List<Fact> facts = CreateFacts(_ass[index]);
            if (facts == null)
            {
                return STATUS_Error;
            }
            for (int f = 0; f < facts.Count; f++)
            {
                Fact fact = facts[f];
                if (fact == null)
                {
                    return STATUS_Error;
                }
                _fact.Add(fact);
                //
                _status = STATUS_PostPrepared;

                //	check accounts
                if (!fact.CheckAccounts())
                {
                    return STATUS_InvalidAccount;
                }

                //	distribute
                if (!fact.Distribute())
                {
                    return STATUS_Error;
                }

                //  balanceSource
                if (!fact.IsSourceBalanced())
                {
                    fact.BalanceSource();
                    if (!fact.IsSourceBalanced())
                        return STATUS_NotBalanced;
                }

                //  balanceSegments
                int columnExist = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM vaf_column WHERE vaf_tableview_id =  
                                  (SELECT vaf_tableview_id   FROM vaf_tableview   WHERE UPPER(TableName) LIKE UPPER('VAB_AccountBook'))
                                  AND UPPER(ColumnName) LIKE UPPER('FRPT_IsNotPostInterCompany') "));
                string isPostInterCompany = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT FRPT_IsNotPostInterCompany FROM VAB_AccountBook 
                                            WHERE IsActive = 'Y' AND VAB_AccountBook_ID =  " + fact.GetAcctSchema().GetVAB_AccountBook_ID()));
                if (columnExist == 0 || (columnExist > 0 && isPostInterCompany == "N"))
                {
                    if (!fact.IsSegmentBalanced())
                    {
                        fact.BalanceSegments();
                        if (!fact.IsSegmentBalanced())
                        {
                            return STATUS_NotBalanced;
                        }
                    }
                }

                //  balanceAccounting
                if (!fact.IsAcctBalanced())
                {
                    fact.BalanceAccounting();
                    if (!fact.IsAcctBalanced())
                    {
                        return STATUS_NotBalanced;
                    }
                }

            }	//	for all facts

            return STATUS_Posted;
        }

        /// <summary>
        /// Post Commit.
        /// Save Facts & Document
        /// </summary>
        /// <param name="status">status</param>
        /// <returns>Posting Status</returns>
        private String PostCommit(String status)
        {
            log.Info("Sta=" + status + " DT=" + GetDocumentType()
                + " ID=" + _po.Get_ID());
            _status = status;

            Trx trx = GetTrx();
            try
            {
                //  *** Transaction Start       ***
                //  Commit Facts
                if (status.Equals(STATUS_Posted))
                {
                    for (int i = 0; i < _fact.Count; i++)
                    {
                        Fact fact = _fact[i];
                        if (fact == null)
                        {
                            ;
                        }
                        else if (fact.Save(GetTrx()))
                        {
                            ;
                        }
                        else
                        {
                            log.Log(Level.SEVERE, "(fact not saved) ... rolling back");
                            trx.Rollback();
                            trx.Close();
                            Unlock();
                            return STATUS_Error;
                        }
                    }
                }
                //  Commit Doc
                if (!Save(GetTrx()))     //  contains unlock & document status update
                {
                    log.Log(Level.SEVERE, "(doc not saved) ... rolling back");
                    trx.Rollback();
                    trx.Close();
                    Unlock();
                    return STATUS_Error;
                }
                //	Success
                trx.Commit();
                trx.Close();
                trx = null;
                //  *** Transaction End         ***
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "... rolling back", e);
                status = STATUS_Error;
                try
                {
                    if (trx != null)
                    {
                        trx.Rollback();
                    }
                }
                catch { }
                try
                {
                    if (trx != null)
                    {
                        trx.Close();
                    }
                    trx = null;
                }
                catch { }
                Unlock();
            }
            _status = status;
            return status;
        }

        /// <summary>
        /// Get Trx Name and create Transaction
        /// </summary>
        /// <returns>Trx Name</returns>
        protected Trx GetTrx()
        {
            return _trx;
        }

        /// <summary>
        ///  [Obsolete("user GetTrx() instead")]
        /// </summary>
        /// <returns></returns>

        protected Trx GetTrxName()
        {
            return _trx;
        }

        /// <summary>
        /// Unlock Document
        /// </summary>
        private void Unlock()
        {
            StringBuilder sql = new StringBuilder("UPDATE ");
            sql.Append(Get_TableName()).Append(" SET Processing='N' WHERE ")
                .Append(Get_TableName()).Append("_ID=").Append(_po.Get_ID());
            DataBase.DB.ExecuteQuery(sql.ToString(), null, null);		//	outside trx
        }

        /// <summary>
        ///  Load Document Type and GL Info.
        ///  Set p_DocumentType and p_VAGL_Group_ID
        /// </summary>
        /// <returns>document type</returns>
        public String GetDocumentType()
        {
            if (_DocumentType == null)
            {
                SetDocumentType(null);
            }
            return _DocumentType;
        }

        /// <summary>
        /// Load Document Type and GL Info.
        /// Set p_DocumentType and p_VAGL_Group_ID
        /// </summary>
        /// <param name="DocumentType"></param>
        protected void SetDocumentType(String DocumentType)
        {
            if (DocumentType != null)
            {
                _DocumentType = DocumentType;
            }
            //  Set Document Type & VAGL_Group	// TODO - Cache DocTypes
            if (GetVAB_DocTypes_ID() != 0)		//	MatchInv,.. does not have VAB_DocTypes_ID
            {
                String sql = "SELECT DocBaseType, VAGL_Group_ID FROM VAB_DocTypes WHERE VAB_DocTypes_ID=" + GetVAB_DocTypes_ID();
                IDataReader idr = null;
                try
                {
                    idr = DataBase.DB.ExecuteReader(sql, null, null);

                    if (idr.Read())
                    {
                        _DocumentType = Utility.Util.GetValueOfString(idr[0]);//.getString(1);
                        _VAGL_Group_ID = Utility.Util.GetValueOfInt(idr[1]);//.getInt(2);
                    }
                    idr.Close();
                }
                catch (Exception e)
                {
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                    log.Log(Level.SEVERE, sql, e);
                }
            }
            if (_DocumentType == null)
            {
                log.Log(Level.SEVERE, "No DocBaseType for VAB_DocTypes_ID="
                    + GetVAB_DocTypes_ID() + ", DocumentNo=" + GetDocumentNo());
            }

            //  We have a document Type, but no GL Info - search for DocType
            if (_VAGL_Group_ID == 0)
            {
                String sql = "SELECT VAGL_Group_ID FROM VAB_DocTypes "
                    + "WHERE VAF_Client_ID=@param1 AND DocBaseType=@param2";

                IDataReader idr = null;
                try
                {
                    SqlParameter[] param = new SqlParameter[2];
                    param[0] = new SqlParameter("@param1", GetVAF_Client_ID());
                    param[1] = new SqlParameter("@param2", _DocumentType);
                    idr = DataBase.DB.ExecuteReader(sql, param, null);
                    if (idr.Read())
                    {
                        _VAGL_Group_ID = Utility.Util.GetValueOfInt(idr[0]);//.getInt(1);
                    }
                    idr.Close();
                }
                catch (Exception e)
                {
                    if (idr != null)
                        idr.Close();
                    log.Log(Level.SEVERE, sql, e);
                }
            }

            //  Still no VAGL_Group - get Default GL Category
            if (_VAGL_Group_ID == 0)
            {
                String sql = "SELECT VAGL_Group_ID FROM VAGL_Group "
                    + "WHERE VAF_Client_ID=" + GetVAF_Client_ID()
                    + "ORDER BY IsDefault DESC";
                IDataReader idr = null;
                try
                {
                    idr = DataBase.DB.ExecuteReader(sql, null, null);
                    if (idr.Read())
                    {
                        _VAGL_Group_ID = Utility.Util.GetValueOfInt(idr[0]);//.getInt(1);
                    }
                    idr.Close();
                }
                catch (Exception e)
                {
                    if (idr != null)
                        idr.Close();
                    log.Log(Level.SEVERE, sql, e);
                }
            }
            //
            if (_VAGL_Group_ID == 0)
            {
                log.Log(Level.WARNING, "No default VAGL_Group - " + ToString());
            }

            if (_DocumentType == null)
            {
                throw new Exception("Document Type not found");
            }
        }


        /// <summary>
        /// Is the Source Document Balanced
        /// </summary>
        /// <returns>true if (source) baanced</returns>
        public bool IsBalanced()
        {
            //  Multi-Currency documents are source balanced by definition
            if (IsMultiCurrency())
            {
                return true;
            }
            //
            bool retValue = Env.Signum(GetBalance()) == 0;
            if (retValue)
            {
                log.Fine("Yes " + ToString());
            }
            else
            {
                log.Warning("NO - " + ToString());
            }
            return retValue;
        }

        /// <summary>
        /// Is Document convertible to currency and Conversion Type
        /// </summary>
        /// <param name="acctSchema">accounting schema</param>
        /// <returns>true, if vonvertable to accounting currency</returns>
        public bool IsConvertible(MVABAccountBook acctSchema)
        {
            //  No Currency in document
            if (GetVAB_Currency_ID() == NO_CURRENCY)
            {
                log.Fine("(none) - " + ToString());
                return true;
            }
            //  Get All Currencies
            HashSet<int> set = new HashSet<int>();
            set.Add(Utility.Util.GetValueOfInt(GetVAB_Currency_ID()));
            for (int i = 0; _lines != null && i < _lines.Length; i++)
            {
                int VAB_Currency_ID = _lines[i].GetVAB_Currency_ID();
                if (VAB_Currency_ID != NO_CURRENCY)
                {
                    set.Add(Utility.Util.GetValueOfInt(VAB_Currency_ID));
                }
            }

            //  just one and the same
            if (set.Count == 1 && acctSchema.GetVAB_Currency_ID() == GetVAB_Currency_ID())
            {
                log.Fine("(same) Cur=" + GetVAB_Currency_ID() + " - " + ToString());
                return true;
            }

            bool convertible = true;

            //Iterator<int> it = set.iterator();
            IEnumerator<int> it = set.GetEnumerator();
            while (it.MoveNext() && convertible)
            {
                int VAB_Currency_ID = Utility.Util.GetValueOfInt((int)it.Current);//.intValue();
                if (VAB_Currency_ID != acctSchema.GetVAB_Currency_ID())
                {
                    Decimal amt = MVABExchangeRate.GetRate(VAB_Currency_ID, acctSchema.GetVAB_Currency_ID(),
                        GetDateAcct(), GetVAB_CurrencyType_ID(), GetVAF_Client_ID(), GetVAF_Org_ID());
                    //if (amt == null)
                    //{
                    //    convertible = false;
                    //    log.Warning("NOT from VAB_Currency_ID=" + VAB_Currency_ID
                    //        + " to " + acctSchema.GetVAB_Currency_ID()
                    //        + " - " + ToString());
                    //}
                    //else
                    //{
                    log.Fine("From VAB_Currency_ID=" + VAB_Currency_ID);
                    //}
                }
            }

            log.Fine("Convertible=" + convertible + ", AcctSchema VAB_Currency_ID=" + acctSchema.GetVAB_Currency_ID() + " - " + ToString());
            return convertible;
        }

        /// <summary>
        /// Calculate Period from DateAcct.
        /// _VAB_YearPeriod_ID is set to -1 of not open to 0 if not found
        /// </summary>
        public void SetPeriod()
        {
            if (_period != null)
            {
                return;
            }

            //	Period defined in GL Journal (e.g. adjustment period)
            int index = _po.Get_ColumnIndex("VAB_YearPeriod_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    _period = MVABYearPeriod.Get(GetCtx(), Utility.Util.GetValueOfInt(ii));
                }
            }
            if (_period == null)
            {
                _period = MVABYearPeriod.Get(GetCtx(), GetDateAcct(), GetVAF_Org_ID());
            }
            //	Is Period Open?
            if (_period != null && MVABYearPeriod.IsOpen(GetCtx(), GetDateAcct(), GetDocumentType(), GetVAF_Org_ID()))
            {
                _VAB_YearPeriod_ID = _period.GetVAB_YearPeriod_ID();
            }
            else
            {
                _VAB_YearPeriod_ID = -1;
            }
            //
            log.Fine(	// + VAF_Client_ID + " - " 
                GetDateAcct() + " - " + GetDocumentType() + " => " + _VAB_YearPeriod_ID);
        }

        /// <summary>
        /// Get VAB_YearPeriod_ID
        /// </summary>
        /// <returns>period</returns>
        public int GetVAB_YearPeriod_ID()
        {
            if (_period == null)
            {
                SetPeriod();
            }
            return _VAB_YearPeriod_ID;
        }

        /// <summary>
        /// Is Period Open
        /// </summary>
        /// <returns>true if period is open</returns>
        public bool IsPeriodOpen()
        {
            SetPeriod();
            bool open = _VAB_YearPeriod_ID > 0;
            if (open)
            {
                log.Fine("Yes - " + ToString());
            }
            else
            {
                log.Warning("NO - " + ToString());
            }
            return open;
        }

        /// <summary>
        /// Get the Amount
        /// (loaded in loadDocumentDetails)
        /// </summary>
        /// <param name="AmtType"></param>
        /// <returns>Amount</returns>
        public Decimal? GetAmount(int AmtType)
        {
            if (AmtType < 0 || AmtType >= _Amounts.Length)
            {
                return null;
            }
            return _Amounts[AmtType];
        }

        /// <summary>
        /// Set the Amount
        /// </summary>
        /// <param name="AmtType"></param>
        /// <param name="amt">Amount</param>
        protected void SetAmount(int AmtType, Decimal? amt)
        {
            if (AmtType < 0 || AmtType >= _Amounts.Length)
            {
                return;
            }
            if (amt == null)
            {
                _Amounts[AmtType] = Env.ZERO;
            }
            else
            {
                _Amounts[AmtType] = amt.Value;
            }
        }

        /// <summary>
        /// Get Amount with index 0
        /// </summary>
        /// <returns>Amount (primary document amount)</returns>
        public Decimal GetAmount()
        {
            return _Amounts[0];
        }

        /// <summary>
        /// Set Quantity
        /// </summary>
        /// <param name="qty">Quantity</param>
        protected void SetQty(Decimal qty)
        {
            _qty = qty;
        }

        /// <summary>
        /// Get Quantity
        /// </summary>
        /// <returns>Quantity</returns>
        public Decimal? GetQty()
        {
            if (_qty == null)
            {
                int index = _po.Get_ColumnIndex("Qty");
                if (index != -1)
                {
                    _qty = Utility.Util.GetValueOfDecimal(_po.Get_Value(index));
                }
                else
                {
                    _qty = Env.ZERO;
                }
            }
            return _qty;
        }

        /// <summary>
        /// Get the Valid Combination id for Accounting Schema
        /// </summary>
        /// <param name="AcctType"></param>
        /// <param name="as1"></param>
        /// <returns> VAB_Acct_ValidParameter_ID</returns>
        public int GetValidCombination_ID(int AcctType, MVABAccountBook as1)
        {
            int para_1 = 0;     //  first parameter (second is always AcctSchema)
            String sql = null;

            //Account Type - Invoice 
            if (AcctType == ACCTTYPE_Charge)	//	see getChargeAccount in DocLine
            {
                int cmp = GetAmount(AMTTYPE_Charge).Value.CompareTo(Env.ZERO);
                if (cmp == 0)
                {
                    return 0;
                }
                else if (cmp < 0)
                {
                    sql = "SELECT CH_Expense_Acct FROM VAB_Charge_Acct WHERE VAB_Charge_ID=@param1 AND VAB_AccountBook_ID=@param2";
                }
                else
                {
                    sql = "SELECT CH_Revenue_Acct FROM VAB_Charge_Acct WHERE VAB_Charge_ID=@param1 AND VAB_AccountBook_ID=@param2";
                }
                para_1 = GetVAB_Charge_ID();
            }
            else if (AcctType == ACCTTYPE_V_Liability)
            {
                sql = "SELECT V_Liability_Acct FROM C_BP_Vendor_Acct WHERE VAB_BusinessPartner_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_BusinessPartner_ID();
            }
            else if (AcctType == ACCTTYPE_V_Liability_Services)
            {
                sql = "SELECT V_Liability_Services_Acct FROM C_BP_Vendor_Acct WHERE VAB_BusinessPartner_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_BusinessPartner_ID();
            }
            else if (AcctType == ACCTTYPE_C_Receivable)
            {
                sql = "SELECT C_Receivable_Acct FROM C_BP_Customer_Acct WHERE VAB_BusinessPartner_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_BusinessPartner_ID();
            }
            else if (AcctType == ACCTTYPE_C_Receivable_Services)
            {
                sql = "SELECT C_Receivable_Services_Acct FROM C_BP_Customer_Acct WHERE VAB_BusinessPartner_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_BusinessPartner_ID();
            }
            else if (AcctType == ACCTTYPE_V_Prepayment)
            {
                sql = "SELECT V_Prepayment_Acct FROM C_BP_Vendor_Acct WHERE VAB_BusinessPartner_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_BusinessPartner_ID();
            }
            else if (AcctType == ACCTTYPE_C_Prepayment)
            {
                sql = "SELECT C_Prepayment_Acct FROM C_BP_Customer_Acct WHERE VAB_BusinessPartner_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_BusinessPartner_ID();
            }

            //*********** Change
            else if (AcctType == ACCTTYPE_E_Prepayment)
            {
                // Case for Cash Journal
                if (Get_Table_ID() == 407)
                {
                    sql = "SELECT E_Prepayment_Acct FROM C_BP_Employee_Acct WHERE VAB_BusinessPartner_ID=@param1 AND VAB_AccountBook_ID=@param2";
                    para_1 = GetVAB_BusinessPartner_ID();
                }
                else
                {
                    sql = "SELECT E_Prepayment_Acct FROM C_BP_Employee_Acct WHERE VAB_BusinessPartner_ID=@param1 AND VAB_AccountBook_ID=@param2";
                    para_1 = GetVAB_BusinessPartner_ID();
                }
            }

            //Account Type - Payment  
            else if (AcctType == ACCTTYPE_UnallocatedCash)
            {
                sql = "SELECT B_UnallocatedCash_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_Bank_Acct_ID();
            }
            else if (AcctType == ACCTTYPE_BankInTransit)
            {
                sql = "SELECT B_InTransit_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_Bank_Acct_ID();
            }
            else if (AcctType == ACCTTYPE_PaymentSelect)
            {
                sql = "SELECT B_PaymentSelect_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_Bank_Acct_ID();
            }

            //Account Type - Allocation   
            else if (AcctType == ACCTTYPE_DiscountExp)
            {
                sql = "SELECT a.PayDiscount_Exp_Acct FROM VAB_BPart_Category_Acct a, VAB_BusinessPartner bp "
                    + "WHERE a.VAB_BPart_Category_ID=bp.VAB_BPart_Category_ID AND bp.VAB_BusinessPartner_ID=@param1 AND a.VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_BusinessPartner_ID();
            }
            else if (AcctType == ACCTTYPE_DiscountRev)
            {
                sql = "SELECT PayDiscount_Rev_Acct FROM VAB_BPart_Category_Acct a, VAB_BusinessPartner bp "
                    + "WHERE a.VAB_BPart_Category_ID=bp.VAB_BPart_Category_ID AND bp.VAB_BusinessPartner_ID=@param1 AND a.VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_BusinessPartner_ID();
            }
            else if (AcctType == ACCTTYPE_WriteOff)
            {
                sql = "SELECT WriteOff_Acct FROM VAB_BPart_Category_Acct a, VAB_BusinessPartner bp "
                    + "WHERE a.VAB_BPart_Category_ID=bp.VAB_BPart_Category_ID AND bp.VAB_BusinessPartner_ID=@param1 AND a.VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_BusinessPartner_ID();
            }

            //Account Type - Bank Statement   
            else if (AcctType == ACCTTYPE_BankAsset)
            {
                sql = "SELECT B_Asset_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_Bank_Acct_ID();
            }
            else if (AcctType == ACCTTYPE_InterestRev)
            {
                sql = "SELECT B_InterestRev_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_Bank_Acct_ID();
            }
            else if (AcctType == ACCTTYPE_InterestExp)
            {
                sql = "SELECT B_InterestExp_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_Bank_Acct_ID();
            }

            // Account Type - Cash     
            else if (AcctType == ACCTTYPE_CashAsset)
            {
                sql = "SELECT CB_Asset_Acct FROM VAB_CashBook_Acct WHERE VAB_CashBook_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_CashBook_ID();
            }
            else if (AcctType == ACCTTYPE_CashTransfer)
            {
                sql = "SELECT CB_CashTransfer_Acct FROM VAB_CashBook_Acct WHERE VAB_CashBook_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_CashBook_ID();
            }
            else if (AcctType == ACCTTYPE_CashExpense)
            {
                sql = "SELECT CB_Expense_Acct FROM VAB_CashBook_Acct WHERE VAB_CashBook_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_CashBook_ID();
            }
            else if (AcctType == ACCTTYPE_CashReceipt)
            {
                sql = "SELECT CB_Receipt_Acct FROM VAB_CashBook_Acct WHERE VAB_CashBook_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_CashBook_ID();
            }
            else if (AcctType == ACCTTYPE_CashDifference)
            {
                sql = "SELECT CB_Differences_Acct FROM VAB_CashBook_Acct WHERE VAB_CashBook_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_CashBook_ID();
            }

            //Inventory Accounts         
            else if (AcctType == ACCTTYPE_InvDifferences)
            {
                sql = "SELECT W_Differences_Acct FROM VAM_Warehouse_Acct WHERE VAM_Warehouse_ID=@param1 AND VAB_AccountBook_ID=@param2";
                //  "SELECT W_Inventory_Acct, W_Revaluation_Acct, W_InvActualAdjust_Acct FROM VAM_Warehouse_Acct WHERE VAM_Warehouse_ID=? AND VAB_AccountBook_ID=?";
                para_1 = GetVAM_Warehouse_ID();
            }
            else if (AcctType == ACCTTYPE_NotInvoicedReceipts)
            {
                sql = "SELECT NotInvoicedReceipts_Acct FROM VAB_BPart_Category_Acct a, VAB_BusinessPartner bp "
                    + "WHERE a.VAB_BPart_Category_ID=bp.VAB_BPart_Category_ID AND bp.VAB_BusinessPartner_ID=@param1 AND a.VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_BusinessPartner_ID();
            }

            // Project Accounts          	
            else if (AcctType == ACCTTYPE_ProjectAsset)
            {
                sql = "SELECT PJ_Asset_Acct FROM VAB_Project_Acct WHERE VAB_Project_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_Project_ID();
            }
            else if (AcctType == ACCTTYPE_ProjectWIP)
            {
                sql = "SELECT PJ_WIP_Acct FROM VAB_Project_Acct WHERE VAB_Project_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_Project_ID();
            }

            //GL Accounts                 
            else if (AcctType == ACCTTYPE_PPVOffset)
            {
                sql = "SELECT PPVOffset_Acct FROM VAB_AccountBook_GL WHERE VAB_AccountBook_ID=@param1";
                para_1 = -1;
            }
            else if (AcctType == ACCTTYPE_CommitmentOffset)
            {
                sql = "SELECT CommitmentOffset_Acct FROM VAB_AccountBook_GL WHERE VAB_AccountBook_ID=@param1";
                para_1 = -1;
            }
            /**************Manfacturing*******************///Jun,06,2011/
            else if (AcctType == ACCTTYPE_WOMaterialAcct)
            {
                sql = " SELECT COALESCE(woclass.WO_Material_Acct, b.WO_Material_Acct) "
                    + " FROM VAMFG_M_WorkOrderClass_Acct woclass "
                    + " INNER JOIN VAB_AccountBook_Default b ON (woclass.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + ", VAMFG_M_WorkOrder wo "
                    + "WHERE wo.VAMFG_M_WorkOrder_ID = @param1 AND woclass.VAMFG_M_WorkOrderClass_ID = wo.VAMFG_M_WorkOrderClass_ID AND woclass.VAB_AccountBook_ID=@param2  AND woclass.IsActive = 'Y' ";
                para_1 = GetM_WorkOrder_ID();
            }
            else if (AcctType == ACCTTYPE_WOMaterialOverhdAcct)
            {
                sql = " SELECT COALESCE(woclass.WO_MaterialOverhd_Acct, b.WO_MaterialOverhd_Acct) "
                    + " FROM VAMFG_M_WorkOrderClass_Acct woclass "
                    + " INNER JOIN VAB_AccountBook_Default b ON (woclass.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + ", VAMFG_M_WorkOrder wo "
                    + "WHERE wo.VAMFG_M_WorkOrder_ID = @param1 AND woclass.VAMFG_M_WorkOrderClass_ID = wo.VAMFG_M_WorkOrderClass_ID AND woclass.VAB_AccountBook_ID=@param2  AND woclass.IsActive = 'Y' ";
                para_1 = GetM_WorkOrder_ID();
            }
            else if (AcctType == ACCTTYPE_WOResourceAcct)
            {
                sql = " SELECT COALESCE(woclass.WO_Resource_Acct, b.WO_Resource_Acct) "
                    + " FROM VAMFG_M_WorkOrderClass_Acct woclass "
                    + " INNER JOIN VAB_AccountBook_Default b ON (woclass.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + ", VAMFG_M_WorkOrder wo "
                    + "WHERE wo.VAMFG_M_WorkOrder_ID = @param1 AND woclass.VAMFG_M_WorkOrderClass_ID = wo.VAMFG_M_WorkOrderClass_ID AND woclass.VAB_AccountBook_ID=@param2  AND woclass.IsActive = 'Y' ";
                para_1 = GetM_WorkOrder_ID();
            }
            else if (AcctType == ACCTTYPE_WCOverhdAcct)
            {
                sql = " SELECT COALESCE(wc.WC_Overhead_Acct,b.WC_Overhead_Acct) "
                    + " FROM VAMFG_M_WorkCenter_Acct wc"
                    + " INNER JOIN VAB_AccountBook_Default b ON (wc.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + " WHERE wc.VAMFG_M_WorkCenter_ID = @param1 AND wc.VAB_AccountBook_ID=@param2  AND wc.IsActive = 'Y' ";
                para_1 = _workcenter_id;
            }
            else if (AcctType == ACCTTYPE_WOMaterialVarianceAcct)
            {
                sql = " SELECT COALESCE(woclass.WO_MaterialVariance_Acct, b.WO_MaterialVariance_Acct) "
                    + " FROM VAMFG_M_WorkOrderClass_Acct woclass "
                    + " INNER JOIN VAB_AccountBook_Default b ON (woclass.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + ", VAMFG_M_WorkOrder wo "
                    + "WHERE wo.VAMFG_M_WorkOrder_ID = @param1 AND woclass.VAMFG_M_WorkOrderClass_ID = wo.VAMFG_M_WorkOrderClass_ID AND woclass.VAB_AccountBook_ID=@param2  AND woclass.IsActive = 'Y' ";
                para_1 = GetM_WorkOrder_ID();
            }
            else if (AcctType == ACCTTYPE_WOMaterialOverhdVarianceAcct)
            {
                sql = " SELECT COALESCE(woclass.WO_MaterialOverhdVariance_Acct, b.WO_MaterialOverhdVariance_Acct) "
                    + " FROM VAMFG_M_WorkOrderClass_Acct woclass "
                    + " INNER JOIN VAB_AccountBook_Default b ON (woclass.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + ", VAMFG_M_WorkOrder wo "
                    + "WHERE wo.VAMFG_M_WorkOrder_ID = @param1 AND woclass.VAMFG_M_WorkOrderClass_ID = wo.VAMFG_M_WorkOrderClass_ID AND woclass.VAB_AccountBook_ID=@param2  AND woclass.IsActive = 'Y' ";
                para_1 = GetM_WorkOrder_ID();
            }
            else if (AcctType == ACCTTYPE_WOResoureVarianceAcct)
            {
                sql = " SELECT COALESCE(woclass.WO_ResourceVariance_Acct, b.WO_ResourceVariance_Acct) "
                    + " FROM VAMFG_M_WorkOrderClass_Acct woclass "
                    + " INNER JOIN VAB_AccountBook_Default b ON (woclass.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + ", VAMFG_M_WorkOrder wo "
                    + "WHERE wo.VAMFG_M_WorkOrder_ID = @param1 AND woclass.VAMFG_M_WorkOrderClass_ID = wo.VAMFG_M_WorkOrderClass_ID AND woclass.VAB_AccountBook_ID=@param2  AND woclass.IsActive = 'Y' ";
                para_1 = GetM_WorkOrder_ID();
            }
            else if (AcctType == ACCTTYPE_WOOverhdVarianceAcct)
            {
                sql = " SELECT COALESCE(woclass.WO_OverhdVariance_Acct, b.WO_OverhdVariance_Acct) "
                    + " FROM VAMFG_M_WorkOrderClass_Acct woclass "
                    + " INNER JOIN VAB_AccountBook_Default b ON (woclass.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + ", VAMFG_M_WorkOrder wo "
                    + "WHERE wo.VAMFG_M_WorkOrder_ID = @param1 AND woclass.VAMFG_M_WorkOrderClass_ID = wo.VAMFG_M_WorkOrderClass_ID AND woclass.VAB_AccountBook_ID=@param2  AND woclass.IsActive = 'Y' ";
                para_1 = GetM_WorkOrder_ID();
            }
            else if (AcctType == ACCTTYPE_WOScrapAcct)
            {
                sql = " SELECT COALESCE(woclass.WO_Scrap_Acct, b.WO_Scrap_Acct) "
                    + " FROM VAMFG_M_WorkOrderClass_Acct woclass "
                    + " INNER JOIN VAB_AccountBook_Default b ON (woclass.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + ", VAMFG_M_WorkOrder wo "
                    + "WHERE wo.VAMFG_M_WorkOrder_ID = @param1 AND woclass.VAMFG_M_WorkOrderClass_ID = wo.VAMFG_M_WorkOrderClass_ID AND woclass.VAB_AccountBook_ID=@param2  AND woclass.IsActive = 'Y' ";
                para_1 = GetM_WorkOrder_ID();
            }

            else if (AcctType == ACCTTYPE_OverhdAbsorptionAcct)
            {
                sql = " SELECT COALESCE(wcc.Overhead_Absorption_Acct, b.Overhead_Absorption_Acct) "
                    + " FROM VAMFG_M_WorkCenterCost wcc "
                    + " INNER JOIN VAB_AccountBook_Default b ON (wcc.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + " WHERE wcc.VAMFG_M_WorkCenter_ID = @param1 "
                    + " AND wcc.VAB_AccountBook_ID=@param2 "
                    + " AND wcc.VAM_ProductCostElement_ID = @param3 "
                    + " AND wcc.VAM_ProductCostType_ID = @param4 ";
                para_1 = _workcenter_id;
            }
            /**************Manfacturing*******************///Jun,06,2011/
            else
            {
                log.Severe("Not found AcctType=" + AcctType);
                return 0;
            }
            //  Do we have sql & Parameter
            if (sql == null || para_1 == 0)
            {
                log.Severe("No Parameter for AcctType=" + AcctType + " - SQL=" + sql);
                return 0;
            }

            //  Get Acct
            int Account_ID = 0;
            IDataReader idr = null;
            //int len = 0;
            SqlParameter[] param = null;
            try
            {
                if (para_1 == -1)   //  GL Accounts
                {
                    param = new SqlParameter[1];
                    param[0] = new SqlParameter("@param1", as1.GetVAB_AccountBook_ID());
                }
                else
                {
                    /**************Manfacturing****************/
                    if (AcctType == ACCTTYPE_OverhdAbsorptionAcct)
                    {
                        param = new SqlParameter[4];
                        param[0] = new SqlParameter("@param1", para_1);
                        param[1] = new SqlParameter("@param2", as1.GetVAB_AccountBook_ID());
                        param[2] = new SqlParameter("@param3", _costelement_id);
                        param[3] = new SqlParameter("@param4", as1.GetVAM_ProductCostType_ID());
                    }
                    else
                    {
                        param = new SqlParameter[2];
                        param[0] = new SqlParameter("@param1", para_1);
                        param[1] = new SqlParameter("@param2", as1.GetVAB_AccountBook_ID());

                    }
                    /**************Manfacturing****************/
                }
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                if (idr.Read())
                {
                    Account_ID = Utility.Util.GetValueOfInt(idr[0]);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                    idr.Close();

                log.Log(Level.SEVERE, "AcctType=" + AcctType + " - SQL=" + sql, e);
                return 0;
            }
            //	No account
            if (Account_ID == 0)
            {
                log.Severe("NO account Type="
                    + AcctType + ", Record=" + _po.Get_ID());
                return 0;
            }
            return Account_ID;
        }

        /// <summary>
        /// Get the Valid Combination id for Accounting Schema
        /// </summary>
        /// <param name="AcctType"></param>
        /// <param name="as1"></param>
        /// <returns> VAB_Acct_ValidParameter_ID</returns>
        public int GetValidCombination_ID(int AcctType, MVABAccountBook as1, int BPartner_ID)
        {
            int para_1 = 0;     //  first parameter (second is always AcctSchema)
            String sql = null;

            //Account Type - Invoice 
            if (AcctType == ACCTTYPE_Charge)	//	see getChargeAccount in DocLine
            {
                int cmp = GetAmount(AMTTYPE_Charge).Value.CompareTo(Env.ZERO);
                if (cmp == 0)
                {
                    return 0;
                }
                else if (cmp < 0)
                {
                    sql = "SELECT CH_Expense_Acct FROM VAB_Charge_Acct WHERE VAB_Charge_ID=@param1 AND VAB_AccountBook_ID=@param2";
                }
                else
                {
                    sql = "SELECT CH_Revenue_Acct FROM VAB_Charge_Acct WHERE VAB_Charge_ID=@param1 AND VAB_AccountBook_ID=@param2";
                }
                para_1 = GetVAB_Charge_ID();
            }
            else if (AcctType == ACCTTYPE_V_Liability)
            {
                sql = "SELECT V_Liability_Acct FROM C_BP_Vendor_Acct WHERE VAB_BusinessPartner_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = BPartner_ID;
            }
            else if (AcctType == ACCTTYPE_V_Liability_Services)
            {
                sql = "SELECT V_Liability_Services_Acct FROM C_BP_Vendor_Acct WHERE VAB_BusinessPartner_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = BPartner_ID;
            }
            else if (AcctType == ACCTTYPE_C_Receivable)
            {
                sql = "SELECT C_Receivable_Acct FROM C_BP_Customer_Acct WHERE VAB_BusinessPartner_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = BPartner_ID;
            }
            else if (AcctType == ACCTTYPE_C_Receivable_Services)
            {
                sql = "SELECT C_Receivable_Services_Acct FROM C_BP_Customer_Acct WHERE VAB_BusinessPartner_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = BPartner_ID;
            }
            else if (AcctType == ACCTTYPE_V_Prepayment)
            {
                sql = "SELECT V_Prepayment_Acct FROM C_BP_Vendor_Acct WHERE VAB_BusinessPartner_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = BPartner_ID;
            }
            else if (AcctType == ACCTTYPE_C_Prepayment)
            {
                sql = "SELECT C_Prepayment_Acct FROM C_BP_Customer_Acct WHERE VAB_BusinessPartner_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = BPartner_ID;
            }

            //*********** Change
            else if (AcctType == ACCTTYPE_E_Prepayment)
            {
                sql = "SELECT E_Prepayment_Acct FROM C_BP_Employee_Acct WHERE VAB_BusinessPartner_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = BPartner_ID;
            }

            //Account Type - Payment  
            else if (AcctType == ACCTTYPE_UnallocatedCash)
            {
                sql = "SELECT B_UnallocatedCash_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_Bank_Acct_ID();
            }
            else if (AcctType == ACCTTYPE_BankInTransit)
            {
                sql = "SELECT B_InTransit_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_Bank_Acct_ID();
            }
            else if (AcctType == ACCTTYPE_PaymentSelect)
            {
                sql = "SELECT B_PaymentSelect_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_Bank_Acct_ID();
            }

            //Account Type - Allocation   
            else if (AcctType == ACCTTYPE_DiscountExp)
            {
                sql = "SELECT a.PayDiscount_Exp_Acct FROM VAB_BPart_Category_Acct a, VAB_BusinessPartner bp "
                    + "WHERE a.VAB_BPart_Category_ID=bp.VAB_BPart_Category_ID AND bp.VAB_BusinessPartner_ID=@param1 AND a.VAB_AccountBook_ID=@param2";
                para_1 = BPartner_ID;
            }
            else if (AcctType == ACCTTYPE_DiscountRev)
            {
                sql = "SELECT PayDiscount_Rev_Acct FROM VAB_BPart_Category_Acct a, VAB_BusinessPartner bp "
                    + "WHERE a.VAB_BPart_Category_ID=bp.VAB_BPart_Category_ID AND bp.VAB_BusinessPartner_ID=@param1 AND a.VAB_AccountBook_ID=@param2";
                para_1 = BPartner_ID;
            }
            else if (AcctType == ACCTTYPE_WriteOff)
            {
                sql = "SELECT WriteOff_Acct FROM VAB_BPart_Category_Acct a, VAB_BusinessPartner bp "
                    + "WHERE a.VAB_BPart_Category_ID=bp.VAB_BPart_Category_ID AND bp.VAB_BusinessPartner_ID=@param1 AND a.VAB_AccountBook_ID=@param2";
                para_1 = BPartner_ID;
            }

            //Account Type - Bank Statement   
            else if (AcctType == ACCTTYPE_BankAsset)
            {
                sql = "SELECT B_Asset_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_Bank_Acct_ID();
            }
            else if (AcctType == ACCTTYPE_InterestRev)
            {
                sql = "SELECT B_InterestRev_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_Bank_Acct_ID();
            }
            else if (AcctType == ACCTTYPE_InterestExp)
            {
                sql = "SELECT B_InterestExp_Acct FROM VAB_Bank_Acct_Acct WHERE VAB_Bank_Acct_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_Bank_Acct_ID();
            }

            // Account Type - Cash     
            else if (AcctType == ACCTTYPE_CashAsset)
            {
                sql = "SELECT CB_Asset_Acct FROM VAB_CashBook_Acct WHERE VAB_CashBook_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_CashBook_ID();
            }
            else if (AcctType == ACCTTYPE_CashTransfer)
            {
                sql = "SELECT CB_CashTransfer_Acct FROM VAB_CashBook_Acct WHERE VAB_CashBook_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_CashBook_ID();
            }
            else if (AcctType == ACCTTYPE_CashExpense)
            {
                sql = "SELECT CB_Expense_Acct FROM VAB_CashBook_Acct WHERE VAB_CashBook_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_CashBook_ID();
            }
            else if (AcctType == ACCTTYPE_CashReceipt)
            {
                sql = "SELECT CB_Receipt_Acct FROM VAB_CashBook_Acct WHERE VAB_CashBook_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_CashBook_ID();
            }
            else if (AcctType == ACCTTYPE_CashDifference)
            {
                sql = "SELECT CB_Differences_Acct FROM VAB_CashBook_Acct WHERE VAB_CashBook_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_CashBook_ID();
            }

            //Inventory Accounts         
            else if (AcctType == ACCTTYPE_InvDifferences)
            {
                sql = "SELECT W_Differences_Acct FROM VAM_Warehouse_Acct WHERE VAM_Warehouse_ID=@param1 AND VAB_AccountBook_ID=@param2";
                //  "SELECT W_Inventory_Acct, W_Revaluation_Acct, W_InvActualAdjust_Acct FROM VAM_Warehouse_Acct WHERE VAM_Warehouse_ID=? AND VAB_AccountBook_ID=?";
                para_1 = GetVAM_Warehouse_ID();
            }
            else if (AcctType == ACCTTYPE_NotInvoicedReceipts)
            {
                sql = "SELECT NotInvoicedReceipts_Acct FROM VAB_BPart_Category_Acct a, VAB_BusinessPartner bp "
                    + "WHERE a.VAB_BPart_Category_ID=bp.VAB_BPart_Category_ID AND bp.VAB_BusinessPartner_ID=@param1 AND a.VAB_AccountBook_ID=@param2";
                para_1 = BPartner_ID;
            }

            // Project Accounts          	
            else if (AcctType == ACCTTYPE_ProjectAsset)
            {
                sql = "SELECT PJ_Asset_Acct FROM VAB_Project_Acct WHERE VAB_Project_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_Project_ID();
            }
            else if (AcctType == ACCTTYPE_ProjectWIP)
            {
                sql = "SELECT PJ_WIP_Acct FROM VAB_Project_Acct WHERE VAB_Project_ID=@param1 AND VAB_AccountBook_ID=@param2";
                para_1 = GetVAB_Project_ID();
            }

            //GL Accounts                 
            else if (AcctType == ACCTTYPE_PPVOffset)
            {
                sql = "SELECT PPVOffset_Acct FROM VAB_AccountBook_GL WHERE VAB_AccountBook_ID=@param1";
                para_1 = -1;
            }
            else if (AcctType == ACCTTYPE_CommitmentOffset)
            {
                sql = "SELECT CommitmentOffset_Acct FROM VAB_AccountBook_GL WHERE VAB_AccountBook_ID=@param1";
                para_1 = -1;
            }
            /**************Manfacturing*******************///Jun,06,2011/
            else if (AcctType == ACCTTYPE_WOMaterialAcct)
            {
                sql = " SELECT COALESCE(woclass.WO_Material_Acct, b.WO_Material_Acct) "
                    + " FROM VAMFG_M_WorkOrderClass_Acct woclass "
                    + " INNER JOIN VAB_AccountBook_Default b ON (woclass.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + ", VAMFG_M_WorkOrder wo "
                    + "WHERE wo.VAMFG_M_WorkOrder_ID = @param1 AND woclass.VAMFG_M_WorkOrderClass_ID = wo.VAMFG_M_WorkOrderClass_ID AND woclass.VAB_AccountBook_ID=@param2  AND woclass.IsActive = 'Y' ";
                para_1 = GetM_WorkOrder_ID();
            }
            else if (AcctType == ACCTTYPE_WOMaterialOverhdAcct)
            {
                sql = " SELECT COALESCE(woclass.WO_MaterialOverhd_Acct, b.WO_MaterialOverhd_Acct) "
                    + " FROM VAMFG_M_WorkOrderClass_Acct woclass "
                    + " INNER JOIN VAB_AccountBook_Default b ON (woclass.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + ", VAMFG_M_WorkOrder wo "
                    + "WHERE wo.VAMFG_M_WorkOrder_ID = @param1 AND woclass.VAMFG_M_WorkOrderClass_ID = wo.VAMFG_M_WorkOrderClass_ID AND woclass.VAB_AccountBook_ID=@param2  AND woclass.IsActive = 'Y' ";
                para_1 = GetM_WorkOrder_ID();
            }
            else if (AcctType == ACCTTYPE_WOResourceAcct)
            {
                sql = " SELECT COALESCE(woclass.WO_Resource_Acct, b.WO_Resource_Acct) "
                    + " FROM VAMFG_M_WorkOrderClass_Acct woclass "
                    + " INNER JOIN VAB_AccountBook_Default b ON (woclass.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + ", VAMFG_M_WorkOrder wo "
                    + "WHERE wo.VAMFG_M_WorkOrder_ID = @param1 AND woclass.VAMFG_M_WorkOrderClass_ID = wo.VAMFG_M_WorkOrderClass_ID AND woclass.VAB_AccountBook_ID=@param2  AND woclass.IsActive = 'Y' ";
                para_1 = GetM_WorkOrder_ID();
            }
            else if (AcctType == ACCTTYPE_WCOverhdAcct)
            {
                sql = " SELECT COALESCE(wc.WC_Overhead_Acct,b.WC_Overhead_Acct) "
                    + " FROM VAMFG_M_WorkCenter_Acct wc"
                    + " INNER JOIN VAB_AccountBook_Default b ON (wc.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + " WHERE wc.VAMFG_M_WorkCenter_ID = @param1 AND wc.VAB_AccountBook_ID=@param2  AND wc.IsActive = 'Y' ";
                para_1 = _workcenter_id;
            }
            else if (AcctType == ACCTTYPE_WOMaterialVarianceAcct)
            {
                sql = " SELECT COALESCE(woclass.WO_MaterialVariance_Acct, b.WO_MaterialVariance_Acct) "
                    + " FROM VAMFG_M_WorkOrderClass_Acct woclass "
                    + " INNER JOIN VAB_AccountBook_Default b ON (woclass.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + ", VAMFG_M_WorkOrder wo "
                    + "WHERE wo.VAMFG_M_WorkOrder_ID = @param1 AND woclass.VAMFG_M_WorkOrderClass_ID = wo.VAMFG_M_WorkOrderClass_ID AND woclass.VAB_AccountBook_ID=@param2  AND woclass.IsActive = 'Y' ";
                para_1 = GetM_WorkOrder_ID();
            }
            else if (AcctType == ACCTTYPE_WOMaterialOverhdVarianceAcct)
            {
                sql = " SELECT COALESCE(woclass.WO_MaterialOverhdVariance_Acct, b.WO_MaterialOverhdVariance_Acct) "
                    + " FROM VAMFG_M_WorkOrderClass_Acct woclass "
                    + " INNER JOIN VAB_AccountBook_Default b ON (woclass.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + ", VAMFG_M_WorkOrder wo "
                    + "WHERE wo.VAMFG_M_WorkOrder_ID = @param1 AND woclass.VAMFG_M_WorkOrderClass_ID = wo.VAMFG_M_WorkOrderClass_ID AND woclass.VAB_AccountBook_ID=@param2  AND woclass.IsActive = 'Y' ";
                para_1 = GetM_WorkOrder_ID();
            }
            else if (AcctType == ACCTTYPE_WOResoureVarianceAcct)
            {
                sql = " SELECT COALESCE(woclass.WO_ResourceVariance_Acct, b.WO_ResourceVariance_Acct) "
                    + " FROM VAMFG_M_WorkOrderClass_Acct woclass "
                    + " INNER JOIN VAB_AccountBook_Default b ON (woclass.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + ", VAMFG_M_WorkOrder wo "
                    + "WHERE wo.VAMFG_M_WorkOrder_ID = @param1 AND woclass.VAMFG_M_WorkOrderClass_ID = wo.VAMFG_M_WorkOrderClass_ID AND woclass.VAB_AccountBook_ID=@param2  AND woclass.IsActive = 'Y' ";
                para_1 = GetM_WorkOrder_ID();
            }
            else if (AcctType == ACCTTYPE_WOOverhdVarianceAcct)
            {
                sql = " SELECT COALESCE(woclass.WO_OverhdVariance_Acct, b.WO_OverhdVariance_Acct) "
                    + " FROM VAMFG_M_WorkOrderClass_Acct woclass "
                    + " INNER JOIN VAB_AccountBook_Default b ON (woclass.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + ", VAMFG_M_WorkOrder wo "
                    + "WHERE wo.VAMFG_M_WorkOrder_ID = @param1 AND woclass.VAMFG_M_WorkOrderClass_ID = wo.VAMFG_M_WorkOrderClass_ID AND woclass.VAB_AccountBook_ID=@param2  AND woclass.IsActive = 'Y' ";
                para_1 = GetM_WorkOrder_ID();
            }
            else if (AcctType == ACCTTYPE_WOScrapAcct)
            {
                sql = " SELECT COALESCE(woclass.WO_Scrap_Acct, b.WO_Scrap_Acct) "
                    + " FROM VAMFG_M_WorkOrderClass_Acct woclass "
                    + " INNER JOIN VAB_AccountBook_Default b ON (woclass.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + ", VAMFG_M_WorkOrder wo "
                    + "WHERE wo.VAMFG_M_WorkOrder_ID = @param1 AND woclass.VAMFG_M_WorkOrderClass_ID = wo.VAMFG_M_WorkOrderClass_ID AND woclass.VAB_AccountBook_ID=@param2  AND woclass.IsActive = 'Y' ";
                para_1 = GetM_WorkOrder_ID();
            }

            else if (AcctType == ACCTTYPE_OverhdAbsorptionAcct)
            {
                sql = " SELECT COALESCE(wcc.Overhead_Absorption_Acct, b.Overhead_Absorption_Acct) "
                    + " FROM VAMFG_M_WorkCenterCost wcc "
                    + " INNER JOIN VAB_AccountBook_Default b ON (wcc.VAB_AccountBook_ID = b.VAB_AccountBook_ID)"
                    + " WHERE wcc.VAMFG_M_WorkCenter_ID = @param1 "
                    + " AND wcc.VAB_AccountBook_ID=@param2 "
                    + " AND wcc.VAM_ProductCostElement_ID = @param3 "
                    + " AND wcc.VAM_ProductCostType_ID = @param4 ";
                para_1 = _workcenter_id;
            }
            /**************Manfacturing*******************///Jun,06,2011/
            else
            {
                log.Severe("Not found AcctType=" + AcctType);
                return 0;
            }
            //  Do we have sql & Parameter
            if (sql == null || para_1 == 0)
            {
                log.Severe("No Parameter for AcctType=" + AcctType + " - SQL=" + sql);
                return 0;
            }

            //  Get Acct
            int Account_ID = 0;
            IDataReader idr = null;
            //int len = 0;
            SqlParameter[] param = null;
            try
            {
                if (para_1 == -1)   //  GL Accounts
                {
                    param = new SqlParameter[1];
                    param[0] = new SqlParameter("@param1", as1.GetVAB_AccountBook_ID());
                }
                else
                {
                    /**************Manfacturing****************/
                    if (AcctType == ACCTTYPE_OverhdAbsorptionAcct)
                    {
                        param = new SqlParameter[4];
                        param[0] = new SqlParameter("@param1", para_1);
                        param[1] = new SqlParameter("@param2", as1.GetVAB_AccountBook_ID());
                        param[2] = new SqlParameter("@param3", _costelement_id);
                        param[3] = new SqlParameter("@param4", as1.GetVAM_ProductCostType_ID());
                    }
                    else
                    {
                        param = new SqlParameter[2];
                        param[0] = new SqlParameter("@param1", para_1);
                        param[1] = new SqlParameter("@param2", as1.GetVAB_AccountBook_ID());

                    }
                    /**************Manfacturing****************/
                }
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                if (idr.Read())
                {
                    Account_ID = Utility.Util.GetValueOfInt(idr[0]);
                }
                idr.Close();
                if (Account_ID.Equals(0))
                {
                    if (AcctType == ACCTTYPE_E_Prepayment)
                    {
                        sql = "SELECT E_Prepayment_Acct FROM VAB_AccountBook_default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID();
                        Account_ID = Utility.Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    }
                    else if (AcctType == ACCTTYPE_V_Prepayment)
                    {
                        sql = "SELECT V_Prepayment_Acct FROM VAB_AccountBook_default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID();
                        Account_ID = Utility.Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    }
                    else if (AcctType == ACCTTYPE_C_Prepayment)
                    {
                        sql = "SELECT C_Prepayment_Acct FROM VAB_AccountBook_default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID();
                        Account_ID = Utility.Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    }
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                    idr.Close();

                log.Log(Level.SEVERE, "AcctType=" + AcctType + " - SQL=" + sql, e);
                return 0;
            }
            //	No account
            if (Account_ID == 0)
            {
                log.Severe("NO account Type="
                    + AcctType + ", Record=" + _po.Get_ID());
                return 0;
            }
            return Account_ID;
        }

        #region Manfacturing
        /// <summary>
        /// Get M_WorkOrderClass_ID
        /// </summary>
        /// <returns>WorkOrderClass</returns>
        public int GetM_WorkOrder_ID()
        {
            if (_workorder_id == -1)
            {
                int index = _po.Get_ColumnIndex("VAMFG_M_WorkOrder_ID");
                if (index != -1)
                {
                    int ii = Util.GetValueOfInt(_po.Get_Value(index));
                    if (ii != 0)
                    {
                        _workorder_id = ii;
                    }
                }
                if (_workorder_id == -1)
                {
                    _workorder_id = 0;
                }
            }
            return _workorder_id;
        }
        #endregion

        /// <summary>
        /// Get the account for Accounting Schema
        /// </summary>
        /// <param name="AcctType"></param>
        /// <param name="as1"></param>
        /// <returns>Account</returns>
        public MVABAccount GetAccount(int AcctType, MVABAccountBook as1)
        {
            int VAB_Acct_ValidParameter_ID = GetValidCombination_ID(AcctType, as1);
            if (VAB_Acct_ValidParameter_ID == 0)
            {
                return null;
            }
            //	Return Account
            MVABAccount acct = MVABAccount.Get(as1.GetCtx(), VAB_Acct_ValidParameter_ID);
            return acct;
        }

        // Special Case For Cash Journal 
        /// <summary>
        /// Get the account for Accounting Schema Only in case of Cash Journal
        /// </summary>
        /// <param name="AcctType"></param>
        /// <param name="as1"></param>
        /// <returns>Account</returns>
        public MVABAccount GetAccount(int AcctType, MVABAccountBook as1, int BPartner_ID)
        {
            int VAB_Acct_ValidParameter_ID = GetValidCombination_ID(AcctType, as1, BPartner_ID);
            if (VAB_Acct_ValidParameter_ID == 0)
            {
                return null;
            }
            //	Return Account
            MVABAccount acct = MVABAccount.Get(as1.GetCtx(), VAB_Acct_ValidParameter_ID);
            return acct;
        }


        /// <summary>
        /// Save to Disk - set posted flag
        /// </summary>
        /// <param name="trxName">trxName transaction</param>
        /// <returns>true if saved</returns>
        private bool Save(Trx trxName)
        {
            log.Fine(ToString() + "->" + _status);
            StringBuilder sql = new StringBuilder("UPDATE ");
            sql.Append(Get_TableName()).Append(" SET Posted='").Append(_status)
                .Append("',Processing='N' ")
                .Append("WHERE ")
                .Append(Get_TableName()).Append("_ID=").Append(_po.Get_ID());
            int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, trxName);
            return no == 1;
        }

        /// <summary>
        /// Get DocLine with ID
        /// </summary>
        /// <param name="Record_ID"></param>
        /// <returns>DocLine</returns>
        public DocLine DetDocLine(int Record_ID)
        {
            if (_lines == null || _lines.Length == 0 || Record_ID == 0)
            {
                return null;

            }
            for (int i = 0; i < _lines.Length; i++)
            {
                if (_lines[i].Get_ID() == Record_ID)
                {
                    return _lines[i];
                }
            }
            return null;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>String</returns>
        public override String ToString()
        {
            return _po.ToString();
        }

        /// <summary>
        /// Get VAF_Client_ID
        /// </summary>
        /// <returns>client</returns>
        public int GetVAF_Client_ID()
        {
            return _po.GetVAF_Client_ID();
        }

        /// <summary>
        /// 	Get VAF_Org_ID
        /// </summary>
        /// <returns>org</returns>
        public int GetVAF_Org_ID()
        {
            return _po.GetVAF_Org_ID();
        }

        /// <summary>
        /// Get Document No
        /// </summary>
        /// <returns>document No</returns>
        public String GetDocumentNo()
        {
            if (_DocumentNo != null)
            {
                return _DocumentNo;
            }
            int index = _po.Get_ColumnIndex("DocumentNo");
            if (index == -1)
            {
                index = _po.Get_ColumnIndex("Name");
            }
            if (index == -1)
            {
                throw new Exception("No DocumentNo");
            }
            _DocumentNo = (String)_po.Get_Value(index);
            return _DocumentNo;
        }

        /// <summary>
        /// Get Description
        /// </summary>
        /// <returns>Description</returns>
        public String GetDescription()
        {
            if (_Description == null)
            {
                int index = _po.Get_ColumnIndex("Description");
                if (index != -1)
                {
                    _Description = (String)_po.Get_Value(index);
                }
                else
                {
                    _Description = "";
                }
            }
            return _Description;
        }

        /// <summary>
        /// Get VAB_Currency_ID
        /// </summary>
        /// <returns>currency</returns>
        public int GetVAB_Currency_ID()
        {
            if (_VAB_Currency_ID == -1)
            {
                int index = _po.Get_ColumnIndex("VAB_Currency_ID");
                if (index != -1)
                {
                    int? ii = (int?)_po.Get_Value(index);
                    if (ii != null)
                    {
                        _VAB_Currency_ID = Utility.Util.GetValueOfInt(ii);//.intValue();
                    }
                }
                if (_VAB_Currency_ID == -1)
                {
                    _VAB_Currency_ID = NO_CURRENCY;
                }
            }
            return _VAB_Currency_ID;
        }

        /// <summary>
        /// Set VAB_Currency_ID
        /// </summary>
        /// <param name="VAB_Currency_ID">id</param>
        public void SetVAB_Currency_ID(int VAB_Currency_ID)
        {
            _VAB_Currency_ID = VAB_Currency_ID;
        }

        /// <summary>
        ///  	Is Multi Currency
        /// </summary>
        /// <returns>mc</returns>
        public bool IsMultiCurrency()
        {
            return _MultiCurrency;
        }

        /// <summary>
        /// Set Multi Currency
        /// </summary>
        /// <param name="mc">multi currency</param>
        protected void SetIsMultiCurrency(bool mc)
        {
            _MultiCurrency = mc;
        }

        /// <summary>
        /// Is Tax Included
        /// </summary>
        /// <returns>tax incl</returns>
        public bool IsTaxIncluded()
        {
            return _TaxIncluded;
        }

        /// <summary>
        /// Set Tax Includedy
        /// </summary>
        /// <param name="ti">Tax Included</param>
        protected void SetIsTaxIncluded(bool ti)
        {
            _TaxIncluded = ti;
        }

        /// <summary>
        ///  	Get VAB_CurrencyType_ID
        /// </summary>
        /// <returns>ConversionType</returns>
        public int GetVAB_CurrencyType_ID()
        {
            int index = _po.Get_ColumnIndex("VAB_CurrencyType_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        /// 	Get VAGL_Group_ID
        /// </summary>
        /// <returns>categoory</returns>
        public int GetVAGL_Group_ID()
        {
            return _VAGL_Group_ID;
        }

        /// <summary>
        /// Get VAGL_Group_ID
        /// </summary>
        /// <returns>categoory</returns>
        public int GetVAGL_Budget_ID()
        {
            int index = _po.Get_ColumnIndex("VAGL_Budget_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        ///  	Get Accounting Date
        /// </summary>
        /// <returns>currency</returns>
        public DateTime? GetDateAcct()
        {
            if (_DateAcct != null)
            {

                return _DateAcct;
            }
            int index = _po.Get_ColumnIndex("DateAcct");
            if (index != -1)
            {
                _DateAcct = (DateTime?)_po.Get_Value(index);
                if (_DateAcct != null)
                {
                    return _DateAcct;
                }
            }
            throw new Exception("No DateAcct");
        }

        /// <summary>
        /// Set Date Acct
        /// </summary>
        /// <param name="da">accounting date</param>
        protected void SetDateAcct(DateTime? da)
        {
            _DateAcct = da;
        }

        /// <summary>
        /// 	Get Document Date
        /// </summary>
        /// <returns>currency</returns>
        public DateTime? GetDateDoc()
        {
            if (_DateDoc != null)
            {
                return _DateDoc;
            }
            int index = _po.Get_ColumnIndex("DateDoc");
            if (index == -1)
            {
                index = _po.Get_ColumnIndex("MovementDate");
            }
            if (index != -1)
            {
                _DateDoc = (DateTime?)_po.Get_Value(index);
                if (_DateDoc != null)
                {
                    return _DateDoc;
                }
            }
            throw new Exception("No DateDoc");
        }

        /// <summary>
        /// Set Date Doc
        /// </summary>
        /// <param name="dd">dd document date</param>
        protected void SetDateDoc(DateTime? dd)
        {
            _DateDoc = dd;
        }

        /// <summary>
        /// Is Document Posted
        /// </summary>
        /// <returns>true if posted</returns>
        public bool IsPosted()
        {
            if (GetRectifyingProcess())
                return false;

            int index = _po.Get_ColumnIndex("Posted");
            if (index != -1)
            {
                Object posted = _po.Get_Value(index);
                if (posted is Boolean)
                {
                    return Utility.Util.GetValueOfBool(((Boolean)posted));//.booleanValue();
                }
                if (posted is String)
                {
                    return "Y".Equals(posted);
                }
            }
            throw new Exception("No Posted");
        }

        /// <summary>
        /// Is Sales Trx
        /// </summary>
        /// <returns>true if posted</returns>
        public bool IsSOTrx()
        {
            int index = _po.Get_ColumnIndex("IsSOTrx");
            if (index == -1)
            {
                index = _po.Get_ColumnIndex("IsReceipt");
            }
            if (index != -1)
            {
                Object posted = _po.Get_Value(index);
                if (posted is Boolean)
                {
                    return Utility.Util.GetValueOfBool(((Boolean)posted));//.booleanValue();
                }
                if (posted is String)
                {
                    return "Y".Equals(posted);
                }
            }
            return false;
        }

        /// <summary>
        /// Is Return Trx
        /// </summary>
        /// <returns>true if this is a return transaction</returns>
        public bool IsReturnTrx()
        {
            int index = _po.Get_ColumnIndex("IsReturnTrx");
            if (index != -1)
            {
                Object isReturnTrx = _po.Get_Value(index);
                if (isReturnTrx is Boolean)
                {
                    return Utility.Util.GetValueOfBool(((Boolean)isReturnTrx));//.booleanValue();
                }
                if (isReturnTrx is String)
                {
                    return "Y".Equals(isReturnTrx);
                }
            }
            return false;
        }

        /// <summary>
        /// Get VAB_DocTypes_ID
        /// </summary>
        /// <returns>DocType</returns>
        public int GetVAB_DocTypes_ID()
        {
            int index = _po.Get_ColumnIndex("VAB_DocTypes_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                //	DocType does not exist - get DocTypeTarget
                if (ii != null && Utility.Util.GetValueOfInt(ii) == 0)
                {
                    index = _po.Get_ColumnIndex("VAB_DocTypesTarget_ID");
                    if (index != -1)
                    {
                        ii = (int?)_po.Get_Value(index);
                    }
                }
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);
                }
            }
            return 0;
        }


        /// <summary>
        /// Get header level VAB_Charge_ID
        /// </summary>
        /// <returns>Charge</returns>
        public int GetVAB_Charge_ID()
        {
            int index = _po.Get_ColumnIndex("VAB_Charge_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);
                }
            }
            return 0;
        }

        /// <summary>
        /// Get SalesRep_ID
        /// </summary>
        /// <returns>SalesRep</returns>
        public int GetSalesRep_ID()
        {
            int index = _po.Get_ColumnIndex("SalesRep_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);
                }
            }
            return 0;
        }

        /// <summary>
        /// Get VAB_Bank_Acct_ID
        /// </summary>
        /// <returns>BankAccount</returns>
        public int GetVAB_Bank_Acct_ID()
        {
            if (_VAB_Bank_Acct_ID == -1)
            {
                int index = _po.Get_ColumnIndex("VAB_Bank_Acct_ID");
                if (index != -1)
                {
                    int? ii = (int?)_po.Get_Value(index);
                    if (ii != null)
                    {
                        _VAB_Bank_Acct_ID = Utility.Util.GetValueOfInt(ii);
                    }
                }
                if (_VAB_Bank_Acct_ID == -1)
                {
                    _VAB_Bank_Acct_ID = 0;
                }
            }
            return _VAB_Bank_Acct_ID;
        }

        /// <summary>
        /// Set VAB_Bank_Acct_ID
        /// </summary>
        /// <param name="VAB_Bank_Acct_ID">bank acct</param>
        protected void SetVAB_Bank_Acct_ID(int VAB_Bank_Acct_ID)
        {
            _VAB_Bank_Acct_ID = VAB_Bank_Acct_ID;
        }

        /// <summary>
        /// Get VAB_CashBook_ID
        /// </summary>
        /// <returns>CashBook</returns>
        public int GetVAB_CashBook_ID()
        {
            if (_VAB_CashBook_ID == -1)
            {
                int index = _po.Get_ColumnIndex("VAB_CashBook_ID");
                if (index != -1)
                {
                    int? ii = (int?)_po.Get_Value(index);
                    if (ii != null)
                    {
                        _VAB_CashBook_ID = Utility.Util.GetValueOfInt(ii);
                    }
                }
                if (_VAB_CashBook_ID == -1)
                {
                    _VAB_CashBook_ID = 0;
                }
            }
            return _VAB_CashBook_ID;
        }

        /// <summary>
        /// Set VAB_CashBook_ID
        /// </summary>
        /// <param name="VAB_CashBook_ID">cash book</param>
        protected void SetVAB_CashBook_ID(int VAB_CashBook_ID)
        {
            _VAB_CashBook_ID = VAB_CashBook_ID;
        }

        /// <summary>
        /// Get VAM_Warehouse_ID
        /// </summary>
        /// <returns>Warehouse</returns>
        public int GetVAM_Warehouse_ID()
        {
            int index = _po.Get_ColumnIndex("VAM_Warehouse_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);
                }
            }
            return 0;
        }


        /// <summary>
        /// Get VAB_BusinessPartner_ID
        /// </summary>
        /// <returns>BPartner</returns>
        public int GetVAB_BusinessPartner_ID()
        {
            if (_VAB_BusinessPartner_ID == -1)
            {
                int index = _po.Get_ColumnIndex("VAB_BusinessPartner_ID");
                if (index != -1)
                {
                    int? ii = (int?)_po.Get_Value(index);
                    if (ii != null)
                    {
                        _VAB_BusinessPartner_ID = Utility.Util.GetValueOfInt(ii);
                    }
                }
                if (_VAB_BusinessPartner_ID == -1)
                {
                    _VAB_BusinessPartner_ID = 0;
                }
            }
            return _VAB_BusinessPartner_ID;
        }

        /// <summary>
        /// Set VAB_BusinessPartner_ID
        /// </summary>
        /// <param name="VAB_BusinessPartner_ID">bp</param>
        protected void SetVAB_BusinessPartner_ID(int VAB_BusinessPartner_ID)
        {
            _VAB_BusinessPartner_ID = VAB_BusinessPartner_ID;
        }

        /// <summary>
        /// Get VAB_BPart_Location_ID
        /// </summary>
        /// <returns>BPartner Location</returns>
        public int GetVAB_BPart_Location_ID()
        {
            int index = _po.Get_ColumnIndex("VAB_BPart_Location_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);
                }
            }
            return 0;
        }

        /// <summary>
        /// Get VAB_Project_ID
        /// </summary>
        /// <returns>Project</returns>
        public int GetVAB_Project_ID()
        {
            int index = _po.Get_ColumnIndex("VAB_Project_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);
                }
            }
            return 0;
        }

        /// <summary>
        /// Get VAB_SalesRegionState_ID
        /// </summary>
        /// <returns>Sales Region</returns>
        public int GetVAB_SalesRegionState_ID()
        {
            int index = _po.Get_ColumnIndex("VAB_SalesRegionState_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);
                }
            }
            return 0;
        }

        /// <summary>
        /// Get VAB_SalesRegionState_ID
        /// </summary>
        /// <returns>Sales Region</returns>
        public int GetBP_VAB_SalesRegionState_ID()
        {
            if (_BP_VAB_SalesRegionState_ID == -1)
            {
                int index = _po.Get_ColumnIndex("VAB_SalesRegionState_ID");
                if (index != -1)
                {
                    int? ii = (int?)_po.Get_Value(index);
                    if (ii != null)
                    {
                        _BP_VAB_SalesRegionState_ID = Utility.Util.GetValueOfInt(ii);
                    }
                }
                if (_BP_VAB_SalesRegionState_ID == -1)
                {
                    _BP_VAB_SalesRegionState_ID = 0;
                }
            }
            return _BP_VAB_SalesRegionState_ID;
        }

        /// <summary>
        /// 	Set VAB_SalesRegionState_ID
        /// </summary>
        /// <param name="VAB_SalesRegionState_ID"></param>
        public void SetBP_VAB_SalesRegionState_ID(int VAB_SalesRegionState_ID)
        {
            _BP_VAB_SalesRegionState_ID = VAB_SalesRegionState_ID;
        }

        /// <summary>
        /// Get VAB_BillingCode_ID
        /// </summary>
        /// <returns>Activity</returns>
        public int GetVAB_BillingCode_ID()
        {
            int index = _po.Get_ColumnIndex("VAB_BillingCode_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);
                }
            }
            return 0;
        }

        /// <summary>
        /// Get VAB_Promotion_ID
        /// </summary>
        /// <returns>Campaign</returns>
        public int GetVAB_Promotion_ID()
        {
            int index = _po.Get_ColumnIndex("VAB_Promotion_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);
                }
            }
            return 0;
        }

        /// <summary>
        /// Get VAM_Product_ID
        /// </summary>
        /// <returns>Product</returns>
        public int GetVAM_Product_ID()
        {
            int index = _po.Get_ColumnIndex("VAM_Product_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);
                }
            }
            return 0;
        }

        /// <summary>
        /// Get VAF_OrgTrx_ID
        /// </summary>
        /// <returns>Trx Org</returns>
        public int GetVAF_OrgTrx_ID()
        {
            int index = _po.Get_ColumnIndex("VAF_OrgTrx_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);
                }
            }
            return 0;
        }

        /// <summary>
        /// Get C_LocFrom_ID
        /// </summary>
        /// <returns>loc from</returns>
        public int GetC_LocFrom_ID()
        {
            return _C_LocFrom_ID;
        }

        /// <summary>
        /// Set C_LocFrom_ID
        /// </summary>
        /// <param name="C_LocFrom_ID">loc from</param>
        protected void SetC_LocFrom_ID(int C_LocFrom_ID)
        {
            _C_LocFrom_ID = C_LocFrom_ID;
        }

        /// <summary>
        /// Get C_LocTo_ID
        /// </summary>
        /// <returns>loc to</returns>
        public int GetC_LocTo_ID()
        {
            return _C_LocTo_ID;
        }

        /// <summary>
        /// Set C_LocTo_ID
        /// </summary>
        /// <param name="C_LocTo_ID">loc to</param>
        protected void SetC_LocTo_ID(int C_LocTo_ID)
        {
            _C_LocTo_ID = C_LocTo_ID;
        }

        /// <summary>
        /// Get User1_ID
        /// </summary>
        /// <returns>User 1</returns>
        public int GetUser1_ID()
        {
            int index = _po.Get_ColumnIndex("User1_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        /// Get User2_ID
        /// </summary>
        /// <returns>User 2</returns>
        public int GetUser2_ID()
        {
            int index = _po.Get_ColumnIndex("User2_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        /// Get User Defined value
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <returns>User defined</returns>
        public int GetValue(String ColumnName)
        {
            int index = _po.Get_ColumnIndex(ColumnName);
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);
                }
            }
            return 0;
        }

        bool _IsRectifyingProcess = false;

        /// <summary>
        /// Check if the process is post rectifying process
        /// </summary>
        /// <returns>true/ false</returns>
        public bool GetRectifyingProcess()
        {
            return _IsRectifyingProcess;
        }

        /// <summary>
        /// Rectify Post
        /// </summary>
        /// <param name="value">True/False</param>
        public void SetRectifyingProcess(bool value)
        {
            _IsRectifyingProcess = value;
        }

        /*************************************************************************/
        //  To be overwritten by Subclasses

        /// <summary>
        /// Load Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public abstract String LoadDocumentDetails();

        /// <summary>
        /// Get Source Currency Balance - subtracts line (and tax) amounts from total - no rounding
        /// </summary>
        /// <returns>positive amount, if total header is bigger than lines</returns>
        public abstract Decimal GetBalance();

        /// <summary>
        /// Create Facts (the accounting logic)
        /// </summary>
        /// <param name="as1">accounting schema</param>
        /// <returns>Facts</returns>
        public abstract List<Fact> CreateFacts(MVABAccountBook as1);
    }
}
