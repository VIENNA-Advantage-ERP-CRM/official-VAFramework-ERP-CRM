/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MResourceUnAvailable
 * Purpose        : Resource Unavailable
 * Class Used     : X_S_ResourceUnAvailable
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
//using System.Windows.Forms;
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
    public class MResourceUnAvailable : X_S_ResourceUnAvailable
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="S_ResourceUnAvailable_ID">id</param>
        /// <param name="trxName">trx</param>
        public MResourceUnAvailable(Ctx ctx, int S_ResourceUnAvailable_ID, Trx trxName):base(ctx, S_ResourceUnAvailable_ID, trxName)
        {
            //super(ctx, S_ResourceUnAvailable_ID, trxName);
        }	//	MResourceUnAvailable

        /// <summary>
        /// MResourceUnAvailable
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">trx</param>
        public MResourceUnAvailable(Ctx ctx,DataRow dr, Trx trxName):base(ctx,dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MResourceUnAvailable


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

    }	//	MResourceUnAvailable

}
