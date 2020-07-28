/********************************************************
 * Module Name    :     Report
 * Purpose        :     Generate Reports
 * Author         :     Jagmohan Bhatt
 * Date           :     
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VAdvantage.Utility;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.Process;

using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace VAdvantage.Print
{
    /// <summary>
    /// <para>Print Data Structure.</para>
    /// <para>Created by DataEngine</para>
    /// <para>A Structure has rows, wich contain elements.</para>
    /// <para>Elements can be end nodes (PrintDataElements) or data structures (PrintData).</para>
    /// <para>The row data is sparse - i.e. null if not existing.</para>
    /// <para>A Structure has optional meta info about content (PrintDataColumn).</para>
    /// </summary>
    [Serializable]
    public class PrintData
    {
        /// <summary>
        /// Data Parent Constructor 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="name">data element name</param>
        public PrintData(Ctx ctx, String name)
        {
            if (name == null)
                throw new ArgumentException("Name cannot be null");
            _ctx = ctx;
            _name = name;
        }	//	PrintData

        /// <summary>
        /// Data Parent Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="name">data element name</param>
        /// <param name="nodes">ArrayList with nodes (content not checked)</param>
        public PrintData(Ctx ctx, String name, List<Object> nodes)
        {
            if (name == null)
                throw new ArgumentException("Name cannot be null");
            _ctx = ctx;
            _name = name;
            if (nodes != null)
                _nodes = nodes;
        }	//	PrintData


        /**	Ctx						*/
        private Ctx _ctx;
        /**	Data Structure Name			*/
        private String _name;
        /** Data Structure rows			*/
        private List<List<Object>> _rows = new List<List<Object>>();
        /** Current Row Data Structure elements		*/
        private List<Object> _nodes = null;
        /**	Current Row					*/
        private int _row = -1;
        /**	List of Function Rows		*/
        private List<int> _functionRows = new List<int>();


        /**	Table has LevelNo			*/
        private bool _hasLevelNo = false;
        /**	Level Number Indicator		*/
        private const String LEVEL_NO = "LEVELNO";

        /**	Optional Column Meta Data	*/
        private PrintDataColumn[] _columnInfo = null;
        /**	Optional sql				*/
        private String _sql = null;
        /** Optional TableName			*/
        private String _TableName = null;


        /**	XML Element Name			*/
        public const String XML_TAG = "viennaData";
        /**	XML Row Name				*/
        public const String XML_ROW_TAG = "row";
        /**	XML Attribute Name			*/
        public const String XML_ATTRIBUTE_NAME = "name";
        /** XML Attribute Count			*/
        public const String XML_ATTRIBUTE_COUNT = "count";
        /** XML Attribute Number		*/
        public const String XML_ATTRIBUTE_NO = "no";
        /** XML Attribute Function Row	*/
        public const String XML_ATTRIBUTE_FUNCTION_ROW = "function_row";

        /// <summary>
        /// Get current context
        /// </summary>
        /// <returns>curent context</returns>
        public Ctx GetCtx()
        {
            return _ctx;
        }	//	getName

        /// <summary>
        /// Get name
        /// </summary>
        /// <returns>name</returns>
        public String GetName()
        {
            return _name;
        }	//	getName


        /// <summary>
        /// Set optional Column Info
        /// </summary>
        /// <param name="newInfo">Column Info</param>
        public void SetColumnInfo(PrintDataColumn[] newInfo)
        {
            _columnInfo = newInfo;
        }	//	setColumnInfo


        /// <summary>
        /// Get optional Column Info
        /// </summary>
        /// <returns>Column Info</returns>
        public PrintDataColumn[] GetColumnInfo()
        {
            return _columnInfo;
        }	//	getColumnInfo


        /// <summary>
        /// Set SQL (optional)
        /// </summary>
        /// <param name="sql">SQL</param>
        public void SetSQL(String sql)
        {
            _sql = sql;
        }	//	setSQL


        /// <summary>
        /// Get Optional SQL
        /// </summary>
        /// <returns>SQL</returns>
        public String GetSQL()
        {
            return _sql;
        }	//	getSQL

        /// <summary>
        /// Set table name
        /// </summary>
        /// <param name="TableName">name</param>
        public void SetTableName(String TableName)
        {
            _TableName = TableName;
        }	//	setTableName

        /// <summary>
        /// Gets the table name
        /// </summary>
        /// <returns>table name</returns>
        public String GetTableName()
        {
            return _TableName;
        }	//	getTableName


        /// <summary>
        /// Returns true if no Nodes in row
        /// </summary>
        /// <returns>true if no Nodes in row</returns>
        public bool IsEmpty()
        {
            if (_nodes == null)
                return true;
            return _nodes.Count == 0;
        }	//	isEmpty


        /// <summary>
        /// Return Number of nodes in row 
        /// </summary>
        /// <returns>number of nodes in row</returns>
        public int GetNodeCount()
        {
            if (_nodes == null)
                return 0;
            return _nodes.Count;
        }	//	getNodeCount

        /// <summary>
        /// Add row
        /// </summary>
        /// <param name="functionRow">true if function row</param>
        /// <param name="levelNo">Line detail Level Number 0=Normal</param>
        public void AddRow(bool functionRow, int levelNo)
        {
            _nodes = new List<Object>();
            _row = _rows.Count;
            _rows.Add(_nodes);
            if (functionRow)
                _functionRows.Add(_row);
            if (_hasLevelNo && levelNo != 0)
                AddNode(new PrintDataElement(LEVEL_NO, levelNo, DisplayType.Integer));
        }	//	addRow


        /// <summary>
        /// Set Row Index
        /// </summary>
        /// <param name="row">row index</param>
        /// <returns>true if success</returns>
        public bool SetRowIndex(int row)
        {
            if (row < 0 || row >= _rows.Count())
                return false;
            _row = row;
            _nodes = (List<Object>)_rows[_row];
            return true;
        }

        /// <summary>
        /// Set Row Index to next
        /// </summary>
        /// <returns>true if success</returns>
        public bool SetRowNext()
        {
            return SetRowIndex(_row + 1);
        }	//	setRowNext


        /// <summary>
        /// Get row count
        /// </summary>
        /// <returns>row count</returns>
        public int GetRowCount()
        {
            return _rows.Count;
        }	//	getRowCount

        /// <summary>
        /// Get row index
        /// </summary>
        /// <returns>row index</returns>
        public int GetRowIndex()
        {
            return _row;
        }	//	getRowIndex

        /// <summary>
        /// Is the Row a Function Row
        /// </summary>
        /// <param name="row">row no</param>
        /// <returns>true if function row</returns>
        public bool IsFunctionRow(int row)
        {
            return _functionRows.Contains(row);
        }	//	isFunctionRow

        /// <summary>
        /// Is the Row a Function Row
        /// </summary>
        /// <returns>true if function row</returns>
        public bool IsFunctionRow()
        {
            return _functionRows.Contains(_row);
        }	//	isFunctionRow

        /// <summary>
        /// Is the current Row a Function Row
        /// </summary>
        /// <returns>true if function row</returns>
        public bool IsPageBreak()
        {
            //	page break requires function and meta data
            if (IsFunctionRow() && _nodes != null)
            {
                for (int i = 0; i < _nodes.Count; i++)
                {
                    Object o = _nodes[i];
                    if (o.GetType() == typeof(PrintDataElement))
                    {
                        PrintDataElement pde = (PrintDataElement)o;
                        if (pde.IsPageBreak())
                            return true;
                    }
                }
            }
            return false;
        }	//	isPageBreak

        /// <summary>
        /// PrintData has Level No
        /// </summary>
        /// <param name="hasLevelNo">true if sql contains LevelNo</param>
        public void SetHasLevelNo(bool hasLevelNo)
        {
            _hasLevelNo = hasLevelNo;
        }	//	hasLevelNo

        /// <summary>
        /// PrintData has Level No
        /// </summary>
        /// <returns>true if sql contains LevelNo</returns>
        public bool HasLevelNo()
        {
            return _hasLevelNo;
        }	//	hasLevelNo


        /// <summary>
        /// Get Line Level Number for current row
        /// </summary>
        /// <returns>line level no 0 = default</returns>
        public int GetLineLevelNo()
        {
            if (_nodes == null || !_hasLevelNo)
                return 0;

            for (int i = 0; i < _nodes.Count; i++)
            {
                Object o = _nodes[i];
                if (o.GetType() == typeof(PrintDataElement))
                {
                    PrintDataElement pde = (PrintDataElement)o;
                    if (LEVEL_NO.Equals(pde.GetColumnName()))
                    {
                        int ii = (int)pde.GetValue();
                        return ii;
                    }
                }
            }
            return 0;
        }	//	getLineLevel


        /// <summary>
        /// Add Parent node to Data Structure row
        /// </summary>
        /// <param name="parent">parent</param>
        public void AddNode(PrintData parent)
        {
            if (parent == null)
                throw new ArgumentException("Parent cannot be null");
            if (_nodes == null)
                AddRow(false, 0);
            _nodes.Add(parent);
        }	//	addNode


        /// <summary>
        /// Add node to Data Structure row
        /// </summary>
        /// <param name="node">node</param>
        public void AddNode(PrintDataElement node)
        {
            if (node == null)
                throw new ArgumentException("Node cannot be null");
            if (_nodes == null)
                AddRow(false, 0);
            _nodes.Add(node);
        }	//	addNode


        /// <summary>
        /// Get Node with index in row
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>PrintData(Element) of index or null</returns>
        public Object GetNode(int index)
        {
            if (_nodes == null || index < 0 || index >= _nodes.Count)
                return null;
            return _nodes[index];
        }	//	getNode


        /// <summary>
        /// Get Node with index in row
        /// </summary>
        /// <returns>PrintData(Element) of index or null</returns>
        public Object GetNode(String name)
        {
            int index = GetIndex(name);
            if (index < 0)
                return null;
            return _nodes[index];
        }	//	getNode



        /// <summary>
        /// Get Node with index in row
        /// </summary>
        /// <param name="AD_Column_ID">Column ID</param>
        /// <param name="fls">Always false : just to overload the other function</param>
        /// <returns>PrintData(Element) of index or null</returns>
        public Object GetNode(int AD_Column_ID, bool fls)
        {
            int index = GetIndex(AD_Column_ID);
            if (index < 0)
                return null;
            return _nodes[index];
        }	//	getNode

        /// <summary>
        /// Get Primary Key in row
        /// </summary>
        /// <returns>PK or null</returns>
        public PrintDataElement GetPKey()
        {
            if (_nodes == null)
                return null;
            for (int i = 0; i < _nodes.Count; i++)
            {
                Object o = _nodes[i];
                if (o.GetType() == typeof(PrintDataElement))
                {
                    PrintDataElement pde = (PrintDataElement)o;
                    if (pde.IsPKey())
                        return pde;
                }
            }
            return null;
        }	//	getPKey


        /// <summary>
        /// Get Index of Node in Structure (not recursing) row
        /// </summary>
        /// <param name="columnName">name</param>
        /// <returns>index or -1</returns>
        public int GetIndex(String columnName)
        {
            if (_nodes == null)
                return -1;
            for (int i = 0; i < _nodes.Count; i++)
            {
                Object o = _nodes[i];
                if (o.GetType() == typeof(PrintDataElement))
                {
                    if (columnName.Equals(((PrintDataElement)o).GetColumnName()))
                        return i;
                }
                else if (o.GetType() == typeof(PrintData))
                {
                    if (columnName.Equals(((PrintData)o).GetName()))
                        return i;
                }
            }
            //	As Data is stored sparse, there might be lots of NULL values
            //	log.log(Level.SEVERE, "PrintData.getIndex - Element not found - " + name);
            return -1;
        }	//	getIndex


        /// <summary>
        /// Get Index of Node in Structure (not recursing) row
        /// </summary>
        /// <param name="AD_Column_ID">AD_Column_ID</param>
        /// <returns>index or -1</returns>
        public int GetIndex(int AD_Column_ID)
        {
            if (_columnInfo == null)
                return -1;
            for (int i = 0; i < _columnInfo.Length; i++)
            {
                if (_columnInfo[i].GetAD_Column_ID() == AD_Column_ID)
                    return GetIndex(_columnInfo[i].GetColumnName());
            }
            //	OK for virtual Columns with TableDirect, Search
            MColumn col = MColumn.Get(GetCtx(), AD_Column_ID);
            if (col != null && col.IsVirtualColumn())
                return -1;		//	not found, but OK		

            return -1;
        }	//	getIndex



        /// <summary>
        /// Dump All Data
        /// </summary>
        public void Dump()
        {
            Dump(this);
        }	//	dump

        /// <summary>
        /// Dump All Data
        /// </summary>
        public void DumpHeader()
        {
            DumpHeader(this);
        }	//	dumpHeader


        /// <summary>
        /// Dump All Data
        /// </summary>
        public void DumpCurrentRow()
        {
            DumpRow(this, _row);
        }	//	dump


        /// <summary>
        /// Dump all PrintData - header and rows
        /// </summary>
        /// <param name="pd">Print Data</param>
        private static void Dump(PrintData pd)
        {
            DumpHeader(pd);
            for (int i = 0; i < pd.GetRowCount(); i++)
                DumpRow(pd, i);
        }	//	dump


        /// <summary>
        /// Dump PrintData Header
        /// </summary>
        /// <param name="pd">Print Data</param>
        private static void DumpHeader(PrintData pd)
        {
            if (pd.GetColumnInfo() != null)
            {
                for (int i = 0; i < pd.GetColumnInfo().Length; i++)
                {
                    //log.config(i + ": " + pd.getColumnInfo()[i]);
                }
            }
        }	//	dump


        /// <summary>
        /// Dump Row
        /// </summary>
        /// <param name="pd">Print Data</param>
        /// <param name="row">row</param>
        private static void DumpRow(PrintData pd, int row)
        {
            if (row < 0 || row >= pd.GetRowCount())
            {
                return;
            }
            pd.SetRowIndex(row);
            if (pd.GetNodeCount() == 0)
            {
                return;
            }
            for (int i = 0; i < pd.GetNodeCount(); i++)
            {
                Object obj = pd.GetNode(i);
                if (obj == null)
                { }
                else if (obj.GetType() == typeof(PrintData))
                {
                    Dump((PrintData)obj);
                }
                else if (obj.GetType() == typeof(PrintDataElement))
                {
                }
            }
        }	//	dumpRow
    }
}
