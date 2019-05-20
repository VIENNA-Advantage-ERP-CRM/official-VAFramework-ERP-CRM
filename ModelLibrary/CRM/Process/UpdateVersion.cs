/********************************************************
 * Project Name   : ViennaAdvantage
 * Class Name     : UpdateVersion
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

namespace VAdvantage.Process
{
    public class UpdateVersion : SvrProcess
    {
        #region Private Variables
        string msg = "";
        string sql = "";
        int C_Period_ID = 0;
        X_C_MasterForecast mf = null;
        X_C_MasterForecast mFor = null;
        bool _includeOpp = false;
        #endregion

        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    continue;
                }
                else if (name.Equals("IsIncludeOpp"))
                {
                    _includeOpp = "Y".Equals(Util.GetValueOfString(para[i].GetParameter()));
                }
                else
                {
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string DoIt()
        {
            mf = new X_C_MasterForecast(GetCtx(), GetRecord_ID(), null);
            if (mf.IsProcessed())
            {
                C_Period_ID = mf.GetC_Period_ID();
                sql = "select * from c_masterforecast where c_period_id = " + C_Period_ID + " and ad_client_ID = " + GetCtx().GetAD_Client_ID();
                IDataReader idr = null;
                try
                {
                    //  bool opp = false;
                    idr = DB.ExecuteReader(sql, null, null);
                    while (idr.Read())
                    {
                        sql = "update c_masterforecast set currentversion = 'N' where ad_client_id = " + GetCtx().GetAD_Client_ID() + " and c_masterforecast_id = " + Util.GetValueOfInt(idr["C_MasterForecast_ID"]);
                        int res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
                        //string check = Util.GetValueOfString(idr["IsIncludeOpp"]);
                        //if (check == "Y")
                        //{
                        //    opp = true;
                        //}
                    }
                    if (_includeOpp)
                    {
                        CreateMasterForecast(_includeOpp);
                        GenerateLines();
                    }
                    else
                    {
                        CreateMasterForecast(_includeOpp);
                        GenerateLines();
                    }
                    msg = Msg.GetMsg(GetCtx(), "NewVersionIsCreatedSuccessfully");
                }
                catch
                {

                }
                if (idr != null)
                    idr.Close();
            }
            else
            {
                msg = Msg.GetMsg("ProcessRecordFirst", "ProcessRecordFirst");
            }

            return msg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mf"></param>
        /// <param name="opp"></param>
        private void CreateMasterForecast(bool opp)
        {
            mFor = new X_C_MasterForecast(GetCtx(), 0, null);
            mFor.SetAD_Client_ID(mf.GetAD_Client_ID());
            mFor.SetAD_Org_ID(mf.GetAD_Org_ID());
            mFor.SetC_Period_ID(C_Period_ID);
            mFor.SetCurrentVersion(true);
            if (opp)
            {
                mFor.SetIsIncludeOpp(true);
            }
            else
            {
                mFor.SetIsIncludeOpp(false);
            }
            string name = mf.GetName().Substring(0, mf.GetName().IndexOf('_') + 1);
            string date = System.DateTime.Now.ToString("dd-MMM-yyyy");
            if (name == "")
            {
                mFor.SetName(mf.GetName() + "_" + date);
            }
            else
            {
                mFor.SetName(name + date);
            }
            if (!mFor.Save())
            {
                log.SaveError("MasterForecastNotSaved", "MasterForecastNotSaved");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void GenerateLines()
        {
            if (C_Period_ID != 0)
            {
                sql = "select Distinct(M_Product_ID) from c_forecastline fl inner join c_forecast f on (fl.c_forecast_id = f.c_forecast_id) where f.c_period_id = " + C_Period_ID + " and f.ad_client_id = " + GetCtx().GetAD_Client_ID() + " and f.isactive = 'Y' and f.processed = 'Y'";
                IDataReader idr = null;
                try
                {

                    idr = DB.ExecuteReader(sql, null, null);
                    while (idr.Read())
                    {
                        Decimal? totalQtyTeam = 0;
                        Decimal? totalPriceTeam = 0;
                        Decimal? totalQtyOpp = 0;
                        Decimal? totalPriceOpp = 0;
                        sql = "select SUM(nvl(qtyentered,0)) from c_forecastline where isactive = 'Y' and m_product_id = " + Util.GetValueOfInt(idr[0]) + " and processed = 'Y'";
                        totalQtyTeam = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                        // sql = "select SUM(nvl(qtyentered,0) * nvl(pricestd,0)) from c_forecastline where m_product_id = " + Util.GetValueOfInt(idr[0]) + " and processed = 'Y'";
                        sql = "select SUM(nvl(pricestd,0)) from c_forecastline where isactive = 'Y' and m_product_id = " + Util.GetValueOfInt(idr[0]) + " and processed = 'Y'";
                        totalPriceTeam = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));

                        if (mFor.IsIncludeOpp())
                        {
                            sql = "select sum(nvl(pl.plannedqty,0))  FROM c_projectline pl inner join c_project p on (p.c_project_id = pl.c_project_id) "
                                + " WHERE pl.planneddate BETWEEN (SELECT startdate FROM c_period WHERE c_period_id = " + C_Period_ID + ") "
                                + " AND (SELECT enddate FROM c_period WHERE c_period_id = " + C_Period_ID + ") AND pl.m_product_id =  " + Util.GetValueOfInt(idr[0]) + " and p.c_order_id is null and p.ref_order_id is null and pl.isactive = 'Y'";
                            totalQtyOpp = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                            sql = " SELECT SUM(NVL(pl.plannedqty,0) * NVL(pl.plannedprice,0)) FROM c_projectline pl inner join c_project p on (p.c_project_id = pl.c_project_id) "
                                + " WHERE pl.planneddate BETWEEN (SELECT startdate FROM c_period WHERE c_period_id = " + C_Period_ID + ") "
                                + " AND (SELECT enddate FROM c_period WHERE c_period_id = " + C_Period_ID + ") AND pl.m_product_id =  " + Util.GetValueOfInt(idr[0]) + " and p.c_order_id is null and p.ref_order_id is null and pl.isactive = 'Y'";
                            totalPriceOpp = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                        }

                        Decimal? totalPrice = Decimal.Add(totalPriceTeam.Value, totalPriceOpp.Value);
                        Decimal? totalQty = Decimal.Add(totalQtyTeam.Value, totalQtyOpp.Value);

                        if (totalQty.Value > 0)
                        {
                            Decimal? avgPrice = Decimal.Divide(totalPrice.Value, totalQty.Value);
                            avgPrice = Decimal.Round(avgPrice.Value, 2, MidpointRounding.AwayFromZero);

                            GenerateMasterForecast(Util.GetValueOfInt(idr[0]), totalQtyTeam, totalQtyOpp, avgPrice);
                        }
                    }
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }
                catch
                {
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }
                if (mFor.IsIncludeOpp())
                {
                    OnlyOpportunityProducts();
                }
                mFor.SetCurrentVersion(true);
                // mFor.SetProcessed(true);
                if (!mFor.Save())
                {
                    log.SaveError("MasterForecastNotSaved", "MasterForecastNotSaved");
                }
            }
        }

        private void OnlyOpportunityProducts()
        {
            //sql = " SELECT distinct(pl.m_product_id) FROM c_projectline pl INNER JOIN c_project p ON p.c_project_id = pl.c_project_id WHERE p.c_order_id IS NULL"
            //    + " AND p.ref_order_id IS NULL AND pl.m_product_id NOT IN (SELECT DISTINCT(M_Product_ID) FROM c_forecastline fl "
            //    + " INNER JOIN c_forecast f ON (fl.c_forecast_id = f.c_forecast_id) WHERE f.c_period_id = " + C_Period_ID
            //    + " AND f.ad_client_id = " + GetCtx().GetAD_Client_ID() + " AND fl.isactive = 'Y')";

            sql = " SELECT distinct(pl.m_product_id) FROM c_projectline pl INNER JOIN c_project p ON p.c_project_id = pl.c_project_id WHERE p.c_order_id IS NULL"
               + " AND p.ref_order_id IS NULL AND pl.m_product_id NOT IN (select m_product_id from c_masterforecastline where isactive = 'Y' and c_masterforecast_id = " + mFor.GetC_MasterForecast_ID() + ")";

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    Decimal? totalQtyOpp = 0;
                    Decimal? totalPriceOpp = 0;
                    sql = "select sum(nvl(pl.plannedqty,0))  FROM c_projectline pl inner join c_project p on (p.c_project_id = pl.c_project_id) "
                           + " WHERE pl.planneddate BETWEEN (SELECT startdate FROM c_period WHERE c_period_id = " + C_Period_ID + ") "
                           + " AND (SELECT enddate FROM c_period WHERE c_period_id = " + C_Period_ID + ") AND pl.m_product_id =  " + Util.GetValueOfInt(idr[0]) + " and p.c_order_id is null and p.ref_order_id is null and pl.isactive = 'Y'";
                    totalQtyOpp = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                    sql = " SELECT SUM(NVL(pl.plannedqty,0) * NVL(pl.plannedprice,0)) FROM c_projectline pl inner join c_project p on (p.c_project_id = pl.c_project_id) "
                        + " WHERE pl.planneddate BETWEEN (SELECT startdate FROM c_period WHERE c_period_id = " + C_Period_ID + ") "
                        + " AND (SELECT enddate FROM c_period WHERE c_period_id = " + C_Period_ID + ") AND pl.m_product_id =  " + Util.GetValueOfInt(idr[0]) + " and p.c_order_id is null and p.ref_order_id is null and pl.isactive = 'Y'";
                    totalPriceOpp = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));

                    if (totalQtyOpp.Value > 0)
                    {
                        Decimal? avgPrice = Decimal.Divide(totalPriceOpp.Value, totalQtyOpp.Value);
                        avgPrice = Decimal.Round(avgPrice.Value, 2, MidpointRounding.AwayFromZero);

                        GenerateMasterForecast(Util.GetValueOfInt(idr[0]), Util.GetValueOfDecimal(Decimal.Zero), totalQtyOpp, avgPrice);
                    }
                }
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            catch
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
        }

        private void GenerateMasterForecast(int M_Product_ID, decimal? totalQtyTeam, Decimal? totalQtyOpp, decimal? avgPrice)
        {
            sql = "select c_uom_id from m_product where m_product_id = " + M_Product_ID;
            //X_C_MasterForecastLine mfLine = new X_C_MasterForecastLine(GetCtx(), 0, null);
            MMasterForecastLine mfLine = new MMasterForecastLine(GetCtx(), 0, null);
            mfLine.SetAD_Client_ID(mf.GetAD_Client_ID());
            mfLine.SetAD_Org_ID(mf.GetAD_Org_ID());
            mfLine.SetM_Product_ID(M_Product_ID);
            mfLine.SetC_MasterForecast_ID(mFor.GetC_MasterForecast_ID());
            mfLine.SetForcastQty(totalQtyTeam);
            mfLine.SetOppQty(totalQtyOpp);
            Decimal? total = Decimal.Add(totalQtyOpp.Value, totalQtyTeam.Value);
            mfLine.SetTotalQty(total);
            mfLine.SetPrice(avgPrice);
            mfLine.SetC_UOM_ID(Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)));
            Decimal? planRevenue = Decimal.Round(Decimal.Multiply(avgPrice.Value, total.Value), 2, MidpointRounding.AwayFromZero);
            mfLine.SetPlannedRevenue(planRevenue);
            //   mfLine.SetProcessed(true);
            if (!mfLine.Save())
            {
                log.SaveError("MasterForecastLineNotSaved", "MasterForecastLineNotSaved");
            }
        }
    }
}
