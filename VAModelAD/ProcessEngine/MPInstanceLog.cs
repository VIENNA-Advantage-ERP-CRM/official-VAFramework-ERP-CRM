/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MPInstanceLog
 * Purpose        : Process Instance Log model.
 *              	(not standard table)
 * Class Used     : None
 * Chronological    Development
 * Raghunandan     27-Oct-2009
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
using VAdvantage.Logging;
using System.IO;

using VAdvantage.ProcessEngine;
namespace VAdvantage.ProcessEngine
{
    public class MPInstanceLog
    {
        #region private Variables
        private int _AD_PInstance_ID;
        private int _Log_ID;
        private DateTime? _P_Date;
        private int _P_ID;
        private Decimal _P_Number;
        private String _P_Msg;
        #endregion

        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="AD_PInstance_ID">instance</param>
        /// <param name="Log_ID">log sequence</param>
        /// <param name="P_Date">date</param>
        /// <param name="P_ID">id</param>
        /// <param name="P_Number">number</param>
        /// <param name="msg">msg</param>
        public MPInstanceLog(int AD_PInstance_ID, int Log_ID, DateTime P_Date,
          int P_ID, Decimal P_Number, String msg)
        {
            SetAD_PInstance_ID(AD_PInstance_ID);
            SetLog_ID(Log_ID);
            SetP_Date(P_Date);
            SetP_ID(P_ID);
            SetP_Number(P_Number);
            SetP_Msg(msg);
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="dr">Data row set</param>
        public MPInstanceLog(DataRow dr)
        {
            SetAD_PInstance_ID(Utility.Util.GetValueOfInt(dr["AD_PInstance_ID"]));
            SetLog_ID(Utility.Util.GetValueOfInt(dr["Log_ID"]));
            SetP_Date(Utility.Util.GetValueOfDateTime(dr["P_Date"]));
            SetP_ID(Utility.Util.GetValueOfInt(dr["P_ID"]));
            SetP_Number(Utility.Util.GetValueOfDecimal(dr["P_Number"]));
            SetP_Msg(dr["msg"].ToString());
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("PPInstance_Log[");
            sb.Append(_Log_ID);
            if (_P_Date != null)
            {
                sb.Append(",Date=").Append(_P_Date);
            }
            if (_P_ID != 0)
            {
                sb.Append(",ID=").Append(_P_ID);
            }
            //if (_P_Number != null)
            {
                sb.Append(",Number=").Append(_P_Number);
            }
            if (_P_Msg != null)
            {
                sb.Append(",").Append(_P_Msg);
            }
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Save to Database
        /// </summary>
        /// <returns>true if saved</returns>
        public bool Save()
        {
            StringBuilder sql = new StringBuilder("INSERT INTO AD_PInstance_Log "
                + "(AD_PInstance_ID, Log_ID, P_Date, P_ID, P_Number, msg)"
                + " VALUES (");
            sql.Append(_AD_PInstance_ID).Append(",")
              .Append(_Log_ID).Append(",");
            if (_P_Date == null)
            {
                sql.Append("NULL,");
            }
            else
            {
                sql.Append(DataBase.DB.TO_DATE(_P_Date, false)).Append(",");
            }
            if (_P_ID == 0)
            {
                sql.Append("NULL,");
            }
            else
            {
                sql.Append(_P_ID).Append(",");
            }
            //if (_P_Number == null)
            //{
            //    sql.Append("NULL,");
            //}
            //else
            //{
                sql.Append(_P_Number).Append(",");
            //}
            if (_P_Msg == null)
            {
                sql.Append("NULL)");
            }
            else
            {
                sql.Append(DataBase.DB.TO_STRING(_P_Msg, 2000)).Append(")");
                //
            }
            int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, null);	//	outside of trx
            return no == 1;
        }

        /// <summary>
        /// Get AD_PInstance_ID
        /// </summary>
        /// <returns>Instance id</returns>
        public int GetAD_PInstance_ID()
        {
            return _AD_PInstance_ID;
        }

        /// <summary>
        /// Set AD_PInstance_ID
        /// </summary>
        /// <param name="AD_PInstance_ID">instance id</param>
        public void SetAD_PInstance_ID(int AD_PInstance_ID)
        {
            _AD_PInstance_ID = AD_PInstance_ID;
        }

        /// <summary>
        /// Get Log_ID
        /// </summary>
        /// <returns>id</returns>
        public int GetLog_ID()
        {
            return _Log_ID;
        }

        /// <summary>
        /// Set Log_ID
        /// </summary>
        /// <param name="Log_ID">id</param>
        public void SetLog_ID(int Log_ID)
        {
            _Log_ID = Log_ID;
        }

        /// <summary>
        /// Get P_Date
        /// </summary>
        /// <returns>date</returns>
        public DateTime? GetP_Date()
        {
            return _P_Date;
        }

        /// <summary>
        /// Set P_Date
        /// </summary>
        /// <param name="P_Date">date</param>
        public void SetP_Date(DateTime? P_Date)
        {
            _P_Date = P_Date;
        }

        /// <summary>
        /// Get P_ID
        /// </summary>
        /// <returns>id</returns>
        public int GetP_ID()
        {
            return _P_ID;
        }

        /// <summary>
        /// Set P_ID
        /// </summary>
        /// <param name="P_ID">id</param>
        public void SetP_ID(int P_ID)
        {
            _P_ID = P_ID;
        }

        /// <summary>
        /// Get P_Number
        /// </summary>
        /// <returns>number</returns>
        public Decimal GetP_Number()
        {
            return _P_Number;
        }

        /// <summary>
        /// Set P_Number
        /// </summary>
        /// <param name="P_Number">number</param>
        public void SetP_Number(Decimal P_Number)
        {
            _P_Number = P_Number;
        }

        /// <summary>
        /// Get msg
        /// </summary>
        /// <returns>Mag</returns>
        public String GetP_Msg()
        {
            return _P_Msg;
        }

        /// <summary>
        /// Set msg
        /// </summary>
        /// <param name="msg">msg</param>
        public void SetP_Msg(String msg)
        {
            _P_Msg = msg;
        }

    }
}
