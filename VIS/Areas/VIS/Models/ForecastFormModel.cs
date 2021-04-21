using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class ForecastFormModel
    {
        StringBuilder sql = new StringBuilder();
        private static VLogger log = VLogger.GetVLogger(typeof(ForecastFormModel).FullName);
        Trx trx = Trx.GetTrx("Forecast" + DateTime.Now.Ticks);
        int ToCurrency = 0;
        int LineNo = 0;
        int Count = 0;
        int PriceList = 0;
        Decimal ConvertedPrice = 0;
        ValueNamePair pp = null;

        /// <summary>
        /// Load org ID and check window (Either team forecast and master Forecast) and Team Forecast reference 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Table_ID">Table ID</param>
        /// <param name="AD_Record_ID">Record ID</param>
        /// <returns></returns>
        public Dictionary<string, object> GetTableAndRecordInfo(Ctx ctx, string AD_Table_ID, string AD_Record_ID)
        {
            Dictionary<string, object> info = new Dictionary<string, object>();
            info["Table_Name"] = MTable.GetTableName(ctx, Util.GetValueOfInt(AD_Table_ID));
            info["AD_Org_ID"] = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Org_ID FROM " + Util.GetValueOfString(info["Table_Name"]) +
                @" WHERE " + Util.GetValueOfString(info["Table_Name"]) + "_ID = " + AD_Record_ID));
            if (Util.GetValueOfString(info["Table_Name"]).Equals(MMasterForecast.Table_Name))
            {
                info["TeamColumn_ID"] = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Column_ID FROM AD_Column " +
                    @" WHERE columnname = 'C_Forecast_ID' AND AD_Table_ID = (SELECT AD_Table_ID FROM Ad_Table where TableName = '" + MForecastLine.Table_Name + "' )"));
            }
            return info;
        }

        public string CreateForecastLine(Ctx ctx, int Org_ID, int Period_ID, bool IncludeSO, int DocType, bool IncludeOpenSO, string OpenOrders, bool IncludeOpportunity,
            string Opportunities, string ProductCategory, Decimal? BudgetQunatity, bool DeleteAndGenerateLines, int Forecast_ID, int TeamForecast_ID, int Table_ID)
        {
            string tableName = Util.GetValueOfString(DB.ExecuteScalar("SELECT TableName FROM AD_Table WHERE AD_Table_ID =" + Table_ID));
            ToCurrency = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_Currency_ID FROM C_Forecast WHERE C_Forecast_ID =" + Forecast_ID));
            LineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT  NVL(MAX(Line), 0)+10 FROM C_ForecastLine WHERE C_Forecast_ID =" + Forecast_ID));
            if (tableName.Equals("C_Forecast"))
            {
                if (DeleteAndGenerateLines)
                {
                    int count = DB.ExecuteQuery("DELETE FROM C_ForecastLine WHERE C_Forecast_ID=" + Forecast_ID, null, trx);
                    if (count > 0)
                    {
                        log.Log(Level.INFO, "ForecastLinesDeleted" + count);
                    }
                }
                if (IncludeSO || IncludeOpenSO)
                {
                    sql.Append(@"SELECT C_OrderLine.m_product_id,C_OrderLine.QtyOrdered AS BaseQty,C_OrderLine.M_AttributeSetInstance_ID,C_OrderLine.C_UOM_ID, C_OrderLine.C_OrderLine_ID,
                       C_Order.C_Order_ID,NVL(PriceEntered,0) AS Price, NVL(QtyEntered,0) AS ForecastQty,C_Order.C_Currency_ID,M_Product.IsBOM,C_OrderLine.C_Charge_ID
                       FROM C_Order  
                       INNER JOIN C_OrderLine  ON C_Order.C_Order_ID =  C_OrderLine.C_Order_ID 
                       INNER JOIN C_Doctype d ON C_Order.C_DocTypeTarget_ID = d.C_Doctype_ID 
                       LEFT JOIN M_Product ON M_Product.M_Product_ID=C_OrderLine.M_Product_ID
                       WHERE d.DocSubTypeSo NOT IN ('" + MDocType.DOCSUBTYPESO_BlanketOrder + "','" + MDocType.DOCSUBTYPESO_Proposal + "')" +
                       " AND C_Order.IsSOTrx='Y' AND C_Order.IsReturnTrx='N' AND C_Order.AD_Org_ID =" + Org_ID +
                       " AND NVL(C_OrderLine.C_OrderLine_ID,0) NOT IN ( SELECT NVL(C_OrderLine_ID,0) FROM C_ForecastLine INNER JOIN C_Forecast ON C_Forecast.C_Forecast_ID = " +
                       "C_ForecastLine.C_Forecast_ID WHERE C_Forecast.AD_Org_ID =" + Org_ID +
                       " AND C_Forecast.DocStatus NOT IN ('VO','RE')  ) AND C_Order.DocStatus IN('CO','CL')");
                    if (Period_ID > 0)
                    {
                        sql.Append(" AND C_Order.DateOrdered BETWEEN (SELECT startdate FROM C_Period WHERE C_Period_ID =  " + Period_ID + ")" +
                            " AND (SELECT ENDDATE FROM C_Period WHERE C_Period_ID = " + Period_ID + ") ");
                    }

                    if (DocType > 0)
                    {
                        sql.Append(" AND C_Order.C_DocTypeTarget_ID= " + DocType);
                    }
                    // open and fully delivered orders
                    if (IncludeOpenSO && IncludeSO)
                    {
                        //if no order is selected then get all the open and fully delivered orders 
                        if (string.IsNullOrEmpty(OpenOrders))
                        {
                            sql.Append(" AND C_OrderLine.QtyOrdered >= C_OrderLine.QtyDelivered");
                        }

                        else
                        {
                            //if order is selected then fetch all the fully delivered orders and check if selected orders are open 
                            sql.Append(" AND C_OrderLine.QtyOrdered = C_OrderLine.QtyDelivered OR (C_Order.C_Order_ID IN (" + OpenOrders + ")" +
                                " AND C_OrderLine.QtyOrdered > C_OrderLine.QtyDelivered)");
                        }

                    }
                    //only fully delivered orders
                    else if (!IncludeOpenSO && IncludeSO)
                    {
                        sql.Append(" AND C_OrderLine.QtyOrdered = C_OrderLine.QtyDelivered");
                    }
                    //only open orders
                    else if (IncludeOpenSO && !IncludeSO)
                    {
                        sql.Append(" AND C_OrderLine.QtyOrdered > C_OrderLine.QtyDelivered");
                        //if orders are selected then fetch selected open orders
                        if (!string.IsNullOrEmpty(OpenOrders))
                        {
                            sql.Append(" AND C_Order.C_Order_ID IN (" + OpenOrders + ")");
                        }
                    }

                    string sql1 = MRole.GetDefault(ctx).AddAccessSQL(sql.ToString(), "C_Order", true, true);
                    DataSet ds = DB.ExecuteDataset(sql1, null, trx);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            //Price conversion from Order currency to Forecast Currency
                            ConvertedPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Price"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Currency_ID"]),
                            ToCurrency, ctx.GetAD_Client_ID(), Org_ID);

                            //create forecast lines 
                            CreateTeamForecastLines(ctx, Forecast_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Order_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_OrderLine_ID"]), 0, 0,
                            Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Charge_ID"]), Org_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                            Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                            Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["BaseQty"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ForecastQty"]), ConvertedPrice);
                        }
                    }
                }


                if (IncludeOpportunity)
                {
                    OpportunityProducts(ctx, Org_ID, Period_ID, Forecast_ID, Opportunities);
                }
                if (!string.IsNullOrEmpty(ProductCategory))
                {
                    PriceList = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_PriceList_ID FROM C_Forecast WHERE C_Forecast_ID=" + Forecast_ID));
                    ProductCategoryProducts(ctx, Org_ID, ProductCategory, BudgetQunatity, PriceList, Forecast_ID);
                }

                if (Count > 0)
                {
                    trx.Commit();
                }
            }


            return Msg.GetMsg(ctx, "LinesInsterted") + Count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="Org_ID"></param>
        /// <param name="Period"></param>
        /// <param name="Forecast_ID"></param>
        /// <param name="Opportunities"></param>
        public void OpportunityProducts(Ctx ctx, int Org_ID, int Period, int Forecast_ID, string Opportunities)
        {
            sql.Clear();
            sql.Append(@"SELECT ProjectLine.M_Product_ID, Project.C_Project_ID,Project.C_Currency_ID,ProjectLine.C_ProjectLine_ID, ProjectLine.PlannedQty AS ForecastQty,
                ProjectLine.BaseQty,ProjectLine.C_UOM_ID,ProjectLine.PlannedPrice AS Price,ProjectLine.M_AttributeSetInstance_ID ,Product.IsBOM 
                FROM C_Project Project INNER JOIN C_ProjectLine ProjectLine ON Project.C_Project_ID = ProjectLine.C_Project_ID INNER JOIN M_Product Product ON Product.M_Product_ID= 
                 ProjectLine.M_Product_ID WHERE Project.C_Order_ID IS NULL " +
                "AND Project.Ref_Order_ID IS NULL AND Project.AD_Org_ID = " + Org_ID + " AND NVL(C_ProjectLine_ID,0) NOT IN ( SELECT NVL(C_ProjectLine_ID,0) " +
                "FROM C_ForecastLine INNER JOIN C_Forecast ON C_Forecast.C_Forecast_ID = C_ForecastLine.C_Forecast_ID WHERE C_Forecast.AD_Org_ID =" + Org_ID +
                " AND C_Forecast.DocStatus NOT IN ('VO','RE'))");
            if (Period > 0)
            {
                sql.Append(" AND Project.C_Period_ID = " + Period);
            }
            if (!string.IsNullOrEmpty(Opportunities))
            {
                //if opportunities are selected then fetch only selected one 
                sql.Append(" AND Project.C_Project_ID IN(" + Opportunities + ")");
            }

            string sql1 = MRole.GetDefault(ctx).AddAccessSQL(sql.ToString(), "C_Project", true, true);
            DataSet ds = DB.ExecuteDataset(sql1, null, trx);
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //Price conversion from Opportunity currency to Forecast Currency
                    ConvertedPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Price"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Currency_ID"]),
                    ToCurrency, ctx.GetAD_Client_ID(), Org_ID);

                    //create forecast lines
                    CreateTeamForecastLines(ctx, Forecast_ID, 0, 0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Project_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ProjectLine_ID"]),
                    0, Org_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                    Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["BaseQty"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ForecastQty"]), ConvertedPrice);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="Org_ID"></param>
        /// <param name="ProductCategories"></param>
        /// <param name="BudgetQuantity"></param>
        /// <param name="PriceList"></param>
        /// <param name="Forecast_ID"></param>
        public void ProductCategoryProducts(Ctx ctx, int Org_ID, string ProductCategories, Decimal? BudgetQuantity, int PriceList, int Forecast_ID)
        {
            sql.Clear();
            sql.Append(@"SELECT M_Product.M_Product_ID,M_Product.C_UOM_ID,M_ProductPrice.PriceStd,M_Product.M_AttributeSetInstance_ID,M_Product.IsBOM,M_Product.C_Currency_ID 
            FROM M_Product LEFT JOIN M_ProductPrice  ON M_Product.M_Product_ID = M_ProductPrice.M_Product_ID AND M_ProductPrice.M_PriceList_Version_ID = 
            (SELECT MAX(M_PriceList_Version_ID) FROM M_PriceList_Version WHERE IsActive = 'Y' AND M_PriceList_ID =" + PriceList + " ) " +
            "AND M_ProductPrice.C_UOM_ID=M_Product.C_UOM_ID AND NVL(M_ProductPrice.M_AttributeSetInstance_ID,0)=NVL(M_Product.M_AttributeSetInstance_ID,0) " +
            "WHERE M_Product_Category_ID IN(" + ProductCategories + ") AND M_Product.AD_Org_ID =" + Org_ID);
            string sql1 = MRole.GetDefault(ctx).AddAccessSQL(sql.ToString(), "M_Product", true, true);
            DataSet ds = DB.ExecuteDataset(sql1, null, trx);
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //Price conversion from Product currency to Forecast Currency
                    ConvertedPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["PriceStd"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Currency_ID"]),
                    ToCurrency, ctx.GetAD_Client_ID(), Org_ID);

                    //Create forecast lines
                    CreateTeamForecastLines(ctx, Forecast_ID, 0, 0, 0, 0, 0, Org_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                    Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                    Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), BudgetQuantity, BudgetQuantity, ConvertedPrice);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="Forecast_ID"></param>
        /// <param name="Order_ID"></param>
        /// <param name="OrderLine_ID"></param>
        /// <param name="Project_ID"></param>
        /// <param name="ProjectLine_ID"></param>
        /// <param name="Charge_ID"></param>
        /// <param name="Org_ID"></param>
        /// <param name="LineNo"></param>
        /// <param name="Product_ID"></param>
        /// <param name="Attribute_ID"></param>
        /// <param name="UOM_ID"></param>
        /// <param name="BOM"></param>
        /// <param name="Quantity"></param>
        /// <param name="UnitPrice"></param>
        /// <returns></returns>
        public string CreateTeamForecastLines(Ctx ctx, int Forecast_ID, int Order_ID, int OrderLine_ID, int Project_ID, int ProjectLine_ID, int Charge_ID, int Org_ID, int Product_ID, int Attribute_ID, int UOM_ID, String BOM, Decimal? BaseQuantity, Decimal? Forecastqty, decimal UnitPrice)
        {
            MForecastLine Line = new MForecastLine(ctx, 0, trx);
            Line.SetAD_Client_ID(ctx.GetAD_Client_ID());
            Line.SetAD_Org_ID(Org_ID);
            Line.SetC_Forecast_ID(Forecast_ID);
            Line.SetLine(LineNo);
            Line.SetC_Order_ID(Order_ID);
            Line.SetC_OrderLine_ID(OrderLine_ID);
            Line.SetC_Project_ID(Project_ID);
            Line.SetC_ProjectLine_ID(ProjectLine_ID);
            Line.SetC_Charge_ID(Charge_ID);
            Line.SetM_Product_ID(Product_ID);
            Line.SetM_AttributeSetInstance_ID(Attribute_ID);
            Line.SetIsBOM(BOM.Equals("Y") ? true : false);
            Line.SetC_UOM_ID(UOM_ID);
            Line.SetBaseQty(Forecastqty);
            Line.SetQtyEntered(BaseQuantity);
            Line.SetUnitPrice(UnitPrice);
            Line.SetTotalPrice(UnitPrice * Forecastqty);

            if (!Line.Save(trx))
            {
                pp = VLogger.RetrieveError();
                if (pp != null)
                {
                    string val = pp.GetName();
                    if (string.IsNullOrEmpty(val))
                    {
                        val = pp.GetValue();
                    }
                    log.Log(Level.SEVERE, Msg.GetMsg(ctx, "TeamForecastNot") + val);

                }

            }
            else
            {
                LineNo += 10;
                Count++;
            }
            return "";
        }
    }
}