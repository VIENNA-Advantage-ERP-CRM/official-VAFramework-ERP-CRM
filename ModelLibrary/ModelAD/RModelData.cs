/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : RModelData
 * Purpose        : Report Model Data - ValueObject.
 *                  - Build SQL from RColumn info and Retrieve Data
 *                  - owned by RModel
 * Class Used     : None
 * Chronological    Development
 * Raghunandan      18-Dec-2009
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

namespace VAdvantage.Model
{
    public class RModelData
    {
        // note: replace  ArrayList

        //The Rows                        
        public List<List<Object>> rows = new List<List<Object>>();

        //The temporary Rows              
        private List<List<Object>> _rows = new List<List<Object>>();

        //The Row MetaData   
        public List<Object> rowsMeta = new List<Object>();
        // The Column Definitions         
        public List<RColumn> cols = new List<RColumn>();

        //Table Name                
        private String _TableName;

        //Functions (Integer - String)    
        //public HashMap<Integer,String>	functions = new HashMap<Integer,String>();
        public Dictionary<int, String> functions = new Dictionary<int, String>();// HashMap<Integer,String>();
        // Groups (Integer)                
        public List<int> groups = new List<int>();

        // Array with row numbers that are groups  
        private List<int> _groupRows = new List<int>();// ArrayList<Integer>();
        private List<bool> _groupRowsIndicator = null;

        //Constant 1                      
        private static Decimal ONE = new Decimal(1.0);

        //Logger			
        private static VLogger log = VLogger.GetVLogger(typeof(RModelData).FullName);

        /// <summary>
        /// Constructor. Use query method to populate data
        /// </summary>
        /// <param name="TableName">TableName</param>
        public RModelData(String TableName)
        {
            _TableName = TableName;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            rows.Clear();
            _rows.Clear();
            rowsMeta.Clear();
            cols.Clear();
        }


        /// <summary>
        /// Query
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="whereClause">the SQL where clause (w/o the WHERE)</param>
        /// <param name="orderClause"></param>
        public void Query(Ctx ctx, String whereClause, String orderClause)
        {
            Query(ctx, whereClause, orderClause, 0);
        }
        /// <summary>
        /// Query
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="whereClause">the SQL where clause (w/o the WHERE)</param>
        /// <param name="orderClause"></param>
        /// <param name="pageNo"></param>
        public void Query(Ctx ctx, String whereClause, String orderClause, int pageNo = 0)
        {
            RColumn rc = null;
            //  Create SQL
            StringBuilder sql = new StringBuilder("SELECT ");
            int size = cols.Count;
            for (int i = 0; i < size; i++)
            {
                rc = (RColumn)cols[i];//.get(i);
                if (i > 0)
                {
                    sql.Append(",");
                }
                sql.Append(rc.GetColSQL());
            }
            sql.Append(" FROM ").Append(_TableName).Append(" ").Append(RModel.TABLE_ALIAS);
            if (whereClause != null && whereClause.Length > 0)
            {
                sql.Append(" WHERE ").Append(whereClause);
            }
            String finalSQL = MRole.GetDefault(ctx, false).AddAccessSQL(
                sql.ToString(), RModel.TABLE_ALIAS, MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            if (orderClause != null && orderClause.Length > 0)
            {
                finalSQL += " ORDER BY " + orderClause;
            }
            log.Fine(finalSQL);

            //  FillData
            int index = 0;      //  rowset index
            _rows.Clear();
            //IDataReader idr = null;
            try
            {
                //Statement stmt = DataBase.createStatement();
                //ResultSet rs = stmt.executeQuery(finalSQL);
                //idr = DataBase.DB.ExecuteReader(finalSQL, null, null);
                DataSet ds = DB.GetDatabase().ExecuteDatasetPaging(finalSQL, pageNo, 50, 0);
                #region  Commented
                //while (idr.Read())
                //{
                //    List<Object> row = new List<Object>(size);
                //    //index = 1;
                //    index = 0;
                //    //  Columns
                //    for (int i = 0; i < size; i++)
                //    {
                //        rc = (RColumn)cols[i];
                //        //  Get ID
                //        if (rc.IsIDcol())
                //        {
                //            //row.add(new KeyNamePair(rs.getInt(index++), rs.getString(index++)));
                //            row.Add(new KeyNamePair(Utility.Util.GetValueOfInt(idr[index++]), Utility.Util.GetValueOfString(idr[index++])));
                //        }
                //        //  Null check
                //        else if (idr[index] == null)
                //        {
                //            index++;
                //            row.Add(null);
                //        }
                //        else if (rc.GetColClass() == typeof(String))
                //        {
                //            row.Add(Utility.Util.GetValueOfString(idr[index++]));
                //        }
                //        else if (rc.GetColClass() == typeof(Decimal))
                //        {
                //            row.Add(Utility.Util.GetValueOfDecimal(idr[index++]));
                //        }
                //        else if (rc.GetColClass() == typeof(Double))
                //        {
                //            row.Add(Utility.Util.GetValueOfDouble(idr[index++]));
                //        }
                //        else if (rc.GetColClass() == typeof(int))
                //        {
                //            row.Add(Utility.Util.GetValueOfInt(idr[index++]));
                //        }
                //        else if (rc.GetColClass() == typeof(DateTime))
                //        {
                //            row.Add(Utility.Util.GetValueOfDateTime(idr[index++]));
                //        }
                //        else if (rc.GetColClass() == typeof(Boolean))
                //        {
                //            row.Add(Utility.Util.GetValueOfBool("Y".Equals(idr[index++])));
                //        }
                //        else    //  should not happen
                //        {
                //            row.Add(Utility.Util.GetValueOfString(idr[index++]));
                //        }
                //    }
                //    _rows.Add(row);
                //}
                //idr.Close();
                #endregion
                //used for instead of While and used DataSet instead of DataReader
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    List<Object> row = new List<Object>(size);
                    //index = 1;
                    index = 0;
                    //  Columns
                    for (int i = 0; i < size; i++)
                    {
                        rc = (RColumn)cols[i];
                        //  Get ID
                        if (rc.IsIDcol())
                        {
                            //row.add(new KeyNamePair(rs.getInt(index++), rs.getString(index++)));
                            row.Add(new KeyNamePair(Utility.Util.GetValueOfInt(ds.Tables[0].Rows[j][index++]), Utility.Util.GetValueOfString(ds.Tables[0].Rows[j][index++])));
                        }
                        //  Null check
                        else if (ds.Tables[0].Rows[j][index] == null)
                        {
                            index++;
                            row.Add(null);
                        }
                        else if (rc.GetColClass() == typeof(String))
                        {
                            row.Add(Utility.Util.GetValueOfString(ds.Tables[0].Rows[j][index++]));
                        }
                        else if (rc.GetColClass() == typeof(Decimal))
                        {
                            row.Add(Utility.Util.GetValueOfDecimal(ds.Tables[0].Rows[j][index++]));
                        }
                        else if (rc.GetColClass() == typeof(Double))
                        {
                            row.Add(Utility.Util.GetValueOfDouble(ds.Tables[0].Rows[j][index++]));
                        }
                        else if (rc.GetColClass() == typeof(int))
                        {
                            row.Add(Utility.Util.GetValueOfInt(ds.Tables[0].Rows[j][index++]));
                        }
                        else if (rc.GetColClass() == typeof(DateTime))
                        {
                            row.Add(Utility.Util.GetValueOfDateTime(ds.Tables[0].Rows[j][index++]));
                        }
                        else if (rc.GetColClass() == typeof(Boolean))
                        {
                            row.Add(Utility.Util.GetValueOfBool("Y".Equals(ds.Tables[0].Rows[j][index++])));
                        }
                        else    //  should not happen
                        {
                            row.Add(Utility.Util.GetValueOfString(ds.Tables[0].Rows[j][index++]));
                        }
                    }
                    _rows.Add(row);
                }

            }
            catch (Exception e)
            {
                //if (idr != null)
                //{
                //    idr.Close();
                //    idr = null;
                //}
                if (index == 0)
                {
                    log.Log(Level.SEVERE, finalSQL, e);
                }
                else
                {
                    log.Log(Level.SEVERE, "Index=" + index + "," + rc, e);
                }
                //e.printStackTrace();
            }
            Process();
        }

        /// <summary>
        /// Process Data
        /// Copy data in _rows to rows and perform functions
        /// </summary>
        private void Process()
        {
            log.Fine("Start Rows=" + _rows.Count);

            //  Row level Funcions
            //  would come here

            //  Group by Values
            int gSize = groups.Count;
            int[] groupBys = new int[gSize];
            Object[] groupBysValue = new Object[gSize];
            Object INITVALUE = new Object();
            for (int i = 0; i < gSize; i++)
            {
                //groupBys[i] = ((Integer)groups.get(i)).intValue();
                groupBys[i] = Utility.Util.GetValueOfInt(groups[i]);
                groupBysValue[i] = INITVALUE;
                log.Fine("GroupBy level=" + i + " col=" + groupBys[i]);
            }
            //  Add additional row to force group change
            if (gSize > 0)
            {
                List<Object> newRow = new List<Object>();
                for (int c = 0; c < cols.Count; c++)
                {
                    newRow.Add("");
                }
                _rows.Add(newRow);
            }

            //  Function Values - Function - GroupValue
            int fSize = functions.Count;
            int[] funcCols = new int[fSize];
            String[] funcFuns = new String[fSize];
            int index = 0;
            //Iterator it = functions.keySet().iterator();
            IEnumerator it = functions.Keys.GetEnumerator();
            while (it.MoveNext())
            {
                Object key = it.Current;
                funcCols[index] = Utility.Util.GetValueOfInt(((int)key));//.intValue();
                //funcFuns[index] = functions[key].ToString();
                funcFuns[index] = Convert.ToString(this.functions[(int)key]);

                log.Fine("Function " + funcFuns[index] + " col=" + funcCols[index]);
                index++;
            }
            //MessageBox.Show("Check Two dimentinal array");
            //Decimal[][] funcVals = new Decimal[fSize][gSize+1];
            Decimal[,] funcVals = new Decimal[fSize, gSize + 1];//try to use Jagged array[fSize][];
            int totalIndex = gSize;  //  place for overall total
            log.Fine("FunctionValues = [ " + fSize + " * " + (gSize + 1) + " ]");
            for (int f = 0; f < fSize; f++)
            {
                for (int g = 0; g < gSize + 1; g++)
                {
                    //funcVals[f][g] = Env.ZERO;
                    funcVals[f, g] = Env.ZERO;
                }
            }

            rows.Clear();
            //  Copy _rows into rows
            for (int r = 0; r < _rows.Count; r++)
            {
                List<Object> row = (List<Object>)_rows[r];
                //  do we have a group break
                bool[] haveBreak = new bool[groupBys.Length];
                for (int level = 0; level < groupBys.Length; level++)
                {
                    int idx = groupBys[level];
                    if (groupBysValue[level] == INITVALUE)
                    {
                        haveBreak[level] = false;
                    }
                    else if (!groupBysValue[level].Equals(row[idx]))
                    {
                        haveBreak[level] = true;
                    }
                    else
                    {
                        haveBreak[level] = false;
                    }
                    //  previous level had a break
                    if (level > 0 && haveBreak[level - 1])
                    {
                        haveBreak[level] = true;
                    }
                }
                //  create group levels - reverse order
                for (int level = groupBys.Length - 1; level >= 0; level--)
                {
                    int idx = groupBys[level];
                    if (groupBysValue[level] == INITVALUE)
                    {
                        groupBysValue[level] = row[idx];
                    }
                    else if (haveBreak[level])
                    {
                        //	log.fine( "GroupBy Change level=" + level + " col=" + idx + " - " + groupBysValue[level]);
                        //  create new row
                        List<Object> newRow = new List<Object>();
                        for (int c = 0; c < cols.Count; c++)
                        {
                            if (c == idx)   //  the group column
                            {
                                // only Try catch block work
                                try
                                {
                                    if (groupBysValue[c] == null || groupBysValue[c].ToString().Length == 0)
                                    {
                                        newRow.Add("=");
                                    }
                                    else
                                    {
                                        newRow.Add(groupBysValue[c]);
                                    }
                                }
                                catch (Exception)
                                {

                                    continue;
                                }

                            }
                            else
                            {
                                bool found = false;
                                for (int fc = 0; fc < funcCols.Length; fc++)
                                {
                                    if (c == funcCols[fc])
                                    {
                                        //	newRow.add("fc= " + fc + " gl=" + level + " " + funcFuns[fc]);
                                        newRow.Add(funcVals[fc, level]);
                                        funcVals[fc, level] = Env.ZERO;
                                        found = true;
                                    }
                                }
                                if (!found)
                                {
                                    newRow.Add(null);
                                }
                            }
                        }   //  for all columns
                        //
                        _groupRows.Add(rows.Count); //  group row indicator
                        rows.Add(newRow);
                        groupBysValue[level] = row[idx];
                    }
                }   //  for all groups

                //	functions
                for (int fc = 0; fc < funcCols.Length; fc++)
                {
                    int col = funcCols[fc];
                    //  convert value to big decimal
                    Object value = row[col];
                    Decimal bd = Env.ZERO;
                    if (value == null)
                    {
                        ;
                    }
                    else if (value is Decimal)
                    {
                        bd = (Decimal)value;
                    }
                    else
                    {
                        try
                        {
                            bd = Utility.Util.GetValueOfDecimal(value);
                        }
                        catch { }
                    }

                    for (int level = 0; level < gSize + 1; level++)
                    {
                        if (funcFuns[fc].Equals(RModel.FUNCTION_SUM))
                        {
                            //funcVals[fc][level] = funcVals[fc][level].add(bd);
                            //add value in two dimesional array
                            try
                            {
                                funcVals[fc, level] += bd;
                            }
                            catch { }
                        }
                        else if (funcFuns[fc].Equals(RModel.FUNCTION_COUNT))
                        {
                            //funcVals[fc][level] = funcVals[fc][level].add(ONE);
                            funcVals[fc, level] = ONE;
                        }
                    }   //  for all group levels
                }   //  for all functions

                rows.Add(row);
            }   //  for all _rows

            //  total row
            if (functions.Count > 0)
            {
                List<Object> newRow = new List<Object>();
                for (int c = 0; c < cols.Count; c++)
                {
                    bool found = false;
                    for (int fc = 0; fc < funcCols.Length; fc++)
                    {
                        if (c == funcCols[fc])
                        {
                            newRow.Add(funcVals[fc, totalIndex]);
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        newRow.Add(null);
                    }
                }   //  for all columns
                //  remove empty row added earlier to force group change
                if (gSize > 0)
                {
                    //MessageBox.Show("rows.Remove(rows.Count - 1)");
                    //rows.Remove(rows.Count - 1);
                }
                _groupRows.Add(rows.Count); //  group row indicator
                rows.Add(newRow);
            }
            log.Fine("End Rows=" + rows.Count);
            _rows.Clear();
        }

        /// <summary>
        /// Is Row a Group Row
        /// </summary>
        /// <param name="row">row index</param>
        /// <returns>true, if group row</returns>
        public bool IsGroupRow(int row)
        {
            //  build bool Array
            if (_groupRowsIndicator == null)
            {
                _groupRowsIndicator = new List<Boolean>(rows.Count);
                for (int r = 0; r < rows.Count; r++)
                {
                    _groupRowsIndicator.Add(Utility.Util.GetValueOfBool(_groupRows.Contains(r)));
                }
            }
            if (row < 0 || row >= _groupRowsIndicator.Count)
            {
                return false;
            }
            //return ((Boolean)_groupRowsIndicator.get(row)).booleanValue();
            return (Utility.Util.GetValueOfBool(_groupRowsIndicator[row]));//.booleanValue();
        }

        /// <summary>
        /// Move Row
        /// </summary>
        /// <param name="from">index</param>
        /// <param name="to">to index</param>
        public void MoveRow(int from, int to)
        {
            if (from < 0 || to >= rows.Count)
            {
                throw new ArgumentException("Row from invalid");
            }
            if (to < 0 || to >= rows.Count)
            {
                throw new ArgumentException("Row to invalid");
            }

            //  Move Data
            //ArrayList<Object> temp = rows.get(from);
            List<Object> temp = (List<Object>)rows[from];
            MessageBox.Show("rows.Remove(from)");
            //rows.Remove(from);
            rows.Insert(to, temp);
            //  Move Description indicator >>> _groupRows is not in sync after row move !!
            if (_groupRowsIndicator != null)
            {
                Boolean tempB = _groupRowsIndicator[from];
                _groupRowsIndicator.RemoveAt(from);//Remove(from);
                _groupRowsIndicator.Insert(to, tempB);//Add(to, tempB);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public String GetTableName()
        {
            return _TableName;
        }
    }
    //public class RModelData
    //{
    //    // note: replace  ArrayList

    //    //The Rows                        
    //    //public List<List<Object>> rows = new List<List<Object>>();
    //    public List<Object> rows = new List<Object>();
    //    //The temporary Rows              
    //    //private List<List<Object>> _rows = new List<List<Object>>();
    //    private List<Object> _rows = new List<Object>();
    //    //The Row MetaData   
    //    public List<Object> rowsMeta = new List<Object>();
    //    // The Column Definitions         
    //    public List<RColumn> cols = new List<RColumn>();

    //    //Table Name                
    //    private String _TableName;

    //    //Functions (Integer - String)    
    //    //public HashMap<Integer,String>	functions = new HashMap<Integer,String>();
    //    public Dictionary<int, String> functions = new Dictionary<int, String>();// HashMap<Integer,String>();
    //    // Groups (Integer)                
    //    public List<int> groups = new List<int>();

    //    // Array with row numbers that are groups  
    //    private List<int> _groupRows = new List<int>();// ArrayList<Integer>();
    //    private List<bool> _groupRowsIndicator = null;

    //    //Constant 1                      
    //    private static Decimal ONE = new Decimal(1.0);

    //    //Logger			
    //    private static VLogger log = VLogger.GetVLogger(typeof(RModelData).FullName);

    //    /// <summary>
    //    /// Constructor. Use query method to populate data
    //    /// </summary>
    //    /// <param name="TableName">TableName</param>
    //    public RModelData(String TableName)
    //    {
    //        _TableName = TableName;
    //    }

    //    /// <summary>
    //    /// Dispose
    //    /// </summary>
    //    public void Dispose()
    //    {
    //        rows.Clear();
    //        _rows.Clear();
    //        rowsMeta.Clear();
    //        cols.Clear();
    //    }

    //    /// <summary>
    //    /// Query
    //    /// </summary>
    //    /// <param name="ctx"></param>
    //    /// <param name="whereClause">the SQL where clause (w/o the WHERE)</param>
    //    /// <param name="orderClause"></param>
    //    public void Query(Ctx ctx, String whereClause, String orderClause)
    //    {
    //        RColumn rc = null;
    //        //  Create SQL
    //        StringBuilder sql = new StringBuilder("SELECT ");
    //        int size = cols.Count;
    //        for (int i = 0; i < size; i++)
    //        {
    //            rc = (RColumn)cols[i];//.get(i);
    //            if (i > 0)
    //            {
    //                sql.Append(",");
    //            }
    //            sql.Append(rc.GetColSQL());
    //        }
    //        sql.Append(" FROM ").Append(_TableName).Append(" ").Append(RModel.TABLE_ALIAS);
    //        if (whereClause != null && whereClause.Length > 0)
    //        {
    //            sql.Append(" WHERE ").Append(whereClause);
    //        }
    //        String finalSQL = MRole.GetDefault(ctx, false).AddAccessSQL(
    //            sql.ToString(), RModel.TABLE_ALIAS, MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
    //        if (orderClause != null && orderClause.Length > 0)
    //        {
    //            finalSQL += " ORDER BY " + orderClause;
    //        }
    //        log.Fine(finalSQL);

    //        //  FillData
    //        int index = 0;      //  rowset index
    //        _rows.Clear();
    //        IDataReader idr = null;
    //        try
    //        {
    //            //Statement stmt = DataBase.createStatement();
    //            //ResultSet rs = stmt.executeQuery(finalSQL);
    //            idr = DataBase.DB.ExecuteReader(finalSQL, null, null);
    //            //DataSet ds = DataBase.DB.ExecuteDataset(finalSQL, null, null);
    //            while (idr.Read())
    //            {
    //                List<Object> row = new List<Object>(size);
    //                index = 1;
    //                //  Columns
    //                for (int i = 0; i < size; i++)
    //                {
    //                    rc = (RColumn)cols[i];
    //                    //  Get ID
    //                    if (rc.IsIDcol())
    //                    {
    //                        row.Add(new KeyNamePair(Utility.Util.GetValueOfInt(idr[index++]), Utility.Util.GetValueOfString(idr[index++])));
    //                    }
    //                    //  Null check
    //                    else if (idr[index] == null)
    //                    {
    //                        index++;
    //                        row.Add(null);
    //                    }
    //                    else if (rc.GetColClass() == typeof(String))
    //                    {
    //                        row.Add(Utility.Util.GetValueOfString(idr[index++]));
    //                    }
    //                    else if (rc.GetColClass() == typeof(Decimal))
    //                    {
    //                        row.Add(Utility.Util.GetValueOfDecimal(idr[index++]));
    //                    }
    //                    else if (rc.GetColClass() == typeof(Double))
    //                    {
    //                        row.Add(Utility.Util.GetValueOfDouble(idr[index++]));
    //                    }
    //                    else if (rc.GetColClass() == typeof(int))
    //                    {
    //                        row.Add(Utility.Util.GetValueOfInt(idr[index++]));
    //                    }
    //                    else if (rc.GetColClass() == typeof(DateTime))
    //                    {
    //                        row.Add(Utility.Util.GetValueOfDateTime(idr[index++]));
    //                    }
    //                    else if (rc.GetColClass() == typeof(Boolean))
    //                    {
    //                        row.Add(Utility.Util.GetValueOfBool("Y".Equals(idr[index++])));
    //                    }
    //                    else    //  should not happen
    //                    {
    //                        row.Add(Utility.Util.GetValueOfString(idr[index++]));
    //                    }
    //                }
    //                _rows.Add(row);
    //            }
    //            idr.Close();
    //        }
    //        catch (Exception e)
    //        {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //                idr = null;
    //            }
    //            if (index == 0)
    //            {
    //                log.Log(Level.SEVERE, finalSQL, e);
    //            }
    //            else
    //            {
    //                log.Log(Level.SEVERE, "Index=" + index + "," + rc, e);
    //            }
    //            //e.printStackTrace();
    //        }
    //        Process();
    //    }

    //    /// <summary>
    //    /// Process Data
    //    /// Copy data in _rows to rows and perform functions
    //    /// </summary>
    //    private void Process()
    //    {
    //        log.Fine("Start Rows=" + _rows.Count);

    //        //  Row level Funcions
    //        //  would come here

    //        //  Group by Values
    //        int gSize = groups.Count;
    //        int[] groupBys = new int[gSize];
    //        Object[] groupBysValue = new Object[gSize];
    //        Object INITVALUE = new Object();
    //        for (int i = 0; i < gSize; i++)
    //        {
    //            //groupBys[i] = ((Integer)groups.get(i)).intValue();
    //            groupBys[i] = Utility.Util.GetValueOfInt(groups[i]);
    //            groupBysValue[i] = INITVALUE;
    //            log.Fine("GroupBy level=" + i + " col=" + groupBys[i]);
    //        }
    //        //  Add additional row to force group change
    //        if (gSize > 0)
    //        {
    //            List<Object> newRow = new List<Object>();
    //            for (int c = 0; c < cols.Count; c++)
    //            {
    //                newRow.Add("");
    //            }
    //            _rows.Add(newRow);
    //        }

    //        //  Function Values - Function - GroupValue
    //        int fSize = functions.Count;
    //        int[] funcCols = new int[fSize];
    //        String[] funcFuns = new String[fSize];
    //        int index = 0;
    //        //Iterator it = functions.keySet().iterator();
    //        IEnumerator it = functions.Keys.GetEnumerator();
    //        while (it.MoveNext())
    //        {
    //            Object key = it.Current;
    //            funcCols[index] = Utility.Util.GetValueOfInt(((int)key));//.intValue();
    //            //funcFuns[index] = functions[key].ToString();
    //            funcFuns[index] =Convert.ToString( this.functions[(int)key]);

    //            log.Fine("Function " + funcFuns[index] + " col=" + funcCols[index]);
    //            index++;
    //        }
    //        //MessageBox.Show("Check Two dimentinal array");
    //        //Decimal[][] funcVals = new Decimal[fSize][gSize+1];
    //        Decimal[,] funcVals = new Decimal[fSize, gSize + 1];//try to use Jagged array[fSize][];
    //        int totalIndex = gSize;  //  place for overall total
    //        log.Fine("FunctionValues = [ " + fSize + " * " + (gSize + 1) + " ]");
    //        for (int f = 0; f < fSize; f++)
    //        {
    //            for (int g = 0; g < gSize + 1; g++)
    //            {
    //                //funcVals[f][g] = Env.ZERO;
    //                funcVals[f, g] = Env.ZERO;
    //            }
    //        }

    //        rows.Clear();
    //        //  Copy _rows into rows
    //        for (int r = 0; r < _rows.Count; r++)
    //        {
    //            List<Object> row = (List<Object>)_rows[r];
    //            //  do we have a group break
    //            bool[] haveBreak = new bool[groupBys.Length];
    //            for (int level = 0; level < groupBys.Length; level++)
    //            {
    //                int idx = groupBys[level];
    //                if (groupBysValue[level] == INITVALUE)
    //                {
    //                    haveBreak[level] = false;
    //                }
    //                else if (!groupBysValue[level].Equals(row[idx]))
    //                {
    //                    haveBreak[level] = true;
    //                }
    //                else
    //                {
    //                    haveBreak[level] = false;
    //                }
    //                //  previous level had a break
    //                if (level > 0 && haveBreak[level - 1])
    //                {
    //                    haveBreak[level] = true;
    //                }
    //            }
    //            //  create group levels - reverse order
    //            for (int level = groupBys.Length - 1; level >= 0; level--)
    //            {
    //                int idx = groupBys[level];
    //                if (groupBysValue[level] == INITVALUE)
    //                {
    //                    groupBysValue[level] = row[idx];
    //                }
    //                else if (haveBreak[level])
    //                {
    //                    //	log.fine( "GroupBy Change level=" + level + " col=" + idx + " - " + groupBysValue[level]);
    //                    //  create new row
    //                    List<Object> newRow = new List<Object>();
    //                    for (int c = 0; c < cols.Count; c++)
    //                    {
    //                        if (c == idx)   //  the group column
    //                        {
    //                            if (groupBysValue[c] == null || groupBysValue[c].ToString().Length == 0)
    //                            {
    //                                newRow.Add("=");
    //                            }
    //                            else
    //                            {
    //                                newRow.Add(groupBysValue[c]);
    //                            }
    //                        }
    //                        else
    //                        {
    //                            bool found = false;
    //                            for (int fc = 0; fc < funcCols.Length; fc++)
    //                            {
    //                                if (c == funcCols[fc])
    //                                {
    //                                    //	newRow.add("fc= " + fc + " gl=" + level + " " + funcFuns[fc]);
    //                                    newRow.Add(funcVals[fc, level]);
    //                                    funcVals[fc,level] = Env.ZERO;
    //                                    found = true;
    //                                }
    //                            }
    //                            if (!found)
    //                            {
    //                                newRow.Add(null);
    //                            }
    //                        }
    //                    }   //  for all columns
    //                    //
    //                    _groupRows.Add(rows.Count); //  group row indicator
    //                    rows.Add(newRow);
    //                    groupBysValue[level] = row[idx];
    //                }
    //            }   //  for all groups

    //            //	functions
    //            for (int fc = 0; fc < funcCols.Length; fc++)
    //            {
    //                int col = funcCols[fc];
    //                //  convert value to big decimal
    //                Object value = row[col];
    //                Decimal bd = Env.ZERO;
    //                if (value == null)
    //                {
    //                    ;
    //                }
    //                else if (value is Decimal)
    //                {
    //                    bd = (Decimal)value;
    //                }
    //                else
    //                {
    //                    try
    //                    {
    //                        bd = Utility.Util.GetValueOfDecimal(value);
    //                    }
    //                    catch (Exception e) { }
    //                }

    //                for (int level = 0; level < gSize + 1; level++)
    //                {
    //                    if (funcFuns[fc].Equals(RModel.FUNCTION_SUM))
    //                    {
    //                        //funcVals[fc][level] = funcVals[fc][level].add(bd);
    //                        //add value in two dimesional array
    //                        funcVals[fc, level] = bd;
    //                    }
    //                    else if (funcFuns[fc].Equals(RModel.FUNCTION_COUNT))
    //                    {
    //                        //funcVals[fc][level] = funcVals[fc][level].add(ONE);
    //                        funcVals[fc, level] = ONE;
    //                    }
    //                }   //  for all group levels
    //            }   //  for all functions

    //            rows.Add(row);
    //        }   //  for all _rows

    //        //  total row
    //        if (functions.Count > 0)
    //        {
    //            List<Object> newRow = new List<Object>();
    //            for (int c = 0; c < cols.Count; c++)
    //            {
    //                bool found = false;
    //                for (int fc = 0; fc < funcCols.Length; fc++)
    //                {
    //                    if (c == funcCols[fc])
    //                    {
    //                        newRow.Add(funcVals[fc, totalIndex]);
    //                        found = true;
    //                    }
    //                }
    //                if (!found)
    //                {
    //                    newRow.Add(null);
    //                }
    //            }   //  for all columns
    //            //  remove empty row added earlier to force group change
    //            if (gSize > 0)
    //            {
    //                rows.Remove(rows.Count - 1);
    //            }
    //            _groupRows.Add(rows.Count); //  group row indicator
    //            rows.Add(newRow);
    //        }
    //        log.Fine("End Rows=" + rows.Count);
    //        _rows.Clear();
    //    }


    //    /// <summary>
    //    /// Is Row a Group Row
    //    /// </summary>
    //    /// <param name="row">row index</param>
    //    /// <returns>true, if group row</returns>
    //    public bool IsGroupRow(int row)
    //    {
    //        //  build bool Array
    //        if (_groupRowsIndicator == null)
    //        {
    //            _groupRowsIndicator = new List<Boolean>(rows.Count);
    //            for (int r = 0; r < rows.Count; r++)
    //            {
    //                _groupRowsIndicator.Add(Utility.Util.GetValueOfBool(_groupRows.Contains(r)));
    //            }
    //        }
    //        if (row < 0 || row >= _groupRowsIndicator.Count)
    //        {
    //            return false;
    //        }
    //        //return ((Boolean)_groupRowsIndicator.get(row)).booleanValue();
    //        return (Utility.Util.GetValueOfBool(_groupRowsIndicator[row]));//.booleanValue();
    //    }

    //    /// <summary>
    //    /// Move Row
    //    /// </summary>
    //    /// <param name="from">index</param>
    //    /// <param name="to">to index</param>
    //    public void MoveRow(int from, int to)
    //    {
    //        if (from < 0 || to >= rows.Count)
    //        {
    //            throw new ArgumentException("Row from invalid");
    //        }
    //        if (to < 0 || to >= rows.Count)
    //        {
    //            throw new ArgumentException("Row to invalid");
    //        }

    //        //  Move Data
    //        //ArrayList<Object> temp = rows.get(from);
    //        List<Object> temp = (List<Object>)rows[from];
    //        rows.Remove(from);
    //        rows.Insert(to, temp);
    //        //  Move Description indicator >>> _groupRows is not in sync after row move !!
    //        if (_groupRowsIndicator != null)
    //        {
    //            Boolean tempB = _groupRowsIndicator[from];
    //            _groupRowsIndicator.RemoveAt(from);//Remove(from);
    //            _groupRowsIndicator.Insert(to, tempB);//Add(to, tempB);
    //        }
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <returns></returns>
    //    public String GetTableName()
    //    {
    //        return _TableName;
    //    }
    //}
}
