/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRfQResponseLine
 * Purpose        : RfQ Response Line Model
 * Class Used     : X_C_RfQResponseLine
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
    public class MRfQResponseLine : X_C_RfQResponseLine
    {

        //	RfQ Line				
        private MRfQLine _rfqLine = null;
        //	Quantities				
        private MRfQResponseLineQty[] _qtys = null;

        /// <summary>
        /// Persistency Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="ignored"></param>
        /// <param name="trxName"></param>
        public MRfQResponseLine(Ctx ctx, int ignored, Trx trxName)
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
        public MRfQResponseLine(Ctx ctx, DataRow dr, Trx trxName)
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
        public MRfQResponseLine(MRfQResponse response, MRfQLine line)
            : base(response.GetCtx(), 0, response.Get_TrxName())
        {
            SetClientOrg(response);
            SetC_RfQResponse_ID(response.GetC_RfQResponse_ID());
            //
            SetC_RfQLine_ID(line.GetC_RfQLine_ID());
            //
            SetIsSelectedWinner(false);
            SetIsSelfService(false);
            //
            MRfQLineQty[] qtys = line.GetQtys();
            for (int i = 0; i < qtys.Length; i++)
            {
                if (qtys[i].IsActive() && qtys[i].IsRfQQty())
                {
                    if (Get_ID() == 0)	//	save this line
                    {
                        Save();
                    }
                    MRfQResponseLineQty qty = new MRfQResponseLineQty(this, qtys[i]);
                    qty.Save();
                }
            }
        }

        /// <summary>
        /// Get Quantities
        /// </summary>
        /// <returns>array of quantities</returns>
        public MRfQResponseLineQty[] GetQtys()
        {
            return GetQtys(false);
        }

        /// <summary>
        /// Get Quantities
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>array of quantities</returns>
        public MRfQResponseLineQty[] GetQtys(bool requery)
        {
            if (_qtys != null && !requery)
            {
                return _qtys;
            }

            List<MRfQResponseLineQty> list = new List<MRfQResponseLineQty>();
            String sql = "SELECT * FROM C_RfQResponseLineQty "
                + "WHERE C_RfQResponseLine_ID=" + GetC_RfQResponseLine_ID() + " AND IsActive='Y'";
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
                    list.Add(new MRfQResponseLineQty(GetCtx(), dr, Get_TrxName()));
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

            _qtys = new MRfQResponseLineQty[list.Count];
            _qtys = list.ToArray();
            return _qtys;
        }

        /// <summary>
        /// Get RfQ Line 
        /// </summary>
        /// <returns>rfq line</returns>
        public MRfQLine GetRfQLine()
        {
            if (_rfqLine == null)
            {
                _rfqLine = MRfQLine.Get(GetCtx(), GetC_RfQLine_ID(), Get_TrxName());
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
                    MRfQResponseLineQty qty = _qtys[i];
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
