/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MProductAttributes
 * Purpose        : Product attributes setting using x-classes
 * Class Used     : X_M_ProductAttributes
 * Chronological    Development
 * Raghunandan     04-Feb-2015
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.ProcessEngine;
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
    public class MProductAttributes : X_M_ProductAttributes
    {
        #region variable

        private string sql = "";
        private int manu_ID = 0;
        private static VLogger _log = VLogger.GetVLogger(typeof(MOrderLine).FullName);
        #endregion

        /// <summary>
        /// Load Cosntructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MProductAttributes(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /// <summary>
        /// Load Cosntructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MProductAttributes(Ctx ctx, int M_ProductAttributes_ID, Trx trxName)
            : base(ctx, M_ProductAttributes_ID, trxName)
        {
        }

        protected override bool BeforeSave(bool newRecord)
        {
            //Arpit
            StringBuilder sql = new StringBuilder();
            //For checking if record is without attribute
            //if (GetM_AttributeSetInstance_ID() == 0 || GetM_AttributeSetInstance_ID() == null)
            //{
            //    _log.SaveError("", Msg.GetMsg(GetCtx(), "NoAttributeSet"));
            //    return false;
            //}

            ////For checking if attribute is already defined
            //sql.Append("SELECT M_AttributeSetInstance_ID FROM M_ProductAttributes WHERE M_Product_ID=" + GetM_Product_ID() +
            //    " AND AD_Client_ID=" + GetAD_Client_ID() + " AND M_ProductAttributes_ID!=" + GetM_ProductAttributes_ID());
            //DataSet ds = new DataSet();
            //ds = DB.ExecuteDataset(sql.ToString(), null, null);
            //if (ds != null && ds.Tables[0].Rows.Count > 0)
            //{
            //    for (Int32 i = 0; i < ds.Tables[0].Rows.Count; i++)
            //    {
            //        if (Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]) > 0)
            //        {
            //            if (GetM_AttributeSetInstance_ID() == Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]))
            //            {
            //                _log.SaveError("", Msg.GetMsg(GetCtx(), "AttributeCodeExists"));
            //                return false;
            //            }
            //        }
            //    }
            //}
            //sql.Clear();
            //Checking if UPC exists//Arpit
            if (!String.IsNullOrEmpty(GetUPC()) &&
                       Util.GetValueOfString(Get_ValueOld("UPC")) != GetUPC())
            {
                //sql.Append(@"SELECT UPCUNIQUE('a','" + GetUPC() + "') as productID FROM Dual");
                //manu_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
                //if (manu_ID != 0 && manu_ID != GetM_Product_ID())

                manu_ID = MProduct.UpcUniqueClientWise(GetAD_Client_ID(), GetUPC());
                if (manu_ID > 0)
                {
                    _log.SaveError("UPCUnique", "");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true if it can be deleted</returns>
        protected override bool BeforeDelete()
        {
            string uqry = "SELECT SUM(cc) as count FROM  (SELECT COUNT(*) AS cc FROM M_MovementLine WHERE M_Product_ID = " + GetM_Product_ID() + " AND M_AttributeSetInstance_ID = "
                + GetM_AttributeSetInstance_ID() + " UNION SELECT COUNT(*) AS cc FROM M_InventoryLine WHERE M_Product_ID = " + GetM_Product_ID() + " AND M_AttributeSetInstance_ID = "
                + GetM_AttributeSetInstance_ID() + " UNION SELECT COUNT(*) AS cc FROM C_OrderLine WHERE M_Product_ID = " + GetM_Product_ID() + " AND M_AttributeSetInstance_ID = "
                + GetM_AttributeSetInstance_ID() + " UNION  SELECT COUNT(*) AS cc FROM M_InOutLine WHERE M_Product_ID = " + GetM_Product_ID() + " AND M_AttributeSetInstance_ID = "
                + GetM_AttributeSetInstance_ID() + ") t";
            int no = Util.GetValueOfInt(DB.ExecuteScalar(uqry));
            if (no > 0)
            {
                log.SaveError("Error", Msg.Translate(GetCtx(), "TransactionAvailable"));
                return false;
            }

            Tuple<String, String, String> aInfo = null;
            if (Env.HasModulePrefix("VAICNT_", out aInfo))
            {
                uqry = "SELECT COUNT(*) AS cc FROM VAICNT_InventoryCountLine WHERE M_Product_ID = " + GetM_Product_ID() + " AND M_AttributeSetInstance_ID = " + GetM_AttributeSetInstance_ID();
                no = Util.GetValueOfInt(DB.ExecuteScalar(uqry));
                if (no > 0)
                {
                    log.SaveError("Error", Msg.Translate(GetCtx(), "CartAvailable"));
                    return false;
                }
            }
            return true;
        }

    }
}
