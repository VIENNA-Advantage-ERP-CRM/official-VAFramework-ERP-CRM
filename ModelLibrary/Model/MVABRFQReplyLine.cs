/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRfQResponseLine
 * Purpose        : RfQ Response Line Model
 * Class Used     : X_VAB_RFQReplyLine
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
    public class MVABRFQReplyLine : X_VAB_RFQReplyLine
    {

        //	RfQ Line				
        private MVABRfQLine _rfqLine = null;
        //	Quantities				
        private MVABRFQReplyLineQty[] _qtys = null;

        /// <summary>
        /// Persistency Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="ignored"></param>
        /// <param name="trxName"></param>
        public MVABRFQReplyLine(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, 0, trxName)
        {
            if (ignored != 0)
            {
                throw new ArgumentException("Multi-Key");
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MVABRFQReplyLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor.
        /// Also creates qtys if RfQ Qty
        /// Is saved if there are qtys(!)
        /// </summary>
        /// <param name="response">response</param>
        /// <param name="line">line</param>
        public MVABRFQReplyLine(MVABRFQReply response, MVABRfQLine line)
            : base(response.GetCtx(), 0, response.Get_TrxName())
        {
            SetClientOrg(response);
            SetVAB_RFQReply_ID(response.GetVAB_RFQReply_ID());
            //
            SetVAB_RFQLine_ID(line.GetVAB_RFQLine_ID());
            //
            SetIsSelectedWinner(false);
            SetIsSelfService(false);
            //
            MVABRfQLineQty[] qtys = line.GetQtys();
            for (int i = 0; i < qtys.Length; i++)
            {
                if (qtys[i].IsActive() && qtys[i].IsRfQQty())
                {
                    if (Get_ID() == 0)	//	save this line
                    {
                        Save();
                    }
                    MVABRFQReplyLineQty qty = new MVABRFQReplyLineQty(this, qtys[i]);
                    qty.Save();
                }
            }
        }

        /// <summary>
        /// Get Quantities
        /// </summary>
        /// <returns>array of quantities</returns>
        public MVABRFQReplyLineQty[] GetQtys()
        {
            return GetQtys(false);
        }

        /// <summary>
        /// Get Quantities
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>array of quantities</returns>
        public MVABRFQReplyLineQty[] GetQtys(bool requery)
        {
            if (_qtys != null && !requery)
            {
                return _qtys;
            }

            List<MVABRFQReplyLineQty> list = new List<MVABRFQReplyLineQty>();
            String sql = "SELECT * FROM VAB_RFQReplyLineQty "
                + "WHERE VAB_RFQReplyLine_ID=" + GetVAB_RFQReplyLine_ID() + " AND IsActive='Y'";
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
                    list.Add(new MVABRFQReplyLineQty(GetCtx(), dr, Get_TrxName()));
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

            _qtys = new MVABRFQReplyLineQty[list.Count];
            _qtys = list.ToArray();
            return _qtys;
        }

        /// <summary>
        /// Get RfQ Line 
        /// </summary>
        /// <returns>rfq line</returns>
        public MVABRfQLine GetRfQLine()
        {
            if (_rfqLine == null)
            {
                _rfqLine = MVABRfQLine.Get(GetCtx(), GetVAB_RFQLine_ID(), Get_TrxName());
            }
            return _rfqLine;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MRfQResponseLine[");
            sb.Append(Get_ID()).Append(",Winner=").Append(IsSelectedWinner())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// 	Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            try
            {
                //	Calculate Complete Date (also used to verify)
                if (GetDateWorkStart() != null && GetDeliveryDays() != 0)
                {
                    SetDateWorkComplete(TimeUtil.AddDays(GetDateWorkStart(), GetDeliveryDays()));
                }
                //	Calculate Delivery Days
                else if (GetDateWorkStart() != null && GetDeliveryDays() == 0 && GetDateWorkComplete() != null)
                {
                    SetDeliveryDays(TimeUtil.GetDaysBetween(GetDateWorkStart(), GetDateWorkComplete()));
                }
                //	Calculate Start Date
                else if (GetDateWorkStart() == null && GetDeliveryDays() != 0 && GetDateWorkComplete() != null)
                {
                    SetDateWorkStart(TimeUtil.AddDays(GetDateWorkComplete(), GetDeliveryDays() * -1));
                }

                if (!IsActive())
                {
                    SetIsSelectedWinner(false);
                }
            }
            catch(Exception e)
            {
               // MessageBox.Show(e.ToString());
                log.Severe(e.ToString());
            }
            return true;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!IsActive())
            {
                GetQtys(false);
                for (int i = 0; i < _qtys.Length; i++)
                {
                    MVABRFQReplyLineQty qty = _qtys[i];
                    if (qty.IsActive())
                    {
                        qty.SetIsActive(false);
                        qty.Save();
                    }
                }
            }
            return success;
        }
    }
}
