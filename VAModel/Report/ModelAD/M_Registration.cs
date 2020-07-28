/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : M_Registration
 * Purpose        : System Registration Model
 * Class Used     : M_Registration inherits X_AD_Registration class
 * Chronological    Development
 * Raghunandan      14-May-2009
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
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class M_Registration : X_AD_Registration
    {
        /// <summary>
        ///Default Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Registration_ID">id</param>
        /// <param name="trxName">transaction</param>
        public M_Registration(Ctx ctx, int AD_Registration_ID, Trx trxName)
            : base(ctx, AD_Registration_ID, trxName)
        {
            SetAD_Client_ID(0);
            SetAD_Org_ID(0);
        }

        /// <summary>
        ///Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public M_Registration(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        ///Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true/false</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //MSystem system = MSystem.get(getCtx());
            //if (system.getName().equals("?")
            ///	|| system.getUserName().equals("?"))
            //{
            log.SaveError("Error", "Define System first");
            //    return false;
            //}
            return true;
        }

    }
}
