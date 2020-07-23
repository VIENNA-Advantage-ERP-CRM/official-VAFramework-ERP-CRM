/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRfQTopicSubscriber
 * Purpose        : RfQ Topic Subscriber Model
 * Class Used     : X_C_RfQLine
 * Chronological    Development
 * Raghunandan     10-Aug.-2009
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
    public class MRfQTopicSubscriber : X_C_RfQ_TopicSubscriber
    {
        //Restrictions					
        private MRfQTopicSubscriberOnly[] _restrictions = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_RfQ_TopicSubscriber_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MRfQTopicSubscriber(Ctx ctx, int C_RfQ_TopicSubscriber_ID, Trx trxName)
            : base(ctx, C_RfQ_TopicSubscriber_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MRfQTopicSubscriber(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Get Restriction Records
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>arry of onlys</returns>
        public MRfQTopicSubscriberOnly[] GetRestrictions(bool requery)
        {
            if (_restrictions != null && !requery)
            {
                return _restrictions;
            }

            List<MRfQTopicSubscriberOnly> list = new List<MRfQTopicSubscriberOnly>();
            String sql = "SELECT * FROM C_RfQ_TopicSubscriberOnly WHERE C_RfQ_TopicSubscriber_ID=" + GetC_RfQ_TopicSubscriber_ID();
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
                    list.Add(new MRfQTopicSubscriberOnly(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }

            _restrictions = new MRfQTopicSubscriberOnly[list.Count];
            _restrictions = list.ToArray();
            return _restrictions;
        }

        /// <summary>
        /// Is the product included?
        /// </summary>
        /// <param name="M_Product_ID">product</param>
        /// <returns>true if no restrictions or included in "positive" only list</returns>
        public bool IsIncluded(int M_Product_ID)
        {
            //	No restrictions
            if (GetRestrictions(false).Length == 0)
            {
                return true;
            }

            for (int i = 0; i < _restrictions.Length; i++)
            {
                MRfQTopicSubscriberOnly restriction = _restrictions[i];
                if (!restriction.IsActive())
                {
                    continue;
                }
                //	Product
                if (restriction.GetM_Product_ID() == M_Product_ID)
                {
                    return true;
                }
                //	Product Category
                if (MProductCategory.IsCategory(restriction.GetM_Product_Category_ID(), M_Product_ID))
                {
                    return true;
                }
            }
            //	must be on "positive" list
            return false;
        }
    }
}
