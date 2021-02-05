/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MProductPrice
 * Purpose        : 
 * Class Used     : X_VAM_ReturnRule
 * Chronological    Development
 * Raghunandan     09-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MReturnPolicy : X_VAM_ReturnRule
    {

        private static VLogger s_log = VLogger.GetVLogger(typeof(MReturnPolicy).FullName);


        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_ReturnRule_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MReturnPolicy(Ctx ctx, int VAM_ReturnRule_ID, Trx trxName)
            : base(ctx, VAM_ReturnRule_ID, trxName)
        {
        }

        /// <summary>
        /// Load Construor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MReturnPolicy(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        public bool CheckReturnPolicy(DateTime? shipDate, DateTime? returnDate, int VAM_Product_ID)
        {
            int VAM_ReturnRuleLine_ID = GetProductLine(VAM_Product_ID);
            if (VAM_ReturnRuleLine_ID == 0)
            {
                MProduct product = new MProduct(GetCtx(), VAM_Product_ID, Get_TrxName());
                GetProductCategoryLine(product.GetVAM_ProductCategory_ID());
            }
            int days = 0;//new code
            long timeFrame;
            if (VAM_ReturnRuleLine_ID == 0)
            {
                //Get dateTime(in days).
                //The datetime dictates the number of days after shipment that the
                //goods can be returned. 
                days = GetTimeFrame();//new code
                timeFrame = GetTimeFrame() * 24 * 60 * 60; // Timeframe in milliseconds
            }
            else
            {
                MReturnPolicyLine rpolicyLine = new MReturnPolicyLine(GetCtx(), VAM_ReturnRuleLine_ID, Get_TrxName());
                days = rpolicyLine.GetTimeFrame();//new code
                timeFrame = rpolicyLine.GetTimeFrame() * 24 * 60 * 60; // Timeframe in milliseconds
            }
            if (timeFrame == 0)
            {
                return false;
            }

            log.Fine("ShipDate : " + shipDate.ToString() + " ReturnDate : " + returnDate.ToString() + " TimeFrame : " + timeFrame);
            //Timestamp allowedDate = new Timestamp(timeFrame * 1000 + shipDate.getTime());
            DateTime dt = shipDate.Value;//new code to check allow date for customer return
            DateTime allowedDate;
            if (days > 0)
            {
                allowedDate = dt.AddDays(days);
            }
            else
            {
                allowedDate = dt;
            }

            log.Fine("Allowed Date : " + allowedDate.ToString());
            if (returnDate > allowedDate)
            {
                return false;
            }
            return true;
        }


        public bool CheckReturnPolicy(DateTime? shipDate, DateTime? returnDate)
        {
            if (PolicyHasLines())
            {
                return true;
            }

            long timeFrame = GetTimeFrame() * 24 * 60 * 60; // Timeframe in milliseconds
            // If timeFrame is 0, returns are not allowed
            if (timeFrame == 0)
            {
                return false;
            }

            log.Fine("ShipDate : " + shipDate.ToString() + " ReturnDate : " + returnDate.ToString() + " TimeFrame : " + timeFrame);
            //Timestamp allowedDate = new Timestamp(timeFrame * 1000 + shipDate.getTime());
            //DateTime allowedDate = CommonFunctions.CovertMilliToDate(timeFrame * 1000 + shipDate.Value.Millisecond);
            DateTime h = shipDate.Value;
            DateTime allowedDate = h.AddMilliseconds(System.Convert.ToDouble(timeFrame * 1000));
            
            
            log.Fine("Allowed Date : " + allowedDate.ToString());
            if (returnDate > allowedDate)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get Lines of Return Policy for a product
        /// </summary>
        /// <param name="VAM_Product_ID">where clause or null (starting with AND)</param>
        /// <returns>lines</returns>
        int GetProductLine(int VAM_Product_ID)
        {
            StringBuilder sql = new StringBuilder("SELECT VAM_ReturnRuleLine_ID FROM VAM_ReturnRuleLine WHERE VAM_ReturnRule_ID =" + GetVAM_ReturnRule_ID());
            int VAM_ReturnRuleLine_ID = 0;

            if (VAM_Product_ID != 0)
                sql.Append("AND VAM_Product_ID =" + VAM_Product_ID);

            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql.ToString(), null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    VAM_ReturnRuleLine_ID = Utility.Util.GetValueOfInt(dr[0].ToString());
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            finally
            {
                ds = null;
            }
            return VAM_ReturnRuleLine_ID;
        }

        /// <summary>
        /// Get Lines of Return Policy for a product
        /// </summary>
        /// <param name="VAM_ProductCategory_ID"></param>
        /// <returns>lines</returns>
        int GetProductCategoryLine(int VAM_ProductCategory_ID)
        {
            StringBuilder sql = new StringBuilder("SELECT VAM_ReturnRuleLine_ID FROM VAM_ReturnRuleLine WHERE VAM_ReturnRule_ID =" + GetVAM_ReturnRule_ID());
            int VAM_ReturnRuleLine_ID = 0;

            if (VAM_ProductCategory_ID != 0)
                sql.Append("AND VAM_Product_ID IS NULL AND VAM_ProductCategory_ID = " + VAM_ProductCategory_ID);

            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql.ToString(), null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    VAM_ReturnRuleLine_ID = Utility.Util.GetValueOfInt(dr[0].ToString());
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            return VAM_ReturnRuleLine_ID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool PolicyHasLines()
        {
            StringBuilder sql = new StringBuilder("SELECT count(*) FROM VAM_ReturnRuleLine WHERE VAM_ReturnRule_ID =" + GetVAM_ReturnRule_ID() + "");
            int lineCount = 0;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                if (idr.Read())
                {
                    lineCount = Utility.Util.GetValueOfInt(idr[0].ToString());
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql.ToString(), e);
            }

            //
            return lineCount != 0;
        }

        public static int GetDefault(Ctx ctx)
        {
            int VAF_Client_ID = ctx.GetVAF_Client_ID();
            int rPolicy_ID = 0;

            String sql = "SELECT VAM_ReturnRule_ID FROM VAM_ReturnRule"
                + " WHERE IsDefault='Y' AND IsActive='Y' AND VAF_Client_ID=" + VAF_Client_ID;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                if (idr.Read())
                {
                    rPolicy_ID = Utility.Util.GetValueOfInt(idr[0].ToString());
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                s_log.Severe(e.ToString());
            }
            return rPolicy_ID;
        }
    }
}