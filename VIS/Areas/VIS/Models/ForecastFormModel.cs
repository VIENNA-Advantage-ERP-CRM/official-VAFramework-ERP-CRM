
/********************************************************
 * Module Name    :    VA Framework
 * Purpose        :    Implement Generate Lines functionality for Team amd Master Forecast
 * Employee Code  :    209
 * Date           :    26-April-2021
  ******************************************************/

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
        #region Private Variables
        private StringBuilder sql = new StringBuilder();
        private static VLogger log = VLogger.GetVLogger(typeof(ForecastFormModel).FullName);
        private ValueNamePair pp = null;
        private string msg = null;
        private string ToCurrencyName = "";
        private string FromCurrencyName = "";
        private int ToCurrency = 0;
        private int LineNo = 0;
        private int MFLineNo = 0;
        private int Count = 0;
        private int PriceList = 0;
        private Decimal ConvertedPrice = 0;
        private int ConversionType = 0;
        private DataSet ds = null;
        private DateTime? DateTrx = null;
        private int Precision = 0;
        #endregion
        /// <summary>
        /// Load org ID and check window (Either team forecast and master Forecast) and Team Forecast reference 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Table_ID">Table ID</param>
        /// <param name="AD_Record_ID">Record ID</param>
        /// <Writer>VIS_0045</Writer>
        /// <returns>info</returns>
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

        /// <summary>
        /// Generate Lines for MasterForecast and TeamForecast
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="Org_ID">Organiation</param>
        /// <param name="Period_ID">Period</param>
        /// <param name="IncludeSO">IncludeSO</param>
        /// <param name="DocType">Document Type</param>
        /// <param name="IncludeOpenSO">IncludeOpenSO</param>
        /// <param name="OpenOrders">Sales Orders</param>
        /// <param name="IncludeOpportunity">IncludeOpportunity</param>
        /// <param name="Opportunities">Product Category</param>
        /// <param name="ProductCategory"></param>
        /// <param name="BudgetQunatity">BudgetQunatity</param>
        /// <param name="DeleteAndGenerateLines">DeleteAndGenerateLines</param>
        /// <param name="Forecast_ID">Team/Master Forecast</param>
        /// <param name="TeamForecast_IDs">Team Forecast References</param>
        /// <param name="Table_ID">Table</param>
        /// <param name="IsMasterForecast"></param>
        /// <Writer>209</Writer>
        /// <returns>info</returns>
        public string CreateForecastLine(Ctx ctx, int Org_ID, int Period_ID, bool IncludeSO, int DocType, bool IncludeOpenSO, string OpenOrders, bool IncludeOpportunity,
            string Opportunities, string ProductCategory, Decimal? BudgetQunatity, bool DeleteAndGenerateLines, int Forecast_ID, string TeamForecast_IDs, int Table_ID, bool IsMasterForecast)
        {
            Trx trx = null;
            string TableName = "";
            try
            {
                trx = Trx.GetTrx("Forecast" + DateTime.Now.Ticks);
                TableName = MTable.GetTableName(ctx, Util.GetValueOfInt(Table_ID));
                
                //Get Currency and conversion Type from header              
                ds = DB.ExecuteDataset("SELECT Forecast.C_Currency_ID ,Forecast.TrxDate,C_Currency.ISO_CODE,C_Currency.StdPrecision,C_ConversionType_ID FROM " + TableName + " Forecast INNER JOIN C_Currency ON " +
                    "C_Currency.C_Currency_ID = Forecast.C_Currency_ID WHERE " + TableName + "_ID =" + Forecast_ID);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    ToCurrency = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_Currency_ID"]);
                    ToCurrencyName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ISO_CODE"]);
                    DateTrx = Util.GetValueOfDateTime(ds.Tables[0].Rows[0]["TrxDate"]);
                    ConversionType = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_ConversionType_ID"]);
                    Precision = Util.GetValueOfInt(ds.Tables[0].Rows[0]["StdPrecision"]);
                }

                if (DeleteAndGenerateLines)
                {
                    int count = DB.ExecuteQuery("DELETE FROM " + TableName + "Line WHERE " + TableName + "_ID =" + Forecast_ID, null, trx);
                    if (count > 0)
                    {
                        log.Log(Level.INFO, "ForecastLinesDeleted" + count);
                    }
                }
                ////fetch lineNo for Team Forecast line 
                //LineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT  NVL(MAX(Line), 0)+10 FROM " + TableName + "Line WHERE " + TableName + "_ID =" + Forecast_ID));
                if (IncludeSO)
                {
                    sql.Append(@"SELECT OrderLine.M_Product_ID,OrderLine.QtyOrdered AS BaseQty,OrderLine.M_AttributeSetInstance_ID,OrderLine.C_UOM_ID, 
                       OrderLine.C_OrderLine_ID,Orders.C_Order_ID,NVL(PriceEntered,0) AS Price, NVL(QtyEntered,0) AS ForecastQty,Orders.C_Currency_ID,
                       Product.IsBOM,Currency.ISO_CODE
                       FROM C_Order Orders
                       INNER JOIN C_OrderLine OrderLine  ON Orders.C_Order_ID =  OrderLine.C_Order_ID 
                       INNER JOIN C_Doctype d ON Orders.C_DocTypeTarget_ID = d.C_Doctype_ID 
                       INNER JOIN C_Currency Currency ON Currency.C_Currency_ID=Orders.C_Currency_ID
                       LEFT JOIN M_Product Product ON Product.M_Product_ID=OrderLine.M_Product_ID
                       WHERE d.DocSubTypeSo NOT IN ('" + MDocType.DOCSUBTYPESO_BlanketOrder + "','" + MDocType.DOCSUBTYPESO_Proposal + "')" +
                       " AND Orders.IsSOTrx='Y' AND Orders.IsReturnTrx='N' AND Orders.AD_Org_ID =" + Org_ID +
                       " AND Orders.DocStatus IN('CO','CL') AND OrderLine.QtyOrdered = OrderLine.QtyDelivered AND NVL(OrderLine.C_OrderLine_ID,0) NOT IN");
                    if (!IsMasterForecast)
                    {
                        //Team Forecast --case
                        sql.Append("(SELECT NVL(C_OrderLine_ID, 0) FROM C_ForecastLine " +
                       "INNER JOIN C_Forecast ON C_Forecast.C_Forecast_ID = C_ForecastLine.C_Forecast_ID WHERE C_Forecast.AD_Org_ID =" + Org_ID +
                       " AND C_Forecast.DocStatus NOT IN ('VO','RE')  )");
                    }
                    else
                    {
                        // Master Forecast --case
                        sql.Append("(SELECT NVL(C_OrderLine_ID, 0) FROM C_MasterForecastLineDetails LineDetails " +
                            "INNER JOIN C_MasterForecastLine Line ON Line.C_MasterForecastLine_ID=LineDetails.C_MasterForecastLine_ID " +
                            "INNER JOIN C_MasterForecast Forecast ON Forecast.C_MasterForecast_ID=Line.C_MasterForecast_ID WHERE Forecast.AD_Org_ID =" + Org_ID +
                            " AND Forecast.DocStatus NOT IN ('VO','RE')   ) AND NVL(OrderLine.C_OrderLine_ID,0) NOT IN (SELECT NVL(line.C_OrderLine_ID, 0) " +
                            " FROM C_ForecastLine Line INNER JOIN C_Forecast Forecast ON Forecast.C_Forecast_ID = Line.C_Forecast_ID INNER JOIN " +
                            " C_MasterForecastLineDetails Details ON  line.C_ForecastLine_ID = Details.C_ForecastLine_ID INNER JOIN c_masterforecastline  mLine " +
                            " ON mline.C_MasterForecastLine_ID = Details.C_MasterForecastLine_ID INNER JOIN C_MasterForecast master ON master.C_MasterForecast_ID = " +
                            " mline.C_MasterForecast_ID WHERE Forecast.ad_org_id =  " + Org_ID +" AND Forecast.docstatus NOT IN ( 'VO', 'RE' ) AND master.ad_org_id =  "+ Org_ID + 
                            " AND master.docstatus NOT IN ( 'VO', 'RE' ))");                       
                       
                    }
                    if (Period_ID > 0)
                    {
                        sql.Append(" AND Orders.DateOrdered BETWEEN (SELECT startdate FROM C_Period WHERE C_Period_ID =  " + Period_ID + ")" +
                            " AND (SELECT ENDDATE FROM C_Period WHERE C_Period_ID = " + Period_ID + ") ");
                    }
                    if (DocType > 0)
                    {
                        sql.Append(" AND Orders.C_DocTypeTarget_ID= " + DocType);
                    }

                    sql.Append(" ORDER BY Currency.ISO_CODE ");
                  
                    string sql1 = MRole.GetDefault(ctx).AddAccessSQL(sql.ToString(), "C_Order", true, true);
                    ds = DB.ExecuteDataset(sql1, null, trx);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            //Price conversion from Orderss currency to Forecast Currency
                            ConvertedPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Price"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Currency_ID"]),
                            ToCurrency, DateTrx, ConversionType, ctx.GetAD_Client_ID(), Org_ID);

                            if (ConvertedPrice == 0)
                            {
                                if (!FromCurrencyName.Contains(Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"])))
                                {
                                    FromCurrencyName += Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"])+",";
                                }
                                continue;
                            }

                            if (!IsMasterForecast)
                            {
                                //create forecast lines 
                                CreateTeamForecastLines(ctx, trx, Forecast_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Order_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_OrderLine_ID"]), 0, 0,
                                0, Org_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                                Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                                Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["BaseQty"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ForecastQty"]), ConvertedPrice);

                            }
                            else
                            {
                                //create master forecast lines
                                CreateMasterForecastLines(ctx, trx, Org_ID, Forecast_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Order_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_OrderLine_ID"]), 0, 0, 0, 0,
                                0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                                Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                                Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["BaseQty"]), ConvertedPrice, "");
                            }
                        }
                    }
                }

                if (IncludeOpenSO)
                {
                    sql.Clear();
                    //fetch only open sales order 
                    sql.Append(@"SELECT OrderLine.M_Product_ID,OrderLine.QtyOrdered AS BaseQty,OrderLine.M_AttributeSetInstance_ID,OrderLine.C_UOM_ID, 
                       OrderLine.C_OrderLine_ID,Orders.C_Order_ID,NVL(PriceEntered,0) AS Price, NVL(QtyEntered,0) AS ForecastQty,Orders.C_Currency_ID,
                       Product.IsBOM,Currency.ISO_CODE
                       FROM C_Order Orders
                       INNER JOIN C_OrderLine OrderLine  ON Orders.C_Order_ID =  OrderLine.C_Order_ID 
                       INNER JOIN C_Doctype d ON Orders.C_DocTypeTarget_ID = d.C_Doctype_ID 
                       INNER JOIN C_Currency Currency ON Currency.C_Currency_ID=Orders.C_Currency_ID
                       LEFT JOIN M_Product Product ON Product.M_Product_ID=OrderLine.M_Product_ID
                       WHERE d.DocSubTypeSo NOT IN ('" + MDocType.DOCSUBTYPESO_BlanketOrder + "','" + MDocType.DOCSUBTYPESO_Proposal + "')" +
                      " AND Orders.IsSOTrx='Y' AND Orders.IsReturnTrx='N' AND Orders.AD_Org_ID =" + Org_ID +
                      " AND Orders.DocStatus IN('CO','CL') AND OrderLine.QtyOrdered > OrderLine.QtyDelivered AND NVL(OrderLine.C_OrderLine_ID,0) NOT IN");
                    if (!IsMasterForecast)
                    {
                        //Team Forecast -- case OrderLine reference must not present in Teamforecast line
                        sql.Append("(SELECT NVL(C_OrderLine_ID, 0) FROM C_ForecastLine " +
                       "INNER JOIN C_Forecast ON C_Forecast.C_Forecast_ID = C_ForecastLine.C_Forecast_ID WHERE C_Forecast.AD_Org_ID =" + Org_ID +
                       " AND C_Forecast.DocStatus NOT IN ('VO','RE')  )");
                    }
                    else
                    {
                        // Master Forecast -- case OrderLine reference must not present in masterforecast linedetails 
                        sql.Append("(SELECT NVL(C_OrderLine_ID, 0) FROM C_MasterForecastLineDetails LineDetails " +
                            "INNER JOIN C_MasterForecastLine Line ON Line.C_MasterForecastLine_ID=LineDetails.C_MasterForecastLine_ID " +
                            "INNER JOIN C_MasterForecast Forecast ON Forecast.C_MasterForecast_ID=Line.C_MasterForecast_ID WHERE Forecast.AD_Org_ID =" + Org_ID +
                            " AND Forecast.DocStatus NOT IN ('VO','RE')) AND NVL(OrderLine.C_OrderLine_ID,0) NOT IN (SELECT NVL(line.C_OrderLine_ID, 0) " +
                            " FROM C_ForecastLine Line INNER JOIN C_Forecast Forecast ON Forecast.C_Forecast_ID = Line.C_Forecast_ID INNER JOIN " +
                            " C_MasterForecastLineDetails Details ON  line.C_ForecastLine_ID = Details.C_ForecastLine_ID INNER JOIN c_masterforecastline  mLine " +
                            " ON mline.C_MasterForecastLine_ID = Details.C_MasterForecastLine_ID INNER JOIN C_MasterForecast master ON master.C_MasterForecast_ID = " +
                            " mline.C_MasterForecast_ID WHERE Forecast.ad_org_id =  " + Org_ID + " AND Forecast.docstatus NOT IN ( 'VO', 'RE' ) AND master.ad_org_id =  " + Org_ID +
                            " AND master.docstatus NOT IN ( 'VO', 'RE' ))");
                      
                    }
                    if (!string.IsNullOrEmpty(OpenOrders))
                    {
                        sql.Append(" AND Orders.C_Order_ID IN (" + OpenOrders + ")");
                    }
                    sql.Append(" ORDER BY Currency.ISO_CODE ");
                    string sql1 = MRole.GetDefault(ctx).AddAccessSQL(sql.ToString(), "C_Order", true, true);
                    ds = DB.ExecuteDataset(sql1, null, trx);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            //Price conversion from Orderss currency to Forecast Currency
                            ConvertedPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Price"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Currency_ID"]),
                            ToCurrency, DateTrx, ConversionType, ctx.GetAD_Client_ID(), Org_ID);

                            if (ConvertedPrice == 0)
                            {
                                if (!FromCurrencyName.Contains(Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"])))
                                {
                                    FromCurrencyName +=Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"])+",";
                                }
                                continue;
                            }

                            if (!IsMasterForecast)
                            {
                                //create forecast lines 
                                CreateTeamForecastLines(ctx, trx, Forecast_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Order_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_OrderLine_ID"]), 0, 0,
                                0, Org_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                                Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                                Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["BaseQty"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ForecastQty"]), ConvertedPrice);

                            }
                            else
                            {
                                CreateMasterForecastLines(ctx, trx, Org_ID, Forecast_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Order_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_OrderLine_ID"]), 0, 0, 0, 0,
                                0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                                Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                                Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["BaseQty"]), ConvertedPrice, "");
                            }
                        }
                    }
                }
                //Include Opportunities for both Master and Team Forecast
                if (IncludeOpportunity)
                {
                    OpportunityProducts(ctx, trx, Org_ID, Period_ID, Forecast_ID, Opportunities, IsMasterForecast,TeamForecast_IDs);
                }
                if (!string.IsNullOrEmpty(ProductCategory))
                {
                    //get Pricelistfrom header 
                    PriceList = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_PriceList_ID FROM " + TableName + " WHERE " + TableName + "_ID=" + Forecast_ID));
                    ProductCategoryProducts(ctx, trx, Org_ID, ProductCategory, BudgetQunatity, PriceList, Forecast_ID, IsMasterForecast);
                }
                if (IsMasterForecast && !string.IsNullOrEmpty(TeamForecast_IDs))
                {
                    //Only in case of Master Forecast
                    TeamForecastProducts(ctx, trx, Org_ID, Period_ID, Forecast_ID, TeamForecast_IDs, IsMasterForecast,IncludeOpenSO,IncludeSO,IncludeOpportunity);
                }
            }
            catch (Exception e)
            {
                if (trx != null)
                {
                    trx.Rollback();
                    trx.Close();
                }
                log.Log(Level.SEVERE, "", e.Message);
            }

            if (Count > 0)
            {
                trx.Commit();
                trx.Close();
            }

            //if conversion not found then display this message 
            if (!string.IsNullOrEmpty(FromCurrencyName))
            {
                msg = Msg.GetMsg(ctx, "ConversionNotFound") + " " + FromCurrencyName.Trim(',') +" "+ Msg.GetMsg(ctx, "To") + " " + ToCurrencyName;

            }

            return Msg.GetMsg(ctx, "LinesInsterted") + " " + Count + " " + msg;
        }

        /// <summary>
        /// Fetch Opportunities 
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="trx">Transaction</param>
        /// <param name="Org_ID">Organization</param>
        /// <param name="Period">Period</param>
        /// <param name="Forecast_ID">Master/Team Forecast</param>
        /// <param name="Opportunities">Opportunity</param>
        /// <param name="IsMasterForecast">IsMasterForecast</param>
        /// <Writer>209</Writer>
        public void OpportunityProducts(Ctx ctx, Trx trx, int Org_ID, int Period, int Forecast_ID, string Opportunities, bool IsMasterForecast ,string TeamForecast_IDs)
        {
            sql.Clear();
            sql.Append(@"SELECT ProjectLine.M_Product_ID, Project.C_Project_ID,Project.C_Currency_ID,ProjectLine.C_ProjectLine_ID, ProjectLine.PlannedQty AS ForecastQty,
                ProjectLine.BaseQty,ProjectLine.C_UOM_ID,ProjectLine.PlannedPrice AS Price,ProjectLine.M_AttributeSetInstance_ID ,Product.IsBOM , Currency.ISO_CODE
                FROM C_Project Project INNER JOIN C_ProjectLine ProjectLine ON Project.C_Project_ID = ProjectLine.C_Project_ID INNER JOIN M_Product Product ON Product.M_Product_ID= 
                 ProjectLine.M_Product_ID INNER JOIN C_Currency Currency ON Currency.C_Currency_ID= Project.C_Currency_ID WHERE Project.C_Order_ID IS NULL " +
                "AND Project.Ref_Order_ID IS NULL AND Project.AD_Org_ID = " + Org_ID + " AND NVL(C_ProjectLine_ID,0) NOT IN ");

            if (!IsMasterForecast)
            {
                //Team Forecast-- case ProjectLine reference must not present in TeamforecastLine
                sql.Append("( SELECT NVL(C_ProjectLine_ID,0) " +
                "FROM C_ForecastLine INNER JOIN C_Forecast ON C_Forecast.C_Forecast_ID = C_ForecastLine.C_Forecast_ID WHERE C_Forecast.AD_Org_ID =" + Org_ID +
                " AND C_Forecast.DocStatus NOT IN ('VO','RE'))");
            }
            else
            {
                //master forecast --  case ProjectLine reference must not present in Masterforecastlinedetails
                sql.Append("(SELECT NVL(C_ProjectLine_ID, 0) FROM C_MasterForecastLineDetails LineDetails " +
                            "INNER JOIN C_MasterForecastLine Line ON Line.C_MasterForecastLine_ID=LineDetails.C_MasterForecastLine_ID " +
                            "INNER JOIN C_MasterForecast Forecast ON Forecast.C_MasterForecast_ID=Line.C_MasterForecast_ID WHERE Forecast.AD_Org_ID =" + Org_ID +
                            " AND Forecast.DocStatus NOT IN ('VO','RE')) AND NVL(C_ProjectLine_ID,0) NOT IN (SELECT NVL(line.C_ProjectLine_ID, 0) " +
                            " FROM C_ForecastLine Line INNER JOIN C_Forecast Forecast ON Forecast.C_Forecast_ID = Line.C_Forecast_ID INNER JOIN " +
                            " C_MasterForecastLineDetails Details ON  line.C_ForecastLine_ID = Details.C_ForecastLine_ID INNER JOIN c_masterforecastline  mLine " +
                            " ON mline.C_MasterForecastLine_ID = Details.C_MasterForecastLine_ID INNER JOIN C_MasterForecast master ON master.C_MasterForecast_ID = " +
                            " mline.C_MasterForecast_ID WHERE Forecast.ad_org_id =  " + Org_ID + " AND Forecast.docstatus NOT IN ( 'VO', 'RE' ) AND master.ad_org_id =  " + Org_ID +
                            " AND master.docstatus NOT IN ( 'VO', 'RE' ))");
                if (!string.IsNullOrEmpty(TeamForecast_IDs))
                {
                    sql.Append(" AND forecast.C_Forecast_ID IN (" + TeamForecast_IDs + ")");
                }
            }

            if (!string.IsNullOrEmpty(Opportunities))
            {
                //if opportunities are selected then fetch only selected one 
                sql.Append(" AND Project.C_Project_ID IN(" + Opportunities + ")");
            }
            sql.Append(" ORDER BY Currency.ISO_CODE ");
            string sql1 = MRole.GetDefault(ctx).AddAccessSQL(sql.ToString(), "C_Project", true, true);
            ds = DB.ExecuteDataset(sql1, null, trx);
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //Price conversion from Opportunity currency to Forecast Currency
                    ConvertedPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Price"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Currency_ID"]),
                    ToCurrency, DateTrx, ConversionType, ctx.GetAD_Client_ID(), Org_ID);

                    if (ConvertedPrice == 0 )
                    {
                        if (!FromCurrencyName.Contains(Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"])))
                        {
                            FromCurrencyName += Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"])+",";
                        }
                        continue;
                    }

                    if (!IsMasterForecast)
                    {
                        //Create TeamForecast Lines
                        CreateTeamForecastLines(ctx, trx, Forecast_ID, 0, 0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Project_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ProjectLine_ID"]),
                        0, Org_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                        Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["BaseQty"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ForecastQty"]), ConvertedPrice);
                    }
                    else
                    {
                        //Create MasterForecast Lines
                        CreateMasterForecastLines(ctx, trx, Org_ID, Forecast_ID, 0, 0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Project_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ProjectLine_ID"]), 0, 0,
                        0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                        Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                        Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["BaseQty"]), ConvertedPrice, "");
                    }
                }
            }
        }

        /// <summary>
        /// Fetch Products from Product window on the basis of Product category
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="trx">Transaction</param>
        /// <param name="Org_ID">Organization</param>
        /// <param name="ProductCategories">Product Categor</param>
        /// <param name="BudgetQuantity">Quantity</param>
        /// <param name="PriceList">Price list</param>
        /// <param name="Forecast_ID">Master/Team Forecast</param>
        /// <param name="IsMasterFoecast">IsmasterForecast</param>
        /// <Writer>209</Writer>
        public void ProductCategoryProducts(Ctx ctx, Trx trx, int Org_ID, string ProductCategories, Decimal? BudgetQuantity, int PriceList, int Forecast_ID, bool IsMasterFoecast)
        {
            sql.Clear();
            sql.Append(@"SELECT Product.M_Product_ID,Product.C_UOM_ID,ProductPrice.PriceStd,Product.M_AttributeSetInstance_ID,Product.IsBOM
            FROM M_Product Product LEFT JOIN M_ProductPrice ProductPrice ON Product.M_Product_ID = ProductPrice.M_Product_ID AND ProductPrice.M_PriceList_Version_ID = 
            (SELECT MAX(M_PriceList_Version_ID) FROM M_PriceList_Version WHERE IsActive = 'Y' AND M_PriceList_ID =" + PriceList + " ) " +
            "AND ProductPrice.C_UOM_ID=Product.C_UOM_ID AND NVL(ProductPrice.M_AttributeSetInstance_ID,0)=NVL(Product.M_AttributeSetInstance_ID,0) " +
            "WHERE Product.M_Product_Category_ID IN(" + ProductCategories + ") AND Product.AD_Org_ID IN (0," + Org_ID + ")");


            string sql1 = MRole.GetDefault(ctx).AddAccessSQL(sql.ToString(), "M_Product", true, true);
            ds = DB.ExecuteDataset(sql1, null, trx);
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!IsMasterFoecast)
                    {
                        //Create TeamForecast Lines
                        CreateTeamForecastLines(ctx, trx, Forecast_ID, 0, 0, 0, 0, 0, Org_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                        Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                        Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), BudgetQuantity, BudgetQuantity, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["PriceStd"]));
                    }
                    else
                    {
                        //Create MasterForecast Lines
                        CreateMasterForecastLines(ctx, trx, Org_ID, Forecast_ID, 0, 0, 0, 0, 0, 0, 0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                      Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                      Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), BudgetQuantity, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["PriceStd"]), ProductCategories);
                    }
                }
            }
        }


        /// <summary>
        /// Fetch products from team forecast and create masterForecastlines 
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="trx">Trasaction</param>
        /// <param name="Org_ID">Organization</param>
        /// <param name="Period">Period</param>
        /// <param name="Forecast_ID">MasterForecast</param>
        /// <param name="TeamForecast_IDs">TeamForecast</param>
        /// <param name="IsMasterForecast">IsMasterForecast</param>
        /// <Writer>209</Writer>
        public void TeamForecastProducts(Ctx ctx, Trx trx, int Org_ID, int Period, int Forecast_ID, string TeamForecast_IDs, bool IsMasterForecast, bool IncludeOpenSO, bool IncludeSO, bool IncludeOpp)
        {
            sql.Clear();
            sql.Append(@"SELECT TLine.M_Product_ID,TLine.M_AttributeSetInstance_ID,TLine.QtyEntered,TForecast.C_Forecast_ID, C_ForecastLine_ID,
            TLine.C_UOM_ID,NVL(UnitPrice,0) AS Price,TForecast.C_Currency_ID,Product.ISBOM,Currency.ISO_CODE FROM C_Forecast TForecast 
            INNER JOIN C_Forecastline TLine ON TLine.C_Forecast_ID = TForecast.C_Forecast_ID LEFT JOIN M_Product Product ON Product.M_Product_ID=TLine.M_Product_ID
            INNER JOIN C_Currency Currency ON Currency.C_Currency_ID= TForecast.C_Currency_ID 
            WHERE  TForecast.AD_Org_ID = " + Org_ID + "   AND TForecast.DocStatus IN ('CO','CL') AND NVL(TLine.C_ForecastLine_ID,0) NOT IN " +
            "(SELECT NVL(C_ForecastLine_ID,0) FROM C_MasterForecastlinedetails LineDetails INNER JOIN C_MasterForecastLine Line ON Line.C_MasterForecastLine_ID=" +
            " LineDetails.C_MasterForecastLine_ID INNER JOIN C_MasterForecast Forecast ON Forecast.C_MasterForecast_ID=Line.C_MasterForecast_ID WHERE " +
            " Forecast.AD_Org_ID =" + Org_ID + " AND Forecast.DocStatus NOT IN ('VO','RE')) AND NVL(TLine.C_OrderLine_ID,0) NOT IN (SELECT NVL(C_OrderLine_ID,0) FROM " +
            " C_MasterForecastlinedetails LineDetails INNER JOIN C_MasterForecastLine Line ON Line.C_MasterForecastLine_ID=LineDetails.C_MasterForecastLine_ID " +
            " INNER JOIN C_MasterForecast Forecast ON Forecast.C_MasterForecast_ID=Line.C_MasterForecast_ID WHERE Forecast.AD_Org_ID =" + Org_ID + " AND Forecast.DocStatus " +
            " NOT IN('VO','RE') AND C_OrderLine_ID IS NOT NULL) AND NVL(TLine.C_ProjectLine_ID,0) NOT IN (SELECT NVL(C_ProjectLine_ID,0) FROM C_MasterForecastlinedetails LineDetails INNER JOIN " +
            " C_MasterForecastLine Line ON Line.C_MasterForecastLine_ID=LineDetails.C_MasterForecastLine_ID INNER JOIN C_MasterForecast Forecast ON " +
            " Forecast.C_MasterForecast_ID=Line.C_MasterForecast_ID WHERE  Forecast.AD_Org_ID =" + Org_ID + " AND Forecast.DocStatus NOT IN ('VO','RE') AND C_ProjectLine_ID IS NOT NULL)");
          


            if (!string.IsNullOrEmpty(TeamForecast_IDs))
            {
                sql.Append(" AND TForecast.C_Forecast_ID IN(" + TeamForecast_IDs + ")");
            }
            sql.Append(" ORDER BY Currency.ISO_CODE ");
            string sql1 = MRole.GetDefault(ctx).AddAccessSQL(sql.ToString(), "C_Forecast", true, true);
            ds = DB.ExecuteDataset(sql1, null, trx);
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //Price conversion from Orders currency to Forecast Currency
                    ConvertedPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Price"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Currency_ID"]),
                    ToCurrency, DateTrx, ConversionType, ctx.GetAD_Client_ID(), Org_ID);

                    if (ConvertedPrice == 0)
                    {
                        if (!FromCurrencyName.Contains(Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"])))
                        {
                            FromCurrencyName += Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"])+",";
                        }
                        continue;
                    }
                    //Create MasterForecastLines
                    CreateMasterForecastLines(ctx, trx, Org_ID, Forecast_ID, 0, 0, 0, 0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Forecast_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ForecastLine_ID"]),
                    0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                    Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                    Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["QtyEntered"]), ConvertedPrice, "");

                }
            }

        }

        /// <summary>
        /// Create Team forecast Lines 
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="trx">Transaction</param>
        /// <param name="Forecast_ID">Team Forecast</param>
        /// <param name="Order_ID">Sales Order</param>
        /// <param name="OrderLine_ID">Order Line</param>
        /// <param name="Project_ID">Opportunity</param>
        /// <param name="ProjectLine_ID">Opportunity Line</param>
        /// <param name="Charge_ID">Charge</param>
        /// <param name="Org_ID">Organization</param>
        /// <param name="Product_ID">Product</param>
        /// <param name="Attribute_ID">Attribute Set Insatnce</param>
        /// <param name="UOM_ID">UOM</param>
        /// <param name="BOM">Bill of Material</param>
        /// <param name="BaseQuantity">Base Quantity</param>
        /// <param name="Forecastqty">Forecast Qunatity</param>
        /// <param name="UnitPrice">Price</param>
        /// <Writer>209</Writer>
        public void CreateTeamForecastLines(Ctx ctx, Trx trx, int Forecast_ID, int Order_ID, int OrderLine_ID, int Project_ID, int ProjectLine_ID, int Charge_ID, int Org_ID, int Product_ID, int Attribute_ID, int UOM_ID, String BOM, Decimal? BaseQuantity, Decimal? Forecastqty, decimal UnitPrice)
        {
            MForecastLine Line =  MForecastLine.GetOrCreate(ctx,trx,Forecast_ID,Product_ID);
            Line.SetAD_Client_ID(ctx.GetAD_Client_ID());
            Line.SetAD_Org_ID(Org_ID);           
            Line.SetC_Order_ID(Order_ID);
            Line.SetC_OrderLine_ID(OrderLine_ID);
            Line.SetC_Project_ID(Project_ID);
            Line.SetC_ProjectLine_ID(ProjectLine_ID);
            Line.SetC_Charge_ID(Charge_ID);
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
                    if (OrderLine_ID > 0)
                    {
                        log.Log(Level.SEVERE, Msg.GetMsg(ctx, "TeamForecastlineNotSave") + val + Msg.GetMsg(ctx, "OrderLine") + ": " + OrderLine_ID);
                    }
                    else if (ProjectLine_ID > 0)
                    {
                        log.Log(Level.SEVERE, Msg.GetMsg(ctx, "TeamForecastlineNotSave") + val + Msg.GetMsg(ctx, "OpportunityLine") + ": " + ProjectLine_ID);
                    }
                    else if (OrderLine_ID == 0 && ProjectLine_ID == 0 && Product_ID > 0)
                    {
                        log.Log(Level.SEVERE, Msg.GetMsg(ctx, "TeamForecastlineNotSave") + val + Msg.GetMsg(ctx, "Product") + ": " + Product_ID);

                    }
                }

            }
            else
            {
                Count++;
            }
        }

        /// <summary>
        /// Create Master Forecast Lines
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="trx">Transaction</param>
        /// <param name="Org_ID">Organization</param>
        /// <param name="MasterForecast_ID">Master Forecast</param>
        /// <param name="Order_ID">Sales Order</param>
        /// <param name="OrderLine_ID">Order Line</param>
        /// <param name="Project_ID">Opportunity</param>
        /// <param name="ProjectLine_ID">Opportunity Line</param>
        /// <param name="TeamForecast_ID">TeamForecast</param>
        /// <param name="ForecastLine_ID">TeamForecastLine</param>
        /// <param name="Charge_ID">Charge</param>
        /// <param name="Product_ID">Product</param>
        /// <param name="Attribute_ID">Attribute Set Instance</param>
        /// <param name="UOM_ID">UOM</param>
        /// <param name="BOM">Bill of Material</param>
        /// <param name="BaseQuantity">Quantity</param>
        /// <param name="UnitPrice">Price</param>
        /// <Writer>209</Writer>
        public void CreateMasterForecastLines(Ctx ctx, Trx trx, int Org_ID, int MasterForecast_ID, int Order_ID, int OrderLine_ID, int Project_ID, int ProjectLine_ID, int TeamForecast_ID, int ForecastLine_ID, int Charge_ID, int Product_ID, int Attribute_ID, int UOM_ID, String BOM, decimal? BaseQuantity, decimal UnitPrice, string ProductCategories)
        {
            MMasterForecastLine Line = MMasterForecastLine.GetOrCreate(ctx, trx, MasterForecast_ID, Product_ID, Attribute_ID,ProductCategories);
            Line.SetAD_Org_ID(Org_ID);
            Line.SetC_MasterForecast_ID(MasterForecast_ID);
            Line.SetC_Charge_ID(Charge_ID);
            Line.SetM_Product_ID(Product_ID);
            Line.SetM_AttributeSetInstance_ID(Attribute_ID);
            Line.SetIsBOM(BOM.Equals("Y") ? true : false);
            if (OrderLine_ID > 0)
            {
                Line.SetSalesOrderQty(BaseQuantity + Line.GetSalesOrderQty());
            }
            else if (ForecastLine_ID > 0)
            {
                Line.SetForcastQty(BaseQuantity + Line.GetForcastQty());
            }
            else if (ProjectLine_ID > 0)
            {
                Line.SetOppQty(BaseQuantity + Line.GetOppQty());
            }
            if (!String.IsNullOrEmpty(ProductCategories))
            {
                Line.SetTotalQty(BaseQuantity);
                Line.SetPrice(UnitPrice);
                Line.SetPlannedRevenue(UnitPrice * BaseQuantity);
            }
            else
            {
                Line.SetTotalQty(Line.GetSalesOrderQty() + Line.GetForcastQty() + Line.GetOppQty());
            }
            //Price = (UnitPrice + Line.GetPrice()) / Line.GetTotalQty());
            //Line.SetPrice(Price);
            //Line.SetPlannedRevenue(Line.GetPrice()*Line.GetTotalQty());
            if (!Line.Save())
            {
                pp = VLogger.RetrieveError();
                if (pp != null)
                {
                    string val = pp.GetName();
                    if (string.IsNullOrEmpty(val))
                    {
                        val = pp.GetValue();
                    }
                    log.Log(Level.SEVERE, Msg.GetMsg(ctx, "MasterForecastLineNotSaved") + val);
                }
            }
            else
            {
                Count++;
                LineNo += 10;
                if (String.IsNullOrEmpty(ProductCategories))
                {
                    MFLineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(LineNo), 0)+10  FROM C_MasterForecastLineDetails WHERE " +
                    "C_MasterForecastLine_ID=" + Line.GetC_MasterForecastLine_ID(), null, Line.Get_Trx()));

                    //Create MasterForecast LineDetails
                    MMasterForecastLineDetails Linedetails = CreateMasterForecastLinedetails(Line, Order_ID, OrderLine_ID, Project_ID, ProjectLine_ID, TeamForecast_ID, ForecastLine_ID,
                                                             Charge_ID, Product_ID, Attribute_ID, UOM_ID, BOM, BaseQuantity, UnitPrice);
                    if (!Linedetails.Save())
                    {

                        pp = VLogger.RetrieveError();
                        if (pp != null)
                        {
                            string val = pp.GetName();
                            if (string.IsNullOrEmpty(val))
                            {
                                val = pp.GetValue();
                            }
                            if (OrderLine_ID > 0)
                            {
                                log.Log(Level.SEVERE, Msg.GetMsg(ctx, "MasterForecastlineDetailsNotSave") + val + Msg.GetMsg(ctx, "OrderLine") + ": " + OrderLine_ID);
                            }
                            else if (ProjectLine_ID > 0)
                            {
                                log.Log(Level.SEVERE, Msg.GetMsg(ctx, "MasterForecastlineDetailsNotSave") + val + Msg.GetMsg(ctx, "OpportunityLine") + ": " + ProjectLine_ID);
                            }
                            else if (ForecastLine_ID > 0)
                            {
                                log.Log(Level.SEVERE, Msg.GetMsg(ctx, "MasterForecastlineDetailsNotSave") + val + Msg.GetMsg(ctx, "TeamForecastLine") + ": " + ForecastLine_ID);
                            }
                            else if (OrderLine_ID == 0 && ProjectLine_ID == 0 && ForecastLine_ID == 0 && Product_ID > 0)
                            {
                                log.Log(Level.SEVERE, Msg.GetMsg(ctx, "MasterForecastlineDetailsNotSave") + val + Msg.GetMsg(ctx, "Product") + ": " + Product_ID);

                            }
                        }
                    }
                    else
                    {
                        MFLineNo += 10;
                        //update Amounts at master forecast line  
                        string _sql = "UPDATE c_masterforecastline SET " +
                        "Price= (Round((SELECT NVL(SUM(TotaAmt),0)/ NVL(SUM(QtyEntered),0) FROM C_MasterForecastLineDetails WHERE C_MasterForecastLine_ID=" + Line.GetC_MasterForecastLine_ID() + "), " +
                         Precision + ")), " +
                        "PlannedRevenue =(ROUND((SELECT SUM(TotaAmt) FROM C_MasterForecastLineDetails WHERE C_MasterForecastLine_ID=" + Line.GetC_MasterForecastLine_ID() + ")," + Precision + "))" +
                        " WHERE C_MasterForecastLine_ID=" + Line.GetC_MasterForecastLine_ID();

                        DB.ExecuteQuery(_sql, null, trx);
                    }
                }
              
            }
        }

        /// <summary>
        /// Create Master ForecastLine Details
        /// </summary>
        /// <param name="Line">Master ForecastLine</param>
        /// <param name="Order_ID">Sales Order</param>
        /// <param name="OrderLine_ID">Order line</param>
        /// <param name="Project_ID">Opportunity</param>
        /// <param name="ProjectLine_ID">Opportunity Line</param>
        /// <param name="TeamForecast_ID">Team Forecast</param>
        /// <param name="ForecastLine_ID">Forecast Line</param>
        /// <param name="Charge_ID">Charge</param>
        /// <param name="Product_ID">Product</param>
        /// <param name="Attribute_ID">Attribute</param>
        /// <param name="UOM_ID">UOM</param>
        /// <param name="BOM">ISBOM</param>
        /// <param name="BaseQuantity">Quantity</param>
        /// <param name="UnitPrice">Unit Price</param>
        /// <Writer>209</Writer>
        /// <returns>Line Details Object</returns>
        public MMasterForecastLineDetails CreateMasterForecastLinedetails(MMasterForecastLine Line, int Order_ID, int OrderLine_ID, int Project_ID, int ProjectLine_ID, int TeamForecast_ID, int ForecastLine_ID, int Charge_ID, int Product_ID, int Attribute_ID, int UOM_ID, String BOM, Decimal? BaseQuantity, decimal UnitPrice)
        {
            MMasterForecastLineDetails lineDetails = new MMasterForecastLineDetails(Line.GetCtx(), 0, Line.Get_Trx());
            lineDetails.SetAD_Client_ID(Line.GetAD_Client_ID());
            lineDetails.SetAD_Org_ID(Line.GetAD_Org_ID());
            lineDetails.SetC_MasterForecastLine_ID(Line.GetC_MasterForecastLine_ID());
            lineDetails.SetM_Product_ID(Product_ID);
            lineDetails.SetLineNo(MFLineNo);
            lineDetails.SetC_Order_ID(Order_ID);
            lineDetails.SetC_OrderLine_ID(OrderLine_ID);
            lineDetails.SetC_Project_ID(Project_ID);
            lineDetails.SetC_ProjectLine_ID(ProjectLine_ID);
            lineDetails.SetC_Forecast_ID(TeamForecast_ID);
            lineDetails.SetC_ForecastLine_ID(ForecastLine_ID);
            lineDetails.SetC_Charge_ID(Charge_ID);
            lineDetails.SetM_AttributeSetInstance_ID(Attribute_ID);
            lineDetails.SetC_UOM_ID(Line.GetC_UOM_ID());
            lineDetails.SetQtyEntered(BaseQuantity);
            lineDetails.SetPriceEntered(UnitPrice);
            lineDetails.SetTotaAmt(UnitPrice * BaseQuantity);

            return lineDetails;

        }
    }
}
