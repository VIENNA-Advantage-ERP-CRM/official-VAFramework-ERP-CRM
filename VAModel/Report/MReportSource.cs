/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MReportSource
 * Purpose        : Report Source Model
 * Class Used     : X_PA_ReportSource
 * Chronological    Development
 * Deepak           19-Jan-2010
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
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Report
{
    public class MReportSource : X_PA_ReportSource
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="PA_ReportSource_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MReportSource(Ctx ctx, int PA_ReportSource_ID, Trx trxName):base(ctx, PA_ReportSource_ID, trxName)
        {
            
            if (PA_ReportSource_ID == 0)
            {
            }
        }	//	MReportSource

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MReportSource(Ctx ctx,DataRow dr, Trx trxName): base(ctx, dr, trxName)
        {
           
        }	//	MReportSource


       /// <summary>
       ///  Get SQL where clause
       /// </summary>
       /// <param name="PA_Hierarchy_ID">hierarchy </param>
        /// <returns> where clause</returns>
        public String GetWhereClause(int PA_Hierarchy_ID)
        {
            String et = GetElementType();
            //	ID for Tree Leaf Value
            int ID = 0;
            //
            if (MAcctSchemaElement.ELEMENTTYPE_Account.Equals(et))
            {
                ID = GetC_ElementValue_ID();
            }
            else if (MAcctSchemaElement.ELEMENTTYPE_Activity.Equals(et))
            {
                ID = GetC_Activity_ID();
            }
            else if (MAcctSchemaElement.ELEMENTTYPE_BPartner.Equals(et))
            {
                ID = GetC_BPartner_ID();
            }
            else if (MAcctSchemaElement.ELEMENTTYPE_Campaign.Equals(et))
            {
                ID = GetC_Campaign_ID();
            }
            else if (MAcctSchemaElement.ELEMENTTYPE_LocationFrom.Equals(et))
            {
                ID = GetC_Location_ID();
            }
            else if (MAcctSchemaElement.ELEMENTTYPE_LocationTo.Equals(et))
            {
                ID = GetC_Location_ID();
            }
            else if (MAcctSchemaElement.ELEMENTTYPE_Organization.Equals(et))
            {
                ID = GetOrg_ID();
            }
            else if (MAcctSchemaElement.ELEMENTTYPE_Product.Equals(et))
            {
                ID = GetM_Product_ID();
            }
            else if (MAcctSchemaElement.ELEMENTTYPE_Project.Equals(et))
            {
                ID = GetC_Project_ID();
            }
            else if (MAcctSchemaElement.ELEMENTTYPE_SalesRegion.Equals(et))
            {
                ID = GetC_SalesRegion_ID();
            }
            else if (MAcctSchemaElement.ELEMENTTYPE_OrgTrx.Equals(et))
            {
                ID = GetOrg_ID();	//	(re)uses Org_ID
            }
            else if (MAcctSchemaElement.ELEMENTTYPE_UserList1.Equals(et))
            {
                ID = GetC_ElementValue_ID();
            }
            else if (MAcctSchemaElement.ELEMENTTYPE_UserList2.Equals(et))
            {
                ID = GetC_ElementValue_ID();
            }
            //		else if (MAcctSchemaElement.ELEMENTTYPE_UserElement1.equals(et))
            //			ID = getUserElement1_ID ();
            //		else if (MAcctSchemaElement.ELEMENTTYPE_UserElement2.equals(et))
            //			ID = getUserElement2_ID ();
            if (ID == 0)
            {
                log.Fine("No Restrictions - No ID for EntityType=" + et);
                return "";
            }
            //
            return MReportTree.GetWhereClause(GetCtx(), PA_Hierarchy_ID, et, ID);
        }	//	getWhereClause


        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MReportSource[")
                .Append(Get_ID()).Append(" - ").Append(GetDescription())
                .Append(" - ").Append(GetElementType());
            sb.Append("]");
            return sb.ToString();
        }	//	toString


        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Client_ID">parent</param>
        /// <param name="AD_Org_ID">parent</param>
        /// <param name="PA_ReportLine_ID">parent</param>
        /// <param name="source">copy source</param>
        /// <param name="trxName">transaction</param>
        /// <returns>Report Source</returns>
        public static MReportSource Copy(Ctx ctx, int AD_Client_ID, int AD_Org_ID,
            int PA_ReportLine_ID, MReportSource source, Trx trxName)
        {
            MReportSource retValue = new MReportSource(ctx, 0, trxName);
            MReportSource.CopyValues(source, retValue, AD_Client_ID, AD_Org_ID);
            retValue.SetPA_ReportLine_ID(PA_ReportLine_ID);
            return retValue;
        }	//	copy

    }	//	MReportSource

}
