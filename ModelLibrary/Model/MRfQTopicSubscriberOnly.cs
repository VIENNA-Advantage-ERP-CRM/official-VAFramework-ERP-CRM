/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRfQTopicSubscriberOnly
 * Purpose        : Subcriber Topic Only List (positive - i.e. must be a match if exists)
 * Class Used     : X_C_RfQ_TopicSubscriberOnly
 * Chronological    Development
 * Raghunandan     11-Aug.-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
namespace VAdvantage.Model
{
    public class MRfQTopicSubscriberOnly : X_C_RfQ_TopicSubscriberOnly
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_RfQ_TopicSubscriberOnly_ID"></param>
        /// <param name="trxName"></param>
        public MRfQTopicSubscriberOnly(Ctx ctx, int C_RfQ_TopicSubscriberOnly_ID, Trx trxName)
            : base(ctx, C_RfQ_TopicSubscriberOnly_ID, trxName)
        {
            
        }	

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MRfQTopicSubscriberOnly(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            
        }
    }
}
