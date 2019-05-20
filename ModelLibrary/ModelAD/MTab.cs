/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MTab
 * Purpose        : 
 * Class Used     : MTab class inherits X_AD_Tab class
 * Chronological    Development
 * Raghunandan      14-may-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Model
{
    public class MTab : X_AD_Tab
    {
        #region Private Variables
        private static CCache<int, MTab> s_cache = new CCache<int, MTab>("AD_Tab", 20);
        //The Fields						
        private MField[] _fields = null;
        //Map of ColumnName and AD_Field_ID	
        //The ListDictionary class implements the IDictionary 
        ///interface using a single-linked array. It behaves like a Hashtable
        //private HashMap<String, int> _columnNameField = null;
        private IDictionary<string, int> _columnNameField = null;
        #endregion

        /// <summary>
        ///Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MTab(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
           
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Tab_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MTab(Ctx ctx, int AD_Tab_ID, Trx trxName)
            : base(ctx, AD_Tab_ID, trxName)
        {
            if (AD_Tab_ID == 0)
            {
                //	setAD_Window_ID (0);
                //	setAD_Table_ID (0);
                //	setName (null);
                SetEntityType(ENTITYTYPE_UserMaintained);	// U
                SetHasTree(false);
                SetIsReadOnly(false);
                SetIsSingleRow(false);
                SetIsSortTab(false);	// N
                SetIsTranslationTab(false);
                SetSeqNo(0);
                SetTabLevel(0);
                SetIsInsertRecord(true);
                SetIsAdvancedTab(false);
            }
        }

        /// <summary>
        ///Get MTab from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Tab_ID">id</param>
        /// <returns>MTab</returns>
        public static MTab Get(Ctx ctx, int AD_Tab_ID)
        {
            int key = (int)AD_Tab_ID;
            MTab retValue = (MTab)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MTab(ctx, AD_Tab_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        ///Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        public MTab(MWindow parent)
            : this(parent.GetCtx(), 0, parent.Get_Trx())
        {
            ///this(parent.getCtx(), 0, parent.get_TrxName());
            SetClientOrg(parent);
            SetAD_Window_ID(parent.GetAD_Window_ID());
            SetEntityType(parent.GetEntityType());
        }

        /// <summary>
        ///Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="from">from copy from</param>
        public MTab(MWindow parent, MTab from)
            : this(parent.GetCtx(), 0, parent.Get_Trx())
        {
            CopyValues(from, this);
            SetClientOrg(parent);
            SetAD_Window_ID(parent.GetAD_Window_ID());
            SetEntityType(parent.GetEntityType());
        }

        /// <summary>
        ///Get Fields
        /// </summary>
        /// <param name="reload">reload data</param>
        /// <param name="trxName">array of lines</param>
        /// <returns>transaction</returns>
        public MField[] GetFields(bool reload, Trx trxName)
        {
            if (_fields != null && !reload)
                return _fields;
            String sql = "SELECT * FROM AD_Field WHERE AD_Tab_ID=" + GetAD_Tab_ID() + " ORDER BY SeqNo";
            List<MField> list = new List<MField>();
            DataSet pstmt = null;
            try
            {
                pstmt = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = pstmt.Tables[0].Rows[i];
                    list.Add(new MField(GetCtx(), rs, trxName));

                }
                pstmt = null;
            }
            catch (Exception e)
            {
                log.Log(VAdvantage.Logging.Level.SEVERE, sql, e);
            }
            _fields = new MField[list.Count];
            _fields = list.ToArray();
            return _fields;
        }

        /// <summary>
        ///Get Field with ID
        /// </summary>
        /// <param name="AD_Field_ID">id</param>
        /// <returns>field or null</returns>
        public MField GetField(int AD_Field_ID)
        {
            if (AD_Field_ID == 0)
                return null;
            MField[] fields = GetFields(false, Get_Trx());
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].GetAD_Field_ID() == AD_Field_ID)
                    return fields[i];
            }
            return null;
        }

        /// <summary>
        ///Get Field with name
        /// </summary>
        /// <param name="columnName">name</param>
        /// <returns>field or null</returns>
        public MField GetField(string columnName)
        {
            int AD_Field_ID = GetAD_Field_ID(columnName);
            return GetField(AD_Field_ID);
        }

        /// <summary>
        ///Get AD_Field_ID in tab
        /// </summary>
        /// <param name="columnName">name</param>
        /// <returns>id</returns>
        public int GetAD_Field_ID(string columnName)
        {
            if (_columnNameField == null)
                FillColumnNameField();
            int? AD_Field_ID = _columnNameField[columnName];
            if (AD_Field_ID == null)
                return 0;
            return (int)AD_Field_ID;
        }

        /// <summary>
        /// Fill ColumnName Field map
        /// </summary>
        private void FillColumnNameField()
        {
            //_columnNameField = IDictionary<string, int>();
            _columnNameField = null;
            String sql = "SELECT ColumnName, f.AD_Field_ID "
                + "FROM AD_Field f"
                + " INNER JOIN AD_Column c ON (f.AD_Column_ID=c.AD_Column_ID) "
                + "WHERE f.AD_Tab_ID=" + GetAD_Tab_ID();
            DataSet pstmt = null;
            try
            {
                pstmt = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = pstmt.Tables[0].Rows[i];
                    string columnName = rs[1].ToString();//.getString(1);
                    int AD_Field_ID = (int)rs[2];//.getInt(2);
                    _columnNameField.Add(columnName, AD_Field_ID);
                }
                pstmt = null;
            }
            catch (Exception e)
            {
                log.Log(VAdvantage.Logging.Level.SEVERE, sql, e);
            }
        }

        /// <summary>
        ///Before Save
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	UPDATE AD_Tab SET IsInsertRecord='N' WHERE IsInsertRecord='Y' AND IsReadOnly='Y'
            if (IsReadOnly() && IsInsertRecord())
            {
                SetIsInsertRecord(false);
            }
            return true;
        }

        /// <summary>
        ///String Info
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MTab[");
            sb.Append(Get_ID())
                .Append("-").Append(GetName())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Get the Parent Table
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        public MTable GetParentTable(MWindow win, MTab currentTab)
        {
            MTab[] tabs = win.GetTabs(false, null);
            MTab tab = tabs[currentTab.GetTabLevel()];

            if (currentTab.GetTabLevel() == 0)
                return null;
            return MTable.Get(GetCtx(),  tab.GetAD_Table_ID());
        }
    }
}
