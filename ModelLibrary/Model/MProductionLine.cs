/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MProductionLine
 * Purpose        : Production Plan model.
 * Class Used     : X_M_ProductionLine
 * Chronological    Development
 * Raghunandan     24-Nov-2009
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

namespace VAdvantage.Model
{
    public class MProductionLine : X_M_ProductionLine
    {

        private MWarehouse wh = null;
        private MProduct product = null;

        /// <summary>
        /// 	Std Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="M_ProductionLine_ID"></param>
        /// <param name="trxName"></param>
        public MProductionLine(Ctx ctx, int M_ProductionLine_ID, Trx trxName)
            : base(ctx, M_ProductionLine_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MProductionLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        protected override bool BeforeSave(bool newRecord)
        {
            // Movement Quantity can not be less than 0 when planned is greater than 0
            if (GetPlannedQty() > 0 && GetMovementQty() < 0)
            {
                log.SaveWarning("Warning", Msg.GetMsg(GetCtx(), "MovementQtyCantbelesszero"));
                return false;
            }

            // Movement Quantity can not be greater than 0 when planned is less than 0
            if (GetPlannedQty() < 0 && GetMovementQty() > 0)
            {
                log.SaveWarning("Warning", Msg.GetMsg(GetCtx(), "MovementQtyCantbegrtzero"));
                return false;
            }

            //Lakhwinder 12 Apr 2021
            //Qty on hand has to check while completing the prodcution , It's handled in associated stored procedure
            //Introduced Material policy tab to derive attribute based qty from storage , respective attribute  will come on production line after completion , Might need to split production line as well.
            // when warehouse disallow negative inventory is false then on hand qty can't be in negative
            //wh = MWarehouse.Get(GetCtx(), GetM_Warehouse_ID());
            //if (wh.IsDisallowNegativeInv() && GetM_Product_ID() > 0)
            //{
            //    product = MProduct.Get(GetCtx(), GetM_Product_ID());
            //    string qry = "SELECT NVL(SUM(NVL(QtyOnHand,0)),0) AS QtyOnHand FROM M_Storage where m_locator_id=" + GetM_Locator_ID() + " and m_product_id=" + GetM_Product_ID();
            //    //if (GetM_AttributeSetInstance_ID() != 0)
            //    //{
            //    qry += " AND NVL(M_AttributeSetInstance_ID , 0) =" + GetM_AttributeSetInstance_ID();
            //    //}
            //    Decimal? OnHandQty = Convert.ToDecimal(DB.ExecuteScalar(qry));

            //    qry = @"SELECT NVL(SUM(MovementQty) , 0) FROM M_ProductionLine WHERE IsActive = 'Y' AND  M_Locator_ID=" + GetM_Locator_ID() + @" AND m_product_id=" + GetM_Product_ID() +
            //        @" AND NVL(M_AttributeSetInstance_ID , 0) =" + GetM_AttributeSetInstance_ID() + @" AND M_Production_ID = " + GetM_Production_ID();
            //    if (!newRecord)
            //    {
            //        qry += @" AND M_ProductionLine_ID <> " + GetM_ProductionLine_ID();
            //    }
            //    Decimal? moveQty = Convert.ToDecimal(DB.ExecuteScalar(qry));
            //    if ((OnHandQty + GetMovementQty() + moveQty) < 0)
            //    {
            //        log.SaveError("", product.GetName() + ", " + Msg.GetMsg(GetCtx(), "VIS_InsufficientQty") + OnHandQty);
            //        return false;
            //    }
            //}

            return true;
        }

        /// <summary>
        /// Set Product - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        public void SetM_Product_ID(String oldM_Product_ID, String newM_Product_ID, int windowNo)
        {
            if (newM_Product_ID == null || Utility.Util.GetValueOfInt(newM_Product_ID) == 0)
            {
                return;
            }
            int M_Product_ID = Utility.Util.GetValueOfInt(newM_Product_ID);
            base.SetM_Product_ID(M_Product_ID);
            if (M_Product_ID == 0)
            {
                SetM_AttributeSetInstance_ID(0);
                return;
            }
            //	Set Attribute
            int M_AttributeSetInstance_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_AttributeSetInstance_ID");
            if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_Product_ID") == M_Product_ID
                && M_AttributeSetInstance_ID != 0)
            {
                SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            }
            else
            {
                SetM_AttributeSetInstance_ID(0);
            }
        }
    }
}
