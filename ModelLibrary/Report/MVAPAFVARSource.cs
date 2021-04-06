/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MReportSource
 * Purpose        : Report Source Model
 * Class Used     : X_VAPA_FVAR_Source
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
    public class MVAPAFVARSource : X_VAPA_FVAR_Source
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAPA_FVAR_Source_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAPAFVARSource(Ctx ctx, int VAPA_FVAR_Source_ID, Trx trxName):base(ctx, VAPA_FVAR_Source_ID, trxName)
        {
            
            if (VAPA_FVAR_Source_ID == 0)
            {
            }
        }	//	MReportSource

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MVAPAFVARSource(Ctx ctx,DataRow dr, Trx trxName): base(ctx, dr, trxName)
        {
           
        }	//	MReportSource


       /// <summary>
       ///  Get SQL where clause
       /// </summary>
       /// <param name="VAPA_FinancialReportingOrder_ID">hierarchy </param>
        /// <returns> where clause</returns>
        public String GetWhereClause(int VAPA_FinancialReportingOrder_ID)
        {
            String et = GetElementType();
            //	ID for Tree Leaf Value
            int ID = 0;
            //
            if (MVABAccountBookElement.ELEMENTTYPE_Account.Equals(et))
            {
                ID = GetVAB_Acct_Element_ID();
            }
            else if (MVABAccountBookElement.ELEMENTTYPE_Activity.Equals(et))
            {
                ID = GetVAB_BillingCode_ID();
            }
            else if (MVABAccountBookElement.ELEMENTTYPE_BPartner.Equals(et))
            {
                ID = GetVAB_BusinessPartner_ID();
            }
            else if (MVABAccountBookElement.ELEMENTTYPE_Campaign.Equals(et))
            {
                ID = GetVAB_Promotion_ID();
            }
            else if (MVABAccountBookElement.ELEMENTTYPE_LocationFrom.Equals(et))
            {
                ID = GetVAB_Address_ID();
            }
            else if (MVABAccountBookElement.ELEMENTTYPE_LocationTo.Equals(et))
            {
                ID = GetVAB_Address_ID();
            }
            else if (MVABAccountBookElement.ELEMENTTYPE_Organization.Equals(et))
            {
                ID = GetOrg_ID();
            }
            else if (MVABAccountBookElement.ELEMENTTYPE_Product.Equals(et))
            {
                ID = GetVAM_Product_ID();
            }
            else if (MVABAccountBookElement.ELEMENTTYPE_Project.Equals(et))
            {
                ID = GetVAB_Project_ID();
            }
            else if (MVABAccountBookElement.ELEMENTTYPE_SalesRegion.Equals(et))
            {
                ID = GetVAB_SalesRegionState_ID();
            }
            else if (MVABAccountBookElement.ELEMENTTYPE_OrgTrx.Equals(et))
            {
                ID = GetOrg_ID();	//	(re)uses Org_ID
            }
            else if (MVABAccountBookElement.ELEMENTTYPE_UserList1.Equals(et))
            {
                ID = GetVAB_Acct_Element_ID();
            }
            else if (MVABAccountBookElement.ELEMENTTYPE_UserList2.Equals(et))
            {
                ID = GetVAB_Acct_Element_ID();
            }
            //		else if (MAcctSchemaElement.ELEMENTTYPE_UserElement1.equals(et))
            //			ID = getUserElement1_ID ();
            //		else if (MAcctSchemaElement.ELEMENTTYPE_UserElement2.equals(et))
            //			ID = getUserElement2_ID ();
            if (ID == 0)
            {
                log.Fine("No Restrictions - No ID for RecordType=" + et);
                return "";
            }
            //
            return MReportTree.GetWhereClause(GetCtx(), VAPA_FinancialReportingOrder_ID, et, ID);
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
        /// <param name="VAF_Client_ID">parent</param>
        /// <param name="VAF_Org_ID">parent</param>
        /// <param name="VAPA_FR_Row_ID">parent</param>
        /// <param name="source">copy source</param>
        /// <param name="trxName">transaction</param>
        /// <returns>Report Source</returns>
        public static MVAPAFVARSource Copy(Ctx ctx, int VAF_Client_ID, int VAF_Org_ID,
            int VAPA_FR_Row_ID, MVAPAFVARSource source, Trx trxName)
        {
            MVAPAFVARSource retValue = new MVAPAFVARSource(ctx, 0, trxName);
            MVAPAFVARSource.CopyValues(source, retValue, VAF_Client_ID, VAF_Org_ID);
            retValue.SetVAPA_FR_Row_ID(VAPA_FR_Row_ID);
            return retValue;
        }	//	copy

    }	//	MReportSource

}
