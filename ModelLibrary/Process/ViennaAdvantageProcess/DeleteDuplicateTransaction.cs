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
        VAdvantage.Model.MTransaction transaction = null;

        protected override void Prepare()
        {
            ;
        }
        protected override string DoIt()
        {
            sql = @"SELECT   AD_Org_ID ,   M_Product_ID ,  M_Locator_ID ,  M_AttributeSetInstance_ID ,  m_movementline_id , m_transaction_id
                    FROM m_transaction WHERE m_movementline_id IS NOT NULL ORDER BY m_product_id";
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

            sql = @"SELECT COUNT(*) ,   AD_Org_ID ,   M_Product_ID ,  M_Locator_ID ,  M_AttributeSetInstance_ID ,  m_movementline_id
                    FROM m_transaction WHERE m_movementline_id IS NOT NULL
                    GROUP BY AD_Org_ID ,   M_Product_ID ,  M_Locator_ID ,  M_AttributeSetInstance_ID ,  m_movementline_id
                    HAVING COUNT(*) > 1 ORDER BY m_product_id";
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
                                DataRow[] dr = dsAllTransactionRecord.Tables[0].Select("AD_Org_ID= " + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["AD_Org_ID"]) +
                                                                                       " AND M_Product_ID=" + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]) +
                                                                                       " AND M_Locator_ID=" + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]) +
                                                                                       " AND M_AttributeSetInstance_ID=" + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]) +
                                                                                       " AND m_movementline_id=" + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["m_movementline_id"]), "m_transaction_id ASC");
                                if (dr.GetLength(0) > 0)
                                {
                                    for (j = 0; j < dr.GetLength(0) - 1; j++)
                                    {
                                        transaction = new VAdvantage.Model.MTransaction(GetCtx(), Util.GetValueOfInt(dr[0]["m_transaction_id"]), Get_Trx());
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
