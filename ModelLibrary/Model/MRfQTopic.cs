/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRfQTopic
 * Purpose        : RfQ Topic Model
 * Class Used     : X_C_RfQ_Topic
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
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MRfQTopic : X_C_RfQ_Topic
    {

        /// <summary>
        /// 	Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_RfQ_Topic_ID"></param>
        /// <param name="trxName"></param>
        public MRfQTopic(Ctx ctx, int C_RfQ_Topic_ID, Trx trxName)
            : base(ctx, C_RfQ_Topic_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MRfQTopic(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// 	Get Current Topic Subscribers
        /// </summary>
        /// <returns>array subscribers</returns>
        public MRfQTopicSubscriber[] GetSubscribers()
        {
            List<MRfQTopicSubscriber> list = new List<MRfQTopicSubscriber>();
            String sql = "SELECT * FROM C_RfQ_TopicSubscriber "
                + "WHERE C_RfQ_Topic_ID=" + GetC_RfQ_Topic_ID() + " AND IsActive='Y'";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MRfQTopicSubscriber(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "getSubscribers", e);
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }

            MRfQTopicSubscriber[] retValue = new MRfQTopicSubscriber[list.Count];
            retValue = list.ToArray();
            return retValue;
        }
    }
}
