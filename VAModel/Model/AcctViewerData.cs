/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : AcctViewerData
 * Purpose        : Account Viewer State - maintaines State information for the Account Viewer
 * Class Used     : None
 * Chronological    Development
 * Raghunandan     18-Dec-2009
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
using VAdvantage.Logging;
using VAdvantage.Login;

namespace VAdvantage.Model
{
    public class AcctViewerData
    {
        #region Private PrivateVariable
        //Window              
        public int windowNo;
        //Client				
        public int AD_Client_ID;
        //All Acct Schema		
        public MAcctSchema[] ASchemas = null;
        //This Acct Schema	
        public MAcctSchema ASchema = null;

        //  Selection Info
        //Document Query		
        public bool documentQuery = false;
        // Acct Schema			
        public int C_AcctSchema_ID = 0;
        //Posting Type		
        public String PostingType = "";
        // Organization		
        public int AD_Org_ID = 0;
        // Date From		
        public DateTime? DateFrom = null;
        // Date To			
        public DateTime? DateTo = null;

        //  Dodument Table Selection Info
        // Table ID			
        public int AD_Table_ID;
        // Record			
        public int Record_ID;

        //Containing Column and Query
        //public HashMap<String,String>	whereInfo = new HashMap<String,String>();
        public Dictionary<String, String> whereInfo = new Dictionary<String, String>();
        //Containing TableName and AD_Table_ID    
        //public HashMap<String,Integer>	tableInfo = new HashMap<String,Integer>();
        public Dictionary<String, int> tableInfo = new Dictionary<String, int>();

        //  Display Info
        //Display Qty			
        public bool displayQty = false;
        //Display Source Surrency
        public bool displaySourceAmt = false;
        //Display Document info	
        public bool displayDocumentInfo = false;
        //
        public String sortBy1 = "";
        public String sortBy2 = "";
        public String sortBy3 = "";
        public String sortBy4 = "";
        //
        public bool group1 = false;
        public bool group2 = false;
        public bool group3 = false;
        public bool group4 = false;

        // Leasing Columns		
        private int _leadingColumns = 0;
        //UserElement1 Reference	
        private String _ref1 = null;
        //UserElement2 Reference	
        private String _ref2 = null;
        private Ctx _ctx = null;
        //Logger	
        private static VLogger log = VLogger.GetVLogger(typeof(AcctViewerData).FullName);
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="windowNo"></param>
        /// <param name="ad_Client_ID"></param>
        /// <param name="ad_Table_ID"></param>
        public AcctViewerData(Ctx ctx, int windowNo1, int ad_Client_ID, int ad_Table_ID)
        {
            windowNo = windowNo1;
            AD_Client_ID = ad_Client_ID;
            if (AD_Client_ID == 0)
            {
                AD_Client_ID = ctx.GetContextAsInt(windowNo, "AD_Client_ID");
            }
            if (AD_Client_ID == 0)
            {
                AD_Client_ID = ctx.GetContextAsInt("AD_Client_ID");
            }
            AD_Table_ID = ad_Table_ID;
            //
            ASchemas = MAcctSchema.GetClientAcctSchema(ctx, AD_Client_ID);
            ASchema = ASchemas[0];
            _ctx = ctx;
        }


        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            ASchemas = null;
            ASchema = null;
            //
            whereInfo.Clear();
            whereInfo = null;
            //
            Env.ClearWinContext(windowNo);
        }

        /// <summary>
        /// Get Accounting Schema
        /// </summary>
        /// <returns>the accounting schema</returns>
        public ListBoxVO GetAcctSchema()
        {
            List<NamePair> options = new List<NamePair>();
            for (int i = 0; i < ASchemas.Length; i++)
            {
                options.Add(new KeyNamePair(ASchemas[i].GetC_AcctSchema_ID(), ASchemas[i].GetName()));
            }
            //return new ListBoxVO(options, null);
            return new ListBoxVO(options, null);
        }

        /// <summary>
        /// Get Posting Type
        /// </summary>
        /// <returns></returns>
        public ListBoxVO GetPostingType()
        {
            int AD_Reference_ID = 125;
            List<NamePair> options = new List<NamePair>(MRefList.GetList(AD_Reference_ID, true));
            return new ListBoxVO(options, null);
        }


        /// <summary>
        /// Get Table with ValueNamePair (TableName, translatedKeyColumnName) and
        /// tableInfo with (TableName, AD_Table_ID) and select the entry for AD_Table_ID
        /// </summary>
        /// <returns></returns>
        public ListBoxVO GetTable()
        {
            List<NamePair> options = new List<NamePair>();
            String defaultKey = null;
            //
            String sql = "SELECT AD_Table_ID, TableName FROM AD_Table t "
                + "WHERE EXISTS (SELECT * FROM AD_Column c"
                + " WHERE t.AD_Table_ID=c.AD_Table_ID AND c.ColumnName='Posted')"
                + " AND IsView='N'";
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    int id = Utility.Util.GetValueOfInt(idr[0]);//.getInt(1);
                    String tableName = Utility.Util.GetValueOfString(idr[1]);//.getString(2);
                    String name = Msg.Translate(_ctx, tableName + "_ID");
                    ValueNamePair pp = new ValueNamePair(tableName, name);
                    options.Add(pp);
                    //tableInfo.put(tableName, new Integer(id));
                    tableInfo.Add(tableName.ToString(), id);
                    if (id == AD_Table_ID)
                    {
                        defaultKey = pp.GetValue();
                    }
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

            return new ListBoxVO(options, defaultKey);
        }

        /// <summary>
        /// Get Org
        ///cb JComboBox to be filled
        /// </summary>
        /// <returns></returns>
        public ListBoxVO GetOrg()
        {
            List<NamePair> options = new List<NamePair>();
            KeyNamePair pp = new KeyNamePair(0, "");
            options.Add(pp);
            String sql = "SELECT AD_Org_ID, Name FROM AD_Org WHERE AD_Client_ID=" + AD_Client_ID + " ORDER BY Value";
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    options.Add(new KeyNamePair(Utility.Util.GetValueOfInt(idr[0]), Utility.Util.GetValueOfString(idr[1])));
                }
                idr.Close();
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            return new ListBoxVO(options, null);
        }

        /// <summary>
        /// Get Button Text
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="selectSQL"></param>
        /// <returns>Text on button</returns>
        public String GetButtonText(String tableName, String columnName, String selectSQL)
        {
            //  SELECT (<embedded>) FROM tableName avd WHERE avd.<selectSQL>
            StringBuilder sql = new StringBuilder("SELECT (");
            Language language = Env.GetLanguage(_ctx);
            sql.Append(VLookUpFactory.GetLookup_TableDirEmbed(language, columnName, "avd"))
                .Append(") FROM ").Append(tableName).Append(" avd WHERE avd.").Append(selectSQL);
            String retValue = "<" + selectSQL + ">";
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, null);
                if (idr.Read())
                {
                    retValue = Utility.Util.GetValueOfString(idr[0]);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            return retValue;
        }

        /// <summary>
        /// Create Query and submit
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>Report Model</returns>
        public RModel Query(Ctx ctx)
        {
            //  Set Where Clause
            StringBuilder whereClause = new StringBuilder();
            //  Add Organization
            if (C_AcctSchema_ID != 0)
            {
                whereClause.Append(RModel.TABLE_ALIAS)
                    .Append(".C_AcctSchema_ID=").Append(C_AcctSchema_ID);
            }

            //	Posting Type Selected
            if (PostingType != null && PostingType.Length > 0)
            {
                if (whereClause.Length > 0)
                {
                    whereClause.Append(" AND ");
                }
                whereClause.Append(RModel.TABLE_ALIAS)
                    .Append(".PostingType='").Append(PostingType).Append("'");
            }

            //
            if (documentQuery)
            {
                if (whereClause.Length > 0)
                {
                    whereClause.Append(" AND ");
                }
                whereClause.Append(RModel.TABLE_ALIAS).Append(".AD_Table_ID=").Append(AD_Table_ID)
                    .Append(" AND ").Append(RModel.TABLE_ALIAS).Append(".Record_ID=").Append(Record_ID);
            }
            else
            {
                //  get values (Queries)
                //Iterator<String> it = whereInfo.values().iterator();
                IEnumerator<String> it = whereInfo.Values.GetEnumerator();

                while (it.MoveNext())
                {
                    String where = (String)it.Current;
                    if (where != null && where.Length > 0)    //  add only if not empty
                    {
                        if (whereClause.Length > 0)
                        {
                            whereClause.Append(" AND ");
                        }
                        whereClause.Append(RModel.TABLE_ALIAS).Append(".").Append(where);
                    }
                }
                if (DateFrom != null || DateTo != null)
                {
                    if (whereClause.Length > 0)
                    {
                        whereClause.Append(" AND ");
                    }
                    if (DateFrom != null && DateTo != null)
                    {
                        whereClause.Append("TRUNC(").Append(RModel.TABLE_ALIAS).Append(".DateAcct,'DD') BETWEEN ")
                            .Append(DataBase.DB.TO_DATE(DateFrom)).Append(" AND ").Append(DataBase.DB.TO_DATE(DateTo));
                    }
                    else if (DateFrom != null)
                    {
                        whereClause.Append("TRUNC(").Append(RModel.TABLE_ALIAS).Append(".DateAcct,'DD') >= ")
                            .Append(DataBase.DB.TO_DATE(DateFrom));
                    }
                    else    //  DateTo != null
                    {
                        whereClause.Append("TRUNC(").Append(RModel.TABLE_ALIAS).Append(".DateAcct,'DD') <= ")
                            .Append(DataBase.DB.TO_DATE(DateTo));
                    }
                }
                //  Add Organization
                if (AD_Org_ID != 0)
                {
                    if (whereClause.Length > 0)
                    {
                        whereClause.Append(" AND ");
                    }
                    whereClause.Append(RModel.TABLE_ALIAS).Append(".AD_Org_ID=").Append(AD_Org_ID);
                }
            }

            //  Set Order By Clause
            StringBuilder orderClause = new StringBuilder();
            if (sortBy1.Length > 0)
            {
                orderClause.Append(RModel.TABLE_ALIAS).Append(".").Append(sortBy1);
            }
            if (sortBy2.Length > 0)
            {
                if (orderClause.Length > 0)
                {
                    orderClause.Append(",");
                }
                orderClause.Append(RModel.TABLE_ALIAS).Append(".").Append(sortBy2);
            }
            if (sortBy3.Length > 0)
            {
                if (orderClause.Length > 0)
                {
                    orderClause.Append(",");
                }
                orderClause.Append(RModel.TABLE_ALIAS).Append(".").Append(sortBy3);
            }
            if (sortBy4.Length > 0)
            {
                if (orderClause.Length > 0)
                {
                    orderClause.Append(",");
                }
                orderClause.Append(RModel.TABLE_ALIAS).Append(".").Append(sortBy4);
            }
            if (orderClause.Length == 0)
            {
                orderClause.Append(RModel.TABLE_ALIAS).Append(".Fact_Acct_ID");
            }
            //get grid view columns
            RModel rm = GetRModel(ctx);

            //  Groups
            if (group1 && sortBy1.Length > 0)
            {
                rm.SetGroup(sortBy1);
            }
            if (group2 && sortBy2.Length > 0)
            {
                rm.SetGroup(sortBy2);
            }
            if (group3 && sortBy3.Length > 0)
            {
                rm.SetGroup(sortBy3);
            }
            if (group4 && sortBy4.Length > 0)
            {
                rm.SetGroup(sortBy4);
            }

            //  Totals
            rm.SetFunction("AmtAcctDr", RModel.FUNCTION_SUM);
            rm.SetFunction("AmtAcctCr", RModel.FUNCTION_SUM);

            rm.Query(ctx, whereClause.ToString(), orderClause.ToString());

            return rm;
        }

        /// <summary>
        /// Create Report Model (Columns)
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>Report Model</returns>
        public RModel GetRModel(Ctx ctx)
        {
            RModel rm = new RModel("Fact_Acct");
            //  Add Key (Lookups)
            List<String> keys = CreateKeyColumns();
            int max = _leadingColumns;
            if (max == 0)
            {
                max = keys.Count;
            }
            for (int i = 0; i < max; i++)
            {
                String column = (String)keys[i];
                if (column != null && column.StartsWith("Date"))
                {
                    rm.AddColumn(new RColumn(ctx, column, DisplayType.Date));
                }
                else if (column != null && column.EndsWith("_ID"))
                {
                    rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir));
                }
            }
            //  Main Info
            rm.AddColumn(new RColumn(ctx, "AmtAcctDr", DisplayType.Amount));
            rm.AddColumn(new RColumn(ctx, "AmtAcctCr", DisplayType.Amount));
            if (displaySourceAmt)
            {
                if (!keys.Contains("DateTrx"))
                {
                    rm.AddColumn(new RColumn(ctx, "DateTrx", DisplayType.Date));
                }
                rm.AddColumn(new RColumn(ctx, "C_Currency_ID", DisplayType.TableDir));
                rm.AddColumn(new RColumn(ctx, "AmtSourceDr", DisplayType.Amount));
                rm.AddColumn(new RColumn(ctx, "AmtSourceCr", DisplayType.Amount));
                rm.AddColumn(new RColumn(ctx, "Rate", DisplayType.Amount,
                    "CASE WHEN (AmtSourceDr + AmtSourceCr) = 0 THEN 0"
                    + " ELSE (AmtAcctDr + AmtAcctCr) / (AmtSourceDr + AmtSourceCr) END"));
            }
            //	Remaining Keys
            for (int i = max; i < keys.Count; i++)
            {
                String column = (String)keys[i];
                if (column != null && column.StartsWith("Date"))
                {
                    rm.AddColumn(new RColumn(ctx, column, DisplayType.Date));
                }
                else if (column.StartsWith("UserElement"))
                {
                    if (column.IndexOf("1") != -1)
                    {
                        rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir, null, 0, _ref1));
                    }
                    else
                    {
                        rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir, null, 0, _ref2));
                    }
                }
                else if (column != null && column.EndsWith("_ID"))
                {
                    rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir));
                }
            }
            //	Info
            if (!keys.Contains("DateAcct"))
            {
                rm.AddColumn(new RColumn(ctx, "DateAcct", DisplayType.Date));
            }
            if (!keys.Contains("C_Period_ID"))
            {
                rm.AddColumn(new RColumn(ctx, "C_Period_ID", DisplayType.TableDir));
            }
            if (displayQty)
            {
                rm.AddColumn(new RColumn(ctx, "C_UOM_ID", DisplayType.TableDir));
                rm.AddColumn(new RColumn(ctx, "Qty", DisplayType.Quantity));
            }
            if (displayDocumentInfo)
            {
                rm.AddColumn(new RColumn(ctx, "AD_Table_ID", DisplayType.TableDir));
                rm.AddColumn(new RColumn(ctx, "Record_ID", DisplayType.ID));
                rm.AddColumn(new RColumn(ctx, "Description", DisplayType.String));
            }
            if (PostingType == null || PostingType.Length == 0)
            {
                rm.AddColumn(new RColumn(ctx, "PostingType", DisplayType.List,
                    MFactAcct.POSTINGTYPE_AD_Reference_ID));
            }
            return rm;
        }

        /// <summary>
        /// Create the key columns in sequence
        /// </summary>
        /// <returns>List of Key Columns</returns>
        private List<String> CreateKeyColumns()
        {
            List<String> columns = new List<String>();
            _leadingColumns = 0;
            //  Sorting Fields
            columns.Add(sortBy1);               //  may add ""
            if (!columns.Contains(sortBy2))
            {
                columns.Add(sortBy2);
            }
            if (!columns.Contains(sortBy3))
            {
                columns.Add(sortBy3);
            }
            if (!columns.Contains(sortBy4))
            {
                columns.Add(sortBy4);
            }

            //  Add Account Segments
            MAcctSchemaElement[] elements = ASchema.GetAcctSchemaElements();
            for (int i = 0; i < elements.Length; i++)
            {
                if (_leadingColumns == 0 && columns.Contains("AD_Org_ID") && columns.Contains("Account_ID"))
                {
                    _leadingColumns = columns.Count;
                }
                //
                MAcctSchemaElement ase = elements[i];
                String columnName = ase.GetColumnName();
                if (columnName.StartsWith("UserElement"))
                {
                    if (columnName.IndexOf("1") != -1)
                    {
                        _ref1 = ase.GetDisplayColumnName();
                    }
                    else
                    {
                        _ref2 = ase.GetDisplayColumnName();
                    }
                }
                if (!columns.Contains(columnName))
                {
                    columns.Add(columnName);

                }
            }
            if (_leadingColumns == 0 && columns.Contains("AD_Org_ID") && columns.Contains("Account_ID"))
            {
                _leadingColumns = columns.Count;
            }
            return columns;
        }
    }
}
