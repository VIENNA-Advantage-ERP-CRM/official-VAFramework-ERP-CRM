using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.DataBase;
using VAdvantage.Classes;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;
using Npgsql;
using MySql.Data.MySqlClient;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Threading;
using System.Runtime.CompilerServices;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MSequence : X_AD_Sequence
    {
        /**	Sequence for Table Document No's	*/
        private static String PREFIX_DOCSEQ = "DocumentNo_";
        /**	Start Number			*/
        public static int INIT_NO = 1000000;	//	1 Mio
        /**	Start System Number		*/
        public static int INIT_SYS_NO = 100;

        private static String NoYearNorMonth = "-";

        static VConnection vConn = new VConnection();

        private static VLogger s_log = VLogger.GetVLogger(typeof(MSequence).FullName);

        public MSequence(Ctx ctx, int AD_Sequence_ID, Trx trxName)
            : base(ctx, AD_Sequence_ID, trxName)
        {

            if (AD_Sequence_ID == 0)
            {
                //	setName (null);
                //
                SetIsTableID(false);
                SetStartNo(INIT_NO);
                SetCurrentNext(INIT_NO);
                SetCurrentNextSys(INIT_SYS_NO);
                SetIncrementNo(1);
                SetIsAutoSequence(true);
                SetIsAudited(false);
                SetStartNewYear(false);
            }
        }	//	MSequence

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MSequence(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }
        public MSequence(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }

        /**
         * 	New Document Sequence Constructor
         *	@param ctx context
         *	@param AD_Client_ID owner
         *	@param tableName name
         *	@param trxName transaction
         */
        public MSequence(Ctx ctx, int AD_Client_ID, String tableName, Trx trxName)
            : this(ctx, 0, trxName)
        {
            SetClientOrg(AD_Client_ID, 0);			//	Client Ownership
            SetName(PREFIX_DOCSEQ + tableName);
            SetDescription("DocumentNo/Value for Table " + tableName);
        }	//	MSequence;

        /**
         * 	New Document Sequence Constructor
         *	@param ctx context
         *	@param AD_Client_ID owner
         *	@param sequenceName name
         *	@param StartNo start
         *	@param trxName trx
         */
        public MSequence(Ctx ctx, int AD_Client_ID, String sequenceName, int StartNo, Trx trxName)
            : this(ctx, 0, trxName)
        {

            SetClientOrg(AD_Client_ID, 0);			//	Client Ownership
            SetName(sequenceName);
            SetDescription(sequenceName);
            SetStartNo(StartNo);
            SetCurrentNext(StartNo);
            SetCurrentNextSys(StartNo / 10);
        }	//	MSequence;

        public static int GetNextID(int AD_Client_ID, String TableName, Trx txtName)
        {
            // if SYSTEM_NATIVE_SEQUENCE is Y in System Config, then fetch Next ID from DB sequence
            if (MSysConfig.IsNativeSequence(false))
            {
                int m_sequence_id = VConnection.Get().GetDatabase().GetNextID(TableName + "_SEQ");
                return m_sequence_id;
            }

            if (DatabaseType.IsMSSql)
                return GetNextIDMSSql(AD_Client_ID, TableName);
            else if (DatabaseType.IsMySql)
                return GetNextIDMySql(AD_Client_ID, TableName);
            else if (DatabaseType.IsOracle)
                return GetNextIDOracle(AD_Client_ID, TableName, txtName);
            else if (DatabaseType.IsPostgre)
                return GetNextIDPostgre(AD_Client_ID, TableName, txtName);
            else
                return -1;
        }

        private static readonly object _idlock = new object();

        //[MethodImpl(MethodImplOptions.Synchronized)]
        public static int GetNextIDOracle(int AD_Client_ID, String TableName, Trx trxName)
        {
            lock (_idlock)
            {

                if (TableName == null || TableName.Length == 0)
                    throw new ArgumentException("TableName missing");
                int retValue = -1;



                //	Check ViennaSys
                bool viennaSys = false;
                if (viennaSys && AD_Client_ID > 11)
                    viennaSys = false;

                String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, AD_Sequence_ID "
                    + "FROM AD_Sequence "
                    + "WHERE Upper(Name)=Upper('" + TableName + "')"
                    + " AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y' "
                    + " FOR UPDATE";

                String updateSQL = "UPDATE AD_Sequence SET  CurrentNext = @CurrentNext@, CurrentNextSys = @CurrentNextSys@ "
                    + "WHERE Upper(Name)=Upper('" + TableName + "')"
                    + " AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y' ";

                //Comment an UPDATE FOR TEST//
                Trx trx = Trx.Get("ConnSH" + DateTime.Now.Ticks + (new Random(4)).ToString());

                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        DataSet ds = new DataSet();
                        ds = DB.ExecuteDataset(selectSQL, null, trx);


                        for (int irow = 0; irow <= ds.Tables[0].Rows.Count - 1; irow++)
                        {
                            int incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());
                            if (viennaSys)
                            {
                                retValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
                                ds.Tables[0].Rows[0]["CurrentNextSys"] = (retValue + incrementNo).ToString();
                            }
                            else
                            {
                                retValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
                                ds.Tables[0].Rows[0]["CurrentNext"] = (retValue + incrementNo).ToString();
                            }
                            updateSQL = updateSQL.Replace("@CurrentNext@", ds.Tables[0].Rows[0]["CurrentNext"].ToString()).Replace("@CurrentNextSys@", ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());

                            if (DataBase.DB.ExecuteQuery(updateSQL, null, trx) < 0)
                            // if (trx.ExecuteNonQuery(updateSQL, null, trx) < 0)
                            // if (trx.ExeNonQuery(updateSQL) < 0)
                            {
                                retValue = -1;
                            }
                        }
                        if (retValue == -1)
                            trx.Rollback();
                        else
                        {
                            if (trx != null)
                                trx.Commit();
                        }

                        break;		//	EXIT
                    }
                    catch (Exception e)
                    {
                        if (trx != null)
                        {
                            trx.Rollback();
                        }
                        s_log.Severe(e.ToString());
                    }
                    finally
                    {
                        trx.Close();
                    }
                }	//	loop

                return retValue;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int GetNextExportID(int AD_Client_ID, String TableName, Trx trxName)
        {
            if (TableName == null || TableName.Length == 0)
                throw new ArgumentException("TableName missing");
            int retValue = -1;

            //	Check ViennaSys
            bool viennaSys = false;
            if (viennaSys && AD_Client_ID > 11)
                viennaSys = false;

            String selectSQL = "SELECT Export_ID, CurrentNextSys, IncrementNo, AD_Sequence_ID "
                + "FROM AD_Sequence "
                + "WHERE Upper(Name)=Upper('" + TableName + "')"
                + " AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y' "
                + " FOR UPDATE";

            trxName = null;
            //SqlParameter[] param = new SqlParameter[1];
            Trx trx = trxName;
            IDbConnection conn = null;

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    if (trx != null)
                        conn = trx.GetConnection();
                    else
                        conn = DB.GetConnection();
                    //	Error
                    if (conn == null)
                        return -1;
#pragma warning disable 612, 618
                    DataSet ds = new DataSet();
                    //OracleDataAdapter da = new OracleDataAdapter(selectSQL, new OracleConnection(Ini.CreateConnectionString()));
                    OracleDataAdapter da = new OracleDataAdapter(selectSQL, (OracleConnection)conn);
                    OracleCommandBuilder bld = new OracleCommandBuilder(da);
                    da.Fill(ds);
                    for (int irow = 0; irow <= ds.Tables[0].Rows.Count - 1; irow++)
                    {
                        //	int AD_Sequence_ID = dr.getInt(4);
                        //
                        int tempRetValue = -1;
                        int incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());
                        if (viennaSys)
                        {
                            tempRetValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNextSys"] = tempRetValue + incrementNo;
                        }
                        else
                        {
                            tempRetValue = int.Parse(ds.Tables[0].Rows[0]["Export_ID"].ToString());
                            ds.Tables[0].Rows[0]["Export_ID"] = tempRetValue + incrementNo;
                        }

                        da.Update(ds);
                        retValue = tempRetValue;
                        s_log.Info("Export ID for Table " + TableName + ": " + retValue);
                    }

                    conn = null;
#pragma warning disable 612, 618
                    break;		//	EXIT
                }
                catch (Exception e)
                {
                    s_log.Severe("Error Generating Export ID for Table " + TableName + ": " + e.ToString());
                    return -1;
                }
                conn = null;
            }	//	loop

            return retValue;
        }

        public static int GetNextIDPostgre(int AD_Client_ID, String TableName, Trx trxName)
        {
            if (TableName == null || TableName.Length == 0)
                throw new ArgumentException("TableName missing");
            int retValue = -1;

            //	Check ViennaSys
            bool viennaSys = false;
            if (viennaSys && AD_Client_ID > 11)
                viennaSys = false;
            // string db_Name = "vienna";

            //if (DataBase.DB.UseMigratedConnection)
            // {
            // db_Name = WindowMigration.MDialog.GetMConnection().Db_name;
            //}
            // else
            // {
            // db_Name = VConnection.Get().Db_name;
            //  }

            String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, AD_Sequence_ID "
                + "FROM AD_Sequence "
                + "WHERE Upper(Name)=Upper('" + TableName + "')"
                + " AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y' "
                + " FOR UPDATE";

            String updateSQL = "UPDATE AD_Sequence SET  CurrentNext = @CurrentNext@, CurrentNextSys = @CurrentNextSys@ "
                + "WHERE Upper(Name)=Upper('" + TableName + "')"
                + " AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y' ";

            //Comment an UPDATE FOR TEST//
            Trx trx = Trx.Get("ConnSH" + DateTime.Now.Ticks + (new Random(4)).ToString());

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    DataSet ds = new DataSet();
                    ds = DB.ExecuteDataset(selectSQL, null, trx);

                    for (int irow = 0; irow <= ds.Tables[0].Rows.Count - 1; irow++)
                    {
                        int incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());
                        if (viennaSys)
                        {
                            retValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNextSys"] = (retValue + incrementNo).ToString();
                        }
                        else
                        {
                            retValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNext"] = (retValue + incrementNo).ToString();
                        }
                        updateSQL = updateSQL.Replace("@CurrentNext@", ds.Tables[0].Rows[0]["CurrentNext"].ToString()).Replace("@CurrentNextSys@", ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());

                        if (DataBase.DB.ExecuteQuery(updateSQL, null, trx) < 0)
                        //if (trx.ExecuteNonQuery(updateSQL, null, trx) < 0)
                        //if (trx.ExeNonQuery(updateSQL) < 0)
                        {
                            retValue = -1;
                        }
                    }
                    if (retValue == -1)
                        trx.Rollback();
                    else
                    {
                        if (trx != null)
                            trx.Commit();
                    }

                    break;		//	EXIT
                }
                catch (Exception e)
                {
                    if (trx != null)
                    {
                        trx.Rollback();
                    }
                    s_log.Severe(e.ToString());
                }
                finally
                {
                    trx.Close();
                }
            }	//	loop

            return retValue;
        }


        public static int GetNextIDMySql(int AD_Client_ID, String TableName)
        {
            if (TableName == null || TableName.Length == 0)
                throw new ArgumentException("TableName missing");
            int retValue = -1;

            //	Check viennaSys
            bool viennaSys = false;
            if (viennaSys && AD_Client_ID > 11)
                viennaSys = false;

            String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, AD_Sequence_ID "
                + "FROM AD_Sequence "
                + "WHERE Name='" + TableName + "'"
                + " AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y' "
                + " FOR UPDATE";

            //SqlParameter[] param = new SqlParameter[1];

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    DataSet ds = new DataSet();
                    MySqlDataAdapter da = new MySqlDataAdapter(selectSQL, new MySqlConnection(DB.GetConnectionString()));
                    MySqlCommandBuilder bld = new MySqlCommandBuilder(da);
                    da.Fill(ds);
                    for (int irow = 0; irow <= ds.Tables[0].Rows.Count - 1; irow++)
                    {
                        //	int AD_Sequence_ID = dr.getInt(4);
                        //
                        int incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());
                        if (viennaSys)
                        {
                            retValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNextSys"] = retValue + incrementNo;
                        }
                        else
                        {
                            retValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNext"] = retValue + incrementNo;
                        }
                        da.Update(ds);
                    }
                    break;		//	EXIT
                }
                catch (Exception e)
                {
                    s_log.Severe(e.ToString());
                }
            }	//	loop

            return retValue;
        }

        public static int GetNextIDMSSql(int AD_Client_ID, String TableName)
        {
            if (TableName == null || TableName.Length == 0)
                throw new ArgumentException("TableName missing");
            int retValue = -1;

            //	Check viennaSys
            bool viennaSys = false;
            if (viennaSys && AD_Client_ID > 11)
                viennaSys = false;

            String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, AD_Sequence_ID "
                + "FROM AD_Sequence "
                + "WHERE Name='" + TableName + "'"
                + " AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y' "
                + " FOR UPDATE";

            //SqlParameter[] param = new SqlParameter[1];

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(selectSQL, new SqlConnection(DB.GetConnectionString()));
                    SqlCommandBuilder bld = new SqlCommandBuilder(da);

                    da.Fill(ds);
                    for (int irow = 0; irow <= ds.Tables[0].Rows.Count - 1; irow++)
                    {
                        //	int AD_Sequence_ID = dr.getInt(4);
                        //
                        int incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());
                        if (viennaSys)
                        {
                            retValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNextSys"] = retValue + incrementNo;
                        }
                        else
                        {
                            retValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNext"] = retValue + incrementNo;
                        }
                        da.Update(ds);
                    }
                    break;		//	EXIT
                }
                catch (Exception e)
                {
                    s_log.Severe(e.ToString());
                }
            }	//	loop

            return retValue;
        }

        public int GetNextID()
        {
            int retValue = GetCurrentNext();
            if (MSysConfig.IsNativeSequence(false) && IsTableID())
            {
                SetCurrentNext(retValue + GetIncrementNo());
            }

            return retValue;
        }

        public int GetCurrentNext()
        {
            if (MSysConfig.IsNativeSequence(false) && IsTableID())
            {
                return DB.GetNextID(GetAD_Client_ID(), GetName(), Get_TrxName());
            }
            else
            {
                return base.GetCurrentNext();
            }
        }

        public void SetCurrentNext(int CurrentNext)
        {
            if (MSysConfig.IsNativeSequence(false) && IsTableID())
            {
                while (true)
                {
                    int id = DB.GetNextID(GetAD_Client_ID(), GetName(), Get_TrxName());
                    if (id < 0 || id >= (CurrentNext - 1))
                        break;
                }
            }
            else
            {
                base.SetCurrentNext(CurrentNext);
            }
        }


        /// <summary>
        /// it will lock the code that check if lock exist for table then return lock otherwise create.
        /// It is required so that if two request for same table reached this portion of code, then they must execute in sync mode. 
        /// </summary>
        private static readonly object lockOnLocking = new object();

        //Contains lock object for each TableName And SeqNo(for document no generated from docType)
        private static readonly Dictionary<string, object> lockObjects = new Dictionary<string, object>();

        private static object GetLock(string tableName)
        {
            lock (lockOnLocking)
            {
                if (lockObjects.ContainsKey(tableName))
                {
                    return lockObjects[tableName];
                }
                return lockObjects[tableName] = new object();
            }
        }

        /// <summary>
        /// Get Document No based on Document Type
        /// </summary>
        /// <param name="C_DocType_ID">document type</param>
        /// <param name="trxName">optional Transaction Name</param>
        /// <param name="ctx">ctx</param>
        /// <param name="definite">asking for a definitive or temporary sequence</param>
        /// <param name="po">object of PO class</param>
        /// <returns>document no or null</returns>
        /// 


        private static CCache<int, X_C_DocType> s_doctypecache = new CCache<int, X_C_DocType>("AD_seq_doctype", 10);
        private static X_C_DocType GetDocType(Ctx ctx, int C_DocType_ID)
        {
            int key = (int)C_DocType_ID;
            X_C_DocType retValue = (X_C_DocType)s_doctypecache[key];
            if (retValue == null)
            {
                retValue = new X_C_DocType(ctx, C_DocType_ID, null);
                s_doctypecache.Add(key, retValue);
            }
            return retValue;
        }

        public static String GetDocumentNo(int C_DocType_ID, Trx trxName, Ctx ctx, Boolean definite, PO po)
        {
            if (C_DocType_ID == 0)
            {
                s_log.Severe("C_DocType_ID=0");
                return null;
            }

            X_C_DocType dt = GetDocType(ctx, C_DocType_ID);	//	wrong for SERVER, but r/o
            if (dt != null && !dt.IsDocNoControlled())
            {
                s_log.Finer("DocType_ID=" + C_DocType_ID + " Not DocNo controlled");
                return null;
            }

            if (dt == null || dt.GetDocNoSequence_ID() == 0)
            {
                s_log.Warning("No Sequence for DocType - " + dt);
                return null;
            }

            // if "Overwrite Sequence on complete" is not upto mark.
            if (definite && !dt.IsOverwriteSeqOnComplete())
            {
                s_log.Warning("DocType_ID=" + C_DocType_ID + " Not Sequence Overwrite on Complete");
                return null;
            }

            lock (GetLock(dt.GetDocNoSequence_ID().ToString()))
            {
                // Get Completed Document sequence if "Overwrite Sequence on complete" is marked as true on Document Type.
                int seqID = definite ? dt.GetCompletedSequence_ID() : dt.GetDocNoSequence_ID();
                MSequence seq = new MSequence(ctx, seqID, trxName);

                if (VLogMgt.IsLevelAll())
                    s_log.Finest("DocType_ID=" + C_DocType_ID + " [" + trxName + "]");
                return GetDocumentNoFromSeq(seq, trxName, po);
            }
            return null;
        }

        public static String GetDocumentNoFromSeq(MSequence seq, Trx trxName, PO po)
        {
            //	Check viennaSys
            Boolean viennaSys = false;
            if (viennaSys && po.GetCtx().GetAD_Client_ID() > 11)
                viennaSys = false;
            //

            int AD_Sequence_ID = seq.GetAD_Sequence_ID();
            Boolean isStartNewYear = seq.IsStartNewYear();
            Boolean isStartNewMonth = seq.IsStartNewMonth();

            Boolean addYearPrefix = seq.IsAddYearPrefix();
            Boolean addMonthYear = seq.IsAddMonthYear();
            String yearMonthFormat = seq.GetYearMonthFormat();
            String separator = seq.GetSeparator();

            String dateColumn = seq.GetDateColumn();
            Boolean isUseOrgLevel = seq.IsOrgLevelSequence();
            String orgColumn = seq.GetOrgColumn();
            int startNo = seq.GetStartNo();
            int incrementNo = seq.GetIncrementNo();
            String prefix = seq.GetPrefix();
            String suffix = seq.GetSuffix();
            //get the PrefixAndDocNoSeperator
            string prefixAndDocNoSeperator = null;
            //check the column available or not
            //column index starts from 0
            if (seq.Get_ColumnIndex("PrefixAndDocNoSeperator") > -1)
                prefixAndDocNoSeperator = seq.GetPrefixAndDocNoSeperator();

            bool isAutoSequence = seq.IsAutoSequence();
            bool isUseSeparateTrx = false;

            String selectSQL = null;
            if (isStartNewYear || isUseOrgLevel)
            {
                selectSQL = "SELECT y.CurrentNext, s.CurrentNextSys, y.prefix, y.suffix, y.ADDYEARPREFIX, y.AddMonthYear "
                        + "FROM AD_Sequence_No y, AD_Sequence s "
                        + "WHERE y.AD_Sequence_ID = s.AD_Sequence_ID "
                        + "AND s.AD_Sequence_ID = " + AD_Sequence_ID
                        + " AND y.CalendarYear = @param1"
                        + " AND y.AD_Org_ID = @param2"
                        + " AND s.IsActive='Y' AND s.IsTableID='N' AND s.IsAutoSequence='Y' "
                        + "ORDER BY s.AD_Client_ID DESC";
            }
            else
            {
                selectSQL = "SELECT s.CurrentNext, s.CurrentNextSys "
                        + "FROM AD_Sequence s "
                        + "WHERE s.AD_Sequence_ID = " + AD_Sequence_ID
                        + " AND s.IsActive='Y' AND s.IsTableID='N' AND s.IsAutoSequence='Y' "
                        + "ORDER BY s.AD_Client_ID DESC";
            }
            selectSQL = selectSQL + " FOR UPDATE";

            Trx trx = null;
            // Check if Maintain Seprate Trx is available on Document Sequence
            if (seq.Get_ColumnIndex("IsUseSeparateTrx") > -1)
            {
                isUseSeparateTrx = seq.IsUseSeparateTrx();
            }

            // if Maintain Seprate Trx is true then it will create separate transaction to get Document No.
            if (isUseSeparateTrx || trxName == null || !trxName.UseSameTrxForDocNo)
            {
                trx = Trx.Get("ConnDDH" + DateTime.Now.Ticks + (new Random(4)).ToString());
            }
            else
            {
                trx = trxName;
            }
            //

            String calendarYearMonth = NoYearNorMonth;
            String yearmonthPrefix = "";

            int docOrg_ID = 0;
            int next = -1;

            String updateSQL = "";
            DataSet rs = new DataSet();
            try
            {
                DateTime? docDate = null;
                if (po != null && dateColumn != null && dateColumn.Length > 0)
                {
                    docDate = (DateTime)po.Get_Value(dateColumn);
                }
                else
                {
                    docDate = DateTime.Now;
                }

                if (isStartNewYear)
                {
                    calendarYearMonth = docDate.Value.Year.ToString();

                    // if Start Document Sequence every month value is True.
                    if (isStartNewMonth)
                    {
                        calendarYearMonth += (docDate.Value.Month.ToString().Length > 1 ? docDate.Value.Month.ToString() : "0" + docDate.Value.Month.ToString());
                    }
                }

                if (isUseOrgLevel)
                {
                    if (po != null && orgColumn != null && orgColumn.Length > 0)
                    {
                        docOrg_ID = Util.GetValueOfInt(po.Get_Value(orgColumn));
                    }
                }

                if (isStartNewYear || isUseOrgLevel)
                {
                    selectSQL = selectSQL.Replace("@param1", "'" + calendarYearMonth + "'").Replace("@param2", docOrg_ID.ToString());
                    rs = trx.ExecuteDataSet(selectSQL, null);

                    // Check organization level document sequence settings and if exist then override general settings.
                    if (rs != null && rs.Tables[0].Rows.Count > 0)
                    {
                        string _prefix = Convert.ToString(rs.Tables[0].Rows[0]["Prefix"]);
                        string _suffix = Convert.ToString(rs.Tables[0].Rows[0]["suffix"]);
                        char addyearprefix = Convert.ToChar(rs.Tables[0].Rows[0]["ADDYEARPREFIX"]);
                        char addmonthyear = Convert.ToChar(rs.Tables[0].Rows[0]["AddMonthYear"]);

                        // if any of these fields contains data, then override parent tab's data....
                        if (!string.IsNullOrEmpty(_prefix) || !string.IsNullOrEmpty(_suffix) || addyearprefix.Equals('Y'))
                        {

                            prefix = _prefix;
                            suffix = _suffix;
                            if (addyearprefix.Equals('Y'))
                            {
                                addYearPrefix = true;
                            }
                            else
                            {
                                addYearPrefix = false;
                            }

                            addMonthYear = addmonthyear.Equals('Y');
                        }
                    }
                }
                else
                {
                    rs = trx.ExecuteDataSet(selectSQL, null);
                }
                if (rs != null && rs.Tables.Count > 0 && rs.Tables[0].Rows.Count > 0)
                {
                    if (s_log.IsLoggable(Level.FINE))
                        s_log.Fine("AD_Sequence_ID=" + AD_Sequence_ID);

                    if (viennaSys)
                    {
                        next = int.Parse(rs.Tables[0].Rows[0]["CurrentNextSys"].ToString());
                        updateSQL = "UPDATE AD_Sequence SET CurrentNextSys = CurrentNextSys + " + incrementNo + " WHERE AD_Sequence_ID = " + AD_Sequence_ID;
                    }
                    else
                    {
                        if (isStartNewYear || isUseOrgLevel)
                            updateSQL = "UPDATE AD_Sequence_No SET CurrentNext = CurrentNext + " + incrementNo + " WHERE AD_Sequence_ID= " + AD_Sequence_ID + " AND CalendarYear = '" + calendarYearMonth + "' AND AD_Org_ID=" + docOrg_ID;
                        else
                            updateSQL = "UPDATE AD_Sequence SET CurrentNext = CurrentNext + " + incrementNo + " WHERE AD_Sequence_ID=" + AD_Sequence_ID;
                        next = int.Parse(rs.Tables[0].Rows[0]["CurrentNext"].ToString());
                    }

                    if (trx.ExecuteQuery(updateSQL, null) < 0)
                    {
                        next = -2;
                    }

                    // Commit or rollback new transaction in case of Maintain seprate trx is true on Document Sequence.
                    if (isUseSeparateTrx || trxName == null || !trxName.UseSameTrxForDocNo)
                    {
                        if (next == -2)
                            trx.Rollback();
                        else
                        {
                            trx.Commit();
                        }
                    }
                }
                else
                { // did not find sequence no
                    if (isUseOrgLevel || isStartNewYear)
                    {	// create sequence (CurrentNo = StartNo + IncrementNo) for this year/org and return first number (=StartNo)
                        next = startNo;

                        X_AD_Sequence_No seqno = new X_AD_Sequence_No(po.GetCtx(), 0, trxName);
                        seqno.SetAD_Sequence_ID(AD_Sequence_ID);
                        seqno.SetAD_Org_ID(docOrg_ID);
                        seqno.SetCalendarYear(calendarYearMonth);
                        seqno.SetCurrentNext(startNo + incrementNo);
                        seqno.Save();
                    }
                    else	// standard
                    {
                        s_log.Warning("(Sequence)- no record found - " + seq);
                        next = -2;
                    }
                }

                if (addYearPrefix)
                {
                    calendarYearMonth = String.Format("YYYYMM", docDate.Value);
                    yearmonthPrefix = docDate.Value.Year.ToString();

                    // Appended the Year based on the Format selected in Year Month Format
                    if (yearMonthFormat == "2")
                    {
                        yearmonthPrefix = yearmonthPrefix.Substring(yearmonthPrefix.Length - 2);
                    }

                    // if Add Month after Year value is True.
                    if (addMonthYear)
                    {
                        // Appended the separator if selected in Year Month Separator.
                        //if addMonthYear is check then it will add prefixAndDocNoSeperator to separate the date
                        yearmonthPrefix += separator + (docDate.Value.Month.ToString().Length > 1 ? docDate.Value.Month.ToString() : "0" + docDate.Value.Month.ToString());
                    }
                    //to separate year/date from sequenceNo adding separtor
                    if (!string.IsNullOrEmpty(prefixAndDocNoSeperator))
                    {
                        yearmonthPrefix += prefixAndDocNoSeperator;
                    }
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, "(DocType) [" + trx + "]", e);
                next = -2;
                if (trx != null)
                {
                    if (isUseSeparateTrx || trxName == null || !trxName.UseSameTrxForDocNo)
                    {
                        trx.Rollback();
                    }
                }
            }
            finally
            {
                if (isUseSeparateTrx || trxName == null || !trxName.UseSameTrxForDocNo)
                {
                    trx.Close();
                }
            }

            //	Error
            if (next < 0)
                return null;

            //	create DocumentNo
            StringBuilder doc = new StringBuilder();
            if (prefix != null && prefix.Length > 0)
                doc.Append(prefix);

            // if Add Year to Prefix checkbox is true then append the year with prefix.
            if (addYearPrefix)
            {
                doc.Append(yearmonthPrefix);
            }
            doc.Append(next);
            if (suffix != null && suffix.Length > 0)
                doc.Append(suffix);
            String documentNo = doc.ToString();
            return documentNo;
        }

        /// <summary>
        /// Get Document No from table
        /// </summary>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="TableName">table name</param>
        /// <param name="trxName">optional Transaction Name</param>
        /// <returns>document no or null</returns>
        /// 
        public static String GetDocumentNo(int AD_Client_ID, String TableName, Trx trxIn, Ctx ctx)
        {
            if (TableName == null || TableName.Length == 0)
            {
                throw new ArgumentException("TableName missing");
            }

            lock (GetLock(TableName))
            {
                //	Check ViennaSys
                bool viennaSys = false;
                if (viennaSys && AD_Client_ID > 11)
                    viennaSys = false;
                //s_log.Severe("DocNo__" + Table_Name + "    " + trxIn.GetTrxName());
                String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, Prefix, Suffix, AD_Sequence_ID "
                    + "FROM AD_Sequence "
                    + "WHERE Name='" + PREFIX_DOCSEQ + TableName + "'"
                    + " AND AD_Client_ID = " + AD_Client_ID
                    + " AND IsActive='Y' AND IsTableID='N' AND IsAutoSequence='Y' ";
                if (VAdvantage.DataBase.DatabaseType.IsOracle)
                {
                    selectSQL += " ORDER BY AD_Client_ID DESC ";
                }
                selectSQL += "FOR UPDATE";


                String updateSQL = "UPDATE AD_Sequence SET  CurrentNext =@CurrentNext@ , CurrentNextSys = @CurrentNextSys@ WHERE Name='" + PREFIX_DOCSEQ + TableName + "'"
                    + " AND AD_Client_ID = " + AD_Client_ID
                    + " AND IsActive='Y' AND IsTableID='N' AND IsAutoSequence='Y' ";

                Trx trx = null;

                if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                {
                    trx = Trx.Get("ConnDH" + DateTime.Now.Ticks + (new Random(4)).ToString());
                }
                else
                {
                    trx = trxIn;
                }

                int next = -1;
                String prefix = "";
                String suffix = "";

                int incrementNo = 0;
                try
                {
                    DataSet ds = new DataSet();
                    //  s_log.Severe("DocNo__ExecuteDSEte" + Table_Name + "    " + trxIn.GetTrxName());
                    //ds = DB.ExecuteDatasetDoc(selectSQL, null, trx);
                    // dynamic result = null;


                    ds = trx.ExecuteDataSet(selectSQL, null);

                    //if (result.GetType() == typeof(DataSet))
                    //{
                    //    ds = result;
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        prefix = ds.Tables[0].Rows[0]["Prefix"].ToString();
                        suffix = ds.Tables[0].Rows[0]["Suffix"].ToString();
                        incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());

                        if (viennaSys)
                        {
                            next = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNextSys"] = (next + incrementNo).ToString();
                        }
                        else
                        {
                            next = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNext"] = (next + incrementNo).ToString();
                        }

                        updateSQL = updateSQL.Replace("@CurrentNextSys@", ds.Tables[0].Rows[0]["CurrentNextSys"].ToString()).Replace("@CurrentNext@", ds.Tables[0].Rows[0]["CurrentNext"].ToString());
                        // s_log.Severe("DocNo__Execute" + Table_Name + "    " + trxIn.GetTrxName());
                        //if (DB.ExecuteQueryDoc(updateSQL, null, trx) < 0)
                        // if (trx.ExecuteNonQuery(updateSQL, null, trx) < 0)
                        if (trx.ExecuteQuery(updateSQL, null) < 0)
                        {
                            next = -2;
                        }
                        //s_log.Severe("DocNo__ExecuteCompleted" + Table_Name + "    " + trxIn.GetTrxName());
                        if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                        {
                            if (next == -2)
                            {
                                if (trx != null)
                                    trx.Rollback();
                            }
                            else
                            {
                                if (trx != null)
                                    trx.Commit();
                            }
                        }
                    }
                    else
                    {
                        MSequence seq = new MSequence(ctx, AD_Client_ID, TableName, trx);
                        next = seq.GetNextID();


                        if (seq.Save(trx))
                        {
                            if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                            {
                                if (trx != null)
                                    trx.Commit();
                            }
                        }
                        else
                        {
                            if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                            {
                                if (trx != null)
                                    trx.Rollback();
                            }
                        }

                    }
                    //}


                }
                catch
                {
                    next = -2;
                    if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                    {
                        if (trx != null)
                        {
                            trx.Rollback();
                        }
                    }
                }
                finally
                {
                    if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                    {
                        if (trx != null)
                            trx.Close();
                    }
                }

                if (next < 0)
                    return null;

                //	create DocumentNo
                StringBuilder doc = new StringBuilder();
                if (prefix != null && prefix.Length > 0)
                    doc.Append(prefix);
                doc.Append(next);
                if (suffix != null && suffix.Length > 0)
                    doc.Append(suffix);
                String documentNo = doc.ToString();
                return documentNo;
            }
            return "";
        }





        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public static String GetDocumentNo(int AD_Client_ID, String TableName, Trx trxName,Ctx ctx)
        //{
        //    if (TableName == null || TableName.Length == 0)
        //    {
        //        throw new ArgumentException("TableName missing");
        //    }

        //    //	Check ViennaSys
        //    bool viennaSys = false;
        //    if (viennaSys && AD_Client_ID > 11)
        //        viennaSys = false;

        //    String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, Prefix, Suffix, AD_Sequence_ID "
        //        + "FROM AD_Sequence "
        //        + "WHERE Name='" + PREFIX_DOCSEQ + TableName + "'"
        //        + " AND AD_Client_ID = " + AD_Client_ID
        //        + " AND IsActive='Y' AND IsTableID='N' AND IsAutoSequence='Y' ";
        //    if (VAdvantage.DataBase.DatabaseType.IsOracle)
        //    {
        //        selectSQL += " ORDER BY AD_Client_ID DESC ";
        //    }
        //    selectSQL += "FOR UPDATE";


        //    String updateSQL = "UPDATE AD_Sequence SET  CurrentNext =@CurrentNext@ , CurrentNextSys = @CurrentNextSys@ WHERE Name='" + PREFIX_DOCSEQ + TableName + "'"
        //        + " AND AD_Client_ID = " + AD_Client_ID
        //        + " AND IsActive='Y' AND IsTableID='N' AND IsAutoSequence='Y' ";

        //    Trx trx = Trx.GetTrx("Conn" + DateTime.Now.Ticks + (new Random(4)).ToString());

        //    //
        //    int incrementNo = 0;
        //    int next = -1;
        //    String prefix = "";
        //    String suffix = "";
        //    try
        //    {
        //        //if (DatabaseType.IsOracle)
        //        //{
        //        //}
        //        //else if (DatabaseType.IsPostgre)
        //        //{

        //        //}
        //        //else if (DatabaseType.IsMSSql)
        //        //{
        //        //}
        //        //else if (DatabaseType.IsMySql)
        //        //{
        //        //}

        //        DataSet ds = new DataSet();
        //        ds = DB.ExecuteDataset(selectSQL, null, trx);

        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            prefix = ds.Tables[0].Rows[0]["Prefix"].ToString();
        //            suffix = ds.Tables[0].Rows[0]["Suffix"].ToString();
        //            incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());

        //            if (viennaSys)
        //            {
        //                next = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
        //                ds.Tables[0].Rows[0]["CurrentNextSys"] = (next + incrementNo).ToString();
        //            }
        //            else
        //            {
        //                next = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
        //                ds.Tables[0].Rows[0]["CurrentNext"] = (next + incrementNo).ToString();
        //            }

        //            updateSQL = updateSQL.Replace("@CurrentNextSys@", ds.Tables[0].Rows[0]["CurrentNextSys"].ToString()).Replace("@CurrentNext@", ds.Tables[0].Rows[0]["CurrentNext"].ToString());

        //            if (DB.ExecuteQuery(updateSQL, null, trx) < 0)
        //            {
        //                next = -2;
        //            }


        //            if (next == -2)
        //                trx.Rollback();
        //            else
        //            {
        //                if (trx != null)
        //                    trx.Commit();
        //            }

        //        }
        //        else
        //        {
        //            MSequence seq = new MSequence(ctx, AD_Client_ID, TableName, trx);
        //            next = seq.GetNextID();

        //            if ((Util.GetValueOfInt(ctx.GetContext("#AD_Client_ID")) == 0)
        //                && ((Util.GetValueOfInt(ctx.GetContext("#AD_Client_ID")) == 0)
        //                && (Util.GetValueOfInt(ctx.GetContext("#AD_Client_ID")) != AD_Client_ID)))
        //            {
        //                trx.Commit();
        //            }
        //            else
        //            {
        //                if (seq.Save(trx))
        //                {
        //                    trx.Commit();
        //                }
        //                else
        //                {
        //                    trx.Close();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        next = -2;
        //        if (trx != null)
        //        {
        //            trx.Rollback();
        //        }
        //    }
        //    finally
        //    {
        //        trx.Close();
        //    }

        //    if (next < 0)
        //        return null;

        //    //	create DocumentNo
        //    StringBuilder doc = new StringBuilder();
        //    if (prefix != null && prefix.Length > 0)
        //        doc.Append(prefix);
        //    doc.Append(next);
        //    if (suffix != null && suffix.Length > 0)
        //        doc.Append(suffix);
        //    String documentNo = doc.ToString();

        //    return documentNo;
        //}


        ///// <summary>
        ///// Get Document No from table
        ///// </summary>
        ///// <param name="AD_Client_ID">client</param>
        ///// <param name="TableName">table name</param>
        ///// <param name="trxName">optional Transaction Name</param>
        ///// <returns>document no or null</returns>
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public static String GetDocumentNo(int AD_Client_ID, String TableName, Trx trxName)
        //{
        //    if (TableName == null || TableName.Length == 0)
        //    {
        //        throw new ArgumentException("TableName missing");
        //    }

        //    //	Check viennaSys
        //    bool viennaSys = false;
        //    if (viennaSys && AD_Client_ID > 11)
        //        viennaSys = false;
        //    //if (CLogMgt.isLevel(LOGLEVEL))
        //      //  _log.Log(LOGLEVEL, TableName + " - ViennaSys=" + ViennaSys + " [" + trxName + "]");

        //    String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, Prefix, Suffix, AD_Sequence_ID "
        //        + "FROM AD_Sequence "
        //        + "WHERE Name='" + PREFIX_DOCSEQ + TableName + "'"
        //        //jz fix duplicated nextID  + " AND AD_Client_ID IN (0,?)"
        //        + " AND AD_Client_ID = " + AD_Client_ID
        //        + " AND IsActive='Y' AND IsTableID='N' AND IsAutoSequence='Y' ";
        //    if (VAdvantage.DataBase.DatabaseType.IsOracle)
        //    {
        //        selectSQL += " ORDER BY AD_Client_ID DESC ";
        //    }
        //  //  selectSQL += "FOR UPDATE";


        //    //
        //    int incrementNo = 0;
        //    int next = -1;
        //    String prefix = "";
        //    String suffix = "";
        //    try
        //    {
        //        //
        //        //IDataAdapter dap = null;
        //        //DataSet ds1 = DataBase.DB.ExecuteDataset(selectSQL,null,null);
        //        //dap.Fill(ds);

        //        OracleCommandBuilder ocb = null;
        //        MySqlCommandBuilder mscb = null;
        //        SqlCommandBuilder scb = null;
        //        NpgsqlCommandBuilder ncb = null;

        //        IDataAdapter dap = null;
        //        //check 

        //        if (DatabaseType.IsOracle)
        //        {
        //            dap = GetDataAdapter(selectSQL);
        //            ocb = new OracleCommandBuilder((OracleDataAdapter)dap);
        //        }
        //        else if (DatabaseType.IsPostgre)
        //        {

        //            string strDB1 = vConn.Db_name;
        //            //string strDB = Ini.GetProperty(GlobalVariable.DBNAME_NODE);
        //            string finalSQL = "set search_path to " + strDB1 + ", public;" + selectSQL;
        //            dap = GetDataAdapter(finalSQL);
        //            ncb = new NpgsqlCommandBuilder((NpgsqlDataAdapter)dap);
        //        }
        //        else if (DatabaseType.IsMSSql)
        //        {
        //            dap = GetDataAdapter(selectSQL);
        //            scb = new SqlCommandBuilder((SqlDataAdapter)dap);
        //        }
        //        else if (DatabaseType.IsMySql)
        //        {
        //            dap = GetDataAdapter(selectSQL);
        //            mscb = new MySqlCommandBuilder((MySqlDataAdapter)dap);
        //        }

        //        DataSet ds = new DataSet();
        //        dap.Fill(ds);

        //       // _log.Fine("AC=" + conn.getAutoCommit() + " -Iso=" + conn.getTransactionIsolation() 
        //        //		+ " - Type=" + pstmt.getResultSetType() + " - Concur=" + pstmt.getResultSetConcurrency());
        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            prefix = ds.Tables[0].Rows[0]["Prefix"].ToString();
        //            suffix = ds.Tables[0].Rows[0]["Suffix"].ToString();
        //            incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());

        //            if (viennaSys)
        //            {
        //                next = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
        //                ds.Tables[0].Rows[0]["CurrentNextSys"] = next + incrementNo;
        //            }
        //            else
        //            {
        //                next = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
        //                ds.Tables[0].Rows[0]["CurrentNext"] = next + incrementNo;
        //            }
        //            dap.Update(ds);

        //        }
        //        else
        //        {
        //            _log.Warning("(Table) - no record found - " + TableName);
        //            MSequence seq = new MSequence(Utility.Env.GetContext(), AD_Client_ID, TableName, null);
        //            next = seq.GetNextID();
        //            seq.Save();
        //            //seq.saveNew();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _log.Log(Level.SEVERE, "(Table) [" + trxName + "]", e);
        //        next = -2;
        //    }

        //    if (next < 0)
        //        return null;

        //    //	create DocumentNo
        //    StringBuilder doc = new StringBuilder();
        //    if (prefix != null && prefix.Length > 0)
        //        doc.Append(prefix);
        //    doc.Append(next);
        //    if (suffix != null && suffix.Length > 0)
        //        doc.Append(suffix);
        //    String documentNo = doc.ToString();

        //    return documentNo;
        //}
        //#pragma warning disable 612, 618
        //        private static IDataAdapter GetDataAdapter(string selectSQL)
        //        {
        //            if (DatabaseType.IsOracle)
        //                return new OracleDataAdapter(selectSQL, Ini.CreateConnectionString());
        //            else if (DatabaseType.IsPostgre)
        //                return new NpgsqlDataAdapter(selectSQL, Ini.CreateConnectionString());
        //            else if (DatabaseType.IsMSSql)
        //                return new SqlDataAdapter(selectSQL, Ini.CreateConnectionString());
        //            else if (DatabaseType.IsMySql)
        //                return new MySqlDataAdapter(selectSQL, Ini.CreateConnectionString());

        //            return null;
        //        }
        //#pragma warning restore 612, 618

        /// <summary>
        /// Get Document No based on Document Type
        /// </summary>
        /// <param name="C_DocType_ID">document type</param>
        /// <param name="trxName">optional Transaction Name</param>
        /// <returns>document no or null</returns>
        /// 
        //[MethodImpl(MethodImplOptions.Synchronized)]
        public static String GetDocumentNo(int C_DocType_ID, Trx trxIn, Ctx ctx)
        {

            if (C_DocType_ID == 0)
            {
                s_log.Severe("C_DocType_ID=0");
                return null;
            }
            X_C_DocType dt = GetDocType(ctx, C_DocType_ID);	//	wrong for SERVER, but r/o
            if (dt != null && !dt.IsDocNoControlled())
            {
                s_log.Finer("DocType_ID=" + C_DocType_ID + " Not DocNo controlled");
                return null;
            }
            if (dt == null || dt.GetDocNoSequence_ID() == 0)
            {
                s_log.Warning("No Sequence for DocType - " + dt);
                return null;
            }

            lock (GetLock(dt.GetDocNoSequence_ID().ToString()))
            {
                int next = -1;
                //	Check ViennaSys
                Boolean viennaSys = false; // Ini.IsPropertyBool(Ini._VIENNASYS);

                String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, Prefix, Suffix, AD_Client_ID, AD_Sequence_ID "
                    + "FROM AD_Sequence "
                    + "WHERE AD_Sequence_ID=" + dt.GetDocNoSequence_ID()
                    + " AND IsActive='Y' AND IsTableID='N' AND IsAutoSequence='Y' ";

                selectSQL += "FOR UPDATE ";

                String updateSQL = "UPDATE AD_Sequence SET CurrentNext = @CurrentNext@, CurrentNextSys=@CurrentNextSys@ "
                   + " WHERE AD_Sequence_ID=" + dt.GetDocNoSequence_ID()
                   + " AND IsActive='Y' AND IsTableID='N' AND IsAutoSequence='Y' ";

                Trx trx = null;

                if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                {
                    trx = Trx.Get("ConnDDH" + DateTime.Now.Ticks + (new Random(4)).ToString());
                }
                else
                {
                    trx = trxIn;
                }

                //	+ " FOR UPDATE";
                //




                String prefix = "";
                String suffix = "";
                int incrementNo = 0;
                try
                {
                    DataSet ds = new DataSet();
                    ds = trx.ExecuteDataSet(selectSQL, null);
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());
                        prefix = ds.Tables[0].Rows[0]["Prefix"].ToString();
                        suffix = ds.Tables[0].Rows[0]["Suffix"].ToString();
                        int AD_Client_ID = int.Parse(ds.Tables[0].Rows[0]["AD_Client_ID"].ToString());
                        if (viennaSys && AD_Client_ID > 11)
                            viennaSys = false;
                        //	AD_Sequence_ID = dr.getInt(7);
                        if (viennaSys)
                        {
                            next = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNextSys"] = (next + incrementNo).ToString();
                        }
                        else
                        {
                            next = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNext"] = (next + incrementNo).ToString();
                        }

                        updateSQL = updateSQL.Replace("@CurrentNextSys@", ds.Tables[0].Rows[0]["CurrentNextSys"].ToString()).Replace("@CurrentNext@", ds.Tables[0].Rows[0]["CurrentNext"].ToString());

                        //if (DB.ExecuteQueryDoc(updateSQL, null, trx) < 0)
                        //if (trx.ExecuteNonQuery(updateSQL, null, trx) < 0)
                        if (trx.ExecuteQuery(updateSQL, null) < 0)
                        {
                            next = -2;
                        }

                        if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                        {
                            if (next == -2)
                                trx.Rollback();
                            else
                            {
                                trx.Commit();
                            }
                        }
                    }
                    else
                    {
                        s_log.Warning("(DocType)- no record found - " + dt);
                        next = -2;
                    }
                    //}
                }
                catch (Exception e)
                {
                    s_log.Log(Level.SEVERE, "(DocType) [" + trx + "]", e);
                    next = -2;
                    if (trx != null)
                    {
                        if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                        {
                            trx.Rollback();
                        }
                    }
                }
                finally
                {
                    if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                    {
                        trx.Close();
                    }
                }


                //	Error
                if (next < 0)
                    return null;

                //	create DocumentNo
                StringBuilder doc = new StringBuilder();
                if (prefix != null && prefix.Length > 0)
                    doc.Append(prefix);
                doc.Append(next);
                if (suffix != null && suffix.Length > 0)
                    doc.Append(suffix);
                String documentNo = doc.ToString();
                //_log.Finer(documentNo + " (" + incrementNo + ")" + " - C_DocType_ID=" + C_DocType_ID + " [" + trxName + "]");
                return documentNo;
            }
            return null;
        }

        /// <summary>
        /// Get Document No from Table (when the document doesn't have Document Type)
        /// </summary>
        /// <param name="TableName">Table Name</param>
        /// <param name="trxName">Transaction Name</param>
        /// <param name="ctx">Context</param>
        /// <param name="po">object of PO class</param>
        /// <returns>document no or null</returns>
        public static String GetDocumentNo(String TableName, Trx trxName, Ctx ctx, PO po)
        {
            if (TableName == null || TableName.Length == 0)
            {
                s_log.Severe("TableName missing");
                return null;
            }

            MSequence seq = MSequence.Get(ctx, TableName, trxName, false);
            if (seq == null || seq.Get_ID() == 0)
            {
                if (!MSequence.CreateTableSequence(ctx, TableName, trxName, false))
                {
                    s_log.Severe("Could not create table sequence");
                    return null;
                }

                seq = MSequence.Get(ctx, TableName, trxName, false);
                if (seq == null || seq.Get_ID() == 0)
                {
                    s_log.Warning("No Sequence for Table - " + TableName);
                    return null;
                }
            }
            return GetDocumentNoFromSeq(seq, trxName, po);
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public static String GetDocumentNo(int C_DocType_ID, Trx trxName,Ctx ctx)
        //{
        //    if (C_DocType_ID == 0)
        //    {
        //        _log.Severe("C_DocType_ID=0");
        //        return null;
        //    }
        //    MDocType dt = MDocType.Get(ctx, C_DocType_ID);	//	wrong for SERVER, but r/o
        //    if (dt != null && !dt.IsDocNoControlled())
        //    {
        //        _log.Finer("DocType_ID=" + C_DocType_ID + " Not DocNo controlled");
        //        return null;
        //    }
        //    if (dt == null || dt.GetDocNoSequence_ID() == 0)
        //    {
        //        _log.Warning("No Sequence for DocType - " + dt);
        //        return null;
        //    }

        //    //	Check ViennaSys
        //    Boolean viennaSys = false; // Ini.IsPropertyBool(Ini._ViennaSYS);
        //    //if (CLogMgt.isLevel(LOGLEVEL))
        //    //_log.Log(LOGLEVEL, "DocType_ID=" + C_DocType_ID + " [" + trxName + "]");
        //    String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, Prefix, Suffix, AD_Client_ID, AD_Sequence_ID "
        //        + "FROM AD_Sequence "
        //        + "WHERE AD_Sequence_ID=" + dt.GetDocNoSequence_ID()
        //        + " AND IsActive='Y' AND IsTableID='N' AND IsAutoSequence='Y' ";
        //    //	+ " FOR UPDATE";
        //    //
        //    int incrementNo = 0;
        //    int next = -1;
        //    String prefix = "";
        //    String suffix = "";
        //    try
        //    {
        //        OracleCommandBuilder ocb = null;
        //        MySqlCommandBuilder mscb = null;
        //        SqlCommandBuilder scb = null;
        //        NpgsqlCommandBuilder ncb = null;

        //        IDataAdapter dap = null;


        //        if (DatabaseType.IsOracle)
        //        {
        //            dap = GetDataAdapter(selectSQL);
        //            ocb = new OracleCommandBuilder((OracleDataAdapter)dap);
        //        }
        //        else if (DatabaseType.IsPostgre)
        //        {
        //            string strDB = vConn.Db_name;//            Ini.GetProperty(GlobalVariable.DBNAME_NODE);
        //            string finalSQL = "set search_path to " + strDB + ", public;" + selectSQL;
        //            dap = GetDataAdapter(finalSQL);
        //            ncb = new NpgsqlCommandBuilder((NpgsqlDataAdapter)dap);
        //        }
        //        else if (DatabaseType.IsMSSql)
        //        {
        //            dap = GetDataAdapter(selectSQL);
        //            scb = new SqlCommandBuilder((SqlDataAdapter)dap);
        //        }
        //        else if (DatabaseType.IsMySql)
        //        {
        //            dap = GetDataAdapter(selectSQL);
        //            mscb = new MySqlCommandBuilder((MySqlDataAdapter)dap);
        //        }
        //        DataSet ds = new DataSet();
        //        dap.Fill(ds);

        //        // _log.Fine("AC=" + conn.getAutoCommit() + " -Iso=" + conn.getTransactionIsolation() 
        //        //	+ " - Type=" + pstmt.getResultSetType() + " - Concur=" + pstmt.getResultSetConcurrency());

        //        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());
        //            prefix = ds.Tables[0].Rows[0]["Prefix"].ToString();
        //            suffix = ds.Tables[0].Rows[0]["Suffix"].ToString();
        //            int AD_Client_ID = int.Parse(ds.Tables[0].Rows[0]["AD_Client_ID"].ToString());
        //            if (viennaSys && AD_Client_ID > 11)
        //                viennaSys = false;
        //            //	AD_Sequence_ID = dr.getInt(7);
        //            if (viennaSys)
        //            {
        //                next = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
        //                ds.Tables[0].Rows[0]["CurrentNextSys"] = next + incrementNo;
        //            }
        //            else
        //            {
        //                next = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
        //                ds.Tables[0].Rows[0]["CurrentNext"] = next + incrementNo;
        //            }
        //            dap.Update(ds);
        //        }
        //        else
        //        {
        //            _log.Warning("(DocType)- no record found - " + dt);
        //            next = -2;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _log.Log(Level.SEVERE, "(DocType) [" + trxName + "]", e);
        //        next = -2;
        //    }
        //    //	Error
        //    if (next < 0)
        //        return null;

        //    //	create DocumentNo
        //    StringBuilder doc = new StringBuilder();
        //    if (prefix != null && prefix.Length > 0)
        //        doc.Append(prefix);
        //    doc.Append(next);
        //    if (suffix != null && suffix.Length > 0)
        //        doc.Append(suffix);
        //    String documentNo = doc.ToString();
        //    _log.Finer(documentNo + " (" + incrementNo + ")" + " - C_DocType_ID=" + C_DocType_ID + " [" + trxName + "]");
        //    return documentNo;
        //}


        /// <summary>
        /// Create Table ID Sequence 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="TableName">table name</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if created</returns>
        public static Boolean CreateTableSequence(Ctx ctx, String TableName, Trx trxName)
        {
            if (MSysConfig.IsNativeSequence(false))
            {
                int nextid = DB.GetSQLValue(trxName, "SELECT CurrentNext FROM AD_Sequence WHERE Name='" + TableName + "' AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y'");

                if (nextid == -1)
                {
                    CreateSequence(ctx, TableName, trxName);
                    nextid = INIT_NO;
                }

                // Creeate sequence in respective DB.
                if (!VConnection.Get().GetDatabase().CreateSequence(TableName, 1, INIT_NO, int.MaxValue, nextid, trxName))
                    return false;

                return true;
            }

            return CreateSequence(ctx, TableName, trxName);
        }	//	createTableSequence

        private static bool CreateSequence(Ctx ctx, string TableName, Trx trxName)
        {
            MSequence seq = new MSequence(ctx, 0, trxName);
            seq.SetClientOrg(0, 0);
            seq.SetName(TableName);
            seq.SetDescription("Table " + TableName);
            seq.SetIsTableID(true);
            return seq.Save();
        }

        /// <summary>
        /// Create Table ID Sequence 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="TableName">table name</param>
        /// <param name="trxName">transaction</param>
        /// <param name="tableID">Used for Record ID</param>
        /// <returns>true if created</returns>
        public static Boolean CreateTableSequence(Ctx ctx, String TableName, Trx trxName, Boolean tableID)
        {
            //MSysConfig.getBooleanValue(MSysConfig.SYSTEM_NATIVE_SEQUENCE, false);
            if (tableID)
            {
                return MSequence.CreateTableSequence(ctx, TableName, trxName);
            }
            else
            {
                MSequence seq = new MSequence(ctx, 0, trxName);
                seq.SetClientOrg(ctx.GetAD_Client_ID(), 0);
                seq.SetName(PREFIX_DOCSEQ + TableName);
                seq.SetDescription("DocumentNo/Value for Table " + TableName);
                seq.SetIsTableID(tableID);
                return seq.Save();
            }
        }

        //public static Boolean CreateTableSequence(Ctx ctx, String TableName, Trx trxName, MTable table)
        //{
        //    if (!table.IsView() && MSysConfig.IsNativeSequence(false))
        //    {
        //        int nextid = DB.GetSQLValue(trxName, "SELECT CurrentNext FROM AD_Sequence WHERE Name='" + TableName + "' AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y'");

        //        if (nextid == -1)
        //        {
        //            MSequence.CreateTableSequence(ctx, TableName, trxName, true);
        //            nextid = INIT_NO;
        //        }

        //        // Creeate sequence in respective DB.
        //        if (!VConnection.Get().GetDatabase().CreateSequence(TableName, 1, INIT_NO, int.MaxValue, nextid, trxName))
        //            return false;

        //        return true;
        //    }
        //    else if (!table.IsView()) {
        //      return  MSequence.CreateTableSequence(ctx, TableName, trxName, true);
        //    }
        //    return true;
        //}

        /// <summary>
        /// Delete Table ID Sequence
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="TableName">table name</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if </returns>
        public static Boolean DeleteTableSequence(Ctx ctx, String TableName, Trx trxName)
        {
            MSequence seq = Get(ctx, TableName, trxName);
            return seq.Delete(true, trxName);
        }	//	deleteTableSequence



        /// <summary>
        /// Get Table Sequence
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="tableName">table name</param>
        /// <param name="trxName"></param>
        /// <returns>Sequence</returns>
        public static MSequence Get(Ctx ctx, String tableName, Trx trxName)
        {
            String sql = "SELECT * FROM AD_Sequence "
                + "WHERE UPPER(Name)='" + tableName.ToUpper() + "'"
                + " AND IsTableID='Y'";
            MSequence retValue = null;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                DataRow dr = null;
                int totalCount = dt.Rows.Count;
                for (int i = 0; i < totalCount; i++)
                {
                    dr = dt.Rows[i];
                    retValue = new MSequence(ctx, dr, null);
                    if (i > 0)
                    {
                        s_log.Log(Level.SEVERE, "More then one sequence for " + tableName);
                    }
                }
                dr = null;

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                s_log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }


            return retValue;
        }	//	get

        /// <summary>
        /// Get Sequence for Table
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="tableName">Table Name</param>
        /// <param name="trxName">Transaction Name</param>
        /// <param name="tableID">Used for Record ID</param>
        /// <returns></returns>
        public static MSequence Get(Ctx ctx, String tableName, Trx trxName, Boolean tableID)
        {
            if (!tableID)
            {
                tableName = PREFIX_DOCSEQ + tableName;
            }

            String sql = "SELECT * FROM AD_Sequence "
                + "WHERE UPPER(Name)='" + tableName.ToUpper() + "'"
                + " AND IsTableID=@IsTableID";
            if (!tableID)
                sql = sql + " AND AD_Client_ID=@AD_Client_ID";
            MSequence retValue = null;
            DataTable dt = null;
            IDataReader idr = null;
            SqlParameter[] param = new SqlParameter[2];
            param[0] = new SqlParameter("@IsTableID", (tableID ? "Y" : "N"));
            if (!tableID)
            {
                param[1] = new SqlParameter("@AD_Client_ID", ctx.GetAD_Client_ID());
            }
            try
            {
                idr = DB.ExecuteReader(sql, param, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                DataRow dr = null;

                int totalCount = dt.Rows.Count;
                for (int i = 0; i < totalCount; i++)
                {
                    dr = dt.Rows[i];
                    retValue = new MSequence(ctx, dr, trxName);
                    if (i > 0)
                    {
                        s_log.Log(Level.SEVERE, "More then one sequence for " + tableName);
                    }
                }
                dr = null;
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                s_log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            return retValue;
        }

        /// <summary>
        /// Check/Initialize Client DocumentNo/Value Sequences 	
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="trxName"></param>
        /// <returns>true if no error</returns>
        public static Boolean CheckClientSequences(Ctx ctx, int AD_Client_ID, Trx trxName)
        {
            String sql = "SELECT TableName "
                + "FROM AD_Table t "
                + "WHERE IsActive='Y' AND IsView='N'"
                //	Get all Tables with DocumentNo or Value
                + " AND AD_Table_ID IN "
                    + "(SELECT AD_Table_ID FROM AD_Column "
                    + "WHERE ColumnName = 'DocumentNo' OR ColumnName = 'Value')"
                //	Ability to run multiple times
                + " AND 'DocumentNo_' || TableName NOT IN "
                    + "(SELECT Name FROM AD_Sequence s "
                    + "WHERE s.AD_Client_ID=@AD_Client_ID)";
            int counter = 0;
            Boolean success = true;
            //
            DataTable dt = null;

            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@AD_Client_ID", AD_Client_ID);
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, param);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                int totCount = dt.Rows.Count;

                for (int i = 0; i < totCount; i++)
                {
                    DataRow dr = dt.Rows[i];

                    String tableName = dr["TableName"].ToString();
                    s_log.Fine("Add: " + tableName);
                    MSequence seq = new MSequence(ctx, AD_Client_ID, tableName, trxName);
                    if (seq.Save())
                    {
                        counter++;
                    }
                    else
                    {
                        s_log.Severe("Not created - AD_Client_ID=" + AD_Client_ID
                            + " - " + tableName);
                        success = false;
                    }

                }


            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                s_log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            s_log.Info("AD_Client_ID=" + AD_Client_ID
                + " - created #" + counter
                + " - success=" + success);
            return success;
        }	//	checkClientSequences

        /// <summary>
        /// Get next DocumentNo
        /// </summary>
        /// <returns>document no</returns>
        public String GetDocumentNo()
        {
            //	create DocumentNo
            StringBuilder doc = new StringBuilder();
            String prefix = GetPrefix();
            if (prefix != null && prefix.Length > 0)
                doc.Append(prefix);
            doc.Append(GetNextID());
            String suffix = GetSuffix();
            if (suffix != null && suffix.Length > 0)
                doc.Append(suffix);
            return doc.ToString();
        }	//	getDocumentNo

        /// <summary>
        /// 	Validate Table Sequence Values
        /// </summary>
        /// <returns>true if updated</returns>
        public bool ValidateTableIDValue()
        {
            if (!IsTableID())
            {
                return false;
            }
            String tableName = GetName();
            int AD_Column_ID = DataBase.DB.GetSQLValue(null, "SELECT MAX(c.AD_Column_ID) "
                + "FROM AD_Table t"
                + " INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID) "
                + "WHERE t.TableName='" + tableName + "'"
                + " AND c.ColumnName='" + tableName + "_ID'");
            if (AD_Column_ID <= 0)
            {
                return false;
            }
            //
            MSystem system = MSystem.Get(GetCtx());
            int IDRangeEnd = 0;
            //if (system.GetIDRangeEnd() != null)
            {
                IDRangeEnd = Utility.Util.GetValueOfInt(system.GetIDRangeEnd());//.intValue();
            }
            bool change = false;
            String info = null;

            //	Current Next
            String sql = "SELECT MAX(" + tableName + "_ID) FROM " + tableName;
            if (IDRangeEnd > 0)
            {
                sql += " WHERE " + tableName + "_ID < " + IDRangeEnd;
            }
            int maxTableID = DataBase.DB.GetSQLValue(null, sql);
            if (maxTableID < INIT_NO)
            {
                maxTableID = INIT_NO - 1;
            }
            maxTableID++;		//	Next
            if (GetCurrentNext() < maxTableID)
            {
                SetCurrentNext(maxTableID);
                info = "CurrentNext=" + maxTableID;
                change = true;
            }

            //	Get Max System_ID used in Table
            sql = "SELECT MAX(" + tableName + "_ID) FROM " + tableName
                + " WHERE " + tableName + "_ID < " + INIT_NO;
            int maxTableSysID = DataBase.DB.GetSQLValue(null, sql);
            if (maxTableSysID <= 0)
            {
                maxTableSysID = INIT_SYS_NO - 1;
            }
            maxTableSysID++;	//	Next
            if (GetCurrentNextSys() < maxTableSysID)
            {
                SetCurrentNextSys(maxTableSysID);
                if (info == null)
                {
                    info = "CurrentNextSys=" + maxTableSysID;
                }
                else
                {
                    info += " - CurrentNextSys=" + maxTableSysID;
                }
                change = true;
            }
            if (info != null)
            {
                log.Config(GetName() + " - " + info);
            }
            return change;
        }	//	validate

    }

}
