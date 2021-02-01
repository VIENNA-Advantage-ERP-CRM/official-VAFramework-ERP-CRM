/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MWindow
 * Purpose        : 
 * Class Used     : MWindow inherits X_VAF_Screen
 * Chronological    Development
 * Raghunandan      05-May-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.SqlExec;
using System.Windows.Forms;
using System.Drawing;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MVAFScreen : X_VAF_Screen
    {
        //Cache
        private static CCache<int, MVAFScreen> s_cache = new CCache<int, MVAFScreen>("VAF_Screen_ID", 20);
        //The Lines						
        private MVAFTab[] _tabs = null;
        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAFScreen).FullName);

        /// <summary>
        ///Get MWindow from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Screen_ID">id</param>
        /// <returns>MWindow</returns>
        public static MVAFScreen Get(Ctx ctx, int VAF_Screen_ID)
        {
            int key = VAF_Screen_ID;
            MVAFScreen retValue = (MVAFScreen)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVAFScreen(ctx, VAF_Screen_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        ///Get workflow nodes with where clause.
        ///Is here as MWFNode is in base
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="whereClause">whereClause where clause w/o the actual WHERE</param>
        /// <returns>nodes</returns>
        //public static X_VAF_WFlow_Node[] GetWFNodes(Ctx ctx, string whereClause)
        //{
        //    String sql = "SELECT * FROM VAF_WFlow_Node";
        //    if (whereClause != null && whereClause.Length > 0)
        //        sql += " WHERE " + whereClause;
        //    List<X_VAF_WFlow_Node> list = new List<X_VAF_WFlow_Node>();
        //    DataSet ds = null;
        //    try
        //    {
        //        ds = BaseLibrary.DataBase.DB.ExecuteDataset(sql, null, null);
        //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        {
        //            DataRow rs = ds.Tables[0].Rows[i];
        //            list.Add(new X_VAF_WFlow_Node(ctx, rs, null));
        //        }
        //        ds = null;
        //    }
        //    catch (Exception e)
        //    {
        //        _log.Log(Level.SEVERE, sql, e);
        //    }

        //    X_VAF_WFlow_Node[] retValue = new X_VAF_WFlow_Node[list.Count];
        //    //list.toArray(retValue);
        //    retValue = list.ToArray();
        //    return retValue;
        //}

        /// <summary>
        ///Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Screen_ID">VAF_Screen_ID</param>
        /// <param name="trxName">transaction</param>
        public MVAFScreen(Ctx ctx, int VAF_Screen_ID, Trx trxName)
            : base(ctx, VAF_Screen_ID, trxName)
        {
            if (VAF_Screen_ID == 0)
            {
                SetWindowType(WINDOWTYPE_Maintain);	// M
                SetEntityType(ENTITYTYPE_UserMaintained);	// U
                SetIsBetaFunctionality(false);
                SetIsDefault(false);
                SetIsCustomDefault(false);
            }
        }

        /// <summary>
        ///Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAFScreen(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        ///Set Window Size
        /// </summary>
        /// <param name="size">size</param>
        //public void SetWindowSize(Dimension size)
        //public void SetWindowSize(Size size)
        //{
        //    if (size != null)
        //    {
        //        SetWinWidth(size.Width);
        //        SetWinHeight(size.Height);
        //    }
        //    else
        //    {
        //        SetWinWidth(0);
        //        SetWinHeight(0);
        //    }
        //}

        /// <summary>
        ///Get Fields
        /// </summary>
        /// <param name="reload">reload data</param>
        /// <param name="trxName">array of lines</param>
        /// <returns>transaction</returns>
        public MVAFTab[] GetTabs(bool reload, Trx trxName)
        {
            if (_tabs != null && !reload)
                return _tabs;
            String sql = "SELECT * FROM VAF_Tab WHERE VAF_Screen_ID=" + GetVAF_Screen_ID() + " ORDER BY SeqNo";
            List<MVAFTab> list = new List<MVAFTab>();
            DataSet ds = null;
            try
            {
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    list.Add(new MVAFTab(GetCtx(), rs, trxName));
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            _tabs = new MVAFTab[list.Count];
            _tabs = list.ToArray();
            return _tabs;
        }

        /// <summary>
        ///Get Tab with ID
        /// </summary>
        /// <param name="VAF_Tab_ID">id</param>
        /// <returns>tab or null</returns>
        public MVAFTab GetTab(int VAF_Tab_ID)
        {
            MVAFTab[] tabs = GetTabs(false, Get_TrxName());
            for (int i = 0; i < tabs.Length; i++)
            {
                if (tabs[i].GetVAF_Tab_ID() == VAF_Tab_ID)
                    return tabs[i];
            }
            return null;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (newRecord)	//	Add to all automatic roles
            {
                MVAFRole[] roles = MVAFRole.GetOf(GetCtx(), "IsManual='N'");
                for (int i = 0; i < roles.Length; i++)
                {
                    MVAFScreenRights wa = new MVAFScreenRights(this, roles[i].GetVAF_Role_ID());
                    wa.Save();
                }
            }
            //	Menu/Workflow
            else if (Is_ValueChanged("IsActive") || Is_ValueChanged("Name")
                || Is_ValueChanged("Description") || Is_ValueChanged("Help"))
            {
                MVAFMenuConfig[] menues = MVAFMenuConfig.Get(GetCtx(), "VAF_Screen_ID=" + GetVAF_Screen_ID());
                for (int i = 0; i < menues.Length; i++)
                {
                    menues[i].SetName(GetName());
                    menues[i].SetDescription(GetDescription());
                    menues[i].SetIsActive(IsActive());
                    menues[i].Save();
                }
                //
                X_VAF_WFlow_Node[] nodes = GetWFNodes(GetCtx(), "VAF_Screen_ID=" + GetVAF_Screen_ID());
                for (int i = 0; i < nodes.Length; i++)
                {
                    bool changed = false;
                    if (nodes[i].IsActive() != IsActive())
                    {
                        nodes[i].SetIsActive(IsActive());
                        changed = true;
                    }
                    if (nodes[i].IsCentrallyMaintained())
                    {
                        nodes[i].SetName(GetName());
                        nodes[i].SetDescription(GetDescription());
                        nodes[i].SetHelp(GetHelp());
                        changed = true;
                    }
                    if (changed)
                        nodes[i].Save();
                }
            }
            return success;
        }
        /// <summary>
        ///Get workflow nodes with where clause.
        ///Is here as MWFNode is in base
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="whereClause">whereClause where clause w/o the actual WHERE</param>
        /// <returns>nodes</returns>
        public static X_VAF_WFlow_Node[] GetWFNodes(Ctx ctx, string whereClause)
        {
            String sql = "SELECT * FROM VAF_WFlow_Node";
            if (whereClause != null && whereClause.Length > 0)
                sql += " WHERE " + whereClause;
            List<X_VAF_WFlow_Node> list = new List<X_VAF_WFlow_Node>();
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    list.Add(new X_VAF_WFlow_Node(ctx, rs, null));
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            X_VAF_WFlow_Node[] retValue = new X_VAF_WFlow_Node[list.Count];
            //list.toArray(retValue);
            retValue = list.ToArray();
            return retValue;
        }
        /// <summary>
        ///String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MWindow[")
                .Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }
    }
}
