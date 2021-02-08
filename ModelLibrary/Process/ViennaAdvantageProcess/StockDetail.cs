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


namespace ViennaAdvantage.Process
{
    public class StockDetail : SvrProcess
    {
        private StringBuilder _Insert = new StringBuilder();

        private int VAM_Warehouse_ID = 0;
        private int VAM_Locator_ID = 0;
        private int VAM_ProductCategory_ID = 0;
        private int VAM_Product_ID = 0;
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
            //                                   (VAF_Client_ID,VAF_Org_ID,value,VAM_ProductCategory_id,VAB_UOM_ID,
            //                                  VAM_Locator_ID,MovementDate,VAM_Product_ID ,CurrentQty,VAM_Warehouse_id)");

            //            StringBuilder sql=new StringBuilder();
            //            sql.Append(@"SELECT tr.vaf_client_id,tr.vaf_org_id,p.value,
            //                                   p.VAM_ProductCategory_id,
            //                                   p.VAB_UOM_id , tr.VAM_Locator_ID , tr.movementdate ,
            //                                   tr.VAM_Product_id,tr.currentqty,
            //                                   lc.VAM_Warehouse_id FROM VAM_Inv_Trx tr INNER JOIN VAM_Product p ON tr.VAM_Product_id =p.VAM_Product_id
            //                                 INNER JOIN VAM_Locator lc ON tr.VAM_Locator_id=lc.VAM_Locator_id");









            //            if (MovementDate != null)
            //            {
            //                sql.Append("  where movementdate=" + GlobalVariable.TO_DATE(MovementDate, true));
            //            }
            //            if (VAM_Warehouse_ID != 0)
            //            {
            //                sql.Append("and  lc.VAM_Warehouse_id=" + VAM_Warehouse_ID);
            //            }
            //            if (VAM_Product_ID != 0)
            //            {
            //                //sql.Append("and  tr.VAM_Product_id=" + VAM_Product_ID);
            //                sql.Append(" and  p.VAM_Product_id=" + VAM_Product_ID);

            //            }
            //            if (VAM_ProductCategory_ID != 0)
            //            {
            //                //sql.Append("and  p.VAM_ProductCategory_id=" + VAM_ProductCategory_ID);
            //                sql.Append("and p.VAM_ProductCategory_id=" + VAM_ProductCategory_ID);
            //            }
            //            if (VAM_Locator_ID != 0)
            //            {
            //              //  sql.Append("and  tr.VAM_Locator_ID=" + VAM_Locator_ID);
            //                sql.Append("and tr.VAM_Locator_ID=" + VAM_Locator_ID);
            //            }
            //            sql.Append(" and tr.movementdate   IN (SELECT MAX(movementdate) FROM VAM_Inv_Trx trn INNER JOIN VAM_Product pd ON trn.VAM_Product_id =pd.VAM_Product_id INNER JOIN VAM_Locator lca ON trn.VAM_Locator_id=lca.VAM_Locator_id");
            //            if (MovementDate != null)
            //            {
            //                sql.Append(" where trn.movementdate=" + GlobalVariable.TO_DATE(MovementDate, true));
            //            }
            //            if (VAM_Warehouse_ID != 0)
            //            {
            //                sql.Append("and  lca.VAM_Warehouse_id=" + VAM_Warehouse_ID);
            //            }
            //            if (VAM_Product_ID != 0)
            //            {
            //                //sql.Append("and  tr.VAM_Product_id=" + VAM_Product_ID);
            //                sql.Append(" and pd.VAM_Product_id=" + VAM_Product_ID);

            //            }
            //            if (VAM_ProductCategory_ID != 0)
            //            {
            //                //sql.Append("and  p.VAM_ProductCategory_id=" + VAM_ProductCategory_ID);
            //                sql.Append(" and pd.VAM_ProductCategory_id=" + VAM_ProductCategory_ID);
            //            }
            //            if (VAM_Locator_ID != 0)
            //            {
            //                //  sql.Append("and  tr.VAM_Locator_ID=" + VAM_Locator_ID);
            //                sql.Append(" and trn.VAM_Locator_ID=" + VAM_Locator_ID);
            //            }
            //            sql.Append(")ORDER BY VAM_Inv_Trx_id DESC");
            //           // sql.Append(") ORDER BY VAM_Inv_Trx_id ASC");
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
                                   (VAF_Client_ID,VAF_Org_ID,value,VAM_ProductCategory_id,VAB_UOM_ID,
                                  VAM_Locator_ID,MovementDate,VAM_Product_ID ,CurrentQty,VAM_Warehouse_id)");
            sql = new StringBuilder();
            sql.Append(@"SELECT tr.vaf_client_id,tr.vaf_org_id,p.value,
                                   p.VAM_ProductCategory_id,
                                   p.VAB_UOM_id , tr.VAM_Locator_ID , tr.movementdate ,
                                   tr.VAM_Product_id,tr.currentqty,
                                   lc.VAM_Warehouse_id FROM VAM_Inv_Trx tr INNER JOIN VAM_Product p ON tr.VAM_Product_id =p.VAM_Product_id
                                 INNER JOIN VAM_Locator lc ON tr.VAM_Locator_id=lc.VAM_Locator_id");
            if (MovementDate != null)
            {
                sql.Append(" where movementdate<" + GlobalVariable.TO_DATE(MovementDate, true));

            }
            if (VAM_Warehouse_ID !=0)
            {
                sql.Append(" and lc.VAM_Warehouse_ID=" + VAM_Warehouse_ID);

            }
            if (VAM_Product_ID != 0 && VAM_ProductCategory_ID != 0 && VAM_Locator_ID != 0 && VAM_Warehouse_ID != 0)
            {

                sql.Append(" and  p.VAM_Product_id=" + VAM_Product_ID);
                sql.Append("and p.VAM_ProductCategory_id=" + VAM_ProductCategory_ID);
                sql.Append("and tr.VAM_Locator_ID=" + VAM_Locator_ID);
                sql.Append(" and tr.movementdate   IN (SELECT MAX(movementdate) FROM VAM_Inv_Trx trn INNER JOIN VAM_Product pd ON trn.VAM_Product_id =pd.VAM_Product_id INNER JOIN VAM_Locator lca ON trn.VAM_Locator_id=lca.VAM_Locator_id");
                sql.Append(" where trn.movementdate<" + GlobalVariable.TO_DATE(MovementDate, true));
                sql.Append(" and pd.VAM_Product_id=" + VAM_Product_ID);
                sql.Append(" and pd.VAM_ProductCategory_id=" + VAM_ProductCategory_ID);
                sql.Append(" and trn.VAM_Locator_ID=" + VAM_Locator_ID);
                sql.Append(" and lca.VAM_Warehouse_id=" + VAM_Warehouse_ID);
                sql.Append(")ORDER BY VAM_Inv_Trx_id DESC");
                //_Insert.Append(sql);
                //no = DB.ExecuteQuery(_Insert.ToString(), null, null);
                DataSet dsProductlocator = DB.ExecuteDataset(sql.ToString(), null, null);
                if (dsProductlocator.Tables[0].Rows.Count > 0)
                {
                    int a = 0;
                    for (a = 0; a < dsProductlocator.Tables[0].Rows.Count; a++)
                    {
                        // if (Util.GetValueOfInt(dsProduct.Tables[0].Rows[]["VAM_Locator_Id"]) == _SelectedLocator)
                        //{
                        _Insert.Append("values(" + Util.GetValueOfInt(dsProductlocator.Tables[0].Rows[0]["vaf_client_ID"]) + @",
                                                 " + Util.GetValueOfInt(dsProductlocator.Tables[0].Rows[0]["vaf_org_ID"]) + @",
                                                ' " + Util.GetValueOfString(dsProductlocator.Tables[0].Rows[0]["Value"]) + @"',
                                                 " + Util.GetValueOfInt(dsProductlocator.Tables[0].Rows[0]["VAM_ProductCategory_id"]) + @",
                                                " + Util.GetValueOfInt(dsProductlocator.Tables[0].Rows[0]["VAB_UOM_id"]) + @",
                                                  " + Util.GetValueOfInt(dsProductlocator.Tables[0].Rows[0]["VAM_Locator_ID"]) + @",
                                                     " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(dsProductlocator.Tables[0].Rows[0]["movementdate"]), true) + @",
                                                  " + Util.GetValueOfInt(dsProductlocator.Tables[0].Rows[0]["VAM_Product_id"]) + @",
                                                 " + Util.GetValueOfInt(dsProductlocator.Tables[0].Rows[0]["currentqty"]) + @", 
                                                  " + Util.GetValueOfInt(dsProductlocator.Tables[0].Rows[0]["VAM_Warehouse_id"]) + @")");
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
            else if (VAM_Product_ID != 0)
            {

                if (VAM_Locator_ID != 0)
                {
                    sql.Append(" and  p.VAM_Product_id=" + VAM_Product_ID);
                    sql.Append("and tr.VAM_Locator_ID=" + VAM_Locator_ID);
                    if (VAM_ProductCategory_ID != 0)
                    {
                        sql.Append("and p.VAM_ProductCategory_id=" + VAM_ProductCategory_ID);

                    }
                    sql.Append(" and tr.movementdate   IN (SELECT MAX(movementdate) FROM VAM_Inv_Trx trn INNER JOIN VAM_Product pd ON trn.VAM_Product_id =pd.VAM_Product_id  INNER JOIN VAM_Locator lca ON trn.VAM_Locator_id=lca.VAM_Locator_id");
                    sql.Append(" where trn.movementdate<" + GlobalVariable.TO_DATE(MovementDate, true));
                    sql.Append(" and pd.VAM_Product_id=" + VAM_Product_ID);
                    if (VAM_Warehouse_ID != 0)
                    {
                        sql.Append(" and lca.VAM_Warehouse_id=" + VAM_Warehouse_ID);
                    }

                    if (VAM_Locator_ID != 0)
                    {
                        sql.Append(" and trn.VAM_Locator_ID=" + VAM_Locator_ID);
                    }
                    if (VAM_ProductCategory_ID != 0)
                    {
                        sql.Append(" and pd.VAM_ProductCategory_id=" + VAM_ProductCategory_ID);
                    }
                    sql.Append(")ORDER BY VAM_Inv_Trx_id DESC");
                    DataSet dsProdLocator = DB.ExecuteDataset(sql.ToString(), null, null);
                    if (dsProdLocator.Tables[0].Rows.Count > 0)
                    {

                        for (int a = 0; a < dsProdLocator.Tables[0].Rows.Count; a++)
                        {
                            // if (Util.GetValueOfInt(dsProduct.Tables[0].Rows[]["VAM_Locator_Id"]) == _SelectedLocator)
                            //{
                            _Insert.Append("values(" + Util.GetValueOfInt(dsProdLocator.Tables[0].Rows[0]["vaf_client_ID"]) + @",
                                                 " + Util.GetValueOfInt(dsProdLocator.Tables[0].Rows[0]["vaf_org_ID"]) + @",
                                                ' " + Util.GetValueOfString(dsProdLocator.Tables[0].Rows[0]["Value"]) + @"',
                                                 " + Util.GetValueOfInt(dsProdLocator.Tables[0].Rows[0]["VAM_ProductCategory_id"]) + @",
                                                " + Util.GetValueOfInt(dsProdLocator.Tables[0].Rows[0]["VAB_UOM_id"]) + @",
                                                  " + Util.GetValueOfInt(dsProdLocator.Tables[0].Rows[0]["VAM_Locator_ID"]) + @",
                                                     " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(dsProdLocator.Tables[0].Rows[0]["movementdate"]), true) + @",
                                                  " + Util.GetValueOfInt(dsProdLocator.Tables[0].Rows[0]["VAM_Product_id"]) + @",
                                                 " + Util.GetValueOfInt(dsProdLocator.Tables[0].Rows[0]["currentqty"]) + @", 
                                                  " + Util.GetValueOfInt(dsProdLocator.Tables[0].Rows[0]["VAM_Warehouse_id"]) + @")");
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

                    query = "select VAM_Locator_id from VAM_Locator where VAM_Warehouse_ID=" + VAM_Warehouse_ID;
                    DataSet dslocator = DB.ExecuteDataset(query, null, null);
                    if (dslocator.Tables[0].Rows.Count > 0)
                    {
                        for (int b = 0; b < dslocator.Tables[0].Rows.Count; b++)
                        {
                            _Insert = new StringBuilder();
                            _Insert.Append(@"INSERT INTO  VARPT_Temp_Stock 
                                   (VAF_Client_ID,VAF_Org_ID,value,VAM_ProductCategory_id,VAB_UOM_ID,
                                  VAM_Locator_ID,MovementDate,VAM_Product_ID ,CurrentQty,VAM_Warehouse_id)");
                            sql = new StringBuilder();
                            sql.Append(@"SELECT tr.vaf_client_id,tr.vaf_org_id,p.value,
                                   p.VAM_ProductCategory_id,
                                   p.VAB_UOM_id , tr.VAM_Locator_ID , tr.movementdate ,
                                   tr.VAM_Product_id,tr.currentqty,
                                   lc.VAM_Warehouse_id FROM VAM_Inv_Trx tr INNER JOIN VAM_Product p ON tr.VAM_Product_id =p.VAM_Product_id
                                 INNER JOIN VAM_Locator lc ON tr.VAM_Locator_id=lc.VAM_Locator_id where movementdate<" + GlobalVariable.TO_DATE(MovementDate, true) + @"
                                      and lc.VAM_Warehouse_ID=" + VAM_Warehouse_ID + "  and lc.VAM_Locator_id= " + dslocator.Tables[0].Rows[b]["VAM_Locator_ID"] + " and  p.VAM_Product_id=" + VAM_Product_ID);
                            if (VAM_ProductCategory_ID != 0)
                            {
                                sql.Append("and p.VAM_ProductCategory_id=" + VAM_ProductCategory_ID);

                            }
                            sql.Append(" and tr.movementdate   IN (SELECT MAX(movementdate) FROM VAM_Inv_Trx trn INNER JOIN VAM_Product pd ON trn.VAM_Product_id =pd.VAM_Product_id  INNER JOIN VAM_Locator lca ON trn.VAM_Locator_id=lca.VAM_Locator_id");
                            sql.Append(" where trn.movementdate<" + GlobalVariable.TO_DATE(MovementDate, true));
                            sql.Append(" and pd.VAM_Product_id=" + VAM_Product_ID);
                            if (VAM_Warehouse_ID != 0)
                            {
                                sql.Append(" and lca.VAM_Warehouse_id=" + VAM_Warehouse_ID);
                            }
                            sql.Append(" and trn.VAM_Locator_id= " + dslocator.Tables[0].Rows[b]["VAM_Locator_ID"]);
                            if (VAM_ProductCategory_ID != 0)
                            {
                                sql.Append(" and pd.VAM_ProductCategory_id=" + VAM_ProductCategory_ID);
                            }
                            sql.Append(")ORDER BY VAM_Inv_Trx_id DESC");
                            DataSet dsProduct = DB.ExecuteDataset(sql.ToString(), null, null);
                            if (dsProduct.Tables[0].Rows.Count > 0)
                            {
                                int a = 0;
                                for (a = 0; a < dsProduct.Tables[0].Rows.Count; a++)
                                {
                                    // if (Util.GetValueOfInt(dsProduct.Tables[0].Rows[]["VAM_Locator_Id"]) == _SelectedLocator)
                                    //{
                                    _Insert.Append("values(" + Util.GetValueOfInt(dsProduct.Tables[0].Rows[0]["vaf_client_ID"]) + @",
                                                 " + Util.GetValueOfInt(dsProduct.Tables[0].Rows[0]["vaf_org_ID"]) + @",
                                                ' " + Util.GetValueOfString(dsProduct.Tables[0].Rows[0]["Value"]) + @"',
                                                 " + Util.GetValueOfInt(dsProduct.Tables[0].Rows[0]["VAM_ProductCategory_id"]) + @",
                                                " + Util.GetValueOfInt(dsProduct.Tables[0].Rows[0]["VAB_UOM_id"]) + @",
                                                  " + Util.GetValueOfInt(dsProduct.Tables[0].Rows[0]["VAM_Locator_ID"]) + @",
                                                     " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(dsProduct.Tables[0].Rows[0]["movementdate"]), true) + @",
                                                  " + Util.GetValueOfInt(dsProduct.Tables[0].Rows[0]["VAM_Product_id"]) + @",
                                                 " + Util.GetValueOfInt(dsProduct.Tables[0].Rows[0]["currentqty"]) + @", 
                                                  " + Util.GetValueOfInt(dsProduct.Tables[0].Rows[0]["VAM_Warehouse_id"]) + @")");
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

                query = "select * from VAM_Inv_Trx where vaf_client_ID=" + GetCtx().GetVAF_Client_ID() + " order by VAM_Product_ID";
                DataSet dstransaction = DB.ExecuteDataset(query, null, null);
                if (dstransaction.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < dstransaction.Tables[0].Rows.Count; j++)
                    {

                        if (!lst.Contains(new KeyValuePair<int, int>(Util.GetValueOfInt(dstransaction.Tables[0].Rows[j]["VAM_Product_ID"]), Util.GetValueOfInt(dstransaction.Tables[0].Rows[j]["VAM_Locator_ID"]))))
                        {
                            lst.Add(new KeyValuePair<int, int>(Util.GetValueOfInt(dstransaction.Tables[0].Rows[j]["VAM_Product_ID"]), Util.GetValueOfInt(dstransaction.Tables[0].Rows[j]["VAM_Locator_ID"])));
                            _SelectedLocator = Util.GetValueOfInt(dstransaction.Tables[0].Rows[j]["VAM_Locator_ID"]);

                            _Insert = new StringBuilder();
                            _Insert.Append(@"INSERT INTO  VARPT_Temp_Stock 
                                   (VAF_Client_ID,VAF_Org_ID,value,VAM_ProductCategory_id,VAB_UOM_ID,
                                  VAM_Locator_ID,MovementDate,VAM_Product_ID ,CurrentQty,VAM_Warehouse_id)");
                            sql = new StringBuilder();
                            sql.Append(@"SELECT tr.vaf_client_id,tr.vaf_org_id,p.value,
                                   p.VAM_ProductCategory_id,
                                   p.VAB_UOM_id , tr.VAM_Locator_ID , tr.movementdate ,
                                   tr.VAM_Product_id,tr.currentqty,
                                   lc.VAM_Warehouse_id FROM VAM_Inv_Trx tr INNER JOIN VAM_Product p ON tr.VAM_Product_id =p.VAM_Product_id
                                 INNER JOIN VAM_Locator lc ON tr.VAM_Locator_id=lc.VAM_Locator_id
                            where   lc.VAM_Warehouse_ID=" + VAM_Warehouse_ID + " and tr.VAM_Locator_id=" + _SelectedLocator + "  and p.VAM_Product_id=" + dstransaction.Tables[0].Rows[j]["VAM_Product_ID"] + @" 
                                and movementdate<" + GlobalVariable.TO_DATE(MovementDate, true) + @"");
                            if (VAM_Locator_ID != 0)
                            {
                                sql.Append("and tr.VAM_Locator_ID=" + VAM_Locator_ID);
                            }
                            if (VAM_ProductCategory_ID != 0)
                            {
                                sql.Append("and p.VAM_ProductCategory_id=" + VAM_ProductCategory_ID);

                            }
                            sql.Append(" and tr.movementdate   IN (SELECT MAX(movementdate) FROM VAM_Inv_Trx trn INNER JOIN VAM_Product pd ON trn.VAM_Product_id =pd.VAM_Product_id INNER JOIN VAM_Locator lca ON trn.VAM_Locator_id=lca.VAM_Locator_id");
                            sql.Append(" WHERE  lca.VAM_Warehouse_id=" + VAM_Warehouse_ID + " and trn.VAM_Locator_id=" + _SelectedLocator + " and pd.VAM_Product_id=" + dstransaction.Tables[0].Rows[j]["VAM_Product_ID"] + @" 
                            and trn.movementdate<" + GlobalVariable.TO_DATE(MovementDate, true));
                            if (VAM_Locator_ID != 0)
                            {
                                sql.Append(" and trn.VAM_Locator_ID=" + VAM_Locator_ID);
                            }
                            if (VAM_ProductCategory_ID != 0)
                            {
                                sql.Append(" and pd.VAM_ProductCategory_id=" + VAM_ProductCategory_ID);
                            }
                            sql.Append(")order by  VAM_Inv_Trx_id desc");
                            DataSet dsrecord = DB.ExecuteDataset(sql.ToString(), null, null);
                            if (dsrecord.Tables[0].Rows.Count > 0)
                            {

                                for (int a = 0; a < dsrecord.Tables[0].Rows.Count; a++)
                                {
                                    if (Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["VAM_Locator_Id"]) == _SelectedLocator)
                                    {
                                        _Insert.Append("values(" + Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["vaf_client_ID"]) + @",
                                                 " + Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["vaf_org_ID"]) + @",
                                                ' " + Util.GetValueOfString(dsrecord.Tables[0].Rows[a]["Value"]) + @"',
                                                 " + Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["VAM_ProductCategory_id"]) + @",
                                                " + Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["VAB_UOM_id"]) + @",
                                                  " + Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["VAM_Locator_ID"]) + @",
                                                     " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(dsrecord.Tables[0].Rows[a]["movementdate"]), true) + @",
                                                  " + Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["VAM_Product_id"]) + @",
                                                 " + Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["currentqty"]) + @", 
                                                  " + Util.GetValueOfInt(dsrecord.Tables[0].Rows[a]["VAM_Warehouse_id"]) + @")");
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
                else if (name.Equals("VAM_Warehouse_ID"))
                {
                    VAM_Warehouse_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("MovementDate"))
                {
                    MovementDate = Util.GetValueOfDateTime(para[i].GetParameter());
                }
                else if (name.Equals("VAM_Locator_ID"))
                {
                    VAM_Locator_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("VAM_ProductCategory_ID"))
                {
                    VAM_ProductCategory_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("VAM_Product_ID"))
                {
                    VAM_Product_ID = Util.GetValueOfInt(para[i].GetParameter());
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
