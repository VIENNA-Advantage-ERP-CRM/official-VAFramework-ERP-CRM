/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRfQ
 * Purpose        : RfQ Model
 * Class Used     : X_C_RfQ
 * Chronological    Development
 * Raghunandan     10-Aug.-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;


namespace VAdvantage.Model
{
    public class MRfQ : X_C_RfQ
    {

        //Cache	
        private static CCache<int, MRfQ> s_cache = new CCache<int, MRfQ>("C_RfQ", 10);

        /// <summary>
        /// Get MRfQ from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_RfQ_ID">ID</param>
        /// <param name="trxName">transction</param>
        /// <returns>MRFQ</returns>
        public static MRfQ Get(Ctx ctx, int C_RfQ_ID, Trx trxName)
        {
            int key = C_RfQ_ID;
            MRfQ retValue = (MRfQ)s_cache[key];
            if (retValue != null)
            {
                return retValue;
            }
            retValue = new MRfQ(ctx, C_RfQ_ID, trxName);
            if (retValue.Get_ID() != 0)
            {
                s_cache.Add(key, retValue);
            }
            return retValue;
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_RfQ_ID">ID</param>
        /// <param name="trxName">transaction</param>
        public MRfQ(Ctx ctx, int C_RfQ_ID, Trx trxName)
            :base(ctx, C_RfQ_ID, trxName)
        {
            if (C_RfQ_ID == 0)
            {
                //	setC_RfQ_Topic_ID (0);
                //	setName (null);
                //	setC_Currency_ID (0);	// @$C_Currency_ID @
                //	setSalesRep_ID (0);
                //
                SetDateResponse(DateTime.Now);// Commented by Bharat on 15 Jan 2019 for point given by Puneet
                SetDateWorkStart(DateTime.Now);// Convert.ToDateTime(System.currentTimeMillis()));
                SetIsInvitedVendorsOnly(false);
                SetQuoteType(QUOTETYPE_QuoteSelectedLines);
                SetIsQuoteAllQty(false);
                SetIsQuoteTotalAmt(false);
                SetIsRfQResponseAccepted(true);
                SetIsSelfService(true);
                SetProcessed(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="dr">dataroe</param>
        /// <param name="trxName">transaction</param>
        public MRfQ(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Get active Lines
        /// </summary>
        /// <returns>array of lines</returns>
        public MRfQLine[] GetLines()
        {
            List<MRfQLine> list = new List<MRfQLine>();
            String sql = "SELECT * FROM C_RfQLine "
                + "WHERE C_RfQ_ID=@param1 AND IsActive='Y' "
                + "ORDER BY Line";
            DataTable dt = null;
            IDataReader idr = null;
            SqlParameter[] param = null;
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@param1", GetC_RfQ_ID());
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)// while (dr.next())
                {
                    list.Add(new MRfQLine(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                log.Log(VAdvantage.Logging.Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
                idr.Close();
            }

            MRfQLine[] retValue = new MRfQLine[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get RfQ Responses
        /// </summary>
        /// <param name="activeOnly">active responses only</param>
        /// <param name="completedOnly">complete responses only</param>
        /// <returns>array of lines</returns>
        public MRfQResponse[] GetResponses(bool activeOnly, bool completedOnly)
        {
            List<MRfQResponse> list = new List<MRfQResponse>();
            String sql = "SELECT * FROM C_RfQResponse "
                + "WHERE C_RfQ_ID=@param1";
            if (activeOnly)
            {
                sql += " AND IsActive='Y'";
            }
            if (completedOnly)
            {
                sql += " AND IsComplete='Y'";
            }
            sql += " ORDER BY Price";
            DataTable dt = null;
            IDataReader idr = null;
            SqlParameter[] param = null;
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@param1", GetC_RfQ_ID());
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)// while (dr.next())
                {
                    list.Add(new MRfQResponse(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch 
            {
                //log.log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
                idr.Close();
            }

            MRfQResponse[] retValue = new MRfQResponse[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MRfQ[");
            sb.Append(Get_ID()).Append(",Name=").Append(GetName())
                .Append(",QuoteType=").Append(GetQuoteType())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Is Quote Total Amt Only
        /// </summary>
        /// <returns>true if total amout only</returns>
        public bool IsQuoteTotalAmtOnly()
        {
            return QUOTETYPE_QuoteTotalOnly.Equals(GetQuoteType());
        }

        /// <summary>
        /// Is Quote Selected Lines
        /// </summary>
        /// <returns>true if quote selected lines</returns>
        public bool IsQuoteSelectedLines()
        {
            return QUOTETYPE_QuoteSelectedLines.Equals(GetQuoteType());
        }

        /// <summary>
        /// Is Quote All Lines
        /// </summary>
        /// <returns>true if quote selected lines</returns>
        public bool IsQuoteAllLines()
        {
            return QUOTETYPE_QuoteAllLines.Equals(GetQuoteType());
        }

        /// <summary>
        /// Is "Quote Total Amt Only" Valid
        /// </summary>
        /// <returns>null or error message</returns>
        public String CheckQuoteTotalAmtOnly()
        {
            if (!IsQuoteTotalAmtOnly())
            {
                return null;
            }
            //	Need to check Line Qty
            MRfQLine[] lines = GetLines();
            for (int i = 0; i < lines.Length; i++)
            {
                MRfQLine line = lines[i];
                MRfQLineQty[] qtys = line.GetQtys();
                if (qtys.Length > 1)
                {
                    log.Warning("isQuoteTotalAmtOnlyValid - #" + qtys.Length + " - " + line);
                   
                    String msg = "@Line@ " + line.GetLine()
                        + ": #@C_RfQLineQty@=" + qtys.Length + " - @IsQuoteTotalAmt@";
                    return msg;
                }
            }
            return null;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	Calculate Complete Date (also used to verify)
            if (GetDateWorkStart() != null && GetDeliveryDays() != 0)
            {
                SetDateWorkComplete(TimeUtil.AddDays(GetDateWorkStart(), GetDeliveryDays()));
            }
            //	Calculate Delivery Days
            else if (GetDateWorkStart() != null && GetDeliveryDays() == 0 && GetDateWorkComplete() != null)
            {
                SetDeliveryDays(TimeUtil.GetDaysBetween(GetDateWorkStart(), GetDateWorkComplete()));
            }
            //	Calculate Start Date
            else if (GetDateWorkStart() == null && GetDeliveryDays() != 0 && GetDateWorkComplete() != null)
            {
                SetDateWorkStart(TimeUtil.AddDays(GetDateWorkComplete(), GetDeliveryDays() * -1));
            }
            return true;
        }

        //Added by Vivek 23-12-2015
        public new void SetProcessed(bool processed)
        {
            base.SetProcessed(processed);
            //if (Get_ID() == 0)
            //    return;
            String setline = "SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE C_Rfq_ID =" + GetC_RfQ_ID();
            int noLine = DataBase.DB.ExecuteQuery("UPDATE C_RfQLine " + setline, null, Get_Trx());

            string setQty = @"UPDATE C_RFQLINEQTY SET PROCESSED ='Y' WHERE C_RFQLINEQTY_ID IN (SELECT LQTY.C_RFQLINEQTY_ID FROM C_RFQLINEQTY LQTY INNER JOIN C_RFQLINE LINE
                    ON LINE.C_RFQLINE_ID=LQTY.C_RFQLINE_ID INNER JOIN C_RFQ RFQ ON RFQ.C_RFQ_ID = LINE.C_RFQ_ID WHERE RFQ.C_RFQ_ID =" + GetC_RfQ_ID() + ")";
            int noQty = DataBase.DB.ExecuteQuery(setQty, null, Get_Trx());

            log.Fine(processed + " - Lines=" + noLine + ", Qty=" + noQty);
        }
    }
}
