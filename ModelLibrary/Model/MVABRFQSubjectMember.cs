﻿/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRfQTopicSubscriber
 * Purpose        : RfQ Topic Subscriber Model
 * Class Used     : X_VAB_RFQLine
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
    public class MVABRFQSubjectMember : X_VAB_RFQ_SubjectMember
    {
        //Restrictions					
        private MVABRFQSubjectMemAllow[] _restrictions = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_RFQ_SubjectMember_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVABRFQSubjectMember(Ctx ctx, int VAB_RFQ_SubjectMember_ID, Trx trxName)
            : base(ctx, VAB_RFQ_SubjectMember_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MVABRFQSubjectMember(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Get Restriction Records
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>arry of onlys</returns>
        public MVABRFQSubjectMemAllow[] GetRestrictions(bool requery)
        {
            if (_restrictions != null && !requery)
            {
                return _restrictions;
            }

            List<MVABRFQSubjectMemAllow> list = new List<MVABRFQSubjectMemAllow>();
            String sql = "SELECT * FROM VAB_RFQ_SubjectMem_Allow WHERE VAB_RFQ_SubjectMember_ID=" + GetVAB_RFQ_SubjectMember_ID();
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
                    list.Add(new MVABRFQSubjectMemAllow(GetCtx(), dr, Get_TrxName()));
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

            _restrictions = new MVABRFQSubjectMemAllow[list.Count];
            _restrictions = list.ToArray();
            return _restrictions;
        }

        /// <summary>
        /// Is the product included?
        /// </summary>
        /// <param name="VAM_Product_ID">product</param>
        /// <returns>true if no restrictions or included in "positive" only list</returns>
        public bool IsIncluded(int VAM_Product_ID)
        {
            //	No restrictions
            if (GetRestrictions(false).Length == 0)
            {
                return true;
            }

            for (int i = 0; i < _restrictions.Length; i++)
            {
                MVABRFQSubjectMemAllow restriction = _restrictions[i];
                if (!restriction.IsActive())
                {
                    continue;
                }
                //	Product
                if (restriction.GetVAM_Product_ID() == VAM_Product_ID)
                {
                    return true;
                }
                //	Product Category
                if (MVAMProductCategory.IsCategory(restriction.GetVAM_ProductCategory_ID(), VAM_Product_ID))
                {
                    return true;
                }
            }
            //	must be on "positive" list
            return false;
        }
    }
}