/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MMatchInv
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     08-Jun-2009
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
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MMatchInv : X_M_MatchInv
    {
        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MMatchInv).FullName);

        public int C_OrderLine_ID = 0;
        public String orderDocStatus = String.Empty;
        public String inOutDocStatus = String.Empty;
        public MInOutLine iol = null;

        /**
       * 	Get InOut-Invoice Matches
       *	@param ctx context
       *	@param M_InOutLine_ID shipment
       *	@param C_InvoiceLine_ID invoice
       *	@param trxName transaction
       *	@return array of matches
       */
        public static MMatchInv[] Get(Ctx ctx, int M_InOutLine_ID, int C_InvoiceLine_ID, Trx trxName)
        {
            if (M_InOutLine_ID == 0 || C_InvoiceLine_ID == 0)
                return new MMatchInv[] { };
            //
            String sql = "SELECT * FROM M_MatchInv WHERE M_InOutLine_ID=" + M_InOutLine_ID + " AND C_InvoiceLine_ID=" + C_InvoiceLine_ID;
            List<MMatchInv> list = new List<MMatchInv>();
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MMatchInv(ctx, dr, trxName));
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            MMatchInv[] retValue = new MMatchInv[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /*	Get InOut Invoice Matches
        *	@param ctx context
        *	@param M_InOutLine_ID shipment
        *	@param trxName transaction
        *	@return array of matches
        */
        public static MMatchInv[] Get(Ctx ctx, int M_InOutLine_ID, Trx trxName)
        {
            if (M_InOutLine_ID == 0)
                return new MMatchInv[] { };
            //
            String sql = "SELECT * FROM M_MatchInv WHERE M_InOutLine_ID=" + M_InOutLine_ID;
            List<MMatchInv> list = new List<MMatchInv>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MMatchInv(ctx, dr, trxName));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }
            MMatchInv[] retValue = new MMatchInv[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
       * 	Get Inv Matches for InOut
       *	@param ctx context
       *	@param M_InOut_ID shipment
       *	@param trxName transaction
       *	@return array of matches
       */
        public static MMatchInv[] GetInOut(Ctx ctx, int M_InOut_ID, Trx trxName)
        {
            if (M_InOut_ID == 0)
                return new MMatchInv[] { };
            //
            String sql = "SELECT * FROM M_MatchInv m"
                + " INNER JOIN M_InOutLine l ON (m.M_InOutLine_ID=l.M_InOutLine_ID) "
                + "WHERE l.M_InOut_ID=" + M_InOut_ID;
            List<MMatchInv> list = new List<MMatchInv>();
            IDataReader idr = null;
            DataTable dt = null;
            try
            {

                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MMatchInv(ctx, dr, trxName));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            { dt = null; }

            MMatchInv[] retValue = new MMatchInv[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /* 	Get Inv Matches for Invoice
         *	@param ctx context
         *	@param C_Invoice_ID invoice
         *	@param trxName transaction
         *	@return array of matches
         */
        public static MMatchInv[] GetInvoice(Ctx ctx, int C_Invoice_ID, Trx trxName)
        {
            if (C_Invoice_ID == 0)
                return new MMatchInv[] { };
            //
            String sql = "SELECT * FROM M_MatchInv mi"
                + " INNER JOIN C_InvoiceLine il ON (mi.C_InvoiceLine_ID=il.C_InvoiceLine_ID) "
                + "WHERE il.C_Invoice_ID=" + C_Invoice_ID;
            List<MMatchInv> list = new List<MMatchInv>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MMatchInv(ctx, dr, trxName));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            { dt = null; }

            MMatchInv[] retValue = new MMatchInv[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param M_MatchInv_ID id
         *	@param trxName transaction
         */
        public MMatchInv(Ctx ctx, int M_MatchInv_ID, Trx trxName)
            : base(ctx, M_MatchInv_ID, trxName)
        {
            if (M_MatchInv_ID == 0)
            {
                //	setDateTrx (new DateTime(System.currentTimeMillis()));
                //	setC_InvoiceLine_ID (0);
                //	setM_InOutLine_ID (0);
                //	setM_Product_ID (0);
                SetM_AttributeSetInstance_ID(0);
                //	setQty (Env.ZERO);
                SetPosted(false);
                SetProcessed(false);
                SetProcessing(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MMatchInv(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
        * 	Invoice Line Constructor
        *	@param iLine invoice line
        *	@param dateTrx optional date
        *	@param qty matched quantity
        */
        public MMatchInv(MInvoiceLine iLine, DateTime? dateTrx, Decimal qty)
            : this(iLine.GetCtx(), 0, iLine.Get_TrxName())
        {
            SetClientOrg(iLine);
            SetC_InvoiceLine_ID(iLine.GetC_InvoiceLine_ID());
            SetM_InOutLine_ID(iLine.GetM_InOutLine_ID());
            if (dateTrx != null)
                SetDateTrx(dateTrx);
            SetM_Product_ID(iLine.GetM_Product_ID());
            SetM_AttributeSetInstance_ID(iLine.GetM_AttributeSetInstance_ID());
            SetQty(qty);
            SetProcessed(true);		//	auto
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            //	Set Trx Date
            if (GetDateTrx() == null)
                SetDateTrx(new DateTime(CommonFunctions.CurrentTimeMillis()));
            //	Set Acct Date
            if (GetDateAcct() == null)
            {
                DateTime? ts = GetNewerDateAcct();
                if (ts == null)
                    ts = GetDateTrx();
                SetDateAcct(ts);
            }

            // 
            DataSet ds = DB.ExecuteDataset(@"SELECT  M_Storage.QtyOnHand,
                            NVL(currencyConvert(C_OrderLine.PriceEntered, C_Order.C_Currency_ID, " + GetCtx().GetContextAsInt("$C_Currency_ID") +
                            @", M_InOut.DateAcct, C_Order.C_ConversionType_ID,
                            M_InOut.AD_Client_ID, M_InOut.AD_Org_ID), 0) as POPriceEntered,
                            NVL(currencyConvert(C_InvoiceLine.PriceEntered, C_Invoice.C_Currency_ID, " + GetCtx().GetContextAsInt("$C_Currency_ID") +
                            @", C_Invoice.DateAcct, C_Invoice.C_ConversionType_ID,
                            C_Invoice.AD_Client_ID, C_Invoice.AD_Org_ID), 0) as InvPriceEntered,
                            NVL(currencyConvert(C_ProvisionalInvoiceLine.PriceEntered, C_ProvisionalInvoice.C_Currency_ID, " + GetCtx().GetContextAsInt("$C_Currency_ID") +
                            @", C_ProvisionalInvoice.DateAcct, C_ProvisionalInvoice.C_ConversionType_ID,
                            C_ProvisionalInvoice.AD_Client_ID, C_ProvisionalInvoice.AD_Org_ID), 0) as ProvisionalPriceEntered,
                            M_InOut.IsSOTrx, M_InOut.IsReturnTrx,
                            C_ProvisionalInvoiceline.C_ProvisionalInvoiceline_ID,
                            M_InOutLine.M_AttributeSetInstance_ID
                            FROM M_InOutLine
                            INNER JOIN M_InOut ON M_InOut.M_InOut_ID = M_InOutLine.M_InOut_ID
                            INNER JOIN M_Storage ON(M_InOutLine.M_Locator_ID = M_Storage.M_Locator_ID
                            AND M_InOutLine.M_Product_ID = M_Storage.M_Product_ID
                            AND NVL(M_InOutLine.M_AttributeSetInstance_ID, 0) = NVL(M_Storage.M_AttributeSetInstance_ID, 0))
                            INNER JOIN C_InvoiceLine ON C_InvoiceLine.C_InvoiceLine_ID = " + GetC_InvoiceLine_ID() + @"
                            INNER JOIN C_Invoice ON C_Invoice.C_Invoice_ID = C_InvoiceLine.C_Invoice_ID
                            LEFT JOIN C_OrderLine ON M_InOutLine.C_OrderLine_ID = C_OrderLine.C_OrderLine_ID
                            LEFT JOIN C_Order ON C_Order.C_Order_ID = C_OrderLine.C_Order_ID
                            LEFT JOIN C_ProvisionalInvoiceline ON C_ProvisionalInvoiceline.C_ProvisionalInvoiceline_ID = C_InvoiceLine.C_ProvisionalInvoiceline_ID
                            LEFT JOIN C_ProvisionalInvoice ON C_ProvisionalInvoice.C_ProvisionalInvoice_ID = C_ProvisionalInvoiceline.C_ProvisionalInvoice_ID
                            WHERE M_InOutLine.M_InOutLine_ID = " + GetM_InOutLine_ID(), null, Get_TrxName());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                SetAvailableStock(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["QtyOnHand"]));
                SetPricePO(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["POPriceEntered"]));
                SetProvisionalInvPrice(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["ProvisionalPriceEntered"]));
                SetInvoicedAmt(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["InvPriceEntered"]));
                SetIsSOTrx(Util.GetValueOfString(ds.Tables[0].Rows[0]["IsSOTrx"]).Equals("Y"));
                SetIsReturnTrx(Util.GetValueOfString(ds.Tables[0].Rows[0]["IsReturnTrx"]).Equals("Y"));
                SetC_ProvisionalInvoiceLine_ID(Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_ProvisionalInvoiceline_ID"]));
                SetPriceDifferenceAPPI(Decimal.Subtract(GetInvoicedAmt(), GetProvisionalInvPrice()));
                SetPriceDifferencePIPO(Decimal.Subtract(GetProvisionalInvPrice(), GetPricePO()));
                SetPriceDifferenceAPPO(Decimal.Subtract(GetInvoicedAmt(), GetPricePO()));
                if (GetM_AttributeSetInstance_ID() == 0 && GetM_InOutLine_ID() != 0)
                {
                    SetM_AttributeSetInstance_ID(Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_AttributeSetInstance_ID"]));
                }
            }

            if (GetM_AttributeSetInstance_ID() == 0 && GetM_InOutLine_ID() != 0)
            {
                if (iol == null || iol.Get_ID() <= 0 || iol.Get_ID() != GetM_InOutLine_ID())
                {
                    iol = new MInOutLine(GetCtx(), GetM_InOutLine_ID(), Get_TrxName());
                }
                SetM_AttributeSetInstance_ID(iol.GetM_AttributeSetInstance_ID());
            }

            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (IsCostCalculated() || IsCostImmediate())
            {
                X_M_MatchInvCostTrack costTrack = null;
                DataSet M_MatchInvCostTrack_ID = DB.ExecuteDataset("SELECT * FROM M_MatchInvCostTrack WHERE M_MatchInv_ID = " + GetM_MatchInv_ID());
                if (M_MatchInvCostTrack_ID != null && M_MatchInvCostTrack_ID.Tables[0].Rows.Count > 0)
                {
                    costTrack = new X_M_MatchInvCostTrack(GetCtx(), M_MatchInvCostTrack_ID.Tables[0].Rows[0], null);
                    costTrack.SetIsCostCalculated(IsCostCalculated());
                    costTrack.SetIsCostImmediate(IsCostImmediate());
                }
                else
                {
                    costTrack = new X_M_MatchInvCostTrack(GetCtx(), 0, null);
                    costTrack.SetM_MatchInv_ID(GetM_MatchInv_ID());
                    costTrack.SetM_InOutLine_ID(GetM_InOutLine_ID());
                    costTrack.SetC_InvoiceLine_ID(GetC_InvoiceLine_ID());
                    costTrack.SetQty(GetQty());
                    costTrack.SetM_Product_ID(GetM_Product_ID());
                    costTrack.SetIsCostCalculated(IsCostCalculated());
                    costTrack.SetIsCostImmediate(IsCostImmediate());
                }
                costTrack.Save();
            }
            return true;
        }

        /**
         * 	Get the later Date Acct from invoice or shipment
         *	@return date or null
         */
        private DateTime? GetNewerDateAcct()
        {
            DateTime? invoiceDate = null;
            DateTime? shipDate = null;

            String sql = "SELECT i.DateAcct "
                + "FROM C_InvoiceLine il"
                + " INNER JOIN C_Invoice i ON (i.C_Invoice_ID=il.C_Invoice_ID) "
                + "WHERE C_InvoiceLine_ID=" + GetC_InvoiceLine_ID();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    invoiceDate = Utility.Util.GetValueOfDateTime(dr[0]);//.getTimestamp(1);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }
            sql = "SELECT io.DateAcct "
                + "FROM M_InOutLine iol"
                + " INNER JOIN M_InOut io ON (io.M_InOut_ID=iol.M_InOut_ID) "
                + "WHERE iol.M_InOutLine_ID=" + GetM_InOutLine_ID();
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    shipDate = Utility.Util.GetValueOfDateTime(dr[0]);//.getTimestamp(1);
                }

            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                dt = null;
            }

            if (invoiceDate == null)
                return (DateTime?)shipDate;
            if (shipDate == null)
                return (DateTime?)invoiceDate;
            //if (invoiceDate.after(shipDate))
            if (invoiceDate > shipDate)
            {
                return (DateTime?)invoiceDate;
            }
            return (DateTime?)shipDate;
        }

        /**
         * 	Before Delete
         *	@return true if acct was deleted
         */
        protected override bool BeforeDelete()
        {
            if (IsPosted())
            {
                if (!MPeriod.IsOpen(GetCtx(), GetDateTrx(), MDocBaseType.DOCBASETYPE_MATCHINVOICE, GetAD_Org_ID()))
                    return false;
                SetPosted(false);
                return MFactAcct.Delete(Table_ID, Get_ID(), Get_TrxName()) >= 0;
            }
            return true;
        }

        /**
         * 	After Delete
         *	@param success success
         *	@return success
         */
        protected override bool AfterDelete(bool success)
        {
            if (success)
            {
                //	Get Order and decrease invoices
                if (C_OrderLine_ID == 0)
                {
                    MInvoiceLine iLine = new MInvoiceLine(GetCtx(), GetC_InvoiceLine_ID(), Get_TrxName());
                    C_OrderLine_ID = iLine.GetC_OrderLine_ID();
                    if (C_OrderLine_ID == 0)
                    {
                        MInOutLine ioLine = new MInOutLine(GetCtx(), GetM_InOutLine_ID(), Get_TrxName());
                        C_OrderLine_ID = ioLine.GetC_OrderLine_ID();
                    }
                }
                //	No Order Found
                if (C_OrderLine_ID == 0)
                    return success;
                //	Find MatchPO
                MMatchPO[] mPO = MMatchPO.Get(GetCtx(), C_OrderLine_ID, GetC_InvoiceLine_ID(), Get_TrxName());
                for (int i = 0; i < mPO.Length; i++)
                {
                    if (mPO[i].GetM_InOutLine_ID() == 0)
                    {
                        mPO[i].orderDocStatus = orderDocStatus;
                        mPO[i].inOutDocStatus = inOutDocStatus;
                        mPO[i].Delete(true);
                    }
                    else
                    {
                        mPO[i].SetC_InvoiceLine_ID(null);
                        mPO[i].Save();
                    }
                }
            }
            return success;
        }
    }
}
