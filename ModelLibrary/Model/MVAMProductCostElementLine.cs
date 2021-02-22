/********************************************************
 * Module Name    : 
 * Purpose        : Model for Cost Element.
 * Class Used     : X_VAM_ProductCostElementLine
 * Chronological    Development
 * Amit Bansal      07-April-2016
**********************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVAMVAMProductCostElementLine : X_VAM_ProductCostElementLine
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMVAMProductCostElementLine).FullName);
        private MVAMVAMProductCostElement costElement = null;
        private String costingMethod = null;
        private StringBuilder sql = new StringBuilder();
        private int countRecord = 0;

        public MVAMVAMProductCostElementLine(Ctx ctx, int VAM_ProductCostElementLine_ID, Trx trxName)
            : base(ctx, VAM_ProductCostElementLine_ID, trxName)
        {

        }

        public MVAMVAMProductCostElementLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        protected override bool BeforeSave(bool newRecord)
        {
            try
            {
                costElement = new MVAMVAMProductCostElement(GetCtx(), GetM_Ref_CostElement(), null);
                costingMethod = costElement.GetCostingMethod();
                if (string.IsNullOrEmpty(costingMethod))
                {
                    if (Util.GetValueOfInt(Get_ValueOld("M_Ref_CostElement")) == GetM_Ref_CostElement())
                    {
                        return true;
                    }
                    sql.Clear();
                    sql.Append(@"SELECT COUNT(*) FROM VAM_ProductCostElementLine WHERE M_Ref_CostElement =  " + GetM_Ref_CostElement()
                                 + " AND VAF_Client_ID = " + GetVAF_Client_ID() + " AND VAM_ProductCostElement_ID = " + GetVAM_ProductCostElement_ID() + " AND VAM_ProductCostElementLine_ID != " + GetVAM_ProductCostElementLine_ID());
                    countRecord = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
                    if (countRecord > 0)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "CannotSaveDuplicateRecord"));
                        return false;
                    }
                }
                else
                {
                    if (Util.GetValueOfInt(Get_ValueOld("M_Ref_CostElement")) == GetM_Ref_CostElement())
                    {
                        return true;
                    }
                    sql.Clear();
                    sql.Append(@"SELECT COUNT(*) FROM VAM_ProductCostElementLine WHERE VAF_Client_ID = " + GetVAF_Client_ID()
                                  + " AND VAM_ProductCostElement_ID = " + GetVAM_ProductCostElement_ID()
                                  + " AND CAST(M_Ref_CostElement AS INTEGER) IN (SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND "
                                  + " CostingMethod IN ('A' , 'F' , 'I' , 'L' , 'S' , 'i' , 'p')) AND VAM_ProductCostElementLine_ID != " + GetVAM_ProductCostElementLine_ID());
                    countRecord = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
                    if (countRecord > 0)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "CannotSaveMultipleRecordofMaterial"));
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql.ToString(), e);
            }
            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            return true;
        }
    }
}
