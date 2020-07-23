/********************************************************
 * Module Name    :     Report
 * Purpose        :     Generate Reports
 * Author         :     Jagmohan Bhatt
 * Date           :     25-JUne-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Print
{
    public class PrintDataColumn
    {
        
        public PrintDataColumn(int AD_Column_ID, String columnName, int displayType, int columnSize, String alias, bool isPageBreak)
        {
            _AD_Column_ID = AD_Column_ID;
            _columnName = columnName;
            //
            _displayType = displayType;
            _columnSize = columnSize;
            //
            _alias = alias;
            if (_alias == null)
                _alias = columnName;
            _pageBreak = isPageBreak;
        }	//	PrintDataColumn

        private int _AD_Column_ID;
        private String _columnName;
        private int _displayType;
        private int _columnSize;
        private String _alias;
        private bool _pageBreak;


        public int GetAD_Column_ID()
        {
            return _AD_Column_ID;
        }	//	getAD_Column_ID


        public String GetColumnName()
        {
            return _columnName;
        }	//	getColumnName

        public int GetDisplayType()
        {
            return _displayType;
        }	//	getDisplayType

        public String GetAlias()
        {
            return _alias;
        }	//	getAlias


        public bool HasAlias()
        {
            return !_columnName.Equals(_alias);
        }	//	hasAlias


        public bool IsPageBreak()
        {
            return _pageBreak;
        }	//	isPageBreak
    }
}
