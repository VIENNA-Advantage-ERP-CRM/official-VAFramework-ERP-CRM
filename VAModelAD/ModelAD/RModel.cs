/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : RModel
 * Purpose        : Report model.
 *                  Data is maintained in RModelData
 * Class Used     : Serializable
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
    [Serializable]
    public class RModel
    {
        #region
        // Table Alias for SQL     
        public const String TABLE_ALIAS = "zz";
        // Function: Count         
        public const String FUNCTION_COUNT = "Count";
        // Function: Sum           
        public const String FUNCTION_SUM = "Sum";
        // Helper to retrieve Actual Data  
        public RModelData _data = null;
        // Is Content editable by user     
        private bool _editable = false;
        //	Logger	
        private static VLogger log = VLogger.GetVLogger(typeof(RModel).FullName);

        #endregion

        /// <summary>
        /// Constructor. Creates RModelData
        /// </summary>
        /// <param name="TableName">TableName</param>
        public RModel(String TableName)
        {
            _data = new RModelData(TableName);
        }

        /// <summary>
        /// Get Column Display Name
        /// </summary>
        /// <param name="col">col</param>
        /// <returns>RColumn</returns>
        public RColumn GetRColumn(int col)
        {
            if (col < 0 || col > _data.cols.Count)
            {
                throw new java.lang.IllegalArgumentException("Column invalid");
            }
            return (RColumn)_data.cols[col];//.get(col);
        }

        /// <summary>
        ///  Add Column
        /// </summary>
        /// <param name="rc">rc</param>
        public void AddColumn(RColumn rc)
        {
            _data.cols.Add(rc);
        }

        /// <summary>
        /// Add Column at Index
        /// </summary>
        /// <param name="rc">rc</param>
        /// <param name="index">index</param>
        public void AddColumn(RColumn rc, int index)
        {
            //_data.cols.Add(index, rc);
            _data.cols.Insert(index, rc);
        }

        /// <summary>
        /// Add Row
        /// </summary>
        public void AddRow()
        {
            _data.rows.Add(new List<Object>());
            _data.rowsMeta.Add(null);
        }

        /// <summary>
        /// Add Row at Index
        /// </summary>
        /// <param name="index">index</param>
        public void AddRow(int index)
        {
            //_data.rows.Add(index, new List<Object>());
            _data.rows.Insert(index, new List<Object>());
            //_data.rowsMeta.Add(index, null);
            _data.rowsMeta.Insert(index, null);
        }

        /// <summary>
        /// Add Row
        /// </summary>
        /// <param name="l">l</param>
        public void AddRow(List<Object> l)
        {
            _data.rows.Add(l);
            _data.rowsMeta.Add(null);
        }

        /// <summary>
        /// Add Row at Index
        /// </summary>
        /// <param name="l">l</param>
        /// <param name="index">index</param>
        public void AddRow(List<Object> l, int index)
        {
            //_data.rows.Add(index, l);
            //_data.rowsMeta.Add(index, null);
            _data.rows.Insert(index, l);
            _data.rowsMeta.Insert(index, null);
        }

        /// <summary>
        /// Find index for ColumnName
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns>index or -1 if not found</returns>
        public int GetColumnIndex(String columnName)
        {
            if (columnName == null || columnName.Length == 0)
            {
                return -1;
            }
            //
            for (int i = 0; i < _data.cols.Count; i++)
            {
                RColumn rc = (RColumn)_data.cols[i];
                //	log.fine( "Column " + i + " " + rc.getColSQL() + " ? " + columnName);
                if (rc.GetColSQL().StartsWith(columnName))
                {
                    log.Fine("Column " + i + " " + rc.GetColSQL() + " = " + columnName);
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Query
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="whereClause"></param>
        /// <param name="orderClause"></param>
        public void Query(Ctx ctx, String whereClause, String orderClause)
        {
            _data.Query(ctx, whereClause, orderClause);
        }

        /// <summary>
        /// Set a group column
        /// </summary>
        /// <param name="columnName">columnName</param>
        public void SetGroup(String columnName)
        {
            SetGroup(GetColumnIndex(columnName));
        }

        /// <summary>
        /// Set a group column -
        /// if the group value changes, a new row in inserted
        /// performing the calculations set in setGroupFunction().
        /// If you set multiple groups, start with the highest level
        /// (e.g. Country, Region, City)
        /// The data is assumed to be sorted to result in meaningful groups.
        /// <pre>
        /// A AA 1
        /// A AB 2
        /// B BA 3
        /// B BB 4
        /// after setGroup (0)
        /// A AA 1
        /// A AB 2
        /// A
        /// B BA 3
        /// B BB 4
        /// B
        /// </pre>
        /// </summary>
        /// <param name="col">The Group BY Column</param>
        public void SetGroup(int col)
        {
            log.Config("RModel.setGroup col=" + col);
            if (col < 0 || col >= _data.cols.Count)
            {
                return;
            }
            int ii = col;
            if (!_data.groups.Contains(ii))
            {
                _data.groups.Add(ii);
            }
        }

        /// <summary>
        /// Is Row a Group Row
        /// </summary>
        /// <param name="row">index</param>
        /// <returns>true if row is a group row</returns>
        public bool IsGroupRow(int row)
        {
            return _data.IsGroupRow(row);
        }

        /// <summary>
        /// Set Group Function
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="function"></param>
        public void SetFunction(String columnName, String function)
        {
            SetFunction(GetColumnIndex(columnName), function);
        }

        /// <summary>
        /// Set Group Function -
        /// for the column, set a function like FUNCTION_SUM, FUNCTION_COUNT, ...
        /// </summary>
        /// <param name="col">The column on which the function is performed</param>
        /// <param name="function">The function</param>
        public void SetFunction(int col, String function)
        {
            log.Config("RModel.setFunction col=" + col + " - " + function);
            if (col < 0 || col >= _data.cols.Count)
            {
                return;
            }
            _data.functions.Add(col, function);
        }

        //  TableModel interface

        /// <summary>
        /// Return Total mumber of rows
        /// </summary>
        /// <returns>row count</returns>
        public int GetRowCount()
        {
            return _data.rows.Count;
        }

        /// <summary>
        /// Get total number of columns
        /// </summary>
        /// <returns>count</returns>
        public int GetColumnCount()
        {
            return _data.cols.Count;
        }

        /// <summary>
        /// Get Column Display Name
        /// </summary>
        /// <param name="col">index</param>
        /// <returns>ColumnName</returns>
        public String GetColumnName(int col)
        {
            if (col < 0 || col > _data.cols.Count)
            {
                throw new java.lang.IllegalArgumentException("Column invalid");
            }
            RColumn rc = (RColumn)_data.cols[col];
            if (rc != null)
            {
                return rc.GetColHeader();
            }
            return null;
        }

        /// <summary>
        /// Get Column Class
        /// </summary>
        /// <param name="col">index</param>
        /// <returns>ColumnName</returns>
        public Type GetColumnClass(int col)//public Class<?> getColumnClass (int col)
        {
            if (col < 0 || col > _data.cols.Count)
            {
                throw new java.lang.IllegalArgumentException("Column invalid");
            }
            RColumn rc = (RColumn)_data.cols[col];
            if (rc != null)
            {
                return rc.GetColClass();
            }
            return null;
        }

        /// <summary>
        /// Is Cell Editable
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <returns>true, if editable</returns>
        public bool IsCellEditable(int rowIndex, int columnIndex)
        {
            return _editable;
        }

        /// <summary>
        /// Get Value At
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns>value</returns>
        public Object GetValueAt(int row, int col)
        {
            //  invalid row
            if (row < 0 || row >= _data.rows.Count)
            {
                return null;
            }
            //		throw new java.lang.IllegalArgumentException("Row invalid");
            if (col < 0 || col >= _data.cols.Count)
            {
                return null;
            }
            //		throw new java.lang.IllegalArgumentException("Column invalid");
            //

            //MessageBox.Show("check myRow");
            List<Object> myRow = (List<Object>)_data.rows[row];// (ArrayList<?>)//check it
            //  invalid column
            if (myRow == null || col >= myRow.Count)
            {
                return null;
            }
            //  setValue
            return myRow[col];
        }

        /// <summary>
        /// Set Value At
        /// </summary>
        /// <param name="aValue"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void SetValueAt(Object aValue, int row, int col)
        {
            //  invalid row
            if (row < 0 || row >= _data.rows.Count)
            {
                throw new ArgumentException("Row invalid");
            }
            if (col < 0 || col >= _data.cols.Count)
            {
                throw new ArgumentException("Column invalid");
            }
            if (!IsCellEditable(row, col))
            {
                throw new ArgumentException("Cell is read only");
            }
            //ArrayList<Object> myRow = _data.rows.get(row);
            List<Object> myRow = (List<Object>)_data.rows[row];
            //  invalid row
            if (myRow == null)
            {
                throw new java.lang.IllegalArgumentException("Row not initialized");
            }
            //  not enough columns - add nulls
            if (col >= myRow.Count)
            {
                while (myRow.Count < _data.cols.Count)
                {
                    myRow.Add(null);
                }
            }
            //  setValue
            //myRow.set(col, aValue);
            myRow.Insert(col, aValue);
        }

        /// <summary>
        /// Move Row
        /// </summary>
        /// <param name="from">index</param>
        /// <param name="to">to index</param>
        public void MoveRow(int from, int to)
        {
            _data.MoveRow(from, to);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public String GetTableName()
        {
            return _data.GetTableName();
        }

    }
}
