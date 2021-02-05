/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAF_Org
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
    public class MVAFOrg : X_VAF_Org
    {
        //	Cache
        private static CCache<int, MVAFOrg> cache = new CCache<int, MVAFOrg>("VAF_Org", 20);
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAFOrg).FullName);
        //	Org Info
        private MVAFOrgDetail info = null;
        //	Linked Business Partner
        private int linkedBPartner = -1;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Org_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFOrg(Ctx ctx, int VAF_Org_ID, Trx trxName)
            : base(ctx, VAF_Org_ID, trxName)
        {
            if (VAF_Org_ID == 0)
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
        public MVAFOrg(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="client">client</param>
        /// <param name="name">name</param>
        public MVAFOrg(MVAFClient client, String name)
            : this(client.GetCtx(), 0, client.Get_Trx())
        {
            SetVAF_Client_ID(client.GetVAF_Client_ID());
            SetValue(name);
            SetName(name);
        }

        /// <summary>
        /// Get Organizations Of Client
        /// </summary>
        /// <param name="po">persistent object</param>
        /// <returns>array of orgs whose Organization unit is false</returns>
        public static MVAFOrg[] GetOfClient(PO po)
        {
            List<MVAFOrg> list = new List<MVAFOrg>();
            //JID_1833 - pick only those org, whose Org unit = False
            String sql = "SELECT * FROM VAF_Org WHERE issummary = 'N' AND IsOrgUnit = 'N' AND IsActive = 'Y' AND VAF_Client_ID=" + po.GetVAF_Client_ID() + " ORDER BY Value";
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
                        list.Add(new MVAFOrg(po.GetCtx(), dr, null));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            MVAFOrg[] retValue = new MVAFOrg[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Org from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Org_ID">id</param>
        /// <returns>MOrg</returns>
        public static MVAFOrg Get(Ctx ctx, int VAF_Org_ID)
        {
            int key = VAF_Org_ID;
            MVAFOrg retValue = null;
            if (cache.ContainsKey(key))
            {
                retValue = (MVAFOrg)cache[key];
            }
            if (retValue != null)
                return retValue;
            retValue = new MVAFOrg(ctx, VAF_Org_ID, null);
            if (VAF_Org_ID == 0)
                retValue.Load((Trx)null);
            if (retValue.Get_ID() != VAF_Org_ID)
                cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get Org Info
        /// </summary>
        /// <returns>Org Info</returns>
        public MVAFOrgDetail GetInfo()
        {
            if (info == null)
                info = MVAFOrgDetail.Get(GetCtx(), GetVAF_Org_ID(), Get_Trx());
            return info;
        }

        /// <summary>
        /// Get Linked BPartner
        /// </summary>
        /// <returns>VAB_BusinessPartner_ID</returns>
        public int GetLinkedVAB_BusinessPartner_ID()
        {
            return GetLinkedVAB_BusinessPartner_ID(null);
        }

        /// <summary>
        /// Get Linked BP
        /// </summary>
        /// <param name="trxName">transaction</param>
        /// <returns>VAB_BusinessPartner_ID or 0</returns>
        public int GetLinkedVAB_BusinessPartner_ID(Trx trxName)
        {
            if (linkedBPartner == -1)
            {
                string sql = "SELECT VAB_BusinessPartner_ID FROM VAB_BusinessPartner WHERE VAF_OrgBP_ID=" + GetVAF_Org_ID();
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                int VAB_BusinessPartner_ID = 0;
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    VAB_BusinessPartner_ID = Utility.Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAB_BusinessPartner_ID"].ToString());
                }
                linkedBPartner = VAB_BusinessPartner_ID;
            }
            return linkedBPartner;
        }

        /// <summary>
        /// Get Default Org Warehouse
        /// </summary>
        /// <returns>warehouse</returns>
        public int GetVAM_Warehouse_ID()
        {
            return GetInfo().GetVAM_Warehouse_ID();
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
                info = new MVAFOrgDetail(this);
                info.Save();
                //	Access
                MVAFRoleOrgRights.CreateForOrg(this);
                MVAFRole.GetDefault(GetCtx(), true);	//	reload
            }
            //	Value/Name change
            if (!newRecord && (Is_ValueChanged("Value") || Is_ValueChanged("Name")))
            {
                MAccount.UpdateValueDescription(GetCtx(), "VAF_Org_ID=" + GetVAF_Org_ID(), Get_Trx());
                if ("Y".Equals(GetCtx().GetContext("$Element_OT")))
                    MAccount.UpdateValueDescription(GetCtx(), "VAF_OrgTrx_ID=" + GetVAF_Org_ID(), Get_Trx());
            }

            if (!newRecord)
            {
                if (!IsSummary())
                {

                    int orgTableID = MVAFTableView.Get_Table_ID("VAF_Org");

                    string sql = "SELECT VAF_TreeInfo_ID FROM VAF_TreeInfo "
                      + "WHERE VAF_Client_ID=" + GetCtx().GetVAF_Client_ID() + " AND VAF_TableView_ID=" + orgTableID + " AND IsActive='Y' AND IsAllNodes='Y' "
                      + "ORDER BY IsDefault DESC, VAF_TreeInfo_ID";

                    object VAF_TreeInfo_ID = DB.ExecuteScalar(sql, null, null);

                    DB.ExecuteQuery("Update VAF_TreeInfoChild Set Parent_ID = 0 where Parent_ID=" + GetVAF_Org_ID() + " AND VAF_TreeInfo_ID=" + Util.GetValueOfInt(VAF_TreeInfo_ID));

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
        internal MVABAccountBook GetAcctSchema()
        {
            if (info == null)
                info = MVAFOrgDetail.Get(GetCtx(), GetVAF_Client_ID(), Get_TrxName());
            if (info != null)
            {
                int VAB_AccountBook_ID = info.GetVAB_AccountBook_ID();
                if (VAB_AccountBook_ID != 0)
                    return MVABAccountBook.Get(GetCtx(), VAB_AccountBook_ID);
            }
            return null;
        }


    }
}