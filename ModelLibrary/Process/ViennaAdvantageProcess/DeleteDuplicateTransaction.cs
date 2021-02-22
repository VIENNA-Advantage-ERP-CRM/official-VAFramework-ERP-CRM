/********************************************************
    * Project Name   : Payment Method (ED008)
    * Class Name     : DeleteDuplicateTransaction
    * Purpose        : Delete Duplicate Record
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological    Development
    * Amit Bansal     05-March-2015
******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace ViennaAdvantageServer.Process
{
    public class DeleteDuplicateTransaction : SvrProcess
    {
        private string sql = "";
        private DataSet dsTransaction = null;
        private DataSet dsAllTransactionRecord = null;
        VAdvantage.Model.MVAMInvTrx transaction = null;

        protected override void Prepare()
        {
            ;
        }
        protected override string DoIt()
        {
            sql = @"SELECT   VAF_Org_ID ,   VAM_Product_ID ,  VAM_Locator_ID ,  VAM_PFeature_SetInstance_ID ,  VAM_InvTrf_Line_id , VAM_Inv_Trx_id
                    FROM VAM_Inv_Trx WHERE VAM_InvTrf_Line_id IS NOT NULL ORDER BY VAM_Product_id";
            dsAllTransactionRecord = new DataSet();
            try
            {
                dsAllTransactionRecord = DB.ExecuteDataset(sql, null, Get_Trx());
            }
            catch
            {
                if (dsAllTransactionRecord != null)
                {
                    dsAllTransactionRecord.Dispose();
                }
            }

            sql = @"SELECT COUNT(*) ,   VAF_Org_ID ,   VAM_Product_ID ,  VAM_Locator_ID ,  VAM_PFeature_SetInstance_ID ,  VAM_InvTrf_Line_id
                    FROM VAM_Inv_Trx WHERE VAM_InvTrf_Line_id IS NOT NULL
                    GROUP BY VAF_Org_ID ,   VAM_Product_ID ,  VAM_Locator_ID ,  VAM_PFeature_SetInstance_ID ,  VAM_InvTrf_Line_id
                    HAVING COUNT(*) > 1 ORDER BY VAM_Product_id";
            dsTransaction = new DataSet();
            try
            {
                dsTransaction = DB.ExecuteDataset(sql, null, Get_Trx());
                if (dsTransaction != null)
                {
                    if (dsTransaction.Tables.Count > 0)
                    {
                        if (dsTransaction.Tables[0].Rows.Count > 0)
                        {
                            int i = 0, j = 0;
                            for (i = 0; i < dsTransaction.Tables[0].Rows.Count; i++)
                            {
                                DataRow[] dr = dsAllTransactionRecord.Tables[0].Select("VAF_Org_ID= " + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAF_Org_ID"]) +
                                                                                       " AND VAM_Product_ID=" + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]) +
                                                                                       " AND VAM_Locator_ID=" + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]) +
                                                                                       " AND VAM_PFeature_SetInstance_ID=" + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]) +
                                                                                       " AND VAM_InvTrf_Line_id=" + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_InvTrf_Line_id"]), "VAM_Inv_Trx_id ASC");
                                if (dr.GetLength(0) > 0)
                                {
                                    for (j = 0; j < dr.GetLength(0) - 1; j++)
                                    {
                                        transaction = new VAdvantage.Model.MVAMInvTrx(GetCtx(), Util.GetValueOfInt(dr[0]["VAM_Inv_Trx_id"]), Get_Trx());
                                        transaction.Delete(true, Get_Trx());
                                        Commit();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                if (dsTransaction != null)
                {
                    dsTransaction.Dispose();
                }
                if (dsAllTransactionRecord != null)
                {
                    dsAllTransactionRecord.Dispose();
                }
                return Msg.GetMsg(GetCtx(), "NotDeleted");
            }
            finally
            {
                if (dsTransaction != null)
                {
                    dsTransaction.Dispose();
                }
                if (dsAllTransactionRecord != null)
                {
                    dsAllTransactionRecord.Dispose();
                }
            }
            return Msg.GetMsg(GetCtx(), "SucessfullyDeleted");
        }
    }
}
