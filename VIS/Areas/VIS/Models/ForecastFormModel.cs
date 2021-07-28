
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

        private MTable ProductLineTbl = null;
        private MTable ForecastLineTbl = null;
        private MTable LineComponentTbl = null;
        private PO ProductLinePo = null;
        private PO ForecastLinePO = null;
        private PO LineComponentPO = null;

        private string msg = null;
        private string ToCurrencyName = "";
        private string FromCurrencyName = "";
        private string BudgetFromCurrencyName = "";
        private int FromCurrency = 0;
        private int ToCurrency = 0;
        private int LineNo = 0;
        private int MFLineNo = 0;
        private int FLineNo = 0;
        private int Count = 0;
        private int PriceList = 0;
        private Decimal ConvertedPrice = 0;
        private Decimal PurchaseUnitPrice = 0;
        private int ConversionType = 0;
        private DataSet ds = null;
        private DateTime? DateAcct = null;
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
                    @" WHERE columnname = 'C_Forecast_ID' AND AD_Table_ID = (SELECT AD_Table_ID FROM Ad_Table WHERE TableName = '" + MForecastLine.Table_Name + "' )"));
            }
            else if (Util.GetValueOfString(info["Table_Name"]).Equals("VA073_SalesForecast"))
            {
                info["BudgetColumn_ID"] = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Column_ID FROM AD_Column " +
                    @" WHERE columnname = 'C_MasterForecast_ID' AND AD_Table_ID = (SELECT AD_Table_ID FROM Ad_Table WHERE TableName = 'C_MasterForecastLine' )"));
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
        /// <param name="SalesPriceList_ID">PriceList</param>
        /// <Writer>209</Writer>
        /// <returns>info</returns>
        public string CreateForecastLine(Ctx ctx, int Org_ID, int Period_ID, bool IncludeSO, int DocType, bool IncludeOpenSO, string OpenOrders, bool IncludeOpportunity,
            string Opportunities, string ProductCategory, Decimal? BudgetQunatity, bool DeleteAndGenerateLines, int Forecast_ID, string TeamForecast_IDs, int Table_ID,
            bool IsMasterForecast, bool IsBudgetForecast, string MasterForecast_IDs, int SalesPriceList_ID)
        {
            Trx trx = null;
            string TableName = "";
            try
            {
                trx = Trx.GetTrx("Forecast" + DateTime.Now.Ticks);
                TableName = MTable.GetTableName(ctx, Util.GetValueOfInt(Table_ID));


                //Get Currency and conversion Type from header              
                ds = DB.ExecuteDataset(@"SELECT C_Currency.C_Currency_ID AS ToCurrency ,M_PriceList.C_Currency_ID AS FromCurrency,Forecast.DateAcct,C_Currency.ISO_CODE,
                    C_Currency.StdPrecision,C_ConversionType_ID,Forecast.M_PriceList_ID FROM " + TableName + " Forecast " +
                    " INNER JOIN M_PriceList ON M_PriceList.M_PriceList_ID = Forecast.M_PriceList_ID INNER JOIN C_Currency ON " +
                    "C_Currency.C_Currency_ID=Forecast.C_Currency_ID OR C_Currency.C_Currency_ID=M_PriceList.C_Currency_ID " +
                    " WHERE " + TableName + "_ID =" + Forecast_ID);

                if (IsBudgetForecast)
                {
                    ProductLineTbl = MTable.Get(ctx, "VA073_ProductLine");
                    ForecastLineTbl = MTable.Get(ctx, "VA073_ForecastLine");
                    LineComponentTbl = MTable.Get(ctx, "VA073_LineComponents");
                }
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    if (IsBudgetForecast)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (Util.GetValueOfInt(ds.Tables[0].Rows[i]["ToCurrency"]) == Util.GetValueOfInt(ds.Tables[0].Rows[i]["FromCurrency"]))
                            {
                                FromCurrency = Util.GetValueOfInt(ds.Tables[0].Rows[i]["FromCurrency"]);
                                BudgetFromCurrencyName = Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"]);
                            }
                            else
                            {
                                ToCurrency = Util.GetValueOfInt(ds.Tables[0].Rows[i]["ToCurrency"]);
                                ToCurrencyName = Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"]);
                            }
                        }
                        if (ToCurrency == 0)
                        {
                            ToCurrency = FromCurrency;
                            ToCurrencyName = BudgetFromCurrencyName;
                        }
                    }
                    else
                    {
                        ToCurrency = Util.GetValueOfInt(ds.Tables[0].Rows[0]["ToCurrency"]);
                        ToCurrencyName = Util.GetValueOfString(ds.Tables[0].Rows[0]["ISO_CODE"]);
                    }
                    DateAcct = Util.GetValueOfDateTime(ds.Tables[0].Rows[0]["DateAcct"]);
                    ConversionType = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_ConversionType_ID"]);
                    Precision = Util.GetValueOfInt(ds.Tables[0].Rows[0]["StdPrecision"]);
                    PriceList = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_PriceList_ID"]);
                }

                if (DeleteAndGenerateLines && !IsBudgetForecast)
                {
                    int count = 0;
                    if (!IsMasterForecast)
                    {
                        count = DB.ExecuteQuery("DELETE FROM C_ForecastLineHistory WHERE C_ForecastLine_ID IN (SELECT C_ForecastLine_ID FROM C_ForecastLine" +
                           " WHERE C_Forecast_ID=" + Forecast_ID + ")", null, trx);
                    }
                    else
                    {
                        count = DB.ExecuteQuery("DELETE FROM C_MasterForecastLineDetails WHERE C_MasterForecastLine_ID IN (SELECT C_MasterForecastLine_ID FROM C_MasterForecastLine" +
                           " WHERE C_masterForecast_ID=" + Forecast_ID + ")", null, trx);
                    }
                    if (count >= 0)
                    {
                        count = DB.ExecuteQuery("DELETE FROM " + TableName + "Line WHERE " + TableName + "_ID =" + Forecast_ID, null, trx);

                        if (count > 0)
                        {
                            log.Log(Level.INFO, "ForecastLinesDeleted" + count);
                        }
                    }
                    if(((IsMasterForecast && String.IsNullOrEmpty(TeamForecast_IDs)) ||!IsMasterForecast) && !IncludeSO && !IncludeOpenSO && !IncludeOpportunity && string.IsNullOrEmpty(ProductCategory))
                    {
                        trx.Commit();
                        trx.Close();
                        return Msg.GetMsg(ctx, "ForecastLinesDeleted");
                    }
                }
                else if (DeleteAndGenerateLines && IsBudgetForecast)
                {
                    int count = DB.ExecuteQuery("DELETE FROM VA073_LineComponents WHERE VA073_ForecastLine_ID IN (SELECT VA073_ForecastLine_ID FROM VA073_ForecastLine" +
                        " WHERE VA073_ProductLine_ID IN (SELECT VA073_ProductLine_ID FROM VA073_ProductLine WHERE VA073_SalesForecast_ID = " + Forecast_ID + "))", null, trx);
                    if (count >= 0)
                    {
                        count = DB.ExecuteQuery("DELETE FROM VA073_ForecastLine WHERE VA073_ProductLine_ID IN (SELECT VA073_ProductLine_ID FROM VA073_ProductLine" +
                        " WHERE VA073_SalesForecast_ID=" + Forecast_ID + ")", null, trx);
                    }
                    if (count > 0)
                    {
                        count = DB.ExecuteQuery("DELETE FROM VA073_ProductLine WHERE VA073_SalesForecast_ID=" + Forecast_ID, null, trx);
                        if (count > 0)
                        {
                            log.Log(Level.INFO, "ForecastLinesDeleted" + count);
                        }
                    }
                    if (String.IsNullOrEmpty(MasterForecast_IDs) && !IncludeSO && !IncludeOpenSO && !IncludeOpportunity && string.IsNullOrEmpty(ProductCategory))
                    {
                        trx.Commit();
                        trx.Close();
                        return Msg.GetMsg(ctx, "ForecastLinesDeleted");
                    }
                }
                ////fetch lineNo for Team Forecast line 
                //LineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT  NVL(MAX(Line), 0)+10 FROM " + TableName + "Line WHERE " + TableName + "_ID =" + Forecast_ID));
                if (IncludeSO)
                {
                    sql.Append(@"SELECT OrderLine.M_Product_ID,OrderLine.QtyOrdered AS BaseQty,OrderLine.M_AttributeSetInstance_ID,Product.C_UOM_ID, 
                       OrderLine.C_OrderLine_ID,Orders.C_Order_ID,NVL(PriceEntered,0) AS Price, NVL(QtyEntered,0) AS ForecastQty,Orders.C_Currency_ID,
                       Product.IsBOM,Currency.ISO_CODE,Orders.C_BPartner_ID,Orders.C_BPartner_Location_ID,Product.C_UOM_ID AS BaseUOM,Orders.DateOrdered,
                       (SELECT ProductPrice.PriceStd 
                       FROM M_ProductPrice ProductPrice WHERE  ProductPrice.M_Product_ID=Product.M_Product_ID AND ProductPrice.M_PriceList_Version_ID = 
                       (SELECT MAX(M_PriceList_Version_ID) FROM M_PriceList_Version WHERE IsActive = 'Y' AND M_PriceList_ID=" + PriceList + " ) " +
                       "AND ProductPrice.C_UOM_ID=Product.C_UOM_ID AND NVL(ProductPrice.M_AttributeSetInstance_ID,0)=NVL(Product.M_AttributeSetInstance_ID,0)) AS PurchasePrice " +
                       "FROM C_Order Orders " +
                       "INNER JOIN C_OrderLine OrderLine  ON Orders.C_Order_ID =  OrderLine.C_Order_ID " +
                       "INNER JOIN C_Doctype d ON Orders.C_DocTypeTarget_ID = d.C_Doctype_ID " +
                       "INNER JOIN C_Currency Currency ON Currency.C_Currency_ID=Orders.C_Currency_ID " +
                       "LEFT JOIN M_Product Product ON Product.M_Product_ID=OrderLine.M_Product_ID " +
                       "WHERE d.DocSubTypeSo NOT IN ('" + MDocType.DOCSUBTYPESO_BlanketOrder + "','" + MDocType.DOCSUBTYPESO_Proposal + "')" +
                      " AND Orders.IsSOTrx='Y' AND Orders.IsReturnTrx='N' AND Orders.AD_Org_ID =" + Org_ID + " AND NVL(OrderLine.M_Product_ID,0)>0 " +
                      " AND Orders.DocStatus IN('CO','CL') AND OrderLine.QtyOrdered = OrderLine.QtyDelivered AND NVL(OrderLine.C_OrderLine_ID,0) NOT IN");
                    if (!IsMasterForecast && !IsBudgetForecast)
                    {
                        //Team Forecast --case
                        sql.Append("(SELECT NVL(C_OrderLine_ID, 0) FROM C_ForecastLine " +
                       "INNER JOIN C_Forecast ON C_Forecast.C_Forecast_ID = C_ForecastLine.C_Forecast_ID WHERE C_Forecast.AD_Org_ID =" + Org_ID +
                       " AND C_Forecast.DocStatus NOT IN ('VO','RE')  )");
                    }
                    else if (IsMasterForecast)
                    {
                        // Master Forecast --case
                        sql.Append("(SELECT NVL(C_OrderLine_ID, 0) FROM C_MasterForecastLineDetails LineDetails " +
                            "INNER JOIN C_MasterForecastLine Line ON Line.C_MasterForecastLine_ID=LineDetails.C_MasterForecastLine_ID " +
                            "INNER JOIN C_MasterForecast Forecast ON Forecast.C_MasterForecast_ID=Line.C_MasterForecast_ID WHERE Forecast.AD_Org_ID =" + Org_ID +
                            " AND Forecast.DocStatus NOT IN ('VO','RE')   ) AND NVL(OrderLine.C_OrderLine_ID,0) NOT IN (SELECT NVL(line.C_OrderLine_ID, 0) " +
                            " FROM C_ForecastLine Line INNER JOIN C_Forecast Forecast ON Forecast.C_Forecast_ID = Line.C_Forecast_ID INNER JOIN " +
                            " C_MasterForecastLineDetails Details ON  line.C_ForecastLine_ID = Details.C_ForecastLine_ID INNER JOIN c_masterforecastline  mLine " +
                            " ON mline.C_MasterForecastLine_ID = Details.C_MasterForecastLine_ID INNER JOIN C_MasterForecast master ON master.C_MasterForecast_ID = " +
                            " mline.C_MasterForecast_ID WHERE Forecast.ad_org_id= " + Org_ID + " AND Forecast.docstatus NOT IN ( 'VO', 'RE' ) AND master.ad_org_id = " + Org_ID +
                            " AND master.docstatus NOT IN ( 'VO', 'RE' ))");

                    }
                    else if (IsBudgetForecast)
                    {
                        //Budget Forecast -- case
                        sql.Append("(SELECT NVL(C_OrderLine_ID, 0) FROM VA073_ForecastLine FLine INNER JOIN VA073_ProductLine PLine ON FLine.VA073_ProductLine_ID=" +
                            "PLine.VA073_ProductLine_ID INNER JOIN VA073_SalesForecast Forecast ON Forecast.VA073_SalesForecast_ID=PLine.VA073_SalesForecast_ID " +
                            "WHERE Forecast.AD_Org_ID =" + Org_ID + " AND Forecast.DocStatus NOT IN ('VO','RE')) AND NVL(OrderLine.C_OrderLine_ID,0) NOT IN " +
                            "(SELECT NVL(LineDetails.C_OrderLine_ID, 0) FROM C_MasterForecastLineDetails LineDetails INNER JOIN C_MasterForecastLine Line ON " +
                            "LineDetails.C_MasterForecastLine_ID = Line.C_MasterForecastLine_ID INNER JOIN C_MasterForecast Master ON  Master.C_MasterForecast_ID = Line.C_MasterForecast_ID " +
                            "INNER JOIN VA073_ForecastLine FLine ON FLine.C_MasterForecastLine_ID = Line.C_MasterForecastLine_ID INNER JOIN VA073_ProductLine PLine ON " +
                            "PLine.VA073_ProductLine_ID=FLine.VA073_ProductLine_ID INNER JOIN VA073_SalesForecast Forecast ON Forecast.VA073_SalesForecast_ID = PLine.VA073_SalesForecast_ID " +
                            "WHERE Forecast.ad_org_id =" + Org_ID + " AND Forecast.docstatus NOT IN ( 'VO', 'RE' ) AND Master.ad_org_id = " + Org_ID + " " +
                            "AND Master.docstatus NOT IN ( 'VO', 'RE' )) ");
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
                            if (Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Price"]) == 0)
                            {
                                log.Log(Level.WARNING, Msg.GetMsg(ctx, "PriceNotFound"));
                                continue;
                            }
                            //Price conversion from Orders currency to Forecast Currency
                            ConvertedPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Price"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Currency_ID"]),
                            ToCurrency, DateAcct, ConversionType, ctx.GetAD_Client_ID(), Org_ID);

                            if (ConvertedPrice == 0)
                            {
                                if (!FromCurrencyName.Contains(Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"])) &&
                                !Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"]).Equals(ToCurrencyName))
                                {
                                    FromCurrencyName += Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"]) + ",";
                                }
                                continue;
                            }


                            if (!IsMasterForecast && !IsBudgetForecast)
                            {
                                //create forecast lines 
                                CreateTeamForecastLines(ctx, trx, Forecast_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Order_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_OrderLine_ID"]), 0, 0,
                                0, Org_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                                Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                                Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["BaseQty"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ForecastQty"]), ConvertedPrice,"");

                            }
                            else if (IsMasterForecast)
                            {
                                //create master forecast lines
                                CreateMasterForecastLines(ctx, trx, Org_ID, Forecast_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Order_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_OrderLine_ID"]), 0, 0, 0, 0,
                                0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                                Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["BaseUOM"]),
                                Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["BaseQty"]), ConvertedPrice, "", "", 0, 0);
                            }

                            else if (IsBudgetForecast)
                            {
                                if (Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["PurchasePrice"]) == 0)
                                {
                                    log.Log(Level.WARNING, Msg.GetMsg(ctx, "PurchasePriceNotFound"));
                                    PurchaseUnitPrice = 0;
                                }
                                else
                                {
                                    //Price conversion from pricelist currency to header currency
                                    PurchaseUnitPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["PurchasePrice"]), FromCurrency,
                                    ToCurrency, DateAcct, ConversionType, ctx.GetAD_Client_ID(), Org_ID);
                                    if (PurchaseUnitPrice == 0)
                                    {
                                        log.Log(Level.WARNING, Msg.GetMsg(ctx, "ConversionNotFound") + " " + Msg.GetMsg(ctx, "From") + " " + BudgetFromCurrencyName +
                                            Msg.GetMsg(ctx, "To") + " " + ToCurrencyName);
                                    }
                                }
                                //create Budgetforecast Line
                                CreateBudgetForecastLines(ctx, trx, Org_ID, Forecast_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Order_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_OrderLine_ID"]), 0, 0, 0, 0,
                                  0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]),
                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["BaseUOM"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_BPartner_ID"]),
                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_BPartner_Location_ID"]), 0, 0, Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]),
                                  Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["BaseQty"]), ConvertedPrice, PurchaseUnitPrice,
                                  "", Period_ID, Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["DateOrdered"]), "", 0, 0, 0, "");
                            }
                        }
                    }
                    else
                    {
                        log.Log(Level.INFO, sql1);
                        log.Log(Level.INFO, "Data not found against Sales Order");
                    }
                }

                if (IncludeOpenSO)
                {
                    sql.Clear();
                    //fetch only open sales order 
                    sql.Append(@"SELECT OrderLine.M_Product_ID,OrderLine.QtyOrdered AS BaseQty,OrderLine.M_AttributeSetInstance_ID,Product.C_UOM_ID, 
                       OrderLine.C_OrderLine_ID,Orders.C_Order_ID,NVL(PriceEntered,0) AS Price, NVL(QtyEntered,0) AS ForecastQty,Orders.C_Currency_ID,
                       Product.IsBOM,Currency.ISO_CODE,Orders.C_BPartner_ID,Orders.C_BPartner_Location_ID,Product.C_UOM_ID AS BaseUOM,Orders.DateOrdered,
                       (SELECT ProductPrice.PriceStd 
                       FROM M_ProductPrice ProductPrice WHERE  ProductPrice.M_Product_ID=Product.M_Product_ID AND ProductPrice.M_PriceList_Version_ID = 
                       (SELECT MAX(M_PriceList_Version_ID) FROM M_PriceList_Version WHERE IsActive = 'Y' AND M_PriceList_ID=" + PriceList + " ) " +
                       "AND ProductPrice.C_UOM_ID=Product.C_UOM_ID AND NVL(ProductPrice.M_AttributeSetInstance_ID,0)=NVL(Product.M_AttributeSetInstance_ID,0)) AS PurchasePrice " +
                       "FROM C_Order Orders " +
                       "INNER JOIN C_OrderLine OrderLine  ON Orders.C_Order_ID =  OrderLine.C_Order_ID " +
                       "INNER JOIN C_Doctype d ON Orders.C_DocTypeTarget_ID = d.C_Doctype_ID " +
                       "INNER JOIN C_Currency Currency ON Currency.C_Currency_ID=Orders.C_Currency_ID " +
                       "LEFT JOIN M_Product Product ON Product.M_Product_ID=OrderLine.M_Product_ID " +
                       "WHERE d.DocSubTypeSo NOT IN ('" + MDocType.DOCSUBTYPESO_BlanketOrder + "','" + MDocType.DOCSUBTYPESO_Proposal + "')" +
                      " AND Orders.IsSOTrx='Y' AND Orders.IsReturnTrx='N' AND Orders.AD_Org_ID =" + Org_ID + " AND NVL(OrderLine.M_Product_ID,0)>0 " +
                      " AND Orders.DocStatus IN('CO','CL') AND OrderLine.QtyOrdered > OrderLine.QtyDelivered AND NVL(OrderLine.C_OrderLine_ID,0) NOT IN");
                    if (!IsMasterForecast && !IsBudgetForecast)
                    {
                        //Team Forecast -- case OrderLine reference must not present in Teamforecast line
                        sql.Append("(SELECT NVL(C_OrderLine_ID, 0) FROM C_ForecastLine " +
                       "INNER JOIN C_Forecast ON C_Forecast.C_Forecast_ID = C_ForecastLine.C_Forecast_ID WHERE C_Forecast.AD_Org_ID =" + Org_ID +
                       " AND C_Forecast.DocStatus NOT IN ('VO','RE')  )");
                    }
                    else if (IsMasterForecast)
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
                    else if (IsBudgetForecast)
                    {
                        //Budget Forecast -- case
                        sql.Append("(SELECT NVL(C_OrderLine_ID, 0) FROM VA073_ForecastLine FLine INNER JOIN VA073_ProductLine PLine ON FLine.VA073_ProductLine_ID=" +
                            "PLine.VA073_ProductLine_ID INNER JOIN VA073_SalesForecast Forecast ON Forecast.VA073_SalesForecast_ID=PLine.VA073_SalesForecast_ID " +
                            "WHERE Forecast.AD_Org_ID =" + Org_ID + " AND Forecast.DocStatus NOT IN ('VO','RE')) AND NVL(OrderLine.C_OrderLine_ID,0) NOT IN " +
                            "(SELECT NVL(LineDetails.C_OrderLine_ID, 0) FROM C_MasterForecastLineDetails LineDetails INNER JOIN C_MasterForecastLine Line ON " +
                            "LineDetails.C_MasterForecastLine_ID = Line.C_MasterForecastLine_ID INNER JOIN C_MasterForecast Master ON  Master.C_MasterForecast_ID = Line.C_MasterForecast_ID " +
                            "INNER JOIN VA073_ForecastLine FLine ON FLine.C_MasterForecastLine_ID = Line.C_MasterForecastLine_ID INNER JOIN VA073_ProductLine PLine ON " +
                            "PLine.VA073_ProductLine_ID=FLine.VA073_ProductLine_ID INNER JOIN VA073_SalesForecast Forecast ON Forecast.VA073_SalesForecast_ID = PLine.VA073_SalesForecast_ID " +
                            "WHERE Forecast.ad_org_id =" + Org_ID + " AND Forecast.docstatus NOT IN ( 'VO', 'RE' ) AND Master.ad_org_id = " + Org_ID + " " +
                            "AND Master.docstatus NOT IN ( 'VO', 'RE' )) ");
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
                            if (Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Price"]) == 0)
                            {
                                log.Log(Level.WARNING, Msg.GetMsg(ctx, "PriceNotFound"));
                                continue;
                            }
                            //Price conversion from Orderss currency to Forecast Currency
                            ConvertedPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Price"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Currency_ID"]),
                            ToCurrency, DateAcct, ConversionType, ctx.GetAD_Client_ID(), Org_ID);

                            if (ConvertedPrice == 0)
                            {
                                if (!FromCurrencyName.Contains(Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"])) &&
                                !Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"]).Equals(ToCurrencyName))
                                {
                                    FromCurrencyName += Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"]) + ",";
                                }
                                continue;
                            }

                            if (!IsMasterForecast && !IsBudgetForecast)
                            {
                                //create forecast lines 
                                CreateTeamForecastLines(ctx, trx, Forecast_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Order_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_OrderLine_ID"]), 0, 0,
                                0, Org_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                                Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                                Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["BaseQty"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ForecastQty"]), ConvertedPrice,"");

                            }
                            else if (IsMasterForecast)
                            {
                                //create master ForecastLine
                                CreateMasterForecastLines(ctx, trx, Org_ID, Forecast_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Order_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_OrderLine_ID"]), 0, 0, 0, 0,
                                0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                                Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["BaseUOM"]),
                                Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["BaseQty"]), ConvertedPrice, "", "", 0, 0);
                            }

                            else if (IsBudgetForecast)
                            {
                                if (Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["PurchasePrice"]) == 0)
                                {
                                    log.Log(Level.WARNING, Msg.GetMsg(ctx, "PurchasePriceNotFound"));
                                    PurchaseUnitPrice = 0;
                                }
                                else
                                {
                                    //Price conversion from pricelist currency to header currency
                                    PurchaseUnitPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["PurchasePrice"]), FromCurrency,
                                    ToCurrency, DateAcct, ConversionType, ctx.GetAD_Client_ID(), Org_ID);
                                    if (PurchaseUnitPrice == 0)
                                    {
                                        log.Log(Level.WARNING, Msg.GetMsg(ctx, "ConversionNotFound") + " " + Msg.GetMsg(ctx, "From") + " " + BudgetFromCurrencyName +
                                            Msg.GetMsg(ctx, "To") + " " + ToCurrencyName);
                                    }
                                }

                                //create Budgetforecast Line
                                CreateBudgetForecastLines(ctx, trx, Org_ID, Forecast_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Order_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_OrderLine_ID"]), 0, 0, 0, 0,
                                0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]),
                                Util.GetValueOfInt(ds.Tables[0].Rows[i]["BaseUOM"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_BPartner_ID"]),
                                Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_BPartner_Location_ID"]), 0, 0, Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]),
                                Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["BaseQty"]), ConvertedPrice, PurchaseUnitPrice,
                                "", Period_ID, Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["DateOrdered"]), "", 0, 0, 0, "");
                            }
                        }
                    }
                    else
                    {
                        log.Log(Level.INFO, sql1);
                        log.Log(Level.INFO, "Data not found against Open Sales Order");
                    }
                }
                //Include Opportunities for both Master and Team Forecast
                if (IncludeOpportunity)
                {
                    OpportunityProducts(ctx, trx, Org_ID, Period_ID, Forecast_ID, Opportunities, IsMasterForecast, IsBudgetForecast, TeamForecast_IDs);
                }
                if (!string.IsNullOrEmpty(ProductCategory))
                {
                    //get Pricelistfrom header 
                    ProductCategoryProducts(ctx, trx, Org_ID, ProductCategory, BudgetQunatity, SalesPriceList_ID, PriceList, Forecast_ID, IsMasterForecast, IsBudgetForecast, Period_ID);
                }
                if (IsMasterForecast && !string.IsNullOrEmpty(TeamForecast_IDs))
                {
                    //Only in case of Master Forecast
                    TeamForecastProducts(ctx, trx, Org_ID, Period_ID, Forecast_ID, TeamForecast_IDs, IsMasterForecast, IncludeOpenSO, IncludeSO, IncludeOpportunity);
                }

                if (IsBudgetForecast && !string.IsNullOrEmpty(MasterForecast_IDs))
                {
                    //Only In case of Budget Forecast
                    MasterForecastProducts(ctx, trx, Org_ID, Period_ID, Forecast_ID, MasterForecast_IDs);
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
                return e.Message;
            }

            //if (Count > 0)
            //{
               
            //}

            //if conversion not found then display this message 
            if (!string.IsNullOrEmpty(FromCurrencyName)/* && ((IsBudgetForecast && ToCurrency != FromCurrency) || !IsBudgetForecast)*/)
            {
                msg = Msg.GetMsg(ctx, "ConversionNotFound") + " " + FromCurrencyName.Trim(',') + " " + Msg.GetMsg(ctx, "To") + " " + ToCurrencyName;

            }
            if (Count == 0 && string.IsNullOrEmpty(FromCurrencyName))
            {
                trx.Rollback();
                trx.Close();
                trx = null;
                return Msg.GetMsg(ctx, "NoDataFound") + " " + msg;
            }
            else if(Count == 0 && !string.IsNullOrEmpty(FromCurrencyName))
            {
                trx.Rollback();
                trx.Close();
                trx = null;
                return  msg;
            }

            trx.Commit();
            trx.Close();
            trx = null;
            return Msg.GetMsg(ctx, "LinesInsterted") + " " + msg;
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
        public void OpportunityProducts(Ctx ctx, Trx trx, int Org_ID, int Period, int Forecast_ID, string Opportunities, bool IsMasterForecast, bool IsBudgetForecast, string TeamForecast_IDs)
        {
            sql.Clear();
            sql.Append(@"SELECT ProjectLine.M_Product_ID, Project.C_Project_ID,Project.C_Currency_ID,ProjectLine.C_ProjectLine_ID, ProjectLine.PlannedQty AS ForecastQty,
                ProjectLine.BaseQty,ProjectLine.PlannedPrice AS Price,ProjectLine.M_AttributeSetInstance_ID ,Product.IsBOM ,ProjectLine.C_UOM_ID, Currency.ISO_CODE,Project.C_BPartner_ID,
                Project.C_BPartner_Location_ID,Product.C_UOM_ID AS BaseUOM,Project.C_EnquiryrDate ,(SELECT ProductPrice.PriceStd 
                FROM M_ProductPrice ProductPrice WHERE  ProductPrice.M_Product_ID=Product.M_Product_ID AND ProductPrice.M_PriceList_Version_ID = 
                (SELECT MAX(M_PriceList_Version_ID) FROM M_PriceList_Version WHERE IsActive = 'Y' AND M_PriceList_ID=" + PriceList + " ) " +
                "AND ProductPrice.C_UOM_ID=Product.C_UOM_ID AND NVL(ProductPrice.M_AttributeSetInstance_ID,0)=NVL(Product.M_AttributeSetInstance_ID,0)) AS PurchasePrice " +
                "FROM C_Project Project INNER JOIN C_ProjectLine ProjectLine ON Project.C_Project_ID = ProjectLine.C_Project_ID " +
                "INNER JOIN M_Product Product ON Product.M_Product_ID= ProjectLine.M_Product_ID INNER JOIN C_Currency Currency ON " +
                "Currency.C_Currency_ID= Project.C_Currency_ID  WHERE Project.C_Order_ID IS NULL " +
                "AND Project.Ref_Order_ID IS NULL AND Project.AD_Org_ID = " + Org_ID + " AND NVL(C_ProjectLine_ID,0) NOT IN ");

            if (!IsMasterForecast && !IsBudgetForecast)
            {
                //Team Forecast-- case ProjectLine reference must not present in TeamforecastLine
                sql.Append("( SELECT NVL(C_ProjectLine_ID,0) " +
                "FROM C_ForecastLine INNER JOIN C_Forecast ON C_Forecast.C_Forecast_ID = C_ForecastLine.C_Forecast_ID WHERE C_Forecast.AD_Org_ID =" + Org_ID +
                " AND C_Forecast.DocStatus NOT IN ('VO','RE'))");
            }
            else if (IsMasterForecast)
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
            }

            else if (IsBudgetForecast)
            {

                //Budget Forecast -- case 
                sql.Append("(SELECT NVL(C_ProjectLine_ID, 0) FROM VA073_ForecastLine FLine INNER JOIN VA073_ProductLine PLine ON FLine.VA073_ProductLine_ID=" +
                    "PLine.VA073_ProductLine_ID INNER JOIN VA073_SalesForecast Forecast ON Forecast.VA073_SalesForecast_ID=PLine.VA073_SalesForecast_ID " +
                    "WHERE Forecast.AD_Org_ID =" + Org_ID + " AND Forecast.DocStatus NOT IN ('VO','RE')) AND NVL(C_ProjectLine_ID,0) NOT IN " +
                    "(SELECT NVL(LineDetails.C_ProjectLine_ID, 0) FROM C_MasterForecastLineDetails LineDetails INNER JOIN C_MasterForecastLine Line ON " +
                    "LineDetails.C_MasterForecastLine_ID = Line.C_MasterForecastLine_ID INNER JOIN C_MasterForecast Master ON  Master.C_MasterForecast_ID = Line.C_MasterForecast_ID " +
                    "INNER JOIN VA073_ForecastLine FLine ON FLine.C_MasterForecastLine_ID = Line.C_MasterForecastLine_ID INNER JOIN VA073_ProductLine PLine ON " +
                    "PLine.VA073_ProductLine_ID=FLine.VA073_ProductLine_ID INNER JOIN VA073_SalesForecast Forecast ON Forecast.VA073_SalesForecast_ID = PLine.VA073_SalesForecast_ID " +
                    "WHERE Forecast.ad_org_id =" + Org_ID + " AND Forecast.docstatus NOT IN ( 'VO', 'RE' ) AND Master.ad_org_id = " + Org_ID + " " +
                    "AND Master.docstatus NOT IN ( 'VO', 'RE' )) ");
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
                    if (Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Price"]) == 0)
                    {
                        log.Log(Level.WARNING, Msg.GetMsg(ctx, "PriceNotFound"));
                        continue;
                    }
                    //Price conversion from Opportunity currency to Forecast Currency
                    ConvertedPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Price"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Currency_ID"]),
                    ToCurrency, DateAcct, ConversionType, ctx.GetAD_Client_ID(), Org_ID);

                    if (ConvertedPrice == 0)
                    {
                        if (!FromCurrencyName.Contains(Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"])) &&
                        !Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"]).Equals(ToCurrencyName))
                        {
                            FromCurrencyName += Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"]) + ",";
                        }
                        continue;
                    }

                    if (!IsMasterForecast && !IsBudgetForecast)
                    {
                        //Create TeamForecast Lines
                        CreateTeamForecastLines(ctx, trx, Forecast_ID, 0, 0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Project_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ProjectLine_ID"]),
                        0, Org_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                        Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["BaseQty"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ForecastQty"]), ConvertedPrice, "");
                    }
                    else if (IsMasterForecast)
                    {
                        //Create MasterForecast Lines
                        CreateMasterForecastLines(ctx, trx, Org_ID, Forecast_ID, 0, 0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Project_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ProjectLine_ID"]), 0, 0,
                        0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                        Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["BaseUOM"]),
                        Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["BaseQty"]), ConvertedPrice, "", "", 0, 0);
                    }
                    else if (IsBudgetForecast)
                    {
                        if (Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["PurchasePrice"]) == 0)
                        {
                            log.Log(Level.WARNING, Msg.GetMsg(ctx, "PurchasePriceNotFound"));
                            PurchaseUnitPrice = 0;
                        }
                        else
                        {
                            //Price conversion from pricelist currency to header currency
                            PurchaseUnitPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["PurchasePrice"]), FromCurrency,
                            ToCurrency, DateAcct, ConversionType, ctx.GetAD_Client_ID(), Org_ID);
                            if (PurchaseUnitPrice == 0)
                            {
                                log.Log(Level.WARNING, Msg.GetMsg(ctx, "ConversionNotFound") + " " + Msg.GetMsg(ctx, "From") + " " + BudgetFromCurrencyName +
                                    Msg.GetMsg(ctx, "To") + " " + ToCurrencyName);
                            }

                        }
                        //Create BudgetForecast Lines
                        CreateBudgetForecastLines(ctx, trx, Org_ID, Forecast_ID, 0, 0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Project_ID"]),
                        Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ProjectLine_ID"]), 0, 0, 0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                        Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["BaseUOM"]), 0, 0,
                        Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_BPartner_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_BPartner_Location_ID"]),
                        Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["BaseQty"]), ConvertedPrice,
                        PurchaseUnitPrice, "", Period, Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["C_EnquiryrDate"]), "", 0, 0, 0, "");
                    }
                }
            }
            else
            {
                log.Log(Level.INFO, sql1);
                log.Log(Level.INFO, "Data not found against Opportunity");
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
        ///  <param name="SalesPriceList_ID">PriceList</param>
        /// <param name="PriceList">Price list</param>
        /// <param name="Forecast_ID">Master/Team Forecast</param>
        /// <param name="IsMasterFoecast">IsmasterForecast</param>
        /// <Writer>209</Writer>
        public void ProductCategoryProducts(Ctx ctx, Trx trx, int Org_ID, string ProductCategories, Decimal? BudgetQuantity, int SalesPriceList_ID, int PriceList, int Forecast_ID, bool IsMasterFoecast, bool IsBudgetForecast, int Period)
        {
            sql.Clear();
            sql.Append(@"SELECT Product.M_Product_ID,Product.C_UOM_ID,ProductPrice.PriceStd AS SalesPrice,Product.M_AttributeSetInstance_ID,Product.IsBOM");
            if (IsBudgetForecast)
            {
                sql.Append(@",(SELECT ProductPrice.PriceStd FROM M_ProductPrice ProductPrice WHERE  ProductPrice.M_Product_ID = Product.M_Product_ID AND 
                ProductPrice.M_PriceList_Version_ID =(SELECT MAX(M_PriceList_Version_ID) FROM M_PriceList_Version WHERE IsActive = 'Y' AND M_PriceList_ID = " + PriceList + ") " +
                " AND ProductPrice.C_UOM_ID=Product.C_UOM_ID AND NVL(ProductPrice.M_AttributeSetInstance_ID,0)=NVL(Product.M_AttributeSetInstance_ID,0)) AS PurchasePrice ");
            }

            sql.Append(@" FROM M_Product Product LEFT JOIN M_ProductPrice ProductPrice ON Product.M_Product_ID = ProductPrice.M_Product_ID AND ProductPrice.M_PriceList_Version_ID = 
            (SELECT MAX(M_PriceList_Version_ID) FROM M_PriceList_Version WHERE IsActive = 'Y' AND M_PriceList_ID =" + (IsBudgetForecast ? SalesPriceList_ID : PriceList) + " ) " +
             "AND ProductPrice.C_UOM_ID=Product.C_UOM_ID AND NVL(ProductPrice.M_AttributeSetInstance_ID,0)=NVL(Product.M_AttributeSetInstance_ID,0) " +
             "WHERE Product.IsActive = 'Y' AND Product.M_Product_Category_ID IN(" + ProductCategories + ") AND Product.AD_Org_ID IN (0," + Org_ID + ")");


            string sql1 = MRole.GetDefault(ctx).AddAccessSQL(sql.ToString(), "M_Product", true, true);
            ds = DB.ExecuteDataset(sql1, null, trx);
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!IsMasterFoecast && !IsBudgetForecast)
                    {
                        //Create TeamForecast Lines
                        CreateTeamForecastLines(ctx, trx, Forecast_ID, 0, 0, 0, 0, 0, Org_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                        Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                        Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), BudgetQuantity, BudgetQuantity, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["SalesPrice"]), ProductCategories);
                    }
                    else if (IsMasterFoecast)
                    {
                        //Create MasterForecast Lines
                        CreateMasterForecastLines(ctx, trx, Org_ID, Forecast_ID, 0, 0, 0, 0, 0, 0, 0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                        Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                        Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), BudgetQuantity, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["SalesPrice"]), ProductCategories, "", 0, 0);
                    }

                    else if (IsBudgetForecast)
                    {
                        if (Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["PurchasePrice"]) == 0)
                        {
                            log.Log(Level.WARNING, Msg.GetMsg(ctx, "PurchasePriceNotFound"));
                            PurchaseUnitPrice = 0;

                        }
                        else
                        {
                            //Price conversion from pricelist currency to header currency
                            PurchaseUnitPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["PurchasePrice"]), FromCurrency,
                            ToCurrency, DateAcct, ConversionType, ctx.GetAD_Client_ID(), Org_ID);
                            if (PurchaseUnitPrice == 0)
                            {
                                log.Log(Level.WARNING, Msg.GetMsg(ctx, "ConversionNotFound") + " " + Msg.GetMsg(ctx, "From") + " " + BudgetFromCurrencyName +
                                    Msg.GetMsg(ctx, "To") + " " + ToCurrencyName);
                            }
                        }
                        //Create BudgetForecast Lines  
                        CreateBudgetForecastLines(ctx, trx, Org_ID, Forecast_ID, 0, 0, 0, 0, 0, 0, 0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                        Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]), 0, 0, 0, 0,
                        Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), BudgetQuantity, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["SalesPrice"]),
                        PurchaseUnitPrice, ProductCategories, Period, null, "", 0, 0,0,"");
                    }
                }
            }
            else
            {
                log.Log(Level.INFO, sql1);
                log.Log(Level.INFO, "Data not found against Product Category");
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
            sql.Append(@"SELECT TLine.M_Product_ID,TLine.C_Charge_ID,TLine.M_AttributeSetInstance_ID,TLine.QtyEntered,TLine.BOMUse,TLine.M_BOM_ID," +
             (Env.IsModuleInstalled("VAMFG_") ? "TLine.VAMFG_M_Routing_ID" : "0 AS VAMFG_M_Routing_ID") +
            ",TForecast.C_Forecast_ID, C_ForecastLine_ID,TLine.C_UOM_ID,NVL(UnitPrice,0) AS Price,TForecast.C_Currency_ID,Product.ISBOM,Currency.ISO_CODE,Product.C_UOM_ID AS BaseUOM " +
            "FROM C_Forecast TForecast INNER JOIN C_Forecastline TLine ON TLine.C_Forecast_ID = TForecast.C_Forecast_ID LEFT JOIN M_Product Product ON Product.M_Product_ID=TLine.M_Product_ID " +
            "INNER JOIN C_Currency Currency ON Currency.C_Currency_ID= TForecast.C_Currency_ID " +
            "WHERE  TForecast.AD_Org_ID = " + Org_ID + "   AND TForecast.DocStatus IN ('CO','CL') AND NVL(TLine.C_ForecastLine_ID,0) NOT IN " +
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
                    if (Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Price"]) == 0)
                    {
                        log.Log(Level.WARNING, Msg.GetMsg(ctx, "PriceNotFound"));
                        continue;
                    }
                    //Price conversion from Orders currency to Forecast Currency
                    ConvertedPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Price"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Currency_ID"]),
                    ToCurrency, DateAcct, ConversionType, ctx.GetAD_Client_ID(), Org_ID);

                    if (ConvertedPrice == 0)
                    {
                        if (!FromCurrencyName.Contains(Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"])) &&
                            !Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"]).Equals(ToCurrencyName))
                        {
                            FromCurrencyName += Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"]) + ",";
                        }
                        continue;
                    }
                    //Create MasterForecastLines
                    CreateMasterForecastLines(ctx, trx, Org_ID, Forecast_ID, 0, 0, 0, 0, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Forecast_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ForecastLine_ID"]),
                     Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Charge_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]),
                    Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]),
                    (Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Charge_ID"]) > 0 ? Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]) : Util.GetValueOfInt(ds.Tables[0].Rows[i]["BaseUOM"])),
                    Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]), Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["QtyEntered"]), ConvertedPrice, "",
                    Util.GetValueOfString(ds.Tables[0].Rows[i]["BOMUse"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_BOM_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAMFG_M_Routing_ID"]));

                }
            }
            else
            {
                log.Log(Level.INFO, sql1);
                log.Log(Level.INFO, "Data not found against Team Forecast");
            }

        }

        /// <summary>
        /// Fetch data from Master forecast 
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="trx">Transaction</param>
        /// <param name="Org_ID">Organization</param>
        /// <param name="Period">Period</param>
        /// <param name="Forecast_ID">Budget Forecast</param>
        /// <param name="MasterForecast_IDs">Master Forecast</param>  
        /// <writer>209</writer>
        public void MasterForecastProducts(Ctx ctx, Trx trx, int Org_ID, int Period, int Forecast_ID, string MasterForecast_IDs)
        {
            sql.Clear();
            sql.Append("SELECT Details.M_Product_ID,Details.C_Charge_ID,Details.M_Attributesetinstance_ID, Mforcast.C_MasterForecast_ID,Details.C_MasterForecastLine_ID," +
                " Details.C_Uom_ID, Details.C_OrderLine_ID, Details.C_Order_ID, Details.C_ProjectLine_ID, Details.C_Project_ID, Details.PriceEntered AS Price,Details.IsAdjusted, " +
                "Details.QtyEntered AS Quantity, Mforcast.C_Currency_ID, Product.ISBOM, Currency.ISO_CODE, Orders.C_BPartner_ID  AS OrderBPartner," +
                "Orders.C_BPartner_Location_ID AS OrderLocation, Project.C_BPartner_Location_ID AS ProjectLocation,Line.BOMUse,Line.M_BOM_ID,Line.AdjustedQty," +
                (Env.IsModuleInstalled("VAMFG_") ? "Line.VAMFG_M_Routing_ID" : "0 AS VAMFG_M_Routing_ID") +
                ",Project.C_BPartner_ID AS ProjectBPartner,Mforcast.TrxDate, (SELECT ProductPrice.PriceStd " +
                "FROM M_ProductPrice ProductPrice WHERE  ProductPrice.M_Product_ID = Product.M_Product_ID AND ProductPrice.M_PriceList_Version_ID = " +
                "(SELECT MAX(M_PriceList_Version_ID) FROM M_PriceList_Version WHERE IsActive = 'Y' AND M_PriceList_ID = " + PriceList + ") " +
                "AND ProductPrice.C_UOM_ID=Product.C_UOM_ID AND NVL(ProductPrice.M_AttributeSetInstance_ID,0)=NVL(Product.M_AttributeSetInstance_ID,0)) AS PurchasePrice " +
                "FROM C_MasterForecast Mforcast  " +
                "INNER JOIN C_MasterForecastLine Line ON Mforcast.C_MasterForecast_ID = Line.C_MasterForecast_ID " +
                "INNER JOIN C_MasterForecastLineDetails Details ON Line.C_MasterForecastLine_ID= Details.C_MasterForecastLine_ID  " +
                "LEFT JOIN M_Product Product ON Product.M_Product_ID = Line.M_Product_ID " +
                "INNER JOIN C_Currency Currency ON Currency.C_Currency_ID = Mforcast.C_Currency_ID " +
                "LEFT JOIN C_Order Orders ON Orders.C_Order_ID=Details.C_Order_ID " +
                "LEFT JOIN C_Project Project ON Project.C_Project_ID=Details.C_Project_ID " +
                "WHERE Mforcast.DocStatus IN ( 'CO', 'CL' ) AND Mforcast.AD_Org_ID= " + Org_ID +
                " AND nvl(Line.C_MasterForecastLine_ID, 0) NOT IN " +
                "( SELECT nvl(FLine.C_MasterForecastLine_ID, 0) FROM va073_forecastline FLine INNER JOIN VA073_ProductLine PLine ON FLine.VA073_ProductLine_ID = " +
                "PLine.VA073_ProductLine_ID INNER JOIN VA073_SalesForecast  Forecast ON Forecast.VA073_SalesForecast_ID = PLine.VA073_SalesForecast_ID " +
                "WHERE Forecast.DocStatus NOT IN ( 'VO', 'RE' ) AND Forecast.AD_Org_ID= " + Org_ID + " AND NVL(FLine.C_MasterForecastLine_ID,0) <> 0 ) " +
                "AND NVL(Details.C_OrderLine_ID,0) NOT IN (SELECT NVL(C_OrderLine_ID,0) FROM VA073_forecastline FLine INNER JOIN VA073_ProductLine PLine ON " +
                "FLine.VA073_ProductLine_ID = PLine.VA073_ProductLine_ID INNER JOIN VA073_SalesForecast  Forecast ON Forecast.VA073_SalesForecast_ID = " +
                "PLine.VA073_SalesForecast_ID WHERE Forecast.DocStatus NOT IN ( 'VO', 'RE' ) AND Forecast.AD_Org_ID= " + Org_ID + " AND NVL(C_OrderLine_ID,0) <>0) " +
                "AND NVL(Details.C_ProjectLine_ID,0) NOT IN (SELECT NVL(C_ProjectLine_ID,0) FROM VA073_forecastline FLine INNER JOIN VA073_ProductLine    " +
                "PLine ON FLine.VA073_ProductLine_ID = PLine.VA073_ProductLine_ID INNER JOIN VA073_SalesForecast  Forecast ON Forecast.VA073_SalesForecast_ID = " +
                "PLine.VA073_SalesForecast_ID WHERE Forecast.DocStatus NOT IN ( 'VO', 'RE' ) AND Forecast.AD_Org_ID= " + Org_ID + " AND NVL(C_ProjectLine_ID,0) <>0)");

            if (!string.IsNullOrEmpty(MasterForecast_IDs))
            {
                sql.Append(" AND Mforcast.C_MasterForecast_ID IN(" + MasterForecast_IDs + ")");
            }
            sql.Append(" ORDER BY Currency.ISO_CODE ");
            string sql1 = MRole.GetDefault(ctx).AddAccessSQL(sql.ToString(), "C_MasterForecast", true, true);
            ds = DB.ExecuteDataset(sql1, null, trx);
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Price"]) == 0)
                    {
                        log.Log(Level.WARNING, Msg.GetMsg(ctx, "PriceNotFound"));
                        continue;
                    }
                    //Price conversion from Orders currency to Forecast Currency
                    ConvertedPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Price"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Currency_ID"]),
                    ToCurrency, DateAcct, ConversionType, ctx.GetAD_Client_ID(), Org_ID);

                    if (ConvertedPrice == 0)
                    {
                        if (!FromCurrencyName.Contains(Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"])) &&
                            !Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"]).Equals(ToCurrencyName))
                        {
                            FromCurrencyName += Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"]) + ",";
                        }
                        continue;
                    }

                    if (Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["PurchasePrice"]) == 0)
                    {
                        log.Log(Level.WARNING, Msg.GetMsg(ctx, "PurchasePriceNotFound"));
                        PurchaseUnitPrice = 0;
                    }
                    else
                    {
                        //Price conversion from pricelist currency to header currency
                        PurchaseUnitPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["PurchasePrice"]), FromCurrency,
                        ToCurrency, DateAcct, ConversionType, ctx.GetAD_Client_ID(), Org_ID);
                        if (PurchaseUnitPrice == 0)
                        {
                            log.Log(Level.WARNING, Msg.GetMsg(ctx, "ConversionNotFound") + " " + Msg.GetMsg(ctx, "From") + " " + BudgetFromCurrencyName +
                                Msg.GetMsg(ctx, "To") + " " + ToCurrencyName);
                        }
                    }
                    //Create BudgetForecast Lines
                    CreateBudgetForecastLines(ctx, trx, Org_ID, Forecast_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Order_ID"]),
                    Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_OrderLine_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Project_ID"]),
                    Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ProjectLine_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_MasterForecast_ID"]),
                    Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_MasterForecastLine_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Charge_ID"]),
                    Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]),
                    Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["OrderBPartner"]),
                    Util.GetValueOfInt(ds.Tables[0].Rows[i]["OrderLocation"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["ProjectBPartner"]),
                    Util.GetValueOfInt(ds.Tables[0].Rows[i]["ProjectLocation"]), Util.GetValueOfString(ds.Tables[0].Rows[i]["IsBOM"]),
                    Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Quantity"]), ConvertedPrice, PurchaseUnitPrice,
                    "", Period, Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["TrxDate"]), Util.GetValueOfString(ds.Tables[0].Rows[i]["BOMUse"]),
                    Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_BOM_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAMFG_M_Routing_ID"]),
                    Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["AdjustedQty"]), Util.GetValueOfString(ds.Tables[0].Rows[i]["IsAdjusted"]));

                }
            }
            else
            {
                log.Log(Level.INFO, sql1);
                log.Log(Level.INFO, "Data not found against Master Forecast");
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
        public void CreateTeamForecastLines(Ctx ctx, Trx trx, int Forecast_ID, int Order_ID, int OrderLine_ID, int Project_ID, int ProjectLine_ID, int Charge_ID, int Org_ID, int Product_ID, int Attribute_ID, int UOM_ID, String BOM, Decimal? BaseQuantity, Decimal? Forecastqty, decimal UnitPrice, string ProductCategories)
        {
            MForecastLine Line = MForecastLine.GetOrCreate(ctx, trx, Forecast_ID, Product_ID,  ProductCategories);
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
        public void CreateMasterForecastLines(Ctx ctx, Trx trx, int Org_ID, int MasterForecast_ID, int Order_ID, int OrderLine_ID, int Project_ID, int ProjectLine_ID, int TeamForecast_ID, int ForecastLine_ID, int Charge_ID, int Product_ID, int Attribute_ID, int UOM_ID, String BOM, decimal? BaseQuantity, decimal UnitPrice, string ProductCategories, string BOMUse, int M_BOM_ID, int Routing_ID)
        {
            //set the context to indicate that line is created from form
            ctx.SetContext("Form", true);
            MMasterForecastLine Line = MMasterForecastLine.GetOrCreate(ctx, trx, MasterForecast_ID, Product_ID, Charge_ID, Attribute_ID, ProductCategories);
            Line.SetAD_Org_ID(Org_ID);
            Line.SetC_MasterForecast_ID(MasterForecast_ID);
            Line.SetC_Charge_ID(Charge_ID);
            Line.SetM_Product_ID(Product_ID);
            Line.SetC_UOM_ID(UOM_ID);
           // Line.SetM_AttributeSetInstance_ID(Attribute_ID);
            Line.SetIsBOM(BOM.Equals("Y") ? true : false);
            Line.SetBOMUse(BOMUse.Equals("") ? null : BOMUse);
            if (Env.IsModuleInstalled("VAMFG_"))
            {
                Line.Set_Value("VAMFG_M_Routing_ID", Routing_ID);
            }
            Line.SetM_BOM_ID(M_BOM_ID);
            //if (!Line.Is_New())
            //{
            //    //if not new record then decrement count
            //    Count--;
            //}
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
                    log.Log(Level.SEVERE, Msg.GetMsg(ctx, "MasterForecastLineNotSaved") + val);
                }
            }
            else
            {
                Count++;

                MFLineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(LineNo), 0)+10  FROM C_MasterForecastLineDetails WHERE " +
                "C_MasterForecastLine_ID=" + Line.GetC_MasterForecastLine_ID(), null, Line.Get_Trx()));

                //Create MasterForecast LineDetails
                MMasterForecastLineDetails Linedetails = CreateMasterForecastLinedetails(Line, Order_ID, OrderLine_ID, Project_ID, ProjectLine_ID, TeamForecast_ID, ForecastLine_ID,
                                                         Charge_ID, Product_ID, Attribute_ID, UOM_ID, BOM, BaseQuantity, UnitPrice, ProductCategories);
                if (!Linedetails.Save(Line.Get_Trx()))
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
        public MMasterForecastLineDetails CreateMasterForecastLinedetails(MMasterForecastLine Line, int Order_ID, int OrderLine_ID, int Project_ID, int ProjectLine_ID, int TeamForecast_ID, int ForecastLine_ID, int Charge_ID, int Product_ID, int Attribute_ID, int UOM_ID, String BOM, Decimal? BaseQuantity, decimal UnitPrice, string ProductCategories)
        {
            MMasterForecastLineDetails lineDetails = MMasterForecastLineDetails.GetOrCreate(Line, Product_ID, ProductCategories);
            lineDetails.SetAD_Client_ID(Line.GetAD_Client_ID());
            lineDetails.SetAD_Org_ID(Line.GetAD_Org_ID());
            lineDetails.SetC_MasterForecastLine_ID(Line.GetC_MasterForecastLine_ID());
            lineDetails.SetM_Product_ID(Product_ID);
            if (Util.GetValueOfInt(lineDetails.Get_ID()) == 0)
            {
                //only set line no when new line is created
                lineDetails.SetLineNo(MFLineNo);
            }
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


        /// <summary>
        /// Create Budget Forecast Lines
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="trx">Transaction</param>
        /// <param name="Org_ID">Organization</param>
        /// <param name="BudgetForecast_ID">Budget Forecast</param>
        /// <param name="Order_ID">Sales Order</param>
        /// <param name="OrderLine_ID">Order Line</param>
        /// <param name="Project_ID">Opportunity</param>
        /// <param name="ProjectLine_ID">Opportunity Line</param>
        /// <param name="MasterForecast_ID">MasterForecast</param>
        /// <param name="MasterForecastLine_ID">MasterForecastLine</param>
        /// <param name="Charge_ID">Charge</param>
        /// <param name="Product_ID">Product</param>
        /// <param name="Attribute_ID">Attribute set Instance</param>
        /// <param name="UOM_ID">UOM</param>
        /// <param name="BOM">Billof Material</param>
        /// <param name="BaseQuantity">Quantity</param>
        /// <param name="UnitPrice">Price</param>
        /// <param name="PurchaseUnitPrice">PriceStd From PriceList</param>
        /// <param name="ProductCategories">Product Category</param>
        /// <param name="Period">Period</param>
        /// <param name="Date">Date</param>
        /// <writer>209</writer>
        public void CreateBudgetForecastLines(Ctx ctx, Trx trx, int Org_ID, int BudgetForecast_ID, int Order_ID, int OrderLine_ID, int Project_ID, 
        int ProjectLine_ID, int MasterForecast_ID, int MasterForecastLine_ID, int Charge_ID, int Product_ID, int Attribute_ID, int UOM_ID, 
        int OrderBPartner, int OrderLocation, int ProjectBPartner, int ProjectLocation, String BOM, decimal? BaseQuantity, decimal UnitPrice, 
        decimal? PurchaseUnitPrice, string ProductCategories, int Period, DateTime? Date, string BOMUse, int M_BOM_ID, int Routing_ID, decimal AdjustedQty,string IsAdjusted)
        {
            //set the context to indicate that line is created from form
            ctx.SetContext("Form", true);
            ProductLinePo = GetOrCreate(ctx, trx, BudgetForecast_ID, Product_ID, Charge_ID, Attribute_ID, ProductCategories);
            ProductLinePo.Set_Value("AD_Org_ID", Org_ID);
            ProductLinePo.Set_Value("AD_Client_ID", ctx.GetAD_Client_ID());
            ProductLinePo.Set_Value("C_Charge_ID", Charge_ID);
            ProductLinePo.Set_Value("M_Product_ID", Product_ID);
            ProductLinePo.Set_Value("C_UOM_ID", UOM_ID);
           // ProductLinePo.Set_Value("VA073_AdjustedQty", AdjustedQty+ Util.GetValueOfDecimal(ProductLinePo.Get_Value("VA073_AdjustedQty")));         
            ProductLinePo.Set_Value("", UOM_ID);
            // ProductLinePo.Set_Value("M_AttributeSetInstance_ID", Attribute_ID);
            ProductLinePo.Set_Value("IsBOM", BOM.Equals("Y") ? true : false);
            
            if (Env.IsModuleInstalled("VAMFG_") && Util.GetValueOfInt(ProductLinePo.Get_Value("VAMFG_M_Routing_ID")) == 0)
            {
                ProductLinePo.Set_Value("VAMFG_M_Routing_ID", Routing_ID);
            }
            if (Util.GetValueOfInt(ProductLinePo.Get_Value("M_BOM_ID")) == 0)
            {
                ProductLinePo.Set_Value("M_BOM_ID", M_BOM_ID);
            }
            if (string.IsNullOrEmpty(Util.GetValueOfString(ProductLinePo.Get_Value("VA073_BOMUse"))))
            {
                ProductLinePo.Set_Value("VA073_BOMUse", BOMUse.Equals("") ? null : BOMUse);
            }

           // ProductLinePo.Set_Value("VA073_PurchaseUnitPrice", PurchaseUnitPrice);
           // ProductLinePo.Set_Value("VA073_PurchaseValue", PurchaseUnitPrice * Util.GetValueOfDecimal(ProductLinePo.Get_Value("TotalQty")));
            if (Env.IsModuleInstalled("VAMFG_") && Util.GetValueOfInt(ProductLinePo.Get_Value("M_BOM_ID")) == 0)
            {
                //fetch BOM ,BOMUSE ,Routing of selected Product 
                string _sql = @"SELECT BOM.M_BOM_ID ,BOM.BOMUse,Routing.VAMFG_M_Routing_ID FROM M_Product p 
                    INNER JOIN M_BOM  BOM ON p.M_product_ID = BOM.M_Product_ID 
                    LEFT JOIN VAMFG_M_Routing Routing ON Routing.M_product_ID=p.M_product_ID AND Routing.VAMFG_IsDefault='Y'
                    WHERE p.M_Product_ID=" + Product_ID + " AND p.ISBOM = 'Y' AND BOM.IsActive = 'Y'";

                DataSet ds = DB.ExecuteDataset(_sql, null, trx);
                if (ds != null && ds.Tables[0].Rows.Count == 1)
                {
                    ProductLinePo.Set_Value("VA073_BOMUse", Util.GetValueOfString(ds.Tables[0].Rows[0]["BOMUse"]));
                    ProductLinePo.Set_Value("VAMFG_M_Routing_ID", Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAMFG_M_Routing_ID"]));
                    ProductLinePo.Set_Value("M_BOM_ID", Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_BOM_ID"]));
                }
            }

            if (!ProductLinePo.Save(trx))
            {
                pp = VLogger.RetrieveError();
                if (pp != null)
                {
                    string val = pp.GetName();
                    if (string.IsNullOrEmpty(val))
                    {
                        val = pp.GetValue();
                    }
                    log.Log(Level.SEVERE, Msg.GetMsg(ctx, "VA073_ProductLineNotSave") + val);
                }
            }
            else
            {
                Count++;

                FLineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(LineNo), 0)+10  FROM VA073_ForecastLine WHERE " +
                "VA073_ProductLine_ID=" + ProductLinePo.Get_Value("VA073_ProductLine_ID"), null, ProductLinePo.Get_Trx()));
                //Create MasterForecast LineDetails
                ForecastLinePO = CreateBudgetForecastLineDetails(ProductLinePo, Order_ID, OrderLine_ID, Project_ID, ProjectLine_ID, MasterForecast_ID, MasterForecastLine_ID,
                                      Charge_ID, Product_ID, Attribute_ID, UOM_ID, BOM, BaseQuantity, UnitPrice, Period, Date, OrderBPartner, OrderLocation, ProjectBPartner, 
                                      ProjectLocation, ProductCategories,Routing_ID,BOMUse,M_BOM_ID,IsAdjusted);

                if (!ForecastLinePO.Save(trx))
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
                        else if (MasterForecastLine_ID > 0)
                        {
                            log.Log(Level.SEVERE, Msg.GetMsg(ctx, "MasterForecastlineDetailsNotSave") + val + Msg.GetMsg(ctx, "TeamForecastLine") + ": " + MasterForecastLine_ID);
                        }
                        else if (OrderLine_ID == 0 && ProjectLine_ID == 0 && MasterForecastLine_ID == 0 && Product_ID > 0)
                        {
                            log.Log(Level.SEVERE, Msg.GetMsg(ctx, "MasterForecastlineDetailsNotSave") + val + Msg.GetMsg(ctx, "Product") + ": " + Product_ID);
                        }
                    }
                }
                else
                {
                    FLineNo += 10;
                    //update Amounts at Headers  
                   // string _sql = "UPDATE VA073_ProductLine SET " +
                   // "VA073_ForecastQty=(SELECT NVL(SUM(VA073_Quantity),0) FROM VA073_ForecastLine WHERE (NVL(C_MasterForecast_ID,0)>0 OR (NVL(C_Order_ID,0)=0 AND NVL(C_Project_ID,0)=0)) AND VA073_ProductLine_ID=" + ProductLinePo.Get_Value("VA073_ProductLine_ID") + "), " +
                   //"OppQty=(SELECT NVL(SUM(VA073_Quantity),0) FROM VA073_ForecastLine WHERE NVL(C_Project_ID,0)>0 AND VA073_ProductLine_ID=" + ProductLinePo.Get_Value("VA073_ProductLine_ID") + "), " +
                   //"SalesOrderQty =(SELECT NVL(SUM(VA073_Quantity),0) FROM VA073_ForecastLine WHERE NVL(C_Order_ID,0)>0 AND VA073_ProductLine_ID=" + ProductLinePo.Get_Value("VA073_ProductLine_ID") + "), " +
                   //"TotalQty=(SELECT NVL(SUM(VA073_Quantity),0) FROM VA073_ForecastLine WHERE  VA073_ProductLine_ID=" + ProductLinePo.Get_Value("VA073_ProductLine_ID") + ") , " +
                   // "VA073_Price= (Round((SELECT NVL(SUM(VA073_TotalAmt),0)/ NVL(SUM(VA073_Quantity),0) FROM VA073_Forecastline WHERE VA073_ProductLine_ID=" + ProductLinePo.Get_Value("VA073_ProductLine_ID") + "), " +
                   //  Precision + ")), " +
                   // "PlannedRevenue =(ROUND((SELECT SUM(VA073_TotalAmt) FROM VA073_Forecastline WHERE VA073_ProductLine_ID=" + ProductLinePo.Get_Value("VA073_ProductLine_ID") + ")," + Precision + "))" +
                   // " WHERE VA073_ProductLine_ID=" + ProductLinePo.Get_Value("VA073_ProductLine_ID");

                   // if (DB.ExecuteQuery(_sql, null, trx) > 0)
                   // {
                   //     _sql = "UPDATE VA073_SalesForecast SET VA073_TotalAmt =" +
                   //     "(Round((SELECT SUM(PlannedRevenue) FROM VA073_ProductLine WHERE VA073_SalesForecast_ID= (SELECT VA073_SalesForecast_ID FROM VA073_ProductLine WHERE VA073_ProductLine_ID=" + ProductLinePo.Get_Value("VA073_ProductLine_ID") + " ))," + Precision + "))" +
                   //     ",VA073_TotalQty= (SELECT SUM(TotalQty) FROM VA073_ProductLine WHERE VA073_SalesForecast_ID= (SELECT VA073_SalesForecast_ID FROM VA073_ProductLine WHERE VA073_ProductLine_ID=" + ProductLinePo.Get_Value("VA073_ProductLine_ID") + " )) " +
                   //     " WHERE VA073_SalesForecast_ID=(SELECT VA073_SalesForecast_ID FROM VA073_ProductLine WHERE VA073_ProductLine_ID=" + ProductLinePo.Get_Value("VA073_ProductLine_ID") + " ) ";

                   //     DB.ExecuteQuery(_sql, null, trx);
                   // }

                   // //create line Components 
                   // CreateLineComponents(ctx, trx, ForecastLinePO, Util.GetValueOfInt(ForecastLinePO.Get_Value("M_BOM_ID")));
                }
            }
        }

        /// <summary>
        /// Is Used to Get or Create  Instance of  Product Line
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="trx">Transaction</param>
        /// <param name="BudgetForeCast_ID">Budget Forecast</param>
        /// <param name="M_Product_ID">Product</param>
        /// <param name="Charge_ID">Charge</param>
        /// <param name="M_AttributeSetInstance_ID">AttributeSetInstance</param>
        /// <param name="ProductCategories">Product Category</param>
        /// <writer>209</writer>
        /// <returns>Product Line object</returns>
        public PO GetOrCreate(Ctx ctx, Trx trx, int BudgetForeCast_ID, int M_Product_ID, int Charge_ID, int M_AttributeSetInstance_ID, string ProductCategories)
        {
            PO retValue = null;
            String sql = "SELECT * FROM VA073_ProductLine WHERE ";
            if (!String.IsNullOrEmpty(ProductCategories))
            {
                sql += "NVL(M_Product_ID,0) = " + M_Product_ID + " AND";
            }
            else
            {
               
                sql += " NVL(M_AttributeSetInstance_ID,0)=" + M_AttributeSetInstance_ID + " AND ";
            
                if (M_Product_ID > 0)
                {
                    sql += " NVL(M_Product_ID,0) = " + M_Product_ID + " AND";
                }
                if (Charge_ID > 0)
                {
                    sql += " NVL(C_Charge_ID,0) = " + Charge_ID + " AND";
                }

            }
            sql += " NVL(VA073_SalesForecast_ID,0) =" + BudgetForeCast_ID;


            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trx);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = ProductLineTbl.GetPO(ctx, dr, trx);
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            if (retValue == null)
            {
                retValue = CreateProductLine(ctx, trx, BudgetForeCast_ID, M_Product_ID, M_AttributeSetInstance_ID);
            }
            //else
            //{
            //    if (Count > 0)
            //        Count--;
            //}
            return retValue;
        }

        /// <summary>
        /// Create Product Line 
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="trx">Transaction</param>
        /// <param name="BudgetForecast_ID">Budget Forecast</param>
        /// <param name="M_Product_ID">Product</param>
        /// <param name="M_AttributeSetInstance_ID">AttributeSetInstance</param>
        /// <writer>209</writer>
        /// <returns>Po object</returns>
        public PO CreateProductLine(Ctx ctx, Trx trx, int BudgetForecast_ID, int M_Product_ID, int M_AttributeSetInstance_ID)
        {
            ProductLinePo = ProductLineTbl.GetPO(ctx, 0, trx);
            int LineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(LineNo), 0)+10  FROM VA073_ProductLine WHERE " +
                "VA073_SalesForecast_ID = " + BudgetForecast_ID, null, trx));
            ProductLinePo.Set_Value("LineNo", LineNo);
            ProductLinePo.Set_ValueNoCheck("VA073_SalesForecast_ID", BudgetForecast_ID);
            ProductLinePo.Set_Value("M_Product_ID", M_Product_ID);
            ProductLinePo.Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);

            return ProductLinePo;
        }



        /// <summary>
        /// Create Line Details
        /// </summary>
        /// <param name="Parent">Product Line</param>
        /// <param name="Order_ID">Sales Order</param>
        /// <param name="OrderLine_ID">Order Line</param>
        /// <param name="Project_ID">Opportunity</param>
        /// <param name="ProjectLine_ID">Opportunity Line</param>
        /// <param name="MasterForecast_ID">MasterForecast</param>
        /// <param name="MasterForecastLine_ID">MasterForecastLine</param>
        /// <param name="Charge_ID">Charge</param>
        /// <param name="Product_ID">Product</param>
        /// <param name="Attribute_ID">AttributeSetInstance</param>
        /// <param name="UOM_ID">UOM</param>
        /// <param name="BOM">Bill of material</param>
        /// <param name="Quantity">Quantity</param>
        /// <param name="UnitPrice">UnitPrice</param>
        /// <param name="Period">Period</param>
        /// <param name="Date">TrxDate</param>
        /// <param name="OrderBPartner">BusinessPartner of Order  </param>
        /// <param name="OrderLocation">Location</param>
        /// <param name="ProjectBPartner">BusinessPartner of Project</param>
        /// <param name="ProjectLocation">Location</param>
        /// <returns>PO object</returns>
        public PO CreateBudgetForecastLineDetails(PO Parent, int Order_ID, int OrderLine_ID, int Project_ID, int ProjectLine_ID, int MasterForecast_ID, int MasterForecastLine_ID,
        int Charge_ID, int Product_ID, int Attribute_ID, int UOM_ID, String BOM, decimal? Quantity, decimal UnitPrice, int Period, DateTime? Date, int OrderBPartner, int OrderLocation, 
        int ProjectBPartner, int ProjectLocation, string ProductCategories,int Routing, string BOMUse, int BOM_ID,string IsAdjusted)
        {
            //VA073_ForecastLine object
            ForecastLinePO = GetOrCreate(Parent, Product_ID, ProductCategories);

            ForecastLinePO.SetAD_Client_ID(Parent.GetAD_Client_ID());
            ForecastLinePO.SetAD_Org_ID(Parent.GetAD_Org_ID());
            ForecastLinePO.Set_ValueNoCheck("VA073_ProductLine_ID", Parent.Get_Value("VA073_ProductLine_ID"));
            ForecastLinePO.Set_Value("M_Product_ID", Product_ID);
            ForecastLinePO.Set_Value("C_Period_ID", Period);
            ForecastLinePO.Set_Value("DatePromised", Date);
            if (Util.GetValueOfInt(ForecastLinePO.Get_ID()) == 0)
            {
                //only set line no when new line is created
                ForecastLinePO.Set_Value("LineNo", FLineNo);
            }
            ForecastLinePO.Set_Value("C_Order_ID", Order_ID);
            ForecastLinePO.Set_Value("C_OrderLine_ID", OrderLine_ID);
            ForecastLinePO.Set_Value("C_Project_ID", Project_ID);
            ForecastLinePO.Set_Value("C_ProjectLine_ID", ProjectLine_ID);
            ForecastLinePO.Set_Value("C_MasterForecast_ID", MasterForecast_ID);
            ForecastLinePO.Set_Value("C_MasterForecastLine_ID", MasterForecastLine_ID);
            ForecastLinePO.Set_Value("C_Charge_ID", Charge_ID);
            ForecastLinePO.Set_Value("M_AttributeSetInstance_ID", Attribute_ID);
            ForecastLinePO.Set_Value("C_UOM_ID", Parent.Get_Value("C_UOM_ID"));
            ForecastLinePO.Set_Value("IsBOM", BOM.Equals("Y") ? true : false);
            ForecastLinePO.Set_Value("IsAdjusted", IsAdjusted.Equals("Y") ? true : false);
            ForecastLinePO.Set_Value("VA073_Quantity", Quantity);
            ForecastLinePO.Set_Value("VA073_Price", UnitPrice);
            ForecastLinePO.Set_Value("VA073_TotalAmt", Decimal.Round(Util.GetValueOfDecimal(UnitPrice * Quantity),Precision));
            if (OrderLine_ID > 0)
            {
                ForecastLinePO.Set_Value("C_BPartner_ID", OrderBPartner);
                ForecastLinePO.Set_Value("C_BPartner_Location_ID", OrderLocation);
               
            }
            else if (ProjectLine_ID > 0)
            {
                ForecastLinePO.Set_Value("C_BPartner_ID", ProjectBPartner);
                ForecastLinePO.Set_Value("C_BPartner_Location_ID", ProjectLocation);             
            }
            if (MasterForecastLine_ID > 0)
            {
                ForecastLinePO.Set_Value("M_BOM_ID", BOM_ID > 0 ? BOM_ID : ProductLinePo.Get_Value("M_BOM_ID"));
                if (Env.IsModuleInstalled("VAMFG_"))
                {
                    ForecastLinePO.Set_Value("VAMFG_M_Routing_ID", Routing > 0 ? Routing : ProductLinePo.Get_Value("VAMFG_M_Routing_ID"));
                }
                ForecastLinePO.Set_Value("VA073_BOMUse", !string.IsNullOrEmpty(BOMUse) ? ProductLinePo.Get_Value("VA073_BOMUse") : BOMUse);
            }
            else
            {
                ForecastLinePO.Set_Value("VA073_BOMUse", ProductLinePo.Get_Value("VA073_BOMUse"));
                ForecastLinePO.Set_Value("M_BOM_ID", ProductLinePo.Get_Value("M_BOM_ID"));
                if (Env.IsModuleInstalled("VAMFG_"))
                {
                    ForecastLinePO.Set_Value("VAMFG_M_Routing_ID", ProductLinePo.Get_Value("VAMFG_M_Routing_ID"));
                }
            }
            return ForecastLinePO;
        }

        /// <summary>
        ///  Is Used to Get or Create  Instance of ForecastLine
        /// </summary>
        /// <param name="Line">Product line</param>
        /// <param name="M_Product_ID">Product</param>
        /// <param name="ProductCategories">Product Category</param>
        /// <Writer>209</Writer>
        /// <returns>Forecast Line</returns>
        public PO GetOrCreate(PO Line, int M_Product_ID, string ProductCategories)
        {
            PO retValue = null;
            if (!String.IsNullOrEmpty(ProductCategories))
            {
                String sql = "SELECT * FROM VA073_ForecastLine WHERE NVL(M_Product_ID,0) = " + M_Product_ID + " AND NVL(C_OrderLine_ID,0) =0 AND NVL(C_ProjectLine_ID,0)=0  AND " +
                    " NVL(C_MasterForecastLine_ID,0) =0 AND NVL(VA073_ProductLine_ID,0) =" + Line.Get_Value("VA073_ProductLine_ID");

                DataTable dt = null;
                IDataReader idr = null;
                try
                {
                    idr = DB.ExecuteReader(sql, null, Line.Get_Trx());
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        retValue = ForecastLineTbl.GetPO(Line.GetCtx(), dr, Line.Get_Trx());
                    }
                }
                catch (Exception ex)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    log.Log(Level.SEVERE, sql, ex);
                }
                finally
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    dt = null;
                }
            }
            if (retValue == null)
                retValue = CreateProductlineDetails(Line, M_Product_ID);

            return retValue;
        }

        /// <summary>
        /// Create new ForecastLine
        /// </summary>
        /// <param name="Line">ProductLine</param>
        /// <param name="M_Product_ID">Product</param>
        /// <Writer>209</Writer>
        /// <returns>Forecastline</returns>
        public PO CreateProductlineDetails(PO Line, int M_Product_ID)
        {
            ForecastLinePO = ForecastLineTbl.GetPO(Line.GetCtx(), 0, Line.Get_Trx());
            ForecastLinePO.Set_Value("M_Product_ID", M_Product_ID);
            return ForecastLinePO;
        }

        /// <summary>
        /// Create Line Components 
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="trx">Transaction</param>
        /// <param name="ForecastLine">ForecastLine</param>
        /// <param name="BOM_ID">M_BOM_ID</param>
        public void CreateLineComponents(Ctx ctx, Trx trx, PO ForecastLine, int BOM_ID)
        {
            string _sql = @"SELECT PBOM.BOMQty , PBOM.M_ProductBom_ID , p.C_UOM_ID , p.VA073_ActivityType_ID, 
                    (SELECT PriceStd  FROM  M_ProductPrice WHERE IsActive='Y' AND M_PriceList_Version_ID = (SELECT MAX(M_PriceList_Version_ID) 
                     FROM M_PriceList_Version WHERE IsActive='Y'AND M_PriceList_ID =" + PriceList + ") AND M_Product_ID = PBOM.M_ProductBom_ID AND " +
                    "NVL(M_AttributeSetInstance_ID, 0) = NVL(PBOM.M_AttributeSetInstance_ID, 0) AND C_UOM_ID = p.C_UOM_ID) AS StdPrice ," +
                    "PBOM.M_AttributeSetInstance_ID " +
                    "FROM M_Product p INNER JOIN M_BOM  BOM on p.M_product_ID = BOM.M_Product_ID " +
                    "INNER JOIN M_BOMProduct PBOM ON BOM.M_BOM_ID = PBOM.M_BOM_ID " +
                    "WHERE BOM.M_BOM_ID = " + BOM_ID + " AND p.ISBOM = 'Y'  AND BOM.IsActive='Y'";

            DataSet Componentds = DB.ExecuteDataset(_sql, null, trx);
            if (Componentds != null && Componentds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < Componentds.Tables[0].Rows.Count; i++)
                {
                    LineComponentPO = GetOrCreate(ForecastLine, Util.GetValueOfInt(Componentds.Tables[0].Rows[i]["M_ProductBom_ID"]), Util.GetValueOfInt(Componentds.Tables[0].Rows[i]["C_UOM_ID"]),
                    Util.GetValueOfInt(Componentds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Util.GetValueOfInt(Componentds.Tables[0].Rows[i]["VA073_ActivityType_ID"]));
                    LineComponentPO.Set_Value("VA073_Quantity", Util.GetValueOfDecimal(Componentds.Tables[0].Rows[i]["BOMQty"]) *
                    Util.GetValueOfDecimal(ForecastLine.Get_Value("VA073_Quantity")));
                    if (Util.GetValueOfDecimal(Componentds.Tables[0].Rows[i]["StdPrice"]) == 0)
                    {
                        log.Log(Level.WARNING, Msg.GetMsg(ctx, "PurchasePriceNotFound"));
                        continue;
                    }
                    else
                    {
                        //Price conversion from pricelist currency to header currency

                        ConvertedPrice = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(Componentds.Tables[0].Rows[i]["StdPrice"]), FromCurrency,
                        ToCurrency, DateAcct, ConversionType, ctx.GetAD_Client_ID(), ForecastLine.GetAD_Org_ID());
                        if (ConvertedPrice == 0)
                        {
                            log.Log(Level.WARNING, Msg.GetMsg(ctx, "ConversionNotFound") + " " + Msg.GetMsg(ctx, "From") + " " + FromCurrencyName +
                                Msg.GetMsg(ctx, "To") + " " + ToCurrencyName);
                            continue;
                        }

                    }

                    LineComponentPO.Set_Value("VA073_Price", ConvertedPrice);
                    LineComponentPO.Set_Value("VA073_TotalAmt", Util.GetValueOfDecimal(LineComponentPO.Get_Value("VA073_Quantity")) * ConvertedPrice);
                    if (!LineComponentPO.Save(trx))
                    {
                        ValueNamePair vp = VLogger.RetrieveError();
                        if (vp != null)
                        {
                            string val = vp.GetName();
                            if (string.IsNullOrEmpty(val))
                            {
                                val = vp.GetValue();
                            }
                            log.SaveError("", Msg.GetMsg(ctx, "VA073_NotSaveLineComponent") + Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_ProductBom_ID"]) + "," +
                                Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]) + val);
                        }

                    }
                    else
                    {
                        //Update Purchase Unit Price and Purcahse value at Product Line
                        _sql = "UPDATE VA073_ProductLine " +
                          "SET VA073_PurchaseValue=(SELECT SUM(VA073_LineComponents.VA073_TotalAmt) FROM VA073_LineComponents  INNER JOIN VA073_ForecastLine  " +
                          "ON VA073_ForecastLine.VA073_ForecastLine_ID=VA073_LineComponents.VA073_ForecastLine_ID WHERE VA073_ForecastLine.VA073_ProductLine_ID =" +
                          "(SELECT VA073_ProductLine_ID FROM VA073_ForecastLine WHERE VA073_ForecastLine_ID=" + ForecastLine.Get_Value("VA073_ForecastLine_ID") + "))," +
                          " VA073_PurchaseUnitPrice= ROUND((SELECT SUM(VA073_LineComponents.VA073_TotalAmt) FROM VA073_LineComponents  INNER JOIN VA073_ForecastLine  " +
                          "ON VA073_ForecastLine.VA073_ForecastLine_ID=VA073_LineComponents.VA073_ForecastLine_ID WHERE VA073_ForecastLine.VA073_ProductLine_ID= " +
                          "(SELECT VA073_ProductLine_ID FROM VA073_ForecastLine WHERE VA073_ForecastLine_ID = " + ForecastLine.Get_Value("VA073_ForecastLine_ID") + ")) / " +
                          "(SELECT VA073_ProductLine.TotalQty FROM VA073_ProductLine WHERE VA073_ProductLine_ID =" +
                          "(SELECT VA073_ProductLine_ID FROM VA073_ForecastLine WHERE VA073_ForecastLine_ID=" + ForecastLine.Get_Value("VA073_ForecastLine_ID") + "))," + Precision + ") " +
                          "WHERE VA073_ProductLine_ID=(SELECT VA073_ProductLine_ID FROM VA073_ForecastLine WHERE VA073_ForecastLine_ID=" + ForecastLine.Get_Value("VA073_ForecastLine_ID") + ")";

                    }
                }
            }
        }

        /// <summary>
        /// Is Used to Get or Create  Instance of ForecastLine
        /// </summary>
        /// <param name="Forecastline">Forecast Line</param>
        /// <param name="M_Product_ID">Product</param>
        /// <param name="C_UOM_ID">UOM</param>
        /// <param name="M_AttributeSetInstance_ID">AttributeSetInstance</param>
        /// <param name="VA073_ActivityType_ID">ActivityType</param>
        /// <writer>209</writer>
        /// <returns>Line Components</returns>
        public PO GetOrCreate(PO Forecastline, int M_Product_ID, int C_UOM_ID, int M_AttributeSetInstance_ID, int VA073_ActivityType_ID)
        {
            PO retValue = null;
            String sql = "SELECT * FROM VA073_LineComponents " +
                         " WHERE  VA073_ForecastLine_ID = " + Forecastline.Get_Value("VA073_ForecastLine_ID") +
                         " AND NVL(M_AttributeSetInstance_ID,0)=" + M_AttributeSetInstance_ID +
                         " AND NVL(C_UOM_ID,0)= " + C_UOM_ID +
                         " AND NVL(M_Product_ID,0)=" + M_Product_ID;


            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, Forecastline.Get_Trx());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = LineComponentTbl.GetPO(Forecastline.GetCtx(), dr, Forecastline.Get_Trx());
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            if (retValue == null)
                retValue = LineComponents(Forecastline, M_Product_ID, C_UOM_ID, M_AttributeSetInstance_ID, VA073_ActivityType_ID);

            return retValue;
        }
        /// <summary>
        /// Create new instance of Line Components
        /// </summary>
        /// <param name="parent">Forecast Line</param>
        /// <param name="M_Product_ID">Product</param>
        /// <param name="C_UOM_ID">UOM</param>
        /// <param name="M_AttributeSetInstance_ID">AttributeSetInsatnce</param>
        /// <param name="VA073_ActivityType_ID">Activity Type</param>
        /// <returns>Line Components</returns>
        public PO LineComponents(PO parent, int M_Product_ID, int C_UOM_ID, int M_AttributeSetInstance_ID, int VA073_ActivityType_ID)
        {
            LineComponentPO = LineComponentTbl.GetPO(parent.GetCtx(), 0, parent.Get_Trx());

            int LineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(LineNo), 0) AS DefaultValue FROM VA073_LineComponents WHERE VA073_ForecastLine_ID="
            + parent.Get_Value("VA073_ForecastLine_ID"), null, parent.Get_Trx()));

            LineComponentPO.SetAD_Client_ID(parent.GetAD_Client_ID());
            LineComponentPO.SetAD_Org_ID(parent.GetAD_Org_ID());
            LineComponentPO.Set_Value("M_Product_ID", M_Product_ID);
            LineComponentPO.Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
            LineComponentPO.Set_Value("C_UOM_ID", C_UOM_ID);
            LineComponentPO.Set_ValueNoCheck("VA073_ForecastLine_ID", parent.Get_Value("VA073_ForecastLine_ID"));
            LineComponentPO.Set_Value("LineNo", LineNo + 10);
            LineComponentPO.Set_Value("VA073_ActivityType_ID", VA073_ActivityType_ID);

            return LineComponentPO;
        }

    }

}
