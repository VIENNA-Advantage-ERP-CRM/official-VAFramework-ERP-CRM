/***********************************************************************
 *	Window Access Log Model
 *	
 *  @author Vijaya
 *  @version $Id: MWindowLog.java,v 1.1 2008/09/10 07:14:15 vijaya.murugan Exp $
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.DataBase;
namespace VAdvantage.Model
{
    public class MWindowLog : X_AD_WindowLog
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_WindowLog_ID">id</param>
        /// <param name="trxName">trxName</param>
        public MWindowLog(Ctx ctx, int AD_WindowLog_ID, Trx trxName)
            : base(ctx, AD_WindowLog_ID, trxName)
        {

            if (AD_WindowLog_ID == 0)
            {
                int VAF_Role_ID = ctx.GetVAF_Role_ID();
                SetVAF_Role_ID(VAF_Role_ID);
            }
        }	//	MWindowLog

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MWindowLog(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }	//	MWindowLog


        /// <summary>
        /// 	Window Log
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Session_ID"></param>
        /// <param name="VAF_Client_ID"></param>
        /// <param name="VAF_Org_ID"></param>
        /// <param name="AD_Window_ID"></param>
        /// <param name="VAF_Page_ID"></param>
        public MWindowLog(Ctx ctx, int AD_Session_ID,
        int VAF_Client_ID, int VAF_Org_ID,
        int AD_Window_ID, int VAF_Page_ID)
            : this(ctx, 0, null)
        {

            SetAD_Session_ID(AD_Session_ID);
            SetClientOrg(VAF_Client_ID, VAF_Org_ID);
            //
            if (AD_Window_ID != 0)
                SetAD_Window_ID(AD_Window_ID);
            else if (VAF_Page_ID != 0)
                SetVAF_Page_ID(VAF_Page_ID);
            else
            log.Severe("No Window/Form");
        }

    }
}
