/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_AD_OrgInfo
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
    public class MOrgInfo : X_AD_OrgInfo
    {
        // Static Logger					
         private static VLogger _log = VLogger.GetVLogger(typeof(MOrgInfo).FullName);
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Org_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MOrgInfo(Ctx ctx, int AD_Org_ID, Trx trxName)
            : base(ctx, AD_Org_ID, trxName)
        {
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MOrgInfo(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Organization constructor
        /// </summary>
        /// <param name="org">org</param>
        public MOrgInfo(MOrg org)
            : base(org.GetCtx(), 0, org.Get_TrxName())
        {
            SetClientOrg(org);
            SetDUNS("?");
            SetTaxID("?");
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Org_ID">id</param>
        /// <param name="trx">transaction</param>
        /// <returns>Org Info</returns>
        public static MOrgInfo Get(Ctx ctx, int AD_Org_ID, Trx trxName)
        {
            MOrgInfo retValue = null;
            String sql = "SELECT * FROM AD_OrgInfo WHERE AD_Org_ID=" + AD_Org_ID;
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    retValue = new MOrgInfo(ctx, dr, null);
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            return retValue;
        }

        /// <summary>
        /// Get Default Org Warehouse
        /// </summary>
        /// <returns>warehouse</returns>
        public new int GetM_Warehouse_ID()
        {
            int M_Warehouse_ID = base.GetM_Warehouse_ID();
            if (M_Warehouse_ID != 0)
                return M_Warehouse_ID;
            //
            MWarehouse[] whss = MWarehouse.GetForOrg(GetCtx(), GetAD_Org_ID());
            if (whss.Length > 0)
            {
                M_Warehouse_ID = whss[0].GetM_Warehouse_ID();
                SetM_Warehouse_ID(M_Warehouse_ID);
                return M_Warehouse_ID;
            }
            log.Warning("No Warehouse for AD_Org_ID=" + GetAD_Org_ID());
            return 0;
        }	
    }
}