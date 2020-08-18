/********************************************************
 * Module Name    : Workflow
 * Purpose        : 
 * Class Used     : X_AD_WF_Responsible
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
    public class MWFResponsible : X_AD_WF_Responsible
    {
        //	Cache
        private static CCache<int, MWFResponsible> _cache = new CCache<int, MWFResponsible>("AD_WF_Responsible", 10);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_WF_Responsible_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MWFResponsible(Ctx ctx, int AD_WF_Responsible_ID, Trx trxName)
            : base(ctx, AD_WF_Responsible_ID, trxName)
        {
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MWFResponsible(Ctx ctx, System.Data.DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get WF Responsible from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_WF_Responsible_ID">id</param>
        /// <returns>MWFResponsible</returns>
        public static MWFResponsible Get(Ctx ctx, int AD_WF_Responsible_ID)
        {
            int key = AD_WF_Responsible_ID;
            MWFResponsible retValue = (MWFResponsible)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MWFResponsible(ctx, AD_WF_Responsible_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Is Role Responsible
        /// </summary>
        /// <returns>true if role</returns>
        public MRole GetRole()
        {
            if (!IsRole())
                return null;
            return MRole.Get(GetCtx(), GetAD_Role_ID());
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true if can be saved</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	if (RESPONSIBLETYPE_Human.equals(getResponsibleType()) && getAD_User_ID() == 0)
            //		return true;
            if (RESPONSIBLETYPE_Role.Equals(GetResponsibleType())
                && GetAD_Role_ID() == 0
                && GetAD_Client_ID() > 0)
            {
                log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@RequiredEnter@ @AD_Role_ID@"));
                return false;
            }
            //	User not used
            if (!RESPONSIBLETYPE_Human.Equals(GetResponsibleType()) && GetAD_User_ID() == 0)
                SetAD_User_ID(0);
            //	Role not used
            if (!RESPONSIBLETYPE_Role.Equals(GetResponsibleType()) && GetAD_Role_ID() == 0)
                SetAD_Role_ID(0);



            //Lakhwinder
            if (RESPONSIBLETYPE_Human.Equals(GetResponsibleType()))
            {
                if (GetAD_User_ID() == 0)
                {
                    return false;
                }
                SetAD_Role_ID(0);
            }
            else if (RESPONSIBLETYPE_Role.Equals(GetResponsibleType()))
            {
                if (GetAD_Role_ID() == 0)
                {
                    return false;
                }
                SetAD_User_ID(0);
            }
            else if (RESPONSIBLETYPE_Organization.Equals(GetResponsibleType()))
            {
                SetAD_Role_ID(0);
                SetAD_User_ID(0);
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
                && GetAD_User_ID() != 0;
        }

        /// <summary>
        /// Is Org Responsible
        /// </summary>
        /// <returns>true if Org</returns>
        public bool IsOrganization()
        {
            return RESPONSIBLETYPE_Organization.Equals(GetResponsibleType())
                && GetAD_Org_ID() != 0;
        }

        /// <summary>
        /// Invoker - return true if no user and no role
        /// </summary>
        /// <returns>true if invoker</returns>
        public bool IsInvoker()
        {
            return GetAD_User_ID() == 0 && GetAD_Role_ID() == 0;
        }

        /// <summary>
        /// Is Role Responsible
        /// </summary>
        /// <returns>true if role</returns>
        public bool IsRole()
        {
            return RESPONSIBLETYPE_Role.Equals(GetResponsibleType())
                && GetAD_Role_ID() != 0;
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
            if (GetAD_User_ID() != 0)
                sb.Append(",AD_User_ID=").Append(GetAD_User_ID());
            if (GetAD_Role_ID() != 0)
                sb.Append(",AD_Role_ID=").Append(GetAD_Role_ID());
            sb.Append("]");
            return sb.ToString();
        }
    }
}
