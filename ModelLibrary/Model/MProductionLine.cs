/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MProductionLine
 * Purpose        : Production Plan model.
 * Class Used     : X_VAM_ProductionLine
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
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MProductionLine : X_VAM_ProductionLine
    {

        private MWarehouse wh = null;
        private MProduct product = null;

        /// <summary>
        /// 	Std Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAM_ProductionLine_ID"></param>
        /// <param name="trxName"></param>
        public MProductionLine(Ctx ctx, int VAM_ProductionLine_ID, Trx trxName)
            : base(ctx, VAM_ProductionLine_ID, trxName)
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

            // when warehouse disallow negative inventory is false then on hand qty can't be in negative
            wh = MWarehouse.Get(GetCtx(), GetVAM_Warehouse_ID());
            if (wh.IsDisallowNegativeInv() && GetVAM_Product_ID() > 0)
            {
                product = MProduct.Get(GetCtx(), GetVAM_Product_ID());
                string qry = "SELECT NVL(SUM(NVL(QtyOnHand,0)),0) AS QtyOnHand FROM VAM_Storage where VAM_Locator_id=" + GetVAM_Locator_ID() + " and VAM_Product_id=" + GetVAM_Product_ID();
                //if (GetVAM_PFeature_SetInstance_ID() != 0)
                //{
                qry += " AND NVL(VAM_PFeature_SetInstance_ID , 0) =" + GetVAM_PFeature_SetInstance_ID();
                //}
                Decimal? OnHandQty = Convert.ToDecimal(DB.ExecuteScalar(qry));

                qry = @"SELECT NVL(SUM(MovementQty) , 0) FROM VAM_ProductionLine WHERE IsActive = 'Y' AND  VAM_Locator_ID=" + GetVAM_Locator_ID() + @" AND VAM_Product_id=" + GetVAM_Product_ID() +
                    @" AND NVL(VAM_PFeature_SetInstance_ID , 0) =" + GetVAM_PFeature_SetInstance_ID() + @" AND VAM_Production_ID = " + GetVAM_Production_ID();
                if (!newRecord)
                {
                    qry += @" AND VAM_ProductionLine_ID <> " + GetVAM_ProductionLine_ID();
                }
                Decimal? moveQty = Convert.ToDecimal(DB.ExecuteScalar(qry));
                if ((OnHandQty + GetMovementQty() + moveQty) < 0)
                {
                    log.SaveError("", product.GetName() + ", " + Msg.GetMsg(GetCtx(), "VIS_InsufficientQty") + OnHandQty);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Set Product - Callout
        /// </summary>
        /// <param name="oldVAM_Product_ID">old value</param>
        /// <param name="newVAM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        public void SetVAM_Product_ID(String oldVAM_Product_ID, String newVAM_Product_ID, int windowNo)
        {
            if (newVAM_Product_ID == null || Utility.Util.GetValueOfInt(newVAM_Product_ID) == 0)
            {
                return;
            }
            int VAM_Product_ID = Utility.Util.GetValueOfInt(newVAM_Product_ID);
            base.SetVAM_Product_ID(VAM_Product_ID);
            if (VAM_Product_ID == 0)
            {
                SetVAM_PFeature_SetInstance_ID(0);
                return;
            }
            //	Set Attribute
            int VAM_PFeature_SetInstance_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAM_PFeature_SetInstance_ID");
            if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAM_Product_ID") == VAM_Product_ID
                && VAM_PFeature_SetInstance_ID != 0)
            {
                SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            }
            else
            {
                SetVAM_PFeature_SetInstance_ID(0);
            }
        }
    }
}
