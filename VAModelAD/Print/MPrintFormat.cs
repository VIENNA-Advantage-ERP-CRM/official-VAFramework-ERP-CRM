/********************************************************
 * Module Name    : Report
 * Purpose        : Launch Report
 * Author         : Jagmohan Bhatt
 * Date           : 02-June-2009
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using System.Data;

using System.Data.SqlClient;
using VAdvantage.Common;
using VAdvantage.Print;
using VAdvantage.Login;
using VAdvantage.Logging;

namespace VAdvantage.Print
{
    public class MPrintFormat : X_AD_PrintFormat
    {
        /** Cached Formats						*/
        static private CCache<int, MPrintFormat> s_formats = new CCache<int, MPrintFormat>("AD_PrintFormat", 30);
        private static VLogger s_log = VLogger.GetVLogger(typeof(MPrintFormat).FullName);
        public Ctx _ctx = null;

        private bool isGridReport;
        private int pageNo;
        private int totalPages;
       

        public new Ctx GetCtx()
        {
            return _ctx;
        }

        /**	Language of Report				*/
        private Language _language;

        /// <summary>
        /// Public Constructor.
        /// Use static get methods
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_PrintFormat_ID">AD_PrintFormat_ID</param>
        /// <param name="trxName">transaction</param>
        public MPrintFormat(Ctx ctx, int AD_PrintFormat_ID, Trx trxName)
            : base(ctx, AD_PrintFormat_ID, trxName)
        {
            _ctx = ctx;
            //	Language=[Deutsch,Locale=de_DE,AD_Language=en_US,DatePattern=DD.MM.YYYY,DecimalPoint=false]
            _language = Env.GetLanguage(ctx);
            // Set Report Language -VIS0228
            if (!string.IsNullOrEmpty(ctx.GetContext("Report_Lang")))
            {
                _language = Language.GetLanguage(ctx.GetContext("Report_Lang"));
            }
            if (AD_PrintFormat_ID == 0)
            {
                SetStandardHeaderFooter(true);
                SetIsTableBased(true);
                SetIsForm(false);
                SetIsDefault(false);
            }
            _items = GetItems();
        }	//	MPrintFormat

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">Datarow</param>
        /// <param name="trxName">transaction</param>
        public MPrintFormat(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
            _ctx = ctx;
            _language = Env.GetLanguage(ctx);
            if (!string.IsNullOrEmpty(ctx.GetContext("Report_Lang")))
            {
                _language = Language.GetLanguage(ctx.GetContext("Report_Lang"));
            }
            _items = GetItems();
        }	//	MPrintFormat

        /// <summary>
        /// Set Standard Header
        /// </summary>
        /// <param name="standardHeaderFooter">true if std header</param>
        public void SetStandardHeaderFooter(bool standardHeaderFooter)
        {
            base.SetIsStandardHeaderFooter(standardHeaderFooter);
            if (standardHeaderFooter)
            {
                SetFooterMargin(0);
                SetHeaderMargin(0);
            }
        }	//	setSatndardHeaderFooter

        public static MPrintFormat Copy(Ctx ctx, int from_AD_PrintFormat_ID,
            int to_AD_PrintFormat_ID, int to_Client_ID)
        {
            //_ctx = ctx;
            if (from_AD_PrintFormat_ID == 0)
                throw new ArgumentException("From_AD_PrintFormat_ID is 0");
            //
            MPrintFormat from = new MPrintFormat(ctx, from_AD_PrintFormat_ID, null);
            MPrintFormat to = new MPrintFormat(ctx, to_AD_PrintFormat_ID, null);		//	could be 0
            MPrintFormat.CopyValues(from, to);
            //	New
            if (to_AD_PrintFormat_ID == 0)
            {
                if (to_Client_ID < 0)
                    to_Client_ID = ctx.GetAD_Client_ID();
                to.SetClientOrg(to_Client_ID, 0);
            }
            //	Set Name - Remove TEMPLATE - add copy

            to.SetName(Utility.Util.Replace(to.GetName(), "TEMPLATE", to_Client_ID.ToString()));
            to.SetName(to.GetName()
                + " " + Msg.GetMsg(Env.GetContext(), "Copy", true)
                + " " + to.HashCode());		//	unique name
            //
            to.Save();

            //	Copy Items
            to.SetItems(CopyItems(from, to));
            return to;
        }	//	copyToClient

        /// <summary>
        /// Copy existing Definition To Client
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="from_AD_PrintFormat_ID"></param>
        /// <param name="to_AD_PrintFormat_ID"></param>
        /// <returns>return print format</returns>
        public static MPrintFormat Copy(Ctx ctx, int from_AD_PrintFormat_ID, int to_AD_PrintFormat_ID)
        {
            return Copy(ctx, from_AD_PrintFormat_ID, to_AD_PrintFormat_ID, -1);
        }


        /// <summary>
        /// Get Format
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_PrintFormat_ID">id</param>
        /// <param name="readFromDisk">refresh from disk</param>
        /// <param name="isParent">wether tab is parent or a child</param>
        /// <returns>Format</returns>
        static public MPrintFormat Get(Ctx ctx, int AD_PrintFormat_ID, bool readFromDisk)
        {
            int key = AD_PrintFormat_ID;
            MPrintFormat pf = null;
            if (!readFromDisk)
                pf = (MPrintFormat)s_formats[key];

            if (pf != null)
            {
                if (string.IsNullOrEmpty(pf.GetCtx().GetContext("#TimezoneOffset")))
                    pf.GetCtx().SetContext("#TimezoneOffset", ctx.GetContext("#TimezoneOffset"));
            }

            if (pf == null)
            {
                pf = new MPrintFormat(ctx, AD_PrintFormat_ID, null);
                pf.GetCtx().SetContext("#TimezoneOffset", ctx.GetContext("#TimezoneOffset"));
                s_formats.Add(key, pf);
            }

            return pf;
        }	//	get


        /// <summary>
        /// Get (default) Printformat for Report View or Table
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_ReportView_ID">id or 0</param>
        /// <param name="AD_Table_ID">id or 0</param>
        /// <returns>first print format found or null</returns>
        static public MPrintFormat Get(Ctx ctx, int AD_ReportView_ID, int AD_Table_ID)
        {
            MPrintFormat retValue = null;

            String sql = "SELECT * FROM AD_PrintFormat WHERE ";
            if (AD_ReportView_ID > 0)
                sql += "AD_ReportView_ID=@val1";
            else
                sql += "AD_Table_ID=@val1";
            sql += " ORDER BY IsDefault DESC";

            SqlParameter[] param = new SqlParameter[1];
            try
            {
                param[0] = new SqlParameter("@val1", AD_ReportView_ID > 0 ? AD_ReportView_ID : AD_Table_ID);
                DataSet ds = SqlExec.ExecuteQuery.ExecuteDataset(sql, param);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    retValue = new MPrintFormat(ctx, dr, null);
                }
            }
            catch (Exception e)
            {
               s_log.Severe(e.ToString());
                //log entry if any
            }

            return retValue;
        }

        /// <summary>
        /// Delete Format from Cache
        /// </summary>
        /// <param name="AD_PrintFormat_ID">id</param>
        static public void DeleteFromCache(int AD_PrintFormat_ID)
        {
            int key = AD_PrintFormat_ID;
            s_formats.Add(key, null);
        }	//	deleteFromCache

        /** Items							*/
        private MPrintFormatItem[] _items = null;

        /** Translation View Language		*/
        private String _translationViewLanguage = null;


        private MPrintTableFormat _tFormat;


        /// <summary>
        /// Set Items
        /// </summary>
        /// <param name="items">items</param>
        private void SetItems(MPrintFormatItem[] items)
        {
            if (items != null)
                _items = items;
        }	//	setItems

        /// <summary>
        /// Copy Items
        /// </summary>
        /// <param name="fromFormat">from print format</param>
        /// <param name="toFormat">to print format (client, id)</param>
        /// <returns>items</returns>
        static private MPrintFormatItem[] CopyItems(MPrintFormat fromFormat, MPrintFormat toFormat)
        {
            List<MPrintFormatItem> list = new List<MPrintFormatItem>();

            MPrintFormatItem[] items = fromFormat.GetItems();
            for (int i = 0; i < items.Length; i++)
            {
                MPrintFormatItem pfi = items[i].CopyToClient(toFormat.GetAD_Client_ID(), toFormat.Get_ID());
                if (pfi != null)
                    list.Add(pfi);
            }
            //
            MPrintFormatItem[] retValue = new MPrintFormatItem[list.Count];
            retValue = list.ToArray();
            CopyTranslationItems(items, retValue);	//	JTP fix
            return retValue;
        }	//	copyItems


        /// <summary>
        /// Get Print Format Item
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>Format Item</returns>
        public MPrintFormatItem GetItem(int index)
        {
            if (index < 0 || index >= _items.Length)
                throw new Exception("Index=" + index + " - Length=" + _items.Length);
            return _items[index];
        }	//	getItem


        /// <summary>
        /// Get active Items
        /// </summary>
        /// <returns>items</returns>
        private MPrintFormatItem[] GetItems()
        {
            List<MPrintFormatItem> list = new List<MPrintFormatItem>();
            String sql = "SELECT * FROM AD_PrintFormatItem pfi "
                + "WHERE pfi.AD_PrintFormat_ID=@AD_PrintFormat_ID AND pfi.IsActive='Y'"
                //	Display restrictions - Passwords, etc.
                + " AND NOT EXISTS (SELECT * FROM AD_Field f "
                    + "WHERE pfi.AD_Column_ID=f.AD_Column_ID"
                    + " AND (f.IsEncrypted='Y' OR f.ObscureType IS NOT NULL))"
                + "ORDER BY SeqNo";
            MRole role = MRole.GetDefault(GetCtx(), false);
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@AD_PrintFormat_ID", Get_ID());
                DataSet ds = SqlExec.ExecuteQuery.ExecuteDataset(sql, param);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MPrintFormatItem pfi = new MPrintFormatItem(Env.GetContext(), dr, Get_TrxName());
                    if (role.IsColumnAccess(GetAD_Table_ID(), pfi.GetAD_Column_ID(), true))
                        list.Add(pfi);
                }
            }
            catch (Exception e)
            {
                log.Severe(e.ToString());
                //log entry, if any
            }
            //
            MPrintFormatItem[] retValue = new MPrintFormatItem[list.Count];
            retValue = list.ToArray();
            return retValue;
        }	//	getItems

        /// <summary>
        /// Copy translation records (from - to)
        /// </summary>
        /// <param name="fromItems">from items</param>
        /// <param name="toItems">to items</param>
        static private void CopyTranslationItems(MPrintFormatItem[] fromItems, MPrintFormatItem[] toItems)
        {
            if (fromItems == null || toItems == null)
                return;		//	should not happen

            int counter = 0;
            for (int i = 0; i < fromItems.Length; i++)
            {
                int fromID = fromItems[i].GetAD_PrintFormatItem_ID();
                int toID = toItems[i].GetAD_PrintFormatItem_ID();

                StringBuilder sql = new StringBuilder("UPDATE AD_PrintFormatItem_Trl ne ")
                .Append("SET ")
.Append("PrintName=(SELECT PrintName FROM AD_PrintFormatItem_Trl ol WHERE ol.AD_Language=ne.AD_Language AND AD_PrintFormatItem_ID =").Append(fromID).Append("),")
.Append("PrintNameSuffix=(SELECT PrintNameSuffix FROM AD_PrintFormatItem_Trl ol WHERE ol.AD_Language=ne.AD_Language AND AD_PrintFormatItem_ID =").Append(toID).Append("),")
.Append("IsTranslated=(SELECT IsTranslated FROM AD_PrintFormatItem_Trl ol WHERE ol.AD_Language=ne.AD_Language AND AD_PrintFormatItem_ID=").Append(fromID)
.Append(") WHERE AD_PrintFormatItem_ID=").Append(toID).Append(" AND EXISTS").Append("(")
                .Append("SELECT AD_PrintFormatItem_ID").Append(" FROM AD_PrintFormatItem_trl ol WHERE ol.AD_Language=ne.AD_Language AND AD_PrintFormatItem_ID=").Append(fromID).Append(")");// = 5087); 



                //	Set
                //.Append("SET (PrintName, PrintNameSuffix, IsTranslated) = ")
                //.Append("(")
                //.Append("SELECT PrintName, PrintNameSuffix, IsTranslated ")
                //.Append("FROM AD_PrintFormatItem_Trl old ")
                //.Append("WHERE old.AD_Language=new.AD_Language")
                //.Append(" AND AD_PrintFormatItem_ID =").Append(fromID)
                //.Append(") ")
                ////	WHERE
                //.Append("WHERE  AD_PrintFormatItem_ID=").Append(toID)
                //.Append(" AND EXISTS (SELECT AD_PrintFormatItem_ID ")
                //    .Append(" FROM AD_PrintFormatItem_trl old")
                //    .Append(" WHERE old.AD_Language=new.AD_Language")
                //    .Append(" AND AD_PrintFormatItem_ID =").Append(fromID)
                //    .Append(") ");
                int no = DataBase.DB.ExecuteQuery(sql.ToString(), null);
                if (no == 0)	//	if first has no translation, the rest does neither
                    break;
                counter += no;
            }	//	for
        }	//	copyTranslationItems

        /// <summary>
        /// Copy existing Definition To Client
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_PrintFormat_ID">format</param>
        /// <param name="to_Client_ID">client</param>
        /// <returns>format</returns>
        public static MPrintFormat CopyToClient(Ctx ctx, int AD_PrintFormat_ID, int to_Client_ID)
        {
            return Copy(ctx, AD_PrintFormat_ID, 0, to_Client_ID);
        }	//	copy


        /// <summary>
        /// Create Items.
        /// Using the display order of Fields in some Tab
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="format">format</param>
        /// <returns>items</returns>
        /// <summary>
        /// Create Items.
        /// Using the display order of Fields in some Tab
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="format">format</param>
        /// <returns>items</returns>
        static private MPrintFormatItem[] CreateItems(Ctx ctx, MPrintFormat format, int AD_tab_ID,bool isMRSeq=false)
        {
            List<MPrintFormatItem> list = new List<MPrintFormatItem>();
            bool runOldCode = true;
            //	Get Column List from Tab
            String sql = "SELECT AD_Column_ID " //, Name, IsDisplayed, SeqNo
                + "FROM AD_Field "
                + "WHERE ";
            if (AD_tab_ID == 0)
            {
                sql += " AD_Tab_ID=(SELECT MIN(AD_Tab_ID) FROM AD_Tab WHERE AD_Table_ID=@AD_Table_ID)";
            }
            else
            {
                sql += " AD_Tab_ID=@AD_Table_ID";
            }
            //added check on 03/12/14 - "AND IsDisplayed='Y' AND IsActive='Y'"
            sql += " AND IsEncrypted='N' AND (IsDisplayed='Y' OR MRISDISPLAYED='Y') AND IsActive='Y' AND ObscureType IS NULL ";
            //Lakhwinder
            if (isMRSeq)
            {
                try
                {
                    runOldCode = false;
                    sql = "SELECT AD_Column_ID,Ad_Field_ID  FROM AD_Field WHERE ";
                    if (AD_tab_ID == 0)
                    {
                        sql += " AD_Tab_ID=(SELECT MIN(AD_Tab_ID) FROM AD_Tab WHERE AD_Table_ID=@AD_Table_ID)";
                    }
                    else
                    {
                        sql += " AD_Tab_ID=@AD_Table_ID";
                    }
                    //added check on 03/12/14 - "AND IsDisplayed='Y' AND IsActive='Y'"
                    sql += " AND IsEncrypted='N' AND (IsDisplayed='Y' OR MRISDISPLAYED='Y') AND IsActive='Y' AND ObscureType IS NULL ";
                    bool isMESeqDefined = false;
                    //if multirow sequence is set on tab then MRIsDisplayed Column does have non null value
                    int count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM AD_Field WHERE AD_Tab_ID ="+AD_tab_ID+" AND MRIsDisplayed IS NOT NULL"));
                    if (count > 0)//tab has defined multirow sequence
                    {
                        isMESeqDefined = true;
                        sql += " ORDER BY MRSeqNo ";
                    }
                    else//tab has undefined multirow sequence
                    {
                        sql += " ORDER BY NVL(MRSeqNo,999999),Name ";
                       
                    }

                    SqlParameter[] param = new SqlParameter[1];
                    if (AD_tab_ID == 0)
                    {
                        param[0] = new SqlParameter("@AD_Table_ID", format.GetAD_Table_ID());
                    }
                    else
                    {
                        param[0] = new SqlParameter("@AD_Table_ID", AD_tab_ID);
                    }
                    DataSet ds = DB.ExecuteDataset(sql, param);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            MPrintFormatItem pfi = MPrintFormatItem.CreateFromColumn(format, Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Column_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Field_ID"]), i + 1, isMESeqDefined);
                            if (pfi != null)
                            {
                                list.Add(pfi);
                            }
                        }
                    }
                    

                }
                catch
                {
                    runOldCode = true;
                    sql += " ORDER BY  COALESCE(MRISDISPLAYED,IsDisplayed) DESC, SortNo, MRSeqNo, COALESCE(IsDisplayed,'N'), SeqNo";
                }
                
            }
            else
            {
                sql += " ORDER BY  COALESCE(MRISDISPLAYED,IsDisplayed) DESC, SortNo, MRSeqNo, COALESCE(IsDisplayed,'N'), SeqNo";
            }
            // + "ORDER BY COALESCE(IsDisplayed,'N') DESC, SortNo, COALESCE(MRSeqNo, SeqNo), Name";
            if (runOldCode)
            {
                IDataReader dr = null;
                try
                {
                    SqlParameter[] param = new SqlParameter[1];
                    if (AD_tab_ID == 0)
                    {
                        param[0] = new SqlParameter("@AD_Table_ID", format.GetAD_Table_ID());
                    }
                    else
                    {
                        param[0] = new SqlParameter("@AD_Table_ID", AD_tab_ID);
                    }
                    dr = SqlExec.ExecuteQuery.ExecuteReader(sql, param);
                    int seqNo = 1;
                    while (dr.Read())
                    {
                        MPrintFormatItem pfi = MPrintFormatItem.CreateFromColumn(format, Utility.Util.GetValueOfInt(dr[0].ToString()), seqNo++);
                        if (pfi != null)
                        {
                            list.Add(pfi);
                        }
                    }
                    dr.Close();
                }
                catch (Exception e)
                {
                    if (dr != null)
                    {
                        dr.Close();
                    }
                    s_log.Severe(e.ToString());
                    //log, if any
                }
            }
            //	No Tab found for Table
            if (list.Count == 0)
            {
                sql = "SELECT AD_Column_ID "
                    + "FROM AD_Column "
                    + "WHERE AD_Table_ID='" + format.GetAD_Table_ID() + "' "
                    + "ORDER BY IsIdentifier DESC, SeqNo, Name";
                IDataReader idr = null;
                try
                {
                    idr = SqlExec.ExecuteQuery.ExecuteReader(sql);
                    // DataTable dt = new DataTable();
                    //dt.Load(idr);

                    int seqNo = 1;
                    while (idr.Read())
                    {
                        MPrintFormatItem pfi = MPrintFormatItem.CreateFromColumn(format, Utility.Util.GetValueOfInt(idr[0].ToString()), seqNo++);
                        if (pfi != null)
                        {
                            list.Add(pfi);
                        }
                    }
                    idr.Close();
                }
                catch (Exception e)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    s_log.Severe(e.ToString());
                    //log, if any
                }
            }

            //
            MPrintFormatItem[] retValue = new MPrintFormatItem[list.Count];
            retValue = list.ToArray();
            return retValue;
        }	//	createItems


        /// <summary>
        /// Create MPrintFormat for Table
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Table_ID">AD_Table_ID</param>
        /// <returns>print format</returns>
        static public MPrintFormat CreateFromTable(Ctx ctx, int AD_Table_ID, object AD_Tab_ID,bool isMRSeq=false)
        {
            
            return CreateFromTable(ctx, AD_Table_ID, 0, Convert.ToInt32(AD_Tab_ID),isMRSeq);
        }	//	createFromTable

        /// <summary>
        /// Create MPrintFormat for Table
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Table_ID">AD_Table_ID</param>
        /// <returns>print format</returns>
        static public MPrintFormat CreateFromTable(Ctx ctx, int AD_Table_ID,bool isMRSeq=false)
        {
            return CreateFromTable(ctx, AD_Table_ID, 0, isMRSeq);
        }	//	createFromTable

        //private MPrintTableFormat _tFormat;

        ///// <summary>
        ///// Get Table Format
        ///// </summary>
        ///// <returns>Table Format</returns>
        public MPrintTableFormat GetTableFormat()
        {
            if (_tFormat == null)
                _tFormat = MPrintTableFormat.Get(GetCtx(), GetAD_PrintTableFormat_ID(), GetAD_PrintFont_ID());
            return _tFormat;
        }	//	getTableFormat


        /// <summary>
        /// Save Special Data.
        /// To be extended by sub-classes
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="index">index</param>
        /// <returns>SQL code for INSERT VALUES clause</returns>
        new protected String SaveNewSpecial(Object value, int index)
        {
            //	CreateCopy
            //	String colName = p_info.getColumnName(index);
            //	String colClass = p_info.getColumnClass(index).toString();
            //	String colValue = value == null ? "null" : value.getClass().toString();
            //	log.log(Level.SEVERE, "Unknown class for column " + colName + " (" + colClass + ") - Value=" + colValue);
            if (value == null)
                return "NULL";
            return value.ToString();
        }   //  saveNewSpecial

        /// <summary>
        /// Create MPrintFormat for Table
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Table_ID">AD_Table_ID</param>
        /// <param name="AD_PrintFormat_ID">AD_PrintFormat_ID</param>
        /// <returns>print format</returns>
        static public MPrintFormat CreateFromTable(Ctx ctx, int AD_Table_ID, int AD_PrintFormat_ID)
        {
            int AD_Client_ID = ctx.GetAD_Client_ID();

            MPrintFormat pf = new MPrintFormat(ctx, AD_PrintFormat_ID, null);
            pf.SetAD_Table_ID(AD_Table_ID);

            //	Get Info
            String sql = "SELECT TableName,"		//	1
                + " (SELECT COUNT(*) FROM AD_PrintFormat x WHERE x.AD_Table_ID=t.AD_Table_ID AND x.AD_Client_ID=c.AD_Client_ID) AS Count,"
                + " COALESCE (cpc.AD_PrintColor_ID, pc.AD_PrintColor_ID) AS AD_PrintColor_ID,"	//	3
                + " COALESCE (cpf.AD_PrintFont_ID, pf.AD_PrintFont_ID) AS AD_PrintFont_ID,"
                + " COALESCE (cpp.AD_PrintPaper_ID, pp.AD_PrintPaper_ID) AS AD_PrintPaper_ID "
                + "FROM AD_Table t, AD_Client c"
                + " LEFT OUTER JOIN AD_PrintColor cpc ON (cpc.AD_Client_ID=c.AD_Client_ID AND cpc.IsDefault='Y')"
                + " LEFT OUTER JOIN AD_PrintFont cpf ON (cpf.AD_Client_ID=c.AD_Client_ID AND cpf.IsDefault='Y')"
                + " LEFT OUTER JOIN AD_PrintPaper cpp ON (cpp.AD_Client_ID=c.AD_Client_ID AND cpp.IsDefault='Y'),"
                + " AD_PrintColor pc, AD_PrintFont pf, AD_PrintPaper pp "
                + "WHERE t.AD_Table_ID='" + AD_Table_ID + "' AND c.AD_Client_ID='" + AD_Client_ID + "'"		//	#1/2
                + " AND pc.IsDefault='Y' AND pf.IsDefault='Y' AND pp.IsDefault='Y'";

            string AD_Language = Utility.Env.GetAD_Language(ctx);
            // Set Report Language -VIS0228
            if (!string.IsNullOrEmpty(ctx.GetContext("Report_Lang")))
            {
                AD_Language =ctx.GetContext("Report_Lang");
            }

            bool error = true;
            IDataReader dr = null;
           // IDataReader idr = null;
            String s = "";
            int count = 0;
            try
            {
                dr = SqlExec.ExecuteQuery.ExecuteReader(sql);

                while (dr.Read())
                {
                    count = Util.GetValueOfInt(dr[1]);

                    //	Name
                    String TableName = dr[0].ToString();
                    String ColumnName = TableName + "_ID";
                    s = ColumnName;
                    if (!ColumnName.Equals("T_Report_ID"))
                    {
                        s = Msg.Translate(ctx, ColumnName);
                        if (ColumnName.Equals(s)) //	not found
                            s = Msg.Translate(ctx, TableName);
                    }

                    //if (count == 0)
                    //    count = 1;
                    if (count > 0)
                        count += 1;
                    s += "_" + (count + 1);
                    pf.SetName(s);
                    //
                    pf.SetAD_PrintColor_ID(Utility.Util.GetValueOfInt(dr[2].ToString()));
                    pf.SetAD_PrintFont_ID(Utility.Util.GetValueOfInt(dr[3].ToString()));
                    pf.SetAD_PrintPaper_ID(Utility.Util.GetValueOfInt(dr[4].ToString()));
                    //
                    error = false;
                    break;
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                s_log.Severe(e.ToString());
            }
            if (error)
                return null;

            //	Save & complete
            if (!pf.Save())
                return null;
            //	pf.dump();
            pf.SetItems(CreateItems(ctx, pf,0));
            //
            return pf;
        }	//	createFromTable


        /// <summary>
        /// Create MPrintFormat for Table
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Table_ID">AD_Table_ID</param>
        /// <param name="AD_PrintFormat_ID">AD_PrintFormat_ID</param>
        /// <returns>print format</returns>
        static public MPrintFormat CreateFromTable(Ctx ctx, int AD_Table_ID, int AD_PrintFormat_ID, int AD_Tab_ID,bool IsMRSeq=false)
        {
            int AD_Client_ID = ctx.GetAD_Client_ID();

            MPrintFormat pf = new MPrintFormat(ctx, AD_PrintFormat_ID, null);
            pf.SetAD_Table_ID(AD_Table_ID);
            if (AD_Tab_ID > 0)
            {
                pf.SetAD_Tab_ID(AD_Tab_ID);
            }

            //	Get Info
            String sql = "SELECT TableName,"		//	1
                + " (SELECT COUNT(*) FROM AD_PrintFormat x WHERE x.AD_Table_ID=t.AD_Table_ID AND x.AD_Client_ID=c.AD_Client_ID) AS Count,"
                + " COALESCE (cpc.AD_PrintColor_ID, pc.AD_PrintColor_ID) AS AD_PrintColor_ID,"	//	3
                + " COALESCE (cpf.AD_PrintFont_ID, pf.AD_PrintFont_ID) AS AD_PrintFont_ID,"
                + " COALESCE (cpp.AD_PrintPaper_ID, pp.AD_PrintPaper_ID) AS AD_PrintPaper_ID "
                + "FROM AD_Table t, AD_Client c"
                + " LEFT OUTER JOIN AD_PrintColor cpc ON (cpc.AD_Client_ID=c.AD_Client_ID AND cpc.IsDefault='Y')"
                + " LEFT OUTER JOIN AD_PrintFont cpf ON (cpf.AD_Client_ID=c.AD_Client_ID AND cpf.IsDefault='Y')"
                + " LEFT OUTER JOIN AD_PrintPaper cpp ON (cpp.AD_Client_ID=c.AD_Client_ID AND cpp.IsDefault='Y'),"
                + " AD_PrintColor pc, AD_PrintFont pf, AD_PrintPaper pp "
                + "WHERE t.AD_Table_ID='" + AD_Table_ID + "' AND c.AD_Client_ID='" + AD_Client_ID + "'"		//	#1/2
                + " AND pc.IsDefault='Y' AND pf.IsDefault='Y' AND pp.IsDefault='Y'";

            string sql1 = "SELECT ";
            string AD_Language = Utility.Env.GetAD_Language(ctx);
            // Set Report Language -VIS0228
            if (!string.IsNullOrEmpty(ctx.GetContext("Report_Lang")))
            {
                AD_Language = ctx.GetContext("Report_Lang");
            }


                if (AD_Language == null || AD_Language.Length == 0 || Env.IsBaseLanguage(AD_Language, "AD_Element"))
            {
                sql1 = sql1 + " t.Name,  (SELECT COUNT(*)  FROM AD_PrintFormat x  WHERE x.AD_Tab_ID =t.AD_Tab_ID  "
                + "AND x.AD_Client_ID=c.AD_Client_ID  ) AS COUNT FROM AD_Tab t ,AD_Client c "
                + "WHERE t.AD_Tab_ID ='" + AD_Tab_ID + "' AND c.AD_Client_ID='" + AD_Client_ID + "'";
            }
            else
            {
                sql1 = sql1 + " Distinct tt.Name, (SELECT COUNT(*) FROM AD_PrintFormat x WHERE x.AD_Tab_ID =t.AD_Tab_ID  AND x.AD_Client_ID=c.AD_Client_ID ) AS COUNT"
                            + " FROM AD_Client c, AD_Tab t JOIN AD_Tab_Trl tt ON (tt.AD_Tab_ID=t.ad_tab_id)"
                            + " WHERE t.AD_Tab_ID ='" + AD_Tab_ID + "'  AND tt.AD_Language='" + AD_Language + "'"
                            + " AND c.AD_Client_ID='" + AD_Client_ID + "'";
            }


            bool error = true;
            IDataReader dr = null;
            IDataReader idr = null;
            String s = "";
            int count = 0;
            try
            {
                dr = SqlExec.ExecuteQuery.ExecuteReader(sql);
                idr = SqlExec.ExecuteQuery.ExecuteReader(sql1);
                while (idr.Read())
                {
                    s = idr[0].ToString();
                    count = Util.GetValueOfInt(idr[1]);
                }
                idr.Close();

                while (dr.Read())
                {
                    //int count = countrec;
                    if (count == 0 && s == null || s == "")
                    {
                        count = Utility.Util.GetValueOfInt(dr[1].ToString());
                    }

                    //	Name
                    if (s == null || s == "")
                    {
                        String TableName = dr[0].ToString();
                        String ColumnName = TableName + "_ID";
                        s = ColumnName;
                        if (!ColumnName.Equals("T_Report_ID"))
                        {
                            s = Msg.Translate(ctx, ColumnName);
                            if (ColumnName.Equals(s)) //	not found
                                s = Msg.Translate(ctx, TableName);
                        }
                    }

                    if (count > 0)
                        s += "_" + (count + 1);
                    pf.SetName(s);
                    //
                    pf.SetAD_PrintColor_ID(Utility.Util.GetValueOfInt(dr[2].ToString()));
                    pf.SetAD_PrintFont_ID(Utility.Util.GetValueOfInt(dr[3].ToString()));
                    pf.SetAD_PrintPaper_ID(Utility.Util.GetValueOfInt(dr[4].ToString()));
                    //
                    error = false;
                    break;
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                if (dr != null)
                {
                    dr.Close();
                }
                s_log.Severe(e.ToString());
            }
            if (error)
                return null;

            //	Save & complete
            if (!pf.Save())
                return null;
            //	pf.dump();
            pf.SetItems(CreateItems(ctx, pf, AD_Tab_ID,IsMRSeq));
            //
            return pf;
        }	//	createFromTable



        /// <summary>
        /// Create MPrintFormat for ReportView
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_ReportView_ID">AD_ReportView_ID</param>
        /// <param name="ReportName">optional Report Name</param>
        /// <returns>print format</returns>
        static public MPrintFormat CreateFromReportView(Ctx ctx, int AD_ReportView_ID, String ReportName)
        {
            int AD_Client_ID = ctx.GetAD_Client_ID();

            MPrintFormat pf = new MPrintFormat(ctx, 0, null);
            pf.SetAD_ReportView_ID(AD_ReportView_ID);

            //	Get Info
            String sql = "SELECT t.TableName,"
                + " (SELECT COUNT(*) FROM AD_PrintFormat x WHERE x.AD_ReportView_ID=rv.AD_ReportView_ID AND x.AD_Client_ID=c.AD_Client_ID) AS Count,"
                + " COALESCE (cpc.AD_PrintColor_ID, pc.AD_PrintColor_ID) AS AD_PrintColor_ID,"
                + " COALESCE (cpf.AD_PrintFont_ID, pf.AD_PrintFont_ID) AS AD_PrintFont_ID,"
                + " COALESCE (cpp.AD_PrintPaper_ID, pp.AD_PrintPaper_ID) AS AD_PrintPaper_ID,"
                + " t.AD_Table_ID "
                + "FROM AD_ReportView rv"
                + " INNER JOIN AD_Table t ON (rv.AD_Table_ID=t.AD_Table_ID),"
                + " AD_Client c"
                + " LEFT OUTER JOIN AD_PrintColor cpc ON (cpc.AD_Client_ID=c.AD_Client_ID AND cpc.IsDefault='Y')"
                + " LEFT OUTER JOIN AD_PrintFont cpf ON (cpf.AD_Client_ID=c.AD_Client_ID AND cpf.IsDefault='Y')"
                + " LEFT OUTER JOIN AD_PrintPaper cpp ON (cpp.AD_Client_ID=c.AD_Client_ID AND cpp.IsDefault='Y'),"
                + " AD_PrintColor pc, AD_PrintFont pf, AD_PrintPaper pp "
                + "WHERE rv.AD_ReportView_ID='" + AD_ReportView_ID + "' AND c.AD_Client_ID='" + AD_Client_ID + "'"
                + " AND pc.IsDefault='Y' AND pf.IsDefault='Y' AND pp.IsDefault='Y'";
            bool error = true;
            IDataReader dr = null;
            try
            {
                dr = SqlExec.ExecuteQuery.ExecuteReader(sql);
                while (dr.Read())
                {
                    //	Name
                    String name = ReportName;
                    if (name == null || name.Length == 0)
                        name = dr[0].ToString();		//	TableName
                    int count = Utility.Util.GetValueOfInt(dr[1].ToString());
                    if (count > 0)
                        name += "_" + count;
                    pf.SetName(name);
                    //
                    pf.SetAD_PrintColor_ID(Utility.Util.GetValueOfInt(dr[2].ToString()));
                    pf.SetAD_PrintFont_ID(Utility.Util.GetValueOfInt(dr[3].ToString()));
                    pf.SetAD_PrintPaper_ID(Utility.Util.GetValueOfInt(dr[4].ToString()));
                    //
                    pf.SetAD_Table_ID(Utility.Util.GetValueOfInt(dr[5].ToString()));
                    error = false;
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                s_log.Severe(e.ToString());
                //log. if any
            }
            if (error)
                return null;

            //	Save & complete
            if (!pf.Save())
                return null;
            //	pf.dump();
            pf.SetItems(CreateItems(ctx, pf,0));
            //
            return pf;
        }	//	createFromReportView

        /// <summary>
        /// Get Language
        /// </summary>
        /// <returns>language</returns>
        public Language GetLanguage()
        {
            return _language;
        }	//	getLanguage

        /// <summary>
        /// Set Language
        /// </summary>
        /// <param name="language">language</param>
        public void SetLanguage(VAdvantage.Login.Language language)
        {
            if (language != null)
            {
                _language = language;
                //	log.fine("setLanguage - " + language);
            }
            _translationViewLanguage = null;
        }	//	getLanguage

        /// <summary>
        /// Get AD_Column_ID of Order Columns
        /// </summary>
        /// <returns>Array of AD_Column_IDs in Sort Order</returns>
        public int[] GetOrderAD_Column_IDs()
        {
            //	SortNo - AD_Column_ID
            Dictionary<int, int> map = new Dictionary<int, int>();
            for (int i = 0; i < _items.Length; i++)
            {
                //	Sort Order and Column must be > 0
                try
                {
                    if (_items[i].GetSortNo() != 0 && _items[i].GetAD_Column_ID() != 0)
                    {
                        if (!map.ContainsKey(_items[i].GetSortNo()))
                        {
                            map.Add(_items[i].GetSortNo(), _items[i].GetAD_Column_ID());
                        }
                        else
                        {
                            map[_items[i].GetSortNo()] = _items[i].GetAD_Column_ID();
                        }
                    }
                }
                catch { }
            }
            //	Get SortNo and Sort them
            int[] keys = new int[map.Keys.Count];
            keys = map.Keys.ToArray();
            Array.Sort(keys);

            //	Create AD_Column_ID array
            int[] retValue = new int[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                int value = (int)map[keys[i]];
                retValue[i] = value;
            }
            return retValue;
        }	//	getOrderAD_Column_IDs


        /// <summary>
        /// Get AD_Column_IDs of columns in Report
        /// </summary>
        /// <returns>Array of AD_Column_ID</returns>
        public int[] GetAD_Column_IDs()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i].GetAD_Column_ID() != 0 && _items[i].IsPrinted())
                    list.Add(_items[i].GetAD_Column_ID());
            }
            //	Convert
            int[] retValue = new int[list.Count];
            for (int i = 0; i < list.Count; i++)
                retValue[i] = ((int)list[i]);
            return retValue;
        }	//	getAD_Column_IDs


        /// <summary>
        /// Get Item Count
        /// </summary>
        /// <returns>number of items or -1 if items not defined</returns>
        public int GetItemCount()
        {
            if (_items == null)
                return -1;
            return _items.Length;
        }	//	getItemCount


        /// <summary>
        /// Set Table based.
        /// Reset Form
        /// </summary>
        /// <param name="tableBased">true if table based</param>
        public new void SetIsTableBased(bool tableBased)
        {
            base.SetIsTableBased(tableBased);
            if (tableBased)
                base.SetIsForm(false);
        }	//	setIsTableBased


        /// <summary>
        /// Set Translation View Language.
        /// </summary>
        /// <param name="language">language (checked for base language)</param>
        public void SetTranslationLanguage(VAdvantage.Login.Language language)
        {
            if (language == null || language.IsBaseLanguage())
            {
                _translationViewLanguage = null;
            }
            else
            {
                _translationViewLanguage = language.GetAD_Language();
                _language = language;
            }
        }	//	setTranslationLanguage

        /// <summary>
        /// Get Translation View use
        /// </summary>
        /// <returns>true if a translation view is used</returns>
        public bool IsTranslationView()
        {
            return _translationViewLanguage != null;
        }	//	isTranslationView

        /// <summary>
        /// Update the Query to access the Translation View.
        /// Can be called multiple times, adds only if not set already
        /// </summary>
        /// <param name="query">query to be updated</param>
        public void SetTranslationViewQuery(Query query)
        {
            //	Set Table Name and add add restriction, if a view and language set
            if (_translationViewLanguage != null && query != null && query.GetTableName().ToUpper().EndsWith("_V"))
            {
                query.SetTableName(query.GetTableName() + "t");
                query.AddRestriction("AD_Language", Query.EQUAL, _translationViewLanguage);
            }
        }	//	setTranslationViewQuery

        /// <summary>
        /// Get Optional TableFormat
        /// </summary>
        /// <param name="AD_PrintTableFormat_ID">table format</param>
        new public void SetAD_PrintTableFormat_ID(int AD_PrintTableFormat_ID)
        {
            base.SetAD_PrintTableFormat_ID(AD_PrintTableFormat_ID);
            _tFormat = MPrintTableFormat.Get(GetCtx(), AD_PrintTableFormat_ID, GetAD_PrintFont_ID());
        }	//	getAD_PrintTableFormat_ID

        /// <summary>
        /// Load Special data (images, ..).
        /// To be extended by sub-classes
        /// </summary>
        /// <param name="rs">DataRow</param>
        /// <param name="index">zero based index</param>
        /// <returns>value</returns>
        protected Object LoadSpecial(DataRow rs, int index)
        {
            //	CreateCopy
            //	log.config(p_info.getColumnName(index));
            return null;
        }   //  loadSpecial

        /// <summary>
        /// 
        /// </summary>
        public void SetTranslation()
        {
            StringBuilder sb = new StringBuilder("UPDATE AD_PrintFormatItem_Trl t"
                + " SET (PrintName, PrintNameSuffix)="
                + " (SELECT PrintName, PrintNameSuffix FROM AD_PrintFormatItem i WHERE i.AD_PrintFormatItem_ID=t.AD_PrintFormatItem_ID) "
                + "WHERE AD_PrintFormatItem_ID IN"
                + " (SELECT AD_PrintFormatItem_ID FROM AD_PrintFormatItem WHERE AD_PrintFormat_ID=").Append(Get_ID()).Append(")");
            int no = DataBase.DB.ExecuteQuery(sb.ToString(), null, Get_TrxName());
            log.Fine("setTranslation #" + no);
        }




        //Lakhwinder
        //Grid Report Paging
        public bool IsGridReport
        {
            get
            {
                return isGridReport;
            }
            set
            {
                isGridReport = value;
            }
        }
        public int PageNo
        {
            get
            {
                return pageNo;
            }
            set
            {
                pageNo= value;
            }
        }
        public int TotalPage
        {
            get
            {
                return totalPages;
            }
            set
            {
                totalPages = value;
            }
        }
    }
}
