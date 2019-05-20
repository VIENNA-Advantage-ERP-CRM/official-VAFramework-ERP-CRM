/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRfQResponseLineQty
 * Purpose        : RfQ Response Line Qty
 * Class Used     : X_C_RfQResponseLineQty
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
    public class MRfQResponseLineQty : X_C_RfQResponseLineQty,IComparer<PO>
    {
        //	RfQ Line Qty	
        private MRfQLineQty _rfqQty = null;
        //100
        private const Decimal ONEHUNDRED = 100;

        /// <summary>
        /// Persistency Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_RfQResponseLineQty_ID">ID</param>
        /// <param name="trxName">Transaction</param>
        public MRfQResponseLineQty(Ctx ctx, int C_RfQResponseLineQty_ID, Trx trxName)
            : base(ctx, C_RfQResponseLineQty_ID, trxName)
        {
            if (C_RfQResponseLineQty_ID == 0)
            {
                //	setC_RfQResponseLineQty_ID (0);		//	PK
                //	setC_RfQLineQty_ID (0);
                //	setC_RfQResponseLine_ID (0);
                //
                SetPrice(Env.ZERO);
                SetDiscount(Env.ZERO);
            }

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MRfQResponseLineQty(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            
        }	

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="line"></param>
        /// <param name="qty"></param>
        public MRfQResponseLineQty(MRfQResponseLine line, MRfQLineQty qty)
            : this(line.GetCtx(), 0, line.Get_TrxName())
        {
            SetClientOrg(line);
            SetC_RfQResponseLine_ID(line.GetC_RfQResponseLine_ID());
            SetC_RfQLineQty_ID(qty.GetC_RfQLineQty_ID());
        }	

        /// <summary>
        /// Get RfQ Line Qty
        /// </summary>
        /// <returns>RfQ Line Qty</returns>
        public MRfQLineQty GetRfQLineQty()
        {
            if (_rfqQty == null)
            {
                _rfqQty = MRfQLineQty.Get(GetCtx(), GetC_RfQLineQty_ID(), Get_TrxName());
            }
            return _rfqQty;
        }

        /// <summary>
        /// Is the Amount (price - discount) Valid
        /// </summary>
        /// <returns>true if valid</returns>
        public bool IsValidAmt()
        {
            Decimal price = GetPrice();
            if ( Env.ZERO.CompareTo(price) == 0)
            {
                log.Warning("No Price - " + price);
                return false;
            }
            Decimal discount = GetDiscount();
            //if (discount != null)
            {
                if (Math.Abs(discount).CompareTo(ONEHUNDRED) > 0)
                {
                    log.Warning("Discount > 100 - " + discount);
                    return false;
                }
            }
            Decimal? net = GetNetAmt();
            if (net == null)
            {
                log.Warning("Net is null");
                return false;
            }
            if (net.Value.CompareTo(Env.ZERO) <= 0)
            {
                log.Warning("Net <= 0 - " + net);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get Net Amt (price minus discount in %)
        /// </summary>
        /// <returns>net amount or null</returns>
        public Decimal? GetNetAmt()
        {
            Decimal price = GetPrice();
            if ( Env.ZERO.CompareTo(price) == 0)
            {
                return null;
               // return 0;
            }
            //	
            Decimal discount = GetDiscount();
            if ( Env.ZERO.CompareTo(discount) == 0)
            {
                return price;
            }
            //	Calculate
            //	double result = price.doubleValue() * (100.0 - discount.doubleValue()) / 100.0;
            Decimal factor = Decimal.Subtract(ONEHUNDRED, discount);
            return Decimal.Round(Decimal.Divide(Decimal.Multiply(price, factor), ONEHUNDRED), 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MRfQResponseLineQty[");
            sb.Append(Get_ID()).Append(",Rank=").Append(GetRanking())
                .Append(",Price=").Append(GetPrice())
                .Append(",Discount=").Append(GetDiscount())
                .Append(",Net=").Append(GetNetAmt())
                .Append("]");
            return sb.ToString();
        }
        
        /// <summary>
        /// Compare based on net amount
        /// throws exception if the arguments' types prevent them from
        ///  being compared by this Comparator.
        /// </summary>
        /// <param name="o1">the first object to be compared.</param>
        /// <param name="o2">the second object to be compared.</param>
        /// <returns>a negative integer, zero, or a positive integer as the
        /// first argument is less than, equal to, or greater than the
        /// second. </returns>
        public new int Compare(PO o1, PO o2)
        {
            if (o1 == null)
            {
                throw new ArgumentException("o1 = null");
            }
            if (o2 == null)
            {
                throw new ArgumentException("o2 = null");
            }
            MRfQResponseLineQty q1 = null;
            MRfQResponseLineQty q2 = null;
            if (o1 is MRfQResponseLineQty)//instanceof
            {
                q1 = (MRfQResponseLineQty)o1;
            }
            else
            {
                throw new Exception("ClassCast--o1");
            }
            if (o2 is MRfQResponseLineQty)//instanceof
            {
                q2 = (MRfQResponseLineQty)o2;
            }
            else
            {
                throw new Exception("ClassCast--o2");
            }
            //
            if (!q1.IsValidAmt())
            {
                return -99;
            }
            if (!q2.IsValidAmt())
            {
                return +99;
            }
            Decimal? net1 = q1.GetNetAmt();
            if (net1 == null)
            {
                return -9;
            }
            Decimal? net2 = q2.GetNetAmt();
            if (net2 == null)
            {
                return +9;
            }
            return net1.Value.CompareTo(net2.Value);
        }

        /// <summary>
        /// Is Net Amount equal ?
        /// </summary>
        /// <param name="obj">the reference object with which to compare.</param>
        /// <returns>true if Net Amount equal</returns>
        public override bool Equals(Object obj)
        {
            if (obj is MRfQResponseLineQty)
            {
                MRfQResponseLineQty cmp = (MRfQResponseLineQty)obj;
                if (!cmp.IsValidAmt() || !IsValidAmt())
                {
                    return false;
                }
                Decimal? cmpNet = cmp.GetNetAmt();
                if (cmpNet == null)
                {
                    return false;
                }
                Decimal? net = cmp.GetNetAmt();
                if (net == null)
                {
                    return false;
                }
                return cmpNet.Value.CompareTo(net) == 0;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        /// <summary>
        /// 	Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (!IsActive())
            {
                SetRanking(999);
            }
            return true;
        }
    }
}
