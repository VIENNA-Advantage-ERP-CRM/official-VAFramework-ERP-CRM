/********************************************************
 * Project Name   : VAdvantage
 * Interface Name : ReplenishInterface
 * Purpose        : Custom Replenishment Interface
 * Class Used     : 
 * Chronological    Development
 * Deepak           13-Jan-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;

namespace VAdvantage.Interface
{
    public interface ReplenishInterface
    {
        /// <summary>
        ///Return the Qty To Order
	    /// </summary>
        /// <param name="wh">warehouse</param>
        /// <param name="replenish">temporary replenishment</param>
        /// <returns>qty to order</returns>

        Decimal GetQtyToOrder(VAdvantage.Model.MWarehouse wh, VAdvantage.Model.X_T_Replenish replenish);
    }
}
