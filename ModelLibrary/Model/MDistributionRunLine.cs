/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDistributionRunLine
 * Purpose        :Distribution Run List Line Model
 * Class Used     : X_M_DistributionRunLine
 * Chronological    Development
 * Raghunandan     03-Nov-2009
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
using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MDistributionRunLine : X_M_DistributionRunLine
    {
        #region private variable
        //Product						
        private MProduct _product = null;
        //	Actual Qty					
        private Decimal _actualQty = Env.ZERO;
        //	Actual Min					
        private Decimal _actualMin = Env.ZERO;
        //	Actual Allocation			
        private Decimal _actualAllocation = Env.ZERO;
        //	Last Allocation Difference	
        private Decimal _lastDifference = Env.ZERO;
        //	Max Allocation 				
        private Decimal _maxAllocation = Env.ZERO;
        #endregion

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_DistributionRunLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MDistributionRunLine(Ctx ctx, int M_DistributionRunLine_ID, Trx trxName)
            : base(ctx, M_DistributionRunLine_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MDistributionRunLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Get Actual Qty
        /// </summary>
        /// <returns>actual Qty</returns>
        public Decimal GetActualQty()
        {
            return _actualQty;
        }

        /// <summary>
        /// Add to Actual Qty
        /// </summary>
        /// <param name="add">add number to add</param>
        public void AddActualQty(Decimal add)
        {
            _actualQty = Decimal.Add(_actualQty, add);
        }

        /// <summary>
        /// Get Actual Min Qty
        /// </summary>
        /// <returns>actual Min Qty</returns>
        public Decimal GetActualMin()
        {
            return _actualMin;
        }

        /// <summary>
        /// Add to Actual Min Qty
        /// </summary>
        /// <param name="add">number to add</param>
        public void AddActualMin(Decimal add)
        {
            _actualMin = Decimal.Add(_actualMin, add);
        }

        /// <summary>
        /// Is Actual Min Greater than Total
        /// </summary>
        /// <returns>true if act min > total</returns>
        public bool IsActualMinGtTotal()
        {
            return _actualMin.CompareTo(GetTotalQty()) > 0;
        }

        /// <summary>
        /// Get Actual Allocation Qty
        /// </summary>
        /// <returns>actual Allocation Qty</returns>
        public Decimal GetActualAllocation()
        {
            return _actualAllocation;
        }

        /// <summary>
        /// Add to Actual Min Qty
        /// </summary>
        /// <param name="add">number to add</param>
        public void AddActualAllocation(Decimal add)
        {
            _actualAllocation = Decimal.Add(_actualAllocation, add);
        }

        /// <summary>
        /// Is Actual Allocation equals Total
        /// </summary>
        /// <returns>true if act allocation = total</returns>
        public bool IsActualAllocationEqTotal()
        {
            return _actualAllocation.CompareTo(GetTotalQty()) == 0;
        }

        /// <summary>
        /// Get Allocation Difference
        /// </summary>
        /// <returns>Total - Allocation Qty</returns>
        public Decimal GetActualAllocationDiff()
        {
            return Decimal.Subtract(GetTotalQty(), _actualAllocation);
        }

        /// <summary>
        /// Get Last Allocation Difference
        /// </summary>
        /// <returns>difference</returns>
        public Decimal GetLastDifference()
        {
            return _lastDifference;
        }

        /// <summary>
        /// Set Last Allocation Difference
        /// </summary>
        /// <param name="difference">difference</param>
        public void SetLastDifference(Decimal difference)
        {
            _lastDifference = difference;
        }

        /// <summary>
        /// Get Max Allocation
        /// </summary>
        /// <returns>max allocation</returns>
        public Decimal GetMaxAllocation()
        {
            return _maxAllocation;
        }

        /// <summary>
        /// Set Max Allocation if greater
        /// </summary>
        /// <param name="max">allocation</param>
        /// <param name="set">set to max</param>
        public void SetMaxAllocation(Decimal max, bool set)
        {
            if (set || max.CompareTo(_maxAllocation) > 0)
            {
                _maxAllocation = max;
            }
        }

        /// <summary>
        /// Reset Calculations
        /// </summary>
        public void ResetCalculations()
        {
            _actualQty = Env.ZERO;
            _actualMin = Env.ZERO;
            _actualAllocation = Env.ZERO;
            //	_lastDifference = Env.ZERO;
            _maxAllocation = Env.ZERO;

        }

        /// <summary>
        /// Get Product
        /// </summary>
        /// <returns>product</returns>
        public MProduct GetProduct()
        {
            if (_product == null)
            {
                _product = MProduct.Get(GetCtx(), GetM_Product_ID());
            }
            return _product;
        }

        /// <summary>
        /// Get Product Standard Precision
        /// </summary>
        /// <returns>standard precision</returns>
        public int GetUOMPrecision()
        {
            return GetProduct().GetUOMPrecision();
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MDistributionRunLine[")
                .Append(Get_ID()).Append("-")
                .Append(GetInfo())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Get Info
        /// </summary>
        /// <returns>info</returns>
        public String GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Line=").Append(GetLine())
                .Append(",TotalQty=").Append(GetTotalQty())
                .Append(",SumMin=").Append(GetActualMin())
                .Append(",SumQty=").Append(GetActualQty())
                .Append(",SumAllocation=").Append(GetActualAllocation())
                .Append(",MaxAllocation=").Append(GetMaxAllocation())
                .Append(",LastDiff=").Append(GetLastDifference());
            return sb.ToString();
        }
    }
}
