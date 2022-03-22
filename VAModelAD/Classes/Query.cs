/********************************************************
// Module Name    : Run Time Show Window
// Purpose        : Query Descriptor.
                    Maintains QueryRestrictions (WHERE clause)
// Class Used     : Ctx.cs
// Created By     : Harwinder 
// Date           : 
**********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;
using VAdvantage.Login;

namespace VAdvantage.Classes
{
    /****************************************************************************
      *     
      *        Query Class
      * 
     ******************************************************************************/
    public class Query:IQuery
    {
        #region "Variablr Declaration"
        public string _tableName = "";
        /**	List of QueryRestrictions		*/
        public List<QueryRestriction> _list = new List<QueryRestriction>();
        /**	Record Count				*/
        private int _recordCount = 999999;
        /** New Record Query			*/
        private bool _newRecord = false;
        /** New Record String			*/
        private static string NEWRECORD = "2=3";

        /** Equal 			*/
        public const string EQUAL = "=";
        /** Equal - 0		*/
        public const int EQUAL_INDEX = 0;
        /** Not Equal		*/
        public const string NOT_EQUAL = "!=";
        /** Like			*/
        public const string LIKE = " LIKE ";
        /** Not Like		*/
        public const string NOT_LIKE = " NOT LIKE ";
        /** Greater			*/
        public const string GREATER = ">";
        /** Greater Equal	*/
        public const string GREATER_EQUAL = ">=";
        /** Less			*/
        public const string LESS = "<";
        /** Less Equal		*/
        public const string LESS_EQUAL = "<=";
        /** Between			*/
        public const string BETWEEN = " BETWEEN ";
        /** Between - 8		*/
        public const int BETWEEN_INDEX = 8;
        /** IN			*/
        public const string IN = " IN ";
        /** NOT IN			*/
        public const string NOT_IN = " NOT IN ";

        /**	Operators for Strings				*/
        public static ValueNamePair[] OPERATORS = new ValueNamePair[] {
          new ValueNamePair (EQUAL,         " = "),		//	0
          new ValueNamePair (NOT_EQUAL,     " != "),
          new ValueNamePair (LIKE,          " ~ "),
          new ValueNamePair (NOT_LIKE,      " !~ "),
          new ValueNamePair (GREATER,           " > "),
          new ValueNamePair (GREATER_EQUAL, " >= "),	//	5
          new ValueNamePair (LESS,          " < "),
          new ValueNamePair (LESS_EQUAL,        " <= "),
          new ValueNamePair (BETWEEN,           " >-< ")	//	8
      //	,new ValueNamePair (IN,				" () "),
      //	new ValueNamePair (NOT_IN,			" !() ")			
      };

        /**	Operators for IDs					*/
        public static ValueNamePair[] OPERATORS_ID = new ValueNamePair[] {
          new ValueNamePair (EQUAL,         " = "),		//	0
          new ValueNamePair (NOT_EQUAL,     " != ")
      //	,new ValueNamePair (IN,				" IN "),			
      //	new ValueNamePair (NOT_IN,			" !() ")			
      };

        /**	Operators for Boolean					*/
        public static ValueNamePair[] OPERATORS_YN = new ValueNamePair[] {
      new ValueNamePair (EQUAL,         " = ")
        };


        /** Dummy List of QueryRestrictions		*/
        private List<QueryRestriction> _dummyList = new List<QueryRestriction>();

        private static VLogger s_log = VLogger.GetVLogger(typeof(Query).FullName);

        #endregion

        public Query(string tableName)
        {
            _tableName = tableName;
        }

        public Query()
        {
        }

        /// <summary>
        ///Constructor get TableNAme from Table
        /// </summary>
        /// <param name="AD_Table_ID"></param>
        public Query(int AD_Table_ID)
        {	//	Use Client Ctx as r/o
            _tableName = Model.MTable.GetTableName(Utility.Env.GetCtx(), AD_Table_ID);
        }	//	MQ
        /// <summary>
        ///Create simple Equal Query.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <returns></returns>

        public static Query GetEqualQuery(string columnName, int value)
        {
            Query query = new Query();
            if (columnName.EndsWith("_ID"))
                query.SetTableName(columnName.Substring(0, columnName.Length - 3));
            query.AddRestriction(columnName, EQUAL, value);
            query.SetRecordCount(1);	//	guess
            return query;
        }

        /// <summary>
        ///Create simple Equal Query.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Query GetEqualQuery(String columnName, Object value)
        {
            Query query = new Query();
            query.AddRestriction(columnName, EQUAL, value);
            query.SetRecordCount(1);	//	guess
            return query;
        }


        /// <summary>
        ///Get Query from Parameter
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_PInstance_ID"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static Query Get(Ctx ctx, int AD_PInstance_ID, String tableName)
        {
            Query query = new Query(tableName);
            //	Temporary Tables - add qualifier (not displayed)
            if (tableName.StartsWith("T_"))
                query.AddRestriction(tableName + ".AD_PInstance_ID=" + AD_PInstance_ID);

            //	How many rows do we have?
            int rows = 0;
            String sql = "SELECT COUNT(*) FROM AD_PInstance_Para WHERE AD_PInstance_ID='" + AD_PInstance_ID + "'";

            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql);
                while (dr.Read())
                {
                    rows = Utility.Util.GetValueOfInt(dr[0].ToString());
                }
                dr.Close();

            }
            catch (Exception e1)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                s_log.Log(Level.SEVERE, sql, e1);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }

            if (rows < 1)
                return query;

            //	Msg.getMsg(GetCtx(), "Parameter")
            bool trl = !Utility.Env.IsBaseLanguage(ctx, "AD_Process_Para");// GlobalVariable.IsBaseLanguage();
            if (!trl)
                sql = "SELECT ip.ParameterName,ip.P_String,ip.P_String_To,"			//	1..3
                    + "ip.P_Number,ip.P_Number_To,"									//	4..5
                    + "ip.P_Date,ip.P_Date_To, ip.Info,ip.Info_To, "				//	6..9
                    + "pp.Name, pp.IsRange,nvl(pp.AD_REFERENCE_ID,0) as AD_REFERENCE_ID, nvl(pp.LoadRecursiveData,'N') as LoadRecursiveData,nvl(pp.ShowChildOfSelected,'N') "										//	10..11..12..13
                    + "FROM AD_PInstance_Para ip, AD_PInstance i, AD_Process_Para pp "
                    + "WHERE i.AD_PInstance_ID=ip.AD_PInstance_ID"
                    + " AND pp.AD_Process_ID=i.AD_Process_ID"
                    + " AND pp.ColumnName=ip.ParameterName"
                    + " AND ip.AD_PInstance_ID='" + AD_PInstance_ID + "'";
            else
                sql = "SELECT ip.ParameterName,ip.P_String,ip.P_String_To, ip.P_Number,ip.P_Number_To,"
                    + "ip.P_Date,ip.P_Date_To, ip.Info,ip.Info_To, "
                    + "ppt.Name, pp.IsRange,nvl(pp.AD_REFERENCE_ID,0) as AD_REFERENCE_ID, nvl(pp.LoadRecursiveData,'N') as LoadRecursiveData,nvl(pp.ShowChildOfSelected,'N') "										//	10..11..12..13
                    + "FROM AD_PInstance_Para ip, AD_PInstance i, AD_Process_Para pp, AD_Process_Para_Trl ppt "
                    + "WHERE i.AD_PInstance_ID=ip.AD_PInstance_ID"
                    + " AND pp.AD_Process_ID=i.AD_Process_ID"
                    + " AND pp.ColumnName=ip.ParameterName"
                    + " AND pp.AD_Process_Para_ID=ppt.AD_Process_Para_ID"
                    + " AND ip.AD_PInstance_ID='" + AD_PInstance_ID + "'"
                    + " AND ppt.AD_Language='" + Utility.Env.GetAD_Language(ctx) + "'";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql);
                //	all records
                for (int row = 0; row <= ds.Tables[0].Rows.Count - 1; row++)
                {
                    if (row == rows)
                    {
                        s_log.Info("(Parameter) - more rows than expected");
                        break;
                    }

                    object Parameter = null;
                    object Parameter_To = null;

                    String ParameterName = ds.Tables[0].Rows[row][0].ToString();
                    String P_String = ds.Tables[0].Rows[row][1].ToString();
                    Parameter = P_String;
                    String P_String_To = ds.Tables[0].Rows[row][2].ToString();
                    Parameter_To = P_String_To;
                    //
                    Double? P_Number = null;

                    string val = "";
                    val = ds.Tables[0].Rows[row][3].ToString();

                    if (!string.IsNullOrEmpty(val.Trim()))
                    {
                        P_Number = Convert.ToDouble(val);
                        Parameter = P_Number;
                    }
                    Double? P_Number_To = null;
                    val = ds.Tables[0].Rows[row][4].ToString();
                    if (!string.IsNullOrEmpty(val))
                    {
                        P_Number_To = Convert.ToDouble(val.Trim());
                        Parameter_To = P_Number_To;
                    }

                    //DateTime? ss = null;
                    //
                    string s = ds.Tables[0].Rows[row][5].ToString();
                    DateTime? P_Date = null;
                    if (!string.IsNullOrEmpty(s.Trim()))
                    {
                        P_Date = Convert.ToDateTime(s);
                        Parameter = P_Date;
                    }

                    s = ds.Tables[0].Rows[row][6].ToString();
                    DateTime? P_Date_To = null;
                    if (!string.IsNullOrEmpty(s))
                    {
                        P_Date_To = Convert.ToDateTime(s.Trim());
                        Parameter_To = P_Date_To;
                    }


                    //
                    String Info = ds.Tables[0].Rows[row][7].ToString();
                    String Info_To = ds.Tables[0].Rows[row][8].ToString();
                    //
                    String Name = ds.Tables[0].Rows[row][9].ToString();
                    bool isRange = "Y".Equals(ds.Tables[0].Rows[row][10].ToString());

                    int displayType = Util.GetValueOfInt(ds.Tables[0].Rows[row]["AD_REFERENCE_ID"].ToString());


                    if (ds.Tables[0].Rows[row][12].ToString().Equals("Y") && ((DisplayType.IsID(displayType) || DisplayType.MultiKey == displayType)))
                    {
                        string result = Parameter.ToString();
                        string recResult = GetRecursiveParameterValue(ctx, ParameterName, result.ToString(), ref result, ds.Tables[0].Rows[row][13].ToString().Equals("Y"));
                        if (!string.IsNullOrEmpty(recResult))
                        {
                            Info = Info + ", " + recResult;
                        }
                        double output = 0;
                        if (double.TryParse(result, out output))
                        {
                            P_Number = output;
                        }
                        else
                        {
                            P_String = result;
                        }


                        if (Parameter_To != null && Parameter_To.ToString().Length > 0)
                        {
                            result = Parameter_To.ToString();
                            recResult = GetRecursiveParameterValue(ctx, ParameterName, result.ToString(), ref result, ds.Tables[0].Rows[row][13].ToString().Equals("Y"));
                            if (!string.IsNullOrEmpty(recResult))
                            {
                                Info_To = Info_To + ", " + recResult;
                            }
                            if (double.TryParse(result, out output))
                            {
                                P_Number_To = output;
                            }
                            else
                            {
                                P_String_To = result;
                            }
                        }

                    }

                    if (P_String != null && P_String.EndsWith(","))
                    {
                        P_String = P_String.Substring(0, P_String.Length - 1);
                    }

                    if (P_String_To != null && P_String_To.EndsWith(","))
                    {
                        P_String_To = P_String_To.Substring(0, P_String_To.Length - 1);
                    }

                    //
                    //-------------------------------------------------------------
                    if (!string.IsNullOrEmpty(P_String.Trim()))
                    {
                        if (string.IsNullOrEmpty(P_String_To.Trim()))
                        {
                            if (P_String.IndexOf("%") != -1)
                            {
                                query.AddRestriction(ParameterName, Query.LIKE,
                                    P_String, Name, Info);
                            }
                            else if (P_String.IndexOf(",") != -1)
                            {
                                //query.AddRestriction(query.GetTableName() + "." + ParameterName + " IN (" + P_String + ")");
                                query.AddRestriction(ParameterName, Query.EQUAL, P_String, Name, Info, query.GetTableName() + "." + ParameterName + " IN (" + P_String + ")");
                            }
                            else
                                query.AddRestriction(ParameterName, Query.EQUAL, P_String, Name, Info);

                        }
                        else
                            query.AddRangeRestriction(ParameterName,
                                P_String, P_String_To, Name, Info, Info_To);
                    }
                    //	Number
                    else if (P_Number != null || P_Number_To != null)
                    {
                        if (P_Number_To == null)
                        {
                            if (isRange)
                                query.AddRestriction(ParameterName, Query.GREATER_EQUAL,
                                    P_Number, Name, Info);
                            else
                                query.AddRestriction(ParameterName, Query.EQUAL,
                                    P_Number, Name, Info);
                        }
                        else	//	P_Number_To != null
                        {
                            if (P_Number == null)
                                query.AddRestriction("TRUNC(" + ParameterName + ",'DD')", Query.LESS_EQUAL,
                                    P_Number_To, Name, Info);
                            else
                                query.AddRangeRestriction(ParameterName,
                                    P_Number, P_Number_To, Name, Info, Info_To);
                        }
                    }
                    //	Date
                    else if (P_Date != null || P_Date_To != null)
                    {

                        if (DisplayType.Date == displayType)
                        {
                            if (P_Date_To == null)
                            {
                                if (isRange)
                                    query.AddRestriction("TRUNC(" + ParameterName + ",'DD')", Query.GREATER_EQUAL,
                                        P_Date, Name, Info);
                                else
                                {
                                    query.AddRestriction("TRUNC(" + ParameterName + ",'DD')", Query.EQUAL,
                                                     P_Date, Name, Info);

                                    query.AddDummyRestriction("TRUNC(" + ParameterName + ",'DD')", Query.EQUAL,
                                                   P_Date, Name, Info);
                                }
                            }
                            else	//	P_Date_To != null
                            {
                                if (P_Date == null)
                                    query.AddRestriction("TRUNC(" + ParameterName + ",'DD')", Query.LESS_EQUAL,
                                        P_Date_To, Name, Info_To);
                                else
                                    query.AddRangeRestriction("TRUNC(" + ParameterName + ",'DD')",
                                        P_Date, P_Date_To, Name, Info, Info_To);
                            }
                        }
                        else
                        {
                            if (P_Date_To == null)
                            {
                                if (isRange)
                                    query.AddRestriction("TRUNC(" + ParameterName + ",'DD')", Query.GREATER_EQUAL,
                                        P_Date, Name, Info);
                                else
                                {
                                    string dt2 = DB.TO_DATE(P_Date.Value.AddHours(24), false); ;
                                    string dt1 = DB.TO_DATE(P_Date.Value, false);

                                    query.AddRestriction(" " + tableName + "." + ParameterName + " BETWEEN " + dt1 + " AND " + dt2 + "");

                                    query.AddDummyRestriction("TRUNC(" + ParameterName + ",'DD')", Query.EQUAL,
                                        P_Date, Name, Info);
                                    //query.AddRestriction("TRUNC(" + ParameterName + ",'DD')", Query.EQUAL,
                                    //  P_Date, Name, Info);
                                }
                            }
                            else	//	P_Date_To != null
                            {
                                if (P_Date == null)
                                    query.AddRestriction("TRUNC(" + ParameterName + ",'DD')", Query.LESS_EQUAL,
                                        P_Date_To, Name, Info_To);
                                else
                                    query.AddRangeRestriction("TRUNC(" + ParameterName + ",'DD')",
                                        P_Date, P_Date_To, Name, Info, Info_To);
                            }
                        }
                    }
                }
                ds.Dispose();
                ds = null;
            }
            catch (Exception e2)
            {
                s_log.Log(Level.SEVERE, sql, e2);
            }
            return query;
        }


        internal static string GetTreeWhereClause(Ctx _ctx, string columnName, int _PA_Hierarchy_ID, int value)
        {
            return VAModelAD.Reflection.GetTypes.GetTreeWhereClause(_ctx, columnName, _PA_Hierarchy_ID, value);
        }

        /// <summary>
        /// function will accept columnName and Ids selected. Will Fetch information from Default tree hierarchy and get child records accordingly.
        /// </summary>
        /// <param name="_ctx"></param>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns>Name of orgs separated bY commas, and IDS in Reference Object</returns>
        private static string GetRecursiveParameterValue(Ctx _ctx, string columnName, string value, ref string result, bool ShowChildOfSelected)
        {

            string tableName = columnName.Substring(0, columnName.Length - 3);

            string[] values = value.Split(',');
            String eSql = "";
            string result1 = "";
            StringBuilder finalResult = new StringBuilder();
            StringBuilder nonSummaryResult = new StringBuilder();
            if (values.Length > 0)
            {

                // Get Default Heirarchy
                string sqla = @"SELECT PA_HIERARCHY_id FROM PA_Hierarchy WHERE ISACTIVE ='Y' 
                       ORDER BY ISDEFAULT DESC ,PA_HIERARCHY_id ASC";
                sqla = MRole.GetDefault(_ctx).AddAccessSQL(sqla, "PA_Hierarchy", true, true);
                object ID = DB.ExecuteScalar(sqla);
                int _PA_Hierarchy_ID = 0;
                if (ID != null && ID != DBNull.Value)
                {
                    _PA_Hierarchy_ID = Util.GetValueOfInt(ID);
                }
                Language _language = Language.GetLanguage(_ctx.GetAD_Language());

                //Get Query to fetch identifier value from table based on column selected. it will be used to display identifires on for parameter in report.
                eSql = VLookUpFactory.GetLookup_TableDirEmbed(_language, columnName, columnName.Substring(0, columnName.Length - 3));

                for (int i = 0; i < values.Length; i++)
                {
                    if (!string.IsNullOrEmpty(values[i]))
                    {
                        //try
                        //{
                        string sqlCheckSummary = "SELECT IsSummary FROM " + tableName + " WHERE " + columnName + "=" + values[i];
                        object val = DB.ExecuteScalar(sqlCheckSummary);
                        if (val != null && val != DBNull.Value)
                        {

                            if (val.ToString().Equals("N"))     // If non-summary is selected then add it string and continue to next ID
                            {
                                if (nonSummaryResult.Length > 0)
                                {
                                    nonSummaryResult.Append(", " + values[i]);
                                }
                                else
                                {
                                    nonSummaryResult.Append(values[i]);
                                }
                                continue;
                            }
                        }
                        //}
                        //catch
                        //{
                        //    result = "";
                        //    continue;
                        //}

                        // Fetch child records from tree hierarchy based on ID selected.
                        result1 = Query.GetTreeWhereClause(_ctx, columnName, _PA_Hierarchy_ID, Convert.ToInt32(values[i]));


                        if (result1.IndexOf("(") > -1)
                        {
                            result1 = result1.Substring(result1.IndexOf("(") + 1);
                            result1 = result1.Substring(0, result1.IndexOf(")"));
                        }
                        else
                        {
                            result1 = result1.Substring(result1.IndexOf("=") + 1);
                        }

                        //create list of sleected IDs in stringbuilder
                        if (result1 != values[i] && result1.Length > 0)
                        {
                            if (finalResult.Length > 0)
                            {
                                finalResult.Append(", " + result1);
                            }
                            else
                            {
                                finalResult.Append(result1);
                            }
                        }
                    }
                }
            }




            StringBuilder identifiedsName = new StringBuilder();
            if (finalResult.Length > 0)
            {
                if (finalResult.ToString().IndexOf(",") > -1)
                {
                    eSql = eSql + " AND " + columnName + " IN (" + finalResult.ToString() + ")";
                }
                else
                {
                    eSql = eSql + " AND " + columnName + " = " + finalResult.ToString();
                }
                //eSql = eSql + " AND " + result1;


                //if (!string.IsNullOrEmpty(finalResult.ToString()))
                //{
                //    result = result + "," + finalResult.ToString();
                //}

                DataSet dsIdeintifiers = DB.ExecuteDataset(eSql);



                if (ShowChildOfSelected && (dsIdeintifiers != null && dsIdeintifiers.Tables[0].Rows.Count > 0))
                {
                    for (int s = 0; s < dsIdeintifiers.Tables[0].Rows.Count; s++)
                    {
                        if (identifiedsName.Length > 0)
                        {
                            identifiedsName.Append(", ");
                        }
                        identifiedsName.Append(dsIdeintifiers.Tables[0].Rows[s][0]);
                    }
                }

            }
            if (nonSummaryResult.Length > 0 || finalResult.Length > 0)
            {
                if (nonSummaryResult.Length > 0)
                {
                    result = nonSummaryResult.ToString();
                }

                if (finalResult.Length > 0)
                {
                    if (result.Length > 0)
                    {
                        result += ", ";
                    }
                    result += finalResult.ToString();
                }

            }



            if (identifiedsName != null)
            {
                return identifiedsName.ToString();
            }

            return "";
        }



        /// <summary>
        /// Derive Zoom Table Name from column name.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static String GetZoomTableName(String columnName)
        {
            String tableName = GetZoomColumnName(columnName);
            int index = tableName.LastIndexOf("_ID");
            if (index != -1)
                return tableName.Substring(0, index);
            return tableName;
        }

        /// <summary>
        ///Get Zoom Column Name.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static String GetZoomColumnName(String columnName)
        {
            if (columnName == null)
                return null;
            if (columnName.Equals("SalesRep_ID"))
                return "AD_User_ID";
            if (columnName.Equals("C_DocTypeTarget_ID"))
                return "C_DocType_ID";
            if (columnName.Equals("Bill_BPartner_ID"))
                return "C_BPartner_ID";
            if (columnName.Equals("Bill_Location_ID"))
                return "C_BPartner_Location_ID";
            if (columnName.Equals("Account_ID"))
                return "C_ElementValue_ID";
            //	See also MTab.validateQuery
            //
            return columnName;
        }
        /// <summary>
        ///Add Query Restriction
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="Operator"></param>
        /// <param name="Code"></param>
        /// <param name="InfoName"></param>
        /// <param name="InfoDisplay"></param>
        public void AddRestriction(String columnName, String Operator,
            Object code, String infoName, String infoDisplay)
        {
            QueryRestriction r = new QueryRestriction(columnName, Operator,
                code, infoName, infoDisplay);
            if (!(code.ToString().StartsWith("-")))
            {
                _list.Add(r);
                _dummyList.Add(r);
            }
        }


        /// <summary>
        ///Add Query Restriction
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="Operator"></param>
        /// <param name="Code"></param>
        /// <param name="InfoName"></param>
        /// <param name="InfoDisplay"></param>
        public void AddRestriction(String columnName, String Operator,
            Object code, String infoName, String infoDisplay, string DirectSql)
        {
            QueryRestriction r = new QueryRestriction(columnName, code,
                null, infoName, infoDisplay, null, Operator, DirectSql, true);
            if (!(code.ToString().StartsWith("-")))
            {
                _list.Add(r);
                _dummyList.Add(r);
            }
        }


        /// <summary>
        ///Add Query Restriction
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="Operator"></param>
        /// <param name="Code"></param>
        public void AddRestriction(String columnName, String Operator,
            Object code)
        {
            QueryRestriction r = new QueryRestriction(columnName, Operator,
                code, null, null);
            _list.Add(r);
            _dummyList.Add(r);
        }

        /// <summary>
        ///Add Restriction
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="Operator"></param>
        /// <param name="Code"></param>
        public void AddRestriction(String columnName, String Operator, int code)
        {
            QueryRestriction r = new QueryRestriction(columnName, Operator,
                code, null, null);
            _list.Add(r);
            _dummyList.Add(r);
        }

        /// <summary>
        ///Add Query Restriction
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="Operator"></param>
        /// <param name="Code"></param>
        /// <param name="InfoName"></param>
        /// <param name="InfoDisplay"></param>
        public void AddDummyRestriction(String columnName, String Operator,
            Object code, String infoName, String infoDisplay)
        {
            QueryRestriction r = new QueryRestriction(columnName, Operator,
                code, infoName, infoDisplay);
            if (!(code.ToString().StartsWith("-")))
            {
                //_list.Add(r);
                _dummyList.Add(r);
            }
        }
        /// <summary>
        ///Add Query Restriction
        /// </summary>
        /// <param name="r"></param>
        public void AddRestriction(QueryRestriction r)
        {
            _list.Add(r);
            _dummyList.Add(r);

        }

        public void SetTableName(string tableName)
        {
            _tableName = tableName;
        }

        public void SetRecordCount(int count)
        {
            _recordCount = count;
        }

        /// <summary>
        /// Set ColumnName of index
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="ColumnName">new column name</param>
        public void SetColumnName(int index, String ColumnName)
        {
            if (index < 0 || index >= _list.Count)
                return;
            QueryRestriction r = (QueryRestriction)_list[index];
            r.ColumnName = ColumnName;
        }

        public Object GetCode(int index)
        {
            if (index < 0 || index >= _list.Count)
                return null;
            QueryRestriction r = _list[index];
            return r.Code;
        }	//	getCode

        /// <summary>
        /// Get printable Query Info
        /// </summary>
        /// <returns></returns>
        public String GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            if (_tableName != null)
                sb.Append(_tableName).Append(": ");
            //
            for (int i = 0; i < _list.Count; i++)
            {
                QueryRestriction r = _list[i];
                if (i != 0)
                    sb.Append(r.AndCondition ? " AND " : " OR ");
                //
                sb.Append(r.GetInfoName())
                    .Append(r.GetInfoOperator())
                    .Append(r.GetInfoDisplayAll());
            }
            return sb.ToString();
        }	//	getInfo




        /// <summary>
        ///Add QueryRestriction
        /// </summary>
        /// <param name="whereClause"></param>
        public void AddRestriction(string whereClause)
        {
            if (whereClause == null || whereClause.Trim().Length == 0)
                return;
            QueryRestriction r = new QueryRestriction(whereClause);
            _list.Add(r);
            //_dummyList.Add(r);
            _newRecord = whereClause.Equals(NEWRECORD);
        }

        /// <summary>
        /// Restrictions are active
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return _list.Count != 0;
        }

        /// <summary>
        ///	New Record Query
        /// </summary>
        /// <returns></returns>
        public bool IsNewRecordQuery()
        {
            return _newRecord;
        }	//	isNewRecord



        public int GetRecordCount()
        {
            return _recordCount;
        }

        public string GetWhereClause()
        {
            return GetWhereClause(false);
        }	//	getWhereClause


        public String GetWhereClause(int index)
        {
            StringBuilder sb = new StringBuilder();
            if (index >= 0 && index < _list.Count)
            {
                QueryRestriction r = _list[index];
                sb.Append(r.GetSQL(null));
            }
            return sb.ToString();
        }	//	getWhereClause


        /// <summary>
        ///Create the resulting Query WHERE Clause
        /// </summary>
        /// <param name="fullyQualified">fullyQualified fully qualified Table.ColumnName</param>
        /// <returns></returns>
        public string GetWhereClause(bool fullyQualified)
        {

            bool qualified = fullyQualified;
            if (qualified && (_tableName == null || _tableName.Length == 0))
                qualified = false;
            //
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _list.Count; i++)
            {
                QueryRestriction r = _list[i];
                if (i != 0)
                {
                    sb.Append(r.AndCondition ? " AND " : " OR ");
                    //sb.Append("");
                }
                if (qualified)
                    sb.Append(r.GetSQL(_tableName));
                else
                    sb.Append(r.GetSQL(null));
            }
            return sb.ToString();
        }

        /// <summary>
        ///Get Query Restriction Count
        /// </summary>
        /// <returns></returns>
        public int GetRestrictionCount()
        {
            return _list.Count;
        }	//	getRestrictionCount

        /// <summary>
        /// Create No Record query
        /// </summary>
        /// <param name="tableName">tableName table name</param>
        /// <param name="newRecord">newRecord new Record Indicator (2=3)</param>
        /// <returns>query</returns>
        public static Query GetNoRecordQuery(string tableName, bool newRecord)
        {
            Query query = new Query(tableName);
            if (newRecord)
            {
                query.AddRestriction(NEWRECORD);
            }
            else
            {
                query.AddRestriction("1=2");
            }
            query.SetRecordCount(0);
            return query;
        }

        /// <summary>
        /// Add Range Restriction (BETWEEN)
        /// </summary>
        /// <param name="columnName">ColumnName</param>
        /// <param name="Code">Code, e.g 0, All%</param>
        /// <param name="codeTo">Code, e.g 0, All%</param>
        /// <param name="infoName">Display Name</param>
        /// <param name="infoDisplay">Display of Code (Lookup)</param>
        /// <param name="infoDisplay_to">Display of Code (Lookup)</param>
        public void AddRangeRestriction(string columnName, object code, object codeTo, string infoName,
            string infoDisplay, string infoDisplay_to)
        {
            QueryRestriction r = new QueryRestriction(columnName, code, codeTo, infoName,
                infoDisplay, infoDisplay_to);
            _list.Add(r);
            _dummyList.Add(r);
        }

        /// <summary>
        ///  Add Range Restriction (BETWEEN)
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="code"></param>
        /// <param name="Code_to"></param>
        public void AddRangeRestriction(String ColumnName, Object code, Object Code_to)
        {
            QueryRestriction r = new QueryRestriction(ColumnName, code, Code_to, null, null, null);
            _list.Add(r);
            _dummyList.Add(r);
        }	//	addRangeRestriction

        /// <summary>
        /// Get ColumnName of index
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>ColumnName</returns>
        public string GetColumnName(int index)
        {
            if (index < 0 || index >= _list.Count)
                return null;
            QueryRestriction r = _list[index];
            return r.ColumnName;
        }
        /// <summary>
        /// Get Table name
        /// </summary>
        /// <returns></returns>
        public String GetTableName()
        {
            return _tableName;
        }

        /// <summary>
        /// Get Operator of index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public String GetOperator(int index)
        {
            if (index < 0 || index >= _list.Count)
                return null;
            QueryRestriction r = _list[index];
            return r.Operator;
        }

        /// <summary>
        /// Get Operator of index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Object GetCodeTo(int index)
        {
            if (index < 0 || index >= _list.Count)
                return null;
            QueryRestriction r = _list[index];
            return r.Code_to;
        }

        /// <summary>
        /// Get QueryRestriction Display of index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public String GetInfoDisplay(int index)
        {
            if (index < 0 || index >= _list.Count)
                return null;
            QueryRestriction r = _list[index];
            return r.InfoDisplay;
        }

        /// <summary>
        ///Get TO QueryRestriction Display of index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public String GetInfoDisplay_to(int index)
        {
            if (index < 0 || index >= _list.Count)
                return null;
            QueryRestriction r = _list[index];
            return r.InfoDisplay_to;
        }

        /// <summary>
        ///Get Info Name
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public String GetInfoName(int index)
        {
            if (index < 0 || index >= _list.Count)
                return null;
            QueryRestriction r = _list[index];
            return r.InfoName;
        }

        /// <summary>
        /// Get Info Operator
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public String GetInfoOperator(int index)
        {
            if (index < 0 || index >= _list.Count)
                return null;
            QueryRestriction r = _list[index];
            return r.GetInfoOperator();
        }	//	getInfoOperator

        /// <summary>
        ///Get Display with optional To
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public String GetInfoDisplayAll(int index)
        {
            if (index < 0 || index >= _list.Count)
                return null;
            QueryRestriction r = _list[index];
            try
            {
                if (r.Code.GetType().FullName == "System.DateTime")
                {
                    if (r.InfoDisplay.IndexOf("T") > 0)
                    {
                        r.InfoDisplay = r.InfoDisplay.Substring(0, r.InfoDisplay.IndexOf("T"));
                        r.InfoDisplay_to = r.InfoDisplay_to.Substring(0, r.InfoDisplay_to.IndexOf("T"));
                    }
                }
            }
            catch { }
            return r.GetInfoDisplayAll();
        }

        /// <summary>
        ///String representation
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            if (IsActive())
                return GetWhereClause(true);
            return "MQuery[" + _tableName + ",QueryRestrictions=0]";
        }	//	

        /// <summary>
        ///Get Display Name
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public String GetDisplayName(Ctx ctx)
        {
            String keyColumn = null;
            if (_tableName != null)
                keyColumn = _tableName + "_ID";
            else
                keyColumn = GetColumnName(0);
            String retValue = Utility.Msg.Translate(ctx, keyColumn);
            if (retValue != null && retValue.Length > 0)
                return retValue;
            return _tableName;
        }

        /// <summary>
        ///Clone Query
        /// </summary>
        /// <returns></returns>
        public Query DeepCopy()
        {
            Query newQuery = new Query(_tableName);
            for (int i = 0; i < _list.Count; i++)
                newQuery.AddRestriction(_list[i]);
            return newQuery;
        }	//	clo


        public QueryRestriction GetDummyQuery(int index)
        {
            if (_dummyList.Count >= index + 1)
            {
                return _dummyList[index];
            }
            return null;
        }
    }

    /*************************************************************************

       


/****************************************************************************/

    // QueryRestriction class

    /****************************************************************************/
    public class QueryRestriction
    {
        #region "Declaration"
        /**	Direct Where Clause	*/
        public string DirectWhereClause = null;
        /**	Column Name			*/
        public string ColumnName;
        /** Name				*/
        public string InfoName;
        /** Operator			*/
        public string Operator;
        /** sql Where Code		*/
        public object Code;
        /** Info				*/
        public string InfoDisplay;
        /** sql Where Code To	*/
        public object Code_to;
        /** Info To				*/
        public string InfoDisplay_to;
        /** And/Or Condition	*/
        public bool AndCondition = true;
        #endregion

        /// <summary>
        /// Restriction
        /// </summary>
        /// <param name="columnName">ColumnName</param>
        /// <param name="oprator">Operator, e.g. = != ..</param>
        /// <param name="code">Code e.g 0, All%</param>
        /// <param name="infoName">Display Name</param>
        /// <param name="infoDisplay">Display of Code (Lookup)</param>
        public QueryRestriction(string columnName, string oprator,
                     object code, string infoName, string infoDisplay)
        {

            ColumnName = columnName.Trim();
            if (infoName != null)
                InfoName = infoName;
            else
                InfoName = ColumnName;
            //
            Operator = oprator;

            //	Boolean
            if (code != null && code is bool)
                Code = (((bool)code) == true) ? "Y" : "N";
            else if (code != null && code.GetType() == typeof(KeyNamePair))
                Code = ((KeyNamePair)code).GetKey();
            else if (code != null && code.GetType() == typeof(ValueNamePair))
                Code = ((ValueNamePair)code).GetValue();
            else
                Code = code;
            ///	clean code
            if (Code != null && Code is string)
            {
                if (Code.ToString().StartsWith("'"))
                    Code = Code.ToString().Substring(1);
                if (Code.ToString().EndsWith("'"))
                    Code = Code.ToString().Substring(0, Code.ToString().Length - 2);
            }

            if (infoDisplay != null)
                InfoDisplay = infoDisplay.Trim();
            else if (code != null)
                InfoDisplay = code.ToString();
        }	//	QueryRestriction

        /// <summary>
        /// Range Restriction (BETWEEN)
        /// </summary>
        /// <param name="columnName">ColumnName</param>
        /// <param name="code">Code e.g 0, All%</param>
        /// <param name="code_to">Code, e.g 0, All%</param>
        /// <param name="infoName">Display Name</param>
        /// <param name="infoDisplay">Display of Code (Lookup)</param>
        /// <param name="infoDisplay_to">Display of Code (Lookup)</param>
        public QueryRestriction(string columnName, object code, object code_to, string infoName,
            string infoDisplay, string infoDisplay_to)
            : this(columnName, Query.BETWEEN, code, infoName, infoDisplay)
        {
            //	Code_to
            Code_to = code_to;
            if (Code_to is string)
            {
                if (Code_to.ToString().StartsWith("'"))
                    Code_to = Code_to.ToString().Substring(1);
                if (Code_to.ToString().EndsWith("'"))
                    Code_to = Code_to.ToString().Substring(0, Code_to.ToString().Length - 2);
            }
            //	InfoDisplay_to
            if (infoDisplay_to != null)
                InfoDisplay_to = infoDisplay_to.Trim();
            else if (Code_to != null)
                InfoDisplay_to = Code_to.ToString();
        }

        /// <summary>
        ///Create Restriction with dircet WHERE clause
        /// </summary>
        /// <param name="whereClause"></param>
        public QueryRestriction(String whereClause)
        {
            DirectWhereClause = whereClause;
        }

        /// <summary>
        /// Copy Constructor - Internal Use
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="code"></param>
        /// <param name="code_to"></param>
        /// <param name="infoName"></param>
        /// <param name="infoDisplay"></param>
        /// <param name="infoDisplay_to"></param>
        /// <param name="ooperator"></param>
        /// <param name="directWhereClause"></param>
        /// <param name="andCondition"></param>
        public QueryRestriction(String columnName, Object code, Object code_to,
            String infoName, String infoDisplay, String infoDisplay_to,
            String ooperator, String directWhereClause, bool andCondition)
        {
            ColumnName = columnName;
            InfoName = infoName;
            Code = code;
            Code_to = code_to;
            InfoName = infoName;
            InfoDisplay = infoDisplay;
            InfoDisplay_to = infoDisplay_to;
            Operator = ooperator;
            DirectWhereClause = directWhereClause;
            AndCondition = andCondition;
        }

        /// <summary>
        /// Get Info Name
        /// </summary>
        /// <returns></returns>
        public String GetInfoName()
        {
            return InfoName;
        }	//	getInfoName


        /// <summary>
        /// Return sql construct for this restriction
        /// </summary>
        /// <param name="tableName">optional table name</param>
        /// <returns>sql WHERE construct</returns>
        public string GetSQL(string tableName)
        {
            if (DirectWhereClause != null)
                return DirectWhereClause;
            //
            StringBuilder sb = new StringBuilder();

            // opening parenthesis for case insensitive search
            //if (Code instanceof String)
            if (Code is string)
            {
                sb.Append(" UPPER( ");
            }
            if (tableName != null && tableName.Length > 0)
            {
                //	Assumes - REPLACE(INITCAP(variable),'s','X') or UPPER(variable)
                int pos = ColumnName.LastIndexOf('(') + 1;	//	including (
                int end = ColumnName.IndexOf(')');
                //	We have a Function in the ColumnName
                if (pos != -1 && end != -1)
                    sb.Append(ColumnName.Substring(0, pos))
                        .Append(tableName).Append(".").Append(ColumnName.Substring(pos, (end - pos)))
                        .Append(ColumnName.Substring(end));
                else
                    sb.Append(tableName).Append(".").Append(ColumnName);
            }
            else
                sb.Append(ColumnName);

            // closing parenthesis for case insensitive search
            if (Code is string)
                sb.Append(" ) ");


            //	NULL Operator
            if (Code == null || "NULL".Equals(Code.ToString().ToUpper()) || Null.NULLString.Equals(Code.ToString()))
            {
                if (Operator.Equals(Query.EQUAL))
                    sb.Append(" IS NULL ");
                else
                    sb.Append(" IS NOT NULL ");
            }
            else
            {
                sb.Append(Operator);
                if (Query.IN.Equals(Operator) || Query.NOT_IN.Equals(Operator))
                    sb.Append("(");

                if (typeof(string) == Code.GetType())
                {
                    sb.Append(" UPPER( ");
                    sb.Append(DataBase.DB.TO_STRING(Code.ToString()));
                    //sb.Append("'" + Code.ToString() + "'");
                    sb.Append(" ) ");
                }
                else if (typeof(DateTime) == Code.GetType())
                {
                    sb.Append(DataBase.DB.TO_DATE((DateTime)Code, true));
                    //sb.Append(Convert.ToDateTime((TimeSpan)Code));
                }
                else
                    sb.Append(Code);

                //	Between
                if (Query.BETWEEN.Equals(Operator))
                {
                    //	if (Code_to != null && InfoDisplay_to != null)
                    sb.Append(" AND ");
                    if (typeof(string) == Code_to.GetType())
                    {
                        sb.Append(DataBase.DB.TO_STRING(Code_to.ToString()));
                        //sb.Append("'" + Code_to.ToString() + "'");
                    }
                    else if (typeof(DateTime) == Code_to.GetType())
                    {
                        sb.Append(DataBase.DB.TO_DATE((DateTime)Code_to, true));
                        //sb.Append(DataBase.DB.TO_DATE((TimeSpan)Code_to));
                        //sb.Append(Convert.ToDateTime((DateTime)Code_to));
                    }
                    else
                        sb.Append(Code_to);
                }
                else if (Query.IN.Equals(Operator) || Query.NOT_IN.Equals(Operator))
                    sb.Append(")");
            }
            return sb.ToString();
        }

        /// <summary>
        ///Get String Representation
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return GetSQL(null);   //jz will it be used to generate update set clause???
        }

        /// <summary>
        /// Get Info Operator
        /// </summary>
        /// <returns></returns>
        public String GetInfoOperator()
        {
            for (int i = 0; i < Query.OPERATORS.Length; i++)
            {
                if (Query.OPERATORS[i].GetValue().Equals(Operator))
                    return Query.OPERATORS[i].GetName();
            }
            return Operator;
        }

        /// <summary>
        ///	Get Display with optional To Hello
        /// </summary>
        /// <returns></returns>
        public String GetInfoDisplayAll()
        {
            if (InfoDisplay_to == null)
                return InfoDisplay;
            StringBuilder sb = new StringBuilder(InfoDisplay);
            sb.Append(" - ").Append(InfoDisplay_to);
            return sb.ToString();
        }
    }
}