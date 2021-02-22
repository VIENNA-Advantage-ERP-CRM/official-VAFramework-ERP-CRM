/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MWSPSharedUser
 * Purpose        : Activity model.
 * Class Used     : MWSPSharedUser class
 * Chronological    Development
 * Deepak           17-July 2014
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
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MWSPSharedUser : X_WSP_SharedUser
    {


        public MWSPSharedUser(Ctx ctx, int WSP_SharedUser_ID, Trx trxName)
            : base(ctx, WSP_SharedUser_ID, trxName)
        {
            //super(ctx, VAB_BillingCode_ID, trxName);
        }	//	MWSPSharedUser


        public MWSPSharedUser(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MWSPSharedUser


    }
}