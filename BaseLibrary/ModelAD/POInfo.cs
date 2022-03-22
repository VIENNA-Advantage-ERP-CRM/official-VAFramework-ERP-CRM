using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using VAdvantage.Classes;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    /*******************************************************************************/
    //    POINFO Class
    /*******************************************************************************/

    /// <summary>
    /// Persistet Object Info.
    /// Provides structural information
    /// </summary>
    public class POInfo
    {

        #region "Declaration"
        public static String ACCESSLEVEL_Organization = "1";
        /** Client only = 2 */
        public static String ACCESSLEVEL_ClientOnly = "2";
        /** Client+Organization = 3 */
        public static String ACCESSLEVEL_ClientPlusOrganization = "3";
        /** System only = 4 */
        public static String ACCESSLEVEL_SystemOnly = "4";
        /** System+Client = 6 */
        public static String ACCESSLEVEL_SystemPlusClient = "6";
        /** All = 7 */
        public static String ACCESSLEVEL_All = "7";
        /** TableTrxType AD_Control_Ref_ID=493 */
        /** Mandatory Organization = M */
        public static String TABLETRXTYPE_MandatoryOrganization = "M";
        /** No Organization = N */
        public static String TABLETRXTYPE_NoOrganization = "N";
        /** Optional Organization = O */
        public static String TABLETRXTYPE_OptionalOrganization = "O";

        Ctx m_ctx;
        int _AD_Table_ID;
        private POInfoColumn[] m_columns = null;
        private string m_TableName = "";
        public bool m_hasKeyColumn;
        string m_AccessLevel = ACCESSLEVEL_Organization;
        /** Cache of POInfo     */
        private static CCache<int, POInfo> s_cache = new CCache<int, POInfo>("POInfo", 200);

        ///** Original Values         */
        //private Object[] _mOldValues = null;
        ///** New Values              */
        //private Object[] _mNewValues = null;
        ///** Columns             	*/
        //private POInfoColumn[] _mcolumns = null;

        //Transaction Table		//Add by raghu on 4-March-2011
        private String _TableTrxType = TABLETRXTYPE_OptionalOrganization;
        /** Key Columns					*/
        private String[] _KeyColumns = null;

        #endregion

        /// <summary>
        ///Create Persistent Info
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Table_ID"></param>
        /// <param name="baseLanguageOnly"></param>
        private POInfo(Context ctx, int AD_Table_ID, bool baseLanguageOnly)
        {
            m_ctx = ctx;
            _AD_Table_ID = AD_Table_ID;

            bool baseLanguage = baseLanguageOnly ? true : Env.IsBaseLanguage(ctx, "");
            LoadInfo(baseLanguage);
        }   //  

        private POInfo(Ctx ctx, int AD_Table_ID, bool baseLanguageOnly)
        {
            m_ctx = ctx;
            _AD_Table_ID = AD_Table_ID;

            bool baseLanguage = baseLanguageOnly ? true : Env.IsBaseLanguage(ctx, "");
            LoadInfo(baseLanguage);
        }



        /// <summary>
        ///  Load Table/Column Info
        /// baseLanguage in English
        /// </summary>
        /// <param name="baseLanguage"></param>
        private void LoadInfo(bool baseLanguage)
        {
            List<POInfoColumn> list = new List<POInfoColumn>(20);

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT t.TableName, c.ColumnName,c.AD_Reference_ID,"    //  1..3
                + "c.IsMandatory,c.IsUpdateable,c.DefaultValue,"                //  4..6
                + "e.Name,e.Description, c.AD_Column_ID, "						//  7..9
                + "c.IsKey,c.IsParent, "										//	10..11
                + "c.AD_Reference_Value_ID, vr.Code, "							//	12..13
                + "c.FieldLength, c.ValueMin, c.ValueMax, c.IsTranslated, "		//	14..17
                + "t.AccessLevel, c.ColumnSQL, c.IsEncrypted , c.IsCopy ");				//	18..21
            sql.Append("FROM AD_Table t"
                + " INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID)"
                + " LEFT OUTER JOIN AD_Val_Rule vr ON (c.AD_Val_Rule_ID=vr.AD_Val_Rule_ID)"
                + " INNER JOIN AD_Element");
            if (!baseLanguage)
                sql.Append("_Trl");
            sql.Append(" e "
                + " ON (c.AD_Element_ID=e.AD_Element_ID) "
                + "WHERE t.AD_Table_ID=" + _AD_Table_ID + " "
                + " AND c.IsActive='Y'");
            if (!baseLanguage)
                sql.Append(" AND e.AD_Language='").Append(Env.GetAD_Language(m_ctx)).Append("'");

            IDataReader dr = null;
            try
            {
                dr = DB.ExecuteReader(sql.ToString(), null, null);
                while (dr.Read())
                {
                    if (m_TableName == "")
                        m_TableName = Util.GetValueOfString(dr[0]);
                    string ColumnName = Util.GetValueOfString(dr[1]);
                    int AD_Reference_ID = Util.GetValueOfInt(dr[2]);
                    bool IsMandatory = "Y".Equals(dr[3]);
                    bool IsUpdateable = "Y".Equals(dr[4]);
                    string DefaultLogic = Util.GetValueOfString(dr[5]);
                    string Name = Util.GetValueOfString(dr[6]);
                    string Description = Util.GetValueOfString(dr[7]);
                    int AD_Column_ID = Util.GetValueOfInt(dr[8]);
                    bool IsKey = "Y".Equals(Util.GetValueOfString(dr[9]));
                    if (IsKey)
                        m_hasKeyColumn = true;
                    bool IsParent = "Y".Equals(Util.GetValueOfString(dr[10]));
                    int AD_Reference_Value_ID = Util.GetValueOfInt(dr[11]);
                    string ValidationCode = Util.GetValueOfString(dr[12]);
                    int FieldLength = Util.GetValueOfInt(dr[13]);
                    string ValueMin = Util.GetValueOfString(dr[14]);
                    string ValueMax = Util.GetValueOfString(dr[15]);
                    bool IsTranslated = "Y".Equals(Util.GetValueOfString(dr[16]));
                    //
                    m_AccessLevel = Util.GetValueOfString(dr[17]);
                    String ColumnSQL = Util.GetValueOfString(dr[18]);
                    bool IsEncrypted = "Y".Equals(Util.GetValueOfString(dr[19]));
                    bool IsCopy = "Y".Equals(Util.GetValueOfString(dr[20]));
                    POInfoColumn col = new POInfoColumn(
                        AD_Column_ID, ColumnName, ColumnSQL, AD_Reference_ID,
                        IsMandatory, IsUpdateable,
                        DefaultLogic, Name, Description,
                        IsKey, IsParent,
                        AD_Reference_Value_ID, ValidationCode,
                        FieldLength, ValueMin, ValueMax,
                        IsTranslated, IsEncrypted, IsCopy);
                    list.Add(col);
                }
                dr.Close();
                dr = null;
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                VLogger.Get().Log(Level.SEVERE, sql.ToString(), e);
            }
            m_columns = list.ToArray();
        }

        public bool IsTranslated()
        {
            for (int i = 0; i < m_columns.Length; i++)
            {
                if (m_columns[i].IsTranslated)
                    return true;
            }
            return false;
        }

        public POInfoColumn[] GetPoInfoColumns()
        {
            return m_columns;
        }

        /// <summary>
        /// POInfo Factory
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Table_ID"></param>
        /// <returns>POInfo Object</returns>
        //public static POInfo GetPOInfo(Ctx ctx, int AD_Table_ID)
        //{
        //    int key = AD_Table_ID;
        //    POInfo retValue = null;

        //    s_cache.TryGetValue(key, out  retValue);
        //    if (retValue == null)
        //    {
        //        retValue = new POInfo(ctx, AD_Table_ID, false);
        //        if (retValue.GetColumnCount() == 0)
        //            //	May be run before Language verification
        //            retValue = new POInfo(ctx, AD_Table_ID, true);
        //        else
        //            s_cache.Add(key, retValue);
        //    }
        //    return retValue;
        //}   //  getPOInfo


        public static POInfo GetPOInfo(Ctx ctx, int AD_Table_ID)
        {
            int key = AD_Table_ID;
            POInfo retValue = null;

            s_cache.TryGetValue(key, out retValue);
            if (retValue == null)
            {
                retValue = new POInfo(ctx, AD_Table_ID, false);
                if (retValue.GetColumnCount() == 0)
                    //	May be run before Language verification
                    retValue = new POInfo(ctx, AD_Table_ID, true);
                else
                    s_cache.Add(key, retValue);
            }
            return retValue;
        }   //  getPOInfo

        /// <summary>
        /// Get SQL Query for table
        /// </summary>
        /// <returns>SQL Query (String)</returns>


        /// <summary>
        /// Get ColumnCount
        /// column count
        /// </summary>
        /// <returns></returns>
        public int GetColumnCount()
        {
            return m_columns.Length;
        }   //  getColumnCount

        public string GetTableName()
        {
            return m_TableName;
        }

        /// <summary>
        ///Is key Column
        /// </summary>
        /// <param name="index"></param>
        /// <returns>return true if key column</returns>
        public bool IsKey(int index)
        {
            if (index < 0 || index >= m_columns.Length)
                return false;
            return m_columns[index].IsKey;  //    m_columns[ index].IsKey;
        }   //  isKey

        /// <summary>
        ///Is Copy Column
        /// </summary>
        /// <param name="index"></param>
        /// <returns>return true if Copy column</returns>
        public bool IsCopy(int index)
        {
            if (index < 0 || index >= m_columns.Length)
                return false;
            return m_columns[index].IsCopy;
        }   //  isCopy

        public bool IsColumnTranslated(int index)
        {
            if (index < 0 || index >= m_columns.Length)
                return false;
            return m_columns[index].IsTranslated;
        }

        /// <summary>
        ///Is parent Column 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>return true if key column</returns>
        public bool IsColumnParent(int index)
        {
            if (index < 0 || index >= m_columns.Length)
                return false;
            return m_columns[index].IsParent;  //    m_columns[ index].IsKey;
        }   //  isKey


        /// <summary>
        /// Get Column
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>column</returns>
        public POInfoColumn GetColumn(int index)
        {
            if (index < 0 || index >= m_columns.Length)
                return null;
            return m_columns[index];
        }   //  getColumn


        /// <summary>
        /// Get Column Name
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetColumnName(int index)
        {
            if (index < 0 || index >= m_columns.Length)
                return "";
            return m_columns[index].ColumnName;
        }   //  getColumnName

        /// <summary>
        /// Get Column SQL or Column Name
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetColumnSQL(int index)
        {
            if (index < 0 || index >= m_columns.Length)
                return null;
            if ((m_columns[index].ColumnSQL != null || m_columns[index].ColumnSQL == "") && m_columns[index].ColumnSQL.Length > 0)
                return m_columns[index].ColumnSQL + " AS " + m_columns[index].ColumnName;
            return m_columns[index].ColumnName;
        }   //


        public bool IsEncrypted(int index)
        {
            if (index < 0 || index >= m_columns.Length)
                return false;
            return m_columns[index].IsEncrypted;
        }

        public bool hasKeyColumn()
        {
            return m_hasKeyColumn;
        }	//	hasKeyColumn

        /// <summary>
        /// Is virtual Column 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsVirtualColumn(int index)
        {
            if (index < 0 || index >= m_columns.Length)
                return true;
            return m_columns[index].ColumnSQL != null
                && m_columns[index].ColumnSQL.Length > 0;
        }   // 

        /// <summary>
        ///  Get Column Display Type
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetColumnDisplayType(int index)
        {
            if (index < 0 || index >= m_columns.Length)
                return DisplayType.String;
            return m_columns[index].DisplayType;
        }

        /// <summary>
        /// Get Table Access Level
        /// </summary>
        /// <returns>Table.ACCESS</returns>
        /// <createdBy>Raghu</createdBy>
        public String GetAccessLevel()
        {
            return m_AccessLevel;
        }

        /// <summary>
        /// Get Column Index
        /// </summary>
        /// <param name="ColumnName">column name</param>
        /// <returns></returns>
        public int GetColumnIndex(string ColumnName)
        {
            if (DatabaseType.IsMSSql)
            {
                if (ColumnName.Equals("LineNo"))
                    ColumnName = "[LineNo]";
            }
            for (int i = 0; i < m_columns.Length; i++)
            {
                if (ColumnName.Equals(m_columns[i].ColumnName))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Get Column Index
        /// </summary>
        /// <param name="AD_Column_ID">column</param>
        /// <returns>index of column with ColumnName or -1 if not found</returns>
        public int GetColumnIndex(int AD_Column_ID)
        {
            for (int i = 0; i < m_columns.Length; i++)
            {
                if (AD_Column_ID == m_columns[i].AD_Column_ID)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Is Column Updateable
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsColumnUpdateable(int index)
        {
            if (index < 0 || index >= m_columns.Length)
                return false;
            return m_columns[index].IsUpdateable;
        }   //  isUpdateable

        /// <summary>
        /// Get Type of object
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Type GetColumnClass(int index)
        {
            if (index < 0 || index >= m_columns.Length)
                return typeof(String);
            return m_columns[index].ColumnClass;

        }

        /// <summary>
        /// Get Column FieldLength
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetFieldLength(int index)
        {
            if (index < 0 || index >= m_columns.Length)
                return 0;
            return m_columns[index].FieldLength;
        }   //

        /// <summary>
        /// Get Column FieldLength
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public int GetFieldLength(string columnName)
        {
            int index = GetColumnIndex(columnName);
            if (index >= 0)
                return GetFieldLength(index);
            return 0;
        }


        /// <summary>
        /// Check Column Is Mandatory
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsColumnMandatory(int index)
        {
            if (index < 0 || index >= m_columns.Length)
                return false;
            return m_columns[index].IsMandatory;
        }

        /// <summary>
        ///  Validate Content
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Validate(int index, Object value)
        {
            if (index < 0 || index >= m_columns.Length)
                return "RangeError";
            //	Mandatory (i.e. not null
            if (m_columns[index].IsMandatory && value == null)
            {
                return "IsMandatory";
            }
            if (value == null)
                return null;

            //	Length ignored
            POInfoColumn column = m_columns[index];
            //
            //if (column.ValueMin != null && column.ValueMin != "")
            //{
            //    decimal? value_BD = null;
            //    try
            //    {
            //        if (column.ValueMin_BD != 0)
            //            value_BD = decimal.Parse(value.ToString());
            //    }
            //    catch { }
            //    //	Both are Numeric
            //    if (column.ValueMin_BD != 0 && value_BD != null)
            //    {	//	error: 1 - 0 => 1  -  OK: 1 - 1 => 0 & 1 - 10 => -1
            //        int comp = column.ValueMin_BD.CompareTo(value_BD);
            //        if (comp > 0)
            //        {
            //            //return "MinValue=" + column.ValueMin_BD
            //            //    + "(" + column.ValueMin + ")"
            //            //    + " - compared with Numeric Value=" + value_BD + "(" + value + ")"
            //            //    + " - results in " + comp;
            //            return Msg.GetMsg(m_ctx, "ExceedMinValue", new Object[] { value_BD, column.ValueMin_BD });
            //        }
            //    }
            //    else	//	String
            //    {
            //        int comp = column.ValueMin.CompareTo(value.ToString());
            //        if (comp > 0)
            //        {
            //            //return "MinValue=" + column.ValueMin
            //            //  + " - compared with String Value=" + value
            //            //  + " - results in " + comp;
            //            return Msg.GetMsg(m_ctx, "ExceedMinValue", new Object[] { value, column.ValueMin });
            //        }
            //    }
            //}
            //if (column.ValueMax != null && column.ValueMax != "")
            //{
            //    decimal? value_BD = null;
            //    try
            //    {
            //        if (column.ValueMax_BD != 0)
            //            value_BD = decimal.Parse(value.ToString());
            //    }
            //    catch { }
            //    //	Both are Numeric
            //    if (column.ValueMax_BD != 0 && value_BD != null)
            //    {	//	error 12 - 20 => -1  -  OK: 12 - 12 => 0 & 12 - 10 => 1
            //        int comp = column.ValueMax_BD.CompareTo(value_BD);
            //        if (comp < 0)
            //        {
            //            //return "MaxValue=" + column.ValueMax_BD + "(" + column.ValueMax + ")"
            //            //  + " - compared with Numeric Value=" + value_BD + "(" + value + ")"
            //            //  + " - results in " + comp;
            //            return Msg.GetMsg(m_ctx, "ExceedMaxValue", new Object[] { value_BD, column.ValueMax_BD });
            //        }
            //    }
            //    else	//	String
            //    {
            //        int comp = column.ValueMax.CompareTo(value.ToString());
            //        if (comp < 0)
            //        {
            //            return Msg.GetMsg(m_ctx, "ExceedMaxValue", new Object[] { value, column.ValueMax });
            //            //return "MaxValue=" + m_columns[index].ValueMax
            //            //  + " - compared with String Value=" + value
            //            //  + " - results in " + comp;
            //        }
            //    }
            //}
            return null;
        }  //  validate


        public int getAD_Table_ID()
        {
            return _AD_Table_ID;
        }

        /// <summary>
        /// Set all columns updateable
        /// </summary>
        /// <param name="updateable">Updateable</param>
        public void SetUpdateable(bool updateable)
        {

            for (int i = 0; i < m_columns.Length; i++)
                m_columns[i].IsUpdateable = updateable;
        }

        /// <summary>
        /// Get Lookup
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        //public Lookup GetColumnLookup(int index)
        //{
        //    if (!IsColumnLookup(index))
        //        return null;
        //    //
        //    int WindowNo = 0;
        //    //  List, Table, TableDir
        //    Lookup lookup = null;
        //    try
        //    {
        //        lookup = VLookUpFactory.Get(m_ctx, WindowNo, m_columns[index].AD_Column_ID,
        //            m_columns[index].DisplayType,
        //            m_columns[index].ColumnName,
        //            m_columns[index].AD_Reference_Value_ID,
        //            m_columns[index].IsParent, m_columns[index].ValidationCode);
        //    }
        //    catch
        //    {
        //        lookup = null;          //  cannot create Lookup
        //    }
        //    return lookup;
        //}



        /// <summary>
        /// Get lookup Info
        /// </summary>
        /// <param name="index">index of column</param>
        /// <returns>Lookup info</returns>
        /// <remarks>
        /// Used for MMailTemplate for fetch text against Id Column
        /// </remarks>
        //public VLookUpInfo GetColumnLookupInfo(int index)
        //{
        //    if (!IsColumnLookup(index))
        //        return null;
        //    //
        //    int WindowNo = 0;
        //    //  List, Table, TableDir
        //    VLookUpInfo lookup = null;
        //    try
        //    {
        //        lookup = VLookUpFactory.GetLookUpInfo(m_ctx, WindowNo, m_columns[index].DisplayType, m_columns[index].AD_Column_ID, Env.GetLanguage(m_ctx),
        //            m_columns[index].ColumnName, m_columns[index].AD_Reference_Value_ID,
        //            m_columns[index].IsParent, m_columns[index].ValidationCode);
        //    }
        //    catch
        //    {


        //        lookup = null;          //  cannot create Lookup
        //    }
        //    return lookup;
        //}


        public POInfoColumn GetColumnInfo(int index)
        {
            if (!IsColumnLookup(index))
                return null;
            return m_columns[index].Clone();
        }



        /**
        *  Is Lookup Column
        *  @param index index
        *  @return true if it is a lookup column
        */
        public bool IsColumnLookup(int index)
        {
            if (index < 0 || index >= m_columns.Length)
                return false;
            return DisplayType.IsLookup(m_columns[index].DisplayType);
        }

        /// <summary>
        /// Get Transaction Type
        /// </summary>
        /// <returns>p_trx type</returns>
        /// <writer>Raghu</writer>
        /// <date>04-march-2011</date>
        public String GetTableTrxType()
        {
            return _TableTrxType;
        }

        public int GetAD_Table_ID()
        {
            return _AD_Table_ID;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns> key column name</returns>
        /// <writer>raghu</writer>
        /// <date>07-march-2011</date>
        public String[] GetKeyColumns()
        {
            return _KeyColumns;
        }
    }
        /*******************************************************************************/
        //    POInfoColumn Class
        /*******************************************************************************/

        /// <summary>
        /// PO Info Column Info Value Object
        /// </summary>
        public class POInfoColumn
        {
            #region "Declaration"
            /** Column ID		*/
            public int AD_Column_ID;
            /** Column Name		*/
            public string ColumnName;
            /** Virtual Column 	*/
            public string ColumnSQL;
            /** Display Type	*/
            public int DisplayType;
            ///**	Data Type		*/
            //public Class<?>		ColumnClass;

            /**	Mandatory		*/
            public bool IsMandatory;
            /**	Default Value	*/
            public string DefaultLogic;
            /**	Updateable		*/
            public bool IsUpdateable;
            /**	Label			*/
            public string ColumnLabel;
            /**	Description		*/
            public string ColumnDescription;
            /**	PK				*/
            public bool IsKey;
            /**	FK to Parent	*/
            public bool IsParent;
            /**	Translated		*/
            public bool IsTranslated;
            /**	Encryoted		*/
            public bool IsEncrypted;

            /** Reference Value	*/
            public int AD_Reference_Value_ID;
            /** Validation		*/
            public string ValidationCode;
            public Type ColumnClass;
            /** Field Length	*/
            public int FieldLength;
            /**	Min Value		*/
            public string ValueMin;
            /**	Max Value		*/
            public string ValueMax;
            /**	Min Value		*/
            public decimal ValueMin_BD;
            /**	Max Value		*/
            public decimal ValueMax_BD;
            // Is Copy Value
            public bool IsCopy;

            #endregion

            public POInfoColumn()
            {
            }

            public POInfoColumn(int ad_Column_ID, string columnName, string columnSQL, int displayType,
                                 bool isMandatory, bool isUpdateable, string defaultLogic,
                                 string columnLabel, string columnDescription,
                                 bool isKey, bool isParent,
                                 int ad_Reference_Value_ID, string validationCode,
                                 int fieldLength, string valueMin, string valueMax,
                                 bool isTranslated, bool isEncrypted, bool isCopy)
            {

                AD_Column_ID = ad_Column_ID;
                ColumnName = columnName;
                ColumnSQL = columnSQL;
                DisplayType = displayType;

                if (columnName.Equals("AD_Language")
                || columnName.Equals("EntityType")
                || columnName.Equals("DocBaseType"))
                {
                    DisplayType = VAdvantage.Classes.DisplayType.String;
                    ColumnClass = typeof(System.String);
                }
                else if (columnName.Equals("Posted")
                || columnName.Equals("Processed")
                || columnName.Equals("Processing"))
                {
                    ColumnClass = typeof(System.Boolean);
                    //ColumnClass = typeof(System.Boolean);
                }
                else if (columnName.Equals("Record_ID"))
                {
                    DisplayType = VAdvantage.Classes.DisplayType.ID;
                    ColumnClass = typeof(System.Int32);
                }
                else
                {
                    ColumnClass = VAdvantage.Classes.DisplayType.GetClass(displayType, true);
                }
                IsMandatory = isMandatory;
                IsUpdateable = isUpdateable;
                DefaultLogic = defaultLogic;
                ColumnLabel = columnLabel;
                ColumnDescription = columnDescription;
                IsKey = isKey;
                IsParent = isParent;
                //
                AD_Reference_Value_ID = ad_Reference_Value_ID;
                ValidationCode = validationCode;
                FieldLength = fieldLength;
                ValueMin = valueMin;
                try
                {
                    if (valueMin != null && valueMin.Length > 0)
                        ValueMin_BD = decimal.Parse(valueMin);
                }
                catch
                {
                    //log error
                }
                ValueMax = valueMax;

                try
                {
                    if (valueMax != null && valueMax.Length > 0)
                        ValueMax_BD = decimal.Parse(ValueMax);
                }
                catch (Exception ex)
                {
                    VLogger.Get().Log(Level.SEVERE, "ValueMin=" + valueMin, ex);
                }
                IsTranslated = isTranslated;
                IsEncrypted = isEncrypted;
                IsCopy = isCopy;
            }

        public POInfoColumn Clone()
        {
            POInfoColumn clone = new POInfoColumn();
            clone.AD_Column_ID = AD_Column_ID;
            clone.ColumnName = ColumnName;
            clone.ColumnSQL = ColumnSQL;
            clone.DisplayType = DisplayType;
            clone.ColumnClass = ColumnClass;
            clone.DisplayType = DisplayType;
            clone.ColumnClass = ColumnClass;
            clone.IsMandatory = IsMandatory;
            clone.IsUpdateable = IsUpdateable;
            clone.DefaultLogic = DefaultLogic;
            clone.ColumnLabel = ColumnLabel;
            clone.ColumnDescription = ColumnDescription;
            clone.IsKey = IsKey;
            clone.IsParent = IsParent;
            clone.AD_Reference_Value_ID = AD_Reference_Value_ID;
            clone.ValidationCode = ValidationCode;
            clone.FieldLength = FieldLength;
            clone.ValueMin = ValueMin;
            clone.ValueMin_BD = ValueMin_BD;
            clone.ValueMax = ValueMax;
            clone.ValueMax_BD = ValueMax_BD;
            clone.IsTranslated = IsTranslated;
            clone.IsEncrypted = IsEncrypted;
            clone.IsCopy = IsCopy;
            return clone;
        }
    }
    }
    
