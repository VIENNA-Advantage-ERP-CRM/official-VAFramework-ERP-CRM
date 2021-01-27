/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : DocLine_Allocation
 * Purpose        : Allocation Line
 * Class Used     : DocLine
 * Chronological    Development
 * Raghunandan      20-Jan-2010
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using System.Data.SqlClient;
using VAdvantage.Acct;

namespace VAdvantage.Acct
{
    public class DocLine_Allocation : DocLine
    {

        private int _VAB_Invoice_ID;
        private int _VAB_Payment_ID;
        private int _VAB_CashJRNLLine_ID;
        private int _VAB_Order_ID;
        private Decimal _DiscountAmt;
        private Decimal _WriteOffAmt;
        private Decimal _OverUnderAmt;

        /// <summary>
        /// 	DocLine_Allocation
        /// </summary>
        /// <param name="line"></param>
        /// <param name="doc"></param>
        public DocLine_Allocation(MAllocationLine line, Doc doc)
            : base(line, doc)
        {

            _VAB_Payment_ID = line.GetVAB_Payment_ID();
            _VAB_CashJRNLLine_ID = line.GetVAB_CashJRNLLine_ID();
            _VAB_Invoice_ID = line.GetVAB_Invoice_ID();
            _VAB_Order_ID = line.GetVAB_Order_ID();
            //
            SetAmount(line.GetAmount());
            _DiscountAmt = line.GetDiscountAmt();
            _WriteOffAmt = line.GetWriteOffAmt();
            _OverUnderAmt = line.GetOverUnderAmt();
        }

        /// <summary>
        /// Get Invoice VAB_Currency_ID
        /// </summary>
        /// <returns>0 if no invoice -1 if not found</returns>
        public int GetInvoiceVAB_Currency_ID()
        {
            if (_VAB_Invoice_ID == 0)
            {
                return 0;
            }
            String sql = "SELECT VAB_Currency_ID "
                + "FROM VAB_Invoice "
                + "WHERE VAB_Invoice_ID=@param1";
            return DataBase.DB.GetSQLValue(null, sql, _VAB_Invoice_ID);
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("DocLine_Allocation[");
            sb.Append(Get_ID())
                .Append(",Amt=").Append(GetAmtSource())
                .Append(",Discount=").Append(GetDiscountAmt())
                .Append(",WriteOff=").Append(GetWriteOffAmt())
                .Append(",OverUnderAmt=").Append(GetOverUnderAmt())
                .Append(" - VAB_Payment_ID=").Append(_VAB_Payment_ID)
                .Append(",VAB_CashJRNLLine_ID=").Append(_VAB_CashJRNLLine_ID)
                .Append(",VAB_Invoice_ID=").Append(_VAB_Invoice_ID)
                .Append("]");
            return sb.ToString();
        }


        /// <summary>
        /// Returns the VAB_Order_ID.
        /// </summary>
        /// <returns>VAB_Order_ID.</returns>
        public int GetVAB_Order_ID()
        {
            return _VAB_Order_ID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the discountAmt</returns>
        public Decimal GetDiscountAmt()
        {
            return _DiscountAmt;
        }

        /// <summary>
        /// Returns the overUnderAmt.
        /// </summary>
        /// <returns></returns>
        public Decimal GetOverUnderAmt()
        {
            return _OverUnderAmt;
        }

        /// <summary>
        /// Returns the writeOffAmt.
        /// </summary>
        /// <returns></returns>
        public Decimal GetWriteOffAmt()
        {
            return _WriteOffAmt;
        }

        /// <summary>
        /// Returns the VAB_CashJRNLLine_ID.
        /// </summary>
        /// <returns></returns>
        public int GetVAB_CashJRNLLine_ID()
        {
            return _VAB_CashJRNLLine_ID;
        }

        /// <summary>
        /// Returns the VAB_Invoice_ID.
        /// </summary>
        /// <returns></returns>
        public int GetVAB_Invoice_ID()
        {
            return _VAB_Invoice_ID;
        }

        /// <summary>
        /// Returns the VAB_Payment_ID.
        /// </summary>
        /// <returns></returns>
        public int GetVAB_Payment_ID()
        {
            return _VAB_Payment_ID;
        }
    }
}
