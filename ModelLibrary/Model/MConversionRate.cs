/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MConversionRate
 * Purpose        : Currency Conversion Rate Model
 * Class Used     : X_C_Conversion_Rate
 * Chronological    Development
 * Raghunandan      28-04-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Collections;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Windows.Forms;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MConversionRate : X_C_Conversion_Rate
    {
        TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        //Logger					
        private static VLogger _log = VLogger.GetVLogger(typeof(MConversionRate).FullName);

        /// <summary>
        ///Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Conversion_Rate_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MConversionRate(Ctx ctx, int C_Conversion_Rate_ID, Trx trxName)
            : base(ctx, C_Conversion_Rate_ID, trxName)
        {
            if (C_Conversion_Rate_ID == 0)
            {
                //	setC_Conversion_Rate_ID (0);
                //	setC_Currency_ID (0);
                //	setC_Currency_To_ID (0);
                base.SetDivideRate(Env.ZERO);
                base.SetMultiplyRate(Env.ZERO);
                //SetValidFrom(new DateTime(CommonFunctions.CurrentTimeMillis()));
                SetValidFrom(DateTime.Now.Date);
            }
            UpdateFromServer = true;
        }

        /// <summary>
        ///	Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MConversionRate(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            UpdateFromServer = true;
        }

        /// <summary>
        ///New Constructor
        /// </summary>
        /// <param name="po">parent</param>
        /// <param name="C_ConversionType_ID">conversion type</param>
        /// <param name="C_Currency_ID">currency to</param>
        /// <param name="C_Currency_To_ID"></param>
        /// <param name="MultiplyRate">multiply rate</param>
        /// <param name="ValidFrom">valid from</param>
        public MConversionRate(PO po, int C_ConversionType_ID, int C_Currency_ID, int C_Currency_To_ID,
            Decimal multiplyRate, DateTime? validFrom)
            : this(po.GetCtx(), 0, po.Get_TrxName())
        {
            //this(po.getCtx(), 0, po.get_TrxName());
            SetClientOrg(po);
            SetC_ConversionType_ID(C_ConversionType_ID);
            SetC_Currency_ID(C_Currency_ID);
            SetC_Currency_To_ID(C_Currency_To_ID);
            SetMultiplyRate(multiplyRate);
            SetValidFrom(validFrom);
            UpdateFromServer = true;
        }

        /// <summary>
        /// Convert an amount to base Currency
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Amt">amount to be converted</param>
        /// <param name="CurFrom_ID">The C_Currency_ID FROM</param>
        /// <param name="ConvDate">conversion date - if null - use current date</param>
        /// <param name="C_ConversionType_ID">conversion rate type - if 0 - use Default</param>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">organization</param>
        /// <returns>converted amount</returns>
        public static Decimal ConvertBase(Ctx ctx, Decimal amt, int CurFrom_ID,
            DateTime? convDate, int C_ConversionType_ID, int AD_Client_ID, int AD_Org_ID)
        {
            return Convert(ctx, amt, CurFrom_ID, VAdvantage.Model.MClient.Get(ctx).GetC_Currency_ID(),
                convDate, C_ConversionType_ID, AD_Client_ID, AD_Org_ID);
        }

        /// <summary>
        ///Convert an amount with today's default rate
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Amt">amount to be converted</param>
        /// <param name="CurFrom_ID">The C_Currency_ID FROM</param>
        /// <param name="CurTo_ID">The C_Currency_ID TO</param>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">organization</param>
        /// <returns>converted amount</returns>
        public static Decimal Convert(Ctx ctx, decimal amt, int CurFrom_ID, int CurTo_ID,
            int AD_Client_ID, int AD_Org_ID)
        {
            return Convert(ctx, amt, CurFrom_ID, CurTo_ID, null, 0, AD_Client_ID, AD_Org_ID);
        }

        /// <summary>
        ///Convert an amount
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Amt">amount to be converted</param>
        /// <param name="CurFrom_ID">The C_Currency_ID FROM</param>
        /// <param name="CurTo_ID">The C_Currency_ID TO</param>
        /// <param name="ConvDate">conversion date - if null - use current date</param>
        /// <param name="C_ConversionType_ID">C_ConversionType_ID conversion rate type - if 0 - use Default</param>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">organization</param>
        /// <returns>converted amount or null if no rate</returns>
        public static Decimal Convert(Ctx ctx, Decimal amt, int CurFrom_ID, int CurTo_ID,
            DateTime? convDate, int C_ConversionType_ID,
            int AD_Client_ID, int AD_Org_ID)
        {
            //if (amt == null)
            //{
            //    throw new ArgumentException("Required parameter missing - Amt");
            //}
            if (CurFrom_ID == CurTo_ID || amt.Equals(Env.ZERO))
            {
                return amt;
            }
            //	Get Rate
            Decimal retValue = GetRate(CurFrom_ID, CurTo_ID, convDate, C_ConversionType_ID, AD_Client_ID, AD_Org_ID);
            //if (retValue == null)
            //{
            //    //return null;
            //    return retValue;
            //}
            //	Get Amount in Currency Precision
            retValue = Decimal.Multiply(retValue, amt);
            int stdPrecision = MCurrency.GetStdPrecision(ctx, CurTo_ID);
            if (Env.Scale(retValue) > stdPrecision)
            {
                retValue = Decimal.Round(retValue, stdPrecision, MidpointRounding.AwayFromZero);
            }
            return retValue;
        }

        /// <summary>
        ///Get Currency Conversion Rate
        /// </summary>
        /// <param name="CurFrom_ID">The C_Currency_ID FROM</param>
        /// <param name="CurTo_ID">The C_Currency_ID TO</param>
        /// <param name="ConvDate">The Conversion date - if null - use current date</param>
        /// <param name="ConversionType_ID">Conversion rate type - if 0 - use Default</param>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">organization</param>
        /// <returns>currency Rate or null</returns>
        public static Decimal GetRate(int CurFrom_ID, int CurTo_ID,
            DateTime? convDate, int ConversionType_ID, int AD_Client_ID, int AD_Org_ID)
        {
            string isFetchAllDateOrNot = "N";
            if (CurFrom_ID == CurTo_ID)
            {
                return Env.ONE;
            }
            //	Conversion Type
            int C_ConversionType_ID = ConversionType_ID;
            if (C_ConversionType_ID == 0)
                C_ConversionType_ID = MConversionType.GetDefault(AD_Client_ID);
            //	Conversion Date
            if (convDate == null)
            {
                // convDate = new DateTime(CommonFunctions.CurrentTimeMillis());
                convDate = System.DateTime.Now.Date;
            }

            //	Get Rate
            String sql = "SELECT MultiplyRate "
                + "FROM C_Conversion_Rate "
                + "WHERE C_Currency_ID=" + CurFrom_ID					//	#1
                + " AND C_Currency_To_ID=" + CurTo_ID					//	#2
                + " AND	C_ConversionType_ID=" + C_ConversionType_ID				//	#3
                + " AND " + DataBase.DB.TO_DATE(convDate, true) + " BETWEEN ValidFrom AND ValidTo"	//	#4	TRUNC (?) ORA-00932: inconsistent datatypes: expected NUMBER got TIMESTAMP
                + " AND AD_Client_ID IN (0," + AD_Client_ID + ")"				//	#5
                + " AND AD_Org_ID IN (0," + AD_Org_ID + ") "				//	#6
                + " AND IsActive = 'Y' "                                    // #7
                + "ORDER BY AD_Client_ID DESC, AD_Org_ID DESC, ValidFrom DESC";
            //decimal retValue = null;
            decimal? retValue = null;
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = Utility.Util.GetValueOfDecimal(dr[0].ToString()); 
                    //when record found then not to continue with another record
                    break;
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            // added By Amit
            // if fetch all date or not is true on Conversion type then ppick conversion rate of max previous date
            if (retValue == null)
            {
                try
                {
                    isFetchAllDateOrNot = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT IsFetchAllDateOrnot FROM C_ConversionType
                                     WHERE C_ConversionType_ID = " + C_ConversionType_ID, null, null));
                    if (isFetchAllDateOrNot == "Y")
                    {
                        //System took currencies rate from last updated rate, 
                        //when current date rate not defined and consider previous record is true on Conversion type
                        sql = @"SELECT MultiplyRate FROM C_Conversion_Rate WHERE C_Conversion_Rate_id = (
                              SELECT MAX(C_Conversion_Rate_id) keep (dense_rank last ORDER BY ValidFrom, C_Conversion_Rate_id)
                               FROM C_Conversion_Rate WHERE IsActive = 'Y' AND C_Currency_ID = " + CurFrom_ID + @"
                              AND C_Currency_To_ID   = " + CurTo_ID + @" 
                              AND C_ConversionType_ID= " + C_ConversionType_ID + @"
                              AND ValidFrom < " + DataBase.DB.TO_DATE(convDate, true) + @"
                              AND AD_Client_ID      IN (0," + AD_Client_ID + ")" + @"
                              AND AD_Org_ID         IN (0," + AD_Org_ID + "))";
                        retValue = null;
                        try
                        {
                            ds = DataBase.DB.ExecuteDataset(sql, null, null);
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                DataRow dr = ds.Tables[0].Rows[i];
                                retValue = Utility.Util.GetValueOfDecimal(dr[0].ToString());
                                //when record found then not to continue with another record
                                break;
                            }
                            ds = null;
                        }
                        catch (Exception e)
                        {
                            _log.Log(Level.SEVERE, sql, e);
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.Log(Level.SEVERE, sql, e);
                }
            }
            //end

            if (retValue == null)
            {
                _log.Info("Not found - CurFrom=" + CurFrom_ID
          + ", CurTo=" + CurTo_ID
          + ", " + convDate
          + ", Type=" + ConversionType_ID + (ConversionType_ID == C_ConversionType_ID ? "" : "->" + C_ConversionType_ID)
          + ", Client=" + AD_Client_ID
          + ", Org=" + AD_Org_ID);
                retValue = 0;
            }
            return retValue.Value;
        }

        /**
         * 	Callout
         *	@param MultiplyRateOld old value
         *	@param MultiplyRateNew new value
         *	@param windowNo windowNo
         */
        //@UICallout
        public void SetMultiplyRate(String multiplyRateOld, String multiplyRateNew, int windowNo)
        {
            SetMultiplyRate(ConvertToBigDecimal(multiplyRateNew));
        }

        /// <summary>
        /// Set Multiply Rate
        /// Sets also Divide Rate
        /// </summary>
        /// <param name="MultiplyRate">multiply rate</param>
        public new void SetMultiplyRate(Decimal? multiplyRate)
        {
            bool useNew = true;
            if (multiplyRate == null
                || Env.Signum(System.Convert.ToDecimal(multiplyRate)) == 0
                || (System.Convert.ToDecimal(multiplyRate)).CompareTo(Env.ONE) == 0)
            {
                base.SetDivideRate(Env.ONE);
                base.SetMultiplyRate(Env.ONE);
            }
            else if (!UpdateFromServer)
            {
                useNew = false;
            }
            else
            {
                object count = DB.ExecuteScalar("select count(*) FROM AD_Column where AD_Table_ID =(SELECT AD_Table_ID FROM AD_Table WHERE Lower(TableName)='c_conversiontype') AND Lower(ColumnName)='isautocalculate'");
                if (count != null || count != DBNull.Value && System.Convert.ToInt32(count) > 0)
                {
                    DataSet dsConversion = DB.ExecuteDataset(@"SELECT Surchargepercentage,Surchargevalue,CurrencyRateUpdateFrequency FROM c_conversiontype 
                                                           WHERE isautocalculate='Y' AND isactive   ='Y' AND C_ConversionType_id=" + GetC_ConversionType_ID());
                    if (dsConversion != null && dsConversion.Tables[0].Rows.Count > 0)
                    {
                        Decimal rate1 = 0;
                        Decimal rate2 = 0;
                        if (dsConversion.Tables[0].Rows[0]["Surchargepercentage"] != null && dsConversion.Tables[0].Rows[0]["Surchargepercentage"] != DBNull.Value
                                                        && System.Convert.ToDecimal(dsConversion.Tables[0].Rows[0]["Surchargepercentage"]) != 0)
                        {
                            rate1 = (System.Convert.ToDecimal(multiplyRate) + (System.Convert.ToDecimal(multiplyRate) * ((System.Convert.ToDecimal(dsConversion.Tables[0].Rows[0]["Surchargepercentage"]) * -1) / 100)));
                            if (System.Convert.ToDouble(rate1) != 0.0)	//	no divide by zero
                            {
                                rate2 = Decimal.Round(Decimal.Divide(1, System.Convert.ToDecimal(rate1)), 12);// MidpointRounding.AwayFromZero);
                                //rate2 = Decimal.Round(Decimal.Divide(1, System.Convert.ToDecimal(multiplyRate)), 12);// MidpointRounding.AwayFromZero);
                            }
                            //rate2 = (rate2 + rate2 * (System.Convert.ToDecimal(dsConversion.Tables[0].Rows[0]["Surchargepercentage"]) / 100));
                        }
                        else if (dsConversion.Tables[0].Rows[0]["Surchargevalue"] != null && dsConversion.Tables[0].Rows[0]["Surchargevalue"] != DBNull.Value
                            && System.Convert.ToDecimal(dsConversion.Tables[0].Rows[0]["Surchargevalue"]) != 0)
                        {
                            rate1 = (System.Convert.ToDecimal(multiplyRate) + (System.Convert.ToDecimal(dsConversion.Tables[0].Rows[0]["Surchargevalue"]) * -1));
                            if (System.Convert.ToDouble(rate1) != 0.0)	//	no divide by zero
                            {
                                rate2 = Decimal.Round(Decimal.Divide(1, System.Convert.ToDecimal(rate1)), 12);// MidpointRounding.AwayFromZero);
                                //rate2 = Decimal.Round(Decimal.Divide(1, System.Convert.ToDecimal(multiplyRate)), 12);// MidpointRounding.AwayFromZero);
                            }
                            //rate2 = (rate2 + System.Convert.ToDecimal(dsConversion.Tables[0].Rows[0]["Surchargevalue"]));
                        }
                        else
                        {
                            rate1 = System.Convert.ToDecimal(multiplyRate);
                            if (System.Convert.ToDouble(rate1) != 0.0)	//	no divide by zero
                            {
                                rate2 = Decimal.Round(Decimal.Divide(1, System.Convert.ToDecimal(multiplyRate)), 12);// MidpointRounding.AwayFromZero);
                            }
                        }
                        base.SetMultiplyRate(rate1);
                        base.SetDivideRate(rate2);
                    }
                    else
                    {
                        useNew = false;
                    }
                }
                else
                {
                    useNew = false;
                }


            }
            if (!useNew)
            {
                base.SetMultiplyRate(System.Convert.ToDecimal(multiplyRate));
                if (UpdateFromServer)
                {
                    double dd = System.Convert.ToDouble((1 / System.Convert.ToDouble(multiplyRate)));
                    base.SetDivideRate(System.Convert.ToDecimal(dd));
                }
            }
        }

        /**
         * 	Callout
         *	@param DivideRateOld old value
         *	@param DivideRateNew new value
         *	@param windowNo window no
         */
        //@UICallout
        public void SetDivideRate(String DivideRateOld, String DivideRateNew, int WindowNo)
        {
            SetDivideRate(ConvertToBigDecimal(DivideRateNew));
        }

        /// <summary>
        /// Set Divide Rate.
        /// Sets also Multiply Rate
        /// </summary>
        /// <param name="DivideRate">divide rate</param>
        public new void SetDivideRate(Decimal? divideRate)
        {
            bool useNew = true;
            if (divideRate == null
                || Env.Signum(System.Convert.ToDecimal(divideRate)) == 0
                || ((Decimal)divideRate).CompareTo(Env.ONE) == 0)
            {
                base.SetDivideRate(Env.ONE);
                base.SetMultiplyRate(Env.ONE);
            }
            else if (!UpdateFromServer)
            {
                useNew = false;
            }
            else
            {

                object count = DB.ExecuteScalar("select count(*) FROM AD_Column where AD_Table_ID =(SELECT AD_Table_ID FROM AD_Table WHERE Lower(TableName)='c_conversiontype') AND Lower(ColumnName)='isautocalculate'");
                if (count != null || count != DBNull.Value && System.Convert.ToInt32(count) > 0)
                {
                    DataSet dsConversion = DB.ExecuteDataset(@"SELECT Surchargepercentage,Surchargevalue,CurrencyRateUpdateFrequency FROM c_conversiontype 
                                                           WHERE isautocalculate='Y' AND isactive   ='Y' AND C_ConversionType_id=" + GetC_ConversionType_ID());
                    if (dsConversion != null && dsConversion.Tables[0].Rows.Count > 0)
                    {
                        Decimal rate1 = 0;
                        Decimal rate2 = 0;
                        if (dsConversion.Tables[0].Rows[0]["Surchargepercentage"] != null && dsConversion.Tables[0].Rows[0]["Surchargepercentage"] != DBNull.Value
                                                        && System.Convert.ToDecimal(dsConversion.Tables[0].Rows[0]["Surchargepercentage"]) != 0)
                        {
                            rate1 = (System.Convert.ToDecimal(divideRate) + (System.Convert.ToDecimal(divideRate) * (System.Convert.ToDecimal(dsConversion.Tables[0].Rows[0]["Surchargepercentage"]) / 100)));
                            if (System.Convert.ToDouble(rate1) != 0.0)	//	no divide by zero
                            {
                                rate2 = Decimal.Round(Decimal.Divide(1, System.Convert.ToDecimal(divideRate)), 12);// MidpointRounding.AwayFromZero);
                            }
                            rate2 = (rate2 + rate2 * (System.Convert.ToDecimal(dsConversion.Tables[0].Rows[0]["Surchargepercentage"]) / 100));
                        }
                        else if (dsConversion.Tables[0].Rows[0]["Surchargevalue"] != null && dsConversion.Tables[0].Rows[0]["Surchargevalue"] != DBNull.Value
                            && System.Convert.ToDecimal(dsConversion.Tables[0].Rows[0]["Surchargevalue"]) != 0)
                        {
                            rate1 = (System.Convert.ToDecimal(divideRate) + System.Convert.ToDecimal(dsConversion.Tables[0].Rows[0]["Surchargevalue"]));
                            if (System.Convert.ToDouble(rate1) != 0.0)	//	no divide by zero
                            {
                                rate2 = Decimal.Round(Decimal.Divide(1, System.Convert.ToDecimal(divideRate)), 12);// MidpointRounding.AwayFromZero);
                            }
                            rate2 = (rate2 + System.Convert.ToDecimal(dsConversion.Tables[0].Rows[0]["Surchargevalue"]));
                        }
                        else
                        {
                            rate1 = System.Convert.ToDecimal(divideRate);
                            if (System.Convert.ToDouble(rate1) != 0.0)	//	no divide by zero
                            {
                                rate2 = Decimal.Round(Decimal.Divide(1, System.Convert.ToDecimal(divideRate)), 12);// MidpointRounding.AwayFromZero);
                            }
                        }
                        base.SetDivideRate(rate1);
                        base.SetMultiplyRate(rate2);
                    }
                    else
                    {
                        useNew = false;
                    }
                }
                else
                {
                    useNew = false;
                }


            }
            if (!useNew)
            {
                base.SetDivideRate(System.Convert.ToDecimal(divideRate));
                if (UpdateFromServer)
                {
                    double dd = System.Convert.ToDouble((1 / System.Convert.ToDecimal(divideRate)));
                    base.SetMultiplyRate(System.Convert.ToDecimal(dd));
                }
            }
        }

        /// <summary>
        ///String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MConversionRate[");
            sb.Append(Get_ID())
                .Append(",Currency=").Append(GetC_Currency_ID())
                .Append(",To=").Append(GetC_Currency_To_ID())
                .Append(", Multiply=").Append(GetMultiplyRate())
                .Append(",Divide=").Append(GetDivideRate())
                .Append(", ValidFrom=").Append(GetValidFrom());
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Before Save.
        /// - Same Currency
        /// - Date Range Check
        /// - Set To date to 2056
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <returns>true if OK to save</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //change by amit
            string sql = null;
            if (!newRecord)
            {
                //check any transaction occur for this currency
                // and currency to is available on AcctSchema
                // then not able to change that record
                if (!IsActive())
                {
                    return true;
                }
                else if (IsActive() != Util.GetValueOfBool(Get_ValueOld("IsActive"))
                    || GetC_ConversionType_ID() != Util.GetValueOfInt(Get_ValueOld("C_ConversionType_ID")) ||
                     GetC_Currency_To_ID() != Util.GetValueOfInt(Get_ValueOld("C_Currency_To_ID"))
                    )
                {
                    sql = @"SELECT COUNT(*) FROM C_Conversion_Rate WHERE IsActive = 'Y' " +
                         " AND c_currency_id = " + GetC_Currency_ID() + " AND C_Currency_To_ID = " + GetC_Currency_To_ID() +
                         " AND ( " + GlobalVariable.TO_DATE(GetValidFrom(), true) + " BETWEEN ValidFrom AND ValidTo" +
                         " OR " + GlobalVariable.TO_DATE(GetValidTo(), true) + " BETWEEN ValidFrom AND ValidTo )" +
                         " AND c_conversiontype_id = " + GetC_ConversionType_ID() + " AND AD_Client_ID = " + GetAD_Client_ID() +
                         " AND AD_Org_ID = " + GetAD_Org_ID();
                    if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) > 0)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "CantChange"));
                        return false;
                    }
                    else
                    {
                        sql = @"SELECT COUNT(*) FROM C_Conversion_Rate WHERE IsActive = 'Y' " +
                                " AND c_currency_id = " + GetC_Currency_ID() + " AND C_Currency_To_ID = " + GetC_Currency_To_ID() +
                                " AND ( ValidFrom BETWEEN " + GlobalVariable.TO_DATE(GetValidFrom(), true) +
                                " AND " + GlobalVariable.TO_DATE(GetValidTo(), true) + " OR  ValidTo BETWEEN " +
                                  GlobalVariable.TO_DATE(GetValidTo(), true) + "  AND " +
                                  GlobalVariable.TO_DATE(GetValidTo(), true) + " ) " +
                                " AND c_conversiontype_id = " + GetC_ConversionType_ID() + " AND AD_Client_ID = " + GetAD_Client_ID() +
                                " AND AD_Org_ID = " + GetAD_Org_ID();
                        if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) > 0)
                        {
                            log.SaveError("Error", Msg.GetMsg(GetCtx(), "RecordExistforthisdate"));
                            return false;
                        }
                        else if (IsActive() != Util.GetValueOfBool(Get_ValueOld("IsActive")))
                        {
                            return true;
                        }
                    }
                }

                sql = @"SELECT COUNT(*)  FROM C_Invoice i INNER JOIN C_Conversion_Rate cr ON (i.c_currency_id = cr.c_currency_id AND i.ad_client_id  = cr.ad_client_id)
                                  WHERE i.IsActive = 'Y' AND i.docstatus IN ('CO' , 'CL') AND i.DateAcct BETWEEN "
                              + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(Get_ValueOld("ValidFrom")), true) +
                              " AND " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(Get_ValueOld("ValidTo")), true) +
                              " AND i.c_currency_id = " + Util.GetValueOfInt(Get_ValueOld("C_Currency_ID")) +
                              " AND i.c_conversiontype_id = " + Util.GetValueOfInt(Get_ValueOld("C_ConversionType_ID")) +
                              " AND cr.C_Currency_To_ID IN (SELECT C_Currency_ID FROM C_AcctSchema WHERE IsActive = 'Y' AND AD_Client_ID = " + GetAD_Client_ID() + ")" +
                              " AND cr.C_Conversion_Rate_ID = " + GetC_Conversion_Rate_ID();
                if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "CantChange"));
                    return false;
                }

                sql = @"SELECT COUNT(*) FROM C_Order i INNER JOIN C_Conversion_Rate cr ON (i.c_currency_id = cr.c_currency_id AND i.ad_client_id  = cr.ad_client_id)
                          WHERE i.IsActive = 'Y' AND i.docstatus IN ('CO' , 'CL') AND i.DateAcct BETWEEN "
                              + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(Get_ValueOld("ValidFrom")), true) +
                               " AND " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(Get_ValueOld("ValidTo")), true) +
                               " AND i.c_currency_id = " + Util.GetValueOfInt(Get_ValueOld("C_Currency_ID")) +
                               " AND i.c_conversiontype_id = " + Util.GetValueOfInt(Get_ValueOld("C_ConversionType_ID")) +
                               " AND cr.C_Currency_To_ID IN (SELECT C_Currency_ID FROM C_AcctSchema WHERE IsActive = 'Y' AND AD_Client_ID = " + GetAD_Client_ID() + ")" +
                               " AND cr.C_Conversion_Rate_ID = " + GetC_Conversion_Rate_ID();
                if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "CantChange"));
                    return false;
                }

                sql = @"SELECT COUNT(*) FROM C_Payment i INNER JOIN C_Conversion_Rate cr ON (i.c_currency_id = cr.c_currency_id AND i.ad_client_id  = cr.ad_client_id)
                          WHERE i.IsActive = 'Y' AND i.docstatus IN ('CO' , 'CL') AND i.DateAcct BETWEEN "
                               + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(Get_ValueOld("ValidFrom")), true) +
                               " AND " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(Get_ValueOld("ValidTo")), true) +
                               " AND i.c_currency_id = " + Util.GetValueOfInt(Get_ValueOld("C_Currency_ID")) +
                               " AND i.c_conversiontype_id = " + Util.GetValueOfInt(Get_ValueOld("C_ConversionType_ID")) +
                               " AND cr.C_Currency_To_ID IN (SELECT C_Currency_ID FROM C_AcctSchema WHERE IsActive = 'Y' AND AD_Client_ID = " + GetAD_Client_ID() + ")" +
                               " AND cr.C_Conversion_Rate_ID = " + GetC_Conversion_Rate_ID();
                if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "CantChange"));
                    return false;
                }
            }

            if (newRecord)
            {
                sql = @"SELECT COUNT(*) FROM C_Conversion_Rate WHERE IsActive = 'Y' " +
                            " AND c_currency_id = " + GetC_Currency_ID() + " AND C_Currency_To_ID = " + GetC_Currency_To_ID() +
                            " AND ( " + GlobalVariable.TO_DATE(GetValidFrom(), true) + " BETWEEN ValidFrom AND ValidTo" +
                            " OR " + GlobalVariable.TO_DATE(GetValidTo(), true) + " BETWEEN ValidFrom AND ValidTo )" +
                            " AND c_conversiontype_id = " + GetC_ConversionType_ID() + " AND AD_Client_ID = " + GetAD_Client_ID() +
                            " AND AD_Org_ID = " + GetAD_Org_ID();
                if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "RecordExistforthisdate"));
                    return false;
                }
                else
                {
                    sql = @"SELECT COUNT(*) FROM C_Conversion_Rate WHERE IsActive = 'Y' " +
                                " AND c_currency_id = " + GetC_Currency_ID() + " AND C_Currency_To_ID = " + GetC_Currency_To_ID() +
                                " AND ( ValidFrom BETWEEN " + GlobalVariable.TO_DATE(GetValidFrom(), true) +
                                " AND " + GlobalVariable.TO_DATE(GetValidTo(), true) + " OR  ValidTo BETWEEN " +
                                  GlobalVariable.TO_DATE(GetValidTo(), true) + "  AND " +
                                  GlobalVariable.TO_DATE(GetValidTo(), true) + " ) " +
                                " AND c_conversiontype_id = " + GetC_ConversionType_ID() + " AND AD_Client_ID = " + GetAD_Client_ID() +
                                " AND AD_Org_ID = " + GetAD_Org_ID();
                    if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) > 0)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "RecordExistforthisdate"));
                        return false;
                    }
                }
            }
            //end

            //	From - To is the same
            if (GetC_Currency_ID() == GetC_Currency_To_ID())
            {
                log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@C_Currency_ID@ = @C_Currency_ID@"));
                return false;
            }
            //	Nothing to convert
            if (GetMultiplyRate().CompareTo(Utility.Env.ZERO) <= 0)
            {
                log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@MultiplyRate@ <= 0"));
                return false;
            }

            //	Date Range Check
            DateTime? from = GetValidFrom();
            if (GetValidTo() == null)
            {
                SetValidTo(TimeUtil.GetDay(2056, 1, 29));	//	 no exchange rates after my 100th birthday
            }
            DateTime? to = GetValidTo();

            //if (to.before(from))
            if (to < from)
            {
                //SimpleDateFormat df = DisplayType.getDateFormat(DisplayType.Date);
                log.SaveError("Error", to + " < " + from);
                return false;
            }
            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            //by Amit - 6-5-2016
            if (newRecord)
            {
                string sql = @"SELECT COUNT(*) FROM C_Conversion_Rate WHERE ISActive = 'Y' AND c_currency_id = " + GetC_Currency_To_ID() +
                              " AND C_Currency_To_ID = " + GetC_Currency_ID() + " AND c_conversiontype_id = " + GetC_ConversionType_ID() +
                              " AND ( ValidFrom BETWEEN " + GlobalVariable.TO_DATE(GetValidFrom(), true) + " AND " + GlobalVariable.TO_DATE(GetValidTo(), true) +
                              " OR ValidTo BETWEEN " + GlobalVariable.TO_DATE(GetValidFrom(), true) + " AND " + GlobalVariable.TO_DATE(GetValidTo(), true) + " ) " +
                              " AND AD_Client_ID = " + GetAD_Client_ID() + " AND AD_Org_ID =" + GetAD_Org_ID();
                if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) <= 0)
                {
                    MConversionRate conRate = new MConversionRate(GetCtx(), 0, null);
                    conRate.SetAD_Client_ID(GetAD_Client_ID());
                    conRate.UpdateFromServer = false;
                    conRate.SetAD_Org_ID(GetAD_Org_ID());
                    conRate.SetC_Currency_ID(GetC_Currency_To_ID());
                    conRate.SetC_Currency_To_ID(GetC_Currency_ID());
                    conRate.SetC_ConversionType_ID(GetC_ConversionType_ID());
                    conRate.SetDivideRate(GetMultiplyRate());
                    conRate.SetMultiplyRate(GetDivideRate());
                    conRate.SetValidFrom(GetValidFrom());
                    conRate.SetValidTo(GetValidTo());
                    if (!conRate.Save(Get_Trx()))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        log.Info("Conversion Rate not saved : " + pp.GetValue() + " AND " + pp.GetName());
                    }
                }
            }
            //end
            return true;
        }

        protected override bool BeforeDelete()
        {
            // Cannot delete record if transaction occured agianst this currency and currenct to
            string sql;
            sql = @"SELECT COUNT(*)  FROM C_Invoice i INNER JOIN C_Conversion_Rate cr ON (i.c_currency_id = cr.c_currency_id AND i.ad_client_id  = cr.ad_client_id)
                                  WHERE i.IsActive = 'Y' AND i.docstatus IN ('CO' , 'CL') AND i.DateAcct BETWEEN "
                              + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(Get_Value("ValidFrom")), true) +
                              " AND " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(Get_Value("ValidTo")), true) +
                              " AND i.c_currency_id = " + Util.GetValueOfInt(Get_Value("C_Currency_ID")) +
                              " AND i.c_conversiontype_id = " + Util.GetValueOfInt(Get_Value("C_ConversionType_ID")) +
                              " AND cr.C_Currency_To_ID IN (SELECT C_Currency_ID FROM C_AcctSchema WHERE IsActive = 'Y' AND AD_Client_ID = " + GetAD_Client_ID() + ")" +
                              " AND cr.C_Conversion_Rate_ID = " + GetC_Conversion_Rate_ID();
            if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) > 0)
            {
                log.SaveError("Error", Msg.GetMsg(GetCtx(), "CantDelete"));
                return false;
            }

            sql = @"SELECT COUNT(*) FROM C_Order i INNER JOIN C_Conversion_Rate cr ON (i.c_currency_id = cr.c_currency_id AND i.ad_client_id  = cr.ad_client_id)
                          WHERE i.IsActive = 'Y' AND i.docstatus IN ('CO' , 'CL') AND i.DateAcct BETWEEN "
                          + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(Get_Value("ValidFrom")), true) +
                           " AND " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(Get_Value("ValidTo")), true) +
                           " AND i.c_currency_id = " + Util.GetValueOfInt(Get_Value("C_Currency_ID")) +
                           " AND i.c_conversiontype_id = " + Util.GetValueOfInt(Get_Value("C_ConversionType_ID")) +
                           " AND cr.C_Currency_To_ID IN (SELECT C_Currency_ID FROM C_AcctSchema WHERE IsActive = 'Y' AND AD_Client_ID = " + GetAD_Client_ID() + ")" +
                           " AND cr.C_Conversion_Rate_ID = " + GetC_Conversion_Rate_ID();
            if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) > 0)
            {
                log.SaveError("Error", Msg.GetMsg(GetCtx(), "CantDelete"));
                return false;
            }

            sql = @"SELECT COUNT(*) FROM C_Payment i INNER JOIN C_Conversion_Rate cr ON (i.c_currency_id = cr.c_currency_id AND i.ad_client_id  = cr.ad_client_id)
                          WHERE i.IsActive = 'Y' AND i.docstatus IN ('CO' , 'CL') AND i.DateAcct BETWEEN "
                           + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(Get_Value("ValidFrom")), true) +
                           " AND " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(Get_Value("ValidTo")), true) +
                           " AND i.c_currency_id = " + Util.GetValueOfInt(Get_Value("C_Currency_ID")) +
                           " AND i.c_conversiontype_id = " + Util.GetValueOfInt(Get_Value("C_ConversionType_ID")) +
                           " AND cr.C_Currency_To_ID IN (SELECT C_Currency_ID FROM C_AcctSchema WHERE IsActive = 'Y' AND AD_Client_ID = " + GetAD_Client_ID() + ")" +
                           " AND cr.C_Conversion_Rate_ID = " + GetC_Conversion_Rate_ID();
            if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) > 0)
            {
                log.SaveError("Error", Msg.GetMsg(GetCtx(), "CantDelete"));
                return false;
            }
            return true;
        }

        public bool UpdateFromServer { get; set; }

    }
}
