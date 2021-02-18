/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRfQTopic
 * Purpose        : RfQ Topic Model
 * Class Used     : X_VAB_RFQ_Subject
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
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVABRFQSubject : X_VAB_RFQ_Subject
    {

        /// <summary>
        /// 	Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_RFQ_Subject_ID"></param>
        /// <param name="trxName"></param>
        public MVABRFQSubject(Ctx ctx, int VAB_RFQ_Subject_ID, Trx trxName)
            : base(ctx, VAB_RFQ_Subject_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MVABRFQSubject(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// 	Get Current Topic Subscribers
        /// </summary>
        /// <returns>array subscribers</returns>
        public MVABRFQSubjectMember[] GetSubscribers()
        {
            List<MVABRFQSubjectMember> list = new List<MVABRFQSubjectMember>();
            String sql = "SELECT * FROM VAB_RFQ_SubjectMember "
                + "WHERE VAB_RFQ_Subject_ID=" + GetVAB_RFQ_Subject_ID() + " AND IsActive='Y'";
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
                    list.Add(new MVABRFQSubjectMember(GetCtx(), dr, Get_TrxName()));
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

            MVABRFQSubjectMember[] retValue = new MVABRFQSubjectMember[list.Count];
            retValue = list.ToArray();
            return retValue;
        }
    }
}
