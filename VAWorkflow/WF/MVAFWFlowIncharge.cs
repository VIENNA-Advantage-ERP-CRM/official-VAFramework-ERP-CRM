/********************************************************
 * Module Name    : Workflow
 * Purpose        : 
 * Class Used     : X_VAF_WFlow_Incharge
 * Chronological Development
 * Veena Pandey     02-May-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VAdvantage.WF
{
    public class MVAFWFlowIncharge : X_VAF_WFlow_Incharge
    {
        //	Cache
        private static CCache<int, MVAFWFlowIncharge> _cache = new CCache<int, MVAFWFlowIncharge>("VAF_WFlow_Incharge", 10);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_WFlow_Incharge_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowIncharge(Ctx ctx, int VAF_WFlow_Incharge_ID, Trx trxName)
            : base(ctx, VAF_WFlow_Incharge_ID, trxName)
        {
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowIncharge(Ctx ctx, System.Data.DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get WF Responsible from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_WFlow_Incharge_ID">id</param>
        /// <returns>MWFResponsible</returns>
        public static MVAFWFlowIncharge Get(Ctx ctx, int VAF_WFlow_Incharge_ID)
        {
            int key = VAF_WFlow_Incharge_ID;
            MVAFWFlowIncharge retValue = (MVAFWFlowIncharge)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVAFWFlowIncharge(ctx, VAF_WFlow_Incharge_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Is Role Responsible
        /// </summary>
        /// <returns>true if role</returns>
        public MVAFRole GetRole()
        {
            if (!IsRole())
                return null;
            return MVAFRole.Get(GetCtx(), GetVAF_Role_ID());
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true if can be saved</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	if (RESPONSIBLETYPE_Human.equals(getResponsibleType()) && getVAF_UserContact_ID() == 0)
            //		return true;
            if (RESPONSIBLETYPE_Role.Equals(GetResponsibleType())
                && GetVAF_Role_ID() == 0
                && GetVAF_Client_ID() > 0)
            {
                log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@RequiredEnter@ @VAF_Role_ID@"));
                return false;
            }
            //	User not used
            if (!RESPONSIBLETYPE_Human.Equals(GetResponsibleType()) && GetVAF_UserContact_ID() == 0)
                SetVAF_UserContact_ID(0);
            //	Role not used
            if (!RESPONSIBLETYPE_Role.Equals(GetResponsibleType()) && GetVAF_Role_ID() == 0)
                SetVAF_Role_ID(0);



            //Lakhwinder
            if (RESPONSIBLETYPE_Human.Equals(GetResponsibleType()))
            {
                if (GetVAF_UserContact_ID() == 0)
                {
                    return false;
                }
                SetVAF_Role_ID(0);
            }
            else if (RESPONSIBLETYPE_Role.Equals(GetResponsibleType()))
            {
                if (GetVAF_Role_ID() == 0)
                {
                    return false;
                }
                SetVAF_UserContact_ID(0);
            }
            else if (RESPONSIBLETYPE_Organization.Equals(GetResponsibleType()))
            {
                SetVAF_Role_ID(0);
                SetVAF_UserContact_ID(0);
            }
            //Lakhwinder
            return true;
        }

        /// <summary>
        /// Is Human Responsible
        /// </summary>
        /// <returns>true if human</returns>
        public bool IsHuman()
        {
            return RESPONSIBLETYPE_Human.Equals(GetResponsibleType())
                && GetVAF_UserContact_ID() != 0;
        }

        /// <summary>
        /// Is Org Responsible
        /// </summary>
        /// <returns>true if Org</returns>
        public bool IsOrganization()
        {
            return RESPONSIBLETYPE_Organization.Equals(GetResponsibleType());
            //&& GetVAF_Org_ID() != 0;
        }

        /// <summary>
        /// Is Custom Query
        /// </summary>
        /// <returns>true if Custom Query</returns>
        public bool IsSQL()
        {
            return RESPONSIBLETYPE_SQL.Equals(GetResponsibleType())
                && GetCustomSQL().Trim() != "";
        }

        /// <summary>
        /// Invoker - return true if no user and no role
        /// </summary>
        /// <returns>true if invoker</returns>
        public bool IsInvoker()
        {
            return GetVAF_UserContact_ID() == 0 && GetVAF_Role_ID() == 0
                && GetResponsibleType() != X_VAF_WFlow_Incharge.RESPONSIBLETYPE_SQL
                && GetResponsibleType() != X_VAF_WFlow_Incharge.RESPONSIBLETYPE_Organization;
        }

        /// <summary>
        /// Is Role Responsible
        /// </summary>
        /// <returns>true if role</returns>
        public bool IsRole()
        {
            return RESPONSIBLETYPE_Role.Equals(GetResponsibleType())
                && GetVAF_Role_ID() != 0;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MWFResponsible[");
            sb.Append(Get_ID())
                .Append("-").Append(GetName())
                .Append(",Type=").Append(GetResponsibleType());
            if (GetVAF_UserContact_ID() != 0)
                sb.Append(",VAF_UserContact_ID=").Append(GetVAF_UserContact_ID());
            if (GetVAF_Role_ID() != 0)
                sb.Append(",VAF_Role_ID=").Append(GetVAF_Role_ID());
            sb.Append("]");
            return sb.ToString();
        }
    }
}
