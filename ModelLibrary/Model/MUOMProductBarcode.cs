/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MUOMProductBarcode
 * Purpose        : for UOM Product Barcode
 * Chronological    Development
 * Arpit Rai     19-Jan-2017
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using java.math;
using VAdvantage.Logging;
using ViennaAdvantage.Model;

namespace VAdvantage.Model
{
    public class MUOMProductBarcode : X_C_UOM_ProductBarcode
    {
        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MUOMProductBarcode).FullName);
        StringBuilder sql = new StringBuilder();

        public MUOMProductBarcode(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }
        public MUOMProductBarcode(Ctx ctx, int C_UOM_ProductBarcode_ID, Trx trxName)
            : base(ctx, C_UOM_ProductBarcode_ID, trxName)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            return success;
        }
        protected override bool BeforeSave(bool newRecord)
        {
            try
            {
                if (!String.IsNullOrEmpty(GetUPC()) &&
                       Util.GetValueOfString(Get_ValueOld("UPC")) != GetUPC())
                {
                    sql.Append("SELECT UPCUNIQUE('b','" + GetUPC() + "') as productID FROM Dual");
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));
                    if (count > 0)
                    {
                        String msg_ = Msg.GetMsg(GetCtx(), "UPCUnique");
                        _log.SaveError(msg_, "");
                        sql.Clear();
                        return false;
                    }
                    sql.Clear();
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql.ToString(), e);
            }
            return true;
        }
    }
}
