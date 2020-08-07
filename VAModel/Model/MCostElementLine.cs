/********************************************************
 * Module Name    : 
 * Purpose        : Model for Cost Element.
 * Class Used     : X_M_CostElementLine
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
    public class MCostElementLine : X_M_CostElementLine
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(MCostElementLine).FullName);
        private MCostElement costElement = null;
        private String costingMethod = null;
        private StringBuilder sql = new StringBuilder();
        private int countRecord = 0;

        public MCostElementLine(Ctx ctx, int M_CostElementLine_ID, Trx trxName)
            : base(ctx, M_CostElementLine_ID, trxName)
        {

        }

        public MCostElementLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        protected override bool BeforeSave(bool newRecord)
        {
            try
            {
                costElement = new MCostElement(GetCtx(), GetM_Ref_CostElement(), null);
                costingMethod = costElement.GetCostingMethod();
                if (string.IsNullOrEmpty(costingMethod))
                {
                    if (Util.GetValueOfInt(Get_ValueOld("M_Ref_CostElement")) == GetM_Ref_CostElement())
                    {
                        return true;
                    }
                    sql.Clear();
                    sql.Append(@"SELECT COUNT(*) FROM M_CostElementLine WHERE M_Ref_CostElement =  " + GetM_Ref_CostElement()
                                 + " AND AD_Client_ID = " + GetAD_Client_ID() + " AND M_CostElement_ID = " + GetM_CostElement_ID() + " AND M_CostElementLine_ID != " + GetM_CostElementLine_ID());
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
                    sql.Append(@"SELECT COUNT(*) FROM M_CostElementLine WHERE AD_Client_ID = " + GetAD_Client_ID()
                                  + " AND M_CostElement_ID = " + GetM_CostElement_ID()
                                  + " AND CAST(M_Ref_CostElement AS INTEGER) IN (SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND "
                                  + " CostingMethod IN ('A' , 'F' , 'I' , 'L' , 'S' , 'i' , 'p')) AND M_CostElementLine_ID != " + GetM_CostElementLine_ID());
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
