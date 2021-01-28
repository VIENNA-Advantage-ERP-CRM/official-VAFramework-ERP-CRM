/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MTab
 * Purpose        : 
 * Class Used     : MTab class inherits X_VAF_Tab class
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


namespace VAdvantage.Model
{
    public class MVAFTab : X_VAF_Tab
    {
        #region Private Variables
        private static CCache<int, MVAFTab> s_cache = new CCache<int, MVAFTab>("VAF_Tab", 20);
        //The Fields						
        private MVAFField[] _fields = null;
        //Map of ColumnName and VAF_Field_ID	
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
        public MVAFTab(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
           
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Tab_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFTab(Ctx ctx, int VAF_Tab_ID, Trx trxName)
            : base(ctx, VAF_Tab_ID, trxName)
        {
            if (VAF_Tab_ID == 0)
            {
                //	setVAF_Screen_ID (0);
                //	setVAF_TableView_ID (0);
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
        /// <param name="VAF_Tab_ID">id</param>
        /// <returns>MTab</returns>
        public static MVAFTab Get(Ctx ctx, int VAF_Tab_ID)
        {
            int key = (int)VAF_Tab_ID;
            MVAFTab retValue = (MVAFTab)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVAFTab(ctx, VAF_Tab_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        ///Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        public MVAFTab(MWindow parent)
            : this(parent.GetCtx(), 0, parent.Get_Trx())
        {
            ///this(parent.getCtx(), 0, parent.get_TrxName());
            SetClientOrg(parent);
            SetVAF_Screen_ID(parent.GetVAF_Screen_ID());
            SetEntityType(parent.GetEntityType());
        }

        /// <summary>
        ///Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="from">from copy from</param>
        public MVAFTab(MWindow parent, MVAFTab from)
            : this(parent.GetCtx(), 0, parent.Get_Trx())
        {
            CopyValues(from, this);
            SetClientOrg(parent);
            SetVAF_Screen_ID(parent.GetVAF_Screen_ID());
            SetEntityType(parent.GetEntityType());
        }

        /// <summary>
        ///Get Fields
        /// </summary>
        /// <param name="reload">reload data</param>
        /// <param name="trxName">array of lines</param>
        /// <returns>transaction</returns>
        public MVAFField[] GetFields(bool reload, Trx trxName)
        {
            if (_fields != null && !reload)
                return _fields;
            String sql = "SELECT * FROM VAF_Field WHERE VAF_Tab_ID=" + GetVAF_Tab_ID() + " ORDER BY SeqNo";
            List<MVAFField> list = new List<MVAFField>();
            DataSet pstmt = null;
            try
            {
                pstmt = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = pstmt.Tables[0].Rows[i];
                    list.Add(new MVAFField(GetCtx(), rs, trxName));

                }
                pstmt = null;
            }
            catch (Exception e)
            {
                log.Log(VAdvantage.Logging.Level.SEVERE, sql, e);
            }
            _fields = new MVAFField[list.Count];
            _fields = list.ToArray();
            return _fields;
        }

        /// <summary>
        ///Get Field with ID
        /// </summary>
        /// <param name="VAF_Field_ID">id</param>
        /// <returns>field or null</returns>
        public MVAFField GetField(int VAF_Field_ID)
        {
            if (VAF_Field_ID == 0)
                return null;
            MVAFField[] fields = GetFields(false, Get_Trx());
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].GetVAF_Field_ID() == VAF_Field_ID)
                    return fields[i];
            }
            return null;
        }

        /// <summary>
        ///Get Field with name
        /// </summary>
        /// <param name="columnName">name</param>
        /// <returns>field or null</returns>
        public MVAFField GetField(string columnName)
        {
            int VAF_Field_ID = GetVAF_Field_ID(columnName);
            return GetField(VAF_Field_ID);
        }

        /// <summary>
        ///Get VAF_Field_ID in tab
        /// </summary>
        /// <param name="columnName">name</param>
        /// <returns>id</returns>
        public int GetVAF_Field_ID(string columnName)
        {
            if (_columnNameField == null)
                FillColumnNameField();
            int? VAF_Field_ID = _columnNameField[columnName];
            if (VAF_Field_ID == null)
                return 0;
            return (int)VAF_Field_ID;
        }

        /// <summary>
        /// Fill ColumnName Field map
        /// </summary>
        private void FillColumnNameField()
        {
            //_columnNameField = IDictionary<string, int>();
            _columnNameField = null;
            String sql = "SELECT ColumnName, f.VAF_Field_ID "
                + "FROM VAF_Field f"
                + " INNER JOIN VAF_Column c ON (f.VAF_Column_ID=c.VAF_Column_ID) "
                + "WHERE f.VAF_Tab_ID=" + GetVAF_Tab_ID();
            DataSet pstmt = null;
            try
            {
                pstmt = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = pstmt.Tables[0].Rows[i];
                    string columnName = rs[1].ToString();//.getString(1);
                    int VAF_Field_ID = (int)rs[2];//.getInt(2);
                    _columnNameField.Add(columnName, VAF_Field_ID);
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
            //	UPDATE VAF_Tab SET IsInsertRecord='N' WHERE IsInsertRecord='Y' AND IsReadOnly='Y'
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
        public MVAFTableView GetParentTable(MWindow win, MVAFTab currentTab)
        {
            MVAFTab[] tabs = win.GetTabs(false, null);
            MVAFTab tab = tabs[currentTab.GetTabLevel()];

            if (currentTab.GetTabLevel() == 0)
                return null;
            return MVAFTableView.Get(GetCtx(),  tab.GetVAF_TableView_ID());
        }

        /// <summary>
        /// After save of Tab
        /// </summary>
        /// <param name="newRecord"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return false;

            // check if there is any change in MaintainVersionOnApproval field on Tab
            if (Is_ValueChanged("MaintainVerOnApproval"))
            {
                int windowID = GetVAF_Screen_ID();
                int SeqNo = GetSeqNo();
                // Get all tabs from current window which have greater sequence than current tab
                int[] TabIDs = MVAFTab.GetAllIDs("VAF_Tab", "VAF_Screen_ID = " + windowID + " AND SeqNo > " + SeqNo, Get_TrxName());
                // if there are tabs with sequence greater than current tab
                if (TabIDs.Length > 0)
                {
                    string tabs = string.Join(",", TabIDs);
                    // Update MaintainVerOnApproval field to the value set in current field of current tab in all tabs with greater
                    // sequence of current tab
                    int updateCount = DB.ExecuteQuery("UPDATE VAF_Tab SET MaintainVerOnApproval = '" + (IsMaintainVerOnApproval() ? "Y" : "N") + "' WHERE VAF_Tab_ID IN (" + tabs + ")", null, Get_TrxName());
                    if (updateCount < 0)
                        log.Info("UPDATE VAF_Tab SET MaintainVerOnApproval = '" + (IsMaintainVerOnApproval() ? "Y" : "N") + "' WHERE VAF_Tab_ID IN (" + tabs + ")");
                }
            }
            return true;
        }
    }
}
