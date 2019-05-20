using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VAdvantage.DataBase;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Common;
using System.Threading;
using VAdvantage.Model;
using VAdvantage.Process;
namespace VAdvantage.Classes
{


    /// <summary>
    /// Info Column Details
    /// </summary>
    public class Info_Column
    {

        /// <summary>
        /// Create Info Column (r/o and not color column)
        /// </summary>
        /// <param name="colHeader">Column Header</param>
        /// <param name="colSQL">SQL select code for column</param>
        /// <param name="colClass">class of column - determines display</param>
        public Info_Column(String colHeader, String colSQL, Type colClass)
            : this(colHeader, colSQL, colClass, true, false, null)
        {
        }   //  Info_Column


        /// <summary>
        /// Create Info Column (r/o and not color column)
        /// </summary>
        /// <param name="colHeader">Column Header</param>
        /// <param name="colSQL">SQL select code for column</param>
        /// <param name="colClass">class of column - determines display</param>
        /// <param name="IDcolSQL">SQL select for the ID of the for the displayed column (KeyNamePair)</param>
        public Info_Column(String colHeader, String colSQL, Type colClass, String IDcolSQL)
            : this(colHeader, colSQL, colClass, true, false, IDcolSQL)
        {

        }   //  Info_Column


        /// <summary>
        /// Create Info Column
        /// </summary>
        /// <param name="colHeader">Column Header</param>
        /// <param name="colSQL">SQL select code for column</param>
        /// <param name="colClass">class of column - determines display</param>
        /// <param name="readOnly">column is read only</param>
        /// <param name="colorColumn">if true, value of column determines foreground color</param>
        /// <param name="IDcolSQL">SQL select for the ID of the for the displayed column</param>
        public Info_Column(String colHeader, String colSQL, Type colClass,
            bool readOnly, bool colorColumn, String IDcolSQL)
        {
            SetColHeader(colHeader);
            SetColSQL(colSQL);
            SetColClass(colClass);
            SetReadOnly(readOnly);
            SetColorColumn(colorColumn);
            SetIDcolSQL(IDcolSQL);
        }   //  Info_Column


        private String _colHeader;
        private String _colSQL;
        private Type _colClass;
        private bool _readOnly;
        private bool _colorColumn;
        private String _IDcolSQL = "";
        private int _sequence = 0;
        private int? _width = null;

        /// <summary>
        /// Set Sequence
        /// </summary>
        /// <param name="sequence">sequence no</param>
        /// <returns>this</returns>
        public Info_Column Seq(int sequence)
        {
            _sequence = sequence;
            return this;
        }	//	seq


        /// <summary>
        /// Get Sequene
        /// </summary>
        /// <returns>sequence</returns>
        public int GetSequence()
        {
            return _sequence;
        }	//	getSequence

        /// <summary>
        /// CompareTo
        /// </summary>
        /// <param name="oo">other</param>
        /// <returns></returns>
        public int CompareTo(Info_Column oo)
        {
            Int32 ii = _sequence;
            return ii.CompareTo(oo.GetSequence());
        }	//	compareTo

        public Type GetColClass()
        {
            return _colClass;
        }

        public String GetColHeader()
        {
            return _colHeader;
        }
        public String GetColSQL()
        {
            return _colSQL;
        }
        public bool IsReadOnly()
        {
            return _readOnly;
        }

        public void SetColClass(Type colClass)
        {
            _colClass = colClass;
        }

        /// <summary>
        /// Add ID column SQL for the displayed column
        /// The Class for this should be KeyNamePair
        /// </summary>
        /// <param name="colHeader">column header</param>
        public void SetColHeader(String colHeader)
        {
            _colHeader = colHeader;
            if (colHeader != null)
            {
                int index = colHeader.IndexOf('&');
                if (index != -1)
                    _colHeader = colHeader.Substring(0, index) + colHeader.Substring(index + 1);
            }
        }

        public void SetColSQL(String colSQL)
        {
            _colSQL = colSQL;
        }

        public void SetReadOnly(bool readOnly)
        {
            _readOnly = readOnly;
        }

        public void SetColorColumn(bool colorColumn)
        {
            _colorColumn = colorColumn;
        }

        public bool IsColorColumn()
        {
            return _colorColumn;
        }
        /**
         *  Add ID column SQL for the displayed column
         *  The Class for this should be KeyNamePair
         */
        public void SetIDcolSQL(String IDcolSQL)
        {
            _IDcolSQL = IDcolSQL;
            if (_IDcolSQL == null)
                _IDcolSQL = "";
        }
        public String GetIDcolSQL()
        {
            return _IDcolSQL;
        }
        public bool IsIDcol()
        {
            return _IDcolSQL.Length > 0;
        }

        /**
         * 	Set Width in pixels
         *	@param width width
         */
        public void SetWidth(int width)
        {
            _width = width;
        }

        /**
         * 	Get Optional Width in pixels
         *	@return pixels or null
         */
        public int? GetWidth()
        {
            return _width;
        }
    }
}