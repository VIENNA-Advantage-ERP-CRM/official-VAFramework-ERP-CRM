/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : RColumn
 * Purpose        : Report Column
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
using VAdvantage.Login;

namespace VAdvantage.Model
{
    public class RColumn
    {
        #region PrivateVariable
        // Column Header        
        private String _colHeader;
        //Column SQL            
        private String _colSQL;
        //Column Display Class  
        private Type _colClass;
        // Display Type         
        private int _displayType = 0;
        // Column Size im px    
        private int _colSize = 30;
        private bool _readOnly = true;
        private bool _colorColumn = false;
        private bool _isIDcol = false;
        #endregion


        /// <summary>
        /// Create Report Column
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="columnName"></param>
        /// <param name="displayType"></param>
        public RColumn(Ctx ctx, String columnName, int displayType)
            : this(ctx, columnName, displayType, null, 0, null)
        {
            
        }

        /// <summary>
        /// Create Report Column
        /// </summary>
        /// <param name="ctx">context </param>
        /// <param name="columnName">column name</param>
        /// <param name="displayType">display type</param>
        /// <param name="AD_Reference_Value_ID">List/Table Reference</param>
        public RColumn(Ctx ctx, String columnName, int displayType, int AD_Reference_Value_ID)
            : this(ctx, columnName, displayType, null, AD_Reference_Value_ID, null)
        {
            
        }

        /// <summary>
        /// Create Report Column
        /// </summary>
        /// <param name="ctx">context </param>
        /// <param name="columnName">column name</param>
        /// <param name="displayType">display type</param>
        /// <param name="sql">sql (if null then columnName is used).
        /// Will be overwritten if TableDir or Search  </param>
        public RColumn(Ctx ctx, String columnName, int displayType, String sql)
            : this(ctx, columnName, displayType, sql, 0, null)
        {
            
        }

        /// <summary>
        /// Create Report Column
        /// </summary>
        /// <param name="ctx">context </param>
        /// <param name="columnName">column name</param>
        /// <param name="displayType">display type</param>
        /// <param name="sql">sql (if null then columnName is used). </param>
        /// <param name="AD_Reference_Value_ID">List/Table Reference</param>
        /// <param name="refColumnName">UserReference column name
        /// Will be overwritten if TableDir or Search</param>
        public RColumn(Ctx ctx, String columnName, int displayType,
            String sql, int AD_Reference_Value_ID, String refColumnName)
        {
            _colHeader = Msg.Translate(ctx, columnName);
            if (refColumnName != null)
            {
                _colHeader = Msg.Translate(ctx, refColumnName);
            }
            _displayType = displayType;
            _colSQL = sql;
            if (_colSQL == null || _colSQL.Length == 0)
            {
                _colSQL = columnName;
            }

            //  Strings
            if (DisplayType.IsText(displayType))
            {
                _colClass = typeof(String);// String.class;  //  default size=30
            }
            //  Amounts
            else if (displayType == DisplayType.Amount)
            {
                _colClass = typeof(Decimal);// BigDecimal.class;
                _colSize = 70;
            }
            //  Boolean
            else if (displayType == DisplayType.YesNo)
            {
                _colClass = typeof(Boolean);// Boolean.class;
            }
            //  Date
            else if (DisplayType.IsDate(displayType))
            {
                _colClass = typeof(DateTime);// Timestamp.class;
            }
            //  Number
            else if (displayType == DisplayType.Quantity
                || displayType == DisplayType.Number
                || displayType == DisplayType.CostPrice)
            {
                _colClass = typeof(Double);// Double.class;
                _colSize = 70;
            }
            //  Integer
            else if (displayType == DisplayType.Integer)
            {
                _colClass = typeof(int);//  Integer.class;
            }
            //  List
            else if (displayType == DisplayType.List)
            {

                Language language = Language.GetLanguage(Env.GetAD_Language(ctx));
                _colSQL = "(" + VLookUpFactory.GetLookup_ListEmbed(
                    language, AD_Reference_Value_ID, columnName) + ")";
                _colClass = typeof(String);// String.class;
                _isIDcol = false;
            }
            /**  Table
            else if (displayType == DisplayType.Table)
            {
                Language language = Language.getLanguage(Env.getAD_Language(ctx));
                _colSQL += ",(" + MLookupFactory.getLookup_TableEmbed(
                    language, columnName, RModel.TABLE_ALIAS, AD_Reference_Value_ID) + ")";
                _colClass = String.class;
                _isIDcol = false;
            }	**/
            //  TableDir, Search,...
            else
            {
                _colClass = typeof(String);// String.class;
                Language language = Language.GetLanguage(Env.GetAD_Language(ctx));
                if (columnName.Equals("Account_ID")
                    || columnName.Equals("User1_ID") || columnName.Equals("User2_ID"))
                {
                    _colSQL += ",(" + VLookUpFactory.GetLookup_TableDirEmbed(
                        language, "C_ElementValue_ID", RModel.TABLE_ALIAS, columnName) + ")";
                    _isIDcol = true;
                }
                else if (columnName.StartsWith("UserElement") && refColumnName != null)
                {
                    _colSQL += ",(" + VLookUpFactory.GetLookup_TableDirEmbed(
                        language, refColumnName, RModel.TABLE_ALIAS, columnName) + ")";
                    _isIDcol = true;
                }
                else if (columnName.Equals("C_LocFrom_ID") || columnName.Equals("C_LocTo_ID"))
                {
                    _colSQL += ",(" + VLookUpFactory.GetLookup_TableDirEmbed(
                        language, "C_Location_ID", RModel.TABLE_ALIAS, columnName) + ")";
                    _isIDcol = true;
                }
                else if (columnName.Equals("AD_OrgTrx_ID"))
                {
                    _colSQL += ",(" + VLookUpFactory.GetLookup_TableDirEmbed(
                        language, "AD_Org_ID", RModel.TABLE_ALIAS, columnName) + ")";
                    _isIDcol = true;
                }
                else if (displayType == DisplayType.TableDir)
                {
                    _colSQL += ",(" + VLookUpFactory.GetLookup_TableDirEmbed(
                        language, columnName, RModel.TABLE_ALIAS) + ")";
                    _isIDcol = true;
                }
            }
        }

        /// <summary>
        /// Create Info Column (r/o and not color column)
        /// </summary>
        /// <param name="colHeader">Column Header</param>
        /// <param name="colSQL">SQL select code for column</param>
        /// <param name="colClass">class of column - determines display</param>
        public RColumn(String colHeader, String colSQL, Type colClass)
        {
            _colHeader = colHeader;
            _colSQL = colSQL;
            _colClass = colClass;
        }

        /// <summary>
        /// Column Header
        /// </summary>
        /// <returns></returns>
        public String GetColHeader()
        {
            return _colHeader;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colHeader"></param>
        public void SetColHeader(String colHeader)
        {
            _colHeader = colHeader;
        }

        /// <summary>
        /// Column SQL
        /// </summary>
        /// <returns></returns>
        public String GetColSQL()
        {
            return _colSQL;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colSQL"></param>
        public void SetColSQL(String colSQL)
        {
            _colSQL = colSQL;
        }

        /// <summary>
        /// This column is an ID Column (i.e. two values - int & String are read)
        /// </summary>
        /// <returns></returns>
        public bool IsIDcol()
        {
            return _isIDcol;
        }

        /// <summary>
        /// Column Display Class
        /// </summary>
        /// <returns></returns>
        public Type GetColClass()    //Class getColClass()
        {
            return _colClass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colClass"></param>
        public void SetColClass(Type colClass)  //SetColClass(Class colClass)
        {
            _colClass = colClass;
        }

        /// <summary>
        /// Column Size in px
        /// </summary>
        /// <returns></returns>
        public int GetColSize()
        {
            return _colSize;
        }

        /// <summary>
        /// Column Size in px
        /// </summary>
        /// <param name="colSize"></param>
        public void SetColSize(int colSize)
        {
            _colSize = colSize;
        }

        /// <summary>
        /// Get DisplayType
        /// </summary>
        /// <returns></returns>
        public int GetDisplayType()
        {
            return _displayType;
        }

        /// <summary>
        /// Column is Readonly
        /// </summary>
        /// <returns></returns>
        public bool IsReadOnly()
        {
            return _readOnly;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readOnly"></param>
        public void SetReadOnly(bool readOnly)
        {
            _readOnly = readOnly;
        }

        /// <summary>
        /// This Color Determines the Color of the row
        /// </summary>
        /// <param name="colorColumn"></param>
        public void SetColorColumn(bool colorColumn)
        {
            _colorColumn = colorColumn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsColorColumn()
        {
            return _colorColumn;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public new String ToString()
        {
            StringBuilder sb = new StringBuilder("RColumn[");
            sb.Append(_colSQL).Append("=").Append(_colHeader)
                .Append("]");
            return sb.ToString();
        }
    }
}
