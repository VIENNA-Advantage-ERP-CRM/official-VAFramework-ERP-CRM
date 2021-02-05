/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MLandedCostAllocation
 * Purpose        : For landed cost allocation
 * Class Used     : X_VAB_LCostDistribution
 * Chronological    Development
 * Raghunandan     22-Jun-2009
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
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MLandedCostAllocation : X_VAB_LCostDistribution
    {
        /**
	 * 	Get Cost Allocations for invoice Line
	 *	@param ctx context
	 *	@param VAB_InvoiceLine_ID invoice line
	 *	@param trxName trx
	 *	@return landed cost alloc
	 */
        public static MLandedCostAllocation[] GetOfInvoiceLine(Ctx ctx,
            int VAB_InvoiceLine_ID, Trx trxName)
        {
            List<MLandedCostAllocation> list = new List<MLandedCostAllocation>();
            String sql = "SELECT * FROM VAB_LCostDistribution WHERE VAB_InvoiceLine_ID= " + VAB_InvoiceLine_ID;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MLandedCostAllocation(ctx, dr, trxName));
                }

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }


            MLandedCostAllocation[] retValue = new MLandedCostAllocation[list.Count];
            retValue = list.ToArray();
            return retValue;
        }	//	getOfInvliceLine

        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MLandedCostAllocation).FullName);


        /***************************************************************************
         * 	Standard Constructor
         *	@param ctx context
         *	@param VAB_LCostDistribution_ID id
         *	@param trxName trx
         */
        public MLandedCostAllocation(Ctx ctx, int VAB_LCostDistribution_ID, Trx trxName) :
            base(ctx, VAB_LCostDistribution_ID, trxName)
        {

            if (VAB_LCostDistribution_ID == 0)
            {
                //	setVAM_ProductCostElement_ID(0);
                SetAmt(Env.ZERO);
                SetQty(Env.ZERO);
                SetBase(Env.ZERO);
            }
        }	//	MLandedCostAllocation

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result name
         *	@param trxName trx
         */
        public MLandedCostAllocation(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //super ();
        }	//	MLandedCostAllocation


        /**
         * 	Parent Constructor
         *	@param parent parent
         *	@param VAM_ProductCostElement_ID cost element
         */
        public MLandedCostAllocation(MInvoiceLine parent, int VAM_ProductCostElement_ID)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {

            SetClientOrg(parent);
            SetVAB_InvoiceLine_ID(parent.GetVAB_InvoiceLine_ID());
            SetVAM_ProductCostElement_ID(VAM_ProductCostElement_ID);
        }	//	MLandedCostAllocation

        /**
         * 	Set Amt
         *	@param Amt amount
         *	@param precision precision
         */
        public void SetAmt(double Amt, int precision)
        {

            Decimal bd = new Decimal(Amt);
            if (Env.Scale(bd) > precision)
            {//			bd = bd.setScale(precision, BigDecimal.ROUND_HALF_UP);
                bd = Decimal.Round(bd, precision, MidpointRounding.AwayFromZero);
            }
            base.SetAmt(bd);
        }	//	setAmt

        /**
         * 	Set Allocation Qty (e.g. free products)
         *	@param Qty
         */
        public void SetQty(Decimal Qty)
        {
            base.SetQty(Qty);
        }	//	setQty


    }
}
