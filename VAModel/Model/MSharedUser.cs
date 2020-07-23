/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MSharedUser
 * Purpose        : Activity model.
 * Class Used     : MSharedUser class
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
    public class MSharedUser : X_WSP_SharedUser
    {


        public MSharedUser(Ctx ctx, int WSP_SharedUser_ID, Trx trxName)
            : base(ctx, WSP_SharedUser_ID, trxName)
        {
            //super(ctx, C_Activity_ID, trxName);
        }	//	MSharedUser


        public MSharedUser(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MSharedUser


    }
}