/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAF_OrgDetail
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
    public class MVAFOrgDetail : X_VAF_OrgDetail
    {
        // Static Logger					
         private static VLogger _log = VLogger.GetVLogger(typeof(MVAFOrgDetail).FullName);

        //Account Schema				
        private MVABAccountBook _acctSchema = null;
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Org_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFOrgDetail(Ctx ctx, int VAF_Org_ID, Trx trxName)
            : base(ctx, VAF_Org_ID, trxName)
        {
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MVAFOrgDetail(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Organization constructor
        /// </summary>
        /// <param name="org">org</param>
        public MVAFOrgDetail(X_VAF_Org org)
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
        /// <param name="VAF_Org_ID">id</param>
        /// <param name="trx">transaction</param>
        /// <returns>Org Info</returns>
        public static MVAFOrgDetail Get(Ctx ctx, int VAF_Org_ID, Trx trxName)
        {
            MVAFOrgDetail retValue = null;
            String sql = "SELECT * FROM VAF_OrgDetail WHERE VAF_Org_ID=" + VAF_Org_ID;
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    retValue = new MVAFOrgDetail(ctx, dr, null);
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
        public new int GetVAM_Warehouse_ID()
        {
            int VAM_Warehouse_ID = base.GetVAM_Warehouse_ID();
            if (VAM_Warehouse_ID != 0)
                return VAM_Warehouse_ID;
            //
            MWarehouse[] whss = MWarehouse.GetForOrg(GetCtx(), GetVAF_Org_ID());
            if (whss.Length > 0)
            {
                VAM_Warehouse_ID = whss[0].GetVAM_Warehouse_ID();
                SetVAM_Warehouse_ID(VAM_Warehouse_ID);
                return VAM_Warehouse_ID;
            }
            log.Warning("No Warehouse for VAF_Org_ID=" + GetVAF_Org_ID());
            return 0;
        }

        /// <summary>
        ///Get primary Acct Schema
        /// </summary>
        /// <returns>acct schema</returns>
        public MVABAccountBook GetMAcctSchema()
        {
            if (_acctSchema == null && GetVAB_AccountBook_ID() != 0)
                _acctSchema = new MVABAccountBook(GetCtx(), GetVAB_AccountBook_ID(), null);
            return _acctSchema;
        }

        /// <summary>
        ///Get Default Accounting Currency
        /// </summary>
        /// <returns>currency or 0</returns>
        public int GetVAB_Currency_ID()
        {
            if (_acctSchema == null)
                GetMAcctSchema();
            if (_acctSchema != null)
                return _acctSchema.GetVAB_Currency_ID();
            return 0;
        }	//	getVAB_Currency_ID
    }
}