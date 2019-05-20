using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Globalization;
using VAdvantage.ProcessEngine;
using VAdvantage.Report;

namespace ViennaAdvantage.Process
{
    public class StockDetail : SvrProcess
    {
        private StringBuilder _Insert = new StringBuilder();

        private int M_Warehouse_ID = 0;
        private int M_Locator_ID = 0;
        private int M_Product_Category_ID = 0;
        private int M_Product_ID = 0;
        private string _ZeroQty = "";
        private string query = "";
        private DateTime? MovementDate = null;
        private StringBuilder prd = new StringBuilder();
        private StringBuilder prdcat = new StringBuilder();
        private StringBuilder loc = new StringBuilder();
        private StringBuilder movdat = new StringBuilder();
        private int _SelectedLocator = 0;
        private List<int> ListOfLocator = new List<int>();
        List<KeyValuePair<int, int>> lst = new List<KeyValuePair<int, int>>();
        Dictionary<int, int> ListOfPrdLoc = new Dictionary<int, int>();
        protected override string DoIt()
        {
            query = "Select count(*) from  VARPT_Temp_Stock";
            int no = Util.GetValueOfInt(DB.ExecuteScalar(query, null, null));
            if (no > 0)
            {
                query = "Delete from  VARPT_Temp_Stock";

                no = DB.ExecuteQuery(query, null, null);

            }
            #region commented
            //            _Insert.Append(@"INSERT INTO  VARPT_Temp_Stock 
            //                                   (AD_Client_ID,AD_Org_ID,value,m_product_category_id,C_UOM_ID,
            //                                  M_Locator_ID,MovementDate,M_Product_ID ,CurrentQty,m_warehouse_id)");

            //            StringBuilder sql=new StringBuilder();
            //            sql.Append(@"SELECT tr.ad_client_id,tr.ad_org_id,p.value,
            //                                   p.m_product_category_id,
            //                                   p.c_uom_id , tr.m_locator_ID , tr.movementdate ,
            //                                   tr.m_product_id,tr.currentqty,
            //                                   lc.m_warehouse_id FROM m_transaction tr INNER JOIN m_product p ON tr.m_product_id =p.m_product_id
            //                                 INNER JOIN m_locator lc ON tr.m_locator_id=lc.m_locator_id");









            //            if (MovementDate != null)
            //            {
            //                sql.Append("  where movementdate=" + GlobalVariable.TO_DATE(MovementDate, true));
            //            }
            //            if (M_Warehouse_ID != 0)
            //            {
            //                sql.Append("and  lc.m_warehouse_id=" + M_Warehouse_ID);
            //            }
            //            if (M_Product_ID != 0)
            //            {
            //                //sql.Append("and  tr.m_product_id=" + M_Product_ID);
            //                sql.Append(" and  p.m_product_id=" + M_Product_ID);

            //            }
            //            if (M_Product_Category_ID != 0)
            //            {
            //                //sql.Append("and  p.m_product_category_id=" + M_Product_Category_ID);
            //                sql.Append("and p.m_product_category_id=" + M_Product_Category_ID);
            //            }
            //            if (M_Locator_ID != 0)
            //            {
            //              //  sql.Append("and  tr.m_locator_ID=" + M_Locator_ID);
            //                sql.Append("and tr.m_locator_ID=" + M_Locator_ID);
            //            }
            //            sql.Append(" and tr.movementdate   IN (SELECT MAX(movementdate) FROM m_transaction trn INNER JOIN m_product pd ON trn.m_product_id =pd.m_product_id INNER JOIN m_locator lca ON trn.m_locator_id=lca.m_locator_id");
            //            if (MovementDate != null)
            //            {
            //                sql.Append(" where trn.movementdate=" + GlobalVariable.TO_DATE(MovementDate, true));
            //            }
            //            if (M_Warehouse_ID != 0)
            //            {
            //                sql.Append("and  lca.m_warehouse_id=" + M_Warehouse_ID);
            //            }
            //            if (M_Product_ID != 0)
            //            {
            //                //sql.Append("and  tr.m_product_id=" + M_Product_ID);
            //                sql.Append(" and pd.m_product_id=" + M_Product_ID);

            //            }
            //            if (M_Product_Category_ID != 0)
            //            {
            //                //sql.Append("and  p.m_product_category_id=" + M_Product_Category_ID);
            //                sql.Append(" and pd.m_product_category_id=" + M_Product_Category_ID);
            //            }
            //            if (M_Locator_ID != 0)
            //            {
            //                //  sql.Append("and  tr.m_locator_ID=" + M_Locator_ID);
            //                sql.Append(" and trn.m_locator_ID=" + M_Locator_ID);
            //            }
            //            sql.Append(")ORDER BY m_transaction_id DESC");
            //           // sql.Append(") ORDER BY m_transaction_id ASC");
            //            DataSet ds=DB.ExecuteDataset(sql.ToString(),null,null);
            //            if (ds != null)
            //            {

            //                if (ds.Tables[0].Rows.Count > 0)
            //                {
            //                    _Insert.Append(sql);
            //                     no = DB.ExecuteQuery(_Insert.ToString(), null, null);
            //                }
            //else
            //{ int no = 0;
            #endregion
            StringBuilder sql = new StringBuilder();
            _Insert = new StringBuilder();

            if (MovementDate != null)
            {
                //MovementDate = new DateTime(MovementDate.Value.Year, MovementDate.Value.Month, (MovementDate.Value.Day + 1));
                MovementDate = MovementDate.Value.AddDays(1);
            }

            _Insert.Append(@"INSERT INTO  VARPT_Temp_Stock 
                                   (AD_Client_ID,AD_Org_ID,value,m_product_category_id,C_UOM_ID,
                                  M_Locator_ID,MovementDate,M_Product_ID ,CurrentQty,m_warehouse_id)");
            sql = new StringBuilder();
            sql.Append(@"SELECT tr.ad_client_id,tr.ad_org_id,p.value,
                                   p.m_product_category_id,
                                   p.c_uom_id , tr.m_locator_ID , tr.movementdate ,
                                   tr.m_product_id,tr.currentqty,
                                   lc.m_warehouse_id FROM m_transaction tr INNER JOIN m_product p ON tr.m_product_id =p.m_product_id
                                 INNER JOIN m_locator lc ON tr.m_locator_id=lc.m_locator_id");
            if (MovementDate != null)
            {
                sql.Append(" where movementdate<" + GlobalVariable.TO_DATE(MovementDate, true));

            }
            if (M_Warehouse_ID !=0)
            {
                sql.Append(" and lc.M_Warehouse_ID=" + M_Warehouse_ID);

            }
            if (M_Product_ID != 0 && M_Product_Category_ID != 0 && M_Locator_ID != 0 && M_Warehouse_ID != 0)
            {

                sql.Append(" and  p.m_product_id=" + M_Product_ID);
                sql.Append("and p.m_product_category_id=" + M_Product_Category_ID);
                sql.Append("and tr.m_locator_ID=" + M_Locator_ID);
                sql.Append(" and tr.movementdate   IN (SELECT MAX(movementdate) FROM m_transaction trn INNER JOIN m_product pd ON trn.m_product_id =pd.m_product_id INNER JOIN m_locator lca ON trn.m_locator_id=lca.m_locator_id");
                sql.Append(" where trn.movementdate<" + GlobalVariable.TO_DATE(MovementDate, true));
                sql.Append(" and pd.m_product_id=" + M_Product_ID);
                sql.Append(" and pd.m_product_category_id=" + M_Product_Category_ID);
                sql.Append(" and trn.m_locator_ID=" + M_Locator_ID);
                sql.Append(" and lca.m_warehouse_id=" + M_Warehouse_ID);
                sql.Append(")ORDER BY m_transaction_id DESC");
                //_Insert.Append(sql);
                //no = DB.ExecuteQuery(_Insert.ToString(), null, null);
                DataSet dsProductlocator = DB.ExecuteDataset(sql.ToString(), null, null);
                if (dsProductlocator.Tables[0].Rows.Count > 0)
                {
                    int a = 0;
                    for (a = 0; a < dsProductlocator.Tables[0].Rows.Count; a++)
                    {
                        // if (Util.GetValueOfInt(dsProduct.Tables[0].Rows[]["M_Locator_Id"]) == _SelectedLocator)
                        //{
                        _Insert.Append("values(" + Util.GetValueOfInt(dsProductlocator.Tables[0].Rows[0]["Ad_client_ID"]) + @",
                                                 " + Util.GetValueOfInt(dsProductlocator.Tables[0].Rows[0]["Ad_Org_ID"]) + @",
                                                ' " + Util.GetValueOfString(dsProductlocator.Tables[0].Rows[0]["Value"]) + @"',
                                                 " + Util.GetValueOfInt(dsProductlocator.Tables[0].Rows[0]["m_product_category_id"]) + @",
                                                " + Util.GetValueOfInt(dsProductlocator.Tables[0].Rows[0]["C_Uom_id"]) + @",
                                                  " + Util.GetValueOfInt(dsProductlocator.Tables[0].Rows[0]["m_locator_ID"]) + @",
                                                     " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(dsProductlocator.Tables[0].Rows[0]["movementdate"]), true) + @",
                                                  " + Util.GetValueOfInt(dsProductlocator.Tables[0].Rows[0]["m_product_id"]) + @",
                                                 " + Util.GetValueOfInt(dsProductlocator.Tables[0].Rows[0]["currentqty"]) + @", 
                                                  " + Util.GetValueOfInt(dsProductlocator.Tables[0].Rows[0]["m_warehouse_id"]) + @")");
                        no = DB.ExecuteQuery(_Insert.ToString(), null, null);
                        dsProductlocator.Dispose();
                        if (a == 0)
                            break;
                        // }
                    }

                }
                else
                {
                    dsProductlocator.Dispose();

                }
            }
            else if (M_Product_ID != 0)
            {

                if (M_Locator_ID != 0)
                {
                    sql.Append(" and  p.m_product_id=" + M_Product_ID);
                    sql.Append("and tr.m_locator_ID=" + M_Locator_ID);
                    if (M_Product_Category_ID != 0)
                    {
                        sql.Append("and p.m_product_category_id=" + M_Product_Category_ID);

                    }
                    sql.Append(" and tr.movementdate   IN (SELECT MAX(movementdate) FROM m_transaction trn INNER JOIN m_product pd ON trn.m_product_id =pd.m_product_id  INNER JOIN m_locator lca ON trn.m_locator_id=lca.m_locator_id");
                    sql.Append(" where trn.movementdate<" + GlobalVariable.TO_DATE(MovementDate, true));
                    sql.Append(" and pd.m_product_id=" + M_Product_ID);
                    if (M_Warehouse_ID != 0)
                    {
                        sql.Append(" and lca.m_warehouse_id=" + M_Warehouse_ID);
                    }

                    if (M_Locator_ID != 0)
                    {
                        sql.Append(" and trn.m_locator_ID=" + M_Locator_ID);
                    }
                    if (M_Product_Category_ID != 0)
                    {
                        sql.Append(" and pd.m_product_category_id=" + M_Product_Category_ID);
                    }
                    sql.Append(")ORDER BY m_transaction_id DESC");
                    DataSet dsProdLocator = DB.ExecuteDataset(sql.ToString(), null, null);
                    if (dsProdLocator.Tables[0].Rows.Count > 0)
                    {

                        for (int a = 0; a < dsProdLocator.Tables[0].Rows.Count; a++)
                        {
                            // if (Util.GetValueOfInt(dsProduct.Tables[0].Rows[]["M_Locator_Id"]) == _SelectedLocator)
                            //{
                            _Insert.Append("values(" + Util.GetValueOfInt(dsProdLocator.Tables[0].Rows[0]["Ad_client_ID"]) + @",
                                                 " + Util.GetValueOfInt(dsProdLocator.Tables[0].Rows[0]["Ad_Org_ID"]) + @",
                                                ' " + Util.GetValueOfString(dsProdLocator.Tables[0].Rows[0]["Value"]) + @"',
                                                 " + Util.GetValueOfInt(dsProdLocator.Tables[0].Rows[0]["m_product_category_id"]) + @",
                                                " + Util.GetValueOfInt(dsProdLocator.Tables[0].Rows[0]["C_Uom_id"]) + @",
                                                  " + Util.GetValueOfInt(dsProdLocator.Tables[0].Rows[0]["m_locator_ID"]) + @",
                                                     " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(dsProdLocator.Tables[0].Rows[0]["movementdate"]), true) + @",
                                                  " + Util.GetValueOfInt(dsProdLocator.Tables[0].Rows[0]["m_product_id"]) + @",
                                                 " + Util.GetValueOfInt(dsProdLocator.Tables[0].Rows[0]["currentqty"]) + @", 
                                                  " + Util.GetValueOfInt(dsProdLocator.Tables[0].Rows[0]["m_warehouse_id"]) + @")");
                            no = DB.ExecuteQuery(_Insert.ToString(), null, null);
                            dsProdLocator.Dispose();
                            if (a == 0)
                            {
                                break;
                            }
                            // }
                        }

                    }
                    else
                    {
                        dsProdLocator.Dispose();
                    }
                    //_Insert.Append(sql);
                    //no = DB.ExecuteQuery(_Insert.ToString(), null, null);

                }
                else
                {

                    query = "select m_locator_id from m_locator where M_Warehouse_ID=" + M_Warehouse_ID;
                    DataSet dslocator = DB.ExecuteDataset(query, null, null);
                    if (dslocator.Tables[0].Rows.Count > 0)
                    {
                        for (int b = 0; b < dslocator.Tables[0].Rows.Count; b++)
                        {
                            _Insert = new StringBuilder();
                            _Insert.Append(@"INSERT INTO  VARPT_Temp_Stock 
                                   (AD_Client_ID,AD_Org_ID,value,m_product_category_id,C_UOM_ID,
                                  M_Locator_ID,MovementDate,M_Product_ID ,CurrentQty,m_warehouse_id)");
                            sql = new StringBuilder();
                            sql.Append(@"SELECT tr.ad_client_id,tr.ad_org_id,p.value,
                                   p.m_product_category_id,
                                   p.c_uom_id , tr.m_locator_ID , tr.movementdate ,
                                   tr.m_product_id,tr.currentqty,
                                   lc.m_warehouse_id FROM m_transaction tr INNER JOIN m_product p ON tr.m_product_id =p.m_product_id
                                 INNER JOIN m_locator lc ON tr.m_locator_id=lc.m_locator_id where movementdate<" + GlobalVariable.TO_DATE(MovementDate, true) + @"
                                      and lc.M_Warehouse_ID=" + M_Warehouse_ID + "  and lc.M_locator_id= " + dslocator.Tables[0].Rows[b]["M_Locator_ID"] + " and  p.m_product_id=" + M_Product_ID);
                            if (M_Product_Category_ID != 0)
                            {
                                sql.Append("and p.m_product_category_id=" + M_Product_Category_ID);

                            }
                            sql.Append(" and tr.movementdate   IN (SELECT MAX(movementdate) FROM m_transaction trn INNER JOIN m_product pd ON trn.m_product_id =pd.m_product_id  INNER JOIN m_locator lca ON trn.m_locator_id=lca.m_locator_id");
                            sql.Append(" where trn.movementdate<" + GlobalVariable.TO_DATE(MovementDate, true));
                            sql.Append(" and pd.m_product_id=" + M_Product_ID);
                            if (M_Warehouse_ID != 0)
                            {
                                sql.Append(" and lca.m_warehouse_id=" + M_Warehouse_ID);
                            }
                            sql.Append(" and trn.M_locator_id= " + dslocator.Tables[0].Rows[b]["M_Locator_ID"]);
                            if (M_Product_Category_ID != 0)
                            {
                                sql.Append(" and pd.m_product_category_id=" + M_Product_Category_ID);
                            }
                            sql.Append(")ORDER BY m_transaction_id DESC");
                            DataSet dsProduct = DB.ExecuteDataset(sql.ToString(), null, null);
                            if (dsProduct.Tables[0].Rows.Count > 0)
                            {
                                int a = 0;
                                for (a = 0; a < dsProduct.Tables[0].Rows.Count; a++)
                                {
                                    // if (Util.GetValueOfInt(dsProduct.Tables[0].Rows[]["M_Locator_Id"]) == _SelectedLocator)
                                    //{
                                    _Insert.Append("values(" + Util.GetValueOfInt(dsProduct.Tables[0].Rows[0]["Ad_client_ID"]) + @",
                                                 " + Util.GetValueOfInt(dsProduct.Tables[0].Rows[0]["Ad_Org_ID"]) + @",
                                                ' " + Util.GetValueOfString(dsProduct.Tables[0].Rows[0]["Value"]) + @"',
                                                 " + Util.GetValueOfInt(dsProduct.Tables[0].Rows[0]["m_product_category_id"]) + @",
                                                " + Util.GetValueOfInt(dsProduct.Tables[0].Rows[0]["C_Uom_id"]) + @",
                                                  " + Util.GetValueOfInt(dsProduct.Tables[0].Rows[0]["m_locator_ID"]) + @",
                                                     " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(dsProduct.Tables[0].Rows[0]["movementdate"]), true) + @",
                                                  " + Util.GetValueOfInt(dsProduct.Tables[0].Rows[0]["m_product_id"]) + @",
                                                 " + Util.GetValueOfInt(dsProduct.Tables[0].Rows[0]["currentqty"]) + @", 
                                                  " + Util.GetValueOfInt(dsProduct.Tables[0].Rows[0]["m_warehouse_id"]) + @")");
                                    no = DB.ExecuteQuery(_Insert.ToString(), null, null);
                                    dsProduct.Dispose(); 
                                    if(a==0)
                                    break;
                                    // }
                                }

                            }
                            else
                            {
                                dsProduct.Dispose();
                            }
                            //_Insert.Append(sql);
                            //no = DB.ExecuteQuery(_Insert.ToString(), null, null);

                        }
                    }

                }

            }
            else
            {

                query = "select * from m_transaction where ad_client_ID=" + GetCtx().GetAD_Client_ID() + " order by M_product_ID";
                DataSet dstransaction = DB.ExecuteDataset(query, null, null);
                if (dstransaction.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < dstransaction.Tables[0].Rows.Count; j++)
                    {

                        if (!lst.Contains(new KeyValuePair<int, int>(Util.GetValueOfInt(dstransaction.Tables[0].Rows[j]["M_Product_ID"]), Util.GetValueOfInt(dstransaction.Tables[0].Rows[j]["M_Locator_ID"]))))
                        {
                            lst.Add(new KeyValuePair<int, int>(Util.GetValueOfInt(dstransaction.Tables[0].Rows[j]["M_Product_ID"]), Util.GetValueOfInt(dstransaction.Tables[0].Rows[j]["M_Locator_ID"])));
                            _SelectedLocator = Util.GetValueOfInt(dstransaction.Tables[0].Rows[j]["M_Locator_ID"]);

                            _Insert = new StringBuilder();
                            _Insert.Append(@"INSERT INTO  VARPT_Temp_Stock 
                                   (AD_Client_ID,AD_Org_ID,value,m_product_category_id,C_UOM_ID,
                                  M_Locator_ID,MovementDate,M_Product_ID ,CurrentQty,m_warehouse_id)");
                            sql = new StringBuilder();
                            sql.Append(@"SELECT tr.ad_client_id,tr.ad_org_id,p.value,
                                   p.m_product_category_id,
                                   p.c_uom_id , tr.m_locator_ID , tr.movementdate ,
                                   tr.m_product_id,tr.currentqty,
                                   lc.m_warehouse_id FROM m_transaction tr INNER JOIN m_product p ON tr.m_product_id =p.m_product_id
                                 INNER JOIN m_locator lc ON tr.m_locator_id=lc.m_locator_id
                            where   lc.M_Warehouse_ID=" + M_Warehouse_ID + " and tr.M_locator_id=" + _SelectedLocator + "  and p.m_product_id=" + dstransaction.Tables[0].Rows[j]["M_Product_ID"] + @" 
                                and movementdate<" + GlobalVariable.TO_DATE(MovementDate, true) + @"");
                            if (M_Locator_ID != 0)
                            {
                                sql.Append("and tr.m_locator_ID=" + M_Locator_ID);
                            }
                            if (M_Product_Category_ID != 0)
                            {
                                sql.Append("and p.m_product_category_id=" + M_Product_Category_ID);

                            }
                            sql.Append(" and tr.movementdate   IN (SELECT MAX(movementdate) FROM m_transaction trn INNER JOIN m_product pd ON trn.m_product_id =pd.m_product_id INNER JOIN m_locator lca ON trn.m_locator_id=lca.m_locator_id");
                            sql.Append(" WHERE  lca.m_warehouse_id=" + M_Warehouse_ID + " and trn.M_locator_id=" + _SelectedLocator + " and pd.m_product_id=" + dstransaction.Tables[0].Rows[j]["M_Product_ID"] + @" 
                            and trn.movementdate<" + GlobalVariable.TO_DATE(MovementDate, true));
                            if (M_Locator_ID != 0)
                            {
                                sql.Append(" and trn.m_locator_ID=" + M_Locator_ID);
                            }
                            if (M_Product_Category_ID != 0)
                            {
                                sql.Append(" and pd.m_product_category_id=" + M_Product_Category_ID);
                            }
                            sql.Append(")order by  m_transaction_id desc");
                            DataSet dsrecord = DB.ExecuteDataset(sql.ToString(), null, null);
                            if (dsrecord.Tables[0].Rows.Count > 0)
                            {

                                for (int a = 0; a < dsrecord.Tables[0].Rows.Count; a++)
                                {
                                    if (Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["M_Locator_Id"]) == _SelectedLocator)
                                    {
                                        _Insert.Append("values(" + Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["Ad_client_ID"]) + @",
                                                 " + Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["Ad_Org_ID"]) + @",
                                                ' " + Util.GetValueOfString(dsrecord.Tables[0].Rows[a]["Value"]) + @"',
                                                 " + Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["m_product_category_id"]) + @",
                                                " + Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["C_Uom_id"]) + @",
                                                  " + Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["m_locator_ID"]) + @",
                                                     " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(dsrecord.Tables[0].Rows[a]["movementdate"]), true) + @",
                                                  " + Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["m_product_id"]) + @",
                                                 " + Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["currentqty"]) + @", 
                                                  " + Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["m_warehouse_id"]) + @")");
                                        no = DB.ExecuteQuery(_Insert.ToString(), null, null);
                                        dsrecord.Dispose();
                                        break;
                                    }
                                }

                            }

                            else
                            {
                                dsrecord.Dispose();
                            }
                        }
                    }

                }
            }
            query = "select count(*) from VARPT_Temp_Stock";
            int count = Util.GetValueOfInt(DB.ExecuteScalar(query, null, null));
            MovementDate = MovementDate.Value.AddDays(-1);
            //new DateTime(MovementDate.Value.Year, MovementDate.Value.Month, (MovementDate.Value.Day -1));
            if (count > 0)
            {
                query = "update VARPT_Temp_Stock set movementdate=" + GlobalVariable.TO_DATE(MovementDate, true);
                no = DB.ExecuteQuery(query, null, null);

            }
            if (_ZeroQty == "N")
            {
                query = "update VARPT_Temp_Stock set ZeroQty='Y' where currentqty=0";
                no = DB.ExecuteQuery(query, null, null);

            }
            else
            {
                query = "update VARPT_Temp_Stock set ZeroQty='" + _ZeroQty + "'";
                no = DB.ExecuteQuery(query, null, null);
            }
            return "Completed";
        }


        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("M_Warehouse_ID"))
                {
                    M_Warehouse_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("MovementDate"))
                {
                    MovementDate = Util.GetValueOfDateTime(para[i].GetParameter());
                }
                else if (name.Equals("M_Locator_ID"))
                {
                    M_Locator_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("M_Product_Category_ID"))
                {
                    M_Product_Category_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("M_Product_ID"))
                {
                    M_Product_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("ZeroQty"))
                {
                    _ZeroQty = Util.GetValueOfString(para[i].GetParameter());
                }
            }
        }

        private void Createsql()
        {
        }
    }
}
