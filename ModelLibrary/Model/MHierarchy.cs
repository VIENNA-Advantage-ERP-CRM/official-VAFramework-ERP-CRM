/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MHierarchy
 * Purpose        : Reporting Hierarchy Model
 * Class Used     : X_VAPA_FinancialReportingOrder
 * Chronological    Development
 * Deepak           11-Jan-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Model;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.Login;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MHierarchy:X_VAPA_FinancialReportingOrder
    {
        /// <summary>
        /// Get MHierarchy from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAPA_FinancialReportingOrder_ID">id</param>
        /// <returns>MHierarchy</returns>
        public static MHierarchy Get(Ctx ctx, int VAPA_FinancialReportingOrder_ID)
        {
            int key =VAPA_FinancialReportingOrder_ID;
            MHierarchy retValue = (MHierarchy)s_cache[key];//.get(key);
            if (retValue != null)
            {
                return retValue;
            }
            retValue = new MHierarchy(ctx, VAPA_FinancialReportingOrder_ID, null);
            if (retValue.Get_ID() != 0)
            {
                s_cache.Add(key, retValue);// .put(key, retValue);
            }
            return retValue;
        } //	get

        /**	Cache						*/
        private static CCache<int, MHierarchy> s_cache
            = new CCache<int, MHierarchy>("VAPA_FinancialReportingOrder_ID", 20);

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAPA_FinancialReportingOrder_ID">id</param>
        /// <param name="trxName">trx</param>
        public MHierarchy(Ctx ctx, int VAPA_FinancialReportingOrder_ID, Trx trxName):base(ctx, VAPA_FinancialReportingOrder_ID, trxName)
        {
            //super(ctx, VAPA_FinancialReportingOrder_ID, trxName);
        }	//	MHierarchy

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">trx</param>
        public MHierarchy(Ctx ctx,DataRow dr, Trx trxName):base(ctx,dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MHierarchy

        /// <summary>
        /// Get VAF_TreeInfo_ID based on tree type
        /// </summary>
        /// <param name="TreeType">Tree Type</param>
        /// <returns>id or 0</returns>
        public int GetVAF_TreeInfo_ID(String TreeType)
        {
            if (MVAFTreeInfo.TREETYPE_Activity.Equals(TreeType))
            {
                return GetVAF_TreeInfo_Activity_ID();
            }
            if (MVAFTreeInfo.TREETYPE_BPartner.Equals(TreeType))
            {
                return GetVAF_TreeInfo_BPartner_ID();
            }
            if (MVAFTreeInfo.TREETYPE_Campaign.Equals(TreeType))
            {
                return GetVAF_TreeInfo_Campaign_ID();
            }
            if (MVAFTreeInfo.TREETYPE_ElementValue.Equals(TreeType))
            {
                return GetVAF_TreeInfo_Account_ID();
            }
            if (MVAFTreeInfo.TREETYPE_Organization.Equals(TreeType))
            {
                return GetVAF_TreeInfo_Org_ID();
            }
            if (MVAFTreeInfo.TREETYPE_Product.Equals(TreeType))
            {
                return GetVAF_TreeInfo_Product_ID();
            }
            if (MVAFTreeInfo.TREETYPE_Project.Equals(TreeType))
            {
                return GetVAF_TreeInfo_Project_ID();
            }
            if (MVAFTreeInfo.TREETYPE_SalesRegion.Equals(TreeType))
            {
                return GetVAF_TreeInfo_SalesRegion_ID();
            }
            //
            log.Warning("Not supported: " + TreeType);
            return 0;
        }	//	getVAF_TreeInfo_ID

    }	//	MHierarchy

}
