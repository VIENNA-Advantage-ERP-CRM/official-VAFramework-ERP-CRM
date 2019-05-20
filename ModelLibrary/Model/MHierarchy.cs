/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MHierarchy
 * Purpose        : Reporting Hierarchy Model
 * Class Used     : X_PA_Hierarchy
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
    public class MHierarchy:X_PA_Hierarchy
    {
        /// <summary>
        /// Get MHierarchy from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="PA_Hierarchy_ID">id</param>
        /// <returns>MHierarchy</returns>
        public static MHierarchy Get(Ctx ctx, int PA_Hierarchy_ID)
        {
            int key =PA_Hierarchy_ID;
            MHierarchy retValue = (MHierarchy)s_cache[key];//.get(key);
            if (retValue != null)
            {
                return retValue;
            }
            retValue = new MHierarchy(ctx, PA_Hierarchy_ID, null);
            if (retValue.Get_ID() != 0)
            {
                s_cache.Add(key, retValue);// .put(key, retValue);
            }
            return retValue;
        } //	get

        /**	Cache						*/
        private static CCache<int, MHierarchy> s_cache
            = new CCache<int, MHierarchy>("PA_Hierarchy_ID", 20);

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="PA_Hierarchy_ID">id</param>
        /// <param name="trxName">trx</param>
        public MHierarchy(Ctx ctx, int PA_Hierarchy_ID, Trx trxName):base(ctx, PA_Hierarchy_ID, trxName)
        {
            //super(ctx, PA_Hierarchy_ID, trxName);
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
        /// Get AD_Tree_ID based on tree type
        /// </summary>
        /// <param name="TreeType">Tree Type</param>
        /// <returns>id or 0</returns>
        public int GetAD_Tree_ID(String TreeType)
        {
            if (MTree.TREETYPE_Activity.Equals(TreeType))
            {
                return GetAD_Tree_Activity_ID();
            }
            if (MTree.TREETYPE_BPartner.Equals(TreeType))
            {
                return GetAD_Tree_BPartner_ID();
            }
            if (MTree.TREETYPE_Campaign.Equals(TreeType))
            {
                return GetAD_Tree_Campaign_ID();
            }
            if (MTree.TREETYPE_ElementValue.Equals(TreeType))
            {
                return GetAD_Tree_Account_ID();
            }
            if (MTree.TREETYPE_Organization.Equals(TreeType))
            {
                return GetAD_Tree_Org_ID();
            }
            if (MTree.TREETYPE_Product.Equals(TreeType))
            {
                return GetAD_Tree_Product_ID();
            }
            if (MTree.TREETYPE_Project.Equals(TreeType))
            {
                return GetAD_Tree_Project_ID();
            }
            if (MTree.TREETYPE_SalesRegion.Equals(TreeType))
            {
                return GetAD_Tree_SalesRegion_ID();
            }
            //
            log.Warning("Not supported: " + TreeType);
            return 0;
        }	//	getAD_Tree_ID

    }	//	MHierarchy

}
