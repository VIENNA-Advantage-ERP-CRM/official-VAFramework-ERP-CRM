/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MLdapAccess
 * Purpose        : MLdap Access Model
 * Class Used     : X_VAF_LdapRights
 * Chronological    Development
 * Deepak           03-Feb-2010
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


namespace VAdvantage.Model
{
    public class MVAFLdapRights : X_VAF_LdapRights
    {

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_LdapRights_ID">id</param>
        /// <param name="trxName">trx</param>
        public MVAFLdapRights(Ctx ctx, int VAF_LdapRights_ID, Trx trxName)
            : base(ctx, VAF_LdapRights_ID, trxName)
        {

        }	//	MLdapAccess

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">trx</param>
        public MVAFLdapRights(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }	//	MLdapAccess

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">trx</param>
        public MVAFLdapRights(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }	//	MLdapAccess


        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MLdapAccess[")
                .Append(Get_ID())
                .Append("]");

            return sb.ToString();
        }	//	toString

    }	//	MLdapAccess

}
