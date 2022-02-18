/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_AD_Org
 * Chronological Development
 * Veena Pandey     
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MOrg : X_AD_Org
    {
        //	Cache
        private static CCache<int, MOrg> cache = new CCache<int, MOrg>("AD_Org", 20);
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MOrg).FullName);
        //	Org Info
        private MOrgInfo info = null;
        //	Linked Business Partner
        private int linkedBPartner = -1;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Org_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MOrg(Ctx ctx, int AD_Org_ID, Trx trxName)
            : base(ctx, AD_Org_ID, trxName)
        {
            if (AD_Org_ID == 0)
            {
                //	setValue (null);
                //	setName (null);
                SetIsSummary(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MOrg(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="client">client</param>
        /// <param name="name">name</param>
        public MOrg(MClient client, String name)
            : this(client.GetCtx(), 0, client.Get_Trx())
        {
            SetAD_Client_ID(client.GetAD_Client_ID());
            SetValue(name);
            SetName(name);
        }

        /// <summary>
        /// Get Organizations Of Client
        /// </summary>
        /// <param name="po">persistent object</param>
        /// <returns>array of orgs whose Organization unit is false</returns>
        public static MOrg[] GetOfClient(PO po)
        {
            List<MOrg> list = new List<MOrg>();
            //JID_1833 - pick only those org, whose Org unit = False
            String sql = "SELECT * FROM AD_Org WHERE issummary = 'N' AND IsOrgUnit = 'N' AND IsActive = 'Y' AND AD_Client_ID=" + po.GetAD_Client_ID() + " ORDER BY Value";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    DataRow dr = null;
                    int totCount = ds.Tables[0].Rows.Count;
                    for (int i = 0; i < totCount; i++)
                    {
                        dr = ds.Tables[0].Rows[i];
                        list.Add(new MOrg(po.GetCtx(), dr, null));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            MOrg[] retValue = new MOrg[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Org from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Org_ID">id</param>
        /// <returns>MOrg</returns>
        public static MOrg Get(Ctx ctx, int AD_Org_ID)
        {
            int key = AD_Org_ID;
            MOrg retValue = null;
            if (cache.ContainsKey(key))
            {
                retValue = (MOrg)cache[key];
            }
            if (retValue != null)
                return retValue;
            retValue = new MOrg(ctx, AD_Org_ID, null);
            if (AD_Org_ID == 0)
                retValue.Load((Trx)null);
            if (retValue.Get_ID() != AD_Org_ID)
                cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get Org Info
        /// </summary>
        /// <returns>Org Info</returns>
        public MOrgInfo GetInfo()
        {
            if (info == null)
                info = MOrgInfo.Get(GetCtx(), GetAD_Org_ID(), Get_Trx());
            return info;
        }

        /// <summary>
        /// Get Linked BPartner
        /// </summary>
        /// <returns>C_BPartner_ID</returns>
        public int GetLinkedC_BPartner_ID()
        {
            return GetLinkedC_BPartner_ID(null);
        }

        /// <summary>
        /// Get Linked BP
        /// </summary>
        /// <param name="trxName">transaction</param>
        /// <returns>C_BPartner_ID or 0</returns>
        public int GetLinkedC_BPartner_ID(Trx trxName)
        {
            if (linkedBPartner == -1)
            {
                string sql = "SELECT C_BPartner_ID FROM C_BPartner WHERE AD_OrgBP_ID=" + GetAD_Org_ID();
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                int C_BPartner_ID = 0;
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    C_BPartner_ID = Utility.Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_BPartner_ID"].ToString());
                }
                linkedBPartner = C_BPartner_ID;
            }
            return linkedBPartner;
        }

        /// <summary>
        /// Get Default Org Warehouse
        /// </summary>
        /// <returns>warehouse</returns>
        public int GetM_Warehouse_ID()
        {
            return GetInfo().GetM_Warehouse_ID();
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new Record</param>
        /// <param name="success">save success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;
            if (newRecord)
            {
                //	Info
                info = new MOrgInfo(this);
                info.Save();
                //	Access
                MRoleOrgAccess.CreateForOrg(this);
                MRole.GetDefault(GetCtx(), true);	//	reload
            }
            //	Value/Name change
            if (!newRecord && (Is_ValueChanged("Value") || Is_ValueChanged("Name")))
            {
                MAccount.UpdateValueDescription(GetCtx(), "AD_Org_ID=" + GetAD_Org_ID(), Get_Trx());
                if ("Y".Equals(GetCtx().GetContext("$Element_OT")))
                    MAccount.UpdateValueDescription(GetCtx(), "AD_OrgTrx_ID=" + GetAD_Org_ID(), Get_Trx());
            }

            if (!newRecord)
            {
                if (!IsSummary())
                {

                    int orgTableID = MTable.Get_Table_ID("AD_Org");

                    string sql = "SELECT AD_Tree_ID FROM AD_Tree "
                      + "WHERE AD_Client_ID=" + GetCtx().GetAD_Client_ID() + " AND AD_Table_ID=" + orgTableID + " AND IsActive='Y' AND IsAllNodes='Y' "
                      + "ORDER BY IsDefault DESC, AD_Tree_ID";

                    object AD_Tree_ID = DB.ExecuteScalar(sql, null, null);

                    DB.ExecuteQuery("Update AD_TreeNode Set Parent_ID = 0 where Parent_ID=" + GetAD_Org_ID() + " AND AD_Tree_ID=" + Util.GetValueOfInt(AD_Tree_ID));

                }
            }

            return true;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new Record</param>
        /// <param name="success">save success</param>
        /// <returns>success</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            // Check applied to restrict the records insertion from organization unit window if Cost center and profit center is not selected.
            if (Get_ColumnIndex("IsOrgUnit") > -1)
            {
                if (IsOrgUnit())
                {
                    if (!IsProfitCenter() && !IsCostCenter())
                    {
                        log.SaveError("CheckProfitCostCenter", "");
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// This is Done Bcoz in Organization Structure Form , new organization is being inserted by query. So to implementafter save , this function createad.
        /// 
        /// By Karan
        /// 
        /// </summary>
        /// <param name="newRecord"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public bool PublicAfterSave(bool newRecord, bool success)
        {
            return AfterSave(newRecord, success);
        }

        /// <summary>
        /// Get Primary Accounting Schema
        /// </summary>
        /// <returns>Acct Schema or null</returns>
        internal MAcctSchema GetAcctSchema()
        {
            if (info == null)
                info = MOrgInfo.Get(GetCtx(), GetAD_Org_ID(), Get_TrxName());
            if (info != null)
            {
                int C_AcctSchema_ID = info.GetC_AcctSchema_ID();
                if (C_AcctSchema_ID != 0)
                    return MAcctSchema.Get(GetCtx(), C_AcctSchema_ID);
            }
            return null;
        }
    }
}