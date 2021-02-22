/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVASResUnavailable
 * Purpose        : Resource Unavailable
 * Class Used     : X_VAS_Res_Unavailable
 * Chronological    Development
 * Deepak           2-Jan-2010
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
////////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.Print;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MVASResUnavailable : X_VAS_Res_Unavailable
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAS_Res_Unavailable_ID">id</param>
        /// <param name="trxName">trx</param>
        public MVASResUnavailable(Ctx ctx, int VAS_Res_Unavailable_ID, Trx trxName):base(ctx, VAS_Res_Unavailable_ID, trxName)
        {
            //super(ctx, VAS_Res_Unavailable_ID, trxName);
        }	//	MVASResUnavailable

        /// <summary>
        /// MVASResUnavailable
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">trx</param>
        public MVASResUnavailable(Ctx ctx,DataRow dr, Trx trxName):base(ctx,dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MVASResUnavailable


        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <returns>ture</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            if (GetDateTo() == null)
            {
                SetDateTo(GetDateFrom());
            }
            if (GetDateFrom()>(GetDateTo()))
            { 
                log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@DateTo@ > @DateFrom@"));
                return false;
            }
            return true;
        }	//	beforeSave

    }	//	MVASResUnavailable

}
