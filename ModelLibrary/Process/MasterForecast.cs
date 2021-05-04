/********************************************************
 * Project Name   : ViennaAdvantage
 * Class Name     : MasterForecast
 * Purpose        : 
 * Class Used     : SvrProcess
 * Chronological    Development
 * Lokesh Chauhan   24-Jan-2012
  ******************************************************/
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
using System.Security.Policy;
using VAdvantage.ProcessEngine;
//using ViennaAdvantage.Model;

namespace ViennaAdvantageServer.Process
{
    public class MasterForecast : SvrProcess
    {
        #region Private Variables
        private string msg = "";
        private string sql = "";
        private int C_Period_ID = 0;
        private X_C_MasterForecast mf = null;
        private DataSet dsOrder = null;
        private DataSet dsForecast = null;
        private DataSet dsOpp = null;
        private DataSet _ds = null;
        private MMasterForecastLine mfLine = null;
        private int Count = 0;
        private int LineNo = 0;
        private MTable tbl = null;
        private PO po = null;
        private int Currency = 0;
        int StdPrecision = 0;
        private Decimal? ConvertedAmt = null;
        private Decimal? OppQty = null;
        #endregion
        protected override void Prepare()
        {

        }

        /// <SUMmary>
        /// Consolidate Data FROM sales order , Team Forecast, Opportunity
        /// </SUMmary>
        /// <returns>Info</returns>
        protected override string DoIt()
        {
            mf = new X_C_MasterForecast(GetCtx(), GetRecord_ID(), Get_Trx());
            if (Util.GetValueOfInt(mf.Get_Value("M_PriceList_ID")) == 0)
            {
                return Msg.GetMsg(mf.GetCtx(), "CreatelinesManually");
            }
            C_Period_ID = mf.GetC_Period_ID();
            Currency = Util.GetValueOfInt(mf.Get_Value("C_Currency_ID"));


            StdPrecision= Util.GetValueOfInt(DB.ExecuteScalar("SELECT StdPrecision FROM C_Currency WHERE C_Currency_ID=" + Currency, null, null));

            //Get Table_Id to create PO Object
            // sql = @"SELECT AD_TABLE_ID  FROM AD_TABLE WHERE tablename LIKE 'VA073_MasterForecastLineDetail' AND IsActive = 'Y'";
            // tableId = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            // tbl = new MTable(GetCtx(), tableId, null);
            tbl = MTable.Get(GetCtx(), "VA073_MasterForecastLineDetail");


            // sql = "delete FROM c_masterforecastline WHERE c_masterforecast_id = " + mf.GetC_MasterForecast_ID();
            // int count = DB.ExecuteQuery(sql, null, null);
            if (C_Period_ID != 0)
            {
                sql = "SELECT COUNT(C_MasterForecastLine_ID) FROM c_masterforecastline WHERE c_masterforecast_id = " + GetRecord_ID();
                int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                if (count > 0)
                {
                    sql = "UPDATE c_masterforecastline set Processed = 'Y' WHERE c_masterforecast_id = " + GetRecord_ID();
                    int res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
                    sql = "UPDATE c_masterforecast set Processed = 'Y' WHERE c_masterforecast_id = " + GetRecord_ID();
                    res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
                    msg = Msg.GetMsg(GetCtx(), "RecordsProcessed");
                    return msg;
                }
                if (!Env.IsModuleInstalled("VA073_"))
                {
                    //sql = "SELECT DISTINCT(M_Product_ID) FROM c_forecastline fl INNER JOIN c_forecast f ON (fl.c_forecast_id = f.c_forecast_id) WHERE f.c_period_id = " + C_Period_ID + " AND f.ad_client_id = " + GetCtx().GetAD_Client_ID() + " AND f.isactive = 'Y' AND f.processed = 'Y'";
                    //DataSet idr = null;
                    sql = "SELECT DISTINCT(M_Product_ID), SUM(nvl(qtyentered,0)) AS Quantity,SUM(nvl(pricestd,0)) AS Price,f.C_Currency_ID FROM c_forecastline fl " +
                        "INNER JOIN c_forecast f ON (fl.c_forecast_id = f.c_forecast_id) WHERE f.c_period_id =  " + C_Period_ID +
                        " AND f.ad_client_id = " + mf.GetAD_Client_ID() + " AND f.isactive = 'Y' AND f.processed = 'Y' " +
                        "Group BY M_product_ID,f.C_Currency_ID";
                    try
                    {

                        dsForecast = DB.ExecuteDataset(sql, null, mf.Get_Trx());
                        if (dsForecast != null && dsForecast.Tables[0].Rows.Count > 0)
                        {
                            Decimal? totalQtyTeam = 0;
                            Decimal? totalPriceTeam = 0;
                            Decimal? totalQtyOpp = 0;
                            Decimal? totalPriceOpp = 0;

                            for (int i = 0; i < dsForecast.Tables[0].Rows.Count; i++)
                            {
                                //sql = "SELECT SUM(nvl(qtyentered,0)) AS Quantity,SUM(nvl(pricestd,0)) AS Price,f.C_Currency_ID FROM c_forecastline fl" +
                                //    " INNER JOIN C_Forecast f ON f.C_Forecast_ID = fl.C_Forecast_ID " +
                                //    " WHERE fl.m_product_id = " + Util.GetValueOfInt(_ds[0]) + " AND f.Processed = 'Y' AND f.isactive = 'Y'"+
                                //    " GROUP BY f.C_Currency_ID";

                                //totalQtyTeam = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                                //// sql = "SELECT SUM(nvl(qtyentered,0) * nvl(pricestd,0)) FROM c_forecastline WHERE m_product_id = " + Util.GetValueOfInt(idr[0]) + " AND Processed = 'Y'";
                                //sql = "SELECT SUM(nvl(pricestd,0)) FROM c_forecastline WHERE m_product_id = " + Util.GetValueOfInt(idr[0]) + " AND Processed = 'Y' AND isactive = 'Y'";
                                //totalPriceTeam = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_Trx()));

                                //dsForecast = DB.ExecuteDataset(sql, null, mf.Get_Trx());
                                //if (dsForecast != null && dsForecast.Tables[0].Rows.Count > 0)
                                //{
                                totalPriceTeam = MConversionRate.Convert(mf.GetCtx(), Util.GetValueOfDecimal(dsForecast.Tables[0].Rows[i]["Price"]),
                                     Util.GetValueOfInt(dsForecast.Tables[0].Rows[i]["C_Currency_ID"]), Currency,
                                     Util.GetValueOfDateTime(mf.Get_Value("TRXDATE")),
                                     Util.GetValueOfInt(mf.Get_Value("C_ConversionType_ID")), mf.GetAD_Client_ID(), mf.GetAD_Org_ID());
                                totalQtyTeam = Util.GetValueOfDecimal(dsForecast.Tables[0].Rows[i]["Quantity"]);
                                //}

                                if (mf.IsIncludeOpp())
                                {
                                    sql = "SELECT SUM(NVL(pl.plannedqty,0)) AS Quantity ,SUM(NVL(pl.plannedqty,0) * NVL(pl.plannedprice,0)) AS Price, " +
                                          "p.C_Currency_ID,pl.C_UOM_ID" +
                                          " FROM c_projectline pl INNER JOIN c_project p ON (p.c_project_id = pl.c_project_id) "
                                          + " WHERE " +
                                          "pl.planneddate BETWEEN (SELECT startdate FROM c_period WHERE c_period_id = " + C_Period_ID + ") "
                                          + " AND (SELECT enddate FROM c_period WHERE c_period_id = " + C_Period_ID + ") " +
                                          "AND pl.m_product_id =  " + Util.GetValueOfInt(dsForecast.Tables[0].Rows[i]["M_Product_ID"]) +
                                          " AND p.c_order_id IS NULL AND p.ref_order_id IS NULL AND pl.isactive = 'Y' "
                                          + "GROUP BY C_Currency_ID,pl.C_UOM_ID";

                                    //totalQtyOpp = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_Trx()));

                                    //sql = " SELECT SUM(NVL(pl.plannedqty,0) * NVL(pl.plannedprice,0)) FROM c_projectline pl INNER JOIN c_project p ON (p.c_project_id = pl.c_project_id) "
                                    //    + " WHERE " +
                                    //    " pl.planneddate BETWEEN (SELECT startdate FROM c_period WHERE c_period_id = " + C_Period_ID + ") "
                                    //    + " AND (SELECT enddate FROM c_period WHERE c_period_id = " + C_Period_ID + ") " +
                                    //    " AND pl.m_product_id =  " + Util.GetValueOfInt(idr[0]) +
                                    //    " AND p.c_order_id IS NULL AND p.ref_order_id IS NULL AND pl.isactive = 'Y' AND p.ad_client_id = " + mf.GetAD_Client_ID();
                                    totalQtyOpp = 0;
                                    totalPriceOpp = 0;
                                    dsOpp = DB.ExecuteDataset(sql, null, mf.Get_Trx());
                                    if (dsOpp != null && dsOpp.Tables[0].Rows.Count > 0)
                                    {                                     
                                        for (int k = 0; k < dsOpp.Tables[0].Rows.Count; k++)
                                        {
                                            //Conversion from Project to MasterForecast Currency
                                            totalPriceOpp+= MConversionRate.Convert(mf.GetCtx(), Util.GetValueOfDecimal(dsOpp.Tables[0].Rows[k]["Price"]),
                                                 Util.GetValueOfInt(dsOpp.Tables[0].Rows[k]["C_Currency_ID"]), Currency,
                                                 Util.GetValueOfDateTime(mf.Get_Value("TRXDATE")),
                                                 Util.GetValueOfInt(mf.Get_Value("C_ConversionType_ID")), mf.GetAD_Client_ID(), mf.GetAD_Org_ID());

                                            //Conversion from BaseUOM to UOM on Project Line
                                            decimal? ConvertedQty= MUOMConversion.ConvertProductFrom(mf.GetCtx(), Util.GetValueOfInt(dsForecast.Tables[0].Rows[i]["M_Product_ID"]),
                                            Util.GetValueOfInt(dsOpp.Tables[0].Rows[k]["C_UOM_ID"]), Util.GetValueOfDecimal(dsOpp.Tables[0].Rows[k]["Quantity"]));

                                            if (ConvertedQty == null)
                                            {
                                                totalQtyOpp+= Util.GetValueOfDecimal(dsOpp.Tables[0].Rows[k]["Quantity"]);
                                            }
                                            else
                                            {
                                                totalQtyOpp += ConvertedQty;
                                            }
                                        }
                                    }
                                }


                                Decimal? totalPrice = Decimal.Add(totalPriceTeam.Value, totalPriceOpp.Value);
                                Decimal? totalQty = Decimal.Add(totalQtyTeam.Value, totalQtyOpp.Value);


                                if (totalQty.Value > 0)
                                {
                                    Decimal? avgPrice = Decimal.Divide(totalPrice.Value, totalQty.Value);
                                    avgPrice = Decimal.Round(avgPrice.Value, StdPrecision, MidpointRounding.AwayFromZero);

                                    mfLine = GenerateMasterForecast(Util.GetValueOfInt(dsForecast.Tables[0].Rows[i]["M_Product_ID"]), 0, totalQtyTeam, totalQtyOpp, avgPrice);
                                    if (!mfLine.Save())
                                    {
                                        ValueNamePair vp = VLogger.RetrieveError();
                                        if (vp != null)
                                        {
                                            log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "MasterForecastLineNotSaved") + vp.GetValue() + " - " + vp.GetName());
                                        }
                                        else
                                        {
                                            log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "MasterForecastLineNotSaved"));
                                        }

                                    }
                                }
                            }
                            //if (idr != null)
                            //{
                            //    idr.Close();
                            //    idr = null;
                        } 

                    }
                    catch (Exception e)
                    {
                        log.Log(Level.SEVERE, e.Message);
                    }

                    if (mf.IsIncludeOpp())
                    {
                        OnlyOpportunityProducts();
                    }
                    mf.SetCurrentVersion(true);
                    mf.SetProcessed(true);
                    if (!mf.Save())
                    {
                        log.SaveError("MasterForecastNotSaved", "MasterForecastNotSaved");
                        return GetRetrievedError(mf, "MasterForecastNotSaved");
                    }
                    msg = Msg.GetMsg(GetCtx(), "ProcessCompleted");

                }

                else
                {
                    //VA073 module installed -- Consolidate FROM Sales order , opportunity , Team Forecast
                    TeamForecastProduct();
                    if (mf.IsIncludeOpp())
                    {
                        OnlyOpportunityProducts();
                    }
                    if (Util.GetValueOfBool(mf.Get_Value("VA073_IsIncludeOpenSO")))
                    {
                        SalesOrderProducts();
                    }
                    if (Count == 0)
                    {
                        mf.Get_Trx().Rollback();
                    }
                    else
                    {
                        //UPDATE Master forecast Line Set processed to true
                        sql = "UPDATE C_MasterForecastLine SET Processed='Y' WHERE C_MasterForecast_ID=" + GetRecord_ID();
                        DB.ExecuteQuery(sql, null, mf.Get_Trx());

                        //UPDATE Master forecast Set processed to true
                        sql = "UPDATE C_MasterForecast SET Processed='Y' WHERE C_MasterForecast_ID=" + GetRecord_ID();
                        DB.ExecuteQuery(sql, null, mf.Get_Trx());

                    }

                    msg = Msg.GetMsg(mf.GetCtx(), "ProductLinesDetailCreated") + Count;
                }
            }
            return msg;
        }

        /// <SUMmary>
        ///  Oppportunity Products
        /// </SUMmary>
        /// <returns>No of Lines created</returns>
        private int OnlyOpportunityProducts()
        {
            if (!Env.IsModuleInstalled("VA073_"))
            {
                //sql = " SELECT distinct(pl.m_product_id) FROM c_projectline pl INNER JOIN c_project p ON p.c_project_id = pl.c_project_id WHERE p.c_order_id IS NULL"
                //    + " AND p.ref_order_id IS  ANDNULL pl.m_product_id NOT IN (SELECT DISTINCT(M_Product_ID) FROM c_forecastline fl "
                //    + " INNER JOIN c_forecast f ON (fl.c_forecast_id = f.c_forecast_id) WHERE f.c_period_id = " + C_Period_ID
                //    + " AND f.ad_client_id = " + GetCtx().GetAD_Client_ID() + " AND fl.isactive = 'Y')";
                sql = " SELECT DISTINCT(pl.m_product_id),SUM(nvl(pl.plannedqty,0)) AS Quantity ,SUM(NVL(pl.plannedqty,0) * NVL(pl.plannedprice,0)) AS Price, " +
                        "p.C_Currency_ID,pl.C_UOM_ID " +
                        "FROM c_projectline pl " +
                        "INNER JOIN c_project p ON p.c_project_id = pl.c_project_id " +
                        "WHERE p.c_order_id IS NULL"
                        //       + " AND (SELECT enddate FROM c_period WHERE c_period_id = " + C_Period_ID + ") " +
                        + " AND p.ref_order_id IS NULL AND pl.m_product_id NOT IN (SELECT m_product_id FROM c_masterforecastline WHERE isactive = 'Y'" +
                        " AND c_masterforecast_id = " + GetRecord_ID() + ") AND p.AD_CLient_ID= " +mf.GetAD_Client_ID()+
                        " AND pl.planneddate BETWEEN (SELECT startdate FROM c_period WHERE c_period_id = " + C_Period_ID + ") "+
                        " AND (SELECT enddate FROM c_period WHERE c_period_id = " + C_Period_ID + ") " +
                        " GROUP BY C_Currency_ID,pl.C_UOM_ID,pl.m_product_id";

                //IDataReader idr = null;
                try
                {              
                    Decimal? totalQtyOpp = 0;
                    Decimal? totalPriceOpp = 0;
                    //sql = "SELECT SUM(nvl(pl.plannedqty,0)) AS Quantity ,SUM(NVL(pl.plannedqty,0) * NVL(pl.plannedprice,0)) AS Price, p.C_Currency_ID,pl.C_UOM_ID" +
                    //    " FROM c_projectline pl INNER JOIN c_project p ON (p.c_project_id = pl.c_project_id) "
                    //      + " WHERE " +
                    //       "pl.planneddate BETWEEN (SELECT startdate FROM c_period WHERE c_period_id = " + C_Period_ID + ") "
                    //       + " AND (SELECT enddate FROM c_period WHERE c_period_id = " + C_Period_ID + ") " +
                    //       "AND pl.m_product_id =  " + Util.GetValueOfInt(idr[0]) + " AND p.c_order_id IS NULL AND p.ref_order_id IS NULL AND pl.isactive = 'Y'"
                    //        + " GROUP BY C_Currency_ID,pl.C_UOM_ID";

                    // totalQtyOpp = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));

                    //sql = " SELECT SUM(NVL(pl.plannedqty,0) * NVL(pl.plannedprice,0)) FROM c_projectline pl INNER JOIN c_project p ON (p.c_project_id = pl.c_project_id) "
                    //    + " WHERE  " +
                    //"pl.planneddate BETWEEN (SELECT startdate FROM c_period WHERE c_period_id = " + C_Period_ID + ") "
                    //+ " AND (SELECT enddate FROM c_period WHERE c_period_id = " + C_Period_ID + ") " +
                    //" AND pl.m_product_id =  " + Util.GetValueOfInt(idr[0]) + " AND p.c_order_id IS NULL AND p.ref_order_id IS NULL AND pl.isactive = 'Y'";
                    //totalPriceOpp = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));

                    dsOpp = DB.ExecuteDataset(sql, null, mf.Get_Trx());
                    if (dsOpp != null && dsOpp.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsOpp.Tables[0].Rows.Count; i++)
                        {
                            //Conversion from Project to MasterForecast Currency
                            totalPriceOpp = MConversionRate.Convert(mf.GetCtx(), Util.GetValueOfDecimal(dsOpp.Tables[0].Rows[i]["Price"]),
                                 Util.GetValueOfInt(dsOpp.Tables[0].Rows[i]["C_Currency_ID"]), Currency,
                                 Util.GetValueOfDateTime(mf.Get_Value("TRXDATE")),
                                 Util.GetValueOfInt(mf.Get_Value("C_ConversionType_ID")), mf.GetAD_Client_ID(), mf.GetAD_Org_ID());

                            //Conversion from BaseUOM to UOM on Project Line
                            totalQtyOpp = MUOMConversion.ConvertProductFrom(mf.GetCtx(), Util.GetValueOfInt(dsOpp.Tables[0].Rows[i]["M_Product_ID"]),
                                Util.GetValueOfInt(dsOpp.Tables[0].Rows[i]["C_UOM_ID"]), Util.GetValueOfDecimal(dsOpp.Tables[0].Rows[i]["Quantity"]));
                         
                            if (totalQtyOpp == null)
                            {
                                totalQtyOpp = Util.GetValueOfDecimal(dsOpp.Tables[0].Rows[i]["Quantity"]);
                            }
                            
                            if (totalQtyOpp.Value > 0)
                            {
                                Decimal? avgPrice = Decimal.Divide(totalPriceOpp.Value, totalQtyOpp.Value);
                                avgPrice = Decimal.Round(avgPrice.Value, StdPrecision, MidpointRounding.AwayFromZero);

                                mfLine = GenerateMasterForecast(Util.GetValueOfInt(dsOpp.Tables[0].Rows[i]["M_Product_ID"]), 0, Util.GetValueOfDecimal(Decimal.Zero), totalQtyOpp, avgPrice);
                                if (!mfLine.Save())
                                {
                                    ValueNamePair vp = VLogger.RetrieveError();
                                    if (vp != null)
                                    {
                                        log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "MasterForecastLineNotSaved") + vp.GetValue() + " - " + vp.GetName());
                                    }
                                    else
                                    {
                                        log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "MasterForecastLineNotSaved"));
                                    }

                                }
                            }
                        }
                        //if (idr != null)
                        //{
                        //    idr.Close();
                        //    idr = null;
                        //}
                    }
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, e.Message);
                }
            }
            else
            {
                //VA073_ Module is Installed

                sql = "SELECT pl.m_product_id, p.c_project_id,p.C_Currency_ID,pl.c_projectline_id, pl.plannedqty,pl.C_UOM_ID," +
                    "(NVL(pl.plannedqty,0) * NVL(pl.plannedprice,0)) AS Price,pl.M_AttributeSetInstance_ID " +
                    " FROM C_Project p " +
                    "INNER JOIN C_ProjectLine pl ON p.C_Project_ID = pl.C_Project_ID" +
                    " WHERE p.c_order_id IS NULL AND p.ref_order_id IS NULL AND c_period_id = " + C_Period_ID + " AND p.AD_Org_ID = " + mf.GetAD_Org_ID() +
                    " AND C_ProjectLine_ID NOT IN (SELECT C_ProjectLine_ID FROM va073_masterforecastlinedetail WHERE " +
                    "AD_Org_ID = " + mf.GetAD_Org_ID() + " AND C_Period_ID=" + C_Period_ID + ") AND NVL(pl.M_Product_ID,0)>0 ";


                sql = MRole.GetDefault(GetCtx()).AddAccessSQL(sql, "C_Project", true, true); // fully qualified - RO

                dsOpp = new DataSet();
                dsOpp = DB.ExecuteDataset(sql, null, mf.Get_Trx());
                if (dsOpp != null && dsOpp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsOpp.Tables[0].Rows.Count; i++)
                    {
                        //Create MasterForecastline
                        mfLine = GenerateMasterForecast(Util.GetValueOfInt(dsOpp.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(dsOpp.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), 0, 0, 0);
                        if (!mfLine.Save())
                        {
                            ValueNamePair vp = VLogger.RetrieveError();
                            if (vp != null)
                            {
                                log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "MasterForecastLineNotSaved") + vp.GetValue() + " - " + vp.GetName());
                            }
                            else
                            {
                                log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "MasterForecastLineNotSaved"));
                            }

                        }
                        else
                        {
                            LineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(LineNo), 0)+10  FROM VA073_MasterForecastLineDetail WHERE C_MasterForecastLine_ID=" + mfLine.GetC_MasterForecastLine_ID(), null, mf.Get_Trx()));

                            //Conversion from BaseUOM to UOM on Project Line
                            OppQty = MUOMConversion.ConvertProductFrom(mf.GetCtx(), Util.GetValueOfInt(dsOpp.Tables[0].Rows[i]["M_Product_ID"]),
                                Util.GetValueOfInt(dsOpp.Tables[0].Rows[i]["C_UOM_ID"]), Util.GetValueOfDecimal(dsOpp.Tables[0].Rows[i]["plannedqty"]));
                            if (OppQty == null)
                            {
                                OppQty = Util.GetValueOfDecimal(dsOpp.Tables[0].Rows[i]["plannedqty"]);
                            }

                            //Convert Line Amount as per Currency Defined ON  Master Forecast 
                            ConvertedAmt = MConversionRate.Convert(mf.GetCtx(), Util.GetValueOfDecimal(dsOpp.Tables[0].Rows[i]["Price"]),
                            Util.GetValueOfInt(dsOpp.Tables[0].Rows[i]["C_Currency_ID"]), Currency,
                            Util.GetValueOfDateTime(mf.Get_Value("TRXDATE")),
                            Util.GetValueOfInt(mf.Get_Value("C_ConversionType_ID")), mf.GetAD_Client_ID(), mf.GetAD_Org_ID());

                            //Create Product Line Details
                            po = GenerateProductLineDetails(mfLine, LineNo, 0, 0, Util.GetValueOfInt(dsOpp.Tables[0].Rows[i]["C_Project_ID"]),
                                Util.GetValueOfInt(dsOpp.Tables[0].Rows[i]["C_ProjectLine_ID"]), 0, 0,
                                C_Period_ID, Util.GetValueOfInt(dsOpp.Tables[0].Rows[i]["C_UOM_ID"]), Util.GetValueOfInt(dsOpp.Tables[0].Rows[i]["M_Product_ID"]),
                                OppQty, ConvertedAmt,
                                Util.GetValueOfInt(dsOpp.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]));
                            if (!po.Save())
                            {
                                ValueNamePair vp = VLogger.RetrieveError();
                                if (vp != null)
                                {
                                    log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "ProductLineDetailNotSaved") + " for ProjectLine " + Util.GetValueOfInt(dsOpp.Tables[0].Rows[i]["C_ProjectLine_ID"]) + vp.GetValue() + " - " + vp.GetName());
                                }
                                else
                                {
                                    log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "ProductLineDetailNotSaved") + " for ProjectLine " + Util.GetValueOfInt(dsOpp.Tables[0].Rows[i]["C_ProjectLine_ID"]));
                                }

                            }
                            else
                            {
                                Count++;
                                LineNo += 10;
                                //Update quantities AND price at Product line 
                                sql = "UPDATE c_masterforecastline SET " +
                                    "ForcastQty=(SELECT NVL(SUM(QtyEntered),0) FROM VA073_MasterForecastLineDetail WHERE NVL(C_Forecast_ID,0)>0 AND c_masterforecastline_ID=" + mfLine.GetC_MasterForecastLine_ID() + "), " +
                                    "OppQty=(SELECT NVL(SUM(QtyEntered),0) FROM VA073_MasterForecastLineDetail WHERE NVL(C_Project_ID,0)>0 AND c_masterforecastline_ID=" + mfLine.GetC_MasterForecastLine_ID() + "), " +
                                    "VA073_SalesOrderQty =(SELECT NVL(SUM(QtyEntered),0) FROM VA073_MasterForecastLineDetail WHERE NVL(C_Order_ID,0)>0 AND c_masterforecastline_ID=" + mfLine.GetC_MasterForecastLine_ID() + "), " +
                                    "TotalQty=(SELECT NVL(SUM(QtyEntered),0) FROM VA073_MasterForecastLineDetail WHERE  c_masterforecastline_ID=" + mfLine.GetC_MasterForecastLine_ID() + ") , " +
                                    "Price= (Round((SELECT NVL(SUM(price),0)/ NVL(SUM(QtyEntered),0) FROM VA073_MasterForecastLineDetail WHERE C_MasterForecastLine_ID=" + mfLine.GetC_MasterForecastLine_ID() + "), " +
                                     StdPrecision + ")), " +
                                    "PlannedRevenue =(ROUND((SELECT SUM(price) FROM VA073_MasterForecastLineDetail WHERE C_MasterForecastLine_ID=" + mfLine.GetC_MasterForecastLine_ID() + ")," + StdPrecision + "))" +
                                    " WHERE C_MasterForecastLine_ID=" + mfLine.GetC_MasterForecastLine_ID();

                                DB.ExecuteQuery(sql, null, mf.Get_Trx());
                            }
                        }

                    }
                }
                else
                {
                    log.Log(Level.INFO, Msg.GetMsg(GetCtx(), "NoRecordFoundOpportunity"));
                }
            }
            return Count;
        }

        /// <SUMmary>
        ///  Sales Order Products
        /// </SUMmary>
        /// <returns>No of lines created</returns>
        private int SalesOrderProducts()
        {
            sql = "SELECT ol.m_product_id,ol.QtyOrdered,M_AttributeSetInstance_ID,ol.C_UOM_ID," +
                 " ol.C_OrderLine_ID,o.C_Order_ID,(NVL(PriceEntered,0) * NVL(QtyEntered,0)) AS Price,o.C_Currency_ID FROM C_Order o " +
                 " INNER JOIN C_OrderLine ol ON o.C_Order_ID = ol.C_Order_ID " +
                 " INNER JOIN C_Doctype d ON o.c_DocTypeTarget_ID = d.C_Doctype_ID   " +
                 " WHERE d.DocBaseType='" + MDocBaseType.DOCBASETYPE_SALESORDER + "' " +
                 " AND d.DocSubTypeSo NOT IN ('" + MDocType.DOCSUBTYPESO_BlanketOrder + "','" + MDocType.DOCSUBTYPESO_Proposal + "')" +
                 " AND o.IsSOTrx='Y' AND o.IsReturnTrx='N' AND o.AD_Org_ID = " + mf.GetAD_Org_ID() +
                 " AND o.DateOrdered BETWEEN (SELECT startdate FROM C_Period WHERE C_Period_ID = " + C_Period_ID + ")  " +
                 " AND (SELECT enddate FROM C_Period WHERE C_Period_ID = " + C_Period_ID + ") AND ol.QtyOrdered > ol.QtyDelivered " +
                 " AND ol.C_OrderLine_ID NOT IN (SELECT C_OrderLine_ID FROM va073_masterforecastlinedetail WHERE " +
                 "AD_Org_ID = " + mf.GetAD_Org_ID() + " AND C_Period_ID=" + C_Period_ID + ") AND NVL(ol.M_Product_ID,0)>0 AND o.DocStatus IN('CO','CL') ";

            sql = MRole.GetDefault(GetCtx()).AddAccessSQL(sql, "C_Order", true, true); // fully qualified - RO

            dsOrder = new DataSet();
            dsOrder = DB.ExecuteDataset(sql, null, mf.Get_Trx());
            if (dsOrder != null && dsOrder.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsOrder.Tables[0].Rows.Count; i++)
                {
                    //create MasterForecastLine
                    mfLine = GenerateMasterForecast(Util.GetValueOfInt(dsOrder.Tables[0].Rows[i]["M_Product_ID"]),
                             Util.GetValueOfInt(dsOrder.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), 0, 0, 0);
                    if (!mfLine.Save())
                    {
                        ValueNamePair vp = VLogger.RetrieveError();
                        if (vp != null)
                        {
                            log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "MasterForecastLineNotSaved") + vp.GetValue() + " - " + vp.GetName());
                        }
                        else
                        {
                            log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "MasterForecastLineNotSaved"));
                        }

                    }
                    else
                    {
                        LineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(LineNo), 0)+10  FROM VA073_MasterForecastLineDetail WHERE C_MasterForecastLine_ID=" + mfLine.GetC_MasterForecastLine_ID(), null, mf.Get_Trx()));
                        //Convert Line Amount as per Currency Defined ON  Master Forecast 
                        ConvertedAmt = MConversionRate.Convert(mf.GetCtx(), Util.GetValueOfDecimal(dsOrder.Tables[0].Rows[i]["Price"]),
                            Util.GetValueOfInt(dsOrder.Tables[0].Rows[i]["C_Currency_ID"]), Currency,
                            Util.GetValueOfDateTime(mf.Get_Value("TRXDATE")),
                            Util.GetValueOfInt(mf.Get_Value("C_ConversionType_ID")), mf.GetAD_Client_ID(), mf.GetAD_Org_ID());

                        //Create Product Line Details
                        po = GenerateProductLineDetails(mfLine, LineNo, Util.GetValueOfInt(dsOrder.Tables[0].Rows[i]["C_Order_ID"]),
                               Util.GetValueOfInt(dsOrder.Tables[0].Rows[i]["C_OrderLine_ID"]), 0, 0, 0, 0,
                               C_Period_ID, Util.GetValueOfInt(dsOrder.Tables[0].Rows[i]["C_UOM_ID"]), Util.GetValueOfInt(dsOrder.Tables[0].Rows[i]["M_Product_ID"]),
                               Util.GetValueOfDecimal(dsOrder.Tables[0].Rows[i]["QtyOrdered"]), ConvertedAmt,
                               Util.GetValueOfInt(dsOrder.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]));
                        if (!po.Save())
                        {
                            ValueNamePair vp = VLogger.RetrieveError();
                            if (vp != null)
                            {
                                log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "ProductLineDetailNotSaved") + "for OrderLine" + Util.GetValueOfInt(dsOrder.Tables[0].Rows[i]["C_OrderLine_ID"])
                                    + vp.GetValue() + " - " + vp.GetName());
                            }
                            else
                            {
                                log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "ProductLineDetailNotSaved") + "for OrderLine" + Util.GetValueOfInt(dsOrder.Tables[0].Rows[i]["C_OrderLine_ID"]));
                            }

                        }
                        else
                        {
                            Count++;
                            LineNo += 10;
                            //Update quantities AND price at Product line 
                            sql = "UPDATE c_masterforecastline SET " +
                                    "ForcastQty=(SELECT NVL(SUM(QtyEntered),0) FROM VA073_MasterForecastLineDetail WHERE NVL(C_Forecast_ID,0)>0 AND c_masterforecastline_ID=" + mfLine.GetC_MasterForecastLine_ID() + "), " +
                                    "OppQty=(SELECT NVL(SUM(QtyEntered),0) FROM VA073_MasterForecastLineDetail WHERE NVL(C_Project_ID,0)>0 AND c_masterforecastline_ID=" + mfLine.GetC_MasterForecastLine_ID() + "), " +
                                    "VA073_SalesOrderQty =(SELECT NVL(SUM(QtyEntered),0) FROM VA073_MasterForecastLineDetail WHERE NVL(C_Order_ID,0)>0 AND c_masterforecastline_ID=" + mfLine.GetC_MasterForecastLine_ID() + "), " +
                                    "TotalQty=(SELECT NVL(SUM(QtyEntered),0) FROM VA073_MasterForecastLineDetail WHERE  c_masterforecastline_ID=" + mfLine.GetC_MasterForecastLine_ID() + ") , " +
                                    "Price= (Round((SELECT NVL(SUM(price),0)/ NVL(SUM(QtyEntered),0) FROM VA073_MasterForecastLineDetail WHERE C_MasterForecastLine_ID=" + mfLine.GetC_MasterForecastLine_ID() + "), " +
                                     StdPrecision + ")), " +
                                    "PlannedRevenue =(ROUND((SELECT SUM(price) FROM VA073_MasterForecastLineDetail WHERE C_MasterForecastLine_ID=" + mfLine.GetC_MasterForecastLine_ID() + ")," + StdPrecision + "))" +
                                    " WHERE C_MasterForecastLine_ID=" + mfLine.GetC_MasterForecastLine_ID();

                            DB.ExecuteQuery(sql, null, mf.Get_Trx());

                        }
                    }

                }
            }
            else
            {
                log.Log(Level.INFO, Msg.GetMsg(GetCtx(), "NoRecordFoundSalesOrder"));
            }
            return Count;
        }

        /// <SUMmary>
        /// Team Forecast Products
        /// </SUMmary>
        /// <returns>No of lines created</returns>
        private int TeamForecastProduct()
        {

            sql = @"SELECT fl.M_Product_ID,fl.M_AttributeSetInstance_ID,fl.qtyentered,fl.BaseQty,f.C_Forecast_ID,
                    C_ForecastLine_ID,f.C_Period_ID,fl.C_UOM_ID,NVL(pricestd,0) AS Price,f.C_Currency_ID
                    FROM C_Forecast f " +
                    " INNER JOIN C_Forecastline fl ON fl.c_forecast_id = f.c_forecast_id " +
                    " WHERE f.c_period_id = " + C_Period_ID + " AND f.AD_Org_ID = " + mf.GetAD_Org_ID() +
                    " AND f.isactive = 'Y' AND f.processed = 'Y'" +
                    " AND C_ForecastLine_ID NOT IN (SELECT C_ForecastLine_ID FROM VA073_MasterForecastlinedetail WHERE " +
                    "AD_Org_ID = " + mf.GetAD_Org_ID() + " AND C_Period_ID=" + C_Period_ID + ") AND NVL(fl.M_Product_ID,0)>0 ";

            sql = MRole.GetDefault(mf.GetCtx()).AddAccessSQL(sql, "C_Forecast", true, true); // fully qualified - RO

            dsForecast = new DataSet();
            dsForecast = DB.ExecuteDataset(sql, null, mf.Get_Trx());
            if (dsForecast != null && dsForecast.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsForecast.Tables[0].Rows.Count; i++)
                {
                    //Create MasterForecastLine
                    mfLine = GenerateMasterForecast(Util.GetValueOfInt(dsForecast.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(dsForecast.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), 0, 0, 0);
                    if (!mfLine.Save())
                    {
                        ValueNamePair vp = VLogger.RetrieveError();
                        if (vp != null)
                        {
                            log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "MasterForecastLineNotSaved") + vp.GetValue() + " - " + vp.GetName());
                        }
                        else
                        {
                            log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "MasterForecastLineNotSaved"));
                        }

                    }
                    else
                    {
                        LineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(LineNo), 0)+10  FROM VA073_MasterForecastLineDetail WHERE C_MasterForecastLine_ID=" + mfLine.GetC_MasterForecastLine_ID(), null, mf.Get_Trx()));
                        //Convert Line Amount as per Currency Defined ON  Master Forecast 
                        ConvertedAmt = MConversionRate.Convert(mf.GetCtx(), Util.GetValueOfDecimal(dsForecast.Tables[0].Rows[i]["Price"]),
                            Util.GetValueOfInt(dsForecast.Tables[0].Rows[i]["C_Currency_ID"]), Currency,
                            Util.GetValueOfDateTime(mf.Get_Value("TRXDATE")),
                            Util.GetValueOfInt(mf.Get_Value("C_ConversionType_ID")), mf.GetAD_Client_ID(), mf.GetAD_Org_ID());

                        //Create Product Line Details
                        po = GenerateProductLineDetails(mfLine, LineNo, 0, 0, 0, 0, Util.GetValueOfInt(dsForecast.Tables[0].Rows[i]["C_Forecast_ID"]),
                               Util.GetValueOfInt(dsForecast.Tables[0].Rows[i]["C_ForecastLine_ID"]),
                               Util.GetValueOfInt(dsForecast.Tables[0].Rows[i]["C_Period_ID"]), mfLine.GetC_UOM_ID(),
                               Util.GetValueOfInt(dsForecast.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfDecimal(dsForecast.Tables[0].Rows[i]["qtyentered"]),
                               ConvertedAmt, Util.GetValueOfInt(dsForecast.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]));
                        if (!po.Save())
                        {
                            ValueNamePair vp = VLogger.RetrieveError();
                            if (vp != null)
                            {
                                log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "ProductLineDetailNotSaved") + " for ForecastLine " + Util.GetValueOfInt(dsForecast.Tables[0].Rows[i]["C_ForecastLine_ID"]) + " " + vp.GetValue() + " - " + vp.GetName());
                            }
                            else
                            {
                                log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "ProductLineDetailNotSaved") + " for ForecastLine " + Util.GetValueOfInt(dsForecast.Tables[0].Rows[i]["C_ForecastLine_ID"]));
                            }

                        }
                        else
                        {
                            //Update quantities AND Price at Product line
                            Count++;
                            LineNo += 10;
                            sql = "UPDATE c_masterforecastline SET " +
                                    "ForcastQty=(SELECT NVL(SUM(QtyEntered),0) FROM VA073_MasterForecastLineDetail WHERE NVL(C_Forecast_ID,0)>0 AND c_masterforecastline_ID=" + mfLine.GetC_MasterForecastLine_ID() + "), " +
                                    "OppQty=(SELECT NVL(SUM(QtyEntered),0) FROM VA073_MasterForecastLineDetail WHERE NVL(C_Project_ID,0)>0 AND c_masterforecastline_ID=" + mfLine.GetC_MasterForecastLine_ID() + "), " +
                                    "VA073_SalesOrderQty =(SELECT NVL(SUM(QtyEntered),0) FROM VA073_MasterForecastLineDetail WHERE NVL(C_Order_ID,0)>0 AND c_masterforecastline_ID=" + mfLine.GetC_MasterForecastLine_ID() + "), " +
                                    "TotalQty=(SELECT NVL(SUM(QtyEntered),0) FROM VA073_MasterForecastLineDetail WHERE  c_masterforecastline_ID=" + mfLine.GetC_MasterForecastLine_ID() + ") , " +
                                    "Price= (Round((SELECT NVL(SUM(price),0)/ NVL(SUM(QtyEntered),0) FROM VA073_MasterForecastLineDetail WHERE C_MasterForecastLine_ID=" + mfLine.GetC_MasterForecastLine_ID() + "), " +
                                     StdPrecision + ")), " +
                                    "PlannedRevenue =(ROUND((SELECT SUM(price) FROM VA073_MasterForecastLineDetail WHERE C_MasterForecastLine_ID=" + mfLine.GetC_MasterForecastLine_ID() + ")," + StdPrecision + "))" +
                                    " WHERE C_MasterForecastLine_ID=" + mfLine.GetC_MasterForecastLine_ID();

                            DB.ExecuteQuery(sql, null, mf.Get_Trx());

                        }
                    }

                }
            }
            else
            {
                log.Log(Level.INFO, Msg.GetMsg(GetCtx(), "NoRecordFoundForecast"));
            }
            return Count;
        }

        /// <SUMmary>
        /// Create Master Forecast
        /// </SUMmary>
        /// <param name="M_Product_ID">Product </param>
        /// <param name="totalQtyTeam">Forecast Qty</param>
        /// <param name="totalQtyOpp">Opportunity Qty</param>
        /// <param name="avgPrice">Price</param>
        /// <param name="totalQtySO">Total </param>
        /// <returns>object</returns>
        private MMasterForecastLine GenerateMasterForecast(int M_Product_ID, int M_AttributeSetInstance, decimal? totalQtyTeam, Decimal? totalQtyOpp, decimal? avgPrice)
        {
            MMasterForecastLine mfLine = MMasterForecastLine.GetOrCreate(mf.GetCtx(),mf.Get_Trx(),mf.GetC_MasterForecast_ID(), M_Product_ID,0, M_AttributeSetInstance,"");
            Decimal? qty = mfLine.GetOppQty();
            mfLine.SetC_MasterForecast_ID(mf.GetC_MasterForecast_ID());
            if (totalQtyOpp > 0 && totalQtyTeam > 0)
            {
                mfLine.SetForcastQty(totalQtyTeam+mfLine.GetForcastQty());
                mfLine.SetOppQty(totalQtyOpp);
            }
            else
            {
                mfLine.SetForcastQty(totalQtyTeam );
                mfLine.SetOppQty(totalQtyOpp + mfLine.GetOppQty());
            }
            //Decimal? totalqty = Decimal.Add(mfLine.GetForcastQty(), mfLine.GetOppQty());
            mfLine.SetTotalQty(mfLine.GetForcastQty()+mfLine.GetOppQty());
            //calculate average price for opportunit case only 
            if (totalQtyOpp>0 && totalQtyTeam==0) 
            {
                avgPrice = ((mfLine.GetPrice() * qty) + (avgPrice * totalQtyOpp)) / mfLine.GetTotalQty();
                avgPrice = Decimal.Round(avgPrice.Value, StdPrecision, MidpointRounding.AwayFromZero);
                mfLine.SetPrice(avgPrice); 
            }
            //Team Forecast case
            else
            {
                mfLine.SetPrice(avgPrice);
            }
            Decimal? planRevenue = Decimal.Round(Decimal.Multiply(mfLine.GetPrice(), mfLine.GetTotalQty()), StdPrecision, MidpointRounding.AwayFromZero);
            mfLine.SetPlannedRevenue(planRevenue);

            return mfLine;

        }

        /// <SUMmary>
        /// Create Product Line details 
        /// </SUMmary>
        /// <param name="Parent">C_MasterForecastLine</param>
        /// <param name="Order_ID">Sales Order</param>
        /// <param name="OrderLine_ID">Sales Order line</param>
        /// <param name="Opportunity_ID">Opportunity</param>
        /// <param name="OppLine_ID">Opportunity Line</param>
        /// <param name="Forecast">Team Forecast</param>
        /// <param name="ForecastLine_ID">Forecast Line</param>
        /// <param name="C_Period_ID">Period</param>
        /// <param name="UOM_ID">UOM</param>
        /// <param name="Product_ID">Product</param>
        /// <param name="Quantity">Qunatity</param>
        /// <param name="Price">Price</param>
        /// <param name="M_AttributeSetInstance_ID">Attribute Set Instance</param>
        /// <returns>Product line detail Object</returns>
        private PO GenerateProductLineDetails(MMasterForecastLine Parent, int LineNo, int Order_ID, int OrderLine_ID, int Opportunity_ID, int OppLine_ID, int Forecast, int ForecastLine_ID, int C_Period_ID, int UOM_ID, int Product_ID, Decimal? Quantity, Decimal? Price, int M_AttributeSetInstance_ID)
        {
            //object of VA073_MasterForecastLineDetails
            po = tbl.GetPO(mf.GetCtx(), 0, mf.Get_Trx());
            //LineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(LineNo), 0) AS DefaultValue FROM VA073_MasterForecastLineDetail WHERE C_MasterForecastLine_ID=" + Parent.GetC_MasterForecastLine_ID(), null, mf.Get_Trx()));
            po.SetAD_Client_ID(Parent.GetAD_Client_ID());
            po.SetAD_Org_ID(Parent.GetAD_Org_ID());
            po.Set_Value("LineNo", LineNo);
            po.Set_Value("C_MasterForecastLine_ID", Parent.GetC_MasterForecastLine_ID());
            po.Set_Value("C_Order_ID", Order_ID);
            po.Set_Value("C_OrderLine_ID", OrderLine_ID);
            po.Set_Value("C_Project_ID", Opportunity_ID);
            po.Set_Value("C_ProjectLine_ID", OppLine_ID);
            po.Set_Value("C_Forecast_ID", Forecast);
            po.Set_Value("C_ForecastLine_ID", ForecastLine_ID);
            po.Set_Value("C_Period_ID", C_Period_ID);
            po.Set_Value("C_UOM_ID", UOM_ID);
            po.Set_Value("M_Product_ID", Product_ID);
            po.Set_Value("QtyEntered", Quantity);
            po.Set_Value("Price", Price);
            po.Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);

            return po;

        }
    }
}

