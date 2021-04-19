/********************************************************
 * Module Name    : 
 * Purpose        : Model for Cost Element.
 * Class Used     : X_VAM_CostElementLine
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
    public class MVAMCostElementLine : X_VAM_CostElementLine
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMCostElementLine).FullName);
        private MVAMProductCostElement costElement = null;
        private String costingMethod = null;
        private StringBuilder sql = new StringBuilder();
        private int countRecord = 0;

        public MVAMCostElementLine(Ctx ctx, int VAM_CostElementLine_ID, Trx trxName)
            : base(ctx, VAM_CostElementLine_ID, trxName)
        {

        }

        public MVAMCostElementLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        protected override bool BeforeSave(bool newRecord)
        {
            try
            {
                costElement = new MVAMProductCostElement(GetCtx(), GetVAM_Ref_CostElement(), null);
                costingMethod = costElement.GetCostingMethod();
                if (string.IsNullOrEmpty(costingMethod))
                {
                    if (Util.GetValueOfInt(Get_ValueOld("VAM_Ref_CostElement")) == GetVAM_Ref_CostElement())
                    {
                        return true;
                    }
                    sql.Clear();
                    sql.Append(@"SELECT COUNT(*) FROM VAM_CostElementLine WHERE VAM_Ref_CostElement =  " + GetVAM_Ref_CostElement()
                                 + " AND VAF_Client_ID = " + GetVAF_Client_ID() + " AND VAM_ProductCostElement_ID = " + GetVAM_ProductCostElement_ID() + " AND VAM_CostElementLine_ID != " + GetVAM_CostElementLine_ID());
                    countRecord = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
                    if (countRecord > 0)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "CannotSaveDuplicateRecord"));
                        return false;
                    }
                }
                else
                {
                    if (Util.GetValueOfInt(Get_ValueOld("VAM_Ref_CostElement")) == GetVAM_Ref_CostElement())
                    {
                        return true;
                    }
                    sql.Clear();
                    sql.Append(@"SELECT COUNT(*) FROM VAM_CostElementLine WHERE VAF_Client_ID = " + GetVAF_Client_ID()
                                  + " AND VAM_ProductCostElement_ID = " + GetVAM_ProductCostElement_ID()
                                  + " AND CAST(VAM_Ref_CostElement AS INTEGER) IN (SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND "
                                  + " CostingMethod IN ('A' , 'F' , 'I' , 'L' , 'S' , 'i' , 'p')) AND VAM_CostElementLine_ID != " + GetVAM_CostElementLine_ID());
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
