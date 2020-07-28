/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MTaxDeclaration
 * Purpose        : Tax Declaration Model
 * Class Used     : X_C_TaxDeclaration class
 * Chronological    Development
 * Deepak           20-Nov-2009
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
    public class MTaxDeclaration : X_C_TaxDeclaration
    {
        /// <summary>
        /// Standard Constructors
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_TaxDeclaration_ID">id</param>
        /// <param name="trxName">trx</param>
        public MTaxDeclaration(Ctx ctx, int C_TaxDeclaration_ID, Trx trxName): base(ctx, C_TaxDeclaration_ID, trxName)
        {
           // super(ctx, C_TaxDeclaration_ID, trxName);
        }	//	MTaxDeclaration

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context </param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">trx</param>
        public MTaxDeclaration(Ctx ctx, DataRow dr, Trx trxName):base(ctx,dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MTaxDeclaration

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected Boolean beforeSave(Boolean newRecord)
        {
            if (Is_ValueChanged("DateFrom"))
            {
                SetDateFrom(TimeUtil.GetDay(GetDateFrom()));
            }
            if (Is_ValueChanged("DateTo"))
            {
                SetDateTo(TimeUtil.GetDay(GetDateTo()));
            }
            return true;
        }	//	beforeSave

    }	//	MTaxDeclaration

}
