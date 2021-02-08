/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : OrderBatchProcess
 * Purpose        : Order Batch Processing
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     31-Oct-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
//using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class OrderBatchProcess : ProcessEngine.SvrProcess
    {
        private int _VAB_DocTypesTarget_ID = 0;
        private String _DocStatus = null;
        private int _VAB_BusinessPartner_ID = 0;
        private String _IsSelfService = null;
        private DateTime? _DateOrdered_From = null;
        private DateTime? _DateOrdered_To = null;
        private String _DocAction = null;

        /// <summary>
        /// Prepare
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("VAB_DocTypesTarget_ID"))
                {
                    _VAB_DocTypesTarget_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("DocStatus"))
                {
                    _DocStatus = (String)para[i].GetParameter();
                }
                else if (name.Equals("IsSelfService"))
                {
                    _IsSelfService = (String)para[i].GetParameter();
                }
                else if (name.Equals("VAB_BusinessPartner_ID"))
                {
                    _VAB_BusinessPartner_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("DateOrdered"))
                {
                    _DateOrdered_From = (DateTime)para[i].GetParameter();
                    _DateOrdered_To = (DateTime)para[i].GetParameter_To();
                }
                else if (name.Equals("DocAction"))
                {
                    _DocAction = (String)para[i].GetParameter();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>msg</returns>
        protected override String DoIt()
        {
            log.Info("VAB_DocTypesTarget_ID=" + _VAB_DocTypesTarget_ID + ", DocStatus=" + _DocStatus
                + ", IsSelfService=" + _IsSelfService + ", VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID
                + ", DateOrdered=" + _DateOrdered_From + "->" + _DateOrdered_To
                + ", DocAction=" + _DocAction);

            if (_VAB_DocTypesTarget_ID == 0)
            {
                throw new Exception("@NotFound@: @VAB_DocTypesTarget_ID@");
            }
            if (_DocStatus == null || _DocStatus.Length != 2)
            {
                throw new Exception("@NotFound@: @DocStatus@");
            }
            if (_DocAction == null || _DocAction.Length != 2)
            {
                throw new Exception("@NotFound@: @DocAction@");
            }

            //
            StringBuilder sql = new StringBuilder("SELECT * FROM VAB_Order "
                + "WHERE VAB_DocTypesTarget_ID=" + _VAB_DocTypesTarget_ID + " AND DocStatus='" + _DocStatus + "'");
            if (_IsSelfService != null && _IsSelfService.Length == 1)
            {
                sql.Append(" AND IsSelfService='").Append(_IsSelfService).Append("'");
            }
            if (_VAB_BusinessPartner_ID != 0)
            {
                sql.Append(" AND VAB_BusinessPartner_ID=").Append(_VAB_BusinessPartner_ID);
            }
            if (_DateOrdered_From != null)
            {
                sql.Append(" AND TRUNC(DateOrdered,'DD') >= ").Append(DataBase.DB.TO_DATE(_DateOrdered_From, true));
            }
            if (_DateOrdered_To != null)
            {
                sql.Append(" AND TRUNC(DateOrdered,'DD') <= ").Append(DataBase.DB.TO_DATE(_DateOrdered_To, true));
            }

            int counter = 0;
            int errCounter = 0;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    if (Process(new MVABOrder(GetCtx(), dr, Get_TrxName())))
                    {
                        counter++;
                    }
                    else
                    {
                        errCounter++;
                    }
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }

            return "@Updated@=" + counter + ", @Errors@=" + errCounter;
        }

        /// <summary>
        /// Process Order
        /// </summary>
        /// <param name="order">order</param>
        /// <returns>true if ok</returns>
        private bool Process(MVABOrder order)
        {
            log.Info(order.ToString());
            //
            order.SetDocAction(_DocAction);
            if (order.ProcessIt(_DocAction))
            {
                order.Save();
                AddLog(0, null, null, order.GetDocumentNo() + ": OK");
                return true;
            }
            AddLog(0, null, null, order.GetDocumentNo() + ": Error " + order.GetProcessMsg());
            return false;
        }
    }
}
